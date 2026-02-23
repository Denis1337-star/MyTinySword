using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour, IResourceNode
{
    [SerializeField] private Transform workPoint;
    [SerializeField] private float chopTime = 2f;
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private Sprite treeSprite;
    [SerializeField] private Sprite stumpSprite;

    private SpriteRenderer sr;
    private bool available = true;

    public bool IsAvailable => available;
    public Vector2 WorkPosition => workPoint.position;
    public ResourceSize Size => ResourceSize.Small; // дерево всегда +1

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = treeSprite;
    }

    public void StartWork(Action onFinished)
    {
        if (!available)
            return;

        available = false;

        Invoke(nameof(FinishChop), chopTime);

        void FinishChop()
        {
            sr.sprite = stumpSprite;
            onFinished?.Invoke();
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    private void Respawn()
    {
        available = true;
        sr.sprite = treeSprite;
    }
}

