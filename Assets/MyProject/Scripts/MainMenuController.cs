using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _start;

    private void OnEnable() => _start.onClick.AddListener(StartClicked);
    private void StartClicked() => SceneManager.LoadScene(0);
    private void OnDisable() => _start.onClick.RemoveListener(StartClicked);
}
