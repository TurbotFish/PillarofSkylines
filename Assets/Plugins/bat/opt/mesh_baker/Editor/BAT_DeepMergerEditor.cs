using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using bat.util;
using UnityEditor;
using bat.opt.bake;
using bat.opt.bake.util;


namespace bat.opt.Bake
{

    [CustomEditor(typeof(BAT_DeepBaker))]
    public class BAT_DeepBakerEditor : Editor
    {
        private BAT_DeepBaker m_deepBaker;
        private Texture m_logo;
        private bool m_hasTrouble = false;
        private List<Texture2DItem> m_referencedTextures=new List<Texture2DItem>();
        void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }
            m_deepBaker = (BAT_DeepBaker)target;
            m_logo = Resources.Load<Texture>("bat_logo");
            m_deepBaker.Refresh();
            refreshReferences();
        }
        private string[] m_ClearMeshItems = new string[] { 
        ClearMeshSetting.Nothing.ToString(),
        ClearMeshSetting.MeshFilters.ToString(),
        ClearMeshSetting.FiltersAndRenderers.ToString()};
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            BAT_UIUtil.RememberColor();
            BAT_ShaderConfig[] uvConfigs = m_deepBaker.m_uvConfigs;

            EditorGUILayout.Separator();

            //【begin of all settings
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical("ProgressBarBack");

            //【begin of head region

            EditorGUILayout.BeginVertical("ProgressBarBack");
            
            //logo
            GUI.backgroundColor = BAT_UIUtil.Color_LogoBg;
            EditorGUILayout.BeginHorizontal("HelpBox");
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(m_logo));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //end of logo

            GUI.backgroundColor =  BAT_UIUtil.Color_LightGray;
             EditorGUILayout.Space();
            //【begin of setting title
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = BAT_UIUtil.Color_LightGray;
            GUILayout.Label("Settings", "GUIEditor.BreadcrumbLeft");

            EditorGUILayout.Space();

            //show shader count
            string shadersLabel = "(" + uvConfigs.Length + "shader" + (uvConfigs.Length > 1 ? "s" : "") + ")";
            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(4, 4, 0, 0);
            style.alignment = TextAnchor.MiddleRight;
            GUILayout.Label(shadersLabel);

            //show refresh button
            GUI.contentColor = BAT_UIUtil.Color_BlueGreen;
            var btnStyle = new GUIStyle(EditorStyles.toolbarButton);
            btnStyle.stretchWidth = false;
            if (GUILayout.Button(new GUIContent("Refresh"), btnStyle))
            {
                m_deepBaker.Refresh(true);
                refreshReferences();
            }
            
            EditorGUILayout.EndHorizontal();
            //endof of setting title】

            //set global definition
            BAT_UIUtil.ResetColor();
            EditorGUILayout.BeginVertical();
            var autoBake = EditorGUILayout.Toggle("Auto Bake", m_deepBaker.m_autoBake);
            if (autoBake != m_deepBaker.m_autoBake)
            {
                Undo();
                m_deepBaker.m_autoBake = autoBake;
            }
            //ClearMeshSettings
            int oldSelID=(int)m_deepBaker.m_clearSetting;
            int newSelID = EditorGUILayout.Popup("ClearSetting",oldSelID, m_ClearMeshItems,"MiniToolbarButton");
            if (newSelID != oldSelID)
            {
                Undo();
                m_deepBaker.m_clearSetting = (ClearMeshSetting)newSelID;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
             //endof of head region】


            //【begin the uvconfigs
            GUI.backgroundColor = BAT_UIUtil.Color_DarkGray;
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.backgroundColor = Color.white;
   
            for (int i=0;i<uvConfigs.Length;i++)
            {
                var uvCfg = uvConfigs[i];
                UVConfigEditor(uvCfg);
                if (i != uvConfigs.Length - 1)
                {
                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = BAT_UIUtil.Color_DarkGray;
                    EditorGUILayout.BeginVertical("HelpBox");
                    GUI.backgroundColor = Color.white;
                }
            }

            EditorGUILayout.EndVertical();
            //end of uvconfigs】


            //【begin of References
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical("ProgressBarBack");

            //title
            EditorGUILayout.BeginVertical();
            //GUILayout.Label("References", "GUIEditor.BreadcrumbLeft");

            if (GUILayout.Button(new GUIContent("References"), btnStyle))
            {
                Undo();
                m_deepBaker.m_editor_showReferences = !m_deepBaker.m_editor_showReferences;
            }

            EditorGUILayout.EndVertical();


            if (m_deepBaker.m_editor_showReferences)
            {
                //【begin the referenced textures

                GUI.backgroundColor = Color.white;
                foreach (Texture2DItem item in m_referencedTextures)
                {
                    item.Draw();
                }

                //Fixe button
                if (m_hasTrouble)
                {
                    GUI.contentColor = Color.white;
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("Fix troubles"), EditorStyles.toolbarButton))
                    {
                        FixTroubles();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }

                //end of the referenced textures】
            }

            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
            //end of References】

            EditorGUILayout.EndVertical();
            //end of all settings】

            BAT_UIUtil.ResetColor();

        }

        private void Undo()
        {
            if (m_deepBaker != null)
            {
                BAT_EdtUtil.Undo_RecordObject(m_deepBaker, "modify property");
            }
        }
   
        private BAT_TextureProperty drawT(BAT_TextureProperty p)
        {
            var new_textureName = EditorGUILayout.TextField(p.m_textureName,new GUIStyle("OL TextField"));
            if (new_textureName!=null && !new_textureName.Equals(p.m_textureName))
            {
                Undo();
                p.m_textureName = new_textureName;
            }

            var new_isNormal = EditorGUILayout.ToggleLeft("normal",p.m_isNormal);
            if (new_isNormal!=p.m_isNormal)
            {
                Undo();
                p.m_isNormal = new_isNormal;
            }

            return p;
        }
        private void insertT(List<BAT_TextureProperty> list,int id)
        {
            Undo();
            string content = list.Count == 0 ? "_MainTex" : "_UndefinedTex";
            BAT_TextureProperty tp = new BAT_TextureProperty(content,false);
            if (id+1 >=  list.Count)
            {
                list.Add(tp);
            }
            else
            {
                list.Insert(id+1, tp);
            }
        }
        private void deleteT(List<BAT_TextureProperty> list,int id)
        {
            Undo();
            list.RemoveAt(id);
        }


        private bool UVButton(int id,ref int m_currentSelUVID)
        {
            if (m_currentSelUVID == id)
            {
                GUI.contentColor = BAT_UIUtil.Color_BlueGreen;
            }
            else
            {
                GUI.contentColor = BAT_UIUtil.Color_LightGray;
            }
            string hint = "Click to set the uv" + id + "'s textures";
            if (id != 0)
            {
                hint = "【UV"+id+" not supported now】";
            }
            var uv_content = new GUIContent("UV"+id, hint);
            var btn= GUILayout.Button(uv_content, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true), GUILayout.Height(16));
            //uv1、uv2not supported now
            //if (btn)
            //{
            //    m_currentSelUVID = id;
            //}
            return btn;
        }
        private List<BAT_TextureProperty> m_uvTxtureList = new List<BAT_TextureProperty>();
        private void UVConfigEditor(BAT_ShaderConfig shaderConfig)
        {
            BAT_UIUtil.ResetColor();

            //【begin of properties
            EditorGUILayout.BeginVertical();

            EditorGUILayout.ObjectField(new GUIContent("shader"),shaderConfig.Shader, typeof(Shader),false);
            
            EditorGUILayout.EndVertical();
            //end of properties】

            //Materials button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Materials", "MiniToolbarButton"))
            {
                shaderConfig.m_editor_showMaterials = !shaderConfig.m_editor_showMaterials;
            }

            if(!shaderConfig.m_editor_showMaterials)
            {
                string showIcon = "("+shaderConfig.m_mateirals.Length +")";
                if (GUILayout.Button(showIcon, "MiniToolbarButtonLeft"))
                {
                    shaderConfig.m_editor_showMaterials = !shaderConfig.m_editor_showMaterials;
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //Mateiral items
            if (shaderConfig.m_editor_showMaterials)
            {
                EditorGUILayout.BeginVertical("ProgressBarBack");
                foreach (var mt in shaderConfig.m_mateirals)
                {
                    EditorGUILayout.ObjectField (mt.name, mt,typeof(Material),false);
                }
                EditorGUILayout.EndVertical();
            }

            //【begin of uv configs
            EditorGUILayout.BeginVertical("ProgressBarBack");//GroupBox

            //【begin of uv buttons
            EditorGUILayout.BeginHorizontal();

            UVButton(0, ref shaderConfig.m_editor_uvSel);
            UVButton(1, ref shaderConfig.m_editor_uvSel);
            UVButton(2, ref shaderConfig.m_editor_uvSel);

            EditorGUILayout.EndHorizontal();
            //end of uv buttons】

            EditorGUILayout.Space();

             //【begin of one uv config settings
            BAT_UIUtil.ResetContentColor();
            var selUVCfg = shaderConfig.m_uvConfigs[shaderConfig.m_editor_uvSel];

            EditorGUILayout.BeginHorizontal();
            int orgSize = selUVCfg.m_maxTextureSize;
            int newSize = EditorGUILayout.IntField(new GUIContent("max texure size"), orgSize);
            if(newSize!=orgSize)
            {
                Undo();
                selUVCfg.setMaxTureSize(newSize);
            }
            EditorGUILayout.EndHorizontal();

            m_uvTxtureList.Clear();
            if (selUVCfg.m_uvTxtures != null && selUVCfg.m_uvTxtures.Length > 0)
            {
                m_uvTxtureList.AddRange(selUVCfg.m_uvTxtures);
            }
            bool changed = BAT_UIUtil.EditList(m_uvTxtureList, drawT, insertT, deleteT);
            if (changed)
            {
                selUVCfg.m_uvTxtures = m_uvTxtureList.ToArray();
            }

             // end of one uv config settins】

            EditorGUILayout.EndVertical();
            //end of uvconfigs】

            BAT_UIUtil.ResetColor();
        }

        /// <summary>
        /// Refresh references,and find out troubles.
        /// </summary>
        private void refreshReferences()
        {
            var txs = GetAllNeededTextures();
            m_referencedTextures.Clear();
            foreach (var tx in txs)
            {
                Texture2DItem  item  = new Texture2DItem(tx);
                m_referencedTextures.Add(item);
                if (item.HasTrouble())
                {
                    m_hasTrouble = true;
                }
            }
        }
        /// <summary>
        /// return all needed 2d textures
        /// </summary>
        /// <returns>all 2d textures</returns>
        public List<Texture2D> GetAllNeededTextures()
        {
            var meshRenderers = m_deepBaker.GetComponentsInChildren<Renderer>();
            List<Material> mtList = new List<Material>();
            List<Texture2D> allTextures = new List<Texture2D>();
            Dictionary<Shader, List<BAT_TextureProperty>> allTxProperties = new Dictionary<Shader, List<BAT_TextureProperty>>();
            foreach (var meshRenderer in meshRenderers)
            {
                var mts = meshRenderer.sharedMaterials;
                foreach (var mt in mts)
                {
                    if (mt != null && !mtList.Contains(mt))
                    {
                        mtList.Add(mt);
                        Shader shader = mt.shader;
                        if (!allTxProperties.ContainsKey(shader))
                        {
                            var list = BAT_TextureProperty.ListTextures(shader);
                            allTxProperties.Add(shader, list);
                        }
                    }
                }

            }

            foreach (var mt in mtList)
            {
                List<BAT_TextureProperty> txProperties = allTxProperties[mt.shader];
                foreach (var tp in txProperties)
                {
                    var tx = mt.GetTexture(tp.m_textureName) as Texture2D;
                    if (tx != null && !allTextures.Contains(tx))
                    {
                        allTextures.Add(tx);
                    }
                }
            }
            return allTextures;
        }

        private void FixTroubles()
        {
            if (m_hasTrouble)
            {
                int count = m_referencedTextures.Count;
                //fix troubles
                for (int i = 0; i < count; i++)
                {
                    var t = m_referencedTextures[i];
                    t.Fix();
                }
                AssetDatabase.Refresh();
                //refresh
                refreshReferences();
            }
        }



        [MenuItem("Window/Runtime Mesh Baker/Add DeepBaker")]
        public static void AddDeepBaker()
        {
            var targetGO = UnityEditor.Selection.activeGameObject as GameObject;
            if (targetGO != null)
            {
                BAT_BakerBase childItem = targetGO.GetComponentInChildren<BAT_BakerBase>();
                if (childItem != null)
                {
                    BAT_UIUtil.ShowWarning("You don't need two Bakers in the same tree,because it exist in " + childItem.name);
                    return;
                }
                BAT_BakerBase parentItem = targetGO.GetComponentInParent<BAT_BakerBase>();
                if (parentItem != null)
                {
                    BAT_UIUtil.ShowWarning("You don't need two Bakers in the same tree,because it exist in " + parentItem.name);
                    return;
                }
                BAT_EdtUtil.Undo_AddComponent<BAT_DeepBaker>(targetGO);
            }
        }
    }

    public class Texture2DItem
    {
        public Texture2D m_texture;
        public string m_trouble="";
        public Texture2DItem(Texture2D texture)
        {
            m_texture = texture;
            var f=m_texture.format;
            if (!IsReadableFormat(f))
            {
               m_trouble = "【Format error!】";
            }
            if (m_texture!=null && !IsReadable())
            {
                m_trouble = "【Not readable!】";
            }
           
        }

        private bool IsReadableFormat(TextureFormat f)
        {
			string name = "" + f;
			if (name.IndexOf ("DXT") > 0 || name.StartsWith ("BC")||name.IndexOf ("PVRTC") > 0) 
			{
				return false;
			}
			if(name.IndexOf ("ATC") > 0|| name.IndexOf ("EAC") > 0|| name.IndexOf ("ASTC") > 0 )
			{
				return false;
			}
			if(name.IndexOf ("ETC") > 0 )
			{
				return false;
			}
            return true;
        }

        public bool HasTrouble()
        {
            return m_trouble != null && m_trouble.Length > 0;
        }

        public void Draw()
        {
            GUI.backgroundColor = Color.white;


            EditorGUILayout.BeginHorizontal();


            UnityEngine.Object obj = m_texture;

            string warnStr;
            if (HasTrouble())
            {
                GUI.contentColor = Color.red;
                warnStr = m_trouble;
            }
            else
            {
                GUI.contentColor = Color.white;
                warnStr = "OK";
            }
            
            EditorGUILayout.ObjectField(warnStr, obj, null,false);

            EditorGUILayout.EndHorizontal();

        }

        public void Fix()
        {
            if (!HasTrouble())
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(m_texture);
            TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterSettings tis = new TextureImporterSettings();
            for (RuntimePlatform p = RuntimePlatform.Android; p < RuntimePlatform.XboxOne; p++)
            {
                texImporter.ClearPlatformTextureSettings(p.ToString());
            }
            texImporter.ReadTextureSettings(tis);
            //set auto compressed format
            tis.textureFormat = TextureImporterFormat.AutomaticCompressed;
            //set readable 
            tis.readable = true;

            texImporter.SetTextureSettings(tis);
            AssetDatabase.ImportAsset(path);

            //check the default import format is readbale or not
            if (!IsReadableFormat(m_texture.format))
            {
                tis.textureFormat = TextureImporterFormat.RGBA32;
                texImporter.SetTextureSettings(tis);

                AssetDatabase.ImportAsset(path);
            }

        }


        public bool IsReadable()
        {
            string path = AssetDatabase.GetAssetPath(m_texture);
            TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterSettings tis = new TextureImporterSettings();
            texImporter.ReadTextureSettings(tis);
            return tis.readable;
        }

    }
}
