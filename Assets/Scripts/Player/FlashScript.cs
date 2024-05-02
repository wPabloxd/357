using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScript : MonoBehaviour
{
    void Update()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Shutdown());
        }
    }
    IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
