[System.Serializable]
public class ComboInput
{
    public EnumTypeList<AttackType> attacks;
    public EnumTypeList<MoveType> moves;
    public EnumTypeList<HoldMoveType> holdMoves;

    public ComboInput(System.Collections.Generic.List<EnumTypeChildList<AttackType>> attacks, 
        System.Collections.Generic.List<EnumTypeChildList<MoveType>> moves, 
        System.Collections.Generic.List<EnumTypeChildList<HoldMoveType>> holdMoves)
    {       
        this.attacks = new EnumTypeList<AttackType>(attacks);
        this.moves = new EnumTypeList<MoveType>(moves);
        this.holdMoves = new EnumTypeList<HoldMoveType>(holdMoves);
    }
    public ComboInput()
    {
        attacks = new EnumTypeList<AttackType>(new System.Collections.Generic.List<EnumTypeChildList<AttackType>>());
        moves = new EnumTypeList<MoveType>(new System.Collections.Generic.List<EnumTypeChildList<MoveType>>());
        holdMoves = new EnumTypeList<HoldMoveType>(new System.Collections.Generic.List<EnumTypeChildList<HoldMoveType>>());
    }
    ~ComboInput()
    {
        attacks = null;
        moves = null;
        holdMoves = null;
    }
    public bool IsSame(ComboInput test, bool isFirst,out int numberOfSimilarInputs, bool isStrict = false)
    {
        numberOfSimilarInputs = 0;
        bool attacks = true;
        bool moves = true;
        bool holdMoves = true;

        bool hasAttacks = this.attacks.typesLists.Count != 0;
        bool hasMoves = this.moves.typesLists.Count != 0;
        bool hasHoldMoves = this.holdMoves.typesLists.Count != 0;

        if (isStrict)
        {
            if (hasAttacks && !hasMoves && !hasHoldMoves)
            {
                attacks = this.attacks.CheckTypes(test.attacks, ref numberOfSimilarInputs, isStrict);
                if (!isFirst)
                    moves = this.moves.CheckTypes(test.moves, ref numberOfSimilarInputs, isStrict);
            }
            else if (hasAttacks && hasMoves && !hasHoldMoves)
            {
                attacks = this.attacks.CheckTypes(test.attacks, ref numberOfSimilarInputs, isStrict);
                moves = this.moves.CheckTypes(test.moves, ref numberOfSimilarInputs, isStrict);
            }
            else if (!hasAttacks && hasMoves && !hasHoldMoves)
            {
                attacks = this.attacks.CheckTypes(test.attacks, ref numberOfSimilarInputs, isStrict);
                moves = this.moves.CheckTypes(test.moves, ref numberOfSimilarInputs, isStrict);
            }
        }
        else
        {
            if (hasAttacks)
                attacks = this.attacks.CheckTypes(test.attacks, ref numberOfSimilarInputs, isStrict);
            if (hasMoves)
                moves = this.moves.CheckTypes(test.moves, ref numberOfSimilarInputs, isStrict);
            if (hasHoldMoves)
                holdMoves = this.holdMoves.CheckTypes(test.holdMoves, ref numberOfSimilarInputs, isStrict);
        }

        return attacks && moves && holdMoves;
    }

    // if ComboInputList is empty, create one 
    public void AddAttack(AttackType attack)
        => attacks.AddType(attack);
    public void AddMove(MoveType move)
        => moves.AddType(move);
    public void AddHoldMove(HoldMoveType move)
        => holdMoves.AddType(move);
    public bool HasAttackOrMoveDownInputs()
    {
        if (attacks.typesLists.Count != 0)
        {
            if (attacks.typesLists[0].types.Count != 0)
            {
                return true;
            }
        }
        if (moves.typesLists.Count != 0)
        {
            if (moves.typesLists[0].types.Count != 0)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class ComboInputList
{
    public System.Collections.Generic.List<ComboInput> comboInputs;
    public ComboInputList()
    {
        comboInputs = new System.Collections.Generic.List<ComboInput>();
    }
    ~ComboInputList()
    {
        comboInputs = null;
    }
}