namespace ship_convenient.Helper
{
    public class Utils
    {
        public static double GetPercent(string text)
        {
            double percent = double.Parse(text) / 100;
            if (percent < 0 || percent > 1) throw new ArgumentException("Percent have from 0 to 1");
            return percent;
        }
    }
}
