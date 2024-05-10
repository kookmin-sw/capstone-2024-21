using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ClickManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color preColor;
    [SerializeField] private Color hoverColor;

    void Awake()
    {
        preColor = gameObject.GetComponent<Image>().color;
        hoverColor = gameObject.GetComponent<Image>().color;
    }
    // 버튼 호버
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverColor.a = 0.7f;
        gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = preColor;
    }

    // 버튼 클릭
    public void OnPointerDown(PointerEventData eventData)
    {
        hoverColor = gameObject.GetComponent<Image>().color;
        hoverColor.a = 0.3f;
        gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("클릭 끝");
        if (eventData.pointerEnter == GameObject.Find("SettingIcon"))
        {
            gameObject.GetComponent<Image>().color = preColor;
        }
        else if(eventData.pointerEnter == GameObject.Find("ExitIcon"))
        {
            gameObject.GetComponent<Image>().color = preColor;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        }

    }
}