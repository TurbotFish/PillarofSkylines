// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Coordinates", "Surface Standard Inputs", "Texture UV coordinates set", null, KeyCode.U )]
	public sealed class TextureCoordinatesNode : ParentNode
	{

		private const string DummyPropertyDec = "[HideInInspector] _DummyTex( \"\", 2D ) = \"white\"";
		private const string DummyUniformDec = "uniform sampler2D _DummyTex;";
		private const string DummyTexCoordDef = "uv{0}_DummyTex";
		private const string DummyTexCoordSurfDef = "float2 texCoordDummy{0} = {1}.uv{2}_DummyTex*{3} + {4};";
		private const string DummyTexCoordSurfVar = "texCoordDummy{0}";

		private readonly string[] Dummy = { string.Empty };

		private const string TilingStr = "Tiling";
		private const string OffsetStr = "Offset";
		private const string TexCoordStr = "texcoord_";

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private int m_textureCoordChannel = 0;

		[SerializeField]
		private int m_texcoordId = -1;

		[SerializeField]
		private int m_texcoordSize = 2;

		[SerializeField]
		private string m_surfaceTexcoordName = string.Empty;

		private bool m_forceNodeUpdate = false;
		private TexturePropertyNode m_referenceNode = null;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Tiling" );
			m_inputPorts[ 0 ].Vector2InternalData = new Vector2( 1, 1 );
			AddInputPort( WirePortDataType.FLOAT2, false, "Offset" );
			AddOutputVectorPorts( WirePortDataType.FLOAT2, "UV" );
			m_outputPorts[ 1 ].Name = "U";
			m_outputPorts[ 2 ].Name = "V";
			AddOutputPort( WirePortDataType.FLOAT, "W" );
			AddOutputPort( WirePortDataType.FLOAT, "T" );
			m_textLabelWidth = 90;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_inputPorts[ 0 ].Category = MasterNodePortCategory.Vertex;
			m_inputPorts[ 1 ].Category = MasterNodePortCategory.Vertex;
			UpdateOutput();
			m_previewShaderGUID = "085e462b2de441a42949be0e666cf5d2";
		}

		public override void Reset()
		{
			m_texcoordId = -1;
			m_surfaceTexcoordName = string.Empty;
		}

		void UpdateTitle()
		{
			if ( m_referenceArrayId > -1 && m_referenceNode != null )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				m_additionalContent.text = string.Format( "Value( {0} )", m_referenceNode.PropertyInspectorName );
				m_sizeIsDirty = true;
			}
			else
			{
				m_additionalContent.text = string.Empty;
				m_sizeIsDirty = true;
			}
		}

		void UpdatePorts()
		{
			if ( m_referenceArrayId > -1 )
			{
				m_inputPorts[ 0 ].Locked = true;
				m_inputPorts[ 1 ].Locked = true;
			}
			else
			{
				m_inputPorts[ 0 ].Locked = false;
				m_inputPorts[ 1 ].Locked = false;
			}
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			List<string> arr = new List<string>( UIUtils.TexturePropertyNodeArr() );
			bool guiEnabledBuffer = GUI.enabled;
			if ( arr != null && arr.Count > 0 )
			{
				arr.Insert( 0, "None" );
				GUI.enabled = true;
				m_referenceArrayId = EditorGUILayout.Popup( Constants.AvailableReferenceStr, m_referenceArrayId + 1, arr.ToArray() ) - 1;
			}
			else
			{
				m_referenceArrayId = -1;
				GUI.enabled = false;
				EditorGUILayout.Popup( Constants.AvailableReferenceStr, 0, Dummy );
			}

			GUI.enabled = guiEnabledBuffer;
			if ( EditorGUI.EndChangeCheck() )
			{
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if ( m_referenceNode != null )
				{
					m_referenceNodeId = m_referenceNode.UniqueId;
				}
				else
				{
					m_referenceNodeId = -1;
					m_referenceArrayId = -1;
				}

				UpdateTitle();
				UpdatePorts();
			}

			EditorGUI.BeginChangeCheck();
			m_texcoordSize = EditorGUILayout.IntPopup( Constants.AvailableUVSizesLabel, m_texcoordSize, Constants.AvailableUVSizesStr, Constants.AvailableUVSizes );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateOutput();
			}

			m_textureCoordChannel = EditorGUILayout.IntPopup( Constants.AvailableUVSetsLabel, m_textureCoordChannel, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

			if ( m_referenceArrayId > -1 )
				GUI.enabled = false;

			base.DrawProperties();

			GUI.enabled = guiEnabledBuffer;
		}

		private void UpdateOutput()
		{
			if ( m_texcoordSize == 3 )
			{
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
				m_outputPorts[ 0 ].Name = "UVW";
				m_outputPorts[ 3 ].Visible = true;
				m_outputPorts[ 4 ].Visible = false;
			}
			else if ( m_texcoordSize == 4 )
			{
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
				m_outputPorts[ 0 ].Name = "UVWT";
				m_outputPorts[ 3 ].Visible = true;
				m_outputPorts[ 4 ].Visible = true;
			}
			else
			{
				m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
				m_outputPorts[ 0 ].Name = "UV";
				m_outputPorts[ 3 ].Visible = false;
				m_outputPorts[ 4 ].Visible = false;
			}
			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_forceNodeUpdate )
			{
				m_forceNodeUpdate = false;
				if ( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as TexturePropertyNode;
					m_referenceArrayId = UIUtils.GetTexturePropertyNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
					if ( m_referenceNode != null )
					{
						m_referenceNodeId = m_referenceNode.UniqueId;
					}
				}
				UpdateTitle();
				UpdatePorts();
			}

			if ( m_referenceNode == null && m_referenceNodeId > -1 )
			{
				m_referenceNodeId = -1;
				m_referenceArrayId = -1;
				UpdateTitle();
				UpdatePorts();
			}
		}
		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_textureCoordChannel = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( UIUtils.CurrentShaderVersion() > 2402 )
			{
				if ( UIUtils.CurrentShaderVersion() > 2404 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				m_forceNodeUpdate = true;
			}

			if ( UIUtils.CurrentShaderVersion() > 5001 )
			{
				m_texcoordSize = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				UpdateOutput();
			}
		}

		public override void PropagateNodeData( NodeData nodeData )
		{
			UIUtils.SetCategoryInBitArray( ref m_category, nodeData.Category );

			MasterNodePortCategory propagateCategory = ( nodeData.Category != MasterNodePortCategory.Vertex && nodeData.Category != MasterNodePortCategory.Tessellation ) ? MasterNodePortCategory.Vertex : nodeData.Category;
			nodeData.Category = propagateCategory;
			nodeData.GraphDepth += 1;
			if ( nodeData.GraphDepth > m_graphDepth )
			{
				m_graphDepth = nodeData.GraphDepth;
			}

			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					//m_inputPorts[ i ].GetOutputNode().PropagateNodeCategory( category );
					m_inputPorts[ i ].GetOutputNode().PropagateNodeData( nodeData );
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordChannel );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceNode != null ) ? m_referenceNode.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_texcoordSize );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " cannot be used on Master Node Tessellation port" );
				return "-1";
			}

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );

			if ( m_referenceArrayId > -1 )
			{
				//TexturePropertyNode node = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				m_referenceNode = UIUtils.GetTexturePropertyNode( m_referenceArrayId );
				if ( m_referenceNode != null )
				{
					string propertyName = m_referenceNode.PropertyName;
					int coordSet = ( ( m_textureCoordChannel < 0 ) ? 0 : m_textureCoordChannel );
					string uvName = string.Empty;

					string dummyPropUV = "_tex" + ( m_texcoordSize > 2 ? "" + m_texcoordSize : "" ) + "coord" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" );
					string dummyUV = "uv" + ( coordSet > 0 ? ( coordSet + 1 ).ToString() : "" ) + dummyPropUV;

					if ( isVertex )
					{
						uvName = IOUtils.GetUVChannelName( propertyName, coordSet );
						string vertexInput = Constants.VertexShaderInputStr + ".texcoord";
						if ( coordSet > 0 )
						{
							vertexInput += coordSet.ToString();
						}
						dataCollector.AddToVertexLocalVariables( m_uniqueId, "float" + m_texcoordSize + " " + uvName + " = " + vertexInput + ";" );
						dataCollector.AddToVertexLocalVariables( m_uniqueId, uvName + ".xy = " + vertexInput + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
					}
					else
					{
						uvName = IOUtils.GetUVChannelName( propertyName, coordSet );
						if ( m_texcoordSize > 2 )
						{
							uvName += m_texcoordSize;
							dataCollector.UsingHigherSizeTexcoords = true;
							dataCollector.AddToLocalVariables( m_uniqueId, "float" + m_texcoordSize + " " + uvName + " = " + Constants.InputVarStr + "." + dummyUV + ";" );
							dataCollector.AddToLocalVariables( m_uniqueId, uvName + ".xy = " + Constants.InputVarStr + "." + dummyUV + ".xy * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
						}
						else
						{
							dataCollector.AddToLocalVariables( m_uniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvName, Constants.InputVarStr + "." + dummyUV + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
						}
					}

					dataCollector.AddToUniforms( m_uniqueId, "uniform float4 " + propertyName + "_ST;" );
					dataCollector.AddToProperties( m_uniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
					dataCollector.AddToInput( m_uniqueId, "float" + m_texcoordSize + " " + dummyUV, true );

					return GetOutputVectorItem( 0, outputId, uvName );
				}
			}

			if ( m_texcoordId < 0 )
			{

				m_texcoordId = dataCollector.AvailableUvIndex;
				string texcoordName = TexCoordStr + m_texcoordId;

				bool tessVertexMode = isVertex && dataCollector.TesselationActive;

				string uvChannel = m_textureCoordChannel == 0 ? ".xy" : m_textureCoordChannel + ".xy";

				MasterNodePortCategory portCategory = dataCollector.PortCategory;
				if ( dataCollector.PortCategory != MasterNodePortCategory.Vertex && dataCollector.PortCategory != MasterNodePortCategory.Tessellation )
					dataCollector.PortCategory = MasterNodePortCategory.Vertex;

				// We need to reset local variables if there are already created to force them to be created in the vertex function
				int buffer = m_texcoordId;
				UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables( this );

				bool dirtySpecialVarsBefore = dataCollector.DirtySpecialLocalVariables;
				bool dirtyVertexVarsBefore = dataCollector.DirtyVertexVariables;

				string tiling = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );
				string offset = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false, true );
				dataCollector.PortCategory = portCategory;

				string vertexUV = Constants.VertexShaderInputStr + ".texcoord" + uvChannel;

				if ( !tessVertexMode )
					dataCollector.AddToInput( m_uniqueId, "float" + m_texcoordSize + " " + texcoordName, true );

				bool resetLocals = false;
				// new texture coordinates are calculated on the vertex shader so we need to register its local vars
				if ( !dirtySpecialVarsBefore && dataCollector.DirtySpecialLocalVariables )
				{
					dataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, m_uniqueId, false );
					UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
					resetLocals = true;
				}

				if ( !dirtyVertexVarsBefore && dataCollector.DirtyVertexVariables )
				{
					dataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, m_uniqueId, false );
					UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
					resetLocals = true;
				}

				//Reset local variables again so they wont be caught on the fragment shader
				if ( resetLocals )
					UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables( this );

				if ( tessVertexMode )
				{
					dataCollector.AddVertexInstruction( vertexUV + " = " + vertexUV + " * " + tiling + " + " + offset, m_uniqueId );
					m_surfaceTexcoordName = Constants.VertexShaderInputStr + "." + IOUtils.GetVertexUVChannelName( m_textureCoordChannel ) + ".xy";
				}
				else
				{
					if ( dataCollector.TesselationActive )
					{
						if ( isVertex )
						{
							dataCollector.AddVertexInstruction( vertexUV + " = " + vertexUV + " * " + tiling + " + " + offset, m_uniqueId );
							m_surfaceTexcoordName = Constants.VertexShaderOutputStr + "." + texcoordName;
						}
						else
						{
							dataCollector.AddToProperties( m_uniqueId, DummyPropertyDec + " {}", -1 );
							dataCollector.AddToUniforms( m_uniqueId, DummyUniformDec );
							string texCoordPrefix = ( m_textureCoordChannel == 0 ) ? string.Empty : ( m_textureCoordChannel + 1 ).ToString();
							dataCollector.AddToInput( m_uniqueId, "float2 " + string.Format( DummyTexCoordDef, texCoordPrefix ), true );
							dataCollector.AddToSpecialLocalVariables( m_uniqueId, string.Format( DummyTexCoordSurfDef, m_uniqueId, Constants.InputVarStr, texCoordPrefix, tiling, offset ) );
							m_surfaceTexcoordName = string.Format( DummyTexCoordSurfVar, m_uniqueId );
						}
					}
					else
					{
						dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + texcoordName + ".xy = " + vertexUV + " * " + tiling + " + " + offset, m_uniqueId );
						m_surfaceTexcoordName = ( isVertex ? Constants.VertexShaderOutputStr : Constants.InputVarStr ) + "." + texcoordName;
					}
				}

				m_texcoordId = buffer;
			}

			return GetOutputVectorItem( 0, outputId, m_surfaceTexcoordName );
		}

		public override void Destroy()
		{
			base.Destroy();
			m_referenceNode = null;
		}

	}
}
