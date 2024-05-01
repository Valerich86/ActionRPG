using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Camera _caveCamera;
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private HudController _hud;
    [SerializeField] private InventoryController _inventory;
    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private Transform _weaponSpawnPosition;
    [SerializeField] private Transform _teleportSpawnPosition;
    [SerializeField] private GameObject _teleportClone;
    [SerializeField] private GameObject _lightningsClone;
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
        _mainCamera.Priority = 1;
        StaticData.OnEnemyDying += SpawnMoney;
        StaticData.OnCameraChanged += ChangeCameraPriority;
        StartCoroutine(LevelStart());
    }

    private IEnumerator LevelStart()
    {
        SpawnItems();
        yield return new WaitForSeconds(3);
        SpawnTeleport();
        yield return new WaitForSeconds(5);
        SpawnLightnings();
        yield return new WaitForSeconds(2);
        SpawnPlayerCharacter();
        yield return new WaitForSeconds(5);
        _hud.Construct(_player.GetComponent<HPController>());
        _inventory.Construct(_player, StaticData.PlayerRole.StartMoney);
        _hud.gameObject.SetActive(true);
        StaticData.OnGlobalHintChanged?.Invoke("Как всегда, мягкая посадка...", 5);
        yield return new WaitForSeconds(5);
        ActivateCamera();
        SpawnPlayerItems();
        StaticData.OnGlobalHintChanged?.Invoke("А вот и мои вещи!", 5);
        _caveCamera.gameObject.SetActive(false);
        SpawnEnemies();
    }

    void ActivateCamera()
    {
        _mainCamera.Follow = _player.transform;
        _mainCamera.LookAt = _player.transform;
        ChangeCameraPriority(10);
    }

    void ChangeCameraPriority(int priority) => _mainCamera.Priority = priority;

    void SpawnTeleport()
    {
        GameObject teleport = Instantiate(_teleportClone, _teleportSpawnPosition.position, Quaternion.identity);
        Destroy(teleport, 300);
    }

    void SpawnLightnings()
    {
        GameObject lightnings = Instantiate(_lightningsClone, _teleportSpawnPosition);
        Destroy(lightnings, 300);
    }
    void SpawnPlayerCharacter()
    {
        _player = Instantiate(StaticData.PlayerRole.Clone, _playerSpawnPosition.position, _playerSpawnPosition.rotation).GetComponent<PlayerController>();
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
        GameObject PlItem = Instantiate(StaticData.PlayerRole.Weapon.Clone, _weaponSpawnPosition.position, Quaternion.identity);
        Instantiate(_strongShiningFX, PlItem.transform);
    }

    void SpawnEnemies()
    {
        foreach (var enemy in _enemies)
        {
            for (int i = 0; i < enemy.SpawnPoints.Length; i++)
            {
                Instantiate(enemy.EnemyType.Clone, enemy.SpawnPoints[i].position, Quaternion.identity);
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

    private void OnDisable()
    {
        StaticData.OnEnemyDying -= SpawnMoney;
        StaticData.OnCameraChanged -= ChangeCameraPriority;
    }


    [Serializable]
    public class Enemies
    {
        public EnemySO EnemyType;
        public Transform[] SpawnPoints;
    }

    [Serializable]
    public class Items
    {
        public ItemSO ItemType;
        public int SpawnAmount;
    }

}
