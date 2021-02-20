
using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpaceshipMovementData : IComponentData
{
    public float thrustAccel; // Aceleracion al presionar la tecla de thrust
}
