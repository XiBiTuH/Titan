using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    [SerializeField] private Transform hookshottransform;

    [SerializeField] private Transform hookshottransform2;

    [SerializeField] private Slider fuel_bar;

    [SerializeField] private TrollController TrollController;
    [SerializeField] private ColisionDetector ColisionDetector;

    [SerializeField] private AudioSource gear1;





    public float DropFuelRate = 1;

    public float speed = 12.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 10f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    public Camera playerCamera;


    public Vector3 velocity;
    public Vector3 velocityMomentum;

    bool isGrounded;

    private Vector3 hookPosition;

    private float hookShootSize;

    //States
    private State state;

    private enum State
    {
        Normal,
        HookShotFly,

        HookShotThrown,

        Attacking,

        Grabbed,



    }

    private cameraFOV cameraFOV;

    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    public float DistanceToHook = 15f;

    private Animator animator;

    private float timeAttack;


    private bool asAttacked = false;

    public bool isGrabbed = false;

    public GameObject whoGrabbed;
    void Awake()
    {



        cameraFOV = playerCamera.GetComponent<cameraFOV>();
        animator = this.GetComponent<Animator>();

        fuel_bar.value = 100;
        Cursor.lockState = CursorLockMode.Locked;

        state = State.Normal;

        hookshottransform.gameObject.SetActive(false);
        hookshottransform2.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if(isGrabbed){         
            state = State.Grabbed;
        }
        switch (state)
        {

            default:
                break;
            case State.Normal:
                animator.SetBool("Air", false);
                cameraFOV.setCameraFOV(NORMAL_FOV);
                hookshottransform.transform.localScale = new Vector3(0f, 0f, 0f);
                hookshottransform2.localScale = new Vector3(0f, 0f, 0f);

                MoveCharacter();
                HandleHookShot();
                HandleAttack();
                velocityMomentum = Vector3.zero;
                break;

            case State.Attacking:

                HandleAttack();
                break;

            case State.HookShotThrown:
                animator.SetBool("Air", true);
                HandleAttack();
                if (fuel_bar.value > 0)
                {
                    MoveCharacter();
                    HandleThrow();
                }
                else
                {
                    state = State.Normal;
                }
                break;

            case State.HookShotFly:
                animator.SetBool("Air", true);
                HandleAttack();
                if (fuel_bar.value > 0)
                {
                    HookShotMovement();
                }
                else
                {
                    state = State.Normal;
                }
                break;

            case State.Grabbed:
                controller.enabled = false;

                this.transform.parent = whoGrabbed.transform;
                break;


        }

    }

    private void HandleAttack()
    {

        //SE pressionar o botão
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Slash");
            asAttacked = true;
            timeAttack = Time.time;
        }

        //Se tiver attacad onos ultimos x segundos
        if ((Time.time - timeAttack) <= 2 && asAttacked)
        {
            //Se estiver dentro da weakspot mata o bicho
            if (ColisionDetector.gameObject.GetComponent<ColisionDetector>().isAttacking)
            {
                TrollController.gameObject.GetComponent<TrollController>().DeathFunc();
                asAttacked = false;
            }
        }
        else
        {
            asAttacked = false;
        }
    }


    private void DropFuel()
    {

        fuel_bar.value -= DropFuelRate * Time.deltaTime;
    }


    private bool TestJump()
    {
        return Input.GetButtonDown("Jump");
    }


    private bool TestInputHookShot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }


    private void HookShotMovement()
    {

        //Take fuel out
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            cameraFOV.setCameraFOV(HOOKSHOT_FOV);
            DropFuel();

        }
        else
        {
            cameraFOV.setCameraFOV(NORMAL_FOV);
        }


        if (hookPosition.z > 0)
        {


            hookshottransform.LookAt(hookPosition + new Vector3(1f, 0f, 0f));
            hookshottransform2.LookAt(hookPosition + new Vector3(-1f, 0f, 0f));

        }
        else
        {
            hookshottransform.LookAt(hookPosition + new Vector3(-1f, 0f, 0f));
            hookshottransform2.LookAt(hookPosition + new Vector3(1f, 0f, 0f));
        }



        float hookShotSpeed = 35f;


        Vector3 hookDir = (hookPosition - transform.position).normalized;

        if(Vector3.Distance(hookPosition, transform.position) < 1f){
            gear1.Stop();
        }
        controller.Move(hookDir * hookShotSpeed * Time.deltaTime);


        if (TestInputHookShot())
        {
            state = State.Normal;
            hookshottransform.gameObject.SetActive(false);
            hookshottransform2.gameObject.SetActive(false);
            gear1.Stop();

        }

        if (TestJump())
        {

            velocityMomentum = hookDir * hookShotSpeed / 500;
            float jumpSpeed = 7f;
            velocityMomentum += Vector3.up * jumpSpeed;
            hookshottransform.gameObject.SetActive(false);
            hookshottransform2.gameObject.SetActive(false);

            state = State.Normal;
            gear1.Stop();
        }
    }



    private void HandleHookShot()
    {
        if (TestInputHookShot())
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") && Vector3.Distance(hit.point, transform.position) < DistanceToHook)
                {
                    hookPosition = hit.point;
                    hookShootSize = 0f;
                    hookshottransform.gameObject.SetActive(true);
                    hookshottransform2.gameObject.SetActive(true);
                    if(fuel_bar.value > 0)
                        gear1.Play();
                    state = State.HookShotThrown;
                }
            }
        }
    }

    private void HandleThrow()
    {

        if (hookPosition.z > 0)
        {


            hookshottransform.LookAt(hookPosition + new Vector3(1f, 0f, 0f));
            hookshottransform2.LookAt(hookPosition + new Vector3(-1f, 0f, 0f));

        }
        else
        {
            hookshottransform.LookAt(hookPosition + new Vector3(-1f, 0f, 0f));
            hookshottransform2.LookAt(hookPosition + new Vector3(1f, 0f, 0f));
        }

        float hookSpeed = 70f;
        hookShootSize += hookSpeed * Time.deltaTime;
        hookshottransform.localScale = new Vector3(0.3f, 0.3f, hookShootSize);
        hookshottransform2.localScale = new Vector3(0.3f, 0.3f, hookShootSize);


        if (hookShootSize >= Vector3.Distance(transform.position, hookPosition))
        {
            state = State.HookShotFly;
        }




    }





    private void MoveCharacter()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            speed = 9;
            if (velocity.y < 0)
                velocity.y = -1f;

        }

        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        //Move 
        Vector3 move = transform.right * X + transform.forward * Z;
        controller.Move(move * speed * Time.deltaTime);


        //Jump
        if (TestJump() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Gravity velocity
        velocity.y += gravity * Time.deltaTime;

        //Aplly momentum
        velocity += velocityMomentum;

        controller.Move(velocity * Time.deltaTime);


    }




}
