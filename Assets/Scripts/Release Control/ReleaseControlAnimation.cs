using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReleaseControlAnimation : MonoBehaviour
{
    #region Shape Structue
    [System.Serializable]
    private struct Shape
    {
        public GameObject meshPrefab;
        public AnimationCurve curveX;
        public AnimationCurve curveY;
        public AnimationCurve curveZ;
        public AnimationCurve scaleCurve;
        public float scaleMultiplier;
        public float minOffsetRadius;
        public float maxOffsetRadius;
    }

    #endregion

    #region Inspector Properties
    [SerializeField, Range(0, 1)] private float screamLoudness;
    [SerializeField] private Shape[] prefabShapes;
    [SerializeField, Min(0.001f)] private float secondsForCycle;
    [SerializeField, Min(1)] private int rows;
    [SerializeField, Min(1)] private int elementsInRow;
    [SerializeField, Min(5)] private float width;
    [SerializeField, Min(5)] private float height;
    [SerializeField, Min(1)] private float radius;
    [SerializeField, Range(0, 1)] private float randomStrength;
    [SerializeField, Range(0, 5)] private float curveMovementSpeed;
    [SerializeField, Range(0, 1)] private float startMovingThreahold;
    [SerializeField, Min(1)] private float gridToCircleSeconds;
    [SerializeField, Min(1)] private float circleToGridSeconds;
    public bool debugDrawing = true;
    #endregion

    #region private Properties
    private const float RIGHT_ANGLE = 0.5f * Mathf.PI;
    private GameObject[][] shapes;
    private Vector3[][] shapeCurveOffsets;
    private Vector3 offset;
    private Vector3[][] posAVectors; //for sake of preformance over memory
    private Vector3[][] posBVectors; //for sake of preformance over memory
    private Vector3[][] sampledPositions; //for sake of preformance over memory
    private float[][] elementsCurveDelta; //for sake of preformance over memory
    private float curveStreangth;
    private int elementsInBlock;
    private float sizeBetweenRows;
    private float sizeBetweenElements;
    private float time;
    private float circleDelta = 0;
    private float angleDelta = 0;
    private float angle = 0;
    #endregion

    #region State Machine parameters
    private enum State { Idle, Moving, StopMoving };
    private State state = State.Idle;
    private Dictionary<State, Action> Run;
    float sampledAngle = 0;
    private Quaternion sampleRotation;
    private float rotationSpeed = 0;
    #endregion

    #region Calculations of Position A
    private Vector3 CalculatePointAAt(int elementType, int elementIndex)
    {
        int index = GetElementIndex(elementType, elementIndex);
        return -offset + new Vector3(
            GetPosAX(index),
            GetPosAY(index),
            GetPosAZ(index)
        );
    }
    private Vector3 GetPosA(int elementType, int elementIndex)
    {
        return posAVectors[elementType][elementIndex];
    }
    private float GetPosAX(int index)
    {
        if (index < elementsInBlock)
            return -0.5f * sizeBetweenElements;
        if (index < 2 * elementsInBlock)
        {
            int i = index % elementsInBlock;
            return (i / rows + 0.5f) * sizeBetweenElements;
        }
        return width + 0.5f * sizeBetweenElements;
    }
    private float GetPosAY(int index)
    {
        return (index % rows) * sizeBetweenRows;
    }
    private float GetPosAZ(int index)
    {
        int i = index % elementsInBlock;
        if (index < elementsInBlock)
        {
            return (i / rows) * sizeBetweenElements;
        }
        if (index < 2 * elementsInBlock)
        {
            return width - sizeBetweenElements;
        }
        return width - (i / rows + 1) * sizeBetweenElements;
    }
    #endregion

    #region Calculations of Position B
    private Vector3 CalculatePointBAt(int elementType, int elementIndex)
    {
        int index = GetElementIndex(elementType, elementIndex);
        var rad = radius + Random.Range(prefabShapes[elementType].minOffsetRadius, prefabShapes[elementType].maxOffsetRadius);
        return new Vector3(
            GetPosBX(elementType, index, rad),
            GetPosBY(elementType, index),
            GetPosBZ(elementType, index, rad)
        );
    }
    private Vector3 GetPosB(int elementType, int elementIndex)
    {
        return posBVectors[elementType][elementIndex];
    }
    private float GetPosBX(int elementType, int index, float radius)
    {
        float angleOffset = (2 * Mathf.PI / (3 * elementsInRow)) * ((index / rows) % (3 * elementsInRow)) + RIGHT_ANGLE;
        return (radius * Mathf.Cos(-angleOffset));
    }
    private float GetPosBY(int elementType, int index)
    {
        return (index % rows) * sizeBetweenRows;
    }
    private float GetPosBZ(int elementType, int index, float radius)
    {
        float angleOffset = (2 * Mathf.PI / (3 * elementsInRow)) * ((index / rows) % (3 * elementsInRow)) + RIGHT_ANGLE;
        return radius * Mathf.Sin(-angleOffset);
    }
    #endregion

    #region Init metods
    private void InitSizes()
    {
        elementsInBlock = rows * elementsInRow;
        sizeBetweenElements = width / elementsInRow;
        sizeBetweenRows = height / rows;
        offset = new Vector3(width / 2, 0, width / 2);
    }
    private void InitShapes()
    {
        int totalElementsCount = 3 * elementsInBlock;
        shapes = new GameObject[prefabShapes.Length][];
        for (int i = 0; i < prefabShapes.Length; i++)
        {
            shapes[i] = new GameObject[GetElementCount(totalElementsCount, prefabShapes.Length, i)];
            for (int j = 0; j < shapes[i].Length; j++)
            {
                shapes[i][j] = Instantiate(prefabShapes[i].meshPrefab, transform);
            }
        }
    }
    private void InitPositionA()
    {
        posAVectors = new Vector3[shapes.Length][];
        sampledPositions = new Vector3[shapes.Length][];
        for (int i = 0; i < shapes.Length; i++)
        {
            posAVectors[i] = new Vector3[shapes[i].Length];
            sampledPositions[i] = new Vector3[shapes[i].Length];
            for (int j = 0; j < shapes[i].Length; j++)
            {
                posAVectors[i][j] = CalculatePointAAt(i, j);
                shapes[i][j].transform.position = posAVectors[i][j];
                sampledPositions[i][j] = posAVectors[i][j];
            }
        }
    }
    private void InitPositionB()
    {
        posBVectors = new Vector3[shapes.Length][];
        for (int i = 0; i < shapes.Length; i++)
        {
            posBVectors[i] = new Vector3[shapes[i].Length];
            for (int j = 0; j < shapes[i].Length; j++)
            {
                posBVectors[i][j] = CalculatePointBAt(i, j);
            }
        }
    }
    private void InitShapeCurveOffsets()
    {
        shapeCurveOffsets = new Vector3[shapes.Length][];
        for (int i = 0; i < shapes.Length; i++)
        {
            shapeCurveOffsets[i] = new Vector3[shapes[i].Length];
            for (int j = 0; j < shapeCurveOffsets[i].Length; j++)
            {
                shapeCurveOffsets[i][j] = new Vector3();
            }
        }
    }
    private void InitStateMachine()
    {
        Run = new Dictionary<State, Action>();
        Run.Add(State.Idle, IdleUpdate);
        Run.Add(State.Moving, MovingUpdate);
        Run.Add(State.StopMoving, StopMovingUpdate);
    }
    private void InitElementsCurveDelta()
    {
        curveStreangth = randomStrength * sizeBetweenElements / 2;
        elementsCurveDelta = new float[shapes.Length][];
        for (int i = 0; i < shapes.Length; i++)
        {
            elementsCurveDelta[i] = new float[shapes[i].Length];
            for (int j = 0; j < elementsCurveDelta[i].Length; j++)
            {
                elementsCurveDelta[i][j] = ((float)GetElementIndex(i, j)) / (rows * elementsInRow);
            }
        }
    }
    #endregion

    #region Common Methods
    private int GetElementCount(int totalElementsCount, int numOfElements, int i)
    {
        if (i < totalElementsCount % numOfElements)
        {
            return totalElementsCount / numOfElements + 1;
        }
        else
        {
            return totalElementsCount / numOfElements;
        }
    }
    private int GetElementIndex(int elementType, int elementIndex)
    {
        return prefabShapes.Length * elementIndex + elementType;
    }
    private void Iterate(Func<int, int, Vector3> func)
    {
        for (int i = 0; i < shapes.Length; i++)
        {
            for (int j = 0; j < shapes[i].Length; j++)
            {
                shapes[i][j].transform.localPosition = func.Invoke(i, j) + GetPointOnCurve(i, j);
                shapes[i][j].transform.localScale = prefabShapes[i].scaleMultiplier * prefabShapes[i].scaleCurve.Evaluate(time) * Vector3.one;
            }
        }
    }
    private Vector3 GetPointOnCurve(int i, int j)
    {
        shapeCurveOffsets[i][j].x = prefabShapes[i].curveX.Evaluate(time + elementsCurveDelta[i][j]) * curveStreangth * screamLoudness;
        shapeCurveOffsets[i][j].y = prefabShapes[i].curveY.Evaluate(time + elementsCurveDelta[i][j]) * curveStreangth * screamLoudness;
        shapeCurveOffsets[i][j].z = prefabShapes[i].curveZ.Evaluate(time + elementsCurveDelta[i][j]) * curveStreangth * screamLoudness;
        return shapeCurveOffsets[i][j];
    }
    private void SamplePositions()
    {
        for (int i = 0; i < shapes.Length; i++)
        {
            for (int j = 0; j < shapes[i].Length; j++)
            {
                sampledPositions[i][j] = shapes[i][j].transform.localPosition - shapeCurveOffsets[i][j];
            }
        }
    }
    #endregion

    #region Movement Methods
    private void IdleUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime);
        Iterate((i, j) => Vector3.Lerp(shapes[i][j].transform.localPosition - shapeCurveOffsets[i][j], GetPosA(i, j), Time.deltaTime * 0.5f));
    }
    private void MovingUpdate()
    {
        Iterate((i, j) => Vector3.Lerp(sampledPositions[i][j], GetPosB(i, j), circleDelta));
        circleDelta = Mathf.Clamp01(circleDelta + Time.deltaTime / gridToCircleSeconds);
        UpdateAngle(Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);//Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), Time.deltaTime);

    }
    private void StopMovingUpdate()
    {
        float dt = Mathf.Clamp01((360 - (transform.eulerAngles.y + 360) % 360) /130);// Clamped Lerp Inverse: t<=230 -> 1 and t = 360 -> 0
        circleDelta = Mathf.Clamp01(circleDelta + dt * Time.deltaTime / (circleToGridSeconds));
        transform.rotation = Quaternion.Lerp(sampleRotation, Quaternion.identity, circleDelta);
        Iterate((i, j) => Vector3.Lerp(sampledPositions[i][j], GetPosA(i, j), circleDelta));
    }
    private void UpdateMovement()
    {
        UpdateTime(Time.deltaTime);
        switch (state)
        {
            case State.Idle:
                if (screamLoudness > startMovingThreahold)
                {
                    SetState(State.Moving);
                }
                break;
            case State.Moving:
                if (screamLoudness < startMovingThreahold)
                {
                   if (circleDelta != 1)
                       SetState(State.Idle);
                   else if(angle < 180)
                   {
                        rotationSpeed = Mathf.Lerp(rotationSpeed, 0.5f/circleToGridSeconds, 0.5f*Time.deltaTime);
                   }
                   else if(angle >= 180)
                   {
                        SetState(State.StopMoving);
                   }
                }
                break;
            case State.StopMoving:
                if (screamLoudness > startMovingThreahold)
                {
                    SetState(State.Moving);
                }
                else if (circleDelta > .999f)
                {
                    //transform.rotation = Quaternion.identity;
                    SetState(State.Idle);
                }
                break;
        }
        Run[state].Invoke();
    }
    private void SetState(State s)
    {
        circleDelta = 0;
        SamplePositions();
        state = s;
        sampledAngle = (transform.eulerAngles.y + 360)%360;
        angleDelta = sampledAngle / 360;
        sampleRotation = transform.rotation;
        rotationSpeed = 1 / secondsForCycle;
    }
    private void UpdateTime(float dt)
    {
        time = (time + dt * curveMovementSpeed) % (120.0f * curveMovementSpeed);
    }
    private void UpdateAngle(float dt)
    {
        angleDelta = (angleDelta + dt *rotationSpeed) % 1;
        angle = Mathf.Lerp(0, 360, angleDelta);
        //angleDelta = (360 / secondsForCycle) * circleDelta * dt;
    }
    #endregion

    #region MonoBehavior
    void Start()
    {
        InitSizes();
        InitShapes();
        InitShapeCurveOffsets();
        InitPositionA();
        InitPositionB();
        InitElementsCurveDelta();
        InitStateMachine();
    }

    
    private void Update()
    {
        screamLoudness = Mathf.Clamp01(screamLoudness);
        UpdateMovement();
    }
    #endregion

    #region Debug Helpers

    private void OnDrawGizmosSelected()
    {
        if (prefabShapes == null || prefabShapes.Length == 0 || !debugDrawing)
            return;
        InitSizes();
        for (int i = 0; i < prefabShapes.Length; i++)
        {
            int s = GetElementCount(elementsInBlock * 3, prefabShapes.Length, i);
            MeshFilter mf = prefabShapes[i].meshPrefab.GetComponent<MeshFilter>();
            for (int j = 0; j < s; j++)
            {
                var p = transform.position + CalculatePointAAt(i, j);
                //drawString(""+GetElementIndex(i, j), p, Color.red);
                Gizmos.DrawWireMesh(mf.sharedMesh, p);
            }
        }
        float angle = 2 * Mathf.PI / (3 * elementsInRow);
        Gizmos.color = Color.yellow;

    }
    /*static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            UnityEditor.Handles.EndGUI();
            return;
        }

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }*/
    public void OnMicrophonChangedLevel(float level)
    {
        screamLoudness = Mathf.Lerp(screamLoudness, level, Time.deltaTime);

    }
#endregion
}
