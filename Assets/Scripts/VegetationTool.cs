using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class VegetationTool : MonoBehaviour {

	[Header("Global Param")]
	public int density = 20;
	public float posRandom = 0.3f;
	public float heightRandom = 0.1f;
	[Space()]
	[Header("Normal Target")]
	public bool useNormalTarget = false;
	public Vector3 normalTarget = new Vector3(0,1,0);
	public float AngleMargin = 10;

	[System.Serializable]
	public class VegeElement
	{
		public GameObject vegePrefab;
		public MinMax scaleChange = new MinMax(0.5f,1.5f);
		[Range(0,100)]
		public int proportion;
	}
	[Space()]
	[Header("Spawnable Elements")]
	public List<VegeElement> elements = new List<VegeElement>();

	public void InstantiateVegetation ()
	{
		SphereCollider col = GetComponent<SphereCollider>();
		Vector3 center = col.bounds.center;
		float radius = col.radius*transform.localScale.x;
		float rotator = 2*Mathf.PI / density;
		Vector3 newCenter = center+Vector3.up*radius*0.5f;

		RaycastHit hit;
		GameObject vegeGroup = new GameObject("vegetationGroup");
		vegeGroup.transform.parent = this.transform;

		float totalProportion = 0;
		foreach (VegeElement elem in elements) totalProportion += elem.proportion;
		List<int> proportions = new List<int>();
		for (int k = 0; k<elements.Count;k++)
		{
			proportions.Add(elements[k].proportion);
			if(k>0)
			{
				proportions[k] += proportions[k-1];
			}
		}

		for (int i = 1; i < density ; i++)
		{
			for (int j = 1; j<density/2; j++)
			{
				float x = center.x + radius * Mathf.Cos(rotator*i) * Mathf.Sin(rotator*j);
				float y = center.y + radius * Mathf.Sin(rotator*i) * Mathf.Sin(rotator*j);
				float z = center.z + radius * Mathf.Cos(rotator*j);
				Vector3 pointOnSphere = new Vector3(x,y,z);

				Vector3 dir = pointOnSphere - newCenter;
				float dist = Mathf.Abs((pointOnSphere-newCenter).magnitude);
				//Debug.DrawRay(newCenter,dir,Color.red,3);

				if (Physics.Raycast(newCenter, dir, out hit, dist))
				{
					Vector3 normal = hit.normal;
					if (!useNormalTarget || (useNormalTarget && Vector3.Angle (normal,normalTarget) < AngleMargin+1))
					{
						//choose which vege element should be used :
						float rand = Random.Range(0,totalProportion);
						int chosenElem = 0;
						for (int k = 0; k<elements.Count;k++)
						{
							if (rand < proportions[k])
							{
								chosenElem = k;
								break;
							}
							else
							{
								continue;
							}
						}
						GameObject instance = Instantiate(elements[chosenElem].vegePrefab, hit.point, Quaternion.FromToRotation(Vector3.up,normal)) as GameObject;
						instance.transform.localScale *= Random.Range(elements[chosenElem].scaleChange.min, elements[chosenElem].scaleChange.max);
						instance.transform.position += instance.transform.forward *Random.Range(-posRandom,posRandom) + instance.transform.right *Random.Range(-posRandom,posRandom) + transform.up*Random.Range(0,heightRandom);
						instance.transform.RotateAround(instance.transform.position,normal,Random.Range(0,360));
						instance.transform.parent = vegeGroup.transform;
					}
				}
			}
		}
	}
}
