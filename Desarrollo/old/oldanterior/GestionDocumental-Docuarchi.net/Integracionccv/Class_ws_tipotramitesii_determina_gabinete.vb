Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Public Class Class_ws_tipotramitesii_determina_gabinete_service
    Property Error_gestion As String
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
End Class
Public Structure stru_tipos_tramite_sii
    Dim id_tipotramiteSII As Integer
    Dim nombre_tramite As String
    Dim tipo_gabinete As String
    Dim descripcion_tramite As String
    Dim flujo_trabajo As String
End Structure
Public Class Class_ws_tipotramitesii_determina_gabinete
    Function Solicita_estructura_tramite_SII(ByVal id_tipotramiteSII As Integer,
                                             ByRef stru_tipos_tramite_sii As stru_tipos_tramite_sii) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de la estructura de una tramite SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_tipotramiteSII   : Representa la identificación del tramite SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_tipos_tramite_sii  : Retorna la estructura del tramite SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select id_tipotramiteSII,nombre_tramite,tipo_gabinete,descripcion_tramite,flujo_trabajo " &
                " from ws_tipotramitesii_determina_gabinete " &
                " where id_tipotramiteSII=" & id_tipotramiteSII
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_tramite_SII = "Imposible encontar el detalle del tipo tramite SII (" & id_tipotramiteSII & ") en la tabla (ws_tipotramitesii_determina_gabinete)"
                Exit Function
            Else
                stru_tipos_tramite_sii.id_tipotramiteSII = Datset.Tables(0).Rows(0).Item(0)
                stru_tipos_tramite_sii.nombre_tramite = Datset.Tables(0).Rows(0).Item(1)
                stru_tipos_tramite_sii.tipo_gabinete = Datset.Tables(0).Rows(0).Item(2)
                stru_tipos_tramite_sii.descripcion_tramite = Datset.Tables(0).Rows(0).Item(3)
                stru_tipos_tramite_sii.flujo_trabajo = Datset.Tables(0).Rows(0).Item(4)
                Solicita_estructura_tramite_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_tramite_SII = "Inconssitencia general funcion Solicita_estructura_tramite_SII " & ex.Message
        End Try
    End Function
    Function Lista_descripcion_tramites_flujo_trabajo(ByRef stru_tipos_tramite_sii() As stru_tipos_tramite_sii) As String
        Try
            Erase stru_tipos_tramite_sii
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select id_tipotramiteSII,nombre_tramite,tipo_gabinete,descripcion_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " order by descripcion_tramite,tipo_registro"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_descripcion_tramites_flujo_trabajo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_descripcion_tramites_flujo_trabajo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_tipos_tramite_sii(i)
                    stru_tipos_tramite_sii(i).id_tipotramiteSII = Datset.Tables(0).Rows(i).Item(0)
                    stru_tipos_tramite_sii(i).nombre_tramite = Datset.Tables(0).Rows(i).Item(1)
                    stru_tipos_tramite_sii(i).tipo_gabinete = Datset.Tables(0).Rows(i).Item(2)
                    stru_tipos_tramite_sii(i).descripcion_tramite = Datset.Tables(0).Rows(i).Item(3)
                Next
                Lista_descripcion_tramites_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_descripcion_tramites_flujo_trabajo = "Inconsistencia general función Lista_descripcion_tramites_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function SolicitaListaTramiteSII(ByVal optio_blank As Integer,
                                     ByVal NombreTramite As String,
                                     ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura de los tipos de tramites del sistema SII con condición
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de la lista de tramites
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select id_tipotramiteSII,nombre_tramite,tipo_gabinete,descripcion_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " where nombre_tramite='" & NombreTramite & "' " &
                " order by descripcion_tramite,tipo_registro"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaListaTramiteSII = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaListaTramiteSII = "No se ha encontrado una configuración válida para el trámite SII (" & NombreTramite & ") . Por favor, contacte al administrador del sistema para registrar o verificar la configuración en la tabla (ws_tipotramitesii_determina_gabinete)."
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If optio_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(3)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0) & "|" & Datset.Tables(0).Rows(i).Item(1)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                SolicitaListaTramiteSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaListaTramiteSII = "Inconistencia general función SolicitaListaTramiteSII " & ex.Message
        End Try
    End Function
    Function Solicita_class_lista_tramites_SII(ByVal optio_blank As Integer,
                                               ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura de los tipos de tramites del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de la lista de tramites
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select id_tipotramiteSII,nombre_tramite,tipo_gabinete,descripcion_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " order by descripcion_tramite,tipo_registro"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_lista_tramites_SII = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_lista_tramites_SII = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If optio_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(3)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0) & "|" & Datset.Tables(0).Rows(i).Item(1)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_class_lista_tramites_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_lista_tramites_SII = "Inconistencia general función  Solicita_class_lista_tramites_SII " & ex.Message
        End Try
    End Function
    Function Solicita_class_lista_tramites_rue_default_SII(ByVal optio_blank As Integer,
                                                           ByVal nombre_tramite As String,
                                                           ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura de los tipos de tramites del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        'nombre_tramite:Representa el nombre del tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de la lista de tramites
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select id_tipotramiteSII,nombre_tramite,tipo_gabinete,descripcion_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " where nombre_tramite='" & nombre_tramite & "'" &
                " order by descripcion_tramite,tipo_registro"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_lista_tramites_rue_default_SII = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_lista_tramites_rue_default_SII = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If optio_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(3)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0) & "|" & Datset.Tables(0).Rows(i).Item(1)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_class_lista_tramites_rue_default_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_lista_tramites_rue_default_SII = "Inconistencia general función  Solicita_class_lista_tramites_SII " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_tramite_codigo_rue(ByVal codigo_rue As String,
                                                ByRef nombre_tramite As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre tramite con el codigo de tramite de rue SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'codigo_rue        : Representa parametro codigo rue servicio RUE SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'nombre_tramite  : Retorna nombre del tramite relacionado en la tabla TIPO_DOC_ENTRANTE
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select nombre_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " where CODIGO_RUE='" & codigo_rue & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_tramite_codigo_rue = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_tramite = ""
                Solicita_nombre_tramite_codigo_rue = "YES"
                Exit Function
            Else
                nombre_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_tramite_codigo_rue = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_tramite_codigo_rue = "Inconsistencia general función Solicita_nombre_tramite_codigo_rue " & ex.Message
        End Try
    End Function
    Function Valida_registro_nombre_tramite_sii(ByVal nombre_tramite_sii As String,
                                                ByRef nombre_tramite As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre tramite con el nombre del tramite del SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombre_tramite_sii        : Representa parametro nombre tramite SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'nombre_tramite  : Retorna nombre del tramite relacionado en la tabla TIPO_DOC_ENTRANTE
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_tipotramitesii_determina_gabinete")
            Dim sql_consulta As String = "Select nombre_tramite " &
                " from ws_tipotramitesii_determina_gabinete " &
                " where nombre_tramite='" & nombre_tramite_sii & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Valida_registro_nombre_tramite_sii = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_tramite = ""
                Valida_registro_nombre_tramite_sii = "YES"
                Exit Function
            Else
                nombre_tramite = Datset.Tables(0).Rows(0).Item(0)
                Valida_registro_nombre_tramite_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Valida_registro_nombre_tramite_sii = "Inconsistencia general función Valida_registro_nombre_tramite_sii " & ex.Message
        End Try
    End Function
    Function Lista_tipos_tramite_interface(ByVal stru_tipos_tramite_sii() As stru_tipos_tramite_sii,
                                           ByRef ref_droplist As DropDownList,
                                           ByRef up_dat As UpdatePanel) As String
        Try
            ref_droplist.Items.Clear()
            If stru_tipos_tramite_sii Is Nothing Then
                Lista_tipos_tramite_interface = "YES"
                Exit Function
            End If
            Dim ilist_ As New ListItem("", "")
            ref_droplist.Items.Add(ilist_)
            For i As Integer = 0 To stru_tipos_tramite_sii.Length - 1
                Dim ilist As New ListItem(stru_tipos_tramite_sii(i).descripcion_tramite, stru_tipos_tramite_sii(i).id_tipotramiteSII & "|" &
                                          stru_tipos_tramite_sii(i).nombre_tramite & "|" & stru_tipos_tramite_sii(i).tipo_gabinete & "|" &
                                          stru_tipos_tramite_sii(i).descripcion_tramite)
                ref_droplist.Items.Add(ilist)
            Next
            Lista_tipos_tramite_interface = "YES"
            Exit Function
        Catch ex As Exception
            Lista_tipos_tramite_interface = "Inconsistencia general función Lista_tipos_tramite_interface " & ex.Message
        Finally
            up_dat.Update()
        End Try
    End Function
End Class
