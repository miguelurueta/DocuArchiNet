Public Class CDRadRestriTipoDestInterno
    Property IdRestricionTipoDstInterno As Integer
    Property IdTipoRestriccion As Integer
    Property DescripcionTipo As String
    Property EstadoRestriccion As Integer
End Class
Public Class ClassRadRestriTipoDestInterno
    Function SolicitaListaEstructuraTipoRestriccion(ByVal IdRestricionTipoDstInterno As Integer,
                                                    ByRef CDRadRestriTipoDestInterno As CDRadRestriTipoDestInterno) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con con la lista de tipos de restriciones 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------

        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDRaRestriDestInterno  : Retorna la estructura con la lista de tipos de restricciones
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            CDRadRestriTipoDestInterno = New CDRadRestriTipoDestInterno
            Dim Conect As New conect.Dbase_Conction_Mysql_RA
            Dim SQLConsulta As String = " SELECT IdRestricionTipoDstInterno,IdTipoRestriccion," &
                "DescripcionTipo,EstadoRestriccion" &
                " FROM  ra_restri_tipo_dest_interno " &
                " where  IdRestricionTipoDstInterno=" & IdRestricionTipoDstInterno
            Dim Datset As DataSet = New DataSet("ra_restri_tipo_dest_interno")
            Dim Result = Conect.SELECTION_SELECT_FIELD(SQLConsulta, Datset)
            If Result <> "YES" Then
                Return "Inconsistencia funcion SolicitaRelacionTaramiteRetriccion " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CDRadRestriTipoDestInterno = Nothing
                Return "YES"
            Else
                CDRadRestriTipoDestInterno.IdRestricionTipoDstInterno = Datset.Tables(0).Rows(0).Item("IdRestricionTipoDstInterno")
                CDRadRestriTipoDestInterno.IdTipoRestriccion = Datset.Tables(0).Rows(0).Item("IdTipoRestriccion")
                CDRadRestriTipoDestInterno.DescripcionTipo = Datset.Tables(0).Rows(0).Item("DescripcionTipo")
                CDRadRestriTipoDestInterno.EstadoRestriccion = Datset.Tables(0).Rows(0).Item("EstadoRestriccion")
                Return "YES"
            End If
        Catch ex As Exception
            SolicitaListaEstructuraTipoRestriccion = "Inconsistencia general funcion SolicitaListaEstructuraTipoRestriccion " & ex.Message
        End Try
    End Function
End Class
