using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get { return _instance; }
    }

    private PlayerInputs playerInputs;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
        playerInputs =  new PlayerInputs();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }
    public Vector2 GetPlayerMovement()
    {
        return playerInputs.Player.Movement.ReadValue<Vector2>();
    }
    public Vector2 GetMouseDelta()
    {
        return playerInputs.Player.Look.ReadValue<Vector2>();
    }
    public bool PlayerJump()
    {
        return playerInputs.Player.Jump.triggered;
    }
    public bool PlayerFire()
    {
        if (playerInputs.Player.Fire.WasPressedThisFrame())
        {
            return playerInputs.Player.Fire.triggered;
        }
        return false;
    }
    public bool PlayerFireRelease()
    {
        if (playerInputs.Player.Fire.WasReleasedThisFrame())
        {
            return playerInputs.Player.Fire.triggered;
        }
        return false;
    }
    public bool PlayerADS()
    {
        if (playerInputs.Player.ADS.IsPressed())
        {
            return true;
        }
        return false;
    }
    public bool PlayerCock()
    {
        return playerInputs.Player.Cock.triggered;
    }
    public bool PlayerReload()
    {
        if (playerInputs.Player.Reload.WasPressedThisFrame())
        {
            return playerInputs.Player.Reload.triggered;
        }
        return false;
    }
    public bool Pause()
    {
        return playerInputs.Player.Pause.triggered;
    }
    public bool ScrollDown()
    {
        if (playerInputs.Player.ScrollDown.WasPressedThisFrame())
        {
            return playerInputs.Player.ScrollDown.triggered;
        }
        return false;   
    }
    public bool ScrollUp()
    {
        if (playerInputs.Player.ScrollUp.WasPressedThisFrame())
        {
            return playerInputs.Player.ScrollUp.triggered;
        }
        return false;
    }
    private void OnDisable()
    {
        playerInputs.Disable();
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
