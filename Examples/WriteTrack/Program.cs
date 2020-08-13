using System;
using LSRutil;
using static LSRutil.Constants;

namespace WriteTrack
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new TRKWriter();
            var track = new Track() {
                size = TrackSize.Multiplayer,
                theme = TrackTheme.Ice,
                time = TrackTime.Night
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
