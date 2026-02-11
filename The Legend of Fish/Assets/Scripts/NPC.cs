using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    Player player;
    public bool isTalkingTo = false;
    [SerializeField] string[] dialogue;
    [SerializeField] TMP_Text text;
    public int index = 0;
    public bool doneTalkingTo = false;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void Talk()
    {
        player.health.gameObject.SetActive(false);
        player.rb.velocity = Vector3.zero;
        doneTalkingTo = false;
        player.GetInput.enabled = false;
        player.GetInteractInput.enabled = true;
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
        isTalkingTo = true;
        GetComponentInChildren<TMP_Text>().text = "";
        player.cameraFollow.enabled = false;
        player.cameraFollow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5.6f, transform.position.z - 10);
        player.cameraFollow.gameObject.GetComponent<Camera>().orthographicSize = 3.5f;
        index = 0;
        if (!player.hasInteracted) TryContinue();
        player.hasInteracted = true;
        //StartCoroutine(EndTalk());
    }
    IEnumerator Talk(string sentence)
    {
        string words = "";
        foreach (char c in sentence)
        {
            yield return new WaitForSeconds(0.05f);
            words += c;
            text.text = words;
        }
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
            else if (!doneTalkingTo)
            {
                StartCoroutine(Talk(dialogue[index]));
                index++;
            }
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
        text.text = "";
        player.health.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) doneTalkingTo = false;
    }
}
