using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class SpaceshipPowerUpCollisionSystem : JobComponentSystem
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
        [ReadOnly] public ComponentDataFromEntity<PowerUpData> allPowerUps;
        [ReadOnly] public ComponentDataFromEntity<Translation> allTranslations;
        public EntityCommandBuffer ecb;
        public EntityManager entityManager;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allPowerUps.HasComponent(entityA) && allPowerUps.HasComponent(entityB)) {
                return;
            }

            if (allPowerUps.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allSpaceships.HasComponent(entityA) && allPowerUps.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity powerUp, Entity spaceship) {
            ecb.DestroyEntity(powerUp);
            var powerUpPos = allTranslations[powerUp].Value;
            Game.instance.OnSpaceshipCollidedWithPowerUp(allPowerUps[powerUp].type);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipAsteroidCollisionSystemJob();
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);
        job.allPowerUps = GetComponentDataFromEntity<PowerUpData>(true);
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
