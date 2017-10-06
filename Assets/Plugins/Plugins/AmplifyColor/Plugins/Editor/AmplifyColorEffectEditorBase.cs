// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using AmplifyColor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AmplifyColorEffectEditorBase : Editor
{
	SerializedProperty exposure;
	SerializedProperty useToneMapping;
	SerializedProperty useDithering;

	SerializedProperty qualityLevel;
	SerializedProperty blendAmount;
	SerializedProperty lutTexture;
	SerializedProperty lutBlendTexture;
	SerializedProperty maskTexture;
	SerializedProperty useDepthMask;
	SerializedProperty depthMaskCurve;

	SerializedProperty useVolumes;
	SerializedProperty exitVolumeBlendTime;
	SerializedProperty triggerVolumeProxy;
	SerializedProperty volumeCollisionMask;

	void OnEnable()
	{
		exposure = serializedObject.FindProperty( "Exposure" );
		useToneMapping = serializedObject.FindProperty( "UseToneMapping" );
		useDithering = serializedObject.FindProperty( "UseDithering" );

		qualityLevel = serializedObject.FindProperty( "QualityLevel" );
		blendAmount = serializedObject.FindProperty( "BlendAmount" );
		lutTexture = serializedObject.FindProperty( "LutTexture" );
		lutBlendTexture = serializedObject.FindProperty( "LutBlendTexture" );
		maskTexture = serializedObject.FindProperty( "MaskTexture" );
		useDepthMask = serializedObject.FindProperty( "UseDepthMask" );
		depthMaskCurve = serializedObject.FindProperty( "DepthMaskCurve" );

		useVolumes = serializedObject.FindProperty( "UseVolumes" );
		exitVolumeBlendTime = serializedObject.FindProperty( "ExitVolumeBlendTime" );
		triggerVolumeProxy = serializedObject.FindProperty( "TriggerVolumeProxy" );
		volumeCollisionMask = serializedObject.FindProperty( "VolumeCollisionMask" );

		if ( !Application.isPlaying )
		{
			AmplifyColorBase effect = target as AmplifyColorBase;

			bool needsNewID = string.IsNullOrEmpty( effect.SharedInstanceID );
			if ( !needsNewID )
				needsNewID = FindClone( effect );

			if ( needsNewID )
			{
				effect.NewSharedInstanceID();
				EditorUtility.SetDirty( target );
			}
		}
	}

	bool FindClone( AmplifyColorBase effect )
	{
		GameObject effectPrefab = PrefabUtility.GetPrefabParent( effect.gameObject ) as GameObject;
		PrefabType effectPrefabType = PrefabUtility.GetPrefabType( effect.gameObject );
		bool effectIsPrefab = ( effectPrefabType != PrefabType.None && effectPrefabType != PrefabType.PrefabInstance );
		bool effectHasPrefab = ( effectPrefab != null );

		AmplifyColorBase[] all = Resources.FindObjectsOfTypeAll( typeof( AmplifyColorBase ) ) as AmplifyColorBase[];
		bool foundClone = false;

		foreach ( AmplifyColorBase other in all )
		{
			if ( other == effect || other.SharedInstanceID != effect.SharedInstanceID )
			{
				// skip: same effect or already have different ids
				continue;
			}

			GameObject otherPrefab = PrefabUtility.GetPrefabParent( other.gameObject ) as GameObject;
			PrefabType otherPrefabType = PrefabUtility.GetPrefabType( other.gameObject );
			bool otherIsPrefab = ( otherPrefabType != PrefabType.None && otherPrefabType != PrefabType.PrefabInstance );
			bool otherHasPrefab = ( otherPrefab != null );

			if ( otherIsPrefab && effectHasPrefab && effectPrefab == other.gameObject )
			{
				// skip: other is a prefab and effect's prefab is other
				continue;
			}

			if ( effectIsPrefab && otherHasPrefab && otherPrefab == effect.gameObject )
			{
				// skip: effect is a prefab and other's prefab is effect
				continue;
			}

			if ( !effectIsPrefab && !otherIsPrefab && effectHasPrefab && otherHasPrefab && effectPrefab == otherPrefab )
			{
				// skip: both aren't prefabs and both have the same parent prefab
				continue;
			}

			foundClone = true;
		}

		return foundClone;
	}

	void ToggleContextTitle( SerializedProperty prop, string title )
	{
		GUILayout.Space( 5 );
		GUILayout.BeginHorizontal();
		prop.boolValue = GUILayout.Toggle( prop.boolValue, "", GUILayout.Width( 15 ) );
		GUILayout.BeginVertical();
		GUILayout.Space( 3 );
		GUILayout.Label( title );
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Camera ownerCamera = ( target as AmplifyColorBase ).GetComponent<Camera>();

		GUILayout.BeginVertical();

		if ( ownerCamera != null )
		{
			GUILayout.Label( "HDR Control", EditorStyles.boldLabel );
			GUILayout.Space( -4 );
			EditorGUILayout.PropertyField( exposure );
			EditorGUILayout.PropertyField( useToneMapping );
			EditorGUILayout.PropertyField( useDithering );
		}

		GUILayout.Label( "Color Grading", EditorStyles.boldLabel );
		GUILayout.Space( -4 );
		EditorGUILayout.PropertyField( qualityLevel );
		EditorGUILayout.PropertyField( blendAmount );
		EditorGUILayout.PropertyField( lutTexture );
		EditorGUILayout.PropertyField( lutBlendTexture );
		EditorGUILayout.PropertyField( maskTexture );
		EditorGUILayout.PropertyField( useDepthMask );
		EditorGUILayout.PropertyField( depthMaskCurve );

		GUILayout.Label( "Effect Volumes", EditorStyles.boldLabel );
		GUILayout.Space( -4 );
		EditorGUILayout.PropertyField( useVolumes );
		EditorGUILayout.PropertyField( exitVolumeBlendTime );
		EditorGUILayout.PropertyField( triggerVolumeProxy );
		EditorGUILayout.PropertyField( volumeCollisionMask );

		if ( ownerCamera != null && ( exposure.floatValue != 1.0f || useToneMapping.boolValue || useDithering.boolValue ) &&  !ownerCamera.allowHDR )
		{
			GUILayout.Space( 4 );
			EditorGUILayout.HelpBox( "HDR Control requires Camera HDR to be enabled", MessageType.Warning );
		}

		GUILayout.Space( 4 );
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}
