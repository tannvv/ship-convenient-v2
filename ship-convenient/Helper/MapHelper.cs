using GeoCoordinatePortable;
using ship_convenient.Entities;
using ship_convenient.Model.MapboxModel;

namespace ship_convenient.Helper
{
    public class MapHelper
    {
        public static bool ValidDestinationBetweenShipperAndPackage(PolyLineModel polyLine, Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<GeoCoordinate>? geoCoordinateList = polyLine.PolyPoints;
            if (geoCoordinateList != null)
            {
                int validNumberCoord = 0;
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate shopCoordinate = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
                    double distanceShop = geoCoordinate.GetDistanceTo(shopCoordinate);
                    if (distanceShop <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                foreach (GeoCoordinate geoCoordinate in geoCoordinateList)
                {
                    GeoCoordinate packageCoordinate = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
                    double distancePackage = geoCoordinate.GetDistanceTo(packageCoordinate);
                    if (distancePackage <= spacingValid)
                    {
                        validNumberCoord++;
                        break;
                    };
                }
                if (validNumberCoord == 2)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
