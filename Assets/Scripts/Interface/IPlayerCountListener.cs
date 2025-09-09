using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCountListener
{
    public void OnPlayerCountChanged(int curPlayer, int totalPlayer);
}
