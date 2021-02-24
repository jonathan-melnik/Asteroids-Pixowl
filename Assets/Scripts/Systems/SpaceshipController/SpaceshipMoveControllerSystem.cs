using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipMoveControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        Entities
            .WithAll<SpaceshipTag>()
            .ForEach((ref Translation tr, ref Rotation rot, ref MovementData movement, in MovementThrustData thrust) =>
        {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                movement.angle -= thrust.angularSpeed * dt;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                movement.angle += thrust.angularSpeed * dt;
            }
            movement.accel = Input.GetKey(KeyCode.UpArrow) ? thrust.thrustAccel : 0;
        }).Run();

        return default;
    }
}
