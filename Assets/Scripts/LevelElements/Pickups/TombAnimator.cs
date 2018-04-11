using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    public delegate void TombAnimationFinishedCallback();

    /// <summary>
    /// Base class for animating a Tomb.
    /// </summary>
    public abstract class TombAnimator : MonoBehaviour
    {
        //##################################################################

        private bool isTombActivated;
        protected TombAnimationFinishedCallback animationFinishedCallback;

        //##################################################################

        public bool IsTombActivated { get { return isTombActivated; } }

        //##################################################################

        public virtual bool SetTombState(bool isActivated, bool interactWithPlayer, bool doImmediateTransition, TombAnimationFinishedCallback callback = null)
        {
            if (IsTombActivated == isActivated)
            {
                return false;
            }

            isTombActivated = isActivated;
            animationFinishedCallback = callback;

            return true;
        }

        //##################################################################
    }
} // end of namespace