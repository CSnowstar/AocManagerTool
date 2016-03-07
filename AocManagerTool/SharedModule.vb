Imports System.Globalization

Module SharedModule
#Region "WIN32API"
  Public Const CCHDEVICENAME = 32
  Public Const CCHFORMNAME = 32
  Public Const HKEY_CURRENT_USER = &H80000001
  Public Const REG_DWORD = 4

  Public Structure DEVMODE
    <VBFixedString(CCHDEVICENAME), Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=CCHDEVICENAME)>
    Public dmDeviceName As String
    Dim dmSpecVersion As Short
    Dim dmDriverVersion As Short
    Dim dmSize As Short
    Dim dmDriverExtra As Short
    Dim dmFields As Integer
    Dim dmOrientation As Short
    Dim dmPaperSize As Short
    Dim dmPaperLength As Short
    Dim dmPaperWidth As Short
    Dim dmScale As Short
    Dim dmCopies As Short
    Dim dmDefaultSource As Short
    Dim dmPrintQuality As Short
    Dim dmColor As Short
    Dim dmDuplex As Short
    Dim dmYResolution As Short
    Dim dmTTOption As Short
    Dim dmCollate As Short
    <VBFixedString(CCHFORMNAME), Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst:=CCHFORMNAME)> Public dmFormName As String
    Dim dmLogPixels As Short
    Dim dmBitsPerPel As Integer
    Dim dmPelsWidth As Integer
    Dim dmPelsHeight As Integer
    Dim dmDisplayFlags As Integer
    Dim dmDisplayFrequency As Integer
  End Structure

  Public Declare Function EnumDisplaySettings Lib "user32" Alias "EnumDisplaySettingsA" (ByVal lpszDeviceName As Integer, ByVal iModeNum As Integer, ByRef lpDevMode As DEVMODE) As Boolean
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="LoadLibraryA")>
  Public Function LoadLibrary(<Runtime.InteropServices.In> ByVal lpLibFileName As String) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="LoadLibraryExA")>
  Public Function LoadLibraryEx(<Runtime.InteropServices.In> ByVal lpLibFileName As String, ByVal hFile As Integer, ByVal dwFlags As Integer) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="FreeLibrary")>
  Public Function FreeLibrary(<Runtime.InteropServices.In> ByVal hLibModule As IntPtr) As Integer
  End Function
  <Runtime.InteropServices.DllImport("user32", EntryPoint:="LoadStringA", SetLastError:=True)>
  Public Function LoadString(<Runtime.InteropServices.In> ByVal hInstance As IntPtr,
                                <Runtime.InteropServices.In> ByVal uID As Integer,
                                <Runtime.InteropServices.Out, Runtime.InteropServices.In> ByVal lpBuffer As System.Text.StringBuilder,
                                <Runtime.InteropServices.In> ByVal nBufferMax As Integer) As Integer
  End Function
  Public Const RT_STRING = 6
  Public Delegate Function ERNP(ByVal hModule As IntPtr, ByVal lpszType As Integer, ByVal lpszName As IntPtr, ByVal lParam As Integer) As Boolean
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="EnumResourceNamesA")>
  Public Function EnumResourceNames(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                        <Runtime.InteropServices.In> ByVal lpszType As Integer,
                                        <Runtime.InteropServices.In> ByVal lpEnumFunc As ERNP,
                                        <Runtime.InteropServices.In> ByVal lParam As Integer) As Integer
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="FindResourceA")>
  Public Function FindResourceA(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                    <Runtime.InteropServices.In> ByVal lpName As Integer,
                                    <Runtime.InteropServices.In> ByVal lpType As Integer) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="LoadResource")>
  Public Function LoadResource(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                    <Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="LockResource")>
  Public Function LockResource(<Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="SizeofResource")>
  Public Function SizeofResource(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                    <Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As Integer
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="BeginUpdateResource")>
  Public Function BeginUpdateResource(<Runtime.InteropServices.In> ByVal pFileName As String,
                                        <Runtime.InteropServices.In> ByVal bDeleteExistingResources As Boolean) As IntPtr
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="UpdateResourceA")>
  Public Function UpdateResource(<Runtime.InteropServices.In> ByVal hUpdate As IntPtr,
                                    <Runtime.InteropServices.In> ByVal lpType As Integer,
                                    <Runtime.InteropServices.In> ByVal lpName As Integer,
                                    <Runtime.InteropServices.In> ByVal wLanguage As Short,
                                    <Runtime.InteropServices.In, Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPArray)> ByVal lpData As Byte(),
                                    <Runtime.InteropServices.In> ByVal cbData As Integer) As Boolean
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="EndUpdateResource")>
  Public Function EndUpdateResource(<Runtime.InteropServices.In> ByVal hUpdate As IntPtr,
                                        <Runtime.InteropServices.In> ByVal fDiscard As Boolean) As Integer
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="GetPrivateProfileStringA")>
  Public Function GetPrivateProfileString(<Runtime.InteropServices.In> ByVal lpAppName As String,
                                    <Runtime.InteropServices.In> ByVal lpKeyName As String,
                                    <Runtime.InteropServices.In> ByVal lpDefault As String,
                                    <Runtime.InteropServices.Out> ByVal lpReturnedString As Text.StringBuilder,
                                    <Runtime.InteropServices.In> ByVal nSize As Integer,
                                    <Runtime.InteropServices.In> ByVal lpFileName As String) As Integer
  End Function
  <Runtime.InteropServices.DllImport("kernel32", EntryPoint:="WritePrivateProfileStringA")>
  Public Function WritePrivateProfileString(<Runtime.InteropServices.In> ByVal lpAppName As String,
                                    <Runtime.InteropServices.In> ByVal lpKeyName As String,
                                    <Runtime.InteropServices.In> ByVal lpString As String,
                                    <Runtime.InteropServices.In> ByVal lpFileName As String) As Integer
  End Function
#End Region
  Public Enum DoAfterPing
    Frontpage = 0
    Update = 1
    Verify = 2
  End Enum

  Public Class ModInfo
    Public Property Order As Integer
    Public Property Id As Integer
    Public Property Title As String
    Public Property Exe As String
  End Class

  Public Const gcsDirName = "HawkEmpire"
  'Public Const gcsFtpAddress = "ftp://121.199.22.226/web/hawkclient/"
  'Public Const gcsFtpUsername = "hawkaoc_net"
  'Public Const gcsFtpPassword = "qssq1012xyz"
  Public Const gcsHawkaocUrl = "www.hawkaoc.net"
  Public Const gcsMySqlServer = "121.42.152.168"
  Public Const gcsMySqlUser = "amtclient"
  Public Const gcsMySqlPassword = "read@hawkempire"
  Public Const gcsMySqlPort = "" '5899
  Public Const gcsMySqlDatabase = "ssrc"
  Public Const gcsRC = "http://121.42.152.168/ssrc/"
  Public Const gcsPmd5 = "10f0dce340bc2008d8e620ffce2537ca"
  Public Const gciDlPkgSize = 4096
  Public gApp As Application
  Public gwMain As MainWindow
  Public gwWorkshop As Workshop
  Public gwIdEditor As ideditor
  Public gwSceTrans As sceTrans
  Public gwDllEditor As DllEditor
  Public gwUserMan As UserManager
  Public gwHkiEditor As hkiEditor
  Public gxConfig, gxVersion, gxLocalRes As XElement
  Public gtConn, gtConnRes As New MySql.Data.MySqlClient.MySqlConnection
  Public gpGameProc As New Process With {.EnableRaisingEvents = True}
  Public gbGameRunning As Boolean = False
  Public gbRunUpdateOnLoad As Boolean = True
  Public gbOnline As Boolean = False
  Public glRes As New List(Of gcRes)
  Public gsHawkempirePath, gsManagerPath As String
  Public gtModsInfo As New ObjectModel.ObservableCollection(Of ModInfo)
  Public gPing As Net.NetworkInformation.Ping

  Public Function CSID2Int(ByVal CSID As String) As Integer
    Const cs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!-._~"
    Dim ret As Integer = 0
    For i = 0 To CSID.Length - 1
      ret += cs.Length ^ i * cs.IndexOf(CSID.Substring(i, 1))
    Next
    Return ret
  End Function

  Public Function Int2CSID(ByVal i As Integer) As String
    Const cs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!-._~"
    Dim ret As String = String.Empty
    Do
      ret &= cs.Substring(i Mod cs.Length, 1)
      i \= cs.Length
    Loop While i
    Return ret
  End Function

  Public Function FindVisualChild(Of T)(obj As DependencyObject) As DependencyObject
    For i = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
      Dim child As DependencyObject = VisualTreeHelper.GetChild(obj, i)
      If Not IsNothing(child) And TypeOf child Is T Then
        Return child
      Else
        Return FindVisualChild(Of T)(child)
      End If
    Next
    Return Nothing
  End Function

  Public Function GetLogicalChildren(obj As DependencyObject) As List(Of DependencyObject)
    Dim ret As New List(Of DependencyObject)
    For Each child As DependencyObject In LogicalTreeHelper.GetChildren(obj).OfType(Of DependencyObject)()
      ret.Add(child)
      ret.AddRange(GetLogicalChildren(child))
    Next
    Return ret
  End Function

  Public Function UnixTimeToDate(ByVal ut As Integer) As Date
    Return DateAdd(DateInterval.Second, ut, #1/1/1970#)
  End Function

  Public Function IsAocStarted() As Boolean
    Return Process.GetProcessesByName("age2_x1").Count
  End Function

  Public Function IsDirectoryEmpty(d As String) As Boolean
    Dim di As IO.DirectoryInfo = New IO.DirectoryInfo(d)
    For Each dire In di.EnumerateDirectories("*", IO.SearchOption.AllDirectories)
      dire.EnumerateFiles()
    Next
    Return True
  End Function
  Public Structure hkiFile
    Public version As Single
    Public numGroups As Integer
    Public Groups As structGroup()
    Public Structure structGroup
      Public numHotkeys As Integer
      Public Hotkeys As Hotkey()
      Public Structure Hotkey
        Public key As Forms.Keys
        Public dllEntry As Integer
        Public ctrl As Byte
        Public alt As Byte
        Public shift As Byte
        Public reserved As Byte
      End Structure
    End Structure
  End Structure
End Module

Public Class ImageConverter
  Implements IValueConverter

  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    Return New BitmapImage(New Uri($"resource\res{value}.png", UriKind.Relative))
  End Function

  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    Throw New NotImplementedException()
  End Function
End Class

Public Class FilesizeConverter
  Implements IValueConverter

  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    Select Case value
      Case Is <= 2 << 10
        Return value & " B"
      Case Is <= 2 << 20
        Return (value >> 10) & " K"
      Case Is <= 2L << 30
        Return (value >> 20) & " M"
      Case Else
        Return ""
    End Select
  End Function

  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    Throw New NotImplementedException()
  End Function
End Class

Public Class ResourceStatusConverter
  Implements IValueConverter

  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    Dim clr(,) As Color = {{Color.FromRgb(3, 157, 0), Color.FromRgb(5, 196, 0), Color.FromRgb(18, 217, 13)},'Install
        {Color.FromRgb(167, 10, 36), Color.FromRgb(196, 0, 32), Color.FromRgb(239, 67, 95)},'Delete
        {Color.FromRgb(2, 144, 217), Color.FromRgb(0, 174, 255), Color.FromRgb(43, 187, 254)},'Enable, Start
        {Color.FromRgb(90, 106, 164), Color.FromRgb(116, 129, 176), Color.FromRgb(133, 147, 199)}} 'Disable
    Select Case value
      Case gcRes.ResourceStatus.CanInstall
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(0, 0))
          Case "Back"
            Return New SolidColorBrush(clr(0, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(0, 2))
          Case "Text"
            Return "安装资源"
          Case Else
            Return Nothing
        End Select
      Case gcRes.ResourceStatus.CanDelete
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(1, 0))
          Case "Back"
            Return New SolidColorBrush(clr(1, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(1, 2))
          Case "Text"
            Return "删除资源"
          Case Else
            Return Nothing
        End Select
      Case gcRes.ResourceStatus.CanStart
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(2, 0))
          Case "Back"
            Return New SolidColorBrush(clr(2, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(2, 2))
          Case "Text"
            Return "启动资源"
          Case Else
            Return Nothing
        End Select
      Case gcRes.ResourceStatus.CanEnable
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(2, 0))
          Case "Back"
            Return New SolidColorBrush(clr(2, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(2, 2))
          Case "Text"
            Return "启用模组"
          Case Else
            Return Nothing
        End Select
      Case gcRes.ResourceStatus.CanDisable
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(3, 0))
          Case "Back"
            Return New SolidColorBrush(clr(3, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(3, 2))
          Case "Text"
            Return "停用模组"
          Case Else
            Return Nothing
        End Select
      Case gcRes.ResourceStatus.CanUpdate
        Select Case parameter
          Case "Border"
            Return New SolidColorBrush(clr(2, 0))
          Case "Back"
            Return New SolidColorBrush(clr(2, 1))
          Case "MouseOver"
            Return New SolidColorBrush(clr(2, 2))
          Case "Text"
            Return "更新资源"
          Case Else
            Return Nothing
        End Select
      Case Else
        Return Nothing
    End Select
  End Function

  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    Throw New NotImplementedException()
  End Function
End Class

Public Class VersionConverter
  Implements IValueConverter

  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    Dim ret As New List(Of String)
    If value = 0 Then ret.Add("N/A")
    If (value And 2) = 2 Then ret.Add("AoK")
    If (value And 4) = 4 Then ret.Add("1.0A")
    If (value And 8) = 8 Then ret.Add("1.0C")
    If (value And &H10) = &H10 Then ret.Add("1.4")
    If (value And &H20) = &H20 Then ret.Add("AoFE")
    Return Join(ret.ToArray, "/")
  End Function

  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    Throw New NotImplementedException()
  End Function
End Class

Public Class gcRes
  Implements ComponentModel.INotifyPropertyChanged

  Public Structure ImageInfo
    Public Id As Integer
    Public w As Integer
    Public h As Integer
  End Structure

  Public Structure UpdateFile
    Public Id As Integer
  End Structure

  Private _Status As ResourceStatus
  Private _Progress As Integer

  Public Enum ResourceStatus
    CanInstall
    CanDelete
    CanStart
    CanEnable
    CanDisable
    CanUpdate
  End Enum
  Public Event PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
  Public Property FileSize As Integer
  Public Property CreateDate As Date
  Public Property UpdateDate As Date
  Public Property LatestFileUpdate As Integer
  Public Property ResId As Integer
  Public Property ResType As String
  Public Property Name As String
  Public Property AuthorUid As Integer
  Public Property AuthorName As String
  Public Property Summary As String
  Public Property Intro As String
  Public Property Images As New List(Of ImageInfo)
  Public Property Rate As Single?
  Public Property GameVersion As Integer
  Public Property FromURL As String
  Public Property Downloads As Integer
  Public Property Button As Button
  Public Property UpdateFiles As New List(Of UpdateFile)
  Public Property Status As ResourceStatus
    Get
      Return _Status
    End Get
    Set(value As ResourceStatus)
      _Status = value
      RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs(NameOf(Status)))
    End Set
  End Property
  Public Property Progress As Integer
    Get
      Return _Progress
    End Get
    Set(value As Integer)
      _Progress = value
      RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs(NameOf(Progress)))
    End Set
  End Property
End Class
