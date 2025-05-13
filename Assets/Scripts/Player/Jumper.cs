public class Jumper
{
    private readonly PlayerMover _mover;

    public Jumper(PlayerMover mover)
    {
        _mover = mover;
    }

    public void Jump() =>
        _mover.SetVerticalVelocity(PlayerParams.JumpingForce);
}