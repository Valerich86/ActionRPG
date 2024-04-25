using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _characterWindow;
    [SerializeField] private Button _startGame;
    [SerializeField] private List<RoleButton> _roleButtons;

    private void OnEnable()
    {
        _characterWindow.SetActive(false);
        _startGame.onClick.AddListener(StartClicked);
        foreach (var button in _roleButtons)
        {
            button.Init();
            button.OnClicked += OnClicked;
        }
    }

    private void OnClicked(Role role)
    {
        StaticData.PlayerRole = role;
        SceneManager.LoadScene(0);
    }

    private void StartClicked() => _characterWindow.SetActive(true);

    private void OnDisable()
    {
        _startGame.onClick.RemoveListener(StartClicked);
        foreach (var button in _roleButtons)
        {
            button.UnInit();
            button.OnClicked -= OnClicked;
        }
    }
}

[Serializable]
public class RoleButton
{
    public event Action<Role> OnClicked;
    public Role Role;
    public Button Button;

    public void Init() => Button.onClick.AddListener(OnButtonClicked);
    private void OnButtonClicked() => OnClicked?.Invoke(Role);
    public void UnInit() => Button.onClick.RemoveListener(OnButtonClicked);
}
