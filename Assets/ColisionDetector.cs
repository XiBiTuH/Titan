using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColisionDetector : MonoBehaviour
{

    public bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Sword"){
            isAttacking = true;
        }
    }
        void OnTriggerExit(Collider collider){
        if(collider.gameObject.tag == "Sword"){
            isAttacking = false;
        }
    }
}
