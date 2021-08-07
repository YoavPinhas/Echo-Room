using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MandalaParticles
{
    private enum RotationType { None, PositiveDirection, NegativeDirection};
    private enum Direction { X, Y, Z };

    #region Inspector Parameters
    [Header("Global Properties")]
    [SerializeField] private GameObject shapePrefab;
    [SerializeField] private int numberOfCircles;
    [SerializeField] private int elementsInCircle;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private RotationType ringRotation;
    [SerializeField] private float secondsForCycle;
    [SerializeField] private float secondsFromCloseToOpen;
    [SerializeField] private AnimationCurve openCurve;
    [Header("Elements Properties")]
    [SerializeField] private Direction lookAxis;
    [SerializeField] private Direction rotationAxis;
    [SerializeField] private RotationType elementsRotation;
    [SerializeField] private float elementsRotationSpeed;
    [SerializeField, Min(0.5f)] private float minElementSize = 1;
    [SerializeField, Min(0.5f)] private float maxElementSize = 1;
    [SerializeField, Range(0, 1)] public float WhenOpenNext;
    #endregion

    #region Private Parameters
    private GameObject[] particles;
    private float[] radiuses;
    private float[] currentRadiuses;
    private float[] sizes;
    private float[] currentSizes;
    private float angleBetweenParticles;
    private Vector3 particlesRotationDirection;
    private Vector3 rotationDirection;
    private GameObject circle;
    public bool hasWhereToOpen { get; private set; } = false;
    #endregion

    #region Particles Update Methods
    public void Init(int index, Transform parent)
    {
        
        InitRotation();
        particles = new GameObject[numberOfCircles * elementsInCircle];
        circle = GameObject.Instantiate(new GameObject(), parent);
        
        circle.name = $"Mandala Particles #{index}";
    
        angleBetweenParticles = 2 * Mathf.PI / elementsInCircle;
        var axis = (lookAxis == Direction.X) ? Vector3.right :
                    (lookAxis == Direction.Y) ? Vector3.up :
                    Vector3.forward;
        for (int i = 0; i < numberOfCircles * elementsInCircle; i++)
        {
            particles[i] = GameObject.Instantiate(shapePrefab, circle.transform);
            particles[i].transform.localScale = Vector3.zero;
            float angle = (i % elementsInCircle) * angleBetweenParticles;
            if ((i / elementsInCircle) % 2 == 1)
            {
                angle += 0.5f * angleBetweenParticles;
            }
            Vector3 dir = (parent.position - new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)).normalized;
            particles[i].transform.LookAt(particles[i].transform.position + dir, axis);
        }
        InitParticlesRotation();
        radiuses = new float[numberOfCircles];
        currentRadiuses = new float[numberOfCircles];
        sizes = new float[numberOfCircles];
        currentSizes = new float[numberOfCircles];
        float scaleMagnitude = shapePrefab.transform.localScale.magnitude;
        minElementSize *= scaleMagnitude;
        maxElementSize *= scaleMagnitude;
        for (int i = 0; i < numberOfCircles; i++)
        {
            radiuses[i] = Mathf.Lerp(minRadius, maxRadius, ((float)i)/numberOfCircles);
            sizes[i] = Mathf.Lerp(minElementSize, maxElementSize, ((float)i) / numberOfCircles);
        }

    }
    private void InitParticlesRotation()
    {
        if (elementsRotation == RotationType.None)
        {
            particlesRotationDirection = Vector3.zero;
        }
        else
        {
            switch (rotationAxis)
            {
                case Direction.X:
                    particlesRotationDirection = Vector3.right;
                    break;
                case Direction.Y:
                    particlesRotationDirection = Vector3.up;
                    break;
                case Direction.Z:
                    particlesRotationDirection = Vector3.forward;
                    break;
            }
            particlesRotationDirection *= (elementsRotation == RotationType.PositiveDirection) ? elementsRotationSpeed : -elementsRotationSpeed;
        }
    }
    private void InitRotation()
    {
        if (ringRotation == RotationType.None)
            rotationDirection = Vector3.zero;
        else if (ringRotation == RotationType.PositiveDirection)
            rotationDirection = secondsForCycle * Vector3.forward;
        else if (ringRotation == RotationType.NegativeDirection)
            rotationDirection = -secondsForCycle * Vector3.forward;
    }
    public void SetPositions(float t)
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            currentRadiuses[i] = Mathf.Lerp(minRadius, radiuses[i], t);
            currentSizes[i] = Mathf.Lerp(0, sizes[i], t);
        }
        for (int i = 0; i < particles.Length; i++)
        {
            SetPosition(i, t);
        }
        hasWhereToOpen = t == 1;
    }
    private void SetPosition(int i, float t)
    {
        float angle = (i%elementsInCircle) * angleBetweenParticles;
        if((i / elementsInCircle) % 2 == 1)
        {
            angle += 0.5f * angleBetweenParticles;
        }
       
        var radius = currentRadiuses[i / elementsInCircle];
        particles[i].transform.localPosition = radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
      //  Debug.Log($"{circle.name} size = {currentSizes.Length}\ti={i}\ti/size = {i/elementsInCircle}");
        
        particles[i].transform.localScale = currentSizes[i / elementsInCircle] * Vector3.one;
    }
    public void RotateParticle(float delta)
    {
        foreach(var particle in particles)
        {
            particle.transform.Rotate(Vector3.up * elementsRotationSpeed *delta,Space.Self);
            //particle.transform.eulerAngles += particle.transform.TransformDirection(particlesRotationDirection.normalized)*elementsRotationSpeed * delta;
        }
        circle.transform.localEulerAngles += rotationDirection * delta;
    }
    public float SecondToOpen()
    {
        return secondsFromCloseToOpen;
    }
    public Vector2 GetSize()
    {
        return new Vector2(minRadius, maxRadius);
    }
    #endregion
}
