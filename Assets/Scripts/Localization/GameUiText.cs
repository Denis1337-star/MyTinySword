using System.Collections.Generic;

/// <summary>
/// Единый каталог UI-строк игры: RU/EN для динамических панелей и статичных подписей.
/// Контент из ScriptableObject (здания, юниты, ноды, уровни) хранится в самих конфигах.
/// </summary>
public static class GameUiText
{
    public static string Victory => Lang.Pick("ПОБЕДА", "VICTORY");

    public static string FormatMinutesSeconds(int totalSeconds)
    {
        if (totalSeconds < 0)
            totalSeconds = 0;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return Lang.Pick($"{minutes:00}м {seconds:00}сек", $"{minutes:00}m {seconds:00}s");
    }

    public static string SpeedBoostWatchAd => Lang.Pick("Реклама", "Watch Ad");

    public static string GameSpeedStatus(bool isDoubleSpeed) =>
        isDoubleSpeed
            ? Lang.Pick("Скорость игры 2x", "Game speed 2x")
            : Lang.Pick("Скорость игры 1x", "Game speed 1x");

    public static string Next => Lang.Pick("Далее", "Next");
    public static string Restart => Lang.Pick("Заново", "Restart");
    public static string MainMenu => Lang.Pick("В меню", "Main Menu");
    public static string Back => Lang.Pick("Назад", "Back");
    public static string Close => Lang.Pick("Выйти", "Close");
    public static string Ok => Lang.Pick(OkRu, OkEn);
    public const string OkRu = "Понятно";
    public const string OkEn = "OK";

    public static string Play => Lang.Pick("Играть", "Play");
    public static string Levels => Lang.Pick("Уровни", "Levels");
    public static string Settings => Lang.Pick("Настройки", "Settings");
    public static string SkillTree => Lang.Pick("Дерево развития", "Skill Tree");
    public static string Music => Lang.Pick("Музыка", "Music");
    public static string Sounds => Lang.Pick("Звуки", "Sounds");

    public static string Build => Lang.Pick("Построить", "Build");
    public static string Hire => Lang.Pick("Нанять", "Hire");
    public static string HireWorker => Lang.Pick("Нанять рабочего", "Hire Worker");
    public static string Demolish => Lang.Pick("Снести", "Demolish");
    public static string YesDemolish => Lang.Pick("Да, снести", "Yes, demolish");
    public static string SelectAll => Lang.Pick("Выбрать\nвсех", "Select\nall");

    public static string AllToWood => Lang.Pick("Все на дерево", "All to Wood");
    public static string AllToGold => Lang.Pick("Все на золото", "All to Gold");
    public static string AllToMeat => Lang.Pick("Все на охоту", "All to Meat");

    public static string House => Lang.Pick("Дом", "House");
    public static string Warrior => Lang.Pick("Воин", "Warrior");
    public static string No => Lang.Pick("Нет", "None");

    public static string CostDash => Lang.Pick("Стоимость: -", "Cost: -");
    public static string BuildDash => Lang.Pick("Постройка: -", "Build: -");
    public static string BuildingNotSelected => Lang.Pick("Здание не выбрано", "No building selected");
    public static string SlotNotSelected => Lang.Pick("Слот не выбран", "No slot selected");

    public static string CanDemolishBuilding => Lang.Pick("Это здание можно снести.", "This building can be demolished.");
    public static string CannotDemolishBuilding => Lang.Pick("Это здание нельзя снести.", "This building cannot be demolished.");

    public static string DemolishConfirmNoRefund =>
        Lang.Pick(
            "Вы уверены, что хотите снести здание?\nРесурсы не будут возвращены.",
            "Are you sure you want to demolish this building?\nResources will not be refunded.");

    public static string TutorialDemolishBlocked =>
        Lang.Pick(
            "Сейчас обучение не разрешает снести здание.",
            "The tutorial does not allow demolishing buildings right now.");

    public static string Upgrade => Lang.Pick("Улучшить", "Upgrade");
    public static string Upgrading => Lang.Pick("Улучшается", "Upgrading");
    public static string AnotherUpgradeInProgress => Lang.Pick("Уже идёт улучшение", "Another upgrade in progress");
    public static string RequirementsTitle => Lang.Pick("Надо прокачать", "Requirements");
    public static string NoRequirements => Lang.Pick("Требований нет", "No requirements");

    public static string CurrentBonus => Lang.Pick("Текущий бонус", "Current bonus");
    public static string NextBonus => Lang.Pick("Следующий бонус", "Next bonus");
    public static string CurrentLevel => Lang.Pick("Текущий уровень", "Current level");
    public static string NextLevel => Lang.Pick("Следующий уровень", "Next level");
    public static string UpgradeTime => Lang.Pick("Время улучшения", "Upgrade time");

    public static string LevelNotSet => Lang.Pick("Уровень не задан", "Level not set");
    public static string Error => Lang.Pick("Ошибка", "Error");
    public static string Locked => Lang.Pick("Закрыт", "Locked");
    public static string Completed => Lang.Pick("Пройден", "Completed");
    public static string Available => Lang.Pick("Доступен", "Available");

    public static string Workers(int current, int max) =>
        Lang.Pick($"Рабочие: {current}/{max}", $"Workers: {current}/{max}");

    public static string WorkersEmpty => Workers(0, 0);

    public static string HireCost(int wood, int gold) =>
        Lang.Pick(
            $"Стоимость: дерево {wood} / золото {gold}",
            $"Cost: wood {wood} / gold {gold}");

    public static string UnitCost(int wood, int meat) =>
        Lang.Pick(
            $"Стоимость: дерево {wood} / мясо {meat}",
            $"Cost: wood {wood} / meat {meat}");

    public static string BuildTime(float seconds) =>
        Lang.Pick(
            $"Постройка: {seconds:0.#} секунд",
            $"Build time: {seconds:0.#} sec");

    public static string BuildCostDetails(int wood, int woodCost, int gold, int goldCost) =>
        Lang.Pick(
            $"Стоимость\nДерево: {wood}/{woodCost}\nЗолото: {gold}/{goldCost}",
            $"Cost\nWood: {wood}/{woodCost}\nGold: {gold}/{goldCost}");

    public static string Limit(int current, int allowed) =>
        Lang.Pick($"Лимит: {current}/{allowed}", $"Limit: {current}/{allowed}");

    public static string QueueInfo(int queueCount, int maxQueue, int armySlots, int maxArmySlots, float buildTime) =>
        Lang.Pick(
            $"В очереди: {queueCount}/{maxQueue}  Армия: {armySlots}/{maxArmySlots}\nОбучение: {buildTime:0.#} сек.",
            $"Queue: {queueCount}/{maxQueue}  Army: {armySlots}/{maxArmySlots}\nTraining: {buildTime:0.#} sec.");

    public static string CurrentJob(string jobName) =>
        Lang.Pick($"Текущая работа: {jobName}", $"Current job: {jobName}");

    public static string CurrentJobNone => CurrentJob(No);

    public static string NextJob(string jobName) =>
        Lang.Pick($"Следующая работа: {jobName}", $"Next job: {jobName}");

    public static string NextJobNone => NextJob(No);

    public static string WorkerJobLine(string currentJob, string pendingJob = null)
    {
        if (string.IsNullOrEmpty(pendingJob))
            return Lang.Pick($"Работа: {currentJob}", $"Job: {currentJob}");

        return Lang.Pick(
            $"Работа: {currentJob} → {pendingJob}",
            $"Job: {currentJob} → {pendingJob}");
    }

    public static string BonusPreview(string current, string next) =>
        $"{CurrentBonus}: {current}\n{NextBonus}: {next}";

    public static string DemolishConfirmWithRefund(int woodRefund, int goldRefund) =>
        Lang.Pick(
            "Вы уверены, что хотите снести здание?\nПри сносе будет возвращено:\n" +
            $"Дерево: {woodRefund}\nЗолото: {goldRefund}",
            "Are you sure you want to demolish this building?\nRefund on demolition:\n" +
            $"Wood: {woodRefund}\nGold: {goldRefund}");

    public static string Wood => Lang.Pick("Дерево", "Wood");
    public static string Gold => Lang.Pick("Золото", "Gold");
    public static string Meat => Lang.Pick("Мясо", "Meat");
    public static string GatherObjectiveProgress(ResourceType resourceType, int current, int target)
    {
        string resourceName = resourceType switch
        {
            ResourceType.Wood => Wood,
            ResourceType.Gold => Gold,
            ResourceType.Meat => Meat,
            _ => resourceType.ToString(),
        };
        return Lang.Pick($"{resourceName}:{current}/{target}",
            $"{resourceName}:{current}/{target}");
    }

    public static string BlockBuildingNotSelected => BuildingNotSelected;
    public static string BlockAlreadyBuilding => Lang.Pick("Уже строится", "Already building");
    public static string BlockLimitReached => Lang.Pick("Лимит достигнут", "Limit reached");
    public static string BlockNotEnoughResources => Lang.Pick("Не хватает ресурсов", "Not enough resources");
    public static string BlockQueueFull => Lang.Pick("Очередь заполнена", "Queue is full");
    public static string BlockArmyLimit => Lang.Pick("Достигнут лимит армии", "Army limit reached");
    public static string BlockWorkerLimit => Lang.Pick("Достигнут лимит рабочих", "Worker limit reached");

    public static string Health(int value) => Lang.Pick($"Здоровье: {value}", $"Health: {value}");
    public static string Speed(float value) => Lang.Pick($"Скорость: {value}", $"Speed: {value}");
    public static string Vision(float value) => Lang.Pick($"Обзор: {value}", $"Vision: {value}");
    public static string Damage(int value) => Lang.Pick($"Урон: {value}", $"Damage: {value}");
    public static string AttackRange(float value) => Lang.Pick($"Дистанция атаки: {value}", $"Attack range: {value}");
    public static string Heal(int value) => Lang.Pick($"Лечение: {value}", $"Heal: {value}");

    /// <summary>
    /// Стартовые подсказки InfoPanel на Level_2..Level_5.
    /// </summary>
    public static bool TryGetLevelInfoMessage(string sceneName, out string ru, out string en)
    {
        switch (sceneName)
        {
            case "Level_2":
                ru = "На этом уровне золотых руд намного меньше, жди когда они станут больше и награда за их добычу порадует тебя";
                en = "On this level there are far fewer gold mines. Wait until they grow larger — the reward for mining them will be worth it.";
                return true;

            case "Level_3":
                ru = "На этом уровне враги контролируют ущелье и большое пастбище овец. Следи за своими рабочими, чтобы они не заходили слишком далеко";
                en = "On this level enemies control the gorge and a large sheep pasture. Watch your workers so they don't go too far.";
                return true;

            case "Level_4":
                ru = "На этом уровне очень много врагов, но ты не ограничен в ресурсах";
                en = "On this level there are many enemies, but you are not limited in resources.";
                return true;

            case "Level_5":
                ru = "На этом уровне, ты сильно ограничен в золоте, поэтому я дал тебе 200 на старте";
                en = "On this level you are severely limited in gold, so I gave you 200 at the start.";
                return true;
            case "Level_6":
                ru = "Собери нужное количество дерева. Рядом с базой лес безопасный, большой лес охраняют патрули — береги рабочих.";
                en = "Gather the required amount of wood. The forest near your base is safe; the large forest is guarded by patrols — protect your workers.";
                return true;

            default:
                ru = null;
                en = null;
                return false;
        }
    }

    /// <summary>
    /// Словарь для SceneUiLocalizer: исходный RU-текст кнопки → EN.
    /// </summary>
    private static readonly Dictionary<string, string> StaticSceneTextEn = new()
    {
        ["Играть"] = "Play",
        ["Уровни"] = "Levels",
        ["Настройки"] = "Settings",
        ["Дерево развития"] = "Skill Tree",
        ["Далее"] = "Next",
        ["В меню"] = "Main Menu",
        ["Заново"] = "Restart",
        ["ПОБЕДА"] = "VICTORY",
        ["Победа"] = "Victory",
        ["Назад"] = "Back",
        ["Построить"] = "Build",
        ["Нанять"] = "Hire",
        ["Нанять рабочего"] = "Hire Worker",
        ["Снести"] = "Demolish",
        ["Cнести"] = "Demolish",
        ["Уверен"] = "Confirm",
        ["Дом"] = "House",
        ["Воин"] = "Warrior",
        ["Все на дерево"] = "All to Wood",
        ["Все на золото"] = "All to Gold",
        ["Все на охоту"] = "All to Meat",
        ["Выбрать\nвсех"] = "Select\nall",
        ["Выбрать  всех"] = "Select all",
        ["Музыка"] = "Music",
        ["Звуки"] = "Sounds",
        ["Понятно"] = "OK",
        ["Нет"] = "None",
        ["Улучшить"] = "Upgrade",
        ["Надо прокачать"] = "Requirements",
        ["Надо прокачать:"] = "Requirements:",
        ["Текущий уровень"] = "Current level",
        ["Следующий уровень"] = "Next level",
        ["Время улучшения"] = "Upgrade time",
        ["Текущая работа"] = "Current job",
        ["Следующая работа"] = "Next job",
        ["Старт: дерево"] = "Start: wood",
        ["Позволяет нанять"] = "Allows hiring",
        ["строить"] = "build",
        ["Русский"] = "Русский",
        ["English"] = "English",
    };

    public static bool TryGetStaticSceneEnglish(string russianText, out string englishText)
    {
        if (string.IsNullOrWhiteSpace(russianText))
        {
            englishText = null;
            return false;
        }

        return StaticSceneTextEn.TryGetValue(russianText.Trim(), out englishText);
    }
}
