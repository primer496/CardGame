using System;
using Unity.Netcode;
using System.Collections.Generic;

[Serializable]
public class Card : INetworkSerializable
{
    public int id;
    public CardSuit suit;
    public CardRank rank;
    public int weight;

    // 无参构造，供 JsonUtility 反序列化使用（最小改动）
    public Card() { }

    public Card(int id, CardSuit suit, CardRank rank, int weight)
    {
        this.id = id;
        this.suit = suit;
        this.rank = rank;
        this.weight = weight;
    }

    // Netcode 序列化支持（保持原样）
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        int s = (int)suit;
        serializer.SerializeValue(ref s);
        suit = (CardSuit)s;
        int r = (int)rank;
        serializer.SerializeValue(ref r);
        rank = (CardRank)r;
        serializer.SerializeValue(ref weight);
    }

    public override string ToString()
    {
        return $"Card(id={id}, suit={suit}, rank={rank}, weight={weight})";
    }
}

// 包装类：JsonUtility 无法直接序列化 List<T>，使用包装类最小且稳定
[Serializable]
public class CardListWrapper
{
    public List<Card> cards = new List<Card>();
}
