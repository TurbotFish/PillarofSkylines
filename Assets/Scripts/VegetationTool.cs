using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class VegetationTool : MonoBehaviour {

	public int density = 20;

	public float posChange = 0.3f;


	[System.Serializable]
	public class VegeElement
	{
		public GameObject vegePrefab;
		public MinMax scaleChange = new MinMax(0.5f,1.5f);
		[Range(0,100)]
		public float proportion;
	}

	public List<VegeElement> elements = new List<VegeElement>();

	public void InstantiateVegetation ()
	{
		SphereCollider col = GetComponent<SphereCollider>();
		Vector3 center = col.bounds.center;
		float radius = col.radius*transform.localScale.x;
		float rotator = 2*Mathf.PI / density;

		RaycastHit hit;
		GameObject vegeGroup = new GameObject("vegetationGroup");
		vegeGroup.transform.parent = this.transform;

		float totalProportion = 0;
		foreach (VegeElement elem in elements) totalProportion += elem.proportion;

		for (int i = 1; i < density ; i++)
		{
			for (int j = 1; j<density/2; j++)
			{
				float x = center.x + radius * Mathf.Cos(rotator*i) * Mathf.Sin(rotator*j);
				float y = center.y + radius * Mathf.Sin(rotator*i) * Mathf.Sin(rotator*j);
				float z = center.z + radius * Mathf.Cos(rotator*j);
				Vector3 pointOnSphere = new Vector3(x,y,z);

				Vector3 dir = pointOnSphere - center;

				if (Physics.Raycast(center, dir, out hit, radius))
				{
					Vector3 normal = hit.normal;

					//choose which vege element should be used :
					float rand = Random.Range(0,totalProportion);
					//Debug.Log("rand : " + rand);
					int chosenElem = 0;
					for (int k = 0; k<elements.Count;k++)
					{
						float intermediaryTotal = 0;
						for (int l = 0; l <= k; l++)
						{
							intermediaryTotal += elements[l].proportion;
							//Debug.Log (intermediaryTotal);
						}

						if (rand < intermediaryTotal)
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
					instance.transform.position += instance.transform.forward *Random.Range(-posChange,posChange) + instance.transform.right *Random.Range(-posChange,posChange);
					instance.transform.RotateAround(instance.transform.position,normal,Random.Range(0,360));
					instance.transform.parent = vegeGroup.transform;
					//Debug.Log("pass");
				}
			}
		
		}

	}
}
