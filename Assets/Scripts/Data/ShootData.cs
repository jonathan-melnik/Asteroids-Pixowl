using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShootData : IComponentData
{
    public float offset;
    public ShotType shotType;
}

