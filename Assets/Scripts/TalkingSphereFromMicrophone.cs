using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingSphereFromMicrophone : MonoBehaviour
{
    [SerializeField] private float lowestBump = 0.5f;
    [SerializeField] private float highestBymp = 2;
    private MeshRenderer meshRenderer;
    private float height;
    private float speed = 10;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetHeight(float t)
    {
        float destHeight = Mathf.Lerp(lowestBump, highestBymp, t);
        height = Mathf.Lerp(height, destHeight, Time.deltaTime * speed);
        meshRenderer.material.SetFloat("_NormalPush", height);
    }
}
