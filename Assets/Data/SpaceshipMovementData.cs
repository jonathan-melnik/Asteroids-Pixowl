
using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpaceshipMovementData : IComponentData
{
    public float thrustAccel; // Aceleracion al presionar la tecla de thrust
    public float angularSpeed; // Velocidad de rotacion cuando se presiona una flecha en el teclado
}
