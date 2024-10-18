public class PlayerMovement
{
    private float _PLAYER_MOVEMENT_SPEED = 5.0f;

    private UnityEngine.Transform _transform;
    private UnityEngine.Rigidbody2D _rb;
    private UnityEngine.Animator _player_animator;

    private float _verMov = 0f;
    private float _horMov = 0f;
    private TurnSide _turnSide = default;

    private float _xScale, _yScale, _zScale;

    private bool _isLeft = false;
    public PlayerMovement(UnityEngine.Transform transform, UnityEngine.Rigidbody2D rb, UnityEngine.Animator animator)
    {
        _transform = transform;
        _rb = rb;
        _player_animator = animator;
        
        _xScale = transform.localScale.x;
        _yScale = transform.localScale.y;
        _zScale = transform.localScale.z;
    }
    public TurnSide GetMovementValues(ref float _horMov, ref float _verMov) 
    {
        _horMov = UnityEngine.Input.GetAxisRaw("Horizontal");
        _verMov = UnityEngine.Input.GetAxisRaw("Vertical");

        if (_horMov < 0)
        {
            ChangeDirection(true);
        }
        else if (_horMov > 0)
        {
            ChangeDirection(false);
        }
        
        UnityEngine.Vector2 dir = new UnityEngine.Vector2(_horMov, _verMov);
        _rb.AddForce(dir * _PLAYER_MOVEMENT_SPEED * UnityEngine.Time.deltaTime, UnityEngine.ForceMode2D.Force);
        //_rb.velocity = dir * _PLAYER_MOVEMENT_SPEED;

        return _turnSide;
    }
    public void ChangeDirection(bool isLeft = true)
    {
        _isLeft = !isLeft;

        if (isLeft)
        {
            _turnSide = TurnSide.leftSide;
            _transform.localScale = new UnityEngine.Vector3(-_xScale, _yScale, _zScale);
        }
        else
        {
            _turnSide = TurnSide.rightSide;
            _transform.localScale = new UnityEngine.Vector3(_xScale, _yScale, _zScale);
        }
    }
    public void ChangeSpeed(float speed)
        => _PLAYER_MOVEMENT_SPEED = speed;
    public void StopWalking()
    {
        _horMov = 0f;
        _verMov = 0f;
        _rb.velocity = new UnityEngine.Vector2(_horMov, _verMov);
    }
}
