using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public float Y_Pos { get => _y_pos; }
    private float _y_pos = 0f;

    private System.Collections.Generic.Dictionary<TurnSide, Vector2> _dirByTurn =
    new System.Collections.Generic.Dictionary<TurnSide, Vector2>() {
            { TurnSide.leftSide, Vector2.left },
            { TurnSide.rightSide, Vector2.right }
    };

    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private ComboSystem _comboSystem;

    [Header("Positions")]
    [SerializeField] private GameObject _hitStartingPoint;
    [SerializeField] private GameObject _hitLyingStartingPoint;
    [SerializeField] private GameObject _basePosition;

    [Header("Animators")]
    [SerializeField] private Animator _player_animator;

    private PlayerMovement _movement;
    private PlayerAnimation _playerAnimation;

    private TurnSide _turnSide = default;

    private float _horMov, _verMov;
    private void Start()
    {
        _movement = new PlayerMovement(this.transform, _rb, _player_animator);
        _playerAnimation = new PlayerAnimation(_player_animator);
    }
    private void Update()
    {
        if (!_comboSystem.IsAttacking())
        {
            _turnSide = _movement.GetMovementValues(ref _horMov, ref _verMov);
            _playerAnimation.SetAnimationByDir(new Vector2(_horMov, _verMov));
            _y_pos = _basePosition.transform.position.y;
        }
        _movement.ChangeSpeed(_speed);
        //this.gameObject.layer = LayerMask.NameToLayer("Player Lying");
    }
    public void StopMoving()
    {
        _movement.StopWalking();
        _playerAnimation.SetAnimationByDir(Vector2.zero);
    }
    public Vector2 GetCurFacingDir()
        => _dirByTurn[_turnSide];
    public void TakeDamage()
    {
        _comboSystem.enabled = false;
        StartCoroutine(WaitForAnimation());
    }
    private IEnumerator WaitForAnimation()
    {
        _player_animator.Play("player_hurt_temp");
        var state = _player_animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
        _comboSystem.enabled = true;
    }
}

public enum TurnSide
{
    rightSide = 0,
    leftSide = 1
}