using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct HyperspaceData : IComponentData
{
    [HideInInspector] public float timer;
    public float cooldown;
}
