using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RouletteManager))]
public class RouletteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RouletteManager rouletteManager = (RouletteManager)target;
        if (GUILayout.Button("Change Roulette Rotate Data"))
        {
            rouletteManager.RefreshRotateData();
        }
    }
}
