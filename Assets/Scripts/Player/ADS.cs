using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ADS : MonoBehaviour
{
    [Header("References")]
    Animator anim;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject revolver;
    [SerializeField] GameObject fovController;
    [SerializeField] GameObject crosshair;
    InputManager inputManager;
    CinemachinePOV cinemachineController;
    public float fov = 60;
    public bool adsed;
    float sens;
    void Start()
    {
        inputManager = InputManager.Instance;
        crosshair.SetActive(true);
        anim = GetComponent<Animator>();
        cinemachineController = fovController.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
    }
    void Update()
    {
        sens = GameManager.Instance.mouseSens * 0.01f;
        if(GetComponentInParent<Player>().paused)
        {
            crosshair.SetActive(false);
            cinemachineController.m_HorizontalAxis.m_MaxSpeed = 0;
            cinemachineController.m_VerticalAxis.m_MaxSpeed = 0;
        }
        else
        {
            crosshair.SetActive(true);
            cinemachineController.m_HorizontalAxis.m_MaxSpeed = sens;
            cinemachineController.m_VerticalAxis.m_MaxSpeed = sens;
            if (!revolver.GetComponent<Revolver>().reloading)
            {
                if (inputManager.PlayerADS())
                {
                    crosshair.SetActive(false);
                    revolver.GetComponent<Revolver>().bulletDispersion = false;
                    fovController.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
                    if (!adsed)
                    {
                        cinemachineController.m_HorizontalAxis.m_MaxSpeed *= 0.8f;
                        cinemachineController.m_VerticalAxis.m_MaxSpeed *= 0.8f;
                    }
                    adsed = true;
                    anim.SetBool("ADS", true);
                }
                else
                {
                    crosshair.SetActive(true);
                    revolver.GetComponent<Revolver>().bulletDispersion = true;
                    if (adsed)
                    {
                        cinemachineController.m_HorizontalAxis.m_MaxSpeed *= 1.25f;
                        cinemachineController.m_VerticalAxis.m_MaxSpeed *= 1.25f;
                    }
                    adsed = false;
                    anim.SetBool("ADS", false);
                }
            }
            else
            {
                revolver.GetComponent<Revolver>().bulletDispersion = true;
                adsed = false;
                anim.SetBool("ADS", false);
            }
            fovController.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
        }
    }
}
