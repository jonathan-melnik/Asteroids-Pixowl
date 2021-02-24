using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class PlayerItemCollisionSystem : JobComponentSystem
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
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
        [ReadOnly] public ComponentDataFromEntity<ItemTag> allItems;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allPlayers.HasComponent(entityA) && allItems.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allItems.HasComponent(entityA) && allPlayers.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity player, Entity item) {
            Game.instance.collisionManager.OnPlayerCollidedWithItem(player, item);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new BombAsteroidCollisionSystemJob();
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        job.allItems = GetComponentDataFromEntity<ItemTag>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
