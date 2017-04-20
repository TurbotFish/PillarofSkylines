// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public class ShortcutItem
	{
		public delegate void ShortcutFunction();
		public ShortcutFunction MyKeyDownFunctionPtr;
		public ShortcutFunction MyKeyUpFunctionPtr;
		public string Name;
		public string Description;

		public ShortcutItem( string name, string description )
		{
			Name = name;
			Description = description;
		}

		public ShortcutItem( string name, string description, ShortcutFunction myKeyDownFunctionPtr, ShortcutFunction myKeyUpFunctionPtr = null )
		{
			Name = name;
			Description = description;
			MyKeyDownFunctionPtr = myKeyDownFunctionPtr;
			MyKeyUpFunctionPtr = myKeyUpFunctionPtr;
		}

		public void Destroy()
		{
			MyKeyDownFunctionPtr = null;
			MyKeyUpFunctionPtr = null;
		}
	}

	public class ShortcutsManager
	{
		public static readonly KeyCode ScrollUpKey = KeyCode.PageUp;
		public static readonly KeyCode ScrollDownKey = KeyCode.PageDown;


		private const string ItemWikiFormat = "*<b>[{0}]:</b> {1}\n";
		private Dictionary<KeyCode, ShortcutItem> m_editorShortcutsDict = new Dictionary<KeyCode, ShortcutItem>();
		private List<ShortcutItem> m_editorShortcutsList = new List<ShortcutItem>();

		private Dictionary<KeyCode, ShortcutItem> m_nodesShortcutsDict = new Dictionary<KeyCode, ShortcutItem>();
		private List<ShortcutItem> m_nodesShortcutsList = new List<ShortcutItem>();

		public void DumpShortcutsToDisk( string pathname )
		{
			if ( !System.IO.Directory.Exists( pathname ) )
			{
				System.IO.Directory.CreateDirectory( pathname );
			}

			string list = "=== Full Shortcut List ===\n";
			list += "==== Editor ====\n";
			for ( int i = 0; i < m_editorShortcutsList.Count; i++ )
			{
				list += string.Format( ItemWikiFormat, m_editorShortcutsList[ i ].Name, m_editorShortcutsList[ i ].Description );
			}
			list += "\n";
			list += "==== Nodes ====\n";
			for ( int i = 0; i < m_nodesShortcutsList.Count; i++ )
			{
				list += string.Format( ItemWikiFormat, m_nodesShortcutsList[ i ].Name, m_nodesShortcutsList[ i ].Description );
			}

			string shortcutsPathnames = pathname + "KeyboardShortcuts.txt";
			Debug.Log( " Creating shortcuts file at " + shortcutsPathnames );
			IOUtils.SaveTextfileToDisk( list, shortcutsPathnames, false );
		}

		public void RegisterNodesShortcuts( KeyCode key, string nodeName )
		{
			if ( m_nodesShortcutsDict.ContainsKey( key ) )
			{
				if ( DebugConsoleWindow.DeveloperMode )
				{
					Debug.Log( "Attempting to register an already used node shortcut key " + key );
				}
				return;
			}
			m_nodesShortcutsDict.Add( key, new ShortcutItem( key.ToString(), nodeName ) );
			m_nodesShortcutsList.Add( m_nodesShortcutsDict[ key ] );
		}

		public void RegisterEditorShortcut( KeyCode key, string description, ShortcutItem.ShortcutFunction myKeyDownFunctionPtr, ShortcutItem.ShortcutFunction myKeyUpFunctionPtr = null )
		{
			if ( m_editorShortcutsDict.ContainsKey( key ) )
			{
				if ( DebugConsoleWindow.DeveloperMode )
				{
					Debug.Log( "Attempting to register an already used editor shortcut key " + key );
				}
				return;
			}

			m_editorShortcutsDict.Add( key, new ShortcutItem( key.ToString(), description, myKeyDownFunctionPtr, myKeyUpFunctionPtr ) );
			m_editorShortcutsList.Add( m_editorShortcutsDict[ key ] );
		}

		public bool ActivateShortcut( KeyCode key, bool isKeyDown )
		{
			if ( m_editorShortcutsDict.ContainsKey( key ) )
			{
				if ( isKeyDown )
				{
					if ( m_editorShortcutsDict[ key ].MyKeyDownFunctionPtr != null )
					{
						m_editorShortcutsDict[ key ].MyKeyDownFunctionPtr();
						return true;
					}
				}
				else
				{
					if ( m_editorShortcutsDict[ key ].MyKeyUpFunctionPtr != null )
					{
						m_editorShortcutsDict[ key ].MyKeyUpFunctionPtr();
						return true;
					}
				}
			}
			return false;
		}

		public void Destroy()
		{
			foreach ( KeyValuePair<KeyCode, ShortcutItem> kvp in m_editorShortcutsDict )
			{
				kvp.Value.Destroy();
			}
			m_editorShortcutsDict.Clear();
			m_editorShortcutsDict = null;

			m_editorShortcutsList.Clear();
			m_editorShortcutsList = null;

			m_nodesShortcutsDict.Clear();
			m_nodesShortcutsDict = null;

			m_nodesShortcutsList.Clear();
			m_nodesShortcutsList = null;
		}

		public Dictionary<KeyCode, ShortcutItem> AvailableEditorShortcuts { get { return m_editorShortcutsDict; } }
		public List<ShortcutItem> AvailableEditorShortcutsList { get { return m_editorShortcutsList; } }

		public Dictionary<KeyCode, ShortcutItem> AvailableNodeShortcuts { get { return m_nodesShortcutsDict; } }
		public List<ShortcutItem> AvailableNodesShortcutsList { get { return m_nodesShortcutsList; } }
	}
}
