using System;
using System.Collections.Generic;

namespace LSRutil
{
    public class ExtendedTrack : Track
    {
        public byte x_time;

        public byte size_x;
        public byte size_y;

        public string name = string.Empty;
        public string author = string.Empty;
        public string description = string.Empty;

        public ExtendedTrack()
        {
            size_x = size_y = 7;
            x_time = 12;
            theme = TrackTheme.City;
            elements = new List<TrackElement> { };
        }

        public static ExtendedTrack Convert(Track track)
        {
            var xtk = new ExtendedTrack();
            xtk.SetLegacySize(track.size);
            xtk.theme = track.theme;
            xtk.x_time = (byte)(track.time == TrackTime.Day ? 12 : 0);

            var temp = new List<TrackElement>();
            
            foreach (var piece in track.GetElements())
            {
                temp.Add(new ExtendedTrackElement(piece));
            }

            xtk.elements = temp;
            xtk.playable = track.playable;
            
            return xtk;
        }
        
        public ExtendedTrack(byte sizeX, byte sizeY, TrackTheme theme = TrackTheme.City, byte time = 12)
        {
            this.size_x = sizeX;
            this.size_y = sizeY;
            this.theme = theme;
            this.x_time = time;
            elements = new List<TrackElement> { };
        }

        private void SetLegacySize(TrackSize size)
        {
            size_x = (byte)(((int)size + 1) * 8 - 1);
            size_y = (byte)(((int)size + 1) * 8 - 1);
        }
    }
    public class ExtendedTrackElement : TrackElement
    {
        public byte variant;
        
        /// <summary>
        /// Used by XTK to store element metadata.
        /// </summary>
        public Dictionary<byte, byte> metadata = new Dictionary<byte, byte>();
        
        public ExtendedTrackElement()
        {
            pos = new GridPosition();
        }

        public ExtendedTrackElement(TrackElement piece)
        {
            pos = piece.pos;
            index = piece.index;
            rotation = piece.rotation;
            theme = piece.theme;
        }

        /// <summary>
        /// Instantiates a new track element with given arguments.
        /// </summary>
        /// <param name="index">The index of the track element</param>
        /// <param name="pos">The position of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        public ExtendedTrackElement(uint index, GridPosition pos, int rotation)
        {
            this.index = index;
            this.pos = pos;
            this.rotation = (TrackRotation)rotation;
        }
        
        /// <summary>
        /// Instantiates a new track element with given arguments.
        /// </summary>
        /// <param name="index">The index of the track element</param>
        /// <param name="pos">The position of the track element</param>
        /// <param name="rotation">The rotation of the track element</param>
        /// <param name="theme">The theme of the track element</param>
        public ExtendedTrackElement(uint index, GridPosition pos, int rotation, Track.TrackTheme theme)
        {
            this.index = index;
            this.pos = pos;
            this.rotation = (TrackRotation)rotation;
            this.theme = theme;
        }
        
        /// <summary>
        /// A map of all of the XTK metadata fields and their IDs
        /// </summary>
        public enum XtkMetadataFields
        {
            /// <summary>
            /// Track physicality. Controls how an element reacts.
            /// </summary>
            /// <remarks>
            /// 0: standard, 1: no collisions, 2: no rendering
            /// </remarks>
            TrackPhysicality = 0x00,
            
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
        }
    }
}