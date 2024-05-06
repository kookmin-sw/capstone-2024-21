using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class StaminaManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public Slider staminaBar;
    public float jumpValue = 20f;
    private MovementStateManager movement;
    private PhotonView pv;
    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Slider>();
        //healthPointBar = GameObject.Find("HealthPointBar").GetComponent<Slider>();
        //healthPointCount = GameObject.Find("HealthPointCount").GetComponent<TextMeshProUGUI>();
        pv = GetComponent<PhotonView>();
        movement = GetComponent<MovementStateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            ManageStaminaBar();
            //ManageHealthPointBar();
        }
    }

    void ManageStaminaBar()
    {
        if(movement.isJumpStart == true)
        {
            staminaBar.value -= jumpValue;
            movement.isJumpStart = false;
        }
        if (movement.currentState == movement.Run)
        {
            staminaBar.value -= 15f * Time.deltaTime;
            if(staminaBar.value == 0)
            {
                movement.Run.ExitState(movement, movement.Walk);
                movement.currentState = movement.Walk;
            }
        }
        else
        { 
            staminaBar.value += 15f * Time.deltaTime;
        }
    }

    //void ManageHealthPointBar()
    //{
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        healthPointBar.value -= 5;
    //        healthPointCount.text = healthPointBar.value.ToString();
    //        if(healthPointBar.value == 0)
    //        {
    //            uiManager.isGameOver = true;
    //            uiManager.isUIActivate = true;
    //        }
    //    }
    //}
}
