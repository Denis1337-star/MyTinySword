using System;
using UnityEngine;

/// <summary>
/// Повесь на сцену уровня.
/// Задай Area (например Dangerous) с cost > 1 — рабочие/юниты будут обходить зону,
/// а поиск ресурса предпочтёт более безопасный путь.
///
/// Setup в Unity:
/// 1. Window → AI → Navigation → Areas → добавь Dangerous
/// 2. На NavMeshSurface / Modifier Volume выставь Area = Dangerous в зоне врагов
/// 3. Bake NavMesh
/// 4. На этом компоненте: Area Name = Dangerous, Cost = 8..15
/// </summary>
[DefaultExecutionOrder(-100)]
public sealed class NavMeshAreaCostBootstrap : MonoBehaviour
{
    [Serializable]
    public struct AreaCostEntry
    {
        public string AreaName;
        [Min(1f)] public float Cost;
    }

    [SerializeField]
    private AreaCostEntry[] _areaCosts =
    {
        new() { AreaName = "Dangerous", Cost = 10f }
    };

    private void Awake()
    {
        Apply();
    }

    private void Start()
    {
        // Агенты могли появиться после Awake — применяем ещё раз.
        NavMeshAreaCostService.ApplyToAllAgents();
    }

    public void Apply()
    {
        if (_areaCosts == null)
            return;

        for (int i = 0; i < _areaCosts.Length; i++)
        {
            AreaCostEntry entry = _areaCosts[i];
            if (string.IsNullOrWhiteSpace(entry.AreaName))
                continue;

            NavMeshAreaCostService.SetAreaCostByName(entry.AreaName, entry.Cost);
        }

        NavMeshAreaCostService.ApplyToAllAgents();
    }
}
