using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class vrMovement : MonoBehaviour
{
    public float m_Gravity=30.0f;
    public float m_Sensitivity=0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_rotateIncrement = 90;

    public SteamVR_Action_Boolean rotatePress = null;
    public SteamVR_Action_Boolean movePress = null;
    public SteamVR_Action_Vector2 moveValue = null;
    private float m_speed = 0.0f;
    private CharacterController m_characterController=null;
    public Transform m_CameraRig = null;
    public Transform m_Head = null;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //m_CameraRig = SteamVR_Render.Top().origin;
        //m_Head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHeight();
        CalculateMovement();
        SnapRotation();
    }
    private void CalculateMovement()
    {
        //Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        if (moveValue.axis.magnitude==0)
        {
            m_speed = 0;
        }
        m_speed += moveValue.axis.magnitude * m_Sensitivity;
        m_speed = Mathf.Clamp(m_speed, -m_MaxSpeed, m_MaxSpeed);

        movement += orientation * (m_speed * Vector3.forward);
        movement.y -= m_Gravity * Time.deltaTime;
        m_characterController.Move(movement*Time.deltaTime);
    }
    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.axis.x, moveValue.axis.y);
        rotation *= Mathf.Rad2Deg;
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }
    private void HandleHeight()
    {
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_characterController.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_characterController.height / 2;
        newCenter.y += m_characterController.skinWidth;

        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;
        m_characterController.center = newCenter;
    }
    private void SnapRotation()
    {
        float snapValue = 0.0f;
        if (rotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = -Mathf.Abs(m_rotateIncrement);
        }
        if (rotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(m_rotateIncrement);
        }
        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
}
