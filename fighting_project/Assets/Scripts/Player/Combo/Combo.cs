using UnityEngine;

public enum AttackType { electro = 0, gravi = 1 };
public enum MoveType { moveRight = 0, moveDown = 1, moveLeft = 2, moveUp = 3 };
public enum HoldMoveType { holdMoveRight = 0, holdMoveDown = 1, holdMoveLeft = 2, holdMoveUp = 3 };

[CreateAssetMenu(fileName = "Combo", menuName = "Scriptable Objects/Combo")]
public class Combo : ScriptableObject
{
    public int comboSeriesEnterIndex;
    // List<List<T>> not seen in Inspector
    public System.Collections.Generic.List<ComboInputList> inputsList;
    public System.Collections.Generic.List<Attack> attacks;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnInputted;
    public UnityEngine.Events.UnityEvent OnFinished;
    public UnityEngine.Events.UnityEvent OnComboSeriesEnter;

    private int _curAttack = 0;
    private int _curInput = 0;
    private int _absolutInput = 0;

    public bool CanContinueCombo(ComboInput test, float timePassedFromLastInput, out bool timePassed, out int numberOfSimilarInputs, out bool isStrict)
    {
        numberOfSimilarInputs = 0;
        isStrict = _curAttack < comboSeriesEnterIndex;
        timePassed = timePassedFromLastInput > attacks[_curAttack].windowTime;

        if (timePassed)
            return false;

        bool isFirstInput = _absolutInput == 0;

        if (inputsList[_curAttack].comboInputs[_curInput].IsSame(test, isFirstInput, out numberOfSimilarInputs, isStrict))
        {
            return true;
        }
        else
        {
            if (isStrict)
            {
                ResetCombo();
                return false;
            }
            else
            {
                return false;
            }
        }
    }
    public void NextInput(bool allowAttack)
    {
        if (allowAttack)
            OnInputted?.Invoke();

        _absolutInput++;
        _curInput++;

        if (_curInput == inputsList[_curAttack].comboInputs.Count)
        {
            _curAttack++;
            _curInput = 0;

            if (_curAttack == comboSeriesEnterIndex)
                OnComboSeriesEnter?.Invoke();
            if (_curAttack == inputsList.Count)
                OnFinished?.Invoke();
        }
    }
    public bool TryGetCurrentAttack(out Attack att)
    {
        att = ScriptableObject.CreateInstance<Attack>();
        if (_curInput + 1 == inputsList[_curAttack].comboInputs.Count)
        {
            att = attacks[_curAttack];
            return true;
        }
        else
            return false;
    }
    public float GetCurrentInputWindowTime()
        => attacks[_curAttack].windowTime;
    public int GetCurrentInput()
        => _absolutInput;
    public bool DoesCurrentInputAttack()
        => _curInput + 1 == inputsList[_curAttack].comboInputs.Count;
    public bool IsFirstInput()
        => _absolutInput == 0;
    public void ResetCombo()
    {
        _curAttack = 0;
        _curInput = 0;
        _absolutInput = 0;
    }
}

