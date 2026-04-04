using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUISetup : MonoBehaviour
{
    [SerializeField] private float menuScale = 0.0008f;   // más pequeño aún
    [SerializeField] private float menuDistance = 0.5f;
    [SerializeField] private float menuHeight = 0f;

    [HideInInspector] public Canvas mainCanvas;
    [HideInInspector] public CanvasGroup menuCanvasGroup;
    [HideInInspector] public Button playButton;
    [HideInInspector] public Button multiplayerButton;
    [HideInInspector] public Button optionsButton;
    [HideInInspector] public TextMeshProUGUI titleText;

    void Awake()
    {
        CreateMenu();
    }

    void CreateMenu()
    {
        GameObject canvasGO = new GameObject("MenuCanvas_World");
        canvasGO.transform.SetParent(transform, false);
        mainCanvas = canvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800, 600);
        canvasRect.localScale = Vector3.one * menuScale;

        Camera cam = Camera.main;
        if (cam != null)
        {
            canvasRect.position = cam.transform.position + cam.transform.forward * menuDistance + Vector3.up * menuHeight;
            canvasRect.rotation = Quaternion.LookRotation(canvasRect.position - cam.transform.position);
        }
        else
        {
            canvasRect.localPosition = new Vector3(0, menuHeight, menuDistance);
        }

        menuCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;

        // Fondo oscuro
        GameObject panel = new GameObject("Background");
        panel.transform.SetParent(canvasGO.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.08f, 0.06f, 0.95f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Borde dorado
        GameObject border = new GameObject("GoldenBorder");
        border.transform.SetParent(canvasGO.transform, false);
        Image borderImg = border.AddComponent<Image>();
        borderImg.color = new Color(1f, 0.84f, 0f, 1f);
        RectTransform borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(-20, -20);

        // Panel interior
        GameObject innerPanel = new GameObject("InnerPanel");
        innerPanel.transform.SetParent(canvasGO.transform, false);
        Image innerImg = innerPanel.AddComponent<Image>();
        innerImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        RectTransform innerRect = innerPanel.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.sizeDelta = new Vector2(-26, -26);

        // Título
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(canvasGO.transform, false);
        titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "CURSED ASCENT";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.84f, 0f);
        titleText.fontStyle = FontStyles.Bold;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.75f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        titleGO.AddComponent<TitleGlowEffect>();

        // Botones
        playButton = CreateButton(canvasGO.transform, "PlayButton", "JUGAR", new Vector2(0.5f, 0.55f), new Vector2(300, 70));
        multiplayerButton = CreateButton(canvasGO.transform, "MultiplayerButton", "MULTIJUGADOR", new Vector2(0.5f, 0.38f), new Vector2(300, 70));
        optionsButton = CreateButton(canvasGO.transform, "OptionsButton", "OPCIONES", new Vector2(0.5f, 0.21f), new Vector2(300, 70));
    }

    Button CreateButton(Transform parent, string name, string text, Vector2 anchorPosition, Vector2 size)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);
        RectTransform btnRect = btnGO.AddComponent<RectTransform>();
        btnRect.anchorMin = anchorPosition;
        btnRect.anchorMax = anchorPosition;
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = Vector2.zero;

        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.2f, 0.1f);
        Button button = btnGO.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.2f, 0.1f);
        colors.highlightedColor = new Color(0.5f, 0.35f, 0.2f);
        colors.pressedColor = new Color(0.2f, 0.15f, 0.05f);
        colors.selectedColor = new Color(0.4f, 0.28f, 0.15f);
        button.colors = colors;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);
        TextMeshProUGUI txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 32;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = new Color(1f, 0.84f, 0f);
        txt.fontStyle = FontStyles.Bold;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        btnGO.AddComponent<ButtonHoverEffect>();
        return button;
    }

    public void RepositionInFrontOfCamera()
    {
        if (mainCanvas == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        canvasRect.position = cam.transform.position + cam.transform.forward * menuDistance + Vector3.up * menuHeight;
        canvasRect.rotation = Quaternion.LookRotation(canvasRect.position - cam.transform.position);
    }
}