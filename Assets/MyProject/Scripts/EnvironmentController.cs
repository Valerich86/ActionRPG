using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    public TerrainLayer WaterLayer;
    [SerializeField] private float _waterOffsetY;

    private bool _tide;
    private float _waterOffsetCount;
    void Start()
    {
        _waterOffsetCount = _waterOffsetY;
    }

    void Update()
    {
        TideChanging();
        _waterOffsetCount = (_tide == false) ? _waterOffsetCount - Time.deltaTime : _waterOffsetCount + Time.deltaTime;
        WaterLayer.tileOffset = new Vector2(0, _waterOffsetCount);
    }

    private void TideChanging()
    {
        if (_waterOffsetCount >= _waterOffsetY) _tide = false;
        if (_waterOffsetCount <= 0) _tide = true;
    }
}
