// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Float", "Constants", "Float property", null, KeyCode.Alpha1 )]
	public sealed class RangedFloatNode : PropertyNode
	{
		private const int OriginalFontSize = 11;

		private const string MinValueStr = "Min";
		private const string MaxValueStr = "Max";

		private const float LabelWidth = 8;

		[SerializeField]
		private float m_defaultValue = 0;

		[SerializeField]
		private float m_materialValue = 0;

		[SerializeField]
		private float m_min = 0;

		[SerializeField]
		private float m_max = 0;

		[SerializeField]
		private bool m_floatMode = true;

		private int m_cachedPropertyId = -1;

		public RangedFloatNode() : base() { }
		public RangedFloatNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_insideSize.Set( 50, 0 );
			m_showPreview = false;
			m_selectedLocation = PreviewLocation.BottomCenter;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_availableAttribs.Add( new PropertyAttributes( "Toggle", "[Toggle]" ) );
			m_availableAttribs.Add( new PropertyAttributes( "Int Range", "[IntRange]" ) );
			m_previewShaderGUID = "d9ca47581ac157145bff6f72ac5dd73e";
		}

		public void SetFloatMode( bool value )
		{
			m_floatMode = value;
			if ( value )
			{
				m_insideSize.x = 50;// + ( m_showPreview ? 50 : 0 );
				//m_firstPreviewDraw = true;
			}
			else
			{
				m_insideSize.x = 200;// + ( m_showPreview ? 0 : 0 );
				//m_firstPreviewDraw = true;
			}
			m_sizeIsDirty = true;
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_min = EditorGUILayout.FloatField( MinValueStr, m_min );
			m_max = EditorGUILayout.FloatField( MaxValueStr, m_max );
			if ( m_min > m_max )
				m_min = m_max;

			if ( m_max < m_min )
				m_max = m_min;

			if ( EditorGUI.EndChangeCheck() )
			{
				SetFloatMode( m_min == m_max );
			}

			if ( m_floatMode )
			{
				m_defaultValue = EditorGUILayout.FloatField( Constants.DefaultValueLabel, m_defaultValue );
			}
			else
			{
				m_defaultValue = EditorGUILayout.Slider( Constants.DefaultValueLabel, m_defaultValue, m_min, m_max );
			}
		}

		public override void DrawMaterialProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_min = EditorGUILayout.FloatField( MinValueStr, m_min );
			m_max = EditorGUILayout.FloatField( MaxValueStr, m_max );
			if ( m_min > m_max )
				m_min = m_max;

			if ( m_max < m_min )
				m_max = m_min;

			if ( EditorGUI.EndChangeCheck() )
			{
				SetFloatMode( m_min == m_max );
			}

			EditorGUI.BeginChangeCheck();

			if ( m_floatMode )
			{
				m_materialValue = EditorGUILayout.FloatField( Constants.MaterialValueLabel, m_materialValue );
			}
			else
			{
				m_materialValue = EditorGUILayout.Slider( Constants.MaterialValueLabel, m_materialValue, m_min, m_max );
			}
			if (EditorGUI.EndChangeCheck())
			{
				//MarkForPreviewUpdate();
				if( m_materialMode )
					m_requireMaterialUpdate = true;
			}
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			
			if ( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_InputFloat" );

			if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				PreviewMaterial.SetFloat( m_cachedPropertyId, m_materialValue );
			else
				PreviewMaterial.SetFloat( m_cachedPropertyId, m_defaultValue );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isVisible )
			{
				if ( m_floatMode )
				{
					m_propertyDrawPos.x = m_remainingBox.x - LabelWidth * drawInfo.InvertedZoom;
					m_propertyDrawPos.y = m_outputPorts[ 0 ].Position.y - 2 * drawInfo.InvertedZoom;
					m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
					m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
				}
				else
				{
					m_propertyDrawPos.x = m_remainingBox.x;
					m_propertyDrawPos.y = m_outputPorts[ 0 ].Position.y - 2 * drawInfo.InvertedZoom;
					m_propertyDrawPos.width = 0.7f * m_globalPosition.width;
					m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
				}

				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					if ( m_floatMode )
					{
						UIUtils.DrawFloat( ref m_propertyDrawPos, ref m_materialValue, LabelWidth * drawInfo.InvertedZoom );
					}
					else
					{
						DrawSlider( ref m_materialValue, drawInfo );
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if ( m_currentParameterType != PropertyType.Constant )
						{
							//MarkForPreviewUpdate();
							BeginDelayedDirtyProperty();
						}
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();

					if ( m_floatMode )
					{
						UIUtils.DrawFloat( ref m_propertyDrawPos, ref m_defaultValue, LabelWidth * drawInfo.InvertedZoom );
					}
					else
					{
						DrawSlider( ref m_defaultValue, drawInfo );
					}
					if (EditorGUI.EndChangeCheck())
					{
						//MarkForPreviewUpdate();
						BeginDelayedDirtyProperty();
					}
						
				}
			}
		}

		void DrawSlider( ref float value, DrawInfo drawInfo )
		{	
			int originalFontSize = EditorStyles.numberField.fontSize;
			EditorStyles.numberField.fontSize = ( int ) ( OriginalFontSize * drawInfo.InvertedZoom );

			float rangeWidth = 30 * drawInfo.InvertedZoom;
			float rangeSpacing = 5 * drawInfo.InvertedZoom;

			//Min
			m_propertyDrawPos.width = rangeWidth;
			m_min = EditorGUI.FloatField( m_propertyDrawPos, m_min, UIUtils.MainSkin.textField );

			//Value Slider
			m_propertyDrawPos.x += m_propertyDrawPos.width + rangeSpacing;
			m_propertyDrawPos.width = 0.65f * ( m_globalPosition.width - 3 * m_propertyDrawPos.width );

			Rect slider = m_propertyDrawPos;
			slider.height = 5 * drawInfo.InvertedZoom;
			slider.y += m_propertyDrawPos.height * 0.5f - slider.height * 0.5f;
			GUI.Box( slider, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SliderStyle ) );

			value = GUI.HorizontalSlider( m_propertyDrawPos, value, m_min, m_max , GUIStyle.none, UIUtils.RangedFloatSliderThumbStyle );

			//Value Area
			m_propertyDrawPos.x += m_propertyDrawPos.width + rangeSpacing;
			m_propertyDrawPos.width = rangeWidth;
			value = EditorGUI.FloatField( m_propertyDrawPos, value, UIUtils.MainSkin.textField );

			//Max
			m_propertyDrawPos.x += m_propertyDrawPos.width + rangeSpacing;
			m_propertyDrawPos.width = rangeWidth;
			m_max = EditorGUI.FloatField( m_propertyDrawPos, m_max, UIUtils.MainSkin.textField );

			EditorStyles.numberField.fontSize = originalFontSize;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData;

			return IOUtils.Floatify( m_defaultValue );
		}

		public override string GetPropertyValue()
		{
			if ( m_floatMode )
			{
				return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_defaultValue;
			}
			else
			{
				return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Range( " + m_min + " , " + m_max + ")) = " + m_defaultValue;
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetFloat( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetFloat( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetFloat( m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_min = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_max = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			SetFloatMode( m_min == m_max );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_min );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_max );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ?
				m_materialValue.ToString( Mathf.Abs( m_materialValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel ) :
				m_defaultValue.ToString( Mathf.Abs( m_defaultValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel );
		}

		public float Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}
	}
}
