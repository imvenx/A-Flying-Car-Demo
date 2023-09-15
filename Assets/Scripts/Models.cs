using ArcanepadSDK.Models;

public class ChangeSpeedEvent : ArcaneBaseEvent
{
    public int speed;
    public ChangeSpeedEvent(int speed) : base(GameEvent.ChangeSpeed)
    {
        this.speed = speed;
    }
}

public class EnableDriveEvent : ArcaneBaseEvent { public EnableDriveEvent(int speed) : base(GameEvent.EnableDrive) { } }
public class DisableDriveEvent : ArcaneBaseEvent { public DisableDriveEvent(int speed) : base(GameEvent.DisableDrive) { } }

public static class GameEvent
{
    public static string ChangeSpeed = "ChangeSpeed";
    public static string EnableDrive = "EnableDrive";
    public static string DisableDrive = "DisableDrive";
}