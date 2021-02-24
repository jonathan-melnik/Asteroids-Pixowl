using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class UFOShootControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        Entities
            .WithAll<UFOTag>()
            .WithoutBurst()
            .ForEach((ref UFOData data, in Translation tr, in MovementData movement, in ShootData shoot) =>
            {
                bool isShooting = false;
                if (data.shootTimer > 0) {
                    data.shootTimer -= dt;
                    if (data.shootTimer <= 0) {
                        isShooting = true;
                        data.shootTimer = UnityEngine.Random.Range(data.minShootTime, data.maxShootTime);
                    }
                }

                if (isShooting) {
                    float angle = math.radians(UnityEngine.Random.Range(0, 360f));
                    float3 dir = new float3(math.cos(angle), math.sin(angle), 0);
                    float3 pos = tr.Value + dir * shoot.offset;

                    Game.instance.shootManager.ScheduleShoot(pos, angle, ShotType.Normal, true);
                }
            }).Run();

        return default;
    }
}
