using System;
using System.Collections.Generic;
using UnityEngine;

namespace bat.util
{
    public class BAT_GfxUtil
    {
        private static Dictionary<Texture2D, Color[]> M_PixelsCache = new Dictionary<Texture2D, Color[]>();
        public static Color[] GetPixels(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }
            Color[] pixels = null;
            M_PixelsCache.TryGetValue(texture, out pixels);
            if (pixels != null)
            {
                return pixels;
            }
            pixels = texture.GetPixels();
            M_PixelsCache.Add(texture, pixels);
            return pixels;
        }
        public static void ClearPixelsCache()
        {
            M_PixelsCache.Clear();
        }
        public static void ImageResize(Color[] src, int w, int h, Color[] dst, int newW, int newH)
        {
            int ofs = 0;
            if (newW == 0)
            {
                return;
            }
            int dX = (w << 16) / newW;
            int dY = (h << 16) / newH;
            int sY = 0;
            for (int y = 0; y < newH; y++)
            {
                int startY = w * (sY >> 16);
                int sX = 0;
                for (int x = 0; x < newW; x++)
                {
                    int startX = (sX >> 16);
                    Color rgb = src[startY + startX];
                    dst[ofs + x] = rgb;
                    sX += dX;
                }
                sY += dY;
                ofs += newW;
            }
        }
        public static void ImageShrink(Color[] src, int w, int h, Color[] dst, int newW, int newH)
        {
            int ofs = 0;
            int dX = (w << 16) / newW;
            int dY = (h << 16) / newH;
            int sY = 0;
            int eY = dY;
            for (int y = 0; y < newH; y++)
            {
                int startY = w * (sY >> 16);
                int endY = w * (eY >> 16);
                int sX = 0;
                int eX = dX;
                for (int x = 0; x < newW; x++)
                {
                    int startX = (sX >> 16);
                    int endX = (eX >> 16);
                    uint avg = 0;
                    uint wgt = 0;
                    for (int Y = startY; Y < endY; Y += w)
                    {
                        for (int X = startX; X < endX; X++)
                        {
                            uint rgb = ColorToInt(src[Y + X]);
                            avg = (avg != 0) ? PixelWeightedAverage(avg, wgt, rgb, 1) : rgb;
                            wgt++;
                        }
                    }
                    dst[ofs + x] = IntToColor(avg);
                    sX += dX;
                    eX += dX;
                }
                sY += dY;
                eY += dY;
                ofs += newW;
            }
        }

        private static uint PixelWeightedAverage(uint p1, uint w1, uint p2, uint w2)
        {
            long ag1 = (p1 >> 8) & 0x00FF00FFL;
            long rb1 = (p1) & 0x00FF00FFL;
            long ag2 = (p2 >> 8) & 0x00FF00FFL;
            long rb2 = (p2) & 0x00FF00FFL;
            long v = (w1 << 8) / (w1 + w2);
            long ag = (ag2 << 8) + (v * (ag1 - ag2));
            long rb = (rb2 << 8) + (v * (rb1 - rb2));
            return (uint)(((ag & 0xFF00FF00L)) | ((rb >> 8) & 0x00FF00FFL));
        }

        public static uint ColorToInt(Color color)
        {
            return ((uint)(color.r*255)<<24)|((uint)(color.g*255)<<16)|((uint)(color.b*255)<<8)|((uint)(color.a*255));
        }
        private static float Div255 = 1.0f / 255;
        public static Color IntToColor(uint color)
        {
            float r=((color >> 24)&0xFF) * Div255;
            float g=((color >> 16)&0xFF) * Div255;
            float b=((color >> 8)&0xFF) * Div255;
            float a=((color)&0xFF) * Div255;
            return new Color(r, g, b, a);
        }

 
    }
}
