using System;
using UnityEngine;

public class IntVector3
{
    public int x;
    public int y;
    public int z;

    public IntVector3(Vector3 vectorr)
    {
        x = (int)vectorr.x;
        y = (int)vectorr.y;
        z = (int)vectorr.z;
    }

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

}
