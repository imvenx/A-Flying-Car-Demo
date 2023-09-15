using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    public ArcanePad Pad { get; private set; }
    public float speed = 0.5f;
    private const float maxRotationRateX = 2.5f;
    private const float maxRotationRateY = 2.5f;
    private const float maxRotationRateZ = 2.5f;
    private Quaternion lastGamepadRotation = Quaternion.identity;
    private Quaternion lastReferenceRotation;
    private bool isDrivingEnabled = false;

    // Rotation values to be applied in FixedUpdate
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float rotationZ = 0f;

    public void Initialize(ArcanePad pad)
    {
        Pad = pad;

        Pad.StartGetQuaternion();
        Pad.OnGetQuaternion((GetQuaternionEvent e) =>
        {
            if (!isDrivingEnabled)
            {
                lastReferenceRotation = new Quaternion(-e.y, e.x, e.z, e.w);
                return;
            }
            ComputeRotationBasedOnXYZ(new Quaternion(-e.y, e.x, e.z, e.w));
        });

        Pad.On(GameEvent.ChangeSpeed, (ChangeSpeedEvent e) =>
        {
            speed = e.speed * 0.05f;
        });

        Pad.On(GameEvent.EnableDrive, (EnableDriveEvent e) =>
        {
            isDrivingEnabled = true;
            Debug.Log(isDrivingEnabled);
        });

        Pad.On(GameEvent.DisableDrive, (DisableDriveEvent e) =>
        {
            isDrivingEnabled = false;
            Debug.Log(isDrivingEnabled);
        });
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0, speed));
        if (isDrivingEnabled)
        {
            transform.Rotate(rotationX, rotationY, rotationZ);
        }
    }

    void ComputeRotationBasedOnXYZ(Quaternion gamepadRotation)
    {
        gamepadRotation = Quaternion.Inverse(lastReferenceRotation) * gamepadRotation;

        if (Quaternion.Dot(lastGamepadRotation, gamepadRotation) < 0)
        {
            gamepadRotation = new Quaternion(-gamepadRotation.x, -gamepadRotation.y, -gamepadRotation.z, -gamepadRotation.w);
        }

        rotationX = gamepadRotation.x * maxRotationRateX;
        rotationY = gamepadRotation.y * maxRotationRateY;
        rotationZ = gamepadRotation.z * maxRotationRateZ;

        lastGamepadRotation = gamepadRotation;
    }
}
