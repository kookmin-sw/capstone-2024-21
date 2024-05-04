using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLightning : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        gameObject.GetComponentsInChildren<ParticleSystem>()[0].Play();
        gameObject.GetComponentsInChildren<ParticleSystem>()[1].Play();
    }

    private void OnDisable()
    {
        gameObject.GetComponentsInChildren<ParticleSystem>()[0].Stop();
        gameObject.GetComponentsInChildren<ParticleSystem>()[1].Stop();
    }

}
