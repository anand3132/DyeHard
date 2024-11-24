using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderMetaballs : ScriptableRendererFeature
{
    class RenderMetaballsPass : ScriptableRenderPass
    {
        private const string MetaballRTSmallTextureId = "_MetaballRTSmall";
        private const string MetaballRTLargeTextureId = "_MetaballRTLarge";
        private const string MetaballRTLarge2TextureId = "_MetaballRTLarge2";

        private int _downsamplingAmount = 4;

        public Material BlitMaterial;
        public Material BlurMaterial;
        public Material BlitCopyDepthMaterial;

        private RenderTargetIdentifier _cameraTargetId;
        private RenderTargetIdentifier _cameraDepthTargetId;

        private RenderQueueType renderQueueType;
        private FilteringSettings m_FilteringSettings;
        private RenderObjects.CustomCameraSettings m_CameraSettings;
        private ProfilingSampler m_ProfilingSampler;

        public Material overrideMaterial { get; set; }
        public int overrideMaterialPassIndex { get; set; }

        private readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

        private RenderStateBlock m_RenderStateBlock;

        public RenderMetaballsPass(string profilerTag, RenderPassEvent renderPassEvent, string[] shaderTags,
            RenderQueueType renderQueueType, int layerMask, RenderObjects.CustomCameraSettings cameraSettings, int downsamplingAmount)
        {
            // Assign a name for the profiling sampler, this might need to match the constructor method available in your version.
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            this.renderPassEvent = renderPassEvent;
            this.renderQueueType = renderQueueType;
            this.overrideMaterial = null;
            this.overrideMaterialPassIndex = 0;
            RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            if (shaderTags != null && shaderTags.Length > 0)
            {
                foreach (var passName in shaderTags)
                    m_ShaderTagIdList.Add(new ShaderTagId(passName));
            }
            else
            {
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_CameraSettings = cameraSettings;

            BlitCopyDepthMaterial = new Material(Shader.Find("Hidden/BlitToDepth"));
            BlurMaterial = new Material(Shader.Find("Hidden/KawaseBlur"));
            _downsamplingAmount = downsamplingAmount;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("RenderMetaballsPass");

            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Add your rendering logic here...
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material blitMaterial;
    RenderMetaballsPass _scriptableMetaballsPass;
    public RenderObjects.RenderObjectsSettings renderObjectsSettings = new RenderObjects.RenderObjectsSettings();
    [Range(1, 16)] public int downsamplingAmount;

    public override void Create()
    {
        RenderObjects.FilterSettings filter = renderObjectsSettings.filterSettings;
        _scriptableMetaballsPass = new RenderMetaballsPass("RenderMetaballsPass", renderObjectsSettings.Event,
            filter.PassNames, filter.RenderQueueType, filter.LayerMask, renderObjectsSettings.cameraSettings, downsamplingAmount)
        {
            BlitMaterial = blitMaterial,
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_scriptableMetaballsPass);
    }
}
