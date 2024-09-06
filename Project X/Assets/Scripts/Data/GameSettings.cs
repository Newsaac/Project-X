using System;
using UnityEngine;

[Serializable]
public class GameSettings
{
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode reload = KeyCode.R;
    public float sensitivity = 1.0f;
    public float effectsVolume = 0.2f;
    public float deleteCorpsesIn = 15f;
}
