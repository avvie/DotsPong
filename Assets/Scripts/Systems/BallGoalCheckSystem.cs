using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class BallGoalCheckSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Create a buffer to store operations
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        //system that checks for score
        Entities
            .WithAll<BallTag>()
            .WithoutBurst()
            .ForEach((Entity entity, in Translation trans) =>
            {
                float3 pos = trans.Value;
                float bound = Manager.Instance.xBound;
                //check if the x position of the ball is within bounds, if not: destroy ball and give score to player
                if (pos.x > bound)
                {
                    Manager.Instance.PlayerScored(0);
                    ecb.DestroyEntity(entity);
                }
                else if (pos.x <= -bound)
                {
                    Manager.Instance.PlayerScored(1);
                    ecb.DestroyEntity(entity);
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        return default;
    }
}