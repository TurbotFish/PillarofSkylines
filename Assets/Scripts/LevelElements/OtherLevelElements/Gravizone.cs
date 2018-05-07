using UnityEngine;

public class Gravizone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Game.Player.CharacterController.CharController player = other.GetComponentInParent<Game.Player.CharacterController.CharController>();

            player.ChangeGravityDirection(-transform.up);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Game.Player.CharacterController.CharController player = other.GetComponentInParent<Game.Player.CharacterController.CharController>();

            player.ChangeGravityDirection(Vector3.down);
        }
    }



}
