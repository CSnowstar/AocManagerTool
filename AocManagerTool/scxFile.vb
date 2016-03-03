Public Class scxFile

#Region "Definitions"
  Const PlayerNameLength = 256
  Const PlayerCount = 16

  Public Enum InfoType
    Instruction
    Hints
    Victory
    Defeat
    History
    Scouts
    PregameCinematic
    VictoryCinematic
    LossCinematic
    BackgroundPicture
  End Enum

  Public Enum EffectField
    AIGoal
    Amount
    Resource
    Diplomacy
    NumSelected
    LocationUnit
    UnitID
    PlayerSource
    PlayerTarget
    Technology
    StringTableID
    Unknown
    DisplayTime
    Trigger
    LocationX
    LocationY
    AreaSWX
    AreaSWY
    AreaNEX
    AreaNEY
    UnitGroup
    UnitType
    InstructionPanel
  End Enum

  Public Enum ConditionField
    Amount
    Resource
    UnitObject
    UnitLocation
    UnitClass
    Player
    Technology
    Time
    Unknown
    AreaSWX
    AreaSWY
    AreaNEX
    AreaNEY
    UnitGroup
    UnitType
    AiSignal
  End Enum

  Public Enum VictoryMode As Integer
    Standard
    Conquest
    Score
    Timed
    Custom
  End Enum

  Public Enum DiplomacyI As Integer
    Allied
    Neutral
    Enemy = 3
  End Enum

  Public Enum DiplomacyB As Byte
    Allied
    Neutral
    Enemy = 3
  End Enum

  Public Enum Diplomacy2 As Integer
    Gaia
    Self
    Allied
    Neutral
    Enemy
  End Enum

  Public Enum StartAge As Integer
    None = -1
    Dark
    Feudal
    Castle
    Imperial
    PostImperial
  End Enum

  Public Enum AiMapType As Integer
    Arabia = 9
    Archipelago
    Baltic
    BlackForest
    Coastal
    Continental
    CraterLake
    Fortress
    GoldRush
    Highland
    Islands
    Mediterranean
    Migration
    Rivers
    TeamIslands
    Scandinavia = &H19
    Yucatan = &H1B
    SaltMarsh
    KingOfTheHill = &H1E
    Oasis
    Nomad = &H21
  End Enum

  Public Enum Color As Integer
    Blue
    Red
    Green
    Yellow
    Cyan
    Purple
    Gray
    Orange
  End Enum

  Public Enum EffectType As Integer
    ChangeDiplomacy = 1
    ResearchTechnology
    SendChat
    PlaySound
    SendTribute
    UnlockGate
    LockGate
    ActivateTrigger
    DeactivateTrigger
    AIScriptGoal
    CreateObject
    TaskObject
    DeclareVictory
    KillObject
    RemoveObject
    ChangeView
    Unload
    ChangeOwnership
    Patrol
    DisplayInstructions
    ClearInstructions
    FreezeUnit
    UseAdvancedButtons
    DamageObject
    PlaceFoundation
    ChangeObjectName
    ChangeObjectHP
    ChangeObjectAttack
    StopUnit
    SnapView
    EnableTech = 32
    DisableTech
    EnableUnit
    DisableUnit
    FlashObjects
  End Enum

  Public Enum ConditionType
    BringObjectToArea
    BringObjectToObject
    OwnObjects
    OwnFewerObjects
    ObjectsInArea
    DestroyObject
    CaptureObject
    AccumulateAttribute
    ResearchTechnology
    Timer
    ObjectSelected
    AiSignal
    PlayerDefeated
    ObjectHasTarget
    ObjectVisible
    ObjectNotVisible
    ResearchingTechnology
    UnitsGarrisoned
    DifficultyLevel
    OwnFewerFoundations
    SelectedObjectsInArea
    PoweredObjectsInArea
    UnitsQueuedPastPopCap
  End Enum

  Public Class Player
    Public Property Name As BytesString
    Public Property StringTableName As Integer
    Public Property IsActive As Integer
    Public Property IsHuman As Integer
    Public Property Civilization As Integer
    Public Property Ai As BytesString
    Public Property AiFile As BytesString
    Public Property Personality As Byte
    Public Property Gold As Integer
    Public Property Wood As Integer
    Public Property Food As Integer
    Public Property Stone As Integer
    Public Property Orex As Integer
    Public Property Diplomacies As New List(Of DiplomacyI)(16)
    Public Property AlliedVictory As Integer
    Public Property DisabledTechs As New List(Of Integer)(30)
    Public Property DisabledUnits As New List(Of Integer)(30)
    Public Property DisabledBuildings As New List(Of Integer)(20)
    Public Property StartAge As StartAge
  End Class

  Public Structure Terrain
    Public Id As Byte
    Public Elevation As UShort
    Public Overrides Function ToString() As String
      Return $"{Id}, {Elevation}"
    End Function
  End Structure

  Public Class Resource
    Public Property Food As Single
    Public Property Wood As Single
    Public Property Gold As Single
    Public Property Stone As Single
    Public Property Orex As Single
    Public Property PopulationLimit As Single
    Public Overrides Function ToString() As String
      Return $"F{Food}{vbTab}W{Wood}{vbTab}G{Gold}{vbTab}S{Stone}{vbTab}Pop{PopulationLimit}"
    End Function
  End Class

  Public Class Unit
    Public Property PosX As Single
    Public Property PosY As Single
    Public Property Id As Integer
    Public Property UnitId As UShort
    Public Property Rotation As Single
    Public Property Frame As UShort
    Public Property Garrison As Integer
    Public Overrides Function ToString() As String
      Return $"{UnitId}{vbTab}{Id}{vbTab}({PosX}, {PosY})"
    End Function
  End Class

  Public Class PlayerMisc
    Public Property Name As BytesString
    Public Property CameraX As Single
    Public Property CameraY As Single
    Public Property AlliedVictory As Byte
    Public Property Diplomacy As New List(Of DiplomacyB)(9)
    Public Property Diplomacy2 As New List(Of Diplomacy2)(9)
    Public Property Color As Color
    Public Overrides Function ToString() As String
      Return $"{Name}, {Color}"
    End Function
  End Class

  Public Class Effect
    Friend _fields As New List(Of Integer)(23)
    Public Property Type As EffectType
    Public Property Fields(ByVal fieldType As EffectField) As Integer
      Get
        Return _fields(fieldType)
      End Get
      Set(value As Integer)
        _fields(fieldType) = value
      End Set
    End Property
    Public Property Text As BytesString
    Public Property SoundFile As BytesString
    Public Property UnitIDs As New List(Of Integer)
    Public Overrides Function ToString() As String
      Return Type
    End Function
  End Class

  Public Class Condition
    Friend _fields As New List(Of Integer)(16)
    Public Property Type As ConditionType
    Public Property Fields(ByVal fieldType As EffectField) As Integer
      Get
        Return _fields(fieldType)
      End Get
      Set(value As Integer)
        _fields(fieldType) = value
      End Set
    End Property
    Public Overrides Function ToString() As String
      Return Type
    End Function
  End Class

  Public Class Trigger
    Public Property IsEnabled As Integer
    Public Property IsLooping As Integer
    Public Property IsObjective As Byte
    Public Property DescriptionOrder As Integer
    Public Property Description As BytesString
    Public Property Name As BytesString
    Public Property Effects As New List(Of Effect)
    Public Property EffectOrder As New List(Of Integer)
    Public Property Conditions As New List(Of Condition)
    Public Property ConditionOrder As New List(Of Integer)
    Public Overrides Function ToString() As String
      Return $"{Name}{vbTab}{Conditions.Count} Conditions, {Effects.Count} Effects"
    End Function
  End Class

  Public Class BITMAPDIB
    Public Property biSize As Integer
    Public Property biWidth As Integer
    Public Property biHeight As Integer
    Public Property biPlanes As Short
    Public Property biBitCount As Short
    Public Property biCompression As Integer
    Public Property biSizeImage As Integer
    Public Property biXPelsPerMeter As Integer
    Public Property biYPelsPerMeter As Integer
    Public Property biClrUsed As Integer
    Public Property biClrImportant As Integer
    Public Property colors As New List(Of RGB)(256)
    Public Property imageData As Byte()
  End Class

  Public Structure RGB
    Public Property Blue As Byte
    Public Property Green As Byte
    Public Property Red As Byte
  End Structure
#End Region

  Private Property Encoding As Text.Encoding
  Public Property FileName As String

  Private _StringTableInfos As New List(Of Integer)(5)
  Private _StringInfos As New List(Of BytesString)(9)

  Private _Version As Byte()
  Public ReadOnly Property LastSave As Integer
  Public Property Instruction As BytesString
  Private _PlayerCount As Integer
  Private _NextUid As Integer
  Private _Version2 As Single
  Public Property Players As New List(Of Player)(16)
  Public Property OriginalFilename As BytesString
  Public Property StringTableInfos(ByVal type As InfoType) As Integer
    Get
      Return _StringTableInfos(type)
    End Get
    Set(value As Integer)
      _StringTableInfos(type) = value
    End Set
  End Property
  Public Property StringInfos(ByVal type As InfoType) As BytesString
    Get
      Return _StringInfos(type)
    End Get
    Set(value As BytesString)
      _StringInfos(type) = value
    End Set
  End Property
  Public Property BitmapX As Integer
  Public Property BitmapY As Integer
  Public ReadOnly Property HasBitmap As Boolean
  Public Property Bitmap As BITMAPDIB
  Public Property Conquest As ULong
  Public Property Relics As ULong
  Public Property Explored As ULong
  Public Property AllMustMeet As Integer
  Public Property Mode As VictoryMode
  Public Property Score As Integer
  Public Property Time As Integer
  Public Property AllTechs As Integer
  Public Property CameraX As Integer
  Public Property CameraY As Integer
  Public Property MapType As AiMapType
  Public Property MapX As Integer
  Public Property MapY As Integer
  Public Property Map As Terrain(,)
  Public Property Resources As New List(Of Resource)(16)
  Public Property Units As New List(Of List(Of Unit))(9)
  Public Property Misc As New List(Of PlayerMisc)(8)
  Public Property Triggers As New List(Of Trigger)
  Public Property TriggerOrder As New List(Of Integer)
  Public Property HasAiFile As Long
  Public Property AIFiles As New Dictionary(Of BytesString, BytesString)

  Public Sub New(ByVal stream As IO.Stream, ByVal encoding As Text.Encoding)
    _Encoding = encoding
    Using br As New IO.BinaryReader(stream)
      _Version = br.ReadBytes(4)
      br.ReadBytes(8)
      LastSave = br.ReadInt32()
      Instruction = ReadBytes32(br)
      br.ReadBytes(4)
      _PlayerCount = br.ReadInt32()
      Using ds As New IO.Compression.DeflateStream(stream, IO.Compression.CompressionMode.Decompress)
        Using dr As New IO.BinaryReader(ds)
          _NextUid = dr.ReadInt32()
          _Version2 = dr.ReadSingle()
          For i As Integer = 0 To PlayerCount - 1
            Players.Add(New Player With {
                        .Name = ReadBytesFixedLength(dr, PlayerNameLength)})
          Next
          For i As Integer = 0 To PlayerCount - 1
            Players(i).StringTableName = dr.ReadInt32()
          Next
          For i As Integer = 0 To PlayerCount - 1
            Players(i).IsActive = dr.ReadInt32()
            Players(i).IsHuman = dr.ReadInt32()
            Players(i).Civilization = dr.ReadInt32()
            dr.ReadBytes(4)
          Next
          dr.ReadBytes(9)
          OriginalFilename = ReadBytes16(dr)
          For i As Integer = 0 To 5
            _StringTableInfos.Add(dr.ReadInt32())
          Next
          For i As Integer = 0 To 9
            _StringInfos.Add(ReadBytes16(dr))
          Next
          HasBitmap = dr.ReadInt32()
          BitmapX = dr.ReadInt32()
          BitmapY = dr.ReadInt32()
          dr.ReadBytes(2)
          If HasBitmap Then
            Bitmap = New BITMAPDIB With {
              .biSize = dr.ReadInt32(),
              .biWidth = dr.ReadInt32(),
              .biHeight = dr.ReadInt32(),
              .biPlanes = dr.ReadInt16(),
              .biBitCount = dr.ReadInt16(),
              .biCompression = dr.ReadInt32(),
              .biSizeImage = dr.ReadInt32(),
              .biXPelsPerMeter = dr.ReadInt32(),
              .biYPelsPerMeter = dr.ReadInt32(),
              .biClrUsed = dr.ReadInt32(),
              .biClrImportant = dr.ReadInt32()}
            For i As Integer = 0 To 255
              Bitmap.colors.Add(New RGB With {
                                .Red = dr.ReadByte(),
                                .Green = dr.ReadByte(),
                                .Blue = dr.ReadByte()})
              dr.ReadBytes(1)
            Next
            Bitmap.imageData = dr.ReadBytes(((BitmapX - 1) \ 4 + 1) * 4 * BitmapY)
          End If
          For i As Integer = 0 To 31
            dr.ReadBytes(dr.ReadInt16())
          Next
          For i As Integer = 0 To PlayerCount - 1
            Players(i).Ai = ReadBytes16(dr)
          Next
          For i As Integer = 0 To PlayerCount - 1
            dr.ReadBytes(8)
            Players(i).AiFile = ReadBytes32(dr)
          Next
          For i As Integer = 0 To PlayerCount - 1
            Players(i).Personality = dr.ReadByte()
          Next
          dr.ReadBytes(4)
          For i As Integer = 0 To PlayerCount - 1
            Players(i).Gold = dr.ReadInt32()
            Players(i).Wood = dr.ReadInt32()
            Players(i).Food = dr.ReadInt32()
            Players(i).Stone = dr.ReadInt32()
            Players(i).Orex = dr.ReadInt32()
            dr.ReadBytes(4)
          Next
          dr.ReadBytes(4)
          Conquest = dr.ReadInt64()
          Relics = dr.ReadInt64()
          Explored = dr.ReadInt64()
          AllMustMeet = dr.ReadInt32()
          Mode = dr.ReadInt32()
          Score = dr.ReadInt32()
          Time = dr.ReadInt32()
          For i As Integer = 0 To PlayerCount - 1
            For j As Integer = 0 To PlayerCount - 1
              Players(i).Diplomacies.Add(dr.ReadInt32())
            Next
          Next
          dr.ReadBytes(11524)
          For i As Integer = 0 To PlayerCount - 1
            Players(i).AlliedVictory = dr.ReadInt32()
          Next
          For i As Integer = 0 To PlayerCount - 1
            dr.ReadInt32()
            For j As Integer = 0 To 29
              Players(i).DisabledTechs.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To PlayerCount - 1
            dr.ReadInt32()
            For j As Integer = 0 To 29
              Players(i).DisabledUnits.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To PlayerCount - 1
            dr.ReadInt32()
            For j As Integer = 0 To 19
              Players(i).DisabledBuildings.Add(dr.ReadInt32())
            Next
          Next
          dr.ReadBytes(8)
          AllTechs = dr.ReadInt32()
          For i As Integer = 0 To PlayerCount - 1
            Players(i).StartAge = dr.ReadInt32()
          Next
          dr.ReadBytes(4)
          CameraX = dr.ReadInt32()
          CameraY = dr.ReadInt32()
          MapType = dr.ReadInt32()
          MapX = dr.ReadInt32()
          MapY = dr.ReadInt32()
          ReDim Map(MapX - 1, MapY - 1)
          For i As Integer = 0 To MapX - 1
            For j As Integer = 0 To MapY - 1
              Map(i, j).Id = dr.ReadByte()
              Map(i, j).Elevation = dr.ReadUInt16()
            Next
          Next
          dr.ReadBytes(4)
          For i As Integer = 0 To 7
            Resources.Add(New Resource With {
                          .Food = dr.ReadSingle(),
                          .Wood = dr.ReadSingle(),
                          .Gold = dr.ReadSingle(),
                          .Stone = dr.ReadSingle(),
                          .Orex = dr.ReadSingle()})
            dr.ReadBytes(4)
            Resources(i).PopulationLimit = dr.ReadSingle()
          Next
          For i As Integer = 0 To 8
            Units.Add(New List(Of Unit))
            For j As Integer = 0 To dr.ReadInt32() - 1
              Units(i).Add(New Unit With {
                           .PosX = dr.ReadSingle(),
                           .PosY = dr.ReadSingle()})
              dr.ReadBytes(4)
              Units(i)(j).Id = dr.ReadInt32()
              Units(i)(j).UnitId = dr.ReadInt16()
              dr.ReadBytes(1)
              Units(i)(j).Rotation = dr.ReadSingle()
              Units(i)(j).Frame = dr.ReadInt16()
              Units(i)(j).Garrison = dr.ReadInt32()
            Next
          Next
          dr.ReadBytes(4)
          For i As Integer = 0 To 7
            Misc.Add(New PlayerMisc With {
                     .Name = ReadBytes16(dr),
                     .CameraX = dr.ReadSingle(),
                     .CameraY = dr.ReadSingle()})
            dr.ReadInt32()
            Misc(i).AlliedVictory = dr.ReadByte()
            dr.ReadBytes(2)
            For j As Integer = 0 To 8
              Misc(i).Diplomacy.Add(dr.ReadByte())
            Next
            For j As Integer = 0 To 8
              Misc(i).Diplomacy2.Add(dr.ReadInt32())
            Next
            Misc(i).Color = dr.ReadInt32()
            dr.ReadBytes(CLng(dr.ReadSingle()) * 4 + CLng(dr.ReadInt16()) * 44 + 11)
          Next
          dr.ReadBytes(9)
          For i As Integer = 0 To dr.ReadInt32() - 1
            Triggers.Add(New Trigger With {
                        .IsEnabled = dr.ReadInt32(),
                        .IsLooping = dr.ReadInt32()})
            dr.ReadBytes(1)
            Triggers(i).IsObjective = dr.ReadByte()
            Triggers(i).DescriptionOrder = dr.ReadInt32()
            dr.ReadBytes(4)
            Triggers(i).Description = ReadBytes32(dr)
            Triggers(i).Name = ReadBytes32(dr)
            For j As Integer = 0 To dr.ReadInt32() - 1
              Triggers(i).Effects.Add(New Effect With {
                                      .Type = dr.ReadInt32()})
              For k As Integer = 0 To dr.ReadInt32() - 1
                Triggers(i).Effects(j)._fields.Add(dr.ReadInt32())
              Next
              Triggers(i).Effects(j).Text = ReadBytes32(dr)
              Triggers(i).Effects(j).SoundFile = ReadBytes32(dr)
              For k As Integer = 0 To Triggers(i).Effects(j).Fields(EffectField.NumSelected) - 1
                Triggers(i).Effects(j).UnitIDs.Add(dr.ReadInt32())
              Next
            Next
            For j As Integer = 0 To Triggers(i).Effects.Count - 1
              Triggers(i).EffectOrder.Add(dr.ReadInt32())
            Next
            For j As Integer = 0 To dr.ReadInt32() - 1
              Triggers(i).Conditions.Add(New Condition With {
                                         .Type = dr.ReadInt32()})
              For k As Integer = 0 To dr.ReadInt32() - 1
                Triggers(i).Conditions(j)._fields.Add(dr.ReadInt32())
              Next
            Next
            For j As Integer = 0 To Triggers(i).Conditions.Count - 1
              Triggers(i).ConditionOrder.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To Triggers.Count - 1
            TriggerOrder.Add(dr.ReadInt32())
          Next
          HasAiFile = dr.ReadInt64()
          If HasAiFile Then
            For i = 0 To dr.ReadInt32() - 1
              AIFiles.Add(ReadBytes32(dr), ReadBytes32(dr))
            Next
          End If
        End Using
      End Using
    End Using
  End Sub

  Public Sub New(ByVal stream As IO.Stream)
    Me.New(stream, Text.Encoding.ASCII)
  End Sub

  Public Sub New(ByVal fileName As String, ByVal encoding As Text.Encoding)
    Me.New(New IO.FileStream(fileName, IO.FileMode.Open), encoding)
    Me.FileName = fileName
  End Sub

  Public Sub New(ByVal fileName As String)
    Me.New(New IO.FileStream(fileName, IO.FileMode.Open))
    Me.FileName = fileName
  End Sub

  Public Sub New(ByVal bytes As Byte(), ByVal encoding As Text.Encoding)
    Me.New(New IO.MemoryStream(bytes), encoding)
  End Sub

  Public Sub New(ByVal bytes As Byte())
    Me.New(New IO.MemoryStream(bytes))
  End Sub

  Public Sub SetEncoding(ByVal encoding As Text.Encoding)
    _Encoding = encoding
    Instruction.SetEncoding(encoding)
    Players.ForEach(Sub(x)
                      x.Name.SetEncoding(encoding)
                      x.Ai.SetEncoding(encoding)
                      x.AiFile.SetEncoding(encoding)
                    End Sub)
    OriginalFilename.SetEncoding(encoding)
    _StringInfos.ForEach(Sub(x) x.SetEncoding(encoding))
    Misc.ForEach(Sub(x) x.Name.SetEncoding(encoding))
    Triggers.ForEach(Sub(x)
                       x.Name.SetEncoding(encoding)
                       x.Description.SetEncoding(encoding)
                       x.Effects.ForEach(Sub(y)
                                           y.Text.SetEncoding(encoding)
                                           y.SoundFile.SetEncoding(encoding)
                                         End Sub)
                     End Sub)
    For Each x In AIFiles
      x.Key.SetEncoding(encoding)
      x.Value.SetEncoding(encoding)
    Next
  End Sub

  Public Function SaveAsStream() As IO.MemoryStream
    Dim ms1 As New IO.MemoryStream()
    Using bw1 As New IO.BinaryWriter(ms1)
      bw1.Write(_Version)
      bw1.Write(Instruction.Bytes.Length + 20)
      bw1.Write(2I)
      bw1.Write(LastSave)
      bw1.Write(Instruction.Bytes)
      bw1.Write(0I)
      bw1.Write(_PlayerCount)
      Using ms As New IO.MemoryStream(), bw As New IO.BinaryWriter(ms), cs As New IO.Compression.DeflateStream(ms1, IO.Compression.CompressionMode.Compress)
        bw.Write(_NextUid)
        bw.Write(_Version2)
        Players.ForEach(Sub(x) bw.Write(x.Name.Bytes))
        Players.ForEach(Sub(x) bw.Write(x.StringTableName))
        Players.ForEach(Sub(x)
                          bw.Write(x.IsActive)
                          bw.Write(x.IsHuman)
                          bw.Write(x.Civilization)
                          bw.Write(4I)
                        End Sub)
        bw.Write(1I)
        bw.Write(CByte(0))
        bw.Write(-1.0F)
        bw.Write16(OriginalFilename)
        _StringTableInfos.ForEach(Sub(x) bw.Write(x))
        _StringInfos.ForEach(Sub(x) bw.Write16(x))
        bw.Write(IIf(HasBitmap, 1I, 0I))
        bw.Write(BitmapX)
        bw.Write(BitmapY)
        bw.Write(IIf(HasBitmap, -1S, 1S))
        If HasBitmap Then
          bw.Write(Bitmap.biSize)
          bw.Write(Bitmap.biWidth)
          bw.Write(Bitmap.biHeight)
          bw.Write(Bitmap.biPlanes)
          bw.Write(Bitmap.biBitCount)
          bw.Write(Bitmap.biCompression)
          bw.Write(Bitmap.biSizeImage)
          bw.Write(Bitmap.biXPelsPerMeter)
          bw.Write(Bitmap.biYPelsPerMeter)
          bw.Write(Bitmap.biClrUsed)
          bw.Write(Bitmap.biClrImportant)
          Bitmap.colors.ForEach(Sub(x)
                                  bw.Write(x.Red)
                                  bw.Write(x.Green)
                                  bw.Write(x.Blue)
                                  bw.Write(CByte(0))
                                End Sub)
          bw.Write(Bitmap.imageData)
        End If
        For i As Integer = 0 To 31
          bw.Write(0S)
        Next
        Players.ForEach(Sub(x) bw.Write16(x.Ai))
        Players.ForEach(Sub(x)
                          bw.Write(0L)
                          bw.Write32(x.AiFile)
                        End Sub)
        Players.ForEach(Sub(x) bw.Write(x.Personality))
        bw.Write(-99I)
        Players.ForEach(Sub(x)
                          bw.Write(x.Gold)
                          bw.Write(x.Wood)
                          bw.Write(x.Food)
                          bw.Write(x.Stone)
                          bw.Write(x.Orex)
                          bw.Write(0I)
                        End Sub)
        bw.Write(-99I)
        bw.Write(Conquest)
        bw.Write(Relics)
        bw.Write(Explored)
        bw.Write(AllMustMeet)
        bw.Write(Mode)
        bw.Write(Score)
        bw.Write(Time)
        Players.ForEach(Sub(x) x.Diplomacies.ForEach(Sub(y) bw.Write(y)))
        For i As Integer = 1 To 720
          bw.Write(0D)
        Next
        bw.Write(-99I)
        Players.ForEach(Sub(x) bw.Write(x.AlliedVictory))
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledTechs, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledTechs.ForEach(Sub(y) bw.Write(y)))
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledUnits, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledUnits.ForEach(Sub(y) bw.Write(y)))
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledBuildings, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledBuildings.ForEach(Sub(y) bw.Write(y)))
        bw.Write(0L)
        bw.Write(AllTechs)
        Players.ForEach(Sub(x) bw.Write(x.StartAge))
        bw.Write(-99I)
        bw.Write(CameraX)
        bw.Write(CameraY)
        bw.Write(MapType)
        bw.Write(MapX)
        bw.Write(MapY)
        For i = 0 To MapX - 1
          For j = 0 To MapY - 1
            bw.Write(Map(i, j).Id)
            bw.Write(Map(i, j).Elevation)
          Next
        Next
        bw.Write(9I)
        Resources.ForEach(Sub(x)
                            bw.Write(x.Food)
                            bw.Write(x.Wood)
                            bw.Write(x.Gold)
                            bw.Write(x.Stone)
                            bw.Write(x.Orex)
                            bw.Write(0I)
                            bw.Write(x.PopulationLimit)
                          End Sub)
        Units.ForEach(Sub(x)
                        bw.Write(x.Count)
                        x.ForEach(Sub(y)
                                    bw.Write(y.PosX)
                                    bw.Write(y.PosY)
                                    bw.Write(1.0F)
                                    bw.Write(y.Id)
                                    bw.Write(y.UnitId)
                                    bw.Write(CByte(2))
                                    bw.Write(y.Rotation)
                                    bw.Write(y.Frame)
                                    bw.Write(y.Garrison)
                                  End Sub)
                      End Sub)
        bw.Write(9I)
        Misc.ForEach(Sub(x)
                       bw.Write16(x.Name)
                       bw.Write(x.CameraX)
                       bw.Write(x.CameraY)
                       bw.Write(0I)
                       bw.Write(x.AlliedVictory)
                       bw.Write(9S)
                       x.Diplomacy.ForEach(Sub(y) bw.Write(y))
                       x.Diplomacy2.ForEach(Sub(y) bw.Write(y))
                       bw.Write(x.Color)
                       bw.Write(2.0F)
                       bw.Write(0L)
                       bw.Write(0L)
                       bw.Write(CByte(0))
                       bw.Write(-1I)
                     End Sub)
        bw.Write(1.6R)
        bw.Write(CByte(0))
        bw.Write(Triggers.Count)
        Triggers.ForEach(Sub(x)
                           bw.Write(x.IsEnabled)
                           bw.Write(x.IsLooping)
                           bw.Write(CByte(0))
                           bw.Write(x.IsObjective)
                           bw.Write(x.DescriptionOrder)
                           bw.Write(0I)
                           bw.Write32(x.Description)
                           bw.Write32(x.Name)
                           bw.Write(x.Effects.Count)
                           x.Effects.ForEach(Sub(y)
                                               bw.Write(y.Type)
                                               bw.Write(y._fields.Count)
                                               y._fields.ForEach(Sub(z) bw.Write(z))
                                               bw.Write32(y.Text)
                                               bw.Write32(y.SoundFile)
                                               y.UnitIDs.ForEach(Sub(z) bw.Write(z))
                                             End Sub)
                           x.EffectOrder.ForEach(Sub(y) bw.Write(y))
                           bw.Write(x.Conditions.Count)
                           x.Conditions.ForEach(Sub(y)
                                                  bw.Write(y.Type)
                                                  bw.Write(y._fields.Count)
                                                  y._fields.ForEach(Sub(z) bw.Write(z))
                                                End Sub)
                           x.ConditionOrder.ForEach(Sub(y) bw.Write(y))
                         End Sub)
        TriggerOrder.ForEach(Sub(x) bw.Write(x))
        bw.Write(HasAiFile)
        If HasAiFile Then
          bw.Write(AIFiles.Count)
          AIFiles.ToList.ForEach(Sub(x)
                                   bw.Write32(x.Key)
                                   bw.Write32(x.Value)
                                 End Sub)
        End If
        ms.Seek(0, IO.SeekOrigin.Begin)
        ms.CopyTo(cs)
      End Using
    End Using
    ms1 = New IO.MemoryStream(ms1.ToArray())
    Return ms1
  End Function

  Public Sub Save()
    With SaveAsStream()
      .CopyTo(New IO.FileStream(FileName, IO.FileMode.Truncate))
      .Close()
    End With
  End Sub

  Public Sub SaveAs(ByVal fileName As String)
    With SaveAsStream()
      .CopyTo(New IO.FileStream(fileName, IO.FileMode.Create))
      .Close()
    End With
  End Sub

  Public Function GetBytes() As Byte()
    Return SaveAsStream().ToArray()
  End Function

  Private Function ReadBytes16(ByVal br As IO.BinaryReader) As BytesString
    Return New BytesString(br.ReadBytes(br.ReadInt16()), Encoding)
  End Function

  Private Function ReadBytes32(ByVal br As IO.BinaryReader) As BytesString
    Return New BytesString(br.ReadBytes(br.ReadInt32()), Encoding)
  End Function

  Private Function ReadBytesFixedLength(ByVal br As IO.BinaryReader, ByVal length As Integer) As BytesString
    Return New BytesString(br.ReadBytes(length), length, Encoding)
  End Function

End Class

Public Class BytesString
  Private _Str As String
  Public Property Str As String
    Get
      Return _Str
    End Get
    Set(value As String)
      _Str = value
      _Bytes = If(FixedLength = -1,
        Encoding.GetBytes(_Str),
        GetBytesFixedLength(_Str, FixedLength))
    End Set
  End Property

  Private Property FixedLength As Integer
  Public ReadOnly Property Bytes As Byte()
  Public Property Encoding As Text.Encoding

  Public Sub New(ByVal encoding As Text.Encoding)
    FixedLength = -1
    Me.Encoding = encoding
  End Sub

  Public Sub New(ByVal bytes As Byte(), ByVal encoding As Text.Encoding)
    Me.New(encoding)
    Me.Bytes = bytes
    _Str = encoding.GetString(bytes).TrimEnd(vbNullChar)
  End Sub

  Public Sub New(ByVal bytes As Byte(), ByVal maxLength As Integer, ByVal encoding As Text.Encoding)
    Me.New(bytes, encoding)
    Me.FixedLength = maxLength
  End Sub

  Public Sub SetEncoding(ByVal encoding As Text.Encoding)
    Me.Encoding = encoding
    _Str = encoding.GetString(_Bytes).TrimEnd(vbNullChar)
  End Sub

  Public Sub Transcode(ByVal newEncoding As Text.Encoding)
    _Bytes = Text.Encoding.Convert(Encoding, newEncoding, _Bytes)
  End Sub

  Public Overrides Function ToString() As String
    Return _Str
  End Function

  Public Shared Narrowing Operator CType(ByVal bytesString As BytesString) As String
    Return bytesString.ToString()
  End Operator

  Private Function GetBytesFixedLength(ByVal s As String, ByVal length As Integer) As Byte()
    Dim y(length - 1) As Byte
    Encoding.GetBytes(s).Take(length).ToArray().CopyTo(y, 0)
    Return y
  End Function

End Class

Module BinaryWriterWritesByteString
  <Runtime.CompilerServices.Extension()>
  Public Sub Write16(ByVal binaryWriter As IO.BinaryWriter, ByVal bytesString As BytesString)
    binaryWriter.Write(CShort(bytesString.Bytes.Length))
    binaryWriter.Write(bytesString.Bytes)
  End Sub

  <Runtime.CompilerServices.Extension()>
  Public Sub Write32(ByVal binaryWriter As IO.BinaryWriter, ByVal bytesString As BytesString)
    binaryWriter.Write(bytesString.Bytes.Length)
    binaryWriter.Write(bytesString.Bytes)
  End Sub
End Module
