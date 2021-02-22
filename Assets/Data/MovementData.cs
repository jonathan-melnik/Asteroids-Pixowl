using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    [HideInInspector] public float accel;
    [HideInInspector] public float3 velocity;
    public float maxSpeed;
    [HideInInspector] public float angle;
    public float angleOffset; // Se usa para que el angulo de la visual de la nave sea independiente de la logica

}
