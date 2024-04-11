using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Door : MonoBehaviour
{
    public bool open = false;
    public float smoot = 2f;

    public Vector3 doorOpenVector = new Vector3(0, -90f, 0);

    Quaternion ToDoorAngleRight;
    Quaternion ToDoorAngleLeft;

    public PhotonView pv;

    private void Start()
    {
        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }

    private void Update()
    {
        if (transform.parent.name.Contains("axis"))
        {
            transform.parent.rotation = Quaternion.Slerp(transform.rotation, ToDoorAngleRight, smoot * Time.deltaTime);
        }
        else
        {
            if(transform.name.Contains("L"))
            transform.rotation = Quaternion.Slerp(transform.rotation, ToDoorAngleLeft, smoot * Time.deltaTime);
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, ToDoorAngleRight, smoot * Time.deltaTime);
            }
        }

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
            ToDoorAngleRight = Quaternion.Euler(transform.eulerAngles + doorOpenVector);
            ToDoorAngleLeft = Quaternion.Euler(transform.eulerAngles - doorOpenVector);


        }
        else
        {
            ToDoorAngleRight = Quaternion.Euler(transform.eulerAngles - doorOpenVector);
            ToDoorAngleLeft = Quaternion.Euler(transform.eulerAngles + doorOpenVector);

        }

    }

}

