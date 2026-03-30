using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Контроллер ручного управления камерой
/// Отвечает за pan одним пальцем и zoom двумя пальцами
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 0.003f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    private Vector2 lastTouchPos;   // Последняя позиция пальца для вычисления delta при drag
    private float lastPinchDist;     // Последняя дистанция между двумя пальцами для pinch zoom
    private bool isDragging;       // Флаг, показывает, что игрок сейчас действительно тащит камеру вручную

    public bool IsDragging => isDragging;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    private void OnValidate()
    {
        // Не допускаем некорректный диапазон zoom
        minZoom = Mathf.Max(0.01f, minZoom);
        maxZoom = Mathf.Max(minZoom, maxZoom);

        moveSpeed = Mathf.Max(0f, moveSpeed);
        zoomSpeed = Mathf.Max(0f, zoomSpeed);
    }

    private void Update()
    {
        if (virtualCamera == null)
            return;

        var touches = Touch.activeTouches;

        if (touches.Count == 0)   // Если касаний нет — сбрасываем временное input-состояние
        {
            ResetTouchState();
            return;
        }

        if (touches.Count >= 2)  // Если касаний нет — сбрасываем временное input-состояние
        {
            HandleZoom(touches);
            return;
        }

        HandlePan(touches[0]);    // Один палец — ручной drag камеры
    }

    /// <summary>
    /// Обрабатывает pinch zoom по двум пальцам.
    /// </summary>
    private void HandleZoom(System.Collections.Generic.IReadOnlyList<Touch> touches)
    {
        Vector2 p0 = touches[0].screenPosition;
        Vector2 p1 = touches[1].screenPosition;

        float currentDist = Vector2.Distance(p0, p1);

        // Если это не первый кадр pinch-жеста —
        // можно вычислить изменение расстояния между пальцами
        if (lastPinchDist > 0f)
        {
            float delta = currentDist - lastPinchDist;
            Zoom(delta);
        }

        lastPinchDist = currentDist;
        isDragging = false;  // Zoom не считаем drag-перемещением камеры
    }

    /// <summary>
    /// Обрабатывает drag камеры одним пальцем.
    /// </summary>
    private void HandlePan(Touch touch)
    {
        lastPinchDist = 0f;

        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPos = touch.screenPosition;
            isDragging = false;
            return;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.screenPosition - lastTouchPos;

            if (delta.sqrMagnitude > 0.1f)  // Игнорируем слишком мелкие шумовые движения пальца
            {
                isDragging = true;
                Move(delta);
            }

            lastTouchPos = touch.screenPosition;
            return;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            isDragging = false;
    }

    /// <summary>
    /// Перемещает виртуальную камеру
    /// Скорость pan масштабируется от текущего zoom,
    /// чтобы управление ощущалось естественно на разных уровнях приближения
    /// </summary>
    private void Move(Vector2 delta)
    {
        float zoomFactor = virtualCamera.m_Lens.OrthographicSize;
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed * zoomFactor;

        virtualCamera.transform.position += move;
    }
    /// <summary>
    /// Изменяет orthographic size камеры в допустимых пределах.
    /// </summary>
    private void Zoom(float delta)
    {
        float size = virtualCamera.m_Lens.OrthographicSize;
        size -= delta * zoomSpeed;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(size, minZoom, maxZoom);
    }
    /// <summary>
    /// Изменяет orthographic size камеры в допустимых пределах.
    /// </summary>
    private void ResetTouchState()
    {
        isDragging = false;
        lastPinchDist = 0f;
    }
}