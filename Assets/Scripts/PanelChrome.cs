using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PanelChrome : MonoBehaviour
{
    [SerializeField] private Sprite backArrow;
    [SerializeField] private Sprite bottomNavigation;
    [SerializeField] private Sprite scrollToTop;

    private void OnEnable() => Build();
    private void OnValidate() => Build();

    private void Build()
    {
        var root = transform.Find("FixedChrome") as RectTransform;
        if (root == null)
        {
            root = new GameObject("FixedChrome", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(transform, false);
        }

        // This chrome must remain visible whenever Panel 3 itself is shown.
        root.gameObject.SetActive(true);
        root.SetAsLastSibling();
        Stretch(root);
        CreateImage(root, "BackArrow", backArrow, new Vector2(65.9439f, 107.0512f),
            new Vector2(84.9192f, -107.8115f), new Vector2(0f, 1f));
        // The bottom bar is positioned manually in the editor. Only give it a default
        // layout the first time it is created; never overwrite later adjustments.
        CreateImage(root, "BottomNavigation", bottomNavigation, Vector2.zero, Vector2.zero,
            new Vector2(0.5f, 0f), true, 150f, false);
    }

    private static RectTransform CreateImage(RectTransform parent, string objectName, Sprite sprite, Vector2 size,
        Vector2 position, Vector2 anchor, bool stretchX = false, float height = 0f, bool overwriteLayout = true)
    {
        var child = parent.Find(objectName) as RectTransform;
        var wasCreated = child == null;

        if (child == null)
        {
            child = new GameObject(objectName, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            child.SetParent(parent, false);
        }

        var image = child.GetComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = objectName == "ScrollToTop";

        if (wasCreated || overwriteLayout)
        {
            if (stretchX)
            {
                child.anchorMin = new Vector2(0f, anchor.y);
                child.anchorMax = new Vector2(1f, anchor.y);
                child.sizeDelta = new Vector2(0f, height);
            }
            else
            {
                child.anchorMin = anchor;
                child.anchorMax = anchor;
                child.sizeDelta = size;
            }

            child.anchoredPosition = position;
        }

        return child;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
