using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ShieldData : IComponentData
{
    public float timer;
    public float time;
}
