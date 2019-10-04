using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour
{
    [SerializeField]
    private float dieTime;
    [SerializeField]
    private int dmg;

    private bool active;
    private Vector2 directionWithSpeed;

    private void Awake()
    {
        active = false;
    }

    [ServerCallback]
    private void Start()
    {
        Destroy(gameObject, dieTime);
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        active = true;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().ApplyDmg(dmg);
        }
        Destroy(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        transform.Translate(directionWithSpeed, Space.World);
    }

    [ServerCallback]
    public void SetFlyDirectionWithSpeed(Vector2 directionWithSpeed)
    {
        this.directionWithSpeed = directionWithSpeed;
    }

    private void OnValidate()
    {
        if (dieTime == 0)
        {
            Debug.LogWarning("dieTime is 0. Bullet dies instant");
        }
        if (dmg == 0)
        {
            Debug.LogWarning("dmg is 0. Bullet does no dmg");
        }
    }
}
