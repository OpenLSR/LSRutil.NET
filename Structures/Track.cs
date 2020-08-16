using System;
using System.Collections.Generic;
using static LSRutil.Constants;

namespace LSRutil
{
    public class Track
    {
        /// <summary>
        /// The theme of the track.
        /// </summary>
        public TrackTheme theme;

        /// <summary>
        /// The size of the track.
        /// </summary>
        public TrackSize size;

        /// <summary>
        /// The track time of day.
        /// </summary>
        public TrackTime time;

        /// <summary>
        /// All of the TrackElements in the track.
        /// </summary>
        private List<TrackElement> elements;

        /// <summary>
        /// The track's playablity outside of the editor.
        /// </summary>
        public bool playable;

        /// <summary>
        /// Instanciates a new track.
        /// </summary>
        /// <param name="theme">The theme of the track</param>
        /// <param name="size">The size of the track</param>
        /// <param name="time">The track time of day</param>
        public Track(TrackTheme theme = TrackTheme.City, TrackSize size = TrackSize.Singleplayer, TrackTime time = TrackTime.Day)
        {
            this.theme = theme;
            this.size = size;
            this.time = time;
            this.elements = new List<TrackElement> { };
        }

        /// <summary>
        /// Adds a TrackElement to the track.
        /// </summary>
        /// <param name="element">The TrackElement to add</param>
        public void Add(TrackElement element)
        {
            elements.Add(element);
        }

        public void SetElements(List<TrackElement> elements)
        {
            this.elements = elements;
        }

        /// <summary>
        /// Returns a list of all of the elements in a track.
        /// </summary>
        /// <returns>A list of TrackElements</returns>
        public List<TrackElement> GetElements()
        {
            return elements;
        }

        public int GetMaxElements()
        {
            return (int)Math.Pow((((int)this.size + 1) * 8), 2);
        }

        /// <summary>
        /// Prints information about this track to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("## Track information ##");
            Console.WriteLine("Track Size: {0}", this.size);
            Console.WriteLine("Track Theme: {0}", this.theme);
            Console.WriteLine("Time of Day: {0}", this.time);
            Console.WriteLine("---");
        }
    }
}
