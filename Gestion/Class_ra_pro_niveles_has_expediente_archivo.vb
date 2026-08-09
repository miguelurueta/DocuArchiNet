Public Structure stru_expediente
    Dim id_registro As Integer
    Dim ra_pro_niveles_id_nivel As Integer
    Dim expediente_archivo_ID_EXPEDIENTE As Integer
    Dim ALEAS_EXPEDIENTE As String
End Structure
Public Class Class_ra_pro_niveles_has_expediente_archivo
    Function Solicita_expedientes_relacion_nivel(ByVal id_nivel As Integer, _
                                                 ByRef stru_expediente() As stru_expediente) As String
        Try
            Dim Parametro_Consulta = "select id_registro,ra_pro_niveles_id_nivel,expediente_archivo_ID_EXPEDIENTE,ea.ALEAS_EXPEDIENTE " & _
                " from ra_pro_niveles_has_expediente_archivo " & _
                " inner join expediente_archivo as ea on (ea.ID_EXPEDIENTE=expediente_archivo_ID_EXPEDIENTE)" & _
            "  WHERE ra_pro_niveles_id_nivel=" & id_nivel & " order by id_registro desc "
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles_has_expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_expedientes_relacion_nivel = "Funcion Solicita_expedientes_relacion_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_expediente = Nothing
                Solicita_expedientes_relacion_nivel = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_expediente(i)
                    stru_expediente(i).id_registro = Datset.Tables(0).Rows(i).Item(0)
                    stru_expediente(i).ra_pro_niveles_id_nivel = Datset.Tables(0).Rows(i).Item(1)
                    stru_expediente(i).expediente_archivo_ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(2)
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru_expediente(i).ALEAS_EXPEDIENTE = ""
                    Else
                        stru_expediente(i).ALEAS_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(3)
                    End If
                Next
                Solicita_expedientes_relacion_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_expedientes_relacion_nivel = "Inconsistencia general función Solicita_expedientes_relacion_nivel " & ex.Message
        End Try
    End Function
    Function Solicita_id_nivel_expediente(ByVal id_expediente As Integer, _
                                          ByRef id_nivel As Integer) As String
        Try
            Dim Parametro_Consulta = "select ra_pro_niveles_id_nivel " & _
             " from ra_pro_niveles_has_expediente_archivo " & _
             " WHERE expediente_archivo_ID_EXPEDIENTE=" & id_expediente
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles_has_expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_nivel_expediente = "Funcion Solicita_id_nivel_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_nivel_expediente = "Imposible encontrar el nivel del expeidente (" & id_expediente & ") en la relación del nivel de produción"
                Exit Function
            Else
                id_nivel = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_nivel_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_nivel_expediente = "Inconsistencia general función Solicita_id_nivel_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_ubicacion_expediente_estructura(ByVal id_producion As Long, _
                                                      ByRef nombre_nivel As String, _
                                                      ByRef nombre_expediente As String, _
                                                      ByRef nombre_propietario_nivel As String, _
                                                      ByRef cargo_propietario_nivel As String, _
                                                      ByRef update As UpdatePanel, _
                                                      ByRef modal As AjaxControlToolkit.ModalPopupExtender) As String
        Try
            Dim Parametro_Consulta = "select rdi.Nombre_Remitente, rdi.Cargo_Remite, ea.ALEAS_EXPEDIENTE, rpn.nombre_nivel  " & _
         " from registro_producion_documental as rpd " & _
         " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rpd.remit_dest_interno_idremit_dest_interno) " & _
         " inner join expediente_archivo as ea on (ea.ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE) " & _
         " inner join ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE) " & _
         " inner join ra_pro_niveles as rpn on (rpn.id_nivel=rpnhea.ra_pro_niveles_id_nivel) " & _
         " WHERE ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_ubicacion_expediente_estructura = "Funcion  Solicita_ubicacion_expediente_estructura dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_ubicacion_expediente_estructura = "Imposible encontrar la ubicación del documento (" & id_producion & ")"
                Exit Function
            Else
                nombre_propietario_nivel = Datset.Tables(0).Rows(0).Item(0)
                cargo_propietario_nivel = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    nombre_expediente = ""
                Else
                    nombre_expediente = Datset.Tables(0).Rows(0).Item(2)
                End If
                nombre_nivel = Datset.Tables(0).Rows(0).Item(3)
                update.Update()
                modal.Show()
                Solicita_ubicacion_expediente_estructura = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_ubicacion_expediente_estructura = "Inconsistencia general función Solicita_ubicacion_expediente_estructura " & ex.Message
        End Try
    End Function

    Function Solicita_relacion_nivel_expediente(ByVal id_expediente As Integer, _
                                               ByVal id_nivel As Integer, _
                                               ByRef estado_propietario As String) As String
        Try
            Dim Parametro_Consulta = "select ra_pro_niveles_id_nivel " & _
           " from ra_pro_niveles_has_expediente_archivo WHERE expediente_archivo_ID_EXPEDIENTE=" & id_expediente & _
           " and ra_pro_niveles_id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relacion_nivel_expediente = "Funcion  Solicita_relacion_nivel_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_propietario = "NO"
                Solicita_relacion_nivel_expediente = "YES"
                Exit Function
            Else
                estado_propietario = "YES"
                Solicita_relacion_nivel_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_relacion_nivel_expediente = "Inconsistencia general función Solicita_relacion_nivel_expediente " & ex.Message
        End Try
    End Function
End Class
