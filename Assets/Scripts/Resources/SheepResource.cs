using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepResource : MonoBehaviour,IResourceNode
{

    [SerializeField] private int meatAmount = 2;
    [SerializeField] private float workTime = 2f;
    [SerializeField] private float respawnTime = 15f;

    private bool available = true;

    public Vector2 WorkPosition => transform.position;
    public bool IsAvailable => available;

    public int Priority => 1;

    public void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;

        if (TryGetComponent(out SheepAI ai))
            ai.enabled = false;

        StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(workTime);

        callback?.Invoke(meatAmount);

        gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;

        if (TryGetComponent(out SheepAI ai))
            ai.enabled = true;

        gameObject.SetActive(true);
    }
}
