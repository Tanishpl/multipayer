using Unity.Netcode;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
       // _cam = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        HandleMovement();
      // HandleRotation();
    }

    

    [SerializeField] private float _acceleration = 80;
    [SerializeField] private float _maxVelocity = 10;
    private Vector3 _input;
    private Rigidbody _rb;

    private void HandleMovement()
    {
        _rb.velocity += _input.normalized * (_acceleration * Time.deltaTime);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxVelocity);
    }


    

   /*[SerializeField] private float _rotationSpeed = 450;
    private Plane _groundPlane = new(Vector3.up, Vector3.zero);
    private Camera _cam;

    private void HandleRotation()
    {
       var ray = _cam.ScreenPointToRay(Input.mousePosition);

        if (_groundPlane.Raycast(ray, out var enter))
        {
           var hitPoint = ray.GetPoint(enter);

           var dir = hitPoint - transform.position;
           var rot = Quaternion.LookRotation(dir);

           transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _rotationSpeed * Time.deltaTime);
       }
   }
  */
    
}