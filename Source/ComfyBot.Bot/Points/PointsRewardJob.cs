namespace ComfyBot.Bot.Points;

using System;
using System.Collections.Generic;
using System.Timers;

using ChatBot.Chatters;
using ComfyBot.Common.Initialization;

public class PointsRewardJob : IInitializerJob, ICompletableJob
{
    private readonly IChattersCache chattersCache;
    private static Timer timer;

    public PointsRewardJob(IChattersCache chattersCache)
    {
        this.chattersCache = chattersCache;
    }

    public void Execute()
    {
        timer = new Timer(10000) { Enabled = true, AutoReset = true };
        timer.Elapsed += OnElapsedTime;
        Console.WriteLine("Timer started");
    }

    private void OnElapsedTime(object sender, ElapsedEventArgs e)
    {
        IEnumerable<Chatter> enumerable = chattersCache.GetAll();

        //TODO write points to DB
        //TODO add bonus for everyone who was active 
    }

    public void Complete()
    {
        timer.Elapsed -= OnElapsedTime;
        timer.Stop();
        timer.Dispose();
    }
}