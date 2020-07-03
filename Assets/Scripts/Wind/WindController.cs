using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public GameObject smokeObject;
    private Vector3[,,] windArray;
    private int size;
    private Dictionary<int, WindData> windObjectsData;
    private Dictionary<int, Vector3[,,]> windArrayData;
    private Noise noise;

    // Start is called before the first frame update
    void Start()
    {
        windObjectsData = new Dictionary<int, WindData>();
        windArrayData = new Dictionary<int, Vector3[,,]>();
        size = 40;
        windArray = new Vector3[size,size,size];

        noise = new Noise();

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                for (int k = 0; k < size; k++) {
                    windArray[i, j, k] = Vector3.zero;
                }
            }
        }

    }

    void Update() {
    }

    public void doubleArraySize()
    {
        Vector3[,,] newWindArray = new Vector3[size * 2, size * 2, size * 2];
        int halfSize = size / 2;

        for (int i = halfSize; i < size * 2; i++)
        {
            for (int j = halfSize; j < size * 2; j++)
            {
                for (int k = halfSize; k < size * 2; k++)
                {
                    newWindArray[i, j, k] = Vector3.zero;
                }
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    newWindArray[i + halfSize, j + halfSize, k + halfSize] = windArray[i, j, k];
                }
            }
        }
        windArray = newWindArray;
    }

    public void addObject(WindData data)
    {
        windObjectsData.Add(data.index, data);
        Vector3[ , , ] windArray = null;

        switch (data.geometryData.geometryType)
        {
            case GeometryData.GeometryType.CUBE:
                windArray = getCubeWindArray((CubeGeometryData)data.geometryData, data.intensity, data.interference);
                break;
            case GeometryData.GeometryType.SPHERE:
                windArray = getSphereWindArray((SphereGeometryData)data.geometryData, data.intensity, data.interference);
                break;
            case GeometryData.GeometryType.CYLINDER:
                windArray = getCylinderWindArray((CylinderGeometryData)data.geometryData, data.intensity, data.interference);
                break;
            default:
                break;
        }
        windArrayData.Add(data.index, windArray);

        addUpdateArray(data.index);
    }

    public void setObject(WindData data)
    {
        windObjectsData.Remove(data.index);
        windArrayData.Remove(data.index);
        addObject(data);
    }

    public void deleteObject(WindData data)
    {
        windObjectsData.Remove(data.index);
        windArrayData.Remove(data.index);
        deleteUpdateArray();
    }

    private void addUpdateArray(int index)
    {
        GeometryData geometryData = windObjectsData[index].geometryData;
        Vector3 position = geometryData.position;
        Vector3[,,] objectArray = windArrayData[index];
        switch(geometryData.geometryType) {
            case GeometryData.GeometryType.CUBE: {
                CubeGeometryData cubeGeometryData = (CubeGeometryData)geometryData;
                int sizeX = (int)(cubeGeometryData.size.x / 0.1f);
                int sizeY = (int)(cubeGeometryData.size.y / 0.1f);
                int sizeZ = (int)(cubeGeometryData.size.z / 0.1f);
                int beginX = (int)((position.x + 2f ) / 0.1f) - sizeX / 2;
                int beginY = (int)((position.y + 2f ) / 0.1f) - sizeY / 2;
                int beginZ = (int)((position.z + 2f ) / 0.1f) - sizeZ / 2;
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeZ; j++)
                    {
                        for (int k = 0; k < sizeY; k++)
                        {
                            windArray[i + beginX, j + beginY, k + beginZ] = objectArray[i, j, k];
                        }
                    }
                }
                break;
            }
        }

        smokeObject.GetComponent<SmokeController>().setWindArray(windArray);
    }

    private void deleteUpdateArray()
    {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                for (int k = 0; k < size; k++) {
                    windArray[i, j, k] = Vector3.zero;
                }
            }
        }

        foreach(var item in windArrayData) {
            GeometryData geometryData = windObjectsData[item.Key].geometryData;
            Vector3 position = geometryData.position;
            Vector3[,,] objectArray = windArrayData[item.Key];
            switch(geometryData.geometryType) {
                case GeometryData.GeometryType.CUBE: {
                    CubeGeometryData cubeGeometryData = (CubeGeometryData)geometryData;
                    int sizeX = (int)(cubeGeometryData.size.x / 0.1f);
                    int sizeY = (int)(cubeGeometryData.size.y / 0.1f);
                    int sizeZ = (int)(cubeGeometryData.size.z / 0.1f);
                    int beginX = (int)((position.x) / 0.1f) - sizeX / 2 + size / 2;
                    int beginY = (int)((position.y) / 0.1f) - sizeY / 2 + size / 2;
                    int beginZ = (int)((position.z) / 0.1f) - sizeZ / 2 + size / 2;
                    for (int i = 0; i < sizeX; i++)
                    {
                        for (int j = 0; j < sizeZ; j++)
                        {
                            for (int k = 0; k < sizeY; k++)
                            {
                                windArray[i + beginX, j + beginY, k + beginZ] = objectArray[i, j, k];
                            }
                        }
                    }
                    break;
                }
            }
        }
        smokeObject.GetComponent<SmokeController>().setWindArray(windArray);
    }

    private Vector3[ , , ] getCubeWindArray(CubeGeometryData cubeGeometryData, float intensity, float interfence)
    {
        int sizeX = (int)(cubeGeometryData.size.x / 0.1f);
        int sizeY = (int)(cubeGeometryData.size.y / 0.1f);
        int sizeZ = (int)(cubeGeometryData.size.z / 0.1f);
        Vector3[,,] returnArray = new Vector3[sizeX, sizeY, sizeZ];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                double temp = noise.perlin(i, j, 7.89101112131415);
                // Debug.Log(temp - 0.5);
                double noiseValue = intensity * interfence * (temp - 0.5);

                Vector3 wind = cubeGeometryData.direction;
                wind = wind * intensity;
                // Debug.Log(wind);

                Vector3 noiseVector = new Vector3(0, 1, 0) * (float)noiseValue;
                noiseVector = Quaternion.AngleAxis(90, Vector3.up) * noiseVector;

                // Debug.Log(noiseVector);

                wind = wind + noiseVector;

                for (int k = 0; k < sizeY; k++)
                {
                    returnArray[i, j, k] = wind;
                }
            }
        }
        return returnArray;
    }
    private Vector3[,,] getSphereWindArray(SphereGeometryData spheregeometryData, float intensity, float interfence)
    {
        return new Vector3[1, 1, 1];
    }
    private Vector3[,,] getCylinderWindArray(CylinderGeometryData cycleGeometryData, float intensity, float interfence)
    {
        return new Vector3[1, 1, 1];
    }
}
