using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpaceshipHyperspaceSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .ForEach((ref HyperspaceData hyperspace) =>
            {
                hyperspace.timer = math.max(hyperspace.timer - dt, 0);
                Game.instance.uiManager.hyperspace.SetProgress(1 - hyperspace.timer / hyperspace.cooldown);
            }).Run();

        return default;
    }
}
