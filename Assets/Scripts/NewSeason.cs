using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class NewSeason : MonoBehaviour
{
    private readonly string path = Application.streamingAssetsPath + "/SeasonRewards.json";//奖励信息json位置

    /// <summary>
    /// 写新赛季奖励信息的方法
    /// </summary>
    public void CreatNewSeason()
    {
        JSONNode seasonRewards = new JSONObject();
        var rewardsArray = new JSONArray();
        for (var i = 4000; i < 6000; i += 200)
        {
            rewardsArray.Add(i % 1000 == 0 ? 0 : 200);
        }

        seasonRewards["rewards"] = rewardsArray;

        var json = seasonRewards.ToString();
        System.IO.File.WriteAllText(path, json);
    }
}