using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(PaddleMovementSystem)), UpdateAfter(typeof(PLayerInputSystem))]
public class AISystem : JobComponentSystem
{
    private EntityManager manager;
    private float dt;
    private NativeArray<Entity> temp;
    private bool control;
    EntityQuery q;
    
    protected override void OnCreate()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //do an entity querry, to find ball(s)
        q = GetEntityQuery(typeof(SpeedIncreaseOverTimeData));
        temp = q.ToEntityArray(Allocator.TempJob);
        //capture global variable outside of loop
        dt = Time.DeltaTime;

        Entities
            .WithAll<AITag>()
            .WithoutBurst()
            .ForEach((ref PaddleMovementData input, ref DistanceFromBallData dist, in Translation pos) =>
            {
                //helper variable
                float newDist = -100;
                //local refs are faster so we keep one
                Translation ballTrans;
                //initialization of y so compiler wont cry
                ballTrans.Value.y = 0;
                //case where there exists at least one ball
                if (temp.Length > 0)
                {
                    //atm there can be only one, so take a local ref 
                    ballTrans = manager.GetComponentData<Translation>(temp[0]);
                    //calculate distance of paddle to ball in this frame
                    newDist = math.distance(ballTrans.Value.x, pos.Value.x);
                }
                
                //if we have no balls 
                if (temp.Length == 0)
                {
                    //check if we are away from the center
                    if (math.abs(pos.Value.y) > 0.05f)
                    {
                        //.. and move towards it
                        input.direction = (int)math.sign(-pos.Value.y);
                    }
                    else
                    {
                        //.. stay put
                        input.direction = 0;
                    }
                }
                //if ball is moving away 
                else if (newDist - dist.Value > 0)
                {
                    //if away from center
                    if (math.abs(pos.Value.y) > 0.05f)
                    {
                        //.. move towards it
                        input.direction = (int)math.sign(-pos.Value.y);
                    }
                    else
                    {
                        //.. stay put
                        input.direction = 0;
                    }
                    
                }
                //else we need to react
                else
                {
                    //if we are not on top of ball
                    if (math.abs(ballTrans.Value.y - pos.Value.y) > 0.05f)
                    {
                        //.. move towards it
                        input.direction = (int) math.sign(ballTrans.Value.y - pos.Value.y);
                    }
                    else
                    {
                        //.. stay put
                        input.direction = 0;
                    }
                }

                dist.Value = newDist;
            }).Run(); //run main thread
        //clear memmory
        temp.Dispose();
        return inputDeps;
    }
}
