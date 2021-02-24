using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct UFOData : IComponentData
{
    public int size; // 2: grande, 1:chico
}
