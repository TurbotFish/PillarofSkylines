using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FXManager : MonoBehaviour {

   // public ParticleSystem doubleJumpFX,
                          //dashFX;
    #region Singleton
    public static FXManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);

    }
    #endregion

	[Header ("Impact")]
	public float speedThreshold;
	public float impactTimeToDissolve;
	public float impactTimeToAppear;
	public float impactDelay;
	public Ease impactEaseIn, impactEaseOut;
	public List<Material> impactDissolveMats = new List<Material>();
	public GameObject impactExplosion;
	public List<ParticleSystem> impactParticles = new List<ParticleSystem>();
	public GameObject shadowProjector;
	public void ImpactPlay(float speed)
	{
		//Debug.Log (speed);
		if (speed > speedThreshold) {
			shadowProjector.SetActive (false);

			foreach (ParticleSystem ps in impactParticles) {
				ps.Play ();
			}
			foreach (Material mat in impactDissolveMats) {
				mat.DOFloat (1, "_DissolveAmount", impactTimeToDissolve).SetEase(impactEaseIn);
				mat.DOFloat (0, "_DissolveAmount", impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);
			}


			dashLight.DOIntensity (dashLightIntensity, impactTimeToDissolve).SetEase (impactEaseIn);
			dashLight.DOIntensity (0, impactTimeToAppear).SetEase (impactEaseOut).SetDelay(impactDelay);

			GameObject explo;
			explo = Instantiate (impactExplosion, transform.position, transform.rotation) as GameObject;
			Destroy (explo, 6);

			StartCoroutine(EnableShadow(impactTimeToAppear*0.5f + impactDelay));

		}
	}
	IEnumerator EnableShadow (float t)
	{
		yield return new WaitForSecondsRealtime (t);
		shadowProjector.SetActive (true);
	}


	[Header ("Dust")]
	public List<ParticleSystem> dustParticles = new List<ParticleSystem>();
	public void FootDustPlay()
	{
		foreach (ParticleSystem ps in dustParticles) {
			ps.Play ();
		}
	}


	[Header ("Dash")]
	public float dashTimeToDissolve;
	public float dashTimeToAppear;
	public float dashDelay;
	public Ease dashEaseIn, dashEaseOut;
	public Light dashLight;
	public float dashLightIntensity;
	public List<ParticleSystem> dashParticles = new List<ParticleSystem>();
	public GameObject dashExplosion;
	public List<Material> dashDissolveMats = new List<Material>();

	public void DashPlay()
	{
		foreach (ParticleSystem ps in dashParticles) {
			ps.Play ();
		}
		foreach (Material mat in dashDissolveMats) {
			mat.DOFloat (1, "_DissolveAmount", dashTimeToDissolve).SetEase(dashEaseIn);
			mat.DOFloat (0, "_DissolveAmount", dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);
		}
		dashLight.DOIntensity (dashLightIntensity, dashTimeToDissolve).SetEase (dashEaseIn);
		dashLight.DOIntensity (0, dashTimeToAppear).SetEase (dashEaseOut).SetDelay(dashDelay);

		GameObject explo;
		explo = Instantiate (dashExplosion, transform.position + transform.forward*2 + transform.up, Quaternion.identity) as GameObject;
		Destroy (explo, 6);
	}
		



	[Header ("DoubleJump")]
	public float jumpDissolveAmount;
	public float jumpTimeToDissolve;
	public float jumpTimeToAppear;
	public float jumpDelay;
	public Ease jumpEaseIn, jumpEaseOut;
	public List<ParticleSystem> doubleJumpParticles = new List<ParticleSystem>();
	public List<Material> jumpDissolveMats = new List<Material>();

	public void DoubleJumpPlay()
	{
		foreach (ParticleSystem ps in doubleJumpParticles) {
			ps.Play ();
		}
		foreach (Material mat in jumpDissolveMats) {
			mat.SetFloat ("_DissolveAmount", 0.3f);
			mat.DOFloat (jumpDissolveAmount, "_DissolveAmount", jumpTimeToDissolve).SetEase(jumpEaseIn);
			mat.DOFloat (0, "_DissolveAmount", jumpTimeToAppear).SetEase(jumpEaseOut).SetDelay(jumpDelay);

		}
	}


	[Header ("Glide")]
	//public float glideDissolveAmount;
	//public float glideTimeToDissolve;
	public float glideTimeToAppear;
	public Ease glideEaseOut;
	public List<ParticleSystem> glideParticles = new List<ParticleSystem>();
	public List<Material> glideDissolveMats = new List<Material>();
	public List<LocalTrailRenderer> glideTrails = new List<LocalTrailRenderer>();
	public List<Cloth> glideClothes = new List<Cloth>();

	public MinMax velocity;
	public float receivedVelocity;
	public MinMax glideDissolve;
	public float clothMovement;

	public void GlidePlay()
	{
		foreach (ParticleSystem ps in glideParticles) {
			ps.Play ();
		}
		foreach (Material mat in glideDissolveMats) {
			//mat.DOFloat (glideDissolveAmount, "_DissolveAmount", glideTimeToDissolve).SetEase(glideEaseIn);
		}
		foreach (LocalTrailRenderer tr in glideTrails) {
			tr.follow = true;
		}
	}

	public void GlideStop()
	{
		foreach (ParticleSystem ps in glideParticles) {
			ps.Stop ();
		}
		foreach (Material mat in glideDissolveMats) {
			mat.DOFloat (0, "_DissolveAmount", glideTimeToAppear).SetEase(glideEaseOut);
		}
		foreach (LocalTrailRenderer tr in glideTrails) {
			tr.follow = false;
		}

		foreach (Cloth cloth in glideClothes) {
			cloth.randomAcceleration = Vector3.zero;
		}
	}

	public void GlideUpdate()
	{
		foreach (Material mat in glideDissolveMats) {
			mat.SetFloat ("_DissolveAmount", Mathf.Lerp(glideDissolve.min, glideDissolve.max, (receivedVelocity-velocity.min)/(velocity.max-velocity.min)));
		}
		foreach (Cloth cloth in glideClothes) {
			float amount = clothMovement * (receivedVelocity - velocity.min) / (velocity.max - velocity.min)+50;
			cloth.randomAcceleration = new Vector3 (amount*0.5f, amount*0.5f, amount);
		}
	}


}
