Public Class UserManager

    Private Sub test_Click(sender As Object, e As RoutedEventArgs)
        Dim nfx As New nfxFile("..\..\player.nfx")
    End Sub

    Private Sub UserManager_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        gwUserMan = Nothing
    End Sub
End Class
