# LSRutil.NET

LSRutil.NET is a library for reading and manipulating various types of file from the game LEGO Stunt Rally. It allows you to read and write track files, AI data, RFH/RFD resource files, and much more.

[![Nuget](https://img.shields.io/nuget/v/LSRutil.NET?color=004880&label=NuGet&logo=nuget)](https://www.nuget.org/packages/LSRutil.NET)
[![Nuget.Lite](https://img.shields.io/nuget/v/LSRutil.NET.Lite?color=9f0000&label=NuGet%20%28Lite%29&logo=nuget)](https://www.nuget.org/packages/LSRutil.NET.Lite)

## Usage
```cs
using LSRutil;
using LSRutil.TRK;
...

var reader = new TrkReader();
var track = reader.ReadTrack(@"C:\LSR\SavedTracks\dune.trk");

Console.WriteLine("This track is using the {0} theme!", track.theme);
```
Check out all of the [examples](Examples/)!

## Why are there 2 versions?
Well, the Lite version only contains code to read and write .TRK files, so it can be as portable as possible, without having to carry heavy dependancies like DotNetZip alongside it. Functionally, the code for reading tracks is the same in both versions, but the full version supports more formats.

## Projects
I like to think I write useful code, so here are some projects that use LSRutil.NET.
*Know of any I missed?* ***Create a pull request!***

* [**SRToolbox 2.0**](https://github.com/YellowberryHN/SRToolbox) (WIP)
* [**LouveSystems' TrackMasters**](https://trackmasters.louve.systems)

## Need to use another language?
**There might be a version for it.**
* *Coming soon...*

## Contributing
Pull requests are always welcome, but major changes will not be accepted if they do not have a corresponding issue.
Please open an issue first before working on a major feature.

## License
[MIT](https://choosealicense.com/licenses/mit/)