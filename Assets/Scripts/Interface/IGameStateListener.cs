using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateListener
{
    public void OnStateChanged(GameState state);
}
