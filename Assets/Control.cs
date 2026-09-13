using UnityEngine;
using UnityEngine.EventSystems;

public class Control : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move()
    {
        //transform.Translate(new Vector3(direction.x, 0, direction.y) * Time.deltaTime * speed);
        Debug.Log("Move");
    }
    public void Jump() 
    { 
        rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        Debug.Log("Jump");
    }
    
}
