using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCounter : MonoBehaviour,IPlayerCountListener
{
    [SerializeField] private TextMeshProUGUI curPlayerText;
    [SerializeField] private TextMeshProUGUI totalPlayerText;

    public void OnPlayerCountChanged(int curPlayer, int totalPlayer)
    {
        curPlayerText.SetText($"{curPlayer}");
        totalPlayerText.SetText($"{totalPlayer}");
    }
}
