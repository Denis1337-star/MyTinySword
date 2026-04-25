using UnityEngine;

/// <summary>
/// Глобальный контейнер ссылок на основные системы сцены
/// </summary>
public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private ResourceStorage resourcesStorage;
    [SerializeField] private WorkerRegistry workerRegistry;
    [SerializeField] private ResourceRegistry resourceRegistry;
    [SerializeField] private CameraFocusController cameraFocusController;
    [SerializeField] private Camera mainCamera;

    public SelectionSystem SelectionSystem => selectionSystem;
    public ResourceStorage ResourcesStorage => resourcesStorage;
    public WorkerRegistry WorkerRegistry => workerRegistry;
    public ResourceRegistry ResourceRegistry => resourceRegistry;
    public CameraFocusController CameraFocusController => cameraFocusController;
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
    /// Находит и заполняет отсутствующие сценовые ссылки
    /// </summary>
    private void ResolveMissingReferences()
    {
        if (selectionSystem == null)
            selectionSystem = FindObjectOfType<SelectionSystem>(true);

        if (resourcesStorage == null)
            resourcesStorage = FindObjectOfType<ResourceStorage>(true);

        if (workerRegistry == null)
            workerRegistry = FindObjectOfType<WorkerRegistry>(true);

        if (resourceRegistry == null)
            resourceRegistry = FindObjectOfType<ResourceRegistry>(true);

        if (cameraFocusController == null)
            cameraFocusController = FindObjectOfType<CameraFocusController>(true);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
}
