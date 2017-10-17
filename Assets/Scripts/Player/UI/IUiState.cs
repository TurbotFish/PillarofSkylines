using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUiState
{
    bool IsActive { get; }

    void Activate();
    void Deactivate();
}
