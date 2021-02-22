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

            movement.velocity += movement.accel * direction * dt;
            // Limito la velocidad maxima(salvo que maxSpeed sea 0, en ese caso no hago clamp)
            float speed = math.length(movement.velocity);
            if (speed > movement.maxSpeed) {
                movement.velocity = math.normalize(movement.velocity) * movement.maxSpeed;
            }

            tr.Value += movement.velocity * dt + 0.5f * movement.accel * new float3(1, 0, 1) * dt * dt;
        }).Schedule(inputDeps);

        return myJob;
    }
}
