using UnityEngine;
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
	//public float glideTimeToAppear;
	//public Ease glideEaseIn, glideEaseOut;
	public List<ParticleSystem> glideParticles = new List<ParticleSystem>();
	public List<Material> glideDissolveMats = new List<Material>();
	public List<LocalTrailRenderer> glideTrails = new List<LocalTrailRenderer>();

	public MinMax velocity;
	public float receivedVelocity;
	public MinMax glideDissolve;

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
			//mat.DOFloat (0, "_DissolveAmount", glideTimeToAppear).SetEase(glideEaseOut);
		}
		foreach (LocalTrailRenderer tr in glideTrails) {
			tr.follow = false;
		}
	}

	public void GlideUpdate()
	{
		foreach (Material mat in glideDissolveMats) {
			mat.SetFloat ("_DissolveAmount", Mathf.Lerp(glideDissolve.min, glideDissolve.max, (receivedVelocity-velocity.min)/(velocity.max-velocity.min)));
		}
	}


}
