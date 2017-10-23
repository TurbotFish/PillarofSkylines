// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Reflection", "Surface Data", "Per pixel world reflection vector, accepts a <b>Normal</b> vector in tangent space (ie: normalmap)", null, KeyCode.R )]
	public sealed class WorldReflectionVector : ParentNode
	{
		private const string ReflectionVecValStr = "newWorldReflection";
		private const string ReflectionVecDecStr = "float3 {0} = {1};";

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "8e267e9aa545eeb418585a730f50273e";
			//UIUtils.AddNormalDependentCount();
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		//public override void Destroy()
		//{
		//	ContainerGraph.RemoveNormalDependentCount();
		//	base.Destroy();
		//}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			if( m_inputPorts[ 0 ].IsConnected )
				dataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( dataCollector.IsTemplate )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_outputPorts[ 0 ].IsLocalValue )
						return m_outputPorts[ 0 ].LocalValue;


					string value = dataCollector.TemplateDataCollectorInstance.GetWorldReflection( m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
					RegisterLocalVariable( 0, value, ref dataCollector, "worldRefl" + OutputId );
					return m_outputPorts[ 0 ].LocalValue;
				}
				else
				{
					return GetOutputVectorItem( 0, outputId, dataCollector.TemplateDataCollectorInstance.GetWorldReflection() );
				}
			}

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );
			if( isVertex )
			{
				if( m_inputPorts[ 0 ].IsConnected )
				{
					if( m_outputPorts[ 0 ].IsLocalValue )
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

					string normal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					string tangent = GeneratorUtils.GenerateWorldTangent( ref dataCollector, UniqueId );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3x3 tangentToWorld = CreateTangentToWorldPerVertex( " + normal + ", "+ tangent + ", "+ Constants.VertexShaderInputStr + ".tangent.w );" );
					string inputTangent = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 tangentNormal" + OutputId + " = " + inputTangent + ";" );

					string viewDir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );
					dataCollector.AddToVertexLocalVariables( UniqueId, "float3 modWorldNormal" + OutputId + " = ( tangentToWorld[0] * tangentNormal" + OutputId + ".x + tangentToWorld[1] * tangentNormal" + OutputId + ".y + tangentToWorld[2] * tangentNormal" + OutputId + ".z);" );

					RegisterLocalVariable( 0, "reflect( -" + viewDir + ", modWorldNormal"+ OutputId + " )", ref dataCollector, "modReflection" + OutputId );
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
				}
				else
				{
					if( m_outputPorts[ 0 ].IsLocalValue )
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

					string worldNormal = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
					string viewDir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId );

					RegisterLocalVariable( 0, "reflect( -"+ viewDir + ", "+ worldNormal + " )", ref dataCollector, ReflectionVecValStr + OutputId );
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
				}
			}
			else
			{
				if( m_outputPorts[ 0 ].IsLocalValue )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
				
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_REFL ), true );
				
				string result = string.Empty;
				if( m_inputPorts[ 0 ].IsConnected )
				{
					dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
					dataCollector.ForceNormal = true;

					result = "WorldReflectionVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar ) + " )";

					int connCount = 0;
					for( int i = 0; i < m_outputPorts.Count; i++ )
					{
						connCount += m_outputPorts[ i ].ConnectionCount;
					}

					if( connCount > 1 || outputId > 0 )
					{
						dataCollector.AddToLocalVariables( UniqueId, string.Format( ReflectionVecDecStr, ReflectionVecValStr + OutputId, result ) );

						RegisterLocalVariable( 0, result, ref dataCollector, ReflectionVecValStr + OutputId );
						return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
					}
				}
				else
				{
					dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
					result = GeneratorUtils.GenerateWorldReflection( ref dataCollector, UniqueId );
				}

				return GetOutputVectorItem( 0, outputId, result );
				//RegisterLocalVariable( 0, result, ref dataCollector, "worldrefVec" + OutputId );
				//return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
		}

	}
}
