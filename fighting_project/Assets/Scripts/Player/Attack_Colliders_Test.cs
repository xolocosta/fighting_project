using System.Collections;
using UnityEngine;

public class Attack_Colliders_Test : MonoBehaviour
{
    public float Y_Pos { get => _y_pos; }
    private float _y_pos = 0f;

    private System.Collections.Generic.Dictionary<TurnSide, Vector2> _dirByTurn =
    new System.Collections.Generic.Dictionary<TurnSide, Vector2>() {
            { TurnSide.leftSide, Vector2.left },
            { TurnSide.rightSide, Vector2.right }
    };

    //[SerializeField] private ComboSystem _comboSystem;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Animators")]
    [SerializeField] private Animator _player_animator;
    [SerializeField] private Animator _electro_animator;
    [SerializeField] private Animator _gravi_animator;

    [Header("Attacks")]
    [SerializeField] private Attack_temp _electro_attack_1;
    [SerializeField] private Attack_temp _electro_attack_2;
    [SerializeField] private Attack_temp _electro_attack_3;
    [SerializeField] private Attack_temp _gravi_attack_1;
    [SerializeField] private Attack_temp _gravi_attack_2;
    [SerializeField] private Attack_temp _gravi_attack_3;

    [Header("Positions")]
    [SerializeField] private GameObject _hitStartingPoint;
    [SerializeField] private GameObject _hitLyingStartingPoint;

    [Header("Positions")]
    //[SerializeField] private Transform _attackStartingPosition;
    [SerializeField] private GameObject _basePosition;


    private PlayerMovement _movement;
    private PlayerAnimation _playerAnimation;

    private TurnSide _turnSide = default;
    private Attack_temp _curAttack = null;

    private float _horMov, _verMov;

    private CapsuleCollider2D _curCollider = null;
    void Start()
    {
        _movement = new PlayerMovement(this.transform, _rb, _player_animator);
        _playerAnimation = new PlayerAnimation(_player_animator);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (_curCollider == null)
            {
                _curCollider = gameObject.AddComponent<CapsuleCollider2D>();
                _curCollider.isTrigger = true;
                _curCollider.direction = CapsuleDirection2D.Horizontal;
                _curCollider.offset = new Vector2(5.0f, 0.0f);
                _curCollider.size = new Vector2(18.0f, 6.0f);
            }
            else
            {
                Destroy(_curCollider);
                _curCollider = null;
            }
        }

        if (_curAttack == null)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _electro_animator.Play("attack 1");
                Attack(_electro_attack_1);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                _electro_animator.Play("attack 2");
                Attack(_electro_attack_2);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                _electro_animator.Play("attack 3");
                Attack(_electro_attack_3);
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                _gravi_animator.Play("gravi_attack_1");
                Attack(_gravi_attack_1);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                _gravi_animator.Play("gravi_attack_2");
                Attack(_gravi_attack_2);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                _gravi_animator.Play("gravi_attack_3");
                Attack(_gravi_attack_3);
            }

            _movement.StopWalking();
            _playerAnimation.SetAnimationByDir(Vector2.zero);
        }

        if (_curAttack == null)
        {
            _turnSide = _movement.GetMovementValues(ref _horMov, ref _verMov);
            _playerAnimation.SetAnimationByDir(new Vector2(_horMov, _verMov));

            _y_pos = _basePosition.transform.position.y;
        }
    }
    private void Attack(Attack_temp att)
    {
        _curAttack = att;
        StartCoroutine(WaitForAttack(att));
        StartCoroutine(MoveWithDelay(att));
        for (int i = 0; i < att.colliders.Count; i++)
        {
            StartCoroutine(CreateColliderWithDelay(att, i));
        }
    }
    private IEnumerator CreateColliderWithDelay(Attack_temp att, int i)
    {
        AttackColliderClass curCollider = att.colliders[i];
        float startTime = (float)curCollider.attackStartFrame / att.framesSample;
        float endTime = (float)curCollider.attackEndFrame / att.framesSample;
        float duration =  endTime - startTime;
        yield return new WaitForSeconds(startTime);

        GameObject parent;

        if (att.colliders[i].canHitLying)
            parent = Instantiate(_hitLyingStartingPoint, this.transform);
        else
            parent = Instantiate(_hitStartingPoint, this.transform);

        CapsuleCollider2D collider = parent.AddComponent<CapsuleCollider2D>();
        collider.isTrigger = true;
        collider.direction = CapsuleDirection2D.Horizontal;
        collider.offset = curCollider.attackOffset;
        collider.size = new Vector2(curCollider.attackWidth, curCollider.attackHeight);

        yield return new WaitForSeconds(duration);
        Destroy(parent);
    }
    private IEnumerator MoveWithDelay(Attack_temp att)
    {
        if (att.movement.moveVelocity != Vector2.zero)
        {
            float startTime = (float)att.movement.startFrame / (float)att.framesSample;
            float endTime = (float)att.movement.endFrame - (float)att.framesSample;
            float duration = endTime - startTime;
            yield return new WaitForSeconds(startTime);
            _rb.velocity = att.movement.moveVelocity;
            yield return new WaitForSeconds(duration);
            _rb.velocity = Vector2.zero;
        }

    }
    private IEnumerator WaitForAttack(Attack_temp att)
    {
        yield return new WaitForSeconds(att.length);
        _curAttack = null;
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    for (int i = 0; i < 8; i++)
    //    {
    //        //Gizmos.DrawSphere(_attackStartingPosition.position + (Vector3)_dirByTurn[_turnSide] * i, 3.0f);
    //        Gizmos.DrawWireSphere((Vector2)_attackStartingPosition.position + _dirByTurn[_turnSide] * i, 2.5f);
    //    }
    //}
}
