// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public sealed class PortLegendInfo : EditorWindow
	{
	
		private const string NoASEWindowWarning = "Please Open the ASE to get access to shortcut info";
		private const float PixelSeparator = 5;
		private const string EditorShortcutsTitle = "Editor Shortcuts";
		private const string MenuShortcutsTitle = "Menu Shortcuts";
		private const string NodesShortcutsTitle = "Nodes Shortcuts";
		private const string PortShortcutsTitle = "Port Shortcuts";
		private const string PortLegendTitle = "Port Legend";

		private const string KeyboardUsageTemplate = "[{0}] - {1}";
		private const string m_lockedStr = "Locked Port";
		
		private const float WindowSizeX = 250;
		private const float WindowSizeY = 300;
		private const float WindowPosX = 5;
		private const float WindowPosY = 5;

		private Rect m_availableArea;

		private bool m_portAreaFoldout = true;
		private bool m_editorShortcutAreaFoldout = true;
		private bool m_menuShortcutAreaFoldout = true;
		private bool m_nodesShortcutAreaFoldout = true;

		private Vector2 m_currentScrollPos;

		private GUIStyle m_portStyle;
		private GUIStyle m_labelStyleBold;
		private GUIStyle m_labelStyle;

		private GUIContent m_content = new GUIContent( "Helper", "Shows helper info for ASE users" );
		private bool m_init = true;

		private List<ShortcutItem> m_editorShortcuts = null;
		private List<ShortcutItem> m_nodesShortcuts = null;

		public static PortLegendInfo OpenWindow()
		{
			PortLegendInfo currentWindow = ( PortLegendInfo ) PortLegendInfo.GetWindow( typeof( PortLegendInfo ), false );
			currentWindow.minSize = new Vector2( WindowSizeX, WindowSizeY );
			currentWindow.maxSize = new Vector2( WindowSizeX, 2 * WindowSizeY ); ;
			currentWindow.wantsMouseMove = true;
			return currentWindow;
		}

		public void Init()
		{
			m_init = false;
			wantsMouseMove = false;
			titleContent = m_content;
			m_portStyle = new GUIStyle( UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon ) );
			m_portStyle.alignment = TextAnchor.MiddleLeft;
			m_portStyle.imagePosition = ImagePosition.ImageOnly;
			m_portStyle.margin = new RectOffset( 5, 0, 5, 0 );

			m_labelStyleBold = new GUIStyle( UIUtils.GetCustomStyle( CustomStyle.InputPortlabel ) );
			m_labelStyleBold.fontStyle = FontStyle.Bold;
			m_labelStyleBold.fontSize = ( int ) ( Constants.TextFieldFontSize );


			m_labelStyle = new GUIStyle( UIUtils.GetCustomStyle( CustomStyle.InputPortlabel ) );
			m_labelStyle.clipping = TextClipping.Overflow;
			m_labelStyle.imagePosition = ImagePosition.TextOnly;
			m_labelStyle.contentOffset = new Vector2( -10, 0 );
			m_labelStyle.fontSize = ( int ) ( Constants.TextFieldFontSize );

			if ( !EditorGUIUtility.isProSkin )
			{
				m_labelStyleBold.normal.textColor = m_labelStyle.normal.textColor = Color.black;
			}

			m_availableArea = new Rect( WindowPosX, WindowPosY, WindowSizeX - 2 * WindowPosX, WindowSizeY - 2 * WindowPosY );
		}

		void DrawPort( WirePortDataType type )
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUI.color = UIUtils.GetColorForDataType( type, false );
				GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
				GUI.color = Color.white;
				EditorGUILayout.LabelField( UIUtils.GetNameForDataType( type ), m_labelStyle );
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}

		void OnGUI()
		{
			if ( !UIUtils.Initialized || UIUtils.CurrentWindow == null )
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
				return;
			}

			if ( m_init )
			{
				Init();
			}

			KeyCode key = Event.current.keyCode;
			if ( key == ShortcutsManager.ScrollUpKey )
			{
				m_currentScrollPos.y -= 10;
				if ( m_currentScrollPos.y < 0 )
				{
					m_currentScrollPos.y = 0;
				}
				Event.current.Use();
			}

			if ( key == ShortcutsManager.ScrollDownKey )
			{
				m_currentScrollPos.y += 10;
				Event.current.Use();
			}

			if ( Event.current.type == EventType.MouseDrag && Event.current.button > 0 )
			{
				m_currentScrollPos.x += Constants.MenuDragSpeed* Event.current.delta.x;
				if ( m_currentScrollPos.x < 0 )
				{
					m_currentScrollPos.x = 0;
				}

				m_currentScrollPos.y += Constants.MenuDragSpeed * Event.current.delta.y;
				if ( m_currentScrollPos.y < 0 )
				{
					m_currentScrollPos.y = 0;
				}
			}
			
			m_availableArea = new Rect( WindowPosX, WindowPosY, position.width - 2 * WindowPosX, position.height - 2 * WindowPosY );
			GUILayout.BeginArea( m_availableArea );
			{
				m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
				{
					EditorGUILayout.BeginVertical();
					{
						NodeUtils.DrawPropertyGroup( ref m_portAreaFoldout,PortLegendTitle, DrawPortInfo );
						float currLabelWidth = EditorGUIUtility.labelWidth;
						EditorGUIUtility.labelWidth = 1;
						NodeUtils.DrawPropertyGroup( ref m_editorShortcutAreaFoldout, EditorShortcutsTitle, DrawEditorShortcuts );
						NodeUtils.DrawPropertyGroup( ref m_menuShortcutAreaFoldout, MenuShortcutsTitle, DrawMenuShortcuts );
						NodeUtils.DrawPropertyGroup( ref m_nodesShortcutAreaFoldout, NodesShortcutsTitle, DrawNodesShortcuts );
						EditorGUIUtility.labelWidth = currLabelWidth;
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();
			}
			GUILayout.EndArea();
				
		}

		void DrawPortInfo()
		{
			Color originalColor = GUI.color;

			DrawPort( WirePortDataType.OBJECT );
			DrawPort( WirePortDataType.INT );
			DrawPort( WirePortDataType.FLOAT );
			DrawPort( WirePortDataType.FLOAT2 );
			DrawPort( WirePortDataType.FLOAT3 );
			DrawPort( WirePortDataType.FLOAT4 );
			DrawPort( WirePortDataType.COLOR );
			DrawPort( WirePortDataType.SAMPLER2D );
			DrawPort( WirePortDataType.FLOAT3x3 );
			DrawPort( WirePortDataType.FLOAT4x4 );

			EditorGUILayout.BeginHorizontal();
			{
				GUI.color = Constants.LockedPortColor;
				GUILayout.Box( string.Empty, m_portStyle, GUILayout.Width( UIUtils.PortsSize.x ), GUILayout.Height( UIUtils.PortsSize.y ) );
				GUI.color = Color.white;
				EditorGUILayout.LabelField( m_lockedStr, m_labelStyle );
			}
			EditorGUILayout.EndHorizontal();

			GUI.color = originalColor;
		}

		public void DrawEditorShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_editorShortcuts == null )
				{
					m_editorShortcuts = window.ShortcutManagerInstance.AvailableEditorShortcutsList;
				}
				
				EditorGUI.indentLevel--;
				int count = m_editorShortcuts.Count;
				for ( int i = 0; i < count; i++ )
				{
					DrawItem( m_editorShortcuts[ i ].Name, m_editorShortcuts[ i ].Description );
				}
				DrawItem( "LMB Drag", "Box selection" );
				DrawItem( "RMB Drag", "Camera Pan" );
				DrawItem( "Alt + RMB Drag", "Scroll Menu" );
				EditorGUI.indentLevel++;
				
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		public void DrawMenuShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				EditorGUI.indentLevel--;
				DrawItem( ShortcutsManager.ScrollUpKey.ToString(), "Scroll Up Menu" );
				DrawItem( ShortcutsManager.ScrollDownKey.ToString(), "Scroll Down Menu" );
				DrawItem( "RMB Drag", "Scroll Menu" );
				EditorGUI.indentLevel++;
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}
		void DrawItem( string name, string description )
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( name, m_labelStyleBold );
			EditorGUILayout.LabelField( description, m_labelStyle );
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( PixelSeparator );
		}

		public void DrawNodesShortcuts()
		{
			AmplifyShaderEditorWindow window = UIUtils.CurrentWindow;
			if ( window != null )
			{
				if ( m_nodesShortcuts == null )
				{
					m_nodesShortcuts = window.ShortcutManagerInstance.AvailableNodesShortcutsList;
				}
				
				EditorGUI.indentLevel--;
				int count = m_nodesShortcuts.Count;
				for ( int i = 0; i < count; i++ )
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField( m_nodesShortcuts[ i ].Name, m_labelStyleBold );
					EditorGUILayout.LabelField( m_nodesShortcuts[ i ].Description, m_labelStyle );
					EditorGUILayout.EndHorizontal();
					GUILayout.Space( PixelSeparator );
				}
				EditorGUI.indentLevel++;
			}
			else
			{
				EditorGUILayout.LabelField( NoASEWindowWarning );
			}
		}

		private void OnDestroy()
		{
			m_nodesShortcuts = null;
			m_editorShortcuts = null;
			m_portStyle = null;
			m_labelStyle = null;
			m_init = false;
		}
	}
}
