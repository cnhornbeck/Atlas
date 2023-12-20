using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the time taken for each frame (delta time)
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        // Set the rectangle for displaying FPS
        Rect rect = new(0, 0, w, h * 2 / 100);

        // Configure the style
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        // Calculate FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // Format the string to display
        string text = string.Format("{0:0.0} ms\n{1:0.} fps", msec, fps);

        // Draw the FPS
        GUI.Label(rect, text, style);
    }
}
