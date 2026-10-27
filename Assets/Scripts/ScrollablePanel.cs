using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ScrollablePanel : MonoBehaviour
{
    [SerializeField, Min(1f)] private float extraScrollHeight = 6000f;
    [SerializeField, HideInInspector] private float appliedScrollHeight;

    private void Awake()
    {
        EnsureScrollHierarchy();
    }

    private void OnEnable()
    {
        EnsureScrollHierarchy();
    }

    private void OnValidate()
    {
        EnsureScrollHierarchy();
    }

    private void EnsureScrollHierarchy()
    {
        var panelRect = (RectTransform)transform;
        var viewport = transform.Find("Viewport") as RectTransform;

        if (viewport == null)
        {
            viewport = CreateUiObject("Viewport", panelRect);
            StretchToParent(viewport);
        }

        var viewportMask = viewport.GetComponent<RectMask2D>() ?? viewport.gameObject.AddComponent<RectMask2D>();
        // Keep every element visible while arranging the page in the Scene view.
        // The mask is restored automatically when the app is running.
        viewportMask.enabled = Application.isPlaying;

        var content = viewport.Find("Content") as RectTransform;
        if (content == null)
        {
            content = CreateUiObject("Content", viewport);
            StretchToParent(content);
        }

        var directChildren = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child != viewport && child.name != "FixedChrome" && child is RectTransform rectTransform)
            {
                directChildren.Add(rectTransform);
            }
        }

        foreach (var child in directChildren)
        {
            child.SetParent(content, false);

            if (appliedScrollHeight > 0f)
            {
                child.anchoredPosition += Vector2.up * (extraScrollHeight * 0.5f);
            }
        }

        var positionAdjustment = (extraScrollHeight - appliedScrollHeight) * 0.5f;
        content.sizeDelta = new Vector2(0f, extraScrollHeight);
        content.anchoredPosition = new Vector2(0f, -extraScrollHeight * 0.5f);

        if (!Mathf.Approximately(positionAdjustment, 0f))
        {
            foreach (Transform child in content)
            {
                if (child is RectTransform rectTransform)
                {
                    rectTransform.anchoredPosition += Vector2.up * positionAdjustment;
                }
            }
        }

        appliedScrollHeight = extraScrollHeight;

        var scrollRect = GetComponent<ScrollRect>() ?? gameObject.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = false;
        scrollRect.scrollSensitivity = 20f;
    }

    private static RectTransform CreateUiObject(string objectName, Transform parent)
    {
        var child = new GameObject(objectName, typeof(RectTransform));
        var rectTransform = child.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        return rectTransform;
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }
}
