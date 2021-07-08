using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoMnager : MonoBehaviour
{
    //public string ArduinoIP = "10.0.0.2";
    //public string ComputerIP = "10.0.0.1";
    //public int outputStart = 41;
    //public int outputEnd = 53;

    //public int inputStart = 22;
    //public int intputEnd = 40;

    //private Dictionary<int, Func<bool, int, bool>> outputMethods;
    //private Dictionary<int, Func<bool, bool>> inputMethods;
    //public bool[] prevInputs; 
    //private Arduino _arduino;

    //public Action<int,bool> inputEvents;
    //public bool onlyDialogTest;
    //// Start is called before the first frame update
    //public void Init()
    //{
    //    outputMethods = new Dictionary<int, Func<bool, int, bool>>();
    //    inputMethods = new Dictionary<int, Func<bool, bool>>();
    //    if (onlyDialogTest)
    //        return;
    //    inputEvents = new Action<int,bool>((ctx_1, ctx_2) => { });
    //    _arduino = new Arduino(ComputerIP, ArduinoIP);
    //    prevInputs = new bool[intputEnd - inputStart];
    //    _arduino.InputChange = Event;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (onlyDialogTest)
    //        return;
    //    CheckInputs();

    //}

    //private void OnDestroy()
    //{
    //    for(int i = outputStart; i < outputEnd; i++)
    //    {
    //        ChangeOutputState(i, false);
    //    }
    //    _arduino.OnDisable();
    //}


    //public bool Event(int i, bool state)
    //{
    //    if (onlyDialogTest)
    //        return false;
    //    inputEvents.Invoke(inputStart + i, state);
    //    return true;
    //}

    //public void ToggleOutput(int index)
    //{
    //    _arduino.ToggleOutputState(index- outputStart);
    //    _arduino.SendOutputState();

    //}

    //private void CheckInputs()
    //{
    //    if (onlyDialogTest)
    //        return;
    //    for (int i = 0; i < intputEnd-inputStart; i++)
    //    {
    //        bool temp = _arduino.GetInputState(i);
    //        inputEvents.Invoke(inputStart + i, temp);
    //    }
  
    //}
    
    //public void SetOutputMethod(int index, Func<bool, int, bool> method)
    //{
    //    outputMethods.Add(index, method);
    //}

    //public void SetInputMethod(int index, Func<bool, bool> method)
    //{
    //    inputMethods.Add(index, method);
    //}

    //public void StartOutputMehtod(int index, bool on_of)
    //{
    //    if(outputMethods.ContainsKey(index))
    //        outputMethods[index].Invoke(on_of, index);
    //}

    //public void StartIntputMehtod(int index, bool state)
    //{
    //    if(inputMethods.ContainsKey(index))
    //        inputMethods[index].Invoke(state);
    //}
    //public void ChangeOutputState(int index, bool state)
    //{
    //    if (onlyDialogTest)
    //        return;
    //    if (index > outputEnd || index < outputStart)
    //    {
    //        Debug.LogError($"The relay's index ({index}) is out of range ({outputStart}-{outputEnd}).");
    //        return;
    //    }
    //    if (_arduino == null)
    //        return;
    //    _arduino.SetOutputState(index - outputStart, state);
    //    _arduino.SendOutputState();
    //}

    //public void SetInputEvents(Action<int, bool> inputEvents)
    //{
    //    this.inputEvents += inputEvents;
    //}

    //public void RemoveInputEvents(Action<int, bool> inputEvents)
    //{
    //    this.inputEvents -= inputEvents;
    //}

    //public bool OnlyDialogTest()
    //{
    //    return onlyDialogTest;
    //}
}
