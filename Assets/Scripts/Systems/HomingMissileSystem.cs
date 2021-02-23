using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using System.Diagnostics;

[AlwaysSynchronizeSystem]
public class HomingMissileSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref HomingMissileData data) =>
        {
            // Este componente es muy similar a KillAfterTimeSystem, pero llama al manager homingMissileManager y por eso hice un sistema separado,
            // asi en KillAfterTimeSystem podia usar Burst
            data.timer += dt;
            if (data.timer >= data.timeToDie) {
                ecb.DestroyEntity(entity);
                Game.instance.homingMissileManager.OnEntitySelfDestroyed(entity);
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }
}
