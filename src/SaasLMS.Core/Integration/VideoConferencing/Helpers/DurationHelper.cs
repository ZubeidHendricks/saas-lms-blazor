namespace SaasLMS.Core.Integration.VideoConferencing.Helpers;

public static class DurationHelper
{
    public static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
        {
            return $"{Math.Floor(duration.TotalDays)}d {duration.Hours}h {duration.Minutes}m";
        }
        else if (duration.TotalHours >= 1)
        {
            return $"{Math.Floor(duration.TotalHours)}h {duration.Minutes}m";
        }
        else if (duration.TotalMinutes >= 1)
        {
            return $"{duration.Minutes}m";
        }
        else
        {
            return $"{duration.Seconds}s";
        }
    }

    public static TimeSpan ParseDuration(string duration)
    {
        var parts = duration.Split(' ');
        var timeSpan = TimeSpan.Zero;

        foreach (var part in parts)
        {
            if (part.EndsWith('d'))
            {
                timeSpan = timeSpan.Add(TimeSpan.FromDays(double.Parse(part.TrimEnd('d'))));
            }
            else if (part.EndsWith('h'))
            {
                timeSpan = timeSpan.Add(TimeSpan.FromHours(double.Parse(part.TrimEnd('h'))));
            }
            else if (part.EndsWith('m'))
            {
                timeSpan = timeSpan.Add(TimeSpan.FromMinutes(double.Parse(part.TrimEnd('m'))));
            }
            else if (part.EndsWith('s'))
            {
                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(double.Parse(part.TrimEnd('s'))));
            }
        }

        return timeSpan;
    }
}