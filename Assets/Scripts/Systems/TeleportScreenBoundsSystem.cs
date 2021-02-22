using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveForwardSystem))]
[UpdateAfter(typeof(ConstantMoveSystem))]
public class TeleportScreenBoundsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        float screenLeft = ScreenCorners.LowerLeft.Data.x;
        float screenRight = ScreenCorners.UpperRight.Data.x;
        float screenBottom = ScreenCorners.LowerLeft.Data.y;
        float screenTop = ScreenCorners.UpperRight.Data.y;
        float screenWidth = screenRight - screenLeft;
        float screenHeight = screenTop - screenBottom;

        JobHandle myJob = Entities
            .WithAny<MovementData, ConstantMovementData>()
            .ForEach((ref Translation tr) =>
        {
            // Limite horizontal de la pantalla
            if (tr.Value.x < screenLeft) {
                tr.Value.x += screenWidth;
            } else if (tr.Value.x > screenRight) {
                tr.Value.x -= screenWidth;
            }
            // Limite vertical de la pantalla
            if (tr.Value.y < screenBottom) {
                tr.Value.y += screenHeight;
            } else if (tr.Value.y > screenTop) {
                tr.Value.y -= screenHeight;
            }
        }).Schedule(inputDeps);

        return myJob;
    }


}
