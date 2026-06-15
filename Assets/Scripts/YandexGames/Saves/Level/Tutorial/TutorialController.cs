using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет простым пошаговым обучением.
/// Первый этап: tutorial переключается кнопкой "Далее".
/// Позже шаги будут ждать реальные действия игрока.
/// </summary>
public sealed class TutorialController : ValidatedMonoBehaviour
{
    [Header("Level")]
    [SerializeField] private bool _runOnlyOnTutorialLevel = true;

    [Header("UI")]
    [SerializeField] private TutorialPanel _panel;

    [Header("Steps")]
    [SerializeField, TextArea]
    private List<string> _stepMessages = new()
    {
        "Добро пожаловать! Это обучение поможет понять основы игры.",
        "Нажми на дом. В доме живут рабочие, которые добывают ресурсы.",
        "В доме можно назначить всех рабочих на дерево, еду или золото.",
        "Строй здания, чтобы развивать базу и нанимать армию.",
        "Найми юнитов и выбери армию.",
        "Нажми по врагу, чтобы атаковать. Цель — уничтожить вражеский замок."
    };

    private TutorialSaveService _tutorialSaveService;
    private LevelRuntimeService _levelRuntimeService;

    private int _currentStepIndex;
    private bool _isRunning;

    [Inject]
    private void Construct(
        TutorialSaveService tutorialSaveService,
        LevelRuntimeService levelRuntimeService)
    {
        _tutorialSaveService = tutorialSaveService;
        _levelRuntimeService = levelRuntimeService;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        _panel.Hide();
    }

    private void OnEnable()
    {
        _panel.NextButton.onClick.AddListener(GoToNextStep);
        _panel.SkipButton.onClick.AddListener(SkipTutorial);
    }

    private void Start()
    {
        TryStartTutorial();
    }

    private void OnDisable()
    {
        _panel.NextButton.onClick.RemoveListener(GoToNextStep);
        _panel.SkipButton.onClick.RemoveListener(SkipTutorial);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _panel, nameof(_panel));

        if (_stepMessages == null || _stepMessages.Count == 0)
        {
            Debug.LogError($"{name}: список шагов обучения пуст.", this);
            valid = false;
        }

        return valid;
    }

    private void TryStartTutorial()
    {
        if (_tutorialSaveService.IsTutorialCompleted())
        {
            _panel.Hide();
            return;
        }

        if (_runOnlyOnTutorialLevel && !IsCurrentLevelTutorial())
        {
            _panel.Hide();
            return;
        }

        StartTutorial();
    }

    private bool IsCurrentLevelTutorial()
    {
        // Если Level_1 запущен напрямую из Editor, LevelRuntimeService может быть пустой.
        // В этом случае разрешаем tutorial, чтобы удобно тестировать сцену.
        if (_levelRuntimeService == null || !_levelRuntimeService.HasCurrentLevel)
            return true;

        return _levelRuntimeService.CurrentLevel.IsTutorialLevel;
    }

    private void StartTutorial()
    {
        if (_stepMessages == null || _stepMessages.Count == 0)
            return;

        _isRunning = true;
        _currentStepIndex = 0;

        ShowCurrentStep();
    }

    private void GoToNextStep()
    {
        if (!_isRunning)
            return;

        _currentStepIndex++;

        if (_currentStepIndex >= _stepMessages.Count)
        {
            CompleteTutorial();
            return;
        }

        ShowCurrentStep();
    }

    private void SkipTutorial()
    {
        if (!_isRunning)
            return;

        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        _isRunning = false;

        _tutorialSaveService.MarkTutorialCompleted();
        _panel.Hide();
    }

    private void ShowCurrentStep()
    {
        string message = _stepMessages[_currentStepIndex];

        _panel.ShowStep(
            message,
            _currentStepIndex,
            _stepMessages.Count);
    }
}