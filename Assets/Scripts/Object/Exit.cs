using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Exit : MonoBehaviour
{
    public bool open = false;
    public float smoot = 0.1f;
    

    private Vector3 localPositoin = transform.position;
    private Vector3 offset = new Vector3(0, 5f, 0);

    public PhotonView pv;

    private void Start()
    {
        pv = gameObject.AddComponent<PhotonView>();
        pv.ViewID = PhotonNetwork.AllocateViewID(0);
    }

    public IEnumerator OpenDoor(Transform obsTransform)
    {
        Debug.Log("Exit OpenDoor() 코루틴 실행됨 ");
        //float timecnt = 0.0f;

        while (open && Quaternion.Angle(obsTransform.rotation, Quaternion.Euler(OpenDoorAngle)) > 0)  //문이 열려야하고 두사이각이 0보다 큰 경우 실행
        {
            yield return null;
            //Debug.Log("open while문 실행");
            obsTransform.rotation = Quaternion.Slerp(obsTransform.rotation, Quaternion.Euler(OpenDoorAngle), smoot);
            //timecnt += Time.deltaTime;
        }

    }

    public IEnumerator CloseDoor(Transform obsTransform)
    {

        Debug.Log("Exit CloseDoor() 코루틴 실행됨 ");
        float timecnt = 0.0f;

        while (!open && Quaternion.Angle(obsTransform.rotation, Quaternion.Euler(CloseDoorAngle)) > 0) //문이 닫혀야 하고 두사이각이 0보다 큰 경우 실행
        {
            yield return null; //yield return을 만나는 순간마다 다음 구문이 실행되는 프레임으로 나뉘게 됨 
            //Debug.Log("Close while문 실행");
            obsTransform.rotation = Quaternion.Slerp(obsTransform.rotation, Quaternion.Euler(CloseDoorAngle), smoot);
            timecnt += Time.deltaTime;
        }
    }


    [PunRPC]
    public void ChangeDoorState()
    {
        open = !open;

        if (open)
        {
            StartCoroutine(OpenDoor(transform));
        }
        else
        {
            StartCoroutine(CloseDoor(transform));
        }

    }

    public void ChangeDoorStateRPC()
    {
        pv.RPC("ChangeDoorState", RpcTarget.AllBuffered);
    }

}
