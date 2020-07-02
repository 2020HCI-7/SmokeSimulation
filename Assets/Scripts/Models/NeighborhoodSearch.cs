using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborhoodSearch
{
    private class HashEntry
    {
        public HashEntry() {}
        public long timestamp;
        public List<int> particleIndices;
    };

    private int m_numParticles;
    private int m_maxNeighbors;
    private int m_maxParticlesPerCell;
    private int[][] m_neighbors;
    private int[] m_numNeighbors;
    private float m_cellGridSize;
    private float m_radius2;
    private int m_currentTimestamp;
    //Utilities::Hashmap<NeighborhoodSearchCellPos*, HashEntry*> m_gridMap;
    private Dictionary<Vector3, HashEntry> m_gridMap;

    public NeighborhoodSearch()
    {

    }

    public NeighborhoodSearch(
        int numParticles = 0, 
        float radius = 0.1f,
        int maxNeighbors = 60, 
        int maxParticlesPerCell = 50) 
    {
        m_gridMap = new Dictionary<Vector3, HashEntry>();
        m_cellGridSize = radius;
        m_radius2 = radius*radius;
        m_numParticles = numParticles;
        m_maxParticlesPerCell = maxParticlesPerCell;
        m_maxNeighbors = maxNeighbors;

        m_numNeighbors = null;
        m_neighbors = null;

        // if (numParticles != 0)
        // {
        //     m_numNeighbors = new int[m_numParticles];
        //     m_neighbors = new int[m_numParticles][];
        //     for (int i = 0; i < m_numParticles; i++)
        //         m_neighbors[i] = new int[m_maxNeighbors];
        // }
        ArraySizeUpdate(numParticles);

        m_currentTimestamp = 0;
    }

    ~NeighborhoodSearch()
    {

    }

    static int Floor(float v)
    {
        return (int)(v + 32768.0f) - 32768;			// Shift to get positive values 
    }

    public void ArraySizeUpdate(int numParticles)
    {
        if (numParticles != 0)
        {
            m_numParticles = numParticles;
            m_numNeighbors = new int[numParticles];
            m_neighbors = new int[numParticles][];
            for (int i = 0; i < numParticles; i++)
                m_neighbors[i] = new int[m_maxNeighbors];
        }
    }

    public int[][] getNeighbors() 
    {
        return m_neighbors;
    }
    public int[] getNumNeighbors()
    {
        return m_numNeighbors;
    }
    public int getNumParticles()
    {
        return m_numParticles;
    }
    public void setRadius(float radius)
    {
        m_cellGridSize = radius;
        m_radius2 = radius*radius;
    }
    public float getRadius()
    {
        return Mathf.Sqrt(m_radius2);
    }
    public void update()
    {
        m_currentTimestamp++;
    }
    public void neighborhoodSearch(List<Vector3> x) 
    {
        ArraySizeUpdate(x.Count);
        float factor = 1.0f / m_cellGridSize;
        for (int i=0; i < (int) m_numParticles; i++)
        {
            int cellPos1 = Floor(x[i].x * factor)+1;
            int cellPos2 = Floor(x[i].y * factor)+1;
            int cellPos3 = Floor(x[i].z * factor)+1;
            Vector3 cellPos = new Vector3(cellPos1, cellPos2, cellPos3);

            //if (entry != NULL)
            if (m_gridMap.ContainsKey(cellPos))
            {
                HashEntry entry = m_gridMap[cellPos];
                if (entry.timestamp != m_currentTimestamp)
                {
                    entry.timestamp = m_currentTimestamp;
                    entry.particleIndices.Clear();
                }
                entry.particleIndices.Add(i);
            }
            else
            {
                HashEntry newEntry = new HashEntry(); 	
                newEntry.particleIndices = new List<int>();
                newEntry.timestamp = m_currentTimestamp;
                newEntry.particleIndices.Add(i);
                m_gridMap.Add(cellPos, newEntry);
            }
        }
        
		for (int i=0; i < m_numParticles; i++)
        {
			m_numNeighbors[i] = 0;
			int cellPos1 = Floor(x[i].x * factor);
			int cellPos2 = Floor(x[i].y * factor);
			int cellPos3 = Floor(x[i].z * factor);
            for(int j=0; j < 3; j++)
			{				
				for(int k=0; k < 3; k++)
				{									
					for(int l=0; l < 3; l++)
					{
                        Vector3 cellPos = new Vector3(cellPos1+j, cellPos2+k, cellPos3+l);

                        if (m_gridMap.ContainsKey(cellPos))
                        {
                            HashEntry entry = m_gridMap[cellPos];
                            if (entry.timestamp == m_currentTimestamp)
                            {
                                for (int m=0; m < entry.particleIndices.Count; m++) 
                                {
                                    int pi = entry.particleIndices[m];
                                    if (pi != i) 
                                    {
                                        float dist = Vector3.Distance(x[i], x[pi]);
                                        float dist2 = dist * dist;
                                        if (dist2 < m_radius2)
                                        {
                                            if (m_numNeighbors[i] < m_maxNeighbors)
                                                m_neighbors[i][m_numNeighbors[i]++] = pi;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void neighborhoodSearch(List<Vector3> x, int numBoundaryParticles, List<Vector3> boundaryX)
    {
        ArraySizeUpdate(x.Count);
        float factor = 1.0f / m_cellGridSize;
        for (int i=0; i < (int) m_numParticles; i++)
        {
            int cellPos1 = Floor(x[i].x * factor)+1;
            int cellPos2 = Floor(x[i].y * factor)+1;
            int cellPos3 = Floor(x[i].z * factor)+1;
            Vector3 cellPos = new Vector3(cellPos1, cellPos2, cellPos3);

            //if (entry != NULL)
            if (m_gridMap.ContainsKey(cellPos))
            {
                HashEntry entry = m_gridMap[cellPos];
                if (entry.timestamp != m_currentTimestamp)
                {
                    entry.timestamp = m_currentTimestamp;
                    entry.particleIndices.Clear();
                }
                entry.particleIndices.Add(i);
            }
            else
            {
                HashEntry newEntry = new HashEntry(); 	
                newEntry.particleIndices = new List<int>();
                newEntry.timestamp = m_currentTimestamp;
                newEntry.particleIndices.Add(i);
                m_gridMap.Add(cellPos, newEntry);
            }
        }

        for (int i = 0; i < (int)numBoundaryParticles; i++)
        {
            int cellPos1 = Floor(boundaryX[i].x * factor) + 1;
            int cellPos2 = Floor(boundaryX[i].y * factor) + 1;
            int cellPos3 = Floor(boundaryX[i].z * factor) + 1;
            Vector3 cellPos = new Vector3(cellPos1, cellPos2, cellPos3);
            //HashEntry *&entry = m_gridMap[&cellPos];

            if (m_gridMap.ContainsKey(cellPos))
            {
                HashEntry entry = m_gridMap[cellPos];
                if (entry.timestamp != m_currentTimestamp)
                {
                    entry.timestamp = m_currentTimestamp;
                    entry.particleIndices.Clear();
                }
                entry.particleIndices.Add(i);
            }
            else
            {
                HashEntry newEntry = new HashEntry();
                newEntry.particleIndices = new List<int>();
                newEntry.timestamp = m_currentTimestamp;
                newEntry.particleIndices.Add(i);
                m_gridMap.Add(cellPos, newEntry);
            }
        }
        
		for (int i=0; i < m_numParticles; i++)
        {
			m_numNeighbors[i] = 0;
			int cellPos1 = Floor(x[i].x * factor);
			int cellPos2 = Floor(x[i].y * factor);
			int cellPos3 = Floor(x[i].z * factor);
            for(int j=0; j < 3; j++)
			{				
				for(int k=0; k < 3; k++)
				{									
					for(int l=0; l < 3; l++)
					{
                        Vector3 cellPos = new Vector3(cellPos1+j, cellPos2+k, cellPos3+l);

                        if (m_gridMap.ContainsKey(cellPos))
                        {
                            HashEntry entry = m_gridMap[cellPos];
                            if (entry.timestamp == m_currentTimestamp)
                            {
                                for (int m=0; m < entry.particleIndices.Count; m++) 
                                {
                                    int pi = entry.particleIndices[m];
                                    if (pi != i) 
                                    {
                                        float dist = Vector3.Distance(x[i], x[pi]);
                                        float dist2 = dist * dist;
                                        if (dist2 < m_radius2)
                                        {
                                            if (m_numNeighbors[i] < m_maxNeighbors)
                                                m_neighbors[i][m_numNeighbors[i]++] = pi;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}