using UnityEngine;

public interface IPlayerStateListener
{
    public void OnPlayerStateChanged(MovementBaseState curState);
}
