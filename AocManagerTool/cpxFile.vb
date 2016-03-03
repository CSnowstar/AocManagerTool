Public Class cpxFile

  Private _FileName As String
  Private _Signature As Byte()
  Public Property CampaignName As String
  Public Property Scenarios As New Dictionary(Of String, scxFile)
  Public Property Encoding As Text.Encoding = Text.Encoding.ASCII

  Public Sub New(ByVal stream As IO.Stream, ByVal encoding As Text.Encoding)
    Me.Encoding = encoding
    Using br As New IO.BinaryReader(stream)
      _Signature = br.ReadBytes(4)
      CampaignName = encoding.GetString(br.ReadBytes(256))
      For i As Integer = 1 To br.ReadInt32()
        Dim sceLength As Integer = br.ReadInt32()
        Dim sceOffset As Integer = br.ReadInt32()
        br.ReadBytes(255)
        Dim sceName As String = encoding.GetString(br.ReadBytes(257))
        Dim pos As Long = stream.Position
        stream.Seek(sceOffset, IO.SeekOrigin.Begin)
        Scenarios.Add(sceName, New scxFile(br.ReadBytes(sceLength)))
        stream.Seek(pos, IO.SeekOrigin.Begin)
      Next
    End Using
  End Sub

  Public Sub New(ByVal stream As IO.Stream)
    Me.New(stream, Text.Encoding.ASCII)
  End Sub

  Public Sub New(ByVal fileName As String)
    Me.New(New IO.FileStream(fileName, IO.FileMode.Open))
    _FileName = fileName
  End Sub

  Public Function SaveAsStream() As IO.MemoryStream
    Dim ms As New IO.MemoryStream
    Dim q As New Queue(Of Integer)
    Dim bw As New IO.BinaryWriter(ms)
    bw.Write(_Signature)
      bw.Write(GetBytesFixed(CampaignName, 256))
      bw.Write(Scenarios.Count)
      For Each x In Scenarios
        bw.Write(x.Value.GetBytes().Length)
        q.Enqueue(ms.Position)
        bw.Write(0I)
        bw.Write(GetBytesFixed(IO.Path.GetFileNameWithoutExtension(x.Key), 255))
        bw.Write(GetBytesFixed(x.Key, 257))
      Next
      For Each x In Scenarios
        Dim pos As Integer = ms.Position
        ms.Seek(q.Dequeue(), IO.SeekOrigin.Begin)
        bw.Write(pos)
        ms.Seek(pos, IO.SeekOrigin.Begin)
        bw.Write(x.Value.GetBytes())
      Next
    Return ms
  End Function

  Private Function GetStringFixed(ByVal br As IO.BinaryReader, ByVal length As Integer) As String
    Return Encoding.GetString(br.ReadBytes(length))
  End Function

  Private Function GetString16(ByVal br As IO.BinaryReader) As String
    Return Encoding.GetString(br.ReadBytes(br.ReadInt16()))
  End Function

  Private Function GetString32(ByVal br As IO.BinaryReader) As String
    Return Encoding.GetString(br.ReadBytes(br.ReadInt32()))
  End Function

  Private Function GetBytesFixed(ByVal s As String, ByVal length As Integer) As Byte()
    Dim y(length - 1) As Byte
    Encoding.GetBytes(s).Take(length).ToArray().CopyTo(y, 0)
    Return y
  End Function

  Private Sub WriteString16(ByVal bw As IO.BinaryWriter, ByVal s As String)
    Dim y As Byte() = Encoding.GetBytes(s)
    bw.Write(CShort(y.Length))
    bw.Write(y)
  End Sub

  Private Sub WriteString32(ByVal bw As IO.BinaryWriter, ByVal s As String)
    Dim y As Byte() = Encoding.GetBytes(s)
    bw.Write(y.Length)
    bw.Write(y)
  End Sub
End Class
