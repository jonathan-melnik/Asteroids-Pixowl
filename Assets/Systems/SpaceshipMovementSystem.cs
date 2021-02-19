using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        Entities.ForEach((ref Translation tr, ref Rotation rot, ref SpaceshipMovementData movement) => {

            if (Input.GetKey(KeyCode.LeftArrow)) {
                movement.angle += movement.angularSpeed * dt;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                movement.angle -= movement.angularSpeed * dt;
            }

            float angleOffseted = movement.angle + math.radians(movement.angleOffset);
            movement.direction = new float3(math.cos(angleOffseted), 0, math.sin(angleOffseted));

            rot.Value = quaternion.Euler(0, -movement.angle, 0);

            movement.accel.xz = Input.GetKey(KeyCode.UpArrow) ? movement.thrustAccel : 0;
            movement.velocity += movement.accel * movement.direction * dt;

            float velocityLength = math.length(movement.velocity);
            if (velocityLength > movement.maxSpeed) {
                movement.velocity = math.normalize(movement.velocity) * velocityLength;
            }

            tr.Value += movement.velocity * dt + 0.5f * movement.accel * dt * dt;

            float screenWidth = ScreenCorners.UpperRight.Data.x - ScreenCorners.LowerLeft.Data.x;
            if (tr.Value.x < ScreenCorners.LowerLeft.Data.x) {
                tr.Value.x += screenWidth;
            } else if (tr.Value.x > ScreenCorners.UpperRight.Data.x) {
                tr.Value.x -= screenWidth;
            }
            float screenHeight = ScreenCorners.UpperRight.Data.z - ScreenCorners.LowerLeft.Data.z;
            if (tr.Value.z < ScreenCorners.LowerLeft.Data.z) {
                tr.Value.z += screenHeight;
            } else if (tr.Value.z > ScreenCorners.UpperRight.Data.z) {
                tr.Value.z -= screenHeight;
            }

        }).Run();

        return default;
    }
}
