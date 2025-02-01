using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxPassThrough : MonoBehaviour
{
    [SerializeField]
    private float rayDistanceOutsideBox;
    [SerializeField]
    private float rayDistanceInsideBox;
    [SerializeField]
    private Transform rayPointInsideBox;
    [SerializeField]
    private Transform rayPointLeftOfBox;
    [SerializeField]
    private Transform rayPointRightOfBox;
    [SerializeField] 
    private Transform rayPointTopOfBox;
    [SerializeField]
    private Transform grabPoint;

    [SerializeField]
    private Transform grabbedObject;
    [SerializeField]
    private GameObject grabObject1;

    public GameObject Box;


    private int layerIndex;
    private int layerIndex2;
    private int layerToIgnore;


        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        layerToIgnore = ~(LayerMask.GetMask("Boxes"));
        layerIndex = LayerMask.NameToLayer("Boxes");
        layerIndex2 = LayerMask.NameToLayer("Player");


        if (layerIndex == -1 || layerIndex2 == -1)
        {
            Debug.LogError("One of the layers does not exist. Please check the layer names.");
            return;
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(rayPointLeftOfBox.position, transform.right * rayDistanceOutsideBox, Color.green);
        Debug.DrawRay(rayPointRightOfBox.position, -transform.right * rayDistanceOutsideBox, Color.green);
        Debug.DrawRay(rayPointTopOfBox.position, transform.up * rayDistanceOutsideBox, Color.green);
        Debug.DrawRay(rayPointInsideBox.position, transform.right * rayDistanceInsideBox, Color.red);

        
        pushAndPull();
        dropBox();
        leaveBox();
    }




    void pushAndPull()
    {
        //Check for push and pull action
        RaycastHit2D hitInfoRightOfBox = Physics2D.Raycast(rayPointLeftOfBox.position, transform.right, rayDistanceOutsideBox);
        RaycastHit2D hitInfoLeftOfBox = Physics2D.Raycast(rayPointRightOfBox.position, -transform.right, rayDistanceOutsideBox);

        if (hitInfoRightOfBox.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.Space) && grabObject1 == null)
            {
                grabObject1 = hitInfoRightOfBox.collider.gameObject;
                Box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                Box.transform.position = grabPoint.position;
                Box.transform.SetParent(grabObject1.transform);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                grabObject1 = null;
                Box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                Box.transform.SetParent(null);
            }
        }

        else if (hitInfoLeftOfBox.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.Space) && grabObject1 == null)
            {
                grabObject1 = hitInfoLeftOfBox.collider.gameObject;
                Box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                Box.transform.position = grabPoint.position;
                Box.transform.SetParent(grabObject1.transform);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                grabObject1 = null;
                Box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                Box.transform.SetParent(null);
            }
        }
    }

    void dropBox()
    {
        RaycastHit2D hitInfoTopOfBox = Physics2D.Raycast(rayPointTopOfBox.position, transform.up, rayDistanceOutsideBox);
        if (hitInfoTopOfBox.collider != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Physics2D.IgnoreLayerCollision(layerIndex, layerIndex2, true);
            }
        }
    }

    void leaveBox()
    {
        RaycastHit2D hitInsideBox = Physics2D.Raycast(rayPointInsideBox.position, transform.right, rayDistanceInsideBox, layerToIgnore);

        if (hitInsideBox.collider != null)
        {
            // If the object hit is different or first time hit
            if (hitInsideBox.transform != grabbedObject)
            {
                if (grabbedObject != null)
                {
                    Debug.Log(grabbedObject.name + " has exited the raycast!");
                }
                grabbedObject = hitInsideBox.transform; // Update last hit object
            }
        }
        else
        {
            // If the ray stops hitting an object
            if (grabbedObject != null)
            {
                Debug.Log(grabbedObject.name + " has exited the raycast!");
                grabbedObject = null; // Reset the last hit object
                Physics2D.IgnoreLayerCollision(layerIndex, layerIndex2, false);
            }
        }
    }
}
