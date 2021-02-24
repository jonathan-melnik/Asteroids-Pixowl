using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class SpaceshipAsteroidCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld stepPhysicsWorld;
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct SpaceshipAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<SpaceshipTag> allSpaceships;
        [ReadOnly] public ComponentDataFromEntity<AsteroidData> allAsteroids;
        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allAsteroids.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allSpaceships.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity asteroid, Entity spaceship) {
            if (Game.instance.spaceshipManager.shield.IsActive()) {
                return;
            }
            Game.instance.collisionManager.OnSpaceshipCollidedWithAsteroid(spaceship, asteroid);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipAsteroidCollisionSystemJob();
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);
        job.allAsteroids = GetComponentDataFromEntity<AsteroidData>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
