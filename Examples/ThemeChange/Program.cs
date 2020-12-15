using LSRutil;
using LSRutil.TRK;

namespace ThemeChange
{
    class Program
    {
        static string filename = @"C:\LSR\SavedTracks\height.trk";
        static Track.TrackTheme theme = Track.TrackTheme.Ice;

        static void ChangeTheme(TrackElement element)
        {
            element.theme = theme;
        }

        static void Main(string[] args)
        {
            var writer = new TrkWriter();
            var reader = new TrkReader();

            var track = reader.ReadTrack(filename);
            track.theme = theme;

            var elements = track.GetElements();
            elements.ForEach(ChangeTheme);

            writer.WriteTrack(track, "test2.trk");
        }
    }
}
