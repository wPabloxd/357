using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRed : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    [SerializeField] Rigidbody[] rb;
    [SerializeField] CharacterController characterController;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AudioSource splash;
    [SerializeField] AudioClip[] blood;
    [SerializeField] SphereCollider fistColl;
    GameObject player;
    [SerializeField] GameObject ammoBox;
    [SerializeField] GameObject healthBox;
    GameObject revolver;
    bool chasing;
    public bool alive;
    bool punchCD;

    private void OnEnable()
    {
        revolver = GameObject.Find("self");
        player = GameObject.Find("Player");
        fistColl.enabled = false;
        anim = GetComponentInChildren<Animator>();
        for (int i = 0; i < 11; i++)
        {
            rb[i].isKinematic = true;
        }
    }
    void Start()
    {
        alive = true;
    }
    void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Right Hook"))
        {
            fistColl.enabled = true;
            agent.speed = 0;
        }
        else
        {
            fistColl.enabled = false;
            if(Vector3.Distance(player.transform.position, transform.position) > 1.5f)
            {
                agent.speed = 4f;
            }
        }
        if (alive)
        {
            if (GameManager.Instance.loudShot)
            {
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            }
            if (chasing)
            {
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                agent.SetDestination(player.transform.position);
                if (Vector3.Distance(player.transform.position, transform.position) < 1.5f && !punchCD)
                {
                    agent.speed = 0;
                    if (!punchCD)
                    {
                        StartCoroutine(PunchCD());
                        anim.SetTrigger("punch");
                    }
                }
                else
                {
                    anim.ResetTrigger("punch");
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            agent.isStopped = false;
            anim.SetBool("running", true);
            chasing = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            anim.SetBool("running", false);
            chasing = false;
        }
    }
    IEnumerator PunchCD()
    {
        punchCD = true;
        yield return new WaitForSeconds(1);
        punchCD = false;
    }
    public void Death()
    {
        if (alive)
        {
            splash.clip = blood[Random.Range(0, blood.Length)];
            splash.Play();
            fistColl.enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            gameObject.layer = 6;
            alive = false;
            characterController.enabled = false;
            for (int i = 0; i < 11; i++)
            {
                rb[i].isKinematic = false;
            }
            Drop();
            alive = false;
            StartCoroutine(CollidersDespawn());
        }
    }
    IEnumerator CollidersDespawn()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 11; i++)
        {
            rb[i].gameObject.SetActive(false);
        }
    }
    void Drop()
    {
        GameObject drop;
        if (revolver.GetComponent<Revolver>().ammo <= 2)
        {
            drop = Instantiate(ammoBox, transform.position, transform.rotation);
        }
        else if (player.GetComponent<Player>().health <= 40)
        {
            drop = Instantiate(healthBox, transform.position, transform.rotation);
        }
        else if (revolver.GetComponent<Revolver>().ammo <= 4)
        {
            drop = Instantiate(ammoBox, transform.position, transform.rotation);
        }
        else
        {
            int seed = Random.Range(0, 2);
            if (seed == 0)
            {
                drop = Instantiate(ammoBox, transform.position, transform.rotation);
            }
            else
            {
                drop = Instantiate(healthBox, transform.position, transform.rotation);
            }
        }
        drop.transform.parent = null;
    }
}
