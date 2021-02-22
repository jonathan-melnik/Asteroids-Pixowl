using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AsteroidData : IComponentData
{
    public int size; // 3: grande, 2:mediano, 1:chico, 0:destruido
}
