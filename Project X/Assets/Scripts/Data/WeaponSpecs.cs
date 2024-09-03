using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSpecs
{
    public int gunDamage = 1;
    public float fireRate = 0.25f;    
    public float weaponRange = 50f;                                        
    public float hitForce = 100f;
    public bool isAutomatic = true;
}
