using System;
using System.Numerics;
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
            elements = new List<TrackElement> { };
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
            return (int)Math.Pow(((int)size + 1) * 8, 2);
        }

        /// <summary>
        /// Prints information about this track to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("## Track information ##");
            Console.WriteLine("Track Size: {0}", size);
            Console.WriteLine("Track Theme: {0}", theme);
            Console.WriteLine("Time of Day: {0}", time);
            Console.WriteLine("---");
        }
    }

    public class TrackElement
    {
        /// <summary>
        /// The non-normalized ID, as a byte straight from the file.
        /// </summary>
        public byte id { get => _id; set => SetId(value); }
        private byte _id;

        /// <summary>
        /// The theme of the track element.
        /// </summary>
        public TrackTheme theme { get => _theme; set => SetTheme(value); }
        private TrackTheme _theme;

        /// <summary>
        /// The rotation of the track element.
        /// </summary>
        public TrackRotation rotation;

        /// <summary>
        /// The global position of the track element.
        /// </summary>
        public GridPosition pos;

        /// <summary>
        /// An int, unknown purpose.
        /// </summary>
        public int mystery;

        /// <summary> X value of element position, provided for ease of use. </summary>
        public int X { set => pos.X = value; get => pos.X; }
        /// <summary> Y value of element position, provided for ease of use. </summary>
        public int Y { set => pos.Y = value; get => pos.Y; }
        /// <summary> Z value of element position, provided for ease of use. </summary>
        public int Z { set => pos.Z = value; get => pos.Z; }

        /// <summary>
        /// The normalized ID, from 0 to 73.
        /// </summary>
        public int xid { get => _xid; set => SetId(value); }
        private int _xid;

        /// <summary>
        /// Instanciates a new track element with given arguments.
        /// </summary>
        /// <param name="id">The ID of the track element</param>
        /// <param name="theme">The theme of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        /// <param name="pos">The position of the track element</param>
        public TrackElement(byte id, int theme, int rotation, GridPosition pos)
        {
            this.id = id;
            this.theme = (TrackTheme)theme;
            this.rotation = (TrackRotation)rotation;
            this.pos = pos;
        }

        /// <summary>
        /// Instanciates a new track element with given arguments.
        /// </summary>
        /// <param name="id">The ID of the track element</param>
        /// <param name="theme">The theme of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        /// <param name="pos">The position of the track element</param>
        [Obsolete("Deprecated, use GridPosition instead of Vector3.")]
        public TrackElement(byte id, int theme, int rotation, Vector3 pos)
        {
            this.id = id;
            this.theme = (TrackTheme)theme;
            this.rotation = (TrackRotation)rotation;
            this.pos = new GridPosition((int)pos.X, (int)pos.Y, (int)pos.Z);
        }

        /// <summary>
        /// Instanciates a new track element.
        /// </summary>
        public TrackElement()
        {
            pos = new GridPosition();
        }

        /// <summary>
        /// Sets the normalized and non-normalized element ID at the same time.
        /// </summary>
        /// <param name="elementId">The non-normalized element ID</param>
        public void SetId(byte elementId)
        {
            _id = elementId;
            _xid = GetElement(theme, _id);
        }

        public void SetId(int xid)
        {
            _xid = xid;
            _id = GetElement(theme, _xid);
        }

        public void SetTheme(TrackTheme theme)
        {
            _theme = theme;
            if (_xid >= 0) SetId(_xid);
        }

        /// <summary>
        /// Prints information about this element to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("### Element information ###");
            Console.WriteLine("ID: 0x{0:X2}", _id);
            Console.WriteLine("XID: {0}", _xid);
            Console.WriteLine("Theme: {0}", theme);
            Console.WriteLine("Rotation: {0} ({1})", rotation, (int)rotation);
            Console.WriteLine("Position: {0}", pos);
        }
    }
}
