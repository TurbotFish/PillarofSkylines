using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FXManager : MonoBehaviour {
    
	[Header ("Special Material")]
	public Renderer pilouBody;
	public Renderer pilouArmor;
	public Renderer pilouHair;
	public Renderer pilouSkirtA;
	public Renderer pilouSkirtB;

	Material pilouBodyMat;
	Material pilouArmorMat;
	Material pilouHairMat;
	Material pilouSkirtAMat;
	Material pilouSkirtBMat;

	Color pilouSkirtColorOn;
	Color pilouSkirtColorOff;

	void Start()
	{
		pilouBodyMat = pilouBody.material;
		pilouArmorMat = pilouArmor.material;
		pilouHairMat = pilouHair.material;
		pilouSkirtAMat = pilouSkirtA.material;
		pilouSkirtBMat = pilouSkirtB.material;

		pilouSkirtColorOn = pilouSkirtAMat.color;
		pilouSkirtColorOff = new Color (pilouSkirtColorOn.r,pilouSkirtColorOn.g,pilouSkirtColorOn.b,0);

	}

    #region Impact

    [Header ("Impact")]
	public float speedThreshold;
	public float impactTimeToDissolve;
	public float impactTimeToAppear;
	public float impactDelay;
	public Ease impactEaseIn, impactEaseOut;
	//public List<Material> impactDissolveMats = new List<Material>();
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

			pilouBodyMat.DOFloat (1, "_DissolveAmount", impactTimeToDissolve).SetEase(impactEaseIn);
			pilouBodyMat.DOFloat (0, "_DissolveAmount", impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);
			pilouArmorMat.DOFloat (1, "_DissolveAmount", impactTimeToDissolve).SetEase(impactEaseIn);
			pilouArmorMat.DOFloat (0, "_DissolveAmount", impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);
			pilouHairMat.DOFloat (1, "_DissolveAmount", impactTimeToDissolve).SetEase(impactEaseIn);
			pilouHairMat.DOFloat (0, "_DissolveAmount", impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);
			pilouSkirtAMat.DOColor(pilouSkirtColorOff,"_Color",impactTimeToDissolve).SetEase(impactEaseIn);
			pilouSkirtAMat.DOColor(pilouSkirtColorOn,"_Color",impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);
			pilouSkirtBMat.DOColor(pilouSkirtColorOff,"_Color",impactTimeToDissolve).SetEase(impactEaseIn);
			pilouSkirtBMat.DOColor(pilouSkirtColorOn,"_Color",impactTimeToAppear).SetEase(impactEaseOut).SetDelay(impactDelay);

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
    #endregion

    #region Dust
    [Header ("Dust")]
	public List<ParticleSystem> dustParticles = new List<ParticleSystem>();
	public void FootDustPlay()
	{
		foreach (ParticleSystem ps in dustParticles) {
			ps.Play ();
		}
	}
    #endregion

    #region Dash
    [Header ("Dash")]
	public float dashTimeToDissolve;
	public float dashTimeToAppear;
	public float dashDelay;
	public Ease dashEaseIn, dashEaseOut;
	public Light dashLight;
	public float dashLightIntensity;
	public List<ParticleSystem> dashParticles = new List<ParticleSystem>();
	public GameObject dashExplosion;
	//public List<Material> dashDissolveMats = new List<Material>();

	public void DashPlay()
	{
		foreach (ParticleSystem ps in dashParticles) {
			ps.Play ();
		}
		pilouBodyMat.DOFloat (1, "_DissolveAmount", dashTimeToDissolve).SetEase(dashEaseIn);
		pilouBodyMat.DOFloat (0, "_DissolveAmount", dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);
		pilouArmorMat.DOFloat (1, "_DissolveAmount", dashTimeToDissolve).SetEase(dashEaseIn);
		pilouArmorMat.DOFloat (0, "_DissolveAmount", dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);
		pilouHairMat.DOFloat (1, "_DissolveAmount", dashTimeToDissolve).SetEase(dashEaseIn);
		pilouHairMat.DOFloat (0, "_DissolveAmount", dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);
		pilouSkirtAMat.DOColor(pilouSkirtColorOff,"_Color",dashTimeToDissolve).SetEase(dashEaseIn);
		pilouSkirtAMat.DOColor(pilouSkirtColorOn,"_Color",dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);
		pilouSkirtBMat.DOColor(pilouSkirtColorOff,"_Color",dashTimeToDissolve).SetEase(dashEaseIn);
		pilouSkirtBMat.DOColor(pilouSkirtColorOn,"_Color",dashTimeToAppear).SetEase(dashEaseOut).SetDelay(dashDelay);

		dashLight.DOIntensity (dashLightIntensity, dashTimeToDissolve).SetEase (dashEaseIn);
		dashLight.DOIntensity (0, dashTimeToAppear).SetEase (dashEaseOut).SetDelay(dashDelay);

		GameObject explo;
		explo = Instantiate (dashExplosion, transform.position + transform.forward*2 + transform.up, Quaternion.identity) as GameObject;
		Destroy (explo, 6);
	}

    #endregion

    #region Double Jump
    [Header ("DoubleJump")]
	public float jumpDissolveAmount;
	public float jumpTimeToDissolve;
	public float jumpTimeToAppear;
	public float jumpDelay;
	public Ease jumpEaseIn, jumpEaseOut;
	public List<ParticleSystem> doubleJumpParticles = new List<ParticleSystem>();
	//public List<Material> jumpDissolveMats = new List<Material>();

	public void DoubleJumpPlay()
	{
		foreach (ParticleSystem ps in doubleJumpParticles) {
			ps.Play ();
		}

		pilouBodyMat.SetFloat ("DissolveAmount", 0.3f);
		pilouHairMat.SetFloat ("DissolveAmount", 0.3f);
		pilouArmorMat.SetFloat ("DissolveAmount", 0.3f);

		pilouBodyMat.DOFloat (jumpDissolveAmount, "_DissolveAmount", jumpTimeToDissolve).SetEase(jumpEaseIn);
		pilouBodyMat.DOFloat (0, "_DissolveAmount", jumpTimeToAppear).SetEase(jumpEaseOut).SetDelay(jumpDelay);
		pilouArmorMat.DOFloat (jumpDissolveAmount, "_DissolveAmount", jumpTimeToDissolve).SetEase(jumpEaseIn);
		pilouArmorMat.DOFloat (0, "_DissolveAmount", jumpTimeToAppear).SetEase(jumpEaseOut).SetDelay(jumpDelay);
		pilouHairMat.DOFloat (jumpDissolveAmount, "_DissolveAmount", jumpTimeToDissolve).SetEase(jumpEaseIn);
		pilouHairMat.DOFloat (0, "_DissolveAmount", jumpTimeToAppear).SetEase(jumpEaseOut).SetDelay(jumpDelay);


	}
    #endregion

    #region Glide
    [Header ("Glide")]
	//public float glideDissolveAmount;
	//public float glideTimeToDissolve;
	public float glideTimeToAppear;
	public Ease glideEaseOut;
	public List<ParticleSystem> glideParticles = new List<ParticleSystem>();
	//public List<Material> glideDissolveMats = new List<Material>();
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

		//pilouBodyMat.DOFloat (glideDissolveAmount, "_DissolveAmount", glideTimeToDissolve).SetEase(glideEaseIn);
		//pilouHairMat.DOFloat (glideDissolveAmount, "_DissolveAmount", glideTimeToDissolve).SetEase(glideEaseIn);

		foreach (LocalTrailRenderer tr in glideTrails) {
			tr.follow = true;
		}
	}

	public void GlideStop()
	{
		foreach (ParticleSystem ps in glideParticles) {
			ps.Stop ();
		}

		pilouBodyMat.DOFloat (0, "_DissolveAmount", glideTimeToAppear).SetEase(glideEaseOut);
		pilouArmorMat.DOFloat (0, "_DissolveAmount", glideTimeToAppear).SetEase(glideEaseOut);
		pilouHairMat.DOFloat (0, "_DissolveAmount", glideTimeToAppear).SetEase(glideEaseOut);

		foreach (LocalTrailRenderer tr in glideTrails) {
			tr.follow = false;
		}

		foreach (Cloth cloth in glideClothes) {
			cloth.randomAcceleration = Vector3.zero;
		}
	}

	public void GlideUpdate()
	{
		pilouBodyMat.SetFloat ("_DissolveAmount", Mathf.Lerp(glideDissolve.min, glideDissolve.max, (receivedVelocity-velocity.min)/(velocity.max-velocity.min)));
		pilouArmorMat.SetFloat ("_DissolveAmount", Mathf.Lerp(glideDissolve.min, glideDissolve.max, (receivedVelocity-velocity.min)/(velocity.max-velocity.min)));
		pilouHairMat.SetFloat("_DissolveAmount", Mathf.Lerp(glideDissolve.min, glideDissolve.max, (receivedVelocity-velocity.min)/(velocity.max-velocity.min)));

		foreach (Cloth cloth in glideClothes) {
			float amount = clothMovement * (receivedVelocity - velocity.min) / (velocity.max - velocity.min)+50;
			cloth.randomAcceleration = new Vector3 (amount*0.5f, amount*0.5f, amount);
		}
	}
    #endregion

    #region Echo
    [Header ("Echo")]
	public Transform echoParticlesHolder;
	public ParticleSystem echoPose;
	public List<ParticleSystem> holdEchoParticles = new List<ParticleSystem>();
	public float speedToGrow;

	public void EchoPlay ()
	{
		echoParticlesHolder.localScale = Vector3.zero;
		echoParticlesHolder.gameObject.SetActive (true);
		echoParticlesHolder.DOScale (Vector3.zero, speedToGrow).From ().SetEase (Ease.OutSine);
		foreach (ParticleSystem ps in holdEchoParticles) {
			ps.Play ();
		}
	}
	public void EchoStop ()
	{
		echoPose.Play ();
		echoParticlesHolder.gameObject.SetActive (false);
		foreach (ParticleSystem ps in holdEchoParticles) {
			ps.Stop ();
		}	
	}
    #endregion

    #region Phantom
    [Header ("Phantom")]
	public ParticleSystem phantomPose;
	public List<ParticleSystem> phantomIdleParticles = new List<ParticleSystem>();
    
	public void PhantomPlay ()
	{
		foreach (ParticleSystem ps in phantomIdleParticles) {
			ps.Play ();
		}
	}
	public void PhantomStop ()
	{
		phantomPose.Play ();
		foreach (ParticleSystem ps in phantomIdleParticles) {
			ps.Stop ();
		}	
	}
    #endregion

    #region EchoBoost
    [Header("EchoBoost")]
    public ParticleSystem boostParticles;

    public GameObject[] trails;
    public string playerEmissiveVariable = "_Emission";
    public Color playerEmissive = new Color(0.2f, 0.5f, 0.43f);
    public float emissiveTransition = 0.5f;


    public void PlayEchoBoost(float duration) {
        boostParticles.Play();
        
        StartCoroutine(_EchoBoostMaterialChange(duration));

        foreach (GameObject obj in trails)
            obj.SetActive(true);
    }

    public void StopEchoBoost() {
        boostParticles.Stop();

        pilouBody.sharedMaterial.SetColor("_Emission", Color.black);

        foreach (GameObject obj in trails)
            obj.SetActive(false);
    }

    IEnumerator _EchoBoostMaterialChange(float duration) {
        for (float elapsed = 0; elapsed < emissiveTransition; elapsed+=Time.deltaTime)
        {
            pilouBody.sharedMaterial.SetColor(playerEmissiveVariable, Color.Lerp(Color.black, playerEmissive, elapsed / emissiveTransition));
            yield return null;
        }

        yield return new WaitForSeconds(duration - (emissiveTransition * 2));

        for (float elapsed = 0; elapsed < emissiveTransition; elapsed += Time.deltaTime)
        {
            pilouBody.sharedMaterial.SetColor(playerEmissiveVariable, Color.Lerp(playerEmissive, Color.black, elapsed / emissiveTransition));
            yield return null;
        }
    }

    #endregion
}
