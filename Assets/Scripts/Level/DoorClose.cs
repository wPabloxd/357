using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField] GameObject door;
    bool closed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !closed)
        {
            closed = true;
            door.GetComponent<Animator>().ResetTrigger("open");
            door.GetComponent<Animator>().SetTrigger("close");
            door.GetComponent<AudioSource>().Play();
        }
    }
}