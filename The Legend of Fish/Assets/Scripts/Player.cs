using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager.Requests;

public class Player : MonoBehaviour
{
    public InputSubscription GetInput;
    public InputSubscriptionInteract GetInteractInput;
    public float moveSpeed = 1f;
    private Vector3 moveDirection;
    public Rigidbody rb;
    public float jumpPower = 10f;
    public bool onGround = true;
    public LayerMask ground;
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
    public TMP_Text health;
    bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraFollow = FindFirstObjectByType<FollowPlayer>();
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
    // Update is called once per frame
    void Update()
    {
        float forward = GetInput.MoveInput.x;
        float side = GetInput.MoveInput.y;
        if (GetInput.JumpInput && onGround) rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        else moveDirection = new Vector3(forward * moveSpeed, rb.velocity.y, side * moveSpeed);
        rb.velocity = moveDirection;

        onGround = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 0.5f, ground) && contact;
        if (GetInput.isActiveAndEnabled && GetInput.InteractInput && interactTarget) TryInteract();
        else if (GetInput.isActiveAndEnabled && GetInput.AttackInput && enemyTarget) TryDamage();
        else if (GetInput.PauseInput || GetInteractInput.PauseInput) Pause();
        else if (GetInteractInput.isActiveAndEnabled && GetInteractInput.SubmitInput) TryContinue(); 
    }
    void TryContinue()
    {
        if (interactTarget.GetComponent<ChestDemo>()) interactTarget.GetComponent<ChestDemo>().TryContinue();
        else if (interactTarget.GetComponent<NPC>()) interactTarget.GetComponent<NPC>().TryContinue();
            
    }
    void TryInteract()
    {
        if (interactTarget.GetComponent<ChestDemo>() && interactTarget.GetComponent<ChestDemo>().canOpen)interactTarget.GetComponent<ChestDemo>().Open();
        else if (interactTarget.GetComponent<NPC>() && !interactTarget.GetComponent<NPC>().isTalkingTo && !interactTarget.GetComponent<NPC>().doneTalkingTo) interactTarget.GetComponent<NPC>().Talk();
    }
    void TryDamage()
    {
        if (enemyTarget.GetComponent<Health>() && !onCoolDown)
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
        if (other.gameObject.CompareTag("Interact Object"))
        {
            interactTarget = other.gameObject;
            if (interactTarget.GetComponent<NPC>()) interactTarget.transform.LookAt(new Vector3(transform.position.x, 1, transform.position.z));
            if (interactTarget.GetComponent<ChestDemo>() && interactTarget.GetComponent<ChestDemo>().canOpen) interactTarget.GetComponentInChildren<TMP_Text>().text = "Open (B)";
            else if (interactTarget.GetComponent<NPC>() && !interactTarget.GetComponent<NPC>().isTalkingTo) interactTarget.GetComponentInChildren<TMP_Text>().text = "Talk (B)";
        }
        else if (other.gameObject.CompareTag("Enemy")) enemyTarget = other.gameObject;
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
    void Pause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            cameraFollow.enabled = false;
        }
        else if (!isDead)
        {
            Time.timeScale = 1f;
            isPaused = false;
            cameraFollow.enabled = true;
        }
        
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
    public void UpdateHealth()
    {
        if (!(GetComponent<Health>().health > 0))
        {
            isDead = true;
            Pause();
        }
        else health.text = $"Health: {GetComponent<Health>().health}";
    }
}
