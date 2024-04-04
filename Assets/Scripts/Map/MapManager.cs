using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//MapManager(emptyObject) 오브젝트에 들어가있
public class MapManager : MonoBehaviour
{
    GameObject[] gameObjs;

    //맵의 문들과 상호작용 할 수 있도록 해줌 
    private void Awake()
    {
        gameObjs = FindObjectsOfType<GameObject>(); //게임오브젝트 모조리 넣어주고 

        for(int i = 0; i < gameObjs.Length; i++)
        {
            //오브젝트 이름에 "Door"가 들어가 있는데, "Frame"은 들어가지 않았다면 	
            if (gameObjs[i].name.Contains("Door") && !gameObjs[i].name.Contains("Frame"))
            {
                addDoorScript(gameObjs[i]);//스크립트 넣어주고 
                gameObjs[i].tag = "door"; // 태그 설정해주고 
                gameObjs[i].layer = LayerMask.NameToLayer("Interact"); //레이어 설정해주고 
                gameObjs[i].isStatic = false; // static풀어줌 - 이걸 해줘야 회전함!!

                // 문에 PhotonView 컴포넌트 추가
                // gameObjs[i].AddComponent<PhotonView>();
            }
        }

    }

    //Door 스크립트를 넣어주는 함수
    void addDoorScript(GameObject obj)
    {
        //Door 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<Door>() != null)
        {
            obj.gameObject.GetComponent<Door>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어주고 활성화
            Door door = obj.gameObject.AddComponent<Door>();
            door.enabled = true;
            //obj.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            //obj.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;
        }
    }
}
