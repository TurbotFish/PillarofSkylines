using UnityEngine;
using System.Collections.Generic;
using Game.GameControl;

public class GPUIDisplayManager : MonoBehaviour
{
    //##################################################################

    // -- CONSTANTS

    private const string EastPlaneName = "EastPlane";
    private const string WetsPlaneName = "WestPlane";

    //##################################################################

    // -- ATTRIBUTES

    [SerializeField] private Mesh meshToDraw;
    [SerializeField] private Material materialToDraw;
    [SerializeField] private UnityEngine.Rendering.ShadowCastingMode shadowMode;

    private GameController GameController;
    private bool IsInitialized;

	Dictionary<int, List<Matrix4x4>> transformsID = new Dictionary<int, List<Matrix4x4>> ();
	List<Matrix4x4> matrices = new List<Matrix4x4> ();
	List<int> indices = new List<int> ();

	List<Matrix4x4>[] matrices1023 = new List<Matrix4x4>[150];

	bool updatedThisFrame;

    private Texture2D EastMap;
    private Texture2D WestMap;

    private int EastLayer;
    private int WestLayer;
    private int CurrentLayer;
    private Texture2D ColorVariationMap;

	int numberOfCalls;
	int boundaryLow;
	int instancesPerList;

    //##################################################################

    // INITIALIZATION

    public void Initialize(GameController gameController)
    {
        GameController = gameController;

        Shader.WarmupAllShaders();

        EastLayer = LayerMask.NameToLayer(EastPlaneName);
        WestLayer = LayerMask.NameToLayer(WetsPlaneName);

        SurfaceTextureHolder _mapHolder = (SurfaceTextureHolder)Resources.Load("ScriptableObjects/GrassColorMaps");
        EastMap = _mapHolder.eastTex;
        WestMap = _mapHolder.westTex;

        IsInitialized = true;

		for (int i = 0; i < matrices1023.Length; i++) {
			matrices1023 [i] = new List<Matrix4x4> ();
		}
    }

    //##################################################################

    // OPERATIONS

    /// <summary>
    /// Adds stuff to draw.
    /// </summary>
    /// <param name="matrix_list"></param>
    /// <param name="matrix_group_id"></param>
	public void AddStuffToDraw(List<Matrix4x4> _mat, int _id){
		if (!indices.Contains(_id) && _mat.Count > 0)
		{
			transformsID.Add(_id, _mat);
			indices.Add(_id);
			updatedThisFrame = true;
			//Debug.LogFormat("GPUIDisplayManager: AddStuffToDraw: added {0} matrices!", _mat.Count);
		}
		//Debug.Log (_id);
	}

	public void RemoveStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Remove (_id);
		indices.Remove (_id);
		updatedThisFrame = true;
		//Debug.LogFormat("GPUIDisplayManager: RemoveStuffToDraw: removed {0} matrices!", _mat.Count);
	}

	void LateUpdate(){
		if (updatedThisFrame)
			RearrangeListOfObjectsToDraw ();

		SetGPUILayer ();

		GPUIDraw();
		updatedThisFrame = false;
	}

	void GPUIDraw(){

		if (matrices.Count == 0)
			return;

		for (int i = 0; i < numberOfCalls; i++) {

			Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices1023[i], null, shadowMode, false, CurrentLayer, null);
		}
	}

	void RearrangeListOfObjectsToDraw(){
		matrices.Clear ();

		for (int i = 0; i < indices.Count; i++) {
			matrices.AddRange (transformsID[indices[i]]);
		}

		numberOfCalls = matrices.Count / 1024 + 1;
		//Debug.Log (numberOfCalls);
		if (numberOfCalls > matrices1023.Length) {
			numberOfCalls = Mathf.Min (numberOfCalls, matrices1023.Length);
			//Debug.LogError ("Trying to draw too many instances of " + meshToDraw.name);
		}

		//Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);

		for (int i = 0; i < numberOfCalls; i++) {
			boundaryLow = i * 1023;
			instancesPerList = Mathf.Min (1023, matrices.Count - boundaryLow);


			matrices1023 [i].Clear ();
			matrices1023 [i].AddRange (matrices.GetRange (boundaryLow, instancesPerList));
		}
	}

   
    private void SetGPUILayer()
    {
        bool hasLayerChanged = false;
        float playerXPos = GameController.PlayerController.PlayerTransform.position.x;

        if (CurrentLayer != EastLayer && playerXPos > 0)
        {
            CurrentLayer = EastLayer;
            ColorVariationMap = EastMap;
            //materialToDraw.EnableKeyword ("_GPUI_EAST");
            hasLayerChanged = true;
        }
        else if (CurrentLayer != WestLayer && playerXPos < 0)
        {
            CurrentLayer = WestLayer;
            ColorVariationMap = WestMap;
            //materialToDraw.DisableKeyword ("_GPUI_EAST");
            hasLayerChanged = true;
        }

        if (hasLayerChanged)
        {
            Shader.SetGlobalTexture("_GPUIColorMap", ColorVariationMap);
        }
    }

    //##################################################################
}
