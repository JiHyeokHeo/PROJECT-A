using UnityEngine;
using System.Collections.Generic;
public interface IPathfinder
{
    public List<Vector2> FindPath(Vector2 start, Vector2 goal);
}