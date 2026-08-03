Public Structure empresa_gestion_documental
    Dim ID_EMPRESA As Integer
    Dim NIT_EMPRESA As String
    Dim RAZON_SOCIAL_EMPRESA As String
    Dim DIRECCION_EMPRESA As String
    Dim REPLEGAL_EMPRESA As String
    Dim CODIGO_ORGANICO_ACTIVO As Integer
    Dim FECHA_CREACION As String
    Dim TELEFONOS_EMPRESA As String
    Dim ESTADO_EMPRESA As Integer
    Dim CODIGO_CAMARA As String
    Dim VALIDA_VISOR_EXPRES As String
End Structure
Public Class Class_empresa_gestion_documental

    Function Solicita_nombre_identificacion_empresa(ByRef nit_empresa As String,
                                                    ByRef nombre_empresa As String) As String
        Try
            Dim sql_consulta As String = "SELECT NIT_EMPRESA,RAZON_SOCIAL_EMPRESA  from empresa_gestion_documental " &
                     " where ESTADO_EMPRESA=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_identificacion_empresa = "Error función Solicita_nombre_identificacion_empresa " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nit_empresa = Datset.Tables(0).Rows(0).Item(0)
                nombre_empresa = Datset.Tables(0).Rows(0).Item(1)
                Solicita_nombre_identificacion_empresa = "YES"
                Exit Function
            Else
                Solicita_nombre_identificacion_empresa = "Imposible encontrar registro de la empresa"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_identificacion_empresa = "Inconsistencia general función Solicita_nombre_identificacion_empresa " & ex.Message
        End Try
    End Function
    Function Solicita_id_empresa_gestion(ByRef id_empresa As Integer) As String
        Try
            Dim sql_consulta As String = "SELECT ID_EMPRESA  from empresa_gestion_documental " &
                   " where ESTADO_EMPRESA=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_empresa_gestion = "Error función Solicita_id_empresa_gestion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_empresa = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_empresa_gestion = "YES"
                Exit Function
            Else
                Solicita_id_empresa_gestion = "Imposible encontrar registro de la empresa de gestión"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_empresa_gestion = "Inconsitencia general función Solicita_id_empresa_gestion " & ex.Message
        End Try
    End Function
    Function Listar_Empresa_de_Gestion_Activa(ByRef Combo As DropDownList) As String
        '******************************************************
        'Funcion : Lista la empresas de gestion en un combobox
        'Fecha : 2013-10-04
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  RAZON_SOCIAL_EMPRESA " &
                  " from EMPRESA_GESTION_DOCUMENTAL where ESTADO_EMPRESA=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Listar_Empresa_de_Gestion_Activa = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                'update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                'update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            Else
                Combo.Items.Clear()
                'update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            End If

        Catch ex As Exception
            Listar_Empresa_de_Gestion_Activa = "Inconsistencia General Funcion Listar_Empresa_de_Gestion_Activa " & ex.Message
        End Try
    End Function
    Function Listar_Empresa_de_Gestion_Activa(ByRef Combo As DropDownList,
                                              ByRef update As UpdatePanel,
                                              ByVal id_usuario_gestion As Integer) As String
        '******************************************************
        'Funcion : Lista la empresas de gestion en un combobox
        'Fecha : 2013-10-04
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT egd.ID_EMPRESA,egd.RAZON_SOCIAL_EMPRESA FROM remit_dest_interno as rdi " &
             " inner join  empresa_gestion_documental as egd on (rdi.Empresa_Gestion_Documental_id_empresa=egd.ID_EMPRESA and egd.ESTADO_EMPRESA=1) " &
             " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Listar_Empresa_de_Gestion_Activa = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(1).ToString)
                Next
                update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            Else
                Combo.Items.Clear()
                update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            End If

        Catch ex As Exception
            Listar_Empresa_de_Gestion_Activa = "Inconsistencia General Funcion Listar_Empresa_de_Gestion_Activa " & ex.Message
        End Try
    End Function
    Function Solicita_listado_empresa(ByVal id_empresa As Integer,
                                      ByRef refcombo As DropDownList,
                                      ByRef update As UpdatePanel) As String
        Try

            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select  ID_EMPRESA,RAZON_SOCIAL_EMPRESA " &
                  " from EMPRESA_GESTION_DOCUMENTAL where ESTADO_EMPRESA=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_empresa = "Función Solicita_listado_empresa dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim ilis As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis = New System.Web.UI.WebControls.ListItem
                    ilis.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilis)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_empresa Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Solicita_listado_empresa = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Solicita_listado_empresa = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_empresa = "Inconsistencia General Funcion Solicita_listado_empresa " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Solicita_nombre_empresa_por_id(ByVal id_empresa As Integer,
                                            ByRef nombre_empresa As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select RAZON_SOCIAL_EMPRESA " &
                  " from EMPRESA_GESTION_DOCUMENTAL where ID_EMPRESA=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_empresa_por_id = "Función Solicita_nombre_empresa_por_id dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_empresa = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_empresa_por_id = "YES"
                Exit Function
            Else
                Solicita_nombre_empresa_por_id = "Imposible encontrar el nombre de la empresa con el id (" & id_empresa & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_empresa_por_id = "Inconsistencia general funcion Solicita_nombre_empresa_por_id " & ex.Message
        End Try
    End Function
    Function Solicita_detalle_empresa_gestion_radicacion(ByVal id_empresa As Integer,
                                                         ByRef _Plantilla_Impresion() As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Parametro_Consulta As String = "select RAZON_SOCIAL_EMPRESA,NIT_EMPRESA,DIRECCION_EMPRESA from empresa_gestion_documental " &
            " where ID_EMPRESA=" & id_empresa
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_detalle_empresa_gestion_radicacion = " Error función Solicita_detalle_empresa_gestion_radicacion " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_detalle_empresa_gestion_radicacion = "Imposible encontrar entidad radicadora"
                Exit Function
            Else
                For i As Integer = 0 To datset.Tables(0).Columns.Count - 1
                    ReDim Preserve _Plantilla_Impresion(i)
                    _Plantilla_Impresion(i) = datset.Tables(0).Rows(0).Item(i)
                Next
                Solicita_detalle_empresa_gestion_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_detalle_empresa_gestion_radicacion = "Inconsistencia general función Solicita_detalle_empresa_gestion_radicacion " & ex.Message
        End Try
    End Function

    Function Solicita_estructura_empresa_gestion(ByVal id_empresa As Integer,
                                                 ByRef empresa_gestion_documental As empresa_gestion_documental) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita_estructura_empresa_gestion
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_empresa            : Representa el identificador de la empresa de gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'empresa_gestion_documental : Retorna estructura empresa de gestión
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Parametro_Consulta As String = "select ID_EMPRESA,RAZON_SOCIAL_EMPRESA,DIRECCION_EMPRESA,REPLEGAL_EMPRESA,CODIGO_ORGANICO_ACTIVO," &
                "FECHA_CREACION,TELEFONOS_EMPRESA,ESTADO_EMPRESA,CODIGO_CAMARA,VALIDA_VISOR_EXPRES" &
            " from empresa_gestion_documental where ID_EMPRESA=" & id_empresa
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_estructura_empresa_gestion = " Error función Solicita_estructura_empresa_gestion " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_empresa_gestion = "Imposible encontrar los datos de caracterización de la empresa de gestión (" & id_empresa & ")"
                Exit Function
            Else
                If datset.Tables(0).Rows(0).IsNull(9) Then
                    empresa_gestion_documental.VALIDA_VISOR_EXPRES = "0"
                Else
                    empresa_gestion_documental.VALIDA_VISOR_EXPRES = datset.Tables(0).Rows(0).Item(9)
                End If
                If datset.Tables(0).Rows(0).IsNull(8) Then
                    empresa_gestion_documental.CODIGO_CAMARA = "0"
                Else
                    empresa_gestion_documental.CODIGO_CAMARA = datset.Tables(0).Rows(0).Item(8)
                End If
                If datset.Tables(0).Rows(0).IsNull(7) Then
                    empresa_gestion_documental.ESTADO_EMPRESA = 0
                Else
                    empresa_gestion_documental.ESTADO_EMPRESA = datset.Tables(0).Rows(0).Item(7)
                End If
                If datset.Tables(0).Rows(0).IsNull(6) Then
                    empresa_gestion_documental.TELEFONOS_EMPRESA = ""
                Else
                    empresa_gestion_documental.TELEFONOS_EMPRESA = datset.Tables(0).Rows(0).Item(6)
                End If
                If datset.Tables(0).Rows(0).IsNull(5) Then
                    empresa_gestion_documental.FECHA_CREACION = ""
                Else
                    empresa_gestion_documental.FECHA_CREACION = datset.Tables(0).Rows(0).Item(5)
                End If
                If datset.Tables(0).Rows(0).IsNull(4) Then
                    empresa_gestion_documental.CODIGO_ORGANICO_ACTIVO = 0
                Else
                    empresa_gestion_documental.CODIGO_ORGANICO_ACTIVO = datset.Tables(0).Rows(0).Item(4)
                End If
                If datset.Tables(0).Rows(0).IsNull(3) Then
                    empresa_gestion_documental.REPLEGAL_EMPRESA = ""
                Else
                    empresa_gestion_documental.REPLEGAL_EMPRESA = datset.Tables(0).Rows(0).Item(3)
                End If
                If datset.Tables(0).Rows(0).IsNull(2) Then
                    empresa_gestion_documental.DIRECCION_EMPRESA = ""
                Else
                    empresa_gestion_documental.DIRECCION_EMPRESA = datset.Tables(0).Rows(0).Item(2)
                End If
                If datset.Tables(0).Rows(0).IsNull(1) Then
                    empresa_gestion_documental.RAZON_SOCIAL_EMPRESA = ""
                Else
                    empresa_gestion_documental.RAZON_SOCIAL_EMPRESA = datset.Tables(0).Rows(0).Item(1)
                End If
                If datset.Tables(0).Rows(0).IsNull(0) Then
                    empresa_gestion_documental.ID_EMPRESA = 0
                Else
                    empresa_gestion_documental.ID_EMPRESA = datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_estructura_empresa_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_empresa_gestion = "Inconsistencia general función  Solicita_estructura_empresa_gestion " & ex.Message
        End Try
    End Function
End Class
