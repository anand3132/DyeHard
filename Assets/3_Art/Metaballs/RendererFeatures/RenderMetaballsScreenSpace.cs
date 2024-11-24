using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderMetaballsScreenSpace : ScriptableRendererFeature
{
    class RenderMetaballsDepthPass : ScriptableRenderPass
    {
        const string MetaballDepthRTId = "_MetaballDepthRT";
        int _metaballDepthRTId;
        public Material WriteDepthMaterial;

        RTHandle _metaballDepthRT;
        RenderStateBlock _renderStateBlock;
        RenderQueueType _renderQueueType;
        FilteringSettings _filteringSettings;
        ProfilingSampler _profilingSampler;
        List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

        public RenderMetaballsDepthPass(string profilerTag, RenderPassEvent renderPassEvent,
            string[] shaderTags, RenderQueueType renderQueueType, int layerMask)
        {
            profilingSampler = new ProfilingSampler(nameof(RenderObjectsPass));
            _profilingSampler = new ProfilingSampler(profilerTag);
            this.renderPassEvent = renderPassEvent;
            this._renderQueueType = renderQueueType;

            RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;

            _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            if (shaderTags != null && shaderTags.Length > 0)
            {
                foreach (var passName in shaderTags)
                    _shaderTagIdList.Add(new ShaderTagId(passName));
            }
            else
            {
                _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                _shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
                _shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            _metaballDepthRT = RTHandles.Alloc(blitTargetDescriptor, name: MetaballDepthRTId);
            ConfigureTarget(_metaballDepthRT);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = (_renderQueueType == RenderQueueType.Transparent)
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawingSettings =
                CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                drawingSettings.overrideMaterial = WriteDepthMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings,
                    ref _renderStateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (_metaballDepthRT != null)
            {
                RTHandles.Release(_metaballDepthRT);
                _metaballDepthRT = null;
            }
        }
    }

    class RenderMetaballsScreenSpacePass : ScriptableRenderPass
    {
        const string MetaballRTId = "_MetaballRT";
        const string MetaballRT2Id = "_MetaballRT2";
        const string MetaballDepthRTId = "_MetaballDepthRT";

        public Material BlitMaterial;
        Material _blurMaterial;
        Material _blitCopyDepthMaterial;

        public int BlurPasses;
        public float BlurDistance;

        RTHandle _metaballRT;
        RTHandle _metaballRT2;
        RTHandle _metaballDepthRT;
        RTHandle _cameraTarget;
        RTHandle _cameraDepthTarget;

        RenderQueueType _renderQueueType;
        FilteringSettings _filteringSettings;
        ProfilingSampler _profilingSampler;
        List<ShaderTagId> ShaderTagIdList = new List<ShaderTagId>();

        RenderStateBlock _renderStateBlock;

        public RenderMetaballsScreenSpacePass(string profilerTag, RenderPassEvent renderPassEvent,
            string[] shaderTags, RenderQueueType renderQueueType, int layerMask)
        {
            profilingSampler = new ProfilingSampler(nameof(RenderObjectsPass));
            _profilingSampler = new ProfilingSampler(profilerTag);
            this.renderPassEvent = renderPassEvent;
            this._renderQueueType = renderQueueType;

            RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;

            _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            if (shaderTags != null && shaderTags.Length > 0)
            {
                foreach (var passName in shaderTags)
                    ShaderTagIdList.Add(new ShaderTagId(passName));
            }
            else
            {
                ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
                ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            _blitCopyDepthMaterial = new Material(Shader.Find("Hidden/BlitToDepth"));
            _blurMaterial = new Material(Shader.Find("Hidden/KawaseBlur"));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            var renderer = renderingData.cameraData.renderer;

            _metaballRT = RTHandles.Alloc(blitTargetDescriptor, FilterMode.Bilinear, name: MetaballRTId);
            _metaballRT2 = RTHandles.Alloc(blitTargetDescriptor, FilterMode.Bilinear, name: MetaballRT2Id);
            _metaballDepthRT = RTHandles.Alloc(blitTargetDescriptor, name: MetaballDepthRTId);

            ConfigureTarget(_metaballRT);

            _cameraTarget = RTHandles.Alloc(renderer.cameraColorTargetHandle);
            _cameraDepthTarget = RTHandles.Alloc(renderer.cameraDepthTargetHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = (_renderQueueType == RenderQueueType.Transparent)
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawingSettings =
                CreateDrawingSettings(ShaderTagIdList, ref renderingData, sortingCriteria);

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.ClearRenderTarget(true, true, Color.clear);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                Blit(cmd, _cameraDepthTarget, _metaballRT, _blitCopyDepthMaterial);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings,
                    ref _renderStateBlock);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.SetGlobalTexture("_BlurDepthTex", _metaballDepthRT);
                cmd.SetGlobalFloat("_BlurDistance", BlurDistance);
                float offset = 1.5f;
                cmd.SetGlobalFloat("_Offset", offset);
                Blit(cmd, _metaballRT, _metaballRT2, _blurMaterial);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                for (int i = 1; i < BlurPasses; ++i)
                {
                    offset += 1.0f;
                    cmd.SetGlobalFloat("_Offset", offset);
                    Blit(cmd, _metaballRT, _metaballRT2, _blurMaterial);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }

                Blit(cmd, _metaballRT, _cameraTarget, BlitMaterial);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _metaballRT?.Release();
            _metaballRT2?.Release();
            _metaballDepthRT?.Release();
        }
    }

    public override void Create()
    {
        // Initializing your passes here as before
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Enqueue passes as before
    }
}
