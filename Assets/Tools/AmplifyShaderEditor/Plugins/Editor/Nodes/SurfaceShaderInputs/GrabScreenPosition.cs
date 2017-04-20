// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grab Screen Position", "Surface Standard Inputs", "Screen position correctly transformed to be used with Grab Screen Color" )]
	public sealed class GrabScreenPosition : ParentNode
	{
		private const string ProjectStr = "Project";
		private const string ScreenPosStr = "screenPos";
		private readonly string ScreenPosOnFragStr = Constants.InputVarStr + "." + ScreenPosStr;
		private readonly string ProjectionInstruction = "{0}.w += 0.00000000001;\n\t\t\t{0}.xyzw /= {0}.w;";
		private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
														"float scale{0} = -1.0;",
														"#else",
														"float scale{0} = 1.0;",
														"#endif",
														"float halfPosW{1} = {0}.w * 0.5;",
														"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};"};

		[SerializeField]
		private bool m_project = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT4, "XYZW" );
			m_autoWrapProperties = true;
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_project = EditorGUILayout.Toggle( ProjectStr, m_project );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue);

			string localVarName = ScreenPosStr + m_uniqueId;

			dataCollector.AddToInput( m_uniqueId, "float4 "+ ScreenPosStr, true );
			string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName + " = " + ScreenPosOnFragStr + ";";
			dataCollector.AddInstructions( value, true, true );

			dataCollector.AddInstructions( HackInstruction[ 0 ], true, true );
			dataCollector.AddInstructions( string.Format( HackInstruction[ 1 ], m_uniqueId ), true, true );
			dataCollector.AddInstructions( HackInstruction[ 2 ], true, true );
			dataCollector.AddInstructions( string.Format( HackInstruction[ 3 ], m_uniqueId ), true, true );
			dataCollector.AddInstructions( HackInstruction[ 4 ], true, true );
			dataCollector.AddInstructions( string.Format( HackInstruction[ 5 ], localVarName, m_uniqueId ), true, true );
			dataCollector.AddInstructions( string.Format( HackInstruction[ 6 ], localVarName, m_uniqueId ), true, true );
			if( m_project )
			{
				dataCollector.AddInstructions( string.Format( ProjectionInstruction, localVarName ), true, true );
			}

			m_outputPorts[ 0 ].SetLocalValue( localVarName );
			//RegisterLocalVariable(outputId, localVarName ,ref dataCollector)
			return GetOutputColorItem( 0, outputId, localVarName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 3108 )
				m_project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_project );
		}
	}
}
