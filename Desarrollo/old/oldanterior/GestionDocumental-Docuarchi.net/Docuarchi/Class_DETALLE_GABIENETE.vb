Public Structure stru_campo_detalle
    Dim nombre_campo As String
    Dim tipo_campo As String
End Structure

Public Class Class_DETALLE_GABIENETE
    Function Solicita_nombre_campo_valor_gabinete(ByVal gabinete As String,
                                                  ByRef nombre_campo_valor_gabinete As String) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del campo que actua como campo valor para registro
        '        : de version y de migracion
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'gabinete                : Representa el nombre del gabinete del campo valor
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'nombre_campo_valor_gabinete : Representa el nombre del campo valor
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-07-03
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "select  CAMPO  from detalle_gabienete  where GABINETE ='" & gabinete & "' and INDETI_CAMPO_VALOR=1"
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("detalle_gabienete")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_nombre_campo_valor_gabinete = "Error función Solicita_nombre_campo_valor_gabinete  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_campo_valor_gabinete = "Imposible encontrar el campo valor en el gabinete (" & gabinete & ") para el registro de versión"
                Exit Function
            Else
                nombre_campo_valor_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_campo_valor_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_campo_valor_gabinete = "Incosnsitencia general funcion Solicita_nombre_campo_valor_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_campos_dynamic_migracion(ByVal id_gabinete As Integer,
                                                          ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una gabinete para interface de 
        '        : de consulta de documentos para migración y gestion
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_gabinete                : Representa el nombre del gabinete al que pertenece
        '                             la imagen a eliminar
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos de un gabinete 
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
           "DETALLE_GABIENETE " &
           "WHERE id_gabinete='" & id_gabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_estructura_campos_dynamic_migracion = "Funcion  Solicita_estructura_campos_dynamic_migracion dice : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_campos_dynamic_migracion = "Imposible encontrar los campos para gabinete : (" & id_gabinete & ")"
                Exit Function
            Else
                Dim campo_clase_documento As String = ""
                Dim campo_expediente As String = ""
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(y).Item(0) = "TIPODOCUMENTO" Then
                        campo_clase_documento = "TIPODOCUMENTO"

                    End If
                    If Datset.Tables(0).Rows(y).Item(0) = "EXPEDIENTE" Then
                        campo_expediente = "EXPEDIENTE"

                    End If
                Next
                Dim item As New class_campos_table_bostra_table
                item.field = "state"
                item.checkbox = True
                item.visible = True
                item.viisble_sql = 0
                item.visible_like_sql = 0
                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "OPERATION"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 0
                item.clickToSelect = False
                item.visible_like_sql = 0
                item.align = "center"
                item.events = "window.operateEvents"
                item.formatter = "operateFormattertablebootmig"
                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.title = "ID"
                item.field = "da.ID"
                item.visible = False
                item.viisble_sql = 1
                item.visible_like_sql = 0

                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.title = "TIPO"
                item.field = "ESTENSION"
                item.visible = True
                item.viisble_sql = 1
                item.visible_like_sql = 0
                class_campos_table_bostra_table.Add(item)
                If campo_clase_documento <> "" Then
                    item = New class_campos_table_bostra_table
                    item.title = campo_clase_documento
                    item.field = campo_clase_documento
                    item.visible = True
                    item.viisble_sql = 1
                    item.visible_like_sql = 1
                    class_campos_table_bostra_table.Add(item)
                End If
                If campo_expediente <> "" Then
                    item = New class_campos_table_bostra_table
                    item.title = campo_expediente
                    item.field = campo_expediente
                    item.viisble_sql = 1
                    item.visible = True
                    item.visible_like_sql = 1
                    class_campos_table_bostra_table.Add(item)
                End If
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(y).Item(0) <> "TIPODOCUMENTO" And Datset.Tables(0).Rows(y).Item(0) <> "EXPEDIENTE" Then
                        item = New class_campos_table_bostra_table
                        item.title = Datset.Tables(0).Rows(y).Item(0)
                        item.field = Datset.Tables(0).Rows(y).Item(0)
                        item.visible = True
                        item.viisble_sql = 1
                        item.visible_like_sql = 1
                        class_campos_table_bostra_table.Add(item)
                    End If
                Next
                Solicita_estructura_campos_dynamic_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_campos_dynamic_migracion = "Inconsistencia general funcion Solicita_estructura_campos_dynamic_migracion " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraCamposConsultaGabineteBootStra(EstructuraCamposGabinete() As estructura_gabinete,
                                                              ByRef ClassCamposTableBostraTable As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una gabinete para interface de 
        '        : de consulta de documentos para migración y gestion
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'EstructuraCamposGabinete   : Representa la estructura de los campos del gabinete
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'ClassCamposTableBostraTable :Representa la estructura de campos de un gabinete 
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2025-08-26
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            ClassCamposTableBostraTable = New List(Of class_campos_table_bostra_table)
            Dim CampoTlaseDocumento As String = ""
            For i As Integer = 0 To EstructuraCamposGabinete.Length - 1
                If EstructuraCamposGabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    CampoTlaseDocumento = "TIPODOCUMENTO"

                End If
            Next
            Dim item As New class_campos_table_bostra_table
            item.field = "state"
            item.checkbox = True
            item.visible = True
            item.viisble_sql = 0
            item.visible_like_sql = 0
            ClassCamposTableBostraTable.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "OPERATION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEvents"
            item.formatter = "operateFormattertablebootmig"
            ClassCamposTableBostraTable.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "ID"
            item.field = "ID"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 0
            ClassCamposTableBostraTable.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "ESTADO_FIRMA_DIGITAL"
            item.field = "ESTADO_FIRMA_DIGITAL"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 0
            ClassCamposTableBostraTable.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "DBT"
            item.field = "DBT"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 0
            ClassCamposTableBostraTable.Add(item)
            If CampoTlaseDocumento <> "" Then
                item = New class_campos_table_bostra_table
                item.title = CampoTlaseDocumento
                item.field = CampoTlaseDocumento
                item.visible = True
                item.viisble_sql = 1
                item.visible_like_sql = 1
                item.data_sortable = True
                ClassCamposTableBostraTable.Add(item)
            End If
            For y As Integer = 0 To EstructuraCamposGabinete.Length - 1
                If EstructuraCamposGabinete(y).CAMPO <> "TIPODOCUMENTO" And EstructuraCamposGabinete(y).VISIBLE = 1 Then
                    item = New class_campos_table_bostra_table
                    item.title = EstructuraCamposGabinete(y).CAMPO
                    item.field = EstructuraCamposGabinete(y).CAMPO
                    item.visible = True
                    item.viisble_sql = 1
                    item.visible_like_sql = 1
                    item.data_sortable = True
                    item.sortable = True
                    ClassCamposTableBostraTable.Add(item)
                End If
            Next
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaEstructuraCamposConsultaGabineteBootStra " & ex.Message
        End Try
    End Function

    Function SolicitaEstructuraCamposGabinetePorId(ByVal IdGabinete As Integer,
                                                   ByRef EstructuraCamposGabinete() As estructura_gabinete) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una gabinete para interface de 
        '        : de consulta de documentos por identiifcación de gabinete
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'IdGabinete               : Representa la identificación del gabinete
        '                          
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'EstructuraCamposGabinete       :Representa la estructura de campos de un gabinete 
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "SELECT IDENTI,TIPO,CAMPO,VISIBLE,SISTEMA,ESTADO,INFOCAMPO,CAMPO_PUBLICO,CAMPO_UNICO,CAMPO_RADICADO,ALEAS_CAMPO,CAMPO_ENABLE_DISABLE FROM " &
                                      "DETALLE_GABIENETE " &
                                      "WHERE id_gabinete='" & IdGabinete & "' ORDER BY IDENTI"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraCamposGabinetePorId = "Función SolicitaEstructuraCamposGabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstructuraCamposGabinetePorId = "Imposible encontrar los campos para gabinete (" & IdGabinete & ") en la tabla gabinete detalle  "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve EstructuraCamposGabinete(i)
                    EstructuraCamposGabinete(i).IDENTI = Datset.Tables(0).Rows(i).Item(0)
                    EstructuraCamposGabinete(i).TIPO = Datset.Tables(0).Rows(i).Item(1)
                    EstructuraCamposGabinete(i).CAMPO = Datset.Tables(0).Rows(i).Item(2)
                    EstructuraCamposGabinete(i).VISIBLE = Datset.Tables(0).Rows(i).Item(3)
                    EstructuraCamposGabinete(i).SISTEMA = Datset.Tables(0).Rows(i).Item(4)
                    EstructuraCamposGabinete(i).ESTADO = Datset.Tables(0).Rows(i).Item(5)
                    EstructuraCamposGabinete(i).INFOCAMPO = Datset.Tables(0).Rows(i).Item(6)
                    EstructuraCamposGabinete(i).CAMPOPUBLICO = Datset.Tables(0).Rows(i).Item(7)
                    EstructuraCamposGabinete(i).CAMPOUNICO = Datset.Tables(0).Rows(i).Item(8)
                    EstructuraCamposGabinete(i).CAMPO_RADICADO = Datset.Tables(0).Rows(i).Item(9)
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        EstructuraCamposGabinete(i).ALEAS_CAMPO = EstructuraCamposGabinete(i).CAMPO
                    Else
                        EstructuraCamposGabinete(i).ALEAS_CAMPO = Datset.Tables(0).Rows(i).Item(10)
                    End If
                    EstructuraCamposGabinete(i).CAMPO_ENABLE_DISABLE = Datset.Tables(0).Rows(i).Item(11)
                Next
                SolicitaEstructuraCamposGabinetePorId = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaEstructuraCamposGabinetePorId = "Inconsistencia general función id_gabinete " & ex.Message
        End Try
    End Function
    Function SolicitaDetalleCamposGabinete(ByVal NombreGabinete As String,
                                           ByRef EstruCampoDetalleGabinete() As stru_campo_detalle) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de una gabinete para interface de 
        '        : de consulta de documentos por nombre de gabinete
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'NombreGabinete               : Representa el nombre del gabinete 
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'EstructuraCamposGabinete       :Representa la estructura de campos de un gabinete 
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------

        Try
            Erase EstruCampoDetalleGabinete
            Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
            "DETALLE_GABIENETE " &
            "WHERE GABINETE='" & NombreGabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Return "Funcion  SolicitaDetalleCamposGabinete dice : (" & Resulta & ")"
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar los campos para gabinete : (" & NombreGabinete & ")"
            Else
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve EstruCampoDetalleGabinete(y)
                    EstruCampoDetalleGabinete(y).nombre_campo = Datset.Tables(0).Rows(y).Item(0)
                    EstruCampoDetalleGabinete(y).tipo_campo = Datset.Tables(0).Rows(y).Item(1)
                Next
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia general función SolicitaDetalleCamposGabinete " & ex.Message
        End Try
    End Function
    Function Solicita_detalle_campos_gabinete_sin_gestion(ByVal nombre_gabinete As String,
                                                          ByRef stru_campo_detalle() As stru_campo_detalle) As String
        Try
            Erase stru_campo_detalle
            Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
            "DETALLE_GABIENETE " &
            "WHERE GABINETE='" & nombre_gabinete & "' AND VISIBLE=1 and SISTEMA=0 AND ESTADO=0 ORDER BY IDENTI"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_detalle_campos_gabinete_sin_gestion = "Funcion  Solicita_detalle_campos_gabinete_sin_gestion dice : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_detalle_campos_gabinete_sin_gestion = "Imposible encontrar los campos para gabinete : (" & nombre_gabinete & ")"
                Exit Function
            Else
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_campo_detalle(y)
                    stru_campo_detalle(y).nombre_campo = Datset.Tables(0).Rows(y).Item(0)
                    stru_campo_detalle(y).tipo_campo = Datset.Tables(0).Rows(y).Item(1)
                Next
                Solicita_detalle_campos_gabinete_sin_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_detalle_campos_gabinete_sin_gestion = "Inconsistencia general función Solicita_detalle_campos_gabinete_sin_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_detalle_campos_gabinete_publico(ByVal nombre_gabinete As String,
                                                     ByRef stru_campo_detalle() As stru_campo_detalle) As String
        Try
            Erase stru_campo_detalle
            Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
            "DETALLE_GABIENETE " &
            "WHERE GABINETE='" & nombre_gabinete & "' AND VISIBLE=1 AND CAMPO_PUBLICO=1 ORDER BY IDENTI"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_detalle_campos_gabinete_publico = "Funcion  SolicitaDetalleCamposGabinete dice : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_detalle_campos_gabinete_publico = "Imposible encontrar los campos para gabinete : (" & nombre_gabinete & ")"
                Exit Function
            Else
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_campo_detalle(y)
                    stru_campo_detalle(y).nombre_campo = Datset.Tables(0).Rows(y).Item(0)
                    stru_campo_detalle(y).tipo_campo = Datset.Tables(0).Rows(y).Item(1)
                Next
                Solicita_detalle_campos_gabinete_publico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_detalle_campos_gabinete_publico = "Inconsistencia general función Solicita_detalle_campos_gabinete_publico " & ex.Message
        End Try
    End Function
    Function Consulta_Campos_Obligatorio(ByVal Nombre_Gabinete As String,
                                         ByRef Matri_Campos_Gabinete() As String) As String
        '**************************************
        'Funcion : consulta los campos obliga
        'torios de la tabla system1
        'Fecha : 2010-09-02
        'Ing : Miguel Angel Urueta Miranda
        'Modificada 2013-08-09 para la conexion
        'web 
        '**************************************
        Try
            Dim i As Integer = 0
            Dim Sql_consulta = "SELECT SISTEMA,CAMPO FROM " &
                 "DETALLE_GABIENETE " &
                 "WHERE GABINETE='" & Nombre_Gabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Consulta_Campos_Obligatorio = "Error Consultando en tabla 36 " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Campos_Obligatorio = "Imposible encontrar los campos para gabinete en la tabla gabinete detalle"
                Exit Function
            Else
                For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Gabinete(i2)
                    Matri_Campos_Gabinete(i2) = Datset.Tables(0).Rows(i2).Item(0).ToString _
                      & "|" & Datset.Tables(0).Rows(i2).Item(1).ToString
                Next
                Consulta_Campos_Obligatorio = "YES"
            End If

        Catch ex As Exception
            Consulta_Campos_Obligatorio = "Inconsistencia general función Consulta_Campos_Obligatorio " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_campo_nombre(ByVal nombre_gabinete As String,
                                              ByVal nombre_campo As String,
                                              ByRef existencia As String) As String
        Try
            Dim i As Integer = 0
            Dim Sql_consulta = "SELECT id_detalle_gabinete FROM " &
                 "DETALLE_GABIENETE " &
                 "WHERE GABINETE='" & nombre_gabinete & "' AND CAMPO='" & nombre_campo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_existencia_campo_nombre = "Error funcion  Solicita_existencia_campo_nombre " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_existencia_campo_nombre = "YES"
                existencia = "NO"
                Exit Function
            Else
                existencia = "YES"
                Solicita_existencia_campo_nombre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_campo_nombre = "Inconsistencia general función Solicita_existencia_campo_nombre " & ex.Message
        End Try
    End Function
    Function SolicitaNombreCampoRadicadoGabinete(ByVal NombreGabinete As String,
                                                 ByRef NombreCampoRadicado As String) As String
        Try
            Dim Sql_consulta As String = "select  CAMPO  from detalle_gabienete  where GABINETE ='" & NombreGabinete & "' and CAMPO_RADICADO=1"
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("detalle_gabienete")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                SolicitaNombreCampoRadicadoGabinete = "Error función SolicitaNombreCampoRadicadoGabinete  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreCampoRadicadoGabinete = "Imposible encontrar el campo que se comporta como (radicado) en el gabinete (" & NombreGabinete & ")"
                Exit Function
            Else
                NombreCampoRadicado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoRadicadoGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoRadicadoGabinete = "Inconsistencia general función SolicitaNombreCampoRadicadoGabinete " & ex.Message
        End Try
    End Function
    Function SolicitaValoresCamposDocumentoGabinete(ByRef MatriDatosAlmacenamiento() As String,
                                                    ByVal NombreGabinete As String,
                                                    ByVal MatriDatosAsignacionCampos() As Datos_Almacenamiento) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de almacenamineto del documento recibiedo la matriz de datos
        '          y el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete                : Representa el nombre del gabinete
        'MatriDatosAsignacionCampos    : Representa la estructura de asignación
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'MatriDatosAlmacenamiento  : Retorna la estructura de datos de almacenamiento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2014-02-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Numero_Colum As Integer = 0
            Dim Parametro_Consulta As String = "SELECT CAMPO FROM DETALLE_GABIENETE WHERE GABINETE" &
                "='" & NombreGabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If result <> "YES" Then
                SolicitaValoresCamposDocumentoGabinete = "Error Consultando en tabla Detalle_Gabinete " & " " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaValoresCamposDocumentoGabinete = "Imposible encontrar  campos de almacenamiento para el gabinete (" & NombreGabinete & ")"
                Exit Function
            Else
                Erase MatriDatosAlmacenamiento
                Dim icont As Integer = 0
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve MatriDatosAlmacenamiento(icont)
                    MatriDatosAlmacenamiento(icont) = ""
                    Dim nombre_Campo As String = Datset.Tables(0).Rows(i).Item(0)
                    '----Asigna el valor al campo de la matriz 
                    If Not MatriDatosAsignacionCampos Is Nothing Then
                        For z As Integer = 0 To MatriDatosAsignacionCampos.Length - 1
                            If UCase(nombre_Campo) = UCase(MatriDatosAsignacionCampos(z).nombre_campo) Then
                                MatriDatosAlmacenamiento(icont) = MatriDatosAsignacionCampos(z).valor_campo
                                Exit For
                            End If
                        Next
                    End If
                    icont = icont + 1
                Next
            End If
            SolicitaValoresCamposDocumentoGabinete = "YES"
        Catch ex As Exception
            SolicitaValoresCamposDocumentoGabinete = "Error General Funcion SolicitaValoresCamposDocumentoGabinete Error :" & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraCamposGabinete(ByVal NombreTablaGabinete As String,
                                              ByRef MatrizDatosAlmacenamiento() As String,
                                              ByRef MatriEstructuraCamposGabinete() As Datos_Almacenamiento) As String

        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con los datos de almacenamiento y la estructura de campos de
        '          alnacenamiento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreTablaGabinete : Representa nombre de la tabla del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'MatrizDatosAlmacenamiento      : Retorna la matriz de datos para almaceamiento en el gabinete
        'MatriEstructuraCamposGabinete  : Retorna la estructura con los campos de almacenamiento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "SELECT CAMPO FROM DETALLE_GABIENETE WHERE GABINETE" &
                "='" & NombreTablaGabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ConexDatabase As New conect.Dbase_Conction_Mysql_DA
            Dim DatSet As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Result As String = ConexDatabase.SELECTION_SELECT_FIELDA(SqlConsulta, DatSet)
            If Result <> "YES" Then
                Return "Error Consultando en tabla Detalle_Gabinete " & " " & Result
            End If
            If DatSet.Tables(0).Rows.Count = 0 Then
                Return "Imposible Encontrar campos de almacenamiento para el gabinete (" & NombreTablaGabinete & ")"
            Else
                Erase MatrizDatosAlmacenamiento
                Dim Icont As Integer = 0
                Dim IcontEstructura As Integer = 0
                IcontEstructura = MatriEstructuraCamposGabinete.Length
                For i As Integer = 0 To DatSet.Tables(0).Rows.Count - 1
                    ReDim Preserve MatrizDatosAlmacenamiento(Icont)
                    MatrizDatosAlmacenamiento(Icont) = ""
                    Dim nombre_Campo As String = DatSet.Tables(0).Rows(i).Item(0)
                    If Not MatriEstructuraCamposGabinete Is Nothing Then
                        For z As Integer = 0 To MatriEstructuraCamposGabinete.Length - 1
                            If UCase(nombre_Campo) = UCase(MatriEstructuraCamposGabinete(z).nombre_campo) Then
                                MatrizDatosAlmacenamiento(Icont) = MatriEstructuraCamposGabinete(z).valor_campo
                                Exit For
                            End If
                        Next
                    End If
                    Icont = Icont + 1
                Next
                For i As Integer = 0 To DatSet.Tables(0).Rows.Count - 1
                    Dim nombre_Campo As String = DatSet.Tables(0).Rows(i).Item(0)
                    ReDim Preserve MatriEstructuraCamposGabinete(IcontEstructura)
                    MatriEstructuraCamposGabinete(IcontEstructura).nombre_campo = nombre_Campo
                    IcontEstructura += 1
                Next
            End If
            Return "YES"
        Catch ex As Exception
            Return "Error General Funcion SolicitaEstructuraCamposGabinete Error : " & ex.Message
        End Try
    End Function
    Function Actualiza_Valores_Campos_Almacenamiento(ByRef Matri_Datos_Almacen() As String,
                                                     ByVal Nombre_Tabla As String,
                                                     ByVal Matri_Datos_Asignado() As Datos_Almacenamiento) As String
        '******************************************************************
        'Funcion : Obtiene campos del almacenamiento del gabinete y asigna
        'los datos de almacenamiento
        'ingeniero Miguel Angel Urueta Miranda
        'Fecha : 2014-02-23
        'Funcion extraida del workflow cliente y modificada para el modulo
        'web
        '*******************************************************************
        Try
            Dim Numero_Colum As Integer = 0
            Dim Parametro_Consulta As String = "SELECT CAMPO FROM DETALLE_GABIENETE WHERE GABINETE" &
                "='" & Nombre_Tabla & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If result <> "YES" Then
                Actualiza_Valores_Campos_Almacenamiento = "Error Consultando en tabla Detalle_Gabinete " & " " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Actualiza_Valores_Campos_Almacenamiento = "Imposible Encontrar  campos de almacenamiento " & Parametro_Consulta
                Exit Function
            Else

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim nombre_Campo As String = Datset.Tables(0).Rows(i).Item(0)
                    '----Asigna el valor al campo de la matriz 
                    If Not Matri_Datos_Asignado Is Nothing Then
                        For z As Integer = 0 To Matri_Datos_Asignado.Length - 1
                            If UCase(nombre_Campo) = UCase(Matri_Datos_Asignado(z).nombre_campo) Then
                                Matri_Datos_Almacen(i) = Matri_Datos_Asignado(z).valor_campo
                                Exit For
                            End If
                        Next
                    End If

                Next
            End If
            Actualiza_Valores_Campos_Almacenamiento = "YES"
        Catch ex As Exception
            Actualiza_Valores_Campos_Almacenamiento = "Error General Función Actualiza_Valores_Campos_Almacenamiento Error :" & ex.Message
        End Try
    End Function

    Function SolicitaEstructuraCamposGabinete(ByVal NombreGabinete As String,
                                              ByRef EstructuraGabinete() As estructura_gabinete) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de un gabinete por nombre
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstructuraGabinete   : Retorna la estructura de los campos del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta = "SELECT IDENTI,TIPO,CAMPO,VISIBLE,SISTEMA,ESTADO,INFOCAMPO,CAMPO_PUBLICO,CAMPO_UNICO,CAMPO_RADICADO,ALEAS_CAMPO,CAMPO_ENABLE_DISABLE FROM " &
                            "DETALLE_GABIENETE " &
                            "WHERE GABINETE='" & NombreGabinete & "' ORDER BY IDENTI"
            Dim Result As String = ""
            Dim ConecdB As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Result = ConecdB.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return "Función SolicitaEstructuraCamposGabinete dice " & Result

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar los campos para gabinete (" & NombreGabinete & ") en la tabla gabinete detalle  "
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve EstructuraGabinete(i)
                    EstructuraGabinete(i).IDENTI = Datset.Tables(0).Rows(i).Item(0)
                    EstructuraGabinete(i).TIPO = Datset.Tables(0).Rows(i).Item(1)
                    EstructuraGabinete(i).CAMPO = Datset.Tables(0).Rows(i).Item(2)
                    EstructuraGabinete(i).VISIBLE = Datset.Tables(0).Rows(i).Item(3)
                    EstructuraGabinete(i).SISTEMA = Datset.Tables(0).Rows(i).Item(4)
                    EstructuraGabinete(i).ESTADO = Datset.Tables(0).Rows(i).Item(5)
                    EstructuraGabinete(i).INFOCAMPO = Datset.Tables(0).Rows(i).Item(6)
                    EstructuraGabinete(i).CAMPOPUBLICO = Datset.Tables(0).Rows(i).Item(7)
                    EstructuraGabinete(i).CAMPOUNICO = Datset.Tables(0).Rows(i).Item(8)
                    EstructuraGabinete(i).CAMPO_RADICADO = Datset.Tables(0).Rows(i).Item(9)
                    EstructuraGabinete(i).ALEAS_CAMPO = Datset.Tables(0).Rows(i).Item(10)
                    EstructuraGabinete(i).CAMPO_ENABLE_DISABLE = Datset.Tables(0).Rows(i).Item(11)
                Next
                Return "YES"
            End If

        Catch ex As Exception
            Return "Inconsistencia general función SolicitaEstructuraCamposGabinete " & ex.Message
        End Try
    End Function
End Class
