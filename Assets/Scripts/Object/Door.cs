using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Door : MonoBehaviour
{
    public bool open = false;
    public float smoot = 2f;

    public Vector3 doorOpenVector = new Vector3(0, -90f, 0);

    // 문은 초기 각도
    public Vector3 startAngle;

    Quaternion ToDoorAngle;

    public PhotonView pv;

    private void Start()
    {
        // 문의 초기 각도 저장
        startAngle = transform.eulerAngles;
        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, ToDoorAngle, smoot * Time.deltaTime);
    }

    [PunRPC]
    public void ChangeDoorStateRPC()
    {
        open = !open;
        ChangeDoorAngle(open);
    }

    public void ChangeDoorState()
    {
        pv.RPC("ChangeDoorStateRPC", RpcTarget.All);
    }

    void ChangeDoorAngle(bool open)
    {
        if (open)
        {
            ToDoorAngle = Quaternion.Euler(startAngle + doorOpenVector);

        }
        else
        {
            ToDoorAngle = Quaternion.Euler(startAngle);
        }

    }

}

