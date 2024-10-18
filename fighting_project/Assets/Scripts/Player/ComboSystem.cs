using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    private const float _LEEWAY_DURATION = 0.015f; // 20 milliseconds
    private const string _IDLE_TRIGGER_NAME = "Idle";
    private const string _ATTACKING_BOOL_NAME = "Attacking";

    private System.Collections.Generic.Dictionary<AttackType, Animator> _attackTypeToAnimator;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Player _player;

    [Header("Punch Inputs")]
    [SerializeField] private KeyCode _electro_key;
    [SerializeField] private KeyCode _gravi_key;

    [Header("Move Inputs")]
    [SerializeField] private KeyCode _moveRight_key;
    [SerializeField] private KeyCode _moveDown_key;
    [SerializeField] private KeyCode _moveLeft_key;
    [SerializeField] private KeyCode _moveUp_key;

    [Header("Combos")]
    [SerializeField] private System.Collections.Generic.List<Combo> _combos;

    [Header("Positions")]
    [SerializeField] private GameObject _hitStartingPoint;
    [SerializeField] private GameObject _hitLyingStartingPoint;

    [Header("Components")]
    [SerializeField] private Animator _player_animator;
    [SerializeField] private Animator _electro_animator;
    [SerializeField] private Animator _gravi_animator;

    private Attack _curAttack;
    private ComboInput _lastInput = new ComboInput();
    private System.Collections.Generic.Dictionary<int, int> _currentCombos  = new System.Collections.Generic.Dictionary<int, int>();
    private System.Collections.Generic.Dictionary<int, int> _add  = new System.Collections.Generic.Dictionary<int, int>();
    private System.Collections.Generic.List<int> _remove  = new System.Collections.Generic.List<int>();
    private System.Collections.Generic.List<TempComboClass> _tempCombos  = new System.Collections.Generic.List<TempComboClass>();
    private System.Collections.Generic.Queue<Attack> _attacks = new System.Collections.Generic.Queue<Attack>();

    private float _attackTimer = 0;
    private float _comboWindowTime = 0;
    private float _leeway = _LEEWAY_DURATION;
    private bool _isOnComboSeries = false;
    private bool _nonStrictRetry = false;

    private float _moveDuration = 0;

    //private float _nextInputAllowTime = 0;
    //private bool _allowNextInput = false;

    private bool _isAttacking = false;

    private void Start()
    {
        _attackTypeToAnimator = new System.Collections.Generic.Dictionary<AttackType, Animator>() 
        { 
            { AttackType.electro, _electro_animator}, 
            { AttackType.gravi, _gravi_animator } 
        };
        SetComboSeriesEnter();
        SetComboInputted();
        SetComboFinished();

    }
    private void OnDisable()
    {
        StopAllCoroutines();
        SetAttacking(false);
        ResetCombos();
    }
    public void OnAttackAnimationStart(float length)
    {
        _curAttack = _attacks.Dequeue();
        _attackTimer = length;
        //_nextInputAllowTime = length * 0.2f;
    }
    public bool IsAttacking()
        => _isAttacking;

    private void Update()
    {
        if (_isAttacking)
        {
            //if (_attackTimer > 0)
            //    _attackTimer -= Time.deltaTime;
            //else
            //{
            //    _curAttack = null;
            //    //_allowNextInput = false;
            //}


            if (_currentCombos.Count == 0)
            {
                return;
                //if (_attackTimer > _nextInputAllowTime)
                //    return false;
                //else
                //    _allowNextInput = true;
            }
        }

        if (_currentCombos.Count > 0)
        {
            _comboWindowTime += Time.deltaTime;
        }

        RemoveComboIfTimePassed();

        if (TryGetComboInput(out ComboInput input))
        {
            if (_leeway >= _LEEWAY_DURATION)
                _leeway = 0 - Time.deltaTime;
        }
        else if (_leeway < _LEEWAY_DURATION)
        {

        }
        else
        {
            return;
        }


        //Debug.Log($"Not Passed. Time passed: {Time.time} || leeway: {_leeway}");

        MergeComboInputs(input);

        _leeway += Time.deltaTime;
        if (_leeway < _LEEWAY_DURATION)
        {
            return;
        }


        //Debug.Log($"Passed. Time passed: {Time.time} || leeway: {_leeway}");

        if (_currentCombos.Count > 0)
        {
            if (!HasCorrectInput())
            {
                foreach (var item in _tempCombos)
                {
                    if (item.isPassed || item.isStrict)
                        _remove.Add(item.index);
                    else
                        _nonStrictRetry = true;

                }
                _tempCombos.Clear();
            }
        }

        AddCombosToCurrent();
        RemoveCombosFromCurrent();

        if ((!_isOnComboSeries && !_isAttacking)) // || _allowNextInput
        {
            for (int i = 0; i < _combos.Count; i++)
            {
                if (_currentCombos.ContainsKey(i)) continue;
                if (_combos[i].CanContinueCombo(_lastInput, _comboWindowTime, out bool timePassed, out int numberOfSimilarInputs, out bool isStrict))
                {
                    _add.Add(i, numberOfSimilarInputs);
                }
            }
            AddCombosToCurrent();
            RemoveCombosFromCurrent();
        }

        if (!_nonStrictRetry)
        {
            for (int i = 0; i < _currentCombos.Count; i++)
            {
                bool allowAttack = i + 1 == _currentCombos.Count;
                _combos[_currentCombos.ElementAt(i).Key].NextInput(allowAttack);
            }
        }
        else
        {
            _lastInput = new ComboInput();
            _nonStrictRetry = false;
            return;
        }

        if (!_isAttacking && _currentCombos.Count == 0)
        {
            _lastInput = new ComboInput();
        }
    }
    public void SetComboInputted()
    {
        for (int i = 0; i < _combos.Count; i++)
        {
            Combo c = _combos[i];

            c.OnInputted.AddListener(() =>
            {
                if (c.TryGetCurrentAttack(out Attack att))
                {
                    Attack(att);
                }
                _comboWindowTime = 0;
                _lastInput = new ComboInput();
            });
        }
    }
    public void SetComboFinished()
    {
        for (int i = 0; i < _combos.Count; i++)
        {
            Combo c = _combos[i];

            c.OnFinished.AddListener(() =>
            {
                ResetCombos();
            });
        }
    }
    public void SetComboSeriesEnter()
    {
        for (int i = 0; i < _combos.Count; i++)
        {
            Combo c = _combos[i];

            c.OnComboSeriesEnter.AddListener(() =>
            {
                _isOnComboSeries = true;
            });
        }
    }
    private void Attack(Attack att)
    {
        if (att != null)
        {
            _attacks.Enqueue(att);

            SetAttacking(true);

            if (!_isAttacking)
            {
                _player.StopMoving();
                StartCoroutine(WaitForAttack());
            }

        }
    }
    private IEnumerator WaitForAttack()
    {
        while (_attacks.Count > 0)
        {
            _isAttacking = true;

            Attack att = _attacks.Dequeue();
            _curAttack = att;

            StartCoroutine(MoveWithDelay(att));
            for (int i = 0; i < att.colliders.Count; i++)
            {
                StartCoroutine(CreateColliderWithDelay(att, i));
            }

            _attackTimer = att.length;
            //_nextInputAllowTime = length * 0.2f;
            float windowTime = att.windowTime;
            
            if (_attackTypeToAnimator[att.type].GetBool(att.triggerName) == false)
                _attackTypeToAnimator[att.type].SetTrigger(att.triggerName);
            Debug.Log($"Trigger \"{att.triggerName}\" has been set");

            yield return new WaitForSeconds(windowTime);
            if (_attacks.Count > 0)
                yield return new WaitForSeconds(att.shortLength - windowTime);
            else
                yield return new WaitForSeconds(att.length - windowTime);
        }
        _isAttacking = false;
        SetAttacking(false);
    }
    private IEnumerator CreateColliderWithDelay(Attack att, int i)
    {
        AttackColliderClass curCollider = att.colliders[i];
        float startTime = (float)curCollider.attackStartFrame / att.framesSample;
        float endTime = (float)curCollider.attackEndFrame / att.framesSample;
        float duration = endTime - startTime;
        yield return new WaitForSeconds(startTime);

        GameObject parent;

        if (att.colliders[i].canHitLying)
            parent = Instantiate(_hitLyingStartingPoint, this.transform);
        else
            parent = Instantiate(_hitStartingPoint, this.transform);

        // Object Pulling

        CapsuleCollider2D collider = parent.AddComponent<CapsuleCollider2D>();
        collider.isTrigger = true;
        collider.direction = CapsuleDirection2D.Horizontal;
        collider.offset = curCollider.attackOffset;
        collider.size = new Vector2(curCollider.attackWidth, curCollider.attackHeight);

        yield return new WaitForSeconds(duration);
        Destroy(parent);
    }
    private IEnumerator MoveWithDelay(Attack att)
    {
        if (att.movement.moveVelocity != Vector2.zero)
        {
            float startTime = (float)att.movement.startFrame / ((float)att.framesSample + 1.0f);
            float endTime = (float)att.movement.endFrame / ((float)att.framesSample + 1.0f);
            _moveDuration = endTime - startTime;
            Debug.Log($"startTime: {startTime} || endTime: {endTime} || duration: {_moveDuration}");
            yield return new WaitForSeconds(startTime);

            //StartCoroutine(ApplyDirectionOverTime());
            //yield return new WaitUntil(() => _moveDuration <= 0);

            //Action action = () =>
            //{
            //    _rb.AddForce(att.movement.moveVelocity * new Vector2(_player.GetCurFacingDir().x, 1.0f) * Time.deltaTime);
            //};

            //yield return new WaitUntil(() => (_moveDuration -= Time.deltaTime) <= 0)
            //{

            //};

            yield return new WaitUntil(() => IsDurationReached(att));
            //yield return new WaitForSeconds(duration);

            //_rb.velocity = Vector2.zero;
        }
    }

    private bool IsDurationReached(Attack att)
    {
        _rb.AddForce(att.movement.moveVelocity * new Vector2(_player.GetCurFacingDir().x, 1.0f) * Time.deltaTime);
        //Debug.Log($"Before : {(float)_moveDuration} => " + Time.time);
        _moveDuration -= (float)Time.deltaTime;
        //Debug.Log("After : " + (float)_moveDuration + " => " + Time.time);
        return _moveDuration <= 0;
    }


    public void SetAttacking(bool isAttacking)
    {
        _player_animator.SetBool(_ATTACKING_BOOL_NAME, isAttacking);
        _electro_animator.SetBool(_ATTACKING_BOOL_NAME, isAttacking);
        _gravi_animator.SetBool(_ATTACKING_BOOL_NAME, isAttacking);
    }
    public void ResetCombos()
    {
        _lastInput = new ComboInput();
        _isOnComboSeries = false;
        _nonStrictRetry = false;
        _comboWindowTime = 0 - Time.deltaTime;
        _leeway = _LEEWAY_DURATION;
        _currentCombos.Clear();
        _add.Clear();
        _remove.Clear();
        for (int i = 0; i < _combos.Count; i++)
        {
            _combos[i].ResetCombo();
        }
        Debug.Log("Combo Reset");
    }
    private bool TryGetComboInput(out ComboInput c)
    {
        c = new ComboInput();

        if (Input.GetKeyDown(_electro_key))
            c.AddAttack(AttackType.electro);
        if (Input.GetKeyDown(_gravi_key))
            c.AddAttack(AttackType.gravi);

        if (Input.GetKeyDown(_moveRight_key))
            c.AddMove(MoveType.moveRight);
        if (Input.GetKeyDown(_moveDown_key))
            c.AddMove(MoveType.moveDown);
        if (Input.GetKeyDown(_moveLeft_key))
            c.AddMove(MoveType.moveLeft);
        if (Input.GetKeyDown(_moveUp_key))
            c.AddMove(MoveType.moveUp);

        if (Input.GetKey(_moveRight_key))
            c.AddHoldMove(HoldMoveType.holdMoveRight);
        if (Input.GetKey(_moveDown_key))
            c.AddHoldMove(HoldMoveType.holdMoveDown);
        if (Input.GetKey(_moveLeft_key))
            c.AddHoldMove(HoldMoveType.holdMoveLeft);
        if (Input.GetKey(_moveUp_key))
            c.AddHoldMove(HoldMoveType.holdMoveUp);

        //c.AddAttack(AttackType.electro);

        if (c.HasAttackOrMoveDownInputs())
            return true;
        else
            return false;
    }
    private void MergeComboInputs(ComboInput test)
    {
        bool attacks = test.attacks.typesLists.Count > 0;
        bool moves = test.moves.typesLists.Count > 0;
        bool holdMoves = test.holdMoves.typesLists.Count > 0;

        if (attacks)
            for (int i = 0; i < test.attacks.typesLists[0].types.Count; i++)
                _lastInput.AddAttack(test.attacks.typesLists[0].types[i]);
        if (moves)
            for (int i = 0; i < test.moves.typesLists[0].types.Count; i++)
                _lastInput.AddMove(test.moves.typesLists[0].types[i]);
        if (holdMoves)
            for (int i = 0; i < test.holdMoves.typesLists[0].types.Count; i++)
                _lastInput.AddHoldMove(test.holdMoves.typesLists[0].types[i]);
    }
    private bool HasCorrectInput()
    {
        bool hasCorrectInput = false;
        System.Collections.Generic.List<int> tempRemove = new System.Collections.Generic.List<int>();
        System.Collections.Generic.Dictionary<int, int> tempAdd = new System.Collections.Generic.Dictionary<int, int>();

        foreach (var item in _currentCombos)
        {
            Combo c = _combos[item.Key];
            if (c.CanContinueCombo(_lastInput, _comboWindowTime, out bool timePassed, out int numberOfSimilarInputs, out bool isStrict))
            {
                hasCorrectInput = true;
                tempAdd.Add(item.Key, numberOfSimilarInputs);
            }
            else
            {
                tempRemove.Add(item.Key);
                _tempCombos.Add(new TempComboClass(item.Key, timePassed, isStrict));
            }
        }


        if (hasCorrectInput)
        {
            foreach (var item in tempRemove)
                _remove.Add(item);
            foreach (var item in tempAdd)
                _add.Add(item.Key, item.Value);
            RemoveCombosFromCurrent();
            _tempCombos.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }
    private void RemoveComboIfTimePassed()
    {
        if (_currentCombos.Count > 0)
        {
            foreach (var item in _currentCombos)
            {
                if (_comboWindowTime > _combos[item.Key].GetCurrentInputWindowTime())
                {
                    _remove.Add(item.Key);
                    _combos[item.Key].ResetCombo();
                }
            }
            RemoveCombosFromCurrent();
        }
    }
    private void RemoveCombosFromCurrent()
    {
        if (_remove.Count == 0)
            return;
        
        foreach (int i in _remove)
        {
            _currentCombos.Remove(i);
            _combos[i].ResetCombo();
        }
        _remove.Clear();

        if (_currentCombos.Count == 0)
            ResetCombos();
    }
    private void AddCombosToCurrent()
    {
        if (_add.Count == 0)
            return;
        
        System.Collections.Generic.Dictionary<int, int> temp = new System.Collections.Generic.Dictionary<int, int>();
        foreach (var item in _add)
        {
            if (_combos[item.Key].DoesCurrentInputAttack())
                temp.Add(item.Key, item.Value);
            else
                _remove.Add(item.Key);
        }

        if (temp.Count > 0)
            DeterminePriorityCombos(temp, true);
        else
        {
            _remove.Clear();
            DeterminePriorityCombos(_add, false);
        }

        _add.Clear();
    }
    private void DeterminePriorityCombos(System.Collections.Generic.Dictionary<int, int> array, bool hasHit = false)
    {
        int highestComboInputs = 0;
        int highestSimilarInputs = 0;

        if (hasHit)
        {
            GetHighestComboInputs();

            GetHighestSimilarInputs(hasHit);

            foreach (var item in array)
            {
                if (_combos[item.Key].GetCurrentInput() == highestComboInputs)
                {
                    AddRemoveBySimilarInputs(item.Key, item.Value);
                }
                else _remove.Add(item.Key);
            }
        }
        else
        {
            GetHighestSimilarInputs(hasHit);

            foreach (var item in array)
            {
                AddRemoveBySimilarInputs(item.Key, item.Value);
            }
        }


        void GetHighestComboInputs()
        {
            foreach (var item in array)
            {
                if (_combos[item.Key].GetCurrentInput() > highestComboInputs)
                    highestComboInputs = _combos[item.Key].GetCurrentInput();
            }
        }
        void GetHighestSimilarInputs(bool checkComboInputsNumber = false)
        {
            foreach (var item in array)
            {
                if (checkComboInputsNumber)
                {
                    if (_combos[item.Key].GetCurrentInput() != highestComboInputs)
                        continue;
                }
                if (item.Value > highestSimilarInputs)
                    highestSimilarInputs = item.Value;
            }
        }
        void AddRemoveBySimilarInputs(int Key, int Value)
        {
            if (Value == highestSimilarInputs)
                _currentCombos.TryAdd(Key, Value);
            else
                _remove.Add(Key);
        }
    }
    [ContextMenu("Reset Combos")]
    private void ResetCombosContextMenu()
    {
        ResetCombos();
    }
}
