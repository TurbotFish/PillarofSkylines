// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class ShaderEditorModeWindow : MenuParent
	{
		private readonly Color OverallColorOn = new Color( 1f, 1f, 1f, 0.9f );
		private readonly Color OverallColorOff = new Color( 1f, 1f, 1f, 0.3f );
		private readonly Color FontColorOff = new Color( 1f, 1f, 1f, 0.4f );
		private const float _deltaY = 15;
		private const float _deltaX = 10;

		private const float CollSizeX = 180;
		private const float CollSizeY = 70;

		private const string m_currMatStr = "MATERIAL";
		private const string m_currShaderStr = "SHADER";

		private const string m_noMaterialStr = "No Material";
		private const string m_noShaderStr = "No Shader";

		private bool m_init = true;
		private GUIStyle m_materialLabelStyle;
		private GUIStyle m_shaderLabelStyle;

		private GUIContent m_materialContent;
		private GUIContent m_shaderContent;

		public ShaderEditorModeWindow() : base( 0, 0, 0, 0, "ShaderEditorModeWindow", MenuAnchor.BOTTOM_CENTER, MenuAutoSize.NONE ) { }

		public void ConfigStyle( GUIStyle style )
		{
			style.normal.textColor = FontColorOff;
			style.hover.textColor = FontColorOff;
			style.active.textColor = FontColorOff;
			style.focused.textColor = FontColorOff;

			style.onNormal.textColor = FontColorOff;
			style.onHover.textColor = FontColorOff;
			style.onActive.textColor = FontColorOff;
			style.onFocused.textColor = FontColorOff;
		}


		public void Draw( Rect _graphArea, Vector2 mousePos, Shader currentShader, Material currentMaterial, float usableArea, float leftPos, float rightPos )
		{
			if ( m_init )
			{
				m_init = false;
				GUIStyle shaderModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderModeTitle );
				GUIStyle shaderModeNoShader = UIUtils.GetCustomStyle( CustomStyle.ShaderModeNoShader );
				GUIStyle materialModeTitle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
				GUIStyle shaderNoMaterialModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderNoMaterialModeTitle );

				ConfigStyle( shaderModeTitle );
				ConfigStyle( shaderModeNoShader );
				ConfigStyle( materialModeTitle );
				ConfigStyle( shaderNoMaterialModeTitle );

				m_materialLabelStyle = new GUIStyle( shaderNoMaterialModeTitle );
				m_materialLabelStyle.contentOffset = new Vector2( m_materialLabelStyle.contentOffset.x, -m_materialLabelStyle.contentOffset.y );
				m_materialLabelStyle.fontSize += 6;

				m_shaderLabelStyle = new GUIStyle( shaderModeTitle );
				m_shaderLabelStyle.contentOffset = new Vector2( m_shaderLabelStyle.contentOffset.x, -m_shaderLabelStyle.contentOffset.y );
				m_shaderLabelStyle.fontSize += 6;

				m_materialContent = new GUIContent( m_currMatStr, "Select current material on Project view" );
				m_shaderContent = new GUIContent( m_currShaderStr, "Select current shader on Project view" );
			}
			Color buffereredColor = GUI.color;
			//Shader Mode
			{
				GUIStyle style = UIUtils.GetCustomStyle( currentShader == null ? CustomStyle.ShaderModeNoShader : CustomStyle.ShaderModeTitle );
				Texture2D shaderTex = style.active.background;
				Rect shaderPos = _graphArea;
				float deltaMat = _deltaX + leftPos;
				shaderPos.x = deltaMat;
				shaderPos.y += shaderPos.height - shaderTex.height - _deltaY;
				shaderPos.width = shaderTex.width;
				shaderPos.height = shaderTex.height;

				Rect collArea = _graphArea;
				collArea.x = leftPos;
				collArea.y += collArea.height - CollSizeY;
				collArea.width = CollSizeX;
				collArea.height = CollSizeY;
				
				string shaderName = ( currentShader != null ) ? ( currentShader.name ) : m_noShaderStr;
				GUI.color = collArea.Contains( mousePos ) ? OverallColorOn : OverallColorOff;
				GUI.Box( shaderPos, m_shaderContent, m_shaderLabelStyle );
				GUI.Box( shaderPos, shaderName, style );
				if ( GUI.Button( collArea, string.Empty, m_empty ) && currentShader != null )
				{
					Selection.activeObject = currentShader;
				}
			}

			// Material Mode
			{
				GUIStyle style = UIUtils.GetCustomStyle( currentMaterial == null ? CustomStyle.ShaderNoMaterialModeTitle : CustomStyle.MaterialModeTitle );
				Texture2D shaderTex = style.normal.background;

				Rect materialPos = _graphArea;
				float deltaShader = _deltaX + rightPos;
				materialPos.x += materialPos.width - shaderTex.width - deltaShader;
				materialPos.y += materialPos.height - shaderTex.height - _deltaY;
				materialPos.width = shaderTex.width;
				materialPos.height = shaderTex.height;
				
				Rect collArea = _graphArea;
				collArea.x += collArea.width - rightPos - CollSizeX;
				collArea.y += collArea.height - CollSizeY;
				collArea.width = CollSizeX;
				collArea.height = CollSizeY;

				GUI.color = collArea.Contains( mousePos ) ? OverallColorOn : OverallColorOff;
				
				string matName = ( currentMaterial != null ) ? ( currentMaterial.name ) : m_noMaterialStr;
				GUI.Box( materialPos, m_materialContent, m_materialLabelStyle );
				GUI.Box( materialPos, matName, style );
				
				if ( GUI.Button( collArea, string.Empty, m_empty ) && currentMaterial != null )
				{
					Selection.activeObject = currentMaterial;
				}
			}

			GUI.color = buffereredColor;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_materialLabelStyle = null;
			m_shaderLabelStyle = null;
		}
	}
}
