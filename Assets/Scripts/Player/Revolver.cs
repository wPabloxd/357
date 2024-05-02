using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Revolver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] GameObject cam;
    [SerializeField] Transform canonTip;
    [SerializeField] GameObject drum;
    [SerializeField] GameObject player;
    InputManager inputManager;
    [Header("Audio")]
    [SerializeField] AudioSource revolverShot;
    [SerializeField] AudioSource revolverCock;
    [SerializeField] AudioSource revolverUncock;
    [SerializeField] AudioSource revolverSpin;
    [SerializeField] AudioSource revolverReload;
    [SerializeField] AudioClip revolverEmpty;
    [SerializeField] AudioClip[] ricochets;
    [SerializeField] AudioClip revolverBullet;
    [Header("VFX")]
    [SerializeField] VisualEffect muzzleFlash;
    [SerializeField] GameObject flash;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject bulletImpactBloodNonLethal;
    [SerializeField] GameObject bulletImpactBlood;
    [SerializeField] GameObject bulletImpactSparks;
    [Header("UI")]
    [SerializeField] Image drumUI;
    [SerializeField] RectTransform drumUIFake;
    [SerializeField] RectMask2D HPmask;
    [SerializeField] Image HPhud;
    [SerializeField] GameObject RtoReload;
    [SerializeField] Image[] bulletReady = new Image[6];
    [SerializeField] Image[] bulletEmpty = new Image[6];
    public GameObject ammoText;
    [Header("RevolverParameters")]
    public bool reloading;
    public float drumOffset;
    public bool bulletDispersion = true;
    public int ammo = 6;
    [Header("Other")]
    [SerializeField] LayerMask red;
    [SerializeField] LayerMask blue;
    [SerializeField] Collider playerColl;

    bool cocked;
    bool trigger;
    bool bufferTrigger;
    bool bufferCock;
    bool bufferUncock;
    bool bufferReload;

    float drumPos;
    int bulletLoaded;
    bool shotDoubleAction;
    bool alreadyAdded;
    bool alreadySpinning;
    bool emptyDrum;
    bool canReload;
    bool[] emptyShell = new bool[6];
    Vector3 bulletSpread = new Vector3(0.05f, 0.05f, 0.05f);

    void Start()
    {
        ammoText.GetComponent<TextMeshProUGUI>().text = "/" + ammo;
        bulletLoaded = 1;
        inputManager = InputManager.Instance;
    }
    void Update()
    {
        if (!player.GetComponent<Player>().paused)
        {
            RevolverLogic();
        }
    }
    void RevolverLogic()
    {
        if (!emptyDrum)
        {
            for (int i = 0; i < 6; i++)
            {
                if (!emptyShell[i])
                {
                    emptyDrum = false;
                    break;
                }
                emptyDrum = true;
            }
            RtoReload.SetActive(false);
        }
        else
        {
            RtoReload.SetActive(true);
        }
        if (bulletLoaded >= 7)
        {
            bulletLoaded = 1;
        }
        else if (bulletLoaded <= 0)
        {
            bulletLoaded = 6;
        }
        if (inputManager.PlayerFire())
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCocked") && !trigger)
            {
                Shoot();
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle") && !cocked)
            {
                revolverCock.Play();
                revolverUncock.Stop();
                trigger = true;
                shotDoubleAction = true;
                bulletLoaded++;
                if (bulletLoaded >= 7)
                {
                    bulletLoaded = 1;
                }
                if (!emptyShell[bulletLoaded - 1])
                {
                    anim.SetInteger("cocking", 3);
                }
                else
                {
                    anim.SetInteger("cocking", 4);
                }
                bulletLoaded--;
                if (bulletLoaded <= 0)
                {
                    bulletLoaded = 6;
                }
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCock") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevShot") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevUncock") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevDrumPos") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevDrumNeg") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevEmptyshot") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevReload"))
            {
                StartCoroutine(BufferShot());
            }
        }
        if (inputManager.PlayerFireRelease())
        {
            trigger = false;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevTriggerCock"))
            {
                anim.SetInteger("cocking", 2);
                revolverUncock.Play();
                revolverCock.Stop();
                shotDoubleAction = false;
            }
        }
        if (inputManager.PlayerCock())
        {
            if (!cocked && !trigger)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle"))
                {
                    anim.SetInteger("cocking", 1);
                    Cocking();
                }
                else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("RevCocked") && !anim.GetCurrentAnimatorStateInfo(0).IsName("RevCock"))
                {
                    StartCoroutine(BufferingCock());
                }
            }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCocked"))
                {
                    Uncock();
                }
                else if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCock"))
                {
                    StartCoroutine(BufferingUncock());
                }
            }
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("RevShot") || anim.GetCurrentAnimatorStateInfo(0).IsName("RevEmptyshot")) && trigger)
        {
            Shoot();
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle"))
        {
            anim.SetInteger("spinDrum", 0);
            alreadyAdded = false;
            anim.SetBool("uncock", false);
            revolverUncock.Stop();
            if (bufferCock && !cocked)
            {
                anim.SetInteger("cocking", 1);
                Cocking();
            }
            else if (bufferTrigger)
            {
                shotDoubleAction = true;
                revolverCock.Play();
                revolverUncock.Stop();
                trigger = true;
                bulletLoaded++;
                if (bulletLoaded >= 7)
                {
                    bulletLoaded = 1;
                }
                if (!emptyShell[bulletLoaded - 1])
                {
                    anim.SetInteger("cocking", 3);
                }
                else
                {
                    anim.SetInteger("cocking", 4);
                }
                bulletLoaded--;
                if (bulletLoaded <= 0)
                {
                    bulletLoaded = 6;
                }
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevTriggerCock") && !trigger)
        {
            anim.SetInteger("cocking", 2);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCocked"))
        {
            if (!alreadyAdded)
            {
                bulletLoaded++;
                alreadyAdded = true;
            }
            if (bufferTrigger)
            {
                Shoot();
            }
            else if (bufferUncock)
            {
                Uncock();
            }
        }
        if (!alreadySpinning && !cocked && !trigger && anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle"))
        {
            if (inputManager.ScrollUp())
            {
                anim.SetInteger("spinDrum", 1);
                revolverSpin.Play();
                StartCoroutine(SpinningCD(true));
            }
            else if (inputManager.ScrollDown())
            {
                anim.SetInteger("spinDrum", -1);
                revolverSpin.Play();
                StartCoroutine(SpinningCD(false));
            }
        }
        if (inputManager.PlayerReload() && ammo > 0)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle"))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (emptyShell[i])
                    {
                        canReload = true;
                        break;
                    }
                    else
                    {
                        canReload = false;
                    }
                }
                if (canReload)
                {
                    Reload();
                }
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("RevCocked"))
            {
                Uncock();
                StartCoroutine(BufferReload());
            }
            else
            {
                StartCoroutine(BufferReload());
            }
        }
        if (bufferReload && anim.GetCurrentAnimatorStateInfo(0).IsName("RevIdle"))
        {
            for (int i = 0; i < 6; i++)
            {
                if (emptyShell[i])
                {
                    canReload = true;
                    break;
                }
                else
                {
                    canReload = false;
                }
            }
            if (canReload)
            {
                Reload();
            }
        }
        drumUI.rectTransform.eulerAngles = new Vector3(0, 0, drumPos - 60);
        drum.transform.localEulerAngles = new Vector3(0, drumPos, 0);
        drumUIFake.eulerAngles = new Vector3(0, 0, drumPos - 60);
        HPhud.rectTransform.eulerAngles = new Vector3(0, 0, drumPos - 60);
        SpinDrum();
    }
    void Reload()
    {
        reloading = true;
        anim.ResetTrigger("reload");
        bool canReload = false;
        int bulletWasted = 0;
        for (int i = 0; i < 6; i++)
        {
            if (emptyShell[i] == true)
            {     
                canReload = true;
                bulletWasted++;
            }
        }
        if (canReload)
        {
            anim.SetTrigger("reload");
            revolverReload.Play();
            StartCoroutine(ReloadTimer(bulletWasted));  
        }
    }
    IEnumerator ReloadTimer(int bulletsToReload)
    {
        yield return new WaitForSeconds(1);
        if(ammo < bulletsToReload)
        {
            bulletsToReload = ammo;
            ammo = 0;
        }
        else
        {
            ammo -= bulletsToReload;
        }
        for (int i = 0; i < 6; i++)
        {
            if(emptyShell[i] == true && bulletsToReload > 0)
            {
                emptyShell[i] = false;
                bulletReady[i].gameObject.SetActive(true);
                bulletEmpty[i].gameObject.SetActive(false);
                bulletsToReload--;
            }
        }
        ammoText.GetComponent<TextMeshProUGUI>().text = "/" + ammo;
        emptyDrum = false;
        RtoReload.SetActive(false);
        yield return new WaitForSeconds(1);
        reloading = false;
    }
    void Cocking()
    {
        alreadySpinning = false;
        shotDoubleAction = false;
        cocked = true;
        trigger = false;
        anim.ResetTrigger("shot");
        anim.ResetTrigger("shotEmpty");
        revolverCock.Play();
    }
    void Shoot()
    {
        if (shotDoubleAction)
        {
            bulletLoaded++;
            shotDoubleAction = false;
        }
        if (bulletLoaded == 7)
        {
            bulletLoaded = 1;
        }
        trigger = false;
        cocked = false;
        anim.SetInteger("cocking", 0);
        if (!emptyShell[bulletLoaded - 1])
        {
            if(bulletLoaded % 2 == 0)
            {
                Bullet("EnemyBlue", "EnemyRed");
            }
            else
            {
                Bullet("EnemyRed", "EnemyBlue");
            }
            StartCoroutine(ShotNoise());
            anim.SetTrigger("shot");
            revolverShot.clip = revolverBullet;
            revolverShot.Play();
            muzzleFlash.Play();
            flash.SetActive(true);
            emptyShell[bulletLoaded - 1] = true;
            bulletReady[bulletLoaded - 1].gameObject.SetActive(false);
            bulletEmpty[bulletLoaded - 1].gameObject.SetActive(true);
        }
        else
        {
            anim.SetTrigger("shotEmpty");
            revolverShot.clip = revolverEmpty;
            revolverShot.Play();
        }
        alreadySpinning = false;
    }
    IEnumerator ShotNoise()
    {
        GameManager.Instance.loudShot = true;
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.loudShot = false;
    }
    void Bullet(string tag1, string tag2)
    {
        Vector3 direction = GetDirection();
        if (Physics.Raycast(cam.transform.position ,direction,out RaycastHit hit, Mathf.Infinity))
        {
            if (tag1 == "EnemyRed")
            {
                if (hit.transform.gameObject.CompareTag(tag1))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpactBlood, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                    impact.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
                    hit.transform.gameObject.GetComponentInParent<EnemyRed>().Death();
                    hit.transform.gameObject.GetComponentInParent<EnemyRed>().anim.enabled = false;
                    hit.transform.gameObject.GetComponentInParent<Rigidbody>().AddForce(direction * 1000, ForceMode.Acceleration);
                }
                else if(hit.transform.gameObject.CompareTag(tag2))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpactSparks, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                    impact.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
                    hit.transform.GetComponentInParent<AudioSource>().clip = ricochets[Random.Range(0, ricochets.Length)];
                    hit.transform.GetComponentInParent<AudioSource>().Play();
                }
                else if (!hit.transform.gameObject.CompareTag("Door"))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpact, pos, Quaternion.LookRotation(hit.normal));
                    impact.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);
                }
            }
            else if (tag1 == "EnemyBlue")
            {
                if (hit.transform.gameObject.CompareTag(tag2))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpactBloodNonLethal, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                    impact.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
                }
                else if (hit.transform.gameObject.CompareTag(tag1))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpactBlood, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                    impact.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
                    hit.transform.gameObject.GetComponentInParent<EnemyBlue>().Death();
                    hit.transform.gameObject.GetComponentInParent<EnemyBlue>().anim.enabled = false;
                    hit.transform.gameObject.GetComponentInParent<Rigidbody>().AddForce(direction * 1000, ForceMode.Acceleration);
                }
                else if(!hit.transform.gameObject.CompareTag("Door"))
                {
                    Vector3 pos = new Vector3(hit.point.x + hit.normal.x * 0.001f, hit.point.y + hit.normal.y * 0.001f, hit.point.z + hit.normal.z * 0.001f);
                    GameObject impact = Instantiate(bulletImpact, pos, Quaternion.LookRotation(hit.normal));
                    impact.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);
                }
            }
        }
    }
    Vector3 GetDirection()
    {
        Vector3 direction = cam.transform.forward;
        if (bulletDispersion)
        {
            direction += new Vector3 (Random.Range(-bulletSpread.x, bulletSpread.x),Random.Range(-bulletSpread.y, bulletSpread.y),Random.Range(-bulletSpread.z, bulletSpread.z));
            direction.Normalize();
        }
        return direction;
    }
    void Uncock()
    {
        shotDoubleAction = false;
        anim.SetBool("uncock", true);
        revolverUncock.Play();
        cocked = false;
        trigger = false;
        alreadySpinning = false;
        anim.SetInteger("cocking", 0);
    }
    IEnumerator BufferingCock()
    {
        bufferCock = true;
        anim.SetInteger("cocking", 1);
        yield return new WaitForSeconds(0.4f);
        anim.SetInteger("cocking", 0);
        bufferCock = false;
    }
    IEnumerator BufferShot()
    {
        bufferTrigger = true;
        yield return new WaitForSeconds(0.4f);
        bufferTrigger = false;
    }
    IEnumerator BufferingUncock()
    {
        bufferCock = false;
        bufferUncock = true;
        anim.SetInteger("cocking", 0);
        anim.SetBool("uncock", true);
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("uncock", false);
        bufferUncock = false;
    }
    IEnumerator BufferReload()
    {
        bufferReload = true;
        yield return new WaitForSeconds(0.5f);
        bufferReload = false;
    }
    IEnumerator SpinningCD(bool positive)
    {
        alreadySpinning = true;
        yield return new WaitForSeconds(0.5f);
        if (positive)
        {
            bulletLoaded++;
        }
        else
        {
            bulletLoaded--;
        }
        alreadySpinning = false;
    }
    public void SpinDrum()
    {
        switch (bulletLoaded)
        {
            case 1:
                drumPos = drumOffset;
                break;
            case 2:
                drumPos = drumOffset + 60;
                break;
            case 3:
                drumPos = drumOffset + 120;
                break;
            case 4:
                drumPos = drumOffset + 180;
                break;
            case 5:
                drumPos = drumOffset + 240;
                break;
            case 6:
                drumPos = drumOffset + 300;
                break;
        }
    }
}