
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpaceshipMovementData : IComponentData
{
    [HideInInspector] public float3 velocity;
    [HideInInspector] public float3 direction;
    public float angularSpeed;
    [HideInInspector] public float angle;
    public float angleOffset;
    public float maxSpeed;
    [HideInInspector] public float3 accel;
    public float thrustAccel;
}
