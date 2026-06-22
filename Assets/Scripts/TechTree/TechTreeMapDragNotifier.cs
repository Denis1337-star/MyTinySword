using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Сообщает дереву развития, что игрок начал двигать карту.
/// </summary>
public sealed class TechTreeMapDragNotifier : MonoBehaviour, IBeginDragHandler
{
    public event Action DragStarted;

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragStarted?.Invoke();
    }
}