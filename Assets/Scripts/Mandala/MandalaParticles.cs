using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MandalaParticles
{
    private enum RotationType { None, PositiveDirection, NegativeDirection};
    
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
    [SerializeField] private RotationType xRotation;
    [SerializeField] private RotationType yRotation;
    [SerializeField] private RotationType zRotation;
    [SerializeField] private float elementsRotationSpeed;
    [SerializeField, Min(0.5f)] private float minElementSize = 1;
    [SerializeField, Min(0.5f)] private float maxElementSize = 1;
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
    #endregion

    #region Particles Update Methods
    public void Init(int index, Transform parent)
    {
        particles = new GameObject[numberOfCircles * elementsInCircle];
        circle = GameObject.Instantiate(new GameObject(), parent);
        
        circle.name = $"Mandala Particles #{index}";
        if (circle.name == "Mandala Particles #4")
        {
            Debug.Log("Hi");
        }
        for (int i = 0; i < numberOfCircles * elementsInCircle; i++)
        {
            particles[i] = GameObject.Instantiate(shapePrefab, circle.transform);
            particles[i].transform.localScale = Vector3.zero;
        }
        angleBetweenParticles = 2 * Mathf.PI / elementsInCircle;

        radiuses = new float[numberOfCircles];
        currentRadiuses = new float[numberOfCircles];
        sizes = new float[numberOfCircles];
        currentSizes = new float[numberOfCircles];
        float scaleMagnitude = shapePrefab.transform.localScale.magnitude;
        minElementSize *= scaleMagnitude;
        maxElementSize *= scaleMagnitude;
        for (int i = 0; i < numberOfCircles; i++)
        {
            radiuses[i] = Mathf.Lerp(minRadius, maxRadius, (numberOfCircles * numberOfCircles) / (float)(numberOfCircles + i));
            sizes[i] = Mathf.Lerp(minElementSize, maxElementSize, (numberOfCircles*numberOfCircles) / (float)(numberOfCircles + i));
        }


        float dirX = (xRotation == RotationType.None) ? 0 : 
                     (xRotation == RotationType.PositiveDirection) ? 
                     elementsRotationSpeed : -elementsRotationSpeed;
        float dirY = (yRotation == RotationType.None) ? 0 :
                     (yRotation == RotationType.PositiveDirection) ?
                     elementsRotationSpeed : -elementsRotationSpeed;
        float dirZ = (zRotation == RotationType.None) ? 0 :
                     (zRotation == RotationType.PositiveDirection) ?
                     elementsRotationSpeed : -elementsRotationSpeed;
        particlesRotationDirection = new Vector3(dirX, dirY, dirZ);
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
            particle.transform.localEulerAngles += particlesRotationDirection*delta;
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
