using UnityEngine;
using System.Collections.Generic;
using System;

public class Character
{
    long id;
    public string name;
    public DateTime createdDate;

    public Character(long id, string name, DateTime createdDate)
    {
        this.id = id;
        this.name = name;
        this.createdDate = createdDate;
    }
}
