using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class SpaceshipThrustersSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .ForEach((in Translation tr, in Rotation rot, in MovementData movement) =>
        {
            var thrusters = Game.instance.spaceshipSpawner.thrusters;
            thrusters.OnSpaceshipEntityMoved(tr.Value, rot.Value);
            thrusters.OnSpaceshipEntityIsThrusting(movement.accel > 0);
        }).Run();

        return default;
    }


}
