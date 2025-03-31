using UnityEngine;

public class RenderTextureController : MonoBehaviour
{
    [SerializeField]
    private RenderTexture RenderTexture;

    private int _previousHeight;
    private int _previousWidth;

    private void Start()
    {
        this.AdjustRenderTextureSize();
    }

    private void Update()
    {
        if (Screen.width != this._previousWidth || Screen.height != this._previousHeight)
        {
            this.AdjustRenderTextureSize();
        }
    }

    private void AdjustRenderTextureSize()
    {
        this._previousHeight = Screen.height;
        this._previousWidth = Screen.width;
        // this.RenderTexture.width = this._previousWidth / 2;
        // this.RenderTexture.height = this._previousHeight / 2;
    }
}
