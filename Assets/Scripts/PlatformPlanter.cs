using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPlanter : MonoBehaviour {

    public PlatformToPlant platform;
    public float spawnDistance = 10, plantedDistance = 1;
    public float cooldown = 1;
    float lastTimePlanted;
    bool aiming;

	void Update () {
		


        if (Input.GetKeyUp(KeyCode.Mouse1) && aiming) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.tag != "Player") {

                Vector3 normal = hit.normal;
                Vector3 spawnPosition = hit.point + hit.normal * spawnDistance;
                PlatformToPlant newPlatform = Instantiate(platform, spawnPosition, Quaternion.LookRotation(hit.point - spawnPosition));

                newPlatform.PlantInto(hit.point + hit.normal * plantedDistance);
                lastTimePlanted = Time.time;
            }

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            aiming = false;


        } else if (Input.GetKey(KeyCode.Mouse1) && Time.time - cooldown > lastTimePlanted) {

            Time.timeScale = 0.2f;
            Cursor.lockState = CursorLockMode.None;
            aiming = true;
        }

    }
}
