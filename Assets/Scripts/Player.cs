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

    [Header("Visual")]
    public Image sprintBar;
    public GameObject interact;
    public int lastID;
    public GameObject lastObject;

    void Start()
    {
        interact.SetActive(false);
        lastID = -1;
        lastObject = null;
        speed = moveSpeed;
        o_stamina = stamina;
        settings = Settings.instance;
    }

    void Update()
    {
        //Movement
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y).normalized;
        transform.Translate(move * speed * Time.deltaTime);

        //Sprinting
        if((Input.GetKey(settings.sprint) && stamina > 0 && canSprint && (x != 0 || y != 0)) && moveState == PlayerMovementState.Walking)
        {
            moveState = PlayerMovementState.Sprinting;
        }
        else if(Input.GetKeyUp(settings.sprint) || stamina <= 0 || !canSprint || (x == 0 && y == 0))
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

        sprintBar.fillAmount = stamina / o_stamina;
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
