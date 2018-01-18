using UnityEngine;
using System.Collections;

[AddComponentMenu("Cocuy/Simulator")]
public class FluidSimulator : MonoBehaviour {
	int VELOCITY_READ = 0;
	int VELOCITY_WRITE = 1;

    int PRESSURE_READ = 0;
    int PRESSURE_WRITE = 1;

    public ComputeShader m_simulationShader;

    // Compute Shader Kernel Ids
    int m_addVelocityKernel = 0;
    int m_initBoundariesKernel = 0;
    int m_advectVelocityKernel = 0;
    int m_divergenceKernel = 0;
    int m_poissonKernel = 0;
    int m_substractGradientKernel = 0;
    int m_calcVorticityKernel = 0;
    int m_applyVorticityKernel = 0;
    int m_viscosityKernel = 0;
    int m_addObstacleCircleKernel = 0;
    int m_addObstacleTriangleKernel = 0;
    int m_clearBufferKernel = 0;

	public bool m_simulate = true;

	[HideInInspector]
	public float m_speed = 500f;
	public float Speed
	{
		get
		{
			return m_speed;
		}
		set
		{
			m_speed = value;
		}
	}

	[HideInInspector]
	public int m_iterations = 50;
	public int Iterations
	{
		get
		{
			return m_iterations;
		}
		set
		{
			m_iterations = value;
		}
	}

	[HideInInspector]
	public float m_velocityDissipation = 1f;
	public float VelocityDissipation
	{
		get
		{
			return m_velocityDissipation;
		}
		set
		{
			m_velocityDissipation = value;
		}
	}

	[HideInInspector]
	public float m_vorticityScale = 0f;
	public float Vorticity
	{
		get
		{
			return m_vorticityScale;
		}
		set
		{
			m_vorticityScale = value;
		}
	}

	[HideInInspector]
	public float m_viscosity = 0f;
	public float Viscosity
	{
		get
		{
			return m_viscosity;
		}
		set
		{
			m_viscosity = value;
		}
	}

	[HideInInspector]
	public int m_nResolution = 512;
	public int Resolution
	{
		get
		{
			return m_nResolution;
		}
		set
		{
			m_nResolution = value;
		}
	}

	ComputeBuffer[] m_velocityBuffer;
	public ComputeBuffer VelocityBuffer
	{
		get
		{
			return m_velocityBuffer[VELOCITY_READ];
		}
	}

	ComputeBuffer m_divergenceBuffer;
	public ComputeBuffer DivergenceBuffer
	{
		get
		{
			return m_divergenceBuffer;
		}
	}

	ComputeBuffer[] m_pressure;
	public ComputeBuffer PressureBuffer
	{
		get
		{
			return m_pressure[PRESSURE_READ];
		}
	}

	ComputeBuffer m_obstaclesBuffer;
	public ComputeBuffer ObstaclesBuffer
	{
		get
		{
			return m_obstaclesBuffer;
		}
	}

	ComputeBuffer m_vorticityBuffer;

    [HideInInspector]
    public bool m_cacheVelocity = false;
	Vector2[] m_currentVelocity;

	int m_nNumCells;
	int m_nNumGroupsX;
	int m_nNumGroupsY;
	int m_nWidth = 512;
	int m_nHeight = 512;

	//-------------------------------------------
	void Start()
	{
        if (SystemInfo.supportsComputeShaders)
        {
            m_nNumCells = m_nWidth * m_nHeight;
            m_addVelocityKernel = m_simulationShader.FindKernel("AddVelocity");
            m_initBoundariesKernel = m_simulationShader.FindKernel("InitBoundaries");
            m_advectVelocityKernel = m_simulationShader.FindKernel("AdvectVelocity");
            m_divergenceKernel = m_simulationShader.FindKernel("Divergence");
            m_clearBufferKernel = m_simulationShader.FindKernel("ClearBuffer");
            m_poissonKernel = m_simulationShader.FindKernel("Poisson");
            m_substractGradientKernel = m_simulationShader.FindKernel("SubstractGradient");
            m_calcVorticityKernel = m_simulationShader.FindKernel("CalcVorticity");
            m_applyVorticityKernel = m_simulationShader.FindKernel("ApplyVorticity");
            m_viscosityKernel = m_simulationShader.FindKernel("Viscosity");
            m_addObstacleCircleKernel = m_simulationShader.FindKernel("AddCircleObstacle");
            m_addObstacleTriangleKernel = m_simulationShader.FindKernel("AddTriangleObstacle");
        }
		else
		{
			m_simulate = false;
			Debug.LogError("<CocuyError> It seems your target Hardware does not support Compute Shaders. Cocuy needs support for Compute Shaders to work.");
		}
	}

	// Sets the size of the simulation
	// /nWidth: Width of the simulation
	// /nHeight: Height of the simulation
	public void SetSize(int nWidth, int nHeight)
	{
		uint groupSizeX = 8;
        uint groupSizeY = 8;
        uint groupSizeZ = 8;
        m_simulationShader.GetKernelThreadGroupSizes(0, out groupSizeX, out groupSizeY, out groupSizeZ);

        m_nWidth = nWidth;
		m_nHeight = nHeight;
		m_nNumCells = m_nWidth * m_nHeight;
		m_nNumGroupsX = Mathf.CeilToInt((float)m_nWidth / (float)groupSizeX);
		m_nNumGroupsY = Mathf.CeilToInt((float)m_nHeight / (float)groupSizeX);
	}

	// Get the width of the simulation
	public int GetWidth()
	{
		return m_nWidth;
	}

	// Get the height of the simulation
	public int GetHeight()
	{
		return m_nHeight;
	}

	// Adds a velocity splat to the simulation at /position with a specific /fRadius and /velocity.
	// /position: Centre of the splat as a normalised value [0-1] of the simulation space.
	// /fRadius: Radius of the splat in simulation space.
	// /velocity: Velocity at the centre of the splat. The value is linearly diffused towards the edges of the splat.
	public void AddVelocity(Vector2 position, Vector2 velocity, float fRadius)
	{
		if (m_simulationShader != null && m_velocityBuffer != null && m_velocityBuffer.Length >= 2)
		{
			float[] pos = { position.x, position.y };
            m_simulationShader.SetFloats("_Position", pos);
			float[] val = { velocity.x, velocity.y};
            m_simulationShader.SetFloats("_Value", val);
            m_simulationShader.SetFloat("_Radius", fRadius);
            m_simulationShader.SetInts("_Size", new int[] { m_nWidth, m_nHeight });
            m_simulationShader.SetBuffer(m_addVelocityKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
            m_simulationShader.SetBuffer(m_addVelocityKernel, "_VelocityOut", m_velocityBuffer[VELOCITY_WRITE]);
            m_simulationShader.Dispatch(m_addVelocityKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
            FlipVelocityBuffers();
		}
	}

    // Deprecated
	public void AddObstacle(Vector2 position, float fRadius)
	{
        AddObstacleCircle(position, fRadius);
	}

    // position in normalised local space
    // fRadius in world space
    public void AddObstacleCircle(Vector2 position, float fRadius, bool bStatic = false)
    { 
        float[] pos = { position.x, position.y };
        m_simulationShader.SetFloats("_Position", pos);
        m_simulationShader.SetFloat("_Radius", fRadius);
        m_simulationShader.SetInt("_Static", bStatic ? 1 : 0);
        m_simulationShader.SetBuffer(m_addObstacleCircleKernel, "_Obstacles", m_obstaclesBuffer);
        m_simulationShader.Dispatch(m_addObstacleCircleKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
    }

    // points in normalised local space
    public void AddObstacleTriangle(Vector2 p1, Vector2 p2, Vector2 p3, bool bStatic = false)
    {
        float[] pos1 = { p1.x, p1.y };
        float[] pos2 = { p2.x, p2.y };
        float[] pos3 = { p3.x, p3.y };
        m_simulationShader.SetFloats("_P1", pos1);
        m_simulationShader.SetFloats("_P2", pos2);
        m_simulationShader.SetFloats("_P3", pos3);
        m_simulationShader.SetInt("_Static", bStatic ? 1 : 0);
        m_simulationShader.SetBuffer(m_addObstacleTriangleKernel, "_Obstacles", m_obstaclesBuffer);
        m_simulationShader.Dispatch(m_addObstacleTriangleKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
    }

	// Get velocity at the (x,y) coordinate
	public Vector2 GetVelocity(int x, int y)
	{
		return m_currentVelocity[y * m_nWidth + x] * m_speed;
	}

	void UpdateParams()
	{
		m_simulationShader.SetFloat("_Dissipation", m_velocityDissipation);
        m_simulationShader.SetFloat("_ElapsedTime", Time.deltaTime);
        m_simulationShader.SetFloat("_Speed", m_speed);
        m_simulationShader.SetFloat("_VorticityScale", m_vorticityScale);

        float centreFactor = 1.0f / (m_viscosity);
        float stencilFactor = 1.0f / (4.0f + centreFactor);
        m_simulationShader.SetFloat("_Alpha", centreFactor);
        m_simulationShader.SetFloat("_rBeta", stencilFactor);
    }

	void Update()
	{
		if (m_simulate)
		{
			UpdateParams();
			CreateBuffersIfNeeded();

			// INIT BOUNDARIES
			m_simulationShader.SetBuffer(m_initBoundariesKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
            m_simulationShader.Dispatch(m_initBoundariesKernel, m_nNumGroupsX, m_nNumGroupsY, 1);

            // ADVECT
            m_simulationShader.SetBuffer(m_advectVelocityKernel, "_Obstacles", m_obstaclesBuffer);
            m_simulationShader.SetBuffer(m_advectVelocityKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
            m_simulationShader.SetBuffer(m_advectVelocityKernel, "_VelocityOut", m_velocityBuffer[VELOCITY_WRITE]);
            m_simulationShader.Dispatch(m_advectVelocityKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
            FlipVelocityBuffers();

            // VORTICITY CONFINEMENT
            // Calculate vorticity
            m_simulationShader.SetBuffer(m_calcVorticityKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
            m_simulationShader.SetBuffer(m_calcVorticityKernel, "_Vorticity", m_vorticityBuffer);
            m_simulationShader.Dispatch(m_calcVorticityKernel, m_nNumGroupsX, m_nNumGroupsY, 1);

			// Apply vorticity force
			m_simulationShader.SetBuffer(m_applyVorticityKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
			m_simulationShader.SetBuffer(m_applyVorticityKernel, "_Vorticity", m_vorticityBuffer);
			m_simulationShader.SetBuffer(m_applyVorticityKernel, "_VelocityOut", m_velocityBuffer[VELOCITY_WRITE]);
            m_simulationShader.Dispatch(m_applyVorticityKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
            FlipVelocityBuffers();

			// VISCOSITY
			if (m_viscosity > 0.0f)
			{
                for (int i = 0; i < m_iterations; ++i)
				{
                    m_simulationShader.SetBuffer(m_viscosityKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
                    m_simulationShader.SetBuffer(m_viscosityKernel, "_VelocityOut", m_velocityBuffer[VELOCITY_WRITE]);
                    m_simulationShader.Dispatch(m_viscosityKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
                    FlipVelocityBuffers();
				}
			}

			// DIVERGENCE
			m_simulationShader.SetBuffer(m_divergenceKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
			m_simulationShader.SetBuffer(m_divergenceKernel, "_Obstacles", m_obstaclesBuffer);
			m_simulationShader.SetBuffer(m_divergenceKernel, "_Divergence", m_divergenceBuffer);
            m_simulationShader.Dispatch(m_divergenceKernel, m_nNumGroupsX, m_nNumGroupsY, 1);

            // CLEAR PRESSURE
            m_simulationShader.SetBuffer(m_clearBufferKernel, "_Buffer", m_pressure[PRESSURE_READ]);
            m_simulationShader.Dispatch(m_clearBufferKernel, m_nNumGroupsX, m_nNumGroupsY, 1);

            // POISSON
            m_simulationShader.SetBuffer(m_poissonKernel, "_Divergence", m_divergenceBuffer);
            
            m_simulationShader.SetBuffer(m_poissonKernel, "_Obstacles", m_obstaclesBuffer);
            for (int i = 0; i < m_iterations; ++i)
			{
                m_simulationShader.SetBuffer(m_poissonKernel, "_PressureIn", m_pressure[PRESSURE_READ]);
                m_simulationShader.SetBuffer(m_poissonKernel, "_PressureOut", m_pressure[PRESSURE_WRITE]);
                m_simulationShader.Dispatch(m_poissonKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
				FlipPressureBuffers();
			}

			// SUBSTRACT GRADIENT
			m_simulationShader.SetBuffer(m_substractGradientKernel, "_PressureIn", m_pressure[PRESSURE_READ]);
			m_simulationShader.SetBuffer(m_substractGradientKernel, "_VelocityIn", m_velocityBuffer[VELOCITY_READ]);
			m_simulationShader.SetBuffer(m_substractGradientKernel, "_VelocityOut", m_velocityBuffer[VELOCITY_WRITE]);
			m_simulationShader.SetBuffer(m_substractGradientKernel, "_Obstacles", m_obstaclesBuffer);
            m_simulationShader.Dispatch(m_substractGradientKernel, m_nNumGroupsX, m_nNumGroupsY, 1);
            FlipVelocityBuffers();

            // CLEAR OBSTACLES
            m_simulationShader.SetBuffer(m_clearBufferKernel, "_Buffer", m_obstaclesBuffer);
            m_simulationShader.Dispatch(m_clearBufferKernel, m_nNumGroupsX, m_nNumGroupsY, 1);

			// Cache velocities
            if (m_cacheVelocity)
            {
                m_velocityBuffer[VELOCITY_READ].GetData(m_currentVelocity);
            }
		}
	}

	public void InitShaders()
	{
		CreateBuffersIfNeeded();
		UpdateParams();
		int[] size = new int[] { m_nWidth, m_nHeight };
		m_simulationShader.SetInts("_Size", size);
		m_currentVelocity = new Vector2[m_nNumCells];
	}

	void CreateBuffersIfNeeded()
	{
		if (m_velocityBuffer == null)
		{
			m_velocityBuffer = new ComputeBuffer[2];
			for (int i = 0; i < 2; ++i)
			{
				m_velocityBuffer[i] = new ComputeBuffer(m_nNumCells, 8, ComputeBufferType.Default);
			}
		}
		if (m_divergenceBuffer == null)
		{
			m_divergenceBuffer = new ComputeBuffer(m_nNumCells, 4, ComputeBufferType.Default);
		}
		if (m_pressure == null)
		{
			m_pressure = new ComputeBuffer[2];
			for (int i = 0; i < 2; ++i)
			{
				m_pressure[i] = new ComputeBuffer(m_nNumCells, 4, ComputeBufferType.Default);
			}
		}
		if (m_obstaclesBuffer == null)
		{
			m_obstaclesBuffer = new ComputeBuffer(m_nNumCells, 8, ComputeBufferType.Default);
		}
		if (m_vorticityBuffer == null)
		{
			m_vorticityBuffer = new ComputeBuffer(m_nNumCells, 4, ComputeBufferType.Default);
		}
	}

	public float GetSimulationSpeed()
	{
		return m_speed;
	}

	void OnDisable()
	{
		if (m_velocityBuffer != null && m_velocityBuffer.Length == 2)
		{
			if (m_velocityBuffer[0] != null)
			{
				m_velocityBuffer[0].Dispose();
			}
			if (m_velocityBuffer[1] != null)
			{
				m_velocityBuffer[1].Dispose();
			}
		}
		if (m_divergenceBuffer != null)
		{
			m_divergenceBuffer.Dispose();
		}
		if (m_pressure != null && m_pressure.Length == 2)
		{
			if (m_pressure[0] != null)
			{
				m_pressure[0].Dispose();
			}
			if (m_pressure[1] != null)
			{
				m_pressure[1].Dispose();
			}
		}
		if (m_obstaclesBuffer != null)
		{
			m_obstaclesBuffer.Dispose();
		}
		if (m_vorticityBuffer != null)
		{
			m_vorticityBuffer.Dispose();
		}
	}

	void FlipVelocityBuffers()
	{
        int aux = VELOCITY_READ;
        VELOCITY_READ = VELOCITY_WRITE;
        VELOCITY_WRITE = aux;
	}

    void FlipPressureBuffers()
    {
        int aux = PRESSURE_READ;
        PRESSURE_READ = PRESSURE_WRITE;
        PRESSURE_WRITE = aux;
    }
}
