Public Class Class_ra_con_campos_consulta_publica
    Function Solicita_campos_lista_documentos_matricualdo(ByVal id_registro_consulta_publica As Integer,
                                                          ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          resultados de la consulta de documentos relacionados a un matriucualdo
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-10
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim SQL_Consulta As String = "select  dg.CAMPO,rcc.ALEAS_campo_documentos_matriculado,dg.TIPO,rcc.opt_lista_campo_documentos_matriculado" &
            " from  ra_con_campos_consulta_publica as rcc" &
            " inner join detalle_gabienete as dg on (dg.id_detalle_gabinete=rcc.detalle_gabienete_id_detalle_gabinete)" &
            " where ra_con_registros_publicos_id_registro_publico=" & id_registro_consulta_publica & " and opt_lista_campo_documentos_matriculado=1 order by orden_campos_documentos_matriculado"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQL_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_documentos_matricualdo = " Función Solicita_campos_lista_documentos_matricualdo dice " & Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.title = "ID"
            item.field = "ID"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_documentos_matricualdo = "No se encontraron registro de campos para listar documentos relacionados a un matriculado"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(i).Item("ALEAS_campo_documentos_matriculado")
                    item.field = Datset.Tables(0).Rows(i).Item("CAMPO")
                    If Datset.Tables(0).Rows(i).Item("opt_lista_campo_documentos_matriculado") = 0 Then
                        item.visible = False
                    Else
                        item.visible = True
                    End If
                    item.viisble_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_documentos_matriculado")
                    item.visible_like_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_documentos_matriculado")
                    class_campos_table_bostra_table.Add(item)
                Next
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "SOPORTE DOCUMENTAL"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 0
                item.clickToSelect = False
                item.visible_like_sql = 0
                item.align = "center"
                item.events = "window.operateEventsDocumentoMatriculado"
                item.formatter = "operateFormattertablebootdocumentomatriculado"
                class_campos_table_bostra_table.Add(item)
                Solicita_campos_lista_documentos_matricualdo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_documentos_matricualdo = "Inconsistencia general funcion Solicita_campos_lista_documentos_matricualdo " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_documentos_relacionados_actos(ByVal id_registro_consulta_publica As Integer,
                                                                 ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          resultados de la consulta de documentos relacionados a un acto
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim SQL_Consulta As String = "select  dg.CAMPO,rcc.ALEAS_campo_documentos_acto_expediente,dg.TIPO,rcc.opt_lista_campo_documento_acto_expediente" &
            " from  ra_con_campos_consulta_publica as rcc" &
            " inner join detalle_gabienete as dg on (dg.id_detalle_gabinete=rcc.detalle_gabienete_id_detalle_gabinete)" &
            " where ra_con_registros_publicos_id_registro_publico=" & id_registro_consulta_publica & " and opt_lista_campo_documento_acto_expediente=1 order by orden_campo_documentos_acto_expediente"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQL_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_documentos_relacionados_actos = " Función Solicita_campos_lista_documentos_relacionados_actos dice " & Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table

            item = New class_campos_table_bostra_table
            item.title = "ID"
            item.field = "ID"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_documentos_relacionados_actos = "No se encontraron registro de campos para listar documentos relacionados a un acto del matriculado"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(i).Item("ALEAS_campo_documentos_acto_expediente")
                    item.field = Datset.Tables(0).Rows(i).Item("CAMPO")
                    If Datset.Tables(0).Rows(i).Item("opt_lista_campo_documento_acto_expediente") = 0 Then
                        item.visible = False
                    Else
                        item.visible = True
                    End If
                    item.viisble_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_documento_acto_expediente")
                    item.visible_like_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_documento_acto_expediente")

                    class_campos_table_bostra_table.Add(item)
                Next
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "SOPORTE DOCUMENTAL"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 0
                item.clickToSelect = False
                item.visible_like_sql = 0
                item.align = "center"
                item.events = "window.operateEventsActosDocumentos"
                item.formatter = "operateFormattertablebootmigactosdocumentos"
                class_campos_table_bostra_table.Add(item)
                Solicita_campos_lista_documentos_relacionados_actos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_documentos_relacionados_actos = "Inconsistencia general funcion Solicita_campos_lista_documentos_relacionados_actos " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_consulta_actos_matriculado_lista_bot(ByVal id_registro_consulta_publica As Integer,
                                                                        ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          resultados de la consulta de actos del matriuculado consulta pública
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-08
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim SQL_Consulta As String = "select  dg.CAMPO,rcc.ALEAS_campo_acto_expediente,dg.TIPO,rcc.opt_lista_campo_acto_expediente" &
            " from  ra_con_campos_consulta_publica as rcc" &
            " inner join detalle_gabienete as dg on (dg.id_detalle_gabinete=rcc.detalle_gabienete_id_detalle_gabinete)" &
            " where ra_con_registros_publicos_id_registro_publico=" & id_registro_consulta_publica & " and opt_lista_campo_acto_expediente=1 order by orden_campo_acto_expediente"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQL_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_consulta_actos_matriculado_lista_bot = " Función Solicita_campos_lista_consulta_matriculado_lista_bot dice " & Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_consulta_actos_matriculado_lista_bot = "No se encontraron registro de campos para el matriculado"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(i).Item("ALEAS_campo_acto_expediente")
                    item.field = Datset.Tables(0).Rows(i).Item("CAMPO")
                    If Datset.Tables(0).Rows(i).Item("opt_lista_campo_acto_expediente") = 0 Then
                        item.visible = False
                    Else
                        item.visible = True
                    End If
                    item.viisble_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_acto_expediente")
                    item.visible_like_sql = Datset.Tables(0).Rows(i).Item("opt_lista_campo_acto_expediente")
                    class_campos_table_bostra_table.Add(item)
                Next
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "DOCUMENTOS"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 0
                item.clickToSelect = False
                item.visible_like_sql = 0
                item.align = "center"
                item.events = "window.operateEventsActos"
                item.formatter = "operateFormattertablebootmigactos"
                class_campos_table_bostra_table.Add(item)
                Solicita_campos_lista_consulta_actos_matriculado_lista_bot = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_consulta_actos_matriculado_lista_bot = "Inconsistencia general funcion Solicita_campos_lista_consulta_actos_matriculado_lista_bot " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_consulta_matriculado_lista_bot(ByVal id_registro_consulta_publica As Integer,
                                                                  ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          resultados de la consulta de matriuculados consulta publica
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim SQL_Consulta As String = "select  dg.CAMPO,rcc.ALEAS_campo_matriculado,dg.TIPO,rcc.opt_lista_campo_matriculado," &
            "rcc.opt_search_campo_matriculado" &
            " from  ra_con_campos_consulta_publica as rcc" &
            " inner join detalle_gabienete as dg on (dg.id_detalle_gabinete=rcc.detalle_gabienete_id_detalle_gabinete)" &
            " where ra_con_registros_publicos_id_registro_publico=" & id_registro_consulta_publica & " and opt_lista_campo_matriculado=1 order by orden_campo_matriculado"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQL_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_campos_lista_consulta_matriculado_lista_bot = " Función Solicita_campos_lista_consulta_matriculado_lista_bot dice " & Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table

            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_campos_lista_consulta_matriculado_lista_bot = "No se encontraron registro de campos para el matriculado"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New class_campos_table_bostra_table
                    item.title = Datset.Tables(0).Rows(i).Item("ALEAS_campo_matriculado")
                    item.field = Datset.Tables(0).Rows(i).Item("CAMPO")
                    If Datset.Tables(0).Rows(i).Item("opt_lista_campo_matriculado") = 0 Then
                        item.visible = False
                    Else
                        item.visible = True
                    End If
                    item.viisble_sql = Datset.Tables(0).Rows(i).Item("opt_search_campo_matriculado")
                    item.visible_like_sql = Datset.Tables(0).Rows(i).Item("opt_search_campo_matriculado")
                    class_campos_table_bostra_table.Add(item)
                Next
                item = New class_campos_table_bostra_table
                item.field = "operate"
                item.title = "OPTION MATRICULADO"
                item.checkbox = False
                item.visible = True
                item.viisble_sql = 0
                item.clickToSelect = False
                item.visible_like_sql = 0
                item.align = "center"
                item.events = "window.operateEvents"
                item.formatter = "operateFormattertablebootmig"
                class_campos_table_bostra_table.Add(item)
                Solicita_campos_lista_consulta_matriculado_lista_bot = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campos_lista_consulta_matriculado_lista_bot = "Inconsistencia general funcion Solicita_campos_lista_consulta_matriculado_lista_bot " & ex.Message
        End Try
    End Function
End Class
