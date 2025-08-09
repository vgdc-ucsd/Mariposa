using UnityEngine;
using Discord;

public class DiscordManager : MonoBehaviour
{
    Discord.Discord discord;
    const long CLIENT_ID = 1402102402580217866;
    const string ICON_ID = "mariposa_icon";

    void Start()
    {
        discord = new Discord.Discord(CLIENT_ID, (ulong)CreateFlags.NoRequireDiscord);

        ActivityManager activityManager = discord.GetActivityManager();
        Activity activity = new Activity
        {
            Assets = {
                LargeImage = ICON_ID,
                LargeText = "Mariposa",
            },
            Timestamps = {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds(),
            }
        };

        activityManager.UpdateActivity(activity, res => {});
    }

    void Update()
    {
        discord.RunCallbacks();   
    }

    void OnDisable()
    {
        if (discord != null) discord.Dispose();
    }
}
