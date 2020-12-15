using LSRutil;
using LSRutil.TRK;

namespace WriteTrack
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new TrkWriter();
            var track = new Track() {
                size = Track.TrackSize.Multiplayer,
                theme = Track.TrackTheme.Ice,
                time = Track.TrackTime.Night
            };

            track.Add(new TrackElement() {
                theme = track.theme, // theme is set first, because that's important for the ID.
                xid = 0,
                rotation = 0,
                pos = new GridPosition(0,0,0)
            });

            writer.WriteTrack(track, "test.trk");
        }
    }
}
