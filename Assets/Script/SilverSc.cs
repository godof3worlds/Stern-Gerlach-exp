using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
//this code is part of the code foor steern gerlach experinment simulator
//has dta and code on the quantum state and movvement of the silver atoms in the experiment

public class SilverSc : MonoBehaviour
{
    [SerializeField]
    private
        int spin = 0;
    [SerializeField]
    private float speed = 0.1f;

    Rigidbody rb;
    Vector3 PrevAxis = new Vector3(0,0,0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        // Move the silver atom forward based on its speed
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Field"))
        {
            GameObject field = collision.gameObject;
            Vector3 Axis = (field.transform.position).normalized;//get the rotational data of the field to determine the spin axis
            
            Debug.Log("Hit the field!");
            // When the silver atom enters the X-field, it splits into two paths based on its spin
            if (Axis != PrevAxis) // Check if the axis has changed
            {
                OnObserved();
                if (spin == 1)
                {
                    // Spin up: move to the right
                    
                    rb.linearVelocity = Vector3.Cross(Axis, Vector3.up)*speed;
                }
                else if (spin == -1)
                {
                    // Spin down: move to the left
                    rb.linearVelocity = Vector3.Cross(Axis, Vector3.up)*(-speed);
                }
            }
            PrevAxis = Axis;
        }
    }
    public void OnObserved()
    {
        // When the silver atom is observed, it collapses into a definite spin state
        spin = Random.Range(0, 2) == 0 ? -1 : 1; // Randomly choose spin up or down
    }
}
