// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Position", "Vertex Data", "Vertex position vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class PosVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "vertex";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_drawPreviewAsSphere = true;
			m_outputPorts[ 4 ].Visible = false;
			m_previewShaderGUID = "a5c14f759dd021b4b8d4b6eeb85ac227";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			}
			else
			{
				dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
				dataCollector.AddToIncludes( m_uniqueId, Constants.UnityShaderVariables );

#if UNITY_5_4_OR_NEWER
				string matrix = "unity_WorldToObject";
#else
				string matrix = "_World2Object";
#endif

				dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "vertexPos", "mul( " + matrix + ", float4( " + Constants.InputVarStr + ".worldPos , 1 ) )" );
				return GetOutputVectorItem( 0, outputId, "vertexPos" );
			}
		}
	}
}
