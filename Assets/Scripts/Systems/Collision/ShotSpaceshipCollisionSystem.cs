using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ShotSpaceshipCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld _stepPhysicsWorld;
    private BuildPhysicsWorld _buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct ShotAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<ShotTag> allShots;
        [ReadOnly] public ComponentDataFromEntity<SpaceshipTag> allSpaceships;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allSpaceships.HasComponent(entityA) && allShots.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allShots.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity spaceship, Entity shot) {
            if (Game.instance.spaceshipManager.shield.IsActive()) {
                return;
            }
            Game.instance.collisionManager.OnShotCollidedWithSpaceship(shot, spaceship);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new ShotAsteroidCollisionSystemJob();
        job.allShots = GetComponentDataFromEntity<ShotTag>(true);
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);

        JobHandle jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
