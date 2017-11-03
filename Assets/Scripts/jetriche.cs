using UnityEngine;

public class jetriche : MonoBehaviour {

    [SerializeField] Player player;

	void Start () {
        player.numberOfAerialJumps = 1000;
	}
}
