using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipShootSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in Translation tr, in MovementData movement, in ShotData shot) =>
            {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    float angle = movement.angle + math.radians(movement.angleOffset);
                    float3 dir = new float3(math.cos(angle), 0, math.sin(angle));
                    float3 pos = tr.Value + dir * shot.offset;
                    Game.instance.shotSpawner.Spawn(pos, angle, shot.speed);
                }
            }).Run();

        return default;
    }
}
