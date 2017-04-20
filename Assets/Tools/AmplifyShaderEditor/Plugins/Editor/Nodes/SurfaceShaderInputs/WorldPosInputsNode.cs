// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World Position", "Surface Standard Inputs", "World space position" )]
	public sealed class WorldPosInputsNode : SurfaceShaderINParentNode
	{
		[SerializeField]
		private bool m_addInstruction = false;

		public override void Reset()
		{
			base.Reset();
			m_addInstruction = true;
		}

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.WORLD_POS;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "70d5405009b31a349a4d8285f30cf5d9";
			InitialSetup();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				if ( m_addInstruction )
				{
					string precision = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT3 );
					dataCollector.AddVertexInstruction( precision + " worldPosition = mul(unity_ObjectToWorld, " + Constants.VertexShaderInputStr + ".vertex)", m_uniqueId );
					m_addInstruction = false;
				}

				return GetOutputVectorItem( 0, outputId, "worldPosition" );
			}
			else
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			}
		}
	}
}
