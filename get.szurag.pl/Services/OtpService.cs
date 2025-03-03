using OtpNet;

namespace get.szurag.pl.Services;

public class OtpService
{
    public static string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public static string GenerateOtpCode(string secretKey)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        return totp.ComputeTotp();
    }

    public static bool ValidateOtp(string secretKey, string userOtp)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        return totp.VerifyTotp(userOtp, out long timeStepMatched, new VerificationWindow(1, 1));
    }
}