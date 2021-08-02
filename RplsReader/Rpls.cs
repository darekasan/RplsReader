using System;
using System.Collections.Generic;
using System.Text;
using AribB24.DotNet;
using System.Text.Json.Serialization;

namespace RplsReader
{
    public class Rpls
    {
        [JsonIgnore]
        public int Version { get; private set; }
        public DateTime Date { get; private set; }
        public string ChannelName { get; private set; }
        public string Title { get; private set; }
        public string Detail { get; private set; }

        public RplsPlaylist Playlist { get; private set; }
        public RplsMarker Marker { get; private set; }
        

        private int playlistOffset;
        private int markerOffset;
        private int vendorSpecificOffset;

        class Offset
        {
            public static readonly int VERSION = 4;                // int
            public static readonly int PLAYLIST_OFFSET = 8;        // int
            public static readonly int MARKER_OFFSET = 12;         // int
            public static readonly int VENDOR_SPECIFIC_OFFSET = 16;// int
            public static readonly int DATE = 50;                  // byte[7]
            public static readonly int DURATION = 57;              // byte[3]
            public static readonly int VENDOR_ID = 60;             // UInt16
            public static readonly int MODEL_ID = 62;              // int
            public static readonly int CHANNEL_NUM = 65;           // UInt16
            public static readonly int CHANNEL_NAME_LENGTH = 67;   // byte
            public static readonly int CHANNEL_NAME = 68;          // byte[20]
            public static readonly int TITLE_LENGTH = 88;          // byte
            public static readonly int TITLE = 89;                 // byte[255]
            public static readonly int DETAIL_LENGTH = 344;        // UInt16
            public static readonly int DETAIL = 346;                // byte[1200]
        }

        public static Rpls Parse(string path)
        {
            byte[] rawRpls = System.IO.File.ReadAllBytes(path);
            return Parse(rawRpls, 0);
        }

        public static Rpls Parse(byte[] buffer, int offset)
        {
            var rpls = new Rpls();
            var decoder = new B24Decoder();
            rpls.Version = buffer[offset + Offset.VERSION];

            rpls.playlistOffset = (int)ToUInt32(buffer, offset + Offset.PLAYLIST_OFFSET);
            rpls.markerOffset = (int)ToUInt32(buffer, offset + Offset.MARKER_OFFSET);
            rpls.vendorSpecificOffset = (int)ToUInt32(buffer, offset + Offset.VENDOR_SPECIFIC_OFFSET);

            rpls.Date = new DateTime(
                DecodeBcd(buffer, offset + Offset.DATE, 2), // Year
                DecodeBcd(buffer[offset + Offset.DATE + 2]), // Month
                DecodeBcd(buffer[offset + Offset.DATE + 3]), // Day
                DecodeBcd(buffer[offset + Offset.DATE + 4]), // Hour
                DecodeBcd(buffer[offset + Offset.DATE + 5]), // Minute
                DecodeBcd(buffer[offset + Offset.DATE + 6])); // Second

            rpls.ChannelName = decoder.GetString(
                buffer,
                offset + Offset.CHANNEL_NAME,
                buffer[offset + Offset.CHANNEL_NAME_LENGTH]);

            rpls.Title = decoder.GetString(
                buffer,
                offset + Offset.TITLE,
                buffer[offset + Offset.TITLE_LENGTH]);

            int detailLength = ToUInt16(buffer, Offset.DETAIL_LENGTH);
            int detail2Length = 0;
            for (int i = offset + Offset.DETAIL + detailLength; i < offset + Offset.DETAIL + 1200; i++, detail2Length++)
            {
                if (buffer[i] == 0) break;
            }

            rpls.Detail = 
                decoder.GetString(
                    buffer,
                    offset + Offset.DETAIL,
                    ToUInt16(buffer,Offset.DETAIL_LENGTH)
                ).Trim() + "\r\n\r\n" +
                decoder.GetString(
                    buffer,
                    offset + Offset.DETAIL+ detailLength,
                    detail2Length
                ).Trim();

            rpls.Playlist = RplsPlaylist.Parse(buffer, offset + rpls.playlistOffset);
            rpls.Marker = RplsMarker.Parse(buffer, offset + rpls.markerOffset, rpls.Playlist);

            return rpls;
        }
        
        // 0x45, 0x12 → 4512みたいに変換するやつ(BE)
        public static int DecodeBcd(byte[] buffer,int offset,int length)
        {
            int result = 0;
            for(int i = 0; i < length; i++)
            {
                result += DecodeBcd(buffer[length-1-i+offset]) * (int)Math.Pow(10,2*i);
            }
            return result;
        }

        // 0x45 → 45みたいに変換するやつ
        public static int DecodeBcd(byte value)
        {
            return (((value & 0xf0) / 0x10) * 10) + (value & 0x0f);
        }

        public static UInt32 ToUInt32(byte[] b,int o)
        {
            return 
                ((UInt32)b[o] << 24) |
                ((UInt32)b[o + 1] << 16) |
                ((UInt32)b[o + 2] << 8) |
                ((UInt32)b[o + 3])
                ;
        }

        public static UInt16 ToUInt16(byte[] b, int o)
        {
            return (UInt16)
                ((b[o] << 8) |
                b[o + 1])
                ;
        }
    }
}
