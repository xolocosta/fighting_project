public class PlayerAnimation
{
    private const string _PLAYER_IDLE_BOOL_NAME = "Idle";
    private const string _PLAYER_WALKING_BOOL_NAME = "Walking";

    private UnityEngine.Animator _player_animator;

    public PlayerAnimation(UnityEngine.Animator player_animator)
    {
        _player_animator = player_animator;
    }
    public void SetAnimationByDir(UnityEngine.Vector2 dir)
    {
        if (dir.x > 0 || dir.x < 0)
        {
            _player_animator.SetBool(_PLAYER_IDLE_BOOL_NAME, false);
            _player_animator.SetBool(_PLAYER_WALKING_BOOL_NAME, true);
        }
        else
        {
            _player_animator.SetBool(_PLAYER_IDLE_BOOL_NAME, true);
            _player_animator.SetBool(_PLAYER_WALKING_BOOL_NAME, false);
        }
    }
}
