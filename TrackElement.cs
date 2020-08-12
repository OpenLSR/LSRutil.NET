using System;
using System.Numerics;
using static LSRutil.Constants;

namespace LSRutil
{
    public class TrackElement
    {
        /// <summary>
        /// The non-normalized ID, as a byte straight from the file.
        /// </summary>
        public byte id;

        /// <summary>
        /// The theme of the track element.
        /// </summary>
        public TrackTheme theme;

        /// <summary>
        /// The rotation of the track element.
        /// </summary>
        public TrackRotation rotation;

        /// <summary>
        /// The global position of the track element.
        /// </summary>
        public GridPosition pos;

        /// <summary> X value of element position, provided for ease of use. </summary>
        public int X { set => this.pos.X = value; get => this.pos.X; }
        /// <summary> Y value of element position, provided for ease of use. </summary>
        public int Y { set => this.pos.Y = value; get => this.pos.Y; }
        /// <summary> Z value of element position, provided for ease of use. </summary>
        public int Z { set => this.pos.Z = value; get => this.pos.Z; }

        /// <summary>
        /// The normalized ID, from 0 to 73.
        /// </summary>
        public int xid;

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
            this.xid = GetElement(this.theme, this.id);
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
            this.xid = GetElement(this.theme, this.id);
        }

        /// <summary>
        /// Instanciates a new track element.
        /// </summary>
        public TrackElement()
        {
            this.pos = new GridPosition();
        }

        /// <summary>
        /// Sets the normalized and non-normalized element ID at the same time.
        /// </summary>
        /// <param name="elementId">The non-normalized element ID</param>
        public void SetId(byte elementId)
        {
            this.id = elementId;
            this.xid = GetElement(this.theme, this.id);
        }

        /// <summary>
        /// Prints information about this element to the console. This should only be used for debugging.
        /// </summary>
        public void GetInfo()
        {
            Console.WriteLine("### Element information ###");
            Console.WriteLine("ID: 0x{0:X2}", this.id);
            Console.WriteLine("XID: {0}", this.xid);
            Console.WriteLine("Theme: {0}", this.theme);
            Console.WriteLine("Rotation: {0} ({1})", this.rotation, (int)this.rotation);
            Console.WriteLine("Position: {0}", this.pos);
        }
    }
}
