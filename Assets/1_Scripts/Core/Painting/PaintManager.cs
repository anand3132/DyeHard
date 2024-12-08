using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace RedGaint
{
    public class PaintManager : Singleton<PaintManager>
    {

        public Shader texturePaint;
        public Shader extendIslands;

        int prepareUVID = Shader.PropertyToID("_PrepareUV");
        int positionID = Shader.PropertyToID("_PainterPosition");
        int hardnessID = Shader.PropertyToID("_Hardness");
        int strengthID = Shader.PropertyToID("_Strength");
        int radiusID = Shader.PropertyToID("_Radius");
        int blendOpID = Shader.PropertyToID("_BlendOp");
        int colorID = Shader.PropertyToID("_PainterColor");
        int textureID = Shader.PropertyToID("_MainTex");
        int uvOffsetID = Shader.PropertyToID("_OffsetUV");
        int uvIslandsID = Shader.PropertyToID("_UVIslands");

        Material paintMaterial;
        Material extendMaterial;

        CommandBuffer command;

        public override void Awake()
        {
            base.Awake();

            paintMaterial = new Material(texturePaint);
            extendMaterial = new Material(extendIslands);
            command = new CommandBuffer();
            command.name = "CommmandBuffer - " + gameObject.name;
        }

        public void initTextures(Paintable paintable)
        {
            RenderTexture mask = paintable.getMask();
            RenderTexture uvIslands = paintable.getUVIslands();
            RenderTexture extend = paintable.getExtend();
            RenderTexture support = paintable.getSupport();
            Renderer rend = paintable.getRenderer();

            command.SetRenderTarget(mask);
            command.SetRenderTarget(extend);
            command.SetRenderTarget(support);

            paintMaterial.SetFloat(prepareUVID, 1);
            command.SetRenderTarget(uvIslands);
            command.DrawRenderer(rend, paintMaterial, 0);

            Graphics.ExecuteCommandBuffer(command);
            command.Clear();
        }


        public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f,
            float strength = .5f,
            Color? color = null)
        {
            RenderTexture mask = paintable.getMask();
            RenderTexture uvIslands = paintable.getUVIslands();
            RenderTexture extend = paintable.getExtend();
            RenderTexture support = paintable.getSupport();
            Renderer rend = paintable.getRenderer();

            paintMaterial.SetFloat(prepareUVID, 0);
            paintMaterial.SetVector(positionID, pos);
            paintMaterial.SetFloat(hardnessID, hardness);
            paintMaterial.SetFloat(strengthID, strength);
            paintMaterial.SetFloat(radiusID, radius);
            paintMaterial.SetTexture(textureID, support);
            paintMaterial.SetColor(colorID, color ?? Color.red);
            extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
            extendMaterial.SetTexture(uvIslandsID, uvIslands);

            command.SetRenderTarget(mask);
            command.DrawRenderer(rend, paintMaterial, 0);

            command.SetRenderTarget(support);
            command.Blit(mask, support);

            command.SetRenderTarget(extend);
            command.Blit(mask, extend, extendMaterial);

            Graphics.ExecuteCommandBuffer(command);
            command.Clear();
        }

        public static class PaintAnalyzer
        {
            public static int CalculateLeadingColor(RenderTexture renderTexture,
                LinkedList<ColorFillAmount> colorFillAmounts,
                float tolerance = .1f, int sampleCount = 1, int chunkSize = 200)
            {
                // Capture the render texture into a texture
                Texture2D texture =
                    new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();
                RenderTexture.active = null;

                Color[] pixels = texture.GetPixels();
                Object.Destroy(texture);

                // Random sampling or block-based checking
                System.Random rand = new System.Random();
                foreach (var colorFillAmount in colorFillAmounts)
                {
                    // Reset the fill amount for the color at the start
                    colorFillAmount.FillAmount = 0;
                }

                for (int i = 0; i < sampleCount; i++)
                {
                    int randomIndex = rand.Next(0, pixels.Length);
                    Color pixel = pixels[randomIndex];

                    // Check if the pixel's color is within tolerance of any player's color
                    foreach (var colorFillAmount in colorFillAmounts)
                    {
                        if (IsColorWithinTolerance(pixel, colorFillAmount.PlayerColor, tolerance))
                        {
                            // Update the fill amount for the color
                            colorFillAmount.FillAmount++;
                        }
                    }
                }

                // Find the color with the most painted area
                Color leadingColor = Color.clear;
                int maxArea = 0;
                foreach (var colorFillAmount in colorFillAmounts)
                {
                    if (colorFillAmount.FillAmount > maxArea)
                    {
                        maxArea = colorFillAmount.FillAmount;
                        leadingColor = colorFillAmount.PlayerColor;
                    }
                }

                Debug.Log($"Leading color: {leadingColor} with area: {maxArea}");
                return maxArea;
            }

            private static bool IsColorWithinTolerance(Color color1, Color color2, float tolerance)
            {
                float diffR = Mathf.Abs(color1.r - color2.r);
                float diffG = Mathf.Abs(color1.g - color2.g);
                float diffB = Mathf.Abs(color1.b - color2.b);
                float diffA = Mathf.Abs(color1.a - color2.a);

                return diffR <= tolerance && diffG <= tolerance && diffB <= tolerance && diffA <= tolerance;
            }
        } //PaintAnalyzer
    } //PaintManager
} //RedGaint