using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/PolygonBlocker")]
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonBlocker : MonoBehaviour {
	
    public FluidSimulator m_fluid;

	bool m_bInitialised = false;
	private PolygonCollider2D m_collider;

	void Start()
	{
		m_collider = GetComponent<PolygonCollider2D>();
	}

	void BlockFluid(bool bStatic)
	{
        if (m_collider && m_fluid)
        {
            Vector2[] points = m_collider.points;
            int size = points.Length;
            if (size >= 3)
            {
                Ray ray1 = new Ray(transform.TransformPoint(points[0]), new Vector3(0, 0, 1));
                RaycastHit h1 = new RaycastHit();
                if (m_fluid.GetComponent<Collider>().Raycast(ray1, out h1, 10))
                {
                    Ray ray2 = new Ray(transform.TransformPoint(points[1]), new Vector3(0, 0, 1));
                    RaycastHit h2 = new RaycastHit();
                    if (m_fluid.GetComponent<Collider>().Raycast(ray2, out h2, 10))
                    {
                        for (int i = 2; i < size; ++i)
                        {
                            Ray ray3 = new Ray(transform.TransformPoint(points[i]), new Vector3(0, 0, 1));
                            RaycastHit h3 = new RaycastHit();
                            if (m_fluid.GetComponent<Collider>().Raycast(ray3, out h3, 10))
                            {
                                m_fluid.AddObstacleTriangle(h1.textureCoord, h2.textureCoord, h3.textureCoord, bStatic);
                            }
                            h2 = h3;
                        }
                    }
                }
            }
        }
	}

	void LateUpdate()
	{
		if (!m_bInitialised)
		{
			if (gameObject.isStatic)
			{
				BlockFluid(true);
			}
			m_bInitialised = true;
		}
		if (!gameObject.isStatic)
		{
			BlockFluid(false);
		}
	}
}
