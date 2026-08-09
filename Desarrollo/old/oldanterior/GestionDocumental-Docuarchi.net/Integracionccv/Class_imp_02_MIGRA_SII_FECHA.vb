Public Structure stru_radicado_si
    Dim idliquidacion As Integer
    Dim fecha As String
    Dim tipotramite As String
    Dim idmatriculabase As String
    Dim idproponentebase As String
    Dim identificacionbase As String
    Dim nombrebase As String
    Dim numerorecibo As String
    Dim numerorecuperacion As String
    Dim numeroradicacion As String
    Dim tramitepresencial As String
    Dim firmadoelectronicamente As String
    Dim IMP_02_ID_CLAVE As Integer
    Dim estado_migrado As Integer
End Structure
Public Class Class_imp_02_MIGRA_SII_FECHA
    Function Solicita_rago_fecha_migracion_tramite_sii(ByVal fecha_ini As String, _
                                                       ByVal fecha_fin As String, _
                                                       ByRef stru_radicado_si() As stru_radicado_si) As String
        Try
            Dim Parametro_Consulta As String = "select idliquidacion,fecha,tipotramite,idmatriculabase,idproponentebase,nombrebase  " & _
                ",numerorecibo,numerorecuperacion,numeroradicacion,tramitepresencial,firmadoelectronicamente,IMP_02_ID_CLAVE,estado_migrado" & _
               " from imp_02_migra_sii_fecha  where estado_migrado=0 and fecha between  '" & fecha_ini & "' and '" & fecha_fin & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("imp_02_migra_sii_fecha")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_rago_fecha_migracion_tramite_sii = "Funcion  Solicita_radicado_existencia_radicado_asignado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_rago_fecha_migracion_tramite_sii = "Imposible encontrar radicados para migrar en el rago de fecha informado"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_radicado_si(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) Then
                        stru_radicado_si(i).idliquidacion = 0
                    Else
                        stru_radicado_si(i).idliquidacion = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(1) Then
                        stru_radicado_si(i).fecha = ""
                    Else
                        stru_radicado_si(i).fecha = Trim(Datset.Tables(0).Rows(i).Item(1))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(2) Then
                        stru_radicado_si(i).tipotramite = ""
                    Else
                        stru_radicado_si(i).tipotramite = Trim(Datset.Tables(0).Rows(i).Item(2))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(3) Then
                        stru_radicado_si(i).idmatriculabase = ""
                    Else
                        stru_radicado_si(i).idmatriculabase = Trim(Datset.Tables(0).Rows(i).Item(3))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) Then
                        stru_radicado_si(i).idproponentebase = ""
                    Else
                        stru_radicado_si(i).idproponentebase = Trim(Datset.Tables(0).Rows(i).Item(4))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(5) Then
                        stru_radicado_si(i).nombrebase = ""
                    Else
                        stru_radicado_si(i).nombrebase = Trim(Datset.Tables(0).Rows(i).Item(5))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(6) Then
                        stru_radicado_si(i).numerorecibo = ""
                    Else
                        stru_radicado_si(i).numerorecibo = Trim(Datset.Tables(0).Rows(i).Item(6))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(7) Then
                        stru_radicado_si(i).numerorecuperacion = ""
                    Else
                        stru_radicado_si(i).numerorecuperacion = Trim(Datset.Tables(0).Rows(i).Item(7))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(8) Then
                        stru_radicado_si(i).numeroradicacion = ""
                    Else
                        stru_radicado_si(i).numeroradicacion = Trim(Datset.Tables(0).Rows(i).Item(8))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(9) Then
                        stru_radicado_si(i).tramitepresencial = ""
                    Else
                        stru_radicado_si(i).tramitepresencial = Trim(Datset.Tables(0).Rows(i).Item(9))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(10) Then
                        stru_radicado_si(i).firmadoelectronicamente = ""
                    Else
                        stru_radicado_si(i).firmadoelectronicamente = Trim(Datset.Tables(0).Rows(i).Item(10))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) Then
                        stru_radicado_si(i).IMP_02_ID_CLAVE = 0
                    Else
                        stru_radicado_si(i).IMP_02_ID_CLAVE = Trim(Datset.Tables(0).Rows(i).Item(11))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) Then
                        stru_radicado_si(i).estado_migrado = 0
                    Else
                        stru_radicado_si(i).estado_migrado = Trim(Datset.Tables(0).Rows(i).Item(12))
                    End If
                Next
                Solicita_rago_fecha_migracion_tramite_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_rago_fecha_migracion_tramite_sii = "Inconsistencia general funcion Solicita_rago_fecha_migracion_tramite_sii " & ex.Message
        End Try
    End Function
End Class
