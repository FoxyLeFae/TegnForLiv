using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

[ExecuteAlways]
[DefaultExecutionOrder(100)]
public class ThreeVideoPanel : MonoBehaviour
{
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private Sprite startExerciseButton;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseLeftIcon;
    [SerializeField] private Sprite pauseRightIcon;
    [SerializeField] private Sprite restartIcon;
    [SerializeField] private Sprite kanIkkeGuide;
    [SerializeField] private Sprite traekkeVejretGuide;
    [SerializeField, Min(1f)] private float playerSpacing = 2200f;

    private bool isConfiguring;

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            EnsurePlayers();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            EnsurePlayers();
        }
    }

    private void Start()
    {
        EnsurePlayers();
        ConfigureRuntimeControls();
    }

    private void EnsurePlayers()
    {
        if (isConfiguring || videoClips == null || videoClips.Length == 0)
        {
            return;
        }

        var content = transform.Find("Viewport/Content");
        var playerTemplate = content?.Find("VideoPlayer");
        var playTemplate = content?.Find("Play");
        var pauseTemplate = content?.Find("Pause");
        var stopTemplate = content?.Find("Stop");

        if (content == null || playerTemplate == null || playTemplate == null ||
            pauseTemplate == null || stopTemplate == null)
        {
            return;
        }

        isConfiguring = true;

        try
        {
            ConfigurePlayer(playerTemplate.gameObject, videoClips[0]);
            ConfigureControlIcons(content, string.Empty);
            FindOrCreateStartExercise(content, "StartExercise 1", playerTemplate.GetComponent<RectTransform>());

            var firstPlayerRect = playerTemplate.GetComponent<RectTransform>();

            for (var index = 1; index < videoClips.Length; index++)
            {
                var suffix = index + 1;
                var offset = Vector2.down * (playerSpacing * index);
                var player = FindOrDuplicate(content, $"VideoPlayer {suffix}", playerTemplate.gameObject, offset);

                ConfigurePlayer(player, videoClips[index]);
                FindOrDuplicate(content, $"Play {suffix}", playTemplate.gameObject, offset);
                FindOrDuplicate(content, $"Pause {suffix}", pauseTemplate.gameObject, offset);
                FindOrDuplicate(content, $"Stop {suffix}", stopTemplate.gameObject, offset);
                ConfigureControlIcons(content, $" {suffix}");
                FindOrCreateStartExercise(content, $"StartExercise {suffix}", player.GetComponent<RectTransform>());

            }

            ConfigureLessonContent(content, firstPlayerRect);
        }
        finally
        {
            isConfiguring = false;
        }
    }

    private void ConfigureRuntimeControls()
    {
        var content = transform.Find("Viewport/Content");

        for (var index = 0; index < videoClips.Length; index++)
        {
            var suffix = index == 0 ? string.Empty : $" {index + 1}";
            var playerObject = content.Find($"VideoPlayer{suffix}").gameObject;
            var player = playerObject.GetComponent<VideoPlayer>();
            var controls = playerObject.GetComponent<VideoControls>();

            if (index > 0 && player.targetTexture != null)
            {
                var renderTexture = new RenderTexture(player.targetTexture.descriptor);
                renderTexture.Create();
                player.targetTexture = renderTexture;
                playerObject.GetComponent<RawImage>().texture = renderTexture;
            }

            ConfigureButton(content.Find($"Play{suffix}").GetComponent<Button>(), controls.PlayVideo);
            ConfigureButton(content.Find($"Pause{suffix}").GetComponent<Button>(), controls.PauseVideo);
            ConfigureButton(content.Find($"Stop{suffix}").GetComponent<Button>(), controls.RestartVideo);
            ClearButton(content.Find($"StartExercise {index + 1}").GetComponent<Button>());
        }

        var backToTop = content.Find("ScrollToTop")?.GetComponent<Button>();
        if (backToTop != null)
        {
            ConfigureButton(backToTop, () => GetComponent<ScrollRect>().verticalNormalizedPosition = 1f);
        }
    }

    private static GameObject FindOrDuplicate(Transform content, string objectName, GameObject template, Vector2 offset)
    {
        var existing = content.Find(objectName);
        if (existing != null)
        {
            return existing.gameObject;
        }

        var copy = Instantiate(template, content);
        copy.name = objectName;
        copy.GetComponent<RectTransform>().anchoredPosition += offset;
        return copy;
    }

    private void ConfigureLessonContent(Transform content, RectTransform firstPlayer)
    {
        var secondPlayer = content.Find("VideoPlayer 2") as RectTransform;
        var thirdPlayer = content.Find("VideoPlayer 3") as RectTransform;

        if (secondPlayer == null || thirdPlayer == null)
        {
            return;
        }

        // The page title is the first element on the page and sits immediately above video one.
        CreateText(content, "PageTitle", "\"Jeg kan ikke få luft\"",
            firstPlayer.anchoredPosition + Vector2.up * (firstPlayer.sizeDelta.y * 0.5f + 230f),
            new Vector2(1250f, 180f), 96f, TextAlignmentOptions.Center, true, false, false);
        CreateText(content, "PaceNote", "Okay, det gik måske lidt stærkt. Lad os bryde det lidt ned i trin:",
            firstPlayer.anchoredPosition + Vector2.down * 1700f, new Vector2(1200f, 150f), 48f, TextAlignmentOptions.Left, false, false);
        CreateText(content, "Introduction",
            "Sætningen består af 2 tegn; \"jeg kan ikke\" og \"trække vejret\". Lad os starte med det første.",
            firstPlayer.anchoredPosition + Vector2.down * 2100f, new Vector2(1200f, 300f), 54f, TextAlignmentOptions.Left, false, false);
        CreateText(content, "SectionTitleOne", "\"Jeg kan ikke\"", secondPlayer.anchoredPosition + Vector2.up * 1200f,
            new Vector2(1250f, 180f), 96f, TextAlignmentOptions.Center, true, false, false);

        CreateText(content, "KanIkkeIntro",
            "For at lave tegnet for \"jeg kan ikke\", starter du med en hånd på brystet for at indikere at det er dig du snakker om. Herefter kommer \"kan ikke\" delen:",
            secondPlayer.anchoredPosition + Vector2.down * 1450f, new Vector2(1200f, 620f), 56f, TextAlignmentOptions.Left);
        CreateGuide(content, "KanIkkeGuide", kanIkkeGuide, secondPlayer.anchoredPosition + Vector2.down * 2350f,
            new Vector2(1100f, 1080f));
        CreateText(content, "KanIkkeDetails",
            "Herefter tegner du nærmest et slags \"L\" i luften med højre hånd, men med det loop. Du starter med at vende hånden så håndryggen vender opad. Så knytter du hånden sammen, men lader pege- og langefingeren pege frem. Så tegner du først nedad, og når du så skal lave det her \"loop\" lader du så håndleddet dreje med, så håndryggen ender med at vende nedad, og tegner så ud mod højre.",
            secondPlayer.anchoredPosition + Vector2.down * 3600f, new Vector2(1200f, 1350f), 54f, TextAlignmentOptions.Left);
        CreateText(content, "KanIkkePractice", "Prøv øvelsen herunder:", secondPlayer.anchoredPosition + Vector2.down * 4450f,
            new Vector2(1200f, 120f), 52f, TextAlignmentOptions.Left);
        CreateText(content, "SectionTitleTwo", "\"Trække vejret\"", thirdPlayer.anchoredPosition + Vector2.up * 1200f,
            new Vector2(1250f, 180f), 96f, TextAlignmentOptions.Center, true);
        CreateText(content, "TraekkeIntro", "Denne del er egentlig meget ligetil:", thirdPlayer.anchoredPosition + Vector2.down * 1450f,
            new Vector2(1200f, 140f), 56f, TextAlignmentOptions.Left);
        CreateGuide(content, "TraekkeVejretGuide", traekkeVejretGuide, thirdPlayer.anchoredPosition + Vector2.down * 1950f,
            new Vector2(1100f, 760f));
        CreateText(content, "TraekkeDetails",
            "Du tager samme hånd, og fører den hen på brystkassen. Herefter fører du den en smule væk fra brystkassen, i en nærmest pulserende bevægelse. Dette gentager du 2 gange, og så har du tegnet \"trække vejret\".",
            thirdPlayer.anchoredPosition + Vector2.down * 2775f, new Vector2(1200f, 820f), 54f, TextAlignmentOptions.Left);
        CreateText(content, "TraekkePractice", "Prøv øvelsen herunder:", thirdPlayer.anchoredPosition + Vector2.down * 3360f,
            new Vector2(1200f, 120f), 52f, TextAlignmentOptions.Left);
        CreateText(content, "ClosingText",
            "Du kan tage øvelserne lige så mange gange som du har brug for, men når du føler dig klar, kan du scrolle tilbage til toppen og tage den fulde øvelse igen.",
            thirdPlayer.anchoredPosition + Vector2.down * 4460f, new Vector2(1200f, 560f), 54f, TextAlignmentOptions.Left);
        CreateText(content, "BackToTopLabel", "Tilbage til toppen:", thirdPlayer.anchoredPosition + Vector2.down * 4950f,
            new Vector2(1200f, 120f), 52f, TextAlignmentOptions.Left);

    }

    private static void CreateText(Transform parent, string objectName, string text, Vector2 position, Vector2 size,
        float fontSize, TextAlignmentOptions alignment, bool bold = false, bool overwriteLayout = false, bool overwriteStyle = false)
    {
        var label = parent.Find(objectName) as RectTransform;
        var wasCreated = label == null;
        if (label == null)
        {
            label = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
            label.SetParent(parent, false);
        }

        if (wasCreated || overwriteLayout)
        {
            label.anchorMin = label.anchorMax = new Vector2(0.5f, 0.5f);
            label.pivot = new Vector2(0.5f, 0.5f);
            label.anchoredPosition = position;
            label.sizeDelta = size;
        }
        var textComponent = label.GetComponent<TextMeshProUGUI>();
        if (wasCreated)
        {
            textComponent.text = text;
        }
        if (wasCreated || overwriteStyle)
        {
            textComponent.font = TMP_Settings.defaultFontAsset;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            textComponent.color = new Color32(0, 74, 90, 255);
            textComponent.alignment = alignment;
            textComponent.textWrappingMode = TextWrappingModes.Normal;
            textComponent.overflowMode = TextOverflowModes.Overflow;
        }
        textComponent.raycastTarget = false;
    }

    private static void CreateGuide(Transform parent, string objectName, Sprite sprite, Vector2 position, Vector2 size)
    {
        if (sprite == null)
        {
            return;
        }

        var guide = parent.Find(objectName) as RectTransform;
        var wasCreated = guide == null;
        if (guide == null)
        {
            guide = new GameObject(objectName, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            guide.SetParent(parent, false);
        }

        if (wasCreated)
        {
            guide.anchorMin = guide.anchorMax = new Vector2(0.5f, 0.5f);
            guide.pivot = new Vector2(0.5f, 0.5f);
            guide.anchoredPosition = position;
            guide.sizeDelta = size;
        }
        var rawImage = guide.GetComponent<RawImage>();
        if (rawImage != null)
        {
            rawImage.enabled = false;
        }

        var image = guide.GetComponent<Image>() ?? guide.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
    }

    private void FindOrCreateStartExercise(Transform content, string objectName, RectTransform player)
    {
        if (content.Find(objectName) != null)
        {
            return;
        }

        var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
        var rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.SetParent(content, false);
        rectTransform.anchorMin = player.anchorMin;
        rectTransform.anchorMax = player.anchorMax;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(540f, 150f);
        rectTransform.anchoredPosition = player.anchoredPosition + Vector2.down * 980f;

        var image = buttonObject.GetComponent<Image>();
        image.sprite = startExerciseButton;
        image.preserveAspect = true;
    }

    private static void ConfigureButton(Button button, UnityAction action)
    {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(action);
    }

    private static void ClearButton(Button button)
    {
        button.onClick = new Button.ButtonClickedEvent();
    }

    private void ConfigureControlIcons(Transform content, string suffix)
    {
        SetControlIcon(content, $"Play{suffix}", playIcon);
        SetControlIcon(content, $"Pause{suffix}", pauseLeftIcon, pauseRightIcon);
        SetControlIcon(content, $"Stop{suffix}", restartIcon);
    }

    private static void SetControlIcon(Transform content, string controlName, Sprite primaryIcon, Sprite secondaryIcon = null)
    {
        if (primaryIcon == null)
        {
            return;
        }

        var control = content.Find(controlName);

        if (control == null)
        {
            return;
        }

        var label = control.GetComponentInChildren<TextMeshProUGUI>(true);

        if (label != null)
        {
            label.gameObject.SetActive(false);
        }

        var iconRoot = control.Find("Icon") as RectTransform;

        if (iconRoot == null)
        {
            var iconRootObject = new GameObject("Icon", typeof(RectTransform));
            iconRoot = iconRootObject.GetComponent<RectTransform>();
            iconRoot.SetParent(control, false);
            iconRoot.anchorMin = Vector2.zero;
            iconRoot.anchorMax = Vector2.one;
            iconRoot.offsetMin = Vector2.zero;
            iconRoot.offsetMax = Vector2.zero;
        }

        CreateOrUpdateIcon(iconRoot, "Primary", primaryIcon, secondaryIcon == null
            ? new Vector2(0.15f, 0.1f)
            : new Vector2(0.2f, 0.1f), secondaryIcon == null
            ? new Vector2(0.85f, 0.9f)
            : new Vector2(0.4f, 0.9f));

        if (secondaryIcon != null)
        {
            CreateOrUpdateIcon(iconRoot, "Secondary", secondaryIcon, new Vector2(0.6f, 0.1f), new Vector2(0.8f, 0.9f));
        }
    }

    private static void CreateOrUpdateIcon(RectTransform parent, string iconName, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax)
    {
        var icon = parent.Find(iconName) as RectTransform;

        if (icon == null)
        {
            var iconObject = new GameObject(iconName, typeof(RectTransform), typeof(Image));
            icon = iconObject.GetComponent<RectTransform>();
            icon.SetParent(parent, false);
        }

        icon.anchorMin = anchorMin;
        icon.anchorMax = anchorMax;
        icon.offsetMin = Vector2.zero;
        icon.offsetMax = Vector2.zero;

        var image = icon.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
    }

    private static void ConfigurePlayer(GameObject playerObject, VideoClip clip)
    {
        var player = playerObject.GetComponent<VideoPlayer>();
        player.clip = clip;
        playerObject.GetComponent<VideoControls>().videoPlayer = player;
    }
}
