using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Затемняет экран четырьмя прямоугольниками вокруг подсветки, оставляя центр чистым.
/// </summary>
public sealed class TutorialSpotlightDim : MaskableGraphic, ICanvasRaycastFilter
{
    private RectTransform _spotlight;
    private RectTransform _passThroughRect;
    private bool _spotlightActive;
    private readonly Vector3[] _spotlightCorners = new Vector3[4];

    public void SetSpotlight(RectTransform spotlight)
    {
        _spotlight = spotlight;
        SetVerticesDirty();
    }

    public void SetPassThroughRect(RectTransform passThroughRect)
    {
        _passThroughRect = passThroughRect;
    }

    public void SetSpotlightActive(bool active)
    {
        _spotlightActive = active;
        SetVerticesDirty();
    }

    public void Refresh()
    {
        SetVerticesDirty();
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (IsInsidePassThroughRect(screenPoint, eventCamera))
            return false;

        if (!_spotlightActive || _spotlight == null || !_spotlight.gameObject.activeInHierarchy)
            return true;

        return !RectTransformUtility.RectangleContainsScreenPoint(
            _spotlight,
            screenPoint,
            eventCamera);
    }

    private bool IsInsidePassThroughRect(Vector2 screenPoint, Camera eventCamera)
    {
        if (_passThroughRect == null || !_passThroughRect.gameObject.activeInHierarchy)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            _passThroughRect,
            screenPoint,
            eventCamera);
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();

        Rect fullRect = rectTransform.rect;

        if (!_spotlightActive || _spotlight == null || !_spotlight.gameObject.activeInHierarchy)
        {
            AddQuad(vertexHelper, fullRect.xMin, fullRect.yMin, fullRect.xMax, fullRect.yMax);
            return;
        }

        Rect holeRect = GetHoleRect(fullRect);

        AddQuad(vertexHelper, fullRect.xMin, fullRect.yMin, holeRect.xMin, fullRect.yMax);
        AddQuad(vertexHelper, holeRect.xMax, fullRect.yMin, fullRect.xMax, fullRect.yMax);
        AddQuad(vertexHelper, holeRect.xMin, holeRect.yMax, holeRect.xMax, fullRect.yMax);
        AddQuad(vertexHelper, holeRect.xMin, fullRect.yMin, holeRect.xMax, holeRect.yMin);
    }

    private Rect GetHoleRect(Rect fullRect)
    {
        _spotlight.GetWorldCorners(_spotlightCorners);

        Vector2 min = WorldToLocal(_spotlightCorners[0]);
        Vector2 max = min;

        for (int i = 1; i < _spotlightCorners.Length; i++)
        {
            Vector2 point = WorldToLocal(_spotlightCorners[i]);
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        min.x = Mathf.Clamp(min.x, fullRect.xMin, fullRect.xMax);
        min.y = Mathf.Clamp(min.y, fullRect.yMin, fullRect.yMax);
        max.x = Mathf.Clamp(max.x, fullRect.xMin, fullRect.xMax);
        max.y = Mathf.Clamp(max.y, fullRect.yMin, fullRect.yMax);

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    private Vector2 WorldToLocal(Vector3 worldPosition)
    {
        return rectTransform.InverseTransformPoint(worldPosition);
    }

    private void AddQuad(VertexHelper vertexHelper, float xMin, float yMin, float xMax, float yMax)
    {
        if (xMax <= xMin || yMax <= yMin)
            return;

        int startIndex = vertexHelper.currentVertCount;
        Color32 vertexColor = color;

        vertexHelper.AddVert(new Vector3(xMin, yMin), vertexColor, Vector2.zero);
        vertexHelper.AddVert(new Vector3(xMin, yMax), vertexColor, Vector2.up);
        vertexHelper.AddVert(new Vector3(xMax, yMax), vertexColor, Vector2.one);
        vertexHelper.AddVert(new Vector3(xMax, yMin), vertexColor, Vector2.right);

        vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }
}
