using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MandalaParticle : MonoBehaviour
{
    public enum RotationType { None, PositiveDirection, NegativeDirection };
    public enum Direction { X, Y, Z };

    public Direction lookAxis;
    public Direction rotationAxis;
    public RotationType elementsRotation;
    public float elementsRotationSpeed;
    public float minElementSize = 1;
    public float maxElementSize = 1;
    public float WhenOpenNext;
    private Vector3 targetSize;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public MandalaRing ring;

    public void SetPosition(float t)
    {
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetSize, t);
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.up * elementsRotationSpeed * Time.deltaTime, Space.Self);
    }
    public void SetTargetSize(float size)
    {
        targetSize = Vector3.one * size;
    }
}
