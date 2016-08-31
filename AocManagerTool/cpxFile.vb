Public Class cpxFile

  Public ReadOnly Property FileName As String

  Public ReadOnly Property CampaignName As String
    Get
      Return _Encoding.GetString(_CampaignName)
    End Get
  End Property

  Public ReadOnly Property ScenarioCount As Integer
    Get
      Return _Scenarios.Count
    End Get
  End Property

  Default Public ReadOnly Property Scenarios(ByVal index As Integer) As scxFile
    Get
      Return _Scenarios(index)
    End Get
  End Property

  Public Property Encoding As Text.Encoding
    Get
      Return _Encoding
    End Get
    Set(value As Text.Encoding)
      _Encoding = value
      For Each x In _Scenarios
        x.Encoding = value
      Next
    End Set
  End Property

  Public Sub New(ByVal stream As IO.Stream, ByVal encoding As Text.Encoding)
    _Encoding = encoding
    Using br As New IO.BinaryReader(stream)
      _Signature = br.ReadBytes(4)
      If _GetVersion() = _CpxVersion.CpxVersion_2 Then
        For i = 1 To br.ReadInt32()
          _UnknownInt32s.Add(br.ReadInt32())
        Next
      End If
      _CampaignName = br.ReadBytes(256)
      For i As Integer = 0 To br.ReadInt32() - 1
        Dim sceLength As Integer = br.ReadInt32()
        Dim sceOffset As Integer = br.ReadInt32()
        Select Case _GetVersion()
          Case _CpxVersion.CpxVersion_1
            _ScenarioNames.Add(br.ReadBytes(255))
            _ScenarioNamesWithExtension.Add(br.ReadBytes(257))
          Case _CpxVersion.CpxVersion_2
            Dim SceNameLen = br.ReadInt16()
            br.ReadInt16()
            _ScenarioNames.Add(br.ReadBytes(SceNameLen))
            SceNameLen = br.ReadInt16()
            br.ReadInt16()
            _ScenarioNamesWithExtension.Add(br.ReadBytes(SceNameLen))
        End Select
        Dim pos As Long = stream.Position
        stream.Seek(sceOffset, IO.SeekOrigin.Begin)
        _Scenarios.Add(New scxFile(br.ReadBytes(sceLength), encoding))
        stream.Seek(pos, IO.SeekOrigin.Begin)
      Next
    End Using
  End Sub

  Public Sub New(ByVal stream As IO.Stream)
    Me.New(stream, Text.Encoding.ASCII)
  End Sub

  Public Sub New(ByVal fileName As String, ByVal encoding As Text.Encoding)
    Me.New(New IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read), encoding)
    _FileName = fileName
  End Sub

  Public Sub New(ByVal fileName As String)
    Me.New(New IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read))
    _FileName = fileName
  End Sub

  Public Function GetStream() As IO.MemoryStream
    Dim ms As New IO.MemoryStream
    Dim q As New Queue(Of Integer)
    Dim bw As New IO.BinaryWriter(ms)
    bw.Write(_Signature)
    If _GetVersion() = _CpxVersion.CpxVersion_2 Then
      bw.Write(_UnknownInt32s.Count)
      _UnknownInt32s.ForEach(Sub(x) bw.Write(x))
    End If
    bw.Write(_CampaignName)
    bw.Write(_Scenarios.Count)
    For i = 0 To _Scenarios.Count - 1
      bw.Write(_Scenarios(i).GetBytes().Length)
      q.Enqueue(ms.Position)
      bw.Write(0I) ' Placeholder for scenario offset
      Select Case _GetVersion()
        Case _CpxVersion.CpxVersion_1
          bw.Write(_ScenarioNames(i))
          bw.Write(_ScenarioNamesWithExtension(i))
        Case _CpxVersion.CpxVersion_2
          bw.Write(CShort(_ScenarioNames(i).Length))
          bw.Write(&HA60S)
          bw.Write(_ScenarioNames(i))
          bw.Write(CShort(_ScenarioNamesWithExtension(i).Length))
          bw.Write(&HA60S)
          bw.Write(_ScenarioNamesWithExtension(i))
      End Select
    Next
    For i = 0 To _Scenarios.Count - 1
      Dim pos As Integer = ms.Position
      ms.Seek(q.Dequeue(), IO.SeekOrigin.Begin)
      bw.Write(pos)
      ms.Seek(pos, IO.SeekOrigin.Begin)
      bw.Write(Scenarios(i).GetBytes())
    Next
    ms.Seek(0, IO.SeekOrigin.Begin)
    Return ms
  End Function

  Public Sub Save()
    Using fs As New IO.FileStream(_FileName, IO.FileMode.Create)
      GetStream().CopyTo(fs)
    End Using
  End Sub

  Public Sub SaveAs(ByVal fileName As String)
    Using fs As New IO.FileStream(fileName, IO.FileMode.Create)
      GetStream().CopyTo(fs)
    End Using
  End Sub

  Private Function TranscodeFixed(ByVal newEncoding As Text.Encoding, ByVal y As Byte()) As Byte()
    Dim ret(y.Length - 1) As Byte
    Dim Converted() As Byte = Text.Encoding.Convert(_Encoding, newEncoding, y)
    Buffer.BlockCopy(Converted, 0, ret, 0, Math.Min(Converted.Length, y.Length))
    Return ret
  End Function

  Public Sub Transcode(ByVal newEncoding As Text.Encoding)
    For i = 0 To _Scenarios.Count - 1
      _ScenarioNames(i) = TranscodeFixed(newEncoding, _ScenarioNames(i))
      _ScenarioNamesWithExtension(i) = TranscodeFixed(newEncoding, _ScenarioNamesWithExtension(i))
      Scenarios(i).Transcode(newEncoding)
    Next
    _Encoding = newEncoding
  End Sub

  Private Function GetBytesFixed(ByVal s As String, ByVal length As Integer) As Byte()
    Dim y(length - 1) As Byte
    _Encoding.GetBytes(s).Take(length).ToArray().CopyTo(y, 0)
    Return y
  End Function

  Private _Encoding As Text.Encoding
  Private _Signature As Byte()
  Private _UnknownInt32s As New List(Of Integer)
  Private _CampaignName As Byte()
  Private _ScenarioNames As New List(Of Byte())
  Private _ScenarioNamesWithExtension As New List(Of Byte())
  Private _Scenarios As New List(Of scxFile)

  Private Enum _CpxVersion
    CpxVersion_1
    CpxVersion_2
    Unknown
  End Enum
  Private Function _GetVersion() As _CpxVersion
    Select Case _Signature(0)
      Case &H31 ' "1.00"
        Return _CpxVersion.CpxVersion_1
      Case &H32 ' "2.00"
        Return _CpxVersion.CpxVersion_2
    End Select
    Return _CpxVersion.Unknown
  End Function

End Class
