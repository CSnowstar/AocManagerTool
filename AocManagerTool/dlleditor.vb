Public Class dlleditor
    Private Const RT_STRING = 6
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="LoadLibraryA")>
    Private Shared Function LoadLibrary(<Runtime.InteropServices.In> ByVal lpLibFileName As String) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="FreeLibrary")>
    Private Shared Function FreeLibrary(<Runtime.InteropServices.In> ByVal hLibModule As IntPtr) As Integer
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="EnumResourceNamesA")>
    Private Shared Function EnumResourceNames(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                             <Runtime.InteropServices.In> ByVal lpszType As Integer,
                                             <Runtime.InteropServices.In> ByVal lpEnumFunc As ERNP,
                                             <Runtime.InteropServices.In> ByVal lParam As Integer) As Integer
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="FindResourceA")>
    Private Shared Function FindResource(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                         <Runtime.InteropServices.In> ByVal lpName As Integer,
                                         <Runtime.InteropServices.In> ByVal lpType As Integer) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="LoadResource")>
    Private Shared Function LoadResource(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                         <Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="LockResource")>
    Private Shared Function LockResource(<Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="SizeofResource")>
    Private Shared Function SizeofResource(<Runtime.InteropServices.In> ByVal hModule As IntPtr,
                                           <Runtime.InteropServices.In> ByVal hResInfo As IntPtr) As Integer
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="BeginUpdateResource")>
    Private Shared Function BeginUpdateResource(<Runtime.InteropServices.In> ByVal pFileName As String,
                                                <Runtime.InteropServices.In> ByVal bDeleteExistingResources As Boolean) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="UpdateResourceA")>
    Private Shared Function UpdateResource(<Runtime.InteropServices.In> ByVal hUpdate As IntPtr,
                                           <Runtime.InteropServices.In> ByVal lpType As Integer,
                                           <Runtime.InteropServices.In> ByVal lpName As Integer,
                                           <Runtime.InteropServices.In> ByVal wLanguage As Short,
                                           <Runtime.InteropServices.In, Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.LPArray)> ByVal lpData As Byte(),
                                           <Runtime.InteropServices.In> ByVal cbData As Integer) As Boolean
    End Function
    <Runtime.InteropServices.DllImport("kernel32", entrypoint:="EndUpdateResource")>
    Private Shared Function EndUpdateResource(<Runtime.InteropServices.In> ByVal hUpdate As IntPtr,
                                              <Runtime.InteropServices.In> ByVal fDiscard As Boolean) As Integer
    End Function
    Private Delegate Function ERNP(ByVal hModule As IntPtr, ByVal lpszType As Integer, ByVal lpszName As IntPtr, ByVal lParam As Integer) As Boolean

    Dim alChanged As New List(Of Short)
    Dim gsFile As String
    Private Function EnumResourceNamesProc(ByVal hModule As IntPtr, ByVal lpszType As Integer, ByVal lpszName As IntPtr, ByVal lParam As Integer) As Boolean
        Dim hResInfo As IntPtr = FindResource(hModule, lpszName, lpszType)
        Dim hData As IntPtr = LockResource(LoadResource(hModule, hResInfo))
        Dim sz As Integer = SizeofResource(hModule, hResInfo)
        Dim y(sz - 1) As Byte
        Runtime.InteropServices.Marshal.Copy(hData, y, 0, sz)
        Dim p As Integer = 0, i As Integer = 0
        Do
            Dim l As Short = BitConverter.ToInt16(y, p)
            p += 2
            If l Then lvw1.Items.Add((lpszName.ToInt32 - 1) * 16 + i).SubItems.Add(System.Text.Encoding.Unicode.GetString(y, p, l * 2))
            p += 2 * l
            i += 1
        Loop While p < sz And i < 16
        Return True
    End Function
    Private Sub dlleditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Height = My.Computer.Screen.WorkingArea.Height
        Me.Top = 0
        lvw1.Height = Me.ClientSize.Height - 20
    End Sub

    Public Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If gbFromAocmoder Then
            gsFile = gsDllFile
        Else
            Dim ofn As New OpenFileDialog
            ofn.InitialDirectory = gsPath
            ofn.Filter = "帝国时代语言 DLL 文件|language*.dll"
            ofn.CheckFileExists = True
            ofn.ShowDialog()
            lvw1.Items.Clear()
            gsFile = ofn.FileName
        End If
        lblFile.ForeColor = Color.Black
        lblFile.Text = "当前文件：" & gsFile
        Dim hM As IntPtr = LoadLibrary(gsFile)
        lvw1.BeginUpdate()
        EnumResourceNames(hM, RT_STRING, AddressOf EnumResourceNamesProc, 0)
        FreeLibrary(hM)
        lvw1.EndUpdate()
    End Sub

    Private Sub lvw1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvw1.SelectedIndexChanged
        If lvw1.SelectedItems.Count Then
            txtID.Text = lvw1.SelectedItems(0).Text
            txtString.Text = lvw1.SelectedItems(0).SubItems(1).Text
        End If
    End Sub

    Private Sub btnModify_Click(sender As Object, e As EventArgs) Handles btnModify.Click
        Dim it As ListViewItem = lvw1.FindItemWithText(txtID.Text, False, 0, False)
        If IsNothing(it) Then
            MessageBox.Show("找不到 ID 为 " & txtID.Text & " 的字符串。")
        Else
            it.SubItems(1).Text = txtString.Text
            If Not alChanged.Contains(Int(txtID.Text / 16) + 1) Then alChanged.Add(Int(txtID.Text / 16) + 1)
        End If
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If IsNothing(lvw1.FindItemWithText(txtID.Text, False, 0, False)) Then
            Dim n As Integer
            If Integer.TryParse(txtID.Text, n) Then
                If n >= 0 And n <= 65535 Then
                    If n > Integer.Parse(lvw1.Items(lvw1.Items.Count - 1).Text) Then
                        lvw1.Items.Add(txtID.Text).SubItems.Add(txtString.Text)
                    Else
                        Dim p As Integer = n
                        Do
                            p += 1
                        Loop While IsNothing(lvw1.FindItemWithText(p.ToString, False, 0, False))
                        lvw1.Items.Insert(lvw1.FindItemWithText(p.ToString, False, 0, False).Index, txtID.Text).SubItems.Add(txtString.Text)
                    End If
                    If Not alChanged.Contains(Int(n / 16) + 1) Then alChanged.Add(Int(n / 16) + 1)
                Else
                    MessageBox.Show("ID 超出允许范围。")
                End If
            Else
                MessageBox.Show("数字格式不合法。")
            End If
        Else
            MessageBox.Show("ID 为 " & txtID.Text & " 的字符串已存在。")
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim it As ListViewItem = lvw1.FindItemWithText(txtID.Text, False, 0, False)
        If IsNothing(it) Then
            MessageBox.Show("找不到 ID 为 " & txtID.Text & " 的字符串。")
        Else
            lvw1.Items.Remove(it)
            If Not alChanged.Contains(Int(txtID.Text / 16) + 1) Then alChanged.Add(Int(txtID.Text / 16) + 1)
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim hUpd As IntPtr = BeginUpdateResource(gsFile, False)
        For Each ele In alChanged
            Dim ms As New IO.MemoryStream
            Dim bw As New IO.BinaryWriter(ms, System.Text.Encoding.Unicode)
            For i As Short = (ele - 1) * 16 To ele * 16 - 1
                Dim it As ListViewItem = lvw1.FindItemWithText(i.ToString, False, 0, False)
                If IsNothing(it) Then
                    bw.Write(0S)
                Else
                    bw.Write(CShort(it.SubItems(1).Text.Length))
                    bw.Write(it.SubItems(1).Text.ToCharArray)
                End If
            Next
            UpdateResource(hUpd, RT_STRING, ele, 2052, ms.GetBuffer, CInt(ms.Length))
        Next
        EndUpdateResource(hUpd, False)
        alChanged.Clear()
        lblFile.ForeColor = Color.Red
        lblFile.Text = "已保存修改至" & gsFile
    End Sub

    Private Sub btnQuit_Click(sender As Object, e As EventArgs) Handles btnQuit.Click
        Me.Hide()
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
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
End Class
