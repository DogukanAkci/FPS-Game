using System;
using DG.Tweening;
using UnityEngine;

public class CharakterController : MonoBehaviour
{
    [Header("Control Values")] [Space(10)] 
    
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = .4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpHeiht = 3f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxRunningSpeed = 20f;
    [SerializeField] private float currentspeed;
    [SerializeField] Rifle _rifle;

    [Header("Animators and Weapons")] [Space(10)]
    
    public Animator[] _animator;
    public GameObject[] weapons;
    private float speed = 1f;
    private string currentKeyInput;
    private Vector3 velocity;
    public bool isGrounded;
    public bool isRunning;
    private bool isBending;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        transform.position = transform.GetChild(0).position;
        transform.localRotation =
            Quaternion.RotateTowards(transform.localRotation, transform.GetChild(0).localRotation, Mathf.Infinity);

        if (_controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        bool isKeyPressing = (Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0);


        if (isKeyPressing)
        {
            if (currentspeed > maxSpeed + 1f && !isRunning)
                currentspeed -= 30f * Time.deltaTime;
            if (currentspeed < maxSpeed && !isRunning)
                currentspeed += 7f * Time.deltaTime;
            else if (!isRunning)
                currentspeed = maxSpeed;
            if (currentspeed < maxRunningSpeed && isRunning)
                currentspeed += 7f * Time.deltaTime;
            else if (isRunning)
                currentspeed = maxRunningSpeed;
        }
        else
        {
            if (currentspeed > 1f)
                currentspeed -= 150f * Time.deltaTime;
            else
                currentspeed = 0;
        }


        float x = Input.GetAxis("Horizontal") * currentspeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * currentspeed * Time.deltaTime;

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move);

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRunning = !isRunning;
            SlowWalking(isRunning);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isBending = !isBending;
            Bending(!isBending);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeiht * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
        currentKeyInput = Input.inputString;
        
        if (Input.anyKey)
        {
            ChangeWeapon(currentKeyInput);
        }
        Animations();
    }

    void SlowWalking(bool isRuning)
    {
        if (isRuning && currentspeed > maxSpeed)
            currentspeed -= 7f * Time.deltaTime;
        else if (!isRuning && currentspeed < maxRunningSpeed)
            currentspeed += 7f * Time.deltaTime;
    }

    void Bending(bool isBending)
    {
        if (!isBending)
        {
            _controller.height = Mathf.Lerp(0f, 4f, .5f);
            transform.DOLocalMoveY(transform.localPosition.y - 1, .5f);

            if (!isRunning)
                speed /= 4;
            else
                speed /= 2;
        }
        else
        {
            _controller.height = Mathf.Lerp(0, 6f, .5f);
            transform.DOLocalMoveY(transform.localPosition.y + 1, .5f);
            if (!isRunning)
                speed *= 4;
            else
                speed *= 2;
        }
    }

    void Animations()
    {
        //TODO: Rifle Animations

        _animator[0].SetFloat("WalkandRun", currentspeed);
        if (_rifle.CanFireRifle)
            _animator[0].SetBool("Shot", _rifle.CanFireRifle);
        else
            _animator[0].SetBool("Shot", _rifle.CanFireRifle);

        //TODO: Gun Animations

        _animator[1].SetFloat("WalkandRunGun", currentspeed);
        if (_rifle.CanFireRifle)
            _animator[1].SetTrigger("ShotGun");
    }

    void ChangeWeapon(string selection)
    {
        if (Int32.TryParse(selection, out int number))
        {
            switch (number)
            {
                case 1:
                    _animator[0].SetBool("RifleHide",!weapons[0].activeSelf);
                    weapons[1].SetActive(false);
                    weapons[0].SetActive(true);
                    _animator[1].SetBool("GunHide",!weapons[1].activeSelf);
                    break;
                case 2:
                    _animator[1].SetBool("GunHide",!weapons[1].activeSelf);
                    weapons[0].SetActive(false);
                    weapons[1].SetActive(true);
                    _animator[0].SetBool("RifleHide",!weapons[0].activeSelf);
                    break;
            }
        }
    }
}