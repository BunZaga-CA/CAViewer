using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrimeDB))]
public class PrimeDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PrimeDB pdb = (PrimeDB)target;
        
        if(GUILayout.Button("Generate Primes"))
        {
            pdb.GeneratePrimes();
            EditorUtility.SetDirty(pdb);
            EditorUtility.SetDirty(serializedObject.targetObject);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        if(GUILayout.Button("Generate Properties"))
        {
            pdb.GenerateProperties();
            EditorUtility.SetDirty(pdb);
            EditorUtility.SetDirty(serializedObject.targetObject);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
