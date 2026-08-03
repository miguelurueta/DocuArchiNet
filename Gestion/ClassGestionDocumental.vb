Imports System.IO
Imports MySql.Data
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO.IsolatedStorage
Public Class ClassGestionDocumental
    Function Retorna_Areas_Departamento_Radicacion(ByVal id_empresa As Integer, _
                                                    ByRef Organigrama As String, _
                                                    ByRef Refcombo As DropDownList) As String
        Try
            Dim Result As String = ""
            Refcombo.Items.Clear()
            '************************************************
            'Consulta el id de la empresa de gestion
            '************************************************
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Dim id_organigrama As Integer = -1
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(Organigrama, _
                                                                       id_empresa, _
                                                                       id_organigrama)
            If Result <> "YES" Then
                Retorna_Areas_Departamento_Radicacion = Result
                Exit Function
            End If
            '***********************************************
            'Lsuta areas departamento
            '***********************************************
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series(id_organigrama, _
                                                                                     Refcombo)
            If Result <> "YES" Then
                Retorna_Areas_Departamento_Radicacion = Result
                Exit Function
            End If
            Retorna_Areas_Departamento_Radicacion = "YES"
        Catch ex As Exception
            Retorna_Areas_Departamento_Radicacion = "Inconsistencia General Funcion Retorna_Areas_Departamento_Radicacion : " & ex.Message
        End Try
    End Function
    Function Retorna_codigo_sub_area_departamento_radicacion(ByVal id_area As Integer, _
                                                             ByVal nombre_sub_area As String, _
                                                             ByRef id_sub_area As Integer) As String
        '-----------------------------------------------------------
        'Función : Retorna codigo sub seccion o sub area con los
        'parametros id area, nombre sub area
        'Fecha : 2016-10-21
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_SUB_AREA  " & _
                   " from sub_areas_depart_radicacion  where CODIGO_AREA = " & id_area & " and Nombre_Area='" & nombre_sub_area & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("sub_areas_depart_radicacion")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_codigo_sub_area_departamento_radicacion = "Funcion  Retorna_codigo_sub_area_departamento_radicacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_codigo_sub_area_departamento_radicacion = "Imposible encontrar la identificación de la sub seccion "
                Exit Function
            Else
                id_sub_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_codigo_sub_area_departamento_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_codigo_sub_area_departamento_radicacion = "Inconsistencia general función Retorna_codigo_sub_area_departamento_radicacion " & ex.Message
        End Try
    End Function
    Function Retorna_Sub_Areas_Departamento_Radicacion(ByVal id_area As Integer, _
                                                  ByRef Refcombo As DropDownList) As String
        '--------------------------------------------------------
        'Función retorna las sub areas de un area especifica
        'con el parametro identificacion de area
        'Ing Miguel Angel Urueta Miranda
        'Fecha 2016-10-20
        '---------------------------------------------------------
        Try
            Refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select Nombre_Area  " & _
                " from sub_areas_depart_radicacion  where CODIGO_AREA = " & id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("sub_areas_depart_radicacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Sub_Areas_Departamento_Radicacion = "Funcion  Retorna_Sub_Areas_Departamento_Radicacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Retorna_Sub_Areas_Departamento_Radicacion = "YES"
                Exit Function
            Else
                Retorna_Sub_Areas_Departamento_Radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Sub_Areas_Departamento_Radicacion = "Inconsistencia General Función Retorna_Sub_Areas_Departamento_Radicacion : " & ex.Message
        End Try
    End Function
    Function Retorna_Sub_Areas_Departamento_Radicacion(ByVal id_area As Integer, _
                                                  ByRef Refcombo As DropDownList, ByVal nombre_sub_area As String) As String
        '--------------------------------------------------------
        'Función retorna las sub areas de un area especifica
        'con el parametro identificacion de area
        'Ing Miguel Angel Urueta Miranda
        'Fecha 2016-10-20
        '---------------------------------------------------------
        Try
            Refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select Nombre_Area  " & _
                " from sub_areas_depart_radicacion  where CODIGO_AREA = " & id_area
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("sub_areas_depart_radicacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Sub_Areas_Departamento_Radicacion = "Funcion  Retorna_Sub_Areas_Departamento_Radicacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If Refcombo.Items.Count > 0 Then
                    For i As Integer = 0 To Refcombo.Items.Count - 1
                        If Refcombo.Items(i).Value = nombre_sub_area Then
                            Refcombo.Text = nombre_sub_area
                            Exit For
                        End If
                    Next

                End If
                Retorna_Sub_Areas_Departamento_Radicacion = "YES"
                Exit Function
            Else
                Retorna_Sub_Areas_Departamento_Radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Sub_Areas_Departamento_Radicacion = "Inconsistencia General Función Retorna_Sub_Areas_Departamento_Radicacion : " & ex.Message
        End Try
    End Function

    

    Function Listar_Entidad_Series_Documentales(ByVal id_empresa As Integer, _
                                                ByVal nombre_organigrama As String, _
                                                ByRef refcombo As DropDownList, _
                                                ByVal nombre_area As String) As String
        Try
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama, _
                                                                       id_empresa, _
                                                                       id_organigrama)
            If Result <> "YES" Then
                Listar_Entidad_Series_Documentales = Result
                Exit Function
            End If
            Dim codigo_area As Integer = 0
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama, _
                                                                                     codigo_area, _
                                                                                     nombre_area)
            If Result <> "YES" Then
                Listar_Entidad_Series_Documentales = Result
                Exit Function
            End If
            Result = Listar_Series_Documentales(codigo_area.ToString, refcombo)
            If Result <> "YES" Then
                Listar_Entidad_Series_Documentales = Result
                Exit Function
            End If
            Listar_Entidad_Series_Documentales = "YES"
        Catch ex As Exception
            Listar_Entidad_Series_Documentales = "Inconsistencia función Listar_Entidad_Series_Documentales " & ex.Message
        End Try
    End Function

    Function Listar_Series_Documentales(ByVal Id_Areadep As String, _
    ByRef refcombo As DropDownList) As String
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta = "select NOMBRE_SERIE FROM SERIES_DOCUMENTALES " & _
            " WHERE Areas_Depart_Radicacion_Codigo_Area=" & Id_Areadep
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Series_Documentales = "Función Listar_Series_Documentales  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Listar_Series_Documentales = "YES"
            Else
                Listar_Series_Documentales = "YES"
            End If

        Catch ex As Exception
            Listar_Series_Documentales = "Inconsistencia General Funcion Listar_Series_Documentales " & ex.Message
        End Try
    End Function
    Function Listar_Series_Documentales_Items(ByVal Id_Areadep As String, _
    ByRef refcombo As DropDownList) As String
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta = "select Id_Series,NOMBRE_SERIE FROM SERIES_DOCUMENTALES " & _
            " WHERE Areas_Depart_Radicacion_Codigo_Area=" & Id_Areadep
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Series_Documentales_Items = "Función Listar_Series_Documentales_Items  Imposible conectar la base de datos del gestor documental " & Result
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
                Listar_Series_Documentales_Items = "YES"
                Exit Function
            Else
                Listar_Series_Documentales_Items = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Series_Documentales_Items = "Inconsistencia General Funcion Listar_Series_Documentales_Items " & ex.Message
        End Try
    End Function
    Function Listar_Entidad_Sub_Series_Documentales(ByVal id_empresa As Integer, _
                                                    ByVal nombre_organigrama As String, _
                                                    ByRef refcombo As DropDownList, _
                                                    ByVal nombre_area As String, _
                                                    ByVal nombre_serie As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionDocumental
            Dim id_organigrama As Integer = 0
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama, id_empresa, id_organigrama)
            If Result <> "YES" Then
                Listar_Entidad_Sub_Series_Documentales = Result
                Exit Function
            End If
            Dim codigo_area As Integer = 0
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama, _
                                                                                     codigo_area, _
                                                                                     nombre_area)
            If Result <> "YES" Then
                Listar_Entidad_Sub_Series_Documentales = Result
                Exit Function
            End If
            'Result = Listar_Series_Documentales(codigo_area, refcombo)
            'If Result <> "YES" Then
            '    Listar_Entidad_Sub_Series_Documentales = Result
            '    Exit Function
            'End If
            Dim id_serie As Integer = 0
            Dim consecutivo_serie As Integer = 0
            Dim consecutivo_Sub_serie As Integer = 0
            Result = Retorna_Id_serie_Documental(codigo_area, nombre_serie, id_serie, consecutivo_serie, consecutivo_Sub_serie)
            If Result <> "YES" Then
                Listar_Entidad_Sub_Series_Documentales = Result
                Exit Function
            End If
            Result = Listar_SubSeries_Documentales(id_serie, refcombo)
            If Result <> "YES" Then
                Listar_Entidad_Sub_Series_Documentales = Result
                Exit Function
            End If
            Listar_Entidad_Sub_Series_Documentales = "YES"
        Catch ex As Exception
            Listar_Entidad_Sub_Series_Documentales = "Inconsistencia función Listar_Entidad_Sub_Series_Documentales " & ex.Message
        End Try
    End Function
    
    Function Listar_SubSeries_Documentales(ByVal id_SerieDoc As String _
    , ByRef refcombo As DropDownList) As String
        Dim Parametro_Consulta = "select NOMBRE_SUBSERIE FROM SUBSERIES_DOCUMENTALES " & _
        " WHERE Series_Documentales_Id_Series=" & id_SerieDoc
        Try
            refcombo.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales = "Función Listar_SubSeries_Documentales  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Listar_SubSeries_Documentales = "YES"
                Exit Function
            Else
                Listar_SubSeries_Documentales = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_SubSeries_Documentales = "Inconsistencia general función Listar_SubSeries_Documentales " & ex.Message
        End Try
    End Function
    Function Listar_SubSeries_Documentales_items(ByVal id_SerieDoc As String _
    , ByRef refcombo As DropDownList) As String
        Dim Parametro_Consulta = "select Id_SubSeries,NOMBRE_SUBSERIE FROM SUBSERIES_DOCUMENTALES " & _
        " WHERE Series_Documentales_Id_Series=" & id_SerieDoc
        Try
            refcombo.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales_items = "Función Listar_SubSeries_Documentales_items  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis As New ListItem
                ilis.Text = ""
                ilis.Value = 0
                refcombo.Items.Add(ilis)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis = New ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis)
                Next
                Listar_SubSeries_Documentales_items = "YES"
                Exit Function
            Else
                Listar_SubSeries_Documentales_items = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_SubSeries_Documentales_items = "Inconsistencia general función Listar_SubSeries_Documentales_items " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_sub_Serie_por_id(ByVal id_sub_serie As Integer, _
                                            ByRef nombre_sub_serie As String) As String
        Try
            Dim Parametro_Consulta As String = "select Nombre_Subserie " & _
              " from subseries_documentales where Id_SubSeries=" & id_sub_serie
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("subseries_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_sub_Serie_por_id = "Función Retorna_nombre_sub_Serie_por_id  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_sub_serie = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_sub_Serie_por_id = "YES"
            Else
                Retorna_nombre_sub_Serie_por_id = "Imposible encontrar el nombre de la sub serie (" & id_sub_serie & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_sub_Serie_por_id = "Inconsistencia general función Retorna_nombre_sub_Serie_por_id " & ex.Message
        End Try
    End Function
    Function Listar_SubSeries_Documentales_default(ByVal id_SerieDoc As String, _
                                                   ByVal nombre_sub_serie As String, _
                                                   ByRef refcombo As DropDownList) As String
        Dim Parametro_Consulta = "select NOMBRE_SUBSERIE FROM SUBSERIES_DOCUMENTALES " & _
        " WHERE Series_Documentales_Id_Series=" & id_SerieDoc
        Try
            refcombo.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales_default = "Función Listar_SubSeries_Documentales_default  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Text = nombre_sub_serie Then
                        refcombo.Text = nombre_sub_serie
                        Exit For
                    End If
                Next
                Listar_SubSeries_Documentales_default = "YES"
                Exit Function
            Else
                Listar_SubSeries_Documentales_default = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_SubSeries_Documentales_default = "Inconsistencia general función Listar_SubSeries_Documentales_default " & ex.Message
        End Try
    End Function
    Function Listar_SubSeries_Documentales_default_item(ByVal id_SerieDoc As String, _
                                                   ByVal id_sub_serie As Integer, _
                                                   ByRef refcombo As DropDownList) As String
        Dim Parametro_Consulta = "select Id_SubSeries,NOMBRE_SUBSERIE FROM SUBSERIES_DOCUMENTALES " & _
        " WHERE Series_Documentales_Id_Series=" & id_SerieDoc
        Try
            refcombo.Items.Clear()
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales_default_item = "Función Listar_SubSeries_Documentales_default_item  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis_ As New ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                refcombo.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis_)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_sub_serie Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Listar_SubSeries_Documentales_default_item = "YES"
                Exit Function
            Else
                Listar_SubSeries_Documentales_default_item = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_SubSeries_Documentales_default_item = "Inconsistencia general función Listar_SubSeries_Documentales_default_item " & ex.Message
        End Try
    End Function
    Function Retorna_Id_serie_Documental(ByVal Id_AreaDep As String, _
                                         ByVal Nombre_SerieDep As String, _
                                         ByRef Id_Serie_Documental As String, _
                                         ByRef Consecutivo_SubSerie As String, _
                                         ByRef Consecutivo_Serie As Integer) As String
        Try

            Dim Parametro_Consulta As String = "Select id_series,Consecutivo_subserie,Consecutivo_serie " & _
            " from series_documentales where Areas_Depart_Radicacion_Codigo_Area=" & Id_AreaDep & _
            " and Nombre_Serie='" & Nombre_SerieDep & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("series_documentales")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_serie_Documental = "Función Retorna_Id_serie_Documental  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Id_Serie_Documental = Datset.Tables(0).Rows(0).Item(0)
                Consecutivo_SubSerie = Datset.Tables(0).Rows(0).Item(1)
                Consecutivo_Serie = Datset.Tables(0).Rows(0).Item(2)
                Retorna_Id_serie_Documental = "YES"
            Else
                Retorna_Id_serie_Documental = "No se pudo encontrar el id de la serie documental función Retorna_Id_serie_Documental"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_serie_Documental = "Inconsistencia general función Retorna_Id_serie_Documental " & ex.Message
        End Try

    End Function
    

    Function lista_areas_permitidas_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                    ByRef refcombo As DropDownList) As String
        '************************************************************
        'Funcion : Función lista las areas de gestion permitidas
        'para el usuario
        'Fecha : 2015-03-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select adr.Nombre_Area from ra_area_departamento_permitida_usuario_gestion as rdp " & _
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA) " & _
            " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_area_departamento_permitida_usuario_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_areas_permitidas_usuario_gestion = "Función lista_areas_permitidas_usuario_gestion  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                lista_areas_permitidas_usuario_gestion = "YES"
            Else
                lista_areas_permitidas_usuario_gestion = "YES"
            End If


        Catch ex As Exception
            lista_areas_permitidas_usuario_gestion = "Inconsistencia General Funcion lista_areas_permitidas_usuario_gestion  : " & ex.Message
        End Try
    End Function
    Function lista_areas_permitidas_usuario_gestion_organigrama(ByVal id_usuario_gestion As Integer, _
                                                                 ByVal id_organigrama As Integer, _
                                                                 ByRef refcombo As DropDownList) As String
        '************************************************************
        'Funcion : Función lista las areas de gestion permitidas
        'para el usuario y el organigrama relacionado
        'Fecha : 2015-03-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select adr.Nombre_Area from ra_area_departamento_permitida_usuario_gestion as rdp " & _
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA and adr.Registro_Organigrama_Id_Organigrama=" & id_organigrama & ") " & _
            " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_area_departamento_permitida_usuario_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_areas_permitidas_usuario_gestion_organigrama = "Función llista_areas_permitidas_usuario_gestion_organigrama  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                lista_areas_permitidas_usuario_gestion_organigrama = "YES"
            Else
                lista_areas_permitidas_usuario_gestion_organigrama = "YES"
            End If
        Catch ex As Exception
            lista_areas_permitidas_usuario_gestion_organigrama = "Inconsistencia General Funcion lista_areas_permitidas_usuario_gestion_organigrama  : " & ex.Message
        End Try
    End Function
    Function lista_areas_permitidas_usuario_gestion_organigrama_items(ByVal id_usuario_gestion As Integer, _
                                                                 ByVal id_organigrama As Integer, _
                                                                 ByRef refcombo As DropDownList) As String
        '************************************************************
        'Funcion : Función lista las areas de gestion permitidas
        'para el usuario y el organigrama relacionado
        'Fecha : 2015-03-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select rdp.AREA_ARCHIVO_ID_AREA,adr.Nombre_Area from ra_area_departamento_permitida_usuario_gestion as rdp " & _
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA and adr.Registro_Organigrama_Id_Organigrama=" & id_organigrama & ") " & _
            " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_area_departamento_permitida_usuario_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_areas_permitidas_usuario_gestion_organigrama_items = "Función lista_areas_permitidas_usuario_gestion_organigrama_items  Imposible conectar la base de datos del gestor documental " & Result
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
                lista_areas_permitidas_usuario_gestion_organigrama_items = "YES"
            Else
                lista_areas_permitidas_usuario_gestion_organigrama_items = "YES"
            End If
        Catch ex As Exception
            lista_areas_permitidas_usuario_gestion_organigrama_items = "Inconsistencia General Funcion lista_areas_permitidas_usuario_gestion_organigrama_items  : " & ex.Message
        End Try
    End Function
    Function lista_areas_permitidas_usuario_gestion_organigrama_default(ByVal id_usuario_gestion As Integer, _
                                                                 ByVal id_organigrama As Integer, _
                                                                 ByVal nombre_area As String, _
                                                                 ByRef refcombo As DropDownList) As String
        '************************************************************
        'Funcion : Función lista las areas de gestion permitidas
        'para el usuario y el organigrama relacionado
        'Fecha : 2015-03-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "Select adr.Nombre_Area from ra_area_departamento_permitida_usuario_gestion as rdp " & _
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdp.AREA_ARCHIVO_ID_AREA and adr.Registro_Organigrama_Id_Organigrama=" & id_organigrama & ") " & _
            " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_area_departamento_permitida_usuario_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_areas_permitidas_usuario_gestion_organigrama_default = "Función lista_areas_permitidas_usuario_gestion_organigrama_default  Imposible conectar la base de datos del gestor documental " & Result
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
                lista_areas_permitidas_usuario_gestion_organigrama_default = "YES"
                Exit Function
            Else
                lista_areas_permitidas_usuario_gestion_organigrama_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_areas_permitidas_usuario_gestion_organigrama_default = "Inconsistencia General Funcion lista_areas_permitidas_usuario_gestion_organigrama_default  : " & ex.Message
        End Try
    End Function
    
   
    Function Lista_AreasDep_Organigrama_Series_Items(ByVal Id_Organigrama As UInteger, _
                                               ByRef refcombo As DropDownList) As String
        Try

            Dim Parametro_Consulta As String = "Select Codigo_Area,Nombre_Area from areas_depart_radicacion where " & _
            " Registro_Organigrama_Id_Organigrama=" & Id_Organigrama
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_AreasDep_Organigrama_Series_Items = "Función Lista_AreasDep_Organigrama_Series_Items  Imposible conectar la base de datos del gestor documental " & Result
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
                Lista_AreasDep_Organigrama_Series_Items = "YES"
            Else
                Lista_AreasDep_Organigrama_Series_Items = "YES"
            End If

        Catch ex As Exception
            Lista_AreasDep_Organigrama_Series_Items = "Inconsistencia general función Lista_AreasDep_Organigrama_Series_Items " & ex.Message
        End Try

    End Function
    
   
    Function Retorna_nombre_area_por_id(ByVal id_area_departamento As Integer, _
                                        ByRef nombre_area As String) As String
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_AREA " & _
              " from AREAS_DEPART_RADICACION where CODIGO_AREA=" & id_area_departamento
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_area_por_id = "Función Retorna_nombre_area_por_id  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_area_por_id = "YES"
            Else
                Retorna_nombre_area_por_id = "Imposible encontrar el nombre del área (" & id_area_departamento & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_area_por_id = "Inconsistencia general función Retorna_nombre_area_por_id " & ex.Message
        End Try
    End Function
   
    
    Function Retorna_datos_usuario_gestion_login_por_id( _
        ByVal id_usuario_gestion As Integer, ByRef login_usuario_workflow As String, _
        ByRef id_usuario_workflow As Integer, ByRef login_usuario_docuarchi As String, _
        ByRef id_usuario_docuarchi As Integer, ByRef login_usuario_radicacion As String, _
        ByRef id_usuario_radicacion As Integer) As String
        '-------------------------------------------------------------
        'Función : Retorna datos usuario gestión con las relaciones
        'de los distintos modulos del gestor documental
        'Fecha : 2015-04-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Remit_Dest_Int,Relacion_Workflow_login,Relacion_Workflow," & _
                "Relacion_Da_login,RELACION_DA,RELACION_LOGIN_RADICACION,RELACION_ID_USUARIO_RADICACION " & _
                " from remit_dest_interno where " & _
               " id_Remit_Dest_Int ='" & id_usuario_gestion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_usuario_gestion_login_por_id = " Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item("id_Remit_Dest_Int")
                If Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow_login") = "No asignado" Then
                    login_usuario_workflow = ""
                Else
                    login_usuario_workflow = Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow_login")
                End If
                id_usuario_workflow = Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow")
                If Dat_reader.Tables(0).Rows(0).Item("Relacion_Da_login") = "No asignado" Then
                    login_usuario_docuarchi = ""
                Else
                    login_usuario_docuarchi = Dat_reader.Tables(0).Rows(0).Item("Relacion_Da_login")
                End If
                id_usuario_docuarchi = Dat_reader.Tables(0).Rows(0).Item("RELACION_DA")

                If Dat_reader.Tables(0).Rows(0).Item("RELACION_LOGIN_RADICACION") = "No asignado" Then
                    login_usuario_radicacion = ""
                Else
                    login_usuario_radicacion = Dat_reader.Tables(0).Rows(0).Item("RELACION_LOGIN_RADICACION")
                End If
                id_usuario_radicacion = Dat_reader.Tables(0).Rows(0).Item("RELACION_ID_USUARIO_RADICACION")
                Retorna_datos_usuario_gestion_login_por_id = "YES"
                Exit Function
            Else
                Retorna_datos_usuario_gestion_login_por_id = "Imposible encontrar el perfil del usuario de gestión "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_usuario_gestion_login_por_id = "Inconsistencia función Retorna_datos_usuario_gestion_login_por_id " & ex.Message
        End Try
    End Function
    Function SolicitaDatosUsuarioGestionLogin(ByVal login_usuario_gestion As String,
        ByRef id_usuario_gestion As Integer, ByRef login_usuario_workflow As String,
        ByRef id_usuario_workflow As Integer, ByRef login_usuario_docuarchi As String,
        ByRef id_usuario_docuarchi As Integer, ByRef login_usuario_radicacion As String,
        ByRef id_usuario_radicacion As Integer) As String
        '-------------------------------------------------------------
        'Función : Retorna datos usuario gestión con las relaciones
        'de los distintos modulos del gestor documental
        'Fecha : 2015-04-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Remit_Dest_Int,Relacion_Workflow_login,Relacion_Workflow," &
                "Relacion_Da_login,RELACION_DA,RELACION_LOGIN_RADICACION,RELACION_ID_USUARIO_RADICACION " &
                " from remit_dest_interno where " &
               " Login_Usuario ='" & login_usuario_gestion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                SolicitaDatosUsuarioGestionLogin = " Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item("id_Remit_Dest_Int")
                If Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow_login") = "No asignado" Then
                    login_usuario_workflow = ""
                Else
                    login_usuario_workflow = Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow_login")
                End If
                id_usuario_workflow = Dat_reader.Tables(0).Rows(0).Item("Relacion_Workflow")
                If Dat_reader.Tables(0).Rows(0).Item("Relacion_Da_login") = "No asignado" Then
                    login_usuario_docuarchi = ""
                Else
                    login_usuario_docuarchi = Dat_reader.Tables(0).Rows(0).Item("Relacion_Da_login")
                End If
                id_usuario_docuarchi = Dat_reader.Tables(0).Rows(0).Item("RELACION_DA")

                If Dat_reader.Tables(0).Rows(0).Item("RELACION_LOGIN_RADICACION") = "No asignado" Then
                    login_usuario_radicacion = ""
                Else
                    login_usuario_radicacion = Dat_reader.Tables(0).Rows(0).Item("RELACION_LOGIN_RADICACION")
                End If
                id_usuario_radicacion = Dat_reader.Tables(0).Rows(0).Item("RELACION_ID_USUARIO_RADICACION")
                SolicitaDatosUsuarioGestionLogin = "YES"
                Exit Function
            Else
                SolicitaDatosUsuarioGestionLogin = "Imposible encontrar el perfil del usuario de gestión "
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosUsuarioGestionLogin = "Inconsistencia función SolicitaDatosUsuarioGestionLogin " & ex.Message
        End Try
    End Function
    
    Function Retorna_numero_radicado_inventario(ByVal id_inventario As Integer, _
                                                ByRef radicado As String)
        '*****************************************************************************
        'Funcion : retorna le numero de radicado
        'Fecha 2015-02-24
        'Ing :Miguel Angel Urueta Miranda
        'Modificado para la versión web por el ingeniero Miguel Angel Urueta Miranda
        'Fecha Modificación 2017-04-20
        '*****************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select RADICADO_DOCUMENTO from registro_producion_documental " & _
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_inventario
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_numero_radicado_inventario = " Error consultando radicado conexión  falta table registro_producion_documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_numero_radicado_inventario = "YES"
                radicado = ""
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    radicado = Datset.Tables(0).Rows(0).Item(0)
                Else
                    radicado = ""
                End If
                Retorna_numero_radicado_inventario = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_numero_radicado_inventario = "Inconsistencia función Retorna_numero_radicado_inventario " & ex.Message
        End Try

    End Function
End Class
