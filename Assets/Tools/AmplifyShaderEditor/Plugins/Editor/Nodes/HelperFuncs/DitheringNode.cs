// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Dithering", "Generic", "Generates a dithering pattern" )]
	public sealed class DitheringNode : ParentNode
	{
		private const string InputTypeStr = "Pattern";
		private const string CustomScreenPosStr = "screenPosition";

		private string m_functionHeader = "Dither4x4Bayer( {0}, {1} )";
		private string m_functionBody = string.Empty;

		[SerializeField]
		private int m_selectedPatternInt = 0;

		private readonly string[] PatternsFuncStr = { "4x4Bayer", "8x8Bayer" };
		private readonly string[] PatternsStr = { "4x4 Bayer Matrix", "8x8 Bayer Matrix" };

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_textLabelWidth = 100;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			m_selectedPatternInt = EditorGUILayout.Popup( "Pattern", m_selectedPatternInt, PatternsStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				GeneratePattern();
			}
			EditorGUILayout.EndVertical();
		}

		private void GeneratePattern()
		{
			switch ( m_selectedPatternInt )
			{
				default:
				case 0:
					m_functionBody = string.Empty;
					m_functionHeader = "Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( {0}, {1} )";
					IOUtils.AddFunctionHeader( ref m_functionBody, "inline float Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( int x, int y )" );
					IOUtils.AddFunctionLine( ref m_functionBody, "const float dither[ 16 ] = {" );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 1,  9,  3, 11," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	13,  5, 15,  7," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 4, 12,  2, 10," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	16,  8, 14,  6 };" );
					IOUtils.AddFunctionLine( ref m_functionBody, "int r = y * 4 + x;" );
					//same as dividing by 16
					IOUtils.AddFunctionLine( ref m_functionBody, "return (dither[r]-1) / 15;" );
					IOUtils.CloseFunctionBody( ref m_functionBody );
					break;
				case 1:
					m_functionBody = string.Empty;
					m_functionHeader = "Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( {0}, {1} )";
					IOUtils.AddFunctionHeader( ref m_functionBody, "inline float Dither" + PatternsFuncStr[ m_selectedPatternInt ] + "( int x, int y )" );
					IOUtils.AddFunctionLine( ref m_functionBody, "const float dither[ 64 ] = {" );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 1, 49, 13, 61,  4, 52, 16, 64," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	33, 17, 45, 29, 36, 20, 48, 32," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 9, 57,  5, 53, 12, 60,  8, 56," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	41, 25, 37, 21, 44, 28, 40, 24," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	 3, 51, 15, 63,  2, 50, 14, 62," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	35, 19, 47, 31, 34, 18, 46, 30," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	11, 59,  7, 55, 10, 58,  6, 54," );
					IOUtils.AddFunctionLine( ref m_functionBody, "	43, 27, 39, 23, 42, 26, 38, 22};" );
					IOUtils.AddFunctionLine( ref m_functionBody, "int r = y * 8 + x;" );
					//same as dividing by 64
					IOUtils.AddFunctionLine( ref m_functionBody, "return (dither[r]-1) / 63;" );
					IOUtils.CloseFunctionBody( ref m_functionBody );
					break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityShaderVariables );

			dataCollector.AddToInput( m_uniqueId, "float4 " + CustomScreenPosStr, true );
			string vertexInstruction = Constants.VertexShaderOutputStr + "." + CustomScreenPosStr + " = ComputeScreenPos( UnityObjectToClipPos( " + Constants.VertexShaderInputStr + ".vertex ) )";
			dataCollector.AddVertexInstruction( vertexInstruction, m_uniqueId );

			string surfInstruction = "( " + Constants.InputVarStr + "." + CustomScreenPosStr + ".xy / " + Constants.InputVarStr + "." + CustomScreenPosStr + ".w ) * _ScreenParams.xy";
			dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, WirePortDataType.FLOAT2, "clipScreen" + m_uniqueId, surfInstruction );

			string functionResult = "";
			switch ( m_selectedPatternInt )
			{
				default:
				case 0:
					functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, "fmod(" + "clipScreen" + m_uniqueId + ".x, 4)", "fmod(" + "clipScreen" + m_uniqueId + ".y, 4)" );
					break;
				case 1:
					functionResult = dataCollector.AddFunctions( m_functionHeader, m_functionBody, "fmod(" + "clipScreen" + m_uniqueId + ".x, 8)", "fmod(" + "clipScreen" + m_uniqueId + ".y, 8)" );
					break;
			}
			return CreateOutputLocalVariable( 0, functionResult, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedPatternInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			GeneratePattern();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedPatternInt );
		}
	}
}
