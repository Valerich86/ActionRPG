
using UnityEngine;


public enum AttackType { melee, long_range }
public class WeaponController : MonoBehaviour
{
    public AttackType Type;
    public float Damage;
    public float Cooldown;
    public float Range;
    public Vector3 WeaponRange;
  
}
