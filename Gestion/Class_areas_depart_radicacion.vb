Public Structure stru_area
    Dim Codigo_Area As Integer
    Dim Nombre_Area As String
End Structure
Public Class Class_areas_depart_radicacion
    Function Lista_datos_organigrama_por_codigo_area(ByVal codigo_area As Integer, _
                                                     ByRef id_organigrama As Integer, _
                                                     ByRef nombre_organigrama As String) As String
        Try
            Dim sql_consulta As String = "SELECT ro.Id_Organigrama,ro.NOMBRE_ORGANIGRAMA " & _
            "FROM areas_depart_radicacion as adr inner join Registro_Organigrama as ro on (ro.Id_Organigrama=adr.Registro_Organigrama_Id_Organigrama)" & _
            "where codigo_Area=" & codigo_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_datos_organigrama_por_codigo_area = "Función Lista_datos_organigrama_por_codigo_area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                nombre_organigrama = Datset.Tables(0).Rows(0).Item(1)
                Lista_datos_organigrama_por_codigo_area = "YES"
            Else
                id_organigrama = 0
                nombre_organigrama = ""
                Lista_datos_organigrama_por_codigo_area = "YES"
            End If

        Catch ex As Exception
            Lista_datos_organigrama_por_codigo_area = "Inconsistencia funcion Lista_datos_organigrama_por_codigo_area " & ex.Message
        End Try
    End Function

    Function Lista_AreasDep_Organigrama_Series(ByVal Id_Organigrama As Integer, _
                                               ByRef LisRef As DropDownList) As String
        '******************************************************************************
        'Funcion lista las areas permitidas para el usuario de radicacion
        'Fecha : 2014-08-07
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************************
        Try
            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama & " and Estado_Area=1" & " order by Nombre_Area"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Lista_AreasDep_Organigrama_Series = " Error Listando areas o departementos   " & result
                Return Lista_AreasDep_Organigrama_Series
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                LisRef.Items.Add("SELECCIONE")
                LisRef.Items.Add("TODAS LAS AREAS")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    LisRef.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0))
                Next
                Lista_AreasDep_Organigrama_Series = "YES"
                Exit Function
            Else
                Lista_AreasDep_Organigrama_Series = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series = ex.Message
        End Try
    End Function
    Function Lista_AreasDep_Organigrama_Series_consulta_areas(ByVal Id_Organigrama As UInteger, _
                                                              ByRef refcombo As DropDownList) As String
        Try

            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_Series_consulta_areas = "Función Lista_AreasDep_Organigrama_Series_consulta_areas  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                refcombo.Items.Add("TODAS LAS AREAS")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Lista_AreasDep_Organigrama_Series_consulta_areas = "YES"
            Else
                Lista_AreasDep_Organigrama_Series_consulta_areas = "YES"
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series_consulta_areas = ex.Message
        End Try

    End Function
    Function Lista_AreasDep_Organigrama_remision(ByVal Id_Organigrama As UInteger, _
                                                 ByRef refcombo As DropDownList) As String
        Try

            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_remision = "Función Lista_AreasDep_Organigrama_remision  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Lista_AreasDep_Organigrama_remision = "YES"
            Else
                Lista_AreasDep_Organigrama_remision = "YES"
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_remision = "Inconsitencia general funcion Lista_AreasDep_Organigrama_remision " & ex.Message
        End Try

    End Function
    Function Lista_AreasDep_Organigrama_Series_Default_Items(ByVal Id_Organigrama As Integer, _
                                                             ByVal id_area As Integer, _
                                                             ByRef refcombo As DropDownList) As String
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select Codigo_Area,Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_Series_Default_Items = "Función Lista_AreasDep_Organigrama_Series_Default_Items  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilist As New ListItem
                ilist.Text = ""
                ilist.Value = 0
                refcombo.Items.Add(ilist)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilist)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_area Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_AreasDep_Organigrama_Series_Default_Items = "YES"
                Exit Function
            Else
                Lista_AreasDep_Organigrama_Series_Default_Items = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series_Default_Items = "Inconsistencia general función Lista_AreasDep_Organigrama_Series_Default_Items " & ex.Message
        End Try

    End Function
    Function Lista_AreasDep_Organigrama_Series_Default(ByVal Id_Organigrama As UInteger, _
                                                       ByVal nombre_area As String, _
                                                       ByRef refcombo As DropDownList) As String
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama & " order by Nombre_Area"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_Series_Default = "Función Lista_AreasDep_Organigrama_Series_Default  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Text = nombre_area Then
                        refcombo.Text = nombre_area
                        Exit For
                    End If
                Next
                Lista_AreasDep_Organigrama_Series_Default = "YES"
                Exit Function
            Else
                Lista_AreasDep_Organigrama_Series_Default = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series_Default = "Inconsistencia general función Lista_AreasDep_Organigrama_Series_Default " & ex.Message
        End Try
    End Function
    Function Lista_AreasDep_Organigrama_Series_Default_por_id_area(ByVal Id_Organigrama As UInteger, _
                                                                   ByVal nombre_area As String, _
                                                                   ByVal id_area As Integer, _
                                                                   ByRef refcombo As DropDownList) As String
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama & _
            " and Codigo_Area=" & id_area & " order by Nombre_Area"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_Series_Default_por_id_area = "Función Lista_AreasDep_Organigrama_Series_Default  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Text = nombre_area Then
                        refcombo.Text = nombre_area
                        Exit For
                    End If
                Next
                Lista_AreasDep_Organigrama_Series_Default_por_id_area = "YES"
                Exit Function
            Else
                Lista_AreasDep_Organigrama_Series_Default_por_id_area = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series_Default_por_id_area = "Inconsistencia general función Lista_AreasDep_Organigrama_Series_Default " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_area_departamento(ByVal id_area As Integer, _
                                               ByRef nombre_area_departamento As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion  " & _
                " where Codigo_Area=" & id_area & " order by Nombre_Area"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_area_departamento = "Funcion  Solicita_ciudad_sede_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_area_departamento = ""
                Solicita_nombre_area_departamento = "Imposible encontrar el nmbre del área con el código (" & id_area & ")"
                Exit Function
            Else
                nombre_area_departamento = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_area_departamento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_area_departamento = "Inconsistencia función Solicita_nombre_area_departamento " & ex.Message
        End Try
    End Function
    

    Function Solicita_areas_departamento_organigrama(ByVal Id_Organigrama As Integer, _
                                                     ByRef LisRef As DropDownList, _
                                                     ByRef up_date As UpdatePanel) As String
        '******************************************************************************
        'Funcion lista las areas permitidas para el usuario de radicacion
        'Fecha : 2019-04-10
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************************
        Try

            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Codigo_Area,Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama & " and Estado_Area=1" & " order by Nombre_Area"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_areas_departamento_organigrama = " Error función Solicita_areas_departamento_organigrama   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Dim ilist As New ListItem
                ilist.Text = "SELECCIONE"
                ilist.Value = 0
                LisRef.Items.Add(ilist)
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim ilist_ As New ListItem
                    ilist_.Value = Dat_reader.Tables(0).Rows(i).Item(0)
                    ilist_.Text = Dat_reader.Tables(0).Rows(i).Item(1)
                    LisRef.Items.Add(ilist_)
                Next
                Solicita_areas_departamento_organigrama = "YES"
                Exit Function
            Else
                Solicita_areas_departamento_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_areas_departamento_organigrama = "Inconsistencia general función Solicita_areas_departamento_organigrama " & ex.Message
        Finally
            up_date.Update()
        End Try

    End Function
    Function Lista_areas_clasificacion(ByVal id_organigrama As Integer, _
                                       ByRef stru_area() As stru_area) As String
        '-------------------------------------------------------------
        'Función : Retorna las areas departamento para clasificación
        'Fecha : 2017-01-23
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  Codigo_Area,Nombre_Area " & _
                      " from areas_depart_radicacion WHERE Estado_Publico_Area=1 and " & _
                      " Registro_Organigrama_Id_Organigrama=" & id_organigrama
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_areas_clasificacion = "Error conexión función Lista_areas_clasificacion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_areas_clasificacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_area(i)
                    stru_area(i).Codigo_Area = Dat_reader.Tables(0).Rows(i).Item(0)
                    stru_area(i).Nombre_Area = Dat_reader.Tables(0).Rows(i).Item(1)
                Next
                Lista_areas_clasificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_areas_clasificacion = "Inconsistencia general función Lista_areas_clasificacion " & ex.Message
        End Try
    End Function
    Function Retorna_cod_Area_Departamento(ByVal id_organigrama As Integer, _
                                           ByRef cod_area As Integer, _
                                           ByVal nombre_area As String) As String
        Try
            Dim Parametro_Consulta As String = "select  CODIGO_AREA " & _
              " from AREAS_DEPART_RADICACION where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & id_organigrama _
              & " and NOMBRE_AREA='" & nombre_area & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_cod_Area_Departamento = "Función Retorna_cod_Area_Departamento  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                cod_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_cod_Area_Departamento = "YES"
            Else
                Retorna_cod_Area_Departamento = "Imposible encontrar la identidad de la depedendencia función Retorna_Id_Organigrama_activo_empresa"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_cod_Area_Departamento = "Inconsistencia General Funcion Retorna_id_Area_Departamento " & ex.Message
        End Try
    End Function

    Function Retorna_id_area_usuario_gestion(ByVal id_organigrama As Integer,
                                             ByVal nombre_area As String,
                                             ByRef id_area As Integer) As String
        '****************************************************************************
        'Funcion : Retorna id area usuario gestion con los paramentors nombre area y
        'organigrama
        'Fecha : 2014-07-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '****************************************************************************

        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Codigo_Area " &
              " from areas_depart_radicacion where Registro_Organigrama_Id_Organigrama=" & id_organigrama &
              " and Nombre_Area='" & nombre_area & "'"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_id_area_usuario_gestion = " Error consultando ide area usuario gestion  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Retorna_id_area_usuario_gestion = "Imposible encontrar el (ID) del área del usuario de gestión"
                Exit Function
            Else
                id_area = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_id_area_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_area_usuario_gestion = "Inconsistencia General Funcion Retorna_id_area_usuario_gestion " & ex.Message
        End Try

    End Function

    Function Retorna_nombre_area_por_id_area(ByVal id_area As Integer,
                                             ByRef nombre_area As String) As String
        '******************************************************
        'Funcion : retorna nombre area con el parametro id area
        '
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2014-02-11
        '******************************************************
        Try
            Dim Parametro_Consulta As String = "Select Nombre_Area from areas_depart_radicacion where Codigo_Area=" & id_area
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_area_por_id_area = "Funcion  Retorna_nombre_area_por_id_area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_area_por_id_area = "Imposible encontrar el nombre del área con la identificación del área ( " & id_area & " )"
                Exit Function
            Else
                nombre_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_area_por_id_area = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_nombre_area_por_id_area = "Inconsistencia general funcion Retorna_nombre_area_por_id_area " & ex.Message
        End Try
    End Function
    Function Lista_areas_usuario_gestion_permitido_para_gestionar_pqr(ByVal id_organigrama As Integer,
                                                                      ByRef drop_lis As DropDownList) As String
        Try
            drop_lis.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Area from areas_depart_radicacion as adr " &
                "inner join remit_dest_interno as rdi on (rdi.Areas_Dep_Radicacion_id_Areas_Dep=adr.Codigo_Area and rdi.estado_usuario_para_gestion_pqr=1)" &
                "where Registro_Organigrama_Id_Organigrama=" & id_organigrama
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_areas_usuario_gestion_permitido_para_gestionar_pqr = " Error Listando tipos documentales   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_areas_usuario_gestion_permitido_para_gestionar_pqr = "YES"
                Exit Function
            Else

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drop_lis.Items.Add(Datset.Tables(0).Rows(i).Item(0).ToString)
                Next
                Lista_areas_usuario_gestion_permitido_para_gestionar_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_areas_usuario_gestion_permitido_para_gestionar_pqr = "Inconsistenca general función Lista_areas_usuario_gestion_permitido_para_gestionar_pqr " & ex.Message
        End Try
    End Function
End Class
