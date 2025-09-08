using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;

public class AimStateManager : MonoBehaviourPun, IUIStateListener
{
    public Cinemachine.AxisState xAxis, yAxis;
    [SerializeField] Transform camFollowPos;

    [SerializeField] UIManager uiManager;

    private PhotonView pv;

    Coroutine UpdateRoutine;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        uiManager = FindObjectOfType<UIManager>();
        if (pv.IsMine)
        {
            var followCam = FindObjectOfType<CinemachineVirtualCamera>();
            followCam.Follow = this.camFollowPos.transform;
            followCam.LookAt = this.camFollowPos.transform;
        }
    }

    public void OnUIStateChanged(UIState state)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator UpdateAxis()
    {
        while(true)
        {
            if (pv.IsMine)
            {
                xAxis.Update(Time.deltaTime);
                yAxis.Update(Time.deltaTime);
            }

            yield return null;
        }
    }


    private void LateUpdate()
    {
        if (pv.IsMine)
        {
            camFollowPos.localEulerAngles = new Vector3(-yAxis.Value, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.Value, transform.eulerAngles.z);
        }
    }


}
