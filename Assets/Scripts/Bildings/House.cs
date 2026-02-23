using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Transform dropPoint;
    public Vector2 DropPoint => dropPoint.position;
}
