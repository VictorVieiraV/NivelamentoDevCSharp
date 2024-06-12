using Newtonsoft.Json.Linq;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoalsAsync(string team, int year)
    {
        int totalGoals = 0;
        int page = 1;
        bool hasMorePages = true;

        using (HttpClient client = new HttpClient())
        {
            while (hasMorePages)
            {
                totalGoals += await GetGoalsForTeamAsync(client, team, year, page, "team1");
                totalGoals += await GetGoalsForTeamAsync(client, team, year, page, "team2");

                var totalPages = await GetTotalPagesAsync(client, year, team, page, "team1");
                hasMorePages = page < totalPages;
                page++;
            }
        }

        return totalGoals;
    }

    private static async Task<int> GetGoalsForTeamAsync(HttpClient client, string team, int year, int page, string teamField)
    {
        string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamField}={team}&page={page}";
        var response = await client.GetStringAsync(url);
        var data = JObject.Parse(response);
        var matches = data["data"];
        return CalculateGoals(matches, $"{teamField}goals");
    }

    private static async Task<int> GetTotalPagesAsync(HttpClient client, int year, string team, int page, string teamField)
    {
        string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamField}={team}&page={page}";
        var response = await client.GetStringAsync(url);
        var data = JObject.Parse(response);
        return (int)data["total_pages"];
    }

    private static int CalculateGoals(JToken matches, string goalField)
    {
        int goals = 0;

        foreach (var match in matches)
        {
            goals += (int)match[goalField];
        }

        return goals;
    }
}