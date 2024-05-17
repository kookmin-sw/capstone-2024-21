using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemMoveManage : MonoBehaviour
{
    public float rotationSpeed = 60f;  // 초당 회전 속도

    bool is_collide = false;
    Vector3 stop_position;



    void Start()
    {
        //Destroy(gameObject, 5f); // 5초뒤 오브젝트를 파괴   
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);  // y 축으로 1 초에 60도씩 회전

        //바닥과 충돌했을때의 포지션으로 고정
        if (is_collide)
        {
            transform.position = stop_position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor") //충돌한 오브젝트의 태그가 floor이면 그때의 포지션 저장 	
        {
            is_collide = true;
            stop_position = transform.position;
        }
        else if (collision.gameObject.tag == "Player")
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        }
    }


}
