using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// Камера для Android RTS:
/// - 1 палец: drag (движение камеры)
/// - 2 пальца: pinch (zoom)
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.01f;  

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;  //ограничение меньше нельзя приблизить
    [SerializeField] private float maxZoom = 10f;  //максимальное отдаление

    private Camera cam;

    private Vector2 lastTouchPosition;  //последняя позиция касания для расчета смещения
    private bool isDragging;  //флаг что сейчас идет перетаскивание
    private float lastPinchDistance;  //Последнее расстояние между пальцами

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Touchscreen.current == null)  //проверка на наличие
            return;

        var touches = Touchscreen.current.touches;  //получает список текущий касаний

        // 1 палец — движение камеры 
        if (touches.Count == 1 && touches[0].isInProgress)
        {
            Vector2 currentPos = touches[0].position.ReadValue();  //считывает позицию пальца

            if (!isDragging)  //если это первое касание 
            {
                lastTouchPosition = currentPos;  //начальная позиция косания
                isDragging = true;  //перетаскивание началось
                return;
            }

            Vector2 delta = currentPos - lastTouchPosition;  //вектор смещения 
            MoveCamera(delta);  //передает вектор 
            lastTouchPosition = currentPos;  //обновляем позицию для след кадра
        }
        else
        {  //если косание закончилось или условия нет
            isDragging = false;
        }

        // 2 пальца — zoom 
        if (touches.Count == 2 &&
            touches[0].isInProgress &&
            touches[1].isInProgress)   //условие что 2 активных косания 
        {
            float currentDistance = Vector2.Distance(
                touches[0].position.ReadValue(),
                touches[1].position.ReadValue()   //расчет растояние между двух пальцев
            );

            if (lastPinchDistance == 0)  //если первое косание небыло 
            {
                lastPinchDistance = currentDistance;  //сохраняет текущее расстояние как базовое
                return;
            }

            float delta = currentDistance - lastPinchDistance;  //Расчет разницы между растояниями 
            ZoomCamera(delta);
            lastPinchDistance = currentDistance;  //обновляет последнее расстояние
        }
        else
        {  //сброс елси условия не выполненно
            lastPinchDistance = 0;
        }
    }

    private void MoveCamera(Vector2 delta)
    {
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed;  //создает вектор движения
        transform.position += move;  
    }

    private void ZoomCamera(float delta)
    {
        cam.orthographicSize -= delta * zoomSpeed;  //изменение размера камеры
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);  //ограничиваем размеры 
    }
}
