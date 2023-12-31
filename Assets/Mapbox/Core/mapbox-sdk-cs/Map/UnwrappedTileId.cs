﻿using System;

namespace Mapbox.Map {
    /// <summary>
    ///     Unwrapped tile identifier in a slippy map. Similar to <see cref="CanonicalTileId"/>,
    ///     but might go around the globe.
    /// </summary>
    public struct UnwrappedTileId : IEquatable<UnwrappedTileId> {
        /// <summary> The zoom level. </summary>
        public readonly int Z;

        /// <summary> The X coordinate in the tile grid. </summary>
        public readonly int X;

        /// <summary> The Y coordinate in the tile grid. </summary>
        public readonly int Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnwrappedTileId"/> struct,
        ///     representing a tile coordinate in a slippy map that might go around the
        ///     globe.
        /// </summary>
        /// <param name="z">The z coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public UnwrappedTileId(int z, int x, int y) {
            this.Z = z;
            this.X = x;
            this.Y = y;
        }

        /// <summary> Gets the canonical tile identifier. </summary>
        /// <value> The canonical tile identifier. </value>
        public CanonicalTileId Canonical {
            get {
                return new CanonicalTileId(this);
            }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.UnwrappedTileId"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.UnwrappedTileId"/>.
        /// </returns>
        public override string ToString() {
            return this.Z + "/" + this.X + "/" + this.Y;
        }

        public bool Equals(UnwrappedTileId other) {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public override int GetHashCode() {
            return (X << 6) ^ (Y << 16) ^ (Z << 8);
            //return X ^ Y ^ Z;
        }

        public override bool Equals(object obj) {
            return this.X == ((UnwrappedTileId)obj).X && this.Y == ((UnwrappedTileId)obj).Y && this.Z == ((UnwrappedTileId)obj).Z;
        }

        public static bool operator ==(UnwrappedTileId a, UnwrappedTileId b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(UnwrappedTileId a, UnwrappedTileId b) {
            return !(a == b);
        }

        public UnwrappedTileId North {
            get {
                return new UnwrappedTileId(Z, X, Y - 1);
            }
        }

        public UnwrappedTileId East {
            get {
                return new UnwrappedTileId(Z, X + 1, Y);
            }
        }

        public UnwrappedTileId South {
            get {
                return new UnwrappedTileId(Z, X, Y + 1);
            }
        }

        public UnwrappedTileId West {
            get {
                return new UnwrappedTileId(Z, X - 1, Y);
            }
        }

        public UnwrappedTileId NorthEast {
            get {
                return new UnwrappedTileId(Z, X + 1, Y - 1);
            }
        }

        public UnwrappedTileId SouthEast {
            get {
                return new UnwrappedTileId(Z, X + 1, Y + 1);
            }
        }

        public UnwrappedTileId NorthWest {
            get {
                return new UnwrappedTileId(Z, X - 1, Y - 1);
            }
        }

        public UnwrappedTileId SouthWest {
            get {
                return new UnwrappedTileId(Z, X - 1, Y + 1);
            }
        }
    }
}
