using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum Entity_Type
    {
        Hero,
        Terrain
    }

    public abstract Entity_Type EType {get;}
}
