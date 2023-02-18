namespace ship_convenient.Model.MapboxModel
{
    public class DirectionApiModel
    {
        public CoordinateApp From { get; set; } = new CoordinateApp();
        public List<CoordinateApp> To { get; set; } = new List<CoordinateApp>();


        public string GetCoordsQuery()
        {
            string result = "";
            result += From.Longitude + "," + From.Latitude + ";";
            foreach (var item in To)
            {
                result += item.Longitude + "," + item.Latitude + ";";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }
    }
}
