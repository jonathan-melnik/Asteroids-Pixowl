using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShotData : IComponentData
{
    public float offset;
    public float speed;
}
