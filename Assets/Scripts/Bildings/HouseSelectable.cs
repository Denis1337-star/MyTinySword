using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(House))]
public class HouseSelectable : MonoBehaviour
{
    private House house;

    private void Awake()
    {
        house = GetComponent<House>();

        if (house == null)
            Debug.LogError($"HouseSelectable: не найден House на объекте {name}", this);
    }
    public House GetHouse()
    {
        return house;
    }
}
