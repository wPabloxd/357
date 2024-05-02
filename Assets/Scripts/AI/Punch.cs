using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    bool punchDamageCD;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !punchDamageCD)
        {
            audioSource.Play();
            other.gameObject.GetComponent<Player>().HealthSystem(20);
            StartCoroutine(AlreadyDamaged());
        }
    }
    IEnumerator AlreadyDamaged()
    {
        punchDamageCD = true;
        yield return new WaitForSeconds(1f);
        punchDamageCD = false;
    }
}
