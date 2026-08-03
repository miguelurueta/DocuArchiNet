Public Class CDeRelacionEstadoRetriccion
    Property IdRestricionTipoDstInterno As Integer
    Property IdTipoRestriccion As Integer
    Property DescripcionTipo As String
    Property MoluloRadicacion As Integer
    Property ModuloRadicacionSimple As Integer
    Property ModuloRadicacionInterna As Integer
End Class
Public Class CDrestriccion
    Property AppError As String
    Property CDeRelacionEstadoRetriccion As CDeRelacionEstadoRetriccion
End Class

Public Class ClassraRestriRelacionTramite
    Function SolicitaEstructuraRelacionTipoRestriccion(ByVal IdTipoTramite As Integer,
                                                       ByRef CDeRelacionEstadoRetriccion As CDeRelacionEstadoRetriccion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita existencia de una restricción relacionada al tipo trámite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'DescripcionRestricion         : Repesenta la decripcón de la restricción
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Existencia                    : Retorna el valor de existencia NO SI
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Conect As New conect.Dbase_Conction_Mysql_RA
            Dim SQLConsulta As String = " SELECT rrdt.IdRestriTipoDestInterno,rrt.IdTipoRestriccion,rrdt.DescripcionRestricion," &
                " rrdt.MoluloRadicacion,rrdt.ModuloRadicacionSimple,rrdt.ModuloRadicacionInterna" &
                " FROM  ra_restri_relacion_tramite as rrr " &
                " inner join ra_restri_dest_interno as rrdt on (rrr.ra_restri_dest_interno_IdRestriTipoDestInterno=rrdt.IdRestriTipoDestInterno)" &
                " inner join ra_restri_tipo_dest_interno as rrt on (rrt.IdRestricionTipoDstInterno=rrdt.ra_restri_tipo_dest_interno_IdRestricionTipoDstInterno)" &
                " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & IdTipoTramite
            Dim Datset As DataSet = New DataSet("ra_restri_dest_interno")
            Result = Conect.SELECTION_SELECT_FIELD(SQLConsulta, Datset)
            If Result <> "YES" Then
                Return "Inconsistencia funcion SolicitaEstructuraEstadoRestriccion " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CDeRelacionEstadoRetriccion = Nothing
                Return "YES"
            Else
                CDeRelacionEstadoRetriccion = New CDeRelacionEstadoRetriccion
                CDeRelacionEstadoRetriccion.IdRestricionTipoDstInterno = Datset.Tables(0).Rows(0).Item(0)
                CDeRelacionEstadoRetriccion.IdTipoRestriccion = Datset.Tables(0).Rows(0).Item(1)
                CDeRelacionEstadoRetriccion.DescripcionTipo = Datset.Tables(0).Rows(0).Item(2)
                CDeRelacionEstadoRetriccion.MoluloRadicacion = Datset.Tables(0).Rows(0).Item(3)
                CDeRelacionEstadoRetriccion.ModuloRadicacionSimple = Datset.Tables(0).Rows(0).Item(4)
                CDeRelacionEstadoRetriccion.ModuloRadicacionInterna = Datset.Tables(0).Rows(0).Item(5)
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaEstructuraEstadoRestriccion " & ex.Message
        End Try
    End Function
    Function SolicitaRelacionTaramiteRetriccion(ByVal IdTipoTramite As Integer,
                                                ByRef idRestriRelacionTramite As Integer) As String

        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita existencia relación tramite restrición
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'DescripcionRestricion         : Repesenta la decripcón de la restricción
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Existencia                    : Retorna el valor de existencia NO SI
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Conect As New conect.Dbase_Conction_Mysql_RA
            Dim SQLConsulta As String = " SELECT idRestriRelacionTramite " &
                " FROM  ra_restri_relacion_tramite " &
                " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & IdTipoTramite
            Dim Datset As DataSet = New DataSet("ra_restri_relacion_tramite")
            Dim Result = Conect.SELECTION_SELECT_FIELD(SQLConsulta, Datset)
            If Result <> "YES" Then
                Return "Inconsistencia funcion SolicitaRelacionTaramiteRetriccion " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                idRestriRelacionTramite = 0
                Return "YES"
            Else
                idRestriRelacionTramite = Datset.Tables(0).Rows(0).Item(0)
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia funcion SolicitaRelacionTaramteRetriccion " & ex.Message
        End Try
    End Function
End Class
