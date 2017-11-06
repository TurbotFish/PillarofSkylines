using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.UI {
    public interface IUiState
    {
        bool IsActive { get; }

        void Initialize(PlayerModel playerModel);

        void Activate(Utilities.EventManager.OnShowMenuEventArgs args);

        void Deactivate();
    }
}