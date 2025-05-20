using UnityEngine;
using UnityEngine.XR;

public class Swimming : MonoBehaviour
{
    private InputDevice LeftControllerDevice;
    private InputDevice RightControllerDevice;
    private Vector3 LeftControllerVelocity;
    private Vector3 RightControllerVelocity;
    private Rigidbody chimpRigidBody; // Renamed for clarity
    private Vector3 swimVelocity;
    public LayerMask whatIsWater;
    public float radius = 0.25f;
    public float swimMultiplier = 1;
    public float minControllerForce = 1;
    public float maxControllerForce = 10; 
    public float vibrationDuration = 0.5f; 
    public bool canSwim = false;
    private SoftChimpMotion.MotionSettings motionSettings; // Modify this line

    // Start is called before the first frame update
    private void Start()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        chimpRigidBody = GameObject.Find("Chimp").GetComponent<Rigidbody>(); // Directly access the Rigidbody of the Chimp object
        motionSettings = GameObject.Find("Chimp").GetComponent<SoftChimpMotion.MotionSettings>(); // Modify this line
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered gravity zone");
            chimpRigidBody.useGravity = false; // Directly toggle gravity
            canSwim = true;
            motionSettings.inWaterOrSpace = true; // Modify this line
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exited gravity zone");
            chimpRigidBody.useGravity = true; // Directly toggle gravity
            canSwim = false;
            motionSettings.inWaterOrSpace = false; // Modify this line
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        LeftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerVelocity);
        RightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
        swimVelocity = new Vector3((((LeftControllerVelocity.x + RightControllerVelocity.x) / 2) * swimMultiplier), ((LeftControllerVelocity.y + RightControllerVelocity.y) / 2 * swimMultiplier), ((LeftControllerVelocity.z + RightControllerVelocity.z) / 2) * swimMultiplier);
        swimVelocity = Quaternion.Euler(0, transform.eulerAngles.y, 0) * swimVelocity; // Rotate the swim velocity vector to be relative to the player's forward direction
        if (canSwim && (LeftControllerVelocity.magnitude > minControllerForce || RightControllerVelocity.magnitude > minControllerForce))
        {
            chimpRigidBody.AddForce(-swimVelocity); // Modify this line

            if (LeftControllerVelocity.magnitude > minControllerForce)
            {
                float amplitude = Mathf.InverseLerp(minControllerForce, maxControllerForce, LeftControllerVelocity.magnitude);
                VibrateController(LeftControllerDevice, amplitude, vibrationDuration);
            }

            if (RightControllerVelocity.magnitude > minControllerForce)
            {
                float amplitude = Mathf.InverseLerp(minControllerForce, maxControllerForce, RightControllerVelocity.magnitude);
                VibrateController(RightControllerDevice, amplitude, vibrationDuration);
            }
        }
    }

    private void VibrateController(InputDevice controller, float amplitude, float duration)
    {
        HapticCapabilities capabilities;
        if (controller.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                controller.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }
}
