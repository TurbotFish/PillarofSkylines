using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Particles Area Manipulator")]
[ExecuteInEditMode]
public class ParticlesAreaManipulator : MonoBehaviour {

	public ParticlesArea m_particlesArea;
	[HideInInspector]
	public bool m_useScaleAsSize = true;
	[HideInInspector]
	public float m_radius = 0.1f;
	[HideInInspector]
	public float m_strength = 1f;
	[HideInInspector]
	public bool m_showGizmo = false;

    bool bdone = false;

	public float GetRadius()
	{
		if (m_useScaleAsSize)
		{
			return Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
		}
		return m_radius;
	}

	void LateUpdate()
	{
        if (m_particlesArea && !bdone)
		{
            //bdone = true;
			Ray ray = new Ray(transform.position, new Vector3(0, 0, 1));
			RaycastHit hitInfo = new RaycastHit();
			if (m_particlesArea.GetComponent<Collider>().Raycast(ray, out hitInfo, 10))
			{
				float fWidth = m_particlesArea.GetComponent<Renderer>().bounds.extents.x * 2f;
				float fRadius = (GetRadius() * m_particlesArea.GetWidth()) / fWidth;
				m_particlesArea.AddParticles(hitInfo.textureCoord, fRadius, m_strength * Time.deltaTime);
			}
		}
	}

	void DrawGizmo()
	{
		float col = m_strength / 10000.0f;
		Gizmos.color = Color.Lerp(Color.yellow, Color.red, col);
		Gizmos.DrawWireSphere(transform.position, GetRadius());
	}

	void OnDrawGizmosSelected()
	{
		DrawGizmo();
	}

	void OnDrawGizmos()
	{
		if (m_showGizmo)
		{
			DrawGizmo();
		}
	}
}