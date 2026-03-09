using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float moveSpeed = 0.003f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    private Vector2 lastTouchPos;   //Позиция последнего тача
    public bool dragging;  //флаг происходит ли движение камеры
    private float lastPinchDist;  //растояние между двумя пальцами

    private void OnEnable() => EnhancedTouchSupport.Enable();  //вызывается когда скрипт активен
    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)  //колекция всех активных касаний
            return;

        var touches = Touch.activeTouches;

        // ЗУМ
        if (touches.Count >= 2)  //если два или более касания
        {
            Vector2 p0 = touches[0].screenPosition;  //координаты пальцев
            Vector2 p1 = touches[1].screenPosition;

            float dist = Vector2.Distance(p0, p1);  //растояние между пальцами

            if (lastPinchDist > 0)
                Zoom(dist - lastPinchDist);  //вызывает зум

            lastPinchDist = dist;
            dragging = false;
            return;
        }

        lastPinchDist = 0;

        // ПАНОРИМ
        if (touches.Count == 1)  
        {
            var touch = touches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)  //при начале косания
            {
                lastTouchPos = touch.screenPosition; //начальная позиция
                dragging = true;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && dragging)  //при лвижение пальца
            {
                Vector2 delta = touch.screenPosition - lastTouchPos;  
                Move(delta);
                lastTouchPos = touch.screenPosition;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)  //при завершение касания 
                dragging = false;
        }
    }

    private void Move(Vector2 delta)
    {
        float zoomFactor = virtualCamera.m_Lens.OrthographicSize;  //значение камеры 
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed * zoomFactor;  //чтобы камера двигалась за пальцем 
        virtualCamera.transform.position += move;  //меняет позицию камеры 
    }

    private void Zoom(float delta)
    {
        float size = virtualCamera.m_Lens.OrthographicSize;  //текущее значение 
        size -= delta * zoomSpeed;  
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(size, minZoom, maxZoom);  //записываеи обратно в диапозоне
    }
    public bool IsDragging() => dragging;
}

