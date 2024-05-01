using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum ItemAction { Use, Sell};
public enum ItemAssignment { Equipment, Loot, Money }
public enum ItemType { Weapon, Armor, Shield, Ability, Food, Cure, Key, ForSale, Other }
public class InventoryController : MonoBehaviour
{

    [SerializeField] private EquipCellController[] _equipCells;
    [SerializeField] private LootCellController[] _lootCells;
    [SerializeField] private TextMeshProUGUI[] _descriptions;
    [SerializeField] private TextMeshProUGUI _moneyCounter;
    [SerializeField] private GameObject _arrow;
    [HideInInspector] public int ArrowAmount { get; private set; }

    private PlayerController _playerController;
    private int _lootsAmount = 0;
    private int _currentMoney;
    private bool _isStartItemPicked = false;
    private bool _isStartItemEquiped = false;


    public void Construct(PlayerController player, int startMoney)
    {
        _playerController = player;
        _currentMoney = startMoney;
        StaticData.OnItemPicked += CheckCell;
        StaticData.OnItemSold += SetMoney;
        StaticData.OnCellEnter += SetDescriptions;
        StaticData.OnCellExit += ClearDescriptions;
        StaticData.OnArrowAmountChanged += SetCurrentArrowAmount;
        _arrow.SetActive(false);
        gameObject.SetActive(true);
        ClearDescriptions();
    }

    private void Update()
    {
        _moneyCounter.text = _currentMoney.ToString();
        _arrow.GetComponentInChildren<TextMeshProUGUI>().text = ArrowAmount.ToString();
    }

    public void SetCurrentArrowAmount(int amount)
    {
        ArrowAmount += amount;
        _playerController.GetComponent<AttackController>().SetCurrentArrowsAmount(ArrowAmount);
    }

    private void ClearDescriptions()
    {
        foreach (var text in _descriptions)
        {
            text.text = string.Empty;
        }
    }

    private void SetDescriptions(ItemSO item)
    {
        _descriptions[0].text = $"'{item.Tytle}'";
        _descriptions[1].text = item.Description;
        _descriptions[2].text = $"Стоимость : {item.Price}";
    }

    void SetLoot(ItemSO item)
    {
        foreach (var cell in _lootCells)
        {
            if (cell.CurrentItem == item)
            {
                cell.Amount += 1;
                return;
            }
        }

        foreach (var cell in _lootCells)
        {
            if (cell.IsEmpty)
            {
                _lootsAmount += 1;
                cell.SetItem(item);
                return;
            }
        }
    }


    private void CheckCell(ItemController item)
    {
        if (_isStartItemPicked == false && item.ItemSO.WeaponSO != null)
        {
            _isStartItemPicked = true;
            StaticData.OnGlobalHintChanged?.Invoke("Надо бы проверить инвентарь...", 10);
        }
        if (item.ItemSO.Assignment == ItemAssignment.Money)
        {
            StaticData.OnHintChanged?.Invoke($"Вы подобрали '{item.ItemSO.Tytle}'");
            Destroy(item.gameObject);
            SetMoney(item.ItemSO.Price);
            return;
        }
        if (_lootsAmount < _lootCells.Length)
        {
            StaticData.OnHintChanged?.Invoke($"Вы подобрали предмет '{item.ItemSO.Tytle}'");
            Destroy(item.gameObject);
            SetLoot(item.ItemSO);
        }
        else StaticData.OnHintChanged?.Invoke("Недостаточно места");
    }

    public void SetWeapon(ItemSO weapon)
    {
        if (_isStartItemEquiped == false)
        {
            _isStartItemEquiped = true;
            StaticData.OnGlobalHintChanged?.Invoke("Отлично, теперь можно и в бой!", 5);
            StartCoroutine(ShowSomeHint("Неподалеку должна быть оружейная лавка. Возможно, найду там что-то полезное", 10));
        }
        ItemSO currentItem;
        if (!_equipCells[0].IsEmpty)
        {
            currentItem = _equipCells[0].CurrentItem;
            _equipCells[0].SetItem(weapon);
            SetLoot(currentItem);
        }
        else _equipCells[0].SetItem(weapon);
        if (weapon.WeaponSO.Type == AttackType.long_range)
        {
            _arrow.SetActive(true);
            SetCurrentArrowAmount(weapon.WeaponSO.StartArrowAmount);
        }
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerWeapon(weapon);
    }

    private void SetMoney(int money) => _currentMoney += money;
    private void SetShield(ItemSO itemSO)
    {
        throw new NotImplementedException();
    }

    private IEnumerator ShowSomeHint(string hint, float time)
    {
        yield return new WaitForSeconds(time);
        StaticData.OnGlobalHintChanged?.Invoke(hint, 10);
    }

    private void OnDisable()
    {
        StaticData.OnItemPicked -= CheckCell;
        StaticData.OnItemSold -= SetMoney;
        StaticData.OnCellEnter -= SetDescriptions;
        StaticData.OnCellExit -= ClearDescriptions;
        StaticData.OnArrowAmountChanged -= SetCurrentArrowAmount;
    }

}
