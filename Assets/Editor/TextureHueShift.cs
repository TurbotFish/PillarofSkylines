using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureHueShift : EditorWindow
{

    string texName = "Graph/Textures/HueShift";

    [SerializeField, Range(0, 1)] float hueShift = 0;

    [MenuItem("Tools/HueShift")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TextureHueShift));
        GetWindow<TextureHueShift>().Show();
    }

    void OnGUI()
    {
        Texture[] textures = Selection.GetFiltered<Texture>(SelectionMode.Assets);

        if (textures.Length == 0)
        {
            EditorGUILayout.HelpBox("Select a Texture in order to change its hue.", MessageType.Warning);
            return;
        }
        EditorGUILayout.Space();

        Texture texture = textures[0];
        
        EditorGUILayout.LabelField("Hue Shift Texture:", EditorStyles.boldLabel);
        EditorGUI.DrawPreviewTexture(new Rect(130, 5, 20, 20), texture);
        
        EditorGUILayout.Space();
        
        texName = AssetDatabase.GetAssetPath(texture) + "_HueShifted";
        texName = GUILayout.TextField(texName);

        if (File.Exists(Application.dataPath + "/" + texName + ".png"))
            EditorGUILayout.HelpBox(texName + ".png already exists and will be replaced.", MessageType.Warning);

        EditorGUILayout.Space();

        SerializedObject sO = new SerializedObject(this);
        SerializedProperty _hueShift = sO.FindProperty("hueShift");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(_hueShift);

        if (EditorGUI.EndChangeCheck())
            sO.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Texture Copy"))
        {
            CreateTextureCopy(texture, hueShift);
            Close();
        }
        Repaint();
    }

    void CreateTextureCopy(Texture texture, float angle)
    {
        Texture2D srcTexture = texture as Texture2D;
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        
        Color[] src = srcTexture.GetPixels();
        Color[] pix = newTexture.GetPixels();

        for (int i = 0; i < texture.height; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                float h = 0, s = 0, v = 0;
                Color.RGBToHSV(src[(i*texture.width)+j], out h, out s, out v);
                h += angle;
                pix[i] = Color.HSVToRGB(h, s, v);
                newTexture.SetPixel(j, i, pix[i]);
            }
        }
        
        newTexture.Apply();

        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath.Remove(Application.dataPath.Length - 7) + "/" + texName + ".png", bytes);
        
        HueShiftPostProcessor.texturePath = texName + ".png";
        AssetDatabase.ImportAsset(texName + ".png");

        DestroyImmediate(newTexture);
    }
}

class HueShiftPostProcessor : AssetPostprocessor
{

    public static string texturePath;
   /* public static FilterMode filterMode;

    void OnPreprocessTexture()
    {
        if (assetPath == texturePath)
        {

        }
    }*/
}