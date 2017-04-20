// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Flipbook UV Animation
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{

	[Serializable]
	[NodeAttributes( "Flipbook UV Animation", "Textures", "Animate a Flipbook Texture Modifying UV Coordinates.", null, KeyCode.None, true, false, null, null, true )]
	public sealed class TFHCFlipBookUVAnimation : ParentNode

	{

		private const string TextureVerticalDirectionStr = "Texture Direction";
		private const string NegativeSpeedBehaviorStr = "If Negative Speed";

		[SerializeField]
		private int _selectedTextureVerticalDirection = 0;

		[SerializeField]
		private int _negativeSpeedBehavior = 0;

		[SerializeField]
		private readonly string[] _TextureVerticalDirectionValues = { "Top To Bottom", "Bottom To Top" };

		[SerializeField]
		private readonly string[] _NegativeSpeedBehaviorValues = { "Switch to Positive", "Reverse Animation" };


		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Columns" );
			AddInputPort( WirePortDataType.FLOAT, false, "Rows" );
			AddInputPort( WirePortDataType.FLOAT, false, "Speed" );
			AddInputPort( WirePortDataType.FLOAT, false, "Start Frame" );
			AddOutputVectorPorts( WirePortDataType.FLOAT2, "UV" );
			m_outputPorts[ 1 ].Name = "U";
			m_outputPorts[ 2 ].Name = "V";
			m_textLabelWidth = 125;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "04fe24be792bfd5428b92132d7cf0f7d";
		}


		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			_selectedTextureVerticalDirection = EditorGUILayout.Popup( TextureVerticalDirectionStr, _selectedTextureVerticalDirection, _TextureVerticalDirectionValues );
			_negativeSpeedBehavior = EditorGUILayout.Popup( NegativeSpeedBehaviorStr, _negativeSpeedBehavior, _NegativeSpeedBehaviorValues );
			EditorGUILayout.EndVertical();
			EditorGUILayout.HelpBox( "Flipbook UV Animation:\n\n  - UV: Texture Coordinates to Flipbook.\n - Columns: number of Columns (X) of the Flipbook Texture.\n  - Rows: number of Rows (Y) of the Flipbook Textures.\n  - Speed: speed of the animation.\n - Texture Direction: set the vertical order of the texture tiles.\n - If Negative Speed: set the behavior when speed is negative.\n\n - Out: UV Coordinates.", MessageType.None );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			_selectedTextureVerticalDirection = ( int ) int.Parse( GetCurrentParam( ref nodeParams ) );
			_negativeSpeedBehavior = ( int ) int.Parse( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, _selectedTextureVerticalDirection );
			IOUtils.AddFieldValueToString( ref nodeInfo, _negativeSpeedBehavior );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			// OPTIMIZATION NOTES
			//
			//  round( fmod( x, y ) ) can be replaced with a faster
			//  floor( frac( x / y ) * y + 0.5 ) => div can be muls with 1/y, almost always static/constant
			//

			string uv = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false, true );
			string columns = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, false, true );
			if ( !m_inputPorts[ 1 ].IsConnected )
				columns = ( float.Parse( columns ) == 0f ? "1" : columns );
			string rows = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, false, true );
			if ( !m_inputPorts[ 2 ].IsConnected )
				rows = ( float.Parse( rows ) == 0f ? "1" : rows );


			string speed = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, false, true );
			string startframe = m_inputPorts[ 4 ].GeneratePortInstructions( ref dataCollector );

			string vcomment1 = "// *** BEGIN Flipbook UV Animation vars ***";
			string vcomment2 = "// Total tiles of Flipbook Texture";
			string vtotaltiles = "float fbtotaltiles" + m_uniqueId + " = " + columns + " * " + rows + ";";
			string vcomment3 = "// Offsets for cols and rows of Flipbook Texture";
			string vcolsoffset = "float fbcolsoffset" + m_uniqueId + " = 1.0f / " + columns + ";";
			string vrowssoffset = "float fbrowsoffset" + m_uniqueId + " = 1.0f / " + rows + ";";
			string vcomment4 = "// Speed of animation";
			string vspeed = "float fbspeed" + m_uniqueId + " = _Time[1] * " + speed + ";";
			string vcomment5 = "// UV Tiling (col and row offset)";
			string vtiling = "float2 fbtiling" + m_uniqueId + " = float2(fbcolsoffset" + m_uniqueId + ", fbrowsoffset" + m_uniqueId + ");";
			string vcomment6 = "// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)";
			string vcomment7 = "// Calculate current tile linear index";
			//float fbcurrenttileindex1 = round( fmod( fbspeed1 + _Float0, fbtotaltiles1 ) );
			string vcurrenttileindex = "float fbcurrenttileindex" + m_uniqueId + " = round( fmod( fbspeed" + m_uniqueId + " + " + startframe + ", fbtotaltiles" + m_uniqueId + ") );";
			vcurrenttileindex += "\n\t\t\tfbcurrenttileindex" + m_uniqueId + " += ( fbcurrenttileindex" + m_uniqueId + " < 0) ? fbtotaltiles" + m_uniqueId + " : 0;";
			//fbcurrenttileindex1 += ( fbcurrenttileindex1 < 0 ) ? fbtotaltiles1 : 0;
			//string vcurrenttileindex = "int fbcurrenttileindex" + m_uniqueId + " = (int)fmod( fbspeed" + m_uniqueId + ", fbtotaltiles" + m_uniqueId + ") + " + startframe + ";";
			string vcomment8 = "// Obtain Offset X coordinate from current tile linear index";

			//float fblinearindextox1 = round( fmod( fbcurrenttileindex1, 5.0 ) );
			//string voffsetx1 = "int fblinearindextox" + m_uniqueId + " = fbcurrenttileindex" + m_uniqueId + " % (int)" + columns + ";";
			string voffsetx1 = "float fblinearindextox" + m_uniqueId + " = round ( fmod ( fbcurrenttileindex" + m_uniqueId + ", " + columns + " ) );";
			string vcomment9 = String.Empty;
			string voffsetx2 = String.Empty;
			if ( _negativeSpeedBehavior != 0 )
			{
				vcomment9 = "// Reverse X animation if speed is negative";
				voffsetx2 = "fblinearindextox" + m_uniqueId + " = (" + speed + " > 0 ? fblinearindextox" + m_uniqueId + " : (int)" + columns + " - fblinearindextox" + m_uniqueId + ");";
			}
			string vcomment10 = "// Multiply Offset X by coloffset";
			string voffsetx3 = "float fboffsetx" + m_uniqueId + " = fblinearindextox" + m_uniqueId + " * fbcolsoffset" + m_uniqueId + ";";
			string vcomment11 = "// Obtain Offset Y coordinate from current tile linear index";
			//float fblinearindextoy1 = round( fmod( ( fbcurrenttileindex1 - fblinearindextox1 ) / 5.0, 5.0 ) );
			string voffsety1 = "float fblinearindextoy" + m_uniqueId + " = round( fmod( ( fbcurrenttileindex" + m_uniqueId + " - fblinearindextox" + m_uniqueId + " ) / " + columns + ", " + rows + " ) );";
			//string voffsety1 = "int fblinearindextoy" + m_uniqueId + " = (int)( ( fbcurrenttileindex" + m_uniqueId + " - fblinearindextox" + m_uniqueId + " ) / " + columns + " ) % (int)" + rows + ";";
			//string vcomment10 = "// Reverse Y to get from Top to Bottom";
			//string voffsety2 = "fblinearindextoy" + m_uniqueId + " = (int)" + rows + " - fblinearindextoy" + m_uniqueId + ";";
			string vcomment12 = String.Empty;
			string voffsety2 = String.Empty;
			if ( _negativeSpeedBehavior == 0 )
			{
				if ( _selectedTextureVerticalDirection == 0 )
				{
					vcomment12 = "// Reverse Y to get tiles from Top to Bottom";
					voffsety2 = "fblinearindextoy" + m_uniqueId + " = (int)(" + rows + "-1) - fblinearindextoy" + m_uniqueId + ";";
				}
			}
			else
			{
				string reverseanimationoperator = String.Empty;
				if ( _selectedTextureVerticalDirection == 0 )
				{
					vcomment12 = "// Reverse Y to get tiles from Top to Bottom and Reverse Y animation if speed is negative";
					reverseanimationoperator = " < ";
				}
				else
				{
					vcomment12 = "// Reverse Y animation if speed is negative";
					reverseanimationoperator = " > ";
				}
				voffsety2 = "fblinearindextoy" + m_uniqueId + " = (" + speed + reverseanimationoperator + " 0 ? fblinearindextoy" + m_uniqueId + " : (int)" + rows + " - fblinearindextoy" + m_uniqueId + ");";
			}
			string vcomment13 = "// Multiply Offset Y by rowoffset";
			string voffsety3 = "float fboffsety" + m_uniqueId + " = fblinearindextoy" + m_uniqueId + " * fbrowsoffset" + m_uniqueId + ";";
			string vcomment14 = "// UV Offset";
			string voffset = "float2 fboffset" + m_uniqueId + " = float2(fboffsetx" + m_uniqueId + ", fboffsety" + m_uniqueId + ");";
			//string voffset = "float2 fboffset" + m_uniqueId + " = float2( ( ( (int)fmod( fbspeed" + m_uniqueId + " , fbtotaltiles" +  m_uniqueId + ") % (int)" + columns + " ) * fbcolsoffset" + m_uniqueId + " ) , ( ( (int)" + rows + " - ( (int)( ( (int)fmod( fbspeed" + m_uniqueId + " , fbtotaltiles" + m_uniqueId + " ) - ( (int)fmod( fbspeed" + m_uniqueId + " , fbtotaltiles" + m_uniqueId + " ) % (int)" + columns + " ) ) / " + columns + " ) % (int)" + rows + " ) ) * fbrowsoffset" + m_uniqueId + " ) );";
			string vcomment15 = "// Flipbook UV";
			string vfbuv = "half2 fbuv" + m_uniqueId + " = " + uv + " * fbtiling" + m_uniqueId + " + fboffset" + m_uniqueId + ";";
			string vcomment16 = "// *** END Flipbook UV Animation vars ***";
			string result = "fbuv" + m_uniqueId;

			dataCollector.AddToLocalVariables( m_uniqueId, vcomment1 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment2 );
			dataCollector.AddToLocalVariables( m_uniqueId, vtotaltiles );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment3 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcolsoffset );
			dataCollector.AddToLocalVariables( m_uniqueId, vrowssoffset );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment4 );
			dataCollector.AddToLocalVariables( m_uniqueId, vspeed );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment5 );
			dataCollector.AddToLocalVariables( m_uniqueId, vtiling );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment6 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment7 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcurrenttileindex );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment8 );
			dataCollector.AddToLocalVariables( m_uniqueId, voffsetx1 );
			if ( _negativeSpeedBehavior != 0 )
			{
				dataCollector.AddToLocalVariables( m_uniqueId, vcomment9 );
				dataCollector.AddToLocalVariables( m_uniqueId, voffsetx2 );
			}
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment10 );
			dataCollector.AddToLocalVariables( m_uniqueId, voffsetx3 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment11 );
			dataCollector.AddToLocalVariables( m_uniqueId, voffsety1 );
			if ( _selectedTextureVerticalDirection == 0 || _negativeSpeedBehavior != 0 )
			{
				dataCollector.AddToLocalVariables( m_uniqueId, vcomment12 );
				dataCollector.AddToLocalVariables( m_uniqueId, voffsety2 );
			}
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment13 );
			dataCollector.AddToLocalVariables( m_uniqueId, voffsety3 );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment14 );
			dataCollector.AddToLocalVariables( m_uniqueId, voffset );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment15 );
			dataCollector.AddToLocalVariables( m_uniqueId, vfbuv );
			dataCollector.AddToLocalVariables( m_uniqueId, vcomment16 );

			return GetOutputVectorItem( 0, outputId, result );

		}
	}
}
