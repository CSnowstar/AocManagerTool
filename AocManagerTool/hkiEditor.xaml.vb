Public Class hkiEditor
#Region "Constants"
    ReadOnly gcaGroupNames() As String = {"单位指令", "游戏指令", "卷动指令", "村民建筑", "城镇中心", "船坞", "兵营", "靶场", "马厩", "攻城武器厂", "修道院", "市场", "战斗部队", "城堡", "磨坊"}
    ReadOnly gcaKeyNames()() As String = {New String() {"经济建筑", "军事建筑", "修理", String.Empty, String.Empty, "停止", "卸载", "组装", "拆装", "治疗", "转化", "驻军", "删除单位", "设置集结点", "强制攻击"},
                                          New String() {String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, "送出聊天信息", "加速", "减速", "下一个闲置村民1", "下一个闲置村民2", "转到最后一个信息提示处1", "转到最后一个信息提示处2", "转到选定物体", "前往城镇中心1", "前往城镇中心2", "前往市场", "倒序回顾聊天", "正序回顾聊天", "显示状态", "前往兵营", "前往靶场", "前往马厩", "前往攻城武器厂", "前往船坞", "前往修道院", "前往铁匠铺", "前往磨坊", "前往大学", "科技树", String.Empty, "游戏时间", "前往下一个闲置部队1", "前往下一个闲置部队2", "闪光", "前往城堡", "前往矿场", "前往伐木场", "军事小地图", "经济小地图", "普通小地图", String.Empty, String.Empty, "外交", String.Empty, "任务", "聊天框", "暂停", "储存游戏", "切换盟友色", "回上一页观看", "储存时代"},
                                          New String() {"向左", "向右", "向上", "向下", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty},
                                          New String() {"住房", "磨坊", "铁匠铺", "船坞", "兵营", "木墙", "市场", "石墙", "箭塔", "炮塔", "城门", String.Empty, "农田", "靶场", "马厩", "修道院", "城镇中心", "攻城武器厂", "大学", "奇观", "城堡", String.Empty, String.Empty, "伐木场", "矿场", "渔网", "哨所", "更多"},
                                          New String() {"村民", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, "警铃", "返回工作"},
                                          New String() {"渔船", "商船", "战舰", "炮舰", "喷火舰", "爆破舰", String.Empty, "运输船", "龙头战舰", "龟船"},
                                          New String() {"剑士", "枪兵", "雄鹰战士", "禁卫军"},
                                          New String() {"弩手", "掷矛兵", "骑射手", "火枪兵"},
                                          New String() {"轻骑兵", "骆驼兵", "骑士"},
                                          New String() {"冲车", "弩炮", "投石车", "火炮"},
                                          New String() {"僧侣", "传教士"},
                                          New String() {"贸易车队"},
                                          New String() {String.Empty, "方阵", "横列", "交错", "侧翼", "巡逻", "警戒", "跟随", "攻击", "防御", "坚守", "不还击"},
                                          New String() {"投石机", "特色兵", "爆破兵"},
                                          New String() {"复耕"}}
#End Region
    Dim k As hkiFile = New hkiFile
    Dim gsDefaultPath As String, gsFile As String
    Public Class lvwItem
        Implements ComponentModel.INotifyPropertyChanged
        Public Event PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
        Private pIndex As Integer
        Private pName As String
        Private pKey As String
        Public Property Index As Integer
            Get
                Return pIndex
            End Get
            Set(value As Integer)
                pIndex = value
            End Set
        End Property
        Public Property Name As String
            Get
                Return pName
            End Get
            Set(value As String)
                pName = value
            End Set
        End Property
        Public Property Key As String
            Get
                Return pKey
            End Get
            Set(value As String)
                pKey = value
                RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Key"))
            End Set
        End Property
    End Class
    Private Sub btnOpen_Click(sender As Object, e As RoutedEventArgs)
        Dim ofd As Forms.OpenFileDialog = New Forms.OpenFileDialog
        ofd.InitialDirectory = gsDefaultPath
        ofd.Filter = "AoC Hotkey Files (*.hki)|*.hki"
        ofd.ShowDialog()
        If Len(ofd.FileName) Then
            gsFile = ofd.FileName
            btnSave.IsEnabled = True
            btnSaveAs.IsEnabled = True
            Dim hModule As IntPtr = LoadLibrary("language.dll")
            Dim s As System.Text.StringBuilder = New System.Text.StringBuilder(255)
            Using br As IO.BinaryReader = New IO.BinaryReader(New IO.Compression.DeflateStream(New IO.FileStream(ofd.FileName, IO.FileMode.Open), IO.Compression.CompressionMode.Decompress))
                k.version = br.ReadSingle()
                k.numGroups = br.ReadInt32()
                ReDim k.Groups(k.numGroups - 1)
                For i As Integer = 0 To k.numGroups - 1
                    k.Groups(i).numHotkeys = br.ReadInt32()
                    ReDim k.Groups(i).Hotkeys(k.Groups(i).numHotkeys - 1)
                    For j As Integer = 0 To k.Groups(i).numHotkeys - 1
                        k.Groups(i).Hotkeys(j).key = br.ReadInt32()
                        k.Groups(i).Hotkeys(j).dllEntry = br.ReadInt32()
                        k.Groups(i).Hotkeys(j).ctrl = br.ReadByte()
                        k.Groups(i).Hotkeys(j).alt = br.ReadByte()
                        k.Groups(i).Hotkeys(j).shift = br.ReadByte()
                        k.Groups(i).Hotkeys(j).reserved = br.ReadByte()
                    Next
                Next
            End Using
            For i As Integer = 0 To k.Groups.Count - 1
                lst1.Items.Add(gcaGroupNames(i))
            Next
            FreeLibrary(hModule)
        End If
    End Sub

    Private Sub hkiEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        gsDefaultPath = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32).OpenSubKey("SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("EXE Path")
        cmbKey.Items.Add("退格键")
        cmbKey.Items.Add("Tab 键")
        cmbKey.Items.Add("回车键")
        cmbKey.Items.Add("空格键")
        cmbKey.Items.Add("PageUp 键")
        cmbKey.Items.Add("PageDown 键")
        cmbKey.Items.Add("End 键")
        cmbKey.Items.Add("Home 键")
        cmbKey.Items.Add("左方向键")
        cmbKey.Items.Add("上方向键")
        cmbKey.Items.Add("右方向键")
        cmbKey.Items.Add("下方向键")
        cmbKey.Items.Add("Insert 键")
        cmbKey.Items.Add("Delete 键")
        cmbKey.Items.Add("0")
        cmbKey.Items.Add("1")
        cmbKey.Items.Add("2")
        cmbKey.Items.Add("3")
        cmbKey.Items.Add("4")
        cmbKey.Items.Add("5")
        cmbKey.Items.Add("6")
        cmbKey.Items.Add("7")
        cmbKey.Items.Add("8")
        cmbKey.Items.Add("9")
        cmbKey.Items.Add("A")
        cmbKey.Items.Add("B")
        cmbKey.Items.Add("C")
        cmbKey.Items.Add("D")
        cmbKey.Items.Add("E")
        cmbKey.Items.Add("F")
        cmbKey.Items.Add("G")
        cmbKey.Items.Add("H")
        cmbKey.Items.Add("I")
        cmbKey.Items.Add("J")
        cmbKey.Items.Add("K")
        cmbKey.Items.Add("L")
        cmbKey.Items.Add("M")
        cmbKey.Items.Add("N")
        cmbKey.Items.Add("O")
        cmbKey.Items.Add("P")
        cmbKey.Items.Add("Q")
        cmbKey.Items.Add("R")
        cmbKey.Items.Add("S")
        cmbKey.Items.Add("T")
        cmbKey.Items.Add("U")
        cmbKey.Items.Add("V")
        cmbKey.Items.Add("W")
        cmbKey.Items.Add("X")
        cmbKey.Items.Add("Y")
        cmbKey.Items.Add("Z")
        cmbKey.Items.Add("小键盘数字 0 键")
        cmbKey.Items.Add("小键盘数字 1 键")
        cmbKey.Items.Add("小键盘数字 2 键")
        cmbKey.Items.Add("小键盘数字 3 键")
        cmbKey.Items.Add("小键盘数字 4 键")
        cmbKey.Items.Add("小键盘数字 5 键")
        cmbKey.Items.Add("小键盘数字 6 键")
        cmbKey.Items.Add("小键盘数字 7 键")
        cmbKey.Items.Add("小键盘数字 8 键")
        cmbKey.Items.Add("小键盘数字 9 键")
        cmbKey.Items.Add("小键盘 * 键")
        cmbKey.Items.Add("小键盘 + 键")
        cmbKey.Items.Add("小键盘 - 键")
        cmbKey.Items.Add("小键盘 . 键")
        cmbKey.Items.Add("小键盘 / 键")
        cmbKey.Items.Add("F1")
        cmbKey.Items.Add("F2")
        cmbKey.Items.Add("F3")
        cmbKey.Items.Add("F4")
        cmbKey.Items.Add("F5")
        cmbKey.Items.Add("F6")
        cmbKey.Items.Add("F7")
        cmbKey.Items.Add("F8")
        cmbKey.Items.Add("F9")
        cmbKey.Items.Add("F10")
        cmbKey.Items.Add("F11")
        cmbKey.Items.Add("F12")
        cmbKey.Items.Add("F13")
        cmbKey.Items.Add("F14")
        cmbKey.Items.Add("F15")
        cmbKey.Items.Add("F16")
        cmbKey.Items.Add("F17")
        cmbKey.Items.Add("F18")
        cmbKey.Items.Add("F19")
        cmbKey.Items.Add("F20")
        cmbKey.Items.Add("F21")
        cmbKey.Items.Add("F22")
        cmbKey.Items.Add("F23")
        cmbKey.Items.Add("F24")
        cmbKey.Items.Add("; (:) 键")
        cmbKey.Items.Add("= (+) 键")
        cmbKey.Items.Add(", (<) 键")
        cmbKey.Items.Add("- (_) 键")
        cmbKey.Items.Add(". (>) 键")
        cmbKey.Items.Add("/ (?) 键")
        cmbKey.Items.Add("` (~) 键")
        cmbKey.Items.Add("[ ({) 键")
        cmbKey.Items.Add("\ (|) 键")
        cmbKey.Items.Add("] (}) 键")
        cmbKey.Items.Add("' ("") 键")
        cmbKey.Items.Add("其他按键 2")
        cmbKey.Items.Add("其他按键 1")
        cmbKey.Items.Add("鼠标中键")
        cmbKey.Items.Add("鼠标滚轮向下")
        cmbKey.Items.Add("鼠标滚轮向上")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        PerformSave()
    End Sub

    Private Sub btnSaveAs_Click(sender As Object, e As RoutedEventArgs)
        Dim sfd As Forms.SaveFileDialog = New Forms.SaveFileDialog
        sfd.FileName = My.Computer.FileSystem.GetName(gsFile)
        sfd.Filter = "AoC Hotkey Files (*.hki)|*.hki"
        sfd.ShowDialog()
        If Len(sfd.FileName) Then
            gsFile = sfd.FileName
            PerformSave()
        End If
    End Sub

    Private Sub PerformSave()
        Dim ms As IO.MemoryStream = New IO.MemoryStream()
        Using bw As IO.BinaryWriter = New IO.BinaryWriter(ms), ds As IO.Compression.DeflateStream = New IO.Compression.DeflateStream(New IO.FileStream(gsFile, IO.FileMode.Create, IO.FileAccess.Write), IO.Compression.CompressionMode.Compress)
            bw.Write(k.version)
            bw.Write(k.numGroups)
            For Each grp As hkiFile.structGroup In k.Groups
                bw.Write(grp.numHotkeys)
                For Each htk As hkiFile.structGroup.Hotkey In grp.Hotkeys
                    bw.Write(htk.key)
                    bw.Write(htk.dllEntry)
                    bw.Write(htk.ctrl)
                    bw.Write(htk.alt)
                    bw.Write(htk.shift)
                    bw.Write(htk.reserved)
                Next
            Next
            Using ms1 As IO.MemoryStream = New IO.MemoryStream(ms.GetBuffer, 0, 2192)
                ms1.CopyTo(ds)
            End Using
        End Using
    End Sub

    Private Sub lst1_SelectionChanged(sender As System.Windows.Controls.ListBox, e As SelectionChangedEventArgs)
        If sender.SelectedIndex > -1 Then
            Dim lvwItems As New List(Of lvwItem)
            For i As Integer = 0 To k.Groups(sender.SelectedIndex).Hotkeys.Count - 1
                If k.Groups(sender.SelectedIndex).Hotkeys(i).dllEntry >= 0 Then
                    If k.Groups(sender.SelectedIndex).Hotkeys(i).key > 0 Then
                        lvwItems.Add(New lvwItem With {.Index = i, .Name = gcaKeyNames(sender.SelectedIndex)(i), .Key = IIf(k.Groups(sender.SelectedIndex).Hotkeys(i).ctrl, "Ctrl + ", String.Empty) & IIf(k.Groups(sender.SelectedIndex).Hotkeys(i).shift, "Shift + ", String.Empty) & IIf(k.Groups(sender.SelectedIndex).Hotkeys(i).alt, "Alt + ", String.Empty) & cmbKey.Items(KeycodeToComboboxindex(k.Groups(sender.SelectedIndex).Hotkeys(i).key))})
                    Else
                        lvwItems.Add(New lvwItem With {.Index = i, .Name = gcaKeyNames(sender.SelectedIndex)(i), .Key = "(无效键)"})
                    End If
                End If
            Next
            lvw1.ItemsSource = lvwItems
        End If
    End Sub
    Private Function KeyToComboboxindex(ByVal keycode As Key) As Integer
        Select Case keycode
            Case Key.Back To Key.Tab
                Return keycode - 2
            Case Key.Enter
                Return 6
            Case Key.Space To Key.Down
                Return keycode - 15
            Case Key.Insert
                Return 12
            Case Key.Delete
                Return 13
            Case Key.D0 To Key.Z
                Return keycode - 20
            Case Key.NumPad0 To Key.Add
                Return keycode - 24
            Case Key.Subtract To Key.F24
                Return keycode - 25
            Case Key.OemSemicolon To Key.OemTilde
                Return keycode - 51
            Case Key.OemOpenBrackets To Key.OemQuotes
                Return keycode - 54
            Case Else
                Return -1
        End Select
    End Function
    Private Function KeycodeToComboboxindex(ByVal keycode As Integer) As Integer
        Select Case keycode
            Case 8 To 9
                Return keycode - 8
            Case 13
                Return 2
            Case 32 To 40
                Return keycode - 29
            Case 45
                Return 12
            Case 46
                Return 13
            Case 48 To 57
                Return keycode - 34
            Case 65 To 90
                Return keycode - 41
            Case 96 To 107
                Return keycode - 46
            Case 109 To 135
                Return keycode - 47
            Case 186 To 192
                Return keycode - 97
            Case 219 To 222
                Return keycode - 123
            Case 251 To 255
                Return keycode - 151
            Case Else
                Return -1
        End Select
    End Function
    Private Function ComboboxindexToKeycode(ByVal index As Integer) As Integer
        Select Case index
            Case 0 To 1
                Return index + 8
            Case 2
                Return index + 11
            Case 3 To 11
                Return index + 29
            Case 12 To 13
                Return index + 33
            Case 14 To 23
                Return index + 34
            Case 24 To 49
                Return index + 41
            Case 50 To 61
                Return index + 46
            Case 62 To 88
                Return index + 47
            Case 89 To 95
                Return index + 97
            Case 96 To 99
                Return index + 123
            Case 100 To 104
                Return index + 151
            Case Else
                Return -1
        End Select
    End Function

    Private Sub lvw1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If sender.SelectedItems.Count Then
            Dim i As Integer = CType(lvw1.SelectedItem, lvwItem).Index
            chkCtrl.IsChecked = k.Groups(lst1.SelectedIndex).Hotkeys(i).ctrl
            chkShift.IsChecked = k.Groups(lst1.SelectedIndex).Hotkeys(i).shift
            chkAlt.IsChecked = k.Groups(lst1.SelectedIndex).Hotkeys(i).alt
            cmbKey.SelectedIndex = KeycodeToComboboxindex(k.Groups(lst1.SelectedIndex).Hotkeys(i).key)
        End If
    End Sub

    Private Sub cmbKey_KeyDown(sender As Object, e As Input.KeyEventArgs)
        If e.Key <> Key.LeftCtrl And e.Key <> Key.RightCtrl And e.Key <> Key.LeftShift And e.Key <> Key.RightShift And e.Key <> Key.LeftAlt And e.Key <> Key.RightAlt Then
            chkCtrl.IsChecked = My.Computer.Keyboard.CtrlKeyDown
            chkShift.IsChecked = My.Computer.Keyboard.ShiftKeyDown
            chkAlt.IsChecked = My.Computer.Keyboard.AltKeyDown
            cmbKey.SelectedIndex = KeyToComboboxindex(e.Key)
            If lvw1.SelectedItems.Count Then
                Dim i As Integer = CType(lvw1.SelectedItem, lvwItem).Index
                CType(lvw1.SelectedItem, lvwItem).Key = IIf(My.Computer.Keyboard.CtrlKeyDown, "Ctrl + ", String.Empty) & IIf(My.Computer.Keyboard.ShiftKeyDown, "Shift + ", String.Empty) & IIf(My.Computer.Keyboard.AltKeyDown, "Alt + ", String.Empty) & cmbKey.SelectedItem
                k.Groups(lst1.SelectedIndex).Hotkeys(i).ctrl = My.Computer.Keyboard.CtrlKeyDown
                k.Groups(lst1.SelectedIndex).Hotkeys(i).shift = My.Computer.Keyboard.ShiftKeyDown
                k.Groups(lst1.SelectedIndex).Hotkeys(i).alt = My.Computer.Keyboard.AltKeyDown
                k.Groups(lst1.SelectedIndex).Hotkeys(i).key = ComboboxindexToKeycode(cmbKey.SelectedIndex)
            End If
        End If
        e.Handled = True
    End Sub

    Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub

    Private Sub chkCtrl_Checked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).ctrl = True
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chkCtrl_Unchecked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).ctrl = False
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chkShift_Checked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).shift = True
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chkShift_Unchecked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).shift = False
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chkAlt_Checked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).alt = True
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chkAlt_Unchecked(sender As Object, e As RoutedEventArgs)
        k.Groups(lst1.SelectedIndex).Hotkeys(lvw1.SelectedIndex).alt = False
        CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
    End Sub

    Private Sub chks_Click(sender As Object, e As RoutedEventArgs)
        If lvw1.SelectedItems.Count Then
            CType(lvw1.SelectedItem, lvwItem).Key = IIf(chkCtrl.IsChecked, "Ctrl + ", String.Empty) & IIf(chkShift.IsChecked, "Shift + ", String.Empty) & IIf(chkAlt.IsChecked, "Alt + ", String.Empty) & cmbKey.SelectedItem
            k.Groups(lst1.SelectedIndex).Hotkeys(CType(lvw1.SelectedItem, lvwItem).Index).ctrl = chkCtrl.IsChecked
            k.Groups(lst1.SelectedIndex).Hotkeys(CType(lvw1.SelectedItem, lvwItem).Index).shift = chkShift.IsChecked
            k.Groups(lst1.SelectedIndex).Hotkeys(CType(lvw1.SelectedItem, lvwItem).Index).alt = chkAlt.IsChecked
            k.Groups(lst1.SelectedIndex).Hotkeys(CType(lvw1.SelectedItem, lvwItem).Index).key = ComboboxindexToKeycode(cmbKey.SelectedIndex)
        End If
    End Sub
End Class
