Partial Public Class Application
  Inherits Windows.Application

  Dim bNewInstance As Boolean
  Dim mtx As Threading.Mutex

  Private Sub Application_DispatcherUnhandledException(sender As Object, e As Windows.Threading.DispatcherUnhandledExceptionEventArgs)
    Dim sb As New Text.StringBuilder
    sb.AppendLine("【错误信息】")
    sb.AppendLine(e.Exception.GetType.ToString)
    sb.AppendLine(e.Exception.Message)
    For Each f In New StackTrace(e.Exception, True).GetFrames
      If f.GetFileLineNumber > 0 Then
        sb.AppendLine(IO.Path.GetFileName(f.GetFileName))
        sb.AppendLine(f.GetMethod.Name)
        sb.AppendLine(f.GetFileLineNumber)
      End If
    Next
    MessageBox.Show(sb.ToString)
    e.Handled = True
  End Sub

  Private Sub Application_Startup(sender As Object, e As StartupEventArgs)
    gApp = Me
    gPing = FindResource("pinPing")
    mtx = New Threading.Mutex(True, "AocManagerTool", bNewInstance)
    If bNewInstance Then
      IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)
      IO.Directory.SetCurrentDirectory("e:\hawkempire\manager\exe")

      If IO.File.Exists("..\..\empires2.exe") Then
        gsManagerPath = IO.Directory.GetParent(IO.Directory.GetCurrentDirectory).FullName
        gsHawkempirePath = IO.Directory.GetParent(gsManagerPath).FullName

        If IO.File.Exists(IO.Path.Combine(gsManagerPath, "xml\localmods.xml")) Then
          Dim bNoException As Boolean = False
          Do
            Try
              gxLocalRes = XElement.Load(IO.Path.Combine(gsManagerPath, "xml\localmods.xml"))
              bNoException = True
            Catch ex As IO.IOException
              MessageBox.Show(ex.Message & "请关闭占用该文件的进程并单击确定。")
            End Try
          Loop Until bNoException
          Dim ResNecessaryElementsList As New List(Of String) From {"id", "type", "status"}
          Dim q = From res In gxLocalRes.<res>, element In ResNecessaryElementsList
                  Select res, element
          For Each x In q
            If Not x.res.Elements(x.element).Any() Then x.res.Remove()
          Next
        Else
          MessageBox.Show("找不到 HawkEmpire\Manager\xml\localmods.xml 配置文件，无法读取工坊已安装内容。")
          gxLocalRes = <localmods></localmods>
        End If

        If My.Computer.FileSystem.FileExists(IO.Path.Combine(gsManagerPath, "xml\config.xml")) Then
          Dim bNoException As Boolean = False
          Do
            Try
              gxConfig = XElement.Load(IO.Path.Combine(gsManagerPath, "xml\config.xml"))
              bNoException = True
            Catch ex As IO.IOException
              MessageBox.Show(ex.Message & "请关闭占用该文件的进程并单击确定。")
            End Try
          Loop Until bNoException
        Else
          MessageBox.Show("找不到 HawkEmpire\Manager\xml\config.xml 配置文件，无法读取现有游戏设置。")
          gxConfig = <config></config>
        End If
        Dim ConfigElements As New List(Of String) From {"aocversion", "music", "scenariosound", "taunt", "dat_14", "dat_c", "dat_fe", "dat_a", "splash", "language", "holdfastpath", "fixdp"}
        For Each x In ConfigElements
          If Not gxConfig.Elements(x).Any() Then gxConfig.Add(x)
        Next

        If IO.File.Exists(IO.Path.Combine(gsManagerPath, "xml\version3.xml")) Then
          Dim bNoException As Boolean = False
          Do
            Try
              gxVersion = XElement.Load(IO.Path.Combine(gsManagerPath, "xml\version3.xml"))
              bNoException = True
            Catch ex As IO.IOException
              MessageBox.Show(ex.Message & "请关闭占用该文件的进程并单击确定。")
            End Try
          Loop Until bNoException
        Else
          MessageBox.Show("找不到 HawkEmpire\Manager\xml\version3.xml 配置文件，无法读取程序已更新内容，程序将要全部自动更新。")
          gxVersion = <HawkEmpire></HawkEmpire>
        End If

        gwMain = New MainWindow
        gwMain.Show()
      Else
        MessageBox.Show("未侦测到帝国时代2。请将本程序置于 HawkEmpire\Manager\exe 目录下重新运行。")
        Shutdown()
      End If
    Else
      MessageBox.Show("帝国时代管家已经运行。")
      Shutdown()
    End If
  End Sub

  Private Sub Application_Exit(sender As Object, e As ExitEventArgs)
    Dim bNoException As Boolean = False
    Do
      Try
        gxLocalRes.Save(IO.Path.Combine(gsManagerPath, "xml\localmods.xml"))
        gxConfig.Save(IO.Path.Combine(gsManagerPath, "xml\config.xml"))
        gxVersion.Save(IO.Path.Combine(gsManagerPath, "xml\version3.xml"))
        mtx.Close()
        bNoException = True
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message & "请关闭占用该文件的进程并单击确定。")
      End Try
    Loop Until bNoException
  End Sub

  Private Sub gPing_PingComplete(sender As Net.NetworkInformation.Ping, e As Net.NetworkInformation.PingCompletedEventArgs)
    Dim DoWhat As DoAfterPing = e.UserState
    If IsNothing(e.Error) Then
      gbOnline = True
      If (DoWhat And DoAfterPing.Frontpage) = DoAfterPing.Frontpage Then gwMain.bgwFrontpage.RunWorkerAsync()
      If (DoWhat And DoAfterPing.Update) = DoAfterPing.Update Then gwMain.bgwUpdate.RunWorkerAsync()
      If (DoWhat And DoAfterPing.Verify) = DoAfterPing.Verify Then
        Dim wc = New Net.WebClient
        AddHandler wc.DownloadFileCompleted, AddressOf WebClient_DownloadFileCompleted
        wc.DownloadFileAsync(New Uri("http://www.hawkaoc.net/hawkclient/age2_x1.0c.exe", UriKind.Absolute), IO.Path.Combine(gsManagerPath, "exe\age2_x1.0c.exe"), "c")
        wc = New Net.WebClient
        AddHandler wc.DownloadFileCompleted, AddressOf WebClient_DownloadFileCompleted
        wc.DownloadFileAsync(New Uri("http://www.hawkaoc.net/hawkclient/age2_x1.4.exe", UriKind.Absolute), IO.Path.Combine(gsManagerPath, "exe\age2_x1.4.exe"), "4")
        wc = New Net.WebClient
        AddHandler wc.DownloadFileCompleted, AddressOf WebClient_DownloadFileCompleted
        wc.DownloadFileAsync(New Uri("http://www.hawkaoc.net/hawkclient/age2_x2.exe", UriKind.Absolute), IO.Path.Combine(gsManagerPath, "exe\age2_x2.exe"), "f")
      End If
    Else
      gbOnline = False
      gwMain.fdvMainPage.Document = gwMain.FindResource("fldOffline")
      MessageBox.Show("目前无法连接至翔鹰服务器，请稍后重试。")
    End If
  End Sub

  Private Sub WebClient_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
    If IsNothing(e.Error) Then
      Select Case e.UserState
        Case "c"
          MessageBox.Show("帝国时代 1.0C 主程序下载完成。")
        Case "4"
          MessageBox.Show("帝国时代 1.4 主程序下载完成。")
        Case "f"
          MessageBox.Show("被遗忘的帝国 主程序下载完成。")
      End Select
    Else
      MessageBox.Show(e.Error.InnerException.Message)
    End If
  End Sub
End Class
