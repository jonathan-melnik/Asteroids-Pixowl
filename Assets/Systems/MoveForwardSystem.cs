using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        float screenLeft = ScreenCorners.LowerLeft.Data.x;
        float screenRight = ScreenCorners.UpperRight.Data.x;
        float screenBottom = ScreenCorners.LowerLeft.Data.z;
        float screenTop = ScreenCorners.UpperRight.Data.z;
        float screenWidth = screenRight - screenLeft;
        float screenHeight = screenTop - screenBottom;

        JobHandle myJob = Entities.ForEach((ref Translation tr, ref Rotation rot, ref MovementData movement) =>
        {
            float angleOffseted = movement.angle + math.radians(movement.angleOffset);
            float3 direction = new float3(math.cos(angleOffseted), 0, math.sin(angleOffseted));

            rot.Value = quaternion.Euler(0, -movement.angle, 0);

            movement.speed += movement.accel * dt;
            // Limito la velocidad maxima(salvo que maxSpeed sea 0, en ese caso no hago clamp)
            if (movement.maxSpeed > 0) {
                movement.speed = math.clamp(movement.speed, -movement.maxSpeed, movement.maxSpeed);
            }

            tr.Value += movement.speed * direction * dt + 0.5f * movement.accel * new float3(1, 0, 1) * dt * dt;

            // Limite horizontal de la pantalla
            if (tr.Value.x < screenLeft) {
                tr.Value.x += screenWidth;
            } else if (tr.Value.x > screenRight) {
                tr.Value.x -= screenWidth;
            }
            // Limite vertical de la pantalla
            if (tr.Value.z < screenBottom) {
                tr.Value.z += screenHeight;
            } else if (tr.Value.z > screenTop) {
                tr.Value.z -= screenHeight;
            }

        }).Schedule(inputDeps);

        return myJob;
    }
}
