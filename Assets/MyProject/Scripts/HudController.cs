using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image _healthbar;

    private HPController _hpController;
    void Start()
    {
        _hpController = FindObjectOfType<PlayerController>().GetComponent<HPController>();
        _hpController.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(HPController health) => _healthbar.fillAmount = health.CurrentHP / health.MaxHP;

    private void OnDisable() => _hpController.OnHealthChanged -= OnHealthChanged;
}
