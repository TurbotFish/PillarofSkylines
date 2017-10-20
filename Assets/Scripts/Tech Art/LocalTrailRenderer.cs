using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LocalTrailRenderer : MonoBehaviour {

	public int vertices = 200;

	public Material material {
		get { return line.material; }
		set { line.material = value; }
	}

	public Color startColor {
		get { return line.startColor; }
		set { line.startColor = value; }
	}

	public Color endColor {
		get { return line.endColor; }
		set { line.endColor = value; }
	}

	public float startWidth {
		get { return line.startWidth; }
		set { line.startWidth = value; }
	}
	public float endWidth {
		get { return line.endWidth; }
		set { line.endWidth = value; }
	}

	LineRenderer line;
	Vector3[] positions, fixedPositions;
	Transform _transform, _parent;

	void SetPosition(int index, Vector3 position) {
        positions[index] = position;
        position = _transform.InverseTransformPoint(_parent ? _parent.TransformPoint(position) : position);
        fixedPositions[index] = position;
        line.SetPosition(index, position);
    }
    
    void SetToNextPosition(int index) {
        line.SetPosition(index, fixedPositions[index + 1]);
    }

	void Awake() {
		_transform = transform;
        _parent = _transform.parent;
        line = GetComponent<LineRenderer>();
		line.positionCount = vertices;
		positions = new Vector3[vertices];
        fixedPositions = new Vector3[vertices];
        for (int i = 0; i < vertices; i++)
			SetPosition(i, _transform.localPosition);
		line.useWorldSpace = false;
	}

	void LateUpdate () {
        for (int i = 0; i < vertices - 1; i++)
            SetToNextPosition(i);//SetPosition(i, positions[i + 1]);;

        SetPosition(vertices - 1, _transform.localPosition);
	}

	void OnDestroy() {
		Destroy(line);
	}
}