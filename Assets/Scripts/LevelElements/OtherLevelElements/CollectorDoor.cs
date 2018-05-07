using UnityEngine;

public class CollectorDoor : MonoBehaviour {

    [TestButton("Update Collectibles Amount", "UpdateCollectiblesAmount", isActiveInEditor = true)]
    [SerializeField] int totalCollectibles;
    int collected;

    private void OnTriggerEnter(Collider other) {
        Collectible lulu = other.GetComponent<Collectible>();

        if (lulu && !lulu.collected) {
            lulu.GoToCollector(transform.position);
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
