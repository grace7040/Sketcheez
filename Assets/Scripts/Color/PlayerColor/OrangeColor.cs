using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class OrangeColor : IColorState
{
    public int Damage { get { return 25; } }
    public bool WallSliding { get { return false; } }
    public float CoolTime { get { return 6f; } }

    public float durationTime = 5f;

    Action<float> OnOrangeAttacked = null;

    public OrangeColor(Action<float> onOrangeAttacked)
    {
        OnOrangeAttacked = onOrangeAttacked;
    }
    public void Attack(Vector3 playerPosition, float playerLocalScaleX)
    {
        AudioManager.Instacne.PlaySFX("Orange");
        OnOrangeAttacked(durationTime);
    }
}