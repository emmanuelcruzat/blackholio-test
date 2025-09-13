using UnityEngine;
using UnityEngine.UI;

public class UILineToCursor : MonoBehaviour
{
    RectTransform lineRect;
    Image lineImage;

    [Header("Visibility Settings")]
    public float minDistance = 50f;    // invisible until cursor is this far away
    public float maxDistance = 500f;   // fully visible when this far away
    public float minAlpha = 0.1f;      // alpha at minDistance
    public float maxAlpha = 1f;        // alpha at maxDistance

    void Awake()
    {
        lineRect = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
    }

    void Update()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePos = Input.mousePosition;

        Vector2 dir = mousePos - screenCenter;
        float distance = dir.magnitude;

        // Position halfway
        lineRect.position = screenCenter + dir / 2f;

        // Rotate toward cursor
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // Stretch length
        lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);

        // Check visibility
        if (distance < minDistance)
        {
            lineImage.enabled = false; // completely invisible
            return;
        }
        else
        {
            lineImage.enabled = true;
        }

        // Fade alpha between minDistance and maxDistance
        float t = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = lineImage.color;
        c.a = alpha;
        lineImage.color = c;
    }
}