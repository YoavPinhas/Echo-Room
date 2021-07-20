using UnityEngine;

public class Scene2Animation : MonoBehaviour, EffectScene
{
    #region Inspector Parameters
    [SerializeField, Range(0, 1)] private float screamLoudness;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField, Range(1,5)] private float screamSensativity = 1;
    [SerializeField, Min(1), Tooltip("Max particles in one burst.")] 
    private float maxParticlesInBurst = 30;
    [SerializeField] private float maxRateOverTime = 10;
    [SerializeField, Tooltip("How the change in the scream loudness will effect the paticle rate change.\n" +
                            "The rate change is a value between 0 and 'Max Particles Rate'.\n" +
                            "The curve must vary from 0 to 1 in both time and value.")] 
    private AnimationCurve particleRateChange;
    #endregion

    #region Private Parameters
    private float delta = 0;
    private ParticleSystem.Burst[] particleSystemBursts;
    private ParticleSystem.EmissionModule[] particleSystemEmissions;
    #endregion

    #region Animation Methods
    private void UpdateDelta()
    {
        delta = Mathf.Lerp(delta, screamLoudness, Time.deltaTime * screamSensativity);
    }
    private void UpdateParticles()
    {
        var pd = particleRateChange.Evaluate(delta);
        for (int i = 0; i < particles.Length; i++)
        {
            particleSystemBursts[i].probability = pd;
            particleSystemBursts[i].count = (int)Mathf.Lerp(0, maxParticlesInBurst, pd);
            particleSystemEmissions[i].rateOverTime = Mathf.Lerp(0, maxRateOverTime, pd);
            particleSystemEmissions[i].SetBurst(0, particleSystemBursts[i]);
        }
    }

    #endregion

    #region MonoBehavior
    private void Start()
    {
        particleSystemEmissions = new ParticleSystem.EmissionModule[particles.Length];
        particleSystemBursts = new ParticleSystem.Burst[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            particleSystemBursts[i] = particles[i].emission.GetBurst(0);
            particleSystemEmissions[i] = particles[i].emission;
        }
    }
    private void Update()
    {
        UpdateParticles();
        UpdateDelta();
    }
    public void OnMicrophonChangedLevel(float level)
    {
        screamLoudness = Mathf.Lerp(screamLoudness, level, Time.deltaTime);
    }

 
    #endregion
}
