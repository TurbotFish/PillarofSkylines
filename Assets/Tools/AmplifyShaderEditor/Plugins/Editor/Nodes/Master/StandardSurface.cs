// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum RenderPath
	{
		All,
		ForwardOnly,
		DeferredOnly
	}

	public enum StandardShaderLightModel
	{
		Standard,
		StandardSpecular,
		Lambert,
		BlinnPhong
	}

	public enum CullMode
	{
		Back,
		Front,
		Off
	}

	public enum AlphaMode
	{
		Opaque = 0,
		Masked = 1,
		Transparent = 2, // Transparent (alpha:fade)
		Translucent = 3,
		Premultiply = 4, // Alpha Premul (alpha:premul)
		Custom = 5,
	}

	public enum RenderType
	{
		Opaque,
		Transparent,
		TransparentCutout,
		Background,
		Overlay,
		TreeOpaque,
		TreeTransparentCutout,
		TreeBillboard,
		Grass,
		GrassBillboard
	}

	public enum RenderQueue
	{
		Background,
		Geometry,
		AlphaTest,
		Transparent,
		Overlay
	}

	public enum RenderPlatforms
	{
		d3d9,
		d3d11,
		glcore,
		gles,
		gles3,
		metal,
		d3d11_9x,
		xbox360,
		xboxone,
		ps4,
		psp2,
		n3ds,
		wiiu
	}

	[Serializable]
	public class NodeCache
	{
		public int TargetNodeId = -1;
		public int TargetPortId = -1;

		public NodeCache( int targetNodeId, int targetPortId )
		{
			SetData( targetNodeId, targetPortId );
		}

		public void SetData( int targetNodeId, int targetPortId )
		{
			TargetNodeId = targetNodeId;
			TargetPortId = targetPortId;
		}

		public void Invalidate()
		{
			TargetNodeId = -1;
			TargetPortId = -1;
		}

		public bool IsValid
		{
			get { return ( TargetNodeId >= 0 ); }
		}

		public override string ToString()
		{
			return "TargetNodeId " + TargetNodeId + " TargetPortId " + TargetPortId;
		}
	}

	[Serializable]
	public class CacheNodeConnections
	{
		public Dictionary<string, List<NodeCache>> NodeCacheArray;

		public CacheNodeConnections()
		{
			NodeCacheArray = new Dictionary<string, List<NodeCache>>();
		}

		public void Add( string key, NodeCache value )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				NodeCacheArray[ key ].Add( value );
			}
			else
			{
				NodeCacheArray.Add( key, new List<NodeCache>() );
				NodeCacheArray[ key ].Add( value );
			}
		}

		public NodeCache Get( string key, int idx = 0 )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				if ( idx < NodeCacheArray[ key ].Count )
					return NodeCacheArray[ key ][ idx ];
			}
			return null;
		}

		public List<NodeCache> GetList( string key )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
			{
				return NodeCacheArray[ key ];
			}
			return null;
		}

		public void Clear()
		{
			foreach ( KeyValuePair<string, List<NodeCache>> kvp in NodeCacheArray )
			{
				kvp.Value.Clear();
			}
			NodeCacheArray.Clear();
		}
	}

	[Serializable]
	[NodeAttributes( "Standard Surface Output", "Master", "Surface shader generator output", null, KeyCode.None, false )]
	public sealed class StandardSurfaceOutputNode : MasterNode, ISerializationCallbackReceiver
	{

		private readonly string[] FadeModeOptions = { "Opaque", "Masked", "Transparent", "Translucent", "Alpha Premultipled", "Custom" };

		private const string GeneralFoldoutStr = " General";
		private const string CustomInspectorStr = "Custom Editor";
		private GUIContent RenderPathContent = new GUIContent( "Render Path", "Selects and generates passes for the supported rendering paths\nDefault: All" );
		private const string ShaderModelStr = "Shader Model";
		private const string LightModelStr = "Light Model";
		private GUIContent LightModelContent = new GUIContent( "Light Model", "Surface shader lighting model defines how the surface reflects light\nDefault: Standard" );
		private GUIContent CullModeContent = new GUIContent( "Cull Mode", "Polygon culling mode prevents rendering of either back-facing or front-facing polygons to save performance, turn it off if you want to render both sides\nDefault: Back" );
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";
		private const string ShaderNameStr = "Shader Name";

		private const string DiscardStr = "Opacity Mask";
		private const string VertexDisplacementStr = "Vertex Offset";
		private const string VertexNormalStr = "Vertex Normal";
		private const string CustomLightModelStr = "C. Light Model";
		private const string AlbedoStr = "Albedo";
		private const string NormalStr = "Normal";
		private const string EmissionStr = "Emission";
		private const string MetallicStr = "Metallic";
		private const string SmoothnessStr = "Smoothness";
		private const string OcclusionStr = "Occlusion";
		private const string TransmissionStr = "Transmission";
		private const string TranslucencyStr = "Translucency";
		private const string RefractionStr = "Refraction";
		private const string AlphaStr = "Opacity";
		private const string AlphaDataStr = "Alpha";
		private const string DebugStr = "Debug";
		private const string SpecularStr = "Specular";
		private const string GlossStr = "Gloss";
		private GUIContent AlphaModeContent = new GUIContent( " Blend Mode", "Defines how the surface blends with the background\nDefault: Opaque" );
		private const string OpacityMaskClipValueStr = "Mask Clip Value";
		private GUIContent OpacityMaskClipValueContent = new GUIContent( "Mask Clip Value", "Default clip value to be compared with opacity alpha ( 0 = fully Opaque, 1 = fully Masked )\nDefault: 0.5" );
		private GUIContent CastShadowsContent = new GUIContent( "Cast Shadows", "Generates a shadow caster pass for vertex modifications and point lights in forward rendering\nDefault: ON" );
		private GUIContent ReceiveShadowsContent = new GUIContent( "Receive Shadows", "Untick it to disable shadow receiving, this includes self-shadowing (only for forward rendering) \nDefault: ON" );
		private GUIContent QueueIndexContent = new GUIContent( "Queue Index", "Value to offset the render queue, accepts both positive values to render later and negative values to render sooner\nDefault: 0" );
		private GUIContent RefractionLayerStr = new GUIContent( "Refraction Layer", "Use it to group or ungroup different refraction shaders into the same or different grabpass (only for forward rendering) \nDefault: 0" );
		private GUIContent RenderQueueContent = new GUIContent( "Render Queue", "Base rendering queue index\n(Background = 1000, Geometry = 2000, AlphaTest = 2450, Transparent = 3000, Overlay = 4000)\nDefault: Geometry" );
		private GUIContent RenderTypeContent = new GUIContent( "Render Type", "Categorizes shaders into several predefined groups, usually to be used with screen shader effects\nDefault: Opaque" );


		private GUIContent m_shaderNameContent;
		private const string DefaultShaderName = "MyNewShader";

		private const string ShaderInputOrderStr = "Shader Input Order";
		private const string PropertyOderFoldoutStr = " Material Properties";

		[SerializeField]
		private BlendOpsHelper m_blendOpsHelper = new BlendOpsHelper();

		[SerializeField]
		private StencilBufferOpHelper m_stencilBufferHelper = new StencilBufferOpHelper();

		[SerializeField]
		private ZBufferOpHelper m_zBufferHelper = new ZBufferOpHelper();

		[SerializeField]
		private OutlineOpHelper m_outlineHelper = new OutlineOpHelper();

		[SerializeField]
		private TessellationOpHelper m_tessOpHelper = new TessellationOpHelper();

		[SerializeField]
		private ColorMaskHelper m_colorMaskHelper = new ColorMaskHelper();

		[SerializeField]
		private RenderingPlatformOpHelper m_renderingPlatformOpHelper = new RenderingPlatformOpHelper();

		[SerializeField]
		private RenderingOptionsOpHelper m_renderingOptionsOpHelper = new RenderingOptionsOpHelper();

		[SerializeField]
		private BillboardOpHelper m_billboardOpHelper = new BillboardOpHelper();

		[SerializeField]
		private StandardShaderLightModel m_currentLightModel;

		[SerializeField]
		private StandardShaderLightModel m_lastLightModel;

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;

		[SerializeField]
		private AlphaMode m_alphaMode = AlphaMode.Opaque;

		[SerializeField]
		private RenderType m_renderType = RenderType.Opaque;

		[SerializeField]
		private RenderQueue m_renderQueue = RenderQueue.Geometry;

		[SerializeField]
		private RenderPath m_renderPath = RenderPath.All;

		[SerializeField]
		private bool m_customBlendMode = false;

		[SerializeField]
		private float m_opacityMaskClipValue = 0.5f;

		[SerializeField]
		private int m_discardPortId = -1;

		private int m_opacityPortId = -1;

		[SerializeField]
		private bool m_keepAlpha = true;

		[SerializeField]
		private bool m_castShadows = true;

		//[SerializeField]
		private bool m_customShadowCaster = false;

		[SerializeField]
		private bool m_receiveShadows = true;

		[SerializeField]
		private int m_queueOrder = 0;

		[SerializeField]
		private int m_grabOrder = 0;


		[SerializeField]
		private CacheNodeConnections m_cacheNodeConnections = new CacheNodeConnections();

		private ReorderableList m_propertyReordableList;
		private int m_lastCount = 0;

		private bool m_usingProSkin = false;
		private GUIStyle m_inspectorFoldoutStyle;
		private GUIStyle m_inspectorToolbarStyle;
		private GUIStyle m_inspectorTooldropdownStyle;
		private GUIStyle m_inspectorPopdropdownStyle;

		private bool m_customBlendAvailable = false;

		private Color m_cachedColor = Color.white;
		private float m_titleOpacity = 0.5f;
		private float m_boxOpacity = 0.5f;

		private InputPort m_refractionPort;

		private GUIStyle m_inspectorDefaultStyle;
		private GUIStyle m_propertyAdjustment;

		protected override void CommonInit( int uniqueId )
		{
			m_currentLightModel = m_lastLightModel = StandardShaderLightModel.Standard;
			m_textLabelWidth = 120;
			base.CommonInit( uniqueId );
			m_shaderNameContent = new GUIContent( ShaderNameStr, string.Empty );
		}

		public override void AddMasterPorts()
		{
			int index = 2;
			base.AddMasterPorts();
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, MetallicStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr, index++, MasterNodePortCategory.Fragment, 4 );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr, index++, MasterNodePortCategory.Fragment, 5 );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT3, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr, index++, MasterNodePortCategory.Fragment, 4 );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr, index++, MasterNodePortCategory.Fragment, 5 );
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1, MasterNodePortCategory.Fragment, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0, MasterNodePortCategory.Fragment, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr, index++, MasterNodePortCategory.Fragment, 2 );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr, index++, MasterNodePortCategory.Fragment, 3 );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr, index++, MasterNodePortCategory.Fragment, 4 );
				}
				break;
			}

			AddInputPort( WirePortDataType.FLOAT3, false, TransmissionStr, index++, MasterNodePortCategory.Fragment, 6 );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT3, false, TranslucencyStr, index++, MasterNodePortCategory.Fragment, 7 );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_currentLightModel == StandardShaderLightModel.Standard ) || ( m_currentLightModel == StandardShaderLightModel.StandardSpecular ) ? false : true;

			AddInputPort( WirePortDataType.FLOAT, false, RefractionStr, index + 2, MasterNodePortCategory.Fragment, 8 );
			m_refractionPort = m_inputPorts[ m_inputPorts.Count - 1 ];
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_alphaMode == AlphaMode.Opaque || m_alphaMode == AlphaMode.Masked );

			AddInputPort( WirePortDataType.FLOAT, false, AlphaStr, index++, MasterNodePortCategory.Fragment, 9 );
			m_inputPorts[ m_inputPorts.Count - 1 ].DataName = AlphaDataStr;
			m_opacityPortId = m_inputPorts.Count - 1;
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = ( m_alphaMode == AlphaMode.Opaque || m_alphaMode == AlphaMode.Masked );

			AddInputPort( WirePortDataType.OBJECT, false, DiscardStr, index++, MasterNodePortCategory.Fragment, 10 );
			m_discardPortId = m_inputPorts.Count - 1;

			index++;

			////////////////////////////////////////////////////////////////////////////////////////////////
			// Vertex functions - Adding ordex index in order to force these to be the last ones 
			////////////////////////////////////////////////////////////////////////////////////////////////

			m_tessOpHelper.VertexOffsetIndexPort = m_inputPorts.Count;
			AddInputPort( WirePortDataType.FLOAT3, false, VertexDisplacementStr, index++, MasterNodePortCategory.Vertex, 11 );
			AddInputPort( WirePortDataType.FLOAT3, false, VertexNormalStr, index++, MasterNodePortCategory.Vertex, 12 );


			AddInputPort( WirePortDataType.OBJECT, false, CustomLightModelStr, index++, MasterNodePortCategory.Fragment, 13 );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;

			m_tessOpHelper.MasterNodeIndexPort = m_inputPorts.Count;
			AddInputPort( WirePortDataType.FLOAT4, false, TessellationOpHelper.TessellationPortStr, index++, MasterNodePortCategory.Tessellation, 14 );


			////////////////////////////////////////////////////////////////////////////////////
			AddInputPort( WirePortDataType.FLOAT3, false, DebugStr, index++, MasterNodePortCategory.Debug, 15 );

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].CustomColor = Color.white;
			}
			m_sizeIsDirty = true;
		}

		public override void ForcePortType()
		{
			int portId = 0;
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
					m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
				}
				break;
			}
			//Transmission
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Translucency
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Refraction
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
			//Alpha
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT, false );
			//Discard
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			//Vertex Offset
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Vertex Normal
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
			//Custom Light
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.OBJECT, false );
			//Tessellation
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT4, false );
			//Debug
			m_inputPorts[ portId++ ].ChangeType( WirePortDataType.FLOAT3, false );
		}

		public override void SetName( string name )
		{
			ShaderName = name;
		}

		public void DrawInspectorProperty()
		{
			if ( m_inspectorDefaultStyle == null )
			{
				m_inspectorDefaultStyle = UIUtils.GetCustomStyle( CustomStyle.ResetToDefaultInspectorButton );
			}

			EditorGUILayout.BeginHorizontal();
			m_customInspectorName = EditorGUILayout.TextField( CustomInspectorStr, m_customInspectorName );
			if ( GUILayout.Button( string.Empty, UIUtils.GetCustomStyle( CustomStyle.ResetToDefaultInspectorButton ) ) )
			{
				GUIUtility.keyboardControl = 0;
				m_customInspectorName = Constants.DefaultCustomInspector;
			}
			EditorGUILayout.EndHorizontal();
		}

		void Swap( ref List<PropertyNode> list, int indexA, int indexB )
		{
			PropertyNode tmp = list[ indexA ];
			list[ indexA ] = list[ indexB ];
			list[ indexB ] = tmp;
		}

		public void DrawMaterialInputs( GUIStyle toolbarstyle )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;

			EditorGUI.BeginChangeCheck();
			UIUtils.CurrentWindow.ExpandedProperties = GUILayout.Toggle( UIUtils.CurrentWindow.ExpandedProperties, PropertyOderFoldoutStr, m_inspectorFoldoutStyle );
			if ( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedProperties", UIUtils.CurrentWindow.ExpandedProperties );
			}

			EditorGUILayout.EndHorizontal();
			if ( !UIUtils.CurrentWindow.ExpandedProperties )
				return;

			cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
			EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
			GUI.color = cachedColor;
			List<ParentNode> nodes = UIUtils.PropertyNodesList();
			if ( m_propertyReordableList == null || nodes.Count != m_lastCount )
			{
				List<PropertyNode> propertyNodes = new List<PropertyNode>();
				for ( int i = 0; i < nodes.Count; i++ )
				{
					PropertyNode node = nodes[ i ] as PropertyNode;
					if ( node != null )
					{
						propertyNodes.Add( node );
					}
				}

				propertyNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_propertyReordableList = new ReorderableList( propertyNodes, typeof( PropertyNode ), true, false, false, false );
				m_propertyReordableList.headerHeight = 0;
				m_propertyReordableList.footerHeight = 0;
				m_propertyReordableList.showDefaultBackground = false;

				m_propertyReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, propertyNodes[ index ].PropertyInspectorName );
				};

				m_propertyReordableList.onChangedCallback = ( list ) =>
				{
					for ( int i = 0; i < propertyNodes.Count; i++ )
					{
						propertyNodes[ i ].OrderIndex = i;
					}
				};

				m_lastCount = m_propertyReordableList.count;
			}

			if ( m_propertyReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		public void DrawGeneralOptions()
		{
			EditorGUI.BeginChangeCheck();
			string newShaderName = EditorGUILayout.TextField( m_shaderNameContent, m_shaderName );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( newShaderName.Length > 0 )
				{
					newShaderName = UIUtils.RemoveShaderInvalidCharacters( newShaderName );
				}
				else
				{
					newShaderName = DefaultShaderName;
				}
				ShaderName = newShaderName;
			}
			m_shaderNameContent.tooltip = m_shaderName;

			m_currentShaderType = ( AvailableShaderTypes ) EditorGUILayout.EnumPopup( m_shaderTypeLabel, m_currentShaderType );

			m_currentLightModel = ( StandardShaderLightModel ) EditorGUILayout.EnumPopup( LightModelContent, m_currentLightModel );

			m_shaderModelIdx = EditorGUILayout.Popup( ShaderModelStr, m_shaderModelIdx, ShaderModelTypeArr );

			EditorGUI.BeginChangeCheck();
			DrawPrecisionProperty();
			if ( EditorGUI.EndChangeCheck() )
				UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision = m_currentPrecisionType;

			m_cullMode = ( CullMode ) EditorGUILayout.EnumPopup( CullModeContent, m_cullMode );

			m_renderPath = ( RenderPath ) EditorGUILayout.EnumPopup( RenderPathContent, m_renderPath );

			m_castShadows = EditorGUILayout.Toggle( CastShadowsContent, m_castShadows );

			m_receiveShadows = EditorGUILayout.Toggle( ReceiveShadowsContent, m_receiveShadows );

			m_queueOrder = EditorGUILayout.IntField( QueueIndexContent, m_queueOrder );

			DrawInspectorProperty();
		
		}
		public override void DrawProperties()
		{
			if ( m_inspectorFoldoutStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
				m_inspectorFoldoutStyle = new GUIStyle( GUI.skin.GetStyle( "foldout" ) );

			if ( m_inspectorToolbarStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
			{
				m_inspectorToolbarStyle = new GUIStyle( GUI.skin.GetStyle( "toolbarbutton" ) );
				m_inspectorToolbarStyle.fixedHeight = 20;
			}

			if ( m_inspectorPopdropdownStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
			{
				m_inspectorPopdropdownStyle = new GUIStyle( GUI.skin.GetStyle( "PopupCurveDropdown" ) );
				m_inspectorPopdropdownStyle.alignment = TextAnchor.MiddleRight;
				m_inspectorPopdropdownStyle.border.bottom = 16;
			}

			if ( m_inspectorTooldropdownStyle == null || EditorGUIUtility.isProSkin != m_usingProSkin )
			{
				m_inspectorTooldropdownStyle = new GUIStyle( GUI.skin.GetStyle( "toolbardropdown" ) );
				m_inspectorTooldropdownStyle.fixedHeight = 20;
				m_inspectorTooldropdownStyle.margin.bottom = 2;
			}

			if ( EditorGUIUtility.isProSkin != m_usingProSkin )
				m_usingProSkin = EditorGUIUtility.isProSkin;

			base.DrawProperties();
			m_buttonStyle.fixedWidth = 200;
			m_buttonStyle.fixedHeight = 50;

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Separator();

				m_titleOpacity = 0.5f;
				m_boxOpacity = ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f );
				m_cachedColor = GUI.color;

				//  General
				bool generalIsVisible = EditorVariablesManager.ExpandedGeneralShaderOptions.Value;
				NodeUtils.DrawPropertyGroup( ref generalIsVisible, GeneralFoldoutStr, DrawGeneralOptions );
				EditorVariablesManager.ExpandedGeneralShaderOptions.Value = generalIsVisible;

				//Blend Mode
				GUI.color = new Color( m_cachedColor.r, m_cachedColor.g, m_cachedColor.b, m_titleOpacity );
				EditorGUILayout.BeginHorizontal( m_inspectorToolbarStyle );
				GUI.color = m_cachedColor;
				
				bool blendOptionsVisible = GUILayout.Toggle( EditorVariablesManager.ExpandedBlendOptions.Value, AlphaModeContent, UIUtils.MenuItemToggleStyle, GUILayout.ExpandWidth( true ) );
				if ( Event.current.button == Constants.FoldoutMouseId )
				{
					EditorVariablesManager.ExpandedBlendOptions.Value = blendOptionsVisible;
				}


				if ( !EditorGUIUtility.isProSkin )
					GUI.color = new Color( 0.25f, 0.25f, 0.25f, 1f );

				float boxSize = 60;
				switch ( m_alphaMode )
				{
					case AlphaMode.Transparent:
					boxSize = 85;
					break;
					case AlphaMode.Translucent:
					boxSize = 80;
					break;
					case AlphaMode.Premultiply:
					boxSize = 120;
					break;
				}
				EditorGUI.BeginChangeCheck();
				m_alphaMode = ( AlphaMode ) EditorGUILayout.Popup( string.Empty, ( int ) m_alphaMode, FadeModeOptions, m_inspectorPopdropdownStyle, GUILayout.Width( boxSize ), GUILayout.Height( 19 ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					//m_customBlendMode = false;
					UpdateFromBlendMode();
				}

				GUI.color = m_cachedColor;
				EditorGUILayout.EndHorizontal();

				m_customBlendAvailable = ( m_alphaMode == AlphaMode.Custom || m_alphaMode == AlphaMode.Opaque );

				if ( EditorVariablesManager.ExpandedBlendOptions.Value )
				{
					GUI.color = new Color( m_cachedColor.r, m_cachedColor.g, m_cachedColor.b, m_boxOpacity );
					EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
					GUI.color = m_cachedColor;
					EditorGUI.indentLevel++;
					EditorGUILayout.Separator();
					EditorGUI.BeginChangeCheck();

					m_renderType = ( RenderType ) EditorGUILayout.EnumPopup( RenderTypeContent, m_renderType );

					m_renderQueue = ( RenderQueue ) EditorGUILayout.EnumPopup( RenderQueueContent, m_renderQueue );

					if ( EditorGUI.EndChangeCheck() )
					{
						if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
							m_alphaMode = AlphaMode.Opaque;
						else if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
							m_alphaMode = AlphaMode.Masked;
						else if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
							m_alphaMode = AlphaMode.Transparent;
						else if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Transparent )
							m_alphaMode = AlphaMode.Translucent;
						else
							m_alphaMode = AlphaMode.Custom;

						UpdateFromBlendMode();
					}

					bool bufferedEnabled = GUI.enabled;

					GUI.enabled = ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom );
					m_inputPorts[ m_discardPortId ].Locked = !GUI.enabled;
					EditorGUI.BeginChangeCheck();
					m_opacityMaskClipValue = EditorGUILayout.FloatField( OpacityMaskClipValueContent, m_opacityMaskClipValue );
					if ( EditorGUI.EndChangeCheck() )
					{
						if ( m_currentMaterial.HasProperty( IOUtils.MaskClipValueName ) )
						{
							m_currentMaterial.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
						}
					}

					GUI.enabled = bufferedEnabled;

					EditorGUI.BeginDisabledGroup( !( m_alphaMode == AlphaMode.Transparent || m_alphaMode == AlphaMode.Premultiply || m_alphaMode == AlphaMode.Translucent || m_alphaMode == AlphaMode.Custom ) );
					m_grabOrder = EditorGUILayout.IntField( RefractionLayerStr, m_grabOrder );
					EditorGUI.EndDisabledGroup();

					EditorGUILayout.Separator();

					if ( !m_customBlendAvailable )
					{
						EditorGUI.indentLevel--;
						EditorGUILayout.HelpBox( "Advanced options are only available for Custom blend modes", MessageType.Warning );
						EditorGUI.indentLevel++;
					}

					EditorGUI.BeginDisabledGroup( !m_customBlendAvailable );
					m_blendOpsHelper.Draw( m_customBlendAvailable );
					m_colorMaskHelper.Draw();

					EditorGUI.EndDisabledGroup();
					EditorGUILayout.Separator();
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}


				m_stencilBufferHelper.Draw( m_inspectorToolbarStyle );
				m_tessOpHelper.Draw( m_inspectorToolbarStyle, m_currentMaterial, m_inputPorts[ m_tessOpHelper.MasterNodeIndexPort ].IsConnected );
				m_outlineHelper.Draw( m_inspectorToolbarStyle, m_currentMaterial );
				m_billboardOpHelper.Draw();
				m_zBufferHelper.Draw( m_inspectorToolbarStyle, m_customBlendAvailable );
				m_renderingOptionsOpHelper.Draw();
				m_renderingPlatformOpHelper.Draw();
				
				DrawMaterialInputs( m_inspectorToolbarStyle );
			}

			EditorGUILayout.EndVertical();

			if ( m_currentLightModel != m_lastLightModel )
			{
				CacheCurrentSettings();
				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				ConnectFromCache();
			}
		}

		private void CacheCurrentSettings()
		{
			m_cacheNodeConnections.Clear();
			for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
			{
				if ( m_inputPorts[ portId ].IsConnected )
				{
					WireReference connection = m_inputPorts[ portId ].GetConnection();
					m_cacheNodeConnections.Add( m_inputPorts[ portId ].Name, new NodeCache( connection.NodeId, connection.PortId ) );
				}
			}
		}

		private void ConnectFromCache()
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				NodeCache cache = m_cacheNodeConnections.Get( m_inputPorts[ i ].Name );
				if ( cache != null )
				{
					UIUtils.SetConnection( m_uniqueId, m_inputPorts[ i ].PortId, cache.TargetNodeId, cache.TargetPortId );
				}
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( mat.HasProperty( IOUtils.MaskClipValueName ) )
					mat.SetFloat( IOUtils.MaskClipValueName, m_opacityMaskClipValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( m_materialMode && mat.HasProperty( IOUtils.MaskClipValueName ) )
				{
					m_opacityMaskClipValue = mat.GetFloat( IOUtils.MaskClipValueName );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			m_tessOpHelper.UpdateFromMaterial( material );
			m_outlineHelper.UpdateFromMaterial( material );

			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom )
			{
				if ( material.HasProperty( IOUtils.MaskClipValueName ) )
					m_opacityMaskClipValue = material.GetFloat( IOUtils.MaskClipValueName );
			}
		}

		public override void UpdateMasterNodeMaterial( Material material )
		{
			m_currentMaterial = material;
			UpdateMaterialEditor();
		}

		void UpdateMaterialEditor()
		{
			FireMaterialChangedEvt();
		}

		public string CreateInstructionsForVertexPort( InputPort port )
		{
			//Vertex displacement and per vertex custom data
			UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Vertex;
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref UIUtils.CurrentDataCollector, false );

			if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
			{
				UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, m_uniqueId, false );
				UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
			}

			if ( UIUtils.CurrentDataCollector.DirtyVertexVariables )
			{
				UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, m_uniqueId, false );
				UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
			}

			return vertexInstructions;
		}

		public void CreateInstructionsForPort( InputPort port, string portName, bool addCustomDelimiters = false, string customDelimiterIn = null, string customDelimiterOut = null, bool ignoreLocalVar = false, bool normalIsConnected = false )
		{
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string newInstruction = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref UIUtils.CurrentDataCollector, ignoreLocalVar );

			if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
			{
				UIUtils.CurrentDataCollector.AddInstructions( UIUtils.CurrentDataCollector.SpecialLocalVariables );
				UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
			}

			if ( UIUtils.CurrentDataCollector.DirtyVertexVariables )
			{
				UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.VertexLocalVariables, port.NodeId, false );
				UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
			}

			if ( UIUtils.CurrentDataCollector.ForceNormal && !normalIsConnected )
			{
				UIUtils.CurrentDataCollector.AddToStartInstructions( "\t\t\t" + Constants.OutputVarStr + ".Normal = float3(0,0,1);\n" );
				UIUtils.CurrentDataCollector.DirtyNormal = true;
				UIUtils.CurrentDataCollector.ForceNormal = false;
			}

			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterIn : ( "\t\t\t" + portName + " = " ) );
			UIUtils.CurrentDataCollector.AddInstructions( newInstruction );
			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterOut : ";\n" );
		}

		public void GenerateTags()
		{
			bool hasVirtualTexture = UIUtils.HasVirtualTexture();

			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + ( ( m_queueOrder >= 0 ) ? "+" : string.Empty ) + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Transparent || m_alphaMode == AlphaMode.Premultiply || m_alphaMode == AlphaMode.Translucent )
				{
					tags += " \"IgnoreProjector\" = \"True\"";
				}
			}

			//add virtual texture support
			if ( hasVirtualTexture )
			{
				tags += " \"Amplify\" = \"True\" ";
			}

			tags = "Tags{ " + tags + " }";
		}

		public override Shader Execute( string pathname, bool isFullPath )
		{
			ForcePortType();
			base.Execute( pathname, isFullPath );

			bool isInstancedShader = UIUtils.IsInstancedShader();
			bool hasVirtualTexture = UIUtils.HasVirtualTexture();
			bool hasTranslucency = false;
			bool hasTransmission = false;
			bool hasEmission = false;
			bool hasOpacity = false;
			bool hasRefraction = false;
			bool hasVertexOffset = false;

			string refractionCode = string.Empty;
			string refractionInstructions = string.Empty;
			string refractionFix = string.Empty;
			
			UIUtils.CurrentDataCollector = new MasterNodeDataCollector( this );
			UIUtils.CurrentDataCollector.TesselationActive = m_tessOpHelper.EnableTesselation;

			// See if each node is being used on frag and/or vert ports
			SetupNodeCategories();
			if ( m_refractionPort.IsConnected || m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				UIUtils.CurrentDataCollector.DirtyNormal = true;
				UIUtils.CurrentDataCollector.ForceNormal = true;
			}
			//this.PropagateNodeData( nodeData );

			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + ( ( m_queueOrder >= 0 ) ? "+" : string.Empty ) + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Transparent || m_alphaMode == AlphaMode.Premultiply )
				{
					tags += " \"IgnoreProjector\" = \"True\"";
				}
			}

			//add virtual texture support
			if ( hasVirtualTexture )
			{
				tags += " \"Amplify\" = \"True\" ";
			}

			//tags = "Tags{ " + tags + " }";

			string outputStruct = "";
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard: outputStruct = "SurfaceOutputStandard"; break;
				case StandardShaderLightModel.StandardSpecular: outputStruct = "SurfaceOutputStandardSpecular"; break;
				case StandardShaderLightModel.Lambert:
				case StandardShaderLightModel.BlinnPhong: outputStruct = "SurfaceOutput"; break;
			}

			// Need to sort before creating local vars so they can inspect the normal port correctly
			SortedList<int, InputPort> sortedPorts = new SortedList<int, InputPort>();
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				sortedPorts.Add( m_inputPorts[ i ].OrderId, m_inputPorts[ i ] );
			}

			bool normalIsConnected = sortedPorts[ 0 ].IsConnected;

			m_tessOpHelper.Reset();
			if ( m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				//Debug Port active
				UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Debug;
				InputPort debugPort = m_inputPorts[ m_inputPorts.Count - 1 ];

				CreateInstructionsForPort( debugPort, Constants.OutputVarStr + ".Emission", false, null, null, false, false );
			}
			else
			{
				// Custom Light Model
				//TODO: Create Custom Light behaviour
				//Collect data from standard nodes
				for ( int i = 0; i < sortedPorts.Count; i++ )
				{
					if ( sortedPorts[ i ].IsConnected )
					{
						if ( i == 0 )// Normal Map is Connected
						{
							UIUtils.CurrentDataCollector.DirtyNormal = true;
						}
						if ( sortedPorts[ i ].Name.Equals( TranslucencyStr ) )
						{
							hasTranslucency = true;
						}
						if ( sortedPorts[ i ].Name.Equals( TransmissionStr ) )
						{
							hasTransmission = true;
						}
						if ( sortedPorts[ i ].Name.Equals( EmissionStr ) )
						{
							hasEmission = true;
						}

						if ( sortedPorts[ i ].Name.Equals( RefractionStr ) )
						{
							hasRefraction = true;
						}

						if ( sortedPorts[ i ].Name.Equals( AlphaStr ) )
						{
							hasOpacity = true;
						}

						if ( hasRefraction )
						{
							UIUtils.CurrentDataCollector.AddToInput( m_uniqueId, "float4 screenPos", true );
							UIUtils.CurrentDataCollector.AddToInput( m_uniqueId, "float3 worldPos", true );

							//not necessary, just being safe
							UIUtils.CurrentDataCollector.DirtyNormal = true;
							UIUtils.CurrentDataCollector.ForceNormal = true;

							UIUtils.CurrentDataCollector.AddGrabPass( "RefractionGrab" + m_grabOrder );
							UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform sampler2D RefractionGrab" + m_grabOrder + ";" );
							UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform float _ChromaticAberration;" );

							UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "[Header(Refraction)]", 210 );
							UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_ChromaticAberration(\"Chromatic Aberration\", Range( 0 , 0.3)) = 0.1", 211 );

							UIUtils.CurrentDataCollector.AddToPragmas( m_uniqueId, "multi_compile _ALPHAPREMULTIPLY_ON" );
						}

						if ( hasTranslucency || hasTransmission )
						{
							//Translucency and Transmission Generation

							//Add properties and uniforms
							UIUtils.CurrentDataCollector.AddToIncludes( m_uniqueId, Constants.UnityPBSLightingLib );

							if ( hasTranslucency )
							{
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "[Header(Translucency)]", 200 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_Translucency(\"Strength\", Range( 0 , 50)) = 1", 201 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransNormalDistortion(\"Normal Distortion\", Range( 0 , 1)) = 0.1", 202 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransScattering(\"Scaterring Falloff\", Range( 1 , 50)) = 2", 203 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransDirect(\"Direct\", Range( 0 , 1)) = 1", 204 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransAmbient(\"Ambient\", Range( 0 , 1)) = 0.2", 205 );
								UIUtils.CurrentDataCollector.AddToProperties( m_uniqueId, "_TransShadow(\"Shadow\", Range( 0 , 1)) = 0.9", 206 );

								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _Translucency;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransNormalDistortion;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransScattering;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransDirect;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransAmbient;" );
								UIUtils.CurrentDataCollector.AddToUniforms( m_uniqueId, "uniform half _TransShadow;" );
							}

							//Add custom struct
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								case StandardShaderLightModel.StandardSpecular:
								outputStruct = "SurfaceOutput" + m_currentLightModel.ToString() + Constants.CustomLightStructStr; break;
							}

							UIUtils.CurrentDataCollector.ChangeCustomInputHeader( m_currentLightModel.ToString() + Constants.CustomLightStructStr );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Albedo", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Normal", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half3 Emission", true );
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Metallic", true );
								break;
								case StandardShaderLightModel.StandardSpecular:
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Specular", true );
								break;
							}
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Smoothness", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "half Occlusion", true );
							UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed Alpha", true );
							if ( hasTranslucency )
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Translucency", true );

							if ( hasTransmission )
								UIUtils.CurrentDataCollector.AddToCustomInput( m_uniqueId, "fixed3 Transmission", true );
						}

						if ( sortedPorts[ i ].Name.Equals( DiscardStr ) )
						{
							//Discard Op Node
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							string opacityValue = "0.0";
							switch ( sortedPorts[ i ].ConnectionType() )
							{
								case WirePortDataType.INT:
								case WirePortDataType.FLOAT:
								{
									opacityValue = IOUtils.MaskClipValueName;//UIUtils.FloatToString( m_opacityMaskClipValue );
								}
								break;

								case WirePortDataType.FLOAT2:
								{
									opacityValue = string.Format( "( {0} ).xx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT3:
								{
									opacityValue = string.Format( "( {0} ).xxx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT4:
								{
									opacityValue = string.Format( "( {0} ).xxxx", IOUtils.MaskClipValueName );
								}
								break;
							}
							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, true, "\t\t\tclip( ", " - " + opacityValue + " );\n", false, normalIsConnected );
						}
						else if ( sortedPorts[ i ].Name.Equals( VertexDisplacementStr ) )
						{
							hasVertexOffset = true;
							UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();
							string vertexInstructions = CreateInstructionsForVertexPort( sortedPorts[ i ] );
							UIUtils.CurrentDataCollector.AddToVertexDisplacement( vertexInstructions );
						}
						else if ( sortedPorts[ i ].Name.Equals( VertexNormalStr ) )
						{
							if ( !hasVertexOffset )
								UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();

							string vertexInstructions = CreateInstructionsForVertexPort( sortedPorts[ i ] );
							UIUtils.CurrentDataCollector.AddToVertexNormal( vertexInstructions );
						}
						else if ( m_tessOpHelper.EnableTesselation && m_tessOpHelper.IsTessellationPort( sortedPorts[ i ].PortId ) )
						{
							UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();
							//Vertex displacement and per vertex custom data
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Tessellation;
							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, sortedPorts[ i ].DataType, ref UIUtils.CurrentDataCollector, false );

							if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
							{
								m_tessOpHelper.AddAdditionalData( UIUtils.CurrentDataCollector.SpecialLocalVariables );
								UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
							}

							if ( UIUtils.CurrentDataCollector.DirtyVertexVariables )
							{
								m_tessOpHelper.AddAdditionalData( UIUtils.CurrentDataCollector.VertexLocalVariables );
								UIUtils.CurrentDataCollector.ClearVertexLocalVariables();
							}

							m_tessOpHelper.AddCustomFunction( vertexInstructions );
						}
						else if ( sortedPorts[ i ].Name.Equals( RefractionStr ) )
						{
							UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();
							UIUtils.CurrentDataCollector.UsingCustomOutput = true;

							refractionFix = " + 0.00001 * i.screenPos * i.worldPos";
							UIUtils.CurrentDataCollector.AddInstructions( "\t\t\to.Normal = o.Normal" + refractionFix + ";\n" );
							WireReference connection = sortedPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							refractionCode = node.GetValueFromOutputStr( connection.PortId, sortedPorts[ i ].DataType, ref UIUtils.CurrentDataCollector, true );
							refractionInstructions = UIUtils.CurrentDataCollector.CustomOutput;

							UIUtils.CurrentDataCollector.UsingCustomOutput = false;
						}
						else
						{
							// Surface shader instruccions
							UIUtils.CurrentDataCollector.PortCategory = MasterNodePortCategory.Fragment;
							// if working on normals and have normal dependent node then ignore local var generation
							bool ignoreLocalVar = ( i == 0 && UIUtils.IsNormalDependent() );

							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, false, null, null, ignoreLocalVar, normalIsConnected );
						}
					}
					else if ( sortedPorts[ i ].Name.Equals( AlphaStr ) )
					{
						UIUtils.CurrentDataCollector.AddInstructions( string.Format( "\t\t\t{0}.{1} = 1;\n", Constants.OutputVarStr, sortedPorts[ i ].DataName ) );
					}
				}
				
				m_billboardOpHelper.FillDataCollector( ref UIUtils.CurrentDataCollector );
			}


			if ( ( m_castShadows && hasOpacity ) || ( m_castShadows && ( UIUtils.CurrentDataCollector.UsingWorldNormal || UIUtils.CurrentDataCollector.UsingWorldReflection || UIUtils.CurrentDataCollector.UsingViewDirection ) ) )
				m_customShadowCaster = true;
			else
				m_customShadowCaster = false;


			for ( int i = 0; i < 4; i++ )
			{
				if ( UIUtils.CurrentDataCollector.GetChannelUsage( i ) == TextureChannelUsage.Required )
				{
					string channelName = UIUtils.GetChannelName( i );
					UIUtils.CurrentDataCollector.AddToProperties( -1, UIUtils.GetTex2DProperty( channelName, TexturePropertyValues.white ), -1 );
				}
			}

			UIUtils.CurrentDataCollector.AddToProperties( -1, IOUtils.DefaultASEDirtyCheckProperty, -1 );
			if ( m_alphaMode == AlphaMode.Masked || m_alphaMode == AlphaMode.Custom/*&& !m_customBlendMode*/ )
			{
				UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format( IOUtils.MaskClipValueProperty, OpacityMaskClipValueStr, m_opacityMaskClipValue ), -1 );
				UIUtils.CurrentDataCollector.AddToUniforms( -1, string.Format( IOUtils.MaskClipValueUniform, m_opacityMaskClipValue ) );
			}

			if ( !UIUtils.CurrentDataCollector.DirtyInputs )
				UIUtils.CurrentDataCollector.AddToInput( m_uniqueId, "fixed filler", true );

			if ( m_currentLightModel == StandardShaderLightModel.BlinnPhong )
				UIUtils.CurrentDataCollector.AddToProperties( -1, "[HideInInspector]_SpecColor(\"SpecularColor\",Color)=(1,1,1,1)", -1 );

			//Tesselation
			if ( m_tessOpHelper.EnableTesselation )
			{
				m_tessOpHelper.AddToDataCollector( ref UIUtils.CurrentDataCollector );
				if ( !UIUtils.CurrentDataCollector.DirtyPerVertexData )
				{
					UIUtils.CurrentDataCollector.OpenPerVertexHeader( false );
				}
			}
			if ( m_outlineHelper.EnableOutline )
			{
				m_outlineHelper.AddToDataCollector( ref UIUtils.CurrentDataCollector );
			}

			UIUtils.CurrentDataCollector.CloseInputs();
			UIUtils.CurrentDataCollector.CloseCustomInputs();
			UIUtils.CurrentDataCollector.CloseProperties();
			UIUtils.CurrentDataCollector.ClosePerVertexHeader();

			//build Shader Body
			string ShaderBody = string.Empty;
			OpenShaderBody( ref ShaderBody, m_shaderName );
			{
				//set properties
				if ( UIUtils.CurrentDataCollector.DirtyProperties )
				{
					ShaderBody += UIUtils.CurrentDataCollector.BuildPropertiesString();
				}
				//set subshader
				OpenSubShaderBody( ref ShaderBody );
				{
					// Add optionalPasses
					if ( m_outlineHelper.EnableOutline )
					{
						AddMultilineBody( ref ShaderBody, m_outlineHelper.OutlineFunctionBody( isInstancedShader, m_customShadowCaster, UIUtils.RemoveInvalidCharacters( ShaderName ) , ( m_billboardOpHelper.IsBillboard ? m_billboardOpHelper.BillboardCylindricalInstructions : null ) ) );
					}

					//Add SubShader tags
					if ( hasEmission )
					{
						tags += " \"IsEmissive\" = \"true\" ";
					}

					tags = "Tags{ " + tags + " }";
					AddRenderTags( ref ShaderBody, tags );
					AddRenderState( ref ShaderBody, "Cull", m_cullMode.ToString() );
					m_customBlendAvailable = ( m_alphaMode == AlphaMode.Custom || m_alphaMode == AlphaMode.Opaque );
					if ( m_zBufferHelper.IsActive && m_customBlendAvailable )
					{
						ShaderBody += m_zBufferHelper.CreateDepthInfo();
					}
					if ( m_stencilBufferHelper.Active )
					{
						ShaderBody += m_stencilBufferHelper.CreateStencilOp();
					}

					if ( m_blendOpsHelper.Active )
					{
						ShaderBody += m_blendOpsHelper.CreateBlendOps();
					}

					// Build Color Mask
					m_colorMaskHelper.BuildColorMask( ref ShaderBody, m_customBlendAvailable );

					//ShaderBody += "\t\tZWrite " + _zWriteMode + '\n';
					//ShaderBody += "\t\tZTest " + _zTestMode + '\n';

					//Add GrabPass
					if ( UIUtils.CurrentDataCollector.DirtyGrabPass )
					{
						ShaderBody += UIUtils.CurrentDataCollector.GrabPass;
					}

					// build optional parameters
					string OptionalParameters = string.Empty;


					//add cg program
					if ( m_customShadowCaster )
						OpenCGInclude( ref ShaderBody );
					else
						OpenCGProgram( ref ShaderBody );
					{
						//Add Includes
						if ( m_customShadowCaster )
						{
							UIUtils.CurrentDataCollector.AddToIncludes( m_uniqueId, Constants.UnityPBSLightingLib );
							UIUtils.CurrentDataCollector.AddToIncludes( m_uniqueId, "Lighting.cginc" );
						}
						if ( UIUtils.CurrentDataCollector.DirtyIncludes )
							ShaderBody += UIUtils.CurrentDataCollector.Includes;

						//define as surface shader and specify lighting model
						if ( UIUtils.GetTextureArrayNodeAmount() > 0 && m_shaderModelIdx < 3 )
						{
							Debug.Log( "Automically changing Shader Model to 3.5 since it's the minimum required by texture arrays." );
							m_shaderModelIdx = 3;
						}

						// if tessellation is active then we need be at least using shader model 4.6
						if ( m_tessOpHelper.EnableTesselation && m_shaderModelIdx < 6 )
						{
							Debug.Log( "Automically changing Shader Model to 4.6 since it's the minimum required by tessellation." );
							m_shaderModelIdx = 6;
						}
						ShaderBody += string.Format( IOUtils.PragmaTargetHeader, ShaderModelTypeArr[ m_shaderModelIdx ] );

						if ( isInstancedShader )
						{
							ShaderBody += IOUtils.InstancedPropertiesHeader;
						}

						//Add pragmas
						if ( UIUtils.CurrentDataCollector.DirtyPragmas && !m_customShadowCaster )
							ShaderBody += UIUtils.CurrentDataCollector.Pragmas;

						//if ( !m_customBlendMode )
						{
							switch ( m_alphaMode )
							{
								case AlphaMode.Opaque:
								case AlphaMode.Masked: break;
								case AlphaMode.Transparent:
								{
									OptionalParameters += "alpha:fade" + Constants.OptionalParametersSep;
								}
								break;
								case AlphaMode.Premultiply:
								{
									OptionalParameters += "alpha:premul" + Constants.OptionalParametersSep;
								}
								break;
							}
						}

						if ( m_keepAlpha )
						{
							OptionalParameters += "keepalpha" + Constants.OptionalParametersSep;
						}

						if ( hasRefraction )
						{
							OptionalParameters += "finalcolor:RefractionF" + Constants.OptionalParametersSep;
						}

						if ( !m_customShadowCaster )
							OptionalParameters += ( ( m_castShadows ) ? "addshadow" + Constants.OptionalParametersSep + "fullforwardshadows" : "" ) + Constants.OptionalParametersSep;
						//OptionalParameters += ( ( m_castShadows ) ? "addshadow" + Constants.OptionalParametersSep + "fullforwardshadows" : "" ) + Constants.OptionalParametersSep;

						OptionalParameters += m_receiveShadows ? "" : "noshadow" + Constants.OptionalParametersSep;

						switch ( m_renderPath )
						{
							case RenderPath.All: break;
							case RenderPath.DeferredOnly: OptionalParameters += "exclude_path:forward" + Constants.OptionalParametersSep; break;
							case RenderPath.ForwardOnly: OptionalParameters += "exclude_path:deferred" + Constants.OptionalParametersSep; break;
						}

						//Add code generation options
						m_renderingOptionsOpHelper.Build( ref OptionalParameters );

						if ( !m_customShadowCaster )
						{
							string customLightSurface = hasTranslucency || hasTransmission ? "Custom" : "";
							m_renderingPlatformOpHelper.SetRenderingPlatforms( ref ShaderBody );

							//Check if Custom Vertex is being used and add tag
							if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
								OptionalParameters += "vertex:" + Constants.VertexDataFunc + Constants.OptionalParametersSep;

							if ( m_tessOpHelper.EnableTesselation )
							{
								m_tessOpHelper.WriteToOptionalParams( ref OptionalParameters );
							}
							AddShaderPragma( ref ShaderBody, "surface surf " + m_currentLightModel.ToString() + customLightSurface + Constants.OptionalParametersSep + OptionalParameters );
						}
						else
						{
							if ( UIUtils.CurrentDataCollector.UsingWorldNormal || UIUtils.CurrentDataCollector.UsingInternalData )
							{
								//ShaderBody += "\t\t#if UNITY_PASS_SHADOWCASTER\n";

								ShaderBody += "\t\t#ifdef UNITY_PASS_SHADOWCASTER\n";
								ShaderBody += "\t\t\t#undef INTERNAL_DATA\n";
								ShaderBody += "\t\t\t#undef WorldReflectionVector\n";
								ShaderBody += "\t\t\t#undef WorldNormalVector\n";
								ShaderBody += "\t\t\t#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;\n";
								ShaderBody += "\t\t\t#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))\n";
								ShaderBody += "\t\t\t#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))\n";
								ShaderBody += "\t\t#endif\n";
							}
						}

						if ( UIUtils.CurrentDataCollector.UsingHigherSizeTexcoords )
						{
							ShaderBody += "\t\t#undef TRANSFORM_TEX\n";
							ShaderBody += "\t\t#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)\n";
						}


						// Add Input struct
						if ( UIUtils.CurrentDataCollector.DirtyInputs )
							ShaderBody += UIUtils.CurrentDataCollector.Inputs + "\n\n";

						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += TessellationOpHelper.CustomAppData;
						}

						// Add Custom Lighting struct
						if ( UIUtils.CurrentDataCollector.DirtyCustomInput )
							ShaderBody += UIUtils.CurrentDataCollector.CustomInput + "\n\n";

						//Add Uniforms
						if ( UIUtils.CurrentDataCollector.DirtyUniforms )
							ShaderBody += UIUtils.CurrentDataCollector.Uniforms + "\n";


						//Add Instanced Properties
						if ( isInstancedShader && UIUtils.CurrentDataCollector.DirtyInstancedProperties )
						{
							UIUtils.CurrentDataCollector.SetupInstancePropertiesBlock( UIUtils.RemoveInvalidCharacters( ShaderName ) );
							ShaderBody += UIUtils.CurrentDataCollector.InstancedProperties + "\n";
						}

						if ( UIUtils.CurrentDataCollector.DirtyFunctions )
							ShaderBody += UIUtils.CurrentDataCollector.Functions + "\n";


						//Tesselation
						if ( m_tessOpHelper.EnableTesselation )
						{
							ShaderBody += m_tessOpHelper.GetCurrentTessellationFunction + "\n";
						}

						//Add Custom Vertex Data
						if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
						{
							ShaderBody += UIUtils.CurrentDataCollector.VertexData;
						}


						//Add custom lighting function
						if ( hasTranslucency || hasTransmission )
						{
							ShaderBody += "\t\tinline half4 Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", half3 viewDir, UnityGI gi )\n\t\t{\n";
							if ( hasTranslucency )
							{
								ShaderBody += "\t\t\t#if !DIRECTIONAL\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = gi.light.color;\n";
								ShaderBody += "\t\t\t#else\n";
								ShaderBody += "\t\t\tfloat3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );\n";
								ShaderBody += "\t\t\t#endif\n";
								ShaderBody += "\t\t\thalf3 lightDir = gi.light.dir + " + Constants.CustomLightOutputVarStr + ".Normal * _TransNormalDistortion;\n";
								ShaderBody += "\t\t\thalf transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );\n";
								ShaderBody += "\t\t\thalf3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * " + Constants.CustomLightOutputVarStr + ".Translucency;\n";
								ShaderBody += "\t\t\thalf4 c = half4( " + Constants.CustomLightOutputVarStr + ".Albedo * translucency * _Translucency, 0 );\n\n";
							}

							if ( hasTransmission )
							{
								ShaderBody += "\t\t\thalf3 transmission = max(0 , -dot(" + Constants.CustomLightOutputVarStr + ".Normal, gi.light.dir)) * gi.light.color * " + Constants.CustomLightOutputVarStr + ".Transmission;\n";
								ShaderBody += "\t\t\thalf4 d = half4(" + Constants.CustomLightOutputVarStr + ".Albedo * transmission , 0);\n\n";
							}

							ShaderBody += "\t\t\tSurfaceOutput" + m_currentLightModel.ToString() + " r;\n";
							ShaderBody += "\t\t\tr.Albedo = " + Constants.CustomLightOutputVarStr + ".Albedo;\n";
							ShaderBody += "\t\t\tr.Normal = " + Constants.CustomLightOutputVarStr + ".Normal;\n";
							ShaderBody += "\t\t\tr.Emission = " + Constants.CustomLightOutputVarStr + ".Emission;\n";
							switch ( m_currentLightModel )
							{
								case StandardShaderLightModel.Standard:
								ShaderBody += "\t\t\tr.Metallic = " + Constants.CustomLightOutputVarStr + ".Metallic;\n";
								break;
								case StandardShaderLightModel.StandardSpecular:
								ShaderBody += "\t\t\tr.Specular = " + Constants.CustomLightOutputVarStr + ".Specular;\n";
								break;
							}
							ShaderBody += "\t\t\tr.Smoothness = " + Constants.CustomLightOutputVarStr + ".Smoothness;\n";
							ShaderBody += "\t\t\tr.Occlusion = " + Constants.CustomLightOutputVarStr + ".Occlusion;\n";
							ShaderBody += "\t\t\tr.Alpha = " + Constants.CustomLightOutputVarStr + ".Alpha;\n";
							ShaderBody += "\t\t\treturn Lighting" + m_currentLightModel.ToString() + " (r, viewDir, gi)" + ( hasTranslucency ? " + c" : "" ) + ( hasTransmission ? " + d" : "" ) + ";\n";
							ShaderBody += "\t\t}\n\n";

							//Add GI function
							ShaderBody += "\t\tinline void Lighting" + m_currentLightModel.ToString() + Constants.CustomLightStructStr + "_GI(" + outputStruct + " " + Constants.CustomLightOutputVarStr + ", UnityGIInput data, inout UnityGI gi )\n\t\t{\n";
							ShaderBody += "\t\t\tUNITY_GI(gi, " + Constants.CustomLightOutputVarStr + ", data);\n";
							ShaderBody += "\t\t}\n\n";
						}

						if ( hasRefraction )
						{
							ShaderBody += "\t\tinline float4 Refraction( Input " + Constants.InputVarStr + ", " + outputStruct + " " + Constants.OutputVarStr + ", float indexOfRefraction, float chomaticAberration ) {\n";
							ShaderBody += "\t\t\tfloat3 worldNormal = " + Constants.OutputVarStr + ".Normal;\n";
							ShaderBody += "\t\t\tfloat4 screenPos = " + Constants.InputVarStr + ".screenPos;\n";
							ShaderBody += "\t\t\t#if UNITY_UV_STARTS_AT_TOP\n";
							ShaderBody += "\t\t\t\tfloat scale = -1.0;\n";
							ShaderBody += "\t\t\t#else\n";
							ShaderBody += "\t\t\t\tfloat scale = 1.0;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t\tfloat halfPosW = screenPos.w * 0.5;\n";
							ShaderBody += "\t\t\tscreenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;\n";
							ShaderBody += "\t\t\t#if SHADER_API_D3D9 || SHADER_API_D3D11\n";
							ShaderBody += "\t\t\t\tscreenPos.w += 0.0000001;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t\tfloat2 projScreenPos = ( screenPos / screenPos.w ).xy;\n";
							ShaderBody += "\t\t\tfloat3 worldViewDir = normalize( UnityWorldSpaceViewDir( " + Constants.InputVarStr + ".worldPos ) );\n";
							ShaderBody += "\t\t\tfloat3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );\n";
							ShaderBody += "\t\t\tfloat2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );\n";
							ShaderBody += "\t\t\tfloat4 redAlpha = tex2D( RefractionGrab" + m_grabOrder + ", ( projScreenPos + cameraRefraction ) );\n";
							ShaderBody += "\t\t\tfloat green = tex2D( RefractionGrab" + m_grabOrder + ", ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;\n";
							ShaderBody += "\t\t\tfloat blue = tex2D( RefractionGrab" + m_grabOrder + ", ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;\n";
							ShaderBody += "\t\t\treturn float4( redAlpha.r, green, blue, redAlpha.a );\n";
							ShaderBody += "\t\t}\n\n";

							ShaderBody += "\t\tvoid RefractionF( Input " + Constants.InputVarStr + ", " + outputStruct + " " + Constants.OutputVarStr + ", inout fixed4 color )\n";
							ShaderBody += "\t\t{\n";
							ShaderBody += "\t\t\t#ifdef UNITY_PASS_FORWARDBASE\n";
							ShaderBody += refractionInstructions;
							ShaderBody += "\t\t\t\tcolor.rgb = color.rgb + Refraction( " + Constants.InputVarStr + ", " + Constants.OutputVarStr + ", " + refractionCode + ", _ChromaticAberration ) * ( 1 - color.a );\n";
							ShaderBody += "\t\t\t\tcolor.a = 1;\n";
							ShaderBody += "\t\t\t#endif\n";
							ShaderBody += "\t\t}\n\n";
						}

						//Add Surface Shader body
						ShaderBody += "\t\tvoid surf( Input " + Constants.InputVarStr + " , inout " + outputStruct + " " + Constants.OutputVarStr + " )\n\t\t{\n";
						{
							//add local vars
							if ( UIUtils.CurrentDataCollector.DirtyLocalVariables )
								ShaderBody += UIUtils.CurrentDataCollector.LocalVariables;

							//add nodes ops
							if ( UIUtils.CurrentDataCollector.DirtyInstructions )
								ShaderBody += UIUtils.CurrentDataCollector.Instructions;
						}
						ShaderBody += "\t\t}\n";
					}
					CloseCGProgram( ref ShaderBody );


					//Add custom Shadow Caster
					if ( m_customShadowCaster )
					{
						OpenCGProgram( ref ShaderBody );
						string customLightSurface = hasTranslucency || hasTransmission ? "Custom" : "";
						m_renderingPlatformOpHelper.SetRenderingPlatforms( ref ShaderBody );

						//Check if Custom Vertex is being used and add tag
						if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
							OptionalParameters += "vertex:" + Constants.VertexDataFunc + Constants.OptionalParametersSep;

						if ( m_tessOpHelper.EnableTesselation )
						{
							m_tessOpHelper.WriteToOptionalParams( ref OptionalParameters );
						}
						if ( hasRefraction )
							ShaderBody += "\t\t#pragma multi_compile _ALPHAPREMULTIPLY_ON\n";

						AddShaderPragma( ref ShaderBody, "surface surf " + m_currentLightModel.ToString() + customLightSurface + Constants.OptionalParametersSep + OptionalParameters );
						CloseCGProgram( ref ShaderBody );

						ShaderBody += "\t\tPass\n";
						ShaderBody += "\t\t{\n";
						ShaderBody += "\t\t\tName \"ShadowCaster\"\n";
						ShaderBody += "\t\t\tTags{ \"LightMode\" = \"ShadowCaster\" }\n";
						ShaderBody += "\t\t\tZWrite On\n";
						ShaderBody += "\t\t\tCGPROGRAM\n";
						ShaderBody += "\t\t\t#pragma vertex vert\n";
						ShaderBody += "\t\t\t#pragma fragment frag\n";
						ShaderBody += "\t\t\t#pragma target 3.0\n";
						//ShaderBody += "\t\t\t#pragma multi_compile_instancing\n";
						ShaderBody += "\t\t\t#pragma multi_compile_shadowcaster\n";
						ShaderBody += "\t\t\t#pragma multi_compile UNITY_PASS_SHADOWCASTER\n";
						ShaderBody += "\t\t\t#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2\n";
						ShaderBody += "\t\t\t# include \"HLSLSupport.cginc\"\n";
						ShaderBody += "\t\t\t#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 )\n";
						ShaderBody += "\t\t\t\t#define CAN_SKIP_VPOS\n";
						ShaderBody += "\t\t\t#endif\n";
						ShaderBody += "\t\t\t#include \"UnityCG.cginc\"\n";
						ShaderBody += "\t\t\t#include \"Lighting.cginc\"\n";
						ShaderBody += "\t\t\t#include \"UnityPBSLighting.cginc\"\n";
						ShaderBody += "\t\t\tsampler3D _DitherMaskLOD;\n";

						ShaderBody += "\t\t\tstruct v2f\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tV2F_SHADOW_CASTER;\n";
						ShaderBody += "\t\t\t\tfloat3 worldPos : TEXCOORD6;\n";
						if ( UIUtils.CurrentDataCollector.UsingWorldNormal || UIUtils.CurrentDataCollector.UsingWorldPosition || UIUtils.CurrentDataCollector.UsingInternalData || UIUtils.CurrentDataCollector.DirtyNormal )
						{
							ShaderBody += "\t\t\t\tfloat4 tSpace0 : TEXCOORD1;\n";
							ShaderBody += "\t\t\t\tfloat4 tSpace1 : TEXCOORD2;\n";
							ShaderBody += "\t\t\t\tfloat4 tSpace2 : TEXCOORD3;\n";
						}
						if ( UIUtils.CurrentDataCollector.UsingTexcoord0 || UIUtils.CurrentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\tfloat4 texcoords01 : TEXCOORD4;\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord2 || UIUtils.CurrentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\tfloat4 texcoords23 : TEXCOORD5;\n";
#if UNITY_5_5_OR_NEWER
						ShaderBody += "\t\t\t\tUNITY_VERTEX_INPUT_INSTANCE_ID\n";
#else
						ShaderBody += "\t\t\t\tUNITY_INSTANCE_ID\n";
#endif
						ShaderBody += "\t\t\t};\n";

						ShaderBody += "\t\t\tv2f vert( appdata_full v )\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tv2f o;\n";

						ShaderBody += "\t\t\t\tUNITY_SETUP_INSTANCE_ID( v );\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( v2f, o );\n";
						ShaderBody += "\t\t\t\tUNITY_TRANSFER_INSTANCE_ID( v, o );\n";

						if ( hasVertexOffset )
						{
							ShaderBody += "\t\t\t\tInput customInputData;\n";

							ShaderBody += "\t\t\t\tvertexDataFunc( v" + ( UIUtils.CurrentDataCollector.TesselationActive ? "" : ", customInputData" ) + " );\n";
						}

						ShaderBody += "\t\t\t\tfloat3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;\n";
						ShaderBody += "\t\t\t\thalf3 worldNormal = UnityObjectToWorldNormal( v.normal );\n";
						if ( UIUtils.CurrentDataCollector.UsingInternalData || UIUtils.CurrentDataCollector.DirtyNormal )
						{
							ShaderBody += "\t\t\t\tfixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );\n";
							ShaderBody += "\t\t\t\tfixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;\n";
							ShaderBody += "\t\t\t\tfixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;\n";
							ShaderBody += "\t\t\t\to.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );\n";
							ShaderBody += "\t\t\t\to.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );\n";
							ShaderBody += "\t\t\t\to.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );\n";
						}
						if ( UIUtils.CurrentDataCollector.UsingTexcoord0 || UIUtils.CurrentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\to.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord2 || UIUtils.CurrentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\to.texcoords23 = float4( v.texcoord2.xy, v.texcoord3.xy );\n";

						//if ( UIUtils.CurrentDataCollector.UsingWorldPosition && !UIUtils.CurrentDataCollector.UsingInternalData )
						ShaderBody += "\t\t\t\to.worldPos = worldPos;\n";
						ShaderBody += "\t\t\t\tTRANSFER_SHADOW_CASTER_NORMALOFFSET( o )\n";

						ShaderBody += "\t\t\t\treturn o;\n";
						ShaderBody += "\t\t\t}\n";

						ShaderBody += "\t\t\tfixed4 frag( v2f IN\n";
						ShaderBody += "\t\t\t#if !defined( CAN_SKIP_VPOS )\n";
						ShaderBody += "\t\t\t, UNITY_VPOS_TYPE vpos : VPOS\n";
						ShaderBody += "\t\t\t#endif\n";
						ShaderBody += "\t\t\t) : SV_Target\n";
						ShaderBody += "\t\t\t{\n";
						ShaderBody += "\t\t\t\tUNITY_SETUP_INSTANCE_ID( IN );\n";
						ShaderBody += "\t\t\t\tInput surfIN;\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( Input, surfIN );\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord0 )
							ShaderBody += "\t\t\t\tsurfIN.uv_texcoord = IN.texcoords01.xy;\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord1 )
							ShaderBody += "\t\t\t\tsurfIN.uv2_texcoord2 = IN.texcoords01.zw;\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord2 )
							ShaderBody += "\t\t\t\tsurfIN.uv3_texcoord3 = IN.texcoords23.xy;\n";
						if ( UIUtils.CurrentDataCollector.UsingTexcoord3 )
							ShaderBody += "\t\t\t\tsurfIN.uv4_texcoord4 = IN.texcoords23.zw;\n";

						if ( UIUtils.CurrentDataCollector.UsingInternalData )
							ShaderBody += "\t\t\t\tfloat3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );\n";
						else
							ShaderBody += "\t\t\t\tfloat3 worldPos = IN.worldPos;\n";
						ShaderBody += "\t\t\t\tfixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );\n";

						//fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
						//fixed3 viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;

						if ( UIUtils.CurrentDataCollector.UsingViewDirection && !UIUtils.CurrentDataCollector.DirtyNormal )
							ShaderBody += "\t\t\t\tsurfIN.viewDir = worldViewDir;\n";
						else if ( UIUtils.CurrentDataCollector.UsingViewDirection )
							ShaderBody += "\t\t\t\tsurfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;\n";

						if ( UIUtils.CurrentDataCollector.UsingWorldPosition )
							ShaderBody += "\t\t\t\tsurfIN.worldPos = worldPos;\n";

						if ( UIUtils.CurrentDataCollector.UsingWorldNormal )
							ShaderBody += "\t\t\t\tsurfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );\n";

						if ( UIUtils.CurrentDataCollector.UsingWorldReflection )
							ShaderBody += "\t\t\t\tsurfIN.worldRefl = -worldViewDir;\n";

						if ( UIUtils.CurrentDataCollector.UsingInternalData )
						{
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;\n";
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;\n";
							ShaderBody += "\t\t\t\tsurfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;\n";
						}

						ShaderBody += "\t\t\t\t" + outputStruct + " o;\n";
						ShaderBody += "\t\t\t\tUNITY_INITIALIZE_OUTPUT( " + outputStruct + ", o )\n";
						ShaderBody += "\t\t\t\tsurf( surfIN, o );\n";
						ShaderBody += "\t\t\t\t#if defined( CAN_SKIP_VPOS )\n";
						ShaderBody += "\t\t\t\tfloat2 vpos = IN.pos;\n";
						ShaderBody += "\t\t\t\t#endif\n";

						if ( hasOpacity )
						//if ( m_alphaMode == AlphaMode.Fade || m_alphaMode == AlphaMode.Transparent )
						{
							ShaderBody += "\t\t\t\thalf alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;\n";
							ShaderBody += "\t\t\t\tclip( alphaRef - 0.01 );\n";
						}

						ShaderBody += "\t\t\t\tSHADOW_CASTER_FRAGMENT( IN )\n";
						ShaderBody += "\t\t\t}\n";

						ShaderBody += "\t\t\tENDCG\n";

						ShaderBody += "\t\t}\n";
					}



				}
				CloseSubShaderBody( ref ShaderBody );

				if ( m_castShadows )
					AddShaderProperty( ref ShaderBody, "Fallback", "Diffuse" );

				if ( !string.IsNullOrEmpty( m_customInspectorName ) )
				{
					AddShaderProperty( ref ShaderBody, "CustomEditor", m_customInspectorName );
				}
			}
			CloseShaderBody( ref ShaderBody );

			// Generate Graph info
			ShaderBody += UIUtils.CurrentWindow.GenerateGraphInfo();

			//TODO: Remove current SaveDebugShader and uncomment SaveToDisk as soon as pathname is editable
			if ( !String.IsNullOrEmpty( pathname ) )
			{
				IOUtils.StartSaveThread( ShaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
			}
			else
			{
				IOUtils.StartSaveThread( ShaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
			}

			// Load new shader into material

			if ( CurrentShader == null )
			{
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				CurrentShader = Shader.Find( ShaderName );
			}
			else
			{
				// need to always get asset datapath because a user can change and asset location from the project window 
				AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentShader ) );
				//ShaderUtil.UpdateShaderAsset( m_currentShader, ShaderBody );
			}

			if ( m_currentShader != null )
			{
				if ( m_currentMaterial != null )
				{
					m_currentMaterial.shader = m_currentShader;
					UIUtils.CurrentDataCollector.UpdateMaterialOnPropertyNodes( m_currentMaterial );
					UpdateMaterialEditor();
					// need to always get asset datapath because a user can change and asset location from the project window
					AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentMaterial ) );
				}

				UIUtils.CurrentDataCollector.UpdateShaderOnPropertyNodes( ref m_currentShader );
			}

			UIUtils.CurrentDataCollector.Destroy();
			UIUtils.CurrentDataCollector = null;

			return m_currentShader;
		}

		public override void UpdateFromShader( Shader newShader )
		{
			if ( m_currentMaterial != null )
			{
				m_currentMaterial.shader = newShader;
			}
			CurrentShader = newShader;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_renderingOptionsOpHelper.Destroy();
			m_renderingOptionsOpHelper = null;
			m_renderingPlatformOpHelper = null;
			m_inspectorDefaultStyle = null;
			m_inspectorFoldoutStyle = null;
			m_propertyAdjustment = null;
			m_zBufferHelper = null;
			m_stencilBufferHelper = null;
			m_blendOpsHelper = null;
			m_tessOpHelper.Destroy();
			m_tessOpHelper = null;
			m_outlineHelper = null;
			m_colorMaskHelper.Destroy();
			m_colorMaskHelper = null;
			m_billboardOpHelper = null;
		}

		public override int VersionConvertInputPortId( int portId )
		{
			int newPort = portId;

			//added translucency input after occlusion
			if ( UIUtils.CurrentShaderVersion() <= 2404 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort += 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort += 1;
					break;
				}
			}

			//added transmission input after translucency
			if ( UIUtils.CurrentShaderVersion() < 2407 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 6 )
						newPort += 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 5 )
						newPort += 1;
					break;
				}
			}

			//added tessellation ports
			if ( UIUtils.CurrentShaderVersion() < 3002 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 13 )
						newPort += 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 10 )
						newPort += 1;
					break;
				}
			}

			//added refraction after translucency
			if ( UIUtils.CurrentShaderVersion() < 3204 )
			{
				switch ( m_currentLightModel )
				{
					case StandardShaderLightModel.Standard:
					case StandardShaderLightModel.StandardSpecular:
					if ( portId >= 8 )
						newPort += 1;
					break;
					case StandardShaderLightModel.Lambert:
					case StandardShaderLightModel.BlinnPhong:
					if ( portId >= 7 )
						newPort += 1;
					break;
				}
			}

			return newPort;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			try
			{
				base.ReadFromString( ref nodeParams );
				m_currentLightModel = ( StandardShaderLightModel ) Enum.Parse( typeof( StandardShaderLightModel ), GetCurrentParam( ref nodeParams ) );
				//if ( _shaderCategory.Length > 0 )
				//	_shaderCategory = UIUtils.RemoveInvalidCharacters( _shaderCategory );
				ShaderName = GetCurrentParam( ref nodeParams );
				if ( m_shaderName.Length > 0 )
					ShaderName = UIUtils.RemoveShaderInvalidCharacters( ShaderName );

				m_renderingOptionsOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				
				m_cullMode = ( CullMode ) Enum.Parse( typeof( CullMode ), GetCurrentParam( ref nodeParams ) );
				m_zBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );

				string alphaMode = GetCurrentParam( ref nodeParams );

				if ( UIUtils.CurrentShaderVersion() < 4003 )
				{
					if ( alphaMode.Equals( "Fade" ) )
					{
						alphaMode = "Transparent";
					}
					else if ( alphaMode.Equals( "Transparent" ) )
					{
						alphaMode = "Premultiply";
					}
				}

				m_alphaMode = ( AlphaMode ) Enum.Parse( typeof( AlphaMode ), alphaMode );
				m_opacityMaskClipValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = true;
				m_castShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_queueOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 11 )
				{
					m_customBlendMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_renderType = ( RenderType ) Enum.Parse( typeof( RenderType ), GetCurrentParam( ref nodeParams ) );
					m_renderQueue = ( RenderQueue ) Enum.Parse( typeof( RenderQueue ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2402 )
				{
					m_renderPath = ( RenderPath ) Enum.Parse( typeof( RenderPath ), GetCurrentParam( ref nodeParams ) );
				}
				if ( UIUtils.CurrentShaderVersion() > 2405 )
				{
					m_renderingPlatformOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2500 )
				{
					m_colorMaskHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2501 )
				{
					m_stencilBufferHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2504 )
				{
					m_tessOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 2505 )
				{
					m_receiveShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 3202 )
				{
					m_blendOpsHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 3203 )
				{
					m_grabOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}

				if ( UIUtils.CurrentShaderVersion() > 5003 )
				{
					m_outlineHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				if ( UIUtils.CurrentShaderVersion() > 5110 )
				{
					m_billboardOpHelper.ReadFromString( ref m_currentReadParamIdx, ref nodeParams );
				}

				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				UpdateFromBlendMode();
				m_customBlendMode = TestCustomBlendMode();

				UIUtils.CurrentWindow.CurrentGraph.CurrentPrecision = m_currentPrecisionType;
			}
			catch ( Exception e )
			{
				Debug.Log( e );
			}
		}
		
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentLightModel );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderName );
			m_renderingOptionsOpHelper.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
			m_zBufferHelper.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_alphaMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_opacityMaskClipValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keepAlpha );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_castShadows );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_queueOrder );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customBlendMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderQueue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderPath );
			m_renderingPlatformOpHelper.WriteToString( ref nodeInfo );
			m_colorMaskHelper.WriteToString( ref nodeInfo );
			m_stencilBufferHelper.WriteToString( ref nodeInfo );
			m_tessOpHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_receiveShadows );
			m_blendOpsHelper.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_grabOrder );
			m_outlineHelper.WriteToString( ref nodeInfo );
			m_billboardOpHelper.WriteToString( ref nodeInfo );
		}

		private bool TestCustomBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
						return false;
				}
				break;
				case AlphaMode.Masked:
				{
					if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
						return false;
				}
				break;
				case AlphaMode.Transparent:
				case AlphaMode.Premultiply:
				{
					if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
				case AlphaMode.Translucent:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
			}
			return true;
		}

		private void UpdateFromBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Geometry;
					m_keepAlpha = true;
					m_refractionPort.Locked = true;
					m_inputPorts[ m_opacityPortId ].Locked = true;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Masked:
				{
					m_renderType = RenderType.TransparentCutout;
					m_renderQueue = RenderQueue.AlphaTest;
					m_keepAlpha = true;
					m_refractionPort.Locked = true;
					m_inputPorts[ m_opacityPortId ].Locked = true;
					m_inputPorts[ m_discardPortId ].Locked = false;
				}
				break;
				case AlphaMode.Transparent:
				case AlphaMode.Premultiply:
				{
					m_renderType = RenderType.Transparent;
					m_renderQueue = RenderQueue.Transparent;
					m_refractionPort.Locked = false;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Translucent:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Transparent;
					m_refractionPort.Locked = false;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = true;
				}
				break;
				case AlphaMode.Custom:
				{
					m_refractionPort.Locked = false;
					m_inputPorts[ m_opacityPortId ].Locked = false;
					m_inputPorts[ m_discardPortId ].Locked = false;
				}
				break;
			}
		}

		public StandardShaderLightModel CurrentLightingModel { get { return m_currentLightModel; } }
	}
}
