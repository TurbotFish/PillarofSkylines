// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Static Switch", "Logical Operators", "Creates a shader keyword toggle", Available = true )]
	public sealed class StaticSwitch : PropertyNode
	{
		[SerializeField]
		private bool m_defaultValue = false;

		[SerializeField]
		private bool m_materialValue = false;

		[SerializeField]
		private int m_multiCompile = 0;

		[SerializeField]
		private int m_currentKeywordId = 0;

		[SerializeField]
		private string m_currentKeyword = string.Empty;

		[SerializeField]
		private bool m_createToggle = true;

		private GUIContent m_dummyContent;

		private const string KeywordStr = "Keyword";
		private const string CustomStr = "Custom";

		public readonly static string[] KeywordTypeStr = { "Shader Feature Keyword", "Multi Compile Keyword", "Define Symbol" };
		public readonly static int[] KeywordTypeInt = { 0, 1, 2 };

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_headerColor = new Color( 0.0f, 0.55f, 0.45f, 1f );
			m_customPrefix = "Keyword ";
			m_autoWrapProperties = false;
			m_freeType = false;
			m_useVarSubtitle = true;
			m_allowPropertyDuplicates = true;
			m_showTitleWhenNotEditing = false;
			m_currentParameterType = PropertyType.Property;
			m_dummyContent = new GUIContent( );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			UIUtils.RegisterPropertyNode( this );
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.UnregisterPropertyNode( this );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections();
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateConnections();
		}

		private void UpdateConnections()
		{
			WirePortDataType mainType = WirePortDataType.FLOAT;
			WirePortDataType portOneType = m_inputPorts[ 0 ].IsConnected ? m_inputPorts[ 0 ].GetOutputConnection().DataType : WirePortDataType.FLOAT;
			WirePortDataType portTwoType = m_inputPorts[ 1 ].IsConnected ? m_inputPorts[ 1 ].GetOutputConnection().DataType : WirePortDataType.FLOAT;

			int highest = UIUtils.GetPriority( mainType );
			if( UIUtils.GetPriority( portOneType ) > highest )
			{
				mainType = portOneType;
				highest = UIUtils.GetPriority( portOneType );
			}

			if( UIUtils.GetPriority( portTwoType ) > highest )
			{
				mainType = portTwoType;
				highest = UIUtils.GetPriority( portTwoType );
			}

			m_inputPorts[ 0 ].ChangeType( mainType, false );
			m_inputPorts[ 1 ].ChangeType( mainType, false );
			m_outputPorts[ 0 ].ChangeType( mainType, false );
		}

		public override string GetPropertyValue()
		{
			if( m_createToggle )
				if( m_multiCompile == 2 )
					return "[Toggle]" + m_currentKeyword + "(\"" + m_currentKeyword + "\", Int) = " + ( m_defaultValue ? 1 : 0 );
				else
					return "[Toggle]" + m_propertyName + "(\"" + m_propertyInspectorName + "\", Int) = " + ( m_defaultValue ? 1 : 0 );
			else
				return string.Empty;
		}

		public override string PropertyName
		{
			get
			{
				if( m_multiCompile == 2 )
					return m_currentKeyword;
				else
					return base.PropertyName.ToUpper();
			}
		}

		public override string GetPropertyValStr()
		{
			if( m_multiCompile == 2 )
				return m_currentKeyword;
			else
				return PropertyName + "_ON";
		}

		public override string GetUniformValue()
		{
			return string.Empty;
		}

		public override bool GetUniformData( out string dataType, out string dataName )
		{
			dataType = string.Empty;
			dataName = string.Empty;
			return false;
		}

		public override void DrawProperties()
		{
			//base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, PropertyGroup );
			CheckPropertyFromInspector();
		}

		void PropertyGroup()
		{
			EditorGUI.BeginChangeCheck();
			m_multiCompile = EditorGUILayoutIntPopup( "Type", m_multiCompile, KeywordTypeStr, KeywordTypeInt );
			if( EditorGUI.EndChangeCheck() )
			{
				BeginPropertyFromInspectorCheck();
			}

			if( m_multiCompile <= 1 )
			{
				ShowPropertyInspectorNameGUI();
				ShowPropertyNameGUI( true );
				bool guiEnabledBuffer = GUI.enabled;
				GUI.enabled = false;
				EditorGUILayoutTextField( "Keyword Name", PropertyName + "_ON" );
				GUI.enabled = guiEnabledBuffer;
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				m_currentKeywordId = EditorGUILayoutPopup( KeywordStr, m_currentKeywordId, UIUtils.AvailableKeywords );
				if( EditorGUI.EndChangeCheck() )
				{
					if( m_currentKeywordId != 0 )
					{
						m_currentKeyword = UIUtils.AvailableKeywords[ m_currentKeywordId ];
					}
				}

				if( m_currentKeywordId == 0 )
				{
					EditorGUI.BeginChangeCheck();
					m_currentKeyword = EditorGUILayoutTextField( CustomStr, m_currentKeyword );
					if( EditorGUI.EndChangeCheck() )
					{
						m_currentKeyword = UIUtils.RemoveInvalidCharacters( m_currentKeyword );
					}
				}
			}

			m_createToggle = EditorGUILayoutToggle( "Material Toggle", m_createToggle );
			if( m_createToggle )
				m_defaultValue = EditorGUILayoutToggle( "Default Value", m_defaultValue );
			EditorGUILayout.HelpBox( "Keyword Type:\n" +
				"The difference is that unused variants of \"Shader Feature\" shaders will not be included into game build while \"Multi Compile\" variants are included regardless of their usage.\n\n" +
				"So \"Shader Feature\" makes most sense for keywords that will be set on the materials, while \"Multi Compile\" for keywords that will be set from code globally.\n\n" +
				"You can set keywords using the material property using the \"Property Name\" or you can set the keyword directly using the \"Keyword Name\".", MessageType.None );
		}

		private Rect m_varRect;
		private bool m_editing;

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_varRect = m_remainingBox;
			m_varRect.size = Vector2.one * 22 * drawInfo.InvertedZoom;
			m_varRect.center = m_remainingBox.center;
		}

		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( drawInfo.CurrentEventType != EventType.MouseDown || !m_createToggle || !m_materialMode )
				return;

			if( m_varRect.Contains( drawInfo.MousePosition ) )
			{
				m_editing = true;
			}
			else if( m_editing )
			{
				m_editing = false;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if( m_editing )
			{
				if( GUI.Button( m_varRect, string.Empty ) )
				{
					m_materialValue = !m_materialValue;
					m_requireMaterialUpdate = true;
					m_editing = false;
				}
			}
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( !m_isVisible )
				return;

			if( m_createToggle && m_materialMode )
			{
				if( !m_editing )
					GUI.Label( m_varRect, string.Empty, UIUtils.Button );

				if( m_materialValue )
				{
					m_dummyContent.image = UIUtils.CheckmarkIcon;
					GUI.Label( m_varRect, m_dummyContent );
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string trueCode = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string falseCode = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			if( m_multiCompile == 1 )
				dataCollector.AddToPragmas( UniqueId, "multi_compile __ " + PropertyName + "_ON" );
			else if( m_multiCompile == 0 )
				dataCollector.AddToPragmas( UniqueId, "shader_feature " + PropertyName + "_ON" );

			string outType = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			if( m_multiCompile == 2 )
				dataCollector.AddLocalVariable( UniqueId, "#ifdef " + m_currentKeyword, true );
			else
				dataCollector.AddLocalVariable( UniqueId, "#ifdef " + PropertyName + "_ON", true );
			dataCollector.AddLocalVariable( UniqueId, "\t" + outType + " staticSwitch" + OutputId + " = " + trueCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#else", true );
			dataCollector.AddLocalVariable( UniqueId, "\t" + outType + " staticSwitch" + OutputId + " = " + falseCode + ";", true );
			dataCollector.AddLocalVariable( UniqueId, "#endif", true );

			m_outputPorts[ 0 ].SetLocalValue( "staticSwitch" + OutputId );
			return m_outputPorts[ 0 ].LocalValue;
		}

		public override void DrawTitle( Rect titlePos )
		{
			if( m_multiCompile == 2 )
			{
				SetAdditonalTitleTextOnCallback( m_currentKeyword, ( instance, newSubTitle ) => instance.AdditonalTitleContent.text = string.Format( Constants.SubTitleVarNameFormatStr, newSubTitle ) );
			}
			else
			{
				m_previousAdditonalTitle = m_additionalContent.text;
			}

			if( !m_isEditing && ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD3 )
			{
				GUI.Label( titlePos, "Static Switch", UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetInt( m_propertyName, m_materialValue ? 1 : 0 );
				if( m_materialValue )
					mat.EnableKeyword( PropertyName + "_ON" );
				else
					mat.DisableKeyword( PropertyName + "_ON" );
			}
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );
			if( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetInt( m_propertyName ) == 1;
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetInt( m_propertyName ) == 1;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_multiCompile = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_defaultValue = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 13104 )
			{
				m_createToggle = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_currentKeyword = GetCurrentParam( ref nodeParams );
				m_currentKeywordId = UIUtils.GetKeywordId( m_currentKeyword );
			}
		}

		public override void ReadFromDeprecated( ref string[] nodeParams, Type oldType = null )
		{
			base.ReadFromDeprecated( ref nodeParams, oldType );
			//if( UIUtils.CurrentShaderVersion() <= 13104 )
			{
				m_currentKeyword = GetCurrentParam( ref nodeParams );
				m_currentKeywordId = UIUtils.GetKeywordId( m_currentKeyword );
				m_createToggle = false;
				m_multiCompile = 2;
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_multiCompile );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_createToggle );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentKeyword );
		}
	}
}
