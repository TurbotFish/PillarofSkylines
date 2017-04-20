// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using AmplifyShaderEditor;
using System;
using System.Collections.Generic;




public class AmplifyShaderEditorWindow : EditorWindow, ISerializationCallbackReceiver
{
	public const string CopyCommand = "Copy";
	public const string PasteCommand = "Paste";
	public const string SelectAll = "SelectAll";
	public const string Duplicate = "Duplicate";
	public const string LiveShaderError = "Live Shader only works with an assigned Master Node on the graph";

	public Texture2D MasterNodeOnTexture = null;
	public Texture2D MasterNodeOffTexture = null;

	public Texture2D GPUInstancedOnTexture = null;
	public Texture2D GPUInstancedOffTexture = null;

	private bool m_initialized = false;
	private bool m_checkInvalidConnections = false;

	// UI 
	private Rect m_graphArea;
	private Texture2D m_graphBgTexture;
	private Texture2D m_graphFgTexture;
	private GUIStyle m_graphFontStyle;
	//private GUIStyle _borderStyle;
	private Texture2D m_wireTexture;

	[SerializeField]
	private ASESelectionMode m_selectionMode = ASESelectionMode.Shader;

	[SerializeField]
	private DuplicatePreventionBuffer m_duplicatePreventionBuffer;

	// Prevent save ops every tick when on live mode
	[SerializeField]
	private double m_lastTimeSaved = 0;

	[SerializeField]
	private bool m_cacheSaveOp = false;
	private const double SaveTime = 1;

	private bool m_markedToSave = false;

	// Graph logic
	[SerializeField]
	private ParentGraph m_mainGraphInstance;

	// Camera control
	[SerializeField]
	private Vector2 m_cameraOffset;

	private Rect m_cameraInfo;

	[SerializeField]
	private float m_cameraZoom;

	[SerializeField]
	private Vector2 m_minNodePos;

	[SerializeField]
	private Vector2 m_maxNodePos;

	[SerializeField]
	private bool m_isDirty;

	[SerializeField]
	private bool m_saveIsDirty;

	[SerializeField]
	private bool m_repaintIsDirty;

	[SerializeField]
	private bool m_liveShaderEditing = false;

	[SerializeField]
	private bool m_shaderIsModified = true;

	[SerializeField]
	private string m_lastOpenedLocation = string.Empty;

	[SerializeField]
	private bool m_zoomChanged = true;

	[SerializeField]
	private float m_lastWindowWidth = 0;

	private bool m_ctrlSCallback = false;

	// Events
	private Vector3 m_currentMousePos;
	private Vector2 m_keyEvtMousePos2D;
	private Vector2 m_currentMousePos2D;
	private Event m_currentEvent;
	private bool m_insideEditorWindow;

	private bool m_lostFocus = false;
	// Selection box for multiple node selection 
	private bool m_multipleSelectionActive = false;
	private bool m_lmbPressed = false;
	private Vector2 m_multipleSelectionStart;
	private Rect m_multipleSelectionArea = new Rect( 0, 0, 0, 0 );
	private bool m_autoPanDirActive = false;
	private bool m_forceAutoPanDir = false;
	private bool m_loadShaderOnSelection = false;

	private double m_time;

	//Context Menu
	private Vector2 m_rmbStartPos;
	private Vector2 m_altKeyStartPos;
	private GraphContextMenu m_contextMenu;
	private ShortcutsManager m_shortcutManager;

	//Clipboard
	private Clipboard m_clipboard;

	//Node Parameters Window
	[SerializeField]
	private bool m_nodeParametersWindowMaximized = true;
	private NodeParametersWindow m_nodeParametersWindow;

	// Tools Window
	private ToolsWindow m_toolsWindow;

	//Editor Options
	private OptionsWindow m_optionsWindow;

	// Mode Window
	private ShaderEditorModeWindow m_modeWindow;

	//Palette Window
	[SerializeField]
	private bool m_paletteWindowMaximized = true;
	private PaletteWindow m_paletteWindow;

	private ContextPalette m_contextPalette;
	private PalettePopUp m_palettePopup;
	private Type m_paletteChosenType;

	// In-Editor Message System
	GenericMessageUI m_genericMessageUI;
	private GUIContent m_genericMessageContent;

	// Drag&Drop Tool 
	private DragAndDropTool m_dragAndDropTool;

	//Custom Styles
	private CustomStylesContainer m_customStyles;

	//private ConfirmationWindow _confirmationWindow;

	private List<MenuParent> m_registeredMenus;

	private PreMadeShaders m_preMadeShaders;

	private AutoPanData[] m_autoPanArea;

	private DrawInfo m_drawInfo;
	private KeyCode m_lastKeyPressed = KeyCode.None;
	private Type m_commentaryTypeNode;

	private int m_onLoadDone = 0;

	private float m_copyPasteDeltaMul = 0;
	private Vector2 m_copyPasteInitialPos = Vector2.zero;
	private Vector2 m_copyPasteDeltaPos = Vector2.zero;

	private int m_repaintCount = 0;
	private bool m_forceUpdateFromMaterialFlag = false;

	private VersionInfo m_versionInfo;

	private UnityEngine.Object m_delayedLoadObject = null;
	private double m_focusOnSelectionTimestamp;
	private double m_focusOnMasterNodeTimestamp;
	private double m_toggleDebugModeTimestamp;
	private double m_wiredDoubleTapTimestamp;
	private bool m_toggleDebug = false;
	private bool m_globalPreview = false;

	private bool m_expandedStencil = false;
	private bool m_expandedTesselation = false;
	private bool m_expandedDepth = false;
	private bool m_expandedRenderingPlatforms = false;
	private bool m_expandedRenderingOptions = false;
	private bool m_expandedProperties = false;
	private const double m_autoZoomTime = 0.25;
	private const double m_toggleTime = 0.25;
	private const double m_wiredDoubleTapTime = 0.25;
	private const double m_doubleClickTime = 0.25;

	private Material m_delayedMaterialSet = null;

	private bool m_mouseDownOnValidArea = false;

	private bool m_removedKeyboardFocus = false;

	private int m_lastHotControl = -1;

	//private Material m_maskingMaterial = null;
	private int m_cachedProjectInLinearId = -1;
	private int m_cachedEditorTimeId = -1;
	private int m_cachedEditorDeltaTimeId = -1;
	//private float m_repaintFrequency = 15;
	//private double m_repaintTimestamp = 0;

	// Auto-Compile samples 
	private bool m_forcingMaterialUpdateFlag = false;
	private bool m_forcingMaterialUpdateOp = false;
	private List<Material> m_materialsToUpdate = new List<Material>();

	private NodeExporterUtils m_nodeExporterUtils;

	// Unity Menu item
	[MenuItem( "Window/Amplify Shader Editor/Open Canvas" )]
	static void OpenMainShaderGraph()
	{
		AmplifyShaderEditorWindow currentWindow = OpenWindow();
		currentWindow.CreateNewGraph( "Empty" );
	}

	public static void ConvertShaderToASE( Shader shader )
	{
		if ( IOUtils.IsASEShader( shader ) )
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CurrentWindow.LoadProjectSelected();
		}
		else
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CreateEmptyFromInvalid( shader );
			UIUtils.ShowMessage( "Convertion complete. Old data will be lost when saving it" );
		}
	}
	
	public static void LoadMaterialToASE( Material material )
	{
		if ( IOUtils.IsASEShader( material.shader ) )
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CurrentWindow.LoadProjectSelected( material );
		}
		else
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CreateEmptyFromInvalid( material.shader );
			UIUtils.SetDelayedMaterialMode( material );
		}
	}

	public static AmplifyShaderEditorWindow OpenWindow()
	{
		AmplifyShaderEditorWindow currentWindow = ( AmplifyShaderEditorWindow ) AmplifyShaderEditorWindow.GetWindow( typeof( AmplifyShaderEditorWindow ) );
		currentWindow.minSize = new Vector2( ( Constants.MINIMIZE_WINDOW_LOCK_SIZE - 150 ), 350 );
		currentWindow.wantsMouseMove = true;
		return currentWindow;
	}
	
	// Shader Graph window
	public void OnEnable()
	{
		IOUtils.Init();

		EditorApplication.update -= UpdateTime;
		EditorApplication.update -= UpdateNodePreviewList;

		EditorApplication.update += UpdateTime;
		EditorApplication.update += UpdateNodePreviewList;

		m_optionsWindow = new OptionsWindow();
		m_optionsWindow.Init();

		m_contextMenu = new GraphContextMenu( m_mainGraphInstance );

		m_paletteWindow = new PaletteWindow( m_contextMenu.MenuItems );
		m_paletteWindow.Resizable = true;
		m_paletteWindow.OnPaletteNodeCreateEvt += OnPaletteNodeCreate;
		m_registeredMenus.Add( m_paletteWindow );

		m_contextPalette = new ContextPalette( m_contextMenu.MenuItems );
		m_contextPalette.OnPaletteNodeCreateEvt += OnContextPaletteNodeCreate;
		m_registeredMenus.Add( m_contextPalette );

		m_genericMessageUI = new GenericMessageUI();
		m_genericMessageUI.OnMessageDisplayEvent += ShowMessageImmediately;

		Selection.selectionChanged += OnProjectSelectionChanged;
		EditorApplication.projectWindowChanged += OnProjectWindowChanged;

		m_focusOnSelectionTimestamp = EditorApplication.timeSinceStartup;
		m_focusOnMasterNodeTimestamp = EditorApplication.timeSinceStartup;
		m_toggleDebugModeTimestamp = EditorApplication.timeSinceStartup;

		m_nodeParametersWindow.IsMaximized = EditorVariablesManager.NodeParametersMaximized.Value;
		if ( DebugConsoleWindow.UseShaderPanelsInfo )
			m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;

		m_paletteWindow.IsMaximized = EditorVariablesManager.NodePaletteMaximized.Value;
		if ( DebugConsoleWindow.UseShaderPanelsInfo )
			m_paletteWindow.IsMaximized = m_paletteWindowMaximized;

		m_shortcutManager = new ShortcutsManager();
		// REGISTER NODE SHORTCUTS
		foreach ( KeyValuePair<KeyCode, ShortcutKeyData> kvp in m_contextMenu.NodeShortcuts )
		{
			m_shortcutManager.RegisterNodesShortcuts( kvp.Key, kvp.Value.Name );
		}

		// REGISTER EDITOR SHORTCUTS
		m_shortcutManager.RegisterEditorShortcut( KeyCode.C, "Create Commentary", () =>
		{
			// Create commentary
			CommentaryNode node = m_mainGraphInstance.CreateNode( m_commentaryTypeNode, true, -1, false ) as CommentaryNode;
			node.CreateFromSelectedNodes( TranformedMousePos, m_mainGraphInstance.SelectedNodes );
			node.Focus();
			m_mainGraphInstance.DeSelectAll();
			m_mainGraphInstance.SelectNode( node, false, false );
			ForceRepaint();
		} );


		m_shortcutManager.RegisterEditorShortcut( KeyCode.F, "Focus On Selection", () =>
		{
			OnToolButtonPressed( ToolButtonType.FocusOnSelection );
			ForceRepaint();
		} );

		m_shortcutManager.RegisterEditorShortcut( KeyCode.Space, "Open Node Palette", null, () =>
		  {
			  m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
		  } );


		m_shortcutManager.RegisterEditorShortcut( KeyCode.W, "Toggle Debug Mode", () =>
	   {
		   double currTime = EditorApplication.timeSinceStartup;
		   bool toggle = ( currTime - m_toggleDebugModeTimestamp ) < m_toggleTime;
		   m_toggleDebugModeTimestamp = currTime;

		   if ( toggle )
		   {
			   m_toggleDebug = !m_toggleDebug;
			   m_optionsWindow.ColoredPorts = !m_optionsWindow.ColoredPorts;
		   }
		   else
		   {
			   m_optionsWindow.ColoredPorts = true;
		   }

		   ForceRepaint();
	   } );

		m_shortcutManager.RegisterEditorShortcut( KeyCode.P, "Global Preview", () =>
		{
			GlobalPreview = !GlobalPreview;
			EditorPrefs.SetBool( "GlobalPreview", GlobalPreview );

			ForceRepaint();
		} );

		m_shortcutManager.RegisterEditorShortcut( KeyCode.Delete, "Delete selected nodes", () =>
		{
			DeleteSelectedNodes();
			ForceRepaint();
		} );

		m_shortcutManager.RegisterEditorShortcut( KeyCode.Backspace, "Delete selected nodes", () =>
		{
			DeleteSelectedNodes();
			ForceRepaint();
		} );


		m_liveShaderEditing = EditorVariablesManager.LiveMode.Value;

		UpdateLiveUI();
	}

	public AmplifyShaderEditorWindow()
	{
		m_versionInfo = new VersionInfo();
		m_minNodePos = new Vector2( float.MaxValue, float.MaxValue );
		m_maxNodePos = new Vector2( float.MinValue, float.MinValue );

		m_duplicatePreventionBuffer = new DuplicatePreventionBuffer();
		m_commentaryTypeNode = typeof( CommentaryNode );
		titleContent = new GUIContent( "Shader Editor" );
		autoRepaintOnSceneChange = true;
		m_mainGraphInstance = new ParentGraph();
		m_mainGraphInstance.OnNodeEvent += OnNodeStoppedMovingEvent;
		m_mainGraphInstance.OnMaterialUpdatedEvent += OnMaterialUpdated;
		m_mainGraphInstance.OnShaderUpdatedEvent += OnShaderUpdated;
		m_mainGraphInstance.OnEmptyGraphDetectedEvt += OnEmptyGraphDetected;

		m_currentMousePos = new Vector3( 0, 0, 0 );
		m_keyEvtMousePos2D = new Vector2( 0, 0 );
		m_multipleSelectionStart = new Vector2( 0, 0 );
		m_initialized = false;
		m_graphBgTexture = null;
		m_graphFgTexture = null;

		m_cameraOffset = new Vector2( 0, 0 );
		CameraZoom = 1;

		m_registeredMenus = new List<MenuParent>();

		m_nodeParametersWindow = new NodeParametersWindow();
		m_nodeParametersWindow.Resizable = true;
		m_registeredMenus.Add( m_nodeParametersWindow );

		m_modeWindow = new ShaderEditorModeWindow();
		//_registeredMenus.Add( _modeWindow );

		m_toolsWindow = new ToolsWindow();
		m_toolsWindow.ToolButtonPressedEvt += OnToolButtonPressed;
		m_registeredMenus.Add( m_toolsWindow );

		m_palettePopup = new PalettePopUp();

		m_clipboard = new Clipboard();

		m_genericMessageContent = new GUIContent();
		m_dragAndDropTool = new DragAndDropTool();
		m_dragAndDropTool.OnValidDropObjectEvt += OnValidObjectsDropped;

		//_confirmationWindow = new ConfirmationWindow( 100, 100, 300, 100 );

		m_customStyles = new CustomStylesContainer();
		m_saveIsDirty = false;

		m_preMadeShaders = new PreMadeShaders();

		Undo.undoRedoPerformed += UndoRedoPerformed;

		float autoPanSpeed = 2;
		m_autoPanArea = new AutoPanData[ 4 ];
		m_autoPanArea[ 0 ] = new AutoPanData( AutoPanLocation.TOP, 25, autoPanSpeed * Vector2.up );
		m_autoPanArea[ 1 ] = new AutoPanData( AutoPanLocation.BOTTOM, 25, autoPanSpeed * Vector2.down );
		m_autoPanArea[ 2 ] = new AutoPanData( AutoPanLocation.LEFT, 25, autoPanSpeed * Vector2.right );
		m_autoPanArea[ 3 ] = new AutoPanData( AutoPanLocation.RIGHT, 25, autoPanSpeed * Vector2.left );

		m_drawInfo = new DrawInfo();
		UIUtils.CurrentWindow = this;

		m_nodeExporterUtils = new NodeExporterUtils( this );
		m_repaintIsDirty = false;
		m_initialized = false;
	}

	void UndoRedoPerformed()
	{
		Debug.Log( "Undo performed" );
		m_repaintIsDirty = true;
		m_saveIsDirty = true;
		m_removedKeyboardFocus = true;
	}


	void Destroy()
	{
		
		m_initialized = false;

		m_nodeExporterUtils.Destroy();
		m_nodeExporterUtils = null;

		m_delayedMaterialSet = null;

		m_materialsToUpdate.Clear();
		m_materialsToUpdate = null;

		IOUtils.Destroy();

		GLDraw.Destroy();

		UIUtils.Destroy();
		m_preMadeShaders.Destroy();
		m_preMadeShaders = null;

		m_customStyles = null;

		m_registeredMenus.Clear();
		m_registeredMenus = null;

		m_mainGraphInstance.Destroy();
		m_mainGraphInstance = null;

		Resources.UnloadAsset( MasterNodeOnTexture );
		MasterNodeOnTexture = null;

		Resources.UnloadAsset( MasterNodeOffTexture );
		MasterNodeOffTexture = null;

		Resources.UnloadAsset( GPUInstancedOnTexture );
		GPUInstancedOnTexture = null;

		Resources.UnloadAsset( GPUInstancedOffTexture );
		GPUInstancedOffTexture = null;

		Resources.UnloadAsset( m_graphBgTexture );
		m_graphBgTexture = null;

		Resources.UnloadAsset( m_graphFgTexture );
		m_graphFgTexture = null;

		Resources.UnloadAsset( m_wireTexture );
		m_wireTexture = null;

		m_contextMenu.Destroy();
		m_contextMenu = null;

		m_shortcutManager.Destroy();
		m_shortcutManager = null;

		m_nodeParametersWindow.Destroy();
		m_nodeParametersWindow = null;

		m_modeWindow.Destroy();
		m_modeWindow = null;

		m_toolsWindow.Destroy();
		m_toolsWindow = null;

		m_optionsWindow.Destroy();
		m_optionsWindow = null;

		m_paletteWindow.Destroy();
		m_paletteWindow = null;

		m_palettePopup.Destroy();
		m_palettePopup = null;

		m_contextPalette.Destroy();
		m_contextPalette = null;

		m_clipboard.ClearClipboard();
		m_clipboard = null;

		m_genericMessageUI.Destroy();
		m_genericMessageUI = null;
		m_genericMessageContent = null;

		m_dragAndDropTool = null;

		//if( m_maskingMaterial != null )
		//	DestroyImmediate( m_maskingMaterial );
		//m_maskingMaterial = null;

		//_confirmationWindow.Destroy();
		//_confirmationWindow = null;
		UIUtils.CurrentWindow = null;
		m_duplicatePreventionBuffer.ReleaseAllData();
		m_duplicatePreventionBuffer = null;

		EditorApplication.projectWindowChanged -= OnProjectWindowChanged;
		Selection.selectionChanged -= OnProjectSelectionChanged;
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	void Init()
	{
		// = AssetDatabase.LoadAssetAtPath( Constants.ASEPath + "", typeof( Texture2D ) ) as Texture2D;
		m_graphBgTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GraphBgTextureGUID ), typeof( Texture2D ) ) as Texture2D;
		if ( m_graphBgTexture != null )
		{
			m_graphFgTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GraphFgTextureGUID ), typeof( Texture2D ) ) as Texture2D;

			//Setup usable area
			m_cameraInfo = position;
			m_graphArea = new Rect( 0, 0, m_cameraInfo.width, m_cameraInfo.height );

			// Creating style state to show current selected object
			m_graphFontStyle = new GUIStyle();
			m_graphFontStyle.fontSize = 32;
			m_graphFontStyle.normal.textColor = Color.white;
			m_graphFontStyle.alignment = TextAnchor.MiddleCenter;
			m_graphFontStyle.fixedWidth = m_cameraInfo.width;
			m_graphFontStyle.fixedHeight = 50;
			m_graphFontStyle.stretchWidth = true;
			m_graphFontStyle.stretchHeight = true;

			m_wireTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.WireTextureGUID ), typeof( Texture2D ) ) as Texture2D;
			MasterNodeOnTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.MasterNodeOnTextureGUID ), typeof( Texture2D ) ) as Texture2D;
			MasterNodeOffTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.MasterNodeOnTextureGUID ), typeof( Texture2D ) ) as Texture2D;

			if ( MasterNodeOffTexture == null )
			{
				MasterNodeOffTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.EditorResourcesGUID ) + "Nodes/MasterNodeIcon.png", typeof( Texture2D ) ) as Texture2D;
			}

			GPUInstancedOnTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GPUInstancedOnTextureGUID ), typeof( Texture2D ) ) as Texture2D;
			GPUInstancedOffTexture = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.GPUInstancedOffTextureGUID ), typeof( Texture2D ) ) as Texture2D;

			m_initialized = m_graphBgTexture != null &&
							m_graphFgTexture != null &&
							m_wireTexture != null &&
							MasterNodeOnTexture != null &&
							MasterNodeOffTexture != null &&
							GPUInstancedOnTexture != null &&
							GPUInstancedOffTexture != null;
		}
	}

	[OnOpenAssetAttribute()]
	static bool OnOpenAsset( int instanceID, int line )
	{
		if ( line > -1 )
		{
			return false;
		}

		Shader selectedShader = Selection.activeObject as Shader;
		if ( selectedShader != null )
		{
			if ( IOUtils.IsASEShader( selectedShader ) )
			{
				if ( UIUtils.CurrentWindow == null )
				{
					OpenWindow();
					UIUtils.CurrentWindow.DelayedObjToLoad = Selection.activeObject;
				}
				else
				{
					UIUtils.CurrentWindow.LoadProjectSelected();
				}
				return true;
			}
		}
		else
		{
			Material mat = Selection.activeObject as Material;
			if ( mat != null )
			{
				if ( IOUtils.IsASEShader( mat.shader ) )
				{
					if ( UIUtils.CurrentWindow == null )
					{
						OpenWindow();
						UIUtils.CurrentWindow.DelayedObjToLoad = Selection.activeObject;
					}
					else
					{
						UIUtils.CurrentWindow.LoadProjectSelected();
					}
					return true;
				}
			}
		}
		return false;
	}


	[MenuItem( "Assets/Create/Shader/Amplify Surface Shader" )]
	public static void CreateNewShader()
	{
		string path = Selection.activeObject == null ? Application.dataPath : ( IOUtils.dataPath + AssetDatabase.GetAssetPath( Selection.activeObject ) );
		if ( path.IndexOf( '.' ) > -1 )
		{
			path = path.Substring( 0, path.LastIndexOf( '/' ) );
		}
		path += "/";
		OpenMainShaderGraph();
		Shader shader = UIUtils.CreateNewEmpty( path );
		Selection.activeObject = shader;
		Selection.objects = new UnityEngine.Object[] { shader };
	}

	[MenuItem( "Assets/Create/Shader/Amplify Post-Process Shader" )]
	public static void CreateNewPostProcess() { }

	[MenuItem( "Assets/Create/Shader/Amplify Post-Process Shader", true )]
	public static bool ValidateCreateNewPostProcess() { return false; }

	public void OnProjectWindowChanged()
	{
		Shader selectedShader = Selection.activeObject as Shader;
		if ( selectedShader != null )
		{
			if ( m_mainGraphInstance != null && selectedShader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
			{
				m_lastOpenedLocation = AssetDatabase.GetAssetPath( selectedShader );
			}
		}
	}

	public void LoadProjectSelected( UnityEngine.Object selectedObject = null )
	{
		if ( m_mainGraphInstance != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			LoadObject( selectedObject == null ? Selection.activeObject : selectedObject );
		}
		else
		{
			m_delayedLoadObject = selectedObject == null ? Selection.activeObject : selectedObject;
		}
	}

	public void LoadObject( UnityEngine.Object objToLoad )
	{
		Shader selectedShader = objToLoad as Shader;
		Shader currentShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		if ( selectedShader != null )
		{
			if ( m_mainGraphInstance.CurrentMasterNode != null &&
				!ShaderIsModified &&
				selectedShader == currentShader &&
				m_selectionMode == ASESelectionMode.Shader )
				return;

			if ( ShaderIsModified && ( selectedShader != currentShader ) )
			{
				//_confirmationWindow.ActivateConfirmation( selectedShader, null, "Save changes on previous shader?", OnSaveShader, true );
				bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
				OnSaveShader( savePrevious, selectedShader, null );
			}
			else
			{
				LoadDroppedObject( true, selectedShader, null );
			}
		}
		else
		{
			Material selectedMaterial = objToLoad as Material;
			if ( selectedMaterial )
			{
				if ( m_selectionMode == ASESelectionMode.Material )
				{
					if ( !ShaderIsModified && selectedMaterial == m_mainGraphInstance.CurrentMasterNode.CurrentMaterial
						&& selectedMaterial.shader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
						return;

					if ( !ShaderIsModified && selectedMaterial.shader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
					{
						m_mainGraphInstance.UpdateMaterialOnMasterNode( selectedMaterial );
						return;
					}
				}

				if ( IOUtils.IsASEShader( selectedMaterial.shader ) )
				{
					if ( ShaderIsModified && ( selectedMaterial.shader != currentShader ) )
					{
						//_confirmationWindow.ActivateConfirmation( selectedMaterial.shader, selectedMaterial, "Save changes on previous shader?", OnSaveShader, true );
						bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
						OnSaveShader( savePrevious, selectedMaterial.shader, selectedMaterial );
					}
					else
					{
						LoadDroppedObject( true, selectedMaterial.shader, selectedMaterial );
					}
				}
			}
		}

		//Need to force one graph draw because it wont call OnGui propertly since its focuses somewhere else
		// Focus() doesn't fix this since it only changes keyboard focus
		m_drawInfo.InvertedZoom = 1 / m_cameraZoom;
		m_mainGraphInstance.Draw( m_drawInfo );


		ShaderIsModified = false;
		Focus();
		Repaint();
	}

	public void OnProjectSelectionChanged()
	{
		if ( m_loadShaderOnSelection )
		{
			LoadProjectSelected();
		}
	}

	ShaderLoadResult OnSaveShader( bool value, Shader shader, Material material )
	{
		if ( value )
		{
			SaveToDisk( false );
		}

		if ( shader != null || material != null )
		{
			LoadDroppedObject( true, shader, material );
		}

		return value ? ShaderLoadResult.LOADED : ShaderLoadResult.FILE_NOT_FOUND;
	}

	public void ResetCameraSettings()
	{
		m_cameraInfo = position;
		m_cameraOffset = new Vector2( m_cameraInfo.width * 0.5f, m_cameraInfo.height * 0.5f );
		CameraZoom = 1;
	}


	public void Reset()
	{
		m_toolsWindow.BorderStyle = null;
		m_selectionMode = ASESelectionMode.Shader;
		ResetCameraSettings();
		UIUtils.ResetMainSkin();
		m_duplicatePreventionBuffer.ReleaseAllData();
		if ( m_genericMessageUI != null )
			m_genericMessageUI.CleanUpMessageStack();
	}

	public Shader CreateNewGraph( string name )
	{
		Reset();
		UIUtils.DirtyMask = false;
		m_mainGraphInstance.CreateNewEmpty( name );
		m_lastOpenedLocation = string.Empty;
		UIUtils.DirtyMask = true;
		return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
	}


	public Shader CreateNewGraph( Shader shader )
	{
		Reset();
		UIUtils.DirtyMask = false;
		m_mainGraphInstance.CreateNewEmpty( shader.name );
		m_mainGraphInstance.CurrentMasterNode.CurrentShader = shader;

		m_lastOpenedLocation = string.Empty;
		UIUtils.DirtyMask = true;
		return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
	}


	public bool SaveToDisk( bool checkTimestamp )
	{
		if ( checkTimestamp )
		{
			if ( !m_cacheSaveOp )
			{
				m_lastTimeSaved = EditorApplication.timeSinceStartup;
				m_cacheSaveOp = true;
			}
			return false;
		}

		m_cacheSaveOp = false;
		ShaderIsModified = false;
		m_mainGraphInstance.LoadedShaderVersion = m_versionInfo.FullNumber;
		m_lastTimeSaved = EditorApplication.timeSinceStartup;

		if ( m_mainGraphInstance.CurrentMasterNodeId == Constants.INVALID_NODE_ID )
		{
			Shader currentShader = m_mainGraphInstance.CurrentMasterNode != null ? m_mainGraphInstance.CurrentMasterNode.CurrentShader : null;
			string newShader;
			if ( !String.IsNullOrEmpty( m_lastOpenedLocation ) )
			{
				newShader = m_lastOpenedLocation;
			}
			else if ( currentShader != null )
			{
				newShader = AssetDatabase.GetAssetPath( currentShader );
			}
			else
			{
				newShader = EditorUtility.SaveFilePanel( "Select Shader to save", Application.dataPath, "MyShader", "shader" );
			}

			if ( !String.IsNullOrEmpty( newShader ) )
			{
				ShowMessage( "No Master node assigned.\nShader file will only have node info" );
				IOUtils.StartSaveThread( GenerateGraphInfo(), newShader );
				AssetDatabase.Refresh();
				LoadFromDisk( newShader );
				return true;
			}
		}
		else
		{
			Shader currShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
			if ( currShader != null )
			{
				m_mainGraphInstance.FireMasterNode( currShader );
				Material material = m_mainGraphInstance.CurrentMaterial;
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, ( material != null ) ? AssetDatabase.GetAssetPath( material ) : AssetDatabase.GetAssetPath( currShader ) );
				return true;
			}
			else
			{
				string shaderName;
				string pathName;
				IOUtils.GetShaderName( out shaderName, out pathName, "MyNewShader", UIUtils.LatestOpenedFolder );
				if ( !String.IsNullOrEmpty( pathName ) )
				{
					UIUtils.CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
					m_mainGraphInstance.FireMasterNode( pathName, true );
					EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, pathName );
					return true;
				}
			}
		}
		return false;
	}

	public void OnToolButtonPressed( ToolButtonType type )
	{
		switch ( type )
		{
			case ToolButtonType.New:
			{
				UIUtils.CreateNewEmpty();
			}
			break;
			case ToolButtonType.Open:
			{
				UIUtils.OpenFile();
			}
			break;
			case ToolButtonType.Save:
			{
				SaveToDisk( false );
			}
			break;
			case ToolButtonType.Library:
			{
				ShowShaderLibrary();
			}
			break;
			case ToolButtonType.Options: { } break;
			case ToolButtonType.Update:
			{
				SaveToDisk( false );
			}
			break;
			case ToolButtonType.Live:
			{
				m_liveShaderEditing = !m_liveShaderEditing;
				EditorVariablesManager.LiveMode.Value = m_liveShaderEditing;
				// 0 off
				// 1 on
				// 2 pending
				if ( m_liveShaderEditing && m_mainGraphInstance.CurrentMasterNode.CurrentShader == null )
				{
					m_liveShaderEditing = false;
				}

				UpdateLiveUI();

				if ( m_liveShaderEditing )
				{
					SaveToDisk( false );
				}
			}
			break;
			case ToolButtonType.OpenSourceCode:
			{
				AssetDatabase.OpenAsset( m_mainGraphInstance.CurrentMasterNode.CurrentShader, 1 );
			}
			break;
			case ToolButtonType.MasterNode:
			{
				m_mainGraphInstance.AssignMasterNode();
			}
			break;

			case ToolButtonType.FocusOnMasterNode:
			{
				double currTime = EditorApplication.timeSinceStartup;
				bool autoZoom = ( currTime - m_focusOnMasterNodeTimestamp ) < m_autoZoomTime;
				m_focusOnMasterNodeTimestamp = currTime;
				FocusOnNode( m_mainGraphInstance.CurrentMasterNode, autoZoom ? 1 : m_cameraZoom, true );
			}
			break;

			case ToolButtonType.FocusOnSelection:
			{

				List<ParentNode> selectedNodes = ( m_mainGraphInstance.SelectedNodes.Count > 0 ) ? m_mainGraphInstance.SelectedNodes : m_mainGraphInstance.AllNodes;

				Vector2 minPos = new Vector2( float.MaxValue, float.MaxValue );
				Vector2 maxPos = new Vector2( float.MinValue, float.MinValue );
				Vector2 centroid = Vector2.zero;

				for ( int i = 0; i < selectedNodes.Count; i++ )
				{
					Rect currPos = selectedNodes[ i ].Position;

					minPos.x = ( currPos.x < minPos.x ) ? currPos.x : minPos.x;
					minPos.y = ( currPos.y < minPos.y ) ? currPos.y : minPos.y;

					maxPos.x = ( ( currPos.x + currPos.width ) > maxPos.x ) ? ( currPos.x + currPos.width ) : maxPos.x;
					maxPos.y = ( ( currPos.y + currPos.height ) > maxPos.y ) ? ( currPos.y + currPos.height ) : maxPos.y;

				}
				centroid = ( maxPos - minPos );


				double currTime = EditorApplication.timeSinceStartup;
				bool autoZoom = ( currTime - m_focusOnSelectionTimestamp ) < m_autoZoomTime;
				m_focusOnSelectionTimestamp = currTime;

				float zoom = m_cameraZoom;
				if ( autoZoom )
				{
					zoom = 1f;
					float canvasWidth = AvailableCanvasWidth;
					float canvasHeight = AvailableCanvasHeight;
					if ( centroid.x > canvasWidth ||
						centroid.y > canvasHeight )
					{
						float hZoom = float.MinValue;
						float vZoom = float.MinValue;
						if ( centroid.x > canvasWidth )
						{
							hZoom = ( centroid.x ) / canvasWidth;
						}

						if ( centroid.y > canvasHeight )
						{
							vZoom = ( centroid.y ) / canvasHeight;
						}
						zoom = ( hZoom > vZoom ) ? hZoom : vZoom;
					}
				}

				FocusOnPoint( minPos + centroid * 0.5f, zoom );
			}
			break;
			case ToolButtonType.ShowInfoWindow:
			{
				PortLegendInfo.OpenWindow();
			}
			break;
			case ToolButtonType.CleanUnusedNodes:
			{
				m_mainGraphInstance.CleanUnusedNodes();
			}
			break;
			case ToolButtonType.Help:
			{
				Application.OpenURL( Constants.HelpURL );
			}
			break;
		}
	}

	void UpdateLiveUI()
	{
		if ( m_toolsWindow != null )
		{
			m_toolsWindow.SetStateOnButton( ToolButtonType.Live, ( m_liveShaderEditing ) ? 1 : 0 );
		}
	}

	public void FocusOnNode( ParentNode node, float zoom, bool selectNode )
	{
		if ( selectNode )
		{
			m_mainGraphInstance.SelectNode( node, false, false );
		}
		FocusOnPoint( node.CenterPosition, zoom );
	}

	public void FocusOnPoint( Vector2 point, float zoom )
	{
		CameraZoom = zoom;
		m_cameraOffset = -point + new Vector2( ( m_cameraInfo.width + m_nodeParametersWindow.RealWidth - m_paletteWindow.RealWidth ) * 0.5f, m_cameraInfo.height * 0.5f ) * zoom;
	}

	void PreTestLeftMouseDown()
	{
		if ( m_currentEvent.type == EventType.mouseDown && m_currentEvent.button == ButtonClickId.LeftMouseButton )
		{
			ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
			if ( node != null )
			{
				m_mainGraphInstance.NodeClicked = node.UniqueId;
				return;
			}
		}

		m_mainGraphInstance.NodeClicked = -1;
	}
	void OnLeftMouseDown()
	{
		Focus();
		m_mouseDownOnValidArea = true;
		m_lmbPressed = true;
		UIUtils.ShowContextOnPick = true;
		ParentNode node = ( m_mainGraphInstance.NodeClicked < 0 ) ? m_mainGraphInstance.CheckNodeAt( m_currentMousePos ) : m_mainGraphInstance.GetClickedNode();
		if ( node != null )
		{
			m_mainGraphInstance.NodeClicked = node.UniqueId;

			if ( m_contextMenu.CheckShortcutKey() )
			{
				if ( node.ConnStatus == NodeConnectionStatus.Island )
				{
					if ( !m_multipleSelectionActive )
					{
						ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
						if ( newNode != null )
						{
							newNode.Vec2Position = TranformedMousePos;
							m_mainGraphInstance.AddNode( newNode, true );
							ForceRepaint();
						}
						( node as CommentaryNode ).AddNodeToCommentary( newNode );
					}
				}
			}
			else
			{
				node.OnClick( m_currentMousePos2D );
				if ( !node.Selected )
				{
					m_mainGraphInstance.SelectNode( node, ( m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control ), true );
				}
				else if ( m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control )
				{
					m_mainGraphInstance.DeselectNode( node );
				}
				return;
			}
		}
		else if ( !m_multipleSelectionActive )
		{
			ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
			if ( newNode != null )
			{
				newNode.Vec2Position = TranformedMousePos;
				m_mainGraphInstance.AddNode( newNode, true );
				ForceRepaint();
			}
			else
			{
				List<WireBezierReference> wireRefs = m_mainGraphInstance.GetWireBezierListInPos( m_currentMousePos2D );
				if ( wireRefs != null && wireRefs.Count > 0 )
				{
					for ( int i = 0; i < wireRefs.Count; i++ )
					{
						// Place wire code here
						ParentNode outNode = m_mainGraphInstance.GetNode( wireRefs[ i ].OutNodeId );
						ParentNode inNode = m_mainGraphInstance.GetNode( wireRefs[ i ].InNodeId );

						OutputPort outputPort = outNode.GetOutputPortByUniqueId( wireRefs[ i ].OutPortId );
						InputPort inputPort = inNode.GetInputPortByUniqueId( wireRefs[ i ].InPortId );

						// Calculate the 4 points for bezier taking into account wire nodes and their automatic tangents
						Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
						Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );

						float mag = ( endPos - startPos ).magnitude;
						float resizedMag = Mathf.Min( mag, Constants.HORIZONTAL_TANGENT_SIZE * m_drawInfo.InvertedZoom );

						Vector3 startTangent = new Vector3( startPos.x + resizedMag, startPos.y );
						Vector3 endTangent = new Vector3( endPos.x - resizedMag, endPos.y );

						if ( inNode != null && inNode.GetType() == typeof( WireNode ) )
							endTangent = endPos + ( ( inNode as WireNode ).TangentDirection ) * mag * 0.33f;

						if ( outNode != null && outNode.GetType() == typeof( WireNode ) )
							startTangent = startPos - ( ( outNode as WireNode ).TangentDirection ) * mag * 0.33f;

						float dist = HandleUtility.DistancePointBezier( m_currentMousePos, startPos, endPos, startTangent, endTangent );
						if ( dist < 10 )
						{
							double doubleTapTime = EditorApplication.timeSinceStartup;
							bool doubleTap = ( doubleTapTime - m_wiredDoubleTapTimestamp ) < m_wiredDoubleTapTime;
							m_wiredDoubleTapTimestamp = doubleTapTime;

							if ( doubleTap )
							{
								ParentNode wireNode = m_mainGraphInstance.CreateNode( typeof( WireNode ), true );

								if ( wireNode != null )
								{
									wireNode.Vec2Position = TranformedMousePos;

									m_mainGraphInstance.CreateConnection( wireNode.InputPorts[ 0 ].NodeId, wireNode.InputPorts[ 0 ].PortId, outputPort.NodeId, outputPort.PortId );
									m_mainGraphInstance.CreateConnection( inputPort.NodeId, inputPort.PortId, wireNode.OutputPorts[ 0 ].NodeId, wireNode.OutputPorts[ 0 ].PortId );

									ForceRepaint();
								}
							}

							break;
						}
					}
				}
				//Reset focus from any textfield which may be selected at this time
				GUIUtility.keyboardControl = 0;
			}
		}

		if ( m_currentEvent.modifiers != EventModifiers.Shift && m_currentEvent.modifiers != EventModifiers.Control)
			m_mainGraphInstance.DeSelectAll();

		if ( UIUtils.ValidReferences() )
		{
			UIUtils.InvalidateReferences();
			return;
		}

		if ( !m_contextMenu.CheckShortcutKey() && m_currentEvent.modifiers != EventModifiers.Shift && m_currentEvent.modifiers != EventModifiers.Control )
		{
			// Only activate multiple selection if no node is selected and shift key not pressed
			m_multipleSelectionActive = true;

			m_multipleSelectionStart = TranformedMousePos;
			m_multipleSelectionArea.position = m_multipleSelectionStart;
			m_multipleSelectionArea.size = Vector2.zero;
		}

		UseCurrentEvent();
	}

	void OnLeftMouseDrag()
	{
		if ( m_lostFocus )
		{
			m_lostFocus = false;
			return;
		}

		if ( !UIUtils.ValidReferences() )
		{
			if ( m_mouseDownOnValidArea && m_insideEditorWindow )
			{
				if ( m_lastKeyPressed == KeyCode.LeftControl )
				{
					m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta, true );
				}
				else
				{
					m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta );
				}
				m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta );
				m_autoPanDirActive = true;
			}
		}
		else
		{
			List<ParentNode> nodes = m_mainGraphInstance.GetNodesInGrid( m_drawInfo.TransformedMousePos );
			if ( nodes != null && nodes.Count > 0 )
			{
				Vector2 currentPortPos = new Vector2();
				Vector2 mousePos = TranformedMousePos;

				if ( UIUtils.InputPortReference.IsValid )
				{
					OutputPort currentPort = null;
					float smallestDistance = float.MaxValue;
					Vector2 smallestPosition = Vector2.zero;
					for ( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
					{
						List<OutputPort> outputPorts = nodes[ nodeIdx ].OutputPorts;
						if ( outputPorts != null )
						{
							for ( int o = 0; o < outputPorts.Count; o++ )
							{
								currentPortPos.x = outputPorts[ o ].Position.x;
								currentPortPos.y = outputPorts[ o ].Position.y;

								currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
								float dist = ( mousePos - currentPortPos ).sqrMagnitude;
								if ( dist < smallestDistance )
								{
									smallestDistance = dist;
									smallestPosition = currentPortPos;
									currentPort = outputPorts[ o ];
								}
							}
						}
					}

					if ( currentPort != null && currentPort.Available && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
					{
						UIUtils.ActivateSnap( smallestPosition, currentPort );
					}
					else
					{
						UIUtils.DeactivateSnap();
					}
				}

				if ( UIUtils.OutputPortReference.IsValid )
				{
					InputPort currentPort = null;
					float smallestDistance = float.MaxValue;
					Vector2 smallestPosition = Vector2.zero;
					for ( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
					{
						List<InputPort> inputPorts = nodes[ nodeIdx ].InputPorts;
						if ( inputPorts != null )
						{
							for ( int i = 0; i < inputPorts.Count; i++ )
							{
								currentPortPos.x = inputPorts[ i ].Position.x;
								currentPortPos.y = inputPorts[ i ].Position.y;

								currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
								float dist = ( mousePos - currentPortPos ).sqrMagnitude;
								if ( dist < smallestDistance )
								{
									smallestDistance = dist;
									smallestPosition = currentPortPos;
									currentPort = inputPorts[ i ];
								}
							}
						}
					}
					if ( currentPort != null && currentPort.Available && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
					{
						UIUtils.ActivateSnap( smallestPosition, currentPort );
					}
					else
					{
						UIUtils.DeactivateSnap();
					}
				}
			}
			else if ( UIUtils.SnapEnabled )
			{
				UIUtils.DeactivateSnap();
			}
		}
		UseCurrentEvent();
	}

	void OnLeftMouseUp()
	{
		m_lmbPressed = false;
		if ( m_multipleSelectionActive )
		{
			m_multipleSelectionActive = false;
			UpdateSelectionArea();
			m_mainGraphInstance.MultipleSelection( m_multipleSelectionArea, (m_currentEvent.modifiers == EventModifiers.Shift || m_currentEvent.modifiers == EventModifiers.Control ), true );
		}

		if ( UIUtils.ValidReferences() )
		{
			//Check if there is some kind of port beneath the mouse ... if so connect to it
			ParentNode targetNode = UIUtils.SnapEnabled ? m_mainGraphInstance.GetNode( UIUtils.SnapPort.NodeId ) : m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
			if ( targetNode != null && targetNode.ConnStatus != NodeConnectionStatus.Island )
			{
				if ( UIUtils.InputPortReference.IsValid && UIUtils.InputPortReference.NodeId != targetNode.UniqueId )
				{
					OutputPort outputPort = UIUtils.SnapEnabled ? targetNode.GetOutputPortByUniqueId( UIUtils.SnapPort.PortId ) : targetNode.CheckOutputPortAt( m_currentMousePos );
					if ( outputPort != null && !outputPort.Locked && ( !UIUtils.InputPortReference.TypeLocked ||
												UIUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
												( UIUtils.InputPortReference.TypeLocked && outputPort.DataType == UIUtils.InputPortReference.DataType ) ) )
					{
						ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.InputPortReference.NodeId );
						InputPort inputPort = originNode.GetInputPortByUniqueId( UIUtils.InputPortReference.PortId );

						if ( !inputPort.CheckValidType( outputPort.DataType ) )
						{
							UIUtils.ShowIncompatiblePortMessage( true, originNode, inputPort, targetNode, outputPort );
							UIUtils.InvalidateReferences();
							UseCurrentEvent();
							return;
						}

						if ( !outputPort.CheckValidType( inputPort.DataType ) )
						{
							UIUtils.ShowIncompatiblePortMessage( false, targetNode, outputPort, originNode, inputPort );
							UIUtils.InvalidateReferences();
							UseCurrentEvent();
							return;
						}

						inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
						outputPort.DummyAdd( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

						if ( UIUtils.DetectNodeLoopsFrom( originNode, new Dictionary<int, int>() ) )
						{
							inputPort.DummyRemove();
							outputPort.DummyRemove();
							UIUtils.InvalidateReferences();
							ShowMessage( "Infinite Loop detected" );
							UseCurrentEvent();
							return;
						}

						inputPort.DummyRemove();
						outputPort.DummyRemove();

						if ( inputPort.IsConnected )
						{
							DeleteConnection( true, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, true, false );
						}

						//link output to input
						if ( outputPort.ConnectTo( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
							targetNode.OnOutputPortConnected( outputPort.PortId, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

						//link input to output
						if ( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, UIUtils.InputPortReference.TypeLocked ) )
							originNode.OnInputPortConnected( UIUtils.InputPortReference.PortId, targetNode.UniqueId, outputPort.PortId );
						m_mainGraphInstance.MarkWireHighlights();
					}
					else if ( outputPort != null && UIUtils.InputPortReference.TypeLocked && UIUtils.InputPortReference.DataType != outputPort.DataType )
					{
						ShowMessage( "Attempting to connect a port locked to type " + UIUtils.InputPortReference.DataType + " into a port of type " + outputPort.DataType );
					}
					ShaderIsModified = true;
				}

				if ( UIUtils.OutputPortReference.IsValid && UIUtils.OutputPortReference.NodeId != targetNode.UniqueId )
				{
					InputPort inputPort = UIUtils.SnapEnabled ? targetNode.GetInputPortByUniqueId( UIUtils.SnapPort.PortId ) : targetNode.CheckInputPortAt( m_currentMousePos );
					if ( inputPort != null && !inputPort.Locked && ( !inputPort.TypeLocked ||
												 inputPort.DataType == WirePortDataType.OBJECT ||
												 ( inputPort.TypeLocked && inputPort.DataType == UIUtils.OutputPortReference.DataType ) ) )
					{
						ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.OutputPortReference.NodeId );
						OutputPort outputPort = originNode.GetOutputPortByUniqueId( UIUtils.OutputPortReference.PortId );

						if ( !inputPort.CheckValidType( outputPort.DataType ) )
						{
							UIUtils.ShowIncompatiblePortMessage( true, targetNode, inputPort, originNode, outputPort );
							UIUtils.InvalidateReferences();
							UseCurrentEvent();
							return;
						}

						if ( !outputPort.CheckValidType( inputPort.DataType ) )
						{
							UIUtils.ShowIncompatiblePortMessage( false, originNode, outputPort, targetNode, inputPort );
							UIUtils.InvalidateReferences();
							UseCurrentEvent();
							return;
						}

						inputPort.DummyAdd( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
						outputPort.DummyAdd( inputPort.NodeId, inputPort.PortId );
						if ( UIUtils.DetectNodeLoopsFrom( targetNode, new Dictionary<int, int>() ) )
						{
							inputPort.DummyRemove();
							outputPort.DummyRemove();
							UIUtils.InvalidateReferences();
							ShowMessage( "Infinite Loop detected" );
							UseCurrentEvent();
							return;
						}

						inputPort.DummyRemove();
						outputPort.DummyRemove();

						if ( inputPort.IsConnected )
						{
							if ( m_currentEvent.control && UIUtils.SwitchPortReference.IsValid )
							{
								ParentNode oldOutputNode = UIUtils.GetNode( inputPort.GetConnection( 0 ).NodeId );
								OutputPort oldOutputPort = oldOutputNode.GetOutputPortByUniqueId( inputPort.GetConnection( 0 ).PortId );

								ParentNode switchNode = UIUtils.GetNode( UIUtils.SwitchPortReference.NodeId );
								InputPort switchPort = switchNode.GetInputPortByUniqueId( UIUtils.SwitchPortReference.PortId );

								switchPort.DummyAdd( oldOutputPort.NodeId, oldOutputPort.PortId );
								oldOutputPort.DummyAdd( switchPort.NodeId, switchPort.PortId );
								if ( UIUtils.DetectNodeLoopsFrom( switchNode, new Dictionary<int, int>() ) )
								{
									switchPort.DummyRemove();
									oldOutputPort.DummyRemove();
									UIUtils.InvalidateReferences();
									ShowMessage( "Infinite Loop detected" );
									UseCurrentEvent();
									return;
								}

								switchPort.DummyRemove();
								oldOutputPort.DummyRemove();

								DeleteConnection( true, inputPort.NodeId, inputPort.PortId, true, false );
								ConnectInputToOutput( switchPort.NodeId, switchPort.PortId, oldOutputPort.NodeId, oldOutputPort.PortId );
							}
							else
							{
								DeleteConnection( true, inputPort.NodeId, inputPort.PortId, true, false );
							}
						}
						inputPort.InvalidateAllConnections();


						//link input to output
						if ( inputPort.ConnectTo( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
							targetNode.OnInputPortConnected( inputPort.PortId, UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
						//link output to input

						if ( outputPort.ConnectTo( inputPort.NodeId, inputPort.PortId, inputPort.DataType, inputPort.TypeLocked ) )
							originNode.OnOutputPortConnected( UIUtils.OutputPortReference.PortId, targetNode.UniqueId, inputPort.PortId );
						m_mainGraphInstance.MarkWireHighlights();
					}
					else if ( inputPort != null && inputPort.TypeLocked && inputPort.DataType != UIUtils.OutputPortReference.DataType )
					{
						ShowMessage( "Attempting to connect a " + UIUtils.OutputPortReference.DataType + "to a port locked to type " + inputPort.DataType );
					}
					ShaderIsModified = true;
				}
				UIUtils.InvalidateReferences();
			}
			else
			{
				if ( UIUtils.ShowContextOnPick )
					m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
				else
					UIUtils.InvalidateReferences();

			}
		}
		UIUtils.ShowContextOnPick = true;
		UseCurrentEvent();
	}

	public void ConnectInputToOutput( int inNodeId, int inPortId, int outNodeId, int outPortId )
	{
		ParentNode inNode = m_mainGraphInstance.GetNode( inNodeId );
		ParentNode outNode = m_mainGraphInstance.GetNode( outNodeId );
		if ( inNode != null && outNode != null )
		{
			InputPort inPort = inNode.GetInputPortByUniqueId( inPortId );
			OutputPort outPort = outNode.GetOutputPortByUniqueId( outPortId );
			if ( inPort != null && outPort != null )
			{
				if ( inPort.ConnectTo( outNodeId, outPortId, inPort.DataType, inPort.TypeLocked ) )
				{
					inNode.OnInputPortConnected( inPortId, outNodeId, outPortId );
				}

				if ( outPort.ConnectTo( inNodeId, inPortId, inPort.DataType, inPort.TypeLocked ) )
				{
					outNode.OnOutputPortConnected( outPortId, inNodeId, inPortId );
				}
			}
			m_mainGraphInstance.MarkWireHighlights();
			ShaderIsModified = true;
		}
	}

	void OnRightMouseDown()
	{
		Focus();
		m_rmbStartPos = m_currentMousePos2D;
		UseCurrentEvent();
	}

	void OnRightMouseDrag()
	{
		// We look at the control to detect when user hits a tooltip ( which has a hot control of 0 )
		// This needs to be checked because on this first "frame" of hitting a tooltip because it generates incorrect mouse delta values 
		if ( GUIUtility.hotControl == 0 && m_lastHotControl != 0 )
		{
			m_lastHotControl = GUIUtility.hotControl;
			return;
		}

		m_lastHotControl = GUIUtility.hotControl;
		if ( m_currentEvent.alt )
		{
			ModifyZoom( Constants.ALT_CAMERA_ZOOM_SPEED * ( m_currentEvent.delta.x + m_currentEvent.delta.y ), m_altKeyStartPos );
		}
		else
		{
			m_cameraOffset += m_cameraZoom * m_currentEvent.delta;
		}
		UseCurrentEvent();
	}

	void OnRightMouseUp()
	{
		//Resetting the hot control test variable so it can be used again on right mouse drag detection ( if we did not do this then m_lastHotControl could be left with a a value of 0 and wouldn't be able to be correctly used on rthe drag ) 
		m_lastHotControl = -1;

		if ( ( m_rmbStartPos - m_currentMousePos2D ).sqrMagnitude < Constants.RMB_SCREEN_DIST )
		{
			ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos, true );
			if ( node == null )
			{
				m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
			}
		}
		UseCurrentEvent();
	}

	void UpdateSelectionArea()
	{
		m_multipleSelectionArea.size = TranformedMousePos - m_multipleSelectionStart;
	}

	public void OnValidObjectsDropped( UnityEngine.Object[] droppedObjs )
	{
		bool propagateDraggedObjsToNode = true;
		// Only supporting single drag&drop object selection
		if ( droppedObjs.Length == 1 )
		{
			ShaderIsModified = true;
			// Check if its a shader, material or game object  and if so load the shader graph code from it
			Shader newShader = droppedObjs[ 0 ] as Shader;
			Material newMaterial = null;
			if ( newShader == null )
			{
				newMaterial = droppedObjs[ 0 ] as Material;
				bool isProcedural = ( newMaterial != null && newMaterial is ProceduralMaterial );
				if ( newMaterial != null && !isProcedural )
				{
					newShader = newMaterial.shader;
					m_mainGraphInstance.UpdateMaterialOnMasterNode( newMaterial );
				}
				else
				{
					GameObject go = droppedObjs[ 0 ] as GameObject;
					if ( go != null )
					{
						Renderer renderer = go.GetComponent<Renderer>();
						if ( renderer )
						{
							newMaterial = renderer.sharedMaterial;
							newShader = newMaterial.shader;
						}
					}
				}
			}

			if ( newShader != null )
			{
				bool savePrevious = false;
				if ( ShaderIsModified )
				{
					Shader currentShader = m_mainGraphInstance.CurrentShader;
					savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
				}
				OnSaveShader( savePrevious, newShader, newMaterial );
				//OnValidShaderFound( currentShader, currentMaterial );
				propagateDraggedObjsToNode = false;
			}

			// if not shader loading then propagate the seletion to whats bellow the mouse
			if ( propagateDraggedObjsToNode )
			{
				ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
				if ( node != null )
				{
					// if there's a node then pass the object into it to see if there's a setup with it
					node.OnObjectDropped( droppedObjs[ 0 ] );
				}
				else
				{
					// If not then check if there's a node that can be created through the dropped object
					ParentNode newNode = m_contextMenu.CreateNodeFromCastType( droppedObjs[ 0 ].GetType() );
					if ( newNode )
					{
						newNode.Vec2Position = TranformedMousePos;
						m_mainGraphInstance.AddNode( newNode, true );
						newNode.SetupFromCastObject( droppedObjs[ 0 ] );
						ForceRepaint();
					}
				}
			}
		}
	}


	public void SetDelayedMaterialMode( Material material )
	{
		if ( material == null )
			return;
		m_delayedMaterialSet = material;
	}

	public ShaderLoadResult LoadDroppedObject( bool value, Shader shader, Material material )
	{
		ShaderLoadResult result;
		if ( value && shader != null )
		{
			string assetDatapath = AssetDatabase.GetAssetPath( shader );
			string latestOpenedFolder = Application.dataPath + assetDatapath.Substring( 6 );
			UIUtils.LatestOpenedFolder = latestOpenedFolder.Substring( 0, latestOpenedFolder.LastIndexOf( '/' ) + 1 );
			result = LoadFromDisk( assetDatapath );
			switch ( result )
			{
				case ShaderLoadResult.LOADED:
				{
					m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
				}
				break;
				case ShaderLoadResult.ASE_INFO_NOT_FOUND:
				{
					ShowMessage( "Loaded shader wasn't created with ASE. Saving it will remove previous data." );
					UIUtils.CreateEmptyFromInvalid( shader );
				}
				break;
				case ShaderLoadResult.FILE_NOT_FOUND:
				case ShaderLoadResult.UNITY_NATIVE_PATHS:
				{
					UIUtils.CreateEmptyFromInvalid( shader );
				}
				break;
			}

			m_mainGraphInstance.UpdateMaterialOnMasterNode( material );
			m_mainGraphInstance.SetMaterialModeOnGraph( material );

			if ( material != null )
			{
				CurrentSelection = ASESelectionMode.Material;
				if ( material.HasProperty( IOUtils.DefaultASEDirtyCheckId ) )
				{
					material.SetInt( IOUtils.DefaultASEDirtyCheckId, 1 );
				}
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, AssetDatabase.GetAssetPath( material ) );
			}
			else
			{
				CurrentSelection = ASESelectionMode.Shader;
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, AssetDatabase.GetAssetPath( shader ) );
			}
		}
		else
		{
			result = ShaderLoadResult.FILE_NOT_FOUND;
		}
		return result;
	}

	bool InsideMenus( Vector2 position )
	{
		for ( int i = 0; i < m_registeredMenus.Count; i++ )
		{
			if ( m_registeredMenus[ i ].IsInside( position ) )
			{
				return true;
			}
		}
		return false;
	}

	void HandleGUIEvents()
	{
		if ( m_currentEvent.type == EventType.KeyDown )
		{
			m_contextMenu.UpdateKeyPress( m_currentEvent.keyCode );
		}
		else if ( m_currentEvent.type == EventType.keyUp )
		{
			m_contextMenu.UpdateKeyReleased( m_currentEvent.keyCode );
		}

		if ( InsideMenus( m_currentMousePos2D ) )
		{
			if ( m_currentEvent.type == EventType.mouseDown )
			{
				m_mouseDownOnValidArea = false;
				UseCurrentEvent();
			}
			return;
		}

		int controlID = GUIUtility.GetControlID( FocusType.Passive );
		switch ( m_currentEvent.GetTypeForControl( controlID ) )
		{
			case EventType.MouseDown:
			{
				GUIUtility.hotControl = controlID;
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseDown();
					}
					break;
					case ButtonClickId.RightMouseButton:
					case ButtonClickId.MiddleMouseButton:
					{
						OnRightMouseDown();
					}
					break;
				}
			}
			break;

			case EventType.MouseUp:
			{
				GUIUtility.hotControl = 0;
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseUp();
					}
					break;
					case ButtonClickId.MiddleMouseButton: break;
					case ButtonClickId.RightMouseButton:
					{
						OnRightMouseUp();
					}
					break;
				}
			}
			break;
			case EventType.MouseDrag:
			{
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseDrag();
					}
					break;
					case ButtonClickId.MiddleMouseButton:
					case ButtonClickId.RightMouseButton:
					{
						OnRightMouseDrag();
					}
					break;
				}
			}
			break;
			case EventType.ScrollWheel:
			{
				OnScrollWheel();
			}
			break;
			case EventType.keyDown:
			{
				OnKeyboardDown();
			}
			break;
			case EventType.keyUp:
			{
				OnKeyboardUp();
			}
			break;
			case EventType.ExecuteCommand:
			case EventType.ValidateCommand:
			{
				switch ( m_currentEvent.commandName )
				{
					case CopyCommand: CopyToClipboard(); break;
					case PasteCommand: PasteFromClipboard( true ); break;
					case SelectAll:
					{
						m_mainGraphInstance.SelectAll();
						ForceRepaint();
					}
					break;
					case Duplicate:
					{
						CopyToClipboard();
						PasteFromClipboard( true );
					}
					break;
				}
			}
			break;
			case EventType.Repaint:
			{
			}
			break;
		}

		m_dragAndDropTool.TestDragAndDrop( m_graphArea );

	}

	public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback )
	{
		m_mainGraphInstance.DeleteConnection( isInput, nodeId, portId, registerOnLog, propagateCallback );
	}

	void DeleteSelectedNodes()
	{
		m_mainGraphInstance.DeleteSelectedNodes();
		ForceRepaint();
	}

	void OnKeyboardUp()
	{
		if ( m_lastKeyPressed == KeyCode.W )
		{
			if ( !m_toggleDebug )
				m_optionsWindow.ColoredPorts = false;

			ForceRepaint();
		}

		if ( m_shortcutManager.ActivateShortcut( m_lastKeyPressed, false ) )
		{
			ForceRepaint();
		}
		m_lastKeyPressed = KeyCode.None;
	}

	bool OnKeyboardPress( KeyCode code )
	{
		return ( m_currentEvent.keyCode == code && m_lastKeyPressed == KeyCode.None );
	}

	void OnKeyboardDown()
	{
		if ( DebugConsoleWindow.DeveloperMode )
		{
			if ( OnKeyboardPress( KeyCode.F8 ) )
			{
				Shader currShader = CurrentGraph.CurrentShader;
				ShaderUtilEx.OpenCompiledShader( currShader, ShaderInspectorPlatformsPopupEx.GetCurrentMode(), ShaderInspectorPlatformsPopupEx.GetCurrentPlatformMask(), ShaderInspectorPlatformsPopupEx.GetCurrentVariantStripping() == 0 );

				string filename = Application.dataPath;
				filename = filename.Replace( "Assets", "Temp/Compiled-" );
				string shaderFilename = AssetDatabase.GetAssetPath( currShader );
				int lastIndex = shaderFilename.LastIndexOf( '/' ) + 1;
				filename = filename + shaderFilename.Substring( lastIndex );

				string compiledContents = IOUtils.LoadTextFileFromDisk( filename );
				Debug.Log( compiledContents );
			}

			if ( OnKeyboardPress( KeyCode.F9 ) )
			{
				m_nodeExporterUtils.CalculateShaderInstructions( CurrentGraph.CurrentShader );
			}
		}

		if ( m_lastKeyPressed == KeyCode.None )
		{
			m_shortcutManager.ActivateShortcut( m_currentEvent.keyCode, true );
		}

		if ( m_currentEvent.control && m_currentEvent.shift && m_currentEvent.keyCode == KeyCode.V )
		{
			PasteFromClipboard( false );
		}

		if ( OnKeyboardPress( KeyCode.LeftAlt ) || OnKeyboardPress( KeyCode.RightAlt ) || OnKeyboardPress( KeyCode.AltGr ) )
		{
			m_altKeyStartPos = m_currentMousePos2D;
		}

		if ( m_currentEvent.keyCode != KeyCode.None )
			m_lastKeyPressed = m_currentEvent.keyCode;
	}

	void OnScrollWheel()
	{
		ModifyZoom( m_currentEvent.delta.y, m_currentMousePos2D );
		UseCurrentEvent();
	}

	void ModifyZoom( float zoomIncrement, Vector2 pivot )
	{
		float minCam = Mathf.Min( ( m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth ) ), ( m_cameraInfo.height - ( m_toolsWindow.Height ) ) );
		if ( minCam < 1 )
			minCam = 1;

		float dynamicMaxZoom = m_mainGraphInstance.MaxNodeDist / minCam;

		Vector2 canvasPos = TranformPosition( pivot );
		CameraZoom = Mathf.Clamp( m_cameraZoom + zoomIncrement * Constants.CAMERA_ZOOM_SPEED, Constants.CAMERA_MIN_ZOOM, Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) );
		m_cameraOffset.x = pivot.x * m_cameraZoom - canvasPos.x;
		m_cameraOffset.y = pivot.y * m_cameraZoom - canvasPos.y;
	}

	void OnSelectionChange()
	{
		ForceRepaint();
	}

	void OnLostFocus()
	{
		m_lostFocus = true;
		m_multipleSelectionActive = false;
		UIUtils.InvalidateReferences();
		m_genericMessageUI.CleanUpMessageStack();
		m_nodeParametersWindow.OnLostFocus();
		m_paletteWindow.OnLostFocus();
	}

	void CopyToClipboard()
	{
		m_copyPasteDeltaMul = 0;
		m_copyPasteDeltaPos = new Vector2( float.MaxValue, float.MaxValue );
		m_clipboard.ClearClipboard();
		m_clipboard.AddToClipboard( m_mainGraphInstance.SelectedNodes );
		m_copyPasteInitialPos = m_mainGraphInstance.SelectedNodesCentroid;
	}

	ParentNode CreateNodeFromClipboardData( int clipId )
	{
		string[] parameters = m_clipboard.CurrentClipboardStrData[ clipId ].Data.Split( IOUtils.FIELD_SEPARATOR );
		ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( Type.GetType( parameters[ IOUtils.NodeTypeId ] ) );
		if ( newNode != null )
		{
			try
			{
				newNode.ReadFromString( ref parameters );
				newNode.ReadInputDataFromString( ref parameters );
				newNode.ReadOutputDataFromString( ref parameters );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}

			newNode.ReleaseUniqueIdData();
			m_mainGraphInstance.AddNode( newNode, true );
			m_clipboard.CurrentClipboardStrData[ clipId ].NewNodeId = newNode.UniqueId;
			return newNode;
		}
		return null;
	}

	void CreateConnectionsFromClipboardData( int clipId )
	{
		if ( String.IsNullOrEmpty( m_clipboard.CurrentClipboardStrData[ clipId ].Connections ) )
			return;
		string[] lines = m_clipboard.CurrentClipboardStrData[ clipId ].Connections.Split( IOUtils.LINE_TERMINATOR );

		// last line is always an empty one
		for ( int lineIdx = 0; lineIdx < lines.Length - 1; lineIdx++ )
		{
			string[] parameters = lines[ lineIdx ].Split( IOUtils.FIELD_SEPARATOR );

			int InNodeId = 0;
			int InPortId = 0;
			int OutNodeId = 0;
			int OutPortId = 0;

			try
			{
				InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
				InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );

				OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
				OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}


			int newInNodeId = m_clipboard.GeNewNodeId( InNodeId );
			int newOutNodeId = m_clipboard.GeNewNodeId( OutNodeId );

			if ( newInNodeId > -1 && newOutNodeId > -1 )
			{
				ParentNode inNode = m_mainGraphInstance.GetNode( newInNodeId );
				ParentNode outNode = m_mainGraphInstance.GetNode( newOutNodeId );

				InputPort inputPort = null;
				OutputPort outputPort = null;

				if ( inNode != null && outNode != null )
				{
					inputPort = inNode.GetInputPortByUniqueId( InPortId );
					outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
					if ( inputPort != null && outputPort != null )
					{
						inputPort.ConnectTo( newOutNodeId, OutPortId, outputPort.DataType, false );
						outputPort.ConnectTo( newInNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

						inNode.OnInputPortConnected( InPortId, newOutNodeId, OutPortId );
						outNode.OnOutputPortConnected( OutPortId, newInNodeId, InPortId );
					}
				}
			}
		}
	}

	void PasteFromClipboard( bool copyConnections )
	{
		if ( m_clipboard.CurrentClipboardStrData.Count == 0 )
		{
			return;
		}

		Vector2 deltaPos = TranformedKeyEvtMousePos - m_copyPasteInitialPos;
		if ( ( m_copyPasteDeltaPos - deltaPos ).magnitude > 5.0f )
		{
			m_copyPasteDeltaMul = 0;
		}
		else
		{
			m_copyPasteDeltaMul += 1;
		}
		m_copyPasteDeltaPos = deltaPos;

		m_mainGraphInstance.DeSelectAll();
		UIUtils.InhibitMessages = true;
		for ( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
		{
			ParentNode node = CreateNodeFromClipboardData( i );
			m_clipboard.CurrentClipboardStrData[ i ].NewNodeId = node.UniqueId;
			Vector2 pos = node.Vec2Position;
			node.Vec2Position = pos + deltaPos + m_copyPasteDeltaMul * Constants.CopyPasteDeltaPos;
			m_mainGraphInstance.SelectNode( node, true, false );
		}

		if ( copyConnections )
		{
			for ( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
			{
				CreateConnectionsFromClipboardData( i );
			}
		}

		UIUtils.InhibitMessages = false;
		ShaderIsModified = true;
		ForceRepaint();
	}

	public string GenerateGraphInfo()
	{
		string graphInfo = IOUtils.ShaderBodyBegin + '\n';
		string nodesInfo = "";
		string connectionsInfo = "";
		graphInfo += m_versionInfo.FullLabel + '\n';
		graphInfo += (
						m_cameraInfo.x.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.y.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.width.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.height.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraOffset.x.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraOffset.y.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraZoom.ToString() + IOUtils.FIELD_SEPARATOR +
						m_nodeParametersWindow.IsMaximized + IOUtils.FIELD_SEPARATOR +
						m_paletteWindow.IsMaximized + '\n'
						);
		m_mainGraphInstance.OrderNodesByGraphDepth();
		m_mainGraphInstance.WriteToString( ref nodesInfo, ref connectionsInfo );
		graphInfo += nodesInfo;
		graphInfo += connectionsInfo;
		graphInfo += IOUtils.ShaderBodyEnd + '\n';

		return graphInfo;
	}

	public ShaderLoadResult LoadFromDisk( string pathname )
	{
		UIUtils.DirtyMask = false;
		if ( UIUtils.IsUnityNativeShader( pathname ) )
		{
			ShowMessage( "Cannot edit native unity shaders.\nReplacing by a new one." );
			return ShaderLoadResult.UNITY_NATIVE_PATHS;
		}

		m_lastOpenedLocation = pathname;
		string buffer = IOUtils.LoadTextFileFromDisk( pathname );
		if ( String.IsNullOrEmpty( buffer ) )
		{
			ShowMessage( "Could not open file " + pathname );
			return ShaderLoadResult.FILE_NOT_FOUND;
		}

		if ( !IOUtils.HasValidShaderBody( ref buffer ) )
		{
			return ShaderLoadResult.ASE_INFO_NOT_FOUND;
		}

		m_mainGraphInstance.CleanNodes();
		Reset();

		ShaderLoadResult loadResult = ShaderLoadResult.LOADED;
		// Find checksum value on body
		int checksumId = buffer.IndexOf( IOUtils.CHECKSUM );
		if ( checksumId > -1 )
		{
			string checkSumStoredValue = buffer.Substring( checksumId );
			string trimmedBuffer = buffer.Remove( checksumId );

			string[] typeValuePair = checkSumStoredValue.Split( IOUtils.VALUE_SEPARATOR );
			if ( typeValuePair != null && typeValuePair.Length == 2 )
			{
				// Check read checksum and compare with the actual shader body to detect external changes
				string currentChecksumValue = IOUtils.CreateChecksum( trimmedBuffer );
				if ( DebugConsoleWindow.DeveloperMode && !currentChecksumValue.Equals( typeValuePair[ 1 ] ) )
				{
					ShowMessage( "Wrong checksum" );
				}

				trimmedBuffer = trimmedBuffer.Replace( "\r", string.Empty );
				// find node info body
				int shaderBodyId = trimmedBuffer.IndexOf( IOUtils.ShaderBodyBegin );
				if ( shaderBodyId > -1 )
				{
					trimmedBuffer = trimmedBuffer.Substring( shaderBodyId );
					//Find set of instructions
					string[] instructions = trimmedBuffer.Split( IOUtils.LINE_TERMINATOR );
					// First line is to be ignored and second line contains version
					string[] versionParams = instructions[ 1 ].Split( IOUtils.VALUE_SEPARATOR );
					if ( versionParams.Length == 2 )
					{
						int version = 0;
						try
						{
							version = Convert.ToInt32( versionParams[ 1 ] );
						}
						catch ( Exception e )
						{
							Debug.LogError( e );
						}

						if ( version > m_versionInfo.FullNumber )
						{
							ShowMessage( "This shader was created on a new ASE version\nPlease install v." + version );
						}

						if ( DebugConsoleWindow.DeveloperMode )
						{
							if ( version < m_versionInfo.FullNumber )
							{
								ShowMessage( "This shader was created on a older ASE version\nSaving will update it to the new one." );
							}
						}

						m_mainGraphInstance.LoadedShaderVersion = version;
					}
					else
					{
						ShowMessage( "Corrupted version" );
					}

					// Dummy values,camera values can only be applied after node loading is complete
					Rect dummyCameraInfo = new Rect();
					Vector2 dummyCameraOffset = new Vector2();
					float dummyCameraZoom = 0;
					bool applyDummy = false;
					bool dummyNodeParametersWindowMaximized = false;
					bool dummyPaletteWindowMaximized = false;

					//Second line contains camera information ( position, size, offset and zoom )
					string[] cameraParams = instructions[ 2 ].Split( IOUtils.FIELD_SEPARATOR );
					if ( cameraParams.Length == 9 )
					{
						applyDummy = true;
						try
						{
							dummyCameraInfo.x = Convert.ToSingle( cameraParams[ 0 ] );
							dummyCameraInfo.y = Convert.ToSingle( cameraParams[ 1 ] );
							dummyCameraInfo.width = Convert.ToSingle( cameraParams[ 2 ] );
							dummyCameraInfo.height = Convert.ToSingle( cameraParams[ 3 ] );
							dummyCameraOffset.x = Convert.ToSingle( cameraParams[ 4 ] );
							dummyCameraOffset.y = Convert.ToSingle( cameraParams[ 5 ] );
							dummyCameraZoom = Convert.ToSingle( cameraParams[ 6 ] );
							dummyNodeParametersWindowMaximized = Convert.ToBoolean( cameraParams[ 7 ] );
							dummyPaletteWindowMaximized = Convert.ToBoolean( cameraParams[ 8 ] );
						}
						catch ( Exception e )
						{
							Debug.LogError( e );
						}
					}
					else
					{
						ShowMessage( "Camera parameters are corrupted" );
					}

					// valid instructions are only between the line after version and the line before the last one ( which contains ShaderBodyEnd ) 
					for ( int instructionIdx = 3; instructionIdx < instructions.Length - 1; instructionIdx++ )
					{
						//TODO: After all is working, convert string parameters to ints in order to speed up reading
						string[] parameters = instructions[ instructionIdx ].Split( IOUtils.FIELD_SEPARATOR );

						// All nodes must be created before wiring the connections ... 
						// Since all nodes on the save op are written before the wires, we can safely create them
						// If that order is not maintained the it's because of external editing and its the users responsability
						switch ( parameters[ 0 ] )
						{
							case IOUtils.NodeParam:
							{
								Type type = Type.GetType( parameters[ IOUtils.NodeTypeId ] );
								if ( type != null )
								{
									Type oldType = type;
									NodeAttributes attribs = m_contextMenu.GetNodeAttributesForType( type );
									if ( attribs == null )
									{
										attribs = m_contextMenu.GetDeprecatedNodeAttributesForType( type );
										if ( attribs != null )
										{
											if ( attribs.Deprecated && attribs.DeprecatedAlternativeType != null )
											{
												type = attribs.DeprecatedAlternativeType;
												ShowMessage( string.Format( "Node {0} is deprecated and was replaced by {1} ", attribs.Name, attribs.DeprecatedAlternative ) );
											}
										}
									}

									ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( type );
									if ( newNode != null )
									{
										try
										{
											if ( oldType != type )
												newNode.ParentReadFromString( ref parameters );
											else
												newNode.ReadFromString( ref parameters );


											if ( oldType == type )
											{
												newNode.ReadInputDataFromString( ref parameters );
												if ( UIUtils.CurrentShaderVersion() > 5107 )
												{
													newNode.ReadOutputDataFromString( ref parameters );
												}
											}
										}
										catch ( Exception e )
										{
											Debug.LogError( e );
										}
										m_mainGraphInstance.AddNode( newNode, false );
									}
								}
								else
								{
									ShowMessage( string.Format( "{0} is not a valid ASE node ", parameters[ IOUtils.NodeTypeId ] ), MessageSeverity.Error );
								}
							}
							break;
							case IOUtils.WireConnectionParam:
							{
								int InNodeId = 0;
								int InPortId = 0;
								int OutNodeId = 0;
								int OutPortId = 0;

								try
								{
									InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
									InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );
									OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
									OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
								}
								catch ( Exception e )
								{
									Debug.LogError( e );
								}


								ParentNode inNode = m_mainGraphInstance.GetNode( InNodeId );
								ParentNode outNode = m_mainGraphInstance.GetNode( OutNodeId );

								//if ( UIUtils.CurrentShaderVersion() < 5002 )
								//{
								//	InPortId = inNode.VersionConvertInputPortId( InPortId );
								//	OutPortId = outNode.VersionConvertOutputPortId( OutPortId );
								//}

								InputPort inputPort = null;
								OutputPort outputPort = null;
								if ( inNode != null && outNode != null )
								{

									if ( UIUtils.CurrentShaderVersion() < 5002 )
									{
										InPortId = inNode.VersionConvertInputPortId( InPortId );
										OutPortId = outNode.VersionConvertOutputPortId( OutPortId );

										inputPort = inNode.GetInputPortByArrayId( InPortId );
										outputPort = outNode.GetOutputPortByArrayId( OutPortId );
									}
									else
									{
										inputPort = inNode.GetInputPortByUniqueId( InPortId );
										outputPort = outNode.GetOutputPortByUniqueId( OutPortId );
									}

									if ( inputPort != null && outputPort != null )
									{
										bool inputCompatible = inputPort.CheckValidType( outputPort.DataType );
										bool outputCompatible = outputPort.CheckValidType( inputPort.DataType );
										if ( inputCompatible && outputCompatible )
										{
											inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false );
											outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

											inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId, false );
											outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
										}
										else if ( DebugConsoleWindow.DeveloperMode )
										{
											if ( !inputCompatible )
												UIUtils.ShowIncompatiblePortMessage( true, inNode, inputPort, outNode, outputPort );

											if ( !outputCompatible )
												UIUtils.ShowIncompatiblePortMessage( true, outNode, outputPort, inNode, inputPort );
										}
									}
									else if ( DebugConsoleWindow.DeveloperMode )
									{
										if ( inputPort == null )
										{
											UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
										}
										else
										{
											UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
										}
									}
								}
								else if ( DebugConsoleWindow.DeveloperMode )
								{
									if ( inNode == null )
									{
										UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
									}
									else
									{
										UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
									}
								}
							}
							break;
						}
					}

					Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( pathname );
					if ( shader )
					{
						m_mainGraphInstance.ForceSignalPropagationOnMasterNode();
						m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
						m_onLoadDone = 2;
						if ( applyDummy )
						{
							m_cameraInfo = dummyCameraInfo;
							m_cameraOffset = dummyCameraOffset;
							CameraZoom = dummyCameraZoom;
							if ( DebugConsoleWindow.UseShaderPanelsInfo )
							{
								m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized = dummyNodeParametersWindowMaximized;
								m_paletteWindowMaximized = m_paletteWindow.IsMaximized = dummyPaletteWindowMaximized;
							}
						}
					}
					else
					{
						ShowMessage( "Could not load shader asset" );
					}
				}
				else
				{
					ShowMessage( "Graph info not found" );
				}
			}
			else
			{
				ShowMessage( "Corrupted checksum" );
			}
		}
		else
		{
			ShowMessage( "Checksum not found" );
		}

		UIUtils.CurrentMasterNode().ForcePortType();
		UIUtils.DirtyMask = true;
		m_checkInvalidConnections = true;
		return loadResult;
	}

	public void ShowPortInfo()
	{
		GetWindow<PortLegendInfo>();
	}

	public void ShowShaderLibrary()
	{
		GetWindow<ShaderLibrary>();
	}

	public void ShowMessage( string message, MessageSeverity severity = MessageSeverity.Normal, bool registerTimestamp = true )
	{
		if ( UIUtils.InhibitMessages )
			return;

		if ( m_genericMessageUI.DisplayingMessage )
		{
			m_genericMessageUI.AddToQueue( message, severity );
		}
		else
		{
			if ( registerTimestamp )
				m_genericMessageUI.StartMessageCounter();

			ShowMessageImmediately( message, severity );
		}
	}

	public void ShowMessageImmediately( string message, MessageSeverity severity = MessageSeverity.Normal )
	{
		if ( UIUtils.InhibitMessages )
			return;

		switch ( severity )
		{
			case MessageSeverity.Normal: { m_genericMessageContent.text = string.Empty; } break;
			case MessageSeverity.Warning: { m_genericMessageContent.text = "Warning!\n"; } break;
			case MessageSeverity.Error: { m_genericMessageContent.text = "Error!!!\n"; } break;
		}
		m_genericMessageContent.text += message;
		Debug.Log( message );
		ShowNotification( m_genericMessageContent );
	}

	void OnGUI()
	{

		if ( !m_initialized )
		{
			UIUtils.InitMainSkin();
			Init();
		}

		if ( m_nodeParametersWindow != null && EditorVariablesManager.NodeParametersMaximized.Value != m_nodeParametersWindow.IsMaximized )
			EditorVariablesManager.NodeParametersMaximized.Value = m_nodeParametersWindow.IsMaximized;
		if ( m_paletteWindow != null && EditorVariablesManager.NodePaletteMaximized.Value != m_paletteWindow.IsMaximized )
			EditorVariablesManager.NodePaletteMaximized.Value = m_paletteWindow.IsMaximized;

		if ( m_checkInvalidConnections )
		{
			m_checkInvalidConnections = false;
			m_mainGraphInstance.DeleteInvalidConnections();
		}

		if ( m_repaintIsDirty )
		{
			m_repaintIsDirty = false;
			ForceRepaint();
		}

		if ( m_forcingMaterialUpdateFlag )
		{
			Focus();
			Debug.Log( Event.current.type );
			if ( m_materialsToUpdate.Count > 0 )
			{
				float percentage = 100.0f * ( float ) ( UIUtils.TotalExampleMaterials - m_materialsToUpdate.Count ) / ( float ) UIUtils.TotalExampleMaterials;
				if ( m_forcingMaterialUpdateOp ) // Read
				{
					Debug.Log( percentage + "% Recompiling " + m_materialsToUpdate[ 0 ].name );
					LoadDroppedObject( true, m_materialsToUpdate[ 0 ].shader, m_materialsToUpdate[ 0 ] );
				}
				else // Write
				{
					Debug.Log( percentage + "% Saving " + m_materialsToUpdate[ 0 ].name );
					SaveToDisk( false );
					m_materialsToUpdate.RemoveAt( 0 );
				}
				m_forcingMaterialUpdateOp = !m_forcingMaterialUpdateOp;
			}
			else
			{
				Debug.Log( "100% - All Materials compiled " );
				m_forcingMaterialUpdateFlag = false;
			}
		}


		if ( m_removedKeyboardFocus )
		{
			m_removedKeyboardFocus = false;
			GUIUtility.keyboardControl = 0;
		}

		m_mainGraphInstance.UpdateMarkForDeletion();

		Vector2 pos = Event.current.mousePosition;
		pos.x += position.x;
		pos.y += position.y;
		m_insideEditorWindow = position.Contains( pos );

		if ( m_delayedLoadObject != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			LoadObject( m_delayedLoadObject );
			m_delayedLoadObject = null;
		}

		if ( m_delayedMaterialSet != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			m_mainGraphInstance.UpdateMaterialOnMasterNode( m_delayedMaterialSet );
			m_mainGraphInstance.SetMaterialModeOnGraph( m_delayedMaterialSet );
			CurrentSelection = ASESelectionMode.Material;
			m_delayedMaterialSet = null;
		}

		Material currentMaterial = m_mainGraphInstance.CurrentMaterial;
		if ( m_forceUpdateFromMaterialFlag )
		{
			m_forceUpdateFromMaterialFlag = false;
			if ( currentMaterial != null )
			{
				m_mainGraphInstance.CopyValuesFromMaterial( currentMaterial );
				m_repaintIsDirty = true;
			}
		}

		m_repaintCount = 0;
		m_cameraInfo = position;
		m_currentEvent = Event.current;

		if ( m_currentEvent.type == EventType.keyDown )
			m_keyEvtMousePos2D = m_currentEvent.mousePosition;

		m_currentMousePos2D = m_currentEvent.mousePosition;
		m_currentMousePos.x = m_currentMousePos2D.x;
		m_currentMousePos.y = m_currentMousePos2D.y;

		m_graphArea.width = m_cameraInfo.width;
		m_graphArea.height = m_cameraInfo.height;

		m_autoPanDirActive = m_lmbPressed || m_forceAutoPanDir || m_multipleSelectionActive || UIUtils.ValidReferences();


		// Need to use it in order to prevent Mismatched LayoutGroup on ValidateCommand when rendering nodes
		if ( Event.current.type == EventType.ValidateCommand )
		{
			Event.current.Use();
		}

		// Nodes Graph background area
		GUILayout.BeginArea( m_graphArea, "Nodes" );
		{
			// Camera movement is simulated by grabing the current camera offset, transforming it into texture space and manipulating the tiled texture uv coords
			GUI.DrawTextureWithTexCoords( m_graphArea, m_graphBgTexture,
				new Rect( ( -m_cameraOffset.x / m_graphBgTexture.width ),
							( m_cameraOffset.y / m_graphBgTexture.height ) - m_cameraZoom * m_cameraInfo.height / m_graphBgTexture.height,
							m_cameraZoom * m_cameraInfo.width / m_graphBgTexture.width,
							m_cameraZoom * m_cameraInfo.height / m_graphBgTexture.height ) );

			Color col = GUI.color;
			GUI.color = new Color( 1, 1, 1, 0.7f );
			GUI.DrawTexture( m_graphArea, m_graphFgTexture, ScaleMode.StretchToFill, true );
			GUI.color = col;
		}
		GUILayout.EndArea();

		bool restoreMouse = false;
		if ( InsideMenus( m_currentMousePos2D ) /*|| _confirmationWindow.IsActive*/ )
		{
			if ( Event.current.type == EventType.mouseDown )
			{
				restoreMouse = true;
				Event.current.type = EventType.ignore;
			}

			// Must guarantee that mouse up ops on menus will reset auto pan if it is set
			if ( Event.current.type == EventType.MouseUp && m_currentEvent.button == ButtonClickId.LeftMouseButton )
			{
				m_lmbPressed = false;
			}

		}
		// Nodes
		GUILayout.BeginArea( m_graphArea );
		{
			m_drawInfo.CameraArea = m_cameraInfo;
			m_drawInfo.TransformedCameraArea = m_graphArea;

			m_drawInfo.MousePosition = m_currentMousePos2D;
			m_drawInfo.CameraOffset = m_cameraOffset;
			m_drawInfo.InvertedZoom = 1 / m_cameraZoom;
			m_drawInfo.LeftMouseButtonPressed = m_currentEvent.button == ButtonClickId.LeftMouseButton;
			m_drawInfo.CurrentEventType = m_currentEvent.type;
			m_drawInfo.ZoomChanged = m_zoomChanged;

			m_drawInfo.TransformedMousePos = m_currentMousePos2D * m_cameraZoom - m_cameraOffset;
			UIUtils.UpdateMainSkin( m_drawInfo );

			// Draw mode indicator

			m_modeWindow.Draw( m_graphArea, m_currentMousePos2D, m_mainGraphInstance.CurrentShader, currentMaterial,
								0.5f * ( m_graphArea.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ),
								( m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0 ),
								( m_paletteWindow.IsMaximized ? m_paletteWindow.RealWidth : 0 ) );

			PreTestLeftMouseDown();
			m_mainGraphInstance.DrawWires( m_wireTexture, m_drawInfo, m_contextPalette.IsActive, m_contextPalette.CurrentPosition );
			m_repaintIsDirty = m_mainGraphInstance.Draw( m_drawInfo ) || m_repaintIsDirty;
			m_mainGraphInstance.DrawGrid( m_drawInfo );
			bool hasUnusedConnNodes = m_mainGraphInstance.HasUnConnectedNodes;
			m_toolsWindow.SetStateOnButton( ToolButtonType.CleanUnusedNodes, hasUnusedConnNodes ? 1 : 0 );

			m_zoomChanged = false;

			MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
			if ( masterNode )
			{
				m_toolsWindow.DrawShaderTitle( m_nodeParametersWindow, m_paletteWindow, AvailableCanvasWidth, m_graphArea.height, masterNode.ShaderName );
			}
		}

		GUILayout.EndArea();

		if ( restoreMouse )
		{
			Event.current.type = EventType.mouseDown;
		}

		m_toolsWindow.InitialX = m_nodeParametersWindow.RealWidth;
		m_toolsWindow.Width = m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth );
		m_toolsWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, false );

		bool autoMinimize = false;
		if ( position.width < m_lastWindowWidth && position.width < Constants.MINIMIZE_WINDOW_LOCK_SIZE )
		{
			autoMinimize = true;
		}

		if ( autoMinimize )
			m_nodeParametersWindow.IsMaximized = false;

		ParentNode selectedNode = ( m_mainGraphInstance.SelectedNodes.Count == 1 ) ? m_mainGraphInstance.SelectedNodes[ 0 ] : m_mainGraphInstance.CurrentMasterNode;
		m_repaintIsDirty = m_nodeParametersWindow.Draw( m_cameraInfo, selectedNode, m_currentMousePos2D, m_currentEvent.button, false ) || m_repaintIsDirty; //TODO: If multiple nodes from the same type are selected also show a parameters window which modifies all of them 
		if ( m_nodeParametersWindow.IsResizing )
			m_repaintIsDirty = true;

		// Test to ignore mouse on main palette when inside context palette ... IsInside also takes active state into account 
		bool ignoreMouseForPalette = m_contextPalette.IsInside( m_currentMousePos2D );
		if ( ignoreMouseForPalette && Event.current.type == EventType.mouseDown )
		{
			Event.current.type = EventType.ignore;
		}
		if ( autoMinimize )
			m_paletteWindow.IsMaximized = false;

		m_paletteWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, !m_contextPalette.IsActive );
		if ( m_paletteWindow.IsResizing )
		{
			m_repaintIsDirty = true;
		}

		if ( ignoreMouseForPalette )
		{
			if ( restoreMouse )
			{
				Event.current.type = EventType.mouseDown;
			}
		}

		if ( m_contextPalette.IsActive )
		{
			m_contextPalette.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button, m_contextPalette.IsActive );
		}

		if ( m_palettePopup.IsActive )
		{
			m_palettePopup.Draw( m_currentMousePos2D );
			m_repaintIsDirty = true;
			int controlID = GUIUtility.GetControlID( FocusType.Passive );
			if ( m_currentEvent.GetTypeForControl( controlID ) == EventType.MouseUp )
			{
				if ( m_currentEvent.button == ButtonClickId.LeftMouseButton )
				{
					m_palettePopup.Deactivate();
					if ( !InsideMenus( m_currentMousePos2D ) )
					{
						CreateNode( m_paletteChosenType, TranformedMousePos );
					}
				}
			}
		}

		// Handle all events ( mouse interaction + others )
		HandleGUIEvents();

		// UI Overlay
		// Selection Box
		if ( m_multipleSelectionActive )
		{
			UpdateSelectionArea();
			Rect transformedArea = m_multipleSelectionArea;
			transformedArea.position = ( transformedArea.position + m_cameraOffset ) / m_cameraZoom;
			transformedArea.size /= m_cameraZoom;

			if ( transformedArea.width < 0 )
			{
				transformedArea.width = -transformedArea.width;
				transformedArea.x -= transformedArea.width;
			}

			if ( transformedArea.height < 0 )
			{
				transformedArea.height = -transformedArea.height;
				transformedArea.y -= transformedArea.height;
			}
			Color original = GUI.color;
			GUI.color = Constants.BoxSelectionColor;
			GUI.Box( transformedArea, "", m_customStyles.Box );
			GUI.backgroundColor = original;
		}

		bool isResizing = m_toolsWindow.IsResizing || m_paletteWindow.IsResizing;
		//Test boundaries for auto-pan
		if ( !isResizing && m_autoPanDirActive )
		{
			m_autoPanArea[ ( int ) AutoPanLocation.LEFT ].AdjustInitialX = m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0;
			m_autoPanArea[ ( int ) AutoPanLocation.RIGHT ].AdjustInitialX = m_paletteWindow.IsMaximized ? -m_paletteWindow.RealWidth : 0;
			Vector2 autoPanDir = Vector2.zero;
			for ( int i = 0; i < m_autoPanArea.Length; i++ )
			{
				if ( m_autoPanArea[ i ].CheckArea( m_currentMousePos2D, m_cameraInfo, false ) )
				{
					autoPanDir += m_autoPanArea[ i ].Velocity;
				}
			}
			m_cameraOffset += autoPanDir;
			if ( !UIUtils.ValidReferences() && m_insideEditorWindow )
			{
				m_mainGraphInstance.MoveSelectedNodes( -autoPanDir );
			}

			m_repaintIsDirty = true;
		}



		m_isDirty = m_isDirty || m_mainGraphInstance.IsDirty;
		if ( m_isDirty )
		{
			m_isDirty = false;
			ShaderIsModified = true;
			EditorUtility.SetDirty( this );
		}

		m_saveIsDirty = m_saveIsDirty || m_mainGraphInstance.SaveIsDirty;
		if ( m_saveIsDirty )
		{
			ShaderIsModified = true;
			m_saveIsDirty = false;
			if ( m_liveShaderEditing && focusedWindow )
			{
				if ( m_mainGraphInstance.CurrentMasterNodeId != Constants.INVALID_NODE_ID )
				{
					SaveToDisk( true );
				}
				else
				{
					ShowMessage( LiveShaderError );
				}
			}
		}

		if ( m_onLoadDone > 0 )
		{
			m_onLoadDone--;
			if ( m_onLoadDone == 0 )
			{
				ShaderIsModified = false;
			}
		}

		if ( m_repaintIsDirty )
		{
			m_repaintIsDirty = false;
			ForceRepaint();
		}

		if ( m_cacheSaveOp )
		{
			if ( ( EditorApplication.timeSinceStartup - m_lastTimeSaved ) > SaveTime )
			{
				SaveToDisk( false );
			}
		}
		m_genericMessageUI.CheckForMessages();

		if ( m_ctrlSCallback )
		{
			m_ctrlSCallback = false;
			OnToolButtonPressed( ToolButtonType.Update );
		}

		m_lastWindowWidth = position.width;
		m_nodeExporterUtils.Update();

		if ( m_markedToSave )
		{
			m_markedToSave = false;
			SaveToDisk( false );
		}
	}

	public void SetCtrlSCallback( bool imediate )
	{
		//MasterNode node = _mainGraphInstance.CurrentMasterNode;
		if ( /*node != null && node.CurrentShader != null && */m_shaderIsModified )
		{
			if ( imediate )
			{
				OnToolButtonPressed( ToolButtonType.Update );
			}
			else
			{
				m_ctrlSCallback = true;
			}
		}
	}

	public void SetSaveIsDirty()
	{
		m_saveIsDirty = true && UIUtils.DirtyMask;
	}

	public void OnPaletteNodeCreate( Type type, string name )
	{
		m_mainGraphInstance.DeSelectAll();
		m_paletteChosenType = type;
		m_palettePopup.Activate( name );
	}

	public void OnContextPaletteNodeCreate( Type type, string name )
	{
		m_mainGraphInstance.DeSelectAll();
		CreateNode( type, m_contextPalette.StartDropPosition * m_cameraZoom - m_cameraOffset );
		//CreateNode( type,  UIUtils.ValidReferences() ? ( m_contextPalette.CurrentPosition2D * m_cameraZoom - m_cameraOffset ) : TranformedMousePos );
	}

	void OnNodeStoppedMovingEvent( ParentNode node )
	{
		CheckZoomBoundaries( node.Vec2Position );
		ShaderIsModified = true;
	}

	void OnMaterialUpdated( MasterNode masterNode )
	{
		if ( masterNode != null )
		{
			if ( masterNode.CurrentMaterial )
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, ShaderIsModified ? 0 : 2, ShaderIsModified ? "Click to update Shader preview." : "Preview up-to-date." );
			}
			else
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
			}
			UpdateLiveUI();
		}
		else
		{
			m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
		}
	}

	void OnShaderUpdated( MasterNode masterNode )
	{
		m_toolsWindow.SetStateOnButton( ToolButtonType.OpenSourceCode, masterNode.CurrentShader != null ? 1 : 0 );
	}

	public void CheckZoomBoundaries( Vector2 newPosition )
	{
		if ( newPosition.x < m_minNodePos.x )
		{
			m_minNodePos.x = newPosition.x;
		}
		else if ( newPosition.x > m_maxNodePos.x )
		{
			m_maxNodePos.x = newPosition.x;
		}

		if ( newPosition.y < m_minNodePos.y )
		{
			m_minNodePos.y = newPosition.y;
		}
		else if ( newPosition.y > m_maxNodePos.y )
		{
			m_maxNodePos.y = newPosition.y;
		}
	}
	public void DestroyNode( ParentNode node ) { m_mainGraphInstance.DestroyNode( node ); }
	public ParentNode CreateNode( Type type, Vector2 position, bool selectNode = true )
	{
		ParentNode node = m_mainGraphInstance.CreateNode( type, true );
		Vector2 newPosition = position;
		node.Vec2Position = newPosition;
		CheckZoomBoundaries( newPosition );

		// Connect node if a wire is active 
		if ( UIUtils.ValidReferences() )
		{
			if ( UIUtils.InputPortReference.IsValid )
			{
				ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.InputPortReference.NodeId );
				OutputPort outputPort = node.GetFirstOutputPortOfType( UIUtils.InputPortReference.DataType, true );
				if ( outputPort != null && ( !UIUtils.InputPortReference.TypeLocked ||
											UIUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
											( UIUtils.InputPortReference.TypeLocked && outputPort.DataType == UIUtils.InputPortReference.DataType ) ) )
				{

					//link output to input
					if ( outputPort.ConnectTo( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
						node.OnOutputPortConnected( outputPort.PortId, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

					//link input to output
					if ( originNode.GetInputPortByUniqueId( UIUtils.InputPortReference.PortId ).ConnectTo( outputPort.NodeId, outputPort.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
						originNode.OnInputPortConnected( UIUtils.InputPortReference.PortId, node.UniqueId, outputPort.PortId );
				}
			}

			if ( UIUtils.OutputPortReference.IsValid )
			{
				ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.OutputPortReference.NodeId );
				InputPort inputPort = node.GetFirstInputPortOfType( UIUtils.OutputPortReference.DataType, true );

				if ( inputPort != null && ( !inputPort.TypeLocked ||
												inputPort.DataType == WirePortDataType.OBJECT ||
												( inputPort.TypeLocked && inputPort.DataType == UIUtils.OutputPortReference.DataType ) ) )
				{

					inputPort.InvalidateAllConnections();
					//link input to output
					if ( inputPort.ConnectTo( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
						node.OnInputPortConnected( inputPort.PortId, UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
					//link output to input

					if ( originNode.GetOutputPortByUniqueId( UIUtils.OutputPortReference.PortId ).ConnectTo( inputPort.NodeId, inputPort.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
						originNode.OnOutputPortConnected( UIUtils.OutputPortReference.PortId, node.UniqueId, inputPort.PortId );
				}
			}
			UIUtils.InvalidateReferences();

			for ( int i = 0; i < m_mainGraphInstance.VisibleNodes.Count; i++ )
			{
				m_mainGraphInstance.VisibleNodes[ i ].OnNodeInteraction( node );
			}
		}

		if ( selectNode )
			m_mainGraphInstance.SelectNode( node, false, false );
		//_repaintIsDirty = true;
		ForceRepaint();
		return node;
	}

	public void UpdateTime()
	{
		double deltaTime = Time.realtimeSinceStartup - m_time;
		m_time = Time.realtimeSinceStartup;


		if ( m_cachedProjectInLinearId == -1 )
			m_cachedProjectInLinearId = Shader.PropertyToID( "_ProjectInLinear" );

		if ( m_cachedEditorTimeId == -1 )
			m_cachedEditorTimeId = Shader.PropertyToID( "_EditorTime" );

		if ( m_cachedEditorDeltaTimeId == -1 )
			m_cachedEditorDeltaTimeId = Shader.PropertyToID( "_EditorDeltaTime" );

		Shader.SetGlobalFloat( "_ProjectInLinear", ( float ) ( PlayerSettings.colorSpace == ColorSpace.Linear ? 1 : 0 ) );
		Shader.SetGlobalFloat( "_EditorTime", ( float ) m_time );
		Shader.SetGlobalFloat( "_EditorDeltaTime", ( float ) deltaTime );
	}

	public void UpdateNodePreviewList()
	{
		UIUtils.CheckNullMaterials();

		for ( int i = 0; i < CurrentGraph.AllNodes.Count; i++ )
		{
			ParentNode node = CurrentGraph.AllNodes[ i ];
			if ( node != null )
			{
				node.RenderNodePreview();
			}
		}

		Repaint();
	}

	public void ForceRepaint()
	{
		m_repaintCount += 1;
		Repaint();
	}

	public void ForceUpdateFromMaterial() { m_forceUpdateFromMaterialFlag = true; }
	void UseCurrentEvent()
	{
		m_currentEvent.Use();
	}

	public void OnBeforeSerialize()
	{
		m_mainGraphInstance.DeSelectAll();
		if ( DebugConsoleWindow.UseShaderPanelsInfo )
		{
			if ( m_nodeParametersWindow != null )
				m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized;

			if ( m_paletteWindow != null )
				m_paletteWindowMaximized = m_paletteWindow.IsMaximized;
		}
	}


	public void OnAfterDeserialize()
	{
		if ( DebugConsoleWindow.UseShaderPanelsInfo )
		{
			if ( m_nodeParametersWindow != null )
				m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;

			if ( m_paletteWindow != null )
				m_paletteWindow.IsMaximized = m_paletteWindowMaximized;
		}
	}

	void OnDestroy()
	{
		m_ctrlSCallback = false;
		Destroy();
	}

	void OnDisable()
	{
		m_ctrlSCallback = false;
		EditorApplication.update -= UpdateTime;
		EditorApplication.update -= UpdateNodePreviewList;
	}

	void OnEmptyGraphDetected( ParentGraph graph )
	{
		if ( m_delayedLoadObject != null )
		{
			LoadObject( m_delayedLoadObject );
			m_delayedLoadObject = null;
			Repaint();
		}
		else
		{
			string lastOpenedObj = EditorPrefs.GetString( IOUtils.LAST_OPENED_OBJ_ID );
			if ( !string.IsNullOrEmpty( lastOpenedObj ) )
			{
				Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( lastOpenedObj );
				if ( shader == null )
				{
					Material material = AssetDatabase.LoadAssetAtPath<Material>( lastOpenedObj );
					if ( material != null )
					{
						LoadDroppedObject( true, material.shader, material );
					}
				}
				else
				{
					LoadDroppedObject( true, shader, null );
				}
				Repaint();
			}
		}
	}


	public void ForceMaterialsToUpdate( ref Dictionary<string, string> availableMaterials )
	{
		m_forcingMaterialUpdateOp = true;
		m_forcingMaterialUpdateFlag = true;
		m_materialsToUpdate.Clear();
		foreach ( KeyValuePair<string, string> kvp in availableMaterials )
		{
			Material material = AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( kvp.Value ) );
			if ( material != null )
			{
				m_materialsToUpdate.Add( material );
			}
		}
	}
	public Vector2 TranformPosition( Vector2 pos )
	{
		return pos * m_cameraZoom - m_cameraOffset;
	}

	public ParentGraph CurrentGraph { get { return m_mainGraphInstance; } }
	public void RefreshAvaibleNodes() { if ( m_contextMenu != null && m_mainGraphInstance != null ) m_contextMenu.RefreshNodes( m_mainGraphInstance ); }

	public bool ShaderIsModified
	{
		get { return m_shaderIsModified; }
		set
		{
			m_shaderIsModified = value && UIUtils.DirtyMask;
			//if ( _shaderIsModified && !Application.isPlaying )
			//{
			//	EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
			//	EditorUtility.SetDirty( Shader.Find( "SimpleColor" ));
			//}

			m_toolsWindow.SetStateOnButton( ToolButtonType.Save, m_shaderIsModified ? 1 : 0 );
			MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
			if ( masterNode != null )
			{
				if ( masterNode.CurrentMaterial )
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 0 : 2 );
				}
				else
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 1 : 2 );
				}
			}
			else
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1 );
			}
		}
	}
	public void RequestSave() { m_markedToSave = true; }
	public void RequestRepaint() { m_repaintIsDirty = true; }
	public CustomStylesContainer CustomStylesInstance { get { return m_customStyles; } }
	public OptionsWindow Options { get { return m_optionsWindow; } }
	public GraphContextMenu ContextMenuInstance { get { return m_contextMenu; } }
	public ShortcutsManager ShortcutManagerInstance { get { return m_shortcutManager; } }
	public bool ToggleDebug
	{
		get { return m_toggleDebug; }
		set { m_toggleDebug = value; }
	}

	public bool GlobalPreview
	{
		get { return m_globalPreview; }
		set { m_globalPreview = value; }
	}

	public double EditorTime
	{
		get { return m_time; }
		set { m_time = value; }
	}


	public bool ExpandedStencil
	{
		get { return m_expandedStencil; }
		set { m_expandedStencil = value; }
	}

	public bool ExpandedTesselation
	{
		get { return m_expandedTesselation; }
		set { m_expandedTesselation = value; }
	}

	public bool ExpandedDepth
	{
		get { return m_expandedDepth; }
		set { m_expandedDepth = value; }
	}

	public bool ExpandedRenderingPlatforms
	{
		get { return m_expandedRenderingPlatforms; }
		set { m_expandedRenderingPlatforms = value; }
	}

	public bool ExpandedRenderingOptions
	{
		get { return m_expandedRenderingOptions; }
		set { m_expandedRenderingOptions = value; }
	}

	public bool ExpandedProperties
	{
		get { return m_expandedProperties; }
		set { m_expandedProperties = value; }
	}

	//public Material MaskingMaterial
	//{
	//	get
	//	{
	//		if ( m_maskingMaterial == null )
	//		{
	//			m_maskingMaterial = new Material( AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "9c34f18ebe2be3e48b201b748c73dec0" ) ) );
	//		}
	//		return m_maskingMaterial;
	//	}
	//	set { m_maskingMaterial = value; }
	//}

	public PaletteWindow CurrentPaletteWindow { get { return m_paletteWindow; } }
	public PreMadeShaders PreMadeShadersInstance { get { return m_preMadeShaders; } }
	public Rect CameraInfo { get { return m_cameraInfo; } }
	public Vector2 TranformedMousePos { get { return m_currentMousePos2D * m_cameraZoom - m_cameraOffset; } }
	public Vector2 TranformedKeyEvtMousePos { get { return m_keyEvtMousePos2D * m_cameraZoom - m_cameraOffset; } }
	public PalettePopUp PalettePopUpInstance { get { return m_palettePopup; } }
	public float AvailableCanvasWidth { get { return ( m_cameraInfo.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ); } }
	public float AvailableCanvasHeight { get { return ( m_cameraInfo.height ); } }
	public DuplicatePreventionBuffer DuplicatePrevBufferInstance { get { return m_duplicatePreventionBuffer; } }
	public string LastOpenedLocation { get { return m_lastOpenedLocation; } }

	public float CameraZoom
	{
		get { return m_cameraZoom; }
		set
		{
			m_cameraZoom = value;
			m_zoomChanged = true;
		}
	}

	public bool ForceAutoPanDir
	{
		get { return m_forceAutoPanDir; }
		set { m_forceAutoPanDir = value; }
	}

	public ASESelectionMode CurrentSelection
	{
		get { return m_selectionMode; }
		set
		{
			m_selectionMode = value;
			m_toolsWindow.BorderStyle = ( m_selectionMode == ASESelectionMode.Material ) ? UIUtils.GetCustomStyle( CustomStyle.MaterialBorder ) : UIUtils.GetCustomStyle( CustomStyle.ShaderBorder );
		}
	}
	public NodeParametersWindow ParametersWindow { get { return m_nodeParametersWindow; } }
	public void MarkToRepaint() { m_repaintIsDirty = true; }
	public UnityEngine.Object DelayedObjToLoad { set { m_delayedLoadObject = value; } }
	public int CurrentVersion { get { return m_versionInfo.FullNumber; } }
	public DrawInfo CameraDrawInfo { get { return m_drawInfo; } }
	public NodeExporterUtils CurrentNodeExporterUtils { get { return m_nodeExporterUtils; } }
}
