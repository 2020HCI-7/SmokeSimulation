using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public static FogController instance;

    public float particleRadius = 0.025f;  // 流体粒子半径

    // // 初始状态粒子在三个维度的数量分布
    // public int width = 15;
    // public int depth = 15;
    // public int height = 15;

    // 包围盒的长宽高
    // public float containerWidth = 100.0f;
    // public float containerDepth = 0.8f;
    // public float containerHeight = 4.0f;
    public Vector3 center = Vector3.zero;
    public Vector3 maxSize = new Vector3(10.0f, 10.0f, 10.0f);
    private float unitSize = 0.2f;
    // 烟雾源实例
    public FogModel model;
    public int m_velocityUpdateMethod = 0;
    private List<GameObject> particleGameobjects = new List<GameObject>();
    private Transform particleParent;
    private int generateIndex = 0;

    public int[][][] density;

    void Awake() {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitFogParticles();
        InitBoundary();
    }

    // Update is called once per frame
    void Update()
    {
        Step();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < particleGameobjects.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(particleGameobjects[i].transform.position, particleRadius);
            // float border = 0.1f;
            // Gizmos.DrawCube(new Vector3(containerWidth/2, 0, 0), new Vector3(border, containerHeight, containerDepth));
            // Gizmos.DrawCube(new Vector3(-containerWidth/2, 0, 0), new Vector3(border, containerHeight, containerDepth));
            // Gizmos.DrawCube(new Vector3(0, containerHeight/2, 0), new Vector3(containerHeight, border, containerDepth));
            // Gizmos.DrawCube(new Vector3(0, -containerHeight/2, 0), new Vector3(containerHeight, border, containerDepth));
            // Gizmos.DrawCube(new Vector3(0, 0, containerDepth/2), new Vector3(containerHeight, containerHeight, border));
            // Gizmos.DrawCube(new Vector3(0, 0, -containerDepth/2), new Vector3(containerHeight, containerHeight, border));
        }
    }

#region 初始化
    private List<Vector3> boundaryParticles = new List<Vector3>();
    void InitFogParticles()
    {
        Debug.Log("Initialize fluid particles");
        model = new FogModel();

        // float diam = 2.0f * particleRadius;
        // float startX = -0.5f * containerWidth + diam;
        // float startY = diam;
        // float startZ = -0.5f * containerDepth + diam;
        float yshift = Mathf.Sqrt(3) * particleRadius;

        List<Vector3> fluidParticles = new List<Vector3>();
        // Utilities.ResizeList(fluidParticles, width * height * depth);
        // Utilities.ResizeList(particleGameobjects, width * height * depth);

        particleParent = GameObject.Find("Particles").transform;
        // for (int i = 0; i < width; i++) {
        //     for (int j = 0; j < height; j++) {
        //         for (int k = 0; k < depth; k++) {
        //             Vector3 position = diam * new Vector3(i, j, k) + new Vector3(startX, startY, startZ);
        //             fluidParticles[i * height * depth + j * depth + k] = position;
                    
        //             GameObject particle = new GameObject();
        //             particle.name = "particle" + i.ToString() + j.ToString() + k.ToString();
        //             particle.transform.position = position;
        //             particle.transform.SetParent(particleParent);
        //             particleGameobjects[i * height * depth + j * depth + k] = particle;
        //         }
        //     }
        // }

        InitDensity();
        
        model.setParticleRadius(particleRadius);

        model.InitModel(fluidParticles.Count, fluidParticles, boundaryParticles.Count, boundaryParticles);

        // Debug.Log("Number of particles: " + (width*height*depth).ToString());
    }

    void InitDensity()
    {
        // Debug.Log((int)(maxSize.x/unitSize));
        density = new int[(int)(maxSize.x/unitSize+1)][][];
        for (int i = 0; i < (int)(maxSize.x/unitSize+1); i++)
        {
            density[i] = new int[(int)(maxSize.y/unitSize+1)][];
            for (int j = 0; j < (int)(maxSize.y/unitSize+1); j++)
            {
                density[i][j] = new int[(int)(maxSize.z/unitSize+1)];
            }
        }
    }

    void InitBoundary()
    {
        // float x1 = -containerWidth / 2.0f;
        // float x2 = containerWidth / 2.0f;
        // float y1 = 0.0f;
        // float y2 = containerHeight;
        // float z1 = -containerDepth / 2.0f;
        // float z2 = containerDepth / 2.0f;
        float x1 = center.x - maxSize.x / 2.0f;
        float x2 = center.x + maxSize.x / 2.0f;
        float y1 = center.y - maxSize.y / 2.0f;
        float y2 = center.y + maxSize.y / 2.0f;
        float z1 = center.z - maxSize.z / 2.0f;
        float z2 = center.z + maxSize.z / 2.0f;

        float diam = 2.0f * particleRadius;

        // Floor
        AddWall(new Vector3(x1, y1, z1), new Vector3(x2, y1, z2));
        // Top
        AddWall(new Vector3(x1, y2, z1), new Vector3(x2, y2, z2));
        // Left
        AddWall(new Vector3(x1, y1, z1), new Vector3(x1, y2, z2));
        // Right
        AddWall(new Vector3(x2, y1, z1), new Vector3(x2, y2, z2));
        // Back
        AddWall(new Vector3(x1, y1, z1), new Vector3(x2, y2, z1));
        // Front
        AddWall(new Vector3(x1, y1, z2), new Vector3(x2, y2, z2));
    }

    private void AddWall(Vector3 minX, Vector3 maxX)
    {
        float particleDistance = 2.0f * model.getParticleRadius();

        Vector3 diff = maxX - minX;

        int stepsX = (int)(diff.x / particleDistance) + 1;
        int stepsY = (int)(diff.y / particleDistance) + 1;
        int stepsZ = (int)(diff.z / particleDistance) + 1;

        int startIndex = boundaryParticles.Count;
        Utilities.ResizeList(boundaryParticles, startIndex + stepsX*stepsY*stepsZ);

        for (int j = 0; j < stepsX; j++) {
            for (int k = 0; k < stepsY; k++) {
                for (int l = 0; l < stepsZ; l++) {
                    Vector3 currPos = minX + new Vector3(j*particleDistance, k*particleDistance, l*particleDistance);
                    boundaryParticles[startIndex + j*stepsY*stepsZ + k*stepsZ + l] = currPos;
                }
            }
        }
    }

#endregion

#region 模拟

    void Step()
    {
        float h = Time.fixedDeltaTime;
        FogParticleData pd = model.particles;

        //Debug.Log(h);

        ClearAccelerations();

        for (int i = 0; i < Random.Range(0, 20); i++)
        {
            GenerateParticle();
        }

        for (int i = 0; i < pd.Size(); i++) 
        {
            model.setDeltaX(i, Vector3.zero);
            pd.SetLastPosition(i, pd.GetOldPosition(i));
            pd.SetOldPosition(i, pd.GetPosition(i));
            Vector3 velocity = pd.GetVelocity(i) + h * pd.GetAcceleration(i);
            pd.SetVelocity(i, velocity);
            Vector3 position = pd.GetPosition(i) + h * pd.GetVelocity(i);
            pd.SetPosition(i, position);
            particleGameobjects[i].transform.position = position;
            
            Vector3 delta = position - center;
            if (delta.x > maxSize.x/2 || delta.x < -maxSize.x/2 || 
                delta.y > maxSize.y/2 || delta.y < -maxSize.y/2 ||
                delta.z > maxSize.z/2 || delta.z < -maxSize.z/2)
            {
                model.DeleteParticle(i);
                Destroy(particleGameobjects[i]);
                particleGameobjects.RemoveAt(i);
                i = i - 1;
            }
        }

        UpdateDensity();

        // neighborhood search
        model.m_neighborhoodSearch.neighborhoodSearch(
            //pd.GetPosition(0), 
            pd.m_x,
            model.numBoundaryParticles(), 
            model.m_boundaryX
            );

        // Solve density constraint
        ConstraintProjection();

        // Update velocities
        for (int i = 0; i < pd.Size(); i++)
        {
            if (m_velocityUpdateMethod == 0)
            {
                VelocityUpdateFirstOrder(i, h, pd.getMass(i), pd.GetPosition(i), pd.GetOldPosition(i));
            }
            else
            {
                VelocityUpdateSecondOrder(i, h, pd.getMass(i), pd.GetPosition(i), pd.GetOldPosition(i), pd.GetLastPosition(i));
            }
        }

        // ComputeXSPHViscosity();

        model.m_neighborhoodSearch.update();
    }

    void GenerateParticle()
    {
        // float diam = 2.0f * particleRadius;
        // float startX = -0.5f * containerWidth + diam;
        // float startY = diam;
        // float startZ = -0.5f * containerDepth + diam;

        Vector3 position = center;
        Vector3 velocity = 
            (new Vector3(
                Random.Range(0.0f, 1.0f),
                Random.Range(-1.0f, 1.0f), 
                Random.Range(-1.0f, 1.0f))).normalized * Random.Range(0, 0.5f);
        model.AddParticle(position, velocity);
        
        GameObject particle = new GameObject();
        particle.name = "particle" + generateIndex.ToString();
        particle.transform.position = position;
        particle.transform.SetParent(particleParent);
        particleGameobjects.Add(particle);
        generateIndex++;
    }

    void UpdateDensity()
    {
        FogParticleData pd = model.particles;

        // 清空
        for (int i = 0; i < maxSize.x; i++)
        {
            for (int j = 0; j < maxSize.y; j++)
            {
                for (int k = 0; k < maxSize.z; k++)
                {
                    density[i][j][k] = 0;
                }
            }
        }

        for (int i = 0; i < pd.Size(); i++) 
        {
            Vector3 delta = pd.m_x[i] - center + maxSize/2.0f;
            // Debug.Log((int)(delta.x/unitSize));
            // Debug.Log((int)(delta.y/unitSize));
            // Debug.Log((int)(delta.z/unitSize));
            density[(int)(delta.x/unitSize)][(int)(delta.y/unitSize)][(int)(delta.z/unitSize)]++;
        }
    }

    void ClearAccelerations()
    {
        FogParticleData pd = model.particles;
        int count = pd.Size();
        // Vector3 grav = new Vector3(0, -9.8f, 0);
        Vector3 grav = new Vector3(0, 0.0f, 0);
        for (int i=0; i < count; i++)
        {
            if (pd.getMass(i) != 0.0f)
            {
                pd.SetAcceleration(i, grav);
            }
        }
    }

    void ConstraintProjection()
    {
        int maxIter = 5;
        int iter = 0;
        FogParticleData pd = model.particles;
        int nParticles = pd.Size();
        int[][] neighbors = model.m_neighborhoodSearch.getNeighbors();
        int[] numNeighbors = model.m_neighborhoodSearch.getNumNeighbors();

        while (iter < maxIter)
        {
            // float avg_density_err = 0.0f;

            for (int i = 0; i < nParticles; i++) 
            {
                ComputePBFDensity(
                    i,
                    nParticles,
                    pd.m_x,
                    pd.m_masses,
                    model.m_boundaryX,
                    model.m_boundaryPsi,
                    numNeighbors[i],
                    neighbors[i],
                    model.m_density0,
                    true
                );
                ComputePBFLagrangeMultiplier(
                    i,
                    nParticles,
                    pd.m_x,
                    pd.m_masses,
                    model.m_boundaryX,
                    model.m_boundaryPsi,
                    model.getDensity(i),
                    numNeighbors[i],
                    neighbors[i],
                    model.m_density0,
                    true
                );
            }

            for (int i = 0; i < nParticles; i++)
            {
                SolveDensityConstraint(
                    i,
                    nParticles,
                    pd.m_x,
                    pd.m_masses,
                    model.m_boundaryX,
                    model.m_boundaryPsi,
                    numNeighbors[i],
                    neighbors[i],
                    model.m_density0,
                    true,
                    model.m_lambda
                );
            }

            for (int i = 0; i < nParticles; i++)
            {
                pd.SetPosition(i, pd.GetPosition(i) + model.getDeltaX(i));
            }

            iter++;
        }
    }

    bool ComputePBFDensity(
        int particleIndex,
        int numberOfParticles,
        List<Vector3> x,
        List<float> mass,
        List<Vector3> boundaryX,
        List<float> boundaryPsi,
        int numNeighbors,
        int[] neighbors,
        float density0,
        bool boundaryHandling
        )
    {
        float density = mass[particleIndex] * CubicKernel.W_zero();
        for (int j = 0; j < numNeighbors; j++)
        {
            int neighborIndex = neighbors[j];
            if (neighborIndex < numberOfParticles)
            {
                density += mass[neighborIndex] * CubicKernel.W(x[particleIndex] - x[neighborIndex]);
            }
            else if (boundaryHandling)
            {
                density += boundaryPsi[neighborIndex - numberOfParticles] * 
                    CubicKernel.W(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
            }
        }

        model.setDensity(particleIndex, density);
        return true;
    }

    bool ComputePBFLagrangeMultiplier(
        int particleIndex,
        int numberOfParticles,
        List<Vector3> x,
        List<float> mass,
        List<Vector3> boundaryX,
        List<float> boundaryPsi,
        float density,
        int numNeighbors,
        int[] neighbors,
        float density0,
        bool boundaryHandling
        )
    {
        float c = Mathf.Max(density / density0 - 1.0f, 0.0f);
        float lambda;

        if (c != 0.0f)
        {
            float sum_grad_C2 = 0.0f;
            Vector3 gradC_i = Vector3.zero;

            for (int j = 0; j < numNeighbors; j++)
            {
                int neighborIndex = neighbors[j];
                if (neighborIndex < numberOfParticles)
                {
                    Vector3 gradC_j = -mass[neighborIndex] / density0 * 
                        CubicKernel.gradW(x[particleIndex] - x[neighborIndex]);
                    sum_grad_C2 += gradC_j.magnitude * gradC_j.magnitude;
                    gradC_i -= gradC_j;
                }
                else if (boundaryHandling)
                {
                    Vector3 gradC_j = -boundaryPsi[neighborIndex - numberOfParticles] / density0 * 
                        CubicKernel.gradW(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
                    sum_grad_C2 += gradC_j.magnitude * gradC_j.magnitude;
                    gradC_i -= gradC_j;
                }
            }

            sum_grad_C2 += gradC_i.magnitude * gradC_i.magnitude;

		    // Compute lambda
            lambda = -c / (sum_grad_C2);
        }
        else
        {
            lambda = 0.0f;
        }

        model.setLambda(particleIndex, lambda);
        return true;
    }

    bool SolveDensityConstraint(
        int particleIndex,
        int numberOfParticles,
        List<Vector3> x,
        List<float> mass,
        List<Vector3> boundaryX,
        List<float> boundaryPsi,
        int numNeighbors,
        int[] neighbors,
        float density0,
        bool boundaryHandling,
        List<float> lambda
        )
    {
        Vector3 corr = Vector3.zero;
        for (int j = 0; j < numNeighbors; j++) 
        {
            int neighborIndex = neighbors[j];
            if (neighborIndex < numberOfParticles)
            {
                Vector3 gradC_j = -mass[neighborIndex] / density0 * CubicKernel.gradW(x[particleIndex] - x[neighborIndex]);
                corr -= (lambda[particleIndex] + lambda[neighborIndex]) * gradC_j;
            }
            else if (boundaryHandling)
            {
                Vector3 gradC_j = -boundaryPsi[neighborIndex - numberOfParticles] / density0 
                    * CubicKernel.gradW(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
                corr -= (lambda[particleIndex]) * gradC_j;
            }
        }
        model.setDeltaX(particleIndex, corr);

        return true;
    }

    void VelocityUpdateFirstOrder(
        int particleIndex,
        float h,
        float mass,
        Vector3 position,
        Vector3 oldPosition
        )
    {
        FogParticleData pd = model.particles;
        if (mass != 0.0f)
        {
            pd.SetVelocity(particleIndex, 
                (1.0f / h) * (position - oldPosition)
                );
        }
    }

    void VelocityUpdateSecondOrder(
        int particleIndex,
        float h,
        float mass,
        Vector3 position,
        Vector3 oldPosition,
        Vector3 positionOfLastStep
        )
    {
        FogParticleData pd = model.particles;
        if (mass != 0.0f)
        {
            pd.SetVelocity(particleIndex, 
                (1.0f / h) * (1.5f * position - 2.0f * oldPosition + 0.5f * positionOfLastStep)
            );
        }
    }

    // 计算黏度，暂时不用
    // void ComputeXSPHViscosity()
    // {
    //     FogParticleData pd = model.particles;
    //     int numParticles = pd.Size();

    //     int[][] neighbors = model.m_neighborhoodSearch.getNeighbors();
    //     int[] numNeighbors = model.m_neighborhoodSearch.getNumNeighbors();

    //     //float viscosity = model.

    // }

#endregion

}
