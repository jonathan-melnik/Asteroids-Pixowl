using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ConstantMovementData : IComponentData
{
    public float3 velocity;
}
