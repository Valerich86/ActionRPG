using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class LootCellController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Sprite _startSprite;
    [SerializeField] private InventoryController _inventory;

    [HideInInspector] public ItemSO CurrentItem;
    [HideInInspector] public bool IsEmpty = true;
    [HideInInspector] public int Amount = 0;
    [HideInInspector] public ItemAction _action;

    private Image _image;
    private TextMeshProUGUI _counter;
    private PlayerController _player;

    private void Start()
    {
        _image = GetComponent<Image>();
        _counter = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Amount > 1) _counter.text = Amount.ToString();
        else _counter.text = string.Empty;
    }
    public void SetItem(ItemSO item)
    {
        CurrentItem = item;
        IsEmpty = false;
        _image.sprite = CurrentItem.Icon;
        Amount += 1;
        _action = ItemAction.Use;
    }

    public void ClearCell()
    {
        Amount -= 1;
        if (Amount == 0)
        {
            CurrentItem = null;
            IsEmpty = true;
            _image.sprite = _startSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = Color.blue;
        if (!IsEmpty) StaticData.OnCellEnter?.Invoke(CurrentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = Color.white;
        if (!IsEmpty) StaticData.OnCellExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsEmpty)
        {
            if (_action == ItemAction.Use)
            {
                switch (CurrentItem.Type)
                {
                    case ItemType.Food:
                        _player.GetComponent<HPController>().Healing(10);
                        break;
                    case ItemType.Cure:
                        _player.GetComponent<HPController>().Healing(CurrentItem.HealPercents);
                        break;
                    case ItemType.Weapon:
                        _inventory.SetWeapon(CurrentItem);
                        break;
                    default: break;
                }
            }
            else StaticData.OnItemSold?.Invoke(CurrentItem.Price);

            ClearCell();
        }
    }
}
