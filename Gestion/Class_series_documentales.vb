Public Class Class_series_documentales
    Function Solicita_lista_serie_id_serie(ByVal id_serie As Integer,
                                           ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista serie documental para despligue
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_serie              : Representa la identificación de la serie
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'control_drow_lista   : Retorna la estructura de la serie para listar
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select  Id_Series,Nombre_Serie  from  series_documentales where Id_Series=" & id_serie &
             " order by Nombre_Serie"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_serie_id_serie = " Función Solicita_lista_serie_id_serie dice " & Result
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
                Solicita_lista_serie_id_serie = "YES"
                Exit Function
            Else
                Solicita_lista_serie_id_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_serie_id_serie = "Inconsistencia general función Solicita_lista_serie_id_serie " & ex.Message
        End Try
    End Function
    Function Lista_series_relacionadas_instrumento_id_area(ByVal id_area_departamento As Integer,
                                                           ByVal id_instrumentos As Integer,
                                                           ByRef drop_list As DropDownList,
                                                           ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE FROM SERIES_DOCUMENTALES  as sd" &
           " WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento &
           " and Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumentos
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_instrumento_id_area = "Función Lista_series_relacionadas_instrumento_id_area  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                Lista_series_relacionadas_instrumento_id_area = "YES"
            Else
                Lista_series_relacionadas_instrumento_id_area = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_instrumento_id_area = "Inconsistencia general función Lista_series_relacionadas_instrumento_id_area " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Lista_series_relacionadas_instrumento_id_area_default(ByVal id_area_departamento As Integer,
                                                                   ByVal id_instrumentos As Integer,
                                                                   ByVal id_serie As Integer,
                                                                   ByRef drop_list As DropDownList,
                                                                   ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE FROM SERIES_DOCUMENTALES " &
           " WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento &
           " and Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumentos
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_instrumento_id_area_default = "Función Lista_series_relacionadas_instrumento_id_area_default  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_serie Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_series_relacionadas_instrumento_id_area_default = "YES"
            Else
                Lista_series_relacionadas_instrumento_id_area_default = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_instrumento_id_area_default = "Inconsistencia general función Lista_series_relacionadas_instrumento_id_area_default " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function

    Function Lista_series_relacionadas_id_area(ByVal id_area_departamento As Integer,
                                               ByVal id_serie As Integer,
                                               ByRef drop_list As DropDownList,
                                               ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE FROM SERIES_DOCUMENTALES " &
           " WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_id_area = "Función Lista_series_relacionadas_id_area  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_serie Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_series_relacionadas_id_area = "YES"
            Else
                Lista_series_relacionadas_id_area = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_id_area = "Inconsistencia general función Lista_series_relacionadas_id_area " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Lista_series_relacionadas_a_instrumento(ByVal id_instrumento As Integer,
                                                     ByRef drop_list As DropDownList,
                                                     ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE,adr.Nombre_Area from SERIES_DOCUMENTALES  as sd" &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=sd.Areas_Depart_Radicacion_Codigo_Area)" &
            " WHERE Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_a_instrumento = "Función Lista_series_relacionadas_a_instrumento  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1) & " (" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                Lista_series_relacionadas_a_instrumento = "YES"
            Else
                Lista_series_relacionadas_a_instrumento = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_a_instrumento = "Inconsistencia general función Lista_series_relacionadas_a_instrumento " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Lista_series_relacionadas_a_instrumento_simple(ByVal id_instrumento As Integer,
                                                            ByRef drop_list As DropDownList,
                                                            ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE,adr.Nombre_Area from SERIES_DOCUMENTALES  as sd" &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=sd.Areas_Depart_Radicacion_Codigo_Area)" &
            " WHERE Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_a_instrumento_simple = "Función Lista_series_relacionadas_a_instrumento_simple  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                Lista_series_relacionadas_a_instrumento_simple = "YES"
            Else
                Lista_series_relacionadas_a_instrumento_simple = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_a_instrumento_simple = "Inconsistencia general función Lista_series_relacionadas_a_instrumento_simple " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Solicita_nombre_serie_documental(ByVal id_serie As Integer,
                                              ByRef nombre_serie As String) As String
        Try
            Dim Parametro_Consulta = "select NOMBRE_SERIE FROM SERIES_DOCUMENTALES " &
             " WHERE Id_Series=" & id_serie
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_serie_documental = "Función Solicita_nombre_serie_documental  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_serie = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_serie_documental = "YES"
                Exit Function
            Else
                Solicita_nombre_serie_documental = "Imposible encontrar la identicación de la serie (" & nombre_serie & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_serie_documental = "Inconsistencia general función Solicita_nombre_serie_documental " & ex.Message
        End Try
    End Function
    Function Lista_series_relacionadas_instrumento_id_area_default_producion(ByVal id_area_departamento As Integer,
                                                                             ByVal id_instrumentos As Integer,
                                                                             ByVal id_serie As Integer,
                                                                             ByRef drop_list As DropDownList,
                                                                             ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE,adr.Nombre_Area FROM SERIES_DOCUMENTALES as sd " &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=sd.Areas_Depart_Radicacion_Codigo_Area)" &
            " WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area_departamento &
            " and Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumentos
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_instrumento_id_area_default_producion = "Función Lista_series_relacionadas_instrumento_id_area_default_producion  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1) & " (" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_serie Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_series_relacionadas_instrumento_id_area_default_producion = "YES"
            Else
                Lista_series_relacionadas_instrumento_id_area_default_producion = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_instrumento_id_area_default_producion = "Inconsistencia general función Lista_series_relacionadas_instrumento_id_area_default_producion " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Lista_series_relacionadas_instrumento_default_producion(ByVal id_instrumentos As Integer,
                                                                     ByVal id_serie As Integer,
                                                                     ByRef drop_list As DropDownList,
                                                                     ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE,adr.Nombre_Area FROM SERIES_DOCUMENTALES as sd " &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=sd.Areas_Depart_Radicacion_Codigo_Area)" &
            " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumentos
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_instrumento_default_producion = "Función Lista_series_relacionadas_instrumento_default_producion  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1) & " (" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_serie Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_series_relacionadas_instrumento_default_producion = "YES"
            Else
                Lista_series_relacionadas_instrumento_default_producion = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_instrumento_default_producion = "Inconsistencia general función Lista_series_relacionadas_instrumento_default_producion " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Lista_series_relacionadas_instrumento_default_producion_simple(ByVal id_instrumentos As Integer,
                                                                            ByVal id_serie As Integer,
                                                                            ByRef drop_list As DropDownList,
                                                                            ByRef Update As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE,adr.Nombre_Area FROM SERIES_DOCUMENTALES as sd " &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=sd.Areas_Depart_Radicacion_Codigo_Area)" &
            " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumentos
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_relacionadas_instrumento_default_producion_simple = "Función Lista_series_relacionadas_instrumento_default_producion_simple  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_serie Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_series_relacionadas_instrumento_default_producion_simple = "YES"
            Else
                Lista_series_relacionadas_instrumento_default_producion_simple = "YES"
            End If
        Catch ex As Exception
            Lista_series_relacionadas_instrumento_default_producion_simple = "Inconsistencia general función Lista_series_relacionadas_instrumento_default_producion_simple " & ex.Message
        Finally
            Update.Update()
        End Try
    End Function
    Function Verifica_serie_relacionada_instrumento_archivistico(ByVal id_instrumento As Integer,
                                                                 ByRef id_serie As Integer) As String
        '----------------------------------------------------
        'Función : Verifica las series relacionadas al 
        'instrumento archivístico
        'Fecha : 2018-06-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Id_Series from series_documentales " &
                " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_serie_relacionada_instrumento_archivistico = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_instrumento = 0
                Verifica_serie_relacionada_instrumento_archivistico = "YES"
                Exit Function
            Else
                id_serie = Datset.Tables(0).Rows(0).Item(0)
                Verifica_serie_relacionada_instrumento_archivistico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_serie_relacionada_instrumento_archivistico = "Inconsistencia general función Verifica_serie_relacionada_instrumento_archivistico " & ex.Message
        End Try
    End Function
    Function Retorna_Id_serie_instrumento_Documental(ByVal Id_AreaDep As String,
                                                     ByVal Nombre_SerieDep As String,
                                                     ByVal id_instrumento As Integer,
                                                     ByRef Id_Serie_Documental As String,
                                                     ByRef Consecutivo_SubSerie As String,
                                                     ByRef Consecutivo_Serie As Integer) As String
        Try

            Dim Parametro_Consulta As String = "Select id_series,Consecutivo_subserie,Consecutivo_serie " &
            " from series_documentales where Areas_Depart_Radicacion_Codigo_Area=" & Id_AreaDep &
            " and Nombre_Serie='" & Nombre_SerieDep & "' and Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_serie_instrumento_Documental = "Función Retorna_Id_serie_instrumento_Documental  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Id_Serie_Documental = Datset.Tables(0).Rows(0).Item(0)
                Consecutivo_SubSerie = Datset.Tables(0).Rows(0).Item(1)
                Consecutivo_Serie = Datset.Tables(0).Rows(0).Item(2)
                Retorna_Id_serie_instrumento_Documental = "YES"
            Else
                Retorna_Id_serie_instrumento_Documental = "No se pudo encontrar el id de la serie documental función Retorna_Id_serie_Documental"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_serie_instrumento_Documental = "Inconsistencia general función Retorna_Id_serie_instrumento_Documental " & ex.Message
        End Try

    End Function
    Function Retorna_id_area_serie_documental(ByVal id_serie As Integer,
                                              ByRef id_area As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Areas_Depart_Radicacion_Codigo_Area " &
                " from series_documentales where Id_Series=" &
                 id_serie
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_area_serie_documental = "Función Retorna_id_area_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_area_serie_documental = "Imposible encontrar el área de la serie documental (" & id_serie & ")"
                Exit Function
            Else
                id_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_area_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_area_serie_documental = "Inconsistencia general función Retorna_id_area_serie_documental " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_serie_id_serie(ByVal id_serie As Integer,
                                          ByRef nombre_serie As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Nombre_Serie " &
                " from series_documentales where Id_Series=" &
                 id_serie
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_serie_id_serie = "Función Retorna_nombre_serie_id_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_serie_id_serie = "Imposible encontrar el nombre de la serie documental con codigo (" & id_serie & ")"
                Exit Function
            Else
                nombre_serie = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_serie_id_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_serie_id_serie = "Inconsistencia genera función Retorna_nombre_serie_id_serie " & ex.Message
        End Try
    End Function

    Function Solicita_id_sistema_meta_dato_serie_documental(ByVal id_serie_documental As Integer,
                                                            ByRef id_sistema_meta_dato As Integer) As String
        '--------------------------------------------------------
        'Fucion : Solicita el sistema meta datos relacionado a la
        'serie documental
        'Fecha : 2022-02-08
        'Ing . Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ra_m_sistema_meta_datos_id_sistema_meta_datos " &
                "from series_documentales where Id_Series=" & id_serie_documental
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_sistema_meta_dato_serie_documental = "Functión Solicita_id_sistema_meta_dato_serie_documental dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_sistema_meta_dato = 0
                Solicita_id_sistema_meta_dato_serie_documental = "YES"
                Exit Function
            Else
                id_sistema_meta_dato = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_sistema_meta_dato_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_sistema_meta_dato_serie_documental = "Inconsistencia general fubncion Solicita_id_sistema_meta_dato_serie_documental " & ex.Message
        End Try
    End Function
End Class
