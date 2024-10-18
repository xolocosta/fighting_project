[System.Serializable]
public class EnumTypeList<Enum>
{
    public System.Collections.Generic.List<EnumTypeChildList<Enum>> typesLists;

    public EnumTypeList(System.Collections.Generic.List<EnumTypeChildList<Enum>> types) 
    {
        typesLists = types;
        // types.GetType().GetGenericArguments()[1]
        // var type = abc.GetType().GetTypeInfo().GenericTypeArguments[1];
        // type type = abc.GetType().GetProperty("Item").PropertyType;
    }
    public EnumTypeList()
    {
        typesLists = new System.Collections.Generic.List<EnumTypeChildList<Enum>>();
    }
    ~EnumTypeList()
    {
        typesLists = null;
    }

    public bool CheckTypes(EnumTypeList<Enum> inputted, ref int numberOfSimilarInputs, bool isStrict = false)
    {
        try
        {
            if (inputted.typesLists.Count == 0)
                return false;

            bool res = false;
            int similarInputs = 0;

            // extra loop if inputted.typesLists.Count > 1
            for (int i = 0; i < typesLists.Count; i++)
            {
                if (CheckTypesChild(typesLists[i].types, inputted.typesLists[0].types, out int x, isStrict))
                {
                    if (x > similarInputs)
                        similarInputs = x;
                    res = true;
                }
            }
            numberOfSimilarInputs += similarInputs;
            return res;

        }
        catch (System.ArrayTypeMismatchException ex)
        {
            throw new System.ArgumentException(ex.ToString());
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public bool CheckTypesChild(System.Collections.Generic.List<Enum> original, System.Collections.Generic.List<Enum> inputted, out int similarInputs, bool isStrict = false)
    {
        try
        {
            similarInputs = 0;

            if ((original.Count == inputted.Count) && original.Count == 0)
                return true;
            else if (original.Count == 0 ||  inputted.Count == 0)
                return false;

            bool res = true;

            if (isStrict)
            {
                if (inputted.Count != original.Count)
                    res = false;
            }
            else
            {
                if (inputted.Count < original.Count)
                    res = false;
            }


            if (res)
            {
                if (!CompareEnums(original, inputted))
                    res = false;
            }

            if (res) similarInputs = original.Count;

            return res;

        }
        catch (System.ArrayTypeMismatchException ex)
        {
            throw new System.ArgumentException(ex.ToString());
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    public void AddType(Enum type)
    {
        try
        {
            if (typesLists.Count == 0)
                typesLists.Add(new EnumTypeChildList<Enum>());

            if (!typesLists[0].types.Contains(type))
                typesLists[0].types.Add(type);
        }
        catch (System.ArrayTypeMismatchException ex)
        {
            throw new System.ArgumentException(ex.ToString());
        }
    }
    private bool CompareEnums(System.Collections.Generic.List<Enum> original, System.Collections.Generic.List<Enum> inputted)
    {
        bool res = true;
        int orig = 0;
        int input = 0;

        foreach (var x in original)
        {
            bool contains = false;
            foreach (var y in inputted)
            {
                orig = System.Convert.ToInt32(x);
                input = System.Convert.ToInt32(y);
                //UnityEngine.Debug.Log($"original enum int: {orig}, inputted enum int: {input}");
                if (orig == input)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                res = contains;
                break;
            }
        }
        return res;
    }
}
[System.Serializable]
public class EnumTypeChildList<Enum>
{
    public System.Collections.Generic.List<Enum> types;
    public EnumTypeChildList()
    {
        types = new System.Collections.Generic.List<Enum>();
    }
    ~EnumTypeChildList()
    {
        types = null;
    }
}