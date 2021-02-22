using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class SpaceshipThrusters : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .ForEach((in Translation tr, in Rotation rot, in MovementData movement) =>
        {
            Game.instance.spaceship.OnSpaceshipEntityMoved(tr.Value, rot.Value);
            Game.instance.spaceship.OnSpaceshipEntityIsThrusting(movement.accel > 0);
        }).Run();

        return default;
    }


}
