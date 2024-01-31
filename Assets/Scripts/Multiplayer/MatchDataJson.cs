using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class MatchDataJson
{
    public static string PlayerVelocityAndPosition(Vector3 velocity, Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "velocity.x", velocity.x.ToString() },
            { "velocity.y", velocity.y.ToString() },
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }

    public static string ItemInit(int posIndex, int itemIndex)
    {
        var Values = new Dictionary<string, string>
        {
            { "posIndex", posIndex.ToString() },
            { "itemIndex", itemIndex.ToString() }
        };

        return JsonConvert.SerializeObject(Values);
    }

    public static string EnemySpawn(int posIndex, int modelIndex , int groupNum)
    {
        var Values = new Dictionary<string, string>
        {
            { "posIndex", posIndex.ToString() },
            { "modelIndex", modelIndex.ToString() },
            { "groupNum", groupNum.ToString() }
        };

        return JsonConvert.SerializeObject(Values);
    }

    public static string Enemystate(int StateNum)
    {
        var Values = new Dictionary<string, string>
        {
            { "stateNum", StateNum.ToString() }
        };

        return JsonConvert.SerializeObject(Values);
    }

    public static string Input(float horizontalInput, float verticalInput, bool attack, bool speedUp, bool normalSpeed)
    {
        var values = new Dictionary<string, string>
        {
            { "horizontalInput", horizontalInput.ToString() },
            { "verticalInput", verticalInput.ToString() },      
            { "attack", attack.ToString() },
            { "speedUp", speedUp.ToString() },
            { "normalSpeed", normalSpeed.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }

    public static string PlayerModelIndex(int index,int groupNum)
    {
        var values = new Dictionary<string, string>
        {
            {"index", index.ToString() },
            {"groupNum", groupNum.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }
    public static string Died(Vector3 position,int meatIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() },
            { "meatIndex" , meatIndex.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }

    public static string Respawned(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }

    public static string Score(int score)
    {
        var values = new Dictionary<string, string>
        {
            { "localScore", score.ToString() }
        };

        return JsonConvert.SerializeObject(values);
    }

    public static string Health(float health)
    {
        var values = new Dictionary<string, string>
        {
            {"playerHealth", health.ToString() }
        }; 
        return JsonConvert.SerializeObject(values);
    }
}