using UnityEngine;

public class Upscaler : MonoBehaviour
{
    [SerializeField]
    private int RenderWidth = 300;
    [SerializeField]
    private int RenderHeight = 400;
    [SerializeField]
    private FilterMode FilterMode = FilterMode.Point; // for sharp pixels

    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private BlitCamera BlitCamera;

    private RenderTexture _lowResTexture;

    private void OnEnable()
    {
        CreateLowResTexture();
    }

    private void OnDisable()
    {
        if (this._lowResTexture) this._lowResTexture.Release();
    }

    private void CreateLowResTexture()
    {
        if (this._lowResTexture) this._lowResTexture.Release();

        this._lowResTexture = new RenderTexture(this.RenderWidth, this.RenderHeight, 24);
        this._lowResTexture.filterMode = this.FilterMode;
        this._lowResTexture.useMipMap = false;
        this._lowResTexture.autoGenerateMips = false;

        this.Camera.targetTexture = this._lowResTexture;
        this.BlitCamera.RenderTexture = this._lowResTexture;
    } 

    private void Update()
    {
        // Recreate the texture if you change resolution during play
        if (this._lowResTexture.width != this.RenderWidth || this._lowResTexture.height != this.RenderHeight)
        {
            CreateLowResTexture();
        }
    }
}
