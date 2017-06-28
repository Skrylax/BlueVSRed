using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void Task();

public class TaskExecutor : MonoBehaviour
{
    public static TaskExecutor taskExecutor;
    private Queue<Task> TaskQueue = new Queue<Task>();
    private object _queueLock = new object();

    void Start() {
        taskExecutor = this;
    }

    // Update is called once per frame
    void Update()
    {
        lock (_queueLock)
        {
            if (TaskQueue.Count > 0)
            {
                //Debug.Log("TaskCount: " + TaskQueue.Count);
                TaskQueue.Dequeue()();
            }
        }
    }

    public void ScheduleTask(Task newTask)
    {
        lock (_queueLock)
        {
            if (TaskQueue.Count < 100)
                TaskQueue.Enqueue(newTask);
        }
    }
}