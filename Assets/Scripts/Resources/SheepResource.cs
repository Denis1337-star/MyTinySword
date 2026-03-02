using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepResource : ResourceNodeBase
{
    [SerializeField] private int meatAmount = 2;
    [SerializeField] private float workTime = 2f;
    [SerializeField] private float respawnTime = 15f;
    public override Vector2 WorkPosition => transform.position;
    public override int Priority => 1;
    public override void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;

        if (TryGetComponent(out SheepAI ai))
            ai.SetFrozen(true);

        StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(workTime);

        callback?.Invoke(meatAmount);

        reservedBy = null;
        gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        reservedBy = null;

        if (TryGetComponent(out SheepAI ai))
            ai.SetFrozen(false);

        gameObject.SetActive(true);
    }
}
