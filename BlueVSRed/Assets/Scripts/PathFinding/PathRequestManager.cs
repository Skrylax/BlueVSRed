using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    private PathRequest currentPathRequest;
    private Pathfinding pathfinding;
    private bool isProcessingPath;


    void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        currentPathRequest = new PathRequest(pathStart, pathEnd, callback);
        pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
    }

    

    /// <summary>
    /// Call when path is finished, so we can start working on the next item in the queue
    /// </summary>
    /// <param name="path">Which path finished</param>
    /// <param name="success">is the path successful</param>
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);

    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }

}
