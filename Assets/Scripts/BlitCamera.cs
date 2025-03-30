using UnityEngine;

public class BlitCamera : MonoBehaviour
{
    private RenderTexture _renderTexture;
    public RenderTexture RenderTexture
    {
        get { return _renderTexture; }
        set
        {
            this._renderTexture = value;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (this.RenderTexture != null)
        {
            Graphics.Blit(this.RenderTexture, dest);
        }
        // This upscales the low-res render texture to the screen
    }
}
