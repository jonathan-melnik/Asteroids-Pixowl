using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using System.Diagnostics;

[AlwaysSynchronizeSystem]
public class BombSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithAll<BombTag>()
            .ForEach((Entity entity, ref PhysicsCollider collider, in BombData bomb) =>
        {
            unsafe {
                // grab the sphere pointer
                SphereCollider* scPtr = (SphereCollider*)collider.ColliderPtr;
                float oldRadius = scPtr->Radius;
                float newRadius = oldRadius + bomb.expansionSpeed * dt;
                if (newRadius > bomb.targetRadius) {
                    ecb.DestroyEntity(entity);
                } else {
                    // update the collider geometry
                    var sphereGeometry = scPtr->Geometry;
                    sphereGeometry.Radius = newRadius;
                    scPtr->Geometry = sphereGeometry;
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;
    }
}
