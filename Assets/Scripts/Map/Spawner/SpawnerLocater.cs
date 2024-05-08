using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerLocater : MonoBehaviour
{
    GameObject[] gameObjs; //모든 오브젝트들 

    List<GameObject> ItemSpawnerTargets = new List<GameObject>();//스포너 후보들
    int ItemSpawnerCount = 4;


    private void Awake()
    {
        gameObjs = FindObjectsOfType<GameObject>();

        for(int i = 0; i < gameObjs.Length; i++)
        {
            //게임 오브젝트들 중에서 이름에 ItemSpawner가 들어가면 스포너 후보에 추가하고 태그와 레이어 설정
            // 배터리가 추출 당했어도 우선 상호작용은 되야하니까 
            if (gameObjs[i].name.Contains("ItemSpawner"))
            {
                ItemSpawnerTargets.Add(gameObjs[i]);
                
                gameObjs[i].tag = "ItemSpawner";
                gameObjs[i].layer = LayerMask.NameToLayer("Interact");
                //gameObjs[i].isStatic = false; // 이거 해줘야 하나? 
            }
        }

        int cnt = 0;
        while (true)
        {
            int i = Random.Range(0, ItemSpawnerTargets.Count); //랜덤으로 인덱스 뽑아서
                                         
            if (gameObjs[i].gameObject.GetComponent<ItemSpawner>() == null)
            {
                //없으면 찾아서 넣어주고 활성화 
                ItemSpawner itemSpawner = gameObjs[i].AddComponent<ItemSpawner>();
                itemSpawner.enabled = true;
                cnt++;
            }

            if(cnt == ItemSpawnerCount)
            {
                break;
            }
        }



    }
}
