Public Class scxFile
  Implements ICloneable
#Region "Enum&Struct"
  Public Enum ScxVersion
    Version118
    Version122
    Version123
    Version124
    Version126
    Unknown
  End Enum

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
    Private _Parent As scxFile
    Public _Name(255) As Byte
    Public Property Name As String
      Get
        Return _Parent._Encoding.GetString(_Name).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Name = _Parent.GetBytesFixed(value, 256)
      End Set
    End Property

    Public Property StringTableName As Integer
    Public Property IsActive As Integer
    Public Property IsHuman As Integer
    Public Property Civilization As Integer

    Public _Ai As Byte()
    Public Property Ai As String
      Get
        Return _Parent._Encoding.GetString(_Ai).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Ai = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public Property AiFile As Byte()
    Public Property Personality As Byte
    Public Property Gold As Integer
    Public Property Wood As Integer
    Public Property Food As Integer
    Public Property Stone As Integer
    Public Property Orex As Integer
    Public Property PlayerNumber As Integer
    Public Property Diplomacies As New List(Of DiplomacyI)(16)
    Public Property AlliedVictory As Integer
    Public Property DisabledTechs As New List(Of Integer)(30)
    Public Property DisabledUnits As New List(Of Integer)(30)
    Public Property DisabledBuildings As New List(Of Integer)(20)
    Public Property StartAge As StartAge

    Public Sub New(ByVal parent As scxFile)
      _Parent = parent
    End Sub
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
    Public Property PosZ As Single
    Public Property Id As Integer
    Public Property UnitId As UShort
    Public Property State As Byte
    Public Property Rotation As Single
    Public Property Frame As Short
    Public Property Garrison As Integer
    Public Overrides Function ToString() As String
      Return $"{UnitId}{vbTab}{Id}{vbTab}({PosX}, {PosY})"
    End Function
  End Class

  Public Class PlayerMisc
    Private _Parent As scxFile

    Public _Name As Byte()
    Public Property Name As String
      Get
        Return _Parent._Encoding.GetString(_Name).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Name = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public Property CameraX As Single
    Public Property CameraY As Single
    Public Property AlliedVictory As Byte
    Public Property Diplomacy As New List(Of DiplomacyB)(9)
    Public Property Diplomacy2 As New List(Of Diplomacy2)(9)
    Public Property Color As Color

    Public Sub New(ByVal parent As scxFile)
      _Parent = parent
    End Sub

    Public Overrides Function ToString() As String
      Return $"{Name}, {Color}"
    End Function
  End Class

  Public Class Effect
    Private _Parent As scxFile
    Public Property Type As EffectType
    Private _fields As New List(Of Integer)(23)
    Public Property Fields(ByVal fieldType As EffectField) As Integer
      Get
        Return _fields(fieldType)
      End Get
      Set(value As Integer)
        _fields(fieldType) = value
      End Set
    End Property
    Public Function GetFields() As List(Of Integer)
      Return _fields
    End Function

    Public _Text As Byte()
    Public Property Text As String
      Get
        Return _Parent._Encoding.GetString(_Text).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Text = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public _SoundFile As Byte()
    Public Property SoundFile As String
      Get
        Return _Parent._Encoding.GetString(_SoundFile).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _SoundFile = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public Property UnitIDs As New List(Of Integer)

    Public Sub New(ByVal parent As scxFile)
      _Parent = parent
    End Sub

    Public Overrides Function ToString() As String
      Return Type
    End Function
  End Class

  Public Class Condition
    Public Property Type As ConditionType
    Private _fields As New List(Of Integer)(16)
    Public Property Fields(ByVal fieldType As EffectField) As Integer
      Get
        Return _fields(fieldType)
      End Get
      Set(value As Integer)
        _fields(fieldType) = value
      End Set
    End Property
    Public Function GetFields() As List(Of Integer)
      Return _fields
    End Function

    Public Overrides Function ToString() As String
      Return Type
    End Function
  End Class

  Public Class Trigger
    Private _Parent As scxFile
    Public Property IsEnabled As Integer
    Public Property IsLooping As Integer
    Public Property IsObjective As Byte
    Public Property DescriptionOrder As Integer
    Public _Description As Byte()
    Public Property Description As String
      Get
        Return _Parent._Encoding.GetString(_Description).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Description = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public _Name As Byte()
    Public Property Name As String
      Get
        Return _Parent._Encoding.GetString(_Name).TrimEnd(vbNullChar)
      End Get
      Set(value As String)
        _Name = _Parent._Encoding.GetBytes(value)
      End Set
    End Property

    Public Property Effects As New List(Of Effect)
    Public Property EffectOrder As New List(Of Integer)
    Public Property Conditions As New List(Of Condition)
    Public Property ConditionOrder As New List(Of Integer)

    Public Sub New(ByVal parent As scxFile)
      _Parent = parent
    End Sub

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

  Public Property Encoding As Text.Encoding
  Public ReadOnly Property FileName As String

  Private _Version As Byte()
  Public ReadOnly Property LastSave As Integer

  Private _Instruction As Byte()
  Public Property Instruction As String
    Get
      Return _Encoding.GetString(_Instruction).TrimEnd(vbNullChar)
    End Get
    Set(value As String)
      _Instruction = _Encoding.GetBytes(value)
    End Set
  End Property

  Private _PlayerCount As Integer
  Private _FormatVersion As Integer
  Private _UnknownInt32s As New List(Of Integer)
  Private _NextUid As Integer
  Private _Version2 As Single
  Public Property Players As New List(Of Player)(16)

  Private _OriginalFilename As Byte()
  Public Property OriginalFilename As String
    Get
      Return _Encoding.GetString(_OriginalFilename).TrimEnd(vbNullChar)
    End Get
    Set(value As String)
      _OriginalFilename = _Encoding.GetBytes(value)
    End Set
  End Property

  Private _StringTableInfos As New List(Of Integer)(5)
  Public Property StringTableInfos(ByVal type As InfoType) As Integer
    Get
      Return _StringTableInfos(type)
    End Get
    Set(value As Integer)
      _StringTableInfos(type) = value
    End Set
  End Property

  Private _StringInfos As New List(Of Byte())(9)
  Public Property StringInfos(ByVal type As InfoType) As String
    Get
      Return _Encoding.GetString(_StringInfos(type)).TrimEnd(vbNullChar)
    End Get
    Set(value As String)
      _StringInfos(type) = _Encoding.GetBytes(value)
    End Set
  End Property

  Public Property BitmapX As Integer
  Public Property BitmapY As Integer
  Public ReadOnly Property HasBitmap As Integer
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
  Public Property LockTeams As Byte
  Public Property PlayerChooseTeams As Byte
  Public Property RandomStartPoints As Byte
  Public Property MaxTeams As Byte
  Public Property Triggers As New List(Of Trigger)
  Public Property TriggerOrder As New List(Of Integer)
  Public Property HasAiFile As Integer
  Public Property NeedsWorkaround As Integer
  Public Property WorkaroundBytes As Byte()
  Public Property AIFiles As New Dictionary(Of Byte(), Byte())

  Public Sub New(ByVal stream As IO.Stream, ByVal encoding As Text.Encoding)
    _Encoding = encoding
    Using br As IO.BinaryReader = New IO.BinaryReader(stream)
      _Version = br.ReadBytes(4)
      br.ReadBytes(4)
      _FormatVersion = br.ReadInt32()
      LastSave = br.ReadInt32()
      _Instruction = br.ReadBytes(br.ReadInt32())
      br.ReadBytes(4)
      _PlayerCount = br.ReadInt32()
      If _FormatVersion = 3 Then
        br.ReadBytes(8)
        For i = 1 To br.ReadInt32()
          _UnknownInt32s.Add(br.ReadInt32())
        Next
      End If
      Using ds As IO.Compression.DeflateStream = New IO.Compression.DeflateStream(stream, IO.Compression.CompressionMode.Decompress)
        Using dr As IO.BinaryReader = New IO.BinaryReader(ds)
          _NextUid = dr.ReadInt32()
          _Version2 = dr.ReadSingle()
          For i As Integer = 0 To 15
            Players.Add(New Player(Me) With {._Name = dr.ReadBytes(256)})
          Next
          For i As Integer = 0 To 15
            Players(i).StringTableName = dr.ReadInt32()
          Next
          For i As Integer = 0 To 15
            Players(i).IsActive = dr.ReadInt32()
            Players(i).IsHuman = dr.ReadInt32()
            Players(i).Civilization = dr.ReadInt32()
            dr.ReadBytes(4)
          Next
          dr.ReadBytes(9)
          _OriginalFilename = dr.ReadBytes(dr.ReadInt16())
          For i As Integer = 0 To If(GetVersion() >= ScxVersion.Version122, 5, 4)
            _StringTableInfos.Add(dr.ReadInt32())
          Next
          For i As Integer = 0 To If(GetVersion() >= ScxVersion.Version122, 9, 8)
            _StringInfos.Add(dr.ReadBytes(dr.ReadInt16()))
          Next
          HasBitmap = dr.ReadInt32()
          BitmapX = dr.ReadInt32()
          BitmapY = dr.ReadInt32()
          dr.ReadBytes(2)
          If BitmapX > 0 AndAlso BitmapY > 0 Then
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
          For i As Integer = 0 To 15
            Players(i)._Ai = dr.ReadBytes(dr.ReadInt16())
          Next
          For i As Integer = 0 To 15
            dr.ReadBytes(8)
            Players(i).AiFile = dr.ReadBytes(dr.ReadInt32())
          Next
          For i As Integer = 0 To 15
            Players(i).Personality = dr.ReadByte()
          Next
          dr.ReadInt32()
          For i As Integer = 0 To 15
            Players(i).Gold = dr.ReadInt32()
            Players(i).Wood = dr.ReadInt32()
            Players(i).Food = dr.ReadInt32()
            Players(i).Stone = dr.ReadInt32()
            Players(i).Orex = dr.ReadInt32()
            dr.ReadInt32()
            If GetVersion() >= ScxVersion.Version124 Then Players(i).PlayerNumber = dr.ReadInt32()
          Next
          dr.ReadInt32()
          Conquest = dr.ReadInt64()
          Relics = dr.ReadInt64()
          Explored = dr.ReadInt64()
          AllMustMeet = dr.ReadInt32()
          Mode = dr.ReadInt32()
          Score = dr.ReadInt32()
          Time = dr.ReadInt32()
          For i As Integer = 0 To 15
            For j As Integer = 0 To 15
              Players(i).Diplomacies.Add(dr.ReadInt32())
            Next
          Next
          dr.ReadBytes(11524)
          For i As Integer = 0 To 15
            Players(i).AlliedVictory = dr.ReadInt32()
          Next
          If GetVersion() >= ScxVersion.Version123 Then
            LockTeams = dr.ReadByte()
            PlayerChooseTeams = dr.ReadByte()
            RandomStartPoints = dr.ReadByte()
            MaxTeams = dr.ReadByte()
          End If
          For i As Integer = 0 To 15
            dr.ReadInt32()
          Next
          For i = 0 To 15
            For j As Integer = 0 To 29
              Players(i).DisabledTechs.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To 15
            dr.ReadInt32()
          Next
          For i = 0 To 15
            For j As Integer = 0 To 29
              Players(i).DisabledUnits.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To 15
            dr.ReadInt32()
          Next
          For i = 0 To 15
            For j As Integer = 0 To If(GetVersion() >= ScxVersion.Version126, 29, 19)
              Players(i).DisabledBuildings.Add(dr.ReadInt32())
            Next
          Next
          dr.ReadBytes(8)
          AllTechs = dr.ReadInt32()
          For i As Integer = 0 To 15
            Players(i).StartAge = dr.ReadInt32() - If(GetVersion() >= ScxVersion.Version126, 2, 0)
          Next
          dr.ReadInt32()
          CameraX = dr.ReadInt32()
          CameraY = dr.ReadInt32()
          If GetVersion() >= ScxVersion.Version122 Then MapType = dr.ReadInt32()
          If GetVersion() >= ScxVersion.Version124 Then dr.ReadBytes(16)
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
            If GetVersion() >= ScxVersion.Version122 Then Resources(i).PopulationLimit = dr.ReadSingle()
          Next
          For i As Integer = 0 To 8
            Units.Add(New List(Of Unit))
            For j As Integer = 0 To dr.ReadInt32() - 1
              Units(i).Add(New Unit With {
                           .PosX = dr.ReadSingle(),
                           .PosY = dr.ReadSingle(),
                           .PosZ = dr.ReadSingle(),
                           .Id = dr.ReadInt32(),
                           .UnitId = dr.ReadInt16(),
                           .State = dr.ReadByte(),
                           .Rotation = dr.ReadSingle(),
                           .Frame = dr.ReadInt16(),
                           .Garrison = dr.ReadInt32()})
            Next
          Next
          dr.ReadInt32()
          For i As Integer = 0 To 7
            Misc.Add(New PlayerMisc(Me))
            Misc(i)._Name = dr.ReadBytes(dr.ReadInt16())
            Misc(i).CameraX = dr.ReadSingle()
            Misc(i).CameraY = dr.ReadSingle()
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
            dr.ReadBytes(If(dr.ReadSingle() = 2.0F, 8, 0) + dr.ReadInt16() * 44 + 11)
          Next
          Dim someDouble = dr.ReadDouble()
          If someDouble = 1.6 Then dr.ReadByte()
          For i As Integer = 0 To dr.ReadInt32() - 1
            Triggers.Add(New Trigger(Me) With {
                      .IsEnabled = dr.ReadInt32(),
                      .IsLooping = dr.ReadInt32()})
            dr.ReadBytes(1)
            Triggers(i).IsObjective = dr.ReadByte()
            Triggers(i).DescriptionOrder = dr.ReadInt32()
            If someDouble = 1.6 Then dr.ReadBytes(4)
            Triggers(i)._Description = dr.ReadBytes(dr.ReadInt32())
            Triggers(i)._Name = dr.ReadBytes(dr.ReadInt32())
            For j As Integer = 0 To dr.ReadInt32() - 1
              Triggers(i).Effects.Add(New Effect(Me) With {.Type = dr.ReadInt32()})
              For k As Integer = 0 To dr.ReadInt32() - 1
                Triggers(i).Effects(j).GetFields().Add(dr.ReadInt32())
              Next
              Triggers(i).Effects(j)._Text = dr.ReadBytes(dr.ReadInt32())
              Triggers(i).Effects(j)._SoundFile = dr.ReadBytes(dr.ReadInt32())
              If Triggers(i).Effects(j).GetFields.Count > EffectField.NumSelected Then
                For k As Integer = 0 To Triggers(i).Effects(j).Fields(EffectField.NumSelected) - 1
                  Triggers(i).Effects(j).UnitIDs.Add(dr.ReadInt32())
                Next
              End If
            Next
            For j As Integer = 0 To Triggers(i).Effects.Count - 1
              Triggers(i).EffectOrder.Add(dr.ReadInt32())
            Next
            For j As Integer = 0 To dr.ReadInt32() - 1
              Triggers(i).Conditions.Add(New Condition With {.Type = dr.ReadInt32()})
              For k As Integer = 0 To dr.ReadInt32() - 1
                Triggers(i).Conditions(j).GetFields().Add(dr.ReadInt32())
              Next
            Next
            For j As Integer = 0 To Triggers(i).Conditions.Count - 1
              Triggers(i).ConditionOrder.Add(dr.ReadInt32())
            Next
          Next
          For i As Integer = 0 To Triggers.Count - 1
            TriggerOrder.Add(dr.ReadInt32())
          Next
          HasAiFile = dr.ReadInt32()
          NeedsWorkaround = dr.ReadInt32()
          If NeedsWorkaround = 1 Then
            WorkaroundBytes = dr.ReadBytes(396)
          End If
          If HasAiFile = 1 Then
            For i = 0 To dr.ReadInt32() - 1
              AIFiles.Add(dr.ReadBytes(dr.ReadInt32()), dr.ReadBytes(dr.ReadInt32()))
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

  Public Function GetVersion() As ScxVersion
    If _Version(2) = &H31 Then ' "1.1X"
      Return ScxVersion.Version118
    Else
      Select Case _Version2
        Case < 1.2201F  ' 1.22
          Return ScxVersion.Version122
        Case < 1.2301F ' 1.23
          Return ScxVersion.Version123
        Case 1.2401F  ' 1.24
          Return ScxVersion.Version124
        Case < 1.2601F  ' 1.26
          Return ScxVersion.Version126
        Case Else
          Return ScxVersion.Unknown
      End Select
    End If
  End Function

  Public Function GetStream() As IO.MemoryStream
    Dim ms1 As New IO.MemoryStream()
    Using bw1 As New IO.BinaryWriter(ms1)
      bw1.Write(_Version)
      bw1.Write(_Encoding.GetByteCount(Instruction) + 20)
      bw1.Write(_FormatVersion)
      bw1.Write(LastSave)
      WriteString32(bw1, Instruction)
      bw1.Write(0I)
      bw1.Write(_PlayerCount)
      If _FormatVersion = 3 Then
        bw1.Write(1000I)
        bw1.Write(1I)
        bw1.Write(_UnknownInt32s.Count)
        _UnknownInt32s.ForEach(Sub(x) bw1.Write(x))
      End If
      Using ms As New IO.MemoryStream(), bw As New IO.BinaryWriter(ms), cs As New IO.Compression.DeflateStream(ms1, IO.Compression.CompressionMode.Compress)
        bw.Write(_NextUid)
        bw.Write(_Version2)
        Players.ForEach(Sub(x) bw.Write(GetBytesFixed(x.Name, 256)))
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
        WriteString16(bw, OriginalFilename)
        _StringTableInfos.ForEach(Sub(x) bw.Write(x))
        _StringInfos.ForEach(Sub(x)
                               bw.Write(CShort(x.Length))
                               bw.Write(x)
                             End Sub)
        bw.Write(HasBitmap)
        bw.Write(BitmapX)
        bw.Write(BitmapY)
        bw.Write(1S)
        If BitmapX > 0 AndAlso BitmapY > 0 Then
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
        Players.ForEach(Sub(x) WriteString16(bw, x.Ai))
        Players.ForEach(Sub(x)
                          bw.Write(0L)
                          bw.Write(x.AiFile.Length)
                          bw.Write(x.AiFile)
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
                          If GetVersion() >= ScxVersion.Version124 Then bw.Write(x.PlayerNumber)
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
        If GetVersion() >= ScxVersion.Version123 Then
          bw.Write(LockTeams)
          bw.Write(PlayerChooseTeams)
          bw.Write(RandomStartPoints)
          bw.Write(MaxTeams)
        End If
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledTechs, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledTechs.ForEach(Sub(y) bw.Write(y)))
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledUnits, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledUnits.ForEach(Sub(y) bw.Write(y)))
        Players.ForEach(Sub(x) bw.Write(Enumerable.Count(x.DisabledBuildings, Function(y) y >= 0)))
        Players.ForEach(Sub(x) x.DisabledBuildings.ForEach(Sub(y) bw.Write(y)))
        bw.Write(0L)
        bw.Write(AllTechs)
        Players.ForEach(Sub(x) bw.Write(x.StartAge + If(GetVersion() >= ScxVersion.Version126, 2, 0)))
        bw.Write(-99I)
        bw.Write(CameraX)
        bw.Write(CameraY)
        If GetVersion() >= ScxVersion.Version122 Then bw.Write(MapType)
        If GetVersion() >= ScxVersion.Version124 Then bw.Write(Enumerable.Repeat(CByte(0), 16).ToArray())
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
                            If GetVersion() >= ScxVersion.Version122 Then bw.Write(x.PopulationLimit)
                          End Sub)
        Units.ForEach(Sub(x)
                        bw.Write(x.Count)
                        x.ForEach(Sub(y)
                                    bw.Write(y.PosX)
                                    bw.Write(y.PosY)
                                    bw.Write(y.PosZ)
                                    bw.Write(y.Id)
                                    bw.Write(y.UnitId)
                                    bw.Write(y.State)
                                    bw.Write(y.Rotation)
                                    bw.Write(y.Frame)
                                    bw.Write(y.Garrison)
                                  End Sub)
                      End Sub)
        bw.Write(9I)
        Misc.ForEach(Sub(x)
                       WriteString16(bw, x.Name)
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
                           WriteString32(bw, x.Description)
                           WriteString32(bw, x.Name)
                           bw.Write(x.Effects.Count)
                           x.Effects.ForEach(Sub(y)
                                               bw.Write(y.Type)
                                               bw.Write(y.GetFields().Count)
                                               y.GetFields().ForEach(Sub(z) bw.Write(z))
                                               WriteString32(bw, y.Text)
                                               WriteString32(bw, y.SoundFile)
                                               y.UnitIDs.ForEach(Sub(z) bw.Write(z))
                                             End Sub)
                           x.EffectOrder.ForEach(Sub(y) bw.Write(y))
                           bw.Write(x.Conditions.Count)
                           x.Conditions.ForEach(Sub(y)
                                                  bw.Write(y.Type)
                                                  bw.Write(y.GetFields().Count)
                                                  y.GetFields().ForEach(Sub(z) bw.Write(z))
                                                End Sub)
                           x.ConditionOrder.ForEach(Sub(y) bw.Write(y))
                         End Sub)
        TriggerOrder.ForEach(Sub(x) bw.Write(x))
        bw.Write(HasAiFile)
        bw.Write(NeedsWorkaround)
        If NeedsWorkaround = 1 Then bw.Write(WorkaroundBytes)
        If HasAiFile = 1 Then
          bw.Write(AIFiles.Count)
          AIFiles.ToList().ForEach(Sub(x)
                                     bw.Write(x.Key.Length)
                                     bw.Write(x.Key)
                                     bw.Write(x.Value.Length)
                                     bw.Write(x.Value)
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
    With GetStream()
      .CopyTo(New IO.FileStream(FileName, IO.FileMode.Truncate))
      .Close()
    End With
  End Sub

  Public Sub SaveAs(ByVal fileName As String)
    With GetStream()
      .CopyTo(New IO.FileStream(fileName, IO.FileMode.Create))
      .Close()
    End With
  End Sub

  Public Function GetBytes() As Byte()
    Return GetStream().ToArray()
  End Function

  Private Function GetBytesFixed(ByVal s As String, ByVal length As Integer) As Byte()
    Dim y(length - 1) As Byte
    Encoding.GetBytes(s).Take(length).ToArray().CopyTo(y, 0)
    Return y
  End Function

  Private Sub WriteString16(ByVal bw As IO.BinaryWriter, ByVal s As String)
    Dim y As Byte() = _Encoding.GetBytes(s)
    bw.Write(CShort(y.Length))
    bw.Write(y)
  End Sub

  Private Sub WriteString32(ByVal bw As IO.BinaryWriter, ByVal s As String)
    Dim y As Byte() = _Encoding.GetBytes(s)
    bw.Write(y.Length)
    bw.Write(y)
  End Sub

  Private Function TranscodeBytesFixed(ByVal newEncoding As Text.Encoding, ByVal y As Byte()) As Byte()
    Dim ret(y.Length - 1) As Byte
    Dim Converted() As Byte = Text.Encoding.Convert(_Encoding, newEncoding, y)
    Buffer.BlockCopy(Converted, 0, ret, 0, Math.Min(Converted.Length, y.Length))
    Return ret
  End Function

  Public Sub Transcode(ByVal newEncoding As Text.Encoding)
    _Instruction = Text.Encoding.Convert(_Encoding, newEncoding, _Instruction)
    _OriginalFilename = Text.Encoding.Convert(_Encoding, newEncoding, _OriginalFilename)
    For i As Integer = 0 To _StringInfos.Count - 1
      _StringInfos(i) = Text.Encoding.Convert(_Encoding, newEncoding, _StringInfos(i))
    Next
    Players.ForEach(Sub(x)
                      x._Name = TranscodeBytesFixed(newEncoding, x._Name)
                      x._Ai = Text.Encoding.Convert(_Encoding, newEncoding, x._Ai)
                    End Sub)
    Misc.ForEach(Sub(x) x._Name = Text.Encoding.Convert(_Encoding, newEncoding, x._Name))
    Triggers.ForEach(Sub(x)
                       x._Name = Text.Encoding.Convert(_Encoding, newEncoding, x._Name)
                       x._Description = Text.Encoding.Convert(_Encoding, newEncoding, x._Description)
                       x.Effects.ForEach(Sub(y)
                                           y._Text = Text.Encoding.Convert(_Encoding, newEncoding, y._Text)
                                           y._SoundFile = Text.Encoding.Convert(_Encoding, newEncoding, y._SoundFile)
                                         End Sub)
                     End Sub)
    _Encoding = newEncoding
  End Sub

  Public Function Clone() As Object Implements ICloneable.Clone
    Return New scxFile(GetBytes())
  End Function
End Class
