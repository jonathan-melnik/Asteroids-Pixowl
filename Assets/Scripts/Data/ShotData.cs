using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShotData : IComponentData
{
    public float speed;
    public ShotType type;
}

public enum ShotType
{
    Normal,
    HomingMissile
}

