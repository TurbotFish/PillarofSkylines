// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex To Fragment
// Donated by Jason Booth - http://u3d.as/DND

using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex To Fragment", "Miscellaneous", "Pass vertex data to the pixel shader", null, KeyCode.None, true, false, null, null, "Jason Booth - http://u3d.as/DND" )]
	public sealed class VertexToFragmentNode : SingleInputOp
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "(VS) In";
			m_outputPorts[ 0 ].Name = "Out";
			m_useInternalPortData = false;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			//TEMPLATES
			if( dataCollector.IsTemplate )
			{
				if( !dataCollector.IsFragmentCategory )
					return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

				string varName = "vertexToFrag" + OutputId;
				if( dataCollector.TemplateDataCollectorInstance.HasCustomInterpolatedData( varName ) )
					return varName;

				MasterNodePortCategory category = dataCollector.PortCategory;
				dataCollector.PortCategory = MasterNodePortCategory.Vertex;
				string data = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
				dataCollector.PortCategory = category;

				dataCollector.TemplateDataCollectorInstance.RegisterCustomInterpolatedData( varName, m_inputPorts[ 0 ].DataType, m_currentPrecisionType, data );
				//return varName;

				m_outputPorts[ 0 ].SetLocalValue( varName );
				return m_outputPorts[ 0 ].LocalValue;
			}


			//SURFACE 

			if( !dataCollector.IsFragmentCategory )
			{
				return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			}

			if( dataCollector.TesselationActive )
			{
				UIUtils.ShowMessage( "Unable to use Vertex to Frag when Tessellation is active" );
				return m_outputPorts[ 0 ].ErrorValue;
			}

			string tpName = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[ 0 ].DataType );

			string interpName = "data" + OutputId;
			dataCollector.AddToInput( UniqueId, tpName + " " + interpName, true );

			MasterNodePortCategory portCategory = dataCollector.PortCategory;
			dataCollector.PortCategory = MasterNodePortCategory.Vertex;

			bool dirtyVertexVarsBefore = dataCollector.DirtyVertexVariables;

			ContainerGraph.ResetNodesLocalVariablesIfNot( this, MasterNodePortCategory.Vertex );

			string vertexVarValue = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			dataCollector.AddLocalVariable( UniqueId, Constants.VertexShaderOutputStr + "." + interpName, vertexVarValue + ";" );
			
			dataCollector.PortCategory = portCategory;

			if( !dirtyVertexVarsBefore && dataCollector.DirtyVertexVariables )
			{
				dataCollector.AddVertexInstruction( dataCollector.VertexLocalVariables, UniqueId, false );
				dataCollector.ClearVertexLocalVariables();
				ContainerGraph.ResetNodesLocalVariablesIfNot( this, MasterNodePortCategory.Vertex );
			}

			ContainerGraph.ResetNodesLocalVariablesIfNot( this, MasterNodePortCategory.Fragment );

			//return Constants.InputVarStr + "." + interpName;

			m_outputPorts[ 0 ].SetLocalValue( Constants.InputVarStr + "." + interpName );
			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
