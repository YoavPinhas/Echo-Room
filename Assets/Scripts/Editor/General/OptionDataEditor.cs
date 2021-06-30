using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OptionData))]
public class OptionDataEditor : Editor
{
    private OptionData data;
    private void OnEnable()
    {
        data = (OptionData)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Save To Text File"))
        {
            SaveData();
        }
        if (GUILayout.Button("Load From Text File"))
        {
            LoadData();
        }
        EditorUtility.SetDirty(data);
    }

    private void LoadData()
    {
        string file = EditorUtility.OpenFilePanel("Open Text Data", Application.dataPath, $"txt");
        if (string.IsNullOrEmpty(file))
            return;
        string text = File.ReadAllText(file);
        text = text.ToLower();
        text = text.Replace(", ", ",");
        text = text.Replace(" ,", ",");
        text = text.Replace(" , ", ",");
        List<string> words = text.Split(',').ToList();
        
        data.keyWords = words.Where(e => (e.Split(' ').Length == 1) && !string.IsNullOrEmpty(e)).ToArray();
        data.sentences = words.Where(e => (e.Split(' ').Length > 1) && !string.IsNullOrEmpty(e)).ToArray();
        
    }

    private void SaveData()
    {
        string file = EditorUtility.SaveFilePanel("Open Text Data", Application.dataPath, $"{data.name}.txt", "txt");
        string text = "";
        foreach(string keyWord in data.keyWords)
        {
            text += $"{keyWord}, ";
        }
        foreach (string sentence in data.sentences)
        {
            text += $"{sentence}, ";
        }
        File.WriteAllText(file, text);
    }
}
