// using UnityEngine;
// using System.Linq;
// using UnityEngine.InputSystem;
// using TMPro;

// public enum DebugMode { Normal, Distance, Vision }

// public class PlayerController : MonoBehaviour {

//     public float speed;
//     public TextMeshProUGUI countText;
//     public TextMeshProUGUI position_Display;
//     public TextMeshProUGUI velocity_Display;
//     public TextMeshProUGUI closestPickup_Display;
//     public GameObject winTextObject;

//     private float movementX;
//     private float movementY;
//     private Vector3 previousPosition;
//     private LineRenderer lineRenderer;

//     private Rigidbody rb;
//     private int count;

//     void Start() {
//         rb = GetComponent<Rigidbody>();
//         count = 0;
//         SetCountText();
//         winTextObject.SetActive(false);
//         lineRenderer = GetComponent<LineRenderer>();
//         closestPickup_Display.text = "Closest Pickup: N/A";
//     }

//     void FixedUpdate() {
//         Vector3 movement = new Vector3(movementX, 0.0f, movementY);
//         rb.AddForce(movement * speed);

//         position_Display.text = "Position: " + transform.position.ToString();

//         Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;
//         previousPosition = transform.position;
//         velocity_Display.text = "Velocity: " + velocity.magnitude.ToString("F2");

//         // Find the closest pickup
//         GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
//         pickups = pickups.Where(pickup => pickup.activeSelf).ToArray();
//         float closestDistance = float.MaxValue;
//         GameObject closestPickup = null;
//         foreach (GameObject pickup in pickups) {
//             float distance = Vector3.Distance(transform.position, pickup.transform.position);
//             if (distance < closestDistance) {
//                 closestDistance = distance;
//                 closestPickup = pickup;
//             }
//         }

//         // Update the closest pickup display
//         if (closestPickup != null) {
//             closestPickup_Display.text = "Closest Pickup: " + closestDistance.ToString("F2") + "m";
//             closestPickup.GetComponent<Renderer>().material.color = Color.blue;
//         }
//     }

//     void OnTriggerEnter(Collider other) {
//         if (other.gameObject.CompareTag("PickUp")) {
//             other.gameObject.SetActive(false);
//             count = count + 1;
//             SetCountText();
//         }
//     }

//     void OnMove(InputValue value) {
//         Vector2 v = value.Get<Vector2>();
//         movementX = v.x;
//         movementY = v.y;
//     }

//     void SetCountText() {
//         countText.text = "Count: " + count.ToString();
//         if (count >= 12) {
//             winTextObject.SetActive(true);
//         }
//     }
// }

using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using TMPro;

public enum DebugMode { Normal, Distance, Vision }

public class PlayerController : MonoBehaviour {

    public float speed;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI position_Display;
    public TextMeshProUGUI velocity_Display;
    public TextMeshProUGUI closestPickup_Display;
    public GameObject winTextObject;

    private float movementX;
    private float movementY;
    private Vector3 previousPosition;
    private LineRenderer lineRenderer;

    private Rigidbody rb;
    private int count;
    
	public enum Mode {normal, Distance, Vision};
	public Mode currentMode = Mode.normal;

    void Start() {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        lineRenderer = GetComponent<LineRenderer>();
        closestPickup_Display.text = "Closest Pickup: 0";

    }

    void FixedUpdate() {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

		if(Input.GetKeyDown(KeyCode.Space)) {
			switch(currentMode) {
				case Mode.normal:
					currentMode = Mode.Distance;
					break;
				case Mode.Distance:
					currentMode = Mode.Vision;
					break;
			}
		}

		if(Input.GetKeyUp(KeyCode.Space)) {
			switch(currentMode) {
				case Mode.normal:
					currentMode = Mode.Distance;
					break;
				case Mode.Distance:
					currentMode = Mode.Vision;
					break;
			}
		}

		switch(currentMode) {
			case Mode.Distance:
				displayVelocAndPosi();
        		findClosestPickUp();
				break;
			case Mode.Vision:
				ShowVisionDebug();
				break;
		}

    }

    void displayVelocAndPosi() {
        position_Display.text = "Position: " + transform.position.ToString();

        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        velocity_Display.text = "Velocity: " + velocity.magnitude.ToString("F2");

    }

    void findClosestPickUp() {
		
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        pickups = pickups.Where(pickup => pickup.activeSelf).ToArray();
        float closestDistance = float.MaxValue;
        GameObject closestPickup = null;
        foreach (GameObject pickup in pickups) {
            float distance = Vector3.Distance(transform.position, pickup.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPickup = pickup;
            }
        }

        if (closestPickup != null) {
            closestPickup_Display.text = "Closest Pickup: " + closestDistance.ToString("F2") + "m";
			closestPickup.GetComponent<Renderer>().material.color = Color.blue;
        } 
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("PickUp")) {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void OnMove(InputValue value) {
        Vector2 v = value.Get<Vector2>();
        movementX = v.x;
        movementY = v.y;
    }

    void SetCountText() {
        countText.text = "Count: " + count.ToString();
        if (count >= 12) {
            winTextObject.SetActive(true);
        }
    }

    private void ShowVisionDebug() {
		position_Display.text = "Position: " + transform.position.ToString();

        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        velocity_Display.text = "Velocity: " + velocity.magnitude.ToString("F2");
		GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
		GameObject closestPickup = null;
        foreach (GameObject pickup in pickups) {
            pickup.GetComponent<Renderer>().material.color = Color.red;
        }

    // Visualize the vision cones for each pickup
        foreach (GameObject pickup in pickups) {
            Vector3 direction = (pickup.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(velocity, direction);

            if (angle < 30.0f) {
                pickup.GetComponent<Renderer>().material.color = Color.green;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, pickup.transform.position);
            }
        }

    // Highlight the closest pickup
        if (closestPickup != null) {
            closestPickup.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

}
