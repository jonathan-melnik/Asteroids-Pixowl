using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ConstantMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;

        JobHandle myJob = Entities.ForEach((ref Translation tr, in ConstantMovementData movement) =>
        {
            tr.Value += movement.velocity * dt;
        }).Schedule(inputDeps);

        return myJob;
    }
}
