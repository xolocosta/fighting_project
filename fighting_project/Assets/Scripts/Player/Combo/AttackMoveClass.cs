[System.Serializable]
public class AttackMoveClass
{
    public UnityEngine.Vector2 moveVelocity;
    public int startFrame;
    public int endFrame;

    public AttackMoveClass(UnityEngine.Vector2 moveVelocity, int startFrame, int endFrame)
    {
        this.moveVelocity = moveVelocity;
        this.startFrame = startFrame;
        this.endFrame = endFrame;
    }
    public AttackMoveClass() { }
}
