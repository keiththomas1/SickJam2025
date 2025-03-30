using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class LowResBlitPass : ScriptableRendererFeature
{
    class LowResRenderPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier source;
        private RTHandle tempRenderTarget;
        private Material blitMaterial;

        public LowResRenderPass(Material material)
        {
            blitMaterial = material;
        }

        public void SetSource(RenderTargetIdentifier src)
        {
            source = src;
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("LowResBlitPass");

            // Blit the low-res render texture to the screen
            cmd.Blit(source, BuiltinRenderTextureType.CameraTarget, blitMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    LowResRenderPass renderPass;
    public Material blitMaterial;

    public override void Create()
    {
        renderPass = new LowResRenderPass(blitMaterial);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderPass.SetSource(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(renderPass);
    }
}