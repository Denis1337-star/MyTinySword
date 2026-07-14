using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

/// <summary>
/// Отображение tutorial UI: dim с вырезом, banner, highlight-рамка.
/// </summary>
public sealed class TutorialUiView : ValidatedMonoBehaviour
{
    private const int HighlightFrameSortingOrder = 60;
    private const float UiHighlightPadding = 10f;

    [Header("Layers")]
    [SerializeField] private GameObject _dimOverlay;
    [SerializeField] private TutorialPanel _fullScreenPanel;
    [SerializeField] private GameObject _bottomBannerRoot;
    [SerializeField] private TMP_Text _bannerMessageText;
    [SerializeField] private TMP_Text _bannerStepCounterText;
    [SerializeField] private RectTransform _highlightFrame;

    [Header("World Highlight")]
    [SerializeField] private Vector2 _worldHighlightSize = new(150f, 130f);
    [SerializeField, Min(0f)] private float _worldHighlightPadding = 20f;

    private CanvasGroup _dimCanvasGroup;
    private TutorialSpotlightDim _spotlightDim;
    private RectTransform _highlightParent;
    private Transform _worldHighlightTarget;
    private RectTransform _uiHighlightTarget;
    private Camera _worldCamera;
    private Camera _uiCamera;

    public Button FullScreenNextButton => _fullScreenPanel.NextButton;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        _dimCanvasGroup = _dimOverlay.GetComponent<CanvasGroup>();
        _highlightParent = _highlightFrame.parent as RectTransform;
        _uiCamera = ResolveUiCamera();

        Image dimImage = _dimOverlay.GetComponent<Image>();
        Color dimColor = dimImage != null
            ? dimImage.color
            : new Color(0f, 0f, 0f, 0.63f);

        if (dimImage != null)
        {
            dimImage.enabled = false;
            dimImage.raycastTarget = false;
        }

        _spotlightDim = CreateSpotlightDim(dimColor);
        _spotlightDim.SetSpotlight(_highlightFrame);

        Image highlightImage = _highlightFrame.GetComponent<Image>();

        if (highlightImage != null)
        {
            highlightImage.raycastTarget = false;
            highlightImage.type = Image.Type.Sliced;
            highlightImage.fillCenter = false;
        }

        Canvas highlightCanvas = _highlightFrame.GetComponent<Canvas>();

        if (highlightCanvas == null)
            highlightCanvas = _highlightFrame.gameObject.AddComponent<Canvas>();

        highlightCanvas.overrideSorting = true;
        highlightCanvas.sortingOrder = HighlightFrameSortingOrder;

        HideAll();
    }

    private void LateUpdate()
    {
        if (!_highlightFrame.gameObject.activeSelf)
            return;

        if (_worldHighlightTarget != null)
            PlaceHighlightOnWorldTarget(_worldHighlightTarget);
        else if (_uiHighlightTarget != null)
            FitHighlightOverUiTarget(_uiHighlightTarget);

        RefreshSpotlight();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _dimOverlay, nameof(_dimOverlay));
        valid &= ValidationUtility.IsAssigned(this, _fullScreenPanel, nameof(_fullScreenPanel));
        valid &= ValidationUtility.IsAssigned(this, _bottomBannerRoot, nameof(_bottomBannerRoot));
        valid &= ValidationUtility.IsAssigned(this, _bannerMessageText, nameof(_bannerMessageText));
        valid &= ValidationUtility.IsAssigned(this, _bannerStepCounterText, nameof(_bannerStepCounterText));
        valid &= ValidationUtility.IsAssigned(this, _highlightFrame, nameof(_highlightFrame));

        if (_dimOverlay != null && _dimOverlay.GetComponent<CanvasGroup>() == null)
        {
            Debug.LogError($"{name}: на {nameof(_dimOverlay)} нужен CanvasGroup.", this);
            valid = false;
        }

        return valid;
    }

    public void Present(
        TutorialStepData step,
        int stepIndex,
        int totalSteps,
        TutorialStepDefinition definition,
        Camera worldCamera,
        RectTransform uiHighlightTarget,
        Transform worldHighlightTarget)
    {
        _worldCamera = worldCamera;
        _uiCamera = ResolveUiCamera();
        ClearHighlight();

        if (definition.DimVisible)
        {
            _dimOverlay.SetActive(true);
            bool spotlightBlocksInput = ShouldSpotlightBlockInput(definition);
            _dimCanvasGroup.blocksRaycasts = definition.DimBlocksInput && !spotlightBlocksInput;
            _spotlightDim.raycastTarget = spotlightBlocksInput;
            _spotlightDim.color = new Color(
                0f,
                0f,
                0f,
                definition.UiMode == TutorialUiMode.FullScreenInfo ? 0.35f : 0.63f);
        }
        else
        {
            _dimOverlay.SetActive(false);
            _dimCanvasGroup.blocksRaycasts = false;
            _spotlightDim.raycastTarget = false;
        }

        switch (definition.UiMode)
        {
            case TutorialUiMode.FullScreenBlock:
                ShowBlockedFullScreen(step, stepIndex, totalSteps);
                break;

            case TutorialUiMode.FullScreenInfo:
                ShowInfoFullScreen(step, stepIndex, totalSteps);
                break;

            case TutorialUiMode.GuidedBanner:
                ShowGuidedBanner(step, stepIndex, totalSteps, worldHighlightTarget);
                break;

            case TutorialUiMode.GuidedUiButton:
                ShowGuidedUiButton(step, stepIndex, totalSteps, uiHighlightTarget);
                break;

            case TutorialUiMode.GuidedUiPanel:
                ShowGuidedUiPanel(step, stepIndex, totalSteps, uiHighlightTarget);
                break;
        }
    }

    public void HideAll()
    {
        ClearHighlight();

        _dimOverlay.SetActive(false);
        _bottomBannerRoot.SetActive(false);

        if (_fullScreenPanel.gameObject.activeInHierarchy)
            _fullScreenPanel.HideImmediate();
    }

    private static bool ShouldSpotlightBlockInput(TutorialStepDefinition definition)
    {
        if (!definition.DimBlocksInput)
            return false;

        return definition.UiMode is TutorialUiMode.GuidedBanner
            or TutorialUiMode.GuidedUiButton
            or TutorialUiMode.GuidedUiPanel;
    }

    private void ShowBlockedFullScreen(TutorialStepData step, int stepIndex, int totalSteps)
    {
        _bottomBannerRoot.SetActive(false);
        _fullScreenPanel.ShowStep(step.GetMessage(YG2.lang), stepIndex, totalSteps);
        _fullScreenPanel.NextButton.gameObject.SetActive(step.AllowManualNext);
    }

    private void ShowInfoFullScreen(TutorialStepData step, int stepIndex, int totalSteps)
    {
        _bottomBannerRoot.SetActive(false);
        _fullScreenPanel.ShowInfo(step.GetMessage(YG2.lang), stepIndex, totalSteps);
    }

    private void ShowGuidedBanner(
        TutorialStepData step,
        int stepIndex,
        int totalSteps,
        Transform worldHighlightTarget)
    {
        HideFullScreenPanel();
        ShowBanner(step, stepIndex, totalSteps);

        if (worldHighlightTarget == null)
            return;

        _worldHighlightTarget = worldHighlightTarget;
        _highlightFrame.gameObject.SetActive(true);
        PlaceHighlightOnWorldTarget(worldHighlightTarget);
    }

    private void ShowGuidedUiButton(
        TutorialStepData step,
        int stepIndex,
        int totalSteps,
        RectTransform uiHighlightTarget)
    {
        ShowGuidedUiTarget(step, stepIndex, totalSteps, uiHighlightTarget);
    }

    private void ShowGuidedUiPanel(
        TutorialStepData step,
        int stepIndex,
        int totalSteps,
        RectTransform uiHighlightTarget)
    {
        ShowGuidedUiTarget(step, stepIndex, totalSteps, uiHighlightTarget);
    }

    private void ShowGuidedUiTarget(
        TutorialStepData step,
        int stepIndex,
        int totalSteps,
        RectTransform uiHighlightTarget)
    {
        HideFullScreenPanel();
        ShowBanner(step, stepIndex, totalSteps);

        if (uiHighlightTarget == null)
            return;

        _uiHighlightTarget = uiHighlightTarget;
        _highlightFrame.gameObject.SetActive(true);
        FitHighlightOverUiTarget(uiHighlightTarget);
    }

    private void ShowBanner(TutorialStepData step, int stepIndex, int totalSteps)
    {
        _bottomBannerRoot.SetActive(true);
        _bannerMessageText.text = step.GetMessage(YG2.lang);
        _bannerStepCounterText.text = $"{stepIndex + 1}/{totalSteps}";
    }

    private void HideFullScreenPanel()
    {
        if (_fullScreenPanel.gameObject.activeInHierarchy)
            _fullScreenPanel.HideImmediate();
    }

    private void PlaceHighlightOnWorldTarget(Transform target)
    {
        if (target == null || _worldCamera == null || _highlightParent == null)
            return;

        if (TryGetWorldTargetScreenRect(target, out Rect screenRect))
        {
            FitHighlightToScreenRect(screenRect);
            return;
        }

        Vector3 worldPosition = ResolveWorldHighlightPosition(target);
        Vector3 screenPosition = _worldCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _highlightParent,
                screenPosition,
                _uiCamera,
                out Vector2 localPosition))
            return;

        _highlightFrame.anchoredPosition = localPosition;
        _highlightFrame.sizeDelta = _worldHighlightSize;
        SetSpotlightActive(true);
    }

    private void FitHighlightToScreenRect(Rect screenRect, float padding)
    {
        Vector2 screenMin = new(screenRect.xMin, screenRect.yMin);
        Vector2 screenMax = new(screenRect.xMax, screenRect.yMax);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _highlightParent,
            screenMin,
            _uiCamera,
            out Vector2 localMin);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _highlightParent,
            screenMax,
            _uiCamera,
            out Vector2 localMax);

        _highlightFrame.anchoredPosition = (localMin + localMax) * 0.5f;
        _highlightFrame.sizeDelta = new Vector2(
            Mathf.Abs(localMax.x - localMin.x),
            Mathf.Abs(localMax.y - localMin.y)) + Vector2.one * padding;

        SetSpotlightActive(true);
    }

    private void FitHighlightToScreenRect(Rect screenRect)
    {
        FitHighlightToScreenRect(screenRect, _worldHighlightPadding);
    }

    private bool TryGetWorldTargetScreenRect(Transform target, out Rect screenRect)
    {
        screenRect = default;

        if (!TryGetWorldTargetBounds(target, out Bounds worldBounds))
            return false;

        Vector3 center = worldBounds.center;
        Vector3 extents = worldBounds.extents;

        Vector3[] corners =
        {
            center + new Vector3(-extents.x, -extents.y, -extents.z),
            center + new Vector3(-extents.x, -extents.y, extents.z),
            center + new Vector3(-extents.x, extents.y, -extents.z),
            center + new Vector3(-extents.x, extents.y, extents.z),
            center + new Vector3(extents.x, -extents.y, -extents.z),
            center + new Vector3(extents.x, -extents.y, extents.z),
            center + new Vector3(extents.x, extents.y, -extents.z),
            center + new Vector3(extents.x, extents.y, extents.z)
        };

        bool hasVisibleCorner = false;
        Vector2 screenMin = default;
        Vector2 screenMax = default;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 screenPoint3D = _worldCamera.WorldToScreenPoint(corners[i]);

            if (screenPoint3D.z < 0f)
                continue;

            Vector2 screenPoint = screenPoint3D;

            if (!hasVisibleCorner)
            {
                screenMin = screenPoint;
                screenMax = screenPoint;
                hasVisibleCorner = true;
                continue;
            }

            screenMin = Vector2.Min(screenMin, screenPoint);
            screenMax = Vector2.Max(screenMax, screenPoint);
        }

        if (!hasVisibleCorner)
            return false;

        screenRect = Rect.MinMaxRect(screenMin.x, screenMin.y, screenMax.x, screenMax.y);
        return screenRect.width > 0f && screenRect.height > 0f;
    }

    private static bool TryGetWorldTargetBounds(Transform target, out Bounds bounds)
    {
        bounds = default;

        if (target == null)
            return false;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    bounds.Encapsulate(renderers[i].bounds);
            }

            return true;
        }

        Collider2D[] colliders2D = target.GetComponentsInChildren<Collider2D>();

        if (colliders2D.Length > 0)
        {
            bounds = colliders2D[0].bounds;

            for (int i = 1; i < colliders2D.Length; i++)
            {
                if (colliders2D[i] != null)
                    bounds.Encapsulate(colliders2D[i].bounds);
            }

            return true;
        }

        Collider[] colliders = target.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0)
            return false;

        bounds = colliders[0].bounds;

        for (int i = 1; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                bounds.Encapsulate(colliders[i].bounds);
        }

        return true;
    }

    private void FitHighlightOverUiTarget(RectTransform target)
    {
        if (target == null || _highlightParent == null)
            return;

        if (!TryGetScreenRect(target, out Rect screenRect))
            return;

        FitHighlightToScreenRect(screenRect, UiHighlightPadding);
    }

    private bool TryGetScreenRect(RectTransform target, out Rect screenRect)
    {
        screenRect = default;

        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        Camera targetCamera = ResolveCanvasCamera(target.GetComponentInParent<Canvas>());
        Vector2 min = RectTransformUtility.WorldToScreenPoint(targetCamera, corners[0]);
        Vector2 max = min;

        for (int i = 1; i < corners.Length; i++)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(targetCamera, corners[i]);
            min = Vector2.Min(min, screenPoint);
            max = Vector2.Max(max, screenPoint);
        }

        screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        return screenRect.width > 0f && screenRect.height > 0f;
    }

    private TutorialSpotlightDim CreateSpotlightDim(Color dimColor)
    {
        const string spotlightDimName = "TutorialSpotlightDim";

        Transform existingTransform = _dimOverlay.transform.Find(spotlightDimName);
        GameObject spotlightDimObject = existingTransform != null
            ? existingTransform.gameObject
            : new GameObject(
                spotlightDimName,
                typeof(RectTransform),
                typeof(CanvasRenderer));

        RectTransform spotlightRect = spotlightDimObject.transform as RectTransform;

        if (spotlightRect == null)
            spotlightRect = spotlightDimObject.AddComponent<RectTransform>();

        spotlightRect.SetParent(_dimOverlay.transform, false);
        spotlightRect.anchorMin = Vector2.zero;
        spotlightRect.anchorMax = Vector2.one;
        spotlightRect.pivot = new Vector2(0.5f, 0.5f);
        spotlightRect.anchoredPosition = Vector2.zero;
        spotlightRect.sizeDelta = Vector2.zero;
        spotlightRect.localScale = Vector3.one;

        TutorialSpotlightDim spotlightDim = spotlightDimObject.GetComponent<TutorialSpotlightDim>();

        if (spotlightDim == null)
            spotlightDim = spotlightDimObject.AddComponent<TutorialSpotlightDim>();

        spotlightDim.color = dimColor;
        spotlightDim.raycastTarget = true;

        return spotlightDim;
    }

    private void SetSpotlightActive(bool active)
    {
        if (_spotlightDim == null)
            return;

        _spotlightDim.SetSpotlightActive(active);
        RefreshSpotlight();
    }

    private void RefreshSpotlight()
    {
        if (_spotlightDim != null)
            _spotlightDim.Refresh();
    }

    private Vector3 ResolveWorldHighlightPosition(Transform target)
    {
        Renderer renderer = target.GetComponentInChildren<Renderer>();

        if (renderer != null)
            return renderer.bounds.center;

        Collider2D collider2D = target.GetComponentInChildren<Collider2D>();

        if (collider2D != null)
            return collider2D.bounds.center;

        Collider collider = target.GetComponentInChildren<Collider>();

        if (collider != null)
            return collider.bounds.center;

        return target.position;
    }

    private Camera ResolveUiCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
            return canvas.worldCamera;

        return Camera.main;
    }

    private Camera ResolveCanvasCamera(Canvas canvas)
    {
        if (canvas == null)
            return _uiCamera;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        if (canvas.worldCamera != null)
            return canvas.worldCamera;

        return _uiCamera;
    }

    private void ClearHighlight()
    {
        _worldHighlightTarget = null;
        _uiHighlightTarget = null;
        _highlightFrame.gameObject.SetActive(false);
        SetSpotlightActive(false);
    }
}
