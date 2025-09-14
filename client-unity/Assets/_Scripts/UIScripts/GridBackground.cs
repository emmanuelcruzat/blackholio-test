using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridBackground : MonoBehaviour
{
    public float gridScale = 10f;
    private Material mat;
    private Camera cam;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        cam = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;

        // Scroll grid with camera
        mat.mainTextureOffset = new Vector2(camPos.x, camPos.y) / gridScale;

        // Adjust grid scale if camera zooms
        float zoom = cam.orthographicSize;
        mat.mainTextureScale = Vector2.one * (1f / zoom);
    }
}