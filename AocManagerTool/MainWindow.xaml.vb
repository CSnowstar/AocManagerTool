Class MainWindow
  Inherits Window
  Dim cFd As New FlowDocument
  Dim cXmlRemote As XElement
  Dim cTxdzPath As String
  Public bgwUpdate, bgwFrontpage As ComponentModel.BackgroundWorker
  Dim reqFrontpage As Net.WebRequest

  Private Sub btnMinimize_Click(sender As Object, e As RoutedEventArgs)
    WindowState = WindowState.Minimized
    e.Handled = True
  End Sub

  Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
    Close()
    e.Handled = True
  End Sub

  Private Sub Window_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
    If e.LeftButton = MouseButtonState.Pressed Then DragMove()
    e.Handled = True
  End Sub

  Private Sub btnStartGameMore_Click(sender As Object, e As RoutedEventArgs)
    popStartGameMore.IsOpen = True
    e.Handled = True
  End Sub

  Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
    Dim q = From node In gxLocalRes.<res>, aMod In gtModsInfo
            Let index = gtModsInfo.IndexOf(aMod)
            Where CInt(node.<id>(0)) = aMod.Id
            Select node, index
    For Each result In q
      result.node.<order>.Value = result.index
    Next
  End Sub

  Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)

    bgwFrontpage = FindResource("bgwFrontpage")
    bgwUpdate = FindResource("bgwUpdate")
    gPing.SendAsync(gcsHawkaocUrl, DoAfterPing.Frontpage Or DoAfterPing.Update)

    If IO.File.Exists(IO.Path.Combine(gsManagerPath, "xml\localmods.xml")) Then
      Dim q = From aMod In gxLocalRes.<res>
              Where aMod.<type>.Value = "mod"
              Order By CInt(aMod.<order>(0))
              Select aMod
      For Each ele In q
        Dim modInfo As New ModInfo
        modInfo.Order = ele.<order>.Value
        modInfo.Id = ele.<id>.Value
        modInfo.Title = ele.<title>.Value
        modInfo.Exe = ele.<exe>.Value
        Dim TheXml = (From x In ele.<files>.<file>
                      Where x.Value.EndsWith(".xml")
                      Select x.Value).SingleOrDefault()
        If TheXml Is Nothing Then
          MessageBox.Show($"资源 {modInfo.Title} 缺少必要文件，请联系该资源的上传者重新规范上传完整文件。")
          ele.Remove()
        Else
          modInfo.Path = IO.Path.Combine(gsHawkempirePath, "Games", XElement.Load(gsHawkempirePath & TheXml).<path>.Value)
          If Not gtModsInfo.Any(Function(p) p.Id = modInfo.Id) Then gtModsInfo.Add(modInfo)
        End If
      Next
      lstModOrder.ItemsSource = gtModsInfo
      For Each ele In gtModsInfo
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = ele.Title
        btn.Tag = ele
        AddHandler btn.Click, AddressOf btnGameList_Click
        stpGameList.Children.Add(btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = ele.Title
        btn.Tag = ele
        AddHandler btn.Click, AddressOf btnSwitchToMod_Click
        wrpStarts.Children.Add(btn)
      Next
    End If

    If My.Computer.FileSystem.FileExists(IO.Path.Combine(gsManagerPath, "xml\config.xml")) Then
      txbSplash.Text = If(gxConfig.<splash>.Value = "1", "开启", "关闭")
      Dim LanguageCaptionMap As New Dictionary(Of String, String) From {
        {"chs", "中文"},
        {"en", "英文"},
        {"bg", "保加利亚语"},
        {"cs", "捷克语"},
        {"de", "德语"},
        {"el", "希腊语"},
        {"es", "西班牙语"},
        {"fr", "法语"},
        {"hu", "匈牙利语"},
        {"it", "意大利语"},
        {"ja", "日语"},
        {"ko", "朝鲜语"},
        {"pl", "波兰语"},
        {"pt", "葡萄牙语"},
        {"ru", "俄语"},
        {"sk", "斯洛伐克语"},
        {"tr", "土耳其语"}
      }
      If String.IsNullOrWhiteSpace(gxConfig.<language>.Value) Then gxConfig.<language>.Value = "chs"
      txbLanguage.Text = LanguageCaptionMap(gxConfig.<language>.Value)

      Select Case gxConfig.<aocversion>.Value
        Case ""
          gxConfig.<aocversion>.Value = "14"
        Case "c"
          txbCurrentVersion.Text = "当前游戏版本：1.0C"
          txbWhichExe.Text = "帝国时代Ⅱ 1.0C"
        Case "14"
          txbCurrentVersion.Text = "当前游戏版本：1.5"
          txbWhichExe.Text = "帝国时代Ⅱ 1.5"
        Case "fe"
          txbCurrentVersion.Text = "当前游戏版本：被遗忘的帝国"
          txbWhichExe.Text = "被遗忘的帝国"
        Case Else
          Dim CurrentVersion = (From aMod In gxLocalRes.<res>
                                Where aMod.<title>.Value = gxConfig.<aocversion>.Value
                                Select aMod.<title>.Value).First()
          txbCurrentVersion.Text = $"当前游戏版本：{CurrentVersion}"
          txbWhichExe.Text = CurrentVersion
      End Select

      If String.IsNullOrWhiteSpace(gxConfig.<holdfastpath>.Value) Then
        btnStartHoldfast.Content = "下载浩方对战平台"
      Else
        btnStartHoldfast.Content = "启动浩方对战平台"
      End If

      cTxdzPath = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Tencent\QQBattleZone\Install").GetValue("ExePath")
      If IsNothing(cTxdzPath) Then
        btnStartTxdz.Content = "下载腾讯对战平台"
      Else
        btnStartTxdz.Content = "启动腾讯对战平台"
      End If

      tglDat14.IsChecked = gxConfig.<dat_14>.Value = "1"
      tglDatC.IsChecked = gxConfig.<dat_c>.Value = "1"
      tglDatFe.IsChecked = gxConfig.<dat_fe>.Value = "1"
      tglDatA.IsChecked = gxConfig.<dat_a>.Value = "1"

      If Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("Extend Population") Then
        txbPopulationExtend.Text = "开启"
      Else
        txbPopulationExtend.Text = "关闭"
      End If

      If Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("Multiple Queue") Then
        txbMultiQueue.Text = "开启"
      Else
        txbMultiQueue.Text = "关闭"
      End If

      txbMusic.Text = If(gxConfig.<music>.Value = "1", "开启", "关闭")

      If gxConfig.<scenariosound>.Value = "en" Then
        txbScenarioSound.Text = "英文"
      ElseIf gxConfig.<scenariosound>.Value = "zh" Then
        txbScenarioSound.Text = "中文"
      End If

      Dim Taunts = From x In gxLocalRes.<res>
                   Where x.<type>.Value = "tau"
                   Select x
      For Each Taunt In Taunts
        Dim btn = New Button With {
          .Style = FindResource("btnSettingStyle"),
          .Content = Taunt.<title>.Value,
          .Tag = CInt(Taunt.<id>(0))}
        AddHandler btn.Click, AddressOf gwMain.btnTauntElse_Click
        gwMain.wrpTaunts.Children.Add(btn)
      Next
      If gxConfig.<taunt>.Value = "en" Then
        txbTaunt.Text = "英文"
      ElseIf gxConfig.<taunt>.Value = "zh" Then
        txbTaunt.Text = "中文"
      ElseIf String.IsNullOrWhiteSpace(gxConfig.<taunt>.Value) Then
      Else
        txbTaunt.Text = Taunts.SingleOrDefault(Function(x) CInt(x.<id>(0)) = CInt(gxConfig.<taunt>(0))).<title>.Value
      End If

      txbHoldfastPath.Text = gxConfig.<holdfastpath>.Value

      If IO.File.Exists(IO.Path.Combine(gsHawkempirePath, "age2_x1\miniupnpc.dll")) Then
        txbPortForwarding.Text = "开启"
      Else
        txbPortForwarding.Text = "关闭"
      End If
    End If

    If IO.File.Exists(IO.Path.Combine(gsManagerPath, "xml\version3.xml")) Then
      Dim UPVersion = (From x In gxVersion.<file>
                       Where x.<id>.Value = "14"
                       Select x.<version>.Value).First()
      txbUPVersion.Text = $"帝国时代主程序版本：{UPVersion}"
      Try
        Dim FilesToCheck As New List(Of String) From {"age2_x1.0c.exe", "age2_x1.5.exe", "age2_x2.exe"}
        Dim MD5s = From x In FilesToCheck
                   Select BitConverter.ToString(
                     (New Security.Cryptography.MD5CryptoServiceProvider).ComputeHash(
                     IO.File.ReadAllBytes(x))).
                     Replace("-", "").
                     ToLower()
        Dim q = From x In gxVersion.<file>
                Where New List(Of String) From {"1", "14", "3"}.Contains(x.<id>.Value)
                Select x.<md5>.Value
        If MD5s.Any(Function(x) Not q.Contains(x)) Then
          MessageBox.Show("检测到帝国时代主程序版本不是最新，版本不同将无法联机游戏。单击确定开始更新。")
          gPing.SendAsyncCancel()
          gPing.SendAsync(gcsHawkaocUrl, DoAfterPing.Verify)
        End If
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If

    Dim ret As Boolean, cntr As Integer = 0
    Do
      Dim dm As New DEVMODE
      dm.dmSize = Len(dm)
      ret = EnumDisplaySettings(0, cntr, dm)
      If Not ret Then Exit Do
      cntr += 1
      Dim s As String = $"{dm.dmPelsWidth}×{dm.dmPelsHeight}"
      If Not lstResolution.Items.Contains(s) Then lstResolution.Items.Add(s)
    Loop
    txtResolutionWidth.Text = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("Screen Width")
    txtResolutionHeight.Text = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("Screen Height")

    If Environment.OSVersion.Version >= New Version(6, 1) Then
      If FileVersionInfo.GetVersionInfo(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "dplayx.dll")).OriginalFilename <> "dplayx.dll" Then
        MessageBox.Show("检测到当前为 Windows 7 以上的操作系统，且未安装 DirectPlay 组件，帝国时代无法正常运行。点击““确定””以安装 DirectPlay。")
        Dim p As New Process
        p.StartInfo.FileName = "dism.exe"
        p.StartInfo.Arguments = "/online /enable-feature /featurename:directplay /all"
        p.Start()
        p.WaitForExit()
        If p.ExitCode Then
          MessageBox.Show("DirectPlay 未能成功安装。")
        Else
          MessageBox.Show("DirectPlay 已成功安装。")
        End If
      End If
      IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "dplayx.dll"))
      IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "age2_x1\dplayx.dll"))
      IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "age2_x1a\dplayx.dll"))
    End If

    e.Handled = True
  End Sub

  Private Sub bgwFrontpage_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs)
    Try
      e.Result = Net.WebRequest.Create(New Uri("http://www.hawkaoc.net/hawkclient/mainpage.xaml")).GetResponse().GetResponseStream()
    Catch ex As Net.WebException
      e.Cancel = True
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub bgwFrontpage_RunWorkerCompleted(sender As Object, e As ComponentModel.RunWorkerCompletedEventArgs)
    If Not e.Cancelled Then
      fdvMainPage.Document = Markup.XamlReader.Load(e.Result)
      Dim links As IEnumerable(Of Hyperlink) = GetLogicalChildren(fdvMainPage.Document).OfType(Of Hyperlink)()
      For i = 0 To links.Count - 1
        AddHandler links(i).Click, AddressOf Hyperlink_Click
      Next
    End If
  End Sub

  Private Sub bgwUpdate_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs)
    Try
      cXmlRemote = XElement.Load(
        Net.WebRequest.Create("http://www.hawkaoc.net/hawkclient/version3.xml").
        GetResponse().
        GetResponseStream())
    Catch ex As Net.WebException
      e.Cancel = True
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub bgwUpdate_RunWorkerCompleted(sender As Object, e As ComponentModel.RunWorkerCompletedEventArgs)
    If Not e.Cancelled Then
      Dim q1 = From Remote In cXmlRemote.<file>
               Where (Aggregate Local In gxVersion.<file> Into All(Local.<id>.Value <> Remote.<id>.Value))
               Select Remote
      Dim q2 = From Remote In cXmlRemote.<file>
               Join Local In gxVersion.<file> On CInt(Remote.<id>(0)) Equals CInt(Local.<id>(0))
               Where New Version(Remote.<version>.Value) > New Version(Local.<version>.Value)
               Select Remote
      Dim q = q1.Union(q2)
      If q.Any() Then
        Dim sb As New Text.StringBuilder
        sb.AppendLine("检测到以下更新：")
        For Each Update In q
          sb.AppendLine($"{Update.<name>.Value}；版本 {Update.<version>.Value}；大小 {Update.<size>.Value}；{Update.<desc>.Value}")
        Next
        sb.Append("单击""确定""开始更新，单击""取消""暂时不更新，可在""控制面板-版本与升级""页面手动更新。")
        If MessageBox.Show(sb.ToString(), "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) = MessageBoxResult.OK Then
          Dim p As New Process With {.StartInfo = New ProcessStartInfo(
            IO.Path.Combine(IO.Directory.GetCurrentDirectory, "amtUpdater.exe"),
            $"""{gsHawkempirePath}""")}
          p.Start()
          gApp.Shutdown()
        End If
      Else
        If Not gbRunUpdateOnLoad Then MessageBox.Show("当前版本已是最新")
      End If
    End If
  End Sub

  Private Sub btnSplashOn_Click(sender As Object, e As RoutedEventArgs)
    txbSplash.Text = "开启"
    gxConfig.<splash>.Value = "1"
    e.Handled = True
  End Sub

  Private Sub btnSplashOff_Click(sender As Object, e As RoutedEventArgs)
    txbSplash.Text = "关闭"
    gxConfig.<splash>.Value = "0"
    e.Handled = True
  End Sub

  Private Sub btnStartGame_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim ConfigVersionMap As New Dictionary(Of String, String) From {
        {"c", "age2_x1.0c.exe"},
        {"14", "age2_x1.5.exe"},
        {"fe", "age2_x2.exe"}
      }
      Dim FileMd5 As String = BitConverter.ToString(
        (New Security.Cryptography.MD5CryptoServiceProvider).ComputeHash(
        IO.File.ReadAllBytes(
        IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe")))).
        Replace("-", "").
        ToLower()
      For Each map In ConfigVersionMap
        Dim CorrectMd5 = (From x In gxVersion.<file>
                          Where x.<name>.Value = map.Value
                          Select x.<md5>.Value).SingleOrDefault()
        If gxConfig.<aocversion>.Value = map.Key And CorrectMd5 = FileMd5 Then
          IO.File.Copy(map.Value, IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"), True)
        End If
      Next
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    gpGameProc.StartInfo = New ProcessStartInfo("age2_x1.exe", If(txbSplash.Text = "关闭", "nostartup", ""))
    gpGameProc.StartInfo.WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1")
    gpGameProc.Start()
    e.Handled = True
  End Sub

  Private Sub btnLanguageChs_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\zh\language.dll"), IO.Path.Combine(gsHawkempirePath, "language.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\zh\language_x1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\zh\language_x1_p1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1_p1.dll"), True)
      txbLanguage.Text = "中文"
      gxConfig.<language>.Value = "chs"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnLanguageEn_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\en\language.dll"), IO.Path.Combine(gsHawkempirePath, "language.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\en\language_x1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\en\language_x1_p1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1_p1.dll"), True)
      txbLanguage.Text = "英文"
      gxConfig.<language>.Value = "en"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub lstResolution_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    txtResolutionWidth.Text = lstResolution.SelectedItem.Split("×")(0)
    txtResolutionHeight.Text = lstResolution.SelectedItem.Split("×")(1)
    If Integer.Parse(txtResolutionWidth.Text) < 800 Then MessageBox.Show("分辨率宽度小于 800 可能会使显示不正常，或者游戏报错。")
    If Integer.Parse(txtResolutionHeight.Text) < 600 Then MessageBox.Show("分辨率高度小于 600 可能会使显示不正常，或者游戏报错。")
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Width", Integer.Parse(txtResolutionWidth.Text), Microsoft.Win32.RegistryValueKind.DWord)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Height", Integer.Parse(txtResolutionHeight.Text), Microsoft.Win32.RegistryValueKind.DWord)
    e.Handled = True
  End Sub

  Private Sub txtResolutionWidth_LostFocus(sender As Object, e As RoutedEventArgs)
    If IsNumeric(txtResolutionWidth.Text) Then
      If Integer.Parse(txtResolutionWidth.Text) > 0 Then
        If Integer.Parse(txtResolutionWidth.Text) < 800 Then MessageBox.Show("分辨率宽度小于 800 可能会使显示不正常，或者游戏报错。")
        Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Width", Integer.Parse(txtResolutionWidth.Text), Microsoft.Win32.RegistryValueKind.DWord)
      Else
        MessageBox.Show("数字范围无效。")
      End If
    Else
      MessageBox.Show("请输入数字。")
    End If
    e.Handled = True
  End Sub

  Private Sub txtResolutionHeight_LostFocus(sender As Object, e As RoutedEventArgs)
    If IsNumeric(txtResolutionHeight.Text) Then
      If Integer.Parse(txtResolutionHeight.Text) > 0 Then
        If Integer.Parse(txtResolutionHeight.Text) < 600 Then MessageBox.Show("分辨率高度小于 600 可能会使显示不正常，或者游戏报错。")
        Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Height", Integer.Parse(txtResolutionHeight.Text), Microsoft.Win32.RegistryValueKind.DWord)
      Else
        MessageBox.Show("数字范围无效。")
      End If
    Else
      MessageBox.Show("请输入数字。")
    End If
    e.Handled = True
  End Sub

  Private Sub btnFullScreen_Click(sender As Object, e As RoutedEventArgs)
    txtResolutionWidth.Text = Forms.SystemInformation.VirtualScreen.Width
    txtResolutionHeight.Text = Forms.SystemInformation.VirtualScreen.Height
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Width", Integer.Parse(txtResolutionWidth.Text), Microsoft.Win32.RegistryValueKind.DWord)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Height", Integer.Parse(txtResolutionHeight.Text), Microsoft.Win32.RegistryValueKind.DWord)
    e.Handled = True
  End Sub

  Private Sub btnWindowedFullScreen_Click(sender As Object, e As RoutedEventArgs)
    txtResolutionWidth.Text = Forms.SystemInformation.WorkingArea.Width - 2 * SystemParameters.FixedFrameVerticalBorderWidth
    txtResolutionHeight.Text = Forms.SystemInformation.WorkingArea.Height - SystemParameters.WindowCaptionHeight - 2 * SystemParameters.FixedFrameHorizontalBorderHeight
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Width", Integer.Parse(txtResolutionWidth.Text), Microsoft.Win32.RegistryValueKind.DWord)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Screen Height", Integer.Parse(txtResolutionHeight.Text), Microsoft.Win32.RegistryValueKind.DWord)
    e.Handled = True
  End Sub

  Private Sub tglDat14_Checked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\allshown\empires2_x1_p1_age2x1c.dat"), IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_14>.Value = "1"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDat14_Unchecked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\original\empires2_x1_p1.dat"), IO.Path.Combine(gsHawkempirePath, "Games\the conquerors 1.4\data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_14>.Value = "0"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatC_Checked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\allshown\empires2_x1_p1_age2x1c.dat"), IO.Path.Combine(gsHawkempirePath, "data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_c>.Value = "1"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatC_Unchecked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\original\empires2_x1_p1.dat"), IO.Path.Combine(gsHawkempirePath, "data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_c>.Value = "0"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatFe_Checked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\allshown\empires2_x1_p1_age2x2.dat"), IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_fe>.Value = "1"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatFe_Unchecked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\original\empires2_x1_p1_fe.dat"), IO.Path.Combine(gsHawkempirePath, "Games\forgotten empires\data\empires2_x1_p1.dat"), True)
      gxConfig.<dat_fe>.Value = "0"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatA_Checked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\allshown\empires2_x1_age2x1a.dat"), IO.Path.Combine(gsHawkempirePath, "data\empires2_x1.dat"), True)
      gxConfig.<dat_a>.Value = "1"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub tglDatA_Unchecked(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dat\original\empires2_x1.dat"), IO.Path.Combine(gsHawkempirePath, "data\empires2_x1.dat"), True)
      gxConfig.<dat_a>.Value = "0"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnPopulationExtendOn_Click(sender As Object, e As RoutedEventArgs)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Extend Population", 1, Microsoft.Win32.RegistryValueKind.DWord)
    txbPopulationExtend.Text = "开启"
    e.Handled = True
  End Sub

  Private Sub btnPopulationExtendOff_Click(sender As Object, e As RoutedEventArgs)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Extend Population", 0, Microsoft.Win32.RegistryValueKind.DWord)
    txbPopulationExtend.Text = "关闭"
    e.Handled = True
  End Sub

  Private Sub btnMultiQueueOn_Click(sender As Object, e As RoutedEventArgs)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Multiple Queue", 1, Microsoft.Win32.RegistryValueKind.DWord)
    txbMultiQueue.Text = "开启"
    e.Handled = True
  End Sub

  Private Sub btnMultiQueueOff_Click(sender As Object, e As RoutedEventArgs)
    Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").SetValue("Multiple Queue", 0, Microsoft.Win32.RegistryValueKind.DWord)
    txbMultiQueue.Text = "关闭"
    e.Handled = True
  End Sub

  Private Sub btnHotkeyC_Click(sender As Object, e As RoutedEventArgs)
    Try
      If MessageBox.Show("本按钮会将所有玩家的快捷键设置为 C 版默认键位，确认继续？", String.Empty, MessageBoxButton.OKCancel, MessageBoxImage.Question) = Forms.DialogResult.OK Then IO.Directory.GetFiles(gsHawkempirePath).ToList.FindAll(Function(p) IO.Path.GetExtension(p) = ".hki").ForEach(Sub(p) IO.File.Copy(IO.Path.Combine(gsManagerPath, "hki\c.hki"), p, True))
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnHotkeyFe_Click(sender As Object, e As RoutedEventArgs)
    Try
      If MessageBox.Show("本按钮会将所有玩家的快捷键设置为 被遗忘的帝国 默认键位，确认继续？", String.Empty, MessageBoxButton.OKCancel, MessageBoxImage.Question) = Forms.DialogResult.OK Then IO.Directory.GetFiles(gsHawkempirePath).ToList.FindAll(Function(p) IO.Path.GetExtension(p) = ".hki").ForEach(Sub(p) IO.File.Copy(IO.Path.Combine(gsManagerPath, "hki\fe.hki"), p, True))
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnMusicOn_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "sound\music.m3u"), IO.Path.Combine(gsHawkempirePath, "sound\music.m3u"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "sound\music.m3u"), IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\sound\music.m3u"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "sound\music.m3u"), IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\sound\music.m3u"), True)
      txbMusic.Text = "开启"
      gxConfig.<music>.Value = "1"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnMusicOff_Click(sender As Object, e As RoutedEventArgs)
    Try
      If IO.File.Exists(IO.Path.Combine(gsHawkempirePath, "sound\music.m3u")) Then IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "sound\music.m3u"))
      If IO.File.Exists(IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\sound\music.m3u")) Then IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\sound\music.m3u"))
      If IO.File.Exists(IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\sound\music.m3u")) Then IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\sound\music.m3u"))
      txbMusic.Text = "关闭"
      gxConfig.<music>.Value = "0"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnScenarioSoundEn_Click(sender As Object, e As RoutedEventArgs)
    Try
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "sound\campaign\en"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "sound\campaign", IO.Path.GetFileName(ele)), True)
      Next
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "sound\scenario\en"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "sound\scenario", IO.Path.GetFileName(ele)), True)
      Next
      txbScenarioSound.Text = "英文"
      gxConfig.<scenariosound>.Value = "en"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnScenarioSoundZh_Click(sender As Object, e As RoutedEventArgs)
    Try
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "sound\campaign\zh"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "sound\campaign", IO.Path.GetFileName(ele)), True)
      Next
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "sound\scenario\zh"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "sound\scenario", IO.Path.GetFileName(ele)), True)
      Next
      txbScenarioSound.Text = "中文"
      gxConfig.<scenariosound>.Value = "zh"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnTauntEn_Click(sender As Object, e As RoutedEventArgs)
    Try
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "taunt\en"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "taunt", IO.Path.GetFileName(ele)), True)
      Next
      txbTaunt.Text = "英文"
      gxConfig.<taunt>.Value = "en"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnTauntZh_Click(sender As Object, e As RoutedEventArgs)
    Try
      For Each ele In IO.Directory.GetFiles(IO.Path.Combine(gsManagerPath, "taunt\zh"))
        IO.File.Copy(ele, IO.Path.Combine(gsHawkempirePath, "taunt", IO.Path.GetFileName(ele)), True)
      Next
      txbTaunt.Text = "中文"
      gxConfig.<taunt>.Value = "zh"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Public Sub btnTauntElse_Click(sender As Button, e As RoutedEventArgs)
    Dim Id As Integer = sender.Tag
    Dim Text = (From x In gxLocalRes.<res>
                Where CInt(x.<id>(0)) = Id
                Select x.<title>.Value).SingleOrDefault()
    Dim Files = (From x In gxLocalRes.<res>
                 Where CInt(x.<id>(0)) = Id
                 Select x).SingleOrDefault().<files>(0).<file>
    Try
      For Each x In Files
        IO.File.Copy(gsHawkempirePath & x.Value, IO.Path.Combine(gsHawkempirePath, "taunt", IO.Path.GetFileName(x.Value)), True)
      Next
      txbTaunt.Text = Text
      gxConfig.<taunt>.Value = Id
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAokts_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(
        IO.Path.Combine(gsManagerPath, "tools\aokts\aoktschs.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\aokts")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAokts14_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(
        IO.Path.Combine(gsManagerPath, "tools\tsup\tschs.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\tsup")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnCampaignManager_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(
        IO.Path.Combine(gsManagerPath, "tools\cpnman\cpnman.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\cpnman")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnIdEditor_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(gwIdEditor) Then
      gwIdEditor = New ideditor
      gwIdEditor.Show()
    Else
      gwIdEditor.Activate()
    End If
  End Sub

  Private Sub btnScenarioTranslator_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(gwSceTrans) Then
      gwSceTrans = New sceTrans
      gwSceTrans.Show()
    Else
      gwSceTrans.Activate()
    End If
  End Sub

  Private Sub btnAiEditor_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(
        IO.Path.Combine(gsManagerPath, "tools\ai editor\aieditor.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\ai editor")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAiScriptBuilder_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(
        IO.Path.Combine(gsManagerPath, "tools\ai script builder\aokaisb.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\ai script builder")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnScriptEd_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\scripted\scripted.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\scripted")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnSetupAi_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\setupai\setupai.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\setupai")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnRmsEditor_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\rmsedit\rmsedit.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\rmsedit")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnTurtlePack_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\turtle pack\turtle pack.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\turtle pack")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAGE_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\age\高级帝国数据编辑器3.8.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\age")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAGEen_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\age\advancedgenieeditor3chs.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\age")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnDllEditor_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(gwDllEditor) Then
      gwDllEditor = New DllEditor
      gwDllEditor.Show()
    Else
      gwDllEditor.Activate()
    End If
  End Sub

  Private Sub btnRecExplorer_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\recorded game explorer\recordedgameexplorer.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\recorded game explorer")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnAocEditor_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\aoceditor\aoceditor1.5.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools\aoceditor")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnSwitchTo14_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      If IO.File.Exists("age2_x1.5.exe") Then
        Try
          IO.File.Copy("age2_x1.5.exe", IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"), True)
          txbCurrentVersion.Text = "当前游戏版本：1.5"
          txbWhichExe.Text = "帝国时代Ⅱ 1.5"
          gxConfig.<aocversion>.Value = "14"
        Catch ex As IO.IOException
          MessageBox.Show(ex.Message)
        End Try
      Else
        MessageBox.Show("帝国时代 1.5 版本主程序文件不存在，无法切换。")
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnSwitchToC_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      If IO.File.Exists("age2_x1.0c.exe") Then
        Try
          IO.File.Copy("age2_x1.0c.exe", IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"), True)
          txbCurrentVersion.Text = "当前游戏版本：1.0C"
          txbWhichExe.Text = "帝国时代Ⅱ 1.0C"
          gxConfig.<aocversion>.Value = "c"
        Catch ex As IO.IOException
          MessageBox.Show(ex.Message)
        End Try
      Else
        MessageBox.Show("帝国时代 1.0C 版本主程序文件不存在，无法切换。")
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnSwitchToFe_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      If IO.File.Exists("age2_x2.exe") Then
        Try
          IO.File.Copy("age2_x2.exe", IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"), True)
          txbCurrentVersion.Text = "当前游戏版本：被遗忘的帝国"
          txbWhichExe.Text = "被遗忘的帝国"
          gxConfig.<aocversion>.Value = "fe"
        Catch ex As IO.IOException
          MessageBox.Show(ex.Message)
        End Try
      Else
        MessageBox.Show("被遗忘的帝国 版本主程序文件不存在，无法切换。")
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnSwitchToA_Click(sender As Object, e As RoutedEventArgs)
    gpGameProc.StartInfo = New ProcessStartInfo(
      "age2_x1.exe",
      If(txbSplash.Text = "关闭", "nostartup", "")) With {
     .WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1a")}
    Try
      gpGameProc.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Public Sub btnSwitchToMod_Click(sender As Button, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      Try
        txbCurrentVersion.Text = "当前游戏版本：" & sender.Content
        txbWhichExe.Text = sender.Content
        IO.File.Copy(gsHawkempirePath & CType(sender.Tag, ModInfo).Exe, IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"), True)
        gxConfig.<aocversion>.Value = sender.Content
      Catch ex As IO.FileNotFoundException
        MessageBox.Show(ex.Message)
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If
    e.Handled = True
  End Sub

  Private Sub btnBrowseHoldfast_Click(sender As Object, e As RoutedEventArgs)
    Dim ofn As New Microsoft.Win32.OpenFileDialog With {
      .InitialDirectory = txbHoldfastPath.Text,
      .Filter = "GameClient.exe|GameClient.exe"}
    ofn.ShowDialog()
    If IO.Path.GetFileName(ofn.FileName) = "GameClient.exe" Then
      txbHoldfastPath.Text = ofn.FileName
      gxConfig.<holdfastpath>.Value = ofn.FileName
      btnStartHoldfast.Content = "启动浩方对战平台"
    End If
    e.Handled = True
  End Sub

  Private Sub btnStartHoldfast_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    If IO.File.Exists(gxConfig.<holdfastpath>.Value) Then
      Dim HoldfastIni = IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Holdfast\platform\HFAuthConfig\GameClient.ini")
      WritePrivateProfileString(
        "帝国时代",
        "UserExeFile",
        IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
        HoldfastIni)
      WritePrivateProfileString(
        "帝国时代",
        "UserParamData",
        If(gxConfig.<splash>.Value = "0", "nostartup", String.Empty),
        HoldfastIni)
      p.StartInfo = New ProcessStartInfo(txbHoldfastPath.Text)
      p.Start()
    Else
      p.StartInfo = New ProcessStartInfo("http://www.cga.com.cn/") With {
        .UseShellExecute = True}
      p.Start()
    End If
    e.Handled = True
  End Sub

  Private Sub btnInstallIpx_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\setupipx.exe")) With {
        .WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools")}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnSetupreg_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsHawkempirePath, "setupreg.exe")) With {
        .WorkingDirectory = gsHawkempirePath}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnPortForwardingOff_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Delete(IO.Path.Combine(gsHawkempirePath, "age2_x1\miniupnpc.dll"))
      txbPortForwarding.Text = "关闭"
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnPortForwardingOn_Click(sender As Object, e As RoutedEventArgs)
    If IO.File.Exists(IO.Path.Combine(gsManagerPath, "dll\miniupnpc.dll")) Then
      Try
        IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\miniupnpc.dll"), IO.Path.Combine(gsHawkempirePath, "age2_x1\miniupnpc.dll"), True)
        txbPortForwarding.Text = "开启"
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    Else
      MessageBox.Show($"所需文件 {IO.Path.Combine(gsManagerPath, "dll\miniupnpc.dll")} 不存在。")
    End If
    e.Handled = True
  End Sub

  Private Sub btnUpdate_Click(sender As Object, e As RoutedEventArgs)
    gbRunUpdateOnLoad = False
    gPing.SendAsyncCancel()
    gPing.SendAsync(gcsHawkaocUrl, DoAfterPing.Update)
    e.Handled = True
  End Sub

  Private Sub btnStart14_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      Try
        IO.File.Copy("age2_x1.5.exe",
                     IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
                     True)
        txbCurrentVersion.Text = "当前游戏版本：1.5"
        txbWhichExe.Text = "帝国时代Ⅱ 1.5"
        gxConfig.<aocversion>.Value = "14"
        gpGameProc.StartInfo = New ProcessStartInfo(
          "age2_x1.exe",
          If(txbSplash.Text = "关闭", "nostartup", String.Empty)) With {
          .WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1")}
        gpGameProc.Start()
      Catch ex As IO.FileNotFoundException
        MessageBox.Show(ex.Message)
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If
    e.Handled = True
  End Sub

  Private Sub btnStartC_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      Try
        IO.File.Copy("age2_x1.0c.exe",
                     IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
                     True)
        txbCurrentVersion.Text = "当前游戏版本：1.0C"
        txbWhichExe.Text = "帝国时代Ⅱ 1.0C"
        gxConfig.<aocversion>.Value = "c"
        gpGameProc.StartInfo = New ProcessStartInfo(
          "age2_x1.exe",
          If(txbSplash.Text = "关闭", "nostartup", String.Empty)) With {
          .WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1")}
        gpGameProc.Start()
      Catch ex As IO.FileNotFoundException
        MessageBox.Show(ex.Message)
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If
    e.Handled = True
  End Sub

  Private Sub btnStartFe_Click(sender As Object, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      Try
        IO.File.Copy("age2_x2.exe",
                     IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
                     True)
        txbCurrentVersion.Text = "当前游戏版本：被遗忘的帝国"
        txbWhichExe.Text = "被遗忘的帝国"
        gxConfig.<aocversion>.Value = "fe"
        gpGameProc.StartInfo = New ProcessStartInfo(
          "age2_x1.exe",
          If(txbSplash.Text = "关闭", "nostartup", String.Empty)) With {
          .WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1")}
        gpGameProc.Start()
      Catch ex As IO.FileNotFoundException
        MessageBox.Show(ex.Message)
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If
    e.Handled = True
  End Sub

  Public Sub btnGameList_Click(sender As Button, e As RoutedEventArgs)
    If IsAocStarted() Then
      MessageBox.Show("帝国时代程序已经启动，无法切换。请关闭帝国时代程序后重试。")
    Else
      Try
        IO.File.Copy(
          gsHawkempirePath & CType(sender.Tag, ModInfo).Exe,
          IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
          True)
        txbCurrentVersion.Text = "当前游戏版本：" & sender.Content
        txbWhichExe.Text = sender.Content
        gxConfig.<aocversion>.Value = sender.Content
        gpGameProc.StartInfo = New ProcessStartInfo(
          "age2_x1.exe",
          If(txbSplash.Text = "关闭", "nostartup", String.Empty)) With {
          .WorkingDirectory = IO.Path.Combine(gsHawkempirePath, "age2_x1")}
        gpGameProc.Start()
      Catch ex As IO.FileNotFoundException
        MessageBox.Show(ex.Message)
      Catch ex As IO.IOException
        MessageBox.Show(ex.Message)
      End Try
    End If
    e.Handled = True
  End Sub

  Private Sub btnUP13Ref_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\脚本编写参考_UP1.3.doc")) With {
        .UseShellExecute = True}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnUnitIdList_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(IO.Path.Combine(gsManagerPath, "tools\aokunit.xls")) With {
        .UseShellExecute = True}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub Hyperlink_Click(sender As Hyperlink, e As EventArgs)
    Try
      Dim p As New Process With {
        .StartInfo = New ProcessStartInfo(sender.Tag) With {
        .UseShellExecute = True}}
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub btnLanguageJa_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\ja\language.dll"), IO.Path.Combine(gsHawkempirePath, "language.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\ja\language_x1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\ja\language_x1_p1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1_p1.dll"), True)
      txbLanguage.Text = "日语"
      gxConfig.<language>.Value = "ja"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnLanguageEs_Click(sender As Object, e As RoutedEventArgs)
    Try
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\es\language.dll"), IO.Path.Combine(gsHawkempirePath, "language.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\es\language_x1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1.dll"), True)
      IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll\es\language_x1_p1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1_p1.dll"), True)
      txbLanguage.Text = "西班牙语"
      gxConfig.<language>.Value = "es"
    Catch ex As IO.FileNotFoundException
      MessageBox.Show(ex.Message)
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub lstLanguagesMore_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    Dim itm As ListBoxItem = lstLanguagesMore.SelectedItem
    Try
      If IO.File.Exists(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language.dll")) Then IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language.dll"), IO.Path.Combine(gsHawkempirePath, "language.dll"), True)
      If IO.File.Exists(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language_x1.dll")) Then IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language_x1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1.dll"), True)
      If IO.File.Exists(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language_x1_p1.dll")) Then IO.File.Copy(IO.Path.Combine(gsManagerPath, "dll", itm.Tag, "language_x1_p1.dll"), IO.Path.Combine(gsHawkempirePath, "language_x1_p1.dll"), True)
      txbLanguage.Text = itm.Content
      gxConfig.<language>.Value = itm.Tag
    Catch ex As IO.IOException
      MessageBox.Show(ex.Message)
    End Try
  End Sub

  Private Sub btnRecAnalyzer_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process
      p.StartInfo.WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools")
      p.StartInfo.FileName = IO.Path.Combine(gsManagerPath, "tools\aoerecanalyzer.exe")
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnRMSG_Click(sender As Object, e As RoutedEventArgs)
    Try
      Dim p As New Process
      p.StartInfo.WorkingDirectory = IO.Path.Combine(gsManagerPath, "tools")
      p.StartInfo.FileName = IO.Path.Combine(gsManagerPath, "tools\rmsg.exe")
      p.Start()
    Catch ex As ComponentModel.Win32Exception
      MessageBox.Show(ex.Message)
    End Try
    e.Handled = True
  End Sub

  Private Sub btnOpenDirCampaign_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "campaign")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\scenario")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\scenario")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "scenario")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirScenario_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "scenario")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\scenario")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\scenario")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "scenario")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirAI_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "ai")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\script.ai")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\script.ai")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "script.ai")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirSave_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "savegame")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\savegame")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\savegame")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "savegame")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirSound_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "sound\scenario")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\sound\scenario")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\sound\scenario")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "sound\scenario")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirData_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "data")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\data")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\data")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "data")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirRms_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "random")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\script.rm")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\script.rm")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "script.rm")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnOpenDirScreenshot_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    p.StartInfo.UseShellExecute = True
    Select Case gxConfig.<aocversion>.Value
      Case "c"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "screenshots")
      Case "14"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\the conquerors 1.4\screenshots")
      Case "fe"
        p.StartInfo.FileName = IO.Path.Combine(gsHawkempirePath, "games\forgotten empires\screenshots")
      Case Else
        p.StartInfo.FileName = IO.Directory.CreateDirectory(IO.Path.Combine(gtModsInfo.SingleOrDefault(Function(x) x.Title = gxConfig.<aocversion>.Value).Path, "screenshots")).FullName
    End Select
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnUserManager_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(gwUserMan) Then
      gwUserMan = New UserManager
      gwUserMan.Show()
    Else
      gwUserMan.Activate()
    End If
  End Sub

  Private Sub btnHkiEditor_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(gwHkiEditor) Then
      gwHkiEditor = New hkiEditor
      gwHkiEditor.Show()
    Else
      gwHkiEditor.Activate()
    End If
  End Sub

  Private Sub tbcMain_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    If e.OriginalSource Is sender Then
      e.Handled = True
      Dim item As TabItem = e.AddedItems(0)
      Dim index As Integer = tbcMain.Items.IndexOf(item)
      If index = 2 Then
        Try
          If (New Devices.Network).Ping(gcsMySqlServer) Then
            If IsNothing(gwWorkshop) Then
              gwWorkshop = New Workshop
              gwWorkshop.Show()
            Else
              gwWorkshop.WindowState = WindowState.Normal
              gwWorkshop.Activate()
            End If
          Else
            MessageBox.Show("无法连接翔鹰帝国创意工坊数据库，请稍后重试。")
          End If
        Catch ex As Net.NetworkInformation.PingException
          MessageBox.Show(ex.Message)
        End Try
      End If
    End If
  End Sub

  Private Sub imgAbout_MouseDown(sender As Object, e As MouseButtonEventArgs)
    Dim p As New Process With {
      .StartInfo = New ProcessStartInfo("http://www.hawkaoc.net/") With {
      .UseShellExecute = True}}
    p.Start()
    e.Handled = True
  End Sub

  Private Sub btnModOrderUp_Click(sender As Object, e As RoutedEventArgs)
    If lstModOrder.SelectedIndex >= 0 Then
      Dim index As Integer = lstModOrder.SelectedIndex
      If index > 0 Then
        gtModsInfo.Move(index, index - 1)
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        stpGameList.Children.RemoveAt(index + 3)
        stpGameList.Children.Insert(index + 2, btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        wrpStarts.Children.RemoveAt(index)
        wrpStarts.Children.Insert(index - 1, btn)
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnModOrderDown_Click(sender As Object, e As RoutedEventArgs)
    If lstModOrder.SelectedIndex >= 0 Then
      Dim index As Integer = lstModOrder.SelectedIndex
      If index < gtModsInfo.Count - 1 Then
        gtModsInfo.Move(index, index + 1)
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        stpGameList.Children.RemoveAt(index + 3)
        stpGameList.Children.Insert(index + 4, btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        wrpStarts.Children.RemoveAt(index)
        wrpStarts.Children.Insert(index + 1, btn)
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnModOrderTop_Click(sender As Object, e As RoutedEventArgs)
    If lstModOrder.SelectedIndex >= 0 Then
      Dim index As Integer = lstModOrder.SelectedIndex
      If index > 0 Then
        gtModsInfo.Move(index, 0)
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        stpGameList.Children.RemoveAt(index + 3)
        stpGameList.Children.Insert(3, btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        wrpStarts.Children.RemoveAt(index)
        wrpStarts.Children.Insert(0, btn)
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub btnModOrderBottom_Click(sender As Object, e As RoutedEventArgs)
    If lstModOrder.SelectedIndex >= 0 Then
      Dim index As Integer = lstModOrder.SelectedIndex
      Dim lastIndex As Integer = gtModsInfo.Count - 1
      If index < lastIndex Then
        gtModsInfo.Move(index, lastIndex)
        Dim btn As New Button
        btn.Style = FindResource("btnStartGameListStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        stpGameList.Children.RemoveAt(index + 3)
        stpGameList.Children.Insert(lastIndex + 3, btn)
        btn = New Button
        btn.Style = FindResource("btnSettingStyle")
        btn.Content = CType(wrpStarts.Children(index), Button).Content
        btn.Tag = CType(stpGameList.Children(index), Button).Tag
        AddHandler btn.Click, AddressOf btnGameList_Click
        wrpStarts.Children.RemoveAt(index)
        wrpStarts.Children.Insert(lastIndex, btn)
      End If
    End If
    e.Handled = True
  End Sub

  Private Sub hypAbout_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process With {
      .StartInfo = New ProcessStartInfo("http://121.42.152.168/website/") With {
      .UseShellExecute = True}}
    p.Start()
    Dim sb As New Text.StringBuilder
    sb.AppendLine("主程序 / yty")
    sb.AppendLine("数据库后台运维 / 雪星")
    sb.AppendLine("美术设计 / 无敌英雄")
    sb.AppendLine("资源库运维 / 孟雨亲王")
    sb.AppendLine("策划 / 源BAOGG、条顿武士")
    sb.AppendLine("监制 / qs、狂~劇情狂、天日")
    sb.AppendLine("壁纸设计 / adongct")
    sb.AppendLine("协力 / clm0081、cycbobby")
    MessageBox.Show(sb.ToString())
  End Sub

  Private Sub hypReconnect_Click(sender As Object, e As RoutedEventArgs)
    gbRunUpdateOnLoad = True
    gPing.SendAsyncCancel()
    gPing.SendAsync(gcsHawkaocUrl, DoAfterPing.Frontpage Or DoAfterPing.Update)
  End Sub

  Private Sub btnStartTxdz_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process
    If IO.File.Exists(cTxdzPath) Then
      WritePrivateProfileString(
        "gamePath",
        "Age2",
        IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
        IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dz\dzsetting.ini"))
      WritePrivateProfileString(
        "gamePath",
        "Age2C",
        IO.Path.Combine(gsHawkempirePath, "age2_x1\age2_x1.exe"),
        IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dz\dzsetting.ini"))
      Dim NoSplash = CInt(gxConfig.<splash>(0))
      WritePrivateProfileString(
          "gameParam",
          "Age2",
          If(NoSplash, "nostartup", String.Empty),
          IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dz\dzsetting.ini"))
      WritePrivateProfileString(
        "gameParam",
        "Age2C",
        If(NoSplash, "nostartup", String.Empty),
        IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dz\dzsetting.ini"))
      p.StartInfo.FileName = cTxdzPath
      p.Start()
    Else
      p.StartInfo.UseShellExecute = True
      If Environment.OSVersion.Version.Major = 10 AndAlso (MsgBox("检测到当前操作系统为 Windows 10，须下载腾讯对战平台 Windows 10 兼容版。" & vbCrLf & "点击““是””下载 Windows 10 兼容版；点击“”否“”下载默认版本。", MsgBoxStyle.YesNo) = MsgBoxResult.Yes) Then
        p.StartInfo.FileName = "http://clientweb.tba.qq.com/download/DuiZhanSetup_win10.exe"
      Else
        p.StartInfo.FileName = "http://dz.qq.com/"
      End If
      p.Start()
    End If
    e.Handled = True
  End Sub

  Private Sub hypChangelog_Click(sender As Object, e As RoutedEventArgs)
    Dim p As New Process With {
    .StartInfo = New ProcessStartInfo("http://www.hawkaoc.net/hawkclient/changelog.txt") With {
    .UseShellExecute = True}}
    p.Start()
    e.Handled = True
  End Sub

End Class
