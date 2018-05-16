using Game.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelElements
{
    /// <summary>
    /// An interface for all objects in the world that react to the player's presence and/or that the player can interact with.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Returns the Transform of the interactable object.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Called when the player enters the trigger collider.
        /// </summary>
        void OnPlayerEnter();
        /// <summary>
        /// Called when the player leaves the trigger collider.
        /// </summary>
        void OnPlayerExit();

        /// <summary>
        /// Used to check whether the player can interact with this object.
        /// </summary>
        /// <returns></returns>
        bool IsInteractable();

        /// <summary>
        /// Called when this object becomes the nearest interactable object to the player.
        /// </summary>
        void OnHoverBegin();
        /// <summary>
        /// Called when this object is not the nearest interactable object to the player anymore.
        /// </summary>
        void OnHoverEnd();
        /// <summary>
        /// Called when the player interacts with this object.
        /// </summary>
        void OnInteraction();
    }
} // end of namespace