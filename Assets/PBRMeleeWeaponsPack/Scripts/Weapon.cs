using Photon.Realtime;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Sprite icon;
    private Rigidbody itemRigidBody;
    public bool settedLightning = false;

    // Use this for initialization
    void Start()
    {
        itemRigidBody = gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        }
    }
}
