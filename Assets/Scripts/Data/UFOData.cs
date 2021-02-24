using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct UFOData : IComponentData
{
    public int size; // 2: grande, 1:chico
    public float minShootTime;
    public float maxShootTime;
    [HideInInspector] public float shootTimer;
}
