using UnityEngine;
using System.Collections.Generic;
using System;

public class FadeObject : MonoBehaviour, IEquatable<FadeObject>
{
    public List<Renderer> Renderers = new List<Renderer>(); 
    public List<Material> Materials = new List<Material>();

    public Vector3 Position;
    public float initialAlpha;

    private void Awake()
    {
        GetFadeObjectData();
    }

    #region AWAKE

    private void GetFadeObjectData()
    {
        Position = transform.position;

        Renderers.AddRange(GetComponentsInChildren<Renderer>());

        foreach (Renderer renderer in Renderers)
        {
            Materials.AddRange(renderer.materials);
        }

        initialAlpha = Materials[0].color.a;
    }


    #endregion AWAKE

    #region IEQUATABLE

    public bool Equals(FadeObject other)
    {
        return Position.Equals(other.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    #endregion IEQUATABLE
}
