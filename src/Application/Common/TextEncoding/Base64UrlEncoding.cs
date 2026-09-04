namespace EdgIQ.MurimiOS.Application.Common.TextEncoding;

public static class Base64UrlEncoding
{
    public static string Encode(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
