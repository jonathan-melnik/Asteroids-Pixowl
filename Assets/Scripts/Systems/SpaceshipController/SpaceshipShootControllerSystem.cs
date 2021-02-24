using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipShootControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .ForEach((in Translation tr, in MovementData movement, in ShootData shoot) =>
            {
                bool isShooting = false;
                ShotType shotType = shoot.shotType;
                if (Input.GetKeyDown(KeyCode.X)) {
                    isShooting = true;
                } else if (Input.GetKeyDown(KeyCode.H)) { // cheat
                    isShooting = true;
                    shotType = ShotType.HomingMissile;
                }

                if (isShooting) {
                    float angle = -movement.angle + math.radians(movement.angleOffset);
                    float3 dir = new float3(math.cos(angle), math.sin(angle), 0);
                    float3 pos = tr.Value + dir * shoot.offset;

                    Game.instance.shootManager.ScheduleShoot(pos, angle, shotType, false);
                }
            }).Run();

        return default;
    }
}
