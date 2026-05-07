using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    Player player;
    public bool isTalkingTo = false;
    [SerializeField] string[] dialogue;
    [SerializeField] TMP_Text textbox;
    [SerializeField] GameObject textbg;
    public int index = 0;
    public bool doneTalkingTo = false;
    public GameObject buttonImage;
    
    //[SerializeField] bool criteriaToChangeText;
    //public bool doChange;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void Talk()
    {
        //player.health.gameObject.SetActive(false);
        player.rb.velocity = Vector3.zero;
        doneTalkingTo = false;
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
        isTalkingTo = true;
        GetComponentInChildren<TMP_Text>().text = "";
        textbg.gameObject.SetActive(true);
        player.cameraFollow.enabled = false;
        player.cameraFollow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5.6f, transform.position.z - 10);
        player.cameraFollow.gameObject.GetComponent<Camera>().orthographicSize = 3.5f;
        index = 0;
        if (!player.hasInteracted) TryContinue();
        player.hasInteracted = true;
        //StartCoroutine(EndTalk());
    }
    bool typing = false;
    bool skipping = false;
    IEnumerator Talk(string sentence)
    {
        typing = true;
        string words = "";
        foreach (char c in sentence)
        {
            if (skipping) break;
            yield return new WaitForSeconds(0.05f);
            words += c;
            textbox.text = words;
        }
        skipping = false;
        typing = false;
        textbox.text = sentence;
    }
    public void TryContinue()
    {
        if (isTalkingTo && !doneTalkingTo)
        {
            if (index > dialogue.Length - 1)
            {
                index = 0;
                EndText();
            }
            else if (!doneTalkingTo && !typing)
            {
                StartCoroutine(Talk(dialogue[index]));
                index++;
            } else if (typing) skipping = true;
        }
    }
    void EndText()
    {
        doneTalkingTo = true;
        player.GetInput.enabled = true;
        player.GetInteractInput.enabled = false;
        isTalkingTo = false;
        player.cameraFollow.enabled = true;
        //player.cameraFollow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5.6f, transform.position.z - 10);
        player.cameraFollow.gameObject.GetComponent<Camera>().orthographicSize = 5f;
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>(true);
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
        player.rb.velocity = Vector3.zero;
        textbg.gameObject.SetActive(false);
        textbox.text = "";
        player.health.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) doneTalkingTo = false;
    }
}
