using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI 아래 GaugeController에 들어있음
public class CraftGaugeController : MonoBehaviour
{
    [SerializeField] private Image craftGaugeImage;

    [SerializeField] private float craftGaugeFillSpeed = 4.0f;

    private void Awake()
    {
    }

    public void SetGaugeZero()
    {
        craftGaugeImage.fillAmount = 0;
    }

    public bool FillBolt()
    {
        craftGaugeImage.fillAmount += craftGaugeFillSpeed / 10.0f * Time.deltaTime; ;


        if (craftGaugeImage.fillAmount == 1)
        {
            craftGaugeImage.fillAmount = 0;
            return true; //크래프팅을 성공
        }

        return false; // 크래프팅에 실패
    }
}
