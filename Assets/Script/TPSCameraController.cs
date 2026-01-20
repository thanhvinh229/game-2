using System;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPSCameraController : MonoBehaviour
{
   
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _zoomLerpSpeed = 10f;
    [SerializeField] private float _minDistance = 3f;
    [SerializeField] private float _maxDistance = 15f;
    

    private GameInput _input;

    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currrentZoom;


    private void Start()
    {
        _input = new GameInput();
        _input.Enable();
        _input.CameraControll.MouseZoom.performed += HandleMouseScolll;

        Cursor.lockState = CursorLockMode.Locked;

        cam= GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currrentZoom = orbital.Radius;
    }

    private void HandleMouseScolll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
    }


     void Update()
    {
        if(scrollDelta.y != 0)
        {
            if(orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * _zoomSpeed, _minDistance, _maxDistance);
                scrollDelta = Vector2.zero;
            }

        }

        float bumperDelta = _input.CameraControll.GamepadZoom.ReadValue<float>();
        if (bumperDelta != 0) 
        { 
            targetZoom =Mathf.Clamp(orbital.Radius - bumperDelta * _zoomSpeed , _minDistance, _maxDistance);
        }
        currrentZoom = Mathf.Lerp(currrentZoom, targetZoom, Time.deltaTime * _zoomLerpSpeed);
        orbital.Radius = currrentZoom;
    }
}
