using System;
using UnityEngine;

namespace bat.util
{
    public struct DrawRect
    {
        public float m_w;
        public  float m_h;
        public float m_gap;
        public float m_margin;
        public DrawRect(float w, float h, float gap, float margin)
        {
            m_w = w;
            m_h = h;
            m_gap = gap;
            m_margin = margin;
        }
    }
    public class GUI_Drawer
    {
		protected static DrawRect m_def = new DrawRect(150,30,10,10);
        protected static GUIStyle M_DefStyle = new GUIStyle();
        public static bool DrawBtn(float idX, string text,float idY=0,float hScale=1,float wScale=1)
        {
            return GUI.Button(GetDefRect(idX,idY,hScale,wScale), text);
        }
        public static void DrawLabel(float idX, string text,float idY=0,float hScale=1,float wScale=1)
        {
            GUI.Label(GetDefRect(idX,idY,hScale,wScale), text);
        }
        public static float DrawHSlider(float idX, float value,float leftValue,float rightValue,float idY=0,float hScale=1,float wScale=1)
        {
            return GUI.HorizontalSlider(GetDefRect(idX,idY,hScale,wScale), value, leftValue, rightValue);
        }
        public static float DrawVSlider(float idX, float value, float topValue, float bottomValue,float idY=0,float hScale=1,float wScale=1)
        {
            return GUI.VerticalSlider(GetDefRect(idX,idY,hScale,wScale), value, topValue, bottomValue);
        }

        public static bool DrawToogle(float idX,bool value, string text, float idY = 0, float hScale = 1, float wScale = 1)
        {
            return GUI.Toggle(GetDefRect(idX, idY, hScale, wScale),value,text);
        }

        private static Rect GetDefRect(float idX, float idY=0,float hScale=1,float wScale=1)
        {
           return new Rect(
               m_def.m_margin + (m_def.m_w + m_def.m_gap) * idX,
               m_def.m_margin+ (m_def.m_h + m_def.m_gap) * idY, m_def.m_w*wScale, m_def.m_h*hScale);
        }
    }
}
