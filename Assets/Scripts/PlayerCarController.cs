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

    public void Initialize(ArcanePad pad)
    {
        Pad = pad;

        Pad.StartGetQuaternion();
        Pad.OnGetQuaternion((GetQuaternionEvent e) =>
        {
            if (PlayerManager.isGamePaused) return;
            ApplyRotationBasedOnXYZ(new Quaternion(-e.y, e.x, e.z, e.w));
        });
    }

    void ApplyRotationBasedOnXYZ(Quaternion gamepadRotation)
    {
        if (Quaternion.Dot(lastGamepadRotation, gamepadRotation) < 0)
        {
            gamepadRotation = new Quaternion(-gamepadRotation.x, -gamepadRotation.y, -gamepadRotation.z, -gamepadRotation.w);
        }

        float rotationRateX = gamepadRotation.x * maxRotationRateX;
        float rotationRateY = gamepadRotation.y * maxRotationRateY;
        float rotationRateZ = gamepadRotation.z * maxRotationRateZ;

        transform.Rotate(rotationRateX, rotationRateY, rotationRateZ);

        lastGamepadRotation = gamepadRotation;
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, 0, speed));
    }
}
