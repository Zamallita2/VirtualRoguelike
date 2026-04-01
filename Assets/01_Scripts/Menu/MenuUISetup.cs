// Script: MenuUISetup.cs
// Pégalo en una carpeta Scripts que crees

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUISetup : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public Image backgroundPanel;
    public TextMeshProUGUI titleText;
    public Button playButton;
    public Button multiplayerButton;
    public Button optionsButton;

    [Header("Colors Medieval Theme")]
    public Color goldColor = new Color(1f, 0.84f, 0f);
    public Color darkBrown = new Color(0.2f, 0.1f, 0.05f, 0.8f);
    public Color stoneGray = new Color(0.3f, 0.3f, 0.3f, 0.9f);

    void Start()
    {
        SetupCanvas();
        SetupBackground();
        SetupTitle();
        SetupButtons();
    }

    void SetupCanvas()
    {
        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas_MenuUI");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.WorldSpace;

            // Posicionar el canvas frente al castillo
            canvasObj.transform.SetParent(this.transform);
            canvasObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            canvasObj.transform.localRotation = Quaternion.identity;

            RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(800, 600);
            canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;

            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }

    void SetupBackground()
    {
        GameObject bgObj = new GameObject("Panel_Background");
        bgObj.transform.SetParent(mainCanvas.transform);

        backgroundPanel = bgObj.AddComponent<Image>();
        backgroundPanel.color = new Color(0, 0, 0, 0.7f);

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Agregar borde dorado
        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = goldColor;
        outline.effectDistance = new Vector2(3, -3);
    }

    void SetupTitle()
    {
        GameObject titleObj = new GameObject("Title_CursedAscent");
        titleObj.transform.SetParent(mainCanvas.transform);

        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "CURSED ASCENT";
        titleText.fontSize = 72;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = goldColor;
        titleText.fontStyle = FontStyles.Bold;

        // Efecto de sombra
        titleText.outlineColor = Color.black;
        titleText.outlineWidth = 0.3f;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(700, 100);
        titleRect.anchoredPosition = Vector2.zero;

        // Animación de parpadeo del título
        titleObj.AddComponent<TitleGlowEffect>();
    }

    void SetupButtons()
    {
        // Botón JUGAR
        playButton = CreateMedievalButton("Button_Play", "JUGAR", new Vector2(0.5f, 0.5f), 0);

        // Botón MULTIJUGADOR
        multiplayerButton = CreateMedievalButton("Button_Multiplayer", "MULTIJUGADOR", new Vector2(0.5f, 0.35f), 1);

        // Botón OPCIONES
        optionsButton = CreateMedievalButton("Button_Options", "OPCIONES", new Vector2(0.5f, 0.2f), 2);
    }

    Button CreateMedievalButton(string name, string buttonText, Vector2 anchorPos, int index)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(mainCanvas.transform);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();

        // Color del botón
        buttonImage.color = stoneGray;

        // Configurar transición
        ColorBlock colors = button.colors;
        colors.normalColor = stoneGray;
        colors.highlightedColor = new Color(0.5f, 0.4f, 0.3f);
        colors.pressedColor = goldColor;
        colors.selectedColor = new Color(0.6f, 0.5f, 0.4f);
        button.colors = colors;

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorPos;
        buttonRect.anchorMax = anchorPos;
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(400, 80);
        buttonRect.anchoredPosition = Vector2.zero;

        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;
        text.color = goldColor;
        text.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Agregar efecto hover y script de sonido
        buttonObj.AddComponent<ButtonHoverEffect>();

        return button;
    }
}