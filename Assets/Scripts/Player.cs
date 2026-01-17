using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerMovementState
{
    Walking,
    Sprinting
}

public class Player : MonoBehaviour
{
    public PlayerMovementState moveState;
    private Settings settings;
    private float speed;
    public float moveSpeed = 5f;

    [Header("Sprinting")]
    public float sprintSpeed = 10f;
    public float stamina = 10;
    private float o_stamina;
    public float drainMultiplyer = 2f;
    public float increaseMultiplyer = 2f;
    public bool canSprint;
    public float sprintCooldown = 4f;

    [Header("Dash")]
    public GameObject dashParticle;   
    public LayerMask wallLayer;
    public float dashDistance = 4f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    public float dashPrice = 10f;
    public bool canDash = true;

    [Header("Visual")]
    public GameObject playerSprite;
    public Image sprintBar;
    public GameObject interact;
    public int lastID;
    public GameObject lastObject;

    private float moveX;
    private float moveY;

    void Start()
    {
        interact.SetActive(false);
        lastID = -1;
        lastObject = null;
        canDash = true;
        speed = moveSpeed;
        o_stamina = stamina;
        settings = Settings.instance;
    }

    void Update()
    {
        //Movement
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized;
        transform.Translate(move * speed * Time.deltaTime);

        //Sprinting
        if((Input.GetKey(settings.sprint) && stamina > 0 && canSprint && (moveX != 0 || moveY != 0)) && moveState == PlayerMovementState.Walking)
        {
            moveState = PlayerMovementState.Sprinting;
        }
        else if(Input.GetKeyUp(settings.sprint) || stamina <= 0 || !canSprint || (moveX == 0 && moveY == 0))
        {
            moveState = PlayerMovementState.Walking;
        }

        if(moveState == PlayerMovementState.Sprinting)
        {
            speed = sprintSpeed;
            stamina = Mathf.Clamp(stamina - (drainMultiplyer * Time.deltaTime), 0, o_stamina);
        }
        else
        {
            if(stamina <= 0)
            {
                StartCoroutine(SprintCooldown());
            }
            speed = moveSpeed;
            stamina = Mathf.Clamp(stamina + (increaseMultiplyer * Time.deltaTime), 0, o_stamina);
        }
        sprintBar.fillAmount = stamina / o_stamina;

        //Detect Objects
        if(Input.GetKeyDown(settings.interact) && interact.activeInHierarchy)
        {
            interact.SetActive(false);
            switch(lastID)
            {
                case 0:
                    Inventory.instance.moneyCount++;
                    Destroy(lastObject);
                    break;
            }
            lastID = -1;
            lastObject = null;
        }

        //Dashing
        if(Input.GetKeyDown(settings.dash) && canDash && stamina >= dashPrice)
        {
            Dash();
        }

    }

    void Dash()
    {
        float x = moveX;
        float y = moveY;

        if(x == 0 && y == 0)
        {
            return;
        }

        stamina -= dashPrice;

        if(moveX != 0 && moveY != 0)
        {
            x *= 0.7f;
            y *= 0.7f;
        }

        Vector2 direction = new Vector2(x, y);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dashDistance, wallLayer);

        float totalDistance;

        if (hit.collider != null)
        {
            totalDistance = hit.distance;
        }
        else
        {
            totalDistance = dashDistance;
        }

        StartCoroutine(InitiateDash(transform.position + (Vector3)(direction * totalDistance)));
        GameObject go = Instantiate(dashParticle, transform.position, Quaternion.Euler(-90, 0, 0));
        if(go.TryGetComponent(out ParticleSystem particle))
        {
            particle.Play();
        }
        Destroy(go, 1f);

        Debug.DrawRay(transform.position, direction * dashDistance, Color.cyan, 0.2f);
    }

    IEnumerator InitiateDash(Vector3 finalPlacement)
    {
        float t = 0;
        while(t < dashDuration)
        {
            t += Time.deltaTime;
            float easeT = 1 - Mathf.Pow(1 - (t / dashDuration), 2);
            transform.position = Vector3.Lerp(transform.position, finalPlacement, easeT);
            if(Vector2.Distance(transform.position, finalPlacement) < 0.1f)
            {
                transform.position = finalPlacement;
                break;
            }
            yield return null;
        }

        yield return StartCoroutine(CooldownDash());
    }

    IEnumerator CooldownDash()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Interact"))
        {
            interact.SetActive(true);
            interact.transform.position = new Vector3(
                other.gameObject.transform.position.x, 
                other.gameObject.transform.position.y + 1f, 
                other.gameObject.transform.position.z);
            
            if(other.gameObject.TryGetComponent(out ItemID itemScript))
            {
                lastID = itemScript.id;
                lastObject = other.gameObject;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        interact.SetActive(false);
        lastID = -1;
        lastObject = null;
    }

    IEnumerator SprintCooldown()
    {
        canSprint = false;
        yield return new WaitForSeconds(sprintCooldown);
        canSprint = true;
    }
}
