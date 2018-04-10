using UnityEngine;
using System.Collections;

namespace bat.util
{
    /// <summary>
    /// 初始化状态
    /// </summary>
    public enum InitState
    {
        NothingInited,
        FirstInited,
        SecondInited
    }
    public abstract class BaseBehavior : AbstractBehavior
    {
        /// <summary>
        /// 当前的初始化状态
        /// </summary>
        protected InitState m_initState = InitState.NothingInited;
        [System.NonSerialized][Tooltip("变换对象")]
        public Transform m_transform;
        /// <summary>
        /// 在此完成第一阶段的初始化
        /// </summary>
        protected override sealed void Awake()
        {
            m_transform = transform;
            DoInit(InitState.FirstInited);
        }
        /// <summary>
        /// 在此完成第二阶段的初始化
        /// </summary>
        protected override sealed void Start()
        {
            DoInit(InitState.SecondInited);
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        protected override sealed void Update()
        {
            if (m_initState != InitState.SecondInited)
            {
                return;
            }
            OnUpdate();
        }
        /// <summary>
        /// 用于首次初始化，完成外部需访问的公共成员的初始化
        /// </summary>
        protected abstract void OnInitFirst();
        /// <summary>
        /// 用于二次初始化，完成内部私有成员的初始化
        /// </summary>
        protected abstract void OnInitSecond();
        /// <summary>
        /// 执行更新
        /// </summary>
        protected abstract void OnUpdate();
        /// <summary>
        /// 用于外部主动调用初始化
        /// </summary>
        /// <param name="targetState">初始化的目标状态</param>
        public void DoInit(InitState targetState=InitState.SecondInited)
        {
            if (m_initState == InitState.NothingInited&& m_initState<targetState)
            {
                OnInitFirst();
                m_initState = InitState.FirstInited;
            }
            if (m_initState == InitState.FirstInited && m_initState<targetState)
            {
                OnInitSecond();
                m_initState = InitState.SecondInited;
            }
        }


    }

    public abstract class AbstractBehavior : MonoBehaviour
    {
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }
    }

}
