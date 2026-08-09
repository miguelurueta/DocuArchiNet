Public Class Class_ra_auto_registro_expediente
    Function SolicitaDatosAutoRegistro(ByVal IdAutoRegistro As Integer,
                                       ByRef NombreAutoRegistro As String,
                                       ByRef FuncionServicioDatos As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos del registro de auto completado
        'con el parametro de auto registro
        'Fecha : 2022-06-13
        'Ing . Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  nombre_auto_registro, funcion_servicio_datos" &
            " from ra_auto_registro_expediente where id_auto_registro='" &
            IdAutoRegistro & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_auto_registro_expediente")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosAutoRegistro = "Función Solicita_datos_auto_registro dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosAutoRegistro = "Imposible encontrar  registro de auto registro de expediente del codigo (" & IdAutoRegistro & ") , por favor revice el codigo del auto registro"
                Exit Function
            Else
                NombreAutoRegistro = Datset.Tables(0).Rows(0).Item(0)
                FuncionServicioDatos = Datset.Tables(0).Rows(0).Item(1)
                SolicitaDatosAutoRegistro = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosAutoRegistro = "Inconsistencia general funcion SolicitaDatosAutoRegistro " & ex.Message
        End Try
    End Function
    Function SolicitaDatosFuncionAutoRegistro(ByVal FuncionServicioDatos As String,
                                              ByVal ValueData As Object,
                                              ByVal IdAutoRegistro As Integer,
                                              ByRef stru_campos_expediente() As stru_campos_expediente) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina datos de auto creación de expedientes desde multiplex fuentes externas o tablas
        '          internas
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'funcion_servicio_datos : Representa el nombre del la función de asignación
        'value_condicion        : Representa el valor condiciona para la busqueda de datos
        'id_auto_registro       : Representa la idnetiifcación de la función de auto registro
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_campos_expediente  : Retorna la estructura de campos y valores para la creación del expdiente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
            Dim Class_ra_radicacion_simplificada As New Class_ra_radicacion_simplificada
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            Select Case FuncionServicioDatos
                Case "ConsultarRadicado"  '----/// caso integración SII camara de comercio
                    Dim SCIncripcionSII As CIncripcionSII = ValueData
                    For i As Integer = 0 To stru_campos_expediente.Length - 1
                        Select Case stru_campos_expediente(i).campo_expediente
                            Case "CODIGO_UNICO"
                                If SCIncripcionSII.MATRICULA_SII <> "" Then
                                    stru_campos_expediente(i).valor_campo_expediente = SCIncripcionSII.MATRICULA_SII
                                Else
                                    stru_campos_expediente(i).valor_campo_expediente = SCIncripcionSII.PROPONENTE_SII
                                End If
                            Case "NOMBRE_PERSONA_EXPEDIENTE"
                                stru_campos_expediente(i).valor_campo_expediente = SCIncripcionSII.RSOCIAL_SII
                                stru_campos_expediente(i).valor_campo_expediente = stru_campos_expediente(i).valor_campo_expediente.Replace("'", "")
                            Case "IDENTIFICACION_PERSONA_EXPEDIENTE"
                                If SCIncripcionSII.NIT_SII = "" Then
                                    stru_campos_expediente(i).valor_campo_expediente = "0"
                                Else
                                    stru_campos_expediente(i).valor_campo_expediente = SCIncripcionSII.NIT_SII
                                End If
                        End Select
                    Next
                    SolicitaDatosFuncionAutoRegistro = "YES"
                    Exit Function
                Case "AUTOREGISTROPLANRADEXPEDIENTE"   '---//// Caso auto registro expediente con datos de la plantilla de radicación
                    Dim Class_ra_auto_rel_campos_plantilla_rad_expediente As New Class_ra_auto_rel_campos_plantilla_rad_expediente
                    Dim ra_auto_rel_campos_plantilla_rad_expediente() As ra_auto_rel_campos_plantilla_rad_expediente = Nothing
                    Result = Class_ra_auto_rel_campos_plantilla_rad_expediente.Solicita_estructura_relacion_campos_plantilla_radicado_expediente(IdAutoRegistro,
                                                                                                                                                 ra_auto_rel_campos_plantilla_rad_expediente)
                    If Result <> "YES" Then
                        SolicitaDatosFuncionAutoRegistro = Result
                        Exit Function
                    End If
                    Dim Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
                    Result = Class_ra_detalle_plantilla_radicado.Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente(ValueData,
                                                                                                                                             ra_auto_rel_campos_plantilla_rad_expediente(0).Nombre_Plantilla_Radicado,
                                                                                                                                             ra_auto_rel_campos_plantilla_rad_expediente)
                    If Result <> "YES" Then
                        SolicitaDatosFuncionAutoRegistro = Result
                        Exit Function
                    End If
                    For i As Integer = 0 To stru_campos_expediente.Length - 1
                        For k As Integer = 0 To ra_auto_rel_campos_plantilla_rad_expediente.Length - 1
                            If UCase(stru_campos_expediente(i).campo_expediente) = UCase(ra_auto_rel_campos_plantilla_rad_expediente(k).nombre_campo) Then
                                stru_campos_expediente(i).valor_campo_expediente = ra_auto_rel_campos_plantilla_rad_expediente(k).value_campo_expediente
                            End If
                        Next
                    Next
                    SolicitaDatosFuncionAutoRegistro = "YES"
                    Exit Function

                Case Else
                    SolicitaDatosFuncionAutoRegistro = "faunción (" & FuncionServicioDatos & "), no validada"
                    Exit Function
            End Select
            SolicitaDatosFuncionAutoRegistro = "YES"
        Catch ex As Exception
            SolicitaDatosFuncionAutoRegistro = "Inconsistencia general funcion Solicita_datos_funcion_auto_registro " & ex.Message
        End Try
    End Function
End Class
