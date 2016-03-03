Public Class DllEditor
  Public Class lvwItem
    Implements ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
    Private pId As UShort
    Public Property Id As UShort
      Get
        Return pId
      End Get
      Set(value As UShort)
        pId = value
      End Set
    End Property
    Private pContent As String
    Public Property Content As String
      Get
        Return pContent
      End Get
      Set(value As String)
        pContent = value
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Content"))
      End Set
    End Property
  End Class
  Dim gsFile As String
  Dim lvwItems As New ObjectModel.ObservableCollection(Of lvwItem)
  Dim alChanged As New List(Of UShort)

  Private Function EnumResourceNamesProc(ByVal hModule As IntPtr, ByVal lpszType As Integer, ByVal lpszName As IntPtr, ByVal lParam As Integer) As Boolean
    Dim hResInfo As IntPtr = FindResourceA(hModule, lpszName, lpszType)
    Dim hData As IntPtr = LockResource(LoadResource(hModule, hResInfo))
    Dim sz As Integer = SizeofResource(hModule, hResInfo)
    Dim y(sz - 1) As Byte
    Runtime.InteropServices.Marshal.Copy(hData, y, 0, sz)
    Dim p As Integer = 0, i As Integer = 0
    Do
      Dim l As Short = BitConverter.ToInt16(y, p)
      p += 2
      If l Then lvwItems.Add(New lvwItem With {.Id = (lpszName.ToInt32 - 1) * 16 + i, .Content = System.Text.Encoding.Unicode.GetString(y, p, l * 2)})
      p += 2 * l
      i += 1
    Loop While p < sz And i < 16
    Return True
  End Function
  Private Sub btnOpen_Click(sender As Object, e As RoutedEventArgs)
    Dim ofn As New Forms.OpenFileDialog
    ofn.InitialDirectory = gsHawkempirePath
    ofn.Filter = "帝国时代语言 DLL 文件|language*.dll"
    ofn.CheckFileExists = True
    ofn.ShowDialog()
    gsFile = ofn.FileName
    txbStatus.Foreground = New SolidColorBrush(Colors.Black)
    txbStatus.Text = "当前文件：" & gsFile
    lvwItems.Clear()
    Dim hM As IntPtr = LoadLibraryEx(gsFile, 0, 2)
    EnumResourceNames(hM, RT_STRING, AddressOf EnumResourceNamesProc, 0)
    FreeLibrary(hM)
    btnModify.IsEnabled = True
    btnAdd.IsEnabled = True
    btnDelete.IsEnabled = True
    btnSave.IsEnabled = True
  End Sub

  Private Sub DllEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    lvw1.ItemsSource = lvwItems
    lvw1.Items.SortDescriptions.Add(New ComponentModel.SortDescription("Id", ComponentModel.ListSortDirection.Ascending))
  End Sub

  Private Sub lvw1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    If lvw1.SelectedItems.Count Then
      txtId.Text = CType(lvw1.SelectedItem, lvwItem).Id
      txtContent.Text = CType(lvw1.SelectedItem, lvwItem).Content
    End If
  End Sub

  Private Sub btnModify_Click(sender As Object, e As RoutedEventArgs)
    Dim i As UShort = CUShort(txtId.Text)
    If lvwItems.Any(Function(p As lvwItem) p.Id = i) Then
      lvwItems.Single(Function(p As lvwItem) p.Id = i).Content = txtContent.Text
      If Not alChanged.Contains(Int(i / 16) + 1) Then alChanged.Add(Int(i / 16) + 1)
    Else
      MessageBox.Show("找不到 ID 为 " & txtId.Text & " 的字符串。")
    End If
  End Sub

  Private Sub txtId_PreviewLostKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
    Dim ret As UShort
    If IsNumeric(txtId.Text) Then
      If Not UShort.TryParse(txtId.Text, ret) Then
        MessageBox.Show("ID 超出允许范围。")
        e.Handled = True
      End If
    Else
      MessageBox.Show("序号必须为数值")
      e.Handled = True
    End If
  End Sub

  Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs)
    Dim i As UShort = CUShort(txtId.Text)
    If lvwItems.Any(Function(p As lvwItem) p.Id = i) Then
      MessageBox.Show("ID 为 " & txtId.Text & " 的字符串已存在。")
    Else
      lvwItems.Add(New lvwItem With {.Id = i, .Content = txtContent.Text})
      If Not alChanged.Contains(Int(i / 16) + 1) Then alChanged.Add(Int(i / 16) + 1)
    End If
  End Sub

  Private Sub btnDelete_Click(sender As Object, e As RoutedEventArgs)
    Dim i As UShort = CUShort(txtId.Text)
    If lvwItems.Any(Function(p As lvwItem) p.Id = i) Then
      lvwItems.Remove(lvwItems.Single(Function(p As lvwItem) p.Id = i))
      If Not alChanged.Contains(Int(i / 16) + 1) Then alChanged.Add(Int(i / 16) + 1)
    Else
      MessageBox.Show("找不到 ID 为 " & txtId.Text & " 的字符串。")
    End If
  End Sub

  Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
    Dim hUpd As IntPtr = BeginUpdateResource(gsFile, False)
    For Each ele In alChanged
      Dim ms As New IO.MemoryStream
      Dim bw As New IO.BinaryWriter(ms, System.Text.Encoding.Unicode)
      For i As UShort = (ele - 1) * 16 To ele * 16 - 1
        Dim tmpI = i
        If lvwItems.Any(Function(p As lvwItem) p.Id = tmpI) Then
          Dim t As String = lvwItems.Single(Function(p As lvwItem) p.Id = tmpI).Content
          bw.Write(CUShort(t.Length))
          bw.Write(t.ToCharArray)
        Else
          bw.Write(0US)
        End If
      Next
      UpdateResource(hUpd, RT_STRING, ele, 2052, ms.GetBuffer, CInt(ms.Length))
    Next
    EndUpdateResource(hUpd, False)
    alChanged.Clear()
    txbStatus.Foreground = New SolidColorBrush(Colors.Red)
    txbStatus.Text = "已保存修改至" & gsFile
  End Sub

  Private Sub btnHelp_Click(sender As Object, e As RoutedEventArgs)
    Dim sb As New System.Text.StringBuilder
    sb.AppendLine("使用步骤：")
    sb.AppendLine("点击【打开语言DLL文件】读入一个DLL文件到列表框中。点击列表框中的某一行，可以使其内容显示在右方文本框中。")
    sb.AppendLine("通过【修改、新增、删除】三个按钮操作。这三个按钮的操作对象均以文本框中数字及内容为准。")
    sb.AppendLine("【修改】按钮可修改现有条目内容。方法为：选择要修改的条目或在上方文本框中输入序号，在下方文本框中修改字符串内容，点击【修改】按钮。")
    sb.AppendLine("【新增】按钮可新增一个条目。方法为：在上方文本框输入条目序号，在下方文本框输入字符串内容，点击【新增】按钮。")
    sb.AppendLine("【删除】按钮可删除一个现有条目。方法为：选择要修改的条目或在上方文本框中输入序号，点击【删除】按钮。")
    sb.AppendLine("序号必须在0~65535之间。")
    sb.AppendLine("文件修改完成后，点击【保存文件】保存修改至原文件中。")
    MessageBox.Show(sb.ToString)
  End Sub

  Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
    If alChanged.Count Then
      If MessageBox.Show("文件已修改，确定放弃更改并退出吗？", "提示", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
        Me.Close()
      End If
    Else
      Me.Close()
    End If
  End Sub

  Private Sub DllEditor_Closed(sender As Object, e As EventArgs) Handles Me.Closed
    gwDllEditor = Nothing
  End Sub
End Class
