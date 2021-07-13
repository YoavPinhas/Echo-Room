using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MandalaParticle;

public class MandalaRing : MonoBehaviour
{
    public float smallRadius;
    public float bigRadius;
    public float secondsForCycle;
    public float secondsForExpension;
    public float secondsForShrink;

    public int numberOfCircles;
    public int particlesInCircle;
    public RotationType ringRotation;
    public MandalaParticle prefab;
    [Range(0, 1)] public float whenSignalToNext;
    public MandalaRing next;
    public bool canOpenNext;

    private bool startMoving = false;
    public bool IsMoving => startMoving;
    private bool needToBeOpen = false;
    private bool needToBeClosed = false;
    private MandalaParticle[] particles;

    private float t;
    private Vector3 rotationDirection;

    public void StartMoving()
    {
        if (IsMoving)
            return;

        needToBeOpen = true;
        startMoving = true;
        StartCoroutine(MovementHandler());
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
    private IEnumerator MovementHandler()
    {
        WaitForEndOfFrame wairFrame = new WaitForEndOfFrame();
        while (needToBeOpen)
        { 
            Rotate();
            t = Mathf.Clamp01(t + Time.deltaTime / secondsForExpension);
            if (next != null && t >= whenSignalToNext && !next.IsMoving && canOpenNext)
            {
                next.StartMoving();
            }
            foreach(MandalaParticle particle in particles)
            {
                particle.SetPosition(t);
                particle.Rotate();
            }
            yield return wairFrame;
        }
    }

    public void StartClose()
    {
        if (needToBeOpen)
        {
            needToBeOpen = false;
            startMoving = false;
            StartCoroutine(CloseHandler());
        }
    }
    private IEnumerator CloseHandler()
    {
        WaitForEndOfFrame wairFrame = new WaitForEndOfFrame();
        while (t > 0)
        {
            Rotate();
            t = Mathf.Clamp01(t - Time.deltaTime / secondsForShrink);
            foreach (MandalaParticle particle in particles)
            {
                particle.SetPosition(t);
                particle.Rotate();
            }
            yield return wairFrame;
        }
    }
    public void Init()
    {
        particles = new MandalaParticle[numberOfCircles * particlesInCircle];
        
        float angleBetweenParticles = 2 * Mathf.PI / particlesInCircle;
        var axis = (prefab.lookAxis == Direction.X) ? Vector3.right :
                    (prefab.lookAxis == Direction.Y) ? Vector3.up :
                    Vector3.forward;

        for (int i = 0; i < numberOfCircles * particlesInCircle; i++)
        {
            GameObject obj = new GameObject($"particle{i}");
            obj.transform.SetParent(transform);
            particles[i] = obj.AddComponent<MandalaParticle>();
            particles[i].rotationAxis = prefab.rotationAxis;
            particles[i].lookAxis = prefab.lookAxis;
            particles[i].elementsRotationSpeed = prefab.elementsRotationSpeed;
            particles[i].elementsRotation = prefab.elementsRotation;
            particles[i].ring = this;
            float angle = (i % particlesInCircle) * angleBetweenParticles;
            if ((i / particlesInCircle) % 2 == 1)
            {
                angle += 0.5f * angleBetweenParticles;
            }
            int row = i / particlesInCircle;
            float maxRadius = Mathf.Lerp(smallRadius, bigRadius, Mathf.InverseLerp(0, numberOfCircles, row));
            Vector3 targetPos = maxRadius * (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));
            Vector3 startPos = smallRadius * (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));
            particles[i].startPosition = startPos;
            particles[i].targetPosition = targetPos;
            Vector3 dir = (transform.position - new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)).normalized;
            particles[i].transform.LookAt(particles[i].transform.position + dir, axis);
        }
    }
    private void Rotate()
    {
        transform.localEulerAngles += rotationDirection * Time.deltaTime;
    }
}
