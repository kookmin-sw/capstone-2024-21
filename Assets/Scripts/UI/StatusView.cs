
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StatusView : MonoBehaviour, IHpListener, IStaminaListener
{
    [SerializeField] private Image HpBar;
    [SerializeField] private Image StaminarBar;

    [SerializeField] private TextMeshProUGUI HpText;
    [SerializeField] private TextMeshProUGUI playerName;

    void Awake()
    {
        playerName.SetText(GameManager.Instance.UserId);
    }

    public void OnHpChanged(float hp, float maxHp)
    {
        HpBar.fillAmount = hp / maxHp;
        HpText.SetText($"{(int)hp}");
        Debug.Log($"{hp}");
    }
    public void OnStaminaChanged(float stamina, float maxStamina)
    {
        StaminarBar.fillAmount = stamina / maxStamina;
        Debug.Log($"{stamina}");
    }
}
