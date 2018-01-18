using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Velocity Manipulator")]
[ExecuteInEditMode]
public class VelocityManipulator : MonoBehaviour
{
	public FluidSimulator m_fluid;

    [HideInInspector]
    public bool m_useScaleAsSize = true;
	
    [HideInInspector]
    public bool m_velocityFromMovement = false;

    [HideInInspector]
	public float m_fluidVelocitySpeed = 1f;

    [HideInInspector]
    public float m_scaleVelocity = 1f;

    [HideInInspector]
	public float m_radius = 0.1f;

    [HideInInspector]
    public bool m_showGizmo = false;

    Vector3 m_direction;
    Vector3 m_speed;
    Vector3 m_prevPosition;

	void Start()
	{
		m_prevPosition = transform.position;
        m_direction = GetDirection();
	}

    public float GetRadius()
    {
        if (m_useScaleAsSize)
        {
            return Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
        return m_radius;
    }

    public Vector3 GetDirection()
    {
        if (m_velocityFromMovement)
        {
            return transform.position - m_prevPosition;
        }
        return transform.rotation * Vector3.down;
    }

    void UpdateValues()
    {
        m_direction = GetDirection();
        if (m_direction != Vector3.zero)
        {
            m_direction.Normalize();
            m_speed = m_direction * m_fluidVelocitySpeed * Time.deltaTime;
        }
        else
        {
            m_speed = Vector3.zero;
        }
        m_prevPosition = transform.position;
    }

    void Update()
    {
        UpdateValues();
    }

	void LateUpdate()
	{
        if (m_fluid)
        {
            Vector3 currentPosition = transform.position;
            if (m_speed != Vector3.zero)
            {
                Ray ray = new Ray(currentPosition, new Vector3(0, 0, 1));
                RaycastHit hitInfo = new RaycastHit();
                if (m_fluid.GetComponent<Collider>().Raycast(ray, out hitInfo, 10))
                {
                    float fWidth = m_fluid.GetComponent<Renderer>().bounds.extents.x * 2f;
                    //float fHeight = m_fluid.renderer.bounds.extents.z * 2f;
                    float fRadius = (GetRadius() * m_fluid.GetWidth()) / fWidth;
                    m_fluid.AddVelocity(hitInfo.textureCoord, -m_speed, fRadius);
                }
            }
        }
	}

    void DrawGizmo()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetRadius());

        if (!m_velocityFromMovement || Application.isPlaying)
        {
            Vector3 end_pos = transform.position - (m_direction*(2f + (m_fluidVelocitySpeed/500f)*5f));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, end_pos);

            Vector3 back_dir = (transform.position - end_pos);
            back_dir.Normalize();

            float angle = 25 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            Vector3 arrow = new Vector3(back_dir.x * cos - back_dir.y * sin,
                                        back_dir.x * sin + back_dir.y * cos, 0f);
            Gizmos.DrawLine(end_pos, end_pos + arrow * 0.5f);

            cos = Mathf.Cos(-angle);
            sin = Mathf.Sin(-angle);
            arrow = new Vector3(back_dir.x * cos - back_dir.y * sin,
                                back_dir.x * sin + back_dir.y * cos, 0f);
            Gizmos.DrawLine(end_pos, end_pos + arrow * 0.5f);
        }
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