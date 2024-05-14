using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SaveService.IsLoading = false;
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 1) SceneManager.LoadScene(2);
        else SceneManager.LoadScene(1);
    }
}
