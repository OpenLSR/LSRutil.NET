# LSRutil.NET

LSRutil.NET is a library for reading and eventually writing the LEGO Stunt Rally .trk file format. It parses tracks into a format that is easy to work with, and allows people to make converters and parsers for the format.

[![Nuget](https://img.shields.io/nuget/v/LSRutil.NET?color=004880&label=NuGet&logo=nuget)](https://www.nuget.org/packages/LSRutil.NET)

## Usage
```cs
using LSRutil;
...

var reader = new TRKReader();
var track = reader.ReadTrack(@"C:\LSR\SavedTracks\dune.trk");

Console.WriteLine("This track is using the {0} theme!", track.theme);
```
Check out the [ReadTrack](Examples/ReadTrack/Program.cs), [WriteTrack](Examples/WriteTrack/Program.cs), and [ThemeChange](Examples/ThemeChange/Program.cs) examples!

## Projects
I like to think I write useful code, so here are some projects that use LSRutil.NET.
*Know of any I missed?* ***Create a pull request!***

* [**SRToolbox 2.0**](https://github.com/YellowberryHN/SRToolbox) (WIP)
* **TrackMasters** (Coming soon!)

## Need to use another language?
**There might be a version for it.**
* *Coming soon...*

## Contributing
Pull requests are always welcome, but major changes will not be accepted if they do not have a cooresponding issue.

## License
[MIT](https://choosealicense.com/licenses/mit/)