// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Space Light Dir", "Forward Render", "Computes object space direction (not normalized) to light, given object space vertex position" )]
	public sealed class ObjSpaceLightDirHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ObjSpaceLightDir";
			//m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_inputPorts[ 0 ].Visible = false;
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
			m_outputPorts[ 0 ].Name = "XYZ";
			m_useInternalPortData = false;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			//return base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );

			dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_POS ), true );
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityShaderVariables );

#if UNITY_5_4_OR_NEWER
			string matrix = "unity_WorldToObject";
#else
				string matrix = "_World2Object";
#endif

			return m_funcType + "( mul( " + matrix + ", float4( " + Constants.InputVarStr + ".worldPos , 1 ) ) )";
		}
	}
}
