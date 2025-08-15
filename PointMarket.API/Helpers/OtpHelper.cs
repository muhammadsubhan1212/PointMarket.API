namespace PointMarket.API.Helpers
{
    public static class OtpHelper
    {
        public static string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static bool VerifyOtp(string inputOtp, string actualOtp)
        {
            return inputOtp == actualOtp;
        }
    }
}
