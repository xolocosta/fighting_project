[System.Serializable]
public class AttackColliderClass
{
    public bool canHitLying;
    public UnityEngine.Vector2 attackForce;
    public UnityEngine.Vector2 attackOffset;
    public int attackStartFrame;
    public int attackEndFrame;
    public float attackHeight;
    public float attackWidth;

    public AttackColliderClass(bool canHitLying, UnityEngine.Vector2 force, UnityEngine.Vector2 offset, int startFrame, int endFrame, float height, float width) 
    {
        this.canHitLying = canHitLying;
        this.attackForce = force;
        this.attackOffset = offset;
        this.attackStartFrame = startFrame;
        this.attackEndFrame = endFrame;
        this.attackHeight = height;
        this.attackWidth = width;
    }
    public AttackColliderClass() 
    {
        canHitLying = default;
        attackForce = default;
        attackOffset = default;
        attackStartFrame = default;
        attackEndFrame = default;
        attackHeight = default;
        attackWidth = default;
    }
}
