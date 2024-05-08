using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI 아래 GaugeController에 들어있음
public class InteractGaugeControler : MonoBehaviour
{
    private Image InteractGuageImage;

    private float GaugeTimer;
    [SerializeField] private float interactGaugeFillSpeed = 4.0f;
    [SerializeField] private float ExitGaugeFillSpeed = 1.6f;

    private void Awake()
    {
        InteractGuageImage = GetComponentInChildren<Image>();
        InteractGuageImage.gameObject.SetActive(false);
        
    }




    public void SetGuageZero()
    {
        GaugeTimer = 0;
    }

    public void AbleInvestinGaugeUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableInvestinGaugeUI()
    {
        gameObject.SetActive(false);
    }

    public bool FillCircle()
    {
        //Debug.Log("수색중");

        InteractGuageImage.fillAmount = GaugeTimer;
        InteractGuageImage.gameObject.SetActive(true);

        GaugeTimer += interactGaugeFillSpeed / 10.0f * Time.deltaTime;

        if (GaugeTimer >= 1)
        {
            GaugeTimer = 0;
            InteractGuageImage.gameObject.SetActive(false);
            return true; //수색을 성공적으로 마침
        }

        return false; // 수색에 실패함
    }

    
    public bool ExitFillCircle()
    {
        Debug.Log("ExitFillCircle 실행");

        InteractGuageImage.fillAmount = GaugeTimer;
        InteractGuageImage.gameObject.SetActive(true);

        GaugeTimer += ExitGaugeFillSpeed / 10.0f * Time.deltaTime;

        if (GaugeTimer >= 1)
        {
            GaugeTimer = 0;
            InteractGuageImage.gameObject.SetActive(false);
            return true; //수색을 성공적으로 마침
        }

        return false; // 수색에 실패함
    }
}
