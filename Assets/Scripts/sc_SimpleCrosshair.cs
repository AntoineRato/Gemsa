using UnityEngine;
using UnityEngine.UI;

public enum CrosshairColorChannel
{
    RED,
    GREEN,
    BLUE,
    ALPHA
}

[System.Serializable]
public class Crosshair
{
    [Range(1, 150), Tooltip("Controls the length of each crosshair line.")]
    public int size = 10;

    [Range(1, 100), Tooltip("Controls the width of each crosshair line.")]
    public int thickness = 2;

    [Range(1, 100), Tooltip("If enabled, controls the thickness of the dot.")]
    public int dotThickness = 2;

    [Range(1, 100), Tooltip("If enabled, controls the thickness of the border.")]
    public int borderThickness = 10;

    [Range(0, 350), Tooltip("Controls the distance between the center of the crosshair and the start of each crosshair line.")]
    public int gap = 5;

    public bool dot = false;
    public bool border = true;

    [Tooltip("Specifies the color of the crosshair.")]
    public Color color = Color.green;

    [Tooltip("Specifies the color of the crosshair.")]
    public Color borderColor = Color.black;

    public int SizeNeeded
    {
        private set { }
        get
        {
            int width;
            if (border)
                width = (size * 2) + (gap * 2) + borderThickness;
            else
                width = (size * 2) + (gap * 2);

            return width > thickness ? width : thickness;
        }
    }
}

public class sc_SimpleCrosshair : MonoBehaviour
{
    [SerializeField, Tooltip("Contains properties that Specify how the crosshair looks.")]
    private Crosshair m_crosshair = null;

    [Tooltip("Specifies the image to draw the crosshair to. If you leave this empty, this script generates a Canvas and an Image with the correct settings for you.")]
    public Image m_crosshairImage;

    //private void Awake()
    //{
    //    if(m_crosshairImage == null)
    //    {
    //        InitialiseCrosshairImage();
    //    }

    //    //GenerateCrosshair();
    //}

    public void InitialiseCrosshairImage()
    {
        GameObject crosshairGameObject = new GameObject();
        crosshairGameObject.name = "Crosshair Canvas";

        Canvas crosshairCanvas = crosshairGameObject.AddComponent<Canvas>();
        crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        crosshairGameObject.AddComponent<CanvasScaler>();

        GameObject imageGameObject = new GameObject();
        imageGameObject.name = "Crosshair Image";
        imageGameObject.transform.parent = crosshairGameObject.transform;

        m_crosshairImage = imageGameObject.AddComponent<Image>();
        m_crosshairImage.rectTransform.localPosition = new Vector2(0, 0);
        m_crosshairImage.raycastTarget = false;
    }

    public void GenerateCrosshair()
    {
        if (m_crosshairImage == null)
            InitialiseCrosshairImage();

        Texture2D crosshairTexture =  DrawCrosshair(m_crosshair);

        m_crosshairImage.rectTransform.sizeDelta = new Vector2(m_crosshair.SizeNeeded, m_crosshair.SizeNeeded);
        Sprite crosshairSprite = Sprite.Create(crosshairTexture,
            new Rect(0, 0, crosshairTexture.width, crosshairTexture.height),
            Vector2.one / 2);

        m_crosshairImage.sprite = crosshairSprite;
    }

    public Crosshair GetCrosshair() { return m_crosshair; }

    #region Draw Crosshair Texture
    public Texture2D DrawCrosshair(Crosshair crosshair = null)
    {
        if(crosshair == null) { crosshair = m_crosshair; }

        int sizeNeeded = crosshair.SizeNeeded;
        int centerBias = sizeNeeded / 2;


        Texture2D crosshairTexture = new Texture2D(sizeNeeded, sizeNeeded, TextureFormat.RGBA32, false);
        crosshairTexture.wrapMode = TextureWrapMode.Clamp;
        crosshairTexture.filterMode = FilterMode.Point;

        DrawBox(0, 0, crosshairTexture.width, crosshairTexture.height, crosshairTexture, new Color(0, 0, 0, 0));

        int startGapShort = Mathf.CeilToInt(crosshair.thickness / 2.0f);

        if (crosshair.border)
        {
            // ***** Border Drawing *****
            // Top
            DrawBox(centerBias - startGapShort - (crosshair.borderThickness / 2),
                centerBias + crosshair.gap - (crosshair.borderThickness / 2),
                crosshair.thickness + crosshair.borderThickness,
                crosshair.size + crosshair.borderThickness,
                crosshairTexture,
                crosshair.borderColor);

            // Right
            DrawBox(centerBias + crosshair.gap - (crosshair.borderThickness / 2),
                centerBias - startGapShort - (crosshair.borderThickness / 2),
                crosshair.size + crosshair.borderThickness,
                crosshair.thickness + crosshair.borderThickness,
                crosshairTexture,
                crosshair.borderColor);


            // Bottom
            DrawBox(centerBias - startGapShort - (crosshair.borderThickness / 2),
                centerBias - crosshair.gap - crosshair.size - (crosshair.borderThickness / 2),
                crosshair.thickness + crosshair.borderThickness,
                crosshair.size + crosshair.borderThickness,
                crosshairTexture,
                crosshair.borderColor);


            // Left
            DrawBox(centerBias - crosshair.gap - crosshair.size - (crosshair.borderThickness / 2),
               centerBias - startGapShort - (crosshair.borderThickness / 2),
               crosshair.size + crosshair.borderThickness,
               crosshair.thickness + crosshair.borderThickness,
               crosshairTexture,
               crosshair.borderColor);

            if (m_crosshair.dot)
            {
                // Dot
                DrawBox(centerBias - (crosshair.dotThickness / 2) - (crosshair.borderThickness / 2),
                   centerBias - (crosshair.dotThickness / 2) - (crosshair.borderThickness / 2),
                   crosshair.dotThickness + crosshair.borderThickness,
                   crosshair.dotThickness + crosshair.borderThickness,
                   crosshairTexture,
                   crosshair.borderColor);
            }
        }

        // ***** Crosshair Drawing *****
        // Top
        DrawBox(centerBias - startGapShort,
            centerBias + crosshair.gap,
            crosshair.thickness,
            crosshair.size,
            crosshairTexture,
            crosshair.color);

        // Right
        DrawBox(centerBias + crosshair.gap,
            centerBias - startGapShort,
            crosshair.size,
            crosshair.thickness,
            crosshairTexture,
            crosshair.color);


        // Bottom
        DrawBox(centerBias - startGapShort,
            centerBias - crosshair.gap - crosshair.size,
            crosshair.thickness,
            crosshair.size,
            crosshairTexture,
            crosshair.color);


        // Left
        DrawBox(centerBias - crosshair.gap - crosshair.size,
           centerBias - startGapShort,
           crosshair.size,
           crosshair.thickness,
           crosshairTexture,
           crosshair.color);

        if (m_crosshair.dot)
        {
            // Dot
            DrawBox(centerBias - (crosshair.dotThickness / 2),
               centerBias - (crosshair.dotThickness / 2),
               crosshair.dotThickness,
               crosshair.dotThickness,
               crosshairTexture,
               crosshair.color);
        }

        crosshairTexture.Apply();
        return crosshairTexture;
    }

    private void DrawBox(int startX, int startY, int width, int height, Texture2D target, Color color)
    {
        if (startX + width > target.width ||
            startY + height > target.height)
        {
            Debug.LogWarning("Crosshair box is out of range.");
            return;
        }
        for (int x = startX; x < startX + width; ++x)
        {
            for (int y = startY; y < startY + height; ++y)
            {
                target.SetPixel(x, y, color);
            }
        }
    }
    #endregion
}
