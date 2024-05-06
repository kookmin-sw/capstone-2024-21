using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Exit : MonoBehaviour
{
    [SerializeField] private bool open = false;
    private Vector3 localPositoin;
    private float offset = 2f;
    private float speed = 1.3f;

    public PhotonView pv;

    private void Start()
    {
        localPositoin = transform.position;

        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }

    private IEnumerator ExitOpenDoor(Transform obsTransform)
    {
        Debug.Log("Exit OpenDoor() 코루틴 실행됨 ");


        Debug.Log("localPositoin :"+ localPositoin);
        while (transform.position.y < localPositoin.y + offset)
        {
            //Debug.Log(transform.position.y);
            yield return null;
            obsTransform.Translate( Vector3.forward * speed * Time.deltaTime);
        }
        Debug.Log("localPositoin :" + transform.position);
    }

    private IEnumerator ExitCloseDoor(Transform obsTransform)
    {
        Debug.Log("Exit OpenDoor() 코루틴 실행됨 ");

        while (transform.position.y > localPositoin.y)
        {
            yield return null;
            obsTransform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }



    [PunRPC]
    public void ChangeExitDoorState()
    {
        open = !open;

        if (open)
        {
            StartCoroutine(ExitOpenDoor(transform));
        }
        else
        {
            StartCoroutine(ExitCloseDoor(transform));
        }

    }

    public void ChangeExitDoorStateRPC()
    {
        pv.RPC("ChangeExitDoorState", RpcTarget.AllBuffered);
    }

}
