using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class IncreaseVelocityOverTimeSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //get enviroment data
        float deltaTime = Time.DeltaTime;
        //nudge the ball a little bit constantly, to introduce some interesting noise
        Entities.ForEach((ref PhysicsVelocity vel, in SpeedIncreaseOverTimeData data)=>
        {
            float2 modifier = new float2((data.increasePerSecond * deltaTime) * UnityEngine.Random.value * math.sign(vel.Linear.x)
                , UnityEngine.Random.value * (data.increasePerSecond * deltaTime) * math.sign(vel.Linear.y));
            
            vel.Linear.xy += modifier;
        }).Run();

        return default;
    }
}