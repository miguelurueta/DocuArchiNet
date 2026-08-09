Public Class CDcarateres
    Property CaraterNoValido As String
    Property CaracterRemplazo As String
End Class
Public Class ClassCarateres
    Function SolicitaEstructuraCarateres(ByVal TipoCaracter As Integer,
                                         ByRef CDcarateres As List(Of CDcarateres)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de carateres no validos en el sistema
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'TipoCaracter        : Representa la identificación del tipo de caracter 
        '                      valore 1-> Carateres no validos 2-> Cacteres no validos matricula SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDcarateres  : Retorna la estructura con los caracteres 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select Caracter,CaracterRemplazo  from  ra_config_caracteres " &
                   " where TipoCarcter=" & TipoCaracter
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_config_caracteres")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraCarateres = " La función SolicitaEstructuraCarateres dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CDcarateres = Nothing
                SolicitaEstructuraCarateres = "YES"
                Exit Function
            Else
                Dim ListCDcarateres As New CDcarateres
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ListCDcarateres = New CDcarateres
                    ListCDcarateres.CaraterNoValido = Datset.Tables(0).Rows(i).Item(0)
                    ListCDcarateres.CaracterRemplazo = Datset.Tables(0).Rows(i).Item(1)
                    CDcarateres.Add(ListCDcarateres)
                Next
                SolicitaEstructuraCarateres = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraCarateres = "Inconsistencia general funcion SolicitaEstructuraCarateres " & ex.Message
        End Try
    End Function
    Function RemplazaCaracteresNoValidos(ByRef CDcarateres As List(Of CDcarateres),
                                         ByRef Expresion As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Remplaza carateres no validos de una expresión
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Expresion        : Representa la expresión que contiene los carateres a remoplazar
        'CDcarateres      : Representa la estructura con los caracteres                    
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Expresion  : Retorna la expresion de texto con los caracteres
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            If Expresion = "" Then
                RemplazaCaracteresNoValidos = "YES"
                Exit Function
            End If
            If CDcarateres Is Nothing Then
                RemplazaCaracteresNoValidos = "YES"
                Exit Function
            End If
            If CDcarateres.Count = 0 Then
                RemplazaCaracteresNoValidos = "YES"
                Exit Function
            End If
            For i As Integer = 0 To CDcarateres.Count - 1
                Expresion = Expresion.Replace(CDcarateres.Item(i).CaraterNoValido, CDcarateres.Item(i).CaracterRemplazo)
            Next
            RemplazaCaracteresNoValidos = "YES"
            Exit Function
        Catch ex As Exception
            RemplazaCaracteresNoValidos = "Inconsistencia general funcion RemplazaCaracteresNoValidos " & ex.Message
        End Try
    End Function
End Class
