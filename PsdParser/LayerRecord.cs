﻿using System.Collections.Generic;

namespace PsdParser
{
    public class LayerRecord
    {
        public int Top { get; }
        public int Left { get; }
        public int Bottom { get; }
        public int Right { get; }

        public ushort Channels { get; }
        public ChannelInformation[] ChannelInfos { get; }

        public BlendMode BlendMode { get; }
        public byte Opacity { get; }
        public bool Clipping { get; }
        public LayerFlags LayerFlags { get; }
        public uint ExtraDataLength { get; }
        public LayerMaskAndAdjustmentLayerData LayerMaskAndAdjustmentLayerData { get; }
        public LayerBlendingRangesData LayerBlendingRangesData { get; }
        public string LayerName { get; }

        public AdditionalLayerInformation[] AdditionalLayerInformations { get; }

        internal LayerRecord(PsdBinaryReader reader, bool isPSB)
        {
            Top = reader.ReadInt32();
            Left = reader.ReadInt32();
            Bottom = reader.ReadInt32();
            Right = reader.ReadInt32();

            Channels = reader.ReadUInt16();
            ChannelInfos = new ChannelInformation[Channels];
            for (int i = 0; i < Channels; i++)
                ChannelInfos[i] = new ChannelInformation(reader, isPSB);

            var signature = new string(reader.ReadChars(4));
            if (signature != "8BIM")
                throw new InvalidSignatureException(signature);

            BlendMode = (BlendMode)reader.ReadInt32();
            Opacity = reader.ReadByte();
            Clipping = reader.ReadByte() is 1;
            LayerFlags = (LayerFlags)reader.ReadByte();
            reader.BaseStream.Position += 1;

            ExtraDataLength = reader.ReadUInt32();
            var extraDataPosition = reader.BaseStream.Position;

            LayerMaskAndAdjustmentLayerData = new LayerMaskAndAdjustmentLayerData(reader);
            LayerBlendingRangesData = new LayerBlendingRangesData(reader);
            LayerName = reader.ReadPascalString(4);

            var maxPadding = AdditionalLayerInformation.MinSize(isPSB);
            var additional = new List<AdditionalLayerInformation>();
            while (maxPadding <= extraDataPosition + ExtraDataLength - reader.BaseStream.Position)
                additional.Add(AdditionalLayerInformation.Parse(reader, isPSB));
            AdditionalLayerInformations = additional.ToArray();
            if (extraDataPosition + ExtraDataLength - reader.BaseStream.Position < maxPadding)
                reader.BaseStream.Position = extraDataPosition + ExtraDataLength;

            InvalidStreamPositionException.ThrowIfInvalid(reader,extraDataPosition,ExtraDataLength);
        }
    }
}