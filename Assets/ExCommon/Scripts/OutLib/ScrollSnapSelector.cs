using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnapSelector : ScrollRect
{
    public int hIndex;
    public int vIndex;

    public int horizontalPages = 3;
    public int verticalPages = 3;
    public float smooth = 10f;

    Vector2 targetPosition;
    float hPerPage;
    float vPerPage;
    bool dragging;

    public bool forcePositionUpdate = false;

    void Awake()
    {
        hPerPage = 1.0f / (float)(horizontalPages - 1);
        vPerPage = 1.0f / (float)(verticalPages - 1);
    }

    void Start()
    {
        targetPosition = GetSnapPosition();
    }

    void Update()
    {
        if (!dragging && normalizedPosition != targetPosition)
        {
            normalizedPosition = Vector2.Lerp(normalizedPosition, targetPosition, smooth * Time.deltaTime);
            if (Vector2.Distance(normalizedPosition, targetPosition) < 0.009f)
            {
                normalizedPosition = targetPosition;
            }
        }
        if (forcePositionUpdate)
        {
            forcePositionUpdate = false;
            targetPosition = GetSnapPosition();
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        dragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        UpdateIndex();
        targetPosition = GetSnapPosition();
        dragging = false;
    }

    void UpdateIndex()
    {
        int xPage = -1;
        int yPage = -1;
        if (horizontal && horizontalPages > 0)
        {
            xPage = Mathf.RoundToInt(normalizedPosition.x / hPerPage);
            xPage = Mathf.Clamp(xPage, 0, horizontalPages - 1);
            hIndex = xPage;
        }

        if (vertical && verticalPages > 0)
        {
            yPage = Mathf.RoundToInt(normalizedPosition.y / vPerPage);
            yPage = Mathf.Clamp(yPage, 0, verticalPages - 1);
            vIndex = yPage;
        }
    }

    Vector2 GetSnapPosition()
    {
        return new Vector2(horizontal && horizontalPages > 0 ? hIndex * hPerPage : normalizedPosition.x, vertical && verticalPages > 0 ? vIndex * vPerPage : normalizedPosition.y);
    }
}