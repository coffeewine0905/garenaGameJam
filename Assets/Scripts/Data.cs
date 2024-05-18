
using System.Collections.Generic;
using System;

[Serializable]
public class PizzaData
{
    public bool IsSpicy { get; set; }
}
[Serializable]
public class CardData
{
    public int ID { get; set; }
}
[Serializable]
public class Player
{
    public int ID { get; set; }
    public int Health { get; set; } = 3;
    public List<CardData> Hand { get; set; } = new List<CardData>();
    public Action RefreshCardAction;
}