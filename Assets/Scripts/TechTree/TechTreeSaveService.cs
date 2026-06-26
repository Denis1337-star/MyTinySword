using System.Collections.Generic;
using UnityEngine;
using YG;

/// <summary>
/// Сервис сохранения и прогресса дерева развития.
/// Работает через YG2.saves и учитывает оффлайн-время улучшений.
/// </summary>
public sealed class TechTreeSaveService
{
    private readonly TechTreeTimeService _timeService;

    public TechTreeSaveService(TechTreeTimeService timeService)
    {
        _timeService = timeService;
    }

    public TechTreeNodeSaveData GetOrCreateNode(TechTreeNodeConfig config)
    {
        EnsureInitialized();

        TechTreeNodeSaveData existingNode = FindNode(config.NodeId);

        if (existingNode != null)
            return existingNode;

        TechTreeNodeSaveData newNode = new()
        {
            NodeId = config.NodeId,
            Level = 0,
            UpgradeEndUnixTime = 0
        };

        YG2.saves.techTree.Nodes.Add(newNode);
        YandexSaveUtility.SaveProgress();

        return newNode;
    }

    public bool IsMaxLevel(TechTreeNodeConfig config)
    {
        TechTreeNodeSaveData node = GetOrCreateNode(config);
        return node.Level >= config.MaxLevel;
    }

    public bool AreRequirementsMet(TechTreeNodeConfig config)
    {
        TechTreeRequirement[] requirements = config.Requirements;

        if (requirements == null || requirements.Length == 0)
            return true;

        for (int i = 0; i < requirements.Length; i++)
        {
            TechTreeRequirement requirement = requirements[i];
            TechTreeNodeSaveData requiredNode = GetOrCreateNode(requirement.RequiredNode);

            if (requiredNode.Level < requirement.RequiredLevel)
                return false;
        }

        return true;
    }

    public bool CanStartUpgrade(TechTreeNodeConfig config)
    {
        TechTreeNodeSaveData node = GetOrCreateNode(config);
        if (node.IsUpgrading)
            return false;
        if (HasAnyActiveUpgrade())
            return false;
        if (node.Level >= config.MaxLevel)
            return false;
        return AreRequirementsMet(config);
    }
    public bool HasAnyActiveUpgrade()
    {
        EnsureInitialized();

        List<TechTreeNodeSaveData> nodes = YG2.saves.techTree.Nodes;

        if (nodes == null)
            return false;

        long currentUnixTime = _timeService.GetCurrentUnixTime();

        for (int i = 0; i < nodes.Count; i++)
        {
            TechTreeNodeSaveData node = nodes[i];

            if (node == null)
                continue;

            if (!node.IsUpgrading)
                continue;

            if (node.UpgradeEndUnixTime > currentUnixTime)
                return true;
        }

        return false;
    }

    public bool TryStartUpgrade(TechTreeNodeConfig config)
    {
        if (!CanStartUpgrade(config))
            return false;

        TechTreeNodeSaveData node = GetOrCreateNode(config);
        int upgradeSeconds = config.GetUpgradeSeconds(node.Level);

        if (upgradeSeconds <= 0)
        {
            Debug.LogError($"[TechTreeSaveService] Некорректное время улучшения у ноды {config.NodeId}.");
            return false;
        }

        long currentUnixTime = _timeService.GetCurrentUnixTime();

        node.UpgradeEndUnixTime = currentUnixTime + upgradeSeconds;

        _timeService.UpdateLastKnownTime();
        YandexSaveUtility.SaveProgress();

        return true;
    }

    public void CompleteReadyUpgrades(IReadOnlyList<TechTreeNodeConfig> configs)
    {
        EnsureInitialized();

        bool changed = false;
        long currentUnixTime = _timeService.GetCurrentUnixTime();

        for (int i = 0; i < configs.Count; i++)
        {
            TechTreeNodeConfig config = configs[i];
            TechTreeNodeSaveData node = GetOrCreateNode(config);

            if (!node.IsUpgrading)
                continue;

            if (node.UpgradeEndUnixTime > currentUnixTime)
                continue;

            node.UpgradeEndUnixTime = 0;
            node.Level = Mathf.Min(node.Level + 1, config.MaxLevel);
            changed = true;
        }

        if (!changed)
            return;

        _timeService.UpdateLastKnownTime();
        YandexSaveUtility.SaveProgress();
    }

    public long GetRemainingSeconds(TechTreeNodeConfig config)
    {
        TechTreeNodeSaveData node = GetOrCreateNode(config);
        if (!node.IsUpgrading)
            return 0;
        long currentUnixTime = _timeService.GetCurrentUnixTime();
        long remainingSeconds = node.UpgradeEndUnixTime - currentUnixTime;
        if (remainingSeconds < 0)
            return 0;
        return remainingSeconds;
    }

    public TechTreeNodeState GetNodeState(TechTreeNodeConfig config)
    {
        TechTreeNodeSaveData node = GetOrCreateNode(config);

        if (node.IsUpgrading)
            return TechTreeNodeState.Upgrading;

        if (node.Level >= config.MaxLevel)
            return TechTreeNodeState.Maxed;

        if (AreRequirementsMet(config))
            return TechTreeNodeState.Available;

        return TechTreeNodeState.Locked;
    }

    private void EnsureInitialized()
    {
        if (YG2.saves.techTree == null)
            YG2.saves.techTree = new TechTreeSaveData();

        if (YG2.saves.techTree.Initialized)
            return;

        YG2.saves.techTree.Initialized = true;
        YG2.saves.techTree.Nodes ??= new List<TechTreeNodeSaveData>();
        YG2.saves.techTree.LastKnownUnixTime = _timeService.GetCurrentUnixTime();

        YandexSaveUtility.SaveProgress();

        Debug.Log("[TechTreeSaveService] Сохранения дерева развития инициализированы.");
    }

    private TechTreeNodeSaveData FindNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return null;

        List<TechTreeNodeSaveData> nodes = YG2.saves.techTree.Nodes;

        if (nodes == null)
            return null;

        for (int i = 0; i < nodes.Count; i++)
        {
            TechTreeNodeSaveData node = nodes[i];

            if (node != null && node.NodeId == nodeId)
                return node;
        }

        return null;
    }
}