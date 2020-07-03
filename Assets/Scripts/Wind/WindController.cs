using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    private Vector4[,,] windArray;
    private int size;
    private Dictionary<int, WindData> windObjectsData;
    private Dictionary<int, Vector4[,,]> windArrayData;

    // Start is called before the first frame update
    void Start()
    {
        windObjectsData = new Dictionary<int, WindData>();
        windArrayData = new Dictionary<int, Vector4[,,]>();
        // size = 400;
        // windArray = new bool[size,size,size];
        windArray = new Vector4[1, 1, 1];

        // for (int i = 0; i < size; i++) {
        //     for (int j = 0; j < size; j++) {
        //         for (int k = 0; k < size; k+=8) {
        //             windArray[i, j, k + 0] = Vector4.zero;
        //             windArray[i, j, k + 1] = Vector4.zero;
        //             windArray[i, j, k + 2] = Vector4.zero;
        //             windArray[i, j, k + 3] = Vector4.zero;
        //             windArray[i, j, k + 4] = Vector4.zero;
        //             windArray[i, j, k + 5] = Vector4.zero;
        //             windArray[i, j, k + 6] = Vector4.zero;
        //             windArray[i, j, k + 7] = Vector4.zero;
        //         }
        //     }
        // }
    }

    public void doubleArraySize()
    {
        size = 300;
        Vector4[,,] newWindArray = new Vector4[size * 2, size * 2, size * 2];
        int halfSize = size / 2;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k += 8)
                {
                    newWindArray[i + halfSize, j + halfSize, k + 0] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 1] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 2] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 3] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 4] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 5] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 6] = windArray[i, j, k + 0];
                    newWindArray[i + halfSize, j + halfSize, k + 7] = windArray[i, j, k + 0];
                }
            }
        }


        for (int i = 0; i < halfSize; i++)
        {
            for (int j = 0; j < halfSize; j++)
            {
                for (int k = 0; k < halfSize; k += 8)
                {
                    newWindArray[i, j, k + 0] = Vector4.zero;
                    newWindArray[i, j, k + 1] = Vector4.zero;
                    newWindArray[i, j, k + 2] = Vector4.zero;
                    newWindArray[i, j, k + 3] = Vector4.zero;
                    newWindArray[i, j, k + 4] = Vector4.zero;
                    newWindArray[i, j, k + 5] = Vector4.zero;
                    newWindArray[i, j, k + 6] = Vector4.zero;
                    newWindArray[i, j, k + 7] = Vector4.zero;
                }
            }
        }

        size = size * 2;
        for (int i = halfSize; i < size; i++)
        {
            for (int j = halfSize; j < size; j++)
            {
                for (int k = halfSize; k < size; k += 8)
                {
                    newWindArray[i, j, k + 0] = Vector4.zero;
                    newWindArray[i, j, k + 1] = Vector4.zero;
                    newWindArray[i, j, k + 2] = Vector4.zero;
                    newWindArray[i, j, k + 3] = Vector4.zero;
                    newWindArray[i, j, k + 4] = Vector4.zero;
                    newWindArray[i, j, k + 5] = Vector4.zero;
                    newWindArray[i, j, k + 6] = Vector4.zero;
                    newWindArray[i, j, k + 7] = Vector4.zero;
                }
            }
        }
    }

    public void addObject(WindData data)
    {
        windObjectsData.Add(data.index, data);
        Vector4[ , , ] windArray = null;

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

    }

    private void deleteUpdateArray()
    {

    }

    private Vector4[ , , ] getCubeWindArray(CubeGeometryData cubegeometryData, float intensity, float interfence)
    {
        return new Vector4[1, 1, 1];
    }
    private Vector4[,,] getSphereWindArray(SphereGeometryData spheregeometryData, float intensity, float interfence)
    {
        return new Vector4[1, 1, 1];
    }
    private Vector4[,,] getCylinderWindArray(CylinderGeometryData cycleGeometryData, float intensity, float interfence)
    {
        return new Vector4[1, 1, 1];
    }
}
