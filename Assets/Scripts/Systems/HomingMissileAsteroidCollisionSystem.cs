using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class HomingMissileAsteroidCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld _stepPhysicsWorld;
    private BuildPhysicsWorld _buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct SpaceshipAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<HomingMissileTag> allMissiles;
        [ReadOnly] public ComponentDataFromEntity<AsteroidData> allAsteroids;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allAsteroids.HasComponent(entityA) && allMissiles.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allMissiles.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity asteroid, Entity shot) {
            Game.instance.collisionManager.OnHomingMissileCollidedWithAsteroid(shot, asteroid);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipAsteroidCollisionSystemJob();
        job.allMissiles = GetComponentDataFromEntity<HomingMissileTag>(true);
        job.allAsteroids = GetComponentDataFromEntity<AsteroidData>(true);

        JobHandle jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
