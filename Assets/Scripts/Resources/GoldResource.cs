using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceSize
{
    Tiny = 1,
    Small = 2,
    Medium = 3,
    Large = 4,
    Huge = 5,
    Giant = 6
}
public class GoldResource : MonoBehaviour, IResourceNode
{
    [Header("Positions")]
    [SerializeField] private Transform workPoint;

    [Header("Timing")]
    [SerializeField] private float mineTime = 3f;
    [SerializeField] private float respawnTime = 15f;
    [SerializeField] private float growInterval = 5f;

    [Header("Visuals")]
    [SerializeField] private Sprite[] sizeSprites; // 0 = Tiny ... 5 = Giant
    [SerializeField] private Animator animator;

    private SpriteRenderer sr;
    private bool available = true;

    private ResourceSize size = ResourceSize.Tiny;

    public bool IsAvailable => available;
    public Vector2 WorkPosition => workPoint.position;
    public ResourceSize Size => size;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();
        InvokeRepeating(nameof(Grow), growInterval, growInterval);
    }

    public void StartWork(System.Action onFinished)
    {
        if (!available)
            return;

        available = false;
        CancelInvoke(nameof(Grow));

        Invoke(nameof(FinishMining), mineTime);

        void FinishMining()
        {
            sr.enabled = false; // камень исчез
            onFinished?.Invoke();
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    private void Respawn()
    {
        size = ResourceSize.Tiny;
        available = true;
        sr.enabled = true;
        UpdateVisual();
        InvokeRepeating(nameof(Grow), growInterval, growInterval);
    }

    private void Grow()
    {
        if (!available)
            return;

        if (size < ResourceSize.Giant)
        {
            size++;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        int index = (int)size - 1;
        sr.sprite = sizeSprites[index];
        animator.SetInteger("Size", index);
    }
}
