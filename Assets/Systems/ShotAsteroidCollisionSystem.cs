using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ShotAsteroidCollisionSystem : JobComponentSystem
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
        [ReadOnly] public ComponentDataFromEntity<ShotTag> allShots;
        [ReadOnly] public ComponentDataFromEntity<AsteroidTag> allAsteroids;
        [ReadOnly] public ComponentDataFromEntity<Translation> allTranslations;
        public EntityCommandBuffer ecb;
        public EntityManager entityManager;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allAsteroids.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                return;
            }
            if (allShots.HasComponent(entityA) && allShots.HasComponent(entityB)) {
                return;
            }

            if (allAsteroids.HasComponent(entityA) && allShots.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allShots.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity asteroid, Entity shot) {
            ecb.DestroyEntity(asteroid);
            ecb.DestroyEntity(shot);
            var asteroidPos = allTranslations[asteroid].Value;
            Game.instance.OnShotCollidedWithAsteroid(asteroidPos);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipAsteroidCollisionSystemJob();
        job.allShots = GetComponentDataFromEntity<ShotTag>(true);
        job.allAsteroids = GetComponentDataFromEntity<AsteroidTag>(true);
        job.allTranslations = GetComponentDataFromEntity<Translation>(true);
        job.ecb = new EntityCommandBuffer(Allocator.TempJob);
        job.entityManager = EntityManager;

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        job.ecb.Playback(EntityManager);
        job.ecb.Dispose();

        return jobHandle;
    }
}
