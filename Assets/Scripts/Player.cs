using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static event Action<Vector3> OnMove;

    public float moveSpeed = 5f;

    private void Start()
    {
        // Start the coroutine
        StartCoroutine(MoveCoroutine());
    }

    void Update()
    {
        // Get input from arrow keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upDownInput = Input.GetAxis("UpDown");

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, upDownInput, verticalInput) * moveSpeed * Time.deltaTime;
        // Move the player
        transform.Translate(movement);

    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            OnMove?.Invoke(transform.position);
            yield return new WaitForSeconds(3f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
        // Check if the player collides with an object tagged as "Cube"
        if (collision.gameObject.CompareTag("cube"))
        {
            Debug.Log("on collision with cube");
            GameObject.Destroy(collision.gameObject);
        }
    }
}