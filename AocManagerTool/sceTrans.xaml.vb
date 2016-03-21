Public Class sceTrans

  Private Const FILTERSTRING = "场景文件|*.scx;*.scn;*.scx2;*.aoe2scenario"

  Public Structure ImportState
    Public State As eImportState
    Public Index As Integer
    Public SubIndex As Integer
  End Structure

  Public Enum eImportState
    I1
    P
    I2
    H
    V
    D
    Y
    S
    N
    TN
    TD
    TE
  End Enum

  Public Class tvwNode
    Implements ComponentModel.INotifyPropertyChanged

    Public Enum eType
      None = -1
      Instruction1
      PlayerParent
      Player
      Instruction2
      Hint
      Victory
      Defeat
      History
      Scout
      NumberParent
      Number
      Trigger
      TriggerName
      TriggerDesc
      TriggerChat
      TriggerDisplay
      TriggerChange
    End Enum

    Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged

    Private _Visibility As Visibility
    Public Property Visibility As Visibility
      Get
        Return _Visibility
      End Get
      Set(value As Visibility)
        _Visibility = value
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Visibility"))
      End Set
    End Property
    Public Property IsObjective As Boolean
    Public Property Display As String
    Private pType As eType
    Public Property Type As eType
      Get
        Return pType
      End Get
      Set(value As eType)
        pType = value
        Select Case value
          Case eType.Instruction1
            _Display = "场景指示"
          Case eType.Player
            _Display = "玩家 " & _Index + 1 & " 名称"
          Case eType.Instruction2
            _Display = "场景指南"
          Case eType.Hint
            _Display = "提示"
          Case eType.Victory
            _Display = "胜利"
          Case eType.Defeat
            _Display = "失败"
          Case eType.History
            _Display = "历史"
          Case eType.Scout
            _Display = "侦察"
          Case eType.Number
            _Display = "玩家 " & _Index + 1 & " 代号"
          Case eType.TriggerName
            _Display = "名称"
          Case eType.TriggerDesc
            _Display = "描述"
          Case eType.TriggerChat
            _Display = "效果 " & _SubIndex + 1 & "：送出聊天"
          Case eType.TriggerDisplay
            _Display = "效果 " & _SubIndex + 1 & "：显示指示"
          Case eType.TriggerChange
            _Display = "效果 " & _SubIndex + 1 & "：改变名称"
        End Select
      End Set
    End Property
    Private _Index As Integer = -1
    Public Property Index As Integer
      Get
        Return _Index
      End Get
      Set(value As Integer)
        _Index = value
        Select Case pType
          Case eType.Player
            _Display = "玩家 " & _Index + 1 & " 名称"
          Case eType.Number
            _Display = "玩家 " & _Index + 1 & " 代号"
        End Select
      End Set
    End Property
    Private _SubIndex As Integer = -1
    Public Property SubIndex As Integer
      Get
        Return _SubIndex
      End Get
      Set(value As Integer)
        _SubIndex = value
        Select Case pType
          Case eType.TriggerChat
            _Display = "效果 " & _SubIndex & "：送出聊天"
          Case eType.TriggerDisplay
            _Display = "效果 " & _SubIndex & "：显示指示"
          Case eType.TriggerChange
            _Display = "效果 " & _SubIndex & "：改变名称"
        End Select
      End Set
    End Property

    Private _Source As String
    Public Property Source As String
      Get
        Return _Source
      End Get
      Set(value As String)
        _Source = value
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Source"))
      End Set
    End Property

    Private _Dest As String = String.Empty
    Public Property Dest As String
      Get
        Return _Dest
      End Get
      Set(value As String)
        _Dest = value
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs("Dest"))
      End Set
    End Property

    Public Property Children As New ObjectModel.ObservableCollection(Of tvwNode)
  End Class

  Dim cScx As scxFile, sFilename As String
  Dim ofn As New Forms.OpenFileDialog
  Dim tvwNodes As New ObjectModel.ObservableCollection(Of tvwNode)

  Private Sub btnOpen_Click(sender As Object, e As RoutedEventArgs)
    mnuHide.IsChecked = False
    ofn.InitialDirectory = gsHawkempirePath
    ofn.Filter = FILTERSTRING
    ofn.ShowDialog()
    sFilename = ofn.FileName
    If IO.File.Exists(sFilename) Then
      tvwNodes.Clear()
      cScx = New scxFile(sFilename, Text.Encoding.GetEncodings(cboSrc.SelectedIndex).GetEncoding())
      txbStatus.Text = $"当前文件： {sFilename}"
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Instruction1,
                   .Source = cScx.Instruction})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.PlayerParent,
                   .Display = "玩家名称"})
      With tvwNodes.Last
        For i = 0 To cScx.Players.Count - 1
          .Children.Add(New tvwNode With {
                        .Type = tvwNode.eType.Player,
                        .Source = cScx.Players(i).Name,
                        .Index = i})
        Next
      End With
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Instruction2,
                   .Source = cScx.StringInfos(scxFile.InfoType.Instruction)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Hint,
                   .Source = cScx.StringInfos(scxFile.InfoType.Hints)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Victory,
                   .Source = cScx.StringInfos(scxFile.InfoType.Victory)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Defeat,
                   .Source = cScx.StringInfos(scxFile.InfoType.Defeat)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.History,
                   .Source = cScx.StringInfos(scxFile.InfoType.History)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.Scout,
                   .Source = cScx.StringInfos(scxFile.InfoType.Scouts)})
      tvwNodes.Add(New tvwNode With {
                   .Type = tvwNode.eType.NumberParent,
                   .Display = "玩家代号"})
      With tvwNodes.Last
        For i = 0 To 7
          .Children.Add(New tvwNode With {
                        .Type = tvwNode.eType.Number,
                        .Source = cScx.Misc(i).Name,
                        .Index = i})
        Next
      End With
      For i = 0 To cScx.Triggers.Count - 1
        tvwNodes.Add(New tvwNode With {
                     .Type = tvwNode.eType.Trigger,
                     .Display = $"触发 {i}",
                     .Index = i,
                     .IsObjective = cScx.Triggers(i).IsObjective})
        With tvwNodes.Last
          .Children.Add(New tvwNode With {
                        .Type = tvwNode.eType.TriggerName,
                        .Source = cScx.Triggers(i).Name,
                        .Index = i})
          .Children.Add(New tvwNode With {
                        .Type = tvwNode.eType.TriggerDesc,
                        .Source = cScx.Triggers(i).Description,
                        .Index = i})
          For j = 0 To cScx.Triggers(i).Effects.Count - 1
            Select Case cScx.Triggers(i).Effects(j).Type
              Case scxFile.EffectType.SendChat
                .Children.Add(New tvwNode With {
                              .Type = tvwNode.eType.TriggerChat,
                              .Source = cScx.Triggers(i).Effects(j).Text,
                              .Index = i,
                              .SubIndex = j})
              Case scxFile.EffectType.DisplayInstructions
                .Children.Add(New tvwNode With {
                              .Type = tvwNode.eType.TriggerDisplay,
                              .Source = cScx.Triggers(i).Effects(j).Text,
                              .Index = i,
                              .SubIndex = j})
              Case scxFile.EffectType.ChangeObjectName
                .Children.Add(New tvwNode With {
                              .Type = tvwNode.eType.TriggerChange,
                              .Source = cScx.Triggers(i).Effects(j).Text,
                              .Index = i,
                              .SubIndex = j})
            End Select
          Next
        End With
      Next
      btnSave.IsEnabled = True
      btnSaveAs.IsEnabled = True
      mnu.IsEnabled = True
    End If
  End Sub

  Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
    Dim enc As Text.Encoding = Text.Encoding.GetEncodings(cboDst.SelectedIndex).GetEncoding
    cScx.Transcode(enc)
    For Each ele In tvwNodes
      RecurseNode(ele)
    Next
    cScx.Save()
    cScx = Nothing
    txbStatus.Text = cScx.FileName & " 已保存"
  End Sub

  Private Sub RecurseNode(nod As tvwNode)
    Select Case nod.Type
      Case tvwNode.eType.Instruction1
        cScx.Instruction = nod.Dest
      Case tvwNode.eType.Player
        cScx.Players(nod.Index).Name = nod.Dest
      Case tvwNode.eType.Instruction2
        cScx.StringInfos(scxFile.InfoType.Instruction) = nod.Dest
      Case tvwNode.eType.Hint
        cScx.StringInfos(scxFile.InfoType.Hints) = nod.Dest
      Case tvwNode.eType.Victory
        cScx.StringInfos(scxFile.InfoType.Victory) = nod.Dest
      Case tvwNode.eType.Defeat
        cScx.StringInfos(scxFile.InfoType.Defeat) = nod.Dest
      Case tvwNode.eType.History
        cScx.StringInfos(scxFile.InfoType.History) = nod.Dest
      Case tvwNode.eType.Scout
        cScx.StringInfos(scxFile.InfoType.Scouts) = nod.Dest
      Case tvwNode.eType.Number
        cScx.Misc(nod.Index).Name = nod.Dest
      Case tvwNode.eType.TriggerName
        cScx.Triggers(nod.Index).Name = nod.Dest
      Case tvwNode.eType.TriggerDesc
        cScx.Triggers(nod.Index).Description = nod.Dest
      Case tvwNode.eType.TriggerChat, tvwNode.eType.TriggerDisplay, tvwNode.eType.TriggerChange
        cScx.Triggers(nod.Index).Effects(nod.SubIndex).Text = nod.Dest
    End Select
    For Each ele In nod.Children
      RecurseNode(ele)
    Next
  End Sub

  Private Sub btnSaveAs_Click(sender As Object, e As RoutedEventArgs)
    Dim sfn As New Microsoft.Win32.SaveFileDialog
    sfn.Filter = FILTERSTRING
    sfn.InitialDirectory = IO.Path.GetFullPath(cScx.FileName)
    If sfn.ShowDialog() Then
      Dim enc As Text.Encoding = Text.Encoding.GetEncodings(cboDst.SelectedIndex).GetEncoding
      cScx.Transcode(enc)
      For Each ele In tvwNodes
        RecurseNode(ele)
      Next
      cScx.SaveAs(sfn.FileName)
      cScx = Nothing
      txbStatus.Text = sfn.FileName & " 已保存"
    End If
  End Sub

  Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
    Close()
  End Sub

  Private Sub sceTrans_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    For Each ele In Text.Encoding.GetEncodings
      cboSrc.Items.Add(ele.DisplayName)
      cboDst.Items.Add(ele.DisplayName)
      If ele.CodePage = 936 Then
        cboSrc.SelectedIndex = cboSrc.Items.Count - 1
        cboDst.SelectedIndex = cboDst.Items.Count - 1
      End If
    Next
    AddHandler cboSrc.SelectionChanged, AddressOf cboSrc_SelectionChanged
    tvw1.ItemsSource = tvwNodes
  End Sub

  Private Sub cboSrc_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    cScx.Encoding = Text.Encoding.GetEncodings(cboSrc.SelectedIndex).GetEncoding
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Instruction1).Source = cScx.Instruction
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Instruction2).Source = cScx.StringInfos(scxFile.InfoType.Instruction)
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Hint).Source = cScx.StringInfos(scxFile.InfoType.Hints)
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Scout).Source = cScx.StringInfos(scxFile.InfoType.Scouts)
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Victory).Source = cScx.StringInfos(scxFile.InfoType.Victory)
    tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Defeat).Source = cScx.StringInfos(scxFile.InfoType.Defeat)
    Dim PlayerNameNodes = tvwNodes.Single(Function(x) x.Type = tvwNode.eType.PlayerParent).Children
    For Each PlayerNameNode In PlayerNameNodes
      PlayerNameNode.Source = cScx.Players(PlayerNameNode.Index).Name
    Next
    Dim NumberNodes = tvwNodes.Single(Function(x) x.Type = tvwNode.eType.NumberParent).Children
    For Each NumberNode In NumberNodes
      NumberNode.Source = cScx.Misc(NumberNode.Index).Name
    Next
    For i = 0 To cScx.Triggers.Count - 1
      Dim tmpI = i
      Dim TriggerNode = tvwNodes.Single(Function(x) x.Type = tvwNode.eType.Trigger And x.Index = tmpI)
      With TriggerNode.Children
        .Single(Function(x) x.Type = tvwNode.eType.TriggerName).Source = cScx.Triggers(i).Name
        .Single(Function(x) x.Type = tvwNode.eType.TriggerDesc).Source = cScx.Triggers(i).Description
      End With
      Dim EffectNodes = TriggerNode.Children.Where(Function(x)
                                                     Return x.Type = tvwNode.eType.TriggerChange Or
                                                     x.Type = tvwNode.eType.TriggerChat Or
                                                     x.Type = tvwNode.eType.TriggerDisplay
                                                   End Function)
      For Each EffectNode In EffectNodes
        EffectNode.Source = cScx.Triggers(i).Effects(EffectNode.SubIndex).Text
      Next
    Next
  End Sub

  Private Sub CopyAll(nod As tvwNode)
    nod.Dest = nod.Source
    For Each ele In nod.Children
      CopyAll(ele)
    Next
  End Sub

  Private Sub mnuHide_Checked(sender As Object, e As RoutedEventArgs)
    Dim ToHide = tvwNodes.
      Where(Function(x) x.Type = tvwNode.eType.Trigger And x.IsObjective = False).
      SelectMany(Function(x) x.Children).
      Where(Function(x) x.Type = tvwNode.eType.TriggerName Or x.Type = tvwNode.eType.TriggerDesc)
    For Each node In ToHide
      node.Visibility = Visibility.Collapsed
    Next
    ToHide = tvwNodes.
      Where(Function(x) x.Type = tvwNode.eType.Trigger And
      x.Children.All(Function(y) y.Visibility = Visibility.Collapsed))
    For Each node In ToHide
      node.Visibility = Visibility.Collapsed
    Next
  End Sub

  Private Sub mnuHide_Unchecked(sender As Object, e As RoutedEventArgs)
    Dim ToShow = tvwNodes.
      Where(Function(x) x.Visibility = Visibility.Collapsed)
    For Each node In ToShow
      node.Visibility = Visibility.Visible
    Next
    ToShow = tvwNodes.
      SelectMany(Function(x) x.Children).
      Where(Function(x) x.Visibility = Visibility.Collapsed)
    For Each node In ToShow
      node.Visibility = Visibility.Visible
    Next
  End Sub

  Private Sub mnuCopyAll_Click(sender As Object, e As RoutedEventArgs)
    If MessageBox.Show("此操作将覆盖所有已修改的译文内容，确定继续吗？", "提示", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
      For Each nod In tvwNodes
        CopyAll(nod)
      Next
    End If
  End Sub

  Private Sub mnuBlankAll_Click(sender As Object, e As RoutedEventArgs)
    If MessageBox.Show("此操作将把所有触发事件的名称设置为空白，确定继续吗？", "提示", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
      Dim ToBlank = tvwNodes.
        Where(Function(x) x.Type = tvwNode.eType.Trigger)
      For Each node In ToBlank
        node.Children(0).Dest = String.Empty
      Next
    End If
  End Sub

  Private Sub mnuNumberAll_Click(sender As Object, e As RoutedEventArgs)
    Dim ToNumber = tvwNodes.
      Where(Function(x) x.Type = tvwNode.eType.Trigger)
    For i = 0 To ToNumber.Count() - 1
      ToNumber(i).Children(0).Dest = $"{txtPrefix.Text}{i}"
    Next
  End Sub

  Private Sub mnuExport_Click(sender As Object, e As RoutedEventArgs)
    Dim exportFile As New Forms.SaveFileDialog
    exportFile.InitialDirectory = IO.Path.GetDirectoryName(ofn.FileName)
    exportFile.FileName = IO.Path.GetFileNameWithoutExtension(ofn.FileName)
    exportFile.DefaultExt = "txt"
    exportFile.Filter = "文本文件|*.txt"
    If exportFile.ShowDialog = Forms.DialogResult.OK Then
      Using sw As New IO.StreamWriter(exportFile.FileName)
        sw.WriteLine("//场景指示")
        sw.WriteLine(tvwNodes(0).Dest)
        For i = 0 To 15
          sw.WriteLine("//玩家" & i + 1 & "名称")
          sw.WriteLine(tvwNodes(1).Children(i).Dest)
        Next
        sw.WriteLine("//场景指南")
        sw.WriteLine(tvwNodes(2).Dest)
        sw.WriteLine("//提示")
        sw.WriteLine(tvwNodes(3).Dest)
        sw.WriteLine("//胜利")
        sw.WriteLine(tvwNodes(4).Dest)
        sw.WriteLine("//失败")
        sw.WriteLine(tvwNodes(5).Dest)
        sw.WriteLine("//历史")
        sw.WriteLine(tvwNodes(6).Dest)
        sw.WriteLine("//侦察")
        sw.WriteLine(tvwNodes(7).Dest)
        For i = 0 To 7
          sw.WriteLine("//玩家" & i + 1 & "代号")
          sw.WriteLine(tvwNodes(8).Children(i).Dest)
        Next
        For Each ele In tvwNodes.Where(Function(p As tvwNode) p.Type = tvwNode.eType.Trigger)
          For Each chd In ele.Children
            sw.Write("//触发" & ele.Index)
            Select Case chd.Type
              Case tvwNode.eType.TriggerName
                sw.WriteLine("名称")
              Case tvwNode.eType.TriggerDesc
                sw.WriteLine("描述")
              Case tvwNode.eType.TriggerChat, tvwNode.eType.TriggerDisplay, tvwNode.eType.TriggerChange
                sw.WriteLine("效果" & chd.SubIndex)
            End Select
            sw.WriteLine(chd.Dest)
          Next
        Next
        sw.Write("//结束")
      End Using
      MessageBox.Show("已导出到 " & exportFile.FileName)
    End If
  End Sub

  Private Sub mnuImport_Click(sender As Object, e As RoutedEventArgs)
    Dim importFile As New Forms.OpenFileDialog
    Dim state As New ImportState, sb As New System.Text.StringBuilder
    importFile.InitialDirectory = IO.Path.GetDirectoryName(ofn.FileName)
    importFile.FileName = IO.Path.GetFileNameWithoutExtension(ofn.FileName)
    importFile.Filter = "文本文件|*.txt"
    importFile.ShowDialog()
    If My.Computer.FileSystem.FileExists(importFile.FileName) Then
      Using sr As New IO.StreamReader(importFile.FileName)
        Do Until sr.EndOfStream
          Dim l As String = sr.ReadLine()
          If l.StartsWith("//") Then
            Select Case state.State
              Case eImportState.I1
                tvwNodes(0).Dest = sb.ToString
              Case eImportState.P
                tvwNodes(1).Children(state.Index).Dest = sb.ToString
              Case eImportState.I2
                tvwNodes(2).Dest = sb.ToString
              Case eImportState.H
                tvwNodes(3).Dest = sb.ToString
              Case eImportState.V
                tvwNodes(4).Dest = sb.ToString
              Case eImportState.D
                tvwNodes(5).Dest = sb.ToString
              Case eImportState.Y
                tvwNodes(6).Dest = sb.ToString
              Case eImportState.S
                tvwNodes(7).Dest = sb.ToString
              Case eImportState.N
                tvwNodes(8).Children(state.Index).Dest = sb.ToString
              Case eImportState.TN
                If tvwNodes.Any(Function(p As tvwNode) p.Type = tvwNode.eType.Trigger And p.Index = state.Index) Then
                  If tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children.Any(Function(p As tvwNode) p.Type = tvwNode.eType.TriggerName) Then tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children(0).Dest = sb.ToString
                End If
              Case eImportState.TD
                If tvwNodes.Any(Function(p As tvwNode) p.Type = tvwNode.eType.Trigger And p.Index = state.Index) Then
                  If tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children.Any(Function(p As tvwNode) p.Type = tvwNode.eType.TriggerDesc) Then tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children(1).Dest = sb.ToString
                End If
              Case eImportState.TE
                If tvwNodes.Any(Function(p As tvwNode) p.Type = tvwNode.eType.Trigger And p.Index = state.Index) Then
                  If tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children.Any(Function(p As tvwNode) (p.Type = tvwNode.eType.TriggerChat Or p.Type = tvwNode.eType.TriggerChange Or p.Type = tvwNode.eType.TriggerDisplay) And p.SubIndex = state.SubIndex) Then tvwNodes.Single(Function(p As tvwNode) p.Index = state.Index).Children.Single(Function(p As tvwNode) p.SubIndex = state.SubIndex).Dest = sb.ToString
                End If
            End Select
            sb.Clear()
            If l.Contains("场景指示") Then
              state.State = eImportState.I1
            ElseIf l.Contains("玩家") And l.Contains("名称") Then
              state.State = eImportState.P
              state.Index = Val(l.Substring(4)) - 1
            ElseIf l.Contains("场景指南") Then
              state.State = eImportState.I2
            ElseIf l.Contains("提示") Then
              state.State = eImportState.H
            ElseIf l.Contains("胜利") Then
              state.State = eImportState.V
            ElseIf l.Contains("失败") Then
              state.State = eImportState.D
            ElseIf l.Contains("历史") Then
              state.State = eImportState.Y
            ElseIf l.Contains("侦察") Then
              state.State = eImportState.S
            ElseIf l.Contains("玩家") And l.Contains("代号") Then
              state.State = eImportState.N
              state.Index = Val(l.Substring(4)) - 1
            ElseIf l.Contains("触发") And l.Contains("名称") Then
              state.State = eImportState.TN
              state.Index = Val(l.Substring(4))
            ElseIf l.Contains("触发") And l.Contains("描述") Then
              state.State = eImportState.TD
              state.Index = Val(l.Substring(4))
            ElseIf l.Contains("触发") And l.Contains("效果") Then
              state.State = eImportState.TE
              state.Index = Val(l.Substring(4))
              state.SubIndex = Val(l.Substring(l.IndexOf("果") + 1))
            End If
          Else
            sb.AppendLine(l)
          End If
        Loop
      End Using
    End If
  End Sub

  Private Sub sceTrans_Closed(sender As Object, e As EventArgs) Handles Me.Closed
    gwSceTrans = Nothing
  End Sub
End Class

<ValueConversion(GetType(sceTrans.tvwNode.eType), GetType(Boolean))>
Public Class IsEnabledConverter
  Implements IValueConverter

  Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
    Select Case CType(value, sceTrans.tvwNode.eType)
      Case sceTrans.tvwNode.eType.None, sceTrans.tvwNode.eType.Trigger
        Return False
      Case Else
        Return True
    End Select
  End Function

  Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
    Return Nothing
  End Function
End Class