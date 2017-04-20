// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Position", "Surface Standard Inputs", "Screen space position" )]
	public sealed class ScreenPosInputsNode : SurfaceShaderINParentNode
	{
		private const string ProjectStr = "Project";
		private const string UVInvertHack = "Scale and Offset";
		private readonly string ProjectionInstruction = "{0}.w += 0.00000000001;\n\t\t\t{0}.xyzw /= {0}.w;";
		private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
														"float scale{0} = -1.0;",
														"#else",
														"float scale{0} = 1.0;",
														"#endif",
														"float halfPosW{1} = {0}.w * 0.5;",
														"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};"};

		[SerializeField]
		private bool m_project = false;

		[SerializeField]
		private bool m_scaleAndOffset = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.SCREEN_POS;
			InitialSetup();
			m_textLabelWidth = 100;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			//m_scaleAndOffset = EditorGUILayout.Toggle( UVInvertHack, m_scaleAndOffset );
			m_project = EditorGUILayout.Toggle( ProjectStr, m_project );
		}

		public override void Reset()
		{
			base.Reset();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
			{
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			bool generateLocalVariable = m_scaleAndOffset || m_project;
			if ( generateLocalVariable )
			{
				string localVarName = "screenPos" + m_uniqueId;				
				string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[0].DataType ) + " " + localVarName + " = " + m_currentInputValueStr + ";";
				dataCollector.AddInstructions( value, true, true );
				if ( m_scaleAndOffset )
				{
					dataCollector.AddInstructions( HackInstruction[ 0 ], true, true );
					dataCollector.AddInstructions( string.Format( HackInstruction[ 1 ], m_uniqueId ), true, true );
					dataCollector.AddInstructions( HackInstruction[ 2 ], true, true );
					dataCollector.AddInstructions( string.Format( HackInstruction[ 3 ], m_uniqueId ), true, true );
					dataCollector.AddInstructions( HackInstruction[ 4 ], true, true );
					dataCollector.AddInstructions( string.Format( HackInstruction[ 5 ], localVarName, m_uniqueId ), true, true );
					dataCollector.AddInstructions( string.Format( HackInstruction[ 6 ], localVarName, m_uniqueId ), true, true );
				}

				if ( m_project )
				{
					dataCollector.AddInstructions( string.Format( ProjectionInstruction, localVarName ), true, true );
				}
				
				m_outputPorts[ 0 ].SetLocalValue( localVarName );
				return GetOutputVectorItem( 0, outputId, localVarName );
			}

			return GetOutputVectorItem( 0, outputId, m_currentInputValueStr );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2400 )
				m_project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if ( UIUtils.CurrentShaderVersion() > 3107 )
			{
				//if ( UIUtils.CurrentShaderVersion() < 3109 )
				//{
				m_scaleAndOffset = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_scaleAndOffset = false;
				//}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_project );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_scaleAndOffset );
		}
	}
}
