using UnityEngine;

public class WaterTextureScroll : MonoBehaviour
{
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;

        rend.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}