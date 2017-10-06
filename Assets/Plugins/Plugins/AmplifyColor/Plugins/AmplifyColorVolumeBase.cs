// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using UnityEngine;
using System.Collections;
using AmplifyColor;

[AddComponentMenu( "" )]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	public Texture2D LutTexture;
	public float Exposure = 1.0f;
	public float EnterBlendTime = 1.0f;
	public int Priority = 0;
	public bool ShowInSceneView = true;

	[HideInInspector] public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();

	void OnDrawGizmos()
	{
		if ( ShowInSceneView )
		{
			BoxCollider box = GetComponent<BoxCollider>();
			BoxCollider2D box2d = GetComponent<BoxCollider2D>();

			if ( box != null || box2d != null )
			{
				Vector3 center, size;
				if ( box != null )
				{
					center = box.center;
					size = box.size;
				}
				else
				{
				#if UNITY_4
					center = box2d.center;
				#else
					center = box2d.offset;
				#endif
					size = box2d.size;
				}

				Gizmos.color = Color.green;
				Gizmos.DrawIcon( transform.position, "lut-volume.png", true );
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube( center, size );
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		BoxCollider box = GetComponent<BoxCollider>();
		BoxCollider2D box2d = GetComponent<BoxCollider2D>();
		if ( box != null || box2d != null )
		{
			Color col = Color.green;
			col.a = 0.2f;
			Gizmos.color = col;
			Gizmos.matrix = transform.localToWorldMatrix;

			Vector3 center, size;
			if ( box != null )
			{
				center = box.center;
				size = box.size;
			}
			else
			{
			#if UNITY_4
				center = box2d.center;
			#else
				center = box2d.offset;
			#endif
				size = box2d.size;
			}
			Gizmos.DrawCube( center, size );
		}
	}
}
