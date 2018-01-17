using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Fluid Follower")]
public class FluidFollower : MonoBehaviour {
	public FluidSimulator m_fluid;
	public ParticlesArea m_particleArea;

    void Start()
    {
        if (m_fluid && !m_fluid.m_cacheVelocity)
        {
            Debug.LogWarning("<CocuyWarning> \"Cache Velocity\" must be set to true on the FluidSumulator component to use the Fluid Follower.");
        }
    }
	
    
    void LateUpdate()
	{
		if (m_fluid)
		{
			Ray ray = new Ray(gameObject.transform.position, new Vector3(0, 0, 1));
			RaycastHit hitInfo = new RaycastHit();
			if (m_fluid.GetComponent<Collider>().Raycast(ray, out hitInfo, 10))
			{
				Vector2 simSize = new Vector2(m_fluid.GetWidth(), m_fluid.GetHeight());
				Vector2 posInSimSpace = new Vector2(hitInfo.textureCoord.x * simSize.x, hitInfo.textureCoord.y * simSize.y);
				Vector2 velInSimSpace = m_fluid.GetVelocity((int)posInSimSpace.x, (int)posInSimSpace.y) * Time.deltaTime;
                Vector2 worldSize = m_particleArea.GetRenderSize();
				Vector2 velInWorldSpace = new Vector2((velInSimSpace.x * worldSize.x) / simSize.x, (velInSimSpace.y * worldSize.y) / simSize.y);
				gameObject.transform.position = gameObject.transform.position + new Vector3(velInWorldSpace.x, velInWorldSpace.y, 0f);
			}
		}
	}
}