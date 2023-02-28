using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public Rigidbody2D rb;

    Vector2 movement; 
    // Update is called once per frame
    void Update()
    {
    
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement.x > 0)
        {
           gameObject.transform.localScale = new Vector3( 1, 1,1);
        }
        if (movement.x < 0)
        {
            gameObject.transform.localScale = new Vector3(- 1, 1,1);

        }
    }
}
