using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;
using UnityEngine.VFX.Utility;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject[] shapes;
    [SerializeField] private float width = 10;
    [SerializeField] private float height = 5;
    [SerializeField] private int rows = 6;
    [SerializeField] private int elementsInRow = 8;
    [SerializeField] private float secondsForOneCycle = 5;
    [SerializeField] private float radius = 5;
    [SerializeField] private float scale = 1;
    [Range(0, 1)]
    public float screamLoudness = 0;
    public float audioLevel { get { return _level; } set { _level = "" + value; } }
    [VFXPropertyBinding("System.Single"), SerializeField]
    ExposedProperty _level = "0";
    [SerializeField] private float microphoneLevelMultiplyer = 1;
    [SerializeField] private float microphoneThreashold = 0;
    public bool DebugDrawing = true;
    public bool DebugDrawText = true;

    private Movable[] particals;
    private float sizeBetweenRows;
    private float sizeBetweenElements;
    private int elemetsCount;
    private float rowSize;
    private Vector3 offset;
    private float time;
    private void Awake()
    {
        particals = new Movable[3 * rows * elementsInRow];
    }
    private void Start()
    {
        sizeBetweenRows = height / rows;
        sizeBetweenElements = width / elementsInRow;
        elemetsCount = rows * elementsInRow;
        rowSize = elementsInRow * sizeBetweenElements;
        offset = new Vector3(rowSize/2,0,rowSize/2);
        for (int i = 0; i < particals.Length; i++)
        {
            CreateInstance(i);
        }
    }

    private void CreateInstance(int i)
    {
        particals[i] = Instantiate(shapes[Random.Range(0, shapes.Length)]).GetComponent<Movable>();
        particals[i].transform.localScale *= scale;
        particals[i].i = i;
        //var pos = PointOnCircle(i)
        //particals[i].radius = Vector3.Distance(pos, transform.position);
        //particals[i].angle = Vector3.SignedAngle((pos - transform.position).normalized, transform.forward, transform.up);
        //particals[i].angle += 360;
        //particals[i].angle %= 360;
        //particals[i].angle *= Mathf.Deg2Rad;
        SetCircleData(i);

        particals[i].CalculatePos = CalculateMove;
        //particals[i].transform.position = PosOnGrid(i);
    }
    private Vector3 PosOnGrid(int i)
    {
        Vector3 newPos = new Vector3();
        newPos.y = (i % rows) * sizeBetweenRows;
        int check = i / (rows * elementsInRow);
        switch(check)
        {
            case 0:
                newPos.z = (i / rows) * sizeBetweenElements;
                break;
            case 1:
                newPos.x = ((i % elemetsCount) / rows) * sizeBetweenElements;
                newPos.z = rowSize;
                break;
            case 2:
                newPos.x = rowSize;
                newPos.z = rowSize - ((i%elemetsCount)/rows) * sizeBetweenElements;
                break;
        }

        return transform.position + newPos - offset;
    }
    private void OnDrawGizmosSelected()
    {
        if (!DebugDrawing)
            return;
        sizeBetweenRows = height / rows;
        sizeBetweenElements = width / elementsInRow;
        elemetsCount = rows * elementsInRow;
        rowSize = elementsInRow * sizeBetweenElements;
        offset = new Vector3(rowSize / 2, 0, rowSize / 2);
        Vector3 pos;
        for (int i = 0; i < 3 * rows * elementsInRow; i++)
        {
            pos = PosOnGrid(i);
            Gizmos.DrawSphere(pos, 0.1f);
            if (DebugDrawText)
            drawString($"{i}", pos);
        }
    }

    static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }

    private void Update()
    {
        time += Time.deltaTime;
        time %= secondsForOneCycle;
        screamLoudness = float.Parse(_level.ToString());
    }
    private void SetCircleData(int i)
    {
        Vector3 result = new Vector3();
        result.y = (i % rows) * sizeBetweenRows;
        var mul = (i/10) % (3 * elementsInRow);
        var angle = (2 * Mathf.PI / (3* elementsInRow)) * mul;
        result.x = (rowSize / 3) * Mathf.Cos(angle);
        result.z = (rowSize / 3) * Mathf.Sin(angle);
        particals[i].radius = rowSize / 3;
        particals[i].angle = angle;
    }
    private Vector3 MovingPos(int i)
    {
        Vector3 result = new Vector3();
        result.y = (i % rows) * sizeBetweenRows + (sizeBetweenRows/2) * Mathf.Sin(6*Mathf.PI * time / secondsForOneCycle + particals[i].angle);
        result.x = radius * Mathf.Cos(2*Mathf.PI*time/secondsForOneCycle - particals[i].angle) + Random.Range(0, 0.2f);
        result.z = radius * Mathf.Sin(2 * Mathf.PI * time / secondsForOneCycle - particals[i].angle) + Random.Range(0, 0.2f);   
        return transform.position + result;
    }
    public Vector3 CalculateMove(int i)
    {
        return Vector3.Lerp(PosOnGrid(i), MovingPos(i), screamLoudness);
    }
}
