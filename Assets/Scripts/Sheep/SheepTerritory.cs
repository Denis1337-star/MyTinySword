using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepTerritory : MonoBehaviour
{
    public Vector2 GetRandomPoint()
    {
        var box = GetComponent<BoxCollider2D>();
        var bounds = box.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}

