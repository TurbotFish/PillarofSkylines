using System;
using UnityEngine;

namespace bat.util
{
    public class AutoRotation:BaseBehavior
    {
        [SerializeField][Tooltip("旋转角速度")]
        protected float m_angleVelocity = 360;
        [SerializeField][Tooltip("相对旋转轴")]
        protected Vector3 m_axis = Vector3.up;
        [SerializeField][Tooltip("开启自动旋转")]
        protected bool m_auto = true;
        protected override void OnInitFirst()
        {
        }

        protected override void OnInitSecond()
        {
        }
        protected override void OnUpdate()
        {
            if (m_auto)
            {
               m_transform.Rotate(m_axis, m_angleVelocity * Time.deltaTime);
            }
        }
    }
}
