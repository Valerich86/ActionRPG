using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Image _healthbar;
    [SerializeField] private HPController _hpController;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _hpController.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(HPController health) => _healthbar.fillAmount = health.CurrentHP / health.MaxHP;

    private void Update() => transform.LookAt(_camera.transform);

    private void OnDisable() => _hpController.OnHealthChanged -= OnHealthChanged;
}
