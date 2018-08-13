# [Able - Askowl Base Library Enabler](http://unitydoc.marrington.net/Able)

## Executive Summary

Unity provides lots of great functionality, but there are always


* {:toc}

> Read the code in the Examples Folder and run the Example scene

## Introduction

## Maths functions

### Clock.cs - time and date conversions

#### Epoch Time

Epoch time was invented by the early Unix creators to represent time as seconds since the start of 1970 in a 32 bit integer for fast calculations. In this form it wraps around on 2038. It also suffered some inaccuracy because it did not account for leap seconds. This conversion is not 2038 limited as it uses doubles. Leap seconds will only be an issue if you are using dates each side of one - an unlikely event with minor implications.

```c#
DateTime now          = DateTime.Now;
double   epochTimeNow = Clock.EpochTimeNow;
double   epochTime    = Clock.EpochTimeAt(now);
AssertAlmostEqual(epochTime, epochTimeNow);

DateTime later          = now.AddDays(1);
double   epochTimeLater = Clock.EpochTimeAt(later);
AssertAlmostEqual(24 * 60 * 60, epochTimeLater - epochTimeNow);

var diff = later.Subtract(Clock.FromEpochTime(epochTimeLater));
AssertAlmostEqual(diff.TotalSeconds, 0);
```

##### double EpochTimeNow;

Epoch time is always UTC.

##### double EpochTimeAt(DateTime when);

Convert local time to UTC if necessary then translate to epoch time. Unlike Unix Epoch time, leap seconds are accounted for.

##### DateTime FromEpochTime(double epochTime);

Convert back from Epoch UTC time to local time, C# style.

### Compare.cs - equality and almost equality

### AlmostEqual for Floating Point

Comparing floating point numbers can be a hit or miss affair. Every mathematical operation is subject to rounding to fit into the number of bits. A single precision 32 bit float has around 7 digits of accuracy.  Even trivial calculations may not compare equal.

Enter `Compare.AlmostEqual`. You can specify the minimum change or use the defaults of 0.001 for single precision and 0.00001 for doubles.

```c#
IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.2f, minimumChange: 0.1f));
IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.2f, minimumChange: 0.11f));

IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.11f));
IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.0999f));

IsFalse(Compare.AlmostEqual(a: 103.11, b: 104, minimumChange: 0.5));
IsTrue(Compare.AlmostEqual(a: 103.11,  b: 104, minimumChange: 0.9));

IsFalse(Compare.AlmostEqual(a: 123.45678, b: 123.45679));
IsTrue(Compare.AlmostEqual(a: 123.456789, b: 123.45679));
```

### AlmostEqual for Integers

Integers don't suffer from rounding problems. Sometimes it is useful to see if two values are close.

```c#

IsFalse(Compare.AlmostEqual(a: 123L, b: 133L, minimumChange: 10L));
IsTrue(Compare.AlmostEqual(a: 123L,  b: 133L, minimumChange: 11L));

IsFalse(Compare.AlmostEqual(a: 123L, b: 125L));
IsFalse(Compare.AlmostEqual(a: 123L, b: 121L));
IsTrue(Compare.AlmostEqual(a: 123L,  b: 124L));
IsTrue(Compare.AlmostEqual(a: 123L,  b: 122L));

IsFalse(Compare.AlmostEqual(a: 1, b: 4, minimumChange: 2));
IsTrue(Compare.AlmostEqual(a: 1,  b: 3, minimumChange: 4));

IsFalse(Compare.AlmostEqual(a: 1, b: 4));
IsTrue(Compare.AlmostEqual(a: 1,  b: 2));
```

### [ExponentialMovingAverage.cs](https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average)

***<u>From Wikipedia</u>***:

> An **exponential moving average (EMA)**, also known as an **exponentially weighted moving average (EWMA)**,[[5\]](https://en.wikipedia.org/wiki/Moving_average#cite_note-5) is a first-order [infinite impulse response](https://en.wikipedia.org/wiki/Infinite_impulse_response) filter that applies weighting factors which decrease [exponentially](https://en.wikipedia.org/wiki/Exponential_decay). The weighting for each older [datum](https://en.wikipedia.org/wiki/Data) decreases exponentially, never reaching zero. The graph at right shows an example of the weight decrease.

 ***<u>From Me (Paul Marrington):</u>***

> An **exponential moving average** is a way to calculate an average where older values have less impact on the average than more recent ones.

It is most often used in financial calculations, but I use it mainly for IoT. Many devices can read wildly until the settle down. Then real-world interactions make then inaccurate again. A classis is the compass or magnetometer. Walk past a mass of steel and they will be attracted - just like an engineer. Using an EMA and these variations will be dampened. EMA is also useful when merging IoT data.

#### EMA Initialisation

The simplest form is to create a new EMA object without parameters.

```c#
var ema = new ExponentialMovingAverage(); // lookback defaults to 8
```

As the comment says, the average is over the last 8 values plus the new one. You can set your own.

```c#
var ema = new ExponentialMovingAverage(lookback: 50);
```

The lookback value is application specific. Ask yourself how many data points back is the information irrelevant to the current value. If you take one reading a second and any reading over 15 seconds old has no value, set lookback to 15.

#### EMA Average Value

If you don't make `Average` calls at consistent time intervals the you will need to consider other methods to make the values equally spaced.

```c#
AreEqual(expected: 1f,      actual: ema.Average(value: 1));
AreEqual(expected: 2.6f,    actual: ema.Average(value: 5));
AreEqual(expected: 2.76f,   actual: ema.Average(value: 3));
AreEqual(expected: 4.056f,  actual: ema.Average(value: 6));
AreEqual(expected: 4.0336f, actual: ema.Average(value: 4));
```

#### EMA Average Angle

Using EMA with angles in degrees is exactly the same except that the result is normalised to be between -180 and +180 degrees.

```c#
AreEqual(expected: -10f,       actual: ema.AverageAngle(degrees: -10));
AreEqual(expected: -5.555555f, actual: ema.AverageAngle(degrees: 10));
AreEqual(expected: -5.432098f, actual: ema.AverageAngle(degrees: -5));
AreEqual(expected: -3.113854f, actual: ema.AverageAngle(degrees: 5));
AreEqual(expected: -3.088552f, actual: ema.AverageAngle(degrees: 357));
AreEqual(expected: -1.513316f, actual: ema.AverageAngle(degrees: 364));
```

### Geodetic.cs - distances and bearings

> **Geodesy**: The branch of mathematics dealing with the shape and area of the earth or large portions of it.
>
> **Origin**: late 16th century: from modern Latin geodaesia, from Greek geōdaisia, from gē ‘earth’ + daiein ‘divide’.
>
> https://en.wikipedia.org/wiki/Geodesy
> https://www.movable-type.co.uk/scripts/latlong.html

> **Paul's Definition**: Calculations of distances and bearings of and between two points on the earth's surface and accounting for the curvature of the earth.

#### Coordinates Data Structure

Yet another data structure to contain coordinates. In the end it is more efficient to have separate definitions than it is to burden one definition with lots of irrelevant data. It is particularly poignant when we are dealing with pass-by-value.

In this world-view, coordinates use 64 bit double floating points for accuracy and know whether they are degrees or radians.

```c#
var location = Geodetic.Coords(-27.46850, 151.94379);
var same = Geodetic.Coords(-0.4794157656, 2.65191941345, radians: true);
location.ToRadians();
same.ToDegrees();
Debug.Log(same.ToString()); // -27.46850, 151.94379
```

#### Distance Between Two Points

In geodetic parlance the shortest distance between two points is an arc, not a straight line. This is kind of important if you don't want to tunnel through earth and dive under the sea to get anywhere.

`Kilometres(from, to)` uses the Haversine formula to calculate the distance taking into account an approximation of the earth's curvature. For display convenience there is a version, `DistanceBetween(from, to)`, that returns a string that is more friendly than the raw kilometres. If the distance is below one kilometre, it returns the value as a whole number of metres (i.e. 43 m). For distances below ten kilometres, one decimal place is provided (4.7 km). Above the kilometres are whole numbers only (23 km).

#### Bearing from One Point to the Next

If you were hiking you would take a bearing between yourself and a known landmark and use that bearing to get there.

```c#
var degrees = Geodetic.BearingDegrees(from, to);
var radians = Geodetic.BearingRadians(from, to);
Assert.AreEqual(degrees, Trig.degrees(radians));
```

#### Find One Point from Another

The next navigational trick is to find a destination coordinate when knowing the bearing and distance of that point. Useful if you want to call an air-strike down on an enemy position you are observing.

```c#
Geodetic.Destination(start: here, distanceKm: 1.2, bearingDegrees: 23.4);
```

### Quaternions.cs - adding features

Unity quaternion math focusses on the needs of the game. Great, but there are a few methods needed for augmented reality that are not provided.

#### AroundAxis

Rotate a quaternion around the X, Y or Z axis by the given number of degrees. This is a useful approact for a clock face, a compass or a merry-go-round.

```c#
// ... A
// rotate z axis for magnetic heading
attitude = attitude.AngleAxis(Trig.zAxis, compass.MagneticHeading);
// ... B
```

#### Inverse

An inverse changes the direction of the rotation. If you rotate a quaternion then rotate it again using the inverse then you will get back the original quaternion.

```c#
// C ...
mainCamera.transform.localRotation = attitude.Inverse();
```

#### LengthSquared

The concept of length or magnitude for a quaternion has no visual representation when dealing with attitude or rotation. The catch is that most algorithms require unit quaternions - where the length squared will approach one.

#### Normalise

> We can compute a fast 1/sqrt(x) by using a tangent-line approximation to the function. This is like a really simple 1-step Newton-Raphson iteration, and by tuning it for our specific case, we can achieve high accuracy for cheap. (A Newton-Raphson iteration is how specialized instruction sets like 3DNow and SSE compute fast 1/sqrt).

|           http://www.cs.uky.edu/~cheng/cs633/quaternion.html |
| -----------------------------------------------------------: |
| **The Inner Product, March 2002<br>**Jonathan Blow ([jon@number-none.com](mailto:jon@number-none.com)) |

This version is on an average 20% faster than `normalized` as provided by Unity.

#### RightToLeftHanded

For rotations, quaternions hold information on direction in 3 dimensions and the rotation of the object. Think of an airplane flying straight in a particular direction. Given a point of reference you can calculate the angle on the X, Y and Z planes. Now the airplane dips it's wing and spins upside-down. The calculations before are exactly the same, but the rotation has changed. Just as the euler angles define the direction of travel, the sign of the rotation defines which way the airplane is spinning.

Two of anything with opposite chirality cannot be superimposed on each other and yet can be otherwise identical. The choice of which rotation direction is positive is arbitrary. The gyroscope used in phones has right-hand chirality, while Unity uses left-handed.

```c#
Quaternion rotateTo = Device.Attitude.RightToLeftHanded(Trig.zAxis);
```

#### RotateBy

Unity is left handed using the Z axis for forard. The iOS gyroscope, for example is right handed. We can reverse the Chirality (a fancy word for handed) by negating the offending axis and W. This effectively reverses the direction of rotation.

```c#
var attitude = GPS.Attitude.RightToLeftHanded(Trig.zAxis);
```

#### SwitchAxis

Different IoT devices define different axes as forward. We need to pivot on the third axis by 90 degrees to correct for the difference. This reverses the chirality, but this function corrects for that.

```c#
// B ...
// Gyro Z axis is Unity camera Y.
attitude.SwitchAxis(pivot: Trig.xAxis)
// ... C
```

### Trig.cs

#### Direction

##### Trig.xAxis, Trig.yAxis and Trig.zAxis

When I prefer to use *X, Y, Z* instead of *right, up, forward* I use Trig.Direction values. These are unit directions with either the X, Y or Z component set 1 one to specify the axis.

##### X, Y and Z

Integersw here only one will be non-zero to define axis. Direction is also recorded as it can be 1 or -1.

##### Name

The name is a character constant, being 'X', 'Y' or 'Z'. Use it for switch statements where the character means more than using the ordinal value.

##### Ord

Ordinal value - the same as for `Vector3` - X=0, Y=1, Z=2. The values can be access by ordinal value (i.e. Trig.xAxis[0] == 1).

##### Vector

When Unity provided functions want to describe a direction, they use constants inside `Vector3` such as `Vector3.up`. To provide directions as XYZ, use `Trig.Y.Vector`.

##### VectorName

Just for kicks and giggles you can also retrieve the name of the asociated vector `Trig.Y.VectorName == "up"`.

##### OtherAxes

When we use an axis as a pivot we will really want the other axes to work with. This field refers to an array of the other two Directions. `Trig.xAxis.OtherAxes[0] == Trig.yAxis && Trig.xAxis.OtherAxes[1] == Trig.zAxis`.

##### Negative

`Vector3` has the concept of direction where positive is up and negative is down, with the same for left and right or forward and back. For Trig.Direction, use the unary minus as in `-Trig.xAxis`. OtherAxes for negative directions will themselves be negative. You can check if a direction is negative with `Trig.xAxis.Negative == false`.

Here is a slice of the unit tests for `Trig.xAxis` only.  Use these as a guide for what you can achieve.

```c#
var xAxis = Trig.xAxis;
var yAxis = Trig.yAxis;
var zAxis = Trig.zAxis;

Assert.AreEqual(1,             xAxis.X);
Assert.AreEqual(0,             xAxis.Y);
Assert.AreEqual(0,             xAxis.Z);
Assert.AreEqual(xAxis[0],      xAxis.X);
Assert.AreEqual(xAxis[1],      xAxis.Y);
Assert.AreEqual(xAxis[2],      xAxis.Z);
Assert.AreEqual('X',           xAxis.Name);
Assert.AreEqual("X Axis",      xAxis.ToString());
Assert.AreEqual(0,             xAxis.Ord);
Assert.AreEqual(Vector3.right, xAxis.Vector);
Assert.AreEqual("right",       xAxis.VectorName);
var otherAxes = xAxis.OtherAxes;
Assert.AreEqual(yAxis, otherAxes[0]);
Assert.AreEqual(zAxis, otherAxes[1]);

// Tests for negative
var minusX = -Trig.xAxis;
var minusY = -Trig.yAxis;
var minusZ = -Trig.zAxis;

Assert.IsFalse(xAxis.Negative);
Assert.IsTrue(minusX.Negative);
Assert.AreEqual(-1,           minusX.X);
Assert.AreEqual(0,            minusX.Y);
Assert.AreEqual(0,            minusX.Z);
Assert.AreEqual(minusX[0],    minusX.X);
Assert.AreEqual(minusX[1],    minusX.Y);
Assert.AreEqual(minusX[2],    minusX.Z);
Assert.AreEqual('X',          minusX.Name);
Assert.AreEqual("-X Axis",    minusX.ToString());
Assert.AreEqual(0,            minusX.Ord);
Assert.AreEqual(Vector3.left, minusX.Vector);
Assert.AreEqual("left",       minusX.VectorName);
otherAxes = minusX.OtherAxes;
Assert.AreEqual(minusY, otherAxes[0]);
Assert.AreEqual(minusZ, otherAxes[1]);
```

#### ToRadians

Convert a number in degrees to radians.

```c#
Assert.IsTrue(Compare.AlmostEqual(1.5708, Trig.ToRadians(90)));
```

#### ToDegrees

Convert a value in radians back to degrees. Note the need to reduce accuracy in the comparison due to the calculation.

```c#
Assert.IsTrue(Compare.AlmostEqual(90, Trig.ToDegrees(1.5708), 1e5));
```

#### Relative Position given Distance and Angle or Bearing

Calculate a relative vector in two dimensions give the distance away and the angle or bearing.

##### RelativePositionFromAngle

This is a trigonometric angle where 0 degrees in +X and 90 degrees is East or +1. Increasing angles move the result counter-clockwise

```c#
expected.Set(3.3f, 0);
actual = Trig.RelativePositionFromBearing(3.3f, Trig.ToRadians(90));
AreEqual(expected, actual);
```

##### RelativePositionFromBearing

This is a compass bearing where 0 degrees in North or +Y and 90 degrees is East or +X. Increasing bearings move the result clockwise

```c#
expected.Set(0, 3.3f);
actual = Trig.RelativePositionFromAngle(3.3f, Trig.ToRadians(90));
AreEqual(expected, actual);
```

## Data Structures

### Disposable.cs - helper for IDisposable.Dispose()

With closures and anonymous functions `using(...){...}` can be implemented where it is needed without creating a new class to manage it.

```c#
    [Test]
    public void DisposableExample() {
      Assert.AreEqual(expected: 0, actual: numberOfMonsters);

      using (Ephemeral()) {
        numberOfMonsters += 2;
        Assert.AreEqual(expected: 2, actual: numberOfMonsters);
      }

      Assert.AreEqual(expected: 0, actual: numberOfMonsters);
    }

    private IDisposable Ephemeral() =>
        new Disposable {Action = () => numberOfMonsters = 0};

    private int numberOfMonsters;
  }
```

### Emitter.cs - the observer pattern

*Somebody* owns an ***Emitter*** and many other somebodies can register interest in said emitter. When anyone who has access to the emitter instance pulls the trigger, all observers are told. There is a second version ***Emitter&lt;T>*** that can pass an object between emitter and all observers.

```c#
var emitter = new Emitter();

using (var subscription = emitter.Subscribe(new Observer1())) {
  // we now have one subscription
  Assert.AreEqual(expected: 0, actual: counter);
  // Tell observers we have something for them
  emitter.Fire();
  // The observer changes the value
  Assert.AreEqual(expected: 1, actual: counter);
  // A manual call to Dispose will stop the observer listening ...
  subscription.Dispose();
  // ... and calls OnComplete that in this case sets count to zero
  Assert.AreEqual(expected: 0, actual: counter);
  // Now if we fire...
  emitter.Fire(); // not listening any more
  // ... the counter doesn't change because we have no observers
  Assert.AreEqual(expected: 0, actual: counter);
}
// Outside the using - and Dispose was called implicitly but OnComplete was not called again
Assert.AreEqual(expected: 0, actual: counter);
```

```c#
private struct Observer1 : IObserver {
  public void OnNext()      { ++counter; }
  public void OnCompleted() { counter--; }
}
```

While the generic version can pass additional information. The last piece of data sent is also stored in ***LastValue*** for reference.

```c#
var emitter = new Emitter<int>();
using (emitter.Subscribe(new Observer3())) {
  emitter.Fire(10);
}
Assert(10, emitter.LastValue);
```

```c#
private struct Observer3 : IObserver<int> {
  public void OnCompleted()            { counter--; }
  public void OnError(Exception error) { throw new Exception(); }
  public void OnNext(int        value) { counter = value; }
}
```

While an ***Emitter*** provides some extra facilities to the built-in ***event*** delegates, it does not improve decoupling. It does pave the way for decoupled events using [***CustomAssets***](/CustomAssets), an extension of ***ScriptableObjects***.

### LinkedList.cs - efficient walking movement