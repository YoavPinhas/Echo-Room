using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Animation : MonoBehaviour
{
    #region Inspector Parameters
    [SerializeField, Range(0, 1)] private float screamLoudness;
    [SerializeField] private Transform ball;
    [SerializeField] private ParticleSystem particles;
    [SerializeField, Min(1), Tooltip("More seconds for cycle = slower speed!\n" +
                            "This parameter determine the minSpeed in a sense.")]
    private float maxScondsForCycle = 25;
    [SerializeField, Min(1), Tooltip("Less seconds for cycle = faster speed!\n" +
                            "This parameter determine the maxSpeed in a sense.")]
    private float minScondsForCycle = 10;
    [SerializeField, Min(0.1f)] private float rotationAcceleration = 1;
    [SerializeField, Range(1,5)] private float screamSensativity = 1;
    [SerializeField, Min(1), Tooltip("How many particles will be created in one cycle at most.")] 
    private float maxParticlesRate = 10;
    [SerializeField, Tooltip("How the change in the scream loudness will effect the paticle rate change.\n" +
                            "The rate change is a value between 0 and 'Max Particles Rate'.\n" +
                            "The curve must vary from 0 to 1 in both time and value.")] 
    private AnimationCurve particleRateChange;
    #endregion

    #region Private Parameters
    private float secondsForCycle = 0;
    private float minSpeed = 0;
    private float maxSpeed = 0;
    private float angle = 0;
    private float destSpeed = 0;
    private float currentSpeed = 0;
    private float delta = 0;
    private ParticleSystem.EmissionModule emission;
    #endregion

    #region Animation Methods
    private void UpdateDelta()
    {
        delta = Mathf.Lerp(delta, screamLoudness, Time.deltaTime * screamSensativity);
    }
    private void UpdateAngle()
    {
        angle = (angle + currentSpeed) % (360);
        currentSpeed = Mathf.Clamp(currentSpeed + rotationAcceleration * Time.deltaTime, 0, destSpeed);
        destSpeed = delta * Mathf.Lerp(minSpeed, maxSpeed, screamLoudness) * Time.deltaTime;
    }
    private void UpdateParticles()
    {
        var amount = Mathf.Lerp(0, maxParticlesRate, particleRateChange.Evaluate(delta));
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(amount);
    }
    private void RotateBall()
    {
        ball.eulerAngles = -angle * Vector3.right;
    }

    #endregion

    #region MonoBehavior
    private void Start()
    {
        //particles.enableEmission = false;
        emission = particles.emission;
        minSpeed = 360 / maxScondsForCycle;
        maxSpeed = 360 / minScondsForCycle;
    }
    private void Update()
    {
        RotateBall();
        UpdateParticles();
        UpdateAngle();
        UpdateDelta();
    }
    public void OnMicrophoneLevelChanged(float level)
    {
        screamLoudness = Mathf.Lerp(screamLoudness, level, Time.deltaTime);
    }
    #endregion
}
