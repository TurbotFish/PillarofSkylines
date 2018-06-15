using UnityEngine;

public class VertexOffsetManager : MonoBehaviour {

	public Texture2D windIntensityTex;
	public GameObject player;

	void Start () {
		Shader.SetGlobalTexture ("_WindTex", windIntensityTex);
	}

	void Update () {
		Vector4 playerPos = new Vector4 (player.transform.position.x, player.transform.position.y + .7f, player.transform.position.z, 0);
		Shader.SetGlobalVector ("_PlayerPos", playerPos);
	}
}
