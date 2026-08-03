Module ModuleGeneral
    Public Function Identi_Host() As String
        'Funcion retorna nombre y ip del usuario
        Try

            Dim nombreHost As String = System.Net.Dns.GetHostName
            Dim hostInfo As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(nombreHost)
            Dim nombrehostIp As String = ""

            For Each ip As System.Net.IPAddress In hostInfo.AddressList
                nombrehostIp = ip.ToString
            Next
            Identi_Host = nombreHost + "|" + nombrehostIp
            Return Identi_Host
        Catch ex As Exception
            Identi_Host = "Error Genral " & ex.ToString
        End Try
    End Function

End Module
