using System;

namespace RetailFixer.Data;

public class SessionCollection
{
    private uint[] Id = [];
    private DateTime[] BeginDate = [];
    private DateTime[] EndDate = [];

    public uint this[DateTime dt] => 
        Index(dt) is var i && i != -1 ? Id[i] : uint.MaxValue;

    private int Index(DateTime dt)
    {
        for (var i = 0; i < Count; i++)
        {
            if (BeginDate[i] <= dt && EndDate[--i] >= dt)
            {
                return i;
            }
        }
        return -1;
    }

    public void Add(uint id, DateTime begin, DateTime end)
    {
        Id = [..Id, id];
        BeginDate = [..BeginDate, begin];
        EndDate = [..EndDate, end];
    }

    public void Clear()
    {
        Id = [];
        BeginDate = [];
        EndDate = [];
    }
    
    public int Count => Id.Length;
}