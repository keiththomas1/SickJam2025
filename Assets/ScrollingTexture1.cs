using UnityEngine;
using UnityEngine.UI;

public class ScrollingTexture1 : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;

    private Material material;
    private Vector2 offset;

    void Start()
    {
        if (this.GetComponent<Renderer>())
        {
            material = GetComponent<Renderer>().material;
        } else if (this.GetComponent<Image>())
        {
            material = GetComponent<Image>().material;
        }
        offset = material.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
    }

}
