# Unix Epoch Time Conversions

The developers of Unix wanted to make time simple, so they chose an arbitrary time and called it zero. For Unix, time zero is the start of 1970, in Greenwich, measured in seconds. It was a 32-bit integer because in 1972 floating point math was slow and not very accurate.

In Unity3D scripting, we are using the more sophisticated .NET DateTime object. Epoch time will often be the common denominator when interfacing with external systems.

Converting between time encodings is not complicated, but wrapping it in a static class does ensure consistency. It involves conversion from local to UTC and accounting for daylight saving if necessary.