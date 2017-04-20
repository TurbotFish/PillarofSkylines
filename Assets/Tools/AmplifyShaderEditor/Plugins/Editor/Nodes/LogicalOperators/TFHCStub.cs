// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>


namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class TFHCStub : DynamicTypeNode
	{
		protected WirePortDataType m_mainInputType = WirePortDataType.FLOAT;
		protected WirePortDataType m_mainOutputType = WirePortDataType.FLOAT;
		protected string m_inputDataPort0 = string.Empty;
		protected string m_inputDataPort1 = string.Empty;
		protected string m_inputDataPort2 = string.Empty;
		protected string m_inputDataPort3 = string.Empty;

		
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

		public void GetInputData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputDataPort0 = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_mainInputType, ignoreLocalvar, true );
			m_inputDataPort1 = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_mainInputType, ignoreLocalvar, true );
			m_inputDataPort2 = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_mainOutputType, ignoreLocalvar, true );
			m_inputDataPort3 = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, m_mainOutputType, ignoreLocalvar, true );
		}

		void UpdateConnections( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();

			WirePortDataType newType = WirePortDataType.FLOAT;
			//if ( portId < 2 )
			//{
			//	for ( int i = 0; i < 2; i++ )
			//	{
			//		if ( m_inputPorts[ i ].IsConnected && UIUtils.GetPriority( m_inputPorts[ i ].DataType ) > UIUtils.GetPriority( newType ) )
			//		{
			//			newType = m_inputPorts[ i ].DataType;
			//		}
			//	}

			//	m_mainInputType = newType;
			//}
			//else
			if ( portId >= 2 )
			{
				for ( int i = 2; i < 4; i++ )
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
	}
}
