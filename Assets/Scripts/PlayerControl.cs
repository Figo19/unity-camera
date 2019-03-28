using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public float movementSpeed = 1f;
    public float rotationSpeed = 1f;
    Rigidbody rb;

    public void Start() {
        rb = GetComponent<Rigidbody>();
    }


    public void FixedUpdate() {

        Vector3 movementVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        rb.transform.Translate(movementVelocity * Time.fixedDeltaTime);

        rb.transform.Rotate(Input.GetAxisRaw("Mouse X") * Vector3.up * rotationSpeed * Time.fixedDeltaTime);
    }




    public void OnValidate() {
        movementSpeed = Mathf.Max(movementSpeed, 0f);
        rotationSpeed = Mathf.Max(rotationSpeed, 0f);

    }
}
