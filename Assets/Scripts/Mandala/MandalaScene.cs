using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandalaScene : MonoBehaviour
{
    #region Inspector Parameters
    [SerializeField, Range(0, 1)] private float screamLoudness;
    [SerializeField] private AnimationCurve screamLoudnessCurve;
    public MandalaParticles[] particles;
    public bool debug = true;
    #endregion

    #region Private Parameters
    private float counter;
    private int currentIndex = 0;
    private int upperBound = 0;
    private float delta;
    private float movementDelta;
    private float seconds;
    #endregion

    #region Mandala Update Methods
    private bool needToShrink => currentIndex >= upperBound;
    private bool needToExpand => currentIndex < upperBound;
    private bool canUpdateCurrentIndex => currentIndex < particles.Length && currentIndex >= 0;
    private void IncreaseCounter()
    {
        counter -= Time.deltaTime;
        if (counter < 0)
            counter = 0;
        movementDelta = 1 - counter / seconds;
        if (counter == 0 && currentIndex < particles.Length)
        {
            currentIndex++;
            if (currentIndex < particles.Length)
            {
                counter = particles[currentIndex].SecondToOpen();
                seconds = counter;
                movementDelta = 0;
            }
        }
    }
    private void DecreaseCounter()
    {
        counter += Time.deltaTime;
        if (counter > seconds)
            counter = seconds;
        movementDelta = 1 - counter / seconds;
        if (counter == seconds && currentIndex >= 0)
        {
            currentIndex--;
            if (currentIndex >= 0)
            {
                if (currentIndex + 1 < particles.Length)
                    particles[currentIndex + 1].SetPositions(0);
                counter = 0;
                seconds = particles[currentIndex].SecondToOpen();
                movementDelta = 1;
            }
        }
    }
    private void UpdatePrticlesPosition()
    {
         if (canUpdateCurrentIndex)
         {
            particles[currentIndex].SetPositions(movementDelta);
         }
    }
    private void UpdateParticlesRotation()
    {
        for (int i = 0; i <= currentIndex; i++)
        {
            if (i == particles.Length)
                return;
            particles[i].RotateParticle(Time.deltaTime);
        }
    }
    #endregion

    #region MonoBehavior Methods
    void Start()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Init(i, transform);
        }
        counter = particles[0].SecondToOpen();
        seconds = counter;
    }
    void Update()
    {
        upperBound = (int)Mathf.Lerp(0, particles.Length+1, delta);
        if (needToShrink)
        {
            DecreaseCounter();
        }
        else if(needToExpand)
        {
            IncreaseCounter();
        }
        UpdatePrticlesPosition();
        UpdateParticlesRotation();
    }
    public void OnMicrophonChangedLevel(float level)
    {
        screamLoudness = Mathf.Lerp(screamLoudness, level, Time.deltaTime);
        delta = screamLoudnessCurve.Evaluate(screamLoudness);
    }
    #endregion
}
