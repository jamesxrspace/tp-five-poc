namespace TPFive.Game.Record
{
    public class Base64
    {
        public static string Encode(string txt)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(txt);
            return System.Convert.ToBase64String(textBytes);
        }

        public static string Decode(string base64)
        {
            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
    }
}
