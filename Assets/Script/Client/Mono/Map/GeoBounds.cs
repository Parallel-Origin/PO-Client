namespace Script.Client.Mono.Map {
    /// <summary>
    ///     A class containing attributes to determine the geolocation bounds in which we are moving.
    /// </summary>
    public class GeoBounds {
        public static readonly double MaximumAllowedLatitude = 85.0510150000;
        public static readonly double MinimumAllowedLatitude = -85.0510150000;

        public static readonly double MaximumAllowedLongitude = 180.0000000000;
        public static readonly double MinimumAllowedLongitude = -180.0000000000;
    }
}