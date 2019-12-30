using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PaddleMovementSystem : JobComponentSystem
{
    private float yBound, deltaTime;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //grab some enviroment data, that neeeds to be defined localy, so every frame :S
        float deltaTime = Time.DeltaTime;
        float yBound = Manager.Instance.yBound;
        
        Entities.ForEach((ref Translation trans, in PaddleMovementData data) =>
            {
                //calculate new y position for paddle
                trans.Value.y = math.clamp(
                    math.lerp(trans.Value.y, trans.Value.y + data.direction, data.speed * deltaTime)
                    , -yBound, yBound);
                
            }).Run();

        return inputDeps;
    }
}