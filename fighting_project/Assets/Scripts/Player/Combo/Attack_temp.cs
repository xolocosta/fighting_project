using UnityEngine;

[CreateAssetMenu(fileName = "Attack_temp", menuName = "Scriptable Objects/Attack_temp")]

public class Attack_temp : ScriptableObject
{
    [SerializeField] private Attack attack;

    public AttackType type;
    public string triggerName;
    public int framesSample;

    public int numberOfFrames;
    public float length;

    public int shortLengthFrameNumber;
    public float shortLength;

    public float attackStartTime;
    public float attackEndTime;

    public System.Collections.Generic.List<AttackColliderClass> colliders;
    public AttackMoveClass movement;

    [Header("Window Time should not be more than \"animation length\" * 0.5")]
    public float windowTime;
    // windowTime must not be more than half of the length

    [ContextMenu("Copy Values")]
    private void CopyValues()
    {
        this.type = attack.type;
        this.triggerName = attack.triggerName;
        this.framesSample = attack.framesSample;
        
        this.numberOfFrames = attack.numberOfFrames;
        this.length = attack.length;
        
        this.shortLengthFrameNumber = attack.shortLengthFrameNumber;
        this.shortLength = attack.shortLength;

        this.windowTime = attack.windowTime;
    }
}
