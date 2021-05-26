using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public Func<int, Vector3> CalculatePos;
    public int i;
    public float radius;
    public float angle;
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, CalculatePos.Invoke(i), Time.deltaTime);
    }
}
