using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Config/Weapon", order = 0)] 


public class Weapon : ScriptableObject
{
    [field: SerializeField] public GameObject Clone { get; private set; }
    [field: SerializeField] public AttackType Type { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public float MeleeCooldown { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public Vector3 WeaponRange { get; private set; }
}
