using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

    [Header("Raycast")]
    [SerializeField] private LayerMask ignoreRaycastLayer;

    private Camera cam;
    private UnitSelectable currentSelection;  //текущий выделеный юнит
    private CameraFocusController focusController;


    private readonly List<UnitSelectable> selectedUnits = new();  //список всех выделеных юнитов

    private void Awake()
    {
        cam = Camera.main;
        focusController = GameServices.Instance.GetComponent<CameraFocusController>();
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Touch.activeTouches.Count == 0)  //если косаний нет = выход
            return;

        var touch = Touch.activeTouches[0];  //первое касание

        if (touch.phase != TouchPhase.Ended) //обработка только завершеных косаний
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))  //проверка если находтся над UI = выход 
            return;
         
        ProcessTap(touch.screenPosition);  //передает позицию косания
    }

    private void ProcessTap(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);  //координат косания в мировые Unity
        int mask = ~ignoreRaycastLayer.value; //инвертируем маску 

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, mask);  //луч в точку косания с дистанцией в 100f

        if (hit.collider != null)  //если попал в колайдер
        {
            //и это  Worker
            if (hit.collider.TryGetComponent(out UnitSelectable selectable))
            {
                Select(selectable);
                return;
            }

            // и это House
            if (hit.collider.TryGetComponent(out HouseSelectable houseSelectable))
            {
                housePanel.Show(houseSelectable.GetHouse());
                return;
            }
        }

        ClearSelection();  //если не один обьект не выделен (тап в пустое место_ сбрасывает)
    }

    private void Select(UnitSelectable selectable)
    {
        if (currentSelection == selectable)  //если уже ввыделен = выход
            return;

        ClearSelection();  //сброс предыдущее выделение

        currentSelection = selectable;  //новый выделеный обьект
        selectable.Select();  

        selectedUnits.Add(selectable);   //добавляем в список 

        if (selectable.TryGetComponent(out Worker worker))  //если выделеный обьект = рабочий
        {
            workerCommandPanel.ShowForWorker(worker);  //открывает панель
            focusController?.FocusOn(worker.transform);  //фокусируем камеру
            return;
        }

        if (selectable.TryGetComponent(out House house))  //если дом
        {
            housePanel.Show(house);  //открывает панель дома
            return;
        }
    }

    public void SelectWorkerFromUI(Worker worker)  //выбрать рабочего из UI
    {
        if (worker == null)
            return;

        if (worker.TryGetComponent(out UnitSelectable selectable))  //получаем компонент у рабочего 
            Select(selectable);  //вызов метода
    }

    public void ClearSelection()
    {
        foreach (var unit in selectedUnits)  //снимает выделение с всех юнитов
            unit.Deselect();

        selectedUnits.Clear();  //оищает список
        currentSelection = null;  //текущее выделение очищает

        workerCommandPanel.Hide();  //скрывает панели
        housePanel.Hide();

        focusController?.CancelFocus(); //отмета фокуса камеры
    }

 
    public IReadOnlyList<UnitSelectable> GetSelectedUnits()  //возрощает неизменяемый список выделеных юнитов 
    {
        return selectedUnits;
    }
}

