using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogParticleData
{
    public List<Vector3> m_x { get; private set; }
    public List<float> m_masses { get; private set; }
    private List<float> m_invMasses;
    private List<Vector3> m_x0;
    private List<Vector3> m_v;
    private List<Vector3> m_a;
    private List<Vector3> m_oldX;
    private List<Vector3> m_lastX;

    public FogParticleData() 
    {
        m_masses = new List<float>();
        m_invMasses = new List<float>();
        m_x0 = new List<Vector3>();
        m_x = new List<Vector3>();
        m_v = new List<Vector3>();
        m_a = new List<Vector3>();
        m_oldX = new List<Vector3>();
        m_lastX = new List<Vector3>();
    }
    ~FogParticleData() 
    {
        m_masses.Clear();
        m_invMasses.Clear();
        m_x0.Clear();
        m_x.Clear();
        m_v.Clear();
        m_a.Clear();
        m_oldX.Clear();
        m_lastX.Clear();
    }

    public void AddVertex(Vector3 vertex)
    {
        m_x0.Add(vertex);
        m_x.Add(vertex);
        m_oldX.Add(vertex);
        m_lastX.Add(vertex);
        m_masses.Add(1.0f);
        m_invMasses.Add(1.0f);
        m_v.Add(Vector3.zero);
        m_a.Add(Vector3.zero);
    }

    public Vector3 GetPosition(int i)
    {
        return m_x[i];
    }

    public void SetPosition(int i, Vector3 pos)
    {
        m_x[i] = pos;
    }

    public Vector3 GetPosition0(int i)
    {
        return m_x0[i];
    }

    public void SetPosition0(int i, Vector3 pos)
    {
        m_x0[i] = pos;
    }

    public Vector3 GetLastPosition(int i)
    {
        return m_lastX[i];
    }

    public void SetLastPosition(int i, Vector3 pos)
    {
        m_lastX[i] = pos;
    }

    public Vector3 GetOldPosition(int i)
    {
        return m_oldX[i];
    }

    public void SetOldPosition(int i, Vector3 pos)
    {
        m_oldX[i] = pos;
    }
    
    public Vector3 GetVelocity(int i)
    {
        return m_v[i];
    }

    public void SetVelocity(int i, Vector3 vel)
    {
        m_v[i] = vel;
    }

    public Vector3 GetAcceleration(int i)
    {
        return m_a[i];
    }

    public void SetAcceleration(int i, Vector3 accel)
    {
        m_a[i] = accel;
    }

    public float getMass(int i)
    {
        return m_masses[i];
    }

    public void SetMass(int i, float mass)
    {
        m_masses[i] = mass;
        if (mass != 0.0f)
            m_invMasses[i] = 1.0f / mass;
        else
            m_invMasses[i] = 0.0f;
    }

    public float GetInvMass(int i)
    {
        return m_invMasses[i];
    }

    public int GetNumberOfParticles()
    {
        return m_x.Count;
    }

    /** Resize the array containing the particle data.
        */
    public void Resize(int newSize)
    {
        Utilities.ResizeList(m_masses, newSize);
        Utilities.ResizeList(m_invMasses, newSize);
        Utilities.ResizeList(m_x0, newSize);
        Utilities.ResizeList(m_x, newSize);
        Utilities.ResizeList(m_v, newSize);
        Utilities.ResizeList(m_a, newSize);
        Utilities.ResizeList(m_oldX, newSize);
        Utilities.ResizeList(m_lastX, newSize);
        // m_masses.resize(newSize);
        // m_invMasses.resize(newSize);
        // m_x0.resize(newSize);
        // m_x.resize(newSize);
        // m_v.resize(newSize);
        // m_a.resize(newSize);
        // m_oldX.resize(newSize);
        // m_lastX.resize(newSize);
    }

    /** Reserve the array containing the particle data.
        */
    // public void Reserve(int newSize)
    // {
    //     m_masses.reserve(newSize);
    //     m_invMasses.reserve(newSize);
    //     m_x0.reserve(newSize);
    //     m_x.reserve(newSize);
    //     m_v.reserve(newSize);
    //     m_a.reserve(newSize);
    //     m_oldX.reserve(newSize);
    //     m_lastX.reserve(newSize);m_masses.reserve(newSize);
    // }

    /** Release the array containing the particle data.
        */
    public void Release()
    {
        m_masses.Clear();
        m_invMasses.Clear();
        m_x0.Clear();
        m_x.Clear();
        m_v.Clear();
        m_a.Clear();
        m_oldX.Clear();
        m_lastX.Clear();
    }

    /** Release the array containing the particle data.
        */
    public int Size() 
    {
        return m_x.Count;
    }

}