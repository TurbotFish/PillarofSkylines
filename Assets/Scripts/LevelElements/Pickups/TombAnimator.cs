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

        // -- ATTRIBUTES

        public bool IsTombActivated { get; private set; }

        protected TombAnimationFinishedCallback animationFinishedCallback;

        //##################################################################

        // -- OPERATIONS

        public virtual bool SetTombState(bool isActivated, bool interactWithPlayer, bool doImmediateTransition, TombAnimationFinishedCallback callback = null)
        {
            if (IsTombActivated == isActivated)
            {
                return false;
            }

            IsTombActivated = isActivated;
            animationFinishedCallback = callback;

            return true;
        }
    }
} // end of namespace