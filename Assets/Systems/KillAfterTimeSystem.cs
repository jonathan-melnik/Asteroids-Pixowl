using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class KillAfterTimeSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, ref KillAfterTimeData data) =>
        {
            data.timer += dt;
            if (data.timer >= data.timeToDie) {
                ecb.DestroyEntity(entity);
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }
}
