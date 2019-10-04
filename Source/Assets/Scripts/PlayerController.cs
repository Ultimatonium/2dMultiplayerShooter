using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    private int health;
    [SyncVar(hook = "OnChangeDeathCount")]
    private int deathCount;

    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private GameObject bulletPrefab;

    private TextMeshProUGUI deathCountText;
    private TextMesh healthBar;

    private void Awake()
    {
        deathCount = 0;
    }

    private void Start()
    {
        healthBar = GetComponentInChildren<TextMesh>();
        OnChangeHealth(health); //fix snyc 
        if (hasAuthority)
        {
            deathCountText = FindObjectOfType<TextMeshProUGUI>();
            CmdSetHealth(maxHealth);
            SetCamera();
        }
        OnChangeDeathCount(deathCount);
    }

    private void Update()
    {
        if (!hasAuthority) return;
        Move();
        Shoot();
    }

    [Client]
    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.parent = this.transform;
    }

    [Client]
    private void Move()
    {
        Vector2 translateVector = new Vector2();
        if (Input.GetKey(KeyCode.DownArrow))
        {
            translateVector += Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            translateVector += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            translateVector += Vector2.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            translateVector += Vector2.up;
        }
        transform.Translate(translateVector.normalized * Time.deltaTime * movementSpeed, Space.World);
    }

    [Client]
    private void Shoot()
    {
        if (!hasAuthority) return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdFireBullet(transform.position, Vector2.down * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CmdFireBullet(transform.position, Vector2.left * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            CmdFireBullet(transform.position, Vector2.right * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CmdFireBullet(transform.position, Vector2.up * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CmdFireBullet(transform.position, (Vector2.up + Vector2.left).normalized * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdFireBullet(transform.position, (Vector2.up + Vector2.right).normalized * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            CmdFireBullet(transform.position, (Vector2.down + Vector2.left).normalized * Time.deltaTime * movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CmdFireBullet(transform.position, (Vector2.down + Vector2.right).normalized * Time.deltaTime * movementSpeed);
        }
    }

    [Command]
    private void CmdFireBullet(Vector2 startPosition, Vector2 directionWithSpeed)
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(newBullet);
        newBullet.GetComponent<BulletController>().SetFlyDirectionWithSpeed(directionWithSpeed);
    }

    [Command]
    private void CmdSetHealth(int health)
    {
        this.health = health;
    }

    [ServerCallback]
    public void ApplyDmg(int dmg)
    {
        health -= dmg;
        CheckDeath();
    }

    [ServerCallback]
    private void CheckDeath()
    {
        if (health <= 0)
        {
            deathCount += 1;
            Respawn();
        }
    }

    [ServerCallback]
    private void Respawn()
    {
        CmdSetHealth(maxHealth);
        NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();
        transform.position = startPositions[Random.Range(0, startPositions.Length - 1)].transform.position;
    }

    [Client]
    private void OnChangeHealth(int health)
    {
        if (healthBar == null) return;
        healthBar.text = health + "/" + maxHealth;
    }

    [Client]
    private void OnChangeDeathCount(int deathCount)
    {
        if (deathCountText == null) return;
        deathCountText.text = "Deaths: " + deathCount;
    }

    private void OnValidate()
    {
        if (movementSpeed == 0
         || maxHealth == 0
         || bulletPrefab == null
           )
        {
            Debug.Log("PlayerController not corectly set up");
        }
    }
}
