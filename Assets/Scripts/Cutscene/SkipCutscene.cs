using UnityEngine;

public class SkipCutscene : MonoBehaviour {

    [SerializeField] GameObject[] toDisable;

    [SerializeField] Transform playerPos;

    [SerializeField] Transform moveCrystal;

    [SerializeField] float crystalYMove = 69.9f;

    [SerializeField] Animator pfAnimator;

    [SerializeField] Game.LevelElements.FavourTombAnimator favour;

    [SerializeField] Game.Cutscene.Cutscene cutscene;

    bool active;

    public void EnableCutsceneSkip() {
        active = true;
    }


	void Update () {
		
        if (active && Input.GetButton("MenuButton")) {
            foreach (GameObject go in toDisable)
                go.SetActive(false);

            Transform player = FindObjectOfType<Game.Player.PlayerController>().transform;
            player.position = playerPos.position;
            player.rotation = playerPos.rotation;

            if (pfAnimator)
                pfAnimator.SetBool("Activate", true);

            if (moveCrystal)
                moveCrystal.position -= Vector3.up * crystalYMove;

            if (favour)
                favour.Skip();

            cutscene.OnCutsceneOver();

            Destroy(gameObject);
        }
        
	}

}
