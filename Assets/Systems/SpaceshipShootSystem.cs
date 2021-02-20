using Unity.Entities;
using Unity.Jobs;
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
            .ForEach((in Translation tr) =>
            {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Game.instance.shotSpawner.Spawn(tr.Value);
                }
            }).Run();

        return default;
    }
}
