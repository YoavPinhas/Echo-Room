using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MandalaParticle;

public class RleaseFrusration : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float screemLoudness;

    [SerializeField] private MandalaRing[] rings;
    private int currentIndex = 0;

    
    void Start()
    {
        //rings = new MandalaRing[Rings.Length];
        //for (int i = 0; i < Rings.Length; i++)
        //{
        //    GameObject obj = new GameObject($"Ring {i}");
        //    rings[i] = obj.AddComponent<MandalaRing>();
        //    rings[i].smallRadius = Rings[i].smallRadius;
        //    rings[i].bigRadius = Rings[i].bigRadius;
        //    rings[i].secondsForCycle = Rings[i].secondsForCycle;
        //    rings[i].secondsForExpension = Rings[i].secondsForExpension;
        //    rings[i].numberOfCircles = Rings[i].numberOfCircles;
        //    rings[i].particlesInCircle = Rings[i].particlesInCircle;
        //    rings[i].ringRotation = Rings[i].ringRotation;
        //}
        //for (int i = 0; i < rings.Length - 1; i++)
        //{
        //    rings[i].next = rings[i + 1];
        //}
        foreach (MandalaRing ring in rings)
        {
            ring.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentIndex = (int)Mathf.Lerp(0, rings.Length - 1, screemLoudness);
        if(currentIndex > 0 && !rings[0].IsMoving)
        {
            rings[0].StartMoving();
        }
        for (int i = 0; i < rings.Length; i++)
        {
            if(currentIndex > i)
            {
                rings[i].canOpenNext = true;
            }
            else
            {
                rings[i].canOpenNext = false;
                rings[i].StartClose();
            }
        }
    }
}
