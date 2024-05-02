using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask ground;
    [SerializeField] Transform cam;
    [SerializeField] Animator deathAnim;
    [SerializeField] GameObject revolver;
    [SerializeField] GameObject rToReload;
    [SerializeField] AudioSource healSFX;
    [SerializeField] AudioSource ammoSFX;
    CapsuleCollider capCollider;
    Rigidbody playerRB;
    InputManager inputManager;
    public bool paused;
    [Header("Movement Related")]
    Vector3 inputMovement;
    Vector3 moveDir;
    Vector3 slopeMoveDir;
    float speed = 3;
    float airSpeed = 0.3f;
    float slopeSin;
    float slopeCos;
    float targetAngle;
    
    bool slopeLimit;
    bool slope;
    public bool moving;
    bool jump;
    bool grounded;

    Vector3 slopeHit;
 
    ContactPoint LastContactPoint;

    [Header("Health")]
    public float health = 100;
    float healthConverter;
    [SerializeField] RectMask2D healthMask;


    void Start()
    {
        paused = false;
        HealthSystem(0);
        IngameMenu.Instance.Continue();
        Cursor.lockState = CursorLockMode.Locked;
        capCollider = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        inputManager = InputManager.Instance;
    }

    void Update()
    {
        if(health <= 0)
        {
            StartCoroutine(Death());
        }
        inputMovement = inputManager.GetPlayerMovement().normalized;
        if (inputManager.Pause() && !paused)
        {
            rToReload.SetActive(false);
            paused = true;
            IngameMenu.Instance.Pause();
        }
        else if(inputManager.Pause() && paused)
        {
            IngameMenu.Instance.EscPressed();
        }
        if (inputMovement.magnitude >= 0.1f)
        {
         
            moving = true;
            targetAngle = Mathf.Atan2(inputMovement.x, inputMovement.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;    
            slopeMoveDir = Vector3.ProjectOnPlane(moveDir.normalized, slopeHit);
        }
        else
        {
            moving = false;
            moveDir = Vector3.zero;
            slopeMoveDir = Vector3.zero;
        }
        if (inputManager.PlayerJump() && grounded && !slopeLimit)
        {
            Jump();
        }
    }
    IEnumerator Death()
    {
        deathAnim.SetTrigger("death");
        revolver.GetComponent<Revolver>().enabled = false;
        yield return new WaitForSeconds(0.76f);
        IngameMenu.Instance.DeathMenu();
        paused = true;
    }
    public void HealthSystem(float damage)
    {
        health -= damage;
        healthConverter = Remap(health, 0, 100, -15f, -121f);
        healthMask.padding = new Vector4(-300f, -88.2f, -288.37f, healthConverter);
    }
    float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source - sourceFrom) * (targetTo - targetFrom) / (sourceTo - sourceFrom);
    }
    private void IsGrounded()
    {
        if(Physics.CheckSphere(transform.position - new Vector3(0, 0.85f, 0), 0.4f, ground))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
    private void FixedUpdate()
    {
        IsGrounded();
        MovePlayer();
    }
    void MovePlayer()
    {
        if (grounded && !slope && !slopeLimit)
        {
            playerRB.drag = 13;
            playerRB.AddRelativeForce(moveDir.normalized * speed * 30, ForceMode.Acceleration);
        }
        else if (grounded && slope && !slopeLimit)
        {
            if (moving)
            {
                playerRB.drag = 13;
                playerRB.AddRelativeForce(Vector3.up * 9.81f, ForceMode.Acceleration);
                playerRB.AddRelativeForce(Vector3.Normalize(slopeHit) * -98.1f, ForceMode.Acceleration);
                playerRB.AddRelativeForce(slopeMoveDir.normalized * speed * 30 * slopeCos, ForceMode.Acceleration);
            }
            else
            {
                playerRB.drag = 13;
                playerRB.AddRelativeForce(Vector3.up * 9.81f, ForceMode.Acceleration);
                playerRB.AddRelativeForce(Vector3.Normalize(slopeHit) * -9.81f, ForceMode.Acceleration);
            }
        }
        else if (grounded && slope && slopeLimit)
        {
            playerRB.drag = 0;
            playerRB.AddRelativeForce(moveDir.normalized * airSpeed * 10, ForceMode.Acceleration);
            playerRB.AddRelativeForce(Vector3.down * 10, ForceMode.Acceleration);
        }
        else if (!grounded)
        {
            playerRB.drag = 1;
            playerRB.AddRelativeForce(moveDir.normalized * airSpeed * 17, ForceMode.Acceleration);
        }
        if (jump)
        {
            if(grounded || slope)
            {
                playerRB.AddRelativeForce(Vector3.up * 7, ForceMode.VelocityChange);
            }
            jump = false;
        }
    }
    void Jump()
    {
        jump = true;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 9)
        {
            foreach (ContactPoint p in collision.contacts)
            {
                if(p.point.y < transform.position.y - 0.56f)
                {
                    LastContactPoint = p;
                    Vector3 bottom = capCollider.bounds.center - (Vector3.up * capCollider.bounds.extents.y);
                    Vector3 curve = bottom + (Vector3.up * capCollider.radius);
                    slopeSin = Mathf.Sqrt(p.normal.x * p.normal.x + p.normal.z * p.normal.z);
                    slopeCos = Mathf.Sqrt(p.normal.y * p.normal.y);
                    Vector3 dir = curve - p.point;
                    slopeHit = p.normal;
                    Debug.DrawLine(curve, p.point, Color.blue, 0.55f);
                    if (p.point.y < curve.y - 0.15f)
                    {
                        grounded = true;
                    }
                    if (slopeSin < 0.05f && p.point.y < transform.position.y - 0.55f)
                    {
                        slope = false;
                        slopeLimit = false;
                    }
                    else if (slopeSin <= 0.708f && p.point.y < transform.position.y - 0.55f)
                    {
                        slope = true;
                        slopeLimit = false;
                    }
                    else if (slopeSin < 1 && p.point.y < transform.position.y - 0.55f)
                    {
                        slope = true;
                        slopeLimit = true;
                    }
                    else if (slopeSin >= 1 && p.point.y < transform.position.y - 0.55f)
                    {
                        slope = false;
                        slopeLimit = false;
                    }
                }               
            }
        }        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if(LastContactPoint.point.y < transform.position.y - 0.56f)
            {
                grounded = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            if (revolver.GetComponent<Revolver>().ammo >= 18)
            {
                return;
            }
            ammoSFX.Play();
            Destroy(other.gameObject);
            revolver.GetComponent<Revolver>().ammo += 4;
            if(revolver.GetComponent<Revolver>().ammo > 18)
            {
                revolver.GetComponent<Revolver>().ammo = 18;
            }
            revolver.GetComponent<Revolver>().ammoText.GetComponent<TextMeshProUGUI>().text = "/" + revolver.GetComponent<Revolver>().ammo;
        }
        else if (other.gameObject.CompareTag("Health"))
        {
            if(health >= 100)
            {
                return;
            }
            healSFX.Play();
            Destroy(other.gameObject);
            health += 30;
            if(health > 100)
            {
                health = 100;
            }
            HealthSystem(0);
        }
    }
}