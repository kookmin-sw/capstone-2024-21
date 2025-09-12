using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHpListener
{
    public void OnHpChanged(float hp, float maxHp);
}
