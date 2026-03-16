using System;

public class RecvBuffer
{
    private ArraySegment<byte> _buffer;
    private int _readPos;
    private int _writePos;


    public RecvBuffer(int bufferSize)
    {
        _buffer = new ArraySegment<byte>(new byte[bufferSize],  0,  bufferSize);
    }

    public int DataSize => _writePos - _readPos; 
    public int FreeSize =>  _buffer.Count - _writePos;

    public ArraySegment<byte> readSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);

    public ArraySegment<byte> writeSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);

    public bool OnRead(int len)
    {
        if(len > DataSize) return false;
        _readPos += len;

        return true;
    }

    public bool OnWrite(int len)
    {
        if(len > FreeSize) return false;
        _writePos += len;

        return true;
    }

    public void Clean()
    {
        int dataSize = DataSize;

        if (dataSize == 0)
        {
            _readPos = 0;
            _writePos = 0;
        }
        else
        {
            Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
            _readPos = 0;
            _writePos = dataSize;
        }
    }
}
