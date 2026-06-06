namespace alloy13dss.Business.Forms;

public interface IRecaptchaV3Verifier
{
    Task<bool> VerifyAsync(string secretKey, string token, string expectedAction, double scoreThreshold);
}
