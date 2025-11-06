using System;
using System.Collections.Generic;
using System.IO;
/*
namespace CottonLibrary.Save
{
    // i literally stole this code from myself. this is from a game called "liminal" i was helping my friends with and i made a whole save data system for it. This is a modified version designed for mod compatibility
    // " i dont care
    // do whatever you want "
    // - py8 (the main dev)
    public abstract class ModdedSaveData<T> where T : ModdedSaveData<T>
    {
        
        public ModdedSaveData(BinaryReader reader, BinaryWriter writer)
        {
            Writer = writer;
            Reader = reader;
        }
        
        public ModdedSaveData() : this(null, null) { }
        
        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }
        
        #region COMPONENT IDENTIFIERS

        public virtual int ComponentVersion => 0;
        public virtual string ComponentIdentifier => "MODDED";
        
        #endregion
        
        # region WRITE VALUES
        public void Write(string value)
        {
            Writer.Write(value);
        }
        public void Write(bool value)
        {
            Writer.Write(value);
        }
        public void Write(int value)
        {
            Writer.Write(value);
        }
        public void Write(uint value)
        {
            Writer.Write(value);
        }
        public void Write(float value)
        {
            Writer.Write(value);
        }
        public void Write(short value)
        {
            Writer.Write(value);
        }
        public void Write(long value)
        {
            Writer.Write(value);
        }
        public void Write(ushort value)
        {
            Writer.Write(value);
        }
        public void Write(ulong value)
        {
            Writer.Write(value);
        }
        public void Write(double value)
        {
            Writer.Write(value);
        }
        public void Write(decimal value)
        {
            Writer.Write(value);
        }
        public void Write(byte value)
        {
            Writer.Write(value);
        }
        public void Write(sbyte value)
        {
            Writer.Write(value);
        }   
        #endregion

        # region WRITE LISTS
        public void WriteList(List<string> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<bool> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<int> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<uint> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<float> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<double> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<decimal> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<short> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<ushort> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<long> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<ulong> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<byte> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        public void WriteList(List<sbyte> values)
        {
            Write(values.Count);
            foreach (var value in values)
                Write(value);
        }
        #endregion
        
        # region READ VALUES
        public string ReadString()
        {
            return Reader.ReadString();
        }
        public bool ReadBool()
        {
            return Reader.ReadBoolean();
        }
        public int ReadInt()
        {
            return Reader.ReadInt32();
        }
        public uint ReadUInt()
        {
            return Reader.ReadUInt32();
        }
        public float ReadFloat()
        {
            return Reader.ReadSingle();
        }
        public short ReadShort()
        {
            return Reader.ReadInt16();
        }
        public long ReadLong()
        {
            return Reader.ReadInt64();
        }
        public ushort ReadUShort()
        {
            return Reader.ReadUInt16();
        }
        public ulong ReadULong()
        {
            return Reader.ReadUInt64();
        }
        public double ReadDouble()
        {
            return Reader.ReadDouble();
        }
        public decimal ReadDecimal()
        {
            return Reader.ReadDecimal();
        }
        public byte ReadByte()
        {
            return Reader.ReadByte();
        }
        public sbyte ReadSByte()
        {
            return Reader.ReadSByte();
        }   
        #endregion
        
        # region READ LISTS
        public List<string> ReadStringList()
        {
            int length = ReadInt();
            List<string> values = new List<string>();
            for (int i = 0; i < length; i++)
                values.Add(ReadString());
            return values;
        }
        public List<bool> ReadBoolList()
        {
            int length = ReadInt();
            List<bool> values = new List<bool>();
            for (int i = 0; i < length; i++)
                values.Add(ReadBool());
            return values;
        }
        public List<int> ReadIntList()
        {
            int length = ReadInt();
            List<int> values = new List<int>();
            for (int i = 0; i < length; i++)
                values.Add(ReadInt());
            return values;
        }
        public List<uint> ReadUIntList()
        {
            int length = ReadInt();
            List<uint> values = new List<uint>();
            for (int i = 0; i < length; i++)
                values.Add(ReadUInt());
            return values;
        }
        public List<float> ReadFloatList()
        {
            int length = ReadInt();
            List<float> values = new List<float>();
            for (int i = 0; i < length; i++)
                values.Add(ReadFloat());
            return values;
        }
        public List<short> ReadShortList()
        {
            int length = ReadInt();
            List<short> values = new List<short>();
            for (int i = 0; i < length; i++)
                values.Add(ReadShort());
            return values;
        }
        public List<long> ReadLongList()
        {
            int length = ReadInt();
            List<long> values = new List<long>();
            for (int i = 0; i < length; i++)
                values.Add(ReadLong());
            return values;
        }
        public List<ushort> ReadUShortList()
        {
            int length = ReadInt();
            List<ushort> values = new List<ushort>();
            for (int i = 0; i < length; i++)
                values.Add(ReadUShort());
            return values;
        }
        public List<ulong> ReadULongList()
        {
            int length = ReadInt();
            List<ulong> values = new List<ulong>();
            for (int i = 0; i < length; i++)
                values.Add(ReadULong());
            return values;
        }
        public List<double> ReadDoubleList()
        {
            int length = ReadInt();
            List<double> values = new List<double>();
            for (int i = 0; i < length; i++)
                values.Add(ReadDouble());
            return values;
        }
        public List<decimal> ReadDecimalList()
        {
            int length = ReadInt();
            List<decimal> values = new List<decimal>();
            for (int i = 0; i < length; i++)
                values.Add(ReadDecimal());
            return values;
        }
        public List<byte> ReadByteList()
        {
            int length = ReadInt();
            List<byte> values = new List<byte>();
            for (int i = 0; i < length; i++)
                values.Add(ReadByte());
            return values;
        }
        public List<sbyte> ReadSByteList()
        {
            int length = ReadInt();
            List<sbyte> values = new List<sbyte>();
            for (int i = 0; i < length; i++)
                values.Add(ReadSByte());
            return values;
        }   
        #endregion

        public abstract void WriteComponent();
        public abstract void ReadComponent();
        
        public abstract void UpgradeComponent(T old);

        public void WriteData()
        {
            Write(ComponentVersion);
            Write(ComponentIdentifier);
            
            WriteComponent();
        }
        public bool ReadData(int versionFrom = 0)
        {
            var version = versionFrom == 0 ? ReadInt() : versionFrom;
            
            if (version != ComponentVersion)
            {
                var constructor = typeof(T).GetConstructor(new[] { typeof(BinaryReader), typeof(BinaryWriter) });
                if (constructor == null)
                {
                    throw new InvalidOperationException($"Save Component {typeof(T)} must have a constructor with (BinaryReader, BinaryWriter) parameters!");
                }
                var old = (T)constructor.Invoke(new object[] { Reader, null });
                old.ReadData(version);
                UpgradeComponent(old);
            }
            if (versionFrom == 0)
                ReadComponent();
            
            return true;
        } 

    }
}*/