using Game.LevelElements;
using System.Collections;
using UnityEngine;

public class AmbientBox : MonoBehaviour, IInteractable
{
    //##################################################################

    [SerializeField] UnityEngine.PostProcessing.PostProcessingProfile postProcess;

    [SerializeField] bool editAmbient = true;
    [SerializeField] bool editFog = false;
	[SerializeField] bool editAudio = false;

    [Header("Ambient")]
    [ConditionalHide("editAmbient")]
    [SerializeField] Color color;
    [ConditionalHide("editAmbient")]
    [SerializeField] float ambientFadeSpeed = 2;


    [Header("Gradient Fog")]
    [ConditionalHide("editFog")]
    [SerializeField] Gradient gradient;
    [ConditionalHide("editFog")]
    [SerializeField] float startDistance = 100;
    [ConditionalHide("editFog")]
    [SerializeField] float endDistance = 200;
    [ConditionalHide("editFog")]
    [SerializeField] float fogFadeSpeed = 2;

	[Header("Audio")]
	[ConditionalHide("editAudio")]
	[SerializeField] AudioClip clip;
	[ConditionalHide("editAudio")]
	[SerializeField] float audioFadeSpeed = 2f;
	[ConditionalHide("editAudio")]
	[SerializeField] AnimationCurve fadeInCurve;
	[ConditionalHide("editAudio")]
	[SerializeField] AnimationCurve fadeOutCurve;

	AudioClip defaultClip;
	float currentVolume, audioTimer, defaultVolume;
	AudioSource atmoSource;

    GradientFog fog;
    UnityEngine.PostProcessing.PostProcessingBehaviour postProcessStack;

    Color defaultColor;
    Gradient defaultGradient;
    float defaultStart, defaultEnd;

    //##################################################################

    #region initialization

    private void Start() {
        defaultColor = RenderSettings.ambientLight;

        fog = FindObjectOfType<GradientFog>(); // TODO: fix that
        defaultGradient = fog.gradient;
        defaultStart = fog.startDistance;
        defaultEnd = fog.endDistance;

        postProcessStack = FindObjectOfType<UnityEngine.PostProcessing.PostProcessingBehaviour>(); // fix that as well prob

		atmoSource = FindObjectOfType<AudioManager> ().transform.Find ("Nature").transform.Find ("Wind").GetComponent<AudioSource> ();//while you're at it you might want to fix this as well ;);)
		if (!atmoSource)
			Debug.LogError ("Coudln't find audiosource on P_AudioManager/Nature/Wind");
		else {
			defaultClip = atmoSource.clip;
			defaultVolume = atmoSource.volume;
		}
    }

    #endregion initialization

    //##################################################################

    #region inquiries

    public Transform Transform { get { return transform; } }

    public bool IsInteractable() {
        return false;
    }

    #endregion inquiries

    //################################################################## 

    #region operations

    public void OnPlayerEnter()
    {
        StopAllCoroutines();
        if (editAmbient)
            StartCoroutine(FadeAmbient(color));

        if (editFog)
        {
            StartCoroutine(FadeFog(gradient, startDistance, endDistance));
            print("Entered " + name + " changing fog");
        }
        if (postProcess)
            postProcessStack.OverrideProfile(postProcess);

		if (editAudio)
			StartCoroutine (FadeAudio (1f, true));
    }

    public void OnPlayerExit()
    {
        StopAllCoroutines();
        if (editAmbient)
            StartCoroutine(FadeAmbient(defaultColor));

        if (editFog)
            StartCoroutine(FadeFog(defaultGradient, defaultStart, defaultEnd));

        if (postProcess)
            postProcessStack.StopOverridingProfile();

		if (editAudio)
			StartCoroutine (FadeAudio (0f, false));
    }

    public void OnHoverBegin()
    {
    }

    public void OnHoverEnd()
    {
    }

    public void OnInteraction()
    {
    }

    private IEnumerator FadeAmbient(Color goal)
    {
        while (RenderSettings.ambientLight != goal)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, goal, Time.deltaTime * ambientFadeSpeed);
            yield return null;
        }
    }

    private IEnumerator FadeFog(Gradient goal, float startGoal, float endGoal)
    {
        GradientFog[] fogs = FindObjectsOfType<GradientFog>();
        fog = fogs[0];
        
        foreach (GradientFog foggy in fogs)
        {
            foggy.secondaryGradient = goal;
            foggy.gradientLerp = 0;
            foggy.GenerateTexture();
        }

        float t = 0;

        while (Mathf.Abs(fog.startDistance - startGoal) > 0.1f || Mathf.Abs(fog.endDistance - endGoal) > 0.1f)
        {
            t += Time.deltaTime * fogFadeSpeed;

            //print(t);

            foreach (GradientFog foggy in fogs) {
                foggy.gradientLerp = t;
                foggy.GenerateTexture();
                foggy.startDistance = Mathf.Lerp(foggy.startDistance, startGoal, t);
                foggy.endDistance = Mathf.Lerp(foggy.endDistance, endGoal, t);

            }
            yield return null;
        }

        foreach (GradientFog foggy in fogs)
        {
            foggy.gradient = goal;
            foggy.gradientLerp = 0;
            foggy.GenerateTexture();
        }
    }

	private IEnumerator FadeAudio(float _targetVolume, bool _entering)
	{

		while (audioTimer <= audioFadeSpeed)
		{
			audioTimer += Time.deltaTime;
			currentVolume = fadeOutCurve.Evaluate (Mathf.Clamp01 (audioTimer / audioFadeSpeed)) * defaultVolume;
			atmoSource.volume = currentVolume;
			yield return null;
		}

		audioTimer = 0f;


		atmoSource.clip = _entering ? clip : defaultClip;

		atmoSource.PlayScheduled (Random.value);

		while (audioTimer <= audioFadeSpeed)
		{
			audioTimer += Time.deltaTime;
			currentVolume = fadeInCurve.Evaluate (Mathf.Clamp01 (audioTimer / audioFadeSpeed)) * defaultVolume;
			atmoSource.volume = currentVolume;
			yield return null;
		}

		audioTimer = 0f;

	}

    #endregion operations

    //##################################################################
}
