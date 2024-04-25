using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentController : MonoBehaviour
{

    public TerrainLayer LavaLayer;
    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private EnemySO[] _enemyTypes;
    [SerializeField] private int _enemyCount;

    private float _lavaMaxOffset = 5;
    private float _lavaOffsetCount = 0;
    void Awake()
    {
        Instantiate(StaticData.PlayerRole.Clone, _playerSpawnPosition.position, Quaternion.identity);
        //EnemyRespawn();
    }

    void Update()
    {
        _lavaOffsetCount += Time.deltaTime;
        if (_lavaOffsetCount >= _lavaMaxOffset) _lavaOffsetCount = 0;
        LavaLayer.tileOffset = new Vector2(0, _lavaOffsetCount);
    }

    private void EnemyRespawn()
    {
        foreach(var type in _enemyTypes)
        {
            for (int i = 0; i < _enemyCount; i++)
            {
                NavMeshTriangulation data = NavMesh.CalculateTriangulation();
                int index = Random.Range(0, data.vertices.Length);
                Vector3 randomPoint = data.vertices[index];
                Instantiate(type.Clone, randomPoint, Quaternion.identity);
            }
        }
    }
}
