using GeoCoordinatePortable;
using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Entities;
using ship_convenient.Model.MapboxModel;
using ship_convenient.Model.RouteModel;
using RouteEntity = ship_convenient.Entities.Route;

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

        public static bool ValidDestinationBetweenDeliverAndPackage(List<RoutePoint> points, Package package, double spacingValid = 2000)
        {
            bool result = false;
            List<GeoCoordinate>? geoCoordinateList = points.Select(x => new GeoCoordinate(x.Latitude, x.Longitude)).ToList();
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

        public static bool ValidSuggestDirectionPackage(string direction, Package package, RouteEntity route)
        {
            bool result = false;
            GeoCoordinate startPackage = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
            GeoCoordinate destinationPackage = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
            if (direction == DirectionTypeConstant.FORWARD)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartPackage = startRoute.GetDistanceTo(startPackage);
                double startToEndPackage = startRoute.GetDistanceTo(destinationPackage);
                if (startToStartPackage < startToEndPackage)
                {
                    result = true;
                }
            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double endToStartPackage = endRoute.GetDistanceTo(startPackage);
                double endToEndPackage = endRoute.GetDistanceTo(destinationPackage);
                if (endToStartPackage < endToEndPackage)
                {
                    result = true;
                }
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                result = true;
            }
            return result;
        }

        public static List<GeoCoordinate> GetListPointOrder(string direction, Package package, RouteEntity route) {
            List<GeoCoordinate> orderPoints = new List<GeoCoordinate>();
            GeoCoordinate startPackage = new GeoCoordinate(package.StartLatitude, package.StartLongitude);
            GeoCoordinate destinationPackage = new GeoCoordinate(package.DestinationLatitude, package.DestinationLongitude);
            if (direction == DirectionTypeConstant.FORWARD)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartPackage = startRoute.GetDistanceTo(startPackage);
                double startToEndPackage = startRoute.GetDistanceTo(destinationPackage);
                if (startToStartPackage < startToEndPackage)
                {
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                }
            }
            else if (direction == DirectionTypeConstant.BACKWARD)
            {
                GeoCoordinate endRoute = new GeoCoordinate(route.ToLatitude, route.ToLongitude);
                double endToStartPackage = endRoute.GetDistanceTo(startPackage);
                double endToEndPackage = endRoute.GetDistanceTo(destinationPackage);
                if (endToStartPackage < endToEndPackage)
                {
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                }
            }
            else if (direction == DirectionTypeConstant.TWO_WAY)
            {
                GeoCoordinate startRoute = new GeoCoordinate(route.FromLatitude, route.FromLongitude);
                double startToStartPackage = startRoute.GetDistanceTo(startPackage);
                double startToEndPackage = startRoute.GetDistanceTo(destinationPackage);
                if (startToStartPackage < startToEndPackage)
                {
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                }
                else
                {
                    orderPoints.Add(new GeoCoordinate(route.ToLatitude, route.ToLongitude));
                    orderPoints.Add(startPackage);
                    orderPoints.Add(destinationPackage);
                    orderPoints.Add(new GeoCoordinate(route.FromLatitude, route.FromLongitude));
                }
            }
            return orderPoints;
        }
    }
}
