using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{

    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private Collider _terrain;
    [SerializeField] private List<Enemies> _enemies;
    [SerializeField] private List<Items> _items;
    [SerializeField] private GameObject _shiningFX;
    [SerializeField] private GameObject _strongShiningFX;
    [SerializeField] private ItemSO _money200;
    [SerializeField] private ItemSO _money500;
    [SerializeField] private ItemSO _money1000;

    private PlayerController _player;
    private GameObject _item;
    void Awake()
    {
        StaticData.OnEnemyDying += SpawnMoney;
        SpawnItems();
        SpawnPlayerCharacter();
        Invoke("SpawnPlayerItems", 3f);
    }

    void SpawnPlayerCharacter()
    {
        _player = Instantiate(StaticData.PlayerRole.Clone, _playerSpawnPosition.position, Quaternion.identity).GetComponent<PlayerController>();
    }

    void SpawnItems()
    {
        foreach (var item in _items)
        {
            for (int i = 0; i < item.SpawnAmount; i++)
            {
                float x = _terrain.transform.position.x + UnityEngine.Random.Range(100, _terrain.bounds.extents.x * 2 - 100);
                float z = _terrain.transform.position.z + UnityEngine.Random.Range(100, _terrain.bounds.extents.z * 2 - 100);
                float y = 30f;
                Vector3 spawnPos = new Vector3(x, y, z);
                _item = Instantiate(item.ItemType.Clone, spawnPos, Quaternion.identity);
                Instantiate(_shiningFX, _item.transform);
            }
        }
    }


    private void SpawnPlayerItems()
    {
        GameObject PlItem = Instantiate(StaticData.PlayerRole.Weapon.Clone, _player.transform.position + Vector3.forward * 20 + Vector3.up * 10, Quaternion.identity);
        Instantiate(_strongShiningFX, PlItem.transform);
        StaticData.OnGlobalHintChanged?.Invoke("В 20 метрах от Вас находится Ваше оружие. Найдите и заберите его", 1000);
    }

    void SpawnEnemies()
    {
        foreach (var enemy in _enemies)
        {
            for (int i = 0; i < enemy.SpawnAmount; i++)
            {
                float x = _terrain.transform.position.x + UnityEngine.Random.Range(100, _terrain.bounds.extents.x * 2 - 100);
                float z = _terrain.transform.position.z + UnityEngine.Random.Range(100, _terrain.bounds.extents.z * 2 - 100);
                float y = 2f;
                Vector3 spawnPos = new Vector3(x, y, z);
                Instantiate(enemy.EnemyType.Clone, spawnPos, Quaternion.identity);
            }
        }
    }

    void SpawnMoney(EnemyController enemy)
    {
        GameObject money;
        switch (enemy.Enemy.Reward)
        {
            case 200:
                money = Instantiate(_money200.Clone, enemy.transform.position + transform.up, Quaternion.identity);
                break;
            case 500:
                money = Instantiate(_money500.Clone, enemy.transform.position + transform.up, Quaternion.identity);
                break;
            case 1000:
                money = Instantiate(_money1000.Clone, enemy.transform.position + transform.up, Quaternion.identity);
                break;
            default: return;
        }
        Instantiate(_strongShiningFX, money.transform);
        money.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
    }



    [Serializable]
    public class Enemies
    {
        public EnemySO EnemyType;
        public int SpawnAmount;
    }

    [Serializable]
    public class Items
    {
        public ItemSO ItemType;
        public int SpawnAmount;
    }

}
