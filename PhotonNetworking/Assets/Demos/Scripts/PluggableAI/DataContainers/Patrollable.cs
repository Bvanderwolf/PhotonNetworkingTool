using UnityEngine;

public class Patrollable
{
    /// <summary>Transform of patrollable object</summary>
    public readonly Transform Transform;

    /// <summary>Is this patrollable spotted by AI?</summary>
    public bool Spotted = false;

    /// <summary>Instance ID of patrollable</summary>
    public readonly int ID;

    public Patrollable(Transform tf, int id)
    {
        Transform = tf;
        ID = id;
    }
}