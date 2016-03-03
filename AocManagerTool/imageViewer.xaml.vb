Public Class imageViewer

    Private Sub img_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Close()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Me.img, New BitmapImage(New Uri(Tag, UriKind.Absolute)))
    End Sub
End Class
