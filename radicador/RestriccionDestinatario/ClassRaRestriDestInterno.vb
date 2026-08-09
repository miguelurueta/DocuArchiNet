Public Class CDRaRestriDestInterno
    Property IdRestricionTipoDstInterno As Integer
    Property rad_restri_tipo_dest_interno_IdRestricionTipoDstInterno As Integer
    Property EstadoRestriccion As Integer
    Property MoluloRadicacion As Integer
    Property ModuloRadicacionSimple As Integer
    Property ModuloRadicacionInterna As Integer
    Property DescripcionRestricion As String
    Property FechaRegistro As String
    Property DescripcionTipo As String
End Class

Public Class ClassRaRestriDestInterno

    Function SolicitaEstructuraRestriccion(ByVal IdRestricionTipoDstInterno As Integer,
                                           ByRef CDRaRestriDestInterno As CDRaRestriDestInterno) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con con la restrición relacionada al tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite         : Repesenta el tipo de tramite de radicación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDRaRestriDestInterno : Retorna la estructura con la restricción
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            CDRaRestriDestInterno = New CDRaRestriDestInterno
            Dim Conect As New conect.Dbase_Conction_Mysql_RA
            Dim SQLConsulta As String = " SELECT IdRestriTipoDestInterno," &
                "ra_restri_tipo_dest_interno_IdRestricionTipoDstInterno,EstadoRestriccion,MoluloRadicacion,ModuloRadicacionSimple," &
                "ModuloRadicacionInterna,DescripcionRestricion,EstadoRestriccion" &
                " FROM  ra_restri_dest_interno " &
                " where IdRestriTipoDestInterno=" & IdRestricionTipoDstInterno
            Dim Datset As DataSet = New DataSet("ra_restri_dest_interno")
            Dim Result = Conect.SELECTION_SELECT_FIELD(SQLConsulta, Datset)
            If Result <> "YES" Then
                Return "Inconsistencia funcion SolicitaRelacionTaramiteRetriccion " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CDRaRestriDestInterno = Nothing
                Return "YES"
            Else
                CDRaRestriDestInterno.IdRestricionTipoDstInterno = Datset.Tables(0).Rows(0).Item("IdRestriTipoDestInterno")
                CDRaRestriDestInterno.rad_restri_tipo_dest_interno_IdRestricionTipoDstInterno = Datset.Tables(0).Rows(0).Item("ra_restri_tipo_dest_interno_IdRestricionTipoDstInterno")
                CDRaRestriDestInterno.EstadoRestriccion = Datset.Tables(0).Rows(0).Item("EstadoRestriccion")
                CDRaRestriDestInterno.MoluloRadicacion = Datset.Tables(0).Rows(0).Item("MoluloRadicacion")
                CDRaRestriDestInterno.ModuloRadicacionSimple = Datset.Tables(0).Rows(0).Item("ModuloRadicacionSimple")
                CDRaRestriDestInterno.ModuloRadicacionInterna = Datset.Tables(0).Rows(0).Item("ModuloRadicacionInterna")
                CDRaRestriDestInterno.DescripcionRestricion = Datset.Tables(0).Rows(0).Item("DescripcionRestricion")
                Return "YES"
            End If

        Catch ex As Exception
            SolicitaEstructuraRestriccion = "Inconsistencia general funcion SolicitaEstructuraRestriccion " & ex.Message
        End Try
    End Function
End Class
