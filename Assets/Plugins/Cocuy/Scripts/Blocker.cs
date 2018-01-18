using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Blocker")]
public class Blocker : MonoBehaviour {

    public FluidSimulator m_fluid;

	[HideInInspector]
	public bool m_useScaleAsSize = true;

	[HideInInspector]
	public float m_radius = 0.1f;

	[HideInInspector]
	public bool m_showGizmo = false;

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
        Ray ray = new Ray(transform.position, new Vector3(0, 0, 1));
        RaycastHit hitInfo = new RaycastHit();
        if (m_fluid.GetComponent<Collider>().Raycast(ray, out hitInfo, 10))
        {
            float fWidth = m_fluid.GetComponent<Renderer>().bounds.extents.x * 2f;
            //float fHeight = m_fluid.renderer.bounds.extents.z * 2f;
            float fRadius = (GetRadius() * m_fluid.GetWidth()) / fWidth;
            m_fluid.AddObstacleCircle(hitInfo.textureCoord, fRadius, false);
        }
	}

	void DrawGuidelines()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, GetRadius());
	}

	void OnDrawGizmosSelected()
	{
		DrawGuidelines();
	}

	void OnDrawGizmos()
	{
		if (m_showGizmo)
		{
			DrawGuidelines();
		}
	}
}
