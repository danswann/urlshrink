namespace urlshrink.Data;

public record OriginalUrl
{
    public string url {get; set;}

    public OriginalUrl(string url)
    {
        this.url = url;
    }
}
public record ShrunkUrl
{
    public string urlShrunk {get; set;}
    public ShrunkUrl(string shorter)
    {
        this.urlShrunk = shorter;
    }
}
public class UrlData
{
    // Long-to-short lookup
    private static Dictionary<string, string> _LTS = new Dictionary<string, string>();
    // Short-to-long lookup
    private static Dictionary<string, string> _STL = new Dictionary<string, string>();


    private static string ShrinkURL(string original)
    {
        // Check if there is already a short version of this url
        if(_LTS.ContainsKey(original)) return _LTS[original];

        var maxAttempts = 50;
        var alphabet = "abcdefghijklmnopqrstuvwxyz1234567890";
        var length = 5;
        var rng = new Random();
        string attempt = "";
        var found = false;
        for(var i = 0; i < maxAttempts; i++)
        {
            // Generate random string
            attempt = new string(Enumerable.Repeat(alphabet, length).Select(s => s[rng.Next(s.Length)]).ToArray());
            // Confirm it doesn't already exist
            if(!_STL.ContainsKey(attempt))
            {
                found = true;
                break;
            }
        }
        if(found) return attempt;
        else throw new TimeoutException();
    }
    public static string AddUrl(string original)
    {
        var shorter = UrlData.ShrinkURL(original);
        _LTS.TryAdd(original, shorter);
        _STL[shorter] = original;
        return shorter;
    }

    public static string? GetOriginal(string shorter)
    {
        if(_STL.ContainsKey(shorter)) return _STL[shorter];
        else return null;
    }
}