using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Diagnostics;

[AlwaysSynchronizeSystem]
public class SpaceshipShieldSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithoutBurst()
            .WithAll<SpaceshipTag>()
            .ForEach((in Translation tr) =>
        {
            Game.instance.spaceshipManager.shield.OnSpaceshipEntityMoved(tr.Value);
        }).Run();

        return default;
    }


}
