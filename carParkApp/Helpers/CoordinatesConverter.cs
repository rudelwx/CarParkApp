using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;

namespace carParkApp.Helpers
{
    public static class CoordinatesConverter
    {

        public static (double latitude, double longitude) ConvertSVY21ToWGS84(double x, double y)
        {
            (CoordinateSystem wgs84, CoordinateSystem svy21) = DefineCoordinateSystem();
            var transform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(svy21, wgs84);
            var result = transform.MathTransform.Transform(new double[] { x, y });

            return (result[1], result[0]); // Return lat, lon
        }

        public static (double x, double y) ConvertWGS84ToSVY21(double latitude, double longitude)
        {

            (CoordinateSystem wgs84, CoordinateSystem svy21) = DefineCoordinateSystem();
            var transform = new CoordinateTransformationFactory().CreateFromCoordinateSystems(wgs84, svy21);
            var result = transform.MathTransform.Transform(new double[] { longitude, latitude  });

            return (result[1], result[0]); // Return x, y
        }

        private static (CoordinateSystem wgs84, CoordinateSystem svy21) DefineCoordinateSystem()
        {
            var factory = new CoordinateSystemFactory();

            // Create WGS84 geographic coordinate system
            var wgs84 = factory.CreateGeographicCoordinateSystem(
                "WGS 84",
                AngularUnit.Degrees,
                HorizontalDatum.WGS84,
                PrimeMeridian.Greenwich,
                new AxisInfo("Longitude", AxisOrientationEnum.East),
                new AxisInfo("Latitude", AxisOrientationEnum.North)
            );

            // Define the Transverse Mercator Projection for SVY21
            var projection = factory.CreateProjection(
                "Transverse_Mercator",
                "Transverse Mercator",
                new List<ProjectionParameter>()
                {
                    new ProjectionParameter("latitude_of_origin", 1.36666666666667),
                    new ProjectionParameter("central_meridian", 103.8333333333333),
                    new ProjectionParameter("scale_factor", 1.0),
                    new ProjectionParameter("false_easting", 28001.642),
                    new ProjectionParameter("false_northing", 38744.572)
                }
            );

            // Define Projected Coordinate System (SVY21)
            var svy21 = factory.CreateProjectedCoordinateSystem(
                "SVY21 / Singapore TM",
                wgs84,
                projection,
                LinearUnit.Metre, // SVY21 uses meters
                new AxisInfo("Easting", AxisOrientationEnum.East),
                new AxisInfo("Northing", AxisOrientationEnum.North)
            );

            return (wgs84, svy21);
        }
    }
}
