Public Class Class_ra_auto_campos_gestion_expediente
    Function SolicitaDatosGestionCamposAutoRegistro(ByVal IdAutoRegistro As Integer,
                                                    ByRef IdFondo As Integer,
                                                    ByRef IdInstrumento As Integer,
                                                    ByRef idArea As Integer,
                                                    ByRef IdSerie As Integer,
                                                    ByRef IdSubSerie As Integer) As String
        '----------------------------------------------------------------
        'Funcion : Solicita datos de auto gestión para los expedientes
        'con el paramtro de identificacion de auto registro
        'Fecha : 2022-06-13
        'Ing . Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  id_fondo, id_instrumento,d_area,id_serie,id_sub_serie" &
            " from ra_auto_campos_gestion_expediente where ra_auto_registro_expediente_id_auto_registro=" &
            IdAutoRegistro
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_auto_campos_gestion_expediente")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosGestionCamposAutoRegistro = "Función SolicitaDatosGestionCamposAutoRegistro dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosGestionCamposAutoRegistro = "Imposible encontrar  registro de campos de auto gestión del codigo (" & IdAutoRegistro & ") , por favor revice el codigo del auto registro"
                Exit Function
            Else
                IdFondo = Datset.Tables(0).Rows(0).Item(0)
                IdInstrumento = Datset.Tables(0).Rows(0).Item(1)
                idArea = Datset.Tables(0).Rows(0).Item(2)
                IdSerie = Datset.Tables(0).Rows(0).Item(3)
                IdSubSerie = Datset.Tables(0).Rows(0).Item(4)
                SolicitaDatosGestionCamposAutoRegistro = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosGestionCamposAutoRegistro = "Inconsistencia general funcion SolicitaDatosGestionCamposAutoRegistro " & ex.Message
        End Try
    End Function
End Class
