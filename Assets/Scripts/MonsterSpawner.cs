using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour,IDeathListener
{
    [SerializeField] private float interval = 300f;
    Transform[] spawnPoints;
    GameObject Robo;

    Coroutine spawnRoutine;

    public void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    public void OnStateChanged(GameState state)
    {
        switch(state)
        {
            case GameState.InGame:
                spawnRoutine = StartCoroutine(SpawnRoutine());
                break;
            case GameState.GameOver:
                StopCoroutine(spawnRoutine);
                break;
        }
    }

    void Awake()
    {
        spawnPoints = GameObject.Find("MonsterSpawns").GetComponentsInChildren<Transform>();
    }

    IEnumerator SpawnRoutine()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            while(true)
            {
                yield return new WaitForSeconds(interval);

                Transform monSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Robo = PhotonNetwork.Instantiate("Prefabs/HelperRobot", monSpawn.position, Quaternion.identity);
            }
        }
    }
}
