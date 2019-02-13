//- If you are developing for augmented reality over distances larger than a room then you will be using GPS, Compass/Magnetometer and other instruments for location and direction. Any calculation for direction and distance between two or more points will need to take into account the curvature of the earth's surface. The branch of mathematics for these calculations is called Geodesy. Able provides a set of basic calculations in the Geodetic class.

#if AskowlTests
namespace Askowl.Able.Examples {
  using NUnit.Framework;

  public class GeodeticTranscript {
    [Test] public void Example() {
      //- Generate coordinate structs in decimal degrees
      var from = Geodetic.Coords(latitude: -27.46850, longitude: 151.94379);
      //- or radians
      var to = Geodetic.Coords(latitude: -0.47941664, longitude: 2.65191906, radians: true);
      //- The calculations below don't need to know which form is used. We can always convert it ourselves
      from = from.ToRadians();
      to = to.ToDegrees();
      //- Now we have two coordinates we can calculate how far apart they are
      double metresApart = Geodetic.Kilometres(from, to) * 1000;
      //- or use the human readable form which will use meters or km as needed
      string distanceApart = Geodetic.DistanceBetween(from, to);
      //- Now we have distance, what direction do we need to travel
      double bearing = Geodetic.BearingDegrees(from, to);
      //- and in radians
      double radians = Geodetic.BearingRadians(from, to);
      //- But what if we know the direction and distance to an object
      var destination = Geodetic.Destination(start: from, distanceKm: metresApart * 1000, bearingDegrees: bearing);
    }
  }
}
#endif