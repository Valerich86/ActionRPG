using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Enemy", menuName = "Config/Enemy", order = 0)]
public class EnemySO : ScriptableObject
{
    [field: SerializeField] public GameObject Clone { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [field: SerializeField] public float MaxHP { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float AgentEnabledDistance { get; private set; }
    [field: SerializeField] public float ShootingDistance { get; private set; }
    [field: SerializeField] public float MeleeAttackDistance { get; private set; }

    [field: SerializeField] public DefenseType Defense;
}
