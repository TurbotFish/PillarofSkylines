// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Custom Expression", "Misc", "Creates Custom Expression" )]
	public sealed class CustomExpressionNode : ParentNode
	{
		private const char LineFeedSeparator = '$';

		private const double MaxTimestamp = 1;

		private const string DefaultInputName = "In";
		private const string CodeTitleStr = "Code";
		private const string OutputTypeStr = "Output Type";
		private const string InputsStr = "Inputs";
		private const string InputNameStr = "Name";
		private const string InputTypeStr = "Type";
		private const string InputValueStr = "Value";
		private readonly string[] AvailableWireTypesStr =   {   "Int",
																"Float",
																"Float2",
																"Float3",
																"Float4",
																"Color",
																"Float3x3",
																"Float4x4"};

		private readonly WirePortDataType[] AvailableWireTypes = {  WirePortDataType.INT,
																	WirePortDataType.FLOAT,
																	WirePortDataType.FLOAT2,
																	WirePortDataType.FLOAT3,
																	WirePortDataType.FLOAT4,
																	WirePortDataType.COLOR,
																	WirePortDataType.FLOAT3x3,
																	WirePortDataType.FLOAT4x4};

		private readonly Dictionary<WirePortDataType, int> WireToIdx = new Dictionary<WirePortDataType, int> {  { WirePortDataType.INT,     0},
																												{ WirePortDataType.FLOAT,   1},
																												{ WirePortDataType.FLOAT2,  2},
																												{ WirePortDataType.FLOAT3,  3},
																												{ WirePortDataType.FLOAT4,  4},
																												{ WirePortDataType.COLOR,   5},
																												{ WirePortDataType.FLOAT3x3,6},
																												{ WirePortDataType.FLOAT4x4,7}};
		[SerializeField]
		private List<bool> m_foldoutValuesFlags = new List<bool>();

		[SerializeField]
		private List<string> m_foldoutValuesLabels = new List<string>();

		[SerializeField]
		private string m_code = " ";

		[SerializeField]
		private int m_outputTypeIdx = 1;

		[SerializeField]
		private bool m_visibleInputsFoldout = true;

		private int m_markedToDelete = -1;
		private const float ButtonLayoutWidth = 15;

		private bool m_repopulateNameDictionary = true;
		private Dictionary<string, int> m_usedNames = new Dictionary<string, int>();

		private double m_lastTimeModified = 0;
		private bool m_nameModified = false;


		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "In0" );
			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[0]" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 95;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_nameModified )
			{
				if ( ( EditorApplication.timeSinceStartup - m_lastTimeModified ) > MaxTimestamp )
				{
					m_nameModified = false;
					m_repopulateNameDictionary = true;
				}
			}

			if ( m_repopulateNameDictionary )
			{
				m_repopulateNameDictionary = false;
				m_usedNames.Clear();
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					m_usedNames.Add( m_inputPorts[ i ].Name, i );
				}
			}

			base.Draw( drawInfo );
		}

		public string GetFirstAvailableName()
		{
			string name = string.Empty;
			for ( int i = 0; i < m_inputPorts.Count + 1; i++ )
			{
				name = DefaultInputName + i;
				if ( !m_usedNames.ContainsKey( name ) )
				{
					return name;
				}
			}
			Debug.LogWarning( "Could not find valid name" );
			return string.Empty;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawBaseProperties );
			NodeUtils.DrawPropertyGroup( ref m_visibleInputsFoldout, InputsStr, DrawInputs, DrawAddRemoveInputs );

		}

		void DrawBaseProperties()
		{
			DrawPrecisionProperty();

			EditorGUILayout.LabelField( CodeTitleStr );
			m_code = EditorGUILayout.TextArea( m_code, UIUtils.MainSkin.textArea );

			EditorGUI.BeginChangeCheck();
			m_outputTypeIdx = EditorGUILayout.Popup( OutputTypeStr, m_outputTypeIdx, AvailableWireTypesStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			}
		}

		void DrawAddRemoveInputs()
		{
			if ( m_inputPorts.Count == 0 )
				m_visibleInputsFoldout = false;

			// Add new port
			if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				AddPortAt( m_inputPorts.Count );
			}

			//Remove port
			if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				RemovePortAt( m_inputPorts.Count - 1 );
			}
		}

		void DrawInputs()
		{
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{

				m_foldoutValuesFlags[ i ] = EditorGUILayout.Foldout( m_foldoutValuesFlags[ i ], m_foldoutValuesLabels[ i ] );

				if ( m_foldoutValuesFlags[ i ] )
				{
					EditorGUI.indentLevel += 1;
					EditorGUI.BeginChangeCheck();
					m_inputPorts[ i ].Name = EditorGUILayout.TextField( InputNameStr, m_inputPorts[ i ].Name );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_nameModified = true;
						m_lastTimeModified = EditorApplication.timeSinceStartup;
						m_inputPorts[ i ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ i ].Name );
						if ( string.IsNullOrEmpty( m_inputPorts[ i ].Name ) )
						{
							m_inputPorts[ i ].Name = DefaultInputName + i;
						}
					}

					int typeIdx = WireToIdx[ m_inputPorts[ i ].DataType ];
					EditorGUI.BeginChangeCheck();
					typeIdx = EditorGUILayout.Popup( InputTypeStr, typeIdx, AvailableWireTypesStr );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_inputPorts[ i ].ChangeType( AvailableWireTypes[ typeIdx ], false );
					}

					if ( !m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].ShowInternalData( true, InputValueStr );
					}

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if ( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							AddPortAt( i );
						}

						//Remove port
						if ( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							m_markedToDelete = i;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel -= 1;
				}
			}

			if ( m_markedToDelete > -1 )
			{
				DeleteInputPortByArrayIdx( m_markedToDelete );
				m_markedToDelete = -1;
				m_repopulateNameDictionary = true;
			}
		}

		void AddPortAt( int idx )
		{
			AddInputPortAt( idx, WirePortDataType.FLOAT, false, GetFirstAvailableName()/*DefaultInputName + idx */);

			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[" + idx + "]" );
			m_repopulateNameDictionary = true;
		}

		void RemovePortAt( int idx )
		{
			if ( m_inputPorts.Count > 0 )
			{
				DeleteInputPortByArrayIdx( idx );
				m_foldoutValuesFlags.RemoveAt( idx );
				m_foldoutValuesLabels.RemoveAt( idx );
				m_repopulateNameDictionary = true;
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_repopulateNameDictionary = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( string.IsNullOrEmpty( m_code ) )
			{
				UIUtils.ShowMessage( "Custom Expression need to have code associated", MessageSeverity.Warning );
				return "0";
			}

			int count = m_inputPorts.Count;
			if ( m_inputPorts.Count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					if ( m_inputPorts[ i ].IsConnected )
					{
						string result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ i ].DataType, true, true );
						dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, result );
					}
					else
					{
						dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, m_inputPorts[ i ].WrappedInternalData );
					}
				}
			}
			return string.Format( Constants.CodeWrapper, m_code );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			// This node is, by default, created with one input port 
			base.ReadFromString( ref nodeParams );
			m_code = GetCurrentParam( ref nodeParams );
			m_code = m_code.Replace( LineFeedSeparator, '\n' );
			m_outputTypeIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( count == 0 )
			{
				DeleteInputPortByArrayIdx( 0 );
				m_foldoutValuesLabels.Clear();
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					m_foldoutValuesFlags.Add( Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) );
					string name = GetCurrentParam( ref nodeParams );
					WirePortDataType type = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
					string internalData = GetCurrentParam( ref nodeParams );
					if ( i == 0 )
					{
						m_inputPorts[ 0 ].ChangeProperties( name, type, false );
					}
					else
					{
						m_foldoutValuesLabels.Add( "[" + i + "]" );
						AddInputPort( type, false, name );
					}
					m_inputPorts[ i ].InternalData = internalData;
				}
			}
			m_repopulateNameDictionary = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			string parsedCode = m_code.Replace( '\n', LineFeedSeparator );
			IOUtils.AddFieldValueToString( ref nodeInfo, parsedCode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeIdx );
			int count = m_inputPorts.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for ( int i = 0; i < count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_foldoutValuesFlags[ i ] );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].Name );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].InternalData );
			}
		}
	}
}
