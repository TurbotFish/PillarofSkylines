using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenuController : MonoBehaviour, IUiState
{

    public bool IsActive { get; private set; }

    //###########################################################

    #region monobehaviour methods

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion monobehaviour methods

    //###########################################################

    void IUiState.Activate()
    {
        if (!this.IsActive)
        {
            this.IsActive = true;
            this.gameObject.SetActive(true);
        }
    }

    void IUiState.Deactivate()
    {
        this.IsActive = false;
        this.gameObject.SetActive(false);
    }

    //###########################################################
}
