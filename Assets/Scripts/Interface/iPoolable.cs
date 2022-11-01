using System;
using UnityEngine;

public interface iPoolable
{
    void GetFromPool();
    void ReturnToPool();
    void DestroyPoolObject();
}
