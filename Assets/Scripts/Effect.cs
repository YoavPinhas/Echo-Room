using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
public class Effect : MonoBehaviour
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private Shape[] shapes;
    [SerializeField] private float width;
    [SerializeField] private float heights;
    [SerializeField] private int rows;
    [SerializeField] private int elementsInRow;
    public AnimationCurve curve;
    public float audioLevel { get {return _level; } set { _level = "" + value; } }
    [VFXPropertyBinding("System.Single"), SerializeField]
    private ExposedProperty _level = "0";
    private VisualEffect[] effects;
    void Start()
    {
        effects = new VisualEffect[shapes.Length];
        for (int i = 0; i < shapes.Length; i++)
        {
            VisualEffect vfx = Instantiate(vfxPrefab).GetComponent<VisualEffect>();
            effects[i] = vfx;
            vfx.SetInt("Element Count", shapes.Length);
            vfx.SetInt("Index", i);
            vfx.SetFloat("Width", width);
            vfx.SetFloat("Height", heights);
            vfx.SetInt("Rows", rows);
            vfx.SetInt("Elements in Row", elementsInRow);
            vfx.SetMesh("Mesh", shapes[i].Mesh);
            vfx.SetMesh("Mesh", shapes[i].Mesh);
            vfx.SetTexture("Texture", shapes[i].Texture);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float screamLoudness = curve.Evaluate(float.Parse(_level.ToString()));
        foreach(VisualEffect vfx in effects)
        {
            vfx.SetFloat("ScreamLoudness", screamLoudness * screamLoudness);
        }
    }
}

[System.Serializable]
public struct Shape
{
    public Mesh Mesh;
    public Texture2D Texture;
}