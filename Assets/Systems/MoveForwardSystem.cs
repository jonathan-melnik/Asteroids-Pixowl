using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

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
        }).Schedule(inputDeps);

        return myJob;
    }
}
