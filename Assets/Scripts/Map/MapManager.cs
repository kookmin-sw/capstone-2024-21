using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MapManager : MonoBehaviour
{
    GameObject[] gameObjs;

    private void Awake()
    {
        gameObjs = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjs.Length; i++)
        {
            if (gameObjs[i].name.Contains("Door") && !gameObjs[i].name.Contains("Frame") && !gameObjs[i].name.Contains("Window"))
            {
                Debug.Log("gameObjs[i].name.Length : " + gameObjs[i].name.Length + "idx of L : " + gameObjs[i].name.LastIndexOf("L"));
                if (gameObjs[i].name.LastIndexOf("L") == gameObjs[i].name.Length-1)
                {
                    Debug.Log("hit!");
                    addDoorLeftScript(gameObjs[i]);
                }
                else
                {
                    addDoorRightScript(gameObjs[i]);
                }
                
                gameObjs[i].tag = "door";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
                gameObjs[i].isStatic = false; // 이걸 해줘야 회전함!!

                // 문에 PhotonView 컴포넌트 추가
                // gameObjs[i].AddComponent<PhotonView>();
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void addDoorRightScript(GameObject obj)
    {
        //Door 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<DoorRight>() != null)
        {
            obj.gameObject.GetComponent<DoorRight>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어줌
            DoorRight door = obj.gameObject.AddComponent<DoorRight>();
            door.enabled = true;
            //obj.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            //obj.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;
        }
    }

    void addDoorLeftScript(GameObject obj)
    {
        //Door 컴포넌트가 있으면 그냥 활성화
        if (obj.gameObject.GetComponent<DoorLeft>() != null)
        {
            obj.gameObject.GetComponent<DoorLeft>().enabled = true;
        }
        else
        {
            //없으면 찾아서 넣어줌
            DoorLeft door = obj.gameObject.AddComponent<DoorLeft>();
            door.enabled = true;
            //obj.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            //obj.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;
        }
    }
}
