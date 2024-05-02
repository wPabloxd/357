using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open;
    bool opened;
    [SerializeField] Animator anim;
    [SerializeField] GameObject lvlManager;
    [SerializeField] int doorNumber;
    [SerializeField] AudioSource doorSound;
    void Update()
    {
        if(open && !opened)
        {
            anim.SetTrigger("open");
            doorSound.Play();
            opened = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {   
        if (other.gameObject.tag == "Player")
        {
            switch (doorNumber)
            {
                case (0):
                    open =  lvlManager.GetComponent<Level1>().OpenFirstDoor();
                    break;
                case (1):
                    open = lvlManager.GetComponent<Level1>().OpenSecondDoor();
                    break;
                case (2):
                    open = lvlManager.GetComponent<Level1>().OpenThirdDoor();
                    break;
                case (3):
                    open = true;
                    break;
                case (4):
                    open = lvlManager.GetComponent<Level1>().OpenInitialDoor();
                    break;
                case (5):
                    open = lvlManager.GetComponent<Level1>().OpenSecondRoomDoor();
                    break;
            }
        }
    }
}
