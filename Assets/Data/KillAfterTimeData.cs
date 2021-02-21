using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct KillAfterTimeData : IComponentData
{
    public float timeToDie;
    [HideInInspector] public float timer;
}
