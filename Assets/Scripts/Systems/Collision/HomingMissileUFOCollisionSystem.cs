using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class HomingMissileUFOCollisionSystem : JobComponentSystem
{
    private StepPhysicsWorld _stepPhysicsWorld;
    private BuildPhysicsWorld _buildPhysicsWorld;

    protected override void OnCreate() {
        base.OnCreate();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    public struct HomingMissileAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<HomingMissileTag> allMissiles;
        [ReadOnly] public ComponentDataFromEntity<UFOTag> allUFOs;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allUFOs.HasComponent(entityA) && allMissiles.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allMissiles.HasComponent(entityA) && allUFOs.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }
        void OnCollision(Entity asteroid, Entity homingMissile) {
            Game.instance.collisionManager.OnHomingMissileCollidedWithUFO(homingMissile, asteroid);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new HomingMissileAsteroidCollisionSystemJob();
        job.allMissiles = GetComponentDataFromEntity<HomingMissileTag>(true);
        job.allUFOs = GetComponentDataFromEntity<UFOTag>(true);

        JobHandle jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
