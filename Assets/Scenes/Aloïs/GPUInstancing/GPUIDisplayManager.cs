using UnityEngine;
using System.Collections.Generic;
using Game.GameControl;

//Ce script a été optimisé avec l'esprit de Bruno Mabille
public class GPUIDisplayManager : MonoBehaviour
{
    //##################################################################

    // -- ATTRIBUTES

    [SerializeField] private Mesh meshToDraw;
    [SerializeField] private Material materialToDraw;
    [SerializeField] private UnityEngine.Rendering.ShadowCastingMode shadowMode;

    private IGameController gameController;
    private bool isInitialized;

    //private Dictionary<int, List<Matrix4x4>> MatrixGroupDictionary = new Dictionary<int, List<Matrix4x4>>();
    private Dictionary<int, List<List<Matrix4x4>>> MatrixGroupDictionary = new Dictionary<int, List<List<Matrix4x4>>>();
    private List<int> MatrixGroupIdList = new List<int>();

    private List<Matrix4x4> matrices = new List<Matrix4x4>();

    private int numberOfCalls;
    private int boundaryLow;
    private int instancesPerList;

    private List<Matrix4x4>[] matrices1023 = new List<Matrix4x4>[150];

    private bool updatedThisFrame;

    private Texture2D eastMap;
    private Texture2D westMap;

    private string east = "EastPlane";
    private string west = "WestPlane";
    private int eastLayer;
    private int westLayer;
    private int currentLayer;
    private Texture2D colorVariationMap;

    //##################################################################

    // INITIALIZATION

    public void Initialize(IGameController gameController)
    {
        this.gameController = gameController;

        Shader.WarmupAllShaders();

        for (int i = 0; i < matrices1023.Length; i++)
        {
            matrices1023[i] = new List<Matrix4x4>();
        }

        eastLayer = LayerMask.NameToLayer(east);
        westLayer = LayerMask.NameToLayer(west);

        SurfaceTextureHolder _mapHolder = (SurfaceTextureHolder)Resources.Load("ScriptableObjects/GrassColorMaps");
        eastMap = _mapHolder.eastTex;
        westMap = _mapHolder.westTex;

        isInitialized = true;
    }

    //##################################################################

    // OPERATIONS

    public void AddStuffToDraw(List<Matrix4x4> matrix_list, int matrix_group_id)
    {
        if (!MatrixGroupIdList.Contains(matrix_group_id) && matrix_list.Count > 0)
        {
            MatrixGroupDictionary.Add(matrix_group_id, new List<List<Matrix4x4>>());

            List<Matrix4x4> current_sub_list = new List<Matrix4x4>();
            for(int matrix_index = 0; matrix_index < matrix_list.Count; matrix_index++)
            {
                if(current_sub_list.Count == 1023)
                {
                    MatrixGroupDictionary[matrix_group_id].Add(current_sub_list);
                    current_sub_list = new List<Matrix4x4>();
                }

                current_sub_list.Add(matrix_list[matrix_index]);
            }

            //MatrixGroupDictionary.Add(_id, _mat);
            MatrixGroupIdList.Add(matrix_group_id);
            updatedThisFrame = true;
            //Debug.LogFormat("GPUIDisplayManager: AddStuffToDraw: added {0} matrices!", _mat.Count);
        }
        //Debug.Log (_id);
    }

    public void RemoveStuffToDraw(int matrix_group_id)
    {
        MatrixGroupDictionary.Remove(matrix_group_id);
        MatrixGroupIdList.Remove(matrix_group_id);
        updatedThisFrame = true;
        //Debug.LogFormat("GPUIDisplayManager: RemoveStuffToDraw: removed {0} matrices!", _mat.Count);
    }

    private void LateUpdate()
    {
        if (!isInitialized)
        {
            return;
        }

        GPUIDraw();
    }

    private void FixedUpdate()
    {
        if (!isInitialized)
        {
            return;
        }

        //++++
        //if (updatedThisFrame)
        //{
        //    RearrangeListOfObjectsToDraw();
        //    updatedThisFrame = false;
        //}
        //++++

        SetGPUILayer();
    }

    private void GPUIDraw()
    {
        //+++++
        //if (matrices.Count == 0)
        //{
        //    return;
        //}
        //++++

        foreach (int matrix_group_id in MatrixGroupIdList)
        {
            List<List<Matrix4x4>> matrix_sub_lists = MatrixGroupDictionary[matrix_group_id];

            foreach(List<Matrix4x4> matrix_sub_list in matrix_sub_lists)
            {
                Graphics.DrawMeshInstanced(meshToDraw, 0, materialToDraw, matrix_sub_list, null, shadowMode, false, currentLayer);
            }
        }

        //++++
        //for (int i = 0; i < numberOfCalls; i++)
        //{
        //    Graphics.DrawMeshInstanced(meshToDraw, 0, materialToDraw, matrices1023[i], null, shadowMode, false, currentLayer, gameController.CameraController.Camera);
        //}
        //++++
    }

    //private void RearrangeListOfObjectsToDraw()
    //{
    //    matrices.Clear();

    //    for (int i = 0; i < MatrixGroupIdList.Count; i++)
    //    {
    //        matrices.AddRange(MatrixGroupDictionary[MatrixGroupIdList[i]]);
    //    }

    //    numberOfCalls = matrices.Count / 1024 + 1;
    //    //Debug.Log (numberOfCalls);
    //    if (numberOfCalls > matrices1023.Length)
    //    {
    //        numberOfCalls = Mathf.Min(numberOfCalls, matrices1023.Length);
    //        Debug.LogError("Trying to draw too many instances of " + meshToDraw.name);
    //    }

    //    //Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);

    //    for (int i = 0; i < numberOfCalls; i++)
    //    {
    //        boundaryLow = i * 1023;
    //        instancesPerList = Mathf.Min(1023, matrices.Count - boundaryLow);

    //        matrices1023[i].Clear();
    //        matrices1023[i].AddRange(matrices.GetRange(boundaryLow, instancesPerList));
    //    }
    //}

    private void SetGPUILayer()
    {
        bool hasLayerChanged = false;
        float playerXPos = gameController.PlayerController.PlayerTransform.position.x;

        if (currentLayer != eastLayer && playerXPos > 0)
        {
            currentLayer = eastLayer;
            colorVariationMap = eastMap;
            //materialToDraw.EnableKeyword ("_GPUI_EAST");
            hasLayerChanged = true;
        }
        else if (currentLayer != westLayer && playerXPos < 0)
        {
            currentLayer = westLayer;
            colorVariationMap = westMap;
            //materialToDraw.DisableKeyword ("_GPUI_EAST");
            hasLayerChanged = true;
        }

        if (hasLayerChanged)
        {
            Shader.SetGlobalTexture("_GPUIColorMap", colorVariationMap);
        }
    }

    //##################################################################
}
