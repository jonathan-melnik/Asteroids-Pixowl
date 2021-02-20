using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    [HideInInspector] public float speed;
    [HideInInspector] public float accel;
    public float maxSpeed;
    [HideInInspector] public float angle;
    public float angleOffset; // Se usa para que el angulo de la visual de la nave sea independiente de la logica
    public float angularSpeed; // Velocidad de rotacion cuando se presiona una flecha en el teclado
}
