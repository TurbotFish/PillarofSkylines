using UnityEditor;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class OptionsWindow
	{
		private bool m_coloredPorts = false;

		public OptionsWindow()
		{
			//Load ();
		}

		public void Init()
		{
			Load();
		}

		public void Destroy()
		{
			Save();
		}

		public void Save()
		{
			EditorPrefs.SetBool( "ColoredPorts", UIUtils.CurrentWindow.ToggleDebug );
			EditorPrefs.SetBool( "ExpandedStencil", UIUtils.CurrentWindow.ExpandedStencil );
			EditorPrefs.SetBool( "ExpandedTesselation", UIUtils.CurrentWindow.ExpandedTesselation );
			EditorPrefs.SetBool( "ExpandedDepth", UIUtils.CurrentWindow.ExpandedDepth );
			EditorPrefs.SetBool( "ExpandedRenderingOptions", UIUtils.CurrentWindow.ExpandedRenderingOptions );
			EditorPrefs.SetBool( "ExpandedRenderingPlatforms", UIUtils.CurrentWindow.ExpandedRenderingPlatforms );
			EditorPrefs.SetBool( "ExpandedProperties", UIUtils.CurrentWindow.ExpandedProperties );
		}

		public void Load()
		{
			UIUtils.CurrentWindow.ToggleDebug = EditorPrefs.GetBool( "ColoredPorts" );
			ColoredPorts = UIUtils.CurrentWindow.ToggleDebug;
			UIUtils.CurrentWindow.ExpandedStencil = EditorPrefs.GetBool( "ExpandedStencil" );
			UIUtils.CurrentWindow.ExpandedTesselation = EditorPrefs.GetBool( "ExpandedTesselation" );
			UIUtils.CurrentWindow.ExpandedDepth = EditorPrefs.GetBool( "ExpandedDepth" );
			UIUtils.CurrentWindow.ExpandedRenderingOptions = EditorPrefs.GetBool( "ExpandedRenderingOptions" );
			UIUtils.CurrentWindow.ExpandedRenderingPlatforms = EditorPrefs.GetBool( "ExpandedRenderingPlatforms" );
			UIUtils.CurrentWindow.ExpandedProperties = EditorPrefs.GetBool( "ExpandedProperties" );
		}

		public bool ColoredPorts
		{
			get { return m_coloredPorts; }
			set
			{
				if ( m_coloredPorts != value )
					EditorPrefs.SetBool( "ColoredPorts", value );

				m_coloredPorts = value;
			}
		}
	}
}
