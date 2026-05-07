using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestDemo : MonoBehaviour {

    //This script goes on the ChestComplete prefab;

    public Animator chestAnim;
    public bool canOpen = true;
    public GameObject itemDrop;
    Player player;
    public bool isCollecting = false;
    [SerializeField] string[] dialogue;
    [SerializeField] TMP_Text textbox;
    [SerializeField] GameObject textbg;
    public int index = 0;
    public bool doneCollecting = false;
    GameObject spawnedObject;
    public GameObject buttonImage;

    // Use this for initialization
    void Awake ()
    {
        chestAnim = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
	}
    public void Open()
    {
        player.rb.velocity = Vector3.zero;
        index = 0;
        chestAnim.SetTrigger("open");
        canOpen = false;
        isCollecting = true;
        GetComponentInChildren<TMP_Text>().text = "";
        textbg.gameObject.SetActive(true);
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>();
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
    }
    public void GiveItem()
    {
        GetComponentInChildren<TMP_Text>().text = "";
        player.cameraFollow.enabled = false;
        player.cameraFollow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5.6f, transform.position.z - 10);
        player.cameraFollow.gameObject.GetComponent<Camera>().orthographicSize = 3.5f;
        if (itemDrop) spawnedObject = Instantiate(itemDrop, new Vector3(transform.position.x - 0.75f, transform.position.y + 1f, transform.position.z - 0.5f), Quaternion.identity);
        player.Recieve();
        if (!player.hasInteracted) TryContinue();
        player.hasInteracted = true;
    }
    bool typing = false;
    bool skipping = false;
    public void TryContinue()
    {
        if (isCollecting && !doneCollecting)
        {
            if (index > dialogue.Length - 1)
            {
                index = 0;
                EndText();
                player.FinishRecieve();
            }
            else if (!doneCollecting && !typing)
            {
                index++;
                StartCoroutine(GiveInfo(dialogue[index-1]));
            } else if (typing) skipping = true;
        }
    }
    void EndText()
    {
        doneCollecting = true;
        player.GetInput.enabled = true;
        player.GetInteractInput.enabled = false;
        isCollecting = false;
        player.cameraFollow.enabled = true;
        //player.cameraFollow.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5.6f, transform.position.z - 10);
        player.cameraFollow.gameObject.GetComponent<Camera>().orthographicSize = 5f;
        MeleeEnemy[] enemies = FindObjectsOfType<MeleeEnemy>(true);
        textbg.gameObject.SetActive(false);
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
        player.rb.velocity = Vector3.zero;
        textbox.text = "";
        
        Destroy(spawnedObject);
    }
    IEnumerator GiveInfo(string sentence)
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) doneCollecting = false;
    }
}
