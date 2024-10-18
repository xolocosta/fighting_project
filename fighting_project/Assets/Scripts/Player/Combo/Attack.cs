using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Objects/Attack")]

public class Attack : ScriptableObject
{
    [SerializeField] private Attack_temp att;

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


    [ContextMenu("Set Values")]
    private void SetValues()
    {
        length = (float)(numberOfFrames + 1) / (float)framesSample;
        shortLength = (float)shortLengthFrameNumber / (float)framesSample;

        windowTime = System.Math.Clamp(windowTime, 0.0f, length * 0.5f);

        attackStartTime = (float)att.colliders[0].attackStartFrame / ((float)framesSample + 1.0f);
        attackEndTime = (float)att.colliders[att.colliders.Count - 1].attackEndFrame / ((float)framesSample + 1.0f);
    }
    [ContextMenu("Copy Values")]
    private void CopyValues()
    {
        this.colliders = att.colliders;
        this.movement = att.movement;
    }
}
