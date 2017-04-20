// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Pixelate UV
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Pixelate UV", "Textures", "Pixelate Texture Modifying UV.", null, KeyCode.None, true, false, null, null, true )]
	public sealed class TFHCPixelateUV : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, true, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Pixels X" );
			AddInputPort( WirePortDataType.FLOAT, false, "Pixels Y" );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_useInternalPortData = true;
			m_previewShaderGUID = "e2f7e3c513ed18340868b8cbd0d85cfb";
		}

		public override void DrawProperties()
		{
			base.DrawProperties ();
			EditorGUILayout.HelpBox ("Pixelate UV.\n\n  - UV is the Texture Coordinates to pixelate.\n  - Pixels X is the number of horizontal pixels\n  - Pixels Y is the number of vertical pixels.", MessageType.None);

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string uv = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false );
			string PixelCount_X = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );
			string PixelCount_Y = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );

			string pixelWidth = "float pixelWidth" + m_uniqueId + " =  1.0f / " + PixelCount_X + ";";
			string pixelHeight = "float pixelHeight" + m_uniqueId + " = 1.0f / " + PixelCount_Y + ";";
			string pixelatedUV = "half2 pixelateduv" + m_uniqueId + " = half2((int)(" + uv + ".x / pixelWidth" + m_uniqueId + ") * pixelWidth" + m_uniqueId + ", (int)(" + uv + ".y / pixelHeight" + m_uniqueId + ") * pixelHeight" + m_uniqueId + ");";
			string result = "pixelateduv" + m_uniqueId;

			dataCollector.AddToLocalVariables( m_uniqueId, pixelWidth);
			dataCollector.AddToLocalVariables( m_uniqueId, pixelHeight);
			dataCollector.AddToLocalVariables( m_uniqueId, pixelatedUV);

			return GetOutputVectorItem( 0, outputId, result);

		}
	}
}
