using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BombData : IComponentData
{
    public float expansionSpeed;
    public float targetRadius;
}
