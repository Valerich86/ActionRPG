using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IMortal
{

    private CharacterController _controller;


    private void OnEnable()
    {
        EventManager.OnDead += Dying;
        _controller = GetComponent<CharacterController>();
    }



    public void Dying()
    {
        _controller.enabled = false;
    }
}
