using System.Collections;
using Game.GameControl;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;

public class PillarStartCutscene : MonoBehaviour {

    // This is a bad script to start the cutscene when entering the Pillar

    [SerializeField] Animator eyeAnim;
    [SerializeField] Animator exitAnim;

    [SerializeField] float animTime = 16;

    private GameController GameController;

    private void Start()
    {
        eyeAnim.enabled = true;
        exitAnim.enabled = true;
        
        StartCoroutine(_WaitForCutscene());
    }

    IEnumerator _WaitForCutscene()
    {
        yield return new WaitForSeconds(0.5f);
        //GameController.SwitchGameState(GameState.Pause, MenuType.NONE);

        GameController = FindObjectOfType<GameController>();
        GameController.PlayerController.CharController.isHandlingInput = false;

        yield return new WaitForSeconds(animTime);

        eyeAnim.enabled = false;
        GameController.PlayerController.CharController.isHandlingInput = true;

        //GameController.SwitchGameState(GameState.Play, MenuType.HUD);
        //print("done " + GameController.CurrentGameState);
    }

}
