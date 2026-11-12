using System.Collections.Generic;
using UnityEngine;

// GENERICS: <T> means this pool works for ANY component type
// (Projectile today, Enemy or Pickup later) without rewriting it.
// Plain C# class, NOT a MonoBehaviour.
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Queue<T> available = new Queue<T>();   // ENCAPSULATION: hidden storage

    public ObjectPool(T prefab, int startSize)
    {
        this.prefab = prefab;
        for (int i = 0; i < startSize; i++)
            Release(Create());                               // pre-make some objects
    }

    private T Create()
    {
        return Object.Instantiate(prefab);
    }

    public T Get()
    {
        T item = available.Count > 0 ? available.Dequeue() : Create();
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(T item)
    {
        item.gameObject.SetActive(false);
        available.Enqueue(item);
    }
}
