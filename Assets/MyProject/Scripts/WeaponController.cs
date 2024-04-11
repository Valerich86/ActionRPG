
using UnityEngine;


public enum AttackType { melee, long_range }
public class WeaponController : MonoBehaviour
{
    public AttackType Type;
    public int Damage;
    public float Cooldown;
    public float Range;
    public Vector3 WeaponRange;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
