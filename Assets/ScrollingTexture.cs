using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;

    private Material material;
    private Vector2 offset;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        offset = material.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;
        material.SetTextureOffset("_BaseMap", offset);
    }

}
