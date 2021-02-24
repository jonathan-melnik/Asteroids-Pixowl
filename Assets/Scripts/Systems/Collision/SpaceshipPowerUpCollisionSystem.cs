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

    public struct SpaceshipPowerUpCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<SpaceshipTag> allSpaceships;
        [ReadOnly] public ComponentDataFromEntity<PowerUpData> allPowerUps;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allPowerUps.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allSpaceships.HasComponent(entityA) && allPowerUps.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity powerUp, Entity spaceship) {
            Game.instance.collisionManager.OnSpaceshipCollidedWithPowerUp(spaceship, powerUp);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipPowerUpCollisionSystemJob();
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);
        job.allPowerUps = GetComponentDataFromEntity<PowerUpData>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
