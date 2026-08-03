Imports FastExcel
Imports Newtonsoft.Json
Public Class Class_Integracion_SII_registro_tarea_flujo
    Property Error_gestion As String
    Property Class_config_general_service As IList(Of Class_config_general_service)
    Property id_flujo As Object
    Property id_usuario_workflow As Object
    Property id_ruta As Object
    Property id_grupo_workflow As Object
    Property id_actividad_workflow As Object
    Property id_actividad_flujo As Object
    Property codigo_rue As Object
    Property id_usuario_workflow_transacion As Object
    Property option_registra_log As Object
End Class
Public Class class_registro_tarea_ccv_SII
    Property recibo As String
    Property codigo_barras As String
    Property matricula As String
    Property rscocial As String
    Property id_ruta As Object
    Property id_actividad As Object
    Property id_actividad_fjujo As Object
    Property id_usuario As Object
    Property id_tramite As Object
    Property id_flujo As Object
    Property codigo_rue As Object
    Property id_usuario_workflow_transacion As Object
    Property option_registra_log As Object
End Class
Public Class CIncripcionSII
    Property LIBRO_SII As String
    Property REGISTRO_SII As String
    Property FECHA_SII As String
    Property HORA_SII As String
    Property MATRICULA_SII As String
    Property PROPONENTE_SII As String
    Property NIT_SII As String
    Property RSOCIAL_SII As String
    Property ACTO_SII As String
    Property NOTICIA_SII As String
    Property RADICADO_SII As String
    Property COD_BARRA_SII As String
    Property URL_SII As String
    Property NACTO_SII As String
End Class
Public Class CDParmeterValoresCamposGabineteSII
    Property IdTareaWorkflow As Long
    Property IdRutaWorkflow As Integer
    Property NombreRutaWorkflow As String
    Property Gabinete As String
End Class
Public Class Class_Integracion_SII
    Function SolicitaDatosCamposIndiceGabineteSII(ByVal CDParmeterValoresCamposGabineteSII As CDParmeterValoresCamposGabineteSII,
                                                  ByRef Radicado As String,
                                                  ByRef CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento)) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Asgina campos y datos de alamacenamiento para indice de gabinete con la integracion
        '          del sistema SII
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'CDParmeterValoresCamposGabineteSII  : Representa la estructura con los parmetros para estructura
        'de campos y datos de gabinete  IdTareaWorkflow-> Representa la identificacón de la tarea workflow
        'IdRutaWorkflow -> Representa la identificación de la ruta  NombreRutaWorkflow-> Representa la 
        'nombre de la ruta workflow
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'Radicado                     : Retorna el consecutivo de recibo del sistema SII
        'CDcamposAsignaAlmacenamiento : Retorna de los valores y los campos de almacenamiento
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim CodigoBarras As String = ""
            Dim ReciboSII As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaReciboCodigoBarrasSII(CDParmeterValoresCamposGabineteSII.IdTareaWorkflow,
                                                                      CDParmeterValoresCamposGabineteSII.NombreRutaWorkflow,
                                                                      CDParmeterValoresCamposGabineteSII.IdRutaWorkflow,
                                                                      ReciboSII,
                                                                      CodigoBarras)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteSII = Result
                Exit Function
            End If
            Radicado = ReciboSII
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim ConsultarRadicado_sii As ConsultarRadicado_sii = Nothing
            Result = Class_ConsultarRadicado_sii.ConSultarRadicado(CodigoBarras,
                                                                   ConsultarRadicado_sii)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteSII = Result
                Exit Function
            End If
            Dim Matricula As String = ConsultarRadicado_sii.matricula
            If Matricula <> "" Then
                Matricula = Matricula.Replace("S0", "")
            End If
            Dim MatriculaProponente As String = ConsultarRadicado_sii.proponente
            '///--------Remplaza caracteres no validos para el campo matricula SII----------////
            Dim ClassCarateres As New ClassCarateres
            Dim CDcarateres As New List(Of CDcarateres)
            Result = ClassCarateres.SolicitaEstructuraCarateres(2,
                                                                CDcarateres)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteSII = Result
                Exit Function
            End If
            If Matricula <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, Matricula)
            End If
            If MatriculaProponente <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, MatriculaProponente)
            End If
            Dim RazonSocial As String = ConsultarRadicado_sii.nombre
            Dim Identificacion As String = ConsultarRadicado_sii.identificacion
            Dim ClassConsultaExpedienteSII = New ClassConsultaExpedienteSII
            Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
            Dim StruSiiCahcheInscripcion As New StruSiiCahcheInscripcion
            If MatriculaProponente <> "" Or Matricula <> "" Then
                Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                    MatriculaProponente,
                                                                                    CDParmeterValoresCamposGabineteSII.Gabinete,
                                                                                    StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    SolicitaDatosCamposIndiceGabineteSII = Result & ". Contenido de la matricula (" & Matricula & MatriculaProponente & ")."
                    Exit Function
                End If
                RazonSocial = StruSiiCahcheInscripcion.Rsocial
                Identificacion = StruSiiCahcheInscripcion.NitIdentificacion
            End If
            If RazonSocial <> "" Then
                RazonSocial = Left(RazonSocial, 40)
                RazonSocial = RazonSocial.Replace("'", "")
                RazonSocial = RazonSocial.Replace("/", "-")
                RazonSocial = RazonSocial.Replace("\", "-")
            End If
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "CODBARRAS"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CodigoBarras
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ENLASE"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = ReciboSII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            Select Case CDParmeterValoresCamposGabineteSII.Gabinete
                Case "MERCANTIL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Matricula
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "ESAL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Matricula
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "RUP"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = MatriculaProponente
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            End Select
            SolicitaDatosCamposIndiceGabineteSII = "YES"
        Catch ex As Exception
            SolicitaDatosCamposIndiceGabineteSII = "Inconsistencia general funcion SolicitaDatosCamposIndiceGabineteSII " & ex.Message
        End Try
    End Function


    Function Formato_campo_nombre_sii(ByVal nombre As String,
                                      ByRef salida_formato_nombre As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Le da formato al campo nombre de integración SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombre               : Representa el campo nombre de la integración SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'salida_formato_nombre  : Retorna campo nombre formateado
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-03
        'Elabora               : Miguel Angel Urueta Miranda 
        '------------------------------------------------------------------------------------------------
        Try
            If nombre = "" Then
                salida_formato_nombre = ""
                Formato_campo_nombre_sii = "YES"
                Exit Function
            Else
                If nombre.Length > 60 Then
                    salida_formato_nombre = Left(nombre, 60)
                Else
                    salida_formato_nombre = nombre
                End If
                salida_formato_nombre = salida_formato_nombre.Replace("'", "")
                salida_formato_nombre = salida_formato_nombre.Replace("/", "")
                salida_formato_nombre = salida_formato_nombre.Replace("&", "")
                salida_formato_nombre = salida_formato_nombre.Replace(";", "")
                salida_formato_nombre = salida_formato_nombre.Replace("%", "")
                salida_formato_nombre = salida_formato_nombre.Replace("\", "")
                salida_formato_nombre = salida_formato_nombre.Replace("#", "")
                Formato_campo_nombre_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Formato_campo_nombre_sii = "Inconsitencia general funcion Formato_campo_nombre_sii " & ex.Message
        End Try
    End Function
    Function Solicita_lista_archivo_sii_rue(ByVal file_archivo_sii_rue As String,
                                            ByVal name_plantilla As String,
                                            ByVal name_index As Object,
                                            ByRef Class_bostra_table_row As Object,
                                            ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de registros del archivo RUES del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'file_archivo_sii_rue : Representa la ruta del archivo a con el contenido de los registros
        'name_plantilla       : Representa el nombre la plantilla que representa los campos
        'name_index           : Representa el nombre o el index de la hoja de excel
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura con los registros del archivo
        'obj_field_boot_table       : Retorna la estructura de las columnas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-16
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_FastExcel As New Class_FastExcel
            Dim Class_imp01_plantillaimp As New Class_imp01_plantillaimp
            Dim id_plantilla As Integer = 0
            Result = Class_imp01_plantillaimp.Solicita_identificacion_plantilla_externa_x_nombe(name_plantilla,
                                                                                                id_plantilla)
            If Result <> "YES" Then
                Solicita_lista_archivo_sii_rue = Result
                Exit Function
            End If
            Dim Class_imp01_campos_plantilla As New Class_imp01_campos_plantilla
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Result = Class_imp01_campos_plantilla.Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII(id_plantilla,
                                                                                                                class_campos_table_bostra_table)
            If Result <> "YES" Then
                Solicita_lista_archivo_sii_rue = Result
                Exit Function
            End If

            Dim Class_FastExcel_interfaz As New Class_FastExcel_interfaz
            Result = Class_FastExcel.Read_file_fast_Excell(file_archivo_sii_rue,
                                                           1,
                                                           Class_FastExcel_interfaz)
            If Result <> "YES" Then
                Solicita_lista_archivo_sii_rue = Result
                Exit Function
            End If

            Dim file_inf As New System.IO.FileInfo(file_archivo_sii_rue)
            '/////////--------Valida los campos del archivo excel con los campos de la plantilla---/////
            Dim mat_campo As String = ""
            Result = Class_FastExcel.Valida_campos_plantilla_fast_excell(class_campos_table_bostra_table,
                                                                         Class_FastExcel_interfaz.colums,
                                                                         mat_campo)
            If Result <> "YES" Then
                Solicita_lista_archivo_sii_rue = Result
                Exit Function
            End If
            If mat_campo <> "" Then
                Solicita_lista_archivo_sii_rue = "Los siguientes campos (" & mat_campo & ") no estan disponibles en el archivo fuente de excel (" & file_inf.Name & "), por favor revise la exitencia de estos campos en el archivo"
                Exit Function
            End If
            Dim dt = New DataTable
            '////////-----Agrega las columnas de la interfaz del fastExcell al data table---///
            Dim colum_map() As String = Nothing
            Dim icont As Integer = 0
            Dim index_recibo As Integer = -1
            Dim salida_nombre As String = ""
            For i As Integer = 0 To Class_FastExcel_interfaz.colums.Count - 1
                If Class_FastExcel_interfaz.colums(i).visible = True Then
                    ReDim Preserve colum_map(icont)
                    colum_map(icont) = Class_FastExcel_interfaz.colums(i).name_colums
                    dt.Columns.Add(Class_FastExcel_interfaz.colums(i).aleas_colum_plantilla, System.Type.GetType("System.String"))
                    If Class_FastExcel_interfaz.colums(i).aleas_colum_plantilla = "RECIBO" Then
                        index_recibo = i
                    End If
                    icont = icont + 1
                End If
            Next
            '////////-----Agrega los registros de la interfaz del fastExcell al data table---///
            Dim Class_wf_int_sii_registro_tarea_rue_virtual As New Class_wf_int_sii_registro_tarea_rue_virtual
            Dim existencia_recibo As String = "NO"
            Dim dat_row As DataRow
            For i As Integer = 0 To Class_FastExcel_interfaz.row.Count - 1
                If index_recibo <> -1 Then
                    Dim val_recibo As String = Class_FastExcel_interfaz.row(i).cells(index_recibo).value
                    Result = Class_wf_int_sii_registro_tarea_rue_virtual.Solicita_existencia_registro_recibo_sii(val_recibo,
                                                                                                                 existencia_recibo)
                    If Result <> "YES" Then
                        Solicita_lista_archivo_sii_rue = Result
                        Exit Function
                    End If
                    If existencia_recibo = "NO" Then
                        dat_row = dt.NewRow()
                        For Each item_ As Object In Class_FastExcel_interfaz.row(i).cells
                            Dim visible As String = ""
                            For z As Integer = 0 To colum_map.Length - 1
                                If item_.colunm_name = colum_map(z) Then
                                    If item_.colunm_name = "NOMBRE" Then
                                        Formato_campo_nombre_sii(item_.value, salida_nombre)
                                        dat_row.Item(z) = salida_nombre
                                    Else
                                        dat_row.Item(z) = item_.value
                                    End If
                                End If
                            Next
                        Next
                        dt.Rows.Add(dat_row)
                    End If
                End If

            Next
            '////////-----Serializa el  data table---///
            Class_bostra_table_row = JsonConvert.SerializeObject(dt)
            Kill(file_archivo_sii_rue)
            Solicita_lista_archivo_sii_rue = "YES"
        Catch ex As Exception
            Solicita_lista_archivo_sii_rue = "Iconsitencia general función Solicita_lista_archivo_sii_rue " & ex.Message
        End Try
    End Function
    Function Solicita_lista_archivo_virtual_sii(ByVal file_archivo_sii_virtual As String,
                                                ByVal name_plantilla As String,
                                                ByVal name_index As Object,
                                                ByRef Class_bostra_table_row As Object,
                                                ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de registros del archivo SII del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'file_archivo_siI_VITUAL : Representa la ruta del archivo a con el contenido de los registros
        'name_plantilla       : Representa el nombre la plantilla que representa los campos
        'name_index           : Representa el nombre o el index de la hoja de excel
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura con los registros del archivo
        'obj_field_boot_table       : Retorna la estructura de las columnas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-16
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_FastExcel As New Class_FastExcel
            Dim Class_imp01_plantillaimp As New Class_imp01_plantillaimp
            Dim id_plantilla As Integer = 0
            Result = Class_imp01_plantillaimp.Solicita_identificacion_plantilla_externa_x_nombe(name_plantilla,
                                                                                                id_plantilla)
            If Result <> "YES" Then
                Solicita_lista_archivo_virtual_sii = Result
                Exit Function
            End If
            Dim Class_imp01_campos_plantilla As New Class_imp01_campos_plantilla
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Result = Class_imp01_campos_plantilla.Solicita_estructura_campos_dynamic_polantilla_externa_virtual_SII(id_plantilla,
                                                                                                                    class_campos_table_bostra_table)
            If Result <> "YES" Then
                Solicita_lista_archivo_virtual_sii = Result
                Exit Function
            End If

            Dim Class_FastExcel_interfaz As New Class_FastExcel_interfaz
            Result = Class_FastExcel.Read_file_fast_Excell(file_archivo_sii_virtual,
                                                           1,
                                                           Class_FastExcel_interfaz)
            If Result <> "YES" Then
                Solicita_lista_archivo_virtual_sii = Result
                Exit Function
            End If

            Dim file_inf As New System.IO.FileInfo(file_archivo_sii_virtual)
            '/////////--------Valida los campos del archivo excel con los campos de la plantilla---/////
            Dim mat_campo As String = ""
            Result = Class_FastExcel.Valida_campos_plantilla_fast_excell(class_campos_table_bostra_table,
                                                                         Class_FastExcel_interfaz.colums,
                                                                         mat_campo)
            If Result <> "YES" Then
                Solicita_lista_archivo_virtual_sii = Result
                Exit Function
            End If
            If mat_campo <> "" Then
                Solicita_lista_archivo_virtual_sii = "Los siguientes campos (" & mat_campo & ") no estan disponibles en el archivo fuente de excel (" & file_inf.Name & "), por favor revise la exitencia de estos campos en el archivo"
                Exit Function
            End If
            Dim dt = New DataTable
            '////////-----Agrega las columnas de la interfaz del fastExcell al data table---///
            Dim colum_map() As String = Nothing
            Dim icont As Integer = 0
            Dim index_recibo As Integer = -1
            Dim index_nombre As Integer = 0
            For i As Integer = 0 To Class_FastExcel_interfaz.colums.Count - 1
                If Class_FastExcel_interfaz.colums(i).visible = True Then
                    ReDim Preserve colum_map(icont)
                    colum_map(icont) = Class_FastExcel_interfaz.colums(i).name_colums
                    dt.Columns.Add(Class_FastExcel_interfaz.colums(i).aleas_colum_plantilla, System.Type.GetType("System.String"))
                    If Class_FastExcel_interfaz.colums(i).aleas_colum_plantilla = "CODIGOBARRAS" Then
                        index_recibo = i
                    End If
                    icont = icont + 1
                End If
            Next
            '////////-----Agrega los registros de la interfaz del fastExcell al data table---///
            Dim Class_wf_int_sii_registro_tarea_rue_virtual As New Class_wf_int_sii_registro_tarea_rue_virtual
            Dim existencia_recibo As String = "NO"
            Dim dat_row As DataRow
            Dim salida_nombre As String = ""

            For i As Integer = 0 To Class_FastExcel_interfaz.row.Count - 1
                Dim val_radicado As String = ""
                If index_recibo <> -1 Then
                    val_radicado = Class_FastExcel_interfaz.row(i).cells(index_recibo).value
                    Result = Class_wf_int_sii_registro_tarea_rue_virtual.Solicita_existencia_registro_radicado_sii(val_radicado,
                                                                                                                   existencia_recibo)
                    If Result <> "YES" Then
                        Solicita_lista_archivo_virtual_sii = Result
                        Exit Function
                    End If
                End If
                If existencia_recibo = "NO" Then
                    dat_row = dt.NewRow()
                    For Each item_ As Object In Class_FastExcel_interfaz.row(i).cells
                        Dim visible As String = ""
                        For z As Integer = 0 To colum_map.Length - 1
                            If item_.colunm_name = colum_map(z) Then
                                If item_.colunm_name = "NOMBRE" Then
                                    Formato_campo_nombre_sii(item_.value, salida_nombre)
                                    dat_row.Item(z) = salida_nombre
                                Else
                                    dat_row.Item(z) = item_.value
                                End If
                            End If
                        Next
                    Next
                    dt.Rows.Add(dat_row)
                End If
            Next
            '////////-----Serializa el  data table---///
            Class_bostra_table_row = JsonConvert.SerializeObject(dt)
            Kill(file_archivo_sii_virtual)
            Solicita_lista_archivo_virtual_sii = "YES"
        Catch ex As Exception
            Solicita_lista_archivo_virtual_sii = "Iconsitencia general función Solicita_lista_archivo_virtual_sii " & ex.Message
        End Try
    End Function
End Class
