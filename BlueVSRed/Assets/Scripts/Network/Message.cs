using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

#region Position struct
/// <summary>
/// Struct to store the player's position in.
/// </summary>
public struct Position
{
    public float x;
    public float y;
    public float z;

    public Position(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

};
#endregion

#region InitMessage
public class InitMessage {

    public int id;
    public string name;
    public bool teamBlue;

    public InitMessage(string name) {
        this.name = name;
    }
}
#endregion

#region LobbyMessage
public class LobbyMessage {

    public bool isReady;
    public bool canStart;
    public int connections;
    public int readyConnections;
    public int countDown;


    public LobbyMessage(bool isReady) {
        this.isReady = isReady;
    }

}
#endregion

#region PlayerMessage
public class PlayerMessage {

    public int id;
    public string pname;
    public int health;
    public int mana;
    public Position position;
    public bool teamBlue;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="name">Player name</param>
    /// <param name="pos">Position of the player</param>
    public PlayerMessage(int id, string pname, int health, int mana, Vector3 pos)
    {
        this.id = id;
        this.pname = pname;
        this.health = health;
        this.mana = mana;
        this.position.x = pos.x;
        this.position.y = pos.y;
        this.position.z = pos.z;
        
    }

    /// <summary>
    /// Convert a Vector3 to a Position.
    /// </summary>
    /// <param name="position">A Vector3</param>
    /// <returns>A Position</returns>
    public static Position ToPosition(Vector3 position) {
        Position tmp;
        tmp.x = position.x;
        tmp.y = position.y;
        tmp.z = position.z;

        return tmp;
    }
}
#endregion

#region TimeMessage
public class TimeMessage {
    public int timeInSeconds;

    public TimeMessage(int timeInSeconds) { 
        this.timeInSeconds = timeInSeconds;
    }
}
#endregion

#region PathMessage
public class PathMessage {
    public int id;
    public Position[] path;

    public PathMessage(int id, Vector3[] path) {
        this.id = id;
        if (path != null)
        {
            this.path = new Position[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                this.path[i] = new Position(path[i].x, path[i].y, path[i].z);
            }
        }

    }
}
#endregion

#region SkillMessage
public class SkillMessage {
    public int id;

    public SkillMessage(int id) {
        this.id = id;
    }
}
#endregion

#region DamageMessage
public class DamageMessage {
    public string type;
    public int targetId;
    public int damage;

    public DamageMessage(int targetId, int damage) {
        this.targetId = targetId;
        this.damage = damage;
    }
}
#endregion
