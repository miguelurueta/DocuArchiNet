Imports MySql.Data.MySqlClient
Public Class Class_remit_dest_interno
    Function SolicitaAutoCompleteDestinatarioRestriccion(ByVal NameDbsAuto As String,
                                                         ByVal NombrePlantilla As String,
                                                         ByVal NameCampoNombre As String,
                                                         ByVal NameCampoPrimary As String,
                                                         ByVal ValueAuto As String,
                                                         ByVal IdTipoRestriccion As Integer,
                                                         ByVal IdRestriccion As Integer,
                                                         ByVal IdUsuarioGestionRadicado As Integer,
                                                         ByRef Country As List(Of class_config_gneral_service_row_option_tom_select)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de auto complete usuario destinatario con restriccion
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'NameDbsAuto        : Representa el conector de base de datos
        'NombrePlantilla    : Representa el nombre de la plantilla de validación
        'NameCampoNombre    : Representa el campo nombre de la plantilla
        'NameCampoPrimary   : Representa el campo primary de la plantilla
        'ValueAuto          : Representa el parametro de busqueda
        'IdTipoRestriccion  : Representa el tipo de restriccion
        'IdRestriccion      : Representa la identificación de la restricción
        'IdUsuarioGestionRadicado: Representa el usuario de gestión que radica
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'country            : Retorna la estructura con los datos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ConecDb As Object
            Dim Result As String = ""
            If NameDbsAuto = "WF" Then
                ConecDb = New conect.Dbase_Conction_Mysql
            Else
                ConecDb = New conect.Dbase_Conction_Mysql_RA
            End If
            Dim SQLconsulta As String = ""
            Select Case IdTipoRestriccion
                Case 1
                    SQLconsulta = "Select " & NameCampoPrimary & "," & NameCampoNombre & ",Cargo_Remite" & " from " & NombrePlantilla & " as pdi " &
                    "inner join ra_restri_usuarios_vinculados_restriccion as rrru on (rrru.remit_dest_interno_id_Remit_Dest_Int=pdi." & NameCampoPrimary & ") " &
                    "inner join ra_restri_relacion_tramite as rrrt on (rrrt.ra_restri_dest_interno_IdRestriTipoDestInterno=rrru.ra_restri_dest_interno_IdRestriTipoDestInterno " &
                    "And rrrt.ra_restri_dest_interno_IdRestriTipoDestInterno =" & IdRestriccion & " ) " &
                    " where " & NameCampoNombre & " Like '%" & ValueAuto & "%' LIMIT 50"
                Case 2
                    SQLconsulta = "Select " & NameCampoPrimary & "," & NameCampoNombre & ",Cargo_Remite" & " from " & NombrePlantilla & " as pdi " &
                    "inner join ra_restri_usuarios_vinculados_restriccion as rrru on (rrru.remit_dest_interno_id_Remit_Dest_Int=pdi." & NameCampoPrimary & ") " &
                    "inner join ra_restri_relacion_tramite as rrrt on (rrrt.ra_restri_dest_interno_IdRestriTipoDestInterno=rrru.ra_restri_dest_interno_IdRestriTipoDestInterno " &
                    "And rrrt.ra_restri_dest_interno_IdRestriTipoDestInterno =" & IdRestriccion & " ) " &
                    "inner join ra_restri_relacion_usuarios_vinculados_restricion As rrv On (rrv.ra_restri_usuarios_vinculados_restriccion_idra_restri_usuarios=" &
                    "rrru.idra_restri_usuarios_vinculados And rrv.remit_dest_interno_id_Remit_Dest_Int=" & IdUsuarioGestionRadicado & ") " &
                    " where " & NameCampoNombre & " Like '%" & ValueAuto & "%' LIMIT 50"
                Case Else
                    Return "Tipo restricción no detectada"
            End Select
            If ValueAuto = "" Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                Country.Add(item_)
                Return "no result"
                Exit Function
            End If
            Dim Datset As DataSet = New DataSet(NombrePlantilla)
            Result = ConecDb.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Return Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                Country.Add(item_)
                Return "no result"
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item_ As New class_config_gneral_service_row_option_tom_select
                    item_.id_value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(1) = False Then
                        Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(1).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = Datset.Tables(0).Rows(i).Item(1).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            item_.tex_value = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                        Else
                            item_.tex_value = Datset.Tables(0).Rows(i).Item(1).ToString()
                        End If
                        item_.text_value_descritipo = Datset.Tables(0).Rows(i).Item(2).ToString()
                    End If
                    Country.Add(item_)
                Next
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia funcion SolicitaAutoCompleteDestinatarioRestriccion " & ex.Message
        End Try
    End Function
    Function Solicita_datos_auto_complete_remitente_interno(ByVal name_dbs_auto As String,
                                                            ByVal nombre_plantilla As String,
                                                            ByVal name_campo_nombre As String,
                                                            ByVal name_campo_primary As String,
                                                            ByVal value_auto As String,
                                                            ByRef country As List(Of class_config_gneral_service_row_option_tom_select)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la lista de destinatario interno de plantillas de val
        '          validación para auto complete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_dbs_auto      : Representa el conector de base de datos
        'nombre_plantilla   : Representa el nombre de la plantilla de validación
        'name_campo_nombre  : Representa el campo nombre de la plantilla
        'name_campo_primary : Representa el campo primary de la plantilla
        'value_auto         : Representa el parametro de busqueda
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'country            : Retorna la estructura con los datos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-20
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ref As Object
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select " & name_campo_primary & "," & name_campo_nombre & ",Cargo_Remite" & " from " & nombre_plantilla & " where " & name_campo_nombre & " like '%" & value_auto & "%' LIMIT 50"
            If name_dbs_auto = "WF" Then
                ref = New conect.Dbase_Conction_Mysql
            Else
                ref = New conect.Dbase_Conction_Mysql_RA
            End If
            If value_auto = "" Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                country.Add(item_)
                Solicita_datos_auto_complete_remitente_interno = "no result"
                Exit Function
            End If
            Dim Datset As DataSet = New DataSet("DAT_ADIC")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_remitente_interno = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                country.Add(item_)
                Solicita_datos_auto_complete_remitente_interno = "no result"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item_ As New class_config_gneral_service_row_option_tom_select
                    item_.id_value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(1) = False Then
                        Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(1).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = Datset.Tables(0).Rows(i).Item(1).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            item_.tex_value = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                        Else
                            item_.tex_value = Datset.Tables(0).Rows(i).Item(1).ToString()
                        End If
                        item_.text_value_descritipo = Datset.Tables(0).Rows(i).Item(2).ToString()
                    End If
                    country.Add(item_)
                Next
                Solicita_datos_auto_complete_remitente_interno = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_auto_complete_remitente_interno = "Inconsistencia general funcion Solicita_datos_auto_complete_remitente_interno " & ex.Message
        End Try
    End Function
    Function solicita_datos_respuesta_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                     ByRef Matr_pie() As String) As String
        '******************************************************************
        'Function : Retorna datos de caracterizacion usuario gestión 
        'respuesta
        'Fecha 2016-06-22
        'Ing : Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select Nombre_Remitente,Cargo_Remite,Correo_Electronico " &
               " FROM  remit_dest_interno  " &
               " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                solicita_datos_respuesta_usuario_gestion = "Función retorna_datos_respuesta_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                solicita_datos_respuesta_usuario_gestion = "Imposible encontrar datos de Caracterización usuario gestión"
                Exit Function
            Else

                Erase Matr_pie
                For i As Integer = 0 To 2
                    ReDim Preserve Matr_pie(i)
                    If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                        Matr_pie(i) = "xxxxxxxxxxxxxx"
                    Else
                        Matr_pie(i) = Datset.Tables(0).Rows(0).Item(i).ToString
                    End If
                Next
                solicita_datos_respuesta_usuario_gestion = "YES"
            End If

        Catch ex As Exception
            solicita_datos_respuesta_usuario_gestion = "Inconsistencia general funcion solicita_datos_respuesta_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_ciudad_sede_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                  ByRef nombre_ciudad As String) As String
        Try
            Dim Parametro_Consulta As String = "Select se.CIUDAD from remit_dest_interno as rdi " &
                " inner join sedes_empresa as se on (se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA)" &
                " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_ciudad_sede_usuario_gestion = "Funcion  Solicita_ciudad_sede_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_ciudad = ""
                Solicita_ciudad_sede_usuario_gestion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_ciudad = ""
                    Solicita_ciudad_sede_usuario_gestion = "YES"
                    Exit Function
                Else
                    nombre_ciudad = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_ciudad_sede_usuario_gestion = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_ciudad_sede_usuario_gestion = "Inconsistencia función Solicita_ciudad_sede_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_id_area_departamento_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                           ByRef id_area As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Areas_Dep_Radicacion_id_Areas_Dep from remit_dest_interno  " &
                " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_area_departamento_usuario_gestion = "Funcion  Solicita_ciudad_sede_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_area = 0
                Solicita_id_area_departamento_usuario_gestion = "Imposible encontrar la identificación del área del usuario de gestión (" & id_usuario_gestion & ")"
                Exit Function
            Else
                id_area = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_area_departamento_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_area_departamento_usuario_gestion = "Inconsistencia función Solicita_id_area_departamento_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_cargo_destinatario_interno(ByVal id_usuario_gestion As Integer,
                                                       ByRef nombre_usuario_gestion As String,
                                                       ByRef cargo_usuario_gestion As String) As String
        '*****************************************************************
        'Funcion : Retorna el nombre y el cargo del usuario de gestion
        'en la forma de presentacion valida para el sistema
        'Fecha : 2015-04-30
        'Ingemiero : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim sqlconsulta As String = "Select Nombre_Remitente,Cargo_Remite from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_set)
            If Result <> "YES" Then
                Retorna_nombre_cargo_destinatario_interno = " función Retorna_nombre_cargo_destinatario_interno Error:   " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count > 0 Then
                nombre_usuario_gestion = Dat_set.Tables(0).Rows(0).Item(0).ToString
                cargo_usuario_gestion = Dat_set.Tables(0).Rows(0).Item(1).ToString
                Retorna_nombre_cargo_destinatario_interno = "YES"
                Exit Function
            Else
                Retorna_nombre_cargo_destinatario_interno = "Función Retorna_nombre_cargo_destinatario_interno dice : Imposible encontrar el usuario de gestión o destinatario "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_cargo_destinatario_interno = "Inconsistencia función Retorna_nombre_cargo_destinatario_interno " & ex.Message
        End Try
    End Function
    Function Retorna_identificacion_destinatario_interno(ByVal id_usuario_gestion As Integer,
                                                         ByRef identificacion As String) As String
        '*****************************************************************
        'Funcion : Retorna identificacion usuario de gestion
        'en la forma de presentacion valida para el sistema
        'Fecha : 2019-04-15
        'Ingemiero : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim sqlconsulta As String = "Select IDENTIFICACION from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_set)
            If Result <> "YES" Then
                Retorna_identificacion_destinatario_interno = " función Retorna_identificacion_destinatario_interno Error:   " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count > 0 Then
                If Dat_set.Tables(0).Rows(0).IsNull(0) = True Then
                    identificacion = "0"
                Else
                    identificacion = Dat_set.Tables(0).Rows(0).Item(0).ToString
                End If

                Retorna_identificacion_destinatario_interno = "YES"
                Exit Function
            Else
                Retorna_identificacion_destinatario_interno = "Usuario de gestión sin nit o identificación, por favor contacte al administrador para actualizar su nit o identificación "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_identificacion_destinatario_interno = "Inconsistencia función Retorna_identificacion_destinatario_interno " & ex.Message
        End Try
    End Function
    Function Solicita_id_area_nombre_area_destinatario(ByVal id_usuario_destinatario As Integer,
                                                       ByRef id_area As Integer,
                                                       ByRef nombre_area As String) As String
        '****************************************************************************
        'Funcion : Retorna id area destinatario con el parametro id destinatario
        'Fecha : 2014-09-01
        'Ingeniero : Miguel Angel Urueta Miranda
        '****************************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select adr.Codigo_Area,adr.nombre_area  from remit_dest_interno  as rdi " &
            " inner join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) " &
            " where id_Remit_Dest_int=" & id_usuario_destinatario
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Datset)
            If Result <> "YES" Then
                Solicita_id_area_nombre_area_destinatario = " Error listado id area destinatario funcion Retorna_id_area_destinario   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_area_nombre_area_destinatario = "Imposible encontrar el area del (remitente o destinatario interno) codigo (" & id_usuario_destinatario & "), por favor asigne una área o contacte a su administrador"
                Exit Function
            Else
                id_area = Datset.Tables(0).Rows(0).Item(0)
                nombre_area = Datset.Tables(0).Rows(0).Item(1)
            End If
            Solicita_id_area_nombre_area_destinatario = "YES"
        Catch ex As Exception
            Solicita_id_area_nombre_area_destinatario = "Inconsistencia funcion Solicita_id_area_nombre_area_destinatario " & ex.Message
        End Try
    End Function
    Function Solicita_correo_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                             ByRef correo_usuario_gestion As String) As String
        '-------------------------------------------
        'Funcion : Retorna el correoelectrnico del
        'usuario de gestion documental con el para
        'metro de id de usuario de gestion
        'Fecha 2016-02-14
        'Ing . Miguel Angel Urueta Miranda
        '-------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT Correo_Electronico" &
                " FROM remit_dest_interno " &
                 " where  id_Remit_Dest_Int='" & id_usuario_gestion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Solicita_correo_usuario_gestion = "Función Retorna_correo_usuario_gestion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    correo_usuario_gestion = ""
                Else
                    correo_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                Solicita_correo_usuario_gestion = "YES"
                Exit Function
            Else
                Solicita_correo_usuario_gestion = "Imposible econtrar correo electrónico del usuario de gestión"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_correo_usuario_gestion = "Inconsistencia general función Solicita_correo_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_workflow_relacionado(ByVal id_usuario_gestion As Integer,
                                                      ByRef id_usuario_workflow As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT Relacion_Workflow" &
               " FROM remit_dest_interno " &
                " where  id_Remit_Dest_Int='" & id_usuario_gestion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Solicita_id_usuario_workflow_relacionado = "Función Solicita_id_usuario_workflow_relacionado " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_workflow = 0
                Else
                    id_usuario_workflow = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_usuario_workflow_relacionado = "YES"
                Exit Function
            Else
                Solicita_id_usuario_workflow_relacionado = "Imposible econtrar el usuario workflow relacionado al usuario de gestión (" & id_usuario_gestion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_usuario_workflow_relacionado = "Inconsistencia general función Solicita_id_usuario_workflow_relacionado"
        End Try
    End Function
    Function Retorna_datos_caracterizacion_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                           ByRef nombre_usuario_gestion As String,
                                                           ByRef cargo_usuario_gestion As String,
                                                           ByRef correo_electronico As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Nombre_Remitente,Cargo_Remite,Correo_Electronico from remit_dest_interno " &
            " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_datos_caracterizacion_usuario_gestion = "función Retorna_datos_caracterizacion_usuario_gestion dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then

                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario_gestion = "NO REPORTA"
                Else
                    nombre_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    cargo_usuario_gestion = "NO REPORTA"
                Else
                    cargo_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = True Then
                    correo_electronico = ""
                Else
                    correo_electronico = Dat_reader.Tables(0).Rows(0).Item(2)
                End If
                Retorna_datos_caracterizacion_usuario_gestion = "YES"
                Exit Function
            Else
                Retorna_datos_caracterizacion_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_caracterizacion_usuario_gestion = "Inconsistencia función Retorna_datos_caracterizacion_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_usuario_areas_departamento(ByVal id_area As Integer,
                                                 ByRef LisRef As DropDownList,
                                                 ByRef up_date As UpdatePanel) As String
        '******************************************************************************
        'Funcion lista usuarios relacionados al usuario al area informada
        'Fecha : 2019-04-10
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************************
        Try

            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select id_Remit_Dest_Int,Nombre_Remitente from remit_dest_interno where " &
            " Areas_Dep_Radicacion_id_Areas_Dep=" & id_area & " and Estado_Usuario=1"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_usuario_areas_departamento = " Error función Solicita_usuario_areas_departamento   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim ilist_ As New ListItem
                    ilist_.Value = Dat_reader.Tables(0).Rows(i).Item(0)
                    ilist_.Text = Dat_reader.Tables(0).Rows(i).Item(1)
                    LisRef.Items.Add(ilist_)
                Next
                Solicita_usuario_areas_departamento = "YES"
                Exit Function
            Else
                Solicita_usuario_areas_departamento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_areas_departamento = "Inconsistencia general función Solicita_usuario_areas_departamento " & ex.Message
        Finally
            up_date.Update()
        End Try

    End Function
    Function Solicita_lista_usuarios_gestion(ByVal id_usuario_gestion As Integer,
                                             ByRef LisRef As DropDownList,
                                             ByRef up_date As UpdatePanel) As String
        Try
            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select id_Remit_Dest_Int,Nombre_Remitente,Cargo_Remite from remit_dest_interno where " &
            " estado_Usuario=1 order by Nombre_Remitente"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_lista_usuarios_gestion = " Error función Solicita_usuario_areas_departamento   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim ilist_ As New ListItem
                    ilist_.Value = Dat_reader.Tables(0).Rows(i).Item(0)
                    ilist_.Text = Dat_reader.Tables(0).Rows(i).Item(1) & "(" & Dat_reader.Tables(0).Rows(i).Item(2) & ")"
                    LisRef.Items.Add(ilist_)
                Next
                For i As Integer = 0 To LisRef.Items.Count - 1
                    If LisRef.Items(i).Value = id_usuario_gestion Then
                        LisRef.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Solicita_lista_usuarios_gestion = "YES"
                Exit Function
            Else
                Solicita_lista_usuarios_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuarios_gestion = "Inconsitencia general funcion Solicita_usuarios_gestion " & ex.Message
        Finally
            up_date.Update()
        End Try
    End Function
    Function Solicita_lista_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                            ByRef LisRef As DropDownList,
                                            ByRef up_date As UpdatePanel) As String
        Try
            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select id_Remit_Dest_Int,Nombre_Remitente,Cargo_Remite from remit_dest_interno where " &
            " estado_Usuario=1 and id_Remit_Dest_Int= " & id_usuario_gestion & " order by Nombre_Remitente"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_lista_usuario_gestion = " Error función Solicita_lista_usuario_gestion   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim ilist_ As New ListItem
                    ilist_.Value = Dat_reader.Tables(0).Rows(i).Item(0)
                    ilist_.Text = Dat_reader.Tables(0).Rows(i).Item(1) & "(" & Dat_reader.Tables(0).Rows(i).Item(2) & ")"
                    LisRef.Items.Add(ilist_)
                Next
                For i As Integer = 0 To LisRef.Items.Count - 1
                    If LisRef.Items(i).Value = id_usuario_gestion Then
                        LisRef.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Solicita_lista_usuario_gestion = "YES"
                Exit Function
            Else
                Solicita_lista_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuario_gestion = "Inconsitencia general funcion Solicita_lista_usuario_gestion " & ex.Message
        Finally
            up_date.Update()
        End Try
    End Function
    Function Solicita_estructura_usuario_gestion_radicacion(ByVal id_usuario_gestion As Integer,
                                                            ByRef text_usuario As String) As String
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select id_Remit_Dest_Int, Login_Usuario, Nombre_Remitente, Cargo_Remite from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Result As String = refcconect.SELECTION_SELECT_FIELD(sqlconsult, datset)
            If Result <> "YES" Then
                Solicita_estructura_usuario_gestion_radicacion = Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                Dim tempo_record As String = "<" & datset.Tables(0).Rows(0).Item(0).ToString() & "> " & datset.Tables(0).Rows(0).Item(2).ToString() & " (" & datset.Tables(0).Rows(0).Item(3).ToString() & ")"
                tempo_record = tempo_record.Replace(",", "")
                text_usuario = tempo_record
                Solicita_estructura_usuario_gestion_radicacion = "YES"
                Exit Function
            Else
                Solicita_estructura_usuario_gestion_radicacion = "Imposible encontrar los datos del usuario de gestión (" & id_usuario_gestion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_usuario_gestion_radicacion = "Inconsistencia general función Solicita_estructura_usuario_gestion_radicacion " & ex.Message
        End Try
    End Function
    Function Retorna_Id_Destinatario(ByVal Nombre_Remitente As String,
                                     ByRef Cod_Remitente As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select * from remit_dest_interno where Nombre_Remitente = '" &
            Nombre_Remitente & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Destinatario = " Error listado id destinatario   " & Result
                Return Retorna_Id_Destinatario
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Id_Destinatario = "Imposible encontrar el id destinatario "
                Exit Function
            Else
                Cod_Remitente = Datset.Tables(0).Rows(0).Item(0).ToString
                Retorna_Id_Destinatario = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Id_Destinatario = "Inconsistencia general funcion Retorna_Id_Destinatario " & ex.Message
        End Try
    End Function
    Function Retorna_id_empresa_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                ByRef id_empresa As Integer) As String
        '*****************************************************************
        'Funcion : Retorna el id empresa  del usuario de gestion
        'Fecha : 2020-08-03
        'Ingemiero : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim sqlconsulta As String = "Select Empresa_Gestion_Documental_id_empresa from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_set)
            If Result <> "YES" Then
                Retorna_id_empresa_usuario_gestion = " función Retorna_id_empresa_usuario_gestion Error:   " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count > 0 Then
                id_empresa = Dat_set.Tables(0).Rows(0).Item(0).ToString
                Retorna_id_empresa_usuario_gestion = "YES"
                Exit Function
            Else
                Retorna_id_empresa_usuario_gestion = "Función Retorna_id_empresa_usuario_gestion dice : Imposible encontrar empresa usuario gestión "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_empresa_usuario_gestion = "Inconsistencia función Retorna_nombre_cargo_destinatario_interno " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_gestion_internos_por_area_auxiliar(ByVal id_organigrama As Integer,
                                                               ByVal nombre_area As String,
                                                               ByRef grediview As GridView,
                                                               ByVal id_empresa As Integer,
                                                               ByVal colum_order_name As String,
                                                               ByVal tipo_consulta As Integer,
                                                               ByVal valor_consulta As String,
                                                               ByVal order_colum As String) As String
        '********************************************************************************
        'Funcion : Lista usuarios por areas de organigrama de la gestion documental
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-04-23
        '********************************************************************************
        Try
            '---------------------------------------------------------
            'Retorna id_area seleccionada
            '---------------------------------------------------------
            Dim update As UpdatePanel = grediview.Page.FindControl("UpdatePanel_auxiliar_destinatarios_internos_popup")
            If update Is Nothing Then
                Lista_usuarios_gestion_internos_por_area_auxiliar = "Imposible encontrar el control  UpdatePanel_auxiliar_destinatarios_internos_popup"
                Exit Function
            End If
            Dim hideselecion As Object = grediview.Page.FindControl("Hidden_destinatario_interno")
            If hideselecion Is Nothing Then
                Lista_usuarios_gestion_internos_por_area_auxiliar = "Imposible encontrar el control  Hidden_destinatario_interno"
                Exit Function
            End If
            If colum_order_name = "" Then
                colum_order_name = "Nombre_Remitente"
            End If
            Dim Result As String = ""
            Dim sqlconsulta As String = ""
            Dim sql_condicion As String = ""
            If nombre_area = "TODAS LAS AREAS" Or nombre_area = "SELECCIONE" Then
                If tipo_consulta = 1 Then
                    sql_condicion = " where rdi.Empresa_Gestion_Documental_id_empresa=" & id_empresa
                End If
                If tipo_consulta = 2 Then
                    sql_condicion = " where (Nombre_Remitente like '%" & valor_consulta & "%'" &
                        " or Cargo_Remite like '%" & valor_consulta & "%'" &
                        " or Correo_Electronico like '%" & valor_consulta & "%') and " &
                        " rdi.Empresa_Gestion_Documental_id_empresa=" & id_empresa
                End If
                sqlconsulta = UCase("Select id_Remit_Dest_Int,Nombre_Remitente as Nombre,Cargo_Remite as Cargo,adr.Nombre_Area,Correo_Electronico," &
                "se.NOMBRE_SEDE as Nombre_Sede,se.TELEFONOS_SEDE as Telefono_Sede from remit_dest_interno as rdi") &
                " left outer  join sedes_empresa as se on (se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA)" &
                " left outer join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) " &
                sql_condicion &
                " and Estado_Usuario=1 order by " & colum_order_name & " " & order_colum
            Else
                Dim Id_area_usuario_gestion As Integer = -1
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = ref_Class_areas_depart_radicacion.Retorna_id_area_usuario_gestion(id_organigrama,
                                                                                            nombre_area,
                                                                                            Id_area_usuario_gestion)
                If Result <> "YES" Then
                    Lista_usuarios_gestion_internos_por_area_auxiliar = Result & " Por favor seleccione el Area o dependencia "
                    Exit Function
                End If
                If tipo_consulta = 1 Then
                    sql_condicion = " where Areas_Dep_Radicacion_id_Areas_Dep=" & Id_area_usuario_gestion
                End If
                If tipo_consulta = 2 Then
                    sql_condicion = " where (Nombre_Remitente like '%" & valor_consulta & "%'" &
                        " or Cargo_Remite like '%" & valor_consulta & "%'" &
                        " or Correo_Electronico like '%" & valor_consulta & "%') and " &
                        " Areas_Dep_Radicacion_id_Areas_Dep=" & Id_area_usuario_gestion
                End If
                sqlconsulta = UCase("Select id_Remit_Dest_Int,Nombre_Remitente as Nombre,Cargo_Remite as Cargo,adr.Nombre_Area,Correo_Electronico," &
                    "se.NOMBRE_SEDE,se.TELEFONOS_SEDE from remit_dest_interno as rdi ") &
                    " left outer join sedes_empresa as se on (se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA)" &
                    " left outer join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) " &
                    sql_condicion &
                    " and Estado_Usuario=1 order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_remit_interno") = {"OPCIONES", "ID_REMIT_DEST_INT",
                                                                               "NOMBRE", "CARGO",
                                                                               "NOMBRE_SEDE", "TELEFONOS_SEDE"}
            HttpContext.Current.Session.Item("SortExpression_interno_remit") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_interno_remit") = order_colum
            HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_INTERNO_REMIT") = tipo_consulta
            HttpContext.Current.Session.Item("RA_DATO_CONSULTA_INTERNO_REMIT") = sqlconsulta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_set)
            If Result <> "YES" Then
                Lista_usuarios_gestion_internos_por_area_auxiliar = " Error Listando usuarios gestion   " & Result
                Exit Function
            End If

            If Dat_set.Tables(0).Rows.Count > 0 Then
                grediview.DataSource = Dat_set
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-arrow-square-down fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Asigna destinatario")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "asig_dest_0002")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")

                        End If

                    Next
                Next
                Lista_usuarios_gestion_internos_por_area_auxiliar = "YES"
                Exit Function
            Else
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_remit_interno"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Lista_usuarios_gestion_internos_por_area_auxiliar = "Error add clase funcion  LLista_usuarios_gestion_internos_por_area_auxiliar " & Result
                    Exit Function
                End If
                Lista_usuarios_gestion_internos_por_area_auxiliar = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_gestion_internos_por_area_auxiliar = "Inconsistencia general Lista_usuarios_gestion_internos_por_area_auxiliar " & ex.Message
        End Try
    End Function
    Function Verifica_nombre_usuario_gestion(ByVal logion_usuario As String,
                                            ByRef existencia_loguin As String) As String
        Try
            Dim Parametro_Consulta As String = "select Nombre_Remitente from remit_dest_interno " &
                " where  Login_Usuario ='" & logion_usuario & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_nombre_usuario_gestion = "Error función Verifica_nombre_usuario_gestion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_loguin = "YES"
                Verifica_nombre_usuario_gestion = "YES"
                Exit Function
            Else
                existencia_loguin = "NO"
                Verifica_nombre_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_nombre_usuario_gestion = "Inconsistencia general función Verifica_nombre_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_detalle_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                              ByRef nombre_usuario_radicador As String,
                                              ByRef cargo_usuario_radicador As String,
                                              ByRef sede_empresa As String) As String
        Try
            Dim Parametro_Consulta As String = "SELECT rdi.Nombre_Remitente, rdi.Cargo_Remite, se.NOMBRE_SEDE FROM  remit_dest_interno as rdi " &
            " inner join sedes_empresa as se on (se.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=rdi.Empresa_Gestion_Documental_id_empresa and  se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA) " &
            " where rdi.id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_detalle_usuario_gestion = " Error funcion Solicita_caraterizacion_usuario_radicador_gestion   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_detalle_usuario_gestion = "Imposible encontrar id sede usuario radicador (" & id_usuario_gestion & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario_radicador = ""
                Else
                    nombre_usuario_radicador = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    cargo_usuario_radicador = ""
                Else
                    cargo_usuario_radicador = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    sede_empresa = ""
                Else
                    sede_empresa = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_detalle_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_detalle_usuario_gestion = "Inconsistencia general función Solicita_detalle_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_identificacion_area_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                          ByRef identificacion_area As Integer) As String

        Try
            Dim Parametro_Consulta As String = "select rdi.Areas_Dep_Radicacion_id_Areas_Dep " &
                                       "from remit_dest_interno as rdi " &
                                       " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_identificacion_area_usuario_gestion = "Función Solicita_identificacion_area_usuario_gestion dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_area_usuario_gestion = "Imposible encontrar los datos de caracterización del usuario de gestión"
                Exit Function
            Else
                If datset.Tables(0).Rows(0).IsNull(0) Then
                    identificacion_area = 0
                Else
                    identificacion_area = datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_identificacion_area_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_area_usuario_gestion = "Inconsistencia general función Solicita_identificacion_area_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_datos_de_caracterizacion_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                               ByRef nombre_usuario_gestion As String,
                                                               ByRef correo_electronico As String,
                                                               ByRef telefono As String,
                                                               ByRef identificacion As String,
                                                               ByRef direccion As String) As String
        '----------------------------------------------------
        'Función : Solicita los datos de caractarización
        'del usuario de gestión con el parametro identi
        'ficador
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-12-01
        '----------------------------------------------------
        Try
            Dim Parametro_Consulta = "select Nombre_Remitente,Correo_Electronico,TELEFONO_USUARIO,IDENTIFICACION,DIRECCION" &
           " from remit_dest_interno where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("campos_plantilla_validacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_de_caracterizacion_usuario_gestion = "Funcion  Solicita_datos_de_caracterizacion_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_de_caracterizacion_usuario_gestion = "Imposible encontrar los datos de caracterización del usuario de gestión (" & id_usuario_gestion & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario_gestion = ""
                Else
                    nombre_usuario_gestion = Trim(Datset.Tables(0).Rows(0).Item(0))
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    correo_electronico = ""
                Else
                    correo_electronico = Trim(Datset.Tables(0).Rows(0).Item(1))
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    telefono = ""
                Else
                    telefono = Trim(Datset.Tables(0).Rows(0).Item(2))
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    identificacion = ""
                Else
                    identificacion = Trim(Datset.Tables(0).Rows(0).Item(3))
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    direccion = ""
                Else
                    direccion = Trim(Datset.Tables(0).Rows(0).Item(4))
                End If
                Solicita_datos_de_caracterizacion_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_de_caracterizacion_usuario_gestion = "Inconsistencia general función Solicita_datos_de_caracterizacion_usuario_gestion " & ex.Message
        End Try
    End Function
End Class
