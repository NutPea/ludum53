using UnityEngine;
using System;
using System.Collections.Generic;
using Cinemachine;

public class CarController : MonoBehaviour
{
    private PlayerInput inputActions;

    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;
    private CarTimeHandler timeHandler;
    private bool isBreaking;
    private bool isDrifting;
    public float maxFallOverAngle = 35;
    public float fallOverAngularDrag = 2f;
    private float startAngularDrag;

    public float extraAcelationSpeed = 100000;
    public float extraAcelationVelocity = 10f;

    private float lastMoveInput;
    public bool canCounterBreak;
    public float counterBreakCooldown = 2f;
    public float breakeMoveVelocity = 5;
    public bool changedDirectionBackwards;
    public bool changedDirectionForward;
    public GameObject freecam;
    CinemachineFreeLook cinemachineFreeLook;

    public MeshRenderer carRenderer;
    public Material brakesOnMaterial;
    public Material brakesOffMaterial;

    public bool stopInput;

    private void Awake()
    {
        inputActions = new PlayerInput();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        timeHandler = GetComponent<CarTimeHandler>();
        carRb.centerOfMass = _centerOfMass;
        inputActions.Keyboard.Horizontal.performed += ctx => OnSteer();
        inputActions.Keyboard.Horizontal.canceled += ctx => OnStopSteer();
        inputActions.Keyboard.Vertical.performed += ctx => OnMoveInput();
        inputActions.Keyboard.Vertical.canceled += ctx => StopInput();
        inputActions.Keyboard.ChangeCamera.performed += ctx => ActivateFreeLookCam();
        inputActions.Keyboard.ChangeCamera.canceled += ctx => DeactivateFreeLookCam();

        inputActions.Keyboard.Break.performed += ((ctx) => {
            if (stopInput){
                return;
            }
            isBreaking = true;
        });

        inputActions.Keyboard.Break.canceled += ((ctx) => {
            if (stopInput){
                return;
            }
            isBreaking = false;
        });

        startAngularDrag = carRb.angularDrag;
        canCounterBreak = true;

        if (freecam){
            cinemachineFreeLook = freecam.GetComponent<CinemachineFreeLook>();
        }

        stopInput = true;
        timeHandler.OnCountDownFinished.AddListener(() => stopInput = false);
    }

    private void OnSteer(){
        steerInput = inputActions.Keyboard.Horizontal.ReadValue<float>();
    }

    private void OnStopSteer(){
        steerInput = 0;
    }

    private void OnMoveInput(){
        moveInput = inputActions.Keyboard.Vertical.ReadValue<float>();
        if (canCounterBreak)
        {
            if (moveInput < lastMoveInput && lastMoveInput > 0.99){
                changedDirectionBackwards = true;
                changedDirectionForward = false;
                canCounterBreak = false;
            }
            if(moveInput > lastMoveInput && lastMoveInput < -0.99){
                changedDirectionBackwards = false;
                changedDirectionForward = true;
                canCounterBreak = false;
            }
            
        }
        lastMoveInput = moveInput;
    }

    private void StopInput(){
        moveInput = 0f;
    }

    void Update()
    {
        AnimateWheels();
        WheelEffects();

        
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        Brake();
        CounterBreak();
        FallOverStop();

    }

    private void FallOverStop()
    {
        if (transform.eulerAngles.z > maxFallOverAngle && transform.eulerAngles.z < 180)
        {
            carRb.constraints = RigidbodyConstraints.FreezeRotationZ;
            if (steerInput <= 0)
            {
                carRb.constraints = RigidbodyConstraints.None;
            }
        }
        if (transform.eulerAngles.z < maxFallOverAngle - 360 && transform.eulerAngles.z > 180)
        {
            carRb.constraints = RigidbodyConstraints.FreezeRotationZ;
            if (steerInput >= 0)
            {
                carRb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    private void CounterBreak()
    {
        if (changedDirectionBackwards)
        {
            if (carRb.velocity.magnitude > breakeMoveVelocity)
            {
                BreakWheels();
            }
            if (moveInput >= 0 || carRb.velocity.magnitude < breakeMoveVelocity)
            {
                changedDirectionBackwards = false;
                Invoke(nameof(ResetCounterBreak), counterBreakCooldown);
            }
        }

        if (changedDirectionForward)
        {
            if (carRb.velocity.magnitude > breakeMoveVelocity)
            {
                BreakWheels();
            }
            if (moveInput <= 0 || carRb.velocity.magnitude < breakeMoveVelocity)
            {
                changedDirectionForward = false;
                Invoke(nameof(ResetCounterBreak), counterBreakCooldown);
            }
        }
    }


    private void ResetCounterBreak()
    {
        canCounterBreak = true;
    }
    void Move()
    {
        if (stopInput)
        {
            return;
        }
        if (carRb.velocity.magnitude < extraAcelationVelocity)
        {
            carRb.AddForce(transform.forward * extraAcelationSpeed * moveInput);
        }

        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration;
        }
    }

    void Steer()
    {
        if (stopInput)
        {
            return;
        }
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {

        if ((isBreaking || moveInput == 0) && steerInput == 0)
        {
            BreakWheels();
            Material[] newMaterials = carRenderer.materials;
            newMaterials[13] = brakesOnMaterial;
            carRenderer.materials = newMaterials;

        }
        else if((isBreaking && steerInput != 0))
        {
            isDrifting = true;
            Material[] newMaterials = carRenderer.materials;
            newMaterials[13] = brakesOnMaterial;
            carRenderer.materials = newMaterials;
            foreach (var wheel in wheels)
            {
                if (wheel.axel == Axel.Rear) {
                    WheelFrictionCurve sidewayFriction_rear = wheel.wheelCollider.gameObject.GetComponent<WheelCollider>().sidewaysFriction;
                    sidewayFriction_rear.stiffness = 1.8f;
                    wheel.wheelCollider.gameObject.GetComponent<WheelCollider>().sidewaysFriction = sidewayFriction_rear;
                }   
            }
        }
        else {
            isDrifting = false;
            Material[] newMaterials = carRenderer.materials;
            newMaterials[13] = brakesOffMaterial;
            carRenderer.materials = newMaterials;
            foreach (var wheel in wheels)
            {
                if (wheel.axel == Axel.Rear) {
                    WheelFrictionCurve sidewayFriction_rear = wheel.wheelCollider.gameObject.GetComponent<WheelCollider>().sidewaysFriction;
                    sidewayFriction_rear.stiffness = 4.8f;
                    wheel.wheelCollider.gameObject.GetComponent<WheelCollider>().sidewaysFriction = sidewayFriction_rear;
                }
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    private void BreakWheels()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0;
            wheel.wheelCollider.brakeTorque = 1000000000;
        }
    }

    void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
    
    void WheelEffects() {
        foreach(var wheel in wheels)
        {
            if ((isDrifting && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.velocity.magnitude >= 10.0f))
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }

    private void ActivateFreeLookCam() {
        if (!cinemachineFreeLook)
        {
            return;
        }
        if (stopInput)
        {
            return;
        }
        cinemachineFreeLook.Priority = 15;
    }

    private void DeactivateFreeLookCam() {
        if (!cinemachineFreeLook)
        {
            return;
        }
        if (stopInput)
        {
            return;
        }
        cinemachineFreeLook.Priority = 5;
    }
}
