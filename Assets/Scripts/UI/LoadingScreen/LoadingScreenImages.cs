using Game.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Model;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LoadingScreenImages", fileName = "LoadingScreenImages")]
    public class LoadingScreenImages : ScriptableObject
    {
        [SerializeField, HideInInspector] private List<LoadingScreenImageElement> loadingScreenImages = new List<LoadingScreenImageElement>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pillarId"></param>
        /// <returns></returns>
        public List<Sprite> GetImages(int pillarId)
        {
            var result = new List<Sprite>();
            foreach (var element in loadingScreenImages)
            {
                if (element.id == pillarId)
                {
                    result.Add(element.sprite);
                }
            }
            return result;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<LoadingScreenImageElement> GetImageElements(int id)
        {
            var result = loadingScreenImages.Where(item => item.id == id).ToList();
            return result;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void RemoveImageElement(LoadingScreenImageElement element)
        {
            loadingScreenImages.RemoveAll(item => item.id == element.id && item.sprite.GetInstanceID() == element.sprite.GetInstanceID());

            EditorUtility.SetDirty(this);
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void AddImageElement(LoadingScreenImageElement element)
        {
            if (loadingScreenImages.FirstOrDefault(item => item.id == element.id && item.sprite.GetInstanceID() == element.sprite.GetInstanceID()) == null)
            {
                loadingScreenImages.Add(element);
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public class LoadingScreenImageElement
    {
        [SerializeField] public int id = -1;
        [SerializeField] public Sprite sprite = null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LoadingScreenImages))]
    public class LoadingScreenImagesInspector : Editor
    {
        private LoadingScreenImages self;

        int pillarIdCount = Enum.GetValues(typeof(PillarId)).Cast<PillarId>().Count();
        Dictionary<int, List<Sprite>> images = new Dictionary<int, List<Sprite>>();
        //Dictionary<int, int> listSizes = new Dictionary<int, int>();
        Dictionary<int, bool> foldouts = new Dictionary<int, bool>();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnEnable()
        {
            self = target as LoadingScreenImages;

            for (int i = -1; i < pillarIdCount; i++)
            {
                images.Add(i, self.GetImages(i));
                //listSizes.Add(i, images[i].Count);
                foldouts.Add(i, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            for (int id = -1; id < pillarIdCount; id++)
            {
                string name = "Open World Images";
                if (Enum.IsDefined((typeof(PillarId)), id))
                {
                    name = ((PillarId)id).ToString() + " Images";
                }
                //EditorGUILayout.LabelField(name);
                foldouts[id] = EditorGUILayout.Foldout(foldouts[id], name);

                if (foldouts[id])
                {
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel++;

                    var spriteList = images[id];

                    //size
                    int listSize = spriteList.Count;
                    listSize = EditorGUILayout.IntField("Size", listSize);

                    if (listSize != spriteList.Count)
                    {
                        while (listSize > spriteList.Count)
                        {
                            spriteList.Add(null);
                        }

                        while (listSize < spriteList.Count)
                        {
                            spriteList.RemoveAt(spriteList.Count - 1);
                        }
                    }

                    //content
                    for (int index = 0; index < spriteList.Count; index++)
                    {
                        spriteList[index] = (Sprite)EditorGUILayout.ObjectField(index.ToString(), spriteList[index], typeof(Sprite), false);
                    }

                    //
                    EditorGUI.indentLevel = indentLevel;

                    //
                    var imageElements = self.GetImageElements(id);

                    foreach (var element in imageElements)
                    {
                        if (spriteList.FirstOrDefault(item => item.GetInstanceID() == element.sprite.GetInstanceID()) == null)
                        {
                            self.RemoveImageElement(element);
                        }
                    }

                    foreach (var sprite in spriteList)
                    {
                        if (imageElements.FirstOrDefault(item => item.sprite.GetInstanceID() == sprite.GetInstanceID()) == null)
                        {
                            self.AddImageElement(new LoadingScreenImageElement() { id = id, sprite = sprite });
                        }
                    }
                }
            }
        }
    }
#endif
} //end of namespace