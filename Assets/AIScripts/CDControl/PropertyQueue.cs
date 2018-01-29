using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyQueue<T> where T: IComparable
{
    private ArrayList nodes = new ArrayList();
    public int Length {
        get { return this.nodes.Count; }
    }
    public bool Contains(object node) {
        return this.nodes.Contains(node);
    }
    public int Count() {
        return this.nodes.Count;
    }
    public T First() {
        if (this.nodes.Count > 0) {
            return (T)this.nodes[0];
        }
        return default(T);
    }
    public T Pop() {
        T value;
        if (Count()>0) {
            value = (T)this.nodes[0];
            nodes.RemoveAt(0);
            return value;
        }
        return default(T);
    }
    public void Push(T bts) {
        this.nodes.Add(bts);
        this.nodes.Sort();
    }
    public void Remove(T bts) {
        this.nodes.Remove(bts);
        this.nodes.Sort();
    }
    public void clear() {
        nodes.Clear();
    }
}
