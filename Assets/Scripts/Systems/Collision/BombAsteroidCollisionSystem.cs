using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BombAsteroidCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld stepPhysicsWorld;
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct BombAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<BombTag> allBombs;
        [ReadOnly] public ComponentDataFromEntity<AsteroidData> allAsteroids;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allAsteroids.HasComponent(entityA) && allBombs.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allBombs.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity asteroid, Entity bomb) {
            Game.instance.collisionManager.OnBombCollidedWithAsteroid(bomb, asteroid);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new BombAsteroidCollisionSystemJob();
        job.allBombs = GetComponentDataFromEntity<BombTag>(true);
        job.allAsteroids = GetComponentDataFromEntity<AsteroidData>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
