using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(AISystem))]
public class PLayerInputSystem : JobComponentSystem
{
    private EntityManager manager;
    private Entity paddleLeft, paddleRight;
    
    protected override void OnCreate()
    {
        //get a reference to the active world manager
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    /*    precondition: this will run once after the first time that one pushes 
    *     a button to change activate or deactivate an "ai"
    */
    private void InitPaddleRefs()
    {
        //query for paddles
        EntityQuery q = GetEntityQuery(typeof(PaddleInputData));
        NativeArray<Entity> temp = q.ToEntityArray(Allocator.TempJob);
        //figure out wich is which paddle is which based on initial scene configuration
        for (int i = 0; i < temp.Length; i++)
        {
            if (manager.HasComponent<AITag>(temp[i]))
            {
                paddleRight = temp[i];
            }
            else
            {
                paddleLeft = temp[i];
            }
        }
        temp.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Input handling for "ai" activation/deactivation
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (paddleLeft == Entity.Null || paddleRight == Entity.Null)
            {
                InitPaddleRefs();
            }

            if (manager.HasComponent<AITag>(paddleRight))
            {
                manager.RemoveComponent<AITag>(paddleRight);    
            }
            else
            {
                manager.AddComponent<AITag>(paddleRight);
            }
        }
        //Input handling for "ai" activation/deactivation
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (paddleLeft == Entity.Null || paddleRight == Entity.Null)
            {
                InitPaddleRefs();
            }
            
            if (manager.HasComponent<AITag>(paddleLeft))
            {
                manager.RemoveComponent<AITag>(paddleLeft);    
            }
            else
            {
                manager.AddComponent<AITag>(paddleLeft);
            }
        }
        
        //Handle user input: gameplay
        Entities.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
            {
                moveData.direction = 0;
                moveData.direction += Input.GetKey(inputData.upKey) ? 1 : 0;
                moveData.direction -= Input.GetKey(inputData.downKey) ? 1 : 0;
                
            }).Run();
        
        return inputDeps;
    }
}