using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//image_F UI에 들어가 있음
public class UIpressF : MonoBehaviour
{
    public void show_image()
    {
        gameObject.SetActive(true);
    }

    public void remove_image()
    {
        gameObject.SetActive(false);
    }
}
