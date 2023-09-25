using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColorState
{
    public float JumpForce { get; }
    public void Attack(PlayerController player);

}
