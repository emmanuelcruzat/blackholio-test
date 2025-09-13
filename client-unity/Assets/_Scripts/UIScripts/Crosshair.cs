using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public RectTransform plusCursor; // assign your "+" UI Image here

    void Start()
    {
        // Hide the OS cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Move the "+" to follow the mouse
        plusCursor.position = Input.mousePosition;
    }
}