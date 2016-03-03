Public Class drsFile
  Inherits Dictionary(Of String, SortedDictionary(Of UInteger, Byte()))

  Private _FileName As String
  Private _Signature As Byte()
  Private _NumTables As UInteger
  Private _FirstOffset As UInteger
  Private _IndexTableDict As Dictionary(Of UInteger, String) = New Dictionary(Of UInteger, String)

  Public Sub New()

  End Sub

  Public Sub New(ByVal fileName As String)
    _FileName = fileName
    Using fs As IO.FileStream = New IO.FileStream(_FileName, IO.FileMode.Open, IO.FileAccess.Read)
      Using br As IO.BinaryReader = New IO.BinaryReader(fs)
        _Signature = br.ReadBytes(56)
        _NumTables = br.ReadUInt32()
        _FirstOffset = br.ReadUInt32()
        For i As UInteger = 0 To _NumTables - 1
          Dim TableName As String = Text.Encoding.ASCII.GetString(br.ReadBytes(4))
          _IndexTableDict.Add(i, TableName)
          Add(TableName, New SortedDictionary(Of UInteger, Byte()))
          Dim TableOffset As UInteger = br.ReadUInt32()
          Dim NumFilesInTable As UInteger = br.ReadUInt32()
          Dim Position As Stack(Of Long) = New Stack(Of Long)
          Position.Push(fs.Position)
          fs.Seek(TableOffset, IO.SeekOrigin.Begin)
          For j As UInteger = 0 To NumFilesInTable - 1
            Dim EntryId As UInteger = br.ReadUInt32()
            Dim EntryOffset As UInteger = br.ReadUInt32()
            Dim EntrySize As UInteger = br.ReadUInt32()
            Position.Push(fs.Position)
            fs.Seek(EntryOffset, IO.SeekOrigin.Begin)
            Me(TableName).Add(EntryId, br.ReadBytes(EntrySize))
            fs.Seek(Position.Pop(), IO.SeekOrigin.Begin)
          Next
          fs.Seek(Position.Pop(), IO.SeekOrigin.Begin)
        Next
      End Using
    End Using
  End Sub

  Public Sub Save()
    Using fs As IO.FileStream = New IO.FileStream(_FileName, IO.FileMode.Create, IO.FileAccess.Write)
      Using bw As IO.BinaryWriter = New IO.BinaryWriter(fs)
        Dim Position As Queue(Of Long) = New Queue(Of Long)
        Dim Stack As Stack(Of Long) = New Stack(Of Long)
        bw.Write(_Signature)
        bw.Write(CUInt(Count))
        bw.Write(0I)
        For Each Table In Me
          bw.Write(Text.Encoding.ASCII.GetBytes(Table.Key))
          Position.Enqueue(fs.Position)
          bw.Write(0I)
          bw.Write(CUInt(Table.Value.Count))
        Next
        For Each Table In Me
          Stack.Push(fs.Position)
          fs.Seek(Position.Dequeue(), IO.SeekOrigin.Begin)
          bw.Write(CUInt(Stack.Peek()))
          fs.Seek(Stack.Pop(), IO.SeekOrigin.Begin)
          For Each Entry In Table.Value
            bw.Write(Entry.Key)
            Position.Enqueue(fs.Position)
            bw.Write(0I)
            bw.Write(CUInt(Entry.Value.Length))
          Next
        Next
        Stack.Push(fs.Position)
        fs.Seek(60, IO.SeekOrigin.Begin)
        bw.Write(CUInt(Stack.Peek()))
        fs.Seek(Stack.Pop(), IO.SeekOrigin.Begin)
        For Each Table In Me
          For Each Entry In Table.Value
            Stack.Push(fs.Position)
            fs.Seek(Position.Dequeue(), IO.SeekOrigin.Begin)
            bw.Write(CUInt(Stack.Peek()))
            fs.Seek(Stack.Pop(), IO.SeekOrigin.Begin)
            bw.Write(Entry.Value)
          Next
        Next
      End Using
    End Using
  End Sub

  Public Sub SaveAs(ByVal fileName As String)
    _FileName = fileName
    Save()
  End Sub

End Class
