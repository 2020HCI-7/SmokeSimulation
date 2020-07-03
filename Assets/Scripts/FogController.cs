using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{

    public float particleRadius = 0.025f;  // 流体粒子半径

    // // 初始状态粒子在三个维度的数量分布
    // public int width = 15;
    // public int depth = 15;
    // public int height = 15;

    // 包围盒的长宽高
    // public float containerWidth = 100.0f;
    // public float containerDepth = 0.8f;
    // public float containerHeight = 4.0f;
    public Vector3 maxSize = new Vector3(10.0f, 10.0f, 10.0f);
    public float unitSize = 0.1f;
    // 烟雾源实例
    public FogModel model;
    public int m_velocityUpdateMethod = 0;
    // private List<GameObject> particleGameobjects = new List<GameObject>();
    private Transform particleParent;
    private int generateIndex = 0;

    public int[][][] density;
    public int windsize=0;
    Vector3[,,] windarray;
    public ComputeShader PBFDensityCS;
    public ComputeShader PBFLagrangeMultiplierCS;
    public ComputeShader SolveDensityConstraintCS;
    public int gsx;
    public int gsy;
    public int gsz;
    public float rate = 10.0f;
    public Texture3D volume;
    public Vector3 center = Vector3.zero;
    public SmokeData data;

    // Start is called before the first frame update
    void Start()
    {
        // gs = (int)(maxSize.x / unitSize);
        // windsize=10;
        // windarray=new Vector4[windsize,windsize,windsize];
        // for(int i=0;i<windsize;++i)
        // {
        //     for(int j=0;j<windsize;++j)
        //     {
                
        //         for(int k=0;k<windsize;++k)
        //         {
        //             windarray[i,j,k]=new Vector4(0.0f,0.1f,0.0f,0.0f);
        //         }
        //     }
        // }
        // InitFogParticles();
        // InitBoundary();
    }

    private bool isInit = false;

    public void Init(SmokeData data)
    {
        // gs = (int)(maxSize.x / unitSize);
        gsx = (int)(maxSize.x / unitSize);
        gsy = (int)(maxSize.y / unitSize);
        gsz = (int)(maxSize.z / unitSize);
        windsize=40;
        windarray=new Vector3[windsize,windsize,windsize];
        for(int i=0;i<windsize;++i)
        {
            for(int j=0;j<windsize;++j)
            {
                
                for(int k=0;k<windsize;++k)
                {
                    windarray[i,j,k]=new Vector3(0.0f,5f,0.0f);
                }
            }
        }
        InitFogParticles();
        InitBoundary();
        this.data = data;
        this.center = data.geometryData.position;
        isInit = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInit)
        {
            Step();
            UpdateMat();
        }
    }

    private void OnDestory() {
        XBuffer.Release();;
        massBuffer.Release();;
        boundaryXBuffer.Release();;
        boundaryPsiBuffer.Release();;
        numNeighborsBuffer.Release();;
        neighborsBuffer.Release();;
        densityBuffer.Release();;
        lambdaBuffer.Release();;
    }

    void UpdateMat()
    {
        var colors = new Color[gsx * gsy * gsz];
        
        for (int i = 0; i < gsx; ++i)
        {
            for (int j = 0; j < gsy; ++j)
            {
                for (int k = 0; k < gsz; ++k)
                {
                    if (density[i][j][k] > rate * 0.3f)
                    {
                        colors[i + j * gsx + k * gsx * gsy] = new Color(data.color.r, data.color.g, data.color.b, 0.3f);
                    }
                    else
                    {
                        colors[i + j * gsx + k * gsx * gsy] = new Color(data.color.r, data.color.g, data.color.b, density[i][j][k] / rate);
                    }
                }
            }
        }

        volume = new Texture3D(gsx, gsy, gsz, TextureFormat.RGBA32, false);
        volume.SetPixels(colors, 0);

        volume.Apply();
        
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.SetTexture("_Volume", volume);
    }
    private void OnDrawGizmos()
    {
        // for (int i = 0; i < particleGameobjects.Count; i++)
        // {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawSphere(particleGameobjects[i].transform.position, particleRadius);
        //     // float border = 0.1f;
        //     // Gizmos.DrawCube(new Vector3(containerWidth/2, 0, 0), new Vector3(border, containerHeight, containerDepth));
        //     // Gizmos.DrawCube(new Vector3(-containerWidth/2, 0, 0), new Vector3(border, containerHeight, containerDepth));
        //     // Gizmos.DrawCube(new Vector3(0, containerHeight/2, 0), new Vector3(containerHeight, border, containerDepth));
        //     // Gizmos.DrawCube(new Vector3(0, -containerHeight/2, 0), new Vector3(containerHeight, border, containerDepth));
        //     // Gizmos.DrawCube(new Vector3(0, 0, containerDepth/2), new Vector3(containerHeight, containerHeight, border));
        //     // Gizmos.DrawCube(new Vector3(0, 0, -containerDepth/2), new Vector3(containerHeight, containerHeight, border));
        // }
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

        // particleParent = GameObject.Find("Particles").transform;
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

        ClearAccelerations();

        // for (int i = 0; i < Random.Range(0, 20); i++)
        for (int i = 0; i < 30; i++)
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
            // particleGameobjects[i].transform.position = position;
            
            Vector3 delta = position - center;
            if (delta.x > maxSize.x/2 || delta.x < -maxSize.x/2 || 
                delta.y > maxSize.y/2 || delta.y < -maxSize.y/2 ||
                delta.z > maxSize.z/2 || delta.z < -maxSize.z/2)
            {
                model.DeleteParticle(i);
                // Destroy(particleGameobjects[i]);
                // particleGameobjects.RemoveAt(i);
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

        //add wind force
        for(int i=0;i<pd.Size();i++)
        {
            Vector3 pos=pd.GetPosition(i);
            Vector3 windforce=windarray[
                (int)((pos.x-center.x+maxSize.x/2)/maxSize.x*windsize),
                (int)((pos.y-center.y+maxSize.y/2)/maxSize.y*windsize),
                (int)((pos.z-center.z+maxSize.z/2)/maxSize.z*windsize)];
            pd.SetVelocity(i,pd.GetVelocity(i)+h*new Vector3(windforce.x,windforce.y,windforce.z));
            pd.SetVelocity(i,pd.GetVelocity(i)+h*new Vector3(0.0f,0.1f,0.0f));
        }
        // ComputeXSPHViscosity();
        
        model.m_neighborhoodSearch.update();
    }
    
    void GenerateParticle()
    {
        Vector3 position;
        Vector3 velocity;
        switch (data.geometryData.geometryType)
        {
            case GeometryData.GeometryType.CONE :
            {
                Vector3 direction = ((ConeGeometryData)data.geometryData).direction;
                float r = ((ConeGeometryData)data.geometryData).r;
                float height = ((ConeGeometryData)data.geometryData).height;
                
                float randomX;
                float randomY;
                float randomZ;
                if (direction.z != 0.0f)
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = -(direction.x * randomX + direction.y * randomY) / direction.z;
                }
                else if (direction.y != 0.0f)
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = -(direction.x * randomX) / direction.y;
                    randomZ = Random.Range(-1.0f, 1.0f);
                }
                else if (direction.x != 0.0f)
                {
                    randomX = 0.0f;
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = Random.Range(-1.0f, 1.0f);
                }
                else
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = Random.Range(-1.0f, 1.0f);
                }

                position = center;
                velocity = 
                    (direction * height + (new Vector3(randomX, randomY, randomZ)).normalized * Random.Range(0, r)).normalized
                    * data.physicalData.speed;
                break;
            }
            case GeometryData.GeometryType.CYCLE :
            {
                Vector3 direction = ((CycleGeometryData)data.geometryData).direction;
                float r = ((CycleGeometryData)data.geometryData).r;

                float randomX;
                float randomY;
                float randomZ;
                if (direction.z != 0.0f)
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = -(direction.x * randomX + direction.y * randomY) / direction.z;
                }
                else if (direction.y != 0.0f)
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = -(direction.x * randomX) / direction.y;
                    randomZ = Random.Range(-1.0f, 1.0f);
                }
                else if (direction.x != 0.0f)
                {
                    randomX = 0.0f;
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = Random.Range(-1.0f, 1.0f);
                }
                else
                {
                    randomX = Random.Range(-1.0f, 1.0f);
                    randomY = Random.Range(-1.0f, 1.0f);
                    randomZ = Random.Range(-1.0f, 1.0f);
                }
                
                position = center + (new Vector3(randomX, randomY, randomZ)).normalized * Random.Range(0, r);
                velocity = direction * data.physicalData.speed;
                break;
            }
            default :
            {
                position = center;
                velocity = 
                    (new Vector3(
                        Random.Range(0.0f, 1.0f),
                        Random.Range(-1.0f, 1.0f), 
                        Random.Range(-1.0f, 1.0f))).normalized * data.physicalData.speed;
                break;
            }
        }
        
        // position = center;
        // velocity = 
        //     (new Vector3(
        //         Random.Range(0.0f, 1.0f),
        //         Random.Range(-1.0f, 1.0f), 
        //         Random.Range(-1.0f, 1.0f))).normalized * data.physicalData.speed;

        model.AddParticle(position, velocity);
        
        // GameObject particle = new GameObject();
        // particle.name = "particle" + generateIndex.ToString();
        // particle.transform.position = position;
        // particle.transform.SetParent(particleParent);
        // particleGameobjects.Add(particle);
        // generateIndex++;
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
            
            ComputePBFDensity(
                nParticles,
                pd.m_x,
                pd.m_masses,
                model.m_boundaryX,
                model.m_boundaryPsi,
                numNeighbors,
                neighbors,
                model.m_density0,
                true
            );
            ComputePBFLagrangeMultiplier(
                nParticles,
                pd.m_x,
                pd.m_masses,
                model.m_boundaryX,
                model.m_boundaryPsi,
                model.m_density,
                numNeighbors,
                neighbors,
                model.m_density0,
                true
            );
            // for (int i = 0; i < nParticles; i++) 
            // {
            //     ComputePBFDensity(
            //         i,
            //         nParticles,
            //         pd.m_x,
            //         pd.m_masses,
            //         model.m_boundaryX,
            //         model.m_boundaryPsi,
            //         numNeighbors[i],
            //         neighbors[i],
            //         model.m_density0,
            //         true
            //     );
            //     ComputePBFLagrangeMultiplier(
            //         i,
            //         nParticles,
            //         pd.m_x,
            //         pd.m_masses,
            //         model.m_boundaryX,
            //         model.m_boundaryPsi,
            //         model.getDensity(i),
            //         numNeighbors[i],
            //         neighbors[i],
            //         model.m_density0,
            //         true
            //     );
            // }

            // SolveDensityConstraint(
            //     nParticles,
            //     pd.m_x,
            //     pd.m_masses,
            //     model.m_boundaryX,
            //     model.m_boundaryPsi,
            //     numNeighbors,
            //     neighbors,
            //     model.m_density0,
            //     true,
            //     model.m_lambda
            // );
            // for (int i = 0; i < nParticles; i++)
            // {
            //     SolveDensityConstraint(
            //         i,
            //         nParticles,
            //         pd.m_x,
            //         pd.m_masses,
            //         model.m_boundaryX,
            //         model.m_boundaryPsi,
            //         numNeighbors[i],
            //         neighbors[i],
            //         model.m_density0,
            //         true,
            //         model.m_lambda
            //     );
            // }

            // for (int i = 0; i < nParticles; i++)
            // {
            //     Debug.Log(model.getDeltaX(i));
            //     pd.SetPosition(i, pd.GetPosition(i) + model.getDeltaX(i));
            // }

            iter++;
        }
    }

    // bool ComputePBFDensity(
    //     int particleIndex,
    //     int numberOfParticles,
    //     List<Vector3> x,
    //     List<float> mass,
    //     List<Vector3> boundaryX,
    //     List<float> boundaryPsi,
    //     int numNeighbors,
    //     int[] neighbors,
    //     float density0,
    //     bool boundaryHandling
    //     )
    // {
    //     float density = mass[particleIndex] * CubicKernel.W_zero();
    //     for (int j = 0; j < numNeighbors; j++)
    //     {
    //         int neighborIndex = neighbors[j];
    //         if (neighborIndex < numberOfParticles)
    //         {
    //             density += mass[neighborIndex] * CubicKernel.W(x[particleIndex] - x[neighborIndex]);
    //         }
    //         else if (boundaryHandling)
    //         {
    //             density += boundaryPsi[neighborIndex - numberOfParticles] * 
    //                 CubicKernel.W(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
    //         }
    //     }

    //     model.setDensity(particleIndex, density);
    //     return true;
    // }
    ComputeBuffer XBuffer;
    ComputeBuffer massBuffer;
    ComputeBuffer boundaryXBuffer;
    ComputeBuffer boundaryPsiBuffer;
    ComputeBuffer numNeighborsBuffer;
    ComputeBuffer neighborsBuffer;
    ComputeBuffer densityBuffer;

    bool ComputePBFDensity(
        int numberOfParticles,
        List<Vector3> x,
        List<float> mass,
        List<Vector3> boundaryX,
        List<float> boundaryPsi,
        int[] numNeighbors,
        int[][] neighbors,
        float density0,
        bool boundaryHandling
        )
    {
        if (XBuffer == null || XBuffer.count != numberOfParticles)
        {
            if (XBuffer != null) 
                XBuffer.Release();
            if (massBuffer != null) 
                massBuffer.Release();
            if (boundaryXBuffer != null) 
                boundaryXBuffer.Release();
            if (boundaryPsiBuffer != null) 
                boundaryPsiBuffer.Release();
            if (numNeighborsBuffer != null) 
                numNeighborsBuffer.Release();
            if (neighborsBuffer != null) 
                neighborsBuffer.Release();
            if (densityBuffer != null)
                densityBuffer.Release();
            XBuffer = new ComputeBuffer(numberOfParticles, 12);
            massBuffer = new ComputeBuffer(numberOfParticles, 4);
            boundaryXBuffer = new ComputeBuffer(numberOfParticles, 12);
            boundaryPsiBuffer = new ComputeBuffer(numberOfParticles, 4);
            numNeighborsBuffer = new ComputeBuffer(numberOfParticles, 4);
            neighborsBuffer = new ComputeBuffer(numberOfParticles, 240);
            densityBuffer = new ComputeBuffer(numberOfParticles, 4);
        }

        int[] tempNeighbors = new int[numberOfParticles*60];
        int index = 0;
        for (int i = 0; i < numberOfParticles; i++)
        {
            for (int j = 0; j < 60; j++)
            {
                tempNeighbors[index++] = neighbors[i][j];
            }
        }

        XBuffer.SetData(x);
        massBuffer.SetData(mass);
        boundaryXBuffer.SetData(boundaryX);
        boundaryPsiBuffer.SetData(boundaryPsi);
        numNeighborsBuffer.SetData(numNeighbors);
        neighborsBuffer.SetData(tempNeighbors);

        var kernel = PBFDensityCS.FindKernel("CSMain");
        PBFDensityCS.SetFloat("m_radius", CubicKernel.m_radius);
        PBFDensityCS.SetFloat("m_k", CubicKernel.m_k);
        PBFDensityCS.SetFloat("m_l", CubicKernel.m_l);
        PBFDensityCS.SetFloat("m_W_zero", CubicKernel.m_W_zero);
        PBFDensityCS.SetInt("numberOfParticles", numberOfParticles);
        PBFDensityCS.SetBool("boundaryHandling", true);

        PBFDensityCS.SetBuffer(kernel, "x", XBuffer);
        PBFDensityCS.SetBuffer(kernel, "mass", massBuffer);
        PBFDensityCS.SetBuffer(kernel, "boundaryX", boundaryXBuffer);
        PBFDensityCS.SetBuffer(kernel, "boundaryPsi", boundaryPsiBuffer);
        PBFDensityCS.SetBuffer(kernel, "numNeighbors", numNeighborsBuffer);
        PBFDensityCS.SetBuffer(kernel, "neighbors", neighborsBuffer);

        PBFDensityCS.SetBuffer(kernel, "density", densityBuffer);

        PBFDensityCS.Dispatch(kernel, 8, 1, 1);

        float[] density = new float[numberOfParticles];
        densityBuffer.GetData(density);

        for (int i = 0; i < numberOfParticles; i++) 
        {
            model.setDensity(i, density[i]);
        }

        return true;
    }

    // bool ComputePBFLagrangeMultiplier(
    //     int particleIndex,
    //     int numberOfParticles,
    //     List<Vector3> x,
    //     List<float> mass,
    //     List<Vector3> boundaryX,
    //     List<float> boundaryPsi,
    //     float density,
    //     int numNeighbors,
    //     int[] neighbors,
    //     float density0,
    //     bool boundaryHandling
    //     )
    // {
    //     float c = Mathf.Max(density / density0 - 1.0f, 0.0f);
    //     float lambda;

    //     if (c != 0.0f)
    //     {
    //         float sum_grad_C2 = 0.0f;
    //         Vector3 gradC_i = Vector3.zero;

    //         for (int j = 0; j < numNeighbors; j++)
    //         {
    //             int neighborIndex = neighbors[j];
    //             if (neighborIndex < numberOfParticles)
    //             {
    //                 Vector3 gradC_j = -mass[neighborIndex] / density0 * 
    //                     CubicKernel.gradW(x[particleIndex] - x[neighborIndex]);
    //                 sum_grad_C2 += gradC_j.magnitude * gradC_j.magnitude;
    //                 gradC_i -= gradC_j;
    //             }
    //             else if (boundaryHandling)
    //             {
    //                 Vector3 gradC_j = -boundaryPsi[neighborIndex - numberOfParticles] / density0 * 
    //                     CubicKernel.gradW(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
    //                 sum_grad_C2 += gradC_j.magnitude * gradC_j.magnitude;
    //                 gradC_i -= gradC_j;
    //             }
    //         }

    //         sum_grad_C2 += gradC_i.magnitude * gradC_i.magnitude;

	// 	    // Compute lambda
    //         lambda = -c / (sum_grad_C2);
    //     }
    //     else
    //     {
    //         lambda = 0.0f;
    //     }

    //     model.setLambda(particleIndex, lambda);
    //     return true;
    // }

    ComputeBuffer lambdaBuffer;

    bool ComputePBFLagrangeMultiplier(
        int numberOfParticles,
        List<Vector3> x,
        List<float> mass,
        List<Vector3> boundaryX,
        List<float> boundaryPsi,
        List<float> m_density,
        int[] numNeighbors,
        int[][] neighbors,
        float density0,
        bool boundaryHandling
        )
    {
        if (XBuffer == null || XBuffer.count != numberOfParticles)
        {
            if (XBuffer != null) 
                XBuffer.Release();
            if (massBuffer != null) 
                massBuffer.Release();
            if (boundaryXBuffer != null) 
                boundaryXBuffer.Release();
            if (boundaryPsiBuffer != null) 
                boundaryPsiBuffer.Release();
            if (numNeighborsBuffer != null) 
                numNeighborsBuffer.Release();
            if (neighborsBuffer != null) 
                neighborsBuffer.Release();
            if (densityBuffer != null)
                densityBuffer.Release();
            XBuffer = new ComputeBuffer(numberOfParticles, 12);
            massBuffer = new ComputeBuffer(numberOfParticles, 4);
            boundaryXBuffer = new ComputeBuffer(numberOfParticles, 12);
            boundaryPsiBuffer = new ComputeBuffer(numberOfParticles, 4);
            numNeighborsBuffer = new ComputeBuffer(numberOfParticles, 4);
            neighborsBuffer = new ComputeBuffer(numberOfParticles, 240);
            densityBuffer = new ComputeBuffer(numberOfParticles, 4);
        }
        
        if (lambdaBuffer == null || lambdaBuffer.count != numberOfParticles)
        {
            if (lambdaBuffer != null)
                lambdaBuffer.Release();
            lambdaBuffer = new ComputeBuffer(numberOfParticles, 4);
        }

        int[] tempNeighbors = new int[numberOfParticles*60];
        int index = 0;
        for (int i = 0; i < numberOfParticles; i++)
        {
            for (int j = 0; j < 60; j++)
            {
                tempNeighbors[index++] = neighbors[i][j];
            }
        }

        XBuffer.SetData(x);
        massBuffer.SetData(mass);
        boundaryXBuffer.SetData(boundaryX);
        boundaryPsiBuffer.SetData(boundaryPsi);
        numNeighborsBuffer.SetData(numNeighbors);
        neighborsBuffer.SetData(tempNeighbors);
        densityBuffer.SetData(m_density);

        var kernel = PBFLagrangeMultiplierCS.FindKernel("CSMain");
        PBFDensityCS.SetFloat("m_radius", CubicKernel.m_radius);
        PBFDensityCS.SetFloat("m_k", CubicKernel.m_k);
        PBFDensityCS.SetFloat("m_l", CubicKernel.m_l);
        PBFDensityCS.SetFloat("m_W_zero", CubicKernel.m_W_zero);
        PBFDensityCS.SetFloat("density0", density0);
        PBFDensityCS.SetInt("numberOfParticles", numberOfParticles);
        PBFDensityCS.SetBool("boundaryHandling", true);

        PBFDensityCS.SetBuffer(kernel, "x", XBuffer);
        PBFDensityCS.SetBuffer(kernel, "mass", massBuffer);
        PBFDensityCS.SetBuffer(kernel, "boundaryX", boundaryXBuffer);
        PBFDensityCS.SetBuffer(kernel, "boundaryPsi", boundaryPsiBuffer);
        PBFDensityCS.SetBuffer(kernel, "numNeighbors", numNeighborsBuffer);
        PBFDensityCS.SetBuffer(kernel, "neighbors", neighborsBuffer);
        PBFDensityCS.SetBuffer(kernel, "density", densityBuffer);

        PBFDensityCS.SetBuffer(kernel, "lambda", lambdaBuffer);

        PBFDensityCS.Dispatch(kernel, 8, 1, 1);

        float[] lambdaArray = new float[numberOfParticles];
        lambdaBuffer.GetData(lambdaArray);

        for (int i = 0; i < numberOfParticles; i++) 
        {
            model.setLambda(i, lambdaArray[i]);
        }

        return true;
    }

    // bool SolveDensityConstraint(
    //     int particleIndex,
    //     int numberOfParticles,
    //     List<Vector3> x,
    //     List<float> mass,
    //     List<Vector3> boundaryX,
    //     List<float> boundaryPsi,
    //     int numNeighbors,
    //     int[] neighbors,
    //     float density0,
    //     bool boundaryHandling,
    //     List<float> lambda
    //     )
    // {
    //     Vector3 corr = Vector3.zero;
    //     for (int j = 0; j < numNeighbors; j++) 
    //     {
    //         int neighborIndex = neighbors[j];
    //         if (neighborIndex < numberOfParticles)
    //         {
    //             Vector3 gradC_j = -mass[neighborIndex] / density0 * CubicKernel.gradW(x[particleIndex] - x[neighborIndex]);
    //             corr -= (lambda[particleIndex] + lambda[neighborIndex]) * gradC_j;
    //         }
    //         else if (boundaryHandling)
    //         {
    //             Vector3 gradC_j = -boundaryPsi[neighborIndex - numberOfParticles] / density0 
    //                 * CubicKernel.gradW(x[particleIndex] - boundaryX[neighborIndex - numberOfParticles]);
    //             corr -= (lambda[particleIndex]) * gradC_j;
    //         }
    //     }
    //     model.setDeltaX(particleIndex, corr);

    //     return true;
    // }
    // ComputeBuffer deltaXBuffer;
    // bool SolveDensityConstraint(
    //     int numberOfParticles,
    //     List<Vector3> x,
    //     List<float> mass,
    //     List<Vector3> boundaryX,
    //     List<float> boundaryPsi,
    //     int[] numNeighbors,
    //     int[][] neighbors,
    //     float density0,
    //     bool boundaryHandling,
    //     List<float> m_lambda
    //     )
    // {
    //     if (XBuffer == null || XBuffer.count != numberOfParticles)
    //     {
    //         if (XBuffer != null) 
    //             XBuffer.Release();
    //         if (massBuffer != null) 
    //             massBuffer.Release();
    //         if (boundaryXBuffer != null) 
    //             boundaryXBuffer.Release();
    //         if (boundaryPsiBuffer != null) 
    //             boundaryPsiBuffer.Release();
    //         if (numNeighborsBuffer != null) 
    //             numNeighborsBuffer.Release();
    //         if (neighborsBuffer != null) 
    //             neighborsBuffer.Release();
    //         if (lambdaBuffer != null)
    //             lambdaBuffer.Release();
    //         XBuffer = new ComputeBuffer(numberOfParticles, 12);
    //         massBuffer = new ComputeBuffer(numberOfParticles, 4);
    //         boundaryXBuffer = new ComputeBuffer(numberOfParticles, 12);
    //         boundaryPsiBuffer = new ComputeBuffer(numberOfParticles, 4);
    //         numNeighborsBuffer = new ComputeBuffer(numberOfParticles, 4);
    //         neighborsBuffer = new ComputeBuffer(numberOfParticles, 240);
    //         lambdaBuffer = new ComputeBuffer(numberOfParticles, 4);
    //     }
        
    //     if (deltaXBuffer == null || deltaXBuffer.count != numberOfParticles)
    //     {
    //         if (deltaXBuffer != null)
    //             deltaXBuffer.Release();
    //         deltaXBuffer = new ComputeBuffer(numberOfParticles, 12);
    //     }

    //     int[] tempNeighbors = new int[numberOfParticles*60];
    //     int index = 0;
    //     for (int i = 0; i < numberOfParticles; i++)
    //     {
    //         for (int j = 0; j < 60; j++)
    //         {
    //             tempNeighbors[index++] = neighbors[i][j];
    //         }
    //     }

    //     XBuffer.SetData(x);
    //     massBuffer.SetData(mass);
    //     boundaryXBuffer.SetData(boundaryX);
    //     boundaryPsiBuffer.SetData(boundaryPsi);
    //     numNeighborsBuffer.SetData(numNeighbors);
    //     neighborsBuffer.SetData(tempNeighbors);
    //     lambdaBuffer.SetData(m_lambda);

    //     var kernel = SolveDensityConstraintCS.FindKernel("CSMain");
    //     PBFDensityCS.SetFloat("m_radius", CubicKernel.m_radius);
    //     PBFDensityCS.SetFloat("m_k", CubicKernel.m_k);
    //     PBFDensityCS.SetFloat("m_l", CubicKernel.m_l);
    //     PBFDensityCS.SetFloat("m_W_zero", CubicKernel.m_W_zero);
    //     PBFDensityCS.SetFloat("density0", density0);
    //     PBFDensityCS.SetInt("numberOfParticles", numberOfParticles);
    //     PBFDensityCS.SetBool("boundaryHandling", true);

    //     PBFDensityCS.SetBuffer(kernel, "x", XBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "mass", massBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "boundaryX", boundaryXBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "boundaryPsi", boundaryPsiBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "numNeighbors", numNeighborsBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "neighbors", neighborsBuffer);
    //     PBFDensityCS.SetBuffer(kernel, "lambda", lambdaBuffer);

    //     PBFDensityCS.SetBuffer(kernel, "deltaX", deltaXBuffer);

    //     PBFDensityCS.Dispatch(kernel, 8, 1, 1);

    //     Vector3[] deltaX = new Vector3[numberOfParticles];
    //     deltaXBuffer.GetData(deltaX);

    //     for (int i = 0; i < numberOfParticles; i++) 
    //     {
    //         model.setDeltaX(i, deltaX[i]);
    //     }

    //     return true;
    // }

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
    public int getSmokeDensity(Vector3 position)
    {
        Vector3 delta = position - center + maxSize / 2.0f;
        return density[(int)(delta.x / unitSize)][(int)(delta.y / unitSize)][(int)(delta.z / unitSize)];
    }

    public void setWindArray(Vector3[,,] _windArray)
    {
        windarray = _windArray;
    }

    public void setBarrierData(Dictionary<int, BarrierData> barrierData)
    {
        
    }

#endregion

}
