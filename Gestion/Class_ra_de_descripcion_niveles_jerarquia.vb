Public Class Class_ra_de_descripcion_niveles_jerarquia
    Function Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion(ByVal id_registro_jerarquia As Integer, ByRef titulo As String, ByRef signatura As String) As String
        '----------------------------------------------------------------------
        'Función : Retorna los datos de descripcion de la relación de jeraquía
        'con el paametro id de jerariquia
        'Fecha : 2017-01-19
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select TITULO,SIGNATURA " & _
                            " from ra_de_descripcion_niveles_jerarquia  " & _
                            " where  RA_DE_REGISTRO_JERARQUIA_ID_REGISTRO_JERARQUIA='" & id_registro_jerarquia & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_descripcion_niveles_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion = "Función Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    titulo = ""
                Else
                    titulo = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    signatura = ""
                Else
                    signatura = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion = "YES"
                Exit Function
            Else
                Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion = "Inconsistencia general función Retorna_datos_de_descripcion_titulo_signatura_nivel_clasificacion " & ex.Message
        End Try
    End Function
    Function Retorna_id_nombre_nivel_descripcion_por_id_jerarquia(ByVal id_jerarquia As Integer, _
                                                                 ByRef id_nivel_descripcion As Integer, _
                                                                 ByRef nombre_nivel_clasificacion As String) As String
        '---------------------------------------------------------------
        'Función : Retorna el nombre del nivel de clasificación,  y el 
        'nombre del nivel de clasficación con el parámetro id jerarquía
        'Fecha : 2017-01-19
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  rdnd.ID_NIVELES_DESCRIPCION, rdnd.NOMBRE_NIVEL_DESCRIPCION " & _
                      " from ra_de_registro_jerarquia as rdrj " & _
                      " inner join ra_de_niveles_descripcion as rdnd on (rdnd.ID_NIVELES_DESCRIPCION=rdrj.RA_DE_NIVELES_DESCRIPCION_ID_NIVELES_DESCRIPCION) " & _
                      " where  ID_REGISTRO_JERARQUIA='" & id_jerarquia & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("ra_de_registro_jerarquia")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_id_nombre_nivel_descripcion_por_id_jerarquia = "Función Retorna_id_nombre_nivel_descripcion_por_id_jerarquia Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_nivel_descripcion = Dat_reader.Tables(0).Rows(0).Item(0)
                nombre_nivel_clasificacion = Dat_reader.Tables(0).Rows(0).Item(1)
                Retorna_id_nombre_nivel_descripcion_por_id_jerarquia = "YES"
                Exit Function
            Else
                Retorna_id_nombre_nivel_descripcion_por_id_jerarquia = "Imposible encontrar la descripción del nivel de clasificación documental del registro de jerarquia número " & id_jerarquia
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_nombre_nivel_descripcion_por_id_jerarquia = "Inconsistencia general función Retorna_id_nombre_nivel_descripcion_por_id_jerarquia " & ex.Message
        End Try
    End Function
End Class
