using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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

    [BurstCompile]
    public struct SpaceshipAsteroidCollisionSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<SpaceshipTag> allSpaceships;
        [ReadOnly] public ComponentDataFromEntity<AsteroidTag> allAsteroids;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allAsteroids.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                return;
            }

            if (allAsteroids.HasComponent(entityA) && allSpaceships.HasComponent(entityB)) {
                UnityEngine.Debug.Log($"La entidad A(Asteroid) colisiono con la entidad B(Spaceship)");
            } else if (allSpaceships.HasComponent(entityA) && allAsteroids.HasComponent(entityB)) {
                UnityEngine.Debug.Log($"La entidad A(Spaceship) colisiono con la entidad B(Asteroid)");
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SpaceshipAsteroidCollisionSystemJob();
        job.allSpaceships = GetComponentDataFromEntity<SpaceshipTag>(true);
        job.allAsteroids = GetComponentDataFromEntity<AsteroidTag>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
}
