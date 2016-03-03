Public Class cpxConverter
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        For Each ele In Text.Encoding.GetEncodings
            cboSrc.Items.Add(ele.DisplayName)
            If ele.CodePage = 936 Then
                cboSrc.SelectedIndex = cboSrc.Items.Count - 1
            End If
        Next
    End Sub
End Class
