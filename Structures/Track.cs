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
            Arctic = 1,
            Ice = 1, // alternate name
            Desert = 2,
            City = 3,
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
        /// The asset ID, as an unsigned int straight from the file.
        /// </summary>
        public uint id { get => _id; set => SetId(value); }
        private uint _id;

        /// <summary>
        /// The theme of the track element.
        /// </summary>
        public Track.TrackTheme? theme { get => _theme; set => SetTheme(value); }
        private Track.TrackTheme? _theme;

        /// <summary>
        /// The rotation of the track element.
        /// </summary>
        public TrackRotation rotation;

        /// <summary>
        /// The global position of the track element.
        /// </summary>
        public GridPosition pos;

        /// <summary>
        /// 4 bytes, unknown purpose. Possibly just garbage data.
        /// </summary>
        public byte[] mystery = new byte[4] {0,0,0,0};

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
        /// The element ID index, from 0 to 72.
        /// </summary>
        public uint? index { get => _index; set => SetIndex(value); }
        private uint? _index;

        /// <summary>
        /// Instanciates a new track element with given arguments.
        /// </summary>
        /// <param name="id">The ID of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        /// <param name="pos">The position of the track element</param>
        public TrackElement(uint id, int rotation, GridPosition pos)
        {
            this.id = id;
            this.rotation = (TrackRotation)rotation;
            this.pos = pos;
        }

        /// <summary>
        /// Instanciates a new track element with given arguments.
        /// </summary>
        /// <param name="index">The index of the track element</param>
        /// <param name="theme">The theme of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        /// <param name="pos">The position of the track element</param>
        public TrackElement(uint index, Track.TrackTheme theme, int rotation, GridPosition pos)
        {
            this.id = id;
            this.rotation = (TrackRotation)rotation;
            this.pos = pos;
        }

        /// <summary>
        /// Instanciates a new track element.
        /// </summary>
        public TrackElement()
        {
            pos = new GridPosition();
        }

        public void SetId(uint id)
        {
            _id = id;
            if(theme != null)
            {
                for (uint i = 0; i < 73; i++)
                {
                    if (elementTable[(int)theme][i] == id)
                    {
                        _index = i;
                        return;
                    }
                }
            } 
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    for (uint i = 0; i < 73; i++)
                    {
                        if (elementTable[j][i] == id)
                        {
                            _theme = (Track.TrackTheme)j;
                            _index = i;
                            return;
                        }
                    }
                }
                _index = null;
                return;
            }
        }

        public void SetIndex(uint? index)
        {
            if(theme != null)
            {
                _id = elementTable[(int)theme][(int)index];
            }
            else
            {
                throw new InvalidOperationException("Cannot set index of a theme-less element");
            }
        }

        public void SetTheme(Track.TrackTheme? theme)
        {
            if(_index != null) _id = elementTable[(int)theme][(int)_index];
            _theme = theme;
        }

        /// <summary>
        /// Prints information about this element to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("### Element information ###");
            Console.WriteLine("Asset ID: 0x{0:X8}", _id);
            Console.WriteLine("Index: {0}", _index);
            Console.WriteLine("Theme: {0}", theme);
            Console.WriteLine("Rotation: {0} ({1})", rotation, (uint)rotation);
            Console.WriteLine("Position: {0}", pos);
        }

        /// <summary>
        /// A list of all possible bytes of track elements in LSR.
        /// </summary>
        private static List<uint[]> elementTable = new List<uint[]> {
            // Jungle
            new uint[] { 0x3B65, 0x3B61, 0x3B62, 0x3B63, 0x3B64, 0x3B66, 0x3B67, 0x3B68, 0x3B69, 0x3B6B, 0x3B6C, 0x3B6F, 0x3B70, 0x3B71, 0x3B72, 0x3B73, 0x3B74, 0x3B75, 0x3B76, 0x3B77, 0x3B78, 0x3B79, 0x3B7A, 0x3B7B, 0x3B7C, 0x3B7D, 0x3B7E, 0x3B7F, 0x3B80, 0x3B81, 0x3B83, 0x3B88, 0x3B89, 0x3B8A, 0x3B8B, 0x3B8C, 0x3B8D, 0x3B8E, 0x3B8F, 0x3B90, 0x3B91, 0x3B92, 0x3B93, 0x3B94, 0x3B66, 0x3B97, 0x3B98, 0x3B9C, 0x3B9D, 0x3B9E, 0x3B9F, 0x3BA0, 0x3BA1, 0x3BA2, 0x3BA3, 0x3BA4, 0x3BA5, 0x3BA6, 0x3BA7, 0x3BA8, 0x3BA9, 0x3BAA, 0x3BAB, 0x3BAC, 0x3BAD, 0x3BAE, 0x3BAF, 0x3BBA, 0x3BBB, 0x3BBC, 0x3BBD, 0x3BBE, 0x3BBF },
            // Arctic
            new uint[] { 0x3F48, 0x3F49, 0x3F4A, 0x3F4B, 0x3F4C, 0x3F4E, 0x3F4F, 0x3F50, 0x3F51, 0x3F53, 0x3F54, 0x3F57, 0x3F58, 0x3F59, 0x3F5A, 0x3F5B, 0x3F5C, 0x3F5D, 0x3F5E, 0x3F5F, 0x3F60, 0x3F61, 0x3F62, 0x3F63, 0x3F64, 0x3F65, 0x3F66, 0x3F67, 0x3F68, 0x3F69, 0x3F6B, 0x3F70, 0x3F71, 0x3F72, 0x3F73, 0x3F74, 0x3F75, 0x3F76, 0x3F77, 0x3F78, 0x3F79, 0x3F7A, 0x3F7B, 0x3F7C, 0x3F7E, 0x3F7F, 0x3F80, 0x3F84, 0x3F85, 0x3F86, 0x3F87, 0x3F88, 0x3F89, 0x3F8A, 0x3F8B, 0x3F8C, 0x3F8D, 0x3F8E, 0x3F8F, 0x3F90, 0x3F91, 0x3F92, 0x3F93, 0x3F94, 0x3F95, 0x3F96, 0x3F97, 0x3FA2, 0x3FA3, 0x3FA4, 0x3FA5, 0x3FA6, 0x3FA7 },
            // Desert
            new uint[] { 0x471D, 0x4719, 0x471A, 0x471B, 0x471C, 0x471E, 0x471F, 0x4720, 0x4721, 0x4723, 0x4724, 0x4727, 0x4728, 0x4729, 0x472A, 0x472B, 0x472C, 0x472D, 0x472E, 0x472F, 0x4730, 0x4731, 0x4732, 0x4733, 0x4734, 0x4735, 0x4736, 0x4737, 0x4738, 0x4739, 0x473B, 0x4740, 0x4741, 0x4742, 0x4743, 0x4744, 0x4745, 0x4746, 0x4747, 0x4748, 0x4749, 0x474A, 0x474B, 0x474C, 0x474E, 0x474F, 0x4750, 0x4754, 0x4755, 0x4756, 0x4757, 0x4758, 0x4759, 0x475A, 0x475B, 0x475C, 0x475D, 0x475E, 0x475F, 0x4760, 0x4761, 0x4762, 0x4763, 0x4764, 0x4765, 0x4766, 0x4767, 0x4772, 0x4773, 0x4774, 0x4775, 0x4776, 0x4777 },
            // City
            new uint[] { 0x4330, 0x4331, 0x4332, 0x4333, 0x4334, 0x4336, 0x4337, 0x4338, 0x4339, 0x433B, 0x433C, 0x433F, 0x4340, 0x4341, 0x4342, 0x4343, 0x4344, 0x4345, 0x4346, 0x4347, 0x4348, 0x4349, 0x434A, 0x434B, 0x434C, 0x434D, 0x434E, 0x434F, 0x4350, 0x4351, 0x4353, 0x4358, 0x4359, 0x435A, 0x435B, 0x435C, 0x435D, 0x435E, 0x435F, 0x4360, 0x4361, 0x4362, 0x4363, 0x4364, 0x4366, 0x4367, 0x4368, 0x436C, 0x436D, 0x436E, 0x436F, 0x4370, 0x4371, 0x4372, 0x4373, 0x4374, 0x4375, 0x4376, 0x4377, 0x4378, 0x4379, 0x437A, 0x437B, 0x437C, 0x437D, 0x437E, 0x437F, 0x438A, 0x438B, 0x438C, 0x438D, 0x438E, 0x438F }
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
        /// Returns all of the elements in the element table for a specific theme.
        /// </summary>
        /// <param name="theme">The theme of the returned elements</param>
        /// <returns>A byte array of all of the elements</returns>
        public static uint[] GetElements(int theme)
        {
            return elementTable[theme];
        }

        public bool Equals(TrackElement element)
        {
            return (this.id, this.index, this.pos, this.rotation, this.theme, this.mystery) ==
                (element.id, element.index, element.pos, element.rotation, element.theme, element.mystery);
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
