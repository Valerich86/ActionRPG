using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image _healthbar;
    [SerializeField] private Image _expBar;
    [SerializeField] private Image _icon;
    [SerializeField] private Canvas _inventoryWindow;
    [SerializeField] private TextMeshProUGUI _hint;
    [SerializeField] private TextMeshProUGUI _globalHint;

    private HPController _hpController;
    private float _experience = 0;


    public void Construct(HPController hp)
    {
        //Cursor.lockState = CursorLockMode.Locked;
        _inventoryWindow.enabled = false;
        _hpController = hp;
        _hpController.OnHealthChanged += OnHealthChanged;
        StaticData.OnHintChanged += SetHint;
        StaticData.OnGlobalHintChanged += SetGlobalHint;
        StaticData.OnEnemyDying += OnExpChanged;
        _icon.sprite = StaticData.PlayerRole.Icon;
        _expBar.fillAmount = 0;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_inventoryWindow.enabled)
            {
                //Cursor.lockState = CursorLockMode.None;
                _inventoryWindow.enabled = true;
                Time.timeScale = 0;
                StaticData.DeactivateSaleWindow?.Invoke(false);
            }
            else
            {
                //Cursor.lockState = CursorLockMode.Locked;
                _inventoryWindow.enabled = false;
                Time.timeScale = 1;
                StaticData.DeactivateSaleWindow?.Invoke(true);
            }
        }
        _expBar.fillAmount = _experience / 1000;
    }

    private void OnHealthChanged(HPController health) => _healthbar.fillAmount = health.CurrentHP / health.MaxHP;

    private void OnExpChanged(EnemyController enemy)
    {
        _experience += enemy.Enemy.Exp;
        SetGlobalHint($"+ {enemy.Enemy.Exp} опыта !", 3);
    }


    public void SetGlobalHint(string hint, int time)
    {
        _globalHint.text = hint;
        Invoke("ClearGlobalHint", time);
    }

    public void SetHint(string hint)
    {
        _hint.text = hint;
        Invoke("ClearHint", 3f);
    }


    public void ClearHint() => _hint.text = string.Empty;

    public void ClearGlobalHint() => _globalHint.text = string.Empty;

    private void OnDisable()
    {
        StaticData.OnHintChanged -= SetHint;
        StaticData.OnGlobalHintChanged -= SetGlobalHint;
        _hpController.OnHealthChanged -= OnHealthChanged;
        StaticData.OnEnemyDying -= OnExpChanged;
    }
}
