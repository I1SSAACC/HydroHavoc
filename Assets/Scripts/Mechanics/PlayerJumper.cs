public class PlayerJumper
{
    private readonly PlayerMover _mover;

    public PlayerJumper(PlayerMover mover)
    {
        _mover = mover;
    }

    public void Jump() =>
        _mover.SetVerticalVelocity(PlayerParams.JumpingForce);
}