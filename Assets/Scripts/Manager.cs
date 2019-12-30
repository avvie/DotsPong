using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    public GameObject BallPrefab;

    public float xBound;
    public float yBound;
    public float ballspeed;
    public float respawnDelay;
    public int[] playerScores;

    public Text mainText, Tutorial;
    public Text[] playerTexts;

    Entity BallEntityPrefab;
    EntityManager manager;

    WaitForSeconds oneSecond;
    WaitForSeconds delay;

    private BlobAssetStore blob;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        //Initialization of pong & dots
        playerScores = new int[2];
        //get a manager reference
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //make some conversions from normal unity scene
        blob = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        BallEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(BallPrefab, settings);
        
        oneSecond = new WaitForSeconds(1f);
        delay = new WaitForSeconds(respawnDelay);
        //show tutorial info
        StartCoroutine(tutorialTimer(delay));
        //start the game
        StartCoroutine(CountdownAndSpawnBall());
    }

    //assigns score to the player
    public void PlayerScored(int playerId)
    {
        playerScores[playerId]++;
        for (int i = 0; i < playerScores.Length && i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerScores[i].ToString();
        }

        StartCoroutine(CountdownAndSpawnBall());
    }

    //countsdown the start of the game
    IEnumerator CountdownAndSpawnBall()
    {
        
        mainText.text = "Get Ready";
        yield return delay;

        for (int i = 3; i > 0; i--)
        {
            mainText.text = i.ToString();
            yield return oneSecond;
        }

        mainText.text = "";
        SpawnBall();
    }

    IEnumerator tutorialTimer(WaitForSeconds delay)
    {
        yield return delay;
        Tutorial.text = "";
    }
    
    IEnumerator tutorialTimer(WaitForSeconds delay, string text)
    {
        Tutorial.text = text;
        yield return delay;
        Tutorial.text = "";
    }

    // spawn a new ball
    public void SpawnBall()
    {
        Entity ball = manager.Instantiate(BallEntityPrefab);
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1, 1) >= 0 ? -1 : 1, UnityEngine.Random.Range(-0.8f, 0.8f), 0);
        Vector3 speed = dir * ballspeed;

        PhysicsVelocity velocity = new PhysicsVelocity()
        {
            Linear = speed.normalized * ballspeed,
            Angular = float3.zero
        };

        manager.AddComponentData(ball, velocity);
    }

    private void OnApplicationQuit()
    {
        blob.Dispose();
    }
}
