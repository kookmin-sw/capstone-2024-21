using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI tagText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickedStartButton()
    {

        SceneManager.LoadScene("InGame");
    }
}
