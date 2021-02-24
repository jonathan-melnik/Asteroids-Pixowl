using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class PlayerEnemyCollisionSystem : JobComponentSystem
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
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> allEnemies;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allPlayers.HasComponent(entityA) && allEnemies.HasComponent(entityB)) {
                OnCollision(entityA, entityB);
            } else if (allEnemies.HasComponent(entityA) && allPlayers.HasComponent(entityB)) {
                OnCollision(entityB, entityA);
            }
        }

        void OnCollision(Entity player, Entity enemy) {
            Game.instance.collisionManager.OnPlayerCollidedWithEnemy(player, enemy);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new BombAsteroidCollisionSystemJob();
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        job.allEnemies = GetComponentDataFromEntity<EnemyTag>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}
