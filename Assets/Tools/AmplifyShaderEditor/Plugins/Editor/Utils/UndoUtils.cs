using UnityEditor;
using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	public class UndoUtils
	{
		private const string MessageFormat = "Changing value {0} on node {1}";

		public static string EditorGUILayoutTextField( ParentNode node, string label, string text, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( label, text, options );
			if ( !text.Equals( newValue ) )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static Enum EditorGUILayoutEnumPopup( ParentNode node, Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( selected, options );
			if ( newValue != selected )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, "Enum", ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static int EditorGUILayoutIntPopup( ParentNode node, string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntPopup( label, selectedValue, displayedOptions, optionValues, options );
			if ( newValue != selectedValue )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static int EditorGUILayoutPopup( ParentNode node, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, "Popup", ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static int EditorGUILayoutPopup( ParentNode node, string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static bool EditorGUILayoutToggle( ParentNode node, string label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static bool EditorGUILayoutToggle( ParentNode node, string label, bool value, GUIStyle style, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, style, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static int EditorGUILayoutIntField( ParentNode node, string label, int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( label, value, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static float EditorGUILayoutFloatField( ParentNode node, string label, float value, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.FloatField( label, value, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}
		public static Color EditorGUILayoutColorField( ParentNode node, string label, Color value, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}

			return newValue;
		}
		public static float EditorGUILayoutSlider( ParentNode node, string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static float EditorGUILayoutSlider( ParentNode node, GUIContent label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}
		public static UnityEngine.Object EditorGUILayoutObjectField( ParentNode node, string label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options )
		{
			UnityEngine.Object newValue = EditorGUILayout.ObjectField( label, obj, objType, allowSceneObjects, options );
			if ( newValue != obj )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		public static Vector2 EditorGUILayoutVector2Field( ParentNode node, string label, Vector2 value, params GUILayoutOption[] options )
		{
			Vector2 newValue = EditorGUILayout.Vector2Field( label, value, options );
			if ( newValue != value )
			{
				Undo.RecordObject( node, string.Format( MessageFormat, label, ( ( node.Attributes != null ) ? node.Attributes.Name : node.GetType().ToString() ) ) );
			}
			return newValue;
		}

		
		//EditorGUILayout.Vector3Field
		//EditorGUILayout.Vector4Field
		//EditorGUILayout.IntSlider
		//EditorGUILayout.ToggleLeft
		//EditorGUILayout.TextArea
		//EditorGUILayout.Foldout


		//EditorGUI.TextField
		//EditorGUI.ColorField
		//EditorGUI.IntField
		//EditorGUI.FloatField
		//EditorGUI.EnumPopup
		//EditorGUI.ObjectField
		//EditorGUI.IntPopup
		//EditorGUI.Toggle
	}
}
