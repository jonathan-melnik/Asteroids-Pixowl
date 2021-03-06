using EazyTools.SoundManager;
using JonMelnik.Game;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SpaceshipHyperspaceSystem))]
[AlwaysSynchronizeSystem]
public class SpaceshipHyperspaceControllerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float dt = Time.DeltaTime;
        float screenLeft = ScreenCorners.LowerLeft.Data.x;
        float screenRight = ScreenCorners.UpperRight.Data.x;
        float screenBottom = ScreenCorners.LowerLeft.Data.y;
        float screenTop = ScreenCorners.UpperRight.Data.y;

        Entities
            .WithAll<SpaceshipTag>()
            .WithoutBurst()
            .ForEach((ref Translation tr, ref MovementData movement, ref HyperspaceData hyperspace) =>
            {
                hyperspace.timer = math.max(hyperspace.timer - dt, 0);
                if (Input.GetKeyDown(KeyCode.Z) && hyperspace.timer == 0) {
                    float x = UnityEngine.Random.Range(screenLeft, screenRight);
                    float y = UnityEngine.Random.Range(screenBottom, screenTop);
                    float3 newPos = new float3(x, y, 0);
                    Game.instance.fxManager.PlayHyperspace(tr.Value, newPos);
                    SoundManager.PlaySound(SFX.game.spaceship.hyperspace);
                    tr.Value = newPos;
                    // Detengo a la nave
                    movement.accel = 0;
                    movement.velocity = float3.zero;
                    hyperspace.timer = hyperspace.cooldown;
                }
            }).Run();

        return default;
    }
}
