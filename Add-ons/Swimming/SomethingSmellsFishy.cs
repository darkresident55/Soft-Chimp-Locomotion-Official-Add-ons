using System.Collections;
using System.Collections.Generic;
using SoftChimpMotion;
using UnityEngine;

public class SomethingSmellsFishy : MonoBehaviour
{
    // this will be set later on!
    [SerializeField] private GameObject LeftHand;
    [SerializeField] private GameObject RightHand;
    [SerializeField] private GameObject ForwardDirection;

    // yesnt.
    [SerializeField] private Vector3 PositionPreviousFrameLeftHand;
    [SerializeField] private Vector3 PositionPreviousFrameRightHand;
    [SerializeField] private Vector3 PlayerPositionPreviousFrame;
    [SerializeField] private Vector3 PlayerPositionCurrentFrame;
    [SerializeField] private Vector3 PositionCurrentFrameLeftHand;
    [SerializeField] private Vector3 PositionCurrentFrameRightHand;

    //Speed
    [SerializeField] private float Speed = 70;
    [SerializeField] private float HandSpeed;

    void Start()
    {
        LeftHand = FindObjectOfType<MotionSettings>().leftController;
        RightHand = FindObjectOfType<MotionSettings>().rightController;
        Rigidbody Rigid = FindObjectOfType<MotionSettings>().rb;
        Rigid.useGravity = false;
        PlayerPositionPreviousFrame = transform.position; //set current positions
        PositionPreviousFrameLeftHand = LeftHand.transform.position; //set previous positions
        PositionPreviousFrameRightHand = RightHand.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // get eye and fish there
        float yRotation = FindObjectOfType<MotionSettings>().headCollider.gameObject.transform.rotation.y;
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);

        // get positons of hands
        PositionCurrentFrameLeftHand = LeftHand.transform.position;
        PositionCurrentFrameRightHand = RightHand.transform.position;

        // position of player
        PlayerPositionCurrentFrame = transform.position;

        // lets just lazy fuck this haehaehahehaeaaa
        var playerDistanceMoved = Vector3.Distance(PlayerPositionCurrentFrame, PlayerPositionPreviousFrame);
        var leftHandDistanceMoved = Vector3.Distance(PositionPreviousFrameLeftHand, PositionCurrentFrameLeftHand);
        var rightHandDistanceMoved = Vector3.Distance(PositionPreviousFrameRightHand, PositionCurrentFrameRightHand);

        // you go fast fish if fish go fishy fish fish with some fish?
        HandSpeed = ((leftHandDistanceMoved - playerDistanceMoved) + (rightHandDistanceMoved - playerDistanceMoved));

        if(Time.timeSinceLevelLoad > 1f)
        {
            transform.position += ForwardDirection.transform.forward * HandSpeed * Speed * Time.deltaTime;
        }

        // set previous position of hands for next frame
        PositionPreviousFrameLeftHand = PositionCurrentFrameLeftHand;
        PositionPreviousFrameRightHand = PositionCurrentFrameRightHand;
        // set player position previous frame
        PlayerPositionPreviousFrame = PlayerPositionCurrentFrame;
    }
}