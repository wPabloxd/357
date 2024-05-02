using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachinePOVExt : CinemachineExtension
{
    [SerializeField] private float clampAngle = 90f;
    [SerializeField] private float mouseSpeed = 10f;
    private InputManager inputManager;
    private Vector3 startingRotation;
    protected override void Awake()
    {
        inputManager = InputManager.Instance;
        startingRotation = transform.localEulerAngles;
        base.Awake();
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if(stage== CinemachineCore.Stage.Aim)
            {
                Vector2 deltaInput = inputManager.GetMouseDelta();
                startingRotation.x += deltaInput.x * mouseSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * mouseSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation = Quaternion.Euler(startingRotation.y, startingRotation.x, 0);
            }
        }
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
