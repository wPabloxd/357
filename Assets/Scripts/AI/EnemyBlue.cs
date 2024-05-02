using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyBlue : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    [SerializeField] Rigidbody[] rb;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject healthBox;
    [SerializeField] GameObject ammoBox;
    [SerializeField] GameObject visor;
    CharacterController characterController;
    NavMeshAgent agent;
    GameObject player;
    GameObject revolver;
    [Header("AudioFX")]
    [SerializeField] AudioSource shot;
    [SerializeField] AudioSource splash;
    [SerializeField] AudioClip[] blood;
    [Header("Rifle")]
    [SerializeField] Transform rifle;
    [SerializeField] GameObject rifleLaser;
    [SerializeField] GameObject rifleTip;
    [SerializeField] GameObject rifleLaserPosition;
    [SerializeField] VisualEffect muzzleFlash;
    [SerializeField] GameObject flash;
    LineRenderer laser;
    bool lasering;
    bool locked;
    float timeToShoot;
    [Header("Other")]
    public bool alive;
    bool onPath;
    Vector3 velAgent;

    private void OnEnable()
    {
        revolver = GameObject.Find("self");
        player = GameObject.Find("Player");
        timeToShoot = 0;
        laser = GetComponent<LineRenderer>();
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        for (int i = 0; i < 12; i++)
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
        if (alive)
        {
            if (GameManager.Instance.loudShot)
            {
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            }
            if (locked && !onPath)
            {
                onPath = true;
                Vector3 pathVector = new Vector3(Random.Range(transform.position.x - 8, transform.position.x + 8), transform.position.y, Random.Range(transform.position.z - 8, transform.position.z + 8));
                for (int i = 0; i < 11; i++)
                {
                    pathVector = new Vector3(Random.Range(transform.position.x - 8, transform.position.x + 8), transform.position.y, Random.Range(transform.position.z - 8, transform.position.z + 8));
                    if(Vector3.Distance(pathVector, transform.position) > 5)
                    {
                        break;
                    }
                }
                StartCoroutine(CalculateAndSetPath(pathVector));
            }
            if (locked)
            {
                if(timeToShoot >= 3)
                {
                    timeToShoot = 0;
                    Shoot();
                }
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            }
            velAgent = transform.InverseTransformDirection(agent.velocity);
            if(velAgent.magnitude > 1f)
            {
                velAgent.Normalize();
            }
            anim.SetFloat("sidewardMovement", velAgent.x);
            anim.SetFloat("forwardMovement", velAgent.z);
        }
    }
    private void Shoot()
    {
        anim.SetTrigger("fire");
        shot.Play();
        player.GetComponent<Player>().HealthSystem(20);
        anim.ResetTrigger("fire");
        flash.SetActive(true);
        muzzleFlash.Play();
    }
    private void FixedUpdate()
    {
        if (locked && alive)
        {
            if (Physics.Raycast(rifleLaserPosition.transform.position, player.transform.position - rifleLaserPosition.transform.position, out RaycastHit hit, 50))
            {
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    timeToShoot += Time.deltaTime;
                    laser.enabled = true;
                    lasering = true;
                }
                else
                {
                    if (timeToShoot >= 2)
                    {
                        timeToShoot -= Time.deltaTime;
                    }
                    lasering = false;
                    laser.enabled = false;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (lasering)
        {
            laser.SetPosition(0, rifleLaser.transform.position);
            laser.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y + 0.4f, player.transform.position.z));
        }
    }
    IEnumerator CalculateAndSetPath(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (alive && NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5, NavMesh.AllAreas))
        {
            NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);
            float pathLength = 0;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            for (int i = 0; i < 11; i++)
            {
                pathLength = 0;
                NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);
                for (int j = 0; j < path.corners.Length - 1; j++)
                {
                    pathLength += Vector3.Distance(path.corners[j], path.corners[j + 1]);
                }
                if(pathLength < 13)
                {
                    break;
                }
            }
            agent.SetPath(path);
            while (agent.speed <= 0.9f)
            {
                agent.speed = Mathf.Lerp(agent.speed, 1, 3f * Time.deltaTime);
                if (agent.speed >= 0.95f)
                {
                    agent.speed = 1;
                    break;
                }
                yield return null;
            }
            while (alive && agent.pathPending)
            {
                yield return null;
            }
            while (alive && agent.remainingDistance > 3)
            {
                yield return null;
            }
            while (alive && agent.remainingDistance < 3)
            {
                agent.speed = Mathf.Lerp(agent.speed, 0, 8f * Time.deltaTime);
                if(agent.speed <= 0.05f)
                {
                    agent.speed = 0;
                    break;
                }
                yield return null;
            }
            if (alive)
            {
                agent.isStopped = true;
                onPath = false;
            }
        }
        yield return null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("targeting", true);
            agent.isStopped = false;
            locked = true;      
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("targeting", false);
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            locked = false;
        }
    }
    public void Death()
    {
        if (alive)
        {
            lasering = false;
            laser.enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            gameObject.GetComponent<LineRenderer>().enabled = false;
            gameObject.layer = 6;
            splash.clip = blood[Random.Range(0, blood.Length)];
            splash.Play();
            alive = false;
            characterController.enabled = false;
            for (int i = 0; i < 12; i++)
            {
                rb[i].isKinematic = false;
            }
            Drop();
            rifle.parent = null;
            alive = false;
            StartCoroutine(CollidersDespawn());
        } 
    }
    IEnumerator CollidersDespawn()
    {
        yield return new WaitForSeconds(1.5f);
        visor.transform.parent = null;
        for (int i = 0; i < 10; i++)
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
