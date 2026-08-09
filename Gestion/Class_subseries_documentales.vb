Public Class Class_subseries_documentales
    Function Solicita_lista_series_sub_documentales_id_serie(ByVal id_serie As Integer,
                                                             ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de series documentales relacionados a la serie
        '          para lista en interface
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_serie               : Representa la identificación de la serie
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista series
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim SQLconsulta = "select Id_SubSeries,Nombre_Subserie " &
            " from subseries_documentales WHERE Series_Documentales_Id_Series=" & id_serie &
            " and Estado_SubSerie=1  order by Nombre_Subserie"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_series_sub_documentales_id_serie = "Función Solicita_lista_series_documentales_id_serie  " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_series_sub_documentales_id_serie = "YES"
                Exit Function
            Else
                Solicita_lista_series_sub_documentales_id_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_series_sub_documentales_id_serie = "Inconsistencia general funcion Solicita_lista_series_sub_documentales_id_serie " & ex.Message
        End Try
    End Function
    Function Retorna_Id_Subserie_Consecutivo_TipDoc(ByVal Nombre_SubSerie As String,
                                                    ByVal id_serie_Doc As String,
                                                    ByRef Id_Subserie As String,
                                                    ByRef id_consecutivo_doc As String) As String
        Try

            Dim Parametro_Consulta As String = "Select id_subseries,Consecutivo_Tip_Doc " &
            " from subseries_documentales where Series_Documentales_Id_Series=" & id_serie_Doc &
            " and Nombre_SUBSerie='" & Nombre_SubSerie & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("subseries_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Subserie_Consecutivo_TipDoc = "Función Retorna_Id_Subserie_Consecutivo_TipDoc  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Id_Subserie = Datset.Tables(0).Rows(0).Item(0)
                id_consecutivo_doc = Datset.Tables(0).Rows(0).Item(1)
                Retorna_Id_Subserie_Consecutivo_TipDoc = "YES"
            Else
                Retorna_Id_Subserie_Consecutivo_TipDoc = "Función Retorna_Id_Subserie_Consecutivo_TipDoc No se pudo encontrar el id de la subserie documental"
            End If
        Catch ex As Exception
            Retorna_Id_Subserie_Consecutivo_TipDoc = ex.ToString
        End Try

    End Function
    Function Retorna_nombre_sub_serie(ByVal id_sub_serie As Integer,
                                      ByRef nombre_sub_serie As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Nombre_Subserie " &
                " from subseries_documentales where Id_SubSeries=" &
                 id_sub_serie
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_sub_serie = "Función Retorna_nombre_serie_id_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_sub_serie = "Imposible encontrar el nombre de la sub serie documental con codigo (" & id_sub_serie & ")"
                Exit Function
            Else
                nombre_sub_serie = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_sub_serie = "Inconsistencia general función Retorna_nombre_sub_serie " & ex.Message
        End Try
    End Function
End Class
