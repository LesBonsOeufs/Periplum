using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class UniqueIDDispenser : EditorWindow
{
    [MenuItem("Window/Unique ID Dispenser")]
    public static void ShowWindow()
    {
        GetWindow<UniqueIDDispenser>("Unique ID Dispenser");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate IDs for All IUniqueID Components", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate IDs"))
            GenerateIDs();
    }

    private void GenerateIDs()
    {
        IEnumerable<IUniqueID> lUniqueIDs = FindObjectsOfType<MonoBehaviour>().OfType<IUniqueID>();
        foreach (IUniqueID uniqueID in lUniqueIDs)
        {
            uniqueID.GiveId();
            EditorUtility.SetDirty((MonoBehaviour)uniqueID); // Mark the object as dirty to save changes.
        }

        Debug.Log("Generated IDs for " + lUniqueIDs.Count() + " IUniqueID components.");
    }
}