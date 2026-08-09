Public Structure stru_ra_de_cuadro_clasificacion
    Dim ID_DE_CUADRO_CLASIFICACION As Integer
    Dim CODIGO_CUADRO As String
    Dim NOMBRE_ENTIDAD_CLASFICACION As String
    Dim FECHA_INICIAL As String
    Dim FECHA_FINAL As String
    Dim empresa_gestion_documental_ID_EMPRESA As Integer
    Dim registro_organigrama_ID_ORGANIGRAMA As Integer
End Structure
Public Class Class_ra_de_cuadro_clasificacion
    Function Retorna_datos_caracterizacion_cuadro_clasificacion(ByVal id_cuadro_clasficacion As Integer, _
                                                                ByRef codigo_pais As String, _
                                                                ByRef fecha_extrema_inicial As String, _
                                                                ByRef fecha_extrema_final As String) As String
        '---------------------------------------------------------------
        'Función : Retorna datos de caracterización cuadro clasificación
        'Fecha : 2017-01-13
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  CODIGO_CUADRO,FECHA_INICIAL,FECHA_FINAL " & _
                      " from ra_de_cuadro_clasificacion where  ID_DE_CUADRO_CLASIFICACION=" & id_cuadro_clasficacion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_caracterizacion_cuadro_clasificacion = "Función Retorna_datos_caracterizacion_cuadro_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                codigo_pais = Dat_reader.Tables(0).Rows(0).Item(0)
                fecha_extrema_inicial = Dat_reader.Tables(0).Rows(0).Item(1)
                fecha_extrema_final = Dat_reader.Tables(0).Rows(0).Item(2)
                Retorna_datos_caracterizacion_cuadro_clasificacion = "YES"
                Exit Function
            Else
                Retorna_datos_caracterizacion_cuadro_clasificacion = "Imposible encontrar el identificador del cuadro de clasificación " & id_cuadro_clasficacion
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_caracterizacion_cuadro_clasificacion = "Inconsistencia general función Retorna_datos_caracterizacion_cuadro_clasificacion " & ex.Message
        End Try
    End Function
    Function Solicita_matriz_estructuras_cuadro_clasficacion(ByVal id_empresa As Integer, _
                                                             ByRef stru_clasficacion() As stru_ra_de_cuadro_clasificacion) As String
        Try
            Erase stru_clasficacion
            Dim Parametro_Consulta As String = "select  ID_DE_CUADRO_CLASIFICACION,CODIGO_CUADRO,NOMBRE_ENTIDAD_CLASFICACION, " & _
                "FECHA_INICIAL,FECHA_FINAL,empresa_gestion_documental_ID_EMPRESA,registro_organigrama_ID_ORGANIGRAMA " & _
                " from ra_de_cuadro_clasificacion where empresa_gestion_documental_ID_EMPRESA=" & id_empresa
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_matriz_estructuras_cuadro_clasficacion = "Función Solicita_matriz_estructuras_cuadro_clasficacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_clasficacion(i)
                    stru_clasficacion(i).ID_DE_CUADRO_CLASIFICACION = Dat_reader.Tables(0).Rows(i).Item("ID_DE_CUADRO_CLASIFICACION")
                    stru_clasficacion(i).CODIGO_CUADRO = Dat_reader.Tables(0).Rows(i).Item("CODIGO_CUADRO")
                    stru_clasficacion(i).NOMBRE_ENTIDAD_CLASFICACION = Dat_reader.Tables(0).Rows(i).Item("NOMBRE_ENTIDAD_CLASFICACION")
                    If Dat_reader.Tables(0).Rows(i).IsNull(2) Then
                        stru_clasficacion(i).FECHA_INICIAL = ""
                    Else
                        stru_clasficacion(i).FECHA_INICIAL = Dat_reader.Tables(0).Rows(i).Item("FECHA_INICIAL")
                    End If
                    If Dat_reader.Tables(0).Rows(i).IsNull(3) Then
                        stru_clasficacion(i).FECHA_FINAL = ""
                    Else
                        stru_clasficacion(i).FECHA_FINAL = Dat_reader.Tables(0).Rows(i).Item("FECHA_FINAL")
                    End If
                    If Dat_reader.Tables(0).Rows(i).IsNull(4) Then
                        stru_clasficacion(i).empresa_gestion_documental_ID_EMPRESA = 0
                    Else
                        stru_clasficacion(i).empresa_gestion_documental_ID_EMPRESA = Dat_reader.Tables(0).Rows(i).Item("empresa_gestion_documental_ID_EMPRESA")
                    End If
                    If Dat_reader.Tables(0).Rows(i).IsNull(5) Then
                        stru_clasficacion(i).registro_organigrama_ID_ORGANIGRAMA = 0
                    Else
                        stru_clasficacion(i).registro_organigrama_ID_ORGANIGRAMA = Dat_reader.Tables(0).Rows(i).Item("registro_organigrama_ID_ORGANIGRAMA")
                    End If
                Next
                Solicita_matriz_estructuras_cuadro_clasficacion = "YES"
                Exit Function
            Else
                Solicita_matriz_estructuras_cuadro_clasficacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_matriz_estructuras_cuadro_clasficacion = "Inconsistencia general función Solicita_matriz_estructuras_cuadro_clasficacion " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_cuadro(ByVal id_cuadro_clasficacion As Integer, _
                                              ByRef stru_clasficacion As stru_ra_de_cuadro_clasificacion) As String
        Try
            Dim Parametro_Consulta As String = "select  ID_DE_CUADRO_CLASIFICACION,CODIGO_CUADRO,NOMBRE_ENTIDAD_CLASFICACION, " & _
                "FECHA_INICIAL,FECHA_FINAL,empresa_gestion_documental_ID_EMPRESA,registro_organigrama_ID_ORGANIGRAMA " & _
                " from ra_de_cuadro_clasificacion where ID_DE_CUADRO_CLASIFICACION=" & id_cuadro_clasficacion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_cuadro_clasificacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_datos_estructura_cuadro = "Función Solicita_datos_estructura_cuadro Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                stru_clasficacion.ID_DE_CUADRO_CLASIFICACION = Dat_reader.Tables(0).Rows(0).Item("ID_DE_CUADRO_CLASIFICACION")
                stru_clasficacion.CODIGO_CUADRO = Dat_reader.Tables(0).Rows(0).Item("CODIGO_CUADRO")
                stru_clasficacion.NOMBRE_ENTIDAD_CLASFICACION = Dat_reader.Tables(0).Rows(0).Item("NOMBRE_ENTIDAD_CLASFICACION")
                If Dat_reader.Tables(0).Rows(0).IsNull(2) Then
                    stru_clasficacion.FECHA_INICIAL = ""
                Else
                    stru_clasficacion.FECHA_INICIAL = Dat_reader.Tables(0).Rows(0).Item("FECHA_INICIAL")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) Then
                    stru_clasficacion.FECHA_FINAL = ""
                Else
                    stru_clasficacion.FECHA_FINAL = Dat_reader.Tables(0).Rows(0).Item("FECHA_FINAL")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(4) Then
                    stru_clasficacion.empresa_gestion_documental_ID_EMPRESA = 0
                Else
                    stru_clasficacion.empresa_gestion_documental_ID_EMPRESA = Dat_reader.Tables(0).Rows(0).Item("empresa_gestion_documental_ID_EMPRESA")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(5) Then
                    stru_clasficacion.registro_organigrama_ID_ORGANIGRAMA = 0
                Else
                    stru_clasficacion.registro_organigrama_ID_ORGANIGRAMA = Dat_reader.Tables(0).Rows(0).Item("registro_organigrama_ID_ORGANIGRAMA")
                End If
                Solicita_datos_estructura_cuadro = "YES"
                Exit Function
            Else
                Solicita_datos_estructura_cuadro = "Imposible encontrar datos del cuadro de clasificacion (" & id_cuadro_clasficacion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_cuadro = "Inconsistencia general función Solicita_datos_estructura_cuadro " & ex.Message
        End Try
    End Function
End Class
