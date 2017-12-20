using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using com.ootii.MeshMerge;

[CustomEditor(typeof(MergedMesh))]
public class MergedMeshEditor : Editor
{
    /// <summary>
    /// Helps us keep track of when the list needs to be saved. This
    /// is important since some changes happen in scene.
    /// </summary>
    private bool mIsDirty;

    /// <summary>
    /// Actual classes we're manipulationg
    /// </summary>
    private MergedMesh mMergedMesh;
    private SerializedObject mMergedMeshSO;

    /// <summary>
    /// Determines if we'll optimize the mesh that we create
    /// </summary>
    private bool mOptimizeMesh = false;

    /// <summary>
    /// Help strings 
    /// </summary>
    private string mMergedMeshHelp = "Merged Mesh was created when source objects were combined using ootii's Mesh Merge tool.";
    private string mNoMeshFiltersHelp = "None of the initial source objects exist. It's safe to remove this component.";

    /// <summary>
    /// Hold a reference to the style
    /// </summary>
    private GUIStyle mWindowStyle;

    /// <summary>
    /// Called when the script object is loaded
    /// </summary>
    void OnEnable()
    {
        // Grab the serialized objects
        mMergedMesh = (MergedMesh)target;
        mMergedMeshSO = new SerializedObject(target);

        mIsDirty = RebuildMeshFilterIDs();
    }

    /// <summary>
    /// Called when the inspector needs to draw
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (mWindowStyle == null)
        {
            mWindowStyle = new GUIStyle(GUI.skin.window);
            mWindowStyle.alignment = TextAnchor.UpperLeft;
            mWindowStyle.fontSize = 9;
            mWindowStyle.fontStyle = FontStyle.Normal;
        }

        // Pulls variables from runtime so we have the latest values.
        mMergedMeshSO.Update();

        // Check if we should validate the IDs
        bool lValidMeshFilterIDs = false;

        // Update the motion controller layers so they can update with the definitions
        int lMeshCount = 0;
        if (mMergedMesh != null) { lMeshCount = mMergedMesh.MeshFilterIDs.Length; }

        // Explain this component
        EditorGUILayout.HelpBox(mMergedMeshHelp, MessageType.None, true);

        // If there is no child meshes, this object can be safely removed
        if (lMeshCount == 0)
        {
            EditorGUILayout.HelpBox(mNoMeshFiltersHelp, MessageType.Warning, true);
        }
        // Only modify the meshes if there's something to modify
        else
        {
            GUILayout.BeginVertical(lMeshCount + " source objects merged", mWindowStyle, GUILayout.Height(10));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(new GUIContent("Show", "Show source meshes"), EditorStyles.miniButtonLeft))
            {
                EnableMergedMeshes(true);
                lValidMeshFilterIDs = true;
            }

            if (GUILayout.Button(new GUIContent("Select", "Select source objects in the hierarchy"), EditorStyles.miniButtonLeft))
            {
                SelectMergedObjects();
                lValidMeshFilterIDs = true;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(new GUIContent("Hide", "Hide source meshes"), EditorStyles.miniButtonRight))
            {
                EnableMergedMeshes(false);
                lValidMeshFilterIDs = true;
            }

            if (GUILayout.Button(new GUIContent("Delete", "Permanently delete source objects"), EditorStyles.miniButtonRight))
            {
                if (EditorUtility.DisplayDialog("Merged Mesh", "Permanently delete source objects?\rThis cannot be undone.", "Delete", "Cancel"))
                {
                    DeleteMergedObjects();
                    lValidMeshFilterIDs = true;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
        }

        GUILayout.BeginVertical("Created mesh", mWindowStyle, GUILayout.Height(10));

        mOptimizeMesh = EditorGUILayout.Toggle(new GUIContent("Optimize Mesh", "Optimizes the mesh before creating the asset."), mOptimizeMesh);

        if (GUILayout.Button(new GUIContent("Save As Asset"), EditorStyles.miniButton))
        {
            MeshFilter lMeshFilter = mMergedMesh.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (lMeshFilter != null)
            {
                Mesh lMesh = lMeshFilter.sharedMesh;
                if (lMesh != null)
                {
                    string lPath = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
                    if (!string.IsNullOrEmpty(lPath))
                    {
                        lPath = FileUtil.GetProjectRelativePath(lPath);

                        // Optimize the mesh as needed
                        if (mOptimizeMesh)
                        {
#if (UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
                            lMesh.Optimize();
#endif
                        }

                        // Store the asset outside of the scene
                        AssetDatabase.CreateAsset(lMesh, lPath);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();

        // Test if we should validate and rebuild the mesh filters
        if (lValidMeshFilterIDs)
        {
            mIsDirty = RebuildMeshFilterIDs();
        }
        

        // If there is a change... update.
        if (GUI.changed)
        {
            mIsDirty = true;
        }

        // If there is a change... update.
        if (mIsDirty)
        {
            // Flag the object as needing to be saved
            EditorUtility.SetDirty(mMergedMesh);

            // Pushes the values back to the runtime so it has the changes
            mMergedMeshSO.ApplyModifiedProperties();

            // Clear out the dirty flag
            mIsDirty = false;
        }
    }

    /// <summary>
    /// Sets the visiblity flag fo the mesh render associated with the child mesh
    /// </summary>
    /// <param name="rEnable">Enabled flag of the mesh renderer</param>
    private void EnableMergedMeshes(bool rEnable)
    {
        if (mMergedMesh == null) { return; }
        if (mMergedMesh.MeshFilterIDs == null) { return; }
        if (mMergedMesh.MeshFilterIDs.Length == 0) { return; }

        for (int i = 0; i < mMergedMesh.MeshFilterIDs.Length; i++)
        {
            int lID = mMergedMesh.MeshFilterIDs[i];

            MeshFilter lMeshFilter = EditorUtility.InstanceIDToObject(lID) as MeshFilter;
            if (lMeshFilter != null)
            {
                GameObject lGameObject = lMeshFilter.gameObject;
                if (lGameObject != null)
                {
                    MeshRenderer lMeshRenderer = lGameObject.GetComponent<MeshRenderer>();
                    if (lMeshRenderer != null)
                    {
                        lMeshRenderer.enabled = rEnable;
                    }

                    // Ensure the game object is enabled if needed
                    if (rEnable && !lGameObject.activeInHierarchy) { lGameObject.SetActive(rEnable); }
                }
            }
        }
    }

    /// <summary>
    /// Selects the objects that make up the merged object
    /// </summary>
    private void SelectMergedObjects()
    {
        if (mMergedMesh == null) { return; }
        if (mMergedMesh.MeshFilterIDs == null) { return; }
        if (mMergedMesh.MeshFilterIDs.Length == 0) { return; }

        // Find the mesh objects
        GameObject[] lGameObjects = new GameObject[mMergedMesh.MeshFilterIDs.Length];
        for (int i = 0; i < mMergedMesh.MeshFilterIDs.Length; i++)
        {
            int lID = mMergedMesh.MeshFilterIDs[i];

            MeshFilter lMeshFilter = EditorUtility.InstanceIDToObject(lID) as MeshFilter;
            if (lMeshFilter != null)
            {
                GameObject lGameObject = lMeshFilter.gameObject;
                lGameObjects[i] = lGameObject;
            }
        }

        // Select the mesh objects making up the merged object
        Selection.objects = lGameObjects;
    }

    /// <summary>
    /// Deletes the objects that make up the merged object
    /// </summary>
    private void DeleteMergedObjects()
    {
        if (mMergedMesh == null) { return; }
        if (mMergedMesh.MeshFilterIDs == null) { return; }
        if (mMergedMesh.MeshFilterIDs.Length == 0) { return; }

        // Find the mesh objects
        for (int i = 0; i < mMergedMesh.MeshFilterIDs.Length; i++)
        {
            int lID = mMergedMesh.MeshFilterIDs[i];

            MeshFilter lMeshFilter = EditorUtility.InstanceIDToObject(lID) as MeshFilter;
            if (lMeshFilter != null)
            {
                GameObject lGameObject = lMeshFilter.gameObject;
                DestroyImmediate(lGameObject);
            }
        }
    }

    /// <summary>
    /// Cycle through the mesh filters to ensure they're valid.
    /// If there is an invalid one, we're going to rebuild the array
    /// </summary>
    private bool RebuildMeshFilterIDs()
    {
        if (mMergedMesh.MeshFilterIDs.Length == 0) { return false; }

        bool lUpdate = false;
        List<int> lMeshFilterIDs = new List<int>();

        // Test and build the temporary array
        for (int i = 0; i < mMergedMesh.MeshFilterIDs.Length; i++)
        {
            int lID = mMergedMesh.MeshFilterIDs[i];

            MeshFilter lMeshFilter = EditorUtility.InstanceIDToObject(lID) as MeshFilter;
            if (lMeshFilter == null)
            {
                lUpdate = true;
            }
            else
            {
                lMeshFilterIDs.Add(lID);
            }
        }

        // If we need to update, do it
        if (lUpdate)
        {
            mMergedMesh.MeshFilterIDs = lMeshFilterIDs.ToArray();
        }

        return lUpdate;
    }
}

