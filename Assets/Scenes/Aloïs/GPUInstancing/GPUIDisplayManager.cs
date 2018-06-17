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

	private Dictionary<int, List<List<Matrix4x4>>> MatrixGroupDictionary = new Dictionary<int, List<List<Matrix4x4>>>();
	private List<int> MatrixGroupIdList = new List<int>();

	private Texture2D EastMap;
	private Texture2D WestMap;

	private int EastLayer;
	private int WestLayer;
	private int CurrentLayer;
	private Texture2D ColorVariationMap;

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
	}

	//##################################################################

	// OPERATIONS

	/// <summary>
	/// Adds stuff to draw.
	/// </summary>
	/// <param name="matrix_list"></param>
	/// <param name="matrix_group_id"></param>
	public void AddStuffToDraw(List<Matrix4x4> matrix_list, int matrix_group_id)
	{
		if (!MatrixGroupIdList.Contains(matrix_group_id) && matrix_list.Count > 0)
		{
			MatrixGroupDictionary.Add(matrix_group_id, new List<List<Matrix4x4>>());

			List<Matrix4x4> current_sub_list = new List<Matrix4x4>();
			for (int matrix_index = 0; matrix_index < matrix_list.Count; matrix_index++)
			{
				if (current_sub_list.Count == 1023) {
					MatrixGroupDictionary [matrix_group_id].Add (current_sub_list);
					current_sub_list = new List<Matrix4x4> ();
				}

				current_sub_list.Add(matrix_list[matrix_index]);

				if (matrix_index == matrix_list.Count - 1) {
					MatrixGroupDictionary [matrix_group_id].Add (current_sub_list);
				}
			}

			MatrixGroupIdList.Add(matrix_group_id);
		}
	}

	/// <summary>
	/// Removes stuff to draw.
	/// </summary>
	/// <param name="matrix_group_id"></param>
	public void RemoveStuffToDraw(int matrix_group_id)
	{
		MatrixGroupDictionary.Remove(matrix_group_id);
		MatrixGroupIdList.Remove(matrix_group_id);
	}

	private void LateUpdate()
	{
		if (!IsInitialized)
		{
			return;
		}

		GPUIDraw();
	}

	private void FixedUpdate()
	{
		if (!IsInitialized)
		{
			return;
		}

		SetGPUILayer();
	}

	/// <summary>
	/// Draws the stuff.
	/// </summary>
	private void GPUIDraw()
	{
		foreach (int matrix_group_id in MatrixGroupIdList)
		{
			List<List<Matrix4x4>> matrix_sub_lists = MatrixGroupDictionary[matrix_group_id];

			foreach (List<Matrix4x4> matrix_sub_list in matrix_sub_lists)
			{
				Graphics.DrawMeshInstanced(meshToDraw, 0, materialToDraw, matrix_sub_list, null, shadowMode, false, CurrentLayer);
			}
		}
	}

	private void SetGPUILayer()
	{
		bool hasLayerChanged = false;
		float playerXPos = GameController.PlayerController.Transform.position.x;

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
