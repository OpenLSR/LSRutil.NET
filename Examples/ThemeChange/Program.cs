using LSRutil;
using static LSRutil.Constants;

namespace ThemeChange
{
    class Program
    {
        static string filename = @"C:\LSR\SavedTracks\height.trk";
        static TrackTheme theme = TrackTheme.Ice;

        static void ChangeTheme(TrackElement element)
        {
            element.theme = theme;
        }

        static void Main(string[] args)
        {
            var writer = new TRKWriter();
            var reader = new TRKReader();

            var track = reader.ReadTrack(filename);
            track.theme = theme;

            var elements = track.GetElements();
            elements.ForEach(ChangeTheme);

            writer.WriteTrack(track, "test2.trk");
        }
    }
}
