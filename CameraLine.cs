using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLine : MonoBehaviour
{ 
    [SerializeField] Vector3 lineVector;
    public Camera _cam;
    [Range(0f, 10f)] public float _lineDistance;

    private float X_Drag;
    private float Y_Drag;
    private Ray _camRay;

    void Awake()
    {

    }
    void Start()
    {
        _cam.transform.position = gameObject.transform.position;
    }
    void Update()
    {

    }
    
    void LateUpdate()
    {
        _camRay = new Ray(_cam.transform.position, new Vector3(0,1f , -gameObject.transform.forward.magnitude));
        Debug.DrawRay(_cam.transform.position, new Vector3(0, 1f , -gameObject.transform.forward.magnitude) * 3f, Color.red);


        

        CamZoom();
        MouseDragOrbit();
    }

    private void MouseDragOrbit()
    {
        if (Input.GetMouseButton(1))
        {
            X_Drag = Input.GetAxis("Mouse X");
            Y_Drag = Input.GetAxis("Mouse Y");
            _cam.transform.RotateAround(gameObject.transform.position, gameObject.transform.up, X_Drag * 5f);
            _cam.transform.RotateAround(gameObject.transform.position, gameObject.transform.right, -Y_Drag * 5f);
            _cam.transform.LookAt(gameObject.transform);
        }
    }
    private void CamZoom()
    {
        _lineDistance += Input.mouseScrollDelta.y * 0.1f;
        _cam.transform.position = _camRay.GetPoint(_lineDistance);

    }

}
