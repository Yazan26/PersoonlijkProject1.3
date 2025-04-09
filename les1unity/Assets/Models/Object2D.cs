using System;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Bijzonderheden wegens beperkingen van JsonUtility:
 * - Models hebben variabelen met kleine letters omdat JsonUtility anders de velden uit de JSON niet correct overzet naar het C# object.
 * - De id is een string in plaats van een Guid omdat JsonUtility Guid niet ondersteunt. Gelukkig geeft dit geen probleem indien we gewoon een string gebruiken in Unity en een Guid in onze backend API.
*/
[Serializable]
public class Object2D
{
    public string Id;

    public string Environment2DID;

 public string PrefabId;

     public float PositionX;

     public float PositionY;

     public float ScaleX;

     public float ScaleY;

    public float RotationZ;

    public int SortingLayer;
    
}