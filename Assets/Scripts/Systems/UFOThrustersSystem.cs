using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class UFOThrustersSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithAll<UFOTag>()
            .WithoutBurst()
            .ForEach((Entity entity, in Translation tr, in Rotation rot, in MovementData movement) =>
        {
            Game.instance.ufoManager.UpdateThrusters(entity, tr.Value, rot.Value, movement.accel > 0);
        }).Run();

        return default;
    }


}
