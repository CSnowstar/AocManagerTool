Public Class ideditor
  Public Class lvwItem
    Implements ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
    Private pId As Integer
    Public Property Id As Integer
      Get
        Return pId
      End Get
      Set(value As Integer)
        pId = value
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Id"))
      End Set
    End Property
    Private pPlayer As Integer
    Public Property Player As Integer
      Get
        Return pPlayer
      End Get
      Set(value As Integer)
        pPlayer = value
      End Set
    End Property
    Private pLocation As Point
    Public Property Location As Point
      Get
        Return pLocation
      End Get
      Set(value As Point)
        pLocation = value
      End Set
    End Property
    Private pUnit As Integer
    Public Property Unit As Integer
      Get
        Return pUnit
      End Get
      Set(value As Integer)
        pUnit = value
      End Set
    End Property
    Private pAngle As Single
    Public Property Angle As Single
      Get
        Return pAngle
      End Get
      Set(value As Single)
        pAngle = value
      End Set
    End Property
    Private pFrame As Integer
    Public Property Frame As Integer
      Get
        Return pFrame
      End Get
      Set(value As Integer)
        pFrame = value
      End Set
    End Property
    Private pGarrison As Integer
    Public Property Garrison As Integer
      Get
        Return pGarrison
      End Get
      Set(value As Integer)
        pGarrison = value
      End Set
    End Property
    Private _pLink As Integer
    Public Property _Link As Integer
      Get
        Return _pLink
      End Get
      Set(value As Integer)
        _pLink = value
      End Set
    End Property
  End Class
  Dim sFilename As String
  Dim cScx As scxFile
  Dim lvwItems As New ObjectModel.ObservableCollection(Of lvwItem)
  WithEvents sfn As New Forms.SaveFileDialog With {.DefaultExt = "scx"}
  Private Sub btnOpen_Click(sender As Object, e As RoutedEventArgs)
    Dim ofn As New Forms.OpenFileDialog
    ofn.InitialDirectory = gsHawkempirePath
    ofn.Filter = "场景文件|*.scx"
    ofn.ShowDialog()
    sFilename = ofn.FileName
    If My.Computer.FileSystem.FileExists(sFilename) Then
      cScx = New scxFile(sFilename, Text.Encoding.GetEncoding("gb2312"))
      txbStatus.Text = "当前文件： " & sFilename
      lvwItems.Clear()
      For i = 0 To 8
        For Each ele In cScx.Units(i)
          lvwItems.Add(New lvwItem With {.Id = ele.Id, .Location = New Point(ele.PosX, ele.PosY), .Player = i, .Unit = ele.UnitId, .Angle = ele.Rotation, .Frame = ele.Frame, .Garrison = ele.Garrison, ._Link = cScx.Units(i).ToList.IndexOf(ele)})
        Next
      Next
    End If
  End Sub

  Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(cScx) Then
      txbStatus.Text = "请先打开一个场景文件。"
    Else
      'cScx.Save (sFilename)
      cScx = Nothing
      txbStatus.Text = sFilename & " 已保存"
    End If
  End Sub

  Private Sub btnSaveAs_Click(sender As Object, e As RoutedEventArgs)
    If IsNothing(cScx) Then
      txbStatus.Text = "请先打开一个场景文件。"
    Else
      sfn.InitialDirectory = gsHawkempirePath
      sfn.ShowDialog()
    End If
  End Sub

  Private Sub sfn_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles sfn.FileOk
    'cScx.Save(sfn.FileName)
    cScx = Nothing
    txbStatus.Text = sfn.FileName & " 已保存"
  End Sub

  Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
    Me.Close()
  End Sub

  Private Sub ideditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    lvw1.ItemsSource = lvwItems
  End Sub

  Private Sub mnuIdAsc_Click(sender As Object, e As RoutedEventArgs)
    lvw1.Items.SortDescriptions.Clear()
    lvw1.Items.SortDescriptions.Add(New ComponentModel.SortDescription("Id", ComponentModel.ListSortDirection.Ascending))
  End Sub

  Private Sub mnuIdDesc_Click(sender As Object, e As RoutedEventArgs)
    lvw1.Items.SortDescriptions.Clear()
    lvw1.Items.SortDescriptions.Add(New ComponentModel.SortDescription("Id", ComponentModel.ListSortDirection.Descending))
  End Sub

  Private Sub mnuIdFind_Click(sender As Object, e As RoutedEventArgs)
    Dim ret As Integer
    If Integer.TryParse(InputBox("请输入要搜索的 ID："), ret) Then
      If lvwItems.Any(Function(p As lvwItem) p.Id = ret) Then
        lvw1.SelectedItem = lvwItems.First(Function(p As lvwItem) p.Id = ret)
        lvw1.ScrollIntoView(lvw1.SelectedItem)
      Else
        MessageBox.Show("找不到 ID：" & ret)
      End If
    Else
      MessageBox.Show("请输入合法的 ID 格式。")
    End If
  End Sub

  Private Sub mnuPlayerAsc_Click(sender As Object, e As RoutedEventArgs)
    lvw1.Items.SortDescriptions.Clear()
    lvw1.Items.SortDescriptions.Add(New ComponentModel.SortDescription("Player", ComponentModel.ListSortDirection.Ascending))
  End Sub

  Private Sub mnuPlayerDesc_Click(sender As Object, e As RoutedEventArgs)
    lvw1.Items.SortDescriptions.Clear()
    lvw1.Items.SortDescriptions.Add(New ComponentModel.SortDescription("Player", ComponentModel.ListSortDirection.Descending))
  End Sub

  Private Sub lvw1_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
    Dim s As String = InputBox("请输入要修改为的 ID：")
    If s.Length = 0 Then Exit Sub
    Dim ret As Integer
    If Integer.TryParse(s, ret) Then
      Dim it As lvwItem = CType(lvw1.SelectedItem, lvwItem)
      it.Id = ret
      cScx.Units(it.Player)(it._Link).Id = ret
    Else
      MessageBox.Show("请输入合法的 ID 格式。")
    End If
  End Sub

  Private Sub btnHelp_Click(sender As Object, e As RoutedEventArgs)
    Dim sb As New System.Text.StringBuilder
    sb.AppendLine("右击 ""ID"" 列，可选择升序、降序排列，可搜索。")
    sb.AppendLine("右击 ""玩家编号"" 列，可选择升序、降序排列。")
    sb.AppendLine("双击某条目，即可编辑 ID 数值。")
    MessageBox.Show(sb.ToString)
  End Sub

  Private Sub ideditor_Closed(sender As Object, e As EventArgs) Handles Me.Closed
    gwIdEditor = Nothing
  End Sub
End Class
