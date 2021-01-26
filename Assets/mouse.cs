using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse : MonoBehaviour
{

    //Sensivity
    public float sensivity = 100.0f;

    //Transforms
    public Transform playerBody;

    //Rotation
    public float xRotation = 0f;

        
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Get Input
        float mouseX = Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;




        //Rotate on the Y Axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        //Rotate on the X Axis
        playerBody.Rotate(Vector3.up * mouseX);
    }

}
