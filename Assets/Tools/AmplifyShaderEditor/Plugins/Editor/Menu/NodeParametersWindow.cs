// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class NodeParametersWindow : MenuParent
	{
		private int m_lastSelectedNode = -1;
		private const string TitleStr = "Node Properties";
		private GUIStyle m_nodePropertiesStyle;

		// width and height are between [0,1] and represent a percentage of the total screen area
		public NodeParametersWindow() : base( 0, 0, 250, 0, string.Empty, MenuAnchor.TOP_LEFT, MenuAutoSize.MATCH_VERTICAL )
		{
			SetMinimizedArea( -225, 0, 260, 0 );
		}

		public bool Draw( Rect parentPosition, ParentNode selectedNode, Vector2 mousePosition, int mouseButtonId ,bool hasKeyboardFocus )
		{
			bool changeCheck = false;
			base.Draw( parentPosition, mousePosition, mouseButtonId , hasKeyboardFocus );
			if ( m_nodePropertiesStyle == null )
			{
				m_nodePropertiesStyle = UIUtils.GetCustomStyle( CustomStyle.NodePropertiesTitle );
				m_nodePropertiesStyle.normal.textColor = m_nodePropertiesStyle.active.textColor = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f ) : new Color( 0f, 0f, 0f );
			}

			if ( m_isMaximized )
			{
				KeyCode key = Event.current.keyCode;
				if ( m_isMouseInside || hasKeyboardFocus )
				{
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
				}


				GUILayout.BeginArea( m_transformedArea, m_content, m_style );
				{
					//Draw selected node parameters
					if ( selectedNode != null )
					{
						// this hack is need because without it the several FloatFields/Textfields/... would show wrong values ( different from the ones they were assigned to show )
						if ( m_lastSelectedNode != selectedNode.UniqueId )
						{
							m_lastSelectedNode = selectedNode.UniqueId;
							GUI.FocusControl( "" );
						}

						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.Separator();
							EditorGUILayout.LabelField( TitleStr, m_nodePropertiesStyle );
							EditorGUILayout.Separator();
							UIUtils.RecordObject( selectedNode , "Changing properties on node " + selectedNode.UniqueId);
							m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
							float labelWidth = EditorGUIUtility.labelWidth;
							if ( selectedNode.TextLabelWidth > 0 )
								EditorGUIUtility.labelWidth = selectedNode.TextLabelWidth;

							changeCheck = selectedNode.SafeDrawProperties();
							EditorGUIUtility.labelWidth = labelWidth;
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndVertical();

						if ( changeCheck )
						{
							if ( selectedNode.ConnStatus == NodeConnectionStatus.Connected )
								UIUtils.CurrentWindow.SetSaveIsDirty();
						}
					}
				}
				// Close window area
				GUILayout.EndArea();
			}
			
			PostDraw();
			return changeCheck;
		}
	}
}
