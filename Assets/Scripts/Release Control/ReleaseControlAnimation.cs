using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool debugDrawing = true;
    #endregion

    #region private Properties
    private GameObject[][] shapes;
    private Vector3[] shapeCurveOffsets;
    private Vector3 offset;
    private Vector3[][] posAVectors; //for sake of optimization
    private int elementsInBlock;
    private float sizeBetweenRows;
    private float sizeBetweenElements;
    private float time;
    #endregion
    #region State Machine parameters
    private enum State { Idle, Moving, StopMoving };
    private State state = State.Moving;
    private Dictionary<State, Func<State>> Run;
    #endregion

    #region Calculations of Position A
    private Vector3 CalculatePointAAt(int elementType, int elementIndex)
    {
        int index = GetElementIndex(elementType, elementIndex);
        return - offset + new Vector3(
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
    private Vector3 GetPosB(int elementType, int elementIndex)
    {
        int index = GetElementIndex(elementType, elementIndex);
        Vector3 result = new Vector3(
            GetPosBX(elementType, index),
            GetPosBY(index),
            GetPosBZ(elementType, index)
        );
        return result;
    }
    private float GetPosBX(int elementType, int index)
    {
        float angleOffset = (2 * Mathf.PI / (3 * elementsInRow)) * ((index / rows) % (3*elementsInRow));
        return radius * Mathf.Cos(time * 2 * Mathf.PI + angleOffset);
    }
    private float GetPosBY(int index)
    {
        return (index % rows) * sizeBetweenRows;
    }
    private float GetPosBZ(int elementType, int index)
    {
        float angleOffset = (2 * Mathf.PI / (3 * elementsInRow)) * ((index/rows) % (3 * elementsInRow));
        return radius * Mathf.Sin(time * 2 * Mathf.PI + angleOffset);
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
            shapes[i] = new GameObject[GetElementCount(totalElementsCount,prefabShapes.Length, i)];
            for (int j = 0; j < shapes[i].Length; j++)
            {
                shapes[i][j] = Instantiate(prefabShapes[i].meshPrefab);
            }
        }
    }
    private void InitPositionA()
    {
        posAVectors = new Vector3[shapes.Length][];
        for (int i = 0; i < shapes.Length; i++)
        {
            posAVectors[i] = new Vector3[shapes[i].Length];
            for (int j = 0; j < shapes[i].Length; j++)
            {
                posAVectors[i][j] = CalculatePointAAt(i, j);
                shapes[i][j].transform.position = posAVectors[i][j];
            }
        }
    }
    private void InitSapeCurveOffsets()
    {
        shapeCurveOffsets = new Vector3[shapes.Length];
        for (int i = 0; i < shapes.Length; i++)
        {
            shapeCurveOffsets[i] = new Vector3();
        }
    }
    private void InitStateMachine()
    {
        Run = new Dictionary<State, Func<State>>();
        Run.Add(State.Idle, IdleUpdate);
        Run.Add(State.Moving, MovingUpdate);
        Run.Add(State.StopMoving, StopMovingUpdate);
    }
    #endregion

    #region State Machine Methods
    private State IdleUpdate()
    {
        return State.Idle;
    }
    private State MovingUpdate()
    {
        for (int i = 0; i < shapes.Length; i++)
        {
            for (int j = 0; j < shapes[i].Length; j++)
            {
                shapes[i][j].transform.position = transform.position + Vector3.Lerp(GetPosA(i, j), GetPosB(i, j), screamLoudness);
            }
        }
        return State.Moving;
    }
    private State StopMovingUpdate()
    {
        return State.StopMoving;
    }
    #endregion

    #region Common Methods
    private int GetElementCount(int totalElementsCount, int numOfElements, int i)
    {
        if (i < totalElementsCount % elementsInRow)
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
    #endregion

    void Start()
    {
        InitSizes();
        InitShapes();
        InitSapeCurveOffsets();
        InitPositionA();
        InitStateMachine();
    }

    private void UpdatePositions()
    {
        state = Run[state].Invoke();
    }
    private void UpdateCurveOffsets()
    {
        for (int i = 0; i < shapeCurveOffsets.Length; i++)
        {
            shapeCurveOffsets[i].x = prefabShapes[i].curveX.Evaluate(time);
            shapeCurveOffsets[i].y = prefabShapes[i].curveY.Evaluate(time);
            shapeCurveOffsets[i].z = prefabShapes[i].curveZ.Evaluate(time);
        }
    }

    private void UpdateTime(float dt)
    {
        time = (time + dt / secondsForCycle) % 1.0f;
    }

    private void FixedUpdate()
    {
        UpdateCurveOffsets();
        UpdatePositions();
        UpdateTime(Time.fixedDeltaTime);
    }

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
                Gizmos.DrawMesh(mf.sharedMesh, transform.position + CalculatePointAAt(i, j));
            }
        }
    }
}
