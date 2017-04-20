// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Vertex To Fragment
// Donated by Jason Booth - http://u3d.as/DND

using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex To Fragment", "Vertex Data", "Pass vertex data to the pixel shader", null, KeyCode.None, true, false, null, null, true )]
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
			string tpName = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[ 0 ].DataType );

			string interpName = "data" + m_uniqueId;
			dataCollector.AddToInput( m_uniqueId, tpName + " " + interpName, true );

			MasterNodePortCategory portCategory = dataCollector.PortCategory;
			if ( dataCollector.PortCategory != MasterNodePortCategory.Vertex && dataCollector.PortCategory != MasterNodePortCategory.Tessellation )
				dataCollector.PortCategory = MasterNodePortCategory.Vertex;

			bool dirtyVertexVarsBefore = dataCollector.DirtyVertexVariables;

			dataCollector.AddVertexInstruction( tpName + " interp" + m_uniqueId + " = " +
			m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, true ), m_uniqueId );

			dataCollector.PortCategory = portCategory;

			if ( !dirtyVertexVarsBefore && dataCollector.DirtyVertexVariables )
			{
				dataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, m_uniqueId, false );
				UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
				UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables( this );
			}

			dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + interpName + " = interp" + m_uniqueId, m_uniqueId );
			return Constants.InputVarStr + "." + interpName;

		}
	}
}
