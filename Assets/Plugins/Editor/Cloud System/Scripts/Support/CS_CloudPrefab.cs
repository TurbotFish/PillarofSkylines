using UnityEngine;
using UnityEditor;
using System.Collections;
using Edelweiss.CloudSystem;
using Edelweiss.CloudSystemEditor;

public class CS_CloudPrefab {

	[MenuItem ("GameObject/Create Other/Cloud")]
	public static void CreateCloudPrefab () {
		CS_CloudRendererTypeLookup l_CloudRendererTypeLookup = new CS_CloudRendererTypeLookup ();
		CloudPrefab.CreateCloudPrefab <CS_Cloud, CS_ParticleData, CS_CreatorData> (l_CloudRendererTypeLookup);
	}
}
