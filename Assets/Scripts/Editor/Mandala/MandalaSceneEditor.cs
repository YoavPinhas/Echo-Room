using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MandalaScene))]
public class MandalaSceneEditor : Editor
{
    private MandalaScene ms;

    private void OnSceneGUI()
    {
        ms = this.target as MandalaScene;
        if (!ms.debug || ms.particles == null || ms.particles.Length == 0)
            return;
        float angle = 2 * Mathf.PI / 10;
        for (int i = 0; i < ms.particles.Length; i++)
        {
            Handles.color = Color.Lerp(Color.red, Color.green, (float)i / ms.particles.Length);
            Vector2 size = ms.particles[i].GetSize();
            Handles.DrawWireDisc(ms.transform.position, ms.transform.forward, size.x);
            Handles.DrawWireDisc(ms.transform.position, ms.transform.forward, size.y);
            
            for (int j = 0; j < 10; j++)
            {
                Vector3 point = new Vector3(Mathf.Cos(angle * j), Mathf.Sin(angle * j), 0);
                Handles.DrawLine(ms.transform.position + size.x * point, ms.transform.position + size.y * point);
            }
        }
    }
}
