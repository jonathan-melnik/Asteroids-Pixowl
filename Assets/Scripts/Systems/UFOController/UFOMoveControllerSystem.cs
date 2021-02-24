using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class UFOMoveControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        Entities
            .WithAll<UFOTag>()
            .ForEach((ref Translation tr, ref Rotation rot, ref MovementData movement, ref UFOControlData control, in MovementThrustData thrust) =>
        {
            if (control.startTurnTimer > 0) {
                control.startTurnTimer -= dt;
                if (control.startTurnTimer <= 0) {
                    if (UnityEngine.Random.value < control.turnChance) {
                        float rand = UnityEngine.Random.value;
                        control.turnRight = rand <= 0.5f;
                        control.turnLeft = rand > 0.5f;
                    } else {
                        control.turnRight = false;
                        control.turnLeft = false;
                    }
                    control.accelerate = true;
                    control.turningTimer = UnityEngine.Random.Range(control.timeTurningMin, control.timeTurningMax);
                }
            }

            if (control.turningTimer > 0) {
                control.turningTimer -= dt;
                if (control.turningTimer <= 0) {
                    control.turnRight = false;
                    control.turnLeft = false;
                    control.accelerate = false;
                    control.startTurnTimer = UnityEngine.Random.Range(control.timeToTurnMin, control.timeToTurnMax);
                }
            }

            if (control.turnRight) {
                movement.angle += thrust.angularSpeed * dt;
            } else if (control.turnLeft) {
                movement.angle -= thrust.angularSpeed * dt;
            }
            movement.accel = control.accelerate ? thrust.thrustAccel : 0;
        }).Run();

        return default;
    }
}
