using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class SpaceshipUFOCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld stepPhysicsWorld;
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct SpaceshipPowerUpCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<SpaceshipTag> allSpaceships;
        [ReadOnly] public ComponentDataFromEntity<UFOTag> allUFOs;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allUFOs.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allSpaceships.HasComponent(entityA) && allUFOs.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity ufo, Entity spaceship) {
            if (Game.instance.spaceshipManager.shield.IsActive()) {
                return;
            }
            Game.instance.collisionManager.OnSpaceshipCollidedWithUFO(spaceship, ufo);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipPowerUpCollisionSystemJob();
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);
        job.allUFOs = GetComponentDataFromEntity<UFOTag>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
