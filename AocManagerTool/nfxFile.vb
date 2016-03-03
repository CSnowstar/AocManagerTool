Public Class nfxFile
    Public Enum eMapSize As Integer
        Tiny
        Small
        Medium
        Large
        Huge
        Gigantic
    End Enum
    Public Enum eMapType As Integer
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
        Random
        Scandavania
        Mongolia
        Yucatan
        SaltMarsh
        Arena
        KingOfTheHill
        Oasis
        GhostLake
        Nomad
        Spain
        Britain
        Mideast
        Texas
        Italy
        Caribbean
        France
        NorseLands
        Japan
        Byzantium
        Custom
        RandomLand
    End Enum
    Public Enum eVisibility As Integer
        Standard
        Explored
        AllSeen
    End Enum
    Public Enum eVictory As Integer
        Standard
        Conquest
        Time = 7
        Score
    End Enum
    Public Enum eResource As Integer
        Standard
        Low
        Medium
        High
    End Enum
    Public Enum eStartAge As Integer
        Standard
        Dark = 2
        Feudal
        Castle
        Imperial
        PostImperial
    End Enum
    Public Enum eGameMode As Byte
        RandomMap
        Regicide
        DeathMatch
        KingOfTheHill = 5
        BuildWonder
        DefendWonder
    End Enum
    Public Enum eDifficulty As Integer
        Hardest
        Hard
        Medium
        Easy
        Easiest
    End Enum
    Public Enum eGraphicDetails As Byte
        Low = 1
        Medium
        High
    End Enum
    Public Enum eDoubleButtonMouse As Byte
        SingleButtonMouse = 1
        DoubleButtonMouse
    End Enum
    Public Enum eMinimapType As Byte
        Normal
        Military = 2
        Economic
    End Enum
    Public Enum eTeam As Byte
        None
        Team1
        Team2
        Team3
        Team4
        Random
    End Enum
    Public Enum eScenario As Byte
        Unavailable
        Won
        Current
    End Enum
    Public Class Account
        Public Class Player
            Private pCivilization As Byte
            Public Property Civilization As Byte
                Get
                    Return pCivilization
                End Get
                Set(value As Byte)
                    pCivilization = value
                End Set
            End Property
            Private pPlayerNumber As Short
            Public ReadOnly Property PlayerNumber As Short
                Get
                    Return pPlayerNumber
                End Get
            End Property
            Private pTeam As eTeam
            Public Property Team As eTeam
                Get
                    Return pTeam
                End Get
                Set(value As eTeam)
                    pTeam = value
                End Set
            End Property
        End Class
        Public Class Campaign
            Private pName As String
            Public Property Name As String
                Get
                    Return pName
                End Get
                Set(value As String)
                    pName = value
                End Set
            End Property
            Private pScenariosCount As Integer
            Public ReadOnly Property ScenariosCount As Integer
                Get
                    Return pScenariosCount
                End Get
            End Property
            Private pWonCount As Integer
            Public Property WonCount As Integer
                Get
                    Return pWonCount
                End Get
                Set(value As Integer)
                    pWonCount = value
                End Set
            End Property
            Private pScenarios As List(Of eScenario)
            Public Property Scenarios As List(Of eScenario)
                Get
                    Return pScenarios
                End Get
                Set(value As List(Of eScenario))
                    pScenarios = value
                End Set
            End Property
        End Class
        Public Class MP_Player
            Private pCivilization As Byte
            Public Property Civilization As Byte
                Get
                    Return pCivilization
                End Get
                Set(value As Byte)
                    pCivilization = value
                End Set
            End Property
            Private pPlayerNumber As Short
            Public ReadOnly Property PlayerNumber As Short
                Get
                    Return pPlayerNumber
                End Get
            End Property
        End Class
        Private pName As String
        Public Property Name As String
            Get
                Return pName
            End Get
            Set(value As String)
                pName = value
            End Set
        End Property
        Private pAccountID As Integer
        Public Property AccountID As Integer
            Get
                Return pAccountID
            End Get
            Set(value As Integer)
                pAccountID = value
            End Set
        End Property
        Private pSelectedCampaignIndex As Integer
        Private pCampaignsCount As Integer
        Public ReadOnly Property CampaignsCount As Integer
            Get
                Return pCampaignsCount
            End Get
        End Property
        Private pCampaigns As List(Of Campaign)
        Public Property Campaigns As List(Of Campaign)
            Get
                Return pCampaigns
            End Get
            Set(value As List(Of Campaign))
                pCampaigns = value
            End Set
        End Property
        Private pMapSize As eMapSize
        Public Property MapSize As eMapSize
            Get
                Return pMapSize
            End Get
            Set(value As eMapSize)
                pMapSize = value
            End Set
        End Property
        Private pMapType As eMapType
        Public Property MapType As eMapType
            Get
                Return pMapType
            End Get
            Set(value As eMapType)
                pMapType = value
            End Set
        End Property
        Private pVisibility As eVisibility
        Public Property Visibility As eVisibility
            Get
                Return pVisibility
            End Get
            Set(value As eVisibility)
                pVisibility = value
            End Set
        End Property
        Private pVictory As eVictory
        Public Property Victory As eVictory
            Get
                Return pVictory
            End Get
            Set(value As eVictory)
                pVictory = value
            End Set
        End Property
        Private pVictoryParam As Integer
        Public Property VictoryParam As Integer
            Get
                Return pVictoryParam
            End Get
            Set(value As Integer)
                pVictoryParam = value
            End Set
        End Property
        Private pTeamTogether As Byte
        Public Property TeamTogether As Byte
            Get
                Return pTeamTogether
            End Get
            Set(value As Byte)
                pTeamTogether = value
            End Set
        End Property
        Private pFullTech As Byte
        Public Property FullTech As Byte
            Get
                Return pFullTech
            End Get
            Set(value As Byte)
                pFullTech = value
            End Set
        End Property
        Private pResource As eResource
        Public Property Resource As eResource
            Get
                Return pResource
            End Get
            Set(value As eResource)
                pResource = value
            End Set
        End Property
        Private pStartAge As eStartAge
        Public Property StartAge As eStartAge
            Get
                Return pStartAge
            End Get
            Set(value As eStartAge)
                pStartAge = value
            End Set
        End Property
        Private pGameMode As eGameMode
        Public Property GameMode As eGameMode
            Get
                Return pGameMode
            End Get
            Set(value As eGameMode)
                pGameMode = value
            End Set
        End Property
        Private pMaxPopulation As Byte
        Public Property MaxPopulation As Byte
            Get
                Return pMaxPopulation
            End Get
            Set(value As Byte)
                pMaxPopulation = value
            End Set
        End Property
        Private pMapTypeName As String
        Public ReadOnly Property MapTypeName As String
            Get
                Return pMapTypeName
            End Get
        End Property
        Private pPlayers As List(Of Player)
        Public Property Players As List(Of Player)
            Get
                Return pPlayers
            End Get
            Set(value As List(Of Player))
                pPlayers = value
            End Set
        End Property
        Private pPlayerCount As Integer
        Public ReadOnly Property PlayerCount As Integer
            Get
                Return pPlayerCount
            End Get
        End Property
        Private pDifficulty As eDifficulty
        Public Property Difficulty As eDifficulty
            Get
                Return pDifficulty
            End Get
            Set(value As eDifficulty)
                pDifficulty = value
            End Set
        End Property
        Private pLockTeam As Byte
        Public Property LockTeam As Byte
            Get
                Return pLockTeam
            End Get
            Set(value As Byte)
                pLockTeam = value
            End Set
        End Property
        Private pMP_MapSize As eMapSize
        Public Property MP_MapSize As eMapSize
            Get
                Return pMP_MapSize
            End Get
            Set(value As eMapSize)
                pMP_MapSize = value
            End Set
        End Property
        Private pMP_MapType As eMapType
        Public Property MP_MapType As eMapType
            Get
                Return pMP_MapType
            End Get
            Set(value As eMapType)
                pMP_MapType = value
            End Set
        End Property
        Private pMP_Visibility As eVisibility
        Public Property MP_Visibility As eVisibility
            Get
                Return pMP_Visibility
            End Get
            Set(value As eVisibility)
                pMP_Visibility = value
            End Set
        End Property
        Private pMP_Victory As eVictory
        Public Property MP_Victory As eVictory
            Get
                Return pMP_Victory
            End Get
            Set(value As eVictory)
                pMP_Victory = value
            End Set
        End Property
        Private pMP_VictoryParam As Integer
        Public Property MP_VictoryParam As Integer
            Get
                Return pMP_VictoryParam
            End Get
            Set(value As Integer)
                pMP_VictoryParam = value
            End Set
        End Property
        Private pMP_TeamTogether As Byte
        Public Property MP_TeamTogether As Byte
            Get
                Return pMP_TeamTogether
            End Get
            Set(value As Byte)
                pMP_TeamTogether = value
            End Set
        End Property
        Private pMP_FullTech As Byte
        Public Property MP_FullTech As Byte
            Get
                Return pMP_FullTech
            End Get
            Set(value As Byte)
                pMP_FullTech = value
            End Set
        End Property
        Private pMP_Resources As eResource
        Public Property MP_Resources As eResource
            Get
                Return pMP_Resources
            End Get
            Set(value As eResource)
                pMP_Resources = value
            End Set
        End Property
        Private pMP_StartAge As eStartAge
        Public Property MP_StartAge As eStartAge
            Get
                Return pMP_StartAge
            End Get
            Set(value As eStartAge)
                pMP_StartAge = value
            End Set
        End Property
        Private pMP_GameMode As eGameMode
        Public Property MP_GameMode As eGameMode
            Get
                Return pMP_GameMode
            End Get
            Set(value As eGameMode)
                pMP_GameMode = value
            End Set
        End Property
        Private pMP_MaxPopulation As Byte
        Public Property MP_MaxPopulation As Byte
            Get
                Return pMP_MaxPopulation
            End Get
            Set(value As Byte)
                pMP_MaxPopulation = value
            End Set
        End Property
        Private pMP_MapTypeName As String
        Public ReadOnly Property MP_MapTypeName As String
            Get
                Return pMP_MapTypeName
            End Get
        End Property
        Private pMP_Players As List(Of MP_Player)
        Public Property MP_Players As List(Of MP_Player)
            Get
                Return pMP_Players
            End Get
            Set(value As List(Of MP_Player))
                pMP_Players = value
            End Set
        End Property
        Private pMP_Difficulty As eDifficulty
        Public Property MP_Difficulty As eDifficulty
            Get
                Return pMP_Difficulty
            End Get
            Set(value As eDifficulty)
                pMP_Difficulty = value
            End Set
        End Property
        Private pMP_LockTeam As Byte
        Public Property MP_LockTeam As Byte
            Get
                Return pMP_LockTeam
            End Get
            Set(value As Byte)
                pMP_LockTeam = value
            End Set
        End Property
        Private pMP_LockSpeed As Byte
        Public Property MP_LockSpeed As Byte
            Get
                Return pMP_LockSpeed
            End Get
            Set(value As Byte)
                pMP_LockSpeed = value
            End Set
        End Property
        Private pCampaignDifficulty As eDifficulty
        Public Property CampaignDifficulty As eDifficulty
            Get
                Return pCampaignDifficulty
            End Get
            Set(value As eDifficulty)
                pCampaignDifficulty = value
            End Set
        End Property
        Private pMusicVolume As Integer
        Public Property MusicVolume As Integer
            Get
                Return pMusicVolume
            End Get
            Set(value As Integer)
                pMusicVolume = value
            End Set
        End Property
        Private pEffectVolume As Integer
        Public Property EffectVolume As Integer
            Get
                Return pEffectVolume
            End Get
            Set(value As Integer)
                pEffectVolume = value
            End Set
        End Property
        Private pScreenRollingSpeed As Byte
        Public Property ScreenRollingSpeed As Byte
            Get
                Return pScreenRollingSpeed
            End Get
            Set(value As Byte)
                pScreenRollingSpeed = value
            End Set
        End Property
        Private pGraphicDetails As eGraphicDetails
        Public Property GraphicDetails As eGraphicDetails
            Get
                Return pGraphicDetails
            End Get
            Set(value As eGraphicDetails)
                pGraphicDetails = value
            End Set
        End Property
        Private pResolutionWidth As Short
        Public Property ResolutionWidth As Short
            Get
                Return pResolutionWidth
            End Get
            Set(value As Short)
                pResolutionWidth = value
            End Set
        End Property
        Private pResolutionHeight As Short
        Public Property ResolutionHeight As Short
            Get
                Return pResolutionHeight
            End Get
            Set(value As Short)
                pResolutionHeight = value
            End Set
        End Property
        Private pTaunt As Byte
        Public Property Taunt As Byte
            Get
                Return pTaunt
            End Get
            Set(value As Byte)
                pTaunt = value
            End Set
        End Property
        Private pSingleClickGarrison As Byte
        Public Property SingleClickGarrison As Byte
            Get
                Return pSingleClickGarrison
            End Get
            Set(value As Byte)
                pSingleClickGarrison = value
            End Set
        End Property
        Private pDoubleButtonMouse As eDoubleButtonMouse
        Public Property DoubleButtonMouse As eDoubleButtonMouse
            Get
                Return pDoubleButtonMouse
            End Get
            Set(value As eDoubleButtonMouse)
                pDoubleButtonMouse = value
            End Set
        End Property
        Private pFriendlyColor As Byte
        Public Property FriendlyColor As Byte
            Get
                Return pFriendlyColor
            End Get
            Set(value As Byte)
                pFriendlyColor = value
            End Set
        End Property
        Private pMinimapType As eMinimapType
        Public Property MinimapType As eMinimapType
            Get
                Return pMinimapType
            End Get
            Set(value As eMinimapType)
                pMinimapType = value
            End Set
        End Property
        Private pScoreDisplay As Byte
        Public Property ScoreDisplay As Byte
            Get
                Return pScoreDisplay
            End Get
            Set(value As Byte)
                pScoreDisplay = value
            End Set
        End Property
        Private pF11Display As Byte
        Public Property F11Display As Byte
            Get
                Return pF11Display
            End Get
            Set(value As Byte)
                pF11Display = value
            End Set
        End Property
        Private pCampaignDifficultyAsked As Byte
        Public ReadOnly Property CampaignDifficultyAsked As Byte
            Get
                Return pCampaignDifficultyAsked
            End Get
        End Property
    End Class
    Private pNextAccountIndex As Integer
    Private pSelectedAccountIndex As Integer
    Private pNumAccounts As Integer
    Private pAccounts As New List(Of Account)
    Public Sub New(ByVal fn As String)
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("gb2312")
        Using ds As IO.Compression.DeflateStream = New IO.Compression.DeflateStream(New IO.FileStream(fn, IO.FileMode.Open, IO.FileAccess.Read), IO.Compression.CompressionMode.Decompress), br As New IO.BinaryReader(ds)
            br.ReadChars(4)
            br.ReadSingle()
            pNextAccountIndex = br.ReadInt32
            pSelectedAccountIndex = br.ReadInt32
            pNumAccounts = br.ReadInt32
            Dim acc As New Account
            acc.Name = enc.GetString(br.ReadBytes(255))
            acc.AccountID = br.ReadInt32

        End Using
    End Sub
End Class
