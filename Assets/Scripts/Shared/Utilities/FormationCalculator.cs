using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Считает позиции юнитов для формации
/// </summary>
public static class FormationCalculator 
{
    /// <summary>
    /// Простая квадратная формация
    /// </summary>
    public static List<Vector2> GetSquareFormation(
        Vector2 center,
        int unitCount,
        float spacing
    )
    {
        List<Vector2> positions = new();

        int rowLength = Mathf.CeilToInt(Mathf.Sqrt(unitCount));

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / rowLength;
            int col = i % rowLength;

            Vector2 offset = new Vector2(
                (col - rowLength / 2f) * spacing,
                -(row * spacing)
            );

            positions.Add(center + offset);
        }

        return positions;
    }
}
