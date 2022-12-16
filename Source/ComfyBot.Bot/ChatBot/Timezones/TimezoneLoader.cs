namespace ComfyBot.Bot.ChatBot.Timezones;

using System;
using System.Collections.Generic;
using System.Linq;

using Common.Http;

public class TimezoneLoader : ITimezoneLoader
{
    private static readonly List<Timezone> areas = new();

    public bool TryLoad(string zone, out Timezone result)
    {
        zone = zone.Replace(' ', '_');

        if (!areas.Any())
        {
            string[] availableZones = HttpService.Instance.GetAsync<string[]>("http://worldtimeapi.org/api/timezone").Result;

            foreach (string availableZone in availableZones)
            {
                MapAndAdd(availableZone);
            }
        }

        result = areas.FirstOrDefault(a => string.Equals(a.Region, zone, StringComparison.CurrentCultureIgnoreCase)
                                           || string.Equals(a.Location, zone, StringComparison.CurrentCultureIgnoreCase) && a.Region == string.Empty
                                           || string.Equals(a.Area, zone, StringComparison.CurrentCultureIgnoreCase) && a.Location == string.Empty);
        return result != null;
    }

    private static void MapAndAdd(string availableZone)
    {
        string[] parts = availableZone.Split('/');

        Timezone area = new Timezone
        {
            Area = parts[0],
            Location = parts.Length > 1 ? parts[1] : string.Empty,
            Region = parts.Length > 2 ? parts[2] : string.Empty
        };
        areas.Add(area);
    }
}