using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightController : MonoBehaviour
{
    PhotonView pv;
    MovementStateManager movementStateManager;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        movementStateManager = GetComponent<MovementStateManager>();
    }

    void Update()
    {
        if (!pv.IsMine) return;

        if (Input.GetButtonDown("qDown"))
        {
            pv.RPC(nameof(RPCFlashLight), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPCFlashLight()
    {
        movementStateManager.lightComponent.enabled = !movementStateManager.lightComponent.enabled;
    }
}