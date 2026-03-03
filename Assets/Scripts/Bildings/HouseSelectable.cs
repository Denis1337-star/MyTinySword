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
    }

    public House GetHouse()
    {
        return house;
    }
}
