// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class ShortcutKeyData
	{
		public bool IsPressed;
		public Type NodeType;
		public string Name;
		public ShortcutKeyData( Type type , string name )
		{
			NodeType = type;
			Name = name;
			IsPressed = false;
		}
	}

	public class GraphContextMenu
	{
		private List<ContextMenuItem> m_items;
		private Dictionary<Type, NodeAttributes> m_itemsDict;
		private Dictionary<Type, NodeAttributes> m_deprecatedItemsDict;
		private Dictionary<Type, Type> m_castTypes;
		private Dictionary<KeyCode, ShortcutKeyData> m_shortcutTypes;
		private GenericMenu m_menu;
		private Vector2 m_scaledClickedPos;
		private KeyCode m_lastKeyPressed;

		public GraphContextMenu( ParentGraph currentGraph )
		{
			RefreshNodes( currentGraph );
		}

		public void RefreshNodes( ParentGraph currentGraph )
		{
			if ( m_items != null )
				m_items.Clear();

			m_items = new List<ContextMenuItem>();

			if ( m_itemsDict != null )
				m_itemsDict.Clear();

			m_itemsDict = new Dictionary<Type, NodeAttributes>();

			if ( m_deprecatedItemsDict != null )
				m_deprecatedItemsDict.Clear();

			m_deprecatedItemsDict = new Dictionary<Type, NodeAttributes>();

			if ( m_castTypes != null )
				m_castTypes.Clear();

			m_castTypes = new Dictionary<Type, Type>();

			if ( m_shortcutTypes != null )
				m_shortcutTypes.Clear();

			m_shortcutTypes = new Dictionary<KeyCode, ShortcutKeyData>();

			m_lastKeyPressed = KeyCode.None;

			// Fetch all available nodes by their attributes
			IEnumerable<Type> availableTypes = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany( type => type.GetTypes() );
			foreach ( Type type in availableTypes )
			{
				foreach ( NodeAttributes attribute in Attribute.GetCustomAttributes( type ).OfType<NodeAttributes>() )
				{
					if ( attribute.Available && !attribute.Deprecated )
					{
						if ( !UIUtils.HasColorCategory( attribute.Category ) )
						{
							if ( !String.IsNullOrEmpty( attribute.CustomCategoryColor ) )
							{
								try
								{
									Color color = new Color();
									ColorUtility.TryParseHtmlString( attribute.CustomCategoryColor, out color );
									UIUtils.AddColorCategory( attribute.Category, color );
								}
								catch ( Exception e )
								{
									Debug.LogException( e );
									UIUtils.AddColorCategory( attribute.Category, Constants.DefaultCategoryColor );
								}
							}
							else
							{
								UIUtils.AddColorCategory( attribute.Category, Constants.DefaultCategoryColor );
							}
						}

						if ( attribute.CastType != null && attribute.CastType.Length > 0 && type != null )
						{
							for ( int i = 0; i < attribute.CastType.Length; i++ )
							{
								m_castTypes.Add( attribute.CastType[ i ], type );
							}
						}

						if ( attribute.ShortcutKey != KeyCode.None && type != null )
							m_shortcutTypes.Add( attribute.ShortcutKey, new ShortcutKeyData( type, attribute.Name ) );

						m_itemsDict.Add( type, attribute );
						m_items.Add( new ContextMenuItem( attribute, type, attribute.Name, attribute.Category, attribute.Description, attribute.ShortcutKey, ( Type newType, Vector2 position ) =>
						{
							ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( newType );
							if ( newNode != null )
							{
								newNode.Vec2Position = position;
								currentGraph.AddNode( newNode, true );
								if ( UIUtils.InputPortReference.IsValid )
								{
									OutputPort port = newNode.GetFirstOutputPortOfType( UIUtils.InputPortReference.DataType, true );
									if ( port != null )
									{
										port.ConnectTo( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked );
										UIUtils.GetNode( UIUtils.InputPortReference.NodeId ).GetInputPortByUniqueId( UIUtils.InputPortReference.PortId ).ConnectTo( port.NodeId, port.PortId, port.DataType, UIUtils.InputPortReference.TypeLocked );
									}
								}
								if ( UIUtils.OutputPortReference.IsValid )
								{
									InputPort port = newNode.GetFirstInputPortOfType( UIUtils.OutputPortReference.DataType, true );
									if ( port != null )
									{
										port.ConnectTo( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId, UIUtils.OutputPortReference.DataType, port.TypeLocked );
										UIUtils.GetNode( UIUtils.OutputPortReference.NodeId ).GetOutputPortByUniqueId( UIUtils.OutputPortReference.PortId ).ConnectTo( port.NodeId, port.PortId, port.DataType, port.TypeLocked );
									}
								}
								UIUtils.InvalidateReferences();
							}
							return newNode;
						} ) );
					}
					else
					{
						m_deprecatedItemsDict.Add( type, attribute );
					}
				}
			}

			//Sort out the final list by name
			m_items.Sort( ( ContextMenuItem item0, ContextMenuItem item1 ) => { return item0.Name.CompareTo( item1.Name ); } );

			// Add them to the context menu
			m_menu = new GenericMenu();
			foreach ( ContextMenuItem item in m_items )
			{
				//The / on the GUIContent creates categories on the context menu
				m_menu.AddItem( new GUIContent( item.Category + "/" + item.Name, item.Description ), false, OnItemSelected, item );
			}
		}

		public void Destroy()
		{
			for ( int i = 0; i < m_items.Count; i++ )
			{
				m_items[ i ].Destroy();
			}
			m_items.Clear();
			m_items = null;

			m_itemsDict.Clear();
			m_itemsDict = null;

			m_deprecatedItemsDict.Clear();
			m_deprecatedItemsDict = null;

			m_castTypes.Clear();
			m_castTypes = null;

			m_shortcutTypes.Clear();
			m_shortcutTypes = null;

			m_menu = null;
		}

		public NodeAttributes GetNodeAttributesForType( Type type )
		{
			if ( type == null )
			{
				Debug.LogError( "Invalid type detected" );
				return null;
			}

			if ( m_itemsDict.ContainsKey( type ) )
				return m_itemsDict[ type ];
			return null;
		}

		public NodeAttributes GetDeprecatedNodeAttributesForType( Type type )
		{
			if ( m_deprecatedItemsDict.ContainsKey( type ) )
				return m_deprecatedItemsDict[ type ];
			return null;
		}

		public void UpdateKeyPress( KeyCode key )
		{
			if ( key == KeyCode.None )
				return;

			m_lastKeyPressed = key;
			if ( m_shortcutTypes.ContainsKey( key ) )
			{
				m_shortcutTypes[ key ].IsPressed = true;
			}
		}

		public void UpdateKeyReleased( KeyCode key )
		{
			if ( key == KeyCode.None )
				return;

			if ( m_shortcutTypes.ContainsKey( key ) )
			{
				m_shortcutTypes[ key ].IsPressed = false;
			}
		}

		public ParentNode CreateNodeFromCastType( Type type )
		{
			if ( m_castTypes.ContainsKey( type ) )
			{
				ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( m_castTypes[ type ] );
				return newNode;
			}
			return null;
		}

		public ParentNode CreateNodeFromShortcut( KeyCode key )
		{
			if ( key == KeyCode.None )
				return null;

			if ( m_shortcutTypes.ContainsKey( key ) )
			{
				ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( m_shortcutTypes[ key ].NodeType );
				return newNode;
			}
			return null;
		}

		public ParentNode CreateNodeFromShortcutKey()
		{
			if ( m_lastKeyPressed == KeyCode.None )
				return null;

			if ( m_shortcutTypes.ContainsKey( m_lastKeyPressed ) && m_shortcutTypes[ m_lastKeyPressed ].IsPressed )
			{
				ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( m_shortcutTypes[ m_lastKeyPressed ].NodeType );
				return newNode;
			}
			return null;
		}

		public bool CheckShortcutKey()
		{
			if ( m_lastKeyPressed == KeyCode.None )
				return false;

			if ( m_shortcutTypes.ContainsKey( m_lastKeyPressed ) && m_shortcutTypes[ m_lastKeyPressed ].IsPressed )
			{
				return true;
			}
			return false;
		}

		void OnItemSelected( object args )
		{
			ContextMenuItem item = ( ContextMenuItem ) args;
			if ( item != null )
				item.CreateNodeFuncPtr( item.NodeType, m_scaledClickedPos );
		}

		public void Show( Vector2 globalClickedPos, Vector2 cameraOffset, float cameraZoom )
		{
			m_scaledClickedPos = globalClickedPos * cameraZoom - cameraOffset;
			m_menu.ShowAsContext();
		}

		public List<ContextMenuItem> MenuItems
		{
			get { return m_items; }
		}

		public KeyCode LastKeyPressed
		{
			get { return m_lastKeyPressed; }
		}

		public Dictionary<KeyCode, ShortcutKeyData> NodeShortcuts { get { return m_shortcutTypes; } }
	}
}
