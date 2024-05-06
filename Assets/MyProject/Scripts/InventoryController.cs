using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public enum ItemAssignment { Equipment, Loot, Money, SetOfArrows }
public enum ItemType { Weapon, Armor, Shield, Quiver, Ability, Cure, Key, ForSale, Other }
public class InventoryController : MonoBehaviour
{

    [SerializeField] private EquipCellController[] _equipCells;
    [SerializeField] private LootCellController[] _lootCells;
    [SerializeField] private TextMeshProUGUI[] _descriptions;
    [SerializeField] private TextMeshProUGUI _moneyCounter;
    [SerializeField] private GameObject _arrow;
    [HideInInspector] public int ArrowAmount { get; private set; } = 20;
    [HideInInspector] public int MaxArrowAmount { get; private set; } = 20;

    private PlayerController _playerController;
    private int _lootsAmount = 0;
    private int _currentMoney;
    private bool _isStartItemPicked = false;
    private bool _isStartItemEquiped = false;
    private Dictionary<ItemSO, int> _pickedItems = new Dictionary<ItemSO, int>();

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
        _arrow.GetComponentInChildren<TextMeshProUGUI>().text = $"{ArrowAmount} / {MaxArrowAmount}";
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    while (_pickedItems.Count > 0)
        //    {
        //        foreach (var cell in _lootCells)
        //        {
        //            if (cell.IsEmpty) continue;
        //            cell.ClearCell();
        //            SetMoney(cell.CurrentItem.Price);
        //        }
        //    }
        //}
    }

    public void SetCurrentArrowAmount(int amount)
    {
        ArrowAmount += amount;
        if (ArrowAmount > MaxArrowAmount) ArrowAmount = MaxArrowAmount;
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

    private void SetLoot(ItemSO item)
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

    public void UpdateList()
    {
        _pickedItems.Clear();
        foreach (var cell in _lootCells)
        {
            if (!cell.IsEmpty)
            {
                try
                {
                    _pickedItems.Add(cell.CurrentItem, cell.Amount);
                }
                catch (Exception)
                {
                    _pickedItems[cell.CurrentItem] += 1;
                }
            }
        }
        foreach (var cell in _equipCells)
        {
            if (!cell.IsEmpty)
            {
                try
                {
                    _pickedItems.Add(cell.CurrentItem, 1);
                }
                catch (Exception)
                {
                    _pickedItems[cell.CurrentItem] += 1;
                }
            }
        }
        Debug.Log(_pickedItems.Count);
    } 

    public void ChangeStatus(bool canUse)
    {
        foreach (var cell in _lootCells)
        {
            if (cell.IsEmpty) continue;
            cell.CanUse = canUse;
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
            StaticData.OnHintChanged?.Invoke($"Вы приобрели предмет '{item.ItemSO.Tytle}'");
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
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerWeapon(weapon);
        if (weapon.WeaponSO.Type == AttackType.long_range)
        {
            _arrow.SetActive(true);
            _playerController.gameObject.GetComponent<AttackController>().SetCurrentArrowsAmount(ArrowAmount);
        }
    }

    public void SetShield(ItemSO shield)
    {
        ItemSO currentItem;
        if (!_equipCells[1].IsEmpty)
        {
            currentItem = _equipCells[1].CurrentItem;
            _equipCells[1].SetItem(shield);
            SetLoot(currentItem);
        }
        else _equipCells[1].SetItem(shield);
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerShield(shield);
    }

    public void SetHelmet(ItemSO helmet)
    {
        ItemSO currentItem;
        if (!_equipCells[2].IsEmpty)
        {
            currentItem = _equipCells[2].CurrentItem;
            _equipCells[2].SetItem(helmet);
            SetLoot(currentItem);
        }
        else _equipCells[2].SetItem(helmet);
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerHelmet(helmet);
    }

    public void SetQuiver(ItemSO quiver)
    {
        ItemSO currentItem;
        if (!_equipCells[3].IsEmpty)
        {
            currentItem = _equipCells[3].CurrentItem;
            _equipCells[3].SetItem(quiver);
            SetLoot(currentItem);
        }
        else _equipCells[3].SetItem(quiver);
        MaxArrowAmount *= 2;
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerQuiver(quiver);
    }

    private void SetMoney(int money) => _currentMoney += money;
   
    private IEnumerator ShowSomeHint(string hint, float time)
    {
        yield return new WaitForSeconds(time);
        StaticData.OnGlobalHintChanged?.Invoke(hint, 10);
    }

    public void PurchaseItem(ItemController item)
    {
        if (item.ItemSO.Price > _currentMoney)
        {
            StaticData.OnHintChanged("Недостаточно денег");
            return;
        }
        else
        {
            SetMoney(- item.ItemSO.Price);
            CheckCell(item);
            UpdateList();
        }
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
