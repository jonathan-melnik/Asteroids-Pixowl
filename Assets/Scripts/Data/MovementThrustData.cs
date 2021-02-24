
using Unity.Entities;

[GenerateAuthoringComponent]
public struct MovementThrustData : IComponentData
{
    public float thrustAccel;
    public float angularSpeed;
}
