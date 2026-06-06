using System.Text.Json;

namespace alloy13dss.Business.Forms;

public class RecaptchaV3Verifier(HttpClient httpClient) : IRecaptchaV3Verifier
{
    public async Task<bool> VerifyAsync(string secretKey, string token, string expectedAction, double scoreThreshold)
    {
        if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = secretKey,
            ["response"] = token
        });

        using var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<RecaptchaV3Response>(stream);

        return result?.Success == true
            && result.Score >= scoreThreshold
            && (string.IsNullOrWhiteSpace(expectedAction) || string.Equals(result.Action, expectedAction, StringComparison.Ordinal));
    }

    private sealed class RecaptchaV3Response
    {
        public bool Success { get; set; }

        public double Score { get; set; }

        public string Action { get; set; }
    }
}
