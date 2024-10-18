[System.Serializable]
public class TempComboClass
{
    public int index;
    public bool isPassed;
    public bool isStrict;
    public TempComboClass(int index, bool isPassed, bool isStrict)
    {
        this.index = index;
        this.isPassed = isPassed;
        this.isStrict = isStrict;
    }
    public TempComboClass() { }
}
