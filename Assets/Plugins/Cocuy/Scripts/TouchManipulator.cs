using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Touch Manipulator")]
public class TouchManipulator : MonoBehaviour {
	private Vector3 m_previousMousePosition;

	public float m_velocityStrength = 10f;
	public float m_velocityRadius = 5f;
	public float m_particlesStrength = 1f;
	public float m_particlesRadius = 5f;

	public FluidSimulator m_fluid;
	public ParticlesArea m_particlesArea;

    void LateUpdate()
	{
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0f));
            RaycastHit hitInfo = new RaycastHit();
            if (m_particlesArea.GetComponent<Collider>().Raycast(ray, out hitInfo, 100))
            {
                float fWidth = m_particlesArea.GetComponent<Renderer>().bounds.extents.x * 2f;
                float fRadius = (m_particlesRadius * m_particlesArea.GetWidth()) / fWidth;
                m_particlesArea.AddParticles(hitInfo.textureCoord, fRadius, m_particlesStrength * Time.deltaTime);

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 direction = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y) * m_velocityStrength * touch.deltaTime;
                    fWidth = m_fluid.GetComponent<Renderer>().bounds.extents.x * 2f;
                    fRadius = (m_velocityRadius * m_fluid.GetWidth()) / fWidth;
                    m_fluid.AddVelocity(hitInfo.textureCoord, direction, fRadius);
                }
            }
        }
	}
}
