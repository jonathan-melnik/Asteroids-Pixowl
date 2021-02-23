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
            .WithStructuralChanges()
            .ForEach((in Translation tr, in MovementData movement, in ShotData shot) =>
            {
                if (Input.GetKeyDown(KeyCode.X)) {
                    float angle = -movement.angle + math.radians(movement.angleOffset);
                    float3 dir = new float3(math.cos(angle), math.sin(angle), 0);
                    float3 pos = tr.Value + dir * shot.offset;

                    if (shot.type == ShotType.Normal) {
                        Game.instance.shotSpawner.Spawn(pos, angle, shot.speed);
                    } else if (shot.type == ShotType.HomingMissile) {
                        Game.instance.homingMissileManager.Spawn(pos, angle, shot.speed);
                    }
                }
            }).Run();

        return default;
    }
}
