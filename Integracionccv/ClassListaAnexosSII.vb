Imports GestionDocumental_Docuarchi.net.Class_lista_imagenes_sii
Imports Newtonsoft.Json
Public Class CDlistaAnexosSII
    Public Property idanexo As String
    Public Property formato As String
    Public Property tipo As String
    Public Property observaciones As String
    Public Property url As String
    Public Property tiposirep As String
    Public Property tipodigitalizacion As String
    Public Property identificador As String
    Public Property nombre As String
    Public Property matricula As String
    Public Property proponente As String
    Public Property fechadocumento As String
    Public Property origen As String
    Public Property identificacion As String
End Class
Public Class CDEstadoAnexosSII
    Public Property error_gestion As String
    Public Property dato_lista As String
End Class
Public Class CDParameterAnexosSII
    Public Property IdTipoChekLista As Object
    Public Property DescripcionTipo As Object
    Public Property IdTipoTaramite As Object
    Public Property MultiAnexos As Object
    Public Property CodigoBarras As String
    Public Property ReciboSII As String
    Public Property Gabinete As String
End Class
Public Class ClassListaAnexosSII
    Function SolicitaEstructuraCamposAnexoReciboBootSIITableBootTra(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos anexos radicados SII para campos
        '          de tablas tipo boot
        '          
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        '
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos  tipo BOOT
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2025-07-19
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "OPERATION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 1
            item.align = "center"
            item.events = "window.operateEventsAnexosSII"
            item.formatter = "operateFormattertablebootListaAnexosSII"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "idanexo"
            item.field = "idanexo"
            item.field_destino = "idanexo"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "formato"
            item.field = "formato"
            item.field_destino = "formato"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "tipo"
            item.field = "tipo"
            item.field_destino = "tipo"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "DOCUMENTO"
            item.field = "observaciones"
            item.field_destino = "observaciones"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "url"
            item.field = "url"
            item.field_destino = "url"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "tiposirep"
            item.field = "tiposirep"
            item.field_destino = "tiposirep"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "tipodigitalizacion"
            item.field = "tipodigitalizacion"
            item.field_destino = "tipodigitalizacion"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "identificacion"
            item.field = "identificacion"
            item.field_destino = "identificacion"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "identificador"
            item.field = "identificador"
            item.field_destino = "identificador"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "nombre"
            item.field = "nombre"
            item.field_destino = "nombre"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "matricula"
            item.field = "matricula"
            item.field_destino = "matricula"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "proponente"
            item.field = "proponente"
            item.field_destino = "proponente"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "fechadocumento"
            item.field = "fechadocumento"
            item.field_destino = "fechadocumento"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "origen"
            item.field = "origen"
            item.field_destino = "origen"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            SolicitaEstructuraCamposAnexoReciboBootSIITableBootTra = "YES"
        Catch ex As Exception
            SolicitaEstructuraCamposAnexoReciboBootSIITableBootTra = "Inconsistencia general funcion SolicitaEstructuraCamposAnexoReciboBootSIITableBootTra " & ex.Message
        End Try
    End Function
    Function SolicitaArchivosAnexosrelacionadosRadicadoSII(ByRef ReciboSII As String,
                                                           ByRef CodigoBarras As String,
                                                           ByRef NombreGabinete As String,
                                                           ByRef Class_bostra_table_row As Object,
                                                           ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita los registro y la estructura de anexos SII relacionados
        '          a un radicado o código de barras
        '        
        '          
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        '
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'ReciboSII               :Representa el consecutivo del radicado SII
        'CodigoBarras            :Representa el codigo de barras relacionado al SII
        'Class_bostra_table_row  :Representa la estructura de los datos
        'class_campos_table_bostra_table : Representa la estructura de los campos
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2025-07-19
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = "Este usuario no cuenta con los permisos necesarios para adjuntar documentos a través de los servicios de integración. "
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE") = "-1" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE") = "0" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = "Es necesario seleccionar una tarea para poder adjuntar un documento desde los servicios web."
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                               HttpContext.Current.Session.Item("SII_COD_BARRAS"))
            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = Result
                Exit Function
            End If
            Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                                    HttpContext.Current.Session.Item("SII_RECIBO"))

            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = Result
                Exit Function
            End If
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabneteTareaWokflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                          HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                          NombreGabinete)
            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = Result
                Exit Function
            End If
            ReciboSII = HttpContext.Current.Session.Item("SII_RECIBO")
            CodigoBarras = HttpContext.Current.Session.Item("SII_COD_BARRAS")
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim ConsultarRadicado_sii As ConsultarRadicado_sii = Nothing
            Dim Class_lista_imagenes_sii As List(Of Class_lista_imagenes_sii.Class_lista_imagenes_sii) = Nothing
            Result = Class_ConsultarRadicado_sii.ConSultarRadicado(HttpContext.Current.Session.Item("SII_COD_BARRAS"),
                                                                   ConsultarRadicado_sii)
            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = "Error integración SII (" & Result & ")"
                Exit Function
            End If
            If ConsultarRadicado_sii.imagenes Is Nothing Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = "El radicado SII (" & CodigoBarras & ") , no tiene enexos SII relacionados "
                Exit Function
            End If
            Dim imagenes_sii_lista As New imagenes_sii_lista
            Result = imagenes_sii_lista.SolicitalistaImagenesAnexosSII(ConsultarRadicado_sii.imagenes,
                                                                       Class_lista_imagenes_sii)
            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Result = SolicitaEstructuraCamposAnexoReciboBootSIITableBootTra(class_campos_table_bostra_table)
            If Result <> "YES" Then
                SolicitaArchivosAnexosrelacionadosRadicadoSII = Result
                Exit Function
            End If
            Class_bostra_table_row = JsonConvert.SerializeObject(Class_lista_imagenes_sii)
            SolicitaArchivosAnexosrelacionadosRadicadoSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaArchivosAnexosrelacionadosRadicadoSII = "Inconsistencia general función SolicitaArchivosAnexosrelacionadosRadicadoSII " & ex.Message
        End Try
    End Function
End Class
