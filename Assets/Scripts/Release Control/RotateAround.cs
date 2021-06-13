using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    #region Inspector Parameters
    [SerializeField, Min(0)] private float minRandomSpeed;
    [SerializeField, Min(0)] private float maxRandomSpeed;
    [SerializeField, Min(1)] private bool rotateAroundX = true;
    [SerializeField, Min(1)] private bool rotateAroundY = true;
    [SerializeField, Min(1)] private bool rotateAroundZ = true;
    #endregion

    #region Private Parameters
    private float speed;
    private Vector2 direction;
    #endregion

    #region MonoBehavior
    void Start()
    {
        if (!rotateAroundX && !rotateAroundY && !rotateAroundZ)
            Destroy(this);
        speed = Random.Range(minRandomSpeed, maxRandomSpeed);
        direction = (new Vector3(
                    (rotateAroundX)?Random.Range(-10, 10):0,
                    (rotateAroundY)?Random.Range(-10, 10):0,
                    (rotateAroundZ)?Random.Range(-10, 10):0)).normalized;
    }

    void Update()
    {
        transform.Rotate(direction * speed);
    }
    #endregion
}
