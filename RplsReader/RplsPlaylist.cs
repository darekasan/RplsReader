using System;
using System.Collections.Generic;
using System.Text;

namespace RplsReader
{
    public class RplsPlaylist
    {
        public RplsPlaylistItem[] Items { get; private set; }
        class Offset
        {
            public static readonly int LENGTH = 0;      // UInt32
            public static readonly int TYPE = 4;        // UInt16
            public static readonly int ITEM_COUNT = 6;  // UInt16
            public static readonly int SUB_ITEM_COUNT = 8; // UInt16
            public static readonly int PAYLOAD = 10;
        }
        public static RplsPlaylist Parse(byte[] buffer, int offset)
        {
            RplsPlaylist playlist = new RplsPlaylist();

            int itemCount = Rpls.ToUInt16(buffer, offset + Offset.ITEM_COUNT);

            playlist.Items = new RplsPlaylistItem[itemCount];
            
            for(int i = 0; i < itemCount; i++)
            {
                playlist.Items[i] = RplsPlaylistItem.Parse(buffer, offset + Offset.PAYLOAD + RplsPlaylistItem.SIZE * i);
            }

            return playlist;
        }
     }


    public class RplsPlaylistItem
    {
        public static readonly int SIZE = 22;

        public string Filename { get; private set; }

        public uint In { get; private set; }
        public uint Out { get; private set; }

        class Offset
        {
            public static readonly int LENGTH = 0;                      // UInt16
            public static readonly int FILENAME = 2;                    // byte[5]
            public static readonly int EXT = 7;                         // byte[4]
            public static readonly int CONNECTION_CONDITION = 12;       // byte
            public static readonly int STC_ID = 13;                     // byte
            public static readonly int IN = 14;                         // UInt32
            public static readonly int OUT = 18;                        // UInt32
        }

        public static RplsPlaylistItem Parse(byte[] buffer, int offset)
        {
            RplsPlaylistItem item = new RplsPlaylistItem();
            item.Filename = 
                System.Text.Encoding.ASCII.GetString(buffer, offset + Offset.FILENAME, 5)+
                "."+
                System.Text.Encoding.ASCII.GetString(buffer, offset + Offset.EXT, 4);

            item.In = Rpls.ToUInt32(buffer, offset + Offset.IN);
            item.Out = Rpls.ToUInt32(buffer, offset + Offset.OUT);

            return item;
        }
    }
}
