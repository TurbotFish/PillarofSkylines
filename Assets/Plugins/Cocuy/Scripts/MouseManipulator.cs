using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Mouse Manipulator")]
public class MouseManipulator : MonoBehaviour {
	private Vector3 m_previousMousePosition;

	[HideInInspector]
	public float m_velocityStrength = 10f;
	[HideInInspector]
	public float m_velocityRadius = 5f;

	[HideInInspector]
	public float m_particlesStrength = 1f;
	[HideInInspector]
	public float m_particlesRadius = 5f;

	public FluidSimulator m_fluid;
	public ParticlesArea m_particlesArea;
	public bool m_alwaysOn = false;

	void DrawGizmo()
	{
		float col = m_particlesStrength / 10000.0f;
		Gizmos.color = Color.Lerp(Color.yellow, Color.red, col);
		Gizmos.DrawWireSphere(transform.position, m_particlesRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_velocityRadius);
	}

	void OnDrawGizmosSelected()
	{
		DrawGizmo();
	}

	void OnDrawGizmos()
	{

		DrawGizmo();
	}

    void LateUpdate()
	{
		if (Input.GetMouseButton(0) || m_alwaysOn)
        {
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
			RaycastHit hitInfo = new RaycastHit();
			if (m_particlesArea.GetComponent<Collider>().Raycast(ray, out hitInfo, 100))
			{
				float fWidth = m_particlesArea.GetComponent<Renderer>().bounds.extents.x * 2f;
				float fRadius = (m_particlesRadius * m_particlesArea.GetWidth()) / fWidth;
				m_particlesArea.AddParticles(hitInfo.textureCoord, fRadius, m_particlesStrength * Time.deltaTime);
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			m_previousMousePosition = Input.mousePosition;
		}

		if (Input.GetMouseButton(1) || m_alwaysOn)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
			RaycastHit hitInfo = new RaycastHit();
            if (m_fluid.GetComponent<Collider>().Raycast(ray, out hitInfo, 100))
			{
				Vector3 direction = (Input.mousePosition - m_previousMousePosition) * m_velocityStrength * Time.deltaTime;
				float fWidth = m_fluid.GetComponent<Renderer>().bounds.extents.x * 2f;
                float fRadius = (m_velocityRadius * m_fluid.GetWidth()) / fWidth;

				if (Input.GetMouseButton(0))
				{
                    m_fluid.AddVelocity(hitInfo.textureCoord, -direction, fRadius);
				}
				else
				{
                    m_fluid.AddVelocity(hitInfo.textureCoord, direction, fRadius);

				}
			}
			m_previousMousePosition = Input.mousePosition;
		}
	}
}
