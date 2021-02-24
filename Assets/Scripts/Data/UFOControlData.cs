using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct UFOControlData : IComponentData
{
    [HideInInspector] public bool turnRight;
    [HideInInspector] public bool turnLeft;
    [HideInInspector] public bool accelerate;
    [HideInInspector] public float startTurnTimer;
    [HideInInspector] public float turningTimer;
    public float timeTurningMin;
    public float timeTurningMax;
    public float timeToTurnMin;
    public float timeToTurnMax;
    public float turnChance; // Tiene que ser un valor entre 0 y 1. Es la chance de doblar hacia uno de los lados   
}
