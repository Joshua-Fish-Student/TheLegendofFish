using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public InputSubscription GetInput;
    public InputSubscriptionInteract GetInteractInput;
    public InputSubscriptionUI GetUIInput;
    public InputSubscriptionCamera GetCameraInput;
    public float moveSpeed = 1f;
    private Vector3 moveDirection;
    public Rigidbody rb;
    public float jumpPower = 10f;
    public bool onGround = true;
    public LayerMask ground;
    public LayerMask enemyMask;
    public GameObject interactTarget;
    public GameObject enemyTarget;
    [SerializeField] bool contact = false;
    bool inWater = false;
    bool isPaused = false;
    public FollowPlayer cameraFollow;
    [SerializeField] int damageAmount;
    bool onCoolDown = false;
    [SerializeField] float delay;
    public bool hasInteracted = false;
    public HeartHandler health;
    bool isDead = false;
    Animator animator;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject pauseUI;
    public bool seen;
    MusicHandler musicHandler;
    public GameObject swordUI;
    [SerializeField] TMP_Text infoUI;
    public int moneys;
    PlayerInput playerInput;
    string currentSchemeName;
    bool blocking = true;
    [SerializeField] GameObject[] popUpStuff;
    bool popUp = false;
    [SerializeField] GameObject deadUI;
    private void Awake()
    {
        print(new Vector3(PlayerPrefs.GetFloat("locationX", 0), PlayerPrefs.GetFloat("locationY", 2.5f), PlayerPrefs.GetFloat("locationZ", 0)));
        if (SceneManager.GetActiveScene().name == PlayerPrefs.GetString("LastSceneIn", "SampleScene") && PlayerPrefs.GetInt("FromLoadingZone", 0) == 0) transform.position = new Vector3(PlayerPrefs.GetFloat("locationX", 0), PlayerPrefs.GetFloat("locationY", 2.5f), PlayerPrefs.GetFloat("locationZ", 0));
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody>();
        cameraFollow = FindFirstObjectByType<FollowPlayer>();
        animator = GetComponentInChildren<Animator>();
        musicHandler = FindObjectOfType<MusicHandler>();
        playerInput = GetComponent<PlayerInput>();
        currentSchemeName = playerInput.currentControlScheme;
        //deadUI = GameObject.Find("Game Over");
        //Pause(true);
        //Pause(false);
        StartCoroutine(AttackCoolDown());
    }
    IEnumerator AttackCoolDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (onCoolDown)
            {
                yield return new WaitForSeconds(delay);
                onCoolDown = false;
            }
        }
    }
    //late update to not break pausing
    void LateUpdate()
    {
        if ((GetInput.PauseInput && GetInput.isActiveAndEnabled) || (GetInteractInput.PauseInput && GetInteractInput.isActiveAndEnabled) || (GetUIInput.PauseInput && GetUIInput.isActiveAndEnabled))
            Pause(!isPaused);
    }
    // Update is called once per frame
    void Update()
    {
        currentSchemeName = playerInput.currentControlScheme;
        if (seen) musicHandler.MusicSwitch("Combat");
        else musicHandler.MusicSwitch("Ambient");
        float forward = GetInput.MoveInput.x;
        float side = GetInput.MoveInput.y;
        if (GetInput.JumpInput && onGround)
        {
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        else if (!GetCameraInput.ShiftInput) moveDirection = new Vector3(forward * moveSpeed, rb.velocity.y, side * moveSpeed);
        rb.velocity = moveDirection;
        float angle = Mathf.Atan2(side, -forward) * Mathf.Rad2Deg - 90;
        angle = (angle + 360f) % 360;
        if ((forward != 0 || side != 0) && !GetCameraInput.ShiftInput)
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
            if (onGround) animator.SetBool("Moving", true);
        }
        else animator.SetBool("Moving", false);
        onGround = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 0.5f, ground) && contact;
        blocking = false;
        if (GetInput.isActiveAndEnabled && GetInput.InteractInput && interactTarget)
        {
            musicHandler.MusicSwitch("Ambient");
            TryInteract();
        }
        else if (GetInput.isActiveAndEnabled && GetInput.BlockInput) Block();
        else if (GetInput.isActiveAndEnabled && GetInput.AttackInput && !GetInput.BlockInput) TryDamage();
        //else if ((GetInput.PauseInput && GetInput.isActiveAndEnabled) || (GetInteractInput.PauseInput && GetInteractInput.isActiveAndEnabled) || (GetUIInput.PauseInput && GetUIInput.isActiveAndEnabled)) Pause(!isPaused);
        else if (interactTarget && GetInteractInput.isActiveAndEnabled && GetInteractInput.SubmitInput) TryContinue();
        else ResetAnimations();
    }
    void TryContinue()
    {
        if (interactTarget.GetComponent<ChestDemo>()) interactTarget.GetComponent<ChestDemo>().TryContinue();
        else if (interactTarget.GetComponent<NPC>()) interactTarget.GetComponent<NPC>().TryContinue();
    }
    void TryInteract()
    {
        Vector3 position = interactTarget.transform.position;
        position.y = transform.position.y;
        transform.LookAt(position);
        if (interactTarget.GetComponent<ChestDemo>() && interactTarget.GetComponent<ChestDemo>().canOpen)
        {
            //transform.LookAt(interactTarget.transform.position);
            GetInput.enabled = false;
            GetInteractInput.enabled = true;
            interactTarget.GetComponentInChildren<SpriteRenderer>(true).gameObject.SetActive(false);
            interactTarget.GetComponent<ChestDemo>().Open();
        }
        else if (interactTarget.GetComponent<NPC>() && !interactTarget.GetComponent<NPC>().isTalkingTo && !interactTarget.GetComponent<NPC>().doneTalkingTo)
        {
            //transform.LookAt(interactTarget.transform.position);
            GetInput.enabled = false;
            GetInteractInput.enabled = true;
            interactTarget.GetComponentInChildren<SpriteRenderer>(true).gameObject.SetActive(false);
            interactTarget.GetComponent<NPC>().Talk();
        }
    }
    void TryDamage()
    {
        animator.SetBool("Blocking", false);
        animator.SetBool("Attacking", true);
        if (enemyTarget && enemyTarget.GetComponent<Health>() && !onCoolDown)
        {
            enemyTarget.GetComponent<Health>().health -= damageAmount;
            onCoolDown = true;
            if (enemyTarget.GetComponent<Health>().health <= 0) enemyTarget.GetComponent<Health>().Destroy();
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground")) contact = true;
    }
    private void OnTriggerStay(Collider other)
    {
        string text;
        if (other.gameObject.CompareTag("Interact Object"))
        {
            interactTarget = other.gameObject;
            if (interactTarget.GetComponent<NPC>()) interactTarget.transform.LookAt(new Vector3(transform.position.x, 1, transform.position.z));
            if (interactTarget.GetComponent<ChestDemo>() && interactTarget.GetComponent<ChestDemo>().canOpen)
            {
                switch (currentSchemeName)
                {
                    case "Gamepad":
                    case "Joystick":
                        text = "Open ";
                        interactTarget.GetComponent<ChestDemo>().buttonImage.SetActive(true);
                        break;
                    case "Keyboard&Mouse":
                        text = "Open (E)";
                        interactTarget.GetComponent<ChestDemo>().buttonImage.SetActive(false);
                        break;
                    default:
                        text = "What are you doing? I told you to 'Please use a different controller'!";
                        interactTarget.GetComponent<ChestDemo>().buttonImage.SetActive(false);
                        break;
                }
                interactTarget.GetComponentInChildren<TMP_Text>().text = text;
            }
            else if (interactTarget.GetComponent<NPC>() && !interactTarget.GetComponent<NPC>().isTalkingTo)
            {
                switch (currentSchemeName)
                {
                    case "Gamepad":
                    case "Joystick":
                        text = "Talk ";
                        interactTarget.GetComponent<NPC>().buttonImage.SetActive(true);
                        break;
                    case "Keyboard&Mouse":
                        text = "Talk (E)";
                        interactTarget.GetComponent<NPC>().buttonImage.SetActive(false);
                        break;
                    default:
                        text = "What are you doing? I told you to 'Please use a different controller'! Even the NPC doesn't recognize that one!";
                        interactTarget.GetComponent<NPC>().buttonImage.SetActive(false);
                        break;
                }
                interactTarget.GetComponentInChildren<TMP_Text>().text = text;
            }
        }
        else if (other.gameObject.CompareTag("Enemy") && Physics.Raycast(transform.position, transform.forward, 2f, enemyMask)) enemyTarget = other.gameObject;
        else if (other.gameObject.CompareTag("Water") && !inWater)
        {
            moveSpeed /= 2;
            inWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == interactTarget)
        {
            interactTarget.GetComponentInChildren<TMP_Text>().text = "";
            if (interactTarget.GetComponentInChildren<SpriteRenderer>()) interactTarget.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
            interactTarget = null;
            ResetAllIndices();
        }
        else if (other.gameObject == enemyTarget) enemyTarget = null;
        if (inWater && other.gameObject.CompareTag("Water"))
        {
            moveSpeed *= 2;
            inWater = false;
        }
    }
    public void Pause(bool setTo, bool isdead = false)
    {
        isDead = isdead;
        
        PauseQuietToggle(setTo);
        if (setTo && !isDead)
        {
            infoUI.text = $"Speed: {moveSpeed}<br>Rubies: {moneys}";
            if (!isPaused)EventSystem.current.SetSelectedGameObject(swordUI);
            Time.timeScale = 0f;
            isPaused = true;
            cameraFollow.enabled = false;
            pauseUI.SetActive(true);
            GetInteractInput.enabled = false;
            GetInput.enabled = false;
            GetUIInput.enabled = true;
        }
        else if (!isDead && !popUp)
        {
            Time.timeScale = 1f;
            isPaused = false;
            cameraFollow.enabled = true;
            pauseUI.SetActive(false);
            if (hasInteracted)GetInteractInput.enabled = true;
            else GetInput.enabled = true;
            GetUIInput.enabled = false;
        } else if (popUp)
        {
            foreach(GameObject gameObject in popUpStuff)
            {
                gameObject.SetActive(false);
                GetInput.gameObject.GetComponent<Gamemanager>().UIReturn(swordUI);
            }
            popUp = false;
        }
        else
        {
            StartCoroutine(Die());
        }
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);
        DisplayGameOverScreen();
    }
    void DisplayGameOverScreen()
    {
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
        popUpStuff[0].gameObject.SetActive(true);
        deadUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(deadUI.transform.GetChild(0).gameObject);
    }
    void ResetAllIndices()
    {
        NPC[] nPCs = FindObjectsOfType<NPC>();
        foreach (var nPc in nPCs)
        {
            nPc.index = 0;
        }
        ChestDemo[] chests = FindObjectsOfType<ChestDemo>();
        foreach (var chest in chests)
        {
            chest.index = 0;
        }
    }
    void ResetAnimations()
    {
        if (animator.GetBool("Attacking")) animator.SetBool("Attacking", false);
        if (animator.GetBool("Blocking")) animator.SetBool("Blocking", false);
    }
    public void UpdateHealth(int damage, GameObject self)
    {
        RaycastHit hit;
        if (blocking && Physics.Raycast(transform.position, transform.forward, out hit, 2f, enemyMask) && hit.transform.gameObject == self)
        {
            animator.SetTrigger("HitWithBlock");
            return;
        }
        animator.SetTrigger("Hit");
        GetComponent<Health>().health -= damage;
        if (!(GetComponent<Health>().health > 0))
        {
            animator.SetTrigger("Dead");
            enabled = false;
            Pause(true, true);
        }
        health.UpdateHearts(GetComponent<Health>().health);
    }
    public void UpdateHealth(int healAmount)
    {
        if (GetComponent<Health>().health + healAmount <= health.maxHealth * 10) GetComponent<Health>().health += healAmount;
        else GetComponent<Health>().health = health.maxHealth * 10;
        health.UpdateHearts(GetComponent<Health>().health);
    }
    public void Recieve()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        animator.SetTrigger("Collect");
    }
    public void FinishRecieve()
    {
        sword.SetActive(true);
        shield.SetActive(true);
    }
    public void PauseQuietToggle(bool isPaused)
    {
        if (isPaused)musicHandler.VolumeSwitch(musicHandler.GetActive().volume / 2f);
        else musicHandler.VolumeSwitch(musicHandler.GetActive().volume * 2f);
    }
    void Block()
    {
        animator.SetBool("Blocking", true);
        blocking = true;
    }
    public void TogglePopUp(bool setTo)
    {
        popUp = setTo;
    }
}
