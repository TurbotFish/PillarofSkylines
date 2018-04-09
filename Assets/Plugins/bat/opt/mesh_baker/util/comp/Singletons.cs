using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace bat.util
{
    /// <summary>
    /// 单例单元管理器
    /// 你可以创建单例组件，每个单例组件对应一个GameObject。
    /// 你可以为单例命名，名字同时也会作为GameObject的名字。
    /// 这些产生的单例一般用作管理器。
    /// </summary>
    public static class Singletons
    {
        private static Dictionary<string, BaseBehavior> m_singletons = new Dictionary<string, BaseBehavior>();
        public static T Get<T>(string name) where T:BaseBehavior
        {
            
            BaseBehavior singleton = null;
            m_singletons.TryGetValue(name, out singleton);
            if (singleton == null)
            {
                GameObject newGo = new GameObject(name);
                singleton = newGo.AddComponent<T>();
                m_singletons.Add(name, singleton);
            }
            return singleton as T;
        }
        public static void Destroy(string name)
        {
            BaseBehavior singleton = null;
            m_singletons.TryGetValue(name, out singleton);
            if (singleton != null)
            {
                m_singletons.Remove(name);
                GameObject.DestroyImmediate(singleton.gameObject);
            }
        }
        public static void Clear()
        {
            List<string> keys = new List<string>();
            foreach (var key in m_singletons.Keys)
            {
                keys.Add(key);
            }
            foreach (var key in keys)
            {
                Destroy(key);
            }
        }

    }
}
