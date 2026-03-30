using UnityEngine;

/// <summary>
/// √лобальный контейнер ссылок на основные сервисы сцены
/// ѕозвол€ет централизованно получать доступ к выбору, ресурсам,
/// реестрам, управлению камерой и главной камере
/// </summary>
public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private SelectionSystem selection;
    [SerializeField] private ResourceStorage resources;
    [SerializeField] private WorkerRegistry workers;
    [SerializeField] private ResourceRegistry resourceNodes;
    [SerializeField] private CameraFocusController cameraFocus;
    [SerializeField] private Camera mainCamera;

    public SelectionSystem Selection => selection;
    public ResourceStorage Resources => resources;
    public WorkerRegistry Workers => workers;
    public ResourceRegistry ResourceNodes => resourceNodes;
    public CameraFocusController CameraFocus => cameraFocus;
    public Camera MainCamera => mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveMissingReferences();
    }

    private void OnValidate()
    {
        ResolveMissingReferences();
    }

    /// <summary>
    /// Ќаходит и заполн€ет отсутствующие сценовые ссылки
    /// ≈сли ссылка уже назначена вручную, повторно еЄ не трогаем
    /// </summary>
    private void ResolveMissingReferences()
    {
        if (selection == null)
            selection = FindObjectOfType<SelectionSystem>(true);

        if (resources == null)
            resources = FindObjectOfType<ResourceStorage>(true);

        if (workers == null)
            workers = FindObjectOfType<WorkerRegistry>(true);

        if (resourceNodes == null)
            resourceNodes = FindObjectOfType<ResourceRegistry>(true);

        if (cameraFocus == null)
            cameraFocus = FindObjectOfType<CameraFocusController>(true);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
}
