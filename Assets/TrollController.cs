using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrollController : MonoBehaviour
{

    private State state;

    [SerializeField] private GameObject cube;

    [SerializeField] private Animator anim;

    [SerializeField] private GameObject player;

    private NavMeshAgent enemy;

    private float time_start;

    private float height_max = 32f;

    [SerializeField] private float distanceToAttack = 20f;

    private bool isDefending = false;




    private enum State
    {
        IDLE,
        WALK,
        ATTACK,
        DEATH,
    }

    private float start_def = 5f;

    private GameObject Catch1;
    private GameObject Catch2;

    private GameObject cameraPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Catch1 = GameObject.Find("Catch1");
        Catch2 = GameObject.Find("Catch2");
        player = GameObject.Find("Player");
        cameraPlayer = GameObject.Find("Main Camera");
        state = State.WALK;

        enemy = this.GetComponent<NavMeshAgent>();

        time_start = 5f;

    }

    // Update is called once per frame
    void Update()
    {


        switch (state)
        {

            default:
                break;
            case State.IDLE:
                HandleIdle();
                checkAttack();
                break;
            case State.WALK:
                moveTowardsPlayer();
                checkAttack();
                break;
            case State.ATTACK:
                HandleAttack();
                break;
            case State.DEATH:
                break;

        }
    }



    private void HandleAttack()
    {
        if ((Time.time - time_start) >= 5f){
            anim.SetTrigger("Attack");
            state = State.IDLE;
            time_start = Time.time;
        }
    }

    private void HandleIdle()
    {
        float numberRandom = 0f;

        enemy.isStopped = true;
        if (cameraPlayer.transform.position.y < height_max)
        {
            state = State.WALK;
            isDefending = false;
            anim.SetBool("isDefending", false);
            cube.SetActive(true);

        }
        else
        {
            if ((Time.time - start_def) >= 5)
            {
                start_def = Time.time;
                numberRandom = Random.Range(0f, 100.0f);

                if (numberRandom > 50f)
                {
                    anim.SetTrigger("Defending");
                    isDefending = true;
                    anim.SetBool("isDefending", true);
                    cube.SetActive(false);

                }
                else
                {
                    anim.SetBool("isDefending", false);
                    cube.SetActive(true);
                    isDefending = false;
                }

            }

            //50% defend





        }
    }


    private void moveTowardsPlayer()
    {

        enemy.isStopped = false;

        if (cameraPlayer.transform.position.y < height_max)
        {
            anim.SetBool("isWalking", true);
            enemy.SetDestination(cameraPlayer.transform.position);


        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("Idle");
            state = State.IDLE;
        }

    }

    private void checkAttack()
    {
        if (( Vector3.Distance(cameraPlayer.transform.position, Catch1.transform.position) < distanceToAttack  || Vector3.Distance(cameraPlayer.transform.position, Catch2.transform.position) < distanceToAttack) && (Time.time - time_start) >= 5)
        {
            state = State.ATTACK;
        }
    }

    public void DeathFunc()
    {
        if (!isDefending)
        {
            state = State.DEATH;
            anim.SetTrigger("Death");
        }

    }
}
