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
    [FormerlySerializedAs("Id")] public string id;

    [FormerlySerializedAs("Environment2DID")] public string environment2DID;

 [FormerlySerializedAs("PrefabId")] public string prefabId;

     [FormerlySerializedAs("PositionX")] public float positionX;

     [FormerlySerializedAs("PositionY")] public float positionY;

     [FormerlySerializedAs("ScaleX")] public float scaleX;

     [FormerlySerializedAs("ScaleY")] public float scaleY;

    [FormerlySerializedAs("RotationZ")] public float rotationZ;

    [FormerlySerializedAs("SortingLayer")] public int sortingLayer;
    
}