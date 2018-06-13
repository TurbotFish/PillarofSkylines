using UnityEngine;
using System.Collections.Generic;
using Game.World;
using Game.GameControl;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GPUISeeder : MonoBehaviour, IWorldObject
{
    //==========================================================================================

    [SerializeField, HideInInspector] List<Matrix4x4> transforms = new List<Matrix4x4>();

    //public Mesh meshToDraw;
    //public Material materialToDraw;

    GameController gameController;
    int instanceID;

    Color gizmoColor = Color.clear;

    [SerializeField, HideInInspector] Matrix4x4 matrix0;

    //public Color grassColor;
    SurfaceTextureHolder surfaceTexHolder;
	SurfaceGrassScaleVariationMaps scaleVariationMaps;

    Color colorOK = new Color(.3f, .65f, 1f, .5f);
    Color colorRebake = new Color(1f, .6f, .2f, .5f);
    Color colorNeverBaked = new Color(1f, .31f, .31f, .5f);

    [HideInInspector] public Color _paintColor = new Color(.1f, .72f, .03f);
    [HideInInspector] public bool _colorsWerePainted;

	[HideInInspector] public Vector3 scaleMultiplier = Vector3.one;
	[SerializeField, HideInInspector] Vector3 currentScaleMultiplier;

    public enum GrowthMode { Mesh, StraightUp };
    public GrowthMode grassRotation = GrowthMode.Mesh;

    private bool isVisible;
    private bool isInitialized;

    //==========================================================================================

    void IWorldObject.Initialize(GameController gameController)
    {
        this.gameController = gameController;

        instanceID = transform.GetInstanceID();
        isInitialized = true;

        if (isVisible)
        {
            OnBecameVisible();
        }
    }

    //==========================================================================================

    void OnBecameVisible()
    {
        if (isInitialized)
        {
            gameController.CameraController.GPUIDisplayManager.AddStuffToDraw(transforms, instanceID);
        }

        isVisible = true;
        //Debug.Log (transform.name + "    " + transform.GetInstanceID() + "      " + Time.frameCount + "    VISIBLE");


		//ALO: toggle activate lines to try out in editor
		//instanceID = transform.GetInstanceID();
		//GPUIDisplayManager.displayManager.AddStuffToDraw(transforms, instanceID);
    }

    void OnBecameInvisible()
    {
        if (isInitialized && isVisible)
        {
			gameController.CameraController.GPUIDisplayManager.RemoveStuffToDraw(transforms, instanceID);
        }

        isVisible = false;
        //Debug.Log (transform.name + "    " + transform.GetInstanceID() + "      " + Time.frameCount + "    INVISIBLE");


    }

    void OnDisable()
    {
        if (isInitialized && isVisible)
        {
			gameController.CameraController.GPUIDisplayManager.RemoveStuffToDraw(transforms, instanceID);
        }

        isVisible = false;
        //Debug.Log (transform.name + "    " + transform.GetInstanceID() + "      " + Time.frameCount + "    DESTROYED");
    }
		

    //==========================================================================================   

    public void BakeGPUIData()
    {
        transforms.Clear();


        Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
        Matrix4x4 objToWorld = transform.localToWorldMatrix;
        Quaternion _rotation = transform.rotation;
        Vector3 _vec = transform.up;

        if (grassRotation == GrowthMode.StraightUp)
        {
            _rotation = transform.position.x < 0f ? Quaternion.Euler(Vector3.back * 90f) : Quaternion.Euler(Vector3.forward * 90f);
            _vec = Vector3.right;
        }

        for (int i = 0; i < _mesh.vertexCount; i++)
        {
			if (_mesh.colors.Length == 0 || _mesh.colors [i].a == 0) {
				Vector3 newPos = objToWorld.MultiplyPoint (_mesh.vertices [i]);
				Quaternion newRot = Quaternion.AngleAxis (SuperRandomnessRotation (instanceID, i), _vec) * _rotation;
				Vector3 newScale = MapDrivenScaleVariation (objToWorld.MultiplyPoint (_mesh.vertices [i])) * SuperRandomnessScale (instanceID, _mesh.vertexCount, i);


				transforms.Add (Matrix4x4.TRS (newPos, newRot, newScale));
			}
        }
		currentScaleMultiplier = scaleMultiplier;
		matrix0 = Matrix4x4.TRS(objToWorld.MultiplyPoint(_mesh.vertices[0]), Quaternion.AngleAxis(SuperRandomnessRotation(instanceID, 0), _vec) * _rotation, MapDrivenScaleVariation(objToWorld.MultiplyPoint(_mesh.vertices[0])) * SuperRandomnessScale(instanceID, _mesh.vertexCount, 0));


#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

	//micro scale variations
    float SuperRandomnessScale(int _id, int _vertCount, int _vertIndex)
    {
        float rng = 1f + ((float)_id % 10f) * .1f * .2f + (((float)_vertIndex % 10f) / 10f) * .6f - (.6f * .5f);
        return rng;
    }

    float SuperRandomnessRotation(int _id, int _vertIndex)
    {
        //returns a value between 0 and 360
        float rng = 360f * (.5f * ((float)_id % 10) * .1f + ((float)_vertIndex % 10f) * .1f * .5f);
        return rng;
    }

	Vector3 MapDrivenScaleVariation(Vector3 _vertPos){
		if(scaleVariationMaps == null)
			scaleVariationMaps = (SurfaceGrassScaleVariationMaps)Resources.Load ("ScriptableObjects/GrassVariationMaps");

		if (scaleVariationMaps == null) 
			Debug.LogError ("The scriptableobject 'GrassVariationMaps' couldn't be found at Resources/ScriptableObjects");
		

		Texture2D _tex = _vertPos.x < 0 ? scaleVariationMaps.WestMap : scaleVariationMaps.EastMap;
		int _mapRes = scaleVariationMaps.mapResolution;

		//reverse u of texture if on east surface
		float _U = _vertPos.x < 0 ? (_vertPos.z + 250f) / 500f : 1f - (_vertPos.z + 250f) / 500f;
		Vector2 _wposToUV = new Vector2 (_U * _mapRes, ((_vertPos.y + 250f) / 500f) * _mapRes);

		Color _pixelCol = _tex.GetPixel((int)_wposToUV.x, (int)_wposToUV.y);

		Vector3 _scaleFromCol = new Vector3 (_pixelCol.r * 2.5f + .5f, _pixelCol.g * 2.5f + .5f, _pixelCol.b * 2.5f + .5f);

		return _scaleFromCol;
	}

    public int AreMatricesUpToDate()
    {
        //0 : baked
        //1 : rebake needed
        //2 : never baked
        int bakingState = 0;

        if (matrix0 == Matrix4x4.zero)
        {
            gizmoColor = colorNeverBaked;
            bakingState = 2;
        }
        else
        {
            Matrix4x4 _objToWorld = transform.localToWorldMatrix;
            Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
            Quaternion _rotation = transform.rotation;

            if (grassRotation == GrowthMode.StraightUp)
            {
                _rotation = transform.position.x < 0f ? Quaternion.Euler(Vector3.back * 90f) : Quaternion.Euler(Vector3.forward * 90f);
            }

			Matrix4x4 _current0 = Matrix4x4.TRS(_objToWorld.MultiplyPoint(_mesh.vertices[0]), _rotation, MapDrivenScaleVariation(_objToWorld.MultiplyPoint(_mesh.vertices[0])) * SuperRandomnessScale(instanceID, _mesh.vertexCount, 0));

            gizmoColor = matrix0 == _current0 ? colorOK : colorRebake;
			gizmoColor = !IsScaleUpToDate () && gizmoColor == colorOK ? colorRebake : gizmoColor;
            bakingState = matrix0 == _current0 ? 0 : 1;
        }

        return bakingState;
    }


    public void PaintSurfaceTexture(bool _vertexPainted, Color _grassColor)
    {
		if (surfaceTexHolder == null)
			surfaceTexHolder = (SurfaceTextureHolder)Resources.Load ("ScriptableObjects/GrassColorMaps");
		
        

        if (surfaceTexHolder == null)
        {
            Debug.LogError("GrassColorMaps scriptable object could not be found at resources/scriptableobjects.");
            return;
        }

		int _mapRes = surfaceTexHolder.mapResolution;

        Vector3 v3Holder = Vector3.zero;
        Vector2 v2Holder = Vector2.zero;
        //get worldpos of vertices
        //get vertex color
        //map worldpos to uvpos
        //paint pixels

        Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
        Matrix4x4 _objToWorld = transform.localToWorldMatrix;

        if (_vertexPainted)
        {
            if (_mesh.colors.Length == 0)
            {
                Debug.LogError("The mesh you're trying to paint with has no vertex colour.");
            }
            else
            {

                for (int i = 0; i < _mesh.vertexCount; i++)
                {
                    if (_mesh.colors[i].a != 1f)
                    {
                        v3Holder = _objToWorld.MultiplyPoint(_mesh.vertices[i]);
                        v2Holder = new Vector2(((v3Holder.z + 250f) / 500) * _mapRes, ((v3Holder.y + 250f) / 500) * _mapRes);

                        if (transform.position.x < 0f)
                        {
                            surfaceTexHolder.westTex.SetPixel((int)v2Holder.x, (int)v2Holder.y, _mesh.colors[i]);
                        }
                        else
                        {
                            surfaceTexHolder.eastTex.SetPixel((int)v2Holder.x, (int)v2Holder.y, _mesh.colors[i]);
                        }
                    }
                }
#if UNITY_EDITOR
                if (transform.position.x < 0f)
                {
                    surfaceTexHolder.westTex.Apply();
                    SaveTexture(surfaceTexHolder.westTex, AssetDatabase.GetAssetPath(surfaceTexHolder.westTex));
                }
                else
                {
                    surfaceTexHolder.eastTex.Apply();
                    SaveTexture(surfaceTexHolder.eastTex, AssetDatabase.GetAssetPath(surfaceTexHolder.eastTex));
                }
#endif
            }
        }
        else
        {
            for (int i = 0; i < _mesh.vertexCount; i++)
            {
                v3Holder = _objToWorld.MultiplyPoint(_mesh.vertices[i]);
                v2Holder = new Vector2(((v3Holder.z + 250f) / 500) * _mapRes, ((v3Holder.y + 250f) / 500) * _mapRes);

                if (transform.position.x < 0f)
                {
                    surfaceTexHolder.westTex.SetPixel((int)v2Holder.x, (int)v2Holder.y, _grassColor);
                }
                else
                {
                    surfaceTexHolder.eastTex.SetPixel((int)v2Holder.x, (int)v2Holder.y, _grassColor);
                }
            }
#if UNITY_EDITOR
            if (transform.position.x < 0f)
            {
                surfaceTexHolder.westTex.Apply();
                SaveTexture(surfaceTexHolder.westTex, AssetDatabase.GetAssetPath(surfaceTexHolder.westTex));
            }
            else
            {
                surfaceTexHolder.eastTex.Apply();
                SaveTexture(surfaceTexHolder.eastTex, AssetDatabase.GetAssetPath(surfaceTexHolder.eastTex));
            }
#endif
        }
    }


	public void PaintScaleVariationMap(){

		currentScaleMultiplier = scaleMultiplier;

		if(scaleVariationMaps == null)
			scaleVariationMaps = (SurfaceGrassScaleVariationMaps)Resources.Load ("ScriptableObjects/GrassVariationMaps");

		if (scaleVariationMaps == null) 
			Debug.LogError ("The scriptableobject 'GrassVariationMaps' couldn't be found at Resources/ScriptableObjects");

		int _mapRes = scaleVariationMaps.mapResolution;
		Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
		Matrix4x4 _objToWorld = transform.localToWorldMatrix;

		Vector3 wpos = Vector3.zero;
		Color scaleColor = new Color (Mathf.Clamp01 ((currentScaleMultiplier.x - .5f) / 2.5f), Mathf.Clamp01 ((currentScaleMultiplier.y - .5f) / 2.5f), Mathf.Clamp01 ((currentScaleMultiplier.z - .5f) / 2.5f), 1);

		for (int i = 0; i < _mesh.vertexCount; i++) {
			wpos = _objToWorld.MultiplyPoint (_mesh.vertices [i]);
			//reverse u of texture if on east surface
			float _U = wpos.x < 0 ? (wpos.z + 250f) / 500f : 1f - (wpos.z + 250f) / 500f;
			Vector2 _wposToUV = new Vector2 (_U * _mapRes, ((wpos.y + 250f) / 500f) * _mapRes);
			if (wpos.x < 0f) {
				scaleVariationMaps.WestMap.SetPixel ((int)_wposToUV.x, (int)_wposToUV.y, scaleColor);
			} else {
				scaleVariationMaps.EastMap.SetPixel ((int)_wposToUV.x, (int)_wposToUV.y, scaleColor);
			}
		}

		#if UNITY_EDITOR
		if (transform.position.x < 0f) {
			scaleVariationMaps.WestMap.Apply ();
			SaveTexture(scaleVariationMaps.WestMap, AssetDatabase.GetAssetPath(scaleVariationMaps.WestMap));
		} else {
			scaleVariationMaps.EastMap.Apply ();
			SaveTexture(scaleVariationMaps.EastMap, AssetDatabase.GetAssetPath(scaleVariationMaps.EastMap));
		}
		#endif


	}

	public bool IsScaleUpToDate(){
		return currentScaleMultiplier == scaleMultiplier;
	}

	public void SetToMapValue(){
		scaleMultiplier = currentScaleMultiplier;
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireCube(transform.position, Vector3.one * 3f);

        Color _cubeColor = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, .2f);
        Gizmos.color = _cubeColor;

        Gizmos.DrawCube(transform.position, Vector3.one * 3f);
    }

#if UNITY_EDITOR
    void SaveTexture(Texture2D _tex, string _path)
    {
        byte[] bytes = _tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(_path, bytes);
    }
#endif
}