using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Generic Heap class
/// </summary>
/// <typeparam name="T">Type</typeparam>
public class Heap<T> where T : IHeapItem<T>
{

    private T[] items;
    private int currentItemCount;

    /// <summary>
    /// Create a new heap of T with maxHeapSize
    /// </summary>
    /// <param name="maxHeapSize"></param>
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /// <summary>
    /// Add an item
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    /// <summary>
    /// Remove the first item
    /// </summary>
    /// <returns>The new first item</returns>
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    /// <summary>
    /// Update an item by sorting up
    /// </summary>
    /// <param name="item">Item to be updated</param>
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /// <summary>
    /// Check if items contains a certain item
    /// </summary>
    /// <param name="item">item to check</param>
    /// <returns>
    /// True: items contains item
    /// False: otherwise
    /// </returns>
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /// <summary>
    /// Sort an item going down by checking its childs
    /// </summary>
    /// <param name="item">Items childs to be sorted</param>
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            //Determine the swapindex by checking both childs
            
            //Check if child left can exists 
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                //Check if child right can exists 
                if (childIndexRight < currentItemCount)
                {
                    //Compare left and right
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        //If left precedes right in the sort order, swapIndex will change to childIndexRight
                        swapIndex = childIndexRight;
                    }
                }

                //Compare item with the item on the current swapIndex
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    //if item precedes the item on the swapIndex in the sort order, swap the items
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// Sort an item going up by checking its parents
    /// </summary>
    /// <param name="item"></param>
    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        //While items parent should be below it in the sort order swap items
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Swap 2 items
    /// </summary>
    /// <param name="itemA">First item</param>
    /// <param name="itemB">Second item</param>
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}