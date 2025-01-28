using UnityEngine;
using UISystem;

public class CheckFPS : SingletonMonoBehaviour<CheckFPS>
{
    private int font_Size;
    private Color font_Color;

    private int screenWidth = 0;
    private int screenHeight = 0;
    private float deltaTime = 0.0f;

    protected override void OnAwakeSingleton()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = screenHeight * 2 / font_Size;
        style.normal.textColor = font_Color;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        Rect rect = new Rect(0, 0, screenWidth, screenHeight * 0.02f);
        GUI.Label(rect, text, style);
    }

    public void EnableFPS(int fps, Color color, int fontSize = 50)
    {
        font_Color = color;
        font_Size = fontSize > 0 ? fontSize : 50;

        Application.targetFrameRate = fps;
    }

    public void DisableFPS()
    {
        Destroy(this.gameObject);
    }
}
