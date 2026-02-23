using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Отвечает ТОЛЬКО за движение юнита
/// Никакого input, никакого selection
/// </summary>
public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;  //Куда должен двигаться
    private bool hasTarget;  //флаг если ли цель куда идти
    public bool HasTarget => hasTarget;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (!hasTarget)
            return;

        Vector2 currentPos = rb.position; //получаем координаты текущие 
        Vector2 newPos = Vector2.MoveTowards(
            currentPos,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime  //двигаем обьект к цели
        );

        rb.MovePosition(newPos);  //перемещение в новую позицию 

        if (Vector2.Distance(currentPos, targetPosition) < 0.05f)  //проверка достиг ли цели
        {
            hasTarget = false;
        }
    }

    public void MoveTo(Vector2 position)  //задает новую цель движения
    {
        targetPosition = position;    //сохраняет точку
        hasTarget = true;  //флаг что имеет цель
    }
}
