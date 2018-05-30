﻿using UnityEngine;

public class CollectorDoor : MonoBehaviour {

    [TestButton("Update Collectibles Amount", "UpdateCollectiblesAmount", isActiveInEditor = true)]
    [SerializeField] int totalCollectibles;
    int collected;


    [SerializeField] Transform[] luluDestinations;

    private void Start() {
        if (luluDestinations.Length < totalCollectibles)
            Debug.LogError("There are less lulu destinations than the given amount of lulu in the world. Either update collectibles amount or add more destinations.");
    }


    private void OnTriggerEnter(Collider other) {
        Collectible lulu = other.GetComponent<Collectible>();

        if (lulu && !lulu.collected) {
            lulu.GoToCollector(luluDestinations[collected].position);
            collected++;

            if (collected >= totalCollectibles) {
                // la porte se dissolve et ouvre le passage
                Destroy(transform.GetChild(0).gameObject);
            }

        }

    }

    void UpdateCollectiblesAmount() {
        totalCollectibles = FindObjectsOfType<Collectible>().Length;
    }

}
