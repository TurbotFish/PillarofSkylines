using UnityEngine;
using System.Collections;

namespace Game.EchoSystem
{
    public class EchoCameraEffect : MonoBehaviour
    {
        new Camera camera;
        float previousFov;

        [SerializeField] float targetFov = 90;
        [SerializeField] float duration = 0.1f;

        [SerializeField]
        AnimationCurve FOVChange;

        [SerializeField] ParticleSystem fx;
        [SerializeField] Material screenMat;

        Texture defaultScreenTexture;
        Texture2D currentScreenTexture;

        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        void Start()
        {
            camera = GetComponent<Camera>();
            previousFov = camera.fieldOfView;
            defaultScreenTexture = screenMat.mainTexture;
            currentScreenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }

        private void OnDestroy()
        {
            screenMat.mainTexture = defaultScreenTexture;
        }

        public void Play()
        {
            /*RenderTexture screenCap = RenderTexture.GetTemporary(Screen.width, Screen.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

            camera.targetTexture = screenCap;
            camera.Render();

            currentScreenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            currentScreenTexture.Apply();

            screenMat.mainTexture = currentScreenTexture;
            camera.targetTexture = null;

            RenderTexture.ReleaseTemporary(screenCap);
            fx.Play();*/
            SetFov(targetFov, duration, true);
        }

        IEnumerator _ScreenCap()
        {
            yield return endOfFrame;

            RenderTexture screenCap = RenderTexture.GetTemporary(Screen.width, Screen.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

            camera.targetTexture = screenCap;
            camera.Render();

            currentScreenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            currentScreenTexture.Apply();

            screenMat.mainTexture = currentScreenTexture;
            camera.targetTexture = null;

            RenderTexture.ReleaseTemporary(screenCap);
            fx.Play();
        }

        public void SetFov(float newFov, float time, bool goBack = false)
        {
            StartCoroutine(_SetFov(newFov, time, goBack));
        }

        IEnumerator _SetFov(float newFov, float time, bool goBack = false)
        {

            previousFov = camera.fieldOfView;

            for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime)
            {

                camera.fieldOfView = Mathf.LerpUnclamped(previousFov, newFov, FOVChange.Evaluate(elapsed / time));
                yield return null;
            }
            camera.fieldOfView = newFov;

            if (goBack)
            {
                SetFov(previousFov, time);
            }
        }

    }
} //end of namespace