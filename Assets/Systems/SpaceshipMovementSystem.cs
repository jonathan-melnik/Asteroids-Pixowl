using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        Entities.ForEach((ref Translation tr, ref Rotation rot, ref MovementData movement, in SpaceshipMovementData spaceshipMovement) =>
        {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                movement.angle += movement.angularSpeed * dt;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                movement.angle -= movement.angularSpeed * dt;
            }
            movement.accel = Input.GetKey(KeyCode.UpArrow) ? spaceshipMovement.thrustAccel : 0;
        }).Run();

        return default;
    }
}
