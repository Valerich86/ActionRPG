using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemSO ItemSO;

    private bool _isActive = true;
    private GameObject _newFX;
    private void OnTriggerEnter(Collider other)
    {
        if (_isActive && other.CompareTag("Player")) 
            StaticData.OnItemPicked?.Invoke(this);
    }

    public void Deactivate()
    {
        _isActive = false;
        Debug.Log("деактивация");
    }
    
}
