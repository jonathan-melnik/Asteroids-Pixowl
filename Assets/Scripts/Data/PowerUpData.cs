using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PowerUpData : IComponentData
{
    public PowerUpType type;
}

public enum PowerUpType
{
    Shield,
    Mines,
    Shotgun
}
