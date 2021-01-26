using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFOV : MonoBehaviour
{

    private Camera playerCamera;
    private float targetFOV;
    private float fov;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        targetFOV = playerCamera.fieldOfView;
        fov = targetFOV;
    }

    // Update is called once per frame
    void Update()
    {
        float fovSpeed =4f;

        fov = Mathf.Lerp(fov, targetFOV, Time.deltaTime * fovSpeed);
        playerCamera.fieldOfView = fov;
    }


    public void setCameraFOV(float targetFOV){
        this.targetFOV = targetFOV;
    }
}
