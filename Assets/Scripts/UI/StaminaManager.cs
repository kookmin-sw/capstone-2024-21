using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class StaminaManager : MonoBehaviour, IPlayerStateListener
{
    [Header("스테미너 증가/감소 량")]
    public float jumpAmount = 20f;
    [SerializeField] private float runAmount = -15f;
    [SerializeField] private float recoverAmount = 15f;

    private float maxStamina = 100f;
    private float _stamina;
    public float stamina
    {
        get { return _stamina; }
        set
        {
            _stamina = value;
            _stamina = Mathf.Clamp(value, 0f, maxStamina);
            OnStaminaChanged?.Invoke(_stamina, maxStamina); //for UI
        }
    }

    public delegate void OnStaminaChangedEvent(float stamina, float maxStamina);
    public event OnStaminaChangedEvent OnStaminaChanged;

    private Coroutine ChangeRoutine;

    private MovementStateManager movement;
    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<MovementStateManager>();
    }

    void Start()
    {
        if (pv.IsMine) //ui는 로컬에만 반영되면 됨
        {
            var SListeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IStaminaListener>();
            foreach (var listener in SListeners) OnStaminaChanged += listener.OnStaminaChanged;
        }
        stamina = maxStamina;
    }

    public void OnPlayerStateChanged(MovementBaseState curState)
    {
        if (ChangeRoutine != null) StopCoroutine(ChangeRoutine);

        if (curState == movement.Run)
        {
            ChangeRoutine = StartCoroutine(ChangeStamina(runAmount));
        }
        else if (curState == movement.Jump)
        {
            stamina -= jumpAmount;
            ChangeRoutine = StartCoroutine(ChangeStamina(recoverAmount));
        }
        else
        {
            ChangeRoutine = StartCoroutine(ChangeStamina(recoverAmount));
        }
    }

    IEnumerator ChangeStamina(float amount)
    {
        while(stamina != maxStamina)
        {
            stamina += amount * Time.deltaTime;
            yield return null;
        }
    }
}
