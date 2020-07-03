using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public GameObject cubeObject;
    public GameObject cylinderObject;
    public GameObject sphereObject;

    public Dictionary<int, GameObject> barrierObjects;
    private bool[ , , ] barrierArray;
    private int size;
    private Dictionary<int, BarrierData> barrierObjectsData;

    // Start is called before the first frame update
    void Start()
    {
        barrierObjects = new Dictionary<int, GameObject>();
        barrierObjectsData = new Dictionary<int, BarrierData>();
        // size = 400;
        // barrierArray = new bool[size,size,size];
        barrierArray = new bool[1, 1, 1];

        // for (int i = 0; i < size; i++) {
        //     for (int j = 0; j < size; j++) {
        //         for (int k = 0; k < size; k+=8) {
        //             barrierArray[i, j, k + 0] = false;
        //             barrierArray[i, j, k + 1] = false;
        //             barrierArray[i, j, k + 2] = false;
        //             barrierArray[i, j, k + 3] = false;
        //             barrierArray[i, j, k + 4] = false;
        //             barrierArray[i, j, k + 5] = false;
        //             barrierArray[i, j, k + 6] = false;
        //             barrierArray[i, j, k + 7] = false;
        //         }
        //     }
        // }
    }

    public void doubleArraySize()
    {
        size = 300;
        bool[ , , ] newBarrierArray = new bool[size * 2, size * 2, size * 2];
        int halfSize = size / 2;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k += 8)
                {
                    newBarrierArray[i + halfSize, j + halfSize, k + 0] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 1] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 2] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 3] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 4] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 5] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 6] = barrierArray[i, j, k + 0];
                    newBarrierArray[i + halfSize, j + halfSize, k + 7] = barrierArray[i, j, k + 0];
                }
            }
        }

        for (int i = 0; i < halfSize; i++)
        {
            for (int j = 0; j < halfSize; j++)
            {
                for (int k = 0; k < halfSize; k += 8)
                {
                    newBarrierArray[i, j, k + 0] = false;
                    newBarrierArray[i, j, k + 1] = false;
                    newBarrierArray[i, j, k + 2] = false;
                    newBarrierArray[i, j, k + 3] = false;
                    newBarrierArray[i, j, k + 4] = false;
                    newBarrierArray[i, j, k + 5] = false;
                    newBarrierArray[i, j, k + 6] = false;
                    newBarrierArray[i, j, k + 7] = false;
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
                    newBarrierArray[i, j, k + 0] = false;
                    newBarrierArray[i, j, k + 1] = false;
                    newBarrierArray[i, j, k + 2] = false;
                    newBarrierArray[i, j, k + 3] = false;
                    newBarrierArray[i, j, k + 4] = false;
                    newBarrierArray[i, j, k + 5] = false;
                    newBarrierArray[i, j, k + 6] = false;
                    newBarrierArray[i, j, k + 7] = false;
                }
            }
        }
    }

    public void addObject(BarrierData data)
    {
        barrierObjectsData.Add(data.index, data);

        GameObject newObject = null;
        Quaternion quaternion = Quaternion.identity;
        Vector3 position = data.geometryData.position;
        
        switch(data.geometryData.geometryType) {
            case GeometryData.GeometryType.CUBE:
                quaternion = Quaternion.Euler(((CubeGeometryData)data.geometryData).direction);
                newObject = Instantiate(cubeObject, position, quaternion, transform);
                newObject.transform.localScale = ((CubeGeometryData)data.geometryData).size;
                break;
            case GeometryData.GeometryType.SPHERE:               
                newObject = Instantiate(sphereObject, position, quaternion, transform);
                float r = ((SphereGeometryData)data.geometryData).r;
                newObject.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
                break;
            case GeometryData.GeometryType.CYLINDER: 
                quaternion = Quaternion.Euler(((CylinderGeometryData)data.geometryData).direction);
                newObject = Instantiate(cylinderObject, position, quaternion, transform);
                newObject.transform.localScale = new Vector3(((CylinderGeometryData)data.geometryData).r * 2, ((CylinderGeometryData)data.geometryData).height,((CylinderGeometryData)data.geometryData).r);
                break;
            default:
                break;
        }
        
        barrierObjects.Add(data.index, newObject);
        
        addUpdateArray();
    }

    public void setObject(BarrierData data)
    {
        
        BarrierData oldData = barrierObjectsData[data.index];
        GameObject barrierObject = barrierObjects[data.index];
        if(oldData.geometryData.geometryType == data.geometryData.geometryType) {
            switch (data.geometryData.geometryType)
            {
                case GeometryData.GeometryType.CUBE:
                    barrierObject.transform.position = ((CubeGeometryData)data.geometryData).position;
                    barrierObject.transform.localEulerAngles = ((CubeGeometryData)data.geometryData).direction;
                    barrierObject.transform.localScale = ((CubeGeometryData)data.geometryData).size;
                    break;
                case GeometryData.GeometryType.SPHERE:
                    barrierObject.transform.position = ((SphereGeometryData)data.geometryData).position;
                    float r = ((SphereGeometryData)data.geometryData).r;
                    barrierObject.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
                    break;
                case GeometryData.GeometryType.CYLINDER:
                    barrierObject.transform.position = ((CylinderGeometryData)data.geometryData).position;
                    barrierObject.transform.localEulerAngles = ((CylinderGeometryData)data.geometryData).direction;
                    barrierObject.transform.localScale = new Vector3(((CylinderGeometryData)data.geometryData).r * 2, ((CylinderGeometryData)data.geometryData).height, ((CylinderGeometryData)data.geometryData).r);
                    break;
                default:
                    break;
            }
            barrierObjectsData[data.index] = data;
        }
        else {
            barrierObjects.Remove(data.index);
            Destroy(barrierObject);

            GameObject newObject = null;
            Quaternion quaternion = Quaternion.identity;
            Vector3 position = data.geometryData.position;

            switch (data.geometryData.geometryType)
            {
                case GeometryData.GeometryType.CUBE:
                    quaternion = Quaternion.Euler(((CubeGeometryData)data.geometryData).direction);
                    newObject = Instantiate(cubeObject, position, quaternion, transform);
                    newObject.transform.localScale = ((CubeGeometryData)data.geometryData).size;
                    break;
                case GeometryData.GeometryType.SPHERE:
                    newObject = Instantiate(cylinderObject, position, quaternion, transform);
                    float r = ((SphereGeometryData)data.geometryData).r;
                    newObject.transform.localScale = new Vector3(r * 2, r * 2, r * 2);
                    break;
                case GeometryData.GeometryType.CYLINDER:
                    quaternion = Quaternion.Euler(((CylinderGeometryData)data.geometryData).direction);
                    newObject = Instantiate(cubeObject, position, quaternion, transform);
                    newObject.transform.localScale = new Vector3(((CylinderGeometryData)data.geometryData).r * 2, ((CylinderGeometryData)data.geometryData).height, ((CylinderGeometryData)data.geometryData).r);
                    break;
                default:
                    break;
            }

            barrierObjects.Add(data.index, newObject);

            barrierObjectsData[data.index] = data;
        }
        setUpdateArray();
    }

    public void deleteObject(BarrierData data)
    {
        GameObject barrierObject = barrierObjects[data.index];
        barrierObjects.Remove(data.index);
        Destroy(barrierObject);

        barrierObjectsData.Remove(data.index);

        deleteUpdateArray();
    }

    private void addUpdateArray()
    {

    }

    private void setUpdateArray()
    {

    }

    private void deleteUpdateArray()
    {

    }
}
