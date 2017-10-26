// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// http://kylehalladay.com/blog/tutorial/2014/02/18/Fresnel-Shaders-From-The-Ground-Up.html
// http://http.developer.nvidia.com/CgTutorial/cg_tutorial_chapter07.html

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Fresnel", "Surface Data", "Simple Fresnel effect" )]
	public sealed class FresnelNode : ParentNode
	{
		private const string FresnedFinalVar = "fresnelNode";

		[SerializeField]
		private ViewSpace m_normalSpace = ViewSpace.World;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "World Normal" );
			AddInputPort( WirePortDataType.FLOAT, false, "Bias" );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
			AddInputPort( WirePortDataType.FLOAT, false, "Power" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_autoWrapProperties = true;
			m_drawPreviewAsSphere = true;
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_inputPorts[ 3 ].FloatInternalData = 5;
			m_previewShaderGUID = "240145eb70cf79f428015012559f4e7d";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if( m_normalSpace == ViewSpace.Tangent && m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 2;
			else if( m_normalSpace == ViewSpace.World && m_inputPorts[ 0 ].IsConnected )
				m_previewMaterialPassId = 1;
			else
				m_previewMaterialPassId = 0;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_normalSpace = (ViewSpace)EditorGUILayoutEnumPopup( "Normal Space", m_normalSpace );
			if( EditorGUI.EndChangeCheck() )
			{
				UpdatePort();
			}

			if( !m_inputPorts[ 1 ].IsConnected )
				m_inputPorts[ 1 ].FloatInternalData = EditorGUILayoutFloatField( m_inputPorts[ 1 ].Name, m_inputPorts[ 1 ].FloatInternalData );
			if( !m_inputPorts[ 2 ].IsConnected )
				m_inputPorts[ 2 ].FloatInternalData = EditorGUILayoutFloatField( m_inputPorts[ 2 ].Name, m_inputPorts[ 2 ].FloatInternalData );
			if( !m_inputPorts[ 3 ].IsConnected )
				m_inputPorts[ 3 ].FloatInternalData = EditorGUILayoutFloatField( m_inputPorts[ 3 ].Name, m_inputPorts[ 3 ].FloatInternalData );
		}

		private void UpdatePort()
		{
			if( m_normalSpace == ViewSpace.World )
				m_inputPorts[ 0 ].ChangeProperties( "World Normal", m_inputPorts[ 0 ].DataType, false );
			else
				m_inputPorts[ 0 ].ChangeProperties( "Normal", m_inputPorts[ 0 ].DataType, false );

			m_sizeIsDirty = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			if( dataCollector.IsFragmentCategory )
				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( PrecisionType.Float, AvailableSurfaceInputs.WORLD_POS ), true );

			string viewdir = GeneratorUtils.GenerateViewDirection( ref dataCollector, UniqueId, PrecisionType.Fixed, ViewSpace.World );

			string normal = string.Empty;
			if( m_inputPorts[ 0 ].IsConnected )
			{
				normal = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, true );

				if( dataCollector.IsFragmentCategory )
				{
					dataCollector.AddToInput( UniqueId, Constants.InternalData, false );

					if( m_normalSpace == ViewSpace.Tangent )
					{
						dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
						dataCollector.ForceNormal = true;
						normal = "WorldNormalVector( " + Constants.InputVarStr + " , " + normal + " )";
					}
				}
				else
				{
					if( m_normalSpace == ViewSpace.Tangent )
					{
						string wtMatrix = GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );
						normal = "mul( " + normal + "," + wtMatrix + " )";
					}
				}
			}
			else
			{
				if( dataCollector.IsFragmentCategory )
				{
					dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
					if( dataCollector.DirtyNormal )
						dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
				}

				normal = dataCollector.IsTemplate ? dataCollector.TemplateDataCollectorInstance.GetWorldNormal() : GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
			}

			string bias = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );
			string scale = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );
			string power = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalvar, true );

			string ndotl = "dot( " + normal + ", " + viewdir + " )";

			string fresnelFinalVar = FresnedFinalVar + OutputId;
			string result = string.Format( "( {0} + {1} * pow( 1.0 - {2}, {3} ) )", bias, scale, ndotl, power );

			RegisterLocalVariable( 0, result, ref dataCollector, fresnelFinalVar );
			return m_outputPorts[ 0 ].LocalValue;
		}
	}
}
