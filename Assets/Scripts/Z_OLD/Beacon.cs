//using UnityEngine;

//public class Beacon : MonoBehaviour {

//    public bool isHomeBeacon;
//    public Beacon otherBeacon;

//    public Transform teleportPoint;

//    public bool activated;

//    [Header("Rendering")]
//    public Renderer socle;
//    public Material matOn, matOff;
    
//    [HideInInspector] public Transform destination;
    
//    private void Awake()
//    {
//        if (otherBeacon != null)
//            destination = otherBeacon.teleportPoint;
//    }

//    public void Activate()
//    {
//        activated = true;
//        socle.sharedMaterial = matOn;

//        if (!isHomeBeacon)
//            otherBeacon.Activate();
//    }

//    public void SetTombReferences(string[] favourIDs)
//    {
//        Game.World.Interaction.FavourStatue[] statues = GetComponentsInChildren<Game.World.Interaction.FavourStatue>();
//        for (int i = 0; i < statues.Length; i++)
//            statues[i].favourID = favourIDs[i];

//    }

//    private void OnValidate()
//    {
//        if (otherBeacon)
//        {
//            if (!otherBeacon.otherBeacon)
//                otherBeacon.otherBeacon = this;

//            Game.World.Interaction.FavourStatue[] statues = GetComponentsInChildren<Game.World.Interaction.FavourStatue>();
//            string[] IDs = new string[statues.Length];
//            for (int a = 0; a < statues.Length; a++)
//                IDs[a] = statues[a].favourID;
//            otherBeacon.SetTombReferences(IDs);

//        }

//    }

//}
