using System;
using System.Numerics;
using System.Collections.Generic;

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
        /// Extension of <see cref="size"/>, used by XTK.
        /// </summary>
        public uint ext_sizeX;

        /// <summary>
        /// Extension of <see cref="size"/>, used by XTK.
        /// </summary>
        public uint ext_sizeY;

        /// <summary>
        /// The track time of day.
        /// </summary>
        public TrackTime time;

        /// <summary>
        /// Extension of <see cref="time"/>, values between 0-23, used by XTK.
        /// </summary>
        public uint ext_time;

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

        /// <summary>
        /// Defines track themes.
        /// </summary>
        public enum TrackTheme
        {
            Jungle = 0,
            Ice = 1,
            Desert = 2,
            City = 3,
            Arctic = 1
        }

        /// <summary>
        /// Defines possible track sizes.
        /// </summary>
        public enum TrackSize
        {
            Multiplayer = 0,
            Singleplayer = 1
        }

        /// <summary>
        /// Defines track time of day.
        /// </summary>
        public enum TrackTime
        {
            Day = 0,
            Night = 1
        }
    }

    public class TrackElement : IEquatable<TrackElement>
    {
        public enum TrackRotation
        {
            West = 0,
            North = 1,
            East = 2,
            South = 3
        }

        public enum TrackOrigin
        {
            Default = 0,
            _1x2,
            _1x3,
            _2x1,
            _2x2,
            _Other2x2,
            _3x3
        }

        /// <summary>
        /// Used by XTK to store element metadata.
        /// </summary>
        public Dictionary<int, int> ext_metadata;

        /// <summary>
        /// The non-normalized ID, as a byte straight from the file.
        /// </summary>
        public byte id { get => _id; set => SetId(value); }
        private byte _id;

        /// <summary>
        /// The theme of the track element.
        /// </summary>
        public Track.TrackTheme theme { get => _theme; set => SetTheme(value); }
        private Track.TrackTheme _theme;

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

        /// <summary>
        /// The origin of the track piece, determines what coordinate of the piece is the rotation axis
        /// </summary>
        public TrackOrigin origin;

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
            this.theme = (Track.TrackTheme)theme;
            this.rotation = (TrackElement.TrackRotation)rotation;
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
            this.theme = (Track.TrackTheme)theme;
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
            _xid = GetElementId(theme, _id);
        }

        /// <summary>
        /// Sets the normalized and non-normalized element ID at the same time.
        /// </summary>
        /// <param name="xid">The normalized element ID</param>
        public void SetId(int xid)
        {
            _xid = xid;
            _id = GetElementId(theme, _xid);
        }

        public void SetTheme(Track.TrackTheme theme)
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

        /// <summary>
        /// A list of all possible bytes of track elements in LSR.
        /// </summary>
        private static List<byte[]> elementTable = new List<byte[]> {
            new byte[] { 0x65, 0x61, 0x62, 0x63, 0x64, 0x66, 0x67, 0x68, 0x69, 0x6B, 0x6C, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x80, 0x81, 0x83, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x66, 0x97, 0x98, 0x9C, 0x9D, 0x9E, 0x9F, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0x00 },
            new byte[] { 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4E, 0x4F, 0x50, 0x51, 0x53, 0x54, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6B, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7E, 0x7F, 0x80, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0x00 },
            new byte[] { 0x1D, 0x19, 0x1A, 0x1B, 0x1C, 0x1E, 0x1F, 0x20, 0x21, 0x23, 0x24, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3B, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4E, 0x4F, 0x50, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x00 },
            new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x36, 0x37, 0x38, 0x39, 0x3B, 0x3C, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x53, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x66, 0x67, 0x68, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90 }
        };

        private static List<TrackOrigin> elementOrigins = new List<TrackOrigin> {
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._1x2,
            TrackOrigin.Default,
            TrackOrigin._2x2,
            TrackOrigin.Default,
            TrackOrigin._1x3,
            TrackOrigin.Default,
            TrackOrigin._2x2,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._1x3,
            TrackOrigin.Default,
            TrackOrigin._2x2,
            TrackOrigin.Default,
            TrackOrigin._1x2,
            TrackOrigin._1x3,
            TrackOrigin._1x2,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._2x1,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._2x1,
            TrackOrigin._2x1,
            TrackOrigin._2x1,
            TrackOrigin._2x1,
            TrackOrigin._2x1,
            TrackOrigin._Other2x2,
            TrackOrigin.Default,
            TrackOrigin._2x1,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._3x3,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin._3x3,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default,
            TrackOrigin.Default
        };

        /// <summary>
        /// Converts normalized element ID to non-normalized element ID.
        /// </summary>
        /// <param name="theme">The theme of the requested element</param>
        /// <param name="xid">The normalized element ID</param>
        /// <returns>The non-normalized element ID</returns>
        public static byte GetElementId(Track.TrackTheme theme, int xid)
        {
            return elementTable[(int)theme][xid];
        }

        /// <summary>
        /// Converts non-normalized element ID to normalized element ID.
        /// </summary>
        /// <param name="theme">The theme of the requested element</param>
        /// <param name="id">The non-normalized element ID</param>
        /// <returns>The normalized element ID</returns>
        public static int GetElementId(Track.TrackTheme theme, byte id)
        {
            return Array.IndexOf(elementTable[(int)theme], id);
        }

        /// <summary>
        /// Returns all of the elements in the element table for a specific theme.
        /// </summary>
        /// <param name="theme">The theme of the returned elements</param>
        /// <returns>A byte array of all of the elements</returns>
        public static byte[] GetElements(int theme)
        {
            return elementTable[theme];
        }

        public bool Equals(TrackElement element)
        {
            return (this.id, this.xid, this.pos, this.rotation, this.theme, this.mystery) ==
                (element.id, element.xid, element.pos, element.rotation, element.theme, element.mystery);
        }

        /// <summary>
        /// A map of all of the XTK metadata fields and their IDs
        /// </summary>
        public enum XtkMetadataFields
        {
            /// <summary>
            /// Used for different textures within themes.
            /// </summary>
            Variant = 0x00,

            /// <summary>
            /// Height block toggle, creates a floating track piece if set to 1.
            /// </summary>
            /// <remarks>
            /// 0: height blocks, 1: no height blocks
            /// </remarks>
            NoHeightBlocks = 0x01,

            /// <summary>
            /// Effect multiplier (speed boost, fan push, belt push, cannon toss, etc).
            /// </summary>
            EffectMultiplier = 0x02,

            /// <summary>
            /// Track physicality. Controls how an element reacts.
            /// </summary>
            /// <remarks>
            /// 0: standard, 1: no collisions, 2: no rendering
            /// </remarks>
            TrackPhysicality = 0x7F
        }
    }
}
