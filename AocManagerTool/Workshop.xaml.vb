﻿Public Class Workshop
  Private Enum eWhichPage
    Loading
    List
    Info
  End Enum
  Public Structure bgwImageArgs
    Public Id As Integer
    Public Source As Image
    Public Filename As String
    Public w As Integer
    Public h As Integer
  End Structure
  Public Structure resFile
    Public id As Integer
    Public name As String
    Public update As Integer
    Public Sub New(id As Integer, name As String, update As Integer)
      Me.id = id
      Me.name = name
      Me.update = update
    End Sub
  End Structure
  Dim ceWhichPage As eWhichPage = eWhichPage.Loading
  Dim Cookie As Net.Cookie
  Dim cdDrsTableName As New Dictionary(Of String, String)()

  Private Sub Window_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
    If e.LeftButton = MouseButtonState.Pressed Then DragMove()
    e.Handled = True
  End Sub

  Private Sub btnMinimize_Click(sender As Object, e As RoutedEventArgs)
    WindowState = WindowState.Minimized
    e.Handled = True
  End Sub

  Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
    Close()
    e.Handled = True
  End Sub

  Private Sub Workshop_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
    gwWorkshop = Nothing
    If gtConnRes.State <> Data.ConnectionState.Closed Then gtConnRes.Close()
  End Sub

  Private Sub Workshop_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    cdDrsTableName.Add(".slp", " pls")
    cdDrsTableName.Add(".wav", " vaw")
    cdDrsTableName.Add(".bina", "anib")
    cdDrsTableName.Add(".shp", " phs")
    Dim bgwInit As ComponentModel.BackgroundWorker = FindResource("bgwInit")
    bgwInit.RunWorkerAsync()
  End Sub

  Private Sub bgwInit_DoWork(sender As ComponentModel.BackgroundWorker, e As ComponentModel.DoWorkEventArgs)
    Try
      gtConnRes.ConnectionString = $"server={gcsMySqlServer};uid={gcsMySqlUser};password={gcsMySqlPassword};database={gcsMySqlDatabase};port={gcsMySqlPort}"
      gtConnRes.Open()
      Dim comm As New MySql.Data.MySqlClient.MySqlCommand("select id,name,e_type,author_bbsid,author_name,t_create,totalsize,summary,content,fromurl,b_gamebase,votereview,count_download,t_update,t_fileup from res where e_type!='undefined' and status=2", gtConnRes)
      Dim rd = comm.ExecuteReader()
      glRes.Clear()
      Do While rd.Read()
        Dim aRes As New gcRes
        aRes.ResId = rd.GetUInt32(0)
        aRes.Name = rd.GetString(1)
        aRes.ResType = rd.GetString(2)
        aRes.AuthorUid = rd.GetUInt32(3)
        aRes.AuthorName = rd.GetString(4)
        aRes.CreateDate = UnixTimeToDate(rd.GetInt32(5))
        aRes.FileSize = rd.GetUInt64(6)
        aRes.Summary = rd.GetString(7)
        aRes.Intro = rd.GetString(8)
        aRes.FromURL = rd.GetString(9)
        aRes.GameVersion = rd.GetInt32(10)
        aRes.Rate = rd.GetInt32(11) / 1000
        aRes.Downloads = rd.GetInt32(12)
        aRes.UpdateDate = UnixTimeToDate(rd.GetInt32(13))
        aRes.LatestFileUpdate = rd.GetInt32(14)
        If aRes.Rate = 0.0 Then aRes.Rate = Nothing
        aRes.Status = (From x In gxLocalRes.<res>
                       Where CInt(x.<id>(0)) = aRes.ResId
                       Select CInt(x.<status>(0))).SingleOrDefault()
        'If aRes.LatestFileUpdate > CInt(localNode("date").InnerText) Then aRes.Status = gcRes.ResourceStatus.CanUpdate
        glRes.Add(aRes)
      Loop
      rd.Close()
      GetCookie()
      glRes = glRes.OrderByDescending(Function(p) p.UpdateDate).ToList()
    Catch ex As MySql.Data.MySqlClient.MySqlException
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub GetCookie()
    Try
      Dim rq As Net.HttpWebRequest = Net.WebRequest.Create($"{gcsRC}login.php")
      rq.AllowAutoRedirect = False
      rq.CookieContainer = New Net.CookieContainer
      rq.Method = "POST"
      rq.ContentType = "application/x-www-form-urlencoded"
      Dim postData As Byte() = Text.Encoding.ASCII.GetBytes($"pmd5={gcsPmd5}&u=amt")
      rq.ContentLength = postData.Length
      rq.GetRequestStream().Write(postData, 0, postData.Length)
      Dim rp As Net.HttpWebResponse = rq.GetResponse()
      If rp.Cookies.Count > 0 Then Cookie = rp.Cookies(0)
      rp.Close()
    Catch ex As Net.WebException
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub bgwInit_RunWorkerCompleted(sender As ComponentModel.BackgroundWorker, e As ComponentModel.RunWorkerCompletedEventArgs)
    lstRes.ItemsSource = glRes
    txbWorkshopLoading.Visibility = Visibility.Hidden
    lstRes.Visibility = Visibility.Visible
    ceWhichPage = eWhichPage.List
    btnWorkshopBack.ToolTip = "返回帝国时代管家主界面。"
  End Sub

  Private Sub btnResList_Click(sender As Button, e As RoutedEventArgs)
    Dim cRes = CType(sender.Tag, gcRes)
    Dim xmlEle = (From x In gxLocalRes.<res>
                  Where CInt(x.<id>(0)) = cRes.ResId
                  Select x).SingleOrDefault()
    cRes.Button = sender
    Select Case cRes.Status
      Case gcRes.ResourceStatus.CanInstall
        CType(LogicalTreeHelper.FindLogicalNode(sender.Parent, NameOf(txb)), FrameworkElement).Visibility = Visibility.Visible
        CType(LogicalTreeHelper.FindLogicalNode(sender.Parent, NameOf(prb)), FrameworkElement).Visibility = Visibility.Visible
        Dim bgwDownloader As ComponentModel.BackgroundWorker = FindResource(NameOf(bgwDownloader))
        bgwDownloader.RunWorkerAsync(cRes)
      Case gcRes.ResourceStatus.CanDelete
        If cRes.ResType = "mod" And cRes.Name = gxConfig.<aocversion>.Value Then
          MessageBox.Show("不能卸载已被选定为当前游戏版本的MOD，请选择其他游戏版本程序后再卸载。")
          Exit Sub
        End If
        If MessageBox.Show($"确定要卸载资源 ""{cRes.Name}"" 吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) = MessageBoxResult.OK Then
          If cRes.ResType = "mod" Then
            For Each ele In gtModsInfo
              If ele.Id = cRes.ResId Then
                gtModsInfo.Remove(ele)
                Exit For
              End If
            Next
            For i = 3 To gwMain.stpGameList.Children.Count - 1
              If CType(CType(gwMain.stpGameList.Children(i), Button).Tag, ModInfo).Id = cRes.ResId Then
                gwMain.stpGameList.Children.RemoveAt(i)
                Exit For
              End If
            Next
            For Each ele In gwMain.wrpStarts.Children
              If CType(ele.Tag, ModInfo).Id = cRes.ResId Then
                gwMain.wrpStarts.Children.Remove(ele)
                Exit For
              End If
            Next
          End If
          For Each ele In xmlEle.<files>.<file>
            IO.File.Delete(gsHawkempirePath & ele.Value)
          Next
          For Each ele In xmlEle.<dirs>.<dir>
            If IO.Directory.Exists(gsHawkempirePath & ele.Value) AndAlso
              (New IO.DirectoryInfo(gsHawkempirePath & ele.Value).EnumerateFiles("*", IO.SearchOption.AllDirectories).Count = 0) Then
              IO.Directory.Delete(gsHawkempirePath & ele.Value, True)
            End If
          Next
          For Each ele In gxLocalRes.DocumentElement.SelectNodes(String.Format("res[id='{0}']", cRes.ResId))
            gxLocalRes.DocumentElement.RemoveChild(ele)
          Next
          cRes.Status = gcRes.ResourceStatus.CanInstall
          MessageBox.Show("资源 """ & cRes.Name & """ 卸载完毕。")
        End If
      Case gcRes.ResourceStatus.CanStart
        Dim exe As String = gsHawkempirePath & xmlEle("exe").InnerText
        If IO.File.Exists(exe) Then
          Dim p As New Process
          p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(exe)
          p.StartInfo.FileName = exe
          p.Start()
        Else
          MessageBox.Show("MOD 程序文件不存在，请联系资源上传人员。")
        End If
      Case gcRes.ResourceStatus.CanEnable
        If IsAocStarted() Then
          MessageBox.Show("帝国时代正在运行。请关闭帝国时代程序后，再启用模组。")
        Else
          Dim dic As New Dictionary(Of String, List(Of String))
          Dim ver As Integer = xmlEle("version").InnerText
          For Each fil In xmlEle.SelectNodes("files/file")
            Dim dirName As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(fil.InnerText)) '"graphics.drs"
            If Not dic.ContainsKey(dirName) Then dic.Add(dirName, New List(Of String))
            dic(dirName).Add(gsHawkempirePath & fil.InnerText)
          Next
          For Each entry In dic
            Dim drs As drsFile
            If (ver And 4) = 4 Or (ver And 8) = 8 Or (ver And &H10) = &H10 Then '1.0,1.4
              drs = New drsFile(IO.Path.Combine(gsHawkempirePath, "data", entry.Key))
            ElseIf (ver And &H20) = &H20 Then 'AoFE
              drs = New drsFile(IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\data", entry.Key))
            Else
              drs = New drsFile
            End If
            For Each slp In entry.Value
              Dim tableName As String = cdDrsTableName(IO.Path.GetExtension(slp))
              Dim nm As UInteger = IO.Path.GetFileNameWithoutExtension(slp)
              If Not drs.ContainsKey(tableName) Then
                drs.Add(tableName, New SortedDictionary(Of UInteger, Byte()))
              End If
              drs(tableName)(nm) = My.Computer.FileSystem.ReadAllBytes(slp)
            Next
            drs.Save()
          Next
          cRes.Status = gcRes.ResourceStatus.CanDisable
          xmlEle("status").InnerText = gcRes.ResourceStatus.CanDisable
        End If
      Case gcRes.ResourceStatus.CanDisable
        If IsAocStarted() Then
          MessageBox.Show("帝国时代正在运行。请关闭帝国时代程序后，再禁用模组。")
        Else
          Dim dic As New Dictionary(Of String, List(Of String))
          Dim ver As Integer = xmlEle("version").InnerText
          For Each fil As Xml.XmlElement In xmlEle.SelectNodes("files/file")
            Dim dirName As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(fil.InnerText)) '"graphics.drs"
            If Not dic.ContainsKey(dirName) Then dic.Add(dirName, New List(Of String))
            dic(dirName).Add(IO.Path.GetFileName(fil.InnerText))
          Next
          For Each entry In dic
            Dim drsMod, drsOrig As New drsFile
            If (ver And 4) = 4 Or (ver And 8) = 8 Or (ver And &H10) = &H10 Then '1.0,1.4
              drsMod = New drsFile(IO.Path.Combine(gsHawkempirePath, "data", entry.Key))
              drsOrig = New drsFile(IO.Path.Combine(gsHawkempirePath, "manager\drs", entry.Key))
            ElseIf (ver And &H20) = &H20 Then 'AoFE
              drsMod = New drsFile(IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\data", entry.Key))
              drsOrig = New drsFile(IO.Path.Combine(gsHawkempirePath, "manager\drs", IO.Path.GetFileNameWithoutExtension(entry.Key) & "_fe" & IO.Path.GetExtension(entry.Key)))
            End If
            For Each slp In entry.Value
              Dim slpId As String = IO.Path.GetFileNameWithoutExtension(slp)
              Dim slpType As String = cdDrsTableName(IO.Path.GetExtension(slp))
              If Not drsOrig.ContainsKey(slpType) OrElse Not drsOrig(slpType).ContainsKey(slpId) Then
                drsMod(slpType).Remove(slpId)
              Else
                drsMod(slpType)(slpId) = drsOrig(slpType)(slpId)
              End If
            Next
            drsMod.Save()
          Next
          xmlEle("status").InnerText = gcRes.ResourceStatus.CanEnable
          cRes.Status = gcRes.ResourceStatus.CanEnable
        End If
      Case gcRes.ResourceStatus.CanUpdate

    End Select
  End Sub

  Private Sub bgwDownloader_DoWork(ByVal sender As ComponentModel.BackgroundWorker, ByVal e As ComponentModel.DoWorkEventArgs)
    Dim cRes As gcRes = e.Argument
    Dim rq As Net.HttpWebRequest = Net.WebRequest.Create(gcsRC & "res.php?action=download&res=" & Int2CSID(cRes.ResId))
    rq.CookieContainer = New Net.CookieContainer
    rq.CookieContainer.Add(Cookie)
    Dim rp As Net.HttpWebResponse = rq.GetResponse
    rp.Close()
    If gtConnRes.State <> Data.ConnectionState.Open Then
      gtConnRes.ConnectionString = String.Format("server={0};uid={1};password={2};database={3};port={4}", gcsMySqlServer, gcsMySqlUser, gcsMySqlPassword, gcsMySqlDatabase, gcsMySqlPort)
      gtConnRes.Open()
    End If
    Dim comm As MySql.Data.MySqlClient.MySqlCommand = New MySql.Data.MySqlClient.MySqlCommand("select id,PathFile(id),t_update from resfile where resid=" & cRes.ResId, gtConnRes)
    Dim rd As MySql.Data.MySqlClient.MySqlDataReader = comm.ExecuteReader()
    Dim lFiles As New List(Of resFile)
    While rd.Read
      lFiles.Add(New resFile(rd.GetInt32(0), rd.GetString(1).Replace("/", "\"), rd.GetInt32(2)))
    End While
    rd.Close()
    Dim detectExist As Xml.XmlElement = gxLocalRes.DocumentElement.SelectSingleNode(String.Format("res[id='{0}']", cRes.ResId))
    If Not IsNothing(detectExist) Then gxLocalRes.DocumentElement.RemoveChild(detectExist)
    Dim resElement As Xml.XmlElement = gxLocalRes.DocumentElement.AppendChild(gxLocalRes.CreateElement("res"))
    resElement.AppendChild(gxLocalRes.CreateElement("id")).InnerText = cRes.ResId
    resElement.AppendChild(gxLocalRes.CreateElement("type")).InnerText = cRes.ResType
    resElement.AppendChild(gxLocalRes.CreateElement("title")).InnerText = cRes.Name
    Select Case cRes.ResType
      Case "cpx", "scx"
        resElement.AppendChild(gxLocalRes.CreateElement("status")).InnerText = gcRes.ResourceStatus.CanDelete
      Case "drs"
        resElement.AppendChild(gxLocalRes.CreateElement("status")).InnerText = gcRes.ResourceStatus.CanEnable
        resElement.AppendChild(gxLocalRes.CreateElement("version")).InnerText = cRes.GameVersion
      Case "mod"
        resElement.AppendChild(gxLocalRes.CreateElement("status")).InnerText = gcRes.ResourceStatus.CanStart
        resElement.AppendChild(gxLocalRes.CreateElement("exe")).InnerText = lFiles.Find(Function(p As resFile) p.name.EndsWith(".exe") And IO.Path.GetFileName(IO.Path.GetDirectoryName(p.name)) = "age2_x1").name
      Case Else
        resElement.AppendChild(gxLocalRes.CreateElement("status")).InnerText = gcRes.ResourceStatus.CanDelete
    End Select
    Dim dirElement As Xml.XmlElement = resElement.AppendChild(gxLocalRes.CreateElement("dirs"))
    Dim fileElement As Xml.XmlElement = resElement.AppendChild(gxLocalRes.CreateElement("files"))
    Dim dlBytes As Long = 0
    For Each fil In lFiles
      rq = Net.WebRequest.Create(gcsRC & "res.php?file=" & Int2CSID(fil.id))
      rq.CookieContainer = New Net.CookieContainer
      rq.CookieContainer.Add(Cookie)
      rp = rq.GetResponse
      Dim rpSm = rp.GetResponseStream
      Dim fullFN As String = gsHawkempirePath & fil.name
      Dim relPath As String = IO.Path.GetDirectoryName(fullFN.Replace(gsHawkempirePath, ""))
      Select Case cRes.ResType
        Case "drs"
          If IsNothing(dirElement.SelectSingleNode("dir['" & relPath & "']")) Then dirElement.AppendChild(gxLocalRes.CreateElement("dir")).InnerText = relPath
          If Not IO.Directory.Exists(IO.Path.GetDirectoryName(fullFN)) Then
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(fullFN))
          End If
        Case Else
          If Not IO.Directory.Exists(IO.Path.GetDirectoryName(fullFN)) Then
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(fullFN))
            dirElement.AppendChild(gxLocalRes.CreateElement("dir")).InnerText = relPath
          End If
      End Select
      Using fs As New IO.FileStream(fullFN, IO.FileMode.Create)
        Dim y(gciDlPkgSize - 1) As Byte
        Dim read As Integer
        Do
          read = rpSm.Read(y, 0, gciDlPkgSize)
          fs.Write(y, 0, read)
          dlBytes += read
          sender.ReportProgress(dlBytes / cRes.FileSize * 100, cRes)
        Loop While read
      End Using
      With fileElement.AppendChild(gxLocalRes.CreateElement("file"))
        .InnerText = fullFN.Replace(gsHawkempirePath, "")
        .Attributes.Append(gxLocalRes.CreateAttribute("date")).InnerText = fil.update
      End With
      rp.Close()
    Next
    e.Result = cRes
  End Sub

  Public Sub bgwDownloader_ProgressChanged(ByVal sender As Object, ByVal e As ComponentModel.ProgressChangedEventArgs)
    CType(e.UserState, gcRes).Progress = e.ProgressPercentage
  End Sub

  Public Sub bgwDownloader_RunWorkerCompleted(ByVal sender As ComponentModel.BackgroundWorker, ByVal e As ComponentModel.RunWorkerCompletedEventArgs)
    Dim cRes As gcRes = e.Result
    Select Case cRes.ResType
      Case "cpx", "rms", "ai", "scx"
        cRes.Status = gcRes.ResourceStatus.CanDelete
      Case "drs"
        cRes.Status = gcRes.ResourceStatus.CanEnable
      Case "mod"
        cRes.Status = gcRes.ResourceStatus.CanStart
        Dim modInfo As New ModInfo
        modInfo.Order = gtModsInfo.Count
        modInfo.Id = cRes.ResId
        modInfo.Title = cRes.Name
        modInfo.Exe = gxLocalRes.DocumentElement.SelectSingleNode(String.Format("res[id='{0}']", cRes.ResId))("exe").InnerText
        If Not gtModsInfo.Any(Function(p) p.Id = modInfo.Id) Then gtModsInfo.Add(modInfo)
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = modInfo.Title
        btn.Tag = modInfo
        AddHandler btn.Click, AddressOf gwMain.btnGameList_Click
        gwMain.stpGameList.Children.Add(btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = modInfo.Title
        btn.Tag = modInfo
        AddHandler btn.Click, AddressOf gwMain.btnSwitchToMod_Click
        gwMain.wrpStarts.Children.Add(btn)
      Case Else
        cRes.Status = gcRes.ResourceStatus.CanDelete
    End Select
    CType(LogicalTreeHelper.FindLogicalNode(cRes.Button.Parent, "txb"), FrameworkElement).Visibility = Visibility.Hidden
    CType(LogicalTreeHelper.FindLogicalNode(cRes.Button.Parent, "prb"), FrameworkElement).Visibility = Visibility.Hidden
  End Sub

  Private Sub btnWorkshopBack_Click(sender As Object, e As RoutedEventArgs)
    Select Case ceWhichPage
      Case eWhichPage.Loading
        Close()
      Case eWhichPage.List
        Close()
      Case eWhichPage.Info
        srvRes.Visibility = Visibility.Hidden
        lstRes.Visibility = Visibility.Visible
        ceWhichPage = eWhichPage.List
        btnWorkshopBack.ToolTip = "返回帝国时代管家主界面。"
    End Select
  End Sub

  Private Sub btnResListMore_Click(sender As Button, e As RoutedEventArgs)
    Dim cRes As gcRes = sender.Tag
    pop.Tag = cRes
    pop.PlacementTarget = LogicalTreeHelper.FindLogicalNode(sender.Parent, "btnResList")
    stpPopup.Children.Clear()
    stpPopup.Children.Add(FindResource("btnResForum"))
    stpPopup.Background = CType(pop.PlacementTarget, Button).Background
    Select Case cRes.Status
      Case gcRes.ResourceStatus.CanInstall
      Case gcRes.ResourceStatus.CanDelete
      Case gcRes.ResourceStatus.CanStart
        Select Case cRes.ResType
          Case "mod"
            stpPopup.Children.Add(FindResource("btnResDelete"))
        End Select
      Case gcRes.ResourceStatus.CanEnable
        Select Case cRes.ResType
          Case "drs"
            stpPopup.Children.Add(FindResource("btnResDelete"))
        End Select
      Case gcRes.ResourceStatus.CanDisable
        Select Case cRes.ResType
          Case "drs"
            stpPopup.Children.Add(FindResource("btnResDelete"))
        End Select
      Case gcRes.ResourceStatus.CanUpdate
    End Select
    pop.IsOpen = True
  End Sub

  Private Sub bdr_MouseLeftButtonDown(sender As Border, e As MouseButtonEventArgs)
    ShowInfo(sender.Tag)
  End Sub

  Private Sub ShowInfo(cRes As gcRes)
    cRes.Images.Clear()
    stpImages.Children.Clear()
    If gtConnRes.State <> Data.ConnectionState.Open Then
      gtConnRes.ConnectionString = String.Format("server={0};uid={1};password={2};database={3};port={4}", gcsMySqlServer, gcsMySqlUser, gcsMySqlPassword, gcsMySqlDatabase, gcsMySqlPort)
      gtConnRes.Open()
    End If
    Dim comm As MySql.Data.MySqlClient.MySqlCommand = New MySql.Data.MySqlClient.MySqlCommand("select id,w,h from resimg where resid=" & cRes.ResId, gtConnRes)
    Dim rd As MySql.Data.MySqlClient.MySqlDataReader = comm.ExecuteReader()
    While rd.Read
      cRes.Images.Add(New gcRes.ImageInfo With {.Id = rd.GetInt32(0), .w = rd.GetInt32(1), .h = rd.GetInt32(2)})
    End While
    rd.Close()
    For Each ele In cRes.Images
      Dim img As New Image
      img.Height = 110
      img.Margin = New Thickness(0, 0, 20, 0)
      RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality)
      img.ToolTip = "点击查看大图"
      img.Visibility = Visibility.Hidden
      AddHandler img.MouseDown, AddressOf img_MouseDown
      stpImages.Children.Add(img)
      Dim bgwImage As New ComponentModel.BackgroundWorker
      AddHandler bgwImage.DoWork, AddressOf bgwImage_DoWork
      AddHandler bgwImage.RunWorkerCompleted, AddressOf bgwImage_RunWorkerCompleted
      Dim args As New bgwImageArgs
      args.Id = ele.Id
      args.Source = img
      args.w = ele.w
      args.h = ele.h
      bgwImage.RunWorkerAsync(args)
    Next
    lstRes.Visibility = Visibility.Hidden
    srvRes.Visibility = Visibility.Visible
    ceWhichPage = eWhichPage.Info
    btnWorkshopBack.ToolTip = "返回资源列表界面。"
    grdRes.DataContext = cRes
  End Sub

  Private Sub bgwImage_DoWork(sender As ComponentModel.BackgroundWorker, e As ComponentModel.DoWorkEventArgs)
    Dim tmpFile As String = IO.Path.GetTempFileName
    Dim args As bgwImageArgs = e.Argument
    Dim rq As Net.HttpWebRequest, rp As Net.HttpWebResponse
    Try
      rq = Net.WebRequest.Create(gcsRC & "res.php?img=" & Int2CSID(args.Id))
      rq.AllowAutoRedirect = True
      rq.CookieContainer = New Net.CookieContainer
      rq.CookieContainer.Add(Cookie)
      rq.Method = "GET"
      rp = rq.GetResponse
      Using fs As New IO.FileStream(tmpFile, IO.FileMode.Create)
        rp.GetResponseStream.CopyTo(fs)
      End Using
      rp.Close()
      args.Filename = tmpFile
      e.Result = args
    Catch ex As Net.WebException
      e.Cancel = True
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub bgwImage_RunWorkerCompleted(ByVal sender As ComponentModel.BackgroundWorker, ByVal e As ComponentModel.RunWorkerCompletedEventArgs)
    If Not e.Cancelled Then
      Dim args As bgwImageArgs = e.Result
      WpfAnimatedGif.ImageBehavior.SetAnimatedSource(args.Source, New BitmapImage(New Uri(args.Filename, UriKind.Absolute)))
      'args.Source.Source = New BitmapImage(New Uri(args.Filename, UriKind.Absolute))
      args.Source.Tag = args
      args.Source.Visibility = Visibility.Visible
    End If
  End Sub

  Private Sub btnResDelete_Click(sender As Button, e As RoutedEventArgs)
    Dim cRes As gcRes = sender.Tag
    If cRes.ResType = "mod" And cRes.Name = gxConfig.DocumentElement.Item("aocversion").InnerText Then
      MessageBox.Show("不能卸载已被选定为当前游戏版本的MOD，请选择其他游戏版本程序后再卸载。")
      Exit Sub
    End If
    If MessageBox.Show("确定要卸载资源 """ & cRes.Name & """ 吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) = MessageBoxResult.OK Then
      If cRes.ResType = "mod" Then
        For Each ele As ModInfo In gtModsInfo
          If ele.Id = cRes.ResId Then gtModsInfo.Remove(ele) : Exit For
        Next
        For i = 3 To gwMain.stpGameList.Children.Count - 1
          If CType(CType(gwMain.stpGameList.Children(i), Button).Tag, ModInfo).Id = cRes.ResId Then gwMain.stpGameList.Children.RemoveAt(i) : Exit For
        Next
        For Each ele In gwMain.wrpStarts.Children
          If CType(ele.Tag, ModInfo).Id = cRes.ResId Then gwMain.wrpStarts.Children.Remove(ele) : Exit For
        Next
      End If
      For Each ele As Xml.XmlElement In gxLocalRes.DocumentElement.SelectNodes(String.Format("res[id='{0}']/files/file", cRes.ResId))
        IO.File.Delete(gsHawkempirePath & ele.InnerText)
      Next
      For Each ele As Xml.XmlElement In gxLocalRes.DocumentElement.SelectNodes(String.Format("res[id='{0}']/dirs/dir", cRes.ResId))
        If IO.Directory.Exists(gsHawkempirePath & ele.InnerText) AndAlso (New IO.DirectoryInfo(gsHawkempirePath & ele.InnerText).EnumerateFiles("*", IO.SearchOption.AllDirectories).Count = 0) Then IO.Directory.Delete(gsHawkempirePath & ele.InnerText, True)
      Next
      For Each ele As Xml.XmlElement In gxLocalRes.DocumentElement.SelectNodes(String.Format("res[id='{0}']", cRes.ResId))
        gxLocalRes.DocumentElement.RemoveChild(ele)
      Next
      cRes.Status = gcRes.ResourceStatus.CanInstall
      MessageBox.Show("资源 """ & cRes.Name & """ 卸载完毕。")
    End If
    pop.IsOpen = False
  End Sub

  Private Sub btnWorkshopDIY_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "drs")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopCampaign_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "cpx")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopScenario_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "scx")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopMap_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "rms")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopRecord_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "mgx")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopMod_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "mod")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnWorkshopAi_Click(sender As Object, e As RoutedEventArgs)
    If ceWhichPage = eWhichPage.Info Then btnWorkshopBack_Click(Me, Nothing)
    Dim listedRes As List(Of gcRes) = glRes.FindAll(Function(p As gcRes) p.ResType = "ai")
    lstRes.ItemsSource = listedRes
    e.Handled = True
  End Sub

  Private Sub btnResForum_Click(sender As Button, e As RoutedEventArgs)
    Dim cRes As gcRes = sender.Tag
    If Uri.IsWellFormedUriString(cRes.FromURL, UriKind.Absolute) Then Process.Start(cRes.FromURL) Else MessageBox.Show("本资源无对应帖子")
    e.Handled = True
  End Sub

  Private Sub btnSearchRes_Click(sender As Object, e As RoutedEventArgs)
    Select Case cboSearchRes.SelectedIndex
      Case 0
        lstRes.ItemsSource = glRes.FindAll(Function(p) p.Name.ToLower.Contains(txtSearchRes.Text.ToLower))
      Case 1
        lstRes.ItemsSource = glRes.FindAll(Function(p) p.AuthorName.ToLower.Contains(txtSearchRes.Text.ToLower))
      Case 2
        lstRes.ItemsSource = glRes.FindAll(Function(p) p.Intro.ToLower.Contains(txtSearchRes.Text.ToLower))
    End Select
    e.Handled = True
  End Sub

  Private Sub btnSort_Click(sender As Object, e As RoutedEventArgs)
    Dim lst As List(Of gcRes) = lstRes.ItemsSource
    If cboSort.SelectedIndex = 0 And cboOrder.SelectedIndex = 0 Then lstRes.ItemsSource = lst.OrderBy(Function(p) p.CreateDate).ToList
    If cboSort.SelectedIndex = 0 And cboOrder.SelectedIndex = 1 Then lstRes.ItemsSource = lst.OrderByDescending(Function(p) p.UpdateDate).ToList
    If cboSort.SelectedIndex = 1 And cboOrder.SelectedIndex = 0 Then lstRes.ItemsSource = lst.OrderBy(Function(p) p.Downloads).ToList
    If cboSort.SelectedIndex = 1 And cboOrder.SelectedIndex = 1 Then lstRes.ItemsSource = lst.OrderByDescending(Function(p) p.Downloads).ToList
    If cboSort.SelectedIndex = 2 And cboOrder.SelectedIndex = 0 Then lstRes.ItemsSource = lst.OrderBy(Function(p) p.Name).ToList
    If cboSort.SelectedIndex = 2 And cboOrder.SelectedIndex = 1 Then lstRes.ItemsSource = lst.OrderByDescending(Function(p) p.Name).ToList
    If cboSort.SelectedIndex = 3 And cboOrder.SelectedIndex = 0 Then lstRes.ItemsSource = lst.OrderBy(Function(p) p.Rate).ToList
    If cboSort.SelectedIndex = 3 And cboOrder.SelectedIndex = 1 Then lstRes.ItemsSource = lst.OrderByDescending(Function(p) p.Rate).ToList
    e.Handled = True
  End Sub

  Private Sub img_MouseDown(sender As Image, e As MouseButtonEventArgs)
    Dim args As bgwImageArgs = sender.Tag
    Dim wndImageViewer As New imageViewer
    wndImageViewer.Owner = Me
    wndImageViewer.Tag = args.Filename
    wndImageViewer.img.Width = args.w
    wndImageViewer.img.Height = args.h
    wndImageViewer.ShowDialog()
    e.Handled = True
  End Sub
End Class
