// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex Binormal World
// Donated by Community Member Kebrus

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Bitangent", "Surface Standard Inputs", "Per pixel world bitangent vector" )]
	public sealed class VertexBinormalNode : ParentNode
	{
		private const string WorldBiTangentDefFrag = "WorldNormalVector( {0}, float3(0,1,0) )";
		private const string WorldBiTangentDefVert = "UnityObjectToWorldDir( {0}.tangent.xyz )";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "76873532ab67d2947beaf07151383cbe";
		}

		public override void PropagateNodeData( NodeData nodeData )
		{
			base.PropagateNodeData( nodeData );
			UIUtils.CurrentDataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );

			if ( isVertex )
			{
				if ( m_outputPorts[ 0 ].IsLocalValue )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );

				dataCollector.AddToVertexLocalVariables( m_uniqueId, string.Format( "half3 worldNormal = UnityObjectToWorldNormal( {0}.normal );", Constants.VertexShaderInputStr ) );
				dataCollector.AddToVertexLocalVariables( m_uniqueId, string.Format( "fixed3 worldTangent = UnityObjectToWorldDir( {0}.tangent.xyz );", Constants.VertexShaderInputStr ) );
				dataCollector.AddToVertexLocalVariables( m_uniqueId, string.Format( "fixed tangentSign = {0}.tangent.w * unity_WorldTransformParams.w;", Constants.VertexShaderInputStr ) );

				RegisterLocalVariable( 0, "cross( worldNormal, worldTangent ) * tangentSign", ref dataCollector, "worldBitangent" + m_uniqueId );
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
			else
			{

				if ( m_outputPorts[ 0 ].IsLocalValue )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );

				RegisterLocalVariable( 0, string.Format( WorldBiTangentDefFrag, Constants.InputVarStr ), ref dataCollector, "worldBiTangentFrag" + m_uniqueId );

				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
		}
	}
}
