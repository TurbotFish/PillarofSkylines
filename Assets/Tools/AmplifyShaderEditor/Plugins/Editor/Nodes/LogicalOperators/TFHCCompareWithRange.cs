// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Compare With Range
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Compare With Range", "Logical Operators", "Check if A is in the range between Range Min and Range Max. If true return value of True else return value of False", null, KeyCode.None, true, false, null, null, true )]
	public sealed class TFHCCompareWithRange : DynamicTypeNode
	{
		private WirePortDataType m_mainInputType = WirePortDataType.FLOAT;
		private WirePortDataType m_mainOutputType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "A";
			m_inputPorts[ 1 ].Name = "Range Min";
			AddInputPort( WirePortDataType.FLOAT, false, "Range Max" );
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections( portId );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections( outputPortId );
		}

		void UpdateConnections( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();
			WirePortDataType newType = WirePortDataType.FLOAT;
			//if ( portId < 3 )
			//{
			//	for ( int i = 0; i < 3; i++ )
			//	{
			//		if ( m_inputPorts[ i ].IsConnected && UIUtils.GetPriority( m_inputPorts[ i ].DataType ) > UIUtils.GetPriority( newType ) )
			//		{
			//			newType = m_inputPorts[ i ].DataType;
			//		}
			//	}
			//	m_mainInputType = newType;
			//}
			//else
			if ( portId >= 3 )
			{
				for ( int i = 3; i < 5; i++ )
				{
					if ( m_inputPorts[ i ].IsConnected && UIUtils.GetPriority( m_inputPorts[ i ].DataType ) > UIUtils.GetPriority( newType ) )
					{
						newType = m_inputPorts[ i ].DataType;
					}
				}

				if ( newType != m_mainOutputType )
				{
					m_mainOutputType = newType;
					m_outputPorts[ 0 ].ChangeType( newType, false );
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			//Check if VALUE is in range between MIN and MAX. If true return VALUE IF TRUE else VALUE IF FALSE"
			string a = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_mainInputType, ignoreLocalvar, true );
			string b = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_mainInputType, ignoreLocalvar, true );
			string c = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_mainInputType, ignoreLocalvar, true );
			string d = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, m_mainOutputType, ignoreLocalvar, true );
			string e = m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, m_mainOutputType, ignoreLocalvar, true );
			string strout = " ( ( " + a + " >= " + b + " && " + a + " <= " + c + " ) ? " + d + " :  " + e + " ) ";
			//Debug.Log(strout);
			return CreateOutputLocalVariable( 0, strout, ref dataCollector );
		}
	}
}
