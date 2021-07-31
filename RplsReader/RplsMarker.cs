using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RplsReader
{
    class RplsMarker
    {
        public RplsMarkerItem[] Items { get; private set; }

        private int itemCount;

        class Offset
        {
            public static readonly int LENGTH = 0; // UInt32
            public static readonly int ITEM_COUNT = 4; // UInt16
            public static readonly int PAYLOAD = 6;
        }

        public static RplsMarker Parse(byte[] buffer, int offset, RplsPlaylist playlist)
        {
            RplsMarker marker = new RplsMarker();

            marker.itemCount = (int)Rpls.ToUInt16(buffer, offset + Offset.ITEM_COUNT);

            marker.Items = new RplsMarkerItem[marker.itemCount];

            for(int i = 0; i < marker.itemCount; i++)
            {
                marker.Items[i] = RplsMarkerItem.Parse(buffer, offset + Offset.PAYLOAD + RplsMarkerItem.SIZE*i, playlist);
            }

            return marker;
        }

    }

    class RplsMarkerItem
    {
        public bool Valid { get; private set; }

        [JsonIgnore]
        public int Type { get; private set; }
        public int PlaylistItemId { get; private set; }
        [JsonIgnore]
        public TimeSpan Time { get; private set; }

        public string TimeText { get; private set; }

        public int TimeMillisecond { get; private set; }

        [JsonIgnore]
        public RplsPlaylistItem PlaylistItem { get; private set; }

        [JsonIgnore]
        public uint RawTime { get; private set; }
        [JsonIgnore]
        public uint RawDuration { get; private set; }

        public static readonly int SIZE = 46;

        class Offset
        {
            public static readonly int TYPE = 0;        // byte (when LSB is 1, marker is invalid)
            public static readonly int NAME_LENGTH = 1; // byte
            public static readonly int MAKER_ID = 2;    // UInt16
            public static readonly int PLAYLIST_ITEM_ID = 4;    // UInt16
            public static readonly int TIME = 6;                // UInt32
            public static readonly int PID = 10;                // UInt16
            public static readonly int THUMBNAIL_ID = 12;       // UInt16
            public static readonly int DURATION = 14;           // UInt32
            public static readonly int MAKERS_INFO = 18;        // UInt32
            public static readonly int NAME = 22;               // byte[24]
        }

        public static RplsMarkerItem Parse(byte[] buffer, int offset, RplsPlaylist playlist)
        {
            RplsMarkerItem item = new RplsMarkerItem();

            item.Type = buffer[offset + Offset.TYPE];
            item.Valid = (item.Type & 0x01) == 0;

            item.PlaylistItemId = Rpls.ToUInt16(buffer, offset + Offset.PLAYLIST_ITEM_ID);

            item.RawTime = Rpls.ToUInt32(buffer, offset + Offset.TIME);
            item.RawDuration = Rpls.ToUInt32(buffer, offset + Offset.DURATION);

            if (playlist != null)
            {
                item.PlaylistItem = playlist.Items[item.PlaylistItemId];
                item.Time = new TimeSpan( (item.RawTime - item.PlaylistItem.In)/45U*TimeSpan.TicksPerMillisecond);

                item.TimeMillisecond = (int)item.Time.TotalMilliseconds;
                item.TimeText = item.Time.ToString();
            }

            return item;
        }
    }
}
