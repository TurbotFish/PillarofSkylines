using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using com.ootii.Collections;
using com.ootii.MeshMerge;

public class MeshMergeWindow : EditorWindow
{
    /// <summary>
    /// Determines if the combine function merges the sub meshes into a single mesh
    /// </summary>
    private bool mMergeSubMeshes = true;

    /// <summary>
    /// Determines if we build the second set of UV values for lightmapping
    /// </summary>
    private bool mBuildUV2 = false;

    /// <summary>
    /// Tracks the currently selection objects
    /// </summary>
    private GameObject[] mSelectedObjects = null;

    /// <summary>
    /// Tracks the materials that are selected
    /// </summary>
    private List<Material> mSelectedMaterials = new List<Material>();

    /// <summary>
    /// Selected object counts
    /// </summary>
    private int mSelectedCount = 0;
    private int mSelectedMaterialCount = 0;
    private int mSelectedMeshCount = 0;
    private int mSelectedVerticesCount = 0;

    /// <summary>
    /// Tracks the materials and mesh filters associated with them
    /// </summary>
    private List<MaterialMeshes> mMaterialMeshes = new List<MaterialMeshes>();

    /// <summary>
    /// Parent to create the merged mesh under
    /// </summary>
    private GameObject mMergedParent = null;

    /// <summary>
    /// Options for what to do after the  merge happens
    /// </summary>
    private List<string> mExecutionOptions = new List<string>();

    /// <summary>
    /// Currently selected execution option
    /// </summary>
    private int mExecutionOptionIndex = 1;

    /// <summary>
    /// Initial text to display
    /// </summary>
    private string mHelpText = "Select multiple source objects in the scene and merge their meshes to reduce draw calls.";

    /// <summary>
    /// Hold a reference to the style
    /// </summary>
    private GUIStyle mTextStyle = null;

    /// <summary>
    /// Hold a reference to the style
    /// </summary>
    private GUIStyle mWindowStyle = null;

    /// <summary>
    /// Hold a reference to the style for the bottom help text
    /// </summary>
    private GUIStyle mHelpStyle = null;

    /// <summary>
    /// Track the position of the button
    /// </summary>
    private Rect mButtonRect;

    /// <summary>
    /// Amount of progress we've made
    /// </summary>
    private float mProgress = 0f;

    /// <summary>
    /// Text to display as we're working
    /// </summary>
    private string mProgressText = "";

    /// <summary>
    /// Track the progress of the processing and report it
    /// </summary>
    private float mProgressVertexCount = 0;
    private float mProgressVertexIndex = 0;


    /// <summary>
    /// Add menu item named "ootii Tools" to the Window menu and
    /// add the "Mesh Merge" component to it.
    /// </summary>
    [MenuItem("Window/ootii Tools/Mesh Merge")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        EditorWindow lWindow = EditorWindow.GetWindow(typeof(MeshMergeWindow));
        lWindow.autoRepaintOnSceneChange = true;
        lWindow.titleContent.text = "Mesh Merge";
        lWindow.Show();
    }

    /// <summary>
    /// Called when the window is activated
    /// </summary>
    public void OnEnable()
    {
        OnSelectionChange();

        mExecutionOptions.Clear();
        mExecutionOptions.Add("Do nothing");
        mExecutionOptions.Add("Disable source meshes");
        mExecutionOptions.Add("Disable source objects");
        mExecutionOptions.Add("Delete source objects");
    }

    /// <summary>
    /// Frame update for GUI objects. Heartbeat of the window that 
    /// allows us to update the UI
    /// </summary>
    public void OnGUI()
    {
        // Determine the selection count
        string lSelectionHelp = "Selected Objects: " + mSelectedCount;
        lSelectionHelp += "\r\n" + "Selected Materials: " + mSelectedMaterialCount;
        lSelectionHelp += "\r\n" + "Selected Meshes: " + mSelectedMeshCount + " (" + mSelectedVerticesCount + " vertices)";

        if (mSelectedCount == 0)
        {
            mProgressText = "";
        }
        else if (mProgress != 1)
        {
            mProgressText = "Estimated processing time: " + (mSelectedVerticesCount / 500f).ToString("0.00 sec");
        }

        // Styles
        if (mWindowStyle == null)
        {
            mWindowStyle = new GUIStyle(GUI.skin.window);
            mWindowStyle.margin = new RectOffset(10, 10, 10, 10);
        }

        if (mTextStyle == null)
        {
            mTextStyle = new GUIStyle(GUIStyle.none);
            mTextStyle.wordWrap = true;
            mTextStyle.stretchWidth = true;
            mTextStyle.fontSize = 12;
            mTextStyle.fontStyle = FontStyle.Bold;
            mTextStyle.normal.textColor = GUI.skin.window.normal.textColor;
            mTextStyle.margin = new RectOffset(10, 10, 10, 10);
        }

        //if (mHelpStyle == null)
        {
            mHelpStyle = new GUIStyle(GUIStyle.none);
            mHelpStyle.wordWrap = true;
            mHelpStyle.stretchWidth = true;
            mHelpStyle.fontSize = 10;
            mHelpStyle.fontStyle = FontStyle.Normal;
            mHelpStyle.normal.textColor = Color.gray;
            mHelpStyle.margin = new RectOffset(10, 10, 10, 10);
        }

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField(mHelpText, mTextStyle);
        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("", mWindowStyle, GUILayout.Height(160));

        // Explain this component
        EditorGUILayout.HelpBox(lSelectionHelp, MessageType.Info, true);

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.EndVertical();

        // Determines if we generage the second set of UV2
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Build UV2", "Determines if we generate the second set of UVs for lightmapping"), GUILayout.Width(80));

        mBuildUV2 = EditorGUILayout.Toggle(mBuildUV2, GUILayout.Width(20));

        EditorGUILayout.EndHorizontal();

        // Determine what we do after the merge
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("After merge", "Choose what to do with the source objects"), GUILayout.Width(80));

        mExecutionOptionIndex = EditorGUILayout.Popup(mExecutionOptionIndex, mExecutionOptions.ToArray(), GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();

        // Merge button
        if (GUILayout.Button("Splice and Merge") && mSelectedObjects != null)
        {
            mProgress = 0f;
            mProgressVertexIndex = 0f;
            mProgressVertexCount = 0f;

            GameObject[] lMergedMeshes = MergeMaterialMeshes();

            // Disable the mesh renderer
            if (mExecutionOptionIndex == 1)
            {
                EnableSourceMeshes(false);
            }
            // Disable the objects
            else if (mExecutionOptionIndex == 2)
            {
                EnableMergedObjects(false);
            }
            // Delete the objects
            else if (mExecutionOptionIndex == 3)
            {
                DeleteMergedObjects();
            }

            // Select the new meshes
            Selection.objects = lMergedMeshes;

            // Force a reprocessing of the selection
            OnSelectionChange();

            // Set the progress
            mProgress = 1f;
            mProgressVertexIndex = 0f;
            mProgressVertexCount = 0f;
            mProgressText = "done";
        }

        if (Event.current.type == EventType.Repaint)
        {
            mButtonRect = GUILayoutUtility.GetLastRect();
        }

        EditorGUI.ProgressBar(new Rect(15, mButtonRect.yMax + 10, Screen.width - 30, 20), mProgress, mProgressText);

        EditorGUILayout.EndVertical();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("* One Merged Mesh is created per material.", mHelpStyle);
        GUILayout.Space(5);
        GUILayout.EndVertical();
    }

    public void OnUpdate()
    {
    }

    /// <summary>
    /// Called when the user changes the selection in the hierarchy or in the scene
    /// </summary>
    public void OnSelectionChange()
    {
        // Grab the currently selected items
        mSelectedObjects = Selection.gameObjects;

        // Reset the materials
        mMaterialMeshes.Clear();

        // Extract the info of the objects we're merging
        ExtractCounts();

        // Reset the progress if we're not selecting a merged mesh
        bool lClearProgress = true;
        if (Selection.activeObject is GameObject)
        {
            if (((GameObject)Selection.activeObject).GetComponent<MergedMesh>() != null)
            {
                lClearProgress = false;
            }
        }

        if (lClearProgress)
        {
            mProgress = 0f;
            mProgressText = "";
        }

        // Force the OnGUI function to render out the window
        Repaint();
    }

    /// <summary>
    /// Extract out the total counts we'll be merging
    /// </summary>
    private void ExtractCounts()
    {
        mSelectedCount = 0;
        mSelectedMaterialCount = 0;
        mSelectedMeshCount = 0;
        mSelectedVerticesCount = 0;

        if (mSelectedObjects == null || mSelectedObjects.Length == 0) { return; }

        for (int i = 0; i < mSelectedObjects.Length; i++)
        {
            ExtractObjectCounts(mSelectedObjects[i]);

            MeshFilter lMeshFilter = mSelectedObjects[i].GetComponent<MeshFilter>();
            if (lMeshFilter != null) { mSelectedVerticesCount += lMeshFilter.sharedMesh.vertexCount; }
        }

        mSelectedCount = mSelectedObjects.Length;
        mSelectedMaterialCount = mSelectedMaterials.Count;

        mSelectedMaterials.Clear();
    }

    /// <summary>
    /// Extracts out the counts for this specific object
    /// </summary>
    /// <param name="rObject">Object we're extracting info for</param>
    private void ExtractObjectCounts(GameObject rObject)
    {
        // If this object has a mesh, extract it
        MeshFilter lMeshFilter = rObject.GetComponent<MeshFilter>();
        if (lMeshFilter != null)
        {
            mSelectedMeshCount += lMeshFilter.sharedMesh.subMeshCount;
        }

        // Ensure we're only extracting rendered meshes
        MeshRenderer lMeshRenderer = rObject.GetComponent<MeshRenderer>();
        if (lMeshRenderer == null || !lMeshRenderer.enabled) { return; }

        for (int i = 0; i < lMeshRenderer.sharedMaterials.Length; i++)
        {
            if (!mSelectedMaterials.Contains(lMeshRenderer.sharedMaterials[i]))
            {
                mSelectedMaterials.Add(lMeshRenderer.sharedMaterials[i]);
            }
        }

        // Process each of the children and extract thier meshes
        for (int i = 0; i < rObject.transform.childCount; i++)
        {
            Transform lChildTransform = rObject.transform.GetChild(i);
            GameObject lChildObject = lChildTransform.gameObject;

            if (!mSelectedObjects.Contains<GameObject>(lChildObject))
            {
                ExtractObjectCounts(lChildObject);
            }
        }
    }

    /// <summary>
    /// Extract the material and sub-mesh information so we can
    /// combine all the sub-meshes as one mesh per material.
    /// </summary>
    /// <param name="rObject"></param>
    private void ExtractMaterialMeshes(GameObject rObject)
    {
        if (rObject == null) { return; }
        if (!rObject.activeInHierarchy) { return; }

        // If this object has a mesh, extract it
        MeshFilter lMeshFilter = rObject.GetComponent<MeshFilter>();
        if (lMeshFilter == null) { return; }

        // Ensure we're only extracting rendered meshes
        MeshRenderer lMeshRenderer = rObject.GetComponent<MeshRenderer>();
        if (lMeshRenderer == null || !lMeshRenderer.enabled) { return; }

        // We need to break the mesh into it's sub mesh parts so we can assign materials
        Mesh lMesh = null;
        Material lMaterial = null;

        Mesh[] lSubMeshes = ExtractSubMeshes(lMeshFilter.sharedMesh);

        int lSubMeshCount = lMeshFilter.sharedMesh.subMeshCount;
        for (int i = 0; i < lSubMeshCount; i++)
        {
            // This is easy, use the shared instance
            if (i == 0)
            {
                lMesh = lMeshFilter.sharedMesh;
                lMaterial = lMeshRenderer.sharedMaterial;
            }
            // Process each sub-mesh individually
            else
            {
                // Create a mesh from the sub-mesh
                //lMesh = CreateMesh(lMeshFilter.sharedMesh, i);
                lMesh = lSubMeshes[i];

                // Find the material it's using
                if (lMeshRenderer.sharedMaterials.Length > i)
                {
                    lMaterial = lMeshRenderer.sharedMaterials[i];
                }
                else
                {
                    lMaterial = lMeshRenderer.sharedMaterials[lMeshRenderer.sharedMaterials.Length - 1];
                }
            }

            // Find the materials holder
            MaterialMeshes lMaterialMeshes = null;
            for (int j = 0; j < mMaterialMeshes.Count; j++)
            {
                if (mMaterialMeshes[j].Material == lMaterial)
                {
                    lMaterialMeshes = mMaterialMeshes[j];
                    break;
                }
            }

            // Create the material holder if we need to
            if (lMaterialMeshes == null)
            {
                lMaterialMeshes = new MaterialMeshes();
                lMaterialMeshes.Material = lMaterial;

                mMaterialMeshes.Add(lMaterialMeshes);
            }

            // Add this sub-mesh to the material holder
            lMaterialMeshes.Meshes.Add(lMesh);
            lMaterialMeshes.MeshFilters.Add(lMeshFilter);
            lMaterialMeshes.Transform.Add(lMeshFilter.transform.localToWorldMatrix);
        }

        // Process each of the children and extract thier meshes
        for (int i = 0; i < rObject.transform.childCount; i++)
        {
            Transform lChildTransform = rObject.transform.GetChild(i);
            GameObject lChildObject = lChildTransform.gameObject;

            ExtractMaterialMeshes(lChildObject);
        }
    }

    /// <summary>
    /// Given a valid set of MeshFilters from the selected GameObjects, we'll
    /// create a single merged mesh
    /// </summary>
    private GameObject[] MergeMaterialMeshes()
    {
        List<GameObject> lMergedMeshes = new List<GameObject>();

        if (mSelectedObjects != null && mSelectedObjects.Length > 0)
        {
            mMergedParent = null;
            if (mSelectedObjects[0].transform.parent != null)
            {
                mMergedParent = mSelectedObjects[0].transform.parent.gameObject;
            }

            for (int i = 0; i < mSelectedObjects.Length; i++)
            {
                ExtractMaterialMeshes(mSelectedObjects[i]);
            }

            for (int i = 0; i < mMaterialMeshes.Count; i++)
            {
                GameObject lMergedMesh = MergeMeshes(mMaterialMeshes[i]);
                if (lMergedMesh != null) { lMergedMeshes.Add(lMergedMesh); }
            }
        }

        return lMergedMeshes.ToArray();
    }

    /// <summary>
    /// Create a seperate 'merged object' for the specific material.
    /// </summary>
    /// <param name="rMaterialMeshFilters"></param>
    private GameObject MergeMeshes(MaterialMeshes rMaterialMeshes)
    {
        if (rMaterialMeshes == null) { return null; }
        if (rMaterialMeshes.Meshes.Count == 0) { return null; }

        int lMeshCount = rMaterialMeshes.Meshes.Count;

        // Determine the position of the new merged object
        Vector3 lCenter = Vector3.zero;
        for (int i = 0; i < lMeshCount; i++)
        {
            lCenter.x += rMaterialMeshes.Transform[i].m03;
            lCenter.y += rMaterialMeshes.Transform[i].m13;
            lCenter.z += rMaterialMeshes.Transform[i].m23;
        }

        lCenter /= lMeshCount;

        // Create the list of meshes that will be merged
        CombineInstance[] lMeshes = new CombineInstance[lMeshCount];
        for (int i = 0; i < lMeshCount; i++)
        {
            // Ensure our meshes are positioned relative to the center
            Matrix4x4 lMatrix = rMaterialMeshes.Transform[i];
            lMatrix.m03 -= lCenter.x;
            lMatrix.m13 -= lCenter.y;
            lMatrix.m23 -= lCenter.z;

            // Process sub meshes
            lMeshes[i].transform = lMatrix;
            lMeshes[i].mesh = rMaterialMeshes.Meshes[i];
        }

        // Create the object that will represent the new mesh
        GameObject lMergedObject = new GameObject();
        lMergedObject.name = "Merged Mesh";
        lMergedObject.transform.position = lCenter;

        // Combine the meshes in the new object
        MeshFilter lMergedMeshFilter = lMergedObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        lMergedMeshFilter.mesh = new Mesh();
        lMergedMeshFilter.sharedMesh.name = "Merged Mesh of " + lMeshCount + " items";
        lMergedMeshFilter.sharedMesh.CombineMeshes(lMeshes, mMergeSubMeshes);

        // Generate UV2 for lightmapping
        if (mBuildUV2)
        {
            Unwrapping.GenerateSecondaryUVSet(lMergedMeshFilter.sharedMesh);
        }

        // Set the material(s) for the new object
        MeshRenderer lMergedMeshRenderer = lMergedObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        lMergedMeshRenderer.sharedMaterial = rMaterialMeshes.Material;

        // Assign the parent of the first selected object's parent
        if (mMergedParent != null)
        {
            lMergedObject.transform.parent = mMergedParent.transform;
        }

        // Store the IDs that make up this object
        if (mExecutionOptionIndex < 3)
        {
            List<int> lMeshFilterIDs = new List<int>();
            for (int i = 0; i < rMaterialMeshes.MeshFilters.Count; i++)
            {
                int lInstanceID = rMaterialMeshes.MeshFilters[i].GetInstanceID();
                if (!lMeshFilterIDs.Contains(lInstanceID)) { lMeshFilterIDs.Add(lInstanceID); }
            }

            MergedMesh lMergedMesh = lMergedObject.AddComponent(typeof(MergedMesh)) as MergedMesh;
            lMergedMesh.MeshFilterIDs = lMeshFilterIDs.ToArray();
        }

        // Return the newly created game object
        return lMergedObject;
    }

    /// <summary>
    /// Once we've merged the mesh, disable the source objects so 
    /// we don't process or render them.
    /// </summary>
    private void EnableSourceMeshes(bool rEnable)
    {
        for (int i = 0; i < mMaterialMeshes.Count; i++)
        {
            MaterialMeshes lMaterialMeshes = mMaterialMeshes[i];

            if (lMaterialMeshes == null) { return; }
            if (lMaterialMeshes.MeshFilters.Count == 0) { return; }

            int lMeshFilterCount = lMaterialMeshes.MeshFilters.Count;
            for (int j = 0; j < lMeshFilterCount; j++)
            {
                MeshFilter lMeshFilter = lMaterialMeshes.MeshFilters[j];
                MeshRenderer lMeshRenderer = lMeshFilter.gameObject.GetComponent<MeshRenderer>();
                if (lMeshRenderer != null)
                {
                    lMeshRenderer.enabled = rEnable;
                }
            }
        }
    }

    /// <summary>
    /// Deletes the objects that make up the merged object
    /// </summary>
    private void EnableMergedObjects(bool rEnable)
    {
        for (int i = 0; i < mMaterialMeshes.Count; i++)
        {
            MaterialMeshes lMaterialMeshes = mMaterialMeshes[i];

            if (lMaterialMeshes == null) { return; }
            if (lMaterialMeshes.MeshFilters.Count == 0) { return; }

            int lMeshFilterCount = lMaterialMeshes.MeshFilters.Count;
            for (int j = 0; j < lMeshFilterCount; j++)
            {
                MeshFilter lMeshFilter = lMaterialMeshes.MeshFilters[j];
                MeshRenderer lMeshRenderer = lMeshFilter.gameObject.GetComponent<MeshRenderer>();
                if (lMeshRenderer != null)
                {
                    GameObject lGameObject = lMeshFilter.gameObject;
                    lGameObject.SetActive(rEnable);
                }
            }
        }
    }

    /// <summary>
    /// Deletes the objects that make up the merged object
    /// </summary>
    private void DeleteMergedObjects()
    {
        for (int i = 0; i < mMaterialMeshes.Count; i++)
        {
            MaterialMeshes lMaterialMeshes = mMaterialMeshes[i];

            if (lMaterialMeshes == null) { return; }
            if (lMaterialMeshes.MeshFilters.Count == 0) { return; }

            int lMeshFilterCount = lMaterialMeshes.MeshFilters.Count;
            for (int j = 0; j < lMeshFilterCount; j++)
            {
                MeshFilter lMeshFilter = lMaterialMeshes.MeshFilters[j];
                if (lMeshFilter != null)
                {
                    if (lMeshFilter.gameObject != null)
                    {
                        MeshRenderer lMeshRenderer = lMeshFilter.gameObject.GetComponent<MeshRenderer>();
                        if (lMeshRenderer != null)
                        {
                            GameObject lGameObject = lMeshFilter.gameObject;
                            DestroyImmediate(lGameObject);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// The most expensive part is cycling through the vertices for
    /// each of the submeshes. This function attempts to batch that.
    /// </summary>
    /// <param name="rMesh">Mesh to split</param>
    /// <returns>Submeshes that were part of the mesh</returns>
    private Mesh[] ExtractSubMeshes(Mesh rMesh)
    {
        if (rMesh == null) { return null; }
        if (rMesh.subMeshCount == 0) { return new Mesh[] { rMesh }; }

        Vector3[] lMeshVertices = rMesh.vertices;
        Vector2[] lMeshUVs = rMesh.uv;

        int lMeshVertexCount = lMeshVertices.Length;
        int lMeshUVCount = lMeshUVs.Length;

        int lSubMeshCount = rMesh.subMeshCount;
        Mesh[] lSubMeshes = new Mesh[lSubMeshCount];

        // Extract out the triangle list for each submesh and create
        // structures to hold thier data
        List<int>[] lOriginalSubMeshTriangles = new List<int>[lSubMeshCount];

        List<Vector3>[] lSubMeshVertices = new List<Vector3>[lSubMeshCount];
        List<Vector2>[] lSubMeshUVs = new List<Vector2>[lSubMeshCount];
        List<int>[] lSubMeshTriangles = new List<int>[lSubMeshCount];
        Dictionary<int, int>[] lSubMeshVertexMap = new Dictionary<int, int>[lSubMeshCount];

        for (int lSubMeshIndex = 0; lSubMeshIndex < lSubMeshCount; lSubMeshIndex++)
        {
            lOriginalSubMeshTriangles[lSubMeshIndex] = new List<int>();
            lOriginalSubMeshTriangles[lSubMeshIndex].AddRange(rMesh.GetTriangles(lSubMeshIndex));

            lSubMeshVertices[lSubMeshIndex] = new List<Vector3>();
            lSubMeshUVs[lSubMeshIndex] = new List<Vector2>();
            lSubMeshTriangles[lSubMeshIndex] = new List<int>();
            lSubMeshVertexMap[lSubMeshIndex] = new Dictionary<int, int>();
        }

        // Find the vertices for each submesh
        mProgressVertexIndex = 0;
        mProgressVertexCount = lMeshVertexCount;
        for (int lMeshVertexIndex = 0; lMeshVertexIndex < lMeshVertexCount; lMeshVertexIndex++)
        {
            for (int lSubMeshIndex = 0; lSubMeshIndex < lSubMeshCount; lSubMeshIndex++)
            {
                if (lOriginalSubMeshTriangles[lSubMeshIndex].Contains(lMeshVertexIndex))
                {
                    lSubMeshVertices[lSubMeshIndex].Add(lMeshVertices[lMeshVertexIndex]);
                    if (lMeshUVCount > lMeshVertexIndex) { lSubMeshUVs[lSubMeshIndex].Add(lMeshUVs[lMeshVertexIndex]); }

                    lSubMeshVertexMap[lSubMeshIndex].Add(lMeshVertexIndex, lSubMeshVertices[lSubMeshIndex].Count - 1);
                }
            }

            mProgressVertexIndex++;
            mProgress = mProgressVertexIndex / mProgressVertexCount;
        }

        // Now we need to build a triangle list for each submesh
        for (int lSubMeshIndex = 0; lSubMeshIndex < lSubMeshCount; lSubMeshIndex++)
        {
            int lVertexCount = lOriginalSubMeshTriangles[lSubMeshIndex].Count;
            for (int lVertexIndex = 0; lVertexIndex < lVertexCount; lVertexIndex++)
            {
                lSubMeshTriangles[lSubMeshIndex].Add(lSubMeshVertexMap[lSubMeshIndex][lOriginalSubMeshTriangles[lSubMeshIndex][lVertexIndex]]);
            }
        }

        // Finally, we can create each of the meshes
        for (int lSubMeshIndex = 0; lSubMeshIndex < lSubMeshCount; lSubMeshIndex++)
        {
            lSubMeshes[lSubMeshIndex] = new Mesh();
            lSubMeshes[lSubMeshIndex].vertices = lSubMeshVertices[lSubMeshIndex].ToArray();
            lSubMeshes[lSubMeshIndex].uv = lSubMeshUVs[lSubMeshIndex].ToArray();
            lSubMeshes[lSubMeshIndex].triangles = lSubMeshTriangles[lSubMeshIndex].ToArray();
            lSubMeshes[lSubMeshIndex].RecalculateBounds();
            lSubMeshes[lSubMeshIndex].RecalculateNormals();
        }

        return lSubMeshes;
    }
}

