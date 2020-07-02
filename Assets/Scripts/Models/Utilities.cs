using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void ResizeList<T>(List<T> list, int size)
    {
        if (size > list.Count)
        {
            T[] array = new T[size];
            list.CopyTo(array);
            list.Clear();
            list.AddRange(array);
        }
        else
        {
            list.RemoveRange(size, list.Count - size);
        }
    }
}