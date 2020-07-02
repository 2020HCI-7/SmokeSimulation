using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogModel
{
    public FogParticleData particles { get; protected set; }
    public NeighborhoodSearch m_neighborhoodSearch { get; protected set; } 
        = new NeighborhoodSearch();	
    public List<Vector3> m_boundaryX { get; protected set;}
        = new List<Vector3>();
    public List<float> m_boundaryPsi { get; protected set;}
        = new List<float>();
    public float m_density0 { get; protected set; }
    public List<float> m_lambda { get; protected set;}
        = new List<float>();		

    //protected float viscosity; // 黏度
    protected float m_particleRadius;
    protected float m_supportRadius;
    protected List<float> m_density = new List<float>();
    protected List<Vector3> m_deltaX = new List<Vector3>();

    public FogModel()
    {
        m_density0 = 1000.0f;
        m_particleRadius = 0.025f;
        //viscosity = static_cast<Real>(0.02);
        m_neighborhoodSearch = null;
        particles = new FogParticleData();
    }

    ~FogModel()
    {
        CleanupModel();
    }

    public void CleanupModel()
    {
        particles.Release();
        m_lambda.Clear();
        m_density.Clear();
        m_deltaX.Clear();
        m_neighborhoodSearch = null;
    }
    public void Reset()
    {
        int nPoints = particles.Size();

        for (int i = 0; i < nPoints; i++) 
        {
            Vector3 x0 = particles.GetPosition0(i);
            particles.SetPosition(i, x0);
            particles.SetLastPosition(i, particles.GetPosition(i));
            particles.SetOldPosition(i, particles.GetPosition(i));
            particles.SetVelocity(i, Vector3.zero);
            particles.SetAcceleration(i, Vector3.zero);
            m_deltaX[i] = Vector3.zero;
            m_lambda[i] = 0.0f;
            m_density[i] = 0.0f;
        }
    }
    public void InitModel(int nFluidParticles, List<Vector3> fluidParticles, int nBoundaryParticles, List<Vector3> boundaryParticles)
    {
        ReleaseParticles();
        ResizeParticles(nFluidParticles);

        // init kernel
        CubicKernel.setRadius(m_supportRadius);

        // copy fluid positions
        for (int i = 0; i < nFluidParticles; i++) 
        {
            particles.SetPosition0(i, fluidParticles[i]);
        }

        Utilities.ResizeList(m_boundaryX, nBoundaryParticles);
        Utilities.ResizeList(m_boundaryPsi, nBoundaryParticles);

        // copy boundary positions
		for (int i = 0; i < (int)nBoundaryParticles; i++)
		{
			m_boundaryX[i] = boundaryParticles[i];
		}

        // initialize masses
        InitMasses();

        // Search boundary neighborhood
        NeighborhoodSearch neighborhoodSearchSH = new NeighborhoodSearch(nBoundaryParticles, m_supportRadius);
        neighborhoodSearchSH.neighborhoodSearch(m_boundaryX);
        
        int[][] neighbors = neighborhoodSearchSH.getNeighbors();
        int[] numNeighbors = neighborhoodSearchSH.getNumNeighbors();

        for (int i = 0; i < (int) nBoundaryParticles; i++)
        {
            // TODO
            //float delta = CubicKernel::W_zero();
            float delta = 0.0f;
            for (int j = 0; j < numNeighbors[i]; j++)
            {
                int neighborIndex = neighbors[i][j];
                //delta += CubicKernel::W(m_boundaryX[i] - m_boundaryX[neighborIndex]);
            }
            float volume = 1.0f / delta;
            m_boundaryPsi[i] = m_density0 * volume;
        }


        // Initialize neighborhood search
        if (m_neighborhoodSearch == null)
            m_neighborhoodSearch = new NeighborhoodSearch(particles.Size(), m_supportRadius);
        m_neighborhoodSearch.setRadius(m_supportRadius);

        Reset();
    }

    protected void InitMasses()
    {
        int nParticles = particles.Size();
        float diam = 2.0f * m_particleRadius;

        for (int i = 0; i < nParticles; i++) 
        {
            particles.SetMass(i, 0.8f * diam * diam * diam * m_density0);
        }
    }
    protected void ResizeParticles(int newSize) 
    {
        particles.Resize(newSize);
        Utilities.ResizeList(m_lambda, newSize);
        Utilities.ResizeList(m_density, newSize);
        Utilities.ResizeList(m_deltaX, newSize);
    }
    protected void ReleaseParticles() 
    {
        particles.Release();
        m_lambda.Clear();
        m_density.Clear();
        m_deltaX.Clear();
    }

    public void AddParticle(Vector3 position, Vector3 velocity)
    {
        int count = particles.Size();
        ResizeParticles(count+1);
        particles.SetPosition0(count, position);
        particles.SetVelocity(count, velocity);
    }

    public void DeleteParticle(int index)
    {
        m_lambda.RemoveAt(index);
        m_density.RemoveAt(index);
        m_deltaX.RemoveAt(index);
        particles.DeleteParticle(index);
    }

    public void setParticleRadius(float val) 
    { 
        m_particleRadius = val; 
        m_supportRadius = 4.0f * m_particleRadius; 
    }
    public float getParticleRadius()
    {
        return m_particleRadius;
    }

    public Vector3 getDeltaX(int i)
    {
        return m_deltaX[i];
    }
    public void setDeltaX(int i, Vector3 val)
    {
        m_deltaX[i] = val;
    }

    public int numBoundaryParticles()
    {
        return m_boundaryX.Count;
    }

    public float getDensity(int i)
    {
        return m_density[i];
    }
    public void setDensity(int i, float val)
    {
        m_density[i] = val;
    }

    public float getLambda(int i)
    {
        return m_lambda[i];
    }
    public void setLambda(int i, float val)
    {
        m_lambda[i] = val;
    }

}
