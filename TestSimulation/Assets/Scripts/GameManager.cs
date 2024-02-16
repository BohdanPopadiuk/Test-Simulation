using System;
using System.Collections;
using ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static Action<AgentController> AgentDied;
    [SerializeField] private AgentController agentPrefab;
    
    [Header("Agents Count")]
    [SerializeField] private int agentsCountAtStart = 3;
    [SerializeField] private int maxAgentsCount = 30;
    
    [Header("SpawnDuration")]
    [SerializeField] private float minSpawnDuration = 2;
    [SerializeField] private float maxSpawnDuration = 6;

    [Header("SpawnPoints")]
    [SerializeField] private Transform topLeftSpawnPoint;
    [SerializeField] private Transform bottomRightSpawnPoint;
    
    private GameObjectPool _agentPool;
    private int _agentsCount;//test

    void Awake()
    {
        _agentPool = new GameObjectPool(agentPrefab.gameObject, 30);
        AgentDied += ReturnAgentToPool;

        for (int i = 0; i < agentsCountAtStart; i++)
        {
            AgentSpawn();
        }
        
        Debug.Log("Start spawner");
        StartCoroutine(AgentSpawner());
    }

    private void OnDestroy()
    {
        AgentDied -= ReturnAgentToPool;
    }

    private void ReturnAgentToPool(AgentController agentController)
    {
        _agentPool.Return(agentController.gameObject);
        _agentsCount--;

        if (_agentsCount >= maxAgentsCount - 1)
        {
            Debug.Log("Start spawner (return)");
            StartCoroutine(AgentSpawner());
        }
    }

    private void AgentSpawn()
    {
        _agentsCount++;
        GameObject newAgent = _agentPool.Get();
        newAgent.transform.position = CalculateAgentPos();
    }

    private Vector3 CalculateAgentPos()
    {
        Vector3 topLeftPos = topLeftSpawnPoint.position;
        Vector3 bottomRightPos = bottomRightSpawnPoint.position;
        
        float x = Random.Range(topLeftPos.x, bottomRightPos.x);
        float y = 1.5f;
        float z = Random.Range(bottomRightPos.z, topLeftPos.z);
        //check the distance to the nearest agent
        return new Vector3(x, y, z);
    }

    private IEnumerator AgentSpawner()
    {
        while (_agentsCount < maxAgentsCount)
        {
            float randomDuration = Random.Range(minSpawnDuration, maxSpawnDuration);
            yield return new WaitForSeconds(randomDuration);
            AgentSpawn();
        }
        Debug.Log("Stop spawner");
    }
}
