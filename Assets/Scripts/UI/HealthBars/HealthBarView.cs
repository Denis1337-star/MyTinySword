using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-представление полоски здоровья над объектом
/// Отвечает за позиционирование, цвет и заполнение полоски
/// </summary>
public class HealthBarView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;

    private Transform target;
    private Camera mainCamera;
    private Vector3 worldOffset;

    /// <summary>
    /// Привязывает полоску к цели и задаёт цвет заполнения
    /// </summary>
    public void Initialize(Transform target, Camera mainCamera, Color fillColor, Vector3 worldOffset)
    {
        this.target = target;
        this.mainCamera = mainCamera;
        this.worldOffset = worldOffset;

        if (fillImage != null)
            fillImage.color = fillColor;
    }

    /// <summary>
    /// Устанавливает текущее заполнение полоски
    /// </summary>
    public void SetFill(float normalizedValue)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
    }

    private void LateUpdate()
    {
        if (target == null || mainCamera == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 worldPosition = target.position + worldOffset;
        transform.position = mainCamera.WorldToScreenPoint(worldPosition);
    }
}
