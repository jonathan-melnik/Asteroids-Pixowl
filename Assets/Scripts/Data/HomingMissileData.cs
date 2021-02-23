using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct HomingMissileData : IComponentData
{
    public float timeToDie;
    [HideInInspector] public float timer;
}
