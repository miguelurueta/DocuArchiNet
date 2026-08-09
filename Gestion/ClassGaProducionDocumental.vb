Imports MySql.Data.MySqlClient
Imports System.IO
Imports AjaxControlToolkit
Imports Ionic.Zip
Imports System.Xml
Public Class class_dowload_expediente_producion
    Public Property id_produccion As Long
    Public Property id_Cont As Integer
    Public Property totalCount As Integer
    Public Property state_propietary As Integer
    Public Property result As String
    Public Property out_source_file_zip As String = ""
    Public Property url_file_zip As String = ""
    Public Property name_document As String = ""
End Class
Public Class CDexpedienteSeleccionado
    Public Property IdExpediente As Integer
    Public Property IdUsuarioGestion As Integer
    Public Property IdNivelExpediente As Integer
End Class
Public Class CDproduccion
    Property AppError As String
    Property CDexpedienteSeleccionado As New List(Of CDexpedienteSeleccionado)
End Class
Public Structure stru_produccion_indice
    Dim ID_REGISTRO_PRODUCION_DOCUMENTAL As Long
    Dim remit_dest_interno_idremit_dest_interno As Integer
    Dim ID_USUARIO_GESTION As Integer
    Dim FECHA_DOCUMENTO As String
    Dim ID_AREA_DEPARTAMENTO As Integer
    Dim NOMBRE_AREA_DEPARTAMENTO As String
    Dim ID_SERIE_DOCUMENTO As Integer
    Dim SERIE_DOCUMENTO As String
    Dim ID_SUBSERIE_DOCUMENTO As Integer
    Dim SUBSERIE_DOCUMENTO As String
    Dim ID_TIPO_DOCUMENTO As Integer
    Dim CONSECUTIVO_TIPO_DOCUMENTO As Integer
    Dim DESCRIPCION_TIPO_DOCUMENTO As String
    Dim SEGUNDO_NOMBRE_DOCUMENTO As String
    Dim ASUNTO_DOCUMENTO As String
    Dim TEMA_DOCUMENTO As String
    Dim FULTEXT_DOCUMENTO As String
    Dim UBICACION_ARCHIVO As String
    Dim UBICACION_CARPETA As String
    Dim UBICACION_CAJA As String
    Dim UBICACION_STAN As String
    Dim ID_DOCUMENTO_DOCUARCHI_ALMACEN As Integer
    Dim ID_DOCUMENTO_DOCUARCHI_TEMPORAL As Integer
    Dim ESTADO_DOCUMENTO_ARCHIVO As Integer
    Dim NOMBRE_GABINETE As String
    Dim RADICADO_DOCUMENTO As String
    Dim ID_PLANTILLA_RADICADO As Integer
    Dim NUMERO_FOLIOS As Integer
    Dim EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE As Integer
    Dim CONSECUTIVO_DOCUMENTO As Integer
    Dim EXPEDIENTE As String
    Dim ID_TIPO_EXPEDIENTE As Integer
    Dim ID_TIPO_UNIDAD_CONSERVACION As Integer
    Dim ID_UNIDAD_CONSERVACION As Integer
    Dim UNIDADCONSERVA As String
    Dim ID_CLASE_DOCUMENTO As Integer
    Dim CLASEDOCUMENTO As String
    Dim FECHA_ELABORACION As String
    Dim ID_TIPO_UNIDAD_DOCUMENTAL As Integer
    Dim ID_EMPRESA_DOCUMENTO As Integer
    Dim DOCUMENTO_PRODUCION_DOCUMENTAL As Integer
    Dim FORMATO As String
    Dim TAMANO As String
    Dim ORIGEN As String
    Dim ESTADO_ELIMINA_PRODUCION_DOCUMENTAL As Integer
    Dim VALOR_HUELLA As String
    Dim FUCION_RESUMEN As String
    Dim ID_INDICE As Long
    Dim RUTA_ARCHIVO As String
    Dim NOMBRE_DOCUARCHI As String
    Dim ORDEN_EN_EXPEDIENTE As Integer
    Dim PAGINA_INICIO As Integer
    Dim PAGINA_FINAL As Integer
End Structure

Public Structure STRU_LISTA_EXP_PRDUCION
    Dim ID_EXPEDIENTE As Integer
    Dim NOMBRE_EXPEDIENTE As String
    Dim SERIE As String
    Dim SUBSERIE As String
End Structure

Public Structure STRU_CAMPOS_GRIDVIEW
    Dim id_aribute As Long
    Dim nombre_campo As String
    Dim tipo_campos As String
    Dim valor_campo As Object
End Structure
Public Structure STRU_USUARIO_GESTION
    Dim Nombre_usuario As String
    Dim Cargo_usuario As String
    Dim Correo_Electronico As String
    Dim Area_usuario As String
End Structure
Public Class ClassGaProducionDocumental

    Function Solicita_id_inventario_documental(ByVal id_imagen As Long,
                                               ByVal nombre_gabinete As String,
                                               ByRef id_inventario_documental As Long) As String
        Try
            Dim Sql_consulta As String = "select  ID_REGISTRO_PRODUCION_DOCUMENTAL  from registro_producion_documental  where ID_DOCUMENTO_DOCUARCHI_ALMACEN =" _
                                         & id_imagen & " and NOMBRE_GABINETE='" & nombre_gabinete & "'"
            id_inventario_documental = 0
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("Gabinete_text")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_inventario_documental = "Error función Solicita_id_inventario_documental  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_inventario_documental = "YES"
                Exit Function
            Else
                id_inventario_documental = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_inventario_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_inventario_documental = "Inconsistencia gneral funcion Solicita_id_inventario_documental " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_registro_relacion_expediente_indice(ByVal id_expediente As Integer,
                                                                     ByRef stru_produccion_indice() As stru_produccion_indice) As String
        Try

            Dim sql_string As String = "Select ID_REGISTRO_PRODUCION_DOCUMENTAL, remit_dest_interno_idremit_dest_interno, ID_USUARIO_GESTION, FECHA_DOCUMENTO, " &
            "ID_AREA_DEPARTAMENTO,NOMBRE_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO, SERIE_DOCUMENTO, ID_SUBSERIE_DOCUMENTO," &
            "SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,CONSECUTIVO_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO," &
            "ASUNTO_DOCUMENTO,TEMA_DOCUMENTO,FULTEXT_DOCUMENTO, UBICACION_ARCHIVO, UBICACION_CARPETA, UBICACION_CAJA," &
            "UBICACION_STAN,ID_DOCUMENTO_DOCUARCHI_ALMACEN,ID_DOCUMENTO_DOCUARCHI_TEMPORAL,ESTADO_DOCUMENTO_ARCHIVO," &
            "NOMBRE_GABINETE,RADICADO_DOCUMENTO,ID_PLANTILLA_RADICADO,NUMERO_FOLIOS,EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE," &
            "CONSECUTIVO_DOCUMENTO ,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION,ID_UNIDAD_CONSERVACION," &
            "UNIDADCONSERVA,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO,FECHA_ELABORACION,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
            "DOCUMENTO_PRODUCION_DOCUMENTAL,FORMATO,TAMANO,ORIGEN,ESTADO_ELIMINA_PRODUCION_DOCUMENTAL from registro_producion_documental " &
            " where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & id_expediente
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_string, Datset)
            If Result <> "YES" Then
                Solicita_estructura_registro_relacion_expediente_indice = "Función Solicita_estructura_registro_relacion_expediente_indice dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_registro_relacion_expediente_indice = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_produccion_indice(i)
                    If Datset.Tables(0).Rows(i).IsNull("ID_REGISTRO_PRODUCION_DOCUMENTAL") = True Then
                        stru_produccion_indice(i).ID_REGISTRO_PRODUCION_DOCUMENTAL = 0
                    Else
                        stru_produccion_indice(i).ID_REGISTRO_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("ID_REGISTRO_PRODUCION_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("remit_dest_interno_idremit_dest_interno") = True Then
                        stru_produccion_indice(i).remit_dest_interno_idremit_dest_interno = 0
                    Else
                        stru_produccion_indice(i).remit_dest_interno_idremit_dest_interno = Datset.Tables(0).Rows(i).Item("remit_dest_interno_idremit_dest_interno")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_USUARIO_GESTION") = True Then
                        stru_produccion_indice(i).ID_USUARIO_GESTION = 0
                    Else
                        stru_produccion_indice(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("FECHA_DOCUMENTO") = True Then
                        stru_produccion_indice(i).FECHA_DOCUMENTO = ""
                    Else
                        ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(i).Item("FECHA_DOCUMENTO").ToString,
                                                                          stru_produccion_indice(i).FECHA_DOCUMENTO)
                        stru_produccion_indice(i).FECHA_DOCUMENTO = Left(stru_produccion_indice(i).FECHA_DOCUMENTO, "10")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_AREA_DEPARTAMENTO") = True Then
                        stru_produccion_indice(i).ID_AREA_DEPARTAMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_AREA_DEPARTAMENTO = Datset.Tables(0).Rows(i).Item("ID_AREA_DEPARTAMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("NOMBRE_AREA_DEPARTAMENTO") = True Then
                        stru_produccion_indice(i).NOMBRE_AREA_DEPARTAMENTO = ""
                    Else
                        stru_produccion_indice(i).NOMBRE_AREA_DEPARTAMENTO = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA_DEPARTAMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_SERIE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ID_SERIE_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_SERIE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ID_SERIE_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("SERIE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).SERIE_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).SERIE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("SERIE_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_SUBSERIE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ID_SUBSERIE_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_SUBSERIE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ID_SUBSERIE_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("SUBSERIE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).SUBSERIE_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).SUBSERIE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("SUBSERIE_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_TIPO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ID_TIPO_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_TIPO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ID_TIPO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("CONSECUTIVO_TIPO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).CONSECUTIVO_TIPO_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).CONSECUTIVO_TIPO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_TIPO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("DESCRIPCION_TIPO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).DESCRIPCION_TIPO_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).DESCRIPCION_TIPO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("DESCRIPCION_TIPO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ASUNTO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ASUNTO_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).ASUNTO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ASUNTO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("TEMA_DOCUMENTO") = True Then
                        stru_produccion_indice(i).TEMA_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).TEMA_DOCUMENTO = Datset.Tables(0).Rows(i).Item("TEMA_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("FULTEXT_DOCUMENTO") = True Then
                        stru_produccion_indice(i).FULTEXT_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).FULTEXT_DOCUMENTO = Datset.Tables(0).Rows(i).Item("FULTEXT_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("UBICACION_ARCHIVO") = True Then
                        stru_produccion_indice(i).UBICACION_ARCHIVO = ""
                    Else
                        stru_produccion_indice(i).UBICACION_ARCHIVO = Datset.Tables(0).Rows(i).Item("UBICACION_ARCHIVO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("UBICACION_CARPETA") = True Then
                        stru_produccion_indice(i).UBICACION_CARPETA = ""
                    Else
                        stru_produccion_indice(i).UBICACION_CARPETA = Datset.Tables(0).Rows(i).Item("UBICACION_CARPETA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("UBICACION_CAJA") = True Then
                        stru_produccion_indice(i).UBICACION_CAJA = ""
                    Else
                        stru_produccion_indice(i).UBICACION_CAJA = Datset.Tables(0).Rows(i).Item("UBICACION_CAJA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("UBICACION_STAN") = True Then
                        stru_produccion_indice(i).UBICACION_STAN = ""
                    Else
                        stru_produccion_indice(i).UBICACION_STAN = Datset.Tables(0).Rows(i).Item("UBICACION_STAN")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_DOCUMENTO_DOCUARCHI_ALMACEN") = True Then
                        stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_ALMACEN = 0
                    Else
                        stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_ALMACEN = Datset.Tables(0).Rows(i).Item("ID_DOCUMENTO_DOCUARCHI_ALMACEN")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_DOCUMENTO_DOCUARCHI_TEMPORAL") = True Then
                        stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_TEMPORAL = 0
                    Else
                        stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_TEMPORAL = Datset.Tables(0).Rows(i).Item("ID_DOCUMENTO_DOCUARCHI_TEMPORAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ESTADO_DOCUMENTO_ARCHIVO") = True Then
                        stru_produccion_indice(i).ESTADO_DOCUMENTO_ARCHIVO = 0
                    Else
                        stru_produccion_indice(i).ESTADO_DOCUMENTO_ARCHIVO = Datset.Tables(0).Rows(i).Item("ESTADO_DOCUMENTO_ARCHIVO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("NOMBRE_GABINETE") = True Then
                        stru_produccion_indice(i).NOMBRE_GABINETE = ""
                    Else
                        stru_produccion_indice(i).NOMBRE_GABINETE = Datset.Tables(0).Rows(i).Item("NOMBRE_GABINETE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("RADICADO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).RADICADO_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).RADICADO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("RADICADO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_PLANTILLA_RADICADO") = True Then
                        stru_produccion_indice(i).ID_PLANTILLA_RADICADO = 0
                    Else
                        stru_produccion_indice(i).ID_PLANTILLA_RADICADO = Datset.Tables(0).Rows(i).Item("ID_PLANTILLA_RADICADO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("NUMERO_FOLIOS") = True Then
                        stru_produccion_indice(i).NUMERO_FOLIOS = 0
                    Else
                        stru_produccion_indice(i).NUMERO_FOLIOS = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIOS")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE") = True Then
                        stru_produccion_indice(i).EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE = 0
                    Else
                        stru_produccion_indice(i).EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("CONSECUTIVO_DOCUMENTO") = True Then
                        stru_produccion_indice(i).CONSECUTIVO_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("EXPEDIENTE") = True Then
                        stru_produccion_indice(i).EXPEDIENTE = ""
                    Else
                        stru_produccion_indice(i).EXPEDIENTE = Datset.Tables(0).Rows(i).Item("EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_TIPO_EXPEDIENTE") = True Then
                        stru_produccion_indice(i).ID_TIPO_EXPEDIENTE = 0
                    Else
                        stru_produccion_indice(i).ID_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("ID_TIPO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_TIPO_UNIDAD_CONSERVACION") = True Then
                        stru_produccion_indice(i).ID_TIPO_UNIDAD_CONSERVACION = 0
                    Else
                        stru_produccion_indice(i).ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_UNIDAD_CONSERVACION") = True Then
                        stru_produccion_indice(i).ID_UNIDAD_CONSERVACION = 0
                    Else
                        stru_produccion_indice(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("UNIDADCONSERVA") = True Then
                        stru_produccion_indice(i).UNIDADCONSERVA = ""
                    Else
                        stru_produccion_indice(i).UNIDADCONSERVA = Datset.Tables(0).Rows(i).Item("UNIDADCONSERVA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_CLASE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ID_CLASE_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_CLASE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ID_CLASE_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("CLASEDOCUMENTO") = True Then
                        stru_produccion_indice(i).CLASEDOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).CLASEDOCUMENTO = Datset.Tables(0).Rows(i).Item("CLASEDOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("FECHA_ELABORACION") = True Then
                        stru_produccion_indice(i).FECHA_ELABORACION = ""
                    Else
                        ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(i).Item("FECHA_ELABORACION"),
                                                                          stru_produccion_indice(i).FECHA_ELABORACION)
                        stru_produccion_indice(i).FECHA_ELABORACION = Left(stru_produccion_indice(i).FECHA_ELABORACION, "10")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_TIPO_UNIDAD_DOCUMENTAL") = True Then
                        stru_produccion_indice(i).ID_TIPO_UNIDAD_DOCUMENTAL = 0
                    Else
                        stru_produccion_indice(i).ID_TIPO_UNIDAD_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ID_EMPRESA_DOCUMENTO") = True Then
                        stru_produccion_indice(i).ID_EMPRESA_DOCUMENTO = 0
                    Else
                        stru_produccion_indice(i).ID_EMPRESA_DOCUMENTO = Datset.Tables(0).Rows(i).Item("ID_EMPRESA_DOCUMENTO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("DOCUMENTO_PRODUCION_DOCUMENTAL") = True Then
                        stru_produccion_indice(i).DOCUMENTO_PRODUCION_DOCUMENTAL = 0
                    Else
                        stru_produccion_indice(i).DOCUMENTO_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("DOCUMENTO_PRODUCION_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("FORMATO") = True Then
                        stru_produccion_indice(i).FORMATO = ""
                    Else
                        stru_produccion_indice(i).FORMATO = Datset.Tables(0).Rows(i).Item("FORMATO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("TAMANO") = True Then
                        stru_produccion_indice(i).TAMANO = ""
                    Else
                        stru_produccion_indice(i).TAMANO = Datset.Tables(0).Rows(i).Item("TAMANO")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ORIGEN") = True Then
                        stru_produccion_indice(i).ORIGEN = ""
                    Else
                        stru_produccion_indice(i).ORIGEN = Datset.Tables(0).Rows(i).Item("ORIGEN")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("ESTADO_ELIMINA_PRODUCION_DOCUMENTAL") = True Then
                        stru_produccion_indice(i).ESTADO_ELIMINA_PRODUCION_DOCUMENTAL = 0
                    Else
                        stru_produccion_indice(i).ESTADO_ELIMINA_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(i).Item("ESTADO_ELIMINA_PRODUCION_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull("SEGUNDO_NOMBRE_DOCUMENTO") = True Then
                        stru_produccion_indice(i).SEGUNDO_NOMBRE_DOCUMENTO = ""
                    Else
                        stru_produccion_indice(i).SEGUNDO_NOMBRE_DOCUMENTO = Datset.Tables(0).Rows(i).Item("SEGUNDO_NOMBRE_DOCUMENTO")
                    End If
                Next
                Solicita_estructura_registro_relacion_expediente_indice = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_registro_relacion_expediente_indice = "Inconsistencia general funcion Solicita_estructura_registro_relacion_expediente_indice " & ex.Message
        End Try

    End Function
    Function Solicita_estructura_id_registro_produccion(ByVal id_registro_produccion As Long,
                                                        ByRef stru_produccion_indice As stru_produccion_indice) As String
        Try
            Dim sql_string As String = "Select ID_REGISTRO_PRODUCION_DOCUMENTAL, remit_dest_interno_idremit_dest_interno, ID_USUARIO_GESTION, FECHA_DOCUMENTO, " &
            "ID_AREA_DEPARTAMENTO,NOMBRE_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO, SERIE_DOCUMENTO, ID_SUBSERIE_DOCUMENTO," &
            "SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,CONSECUTIVO_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO," &
            "ASUNTO_DOCUMENTO,TEMA_DOCUMENTO,FULTEXT_DOCUMENTO, UBICACION_ARCHIVO, UBICACION_CARPETA, UBICACION_CAJA," &
            "UBICACION_STAN,ID_DOCUMENTO_DOCUARCHI_ALMACEN,ID_DOCUMENTO_DOCUARCHI_TEMPORAL,ESTADO_DOCUMENTO_ARCHIVO," &
            "NOMBRE_GABINETE,RADICADO_DOCUMENTO,ID_PLANTILLA_RADICADO,NUMERO_FOLIOS,EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE," &
            "CONSECUTIVO_DOCUMENTO ,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION,ID_UNIDAD_CONSERVACION," &
            "UNIDADCONSERVA,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO,FECHA_ELABORACION,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
            "DOCUMENTO_PRODUCION_DOCUMENTAL,FORMATO,TAMANO,ORIGEN,ESTADO_ELIMINA_PRODUCION_DOCUMENTAL from registro_producion_documental " &
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_string, Datset)
            If Result <> "YES" Then
                Solicita_estructura_id_registro_produccion = "Función Solicita_estructura_id_registro_produccion dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_id_registro_produccion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull("ID_REGISTRO_PRODUCION_DOCUMENTAL") = True Then
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL = 0
                Else
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("ID_REGISTRO_PRODUCION_DOCUMENTAL")
                End If
                If Datset.Tables(0).Rows(0).IsNull("remit_dest_interno_idremit_dest_interno") = True Then
                    stru_produccion_indice.remit_dest_interno_idremit_dest_interno = 0
                Else
                    stru_produccion_indice.remit_dest_interno_idremit_dest_interno = Datset.Tables(0).Rows(0).Item("remit_dest_interno_idremit_dest_interno")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_USUARIO_GESTION") = True Then
                    stru_produccion_indice.ID_USUARIO_GESTION = 0
                Else
                    stru_produccion_indice.ID_USUARIO_GESTION = Datset.Tables(0).Rows(0).Item("ID_USUARIO_GESTION")
                End If
                If Datset.Tables(0).Rows(0).IsNull("FECHA_DOCUMENTO") = True Then
                    stru_produccion_indice.FECHA_DOCUMENTO = ""
                Else
                    ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(0).Item("FECHA_DOCUMENTO").ToString,
                                                                      stru_produccion_indice.FECHA_DOCUMENTO)
                    stru_produccion_indice.FECHA_DOCUMENTO = Left(stru_produccion_indice.FECHA_DOCUMENTO, "10")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_AREA_DEPARTAMENTO") = True Then
                    stru_produccion_indice.ID_AREA_DEPARTAMENTO = 0
                Else
                    stru_produccion_indice.ID_AREA_DEPARTAMENTO = Datset.Tables(0).Rows(0).Item("ID_AREA_DEPARTAMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("NOMBRE_AREA_DEPARTAMENTO") = True Then
                    stru_produccion_indice.NOMBRE_AREA_DEPARTAMENTO = ""
                Else
                    stru_produccion_indice.NOMBRE_AREA_DEPARTAMENTO = Datset.Tables(0).Rows(0).Item("NOMBRE_AREA_DEPARTAMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_SERIE_DOCUMENTO") = True Then
                    stru_produccion_indice.ID_SERIE_DOCUMENTO = 0
                Else
                    stru_produccion_indice.ID_SERIE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_SERIE_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("SERIE_DOCUMENTO") = True Then
                    stru_produccion_indice.SERIE_DOCUMENTO = ""
                Else
                    stru_produccion_indice.SERIE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("SERIE_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_SUBSERIE_DOCUMENTO") = True Then
                    stru_produccion_indice.ID_SUBSERIE_DOCUMENTO = 0
                Else
                    stru_produccion_indice.ID_SUBSERIE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_SUBSERIE_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("SUBSERIE_DOCUMENTO") = True Then
                    stru_produccion_indice.SUBSERIE_DOCUMENTO = ""
                Else
                    stru_produccion_indice.SUBSERIE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("SUBSERIE_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_TIPO_DOCUMENTO") = True Then
                    stru_produccion_indice.ID_TIPO_DOCUMENTO = 0
                Else
                    stru_produccion_indice.ID_TIPO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_TIPO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("CONSECUTIVO_TIPO_DOCUMENTO") = True Then
                    stru_produccion_indice.CONSECUTIVO_TIPO_DOCUMENTO = 0
                Else
                    stru_produccion_indice.CONSECUTIVO_TIPO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("CONSECUTIVO_TIPO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("DESCRIPCION_TIPO_DOCUMENTO") = True Then
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = ""
                Else
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("DESCRIPCION_TIPO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ASUNTO_DOCUMENTO") = True Then
                    stru_produccion_indice.ASUNTO_DOCUMENTO = ""
                Else
                    stru_produccion_indice.ASUNTO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ASUNTO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("TEMA_DOCUMENTO") = True Then
                    stru_produccion_indice.TEMA_DOCUMENTO = ""
                Else
                    stru_produccion_indice.TEMA_DOCUMENTO = Datset.Tables(0).Rows(0).Item("TEMA_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("FULTEXT_DOCUMENTO") = True Then
                    stru_produccion_indice.FULTEXT_DOCUMENTO = ""
                Else
                    stru_produccion_indice.FULTEXT_DOCUMENTO = Datset.Tables(0).Rows(0).Item("FULTEXT_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("UBICACION_ARCHIVO") = True Then
                    stru_produccion_indice.UBICACION_ARCHIVO = ""
                Else
                    stru_produccion_indice.UBICACION_ARCHIVO = Datset.Tables(0).Rows(0).Item("UBICACION_ARCHIVO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("UBICACION_CARPETA") = True Then
                    stru_produccion_indice.UBICACION_CARPETA = ""
                Else
                    stru_produccion_indice.UBICACION_CARPETA = Datset.Tables(0).Rows(0).Item("UBICACION_CARPETA")
                End If
                If Datset.Tables(0).Rows(0).IsNull("UBICACION_CAJA") = True Then
                    stru_produccion_indice.UBICACION_CAJA = ""
                Else
                    stru_produccion_indice.UBICACION_CAJA = Datset.Tables(0).Rows(0).Item("UBICACION_CAJA")
                End If
                If Datset.Tables(0).Rows(0).IsNull("UBICACION_STAN") = True Then
                    stru_produccion_indice.UBICACION_STAN = ""
                Else
                    stru_produccion_indice.UBICACION_STAN = Datset.Tables(0).Rows(0).Item("UBICACION_STAN")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_DOCUMENTO_DOCUARCHI_ALMACEN") = True Then
                    stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = 0
                Else
                    stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = Datset.Tables(0).Rows(0).Item("ID_DOCUMENTO_DOCUARCHI_ALMACEN")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_DOCUMENTO_DOCUARCHI_TEMPORAL") = True Then
                    stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_TEMPORAL = 0
                Else
                    stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_TEMPORAL = Datset.Tables(0).Rows(0).Item("ID_DOCUMENTO_DOCUARCHI_TEMPORAL")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ESTADO_DOCUMENTO_ARCHIVO") = True Then
                    stru_produccion_indice.ESTADO_DOCUMENTO_ARCHIVO = 0
                Else
                    stru_produccion_indice.ESTADO_DOCUMENTO_ARCHIVO = Datset.Tables(0).Rows(0).Item("ESTADO_DOCUMENTO_ARCHIVO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("NOMBRE_GABINETE") = True Then
                    stru_produccion_indice.NOMBRE_GABINETE = ""
                Else
                    stru_produccion_indice.NOMBRE_GABINETE = Datset.Tables(0).Rows(0).Item("NOMBRE_GABINETE")
                End If
                If Datset.Tables(0).Rows(0).IsNull("RADICADO_DOCUMENTO") = True Then
                    stru_produccion_indice.RADICADO_DOCUMENTO = ""
                Else
                    stru_produccion_indice.RADICADO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("RADICADO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_PLANTILLA_RADICADO") = True Then
                    stru_produccion_indice.ID_PLANTILLA_RADICADO = 0
                Else
                    stru_produccion_indice.ID_PLANTILLA_RADICADO = Datset.Tables(0).Rows(0).Item("ID_PLANTILLA_RADICADO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("NUMERO_FOLIOS") = True Then
                    stru_produccion_indice.NUMERO_FOLIOS = 0
                Else
                    stru_produccion_indice.NUMERO_FOLIOS = Datset.Tables(0).Rows(0).Item("NUMERO_FOLIOS")
                End If
                If Datset.Tables(0).Rows(0).IsNull("EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE") = True Then
                    stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE = 0
                Else
                    stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE")
                End If
                If Datset.Tables(0).Rows(0).IsNull("CONSECUTIVO_DOCUMENTO") = True Then
                    stru_produccion_indice.CONSECUTIVO_DOCUMENTO = 0
                Else
                    stru_produccion_indice.CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("CONSECUTIVO_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("EXPEDIENTE") = True Then
                    stru_produccion_indice.EXPEDIENTE = ""
                Else
                    stru_produccion_indice.EXPEDIENTE = Datset.Tables(0).Rows(0).Item("EXPEDIENTE")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_TIPO_EXPEDIENTE") = True Then
                    stru_produccion_indice.ID_TIPO_EXPEDIENTE = 0
                Else
                    stru_produccion_indice.ID_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("ID_TIPO_EXPEDIENTE")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_TIPO_UNIDAD_CONSERVACION") = True Then
                    stru_produccion_indice.ID_TIPO_UNIDAD_CONSERVACION = 0
                Else
                    stru_produccion_indice.ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item("ID_TIPO_UNIDAD_CONSERVACION")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_UNIDAD_CONSERVACION") = True Then
                    stru_produccion_indice.ID_UNIDAD_CONSERVACION = 0
                Else
                    stru_produccion_indice.ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item("ID_UNIDAD_CONSERVACION")
                End If
                If Datset.Tables(0).Rows(0).IsNull("UNIDADCONSERVA") = True Then
                    stru_produccion_indice.UNIDADCONSERVA = ""
                Else
                    stru_produccion_indice.UNIDADCONSERVA = Datset.Tables(0).Rows(0).Item("UNIDADCONSERVA")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_CLASE_DOCUMENTO") = True Then
                    stru_produccion_indice.ID_CLASE_DOCUMENTO = 0
                Else
                    stru_produccion_indice.ID_CLASE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_CLASE_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("CLASEDOCUMENTO") = True Then
                    stru_produccion_indice.CLASEDOCUMENTO = ""
                Else
                    stru_produccion_indice.CLASEDOCUMENTO = Datset.Tables(0).Rows(0).Item("CLASEDOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("FECHA_ELABORACION") = True Then
                    stru_produccion_indice.FECHA_ELABORACION = ""
                Else
                    ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(0).Item("FECHA_ELABORACION"),
                                                                      stru_produccion_indice.FECHA_ELABORACION)
                    stru_produccion_indice.FECHA_ELABORACION = Left(stru_produccion_indice.FECHA_ELABORACION, "10")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_TIPO_UNIDAD_DOCUMENTAL") = True Then
                    stru_produccion_indice.ID_TIPO_UNIDAD_DOCUMENTAL = 0
                Else
                    stru_produccion_indice.ID_TIPO_UNIDAD_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("ID_TIPO_UNIDAD_DOCUMENTAL")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_EMPRESA_DOCUMENTO") = True Then
                    stru_produccion_indice.ID_EMPRESA_DOCUMENTO = 0
                Else
                    stru_produccion_indice.ID_EMPRESA_DOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_EMPRESA_DOCUMENTO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("DOCUMENTO_PRODUCION_DOCUMENTAL") = True Then
                    stru_produccion_indice.DOCUMENTO_PRODUCION_DOCUMENTAL = 0
                Else
                    stru_produccion_indice.DOCUMENTO_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("DOCUMENTO_PRODUCION_DOCUMENTAL")
                End If
                If Datset.Tables(0).Rows(0).IsNull("FORMATO") = True Then
                    stru_produccion_indice.FORMATO = ""
                Else
                    stru_produccion_indice.FORMATO = Datset.Tables(0).Rows(0).Item("FORMATO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("TAMANO") = True Then
                    stru_produccion_indice.TAMANO = ""
                Else
                    stru_produccion_indice.TAMANO = Datset.Tables(0).Rows(0).Item("TAMANO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ORIGEN") = True Then
                    stru_produccion_indice.ORIGEN = ""
                Else
                    stru_produccion_indice.ORIGEN = Datset.Tables(0).Rows(0).Item("ORIGEN")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ESTADO_ELIMINA_PRODUCION_DOCUMENTAL") = True Then
                    stru_produccion_indice.ESTADO_ELIMINA_PRODUCION_DOCUMENTAL = 0
                Else
                    stru_produccion_indice.ESTADO_ELIMINA_PRODUCION_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("ESTADO_ELIMINA_PRODUCION_DOCUMENTAL")
                End If
                If Datset.Tables(0).Rows(0).IsNull("SEGUNDO_NOMBRE_DOCUMENTO") = True Then
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = ""
                Else
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = Datset.Tables(0).Rows(0).Item("SEGUNDO_NOMBRE_DOCUMENTO")
                End If
                Solicita_estructura_id_registro_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_id_registro_produccion = "Inconsistencia general funcion Solicita_estructura_id_registro_produccion " & ex.Message
        End Try

    End Function
    Function Lista_estructura_carpeta(ByVal id_usuario_gestion As Integer,
                                      ByRef treview As TreeView,
                                      ByRef up_date As UpdatePanel) As String
        Dim trenode_principal As New TreeNode
        Try
            Dim Result As String = ""
            Dim stru_exp_principal() As STRU_LISTA_EXP_PRDUCION = Nothing
            treview.Nodes.Clear()
            trenode_principal.Text = "Expedientes-Carpetas"
            Result = Me.Solicita_lista_expedientes_producion_documental(id_usuario_gestion,
                                                                        stru_exp_principal)
            If Result <> "YES" Then
                Lista_estructura_carpeta = Result
                Exit Function
            End If
            If stru_exp_principal Is Nothing Then
                Lista_estructura_carpeta = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_exp_principal.Length - 1
                Dim trenodechild As New TreeNode
                trenodechild.Text = stru_exp_principal(i).NOMBRE_EXPEDIENTE
                If stru_exp_principal(i).SERIE <> "" Then
                    trenodechild.Text = trenodechild.Text '& " (" & stru_exp_principal(i).SERIE & ")"
                    trenodechild.ToolTip = " (" & stru_exp_principal(i).SERIE & ")"
                End If
                If stru_exp_principal(i).SUBSERIE <> "" Then
                    trenodechild.Text = trenodechild.Text '& " (" & stru_exp_principal(i).SUBSERIE & ")"
                    trenodechild.ToolTip = " (" & stru_exp_principal(i).SUBSERIE & ")"
                End If
                trenodechild.Value = stru_exp_principal(i).ID_EXPEDIENTE
                trenodechild.ImageUrl = "../Gestion/imagenes/folder-regular.png"
                Dim stru_exp_segundario() As STRU_LISTA_EXP_PRDUCION = Nothing
                Result = Me.Solicita_expedientes_relacionados_producion_documental(stru_exp_principal(i).ID_EXPEDIENTE, stru_exp_segundario)
                If Result <> "YES" Then
                    Lista_estructura_carpeta = Result
                    Exit Function
                End If
                If Not stru_exp_segundario Is Nothing Then
                    For z As Integer = 0 To stru_exp_segundario.Length - 1
                        Dim trenodechild_segundo As New TreeNode
                        trenodechild_segundo.Text = stru_exp_segundario(z).NOMBRE_EXPEDIENTE
                        If stru_exp_segundario(z).SERIE <> "" Then
                            trenodechild_segundo.Text = trenodechild_segundo.Text '& " (" & stru_exp_segundario(z).SERIE & ")"
                            trenodechild_segundo.ToolTip = " (" & stru_exp_segundario(z).SERIE & ")"
                        End If
                        If stru_exp_segundario(z).SUBSERIE <> "" Then
                            trenodechild_segundo.Text = trenodechild_segundo.Text '& " (" & stru_exp_segundario(z).SUBSERIE & ")"
                            trenodechild_segundo.ToolTip = " (" & stru_exp_segundario(z).SUBSERIE & ")"
                        End If
                        trenodechild_segundo.Value = stru_exp_segundario(z).ID_EXPEDIENTE
                        trenodechild_segundo.ImageUrl = "../Gestion/imagenes/folder-regular.png"
                        trenodechild.ChildNodes.Add(trenodechild_segundo)
                    Next

                End If
                trenode_principal.ChildNodes.Add(trenodechild)
            Next
            Lista_estructura_carpeta = "YES"
        Catch ex As Exception
            Lista_estructura_carpeta = "Inconsistencia general funcion Lista_estructura_carpeta " & ex.Message
        Finally
            treview.Nodes.Add(trenode_principal)
            up_date.Update()
        End Try
    End Function
    Function Solicita_expedientes_relacionados_producion_documental(ByVal id_expediente_padre As Integer,
                                                                    ByRef stru() As STRU_LISTA_EXP_PRDUCION) As String
        Try
            Dim Parametro_Consulta As String = "select rrer.ID_EXPEDIENTE_HIJO,ALEAS_EXPEDIENTE,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD " &
                                      " from ra_pro_relacion_exp_produccion as rrer " &
                                      " inner join expediente_archivo as ea on (rrer.ID_EXPEDIENTE_HIJO=ea.ID_EXPEDIENTE) " &
                                      " where ID_EXPEDIENTE_PADRE=" & id_expediente_padre
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("ra_pro_relacion_exp_produccion")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_expedientes_relacionados_producion_documental = "Función Solicita_lista_expedientes_producion_documental dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_expedientes_relacionados_producion_documental = "YES"
                Exit Function
            Else
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_EXPEDIENTE = datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_EXPEDIENTE = datset.Tables(0).Rows(i).Item(1)
                    If datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru(i).SERIE = ""
                    Else
                        stru(i).SERIE = datset.Tables(0).Rows(i).Item(2)
                    End If
                    If datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru(i).SUBSERIE = ""
                    Else
                        stru(i).SUBSERIE = datset.Tables(0).Rows(i).Item(3)
                    End If
                Next
                Solicita_expedientes_relacionados_producion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_expedientes_relacionados_producion_documental = "Inconsistencia general función Solicita_expedientes_relacionados_producion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_lista_expedientes_producion_documental(ByVal id_usuario_gestion As Integer,
                                                             ByRef stru() As STRU_LISTA_EXP_PRDUCION) As String
        Try
            Dim Parametro_Consulta As String = "select ID_EXPEDIENTE,ALEAS_EXPEDIENTE,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD " &
                                      "from expediente_archivo where ID_USUARIO_GESTION=" & id_usuario_gestion & " AND Estado_Publico_Sub_Expediente=2 and expediente_relacion_producion=0"
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("expediente_archivo")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_lista_expedientes_producion_documental = "Función Solicita_lista_expedientes_producion_documental dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                stru = Nothing
                Solicita_lista_expedientes_producion_documental = "YES"
                Exit Function
            Else
                stru = Nothing
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).ID_EXPEDIENTE = datset.Tables(0).Rows(i).Item(0)
                    stru(i).NOMBRE_EXPEDIENTE = datset.Tables(0).Rows(i).Item(1)
                    If datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru(i).SERIE = ""
                    Else
                        stru(i).SERIE = datset.Tables(0).Rows(i).Item(2)
                    End If
                    If datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru(i).SUBSERIE = ""
                    Else
                        stru(i).SUBSERIE = datset.Tables(0).Rows(i).Item(3)
                    End If
                Next
                Solicita_lista_expedientes_producion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_expedientes_producion_documental = "Inconsistencia general función Solicita_lista_expedientes_producion_documental " & ex.Message
        End Try
    End Function

    Function Solicita_identificacion_serie_documental(ByVal nombre_serie_documental As String,
                                                      ByVal id_area_usuario_gestion As Integer,
                                                      ByRef id_serie_documental As Integer) As String
        Try
            Dim Parametro_Consulta = "select Id_Series " &
         " from series_documentales WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area_usuario_gestion &
         " and Nombre_Serie='" & nombre_serie_documental & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_serie_documental = "Funcion  Solicita_identificacion_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_serie_documental = "Imposible encontrar la identificación de las serie documental " & nombre_serie_documental
                Exit Function
            Else
                id_serie_documental = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_serie_documental = "Inconsistencia general función Solicita_identificacion_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_identificacion_sub_serie_documental(ByVal nombre_sub_serie_documental As String,
                                                          ByVal identificacion_serie As Integer,
                                                          ByRef identificacion_sub_serie As Integer) As String
        Try
            Dim Parametro_Consulta = "select Id_SubSeries " &
         " from subseries_documentales WHERE Series_Documentales_Id_Series=" & identificacion_serie &
         " and Nombre_Subserie='" & nombre_sub_serie_documental & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_sub_serie_documental = "Funcion  Solicita_identificacion_sub_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_sub_serie_documental = "Imposible encontrar la identificación de la sub serie documental " & nombre_sub_serie_documental
                Exit Function
            Else
                identificacion_sub_serie = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_sub_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_sub_serie_documental = "Inconsistencia general función Solicita_identificacion_sub_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_sub_series_documentales_relacionadas_a_la_serie_documental(ByVal id_serie_documental As Integer,
                                                                                 ByRef drop_lis As DropDownList,
                                                                                 ByRef ref_update As UpdatePanel) As String

        Try
            drop_lis.Items.Clear()
            drop_lis.Items.Add("")
            Dim Parametro_Consulta = "select NOMBRE_SUBSERIE FROM SUBSERIES_DOCUMENTALES " &
            " WHERE Series_Documentales_Id_Series=" & id_serie_documental
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental = "Funcion  Solicita_sub_series_documentales_relacionadas_a_la_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drop_lis.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_sub_series_documentales_relacionadas_a_la_serie_documental = "Inconsistencia general función Solicita_sub_series_documentales_relacionadas_a_la_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item(ByVal id_serie_documental As Integer,
                                                                                      ByRef drop_lis As DropDownList,
                                                                                      ByRef ref_update As UpdatePanel) As String

        Try
            drop_lis.Items.Clear()
            Dim Parametro_Consulta = "select Id_SubSeries,Nombre_Subserie FROM SUBSERIES_DOCUMENTALES " &
            " WHERE Series_Documentales_Id_Series=" & id_serie_documental
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item = "Funcion  Solicita_sub_series_documentales_relacionadas_a_la_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item = "YES"
                Exit Function
            Else
                Dim ilis_ As System.Web.UI.WebControls.ListItem
                ilis_ = New System.Web.UI.WebControls.ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_lis.Items.Add(ilis_)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New System.Web.UI.WebControls.ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_lis.Items.Add(ilis_)
                Next
                Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item = "Inconsistencia general función Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_series_relacionadas_al_area_del_usuario_gestion(ByVal id_area As Integer,
                                                                      ByRef drop_lis As DropDownList,
                                                                      ByRef drop_list_subseries As DropDownList,
                                                                      ByRef ref_update As UpdatePanel) As String
        Try
            drop_lis.Items.Clear()
            drop_list_subseries.Items.Clear()
            drop_lis.Items.Add("")
            Dim Parametro_Consulta = "select NOMBRE_SERIE " &
           " from series_documentales WHERE Areas_Depart_Radicacion_Codigo_Area=" & id_area
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_series_relacionadas_al_area_del_usuario_gestion = "Funcion  Solicita_series_relacionadas_al_area_del_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_series_relacionadas_al_area_del_usuario_gestion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drop_lis.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Solicita_series_relacionadas_al_area_del_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_series_relacionadas_al_area_del_usuario_gestion = "Inconsistencia general función Solicita_series_relacionadas_al_area_del_usuario_gestion " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function

    Function Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie(ByVal id_sub_serie_documental As Integer,
                                                                               ByRef numero_tipos_relacionados As Integer) As String
        Try
            Dim Parametro_Consulta = "select Descripcion_Documento " &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & id_sub_serie_documental & " and  Estado_Tipo=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie = "Funcion  Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_tipos_relacionados = 0
                Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie = "YES"
                Exit Function
            Else
                numero_tipos_relacionados = Datset.Tables(0).Rows.Count
                Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie = "Inconsistencia general función Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie " & ex.Message
        End Try
    End Function
    Function Solicita_numero_tipos_de_documentos_relacionados_con_la_serie(ByVal id_serie_documental As Integer,
                                                                           ByRef numero_tipos_relacionados As Integer) As String
        Try
            Dim Parametro_Consulta = "select Series_Documentales_Id_Series " &
          " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & id_serie_documental & " and  Estado_Tipo=1 and sub_serie_id_serie is null"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_tipos_de_documentos_relacionados_con_la_serie = "Funcion  Solicita_numero_tipos_de_documentos_relacionados_con_la_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_tipos_relacionados = 0
                Solicita_numero_tipos_de_documentos_relacionados_con_la_serie = "YES"
                Exit Function
            Else
                numero_tipos_relacionados = Datset.Tables(0).Rows.Count
                Solicita_numero_tipos_de_documentos_relacionados_con_la_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_tipos_de_documentos_relacionados_con_la_serie = "Inconsistencia general función Solicita_numero_tipos_de_documentos_relacionados_con_la_serie " & ex.Message
        End Try
    End Function



    Function Determina_level_nodo_selecionado(ByVal tre_view As TreeView,
                                              ByRef tre_node As TreeNode,
                                              ByRef index_nodo As Integer) As String
        Try
            Dim Result As String = ""
            If tre_view.SelectedNode Is Nothing Then
                index_nodo = -1
                Determina_level_nodo_selecionado = "YES"
                Exit Function
            Else
                If tre_node Is Nothing Then
                    tre_node = tre_view.SelectedNode
                End If
            End If
            If Not tre_node.Parent Is Nothing Then
                index_nodo = index_nodo + 1
                tre_node = tre_node.Parent
                Result = Determina_level_nodo_selecionado(tre_view, tre_node, index_nodo)
                If Result <> "YES" Then
                    Determina_level_nodo_selecionado = Result
                    Exit Function
                End If
            Else
                Determina_level_nodo_selecionado = "YES"
                Return Determina_level_nodo_selecionado
                Exit Function
            End If
        Catch ex As Exception
            Determina_level_nodo_selecionado = "Inconsistencia general función Determina_level_nodo_selecionado " & ex.Message
        End Try
    End Function
    Function SolicitaCargarDocumentoExpediente(ByRef CDexpedienteSeleccionado As CDexpedienteSeleccionado) As String
        Try
            Dim ClassGaExpediente As New ClassGaExpediente
            If InStr(HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                SolicitaCargarDocumentoExpediente = "Por favor, seleccione el expediente al cual desea adjuntar el documento."
                Exit Function
            End If
            Dim split() As String = HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            CDexpedienteSeleccionado.IdExpediente = Val(split(2))
            CDexpedienteSeleccionado.IdNivelExpediente = Val(split(1))
            CDexpedienteSeleccionado.IdUsuarioGestion = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            Dim EstadoExpediente As Integer = 0
            Dim EstadoPublico As Integer = 0
            Dim Result As String = ClassGaExpediente.Retorna_estado_expediente(CDexpedienteSeleccionado.IdExpediente,
                                                                               EstadoExpediente,
                                                                               EstadoPublico)
            If Result <> "YES" Then
                SolicitaCargarDocumentoExpediente = Result
                Exit Function
            End If
            If EstadoExpediente <> 1 Then
                SolicitaCargarDocumentoExpediente = "No es posible adjuntar el documento al expediente porque este se encuentra cerrado."
                Exit Function
            End If
            Dim Class_ra_pro_niveles As New Class_ra_pro_niveles
            Dim Class_ra_pro_permisos_niveles As New Class_ra_pro_permisos_niveles
            Dim EstadoPropietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Class_ra_pro_niveles.Solicita_estado_nivel_propietario(CDexpedienteSeleccionado.IdUsuarioGestion,
                                                                            CDexpedienteSeleccionado.IdNivelExpediente,
                                                                            EstadoPropietario)
            If Result <> "YES" Then
                SolicitaCargarDocumentoExpediente = Result
                Exit Function
            End If
            If EstadoPropietario = "NO" Then
                Result = Class_ra_pro_permisos_niveles.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(CDexpedienteSeleccionado.IdNivelExpediente,
                                                                                                              CDexpedienteSeleccionado.IdUsuarioGestion,
                                                                                                              stru_permisos_niveles)
                If Result <> "YES" Then
                    SolicitaCargarDocumentoExpediente = Result
                    Exit Function
                End If
                If stru_permisos_niveles.carga_archivo = 0 Then
                    SolicitaCargarDocumentoExpediente = "El usuario no tiene permisos para cargar archivos al expediente, ya que el nivel al que pertenece dicho expediente es propiedad de otro usuario."
                    Exit Function
                End If
            End If
            SolicitaCargarDocumentoExpediente = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaCargarDocumentoExpediente = "Inconsistencia general funcion SolicitaAgregarDocumentoExpediente " & ex.Message
        End Try
    End Function
    Function SolicitaListaTipologiasExpediente(ByVal IdExpediente As Integer,
                                               ByRef Control_drow_lista As List(Of control_drow_lista)) As String
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim EstruUnidadConservacion() As expediente_conservacion = Nothing
            Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(IdExpediente,
                                                                         EstruUnidadConservacion)
            If Result <> "YES" Then
                SolicitaListaTipologiasExpediente = Result
                Exit Function
            End If
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If EstruUnidadConservacion(0).CODIGO_SUBSERIE <> 0 Then
                Result = Class_ra_tipo_doc_series.SolicitaTiposDocumentalesRelacionadosSubSerie(EstruUnidadConservacion(0).CODIGO_SUBSERIE,
                                                                                                Control_drow_lista)
                If Result <> "YES" Then
                    SolicitaListaTipologiasExpediente = Result
                    Exit Function
                End If
            Else
                Result = Class_ra_tipo_doc_series.SolicitaTiposDocumentalesRelacionadosSerie(EstruUnidadConservacion(0).CODIGO_SERIE,
                                                                                             Control_drow_lista)
                If Result <> "YES" Then
                    SolicitaListaTipologiasExpediente = Result
                    Exit Function
                End If
            End If
            SolicitaListaTipologiasExpediente = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaListaTipologiasExpediente = "Inconsistencia general funcion SolicitaListaTipologiasExpediente " & ex.Message
        End Try
    End Function
    Function Solicitar_agregar_documento_a_carpeta_expediente(ByVal id_expediente As Integer,
                                                              ByVal id_usuario_gestion As Integer,
                                                              ByVal id_nivel As Integer,
                                                              ByRef ref_droplist As DropDownList,
                                                              ByRef ref_update As UpdatePanel) As String

        Try
            'text_box.Text = ""
            'TextBox_nombre_archivo.Text = ""
            '----------------------------------------------------
            'Solicita opciones de aplicacion tablas de retención
            '----------------------------------------------------
            Dim Result As String = ""
            Dim Refclas_expediente As New ClassGaExpediente
            Dim Refclas_tipo_doc As New Class_ra_tipo_doc_series
            'Dim stru_config As STRU_CONFIG_PRODUCION
            'Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            'Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            'If Result <> "YES" Then
            '    Solicitar_agregar_documento_a_carpeta_expediente = Result
            '    Exit Function
            'End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Solicitar_agregar_documento_a_carpeta_expediente = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Solicitar_agregar_documento_a_carpeta_expediente = Result
                    Exit Function
                End If
                If stru_permisos_niveles.carga_archivo = 0 Then
                    Solicitar_agregar_documento_a_carpeta_expediente = "El usuario no tiene permisos para cargar archivos al expediente, el nivel al que pertenece el expediente es propiedad de otro usuario"
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Retorna datos estructura expediente 
            '----------------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                 estru_unidad_conservacion)
            If Result <> "YES" Then
                Solicitar_agregar_documento_a_carpeta_expediente = Result
                Exit Function
            End If
            ref_droplist.Items.Clear()
            If estru_unidad_conservacion(0).CODIGO_SUBSERIE <> 0 Then
                '---------------------------------------------------------------------
                'Solicita tipos documentales relacionados a la sub serie documental
                '---------------------------------------------------------------------
                Result = Refclas_tipo_doc.Solicita_tipos_documentales_relacionados_a_la_sub_series(estru_unidad_conservacion(0).CODIGO_SUBSERIE,
                                                                                                   ref_droplist,
                                                                                                   ref_update,
                                                                                                   0,
                                                                                                   estru_unidad_conservacion(0).CODIGO_SERIE)
                If Result <> "YES" Then
                    Solicitar_agregar_documento_a_carpeta_expediente = Result
                    Exit Function
                End If
            Else
                If estru_unidad_conservacion(0).CODIGO_SERIE <> 0 Then
                    '-----------------------------------------------------------------
                    'Solicita tipos documentales relacionados a la serie documental
                    '-----------------------------------------------------------------
                    Result = Refclas_tipo_doc.Solicita_tipos_documentales_relacionados_a_la_series(estru_unidad_conservacion(0).CODIGO_SERIE,
                                                                                                   ref_droplist,
                                                                                                   ref_update,
                                                                                                   0)
                    If Result <> "YES" Then
                        Solicitar_agregar_documento_a_carpeta_expediente = Result
                        Exit Function
                    End If
                End If
            End If
            Solicitar_agregar_documento_a_carpeta_expediente = "YES"
        Catch ex As Exception
            Solicitar_agregar_documento_a_carpeta_expediente = "Inconsistencia general función Solicitar_agregar_documento_a_carpeta_expediente " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function

    Function Solicita_listado_extension_de_archivos_permitidas(ByRef extensiones_permitidas As String,
                                                               Optional ByVal dev_punto As String = "") As String
        Try
            Dim Parametro_Consulta = "select ESTENSION " &
            " from da_extension "
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_extension_de_archivos_permitidas = "Funcion  Solicita_listado_extension_de_archivos_permitidas dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_listado_extension_de_archivos_permitidas = "El sistema no pudo encontrar la configuración de las extensiones de archivos permitidas"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If i = 0 Then
                        extensiones_permitidas = Datset.Tables(0).Rows(i).Item(0).ToString.Replace(".", dev_punto)
                    Else
                        extensiones_permitidas = extensiones_permitidas & "," & Datset.Tables(0).Rows(i).Item(0).ToString.Replace(".", dev_punto)
                    End If
                Next
                Solicita_listado_extension_de_archivos_permitidas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_extension_de_archivos_permitidas = "Inconsistencia general función Solicita_listado_extension_de_archivos_permitidas"
        End Try
    End Function
    Function Elimina_fila_gre(
                             ByVal id_parametro_elimnar As String,
                             ByVal nombre_campo_eliminar As String
                             ) As String
        Try
            If HttpContext.Current.Session.Item("DATA_SET_SESION") Is Nothing Then
                Elimina_fila_gre = "YES"
                Exit Function
            End If
            Dim ob As DataSet = HttpContext.Current.Session.Item("DATA_SET_SESION")
            Dim p2 As DataRow = Nothing
            For Each p As DataRow In ob.Tables(0).Rows
                If p.Item(nombre_campo_eliminar) = id_parametro_elimnar Then
                    'p.Delete()
                    p2 = p
                    Exit For
                End If
            Next
            If Not p2 Is Nothing Then
                ob.Tables(0).Rows.Remove(p2)
            End If
            ob.Tables(0).AcceptChanges()
            HttpContext.Current.Session.Item("DATA_SET_SESION") = ob
            Elimina_fila_gre = "YES"
        Catch ex As Exception
            Elimina_fila_gre = "Inconsistencia general función Elimina_fila_gre " & ex.Message
        End Try
    End Function
    Function Agrega_fila_gre(ByRef ref_gridview As GridView,
                             ByRef campo() As STRU_CAMPOS_GRIDVIEW,
                             ByRef ref_update As UpdatePanel, Optional ByVal option_update As Integer = 0) As String
        Try
            If HttpContext.Current.Session.Item("DATA_SET_SESION") Is Nothing Then
                Agrega_fila_gre = "YES"
                Exit Function
            End If
            Dim ref_titulo_label_grid As Label = Nothing
            Dim Hidden_nureg As Object = Nothing
            Dim UpdatePanel_label_resultado As UpdatePanel = Nothing
            If option_update = 1 Then
                ref_titulo_label_grid = HttpContext.Current.Session.Item("GA_ROMULARIO_WEB").FindControl("titulo_label_grid")
                Hidden_nureg = HttpContext.Current.Session.Item("GA_ROMULARIO_WEB").FindControl("Hidden_nureg")
                UpdatePanel_label_resultado = HttpContext.Current.Session.Item("GA_ROMULARIO_WEB").FindControl("UpdatePanel_label_resultado")
            Else
                ref_titulo_label_grid = ref_update.Page.FindControl("titulo_label_grid")
                Hidden_nureg = ref_update.Page.FindControl("Hidden_nureg")
                UpdatePanel_label_resultado = ref_update.Page.FindControl("UpdatePanel_label_resultado")
            End If
            Dim dt As New DataTable
            Dim dr As DataRow
            Dim ob As DataSet = HttpContext.Current.Session.Item("DATA_SET_SESION")
            Dim data_row As DataRow
            data_row = ob.Tables(0).NewRow()
            For i As Integer = 0 To campo.Length - 1
                If campo(i).valor_campo <> "" Then
                    data_row.Item(i) = campo(i).valor_campo
                End If
            Next
            ob.Tables(0).Rows.InsertAt(data_row, 0)
            ob.Tables(0).AcceptChanges()
            ref_gridview.DataSource = ob
            HttpContext.Current.Session.Item("DATA_SET_SESION") = ob
            ref_gridview.DataBind()
            'Aplica_estilo_fecha_gred(ob, ref_gridview)
            For i As Integer = 0 To ref_gridview.Rows.Count - 1
                ref_gridview.Rows(i).Attributes.Add("id", ref_gridview.Rows(i).Cells(1).Text.ToString())
                ref_gridview.Rows(i).Cells(3).Text = Left(ref_gridview.Rows(i).Cells(3).Text, 10)
                ref_gridview.Rows(i).Cells(0).Style.Add("text-align", "center")
                ref_gridview.Rows(i).Cells(0).Style.Add("width", "5px")
                For z As Integer = 1 To ref_gridview.Rows(i).Cells.Count - 1
                    ref_gridview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                    ref_gridview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                Next
            Next
            ref_titulo_label_grid.Text = " " & ref_gridview.Rows.Count & " archivo(s) encontrado(s)"
            Hidden_nureg.value = ref_gridview.Rows.Count
            Dim Refclas As New ClassGredview
            Dim Result As String = Refclas.add_clase_acender_decender(HttpContext.Current.Session.Item("SortExpression_produccion"),
                                                                      HttpContext.Current.Session.Item("Sort_matri_colum_produccion"),
                                                                      HttpContext.Current.Session.Item("SortDirection_produccion"),
                                                                      ref_gridview)
            If Result <> "YES" Then
                Agrega_fila_gre = "Error add clase función  Solicita_listado_usuario_permisos_nivel " & Result
                Exit Function
            End If
            If option_update = 0 Then
                ref_update.Update()
                UpdatePanel_label_resultado.Update()
            End If
            Agrega_fila_gre = "YES"
        Catch ex As Exception
            Agrega_fila_gre = "Inconsistencia general función Agrega_fila_gre " & ex.Message
        End Try
    End Function
    Function Activa_copia_documento_expediente(ByVal id_usuario_gestion As Integer,
                                               ByVal id_documento_producion As Integer,
                                               ByRef Selecion_copia As Long) As String
        Try
            Dim Result As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim Fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_documento_producion,
                                                                          nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_clase_documento,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          Fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Activa_copia_documento_expediente = Result
                Exit Function
            End If
            Dim Id_nivel As Integer = 0
            Dim Ref_class_ra_pro_nivel As New Class_ra_pro_niveles_has_expediente_archivo
            Result = Ref_class_ra_pro_nivel.Solicita_id_nivel_expediente(id_expediente,
                                                                         Id_nivel)
            If Result <> "YES" Then
                Activa_copia_documento_expediente = Result
                Exit Function
            End If
            Dim Ref_clas_ra_pro_nivel As New Class_ra_pro_niveles
            Dim estado_nivel_propietario As String = ""
            Result = Ref_clas_ra_pro_nivel.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                             Id_nivel,
                                                                             estado_nivel_propietario)
            If Result <> "YES" Then
                Activa_copia_documento_expediente = Result
                Exit Function
            End If
            Dim Ref_class_ra_pro_permisos_niveles As New Class_ra_pro_permisos_niveles
            Dim stru_permiso_nivel As stru_permiso_nivel
            If estado_nivel_propietario = "NO" Then
                Result = Ref_class_ra_pro_permisos_niveles.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(Id_nivel,
                                                                                                                  id_usuario_gestion,
                                                                                                                  stru_permiso_nivel)
                If Result <> "YES" Then
                    Activa_copia_documento_expediente = Result
                    Exit Function
                End If
                If stru_permiso_nivel.copiar_archivo = 0 Then
                    Activa_copia_documento_expediente = "El usuario no tiene permiso para copiar archivos del expediente"
                    Exit Function
                End If
            End If
            Selecion_copia = id_documento_producion
            Activa_copia_documento_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Activa_copia_documento_expediente = "Inconsistencia general función Activa_copia_documento_expediente " & ex.Message
        End Try
    End Function
    Function activa_copia_service(ByVal id_imagen As Long,
                                  ByRef valor_campos As String,
                                  ByVal tipo_copia As Integer,
                                  ByVal id_flujo_wf As Long,
                                  ByVal radicado As String,
                                  ByVal obliga_actualiza_indice_expediente_gabinete As Integer,
                                  ByVal wf_copia_doc_produc_expediente_actualiza_exped_gabinete As Integer) As String
        Try

            If HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                activa_copia_service = "Por favor seleccione un expediente para copiar"
                Exit Function
            End If
            If InStr(HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                activa_copia_service = "Debe seleccionar un expediente para copiar el archivo"
                Exit Function
            End If
            Dim split() As String = HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Dim Result As String = ""
            Dim Ref_producion As New ClassGaProducionDocumental
            Result = Ref_producion.Copia_documento_expediente_produccion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                         id_imagen,
                                                                         Val(split(2)),
                                                                         valor_campos,
                                                                         tipo_copia,
                                                                         id_flujo_wf,
                                                                         radicado,
                                                                         obliga_actualiza_indice_expediente_gabinete,
                                                                         wf_copia_doc_produc_expediente_actualiza_exped_gabinete)
            If Result <> "YES" Then
                activa_copia_service = Result
                Exit Function
            End If
            activa_copia_service = "YES"
        Catch ex As Exception
            activa_copia_service = "Inconsistencia general función activa_copia_service " & ex.Message
        End Try
    End Function
    Function Copia_documento_expediente_produccion(ByVal id_usuario_gestion As Integer,
                                                   ByRef id_documento_producion As Long,
                                                   ByVal id_expediente As Integer,
                                                   ByRef campos_valores As String,
                                                   ByVal tipo_copia As Integer,
                                                   ByVal id_tarea_wf As Long,
                                                   ByVal radicado_wf As String,
                                                   ByVal obliga_actualiza_indice_expediente_gabinete As Integer,
                                                   ByVal wf_copia_doc_expediente_actualiza_exped_gabinete As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Copia los documentos desde workflow hasta la estructura de la pro
        '          ducción documental 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_gestion    : Representa la identificación del usuario de gestion
        'id_expediente         : Representa la identificación del expediente 
        'tipo_copia            : Representa el tipo de copia del documento 
        'valor (1) copia desde workflow, valida exitencia de documento copiado y reg
        'istra en la tabla ra_rel_copia_wf_produccion el registro de lacopia de la
        'imagen, para no repetir el proceso
        'id_tarea_wf           : Representa la identificación de la tarea workflow
        'radicado_wf           : Rrepresenta el radicado de la tarea a copiar
        'obliga_actualiza_indice: 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'campos_valores        : Retorna los campos valores de copia
        'id_documento_producion: Retorna la identificación del documento de producción
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-10-17
        'Elabora               : Miguel Angel Urueta Miranda
        'Fecha revisa          : 2023-11-17 
        'Usuario revisa        : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente_ As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim Fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim nombre_documento_radicado As String = ""
            Dim registro_radicado As String = ""
            '--------Solicita el registro del radicado de la producción documental
            Result = Me.Solicita_documento_radicado_produccion(id_documento_producion,
                                                               nombre_documento_radicado,
                                                               registro_radicado)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            If tipo_copia = 0 Then
                If nombre_documento_radicado <> "" Then
                    Copia_documento_expediente_produccion = "El documento (" & nombre_documento_radicado & ") se encuentra relacionado radicado(" & registro_radicado & ") , imposible copiar "
                    Exit Function
                End If
            End If
            '------Solicita los datos de caracterización del documento
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_documento_producion,
                                                                          nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_clase_documento,
                                                                          id_expediente_,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          Fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            '--------- Verifica la existencia de copias de archivo workflow con la tarea 
            Dim ref_Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
            Dim exitencia_copia_wf As String = ""
            If tipo_copia = 1 Then
                Result = ref_Class_ra_rel_copia_wf_produccion.Solicita_existencia_copia_estructura_expediente_workflow(id_imagen,
                                                                                                                       nombre_gabinete,
                                                                                                                       id_tarea_wf,
                                                                                                                       id_expediente,
                                                                                                                       exitencia_copia_wf)
                If Result <> "YES" Then
                    Copia_documento_expediente_produccion = Result
                    Exit Function
                End If
                If exitencia_copia_wf = "YES" Then
                    campos_valores = ""
                    Copia_documento_expediente_produccion = "YES"
                    Exit Function
                End If
            End If
            Dim Id_nivel As Integer = 0
            Dim Ref_class_ra_pro_nivel As New Class_ra_pro_niveles_has_expediente_archivo
            Result = Ref_class_ra_pro_nivel.Solicita_id_nivel_expediente(id_expediente,
                                                                         Id_nivel)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim Ref_clas_ra_pro_nivel As New Class_ra_pro_niveles
            Dim estado_nivel_propietario As String = ""
            Result = Ref_clas_ra_pro_nivel.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                             Id_nivel,
                                                                             estado_nivel_propietario)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim Ref_class_ra_pro_permisos_niveles As New Class_ra_pro_permisos_niveles
            Dim stru_permiso_nivel As stru_permiso_nivel
            If estado_nivel_propietario = "NO" Then
                Result = Ref_class_ra_pro_permisos_niveles.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(Id_nivel,
                                                                                                                  id_usuario_gestion,
                                                                                                                  stru_permiso_nivel)
                If Result <> "YES" Then
                    Copia_documento_expediente_produccion = Result
                    Exit Function
                End If
                If stru_permiso_nivel.copiar_archivo = 0 Then
                    Copia_documento_expediente_produccion = "El usuario no tiene permiso para copiar archivos en el expediente destino"
                    Exit Function
                End If
            End If
            Dim refclas_expediente As New ClassGaExpediente
            '----------------------------------------------
            'Retorna el estado del expediente carpeta
            '----------------------------------------------
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Result = refclas_expediente.Retorna_estado_expediente(id_expediente,
                                                                  estado_expediente,
                                                                  estado_publico)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            If estado_expediente <> 1 Then
                Copia_documento_expediente_produccion = "No se puede copiar el documento a la carpeta o expediente por que está cerrado"
                Exit Function
            End If
            '---------------------------------------------
            'Valida la configuración del gabinete
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_gabinete_destino As String = "PRODUCIONDOC"
            Result = refclas_expediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                               nombre_gabinete_destino)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete_destino,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Copia_documento_expediente_produccion = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Copia_documento_expediente_produccion = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Copia_documento_expediente_produccion = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción asinar unidad documental"
                Exit Function
            End If
            '---------------------------------------------
            'Solicita las opciones de produción documental
            '---------------------------------------------
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If nombre_tipo_documental = "" Then
                    Copia_documento_expediente_produccion = "Debe seleccionar el tipo documental del archivo que desea copiar"
                    Exit Function
                End If
            End If
            Dim matri_documentos_almacenados() As String = Nothing
            Dim Refclas As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete,
                                                                                     matri_documentos_almacenados)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(matri_documentos_almacenados(1))
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If

            Dim icono As String = ""
            ClassDaGabinete.Agrega_icono_image_fownt_java(id_tipo_archivo.ToString,
                                                          icono)
            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                        estru_unidad_conservacion)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            If tipo_copia = 1 Then
                Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_tarea_wf,
                                                                                                    stru_estado)
                If Result <> "YES" Then
                    Copia_documento_expediente_produccion = Result
                    Exit Function
                End If
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim matri_documentos() As String = Nothing
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim date_trans As Object = ""
            ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(date_trans)
            Result = Me.Retorna_parametros_almacenamiento_documento_adjunto(id_expediente,
                                                                            matri_datos_almacen,
                                                                            matri_gestion,
                                                                            matri_documentos,
                                                                            nombre_gabinete_destino,
                                                                            nombre_archivo,
                                                                            estru_unidad_conservacion,
                                                                            "",
                                                                            nombre_tipo_documental,
                                                                            id_clase_documento,
                                                                            1,
                                                                            obliga_actualiza_indice_expediente_gabinete,
                                                                            wf_copia_doc_expediente_actualiza_exped_gabinete)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            If tipo_copia = 1 Then
                If nombre_documento_radicado = "" Then
                    nombre_archivo = radicado_wf
                End If
            End If
            Erase matri_documentos
            For i As Integer = 1 To matri_documentos_almacenados.Length - 1
                ReDim Preserve matri_documentos(i - 1)
                matri_documentos(i - 1) = matri_documentos_almacenados(i)
            Next
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_imagen_ As Integer = 0
            Dim radicado As String = ""
            Dim id_registro As Integer = 0
            Dim estado_frima_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_destino, 0, matri_datos_almacen,
            2, matri_documentos.Length, id_tipo_archivo, matri_documentos, 0, id_imagen_, id_tipo_archivo,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro, 0, estado_frima_digital)
            If Result <> "YES" Then
                Copia_documento_expediente_produccion = Result
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(Fecha_documento,
                                                                  fecha_tempo)
            fecha_tempo = Left(fecha_tempo, 10)
            fecha_tempo = fecha_tempo.Replace("-", "/")
            campos_valores = id_registro & "|" & nombre_archivo.Replace("|", "") & "|" & fecha_tempo & "|" _
                & nombre_tipo_documental.Replace("|", "") & "|" & nombre_gabinete_destino & "|" & estru_unidad_conservacion(0).CODIGO_UNICO &
                "|" & estado_frima_digital & "|" & icono & "|" & id_imagen_

            '-------------------------------------------------------------------------------------------------
            'Inserta registro copia documentos workflow
            ' Campo estado_copia_vincula 3- Copia a expediente producion documental  1- Copia a expediente
            '--------------------------------------------------------------------------------------------------
            If tipo_copia = 1 Then
                Dim sql_insert = "insert into   ra_rel_copia_wf_produccion " &
               "(ID_REGISTRO_PRODUCION_DOCUMENTAL,id_tarea_wf,id_usuario_wf,id_imagen_da,nombre_gabinete,id_producion_wf,id_expediente_destino," &
               "id_ruta_wf,estado_copia_vincula,date_registro_trans,id_estado_tarea,ID_FLUJO_TRABAJO,Id_Actividad,Id_actividad_flujo) values " &
               "(" & id_registro & "," & id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "," &
               id_imagen & ",'" & nombre_gabinete & "'," & id_documento_producion & "," & id_expediente & "," &
               HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",1,'" & date_trans & "'," & stru_estado.id_Estado & "," &
               stru_estado.ID_FLUJO_TRABAJO & "," & stru_estado.Id_Actividad & "," & stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO & ")"
                Dim ref2 As New conect.Dbase_Conction_Mysql_RA
                Result = ref2.SELECTION_INSERT_COMMAND(sql_insert)
                If Result <> "YES" Then
                    Copia_documento_expediente_produccion = "Se copio el archivo pero no se registro la relación con la tarea workflow " & Result
                    Exit Function
                End If
            End If
            id_documento_producion = 0
            Copia_documento_expediente_produccion = "YES"
            Exit Function
        Catch ex As Exception
            Copia_documento_expediente_produccion = "Inconsistencia general función Copia_documento_expediente_produccion " & ex.Message
        End Try
    End Function
    Function Activa_agregar_documento_carpeta_expediente(ByVal id_expediente As Integer,
                                                         ByVal id_usuario_gestion As Integer,
                                                         ByRef nombre_archivo_ As String,
                                                         ByVal archivo_copia As String,
                                                         ByVal dro_list_tipo As DropDownList,
                                                         ByRef id_registro_copia As Integer,
                                                         ByRef ref_gridview As GridView,
                                                         ByRef ref_update As UpdatePanel,
                                                         ByRef campos_valores As String) As String
        Try
            Dim Result As String = ""
            Dim refclas_expediente As New ClassGaExpediente
            '----------------------------------------------
            'Retorna el estado del expediente carpeta
            '----------------------------------------------
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Result = refclas_expediente.Retorna_estado_expediente(id_expediente,
                                                                  estado_expediente,
                                                                  estado_publico)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If estado_expediente <> 1 Then
                Activa_agregar_documento_carpeta_expediente = "No se puede adjuntar el documento a la carpeta o expediente por que está cerrado"
                Exit Function
            End If
            '---------------------------------------------
            'Valida el archivo a almacenar
            '---------------------------------------------
            If archivo_copia = "" Then
                Activa_agregar_documento_carpeta_expediente = "Debe informar el archivo que desea guardar"
                Exit Function
            End If
            '---------------------------------------------
            'Valida la configuración del gabinete
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_gabinete As String = "PRODUCIONDOC"
            Result = refclas_expediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                              nombre_gabinete)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción asinar unidad documental"
                Exit Function
            End If
            '---------------------------------------------
            'Solicita las opciones de produción documental
            '---------------------------------------------
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If

            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If dro_list_tipo.Text = "" Then
                    Activa_agregar_documento_carpeta_expediente = "Debe seleccionar el tipo documental del archivo a adjuntar"
                    Exit Function
                End If

            End If

            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(archivo_copia)
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna estructura expediente
            '-----------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Dim id_tipo_documetal As Integer = 0
            Dim dro_list_selec_value As String = ""
            Dim dro_list_selec_text As String = ""
            If Not dro_list_tipo.SelectedItem Is Nothing Then
                dro_list_selec_value = dro_list_tipo.SelectedItem.Value
                dro_list_selec_text = dro_list_tipo.SelectedItem.Text
            End If
            If dro_list_selec_text <> "" Then
                Dim Split() As String = dro_list_selec_value.Split("|")
                id_tipo_documetal = Split(0)
            End If
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                          estru_unidad_conservacion)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim matri_documentos() As String = Nothing
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim nombre_archivo As String = ""
            nombre_archivo = file_inf.Name.ToString.Replace(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "_doc_adjunto_", "")
            nombre_archivo = nombre_archivo.ToString.Replace("/", "-")
            Result = Me.Retorna_parametros_almacenamiento_documento_adjunto(id_expediente,
                                                                            matri_datos_almacen,
                                                                            matri_gestion,
                                                                            matri_documentos,
                                                                            nombre_gabinete,
                                                                            nombre_archivo,
                                                                            estru_unidad_conservacion,
                                                                            archivo_copia,
                                                                            dro_list_selec_text,
                                                                            id_tipo_documetal,
                                                                            1, 0, 0)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_imagen As Integer = 0
            Dim radicado As String = ""
            Dim id_registro As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
            2, matri_documentos.Length, id_tipo_archivo, matri_documentos, 0, id_imagen, id_tipo_archivo, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro, 1)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(date1al,
                                                                  fecha_tempo)
            fecha_tempo = Left(fecha_tempo, 10)
            campos_valores = "|" & id_registro.ToString & "|" & nombre_archivo.Replace("|", "") & "|" & fecha_tempo & "|" _
              & dro_list_selec_text & "|" & nombre_gabinete & "|" & estru_unidad_conservacion(0).ALEAS_EXPEDIENTE
            id_registro_copia = id_registro
            'Dim refclas As New ClassGaProducionDocumental
            'Dim campo() As STRU_CAMPOS_GRIDVIEW
            'ReDim Preserve campo(0)
            'campo(0).valor_campo = id_registro.ToString
            'ReDim Preserve campo(1)
            'campo(1).valor_campo = nombre_archivo
            'ReDim Preserve campo(2)
            'campo(2).valor_campo = date1al.ToString
            'ReDim Preserve campo(3)
            'campo(3).valor_campo = dro_list_selec_text
            'ReDim Preserve campo(4)
            'campo(4).valor_campo = nombre_gabinete
            'ReDim Preserve campo(5)
            'campo(5).valor_campo = estru_unidad_conservacion(0).CODIGO_UNICO
            'Result = refclas.Agrega_fila_gre(ref_gridview, campo, ref_update)
            'If Result <> "YES" Then
            '    Activa_agregar_documento_carpeta_expediente = Result
            '    Exit Function
            'End If
            Activa_agregar_documento_carpeta_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Activa_agregar_documento_carpeta_expediente = "Inconsistencia general función Activa_agregar_documento_carpeta_expediente " & ex.Message
        End Try
    End Function
    Function Activa_agregar_documento_carpeta_expediente(ByVal id_expediente As Integer,
                                                         ByVal id_usuario_gestion As Integer,
                                                         ByVal archivo_copia As String,
                                                         ByVal id_tipo_documetal As Integer,
                                                         ByVal nombre_tipo_archivo As String,
                                                         ByRef stru_datos_image_lista As stru_datos_image_lista) As String
        Try
            Dim Result As String = ""
            Dim refclas_expediente As New ClassGaExpediente
            '----------------------------------------------
            'Retorna el estado del expediente carpeta
            '----------------------------------------------
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Result = refclas_expediente.Retorna_estado_expediente(id_expediente,
                                                                  estado_expediente,
                                                                  estado_publico)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If estado_expediente <> 1 Then
                Activa_agregar_documento_carpeta_expediente = "No se puede adjuntar el documento a la carpeta o expediente por que está cerrado"
                Exit Function
            End If
            '---------------------------------------------
            'Valida el archivo a almacenar
            '---------------------------------------------
            If archivo_copia = "" Then
                Activa_agregar_documento_carpeta_expediente = "Debe informar el archivo que desea guardar"
                Exit Function
            End If
            '---------------------------------------------
            'Valida la configuración del gabinete
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_gabinete As String = "PRODUCIONDOC"
            Result = refclas_expediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                              nombre_gabinete)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Activa_agregar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción asinar unidad documental"
                Exit Function
            End If
            '---------------------------------------------
            'Solicita las opciones de produción documental
            '---------------------------------------------
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If id_tipo_documetal = 0 Or id_tipo_documetal = -1 Then
                    Activa_agregar_documento_carpeta_expediente = "Debe seleccionar el tipo documental del archivo a adjuntar"
                    Exit Function
                End If
            End If
            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(archivo_copia)
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim Tipo_Documento As Integer = -1
            Dim filinf As New FileInfo(archivo_copia)
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                              Tipo_Documento)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim icono As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            ClassDaGabinete.SolicitaIconoImageFownt(Tipo_Documento.ToString,
                                                      icono)
            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna estructura expediente
            '-----------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                          estru_unidad_conservacion)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim matri_documentos() As String = Nothing
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim nombre_archivo As String = ""
            nombre_archivo = file_inf.Name.ToString.Replace(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "_doc_adjunto_", "")
            nombre_archivo = nombre_archivo.ToString.Replace("/", "-")
            Result = Me.Retorna_parametros_almacenamiento_documento_adjunto(id_expediente,
                                                                            matri_datos_almacen,
                                                                            matri_gestion,
                                                                            matri_documentos,
                                                                            nombre_gabinete,
                                                                            nombre_archivo,
                                                                            estru_unidad_conservacion,
                                                                            archivo_copia,
                                                                            nombre_tipo_archivo,
                                                                            id_tipo_documetal,
                                                                            1, 0, 0)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_imagen As Integer = 0
            Dim radicado As String = ""
            Dim id_registro As Integer = 0
            Dim estado_firma_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
            2, matri_documentos.Length, id_tipo_archivo, matri_documentos, 0, id_imagen, id_tipo_archivo,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro, 1, estado_firma_digital)
            If Result <> "YES" Then
                Activa_agregar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(date1al,
                                                                  fecha_tempo)
            fecha_tempo = Left(fecha_tempo, 10)
            'campos_valores = "|" & id_registro.ToString & "|" & nombre_archivo.Replace("|", "") & "|" & fecha_tempo & "|" _
            ' & nombre_tipo_archivo & "|" & nombre_gabinete & "|" & estru_unidad_conservacion(0).ALEAS_EXPEDIENTE
            'id_registro_copia = id_registro
            stru_datos_image_lista.nombre_gabinete = nombre_gabinete
            stru_datos_image_lista.tipodocumental = nombre_tipo_archivo
            stru_datos_image_lista.id_registro = id_registro
            stru_datos_image_lista.fecha = fecha_tempo
            stru_datos_image_lista.aleas = estru_unidad_conservacion(0).ALEAS_EXPEDIENTE
            stru_datos_image_lista.nombre_archivo = nombre_archivo.Replace("|", "")
            stru_datos_image_lista.icono_icono_awe_some = icono
            stru_datos_image_lista.estado_firma_digital = estado_firma_digital
            stru_datos_image_lista.id_imagen = id_imagen
            Activa_agregar_documento_carpeta_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Activa_agregar_documento_carpeta_expediente = "Inconsistencia general función Activa_agregar_documento_carpeta_expediente " & ex.Message
        End Try
    End Function


    Function Solicita_existencia_sub_serie_produccion_documental(ByVal id_sub_serie As Integer,
                                                                 ByRef exitencia As String) As String
        '----------------------------------------------------------------
        '---------------------------------------------------------------
        'Funcion : Solicita la existencia de una sub serie registrada
        'en la proucción documetal con el parametro id sub serie
        'fecha : 2022-08-05
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select ID_SUBSERIE_DOCUMENTO " &
            " from registro_producion_documental Where ID_SUBSERIE_DOCUMENTO=" & id_sub_serie & " limit 1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_sub_serie_produccion_documental = "Funcion  Solicita_existencia_sub_serie_produccion_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                exitencia = "NO"
                Solicita_existencia_sub_serie_produccion_documental = "YES"
                Exit Function
            Else
                exitencia = "YES"
                Solicita_existencia_sub_serie_produccion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_sub_serie_produccion_documental = "Inconsistencia general funcion Solicita_existencia_sub_serie_produccion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_serie_produccion_documental(ByVal id_serie As Integer,
                                                                 ByRef exitencia As String) As String
        '----------------------------------------------------------------
        '---------------------------------------------------------------
        'Funcion : Solicita la existencia de una  serie registrada
        'en la proucción documetal con el parametro id  serie
        'fecha : 2022-08-05
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select ID_SERIE_DOCUMENTO " &
            " from registro_producion_documental Where ID_SERIE_DOCUMENTO=" & id_serie & " limit 1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_serie_produccion_documental = "Funcion  Solicita_existencia_serie_produccion_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                exitencia = "NO"
                Solicita_existencia_serie_produccion_documental = "YES"
                Exit Function
            Else
                exitencia = "YES"
                Solicita_existencia_serie_produccion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_serie_produccion_documental = "Inconsistencia general funcion Solicita_existencia_serie_produccion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_identificacion_tipo_documental_sub_serie(ByVal id_sub_serie_documental As Integer,
                                                               ByVal nombre_tipo_documental As String,
                                                               ByRef id_tipo_documental_sub_serie As Integer) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipos_Doc_SubSeries " &
           " from tipos_doc_subseries where SubSeries_Documentales_Id_SubSeries=" & id_sub_serie_documental & " and Descripcion_Documento='" & nombre_tipo_documental & "'" &
           " and Estado_Tipo=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_tipo_documental_sub_serie = "Funcion  Solicita_identificacion_tipo_documental_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_tipo_documental_sub_serie = "Imposible encontrar la identificación del tipo documental " & nombre_tipo_documental & " relacionado a la sub serie " & id_tipo_documental_sub_serie
                Exit Function
            Else
                id_tipo_documental_sub_serie = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_tipo_documental_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_tipo_documental_sub_serie = "Inconsistencia general función Solicita_identificacion_tipo_documental_sub_serie " & ex.Message
        End Try
    End Function
    Function Retorna_parametros_almacenamiento_documento_adjunto(ByVal id_expediente As Integer,
                                                                 ByRef matri_datos_almacen() As String,
                                                                 ByRef matri_gestion As estructure_gestion,
                                                                 ByRef matri_documentos() As String,
                                                                 ByVal nombre_gabinete As String,
                                                                 ByVal Nombre_Documento As String,
                                                                 ByVal estru_unidad_conservacion() As expediente_conservacion,
                                                                 ByVal ruta_archivo As String,
                                                                 ByVal nombre_tipo_documento As String,
                                                                 ByVal id_tipo_documento As Integer,
                                                                 ByVal extrae_multi_tif As Integer,
                                                                 ByVal obliga_viculo_expe_gabinete As Integer,
                                                                 ByVal actualiza_expediente_gabinete As Integer) As String
        Try
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim Result As String = ""
            Dim refclas As New ClassNeodynamic
            If ruta_archivo <> "" Then
                Dim file_inf As New FileInfo(ruta_archivo)
                If UCase(file_inf.Extension) = ".TIF" Then
                    If extrae_multi_tif = 1 Then
                        Result = refclas.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                              matri_documentos,
                                                                              HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                        If Result <> "YES" Then
                            Retorna_parametros_almacenamiento_documento_adjunto = "Imposible extraer documento Multi tif "
                            Exit Function
                        End If
                        If matri_documentos Is Nothing Then
                            Retorna_parametros_almacenamiento_documento_adjunto = "La matriz del multi tif esta nothing "
                            Exit Function
                        End If
                    End If
                Else
                    ReDim Preserve matri_documentos(0)
                    matri_documentos(0) = ruta_archivo
                End If
            End If
            Dim Refclasalmacenamiento As New ClassAlmacenamiento
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_adjunto = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "NOMBRE" Then
                    estructura_gabinete(i).VALORCAMPO = Nombre_Documento
                End If
            Next
            '-------------------------------------------
            'Asigna datos gestión
            '-------------------------------------------
            If Not estru_unidad_conservacion Is Nothing Then
                matri_gestion.CLASE_DOCUMENTO = ""
                matri_gestion.EXPEDIENTE = estru_unidad_conservacion(0).CODIGO_UNICO
                matri_gestion.ID_AREA = estru_unidad_conservacion(0).CODIGO_AREA_TRD
                matri_gestion.ID_CLASE_DOCUMENTO = 0
                matri_gestion.ID_EXPEDIENTE = id_expediente
                matri_gestion.ID_SERIE = estru_unidad_conservacion(0).CODIGO_SERIE
                matri_gestion.NOMBRE_SERIE = estru_unidad_conservacion(0).NOMBRE_SERIE
                matri_gestion.ID_SUB_SERIE = estru_unidad_conservacion(0).CODIGO_SUBSERIE
                matri_gestion.NOMBRE_SUB_SERIE = estru_unidad_conservacion(0).NOMBRE_SUBSERIE
                matri_gestion.ID_TIPO_EXPEDIENTE = estru_unidad_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION
                matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
                matri_gestion.ID_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                matri_gestion.UNIDAD_CONSERVACION = ""
                matri_gestion.FECHA_ELABORACION = ""
                matri_gestion.TIPODOCUMENTO = nombre_tipo_documento
            Else
                matri_gestion.CLASE_DOCUMENTO = ""
                matri_gestion.EXPEDIENTE = 0
                matri_gestion.ID_AREA = 0
                matri_gestion.ID_CLASE_DOCUMENTO = 0
                matri_gestion.ID_EXPEDIENTE = id_expediente
                matri_gestion.ID_SERIE = 0
                matri_gestion.NOMBRE_SERIE = ""
                matri_gestion.ID_SUB_SERIE = 0
                matri_gestion.NOMBRE_SUB_SERIE = ""
                matri_gestion.ID_TIPO_EXPEDIENTE = 0
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
                matri_gestion.ID_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                matri_gestion.UNIDAD_CONSERVACION = ""
                matri_gestion.FECHA_ELABORACION = ""
                matri_gestion.TIPODOCUMENTO = nombre_tipo_documento
            End If

            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim Refgestion As New Classgestionrespuesta
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_adjunto = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_adjunto = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna valores expediente a gabinete 
            '---------------------------------------------
            Dim Class_system1 As New Class_system1
            Dim id_gabinete_docuarchi As Integer = 0
            Result = Class_system1.SolicitaIdGabineteDocuarchi(nombre_gabinete,
                                                       id_gabinete_docuarchi)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_adjunto = Result
                Exit Function
            End If

            Dim Class_Ra_gabexp_relacion_index_gabinete_expediente As New Class_Ra_gabexp_relacion_index_gabinete_expediente
            Dim stru_rel_exp_gabinete() As stru_rel_exp_gabinete = Nothing
            If actualiza_expediente_gabinete = 1 Then
                Result = Class_Ra_gabexp_relacion_index_gabinete_expediente.SolicitaValoresCampoExpedienteParaCampoIndiceGabinete(id_expediente,
                                                                                                                                  id_gabinete_docuarchi,
                                                                                                                                  obliga_viculo_expe_gabinete,
                                                                                                                                  stru_rel_exp_gabinete)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_adjunto = Result
                    Exit Function
                End If
                For i As Integer = 0 To estructura_gabinete.Length - 1
                    For z As Integer = 0 To stru_rel_exp_gabinete.Length - 1
                        If estructura_gabinete(i).CAMPO = stru_rel_exp_gabinete(z).CAMPO Then
                            estructura_gabinete(i).VALORCAMPO = stru_rel_exp_gabinete(z).valor_campo_gabinete
                        End If
                    Next
                Next
            End If
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPO_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.TIPODOCUMENTO
                End If
            Next
            Dim i2 As Integer = 0
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).VISIBLE = 1 Then
                    ReDim Preserve matri_datos_almacen(i2)
                    matri_datos_almacen(i2) = estructura_gabinete(i).VALORCAMPO
                    i2 = i2 + 1
                End If
            Next
            Retorna_parametros_almacenamiento_documento_adjunto = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_adjunto = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_adjunto " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_expediente_producion(ByVal id_expediente As Integer,
                                                                ByRef page1 As Page,
                                                                ByVal tipo_consulta As Integer,
                                                                ByVal valor_consulta As String,
                                                                ByRef colum_order_name As String,
                                                                ByRef order_colum As String) As String
        Try

            Dim Result As String = ""
            Dim scripma As GridView = page1.FindControl("data_grid")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral_documentos")
            Dim ref_UpdatePanel_label_resultado As UpdatePanel = page1.FindControl("UpdatePanel_label_resultado")
            Dim ref_UpdatePanel_botones_unidad As UpdatePanel = page1.FindControl("UpdatePanel_botones_unidad")
            Dim ref_titulo_label_grid As Label = page1.FindControl("titulo_label_grid")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID")
            Dim Hidden_nureg As Object = page1.FindControl("Hidden_nureg")
            Dim Datset_consulta As DataSet = New DataSet("registro_producion_documental")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim consulta As String = ""
            If tipo_consulta = 1 Then
                consulta = "Select ID_REGISTRO_PRODUCION_DOCUMENTAL,ESTADO_FIRMA_DIGITAL,ID_DOCUMENTO_DOCUARCHI_ALMACEN,RADICADO_DOCUMENTO, SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,FECHA_DOCUMENTO as FECHA,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL,NOMBRE_GABINETE AS GABINETE, ea.ALEAS_EXPEDIENTE as EXPEDIENTE,FORMATO " _
                                  & "from registro_producion_documental as rpd   " &
                                  " inner join  expediente_archivo as ea on (ea.ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                  " where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & id_expediente &
                                  " and ESTADO_ELIMINA_PRODUCION_DOCUMENTAL=0" &
                                     " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 2 Then
                consulta = "Select ID_REGISTRO_PRODUCION_DOCUMENTAL,ESTADO_FIRMA_DIGITAL,ID_DOCUMENTO_DOCUARCHI_ALMACEN,RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,FECHA_DOCUMENTO as FECHA,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL,NOMBRE_GABINETE AS GABINETE, ea.ALEAS_EXPEDIENTE as EXPEDIENTE,FORMATO " _
                                  & "from registro_producion_documental as rpd  " &
                                   " inner join  expediente_archivo as ea on (ea.ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                   " where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & id_expediente &
                                   " and ESTADO_ELIMINA_PRODUCION_DOCUMENTAL=0" &
                                  " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 3 Then
                consulta = "(Select ID_REGISTRO_PRODUCION_DOCUMENTAL,ESTADO_FIRMA_DIGITAL,ID_DOCUMENTO_DOCUARCHI_ALMACEN,RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,FECHA_DOCUMENTO as FECHA,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL,NOMBRE_GABINETE AS GABINETE, ea.ALEAS_EXPEDIENTE as EXPEDIENTE,FORMATO" _
                                    & " from registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join  expediente_archivo as ea on (ea.ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_niveles as rppn on (rppn.id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & valor_consulta & "%' or FECHA_DOCUMENTO like " & "'%" & valor_consulta & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & valor_consulta & "%')" & " union " &
                                    " (Select ID_REGISTRO_PRODUCION_DOCUMENTAL,ESTADO_FIRMA_DIGITAL,ID_DOCUMENTO_DOCUARCHI_ALMACEN,RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,FECHA_DOCUMENTO as FECHA,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL,NOMBRE_GABINETE AS GABINETE, ea.ALEAS_EXPEDIENTE as EXPEDIENTE,FORMATO" &
                                    " from  registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                     " inner join  expediente_archivo as ea on (ea.ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_permisos_niveles as rppn on (rppn.ra_pro_niveles_id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & valor_consulta & "%' or FECHA_DOCUMENTO like " & "'%" & valor_consulta & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & valor_consulta & "%')" &
                                    " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_SOLICITUD_PRODUCCION") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_PRODUCCION") = valor_consulta
            HttpContext.Current.Session.Item("Sort_matri_colum_produccion") = {"", "ID_REGISTRO_PRODUCION_DOCUMENTAL", "NOMBRE", "FECHA", "TIPODOCUMENTAL", "GABINETE", "EXPEDIENTE"}
            Dim matri() As String = HttpContext.Current.Session.Item("Sort_matri_colum_produccion")
            Result = ref.SELECTION_SELECT_FIELD(consulta, Datset_consulta)
            HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Lista_documentos_relacionados_expediente_producion = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                ref_titulo_label_grid.Text = " archivo(s) encontrado(s)"
                Datset_consulta.Tables(0).Rows.Add(Datset_consulta.Tables(0).NewRow)
                scripma.DataSource = Datset_consulta
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                hideselecion.value = "-1"
                Hidden_nureg.value = 0
                ref_UpdateGeneral_documentos.Update()
                ref_UpdatePanel_botones_unidad.Update()
                ref_UpdatePanel_label_resultado.Update()
                Lista_documentos_relacionados_expediente_producion = "YES"
                Exit Function
            Else
                ref_titulo_label_grid.Text = " " & Datset_consulta.Tables(0).Rows.Count & " archivo(s) encontrado(s)"
                scripma.DataSource = Datset_consulta.CreateDataReader
                hideselecion.value = "-1"
                Hidden_nureg.value = Datset_consulta.Tables(0).Rows.Count
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    scripma.Rows(i).Cells(3).Text = Left(scripma.Rows(i).Cells(3).Text, 10)
                    scripma.Rows(i).Attributes.Add("ondblclick", "prevent_scrol_ondblclick(event,this);")
                    scripma.Rows(i).Attributes.Add("onmouseover", "preven_scrol_onmouseover(event,this);")
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    If scripma.Rows(i).Cells(0).Controls.Count > 2 Then
                        divhtml.Controls.Add(scripma.Rows(i).Cells(0).Controls(1))
                    End If
                    Dim icono As String = "fal fa-file"
                    Dim classgabinete As New ClassDaGabinete
                    If scripma.Rows(i).Cells(10).Text.ToString <> "&nbsp;" Then
                        classgabinete.Agrega_icono_image_fownt_extension(scripma.Rows(i).Cells(10).Text.ToString,
                                                                         icono)
                    End If
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", icono)
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_scrol(event,this, 'vis');")
                    ahtml.Attributes.Add("title", "Visualiza archivo")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-download fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_scrol(event,this,'dow');")
                    ahtml.Attributes.Add("title", "Descarga archivo")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-excel fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_scrol(event,this,'del');")
                    ahtml.Attributes.Add("title", "Elimina archivo")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    If Val(scripma.Rows(i).Cells(2).Text) = 0 Then
                        ihtml.Attributes.Add("class", "fal fa-file-signature")
                    End If
                    If Val(scripma.Rows(i).Cells(2).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-lock-alt")
                    End If
                    If Val(scripma.Rows(i).Cells(2).Text) = 2 Then
                        ihtml.Attributes.Add("class", "fal fa-file-invoice")
                    End If
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_scrol(event,this,'fir');")
                    If Val(scripma.Rows(i).Cells(2).Text) = 0 Then
                        ahtml.Attributes.Add("title", "Firmar y agrear meta dato")
                    End If
                    If Val(scripma.Rows(i).Cells(2).Text) = 1 Then
                        ahtml.Attributes.Add("title", "Documento con firma digital y meta datos")
                    End If
                    If Val(scripma.Rows(i).Cells(2).Text) = 2 Then
                        ahtml.Attributes.Add("title", "Documento con meta datos")
                    End If
                    Dim radicado As String = ""
                    If scripma.Rows(i).Cells(4).Text <> "&nbsp;" Then
                        radicado = scripma.Rows(i).Cells(4).Text
                    End If
                    If scripma.Rows(i).Cells(6).Text <> "&nbsp;" Then
                        scripma.Rows(i).Cells(6).Text = Left(scripma.Rows(i).Cells(6).Text, 10)
                    End If
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(3).Text)
                    ahtml.Attributes.Add("idd_rad", scripma.Rows(i).Cells(8).Text & "|" & scripma.Rows(i).Cells(3).Text & "|" & radicado & "|" & scripma.Rows(i).Cells(3).Text.ToString() & "|" & scripma.Rows(i).Cells(5).Text & "|" & 0)
                    ahtml.Attributes.Add("tip_event", "firma_doc_selecion_rad")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_f_d_" & scripma.Rows(i).Cells(1).Text
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 1 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this,'na');")
                        End If
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_produccion"),
                                                            order_colum,
                                                            scripma)
                If Result <> "YES" Then
                    Lista_documentos_relacionados_expediente_producion = "Error add clase función  Solicita_listado_usuario_permisos_nivel " & Result
                    Exit Function
                End If
                ref_UpdatePanel_label_resultado.Update()
                ref_UpdateGeneral_documentos.Update()
                ref_UpdatePanel_botones_unidad.Update()
                Lista_documentos_relacionados_expediente_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_expediente_producion = "Inconsistencia general función Lista_documentos_relacionados_expediente_producion  " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                           ByRef page1 As Page,
                                                           ByRef texto_documento As String, ByVal p As Integer) As String
        Try
            Dim Result As String = ""
            Dim scripma As GridView = page1.FindControl("data_grid")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral_documentos")
            Dim ref_UpdatePanel_label_resultado As UpdatePanel = page1.FindControl("UpdatePanel_label_resultado")
            Dim ref_UpdatePanel_botones_unidad As UpdatePanel = page1.FindControl("UpdatePanel_botones_unidad")
            Dim ref_titulo_label_grid As Label = page1.FindControl("titulo_label_grid")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID")
            Dim Datset_consulta As DataSet = New DataSet("registro_producion_documental")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim consulta As String = "Select ID_REGISTRO_PRODUCION_DOCUMENTAL,SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL " _
                                    & " from registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_niveles as rppn on (rppn.id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & texto_documento & "%' or FECHA_DOCUMENTO like " & "'%" & texto_documento & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & texto_documento & "%'" & " union " &
                                    " Select ID_REGISTRO_PRODUCION_DOCUMENTAL,SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL" &
                                    " from  registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_permisos_niveles as rppn on (rppn.ra_pro_niveles_id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & texto_documento & "%' or FECHA_DOCUMENTO like " & "'%" & texto_documento & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & texto_documento & "%'"

            Result = ref.SELECTION_SELECT_FIELD(consulta, Datset_consulta)
            If Result <> "YES" Then
                Lista_documentos_relacionados_usuario_gestion = "Error listando datos " & Result
                Exit Function
            End If

            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                ref_titulo_label_grid.Text = " archivo(s) encontrado(s)"
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                ref_UpdatePanel_botones_unidad.Update()
                ref_UpdatePanel_label_resultado.Update()
                Lista_documentos_relacionados_usuario_gestion = "YES"
                Exit Function
            Else
                ref_titulo_label_grid.Text = " " & Datset_consulta.Tables(0).Rows.Count & " archivo(s) encontrado(s)"
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    scripma.Rows(i).Cells(3).Text = Left(scripma.Rows(i).Cells(3).Text, 10)

                    For z As Integer = 1 To scripma.Rows(i).Cells.Count - 1
                        scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                        scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                    Next
                Next
                ref_UpdatePanel_label_resultado.Update()
                ref_UpdateGeneral_documentos.Update()
                ref_UpdatePanel_botones_unidad.Update()
                Lista_documentos_relacionados_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_usuario_gestion = "Inconsistencia general función Lista_documentos_relacionados_usuario_gestion  " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_fecha_gred(ByRef gred As DataSet, ByRef scripma As GridView) As String
        Try
            For i As Integer = 0 To gred.Tables(0).Rows.Count - 1
                For z As Integer = 0 To gred.Tables(0).Columns.Count - 1
                    If gred.Tables(0).Rows(i).Item(z).GetType.ToString = "System.DateTime" Then
                        If gred.Tables(0).Rows(i).IsNull(z) = False Then
                            If InStr(gred.Tables(0).Rows(i).Item(z).ToString, "a") > 0 Or InStr(gred.Tables(0).Rows(i).Item(z).ToString, "m") _
                                Or InStr(gred.Tables(0).Rows(i).Item(z).ToString, "p") Or InStr(gred.Tables(0).Rows(i).Item(z).ToString, ".") Then
                                Dim tempo = Left(gred.Tables(0).Rows(i).Item(z).ToString, 19)
                                If InStr(tempo, "12:00:00") > 0 Then
                                    tempo = Left(gred.Tables(0).Rows(i).Item(z).ToString, 10)
                                End If
                                gred.Tables(0).Rows(i).Item(z) = tempo
                                scripma.Rows(i).Cells(z).Text = tempo
                            Else
                                Dim tempo = Left(gred.Tables(0).Rows(i).Item(z).ToString, 10)
                                gred.Tables(0).Rows(i).Item(z) = tempo
                                scripma.Rows(i).Cells(z).Text = tempo
                            End If

                        End If
                    End If

                Next
            Next
            Aplica_estilo_fecha_gred = "YES"
        Catch ex As Exception
            Aplica_estilo_fecha_gred = "Inconsistencia general función " & ex.Message
        End Try
    End Function

    Function Asigna_datos_interface_edicion_archivo(ByVal id_usuario_gestion As Integer,
                                                    ByVal identificacion_produccion As Integer,
                                                    ByRef ref_textbox_nombre As TextBox,
                                                    ByRef ref_droplis_tipo As DropDownList,
                                                    ByRef ref_updapanel As UpdatePanel) As String
        Try
            '-------------------------------------------
            'Solicita datos caracterización producción
            '-------------------------------------------
            Dim Result As String = ""
            Dim Nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_tipo_documetal As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(identificacion_produccion,
                                                               Nombre_archivo,
                                                               nombre_tipo_documental,
                                                               id_tipo_documetal,
                                                               id_expediente,
                                                               id_imagen,
                                                               nombre_gabinete,
                                                               fecha_documento,
                                                               numero_folios)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_archivo = Result
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_archivo = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Asigna_datos_interface_edicion_archivo = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                     id_usuario_gestion,
                                                                                                     stru_permisos_niveles)
                If Result <> "YES" Then
                    Asigna_datos_interface_edicion_archivo = Result
                    Exit Function
                End If
                If stru_permisos_niveles.cambiar_nombre_archivo = 0 Then
                    Asigna_datos_interface_edicion_archivo = "El usuario no tiene persmiso para cambiar nombre de archivos, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            ref_textbox_nombre.Text = Nombre_archivo
            '-----------------------------------------------
            'Retorna estructura expediente
            '--.---------------------------------------------
            Dim refclas_expediente As New ClassGaExpediente
            Dim refclas_ra_tipo_doc As New Class_ra_tipo_doc_series
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            If id_expediente <> 0 Then
                Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente, estru_unidad_conservacion)
                If Result <> "YES" Then
                    Asigna_datos_interface_edicion_archivo = Result
                    Exit Function
                End If

            End If
            ref_droplis_tipo.Items.Clear()
            If Not estru_unidad_conservacion Is Nothing Then
                If estru_unidad_conservacion(0).CODIGO_SERIE <> 0 Then
                    '-----------------------------------------------------------------
                    'Solicita tipos documentales relacionados a la serie documental
                    '-----------------------------------------------------------------
                    Result = refclas_ra_tipo_doc.Solicita_tipos_documentales_relacionados_a_la_series_defaul(estru_unidad_conservacion(0).CODIGO_SERIE,
                                                                                                             ref_droplis_tipo,
                                                                                                             ref_updapanel,
                                                                                                             nombre_tipo_documental,
                                                                                                             0,
                                                                                                             id_tipo_documetal,
                                                                                                             "SERIE")
                    If Result <> "YES" Then
                        Asigna_datos_interface_edicion_archivo = Result
                        Exit Function
                    End If
                End If
                If estru_unidad_conservacion(0).CODIGO_SUBSERIE <> 0 Then
                    '---------------------------------------------------------------------
                    'Solicita tipos documentales relacionados a la sub serie documental
                    '---------------------------------------------------------------------
                    Result = refclas_ra_tipo_doc.Solicita_tipos_documentales_relacionados_a_la_sub_series_default(estru_unidad_conservacion(0).CODIGO_SUBSERIE,
                                                                                                                  ref_droplis_tipo,
                                                                                                                  ref_updapanel,
                                                                                                                  nombre_tipo_documental,
                                                                                                                  1,
                                                                                                                  id_tipo_documetal,
                                                                                                                  "SUBSERIE",
                                                                                                                  estru_unidad_conservacion(0).CODIGO_SERIE)
                    If Result <> "YES" Then
                        Asigna_datos_interface_edicion_archivo = Result
                        Exit Function
                    End If

                End If
            End If

            Asigna_datos_interface_edicion_archivo = "YES"
        Catch ex As Exception
            Asigna_datos_interface_edicion_archivo = "Inconsistencia función Asigna_datos_interface_edicion_archivo " & ex.Message
        Finally
            ref_updapanel.Update()
        End Try
    End Function
    Function Elimina_archivo_producion_restriccion(ByVal id_producion_archivo As Long) As String
        Try
            Dim Parametro_Consulta = "Update  registro_producion_documental " &
                "set ESTADO_ELIMINA_PRODUCION_DOCUMENTAL=1 " &
            "  where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Elimina_archivo_producion_restriccion = "Función  Elimina_archivo_producion_restriccion dice " & Result
                Exit Function
            Else
                Elimina_archivo_producion_restriccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Elimina_archivo_producion_restriccion = "Inconsistencia general función  Elimina_archivo_producion_restriccion " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_documento_producion_por_id(ByVal id_producion_archivo As Object,
                                                        ByRef nombre_archivo As String) As String
        Try
            Dim Parametro_Consulta = "select SEGUNDO_NOMBRE_DOCUMENTO" &
            " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_documento_producion_por_id = "Función  Solicita_nombre_documento_producion_por_id dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_documento_producion_por_id = "El sistema no pudo encontrar el nombre del documento de producción " & id_producion_archivo
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_archivo = ""
                Else
                    nombre_archivo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_nombre_documento_producion_por_id = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_documento_producion_por_id = "Inconsistencia general función Solicita_nombre_documento_producion_por_id " & ex.Message
        End Try
    End Function
    Function Actualiza_documento_produccio_radicado(ByVal id_registro_ptoducion As Object,
                                                    ByVal id_imagen_docuarchi As Integer,
                                                    ByVal gabinete_docuarchi As String,
                                                    ByVal radicado As String) As String

        Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
        Dim nombre_campo_radicado_gabinete As String = ""
        Dim Result As String = ""
        Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(gabinete_docuarchi,
                                                                            nombre_campo_radicado_gabinete)
        If Result <> "YES" Then
            Actualiza_documento_produccio_radicado = Result
            Exit Function
        End If
        Dim Update_gabinete As String = "Update  " & gabinete_docuarchi & " set " & nombre_campo_radicado_gabinete & "='" & radicado & "', " &
            "enlase ='" & radicado & "' where ID=" & id_imagen_docuarchi
        Dim Update_producion As String = "Update  registro_producion_documental set RADICADO_DOCUMENTO='" & radicado & "', SEGUNDO_NOMBRE_DOCUMENTO='" &
           "RAD-" & radicado & "' where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_ptoducion
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '------------------------------------------
            'Actualiza gabinete
            '------------------------------------------
            If Update_gabinete <> "" Then
                myCommand2.CommandText = Update_gabinete
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_documento_produccio_radicado = "Imposible actualizar la tabla gabinete cambios  : " & Update_gabinete
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Actualiza registro producción documental
            '------------------------------------------
            If Update_producion <> "" Then
                myCommand2.CommandText = Update_producion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_documento_produccio_radicado = "Imposible actualizar la tabla gabinete cambios  : " & Update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            Actualiza_documento_produccio_radicado = "YES"
        Catch e As Exception
            Try

                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_documento_produccio_radicado = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_documento_produccio_radicado = "Error General Actualiza_documento_produccio_radicado " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try

    End Function
    Function Solicita_datos_caracterizacion_archivo_produccion(ByVal id_producion_archivo As Object,
                                                               ByRef nombre_archivo As String,
                                                               ByRef nombre_tipo_documental As String,
                                                               ByRef id_clase_documento As Integer,
                                                               ByRef id_expediente As Integer,
                                                               ByRef id_imagen As Integer,
                                                               ByRef nombre_gabinete As String,
                                                               ByRef fecha_documento As String,
                                                               ByRef numero_folios As Integer) As String
        Try
            Dim Parametro_Consulta = "select SEGUNDO_NOMBRE_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,ID_TIPO_DOCUMENTO" &
                ",EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,ID_DOCUMENTO_DOCUARCHI_ALMACEN,NOMBRE_GABINETE,FECHA_DOCUMENTO,NUMERO_FOLIOS " &
           " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_archivo_produccion = "Funcion  Solicita_datos_caracterizacion_archivo_produccion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_caracterizacion_archivo_produccion = "El sistema no pudo encontrar los datos de caracterización del documento " & id_producion_archivo
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_archivo = ""
                Else
                    nombre_archivo = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre_tipo_documental = ""
                Else
                    nombre_tipo_documental = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    id_clase_documento = 0
                Else
                    id_clase_documento = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    id_imagen = 0
                Else
                    id_imagen = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    nombre_gabinete = ""
                Else
                    nombre_gabinete = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    fecha_documento = ""
                Else
                    Dim tempo_fecha As String = Left(Datset.Tables(0).Rows(0).Item(6), 10)
                    Dim plit_fecha() As String = tempo_fecha.ToString.Split("/")
                    fecha_documento = plit_fecha(2) & "-" & plit_fecha(1) & "-" & plit_fecha(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    numero_folios = 0
                Else
                    numero_folios = Datset.Tables(0).Rows(0).Item(7)
                End If
                Solicita_datos_caracterizacion_archivo_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_caracterizacion_archivo_produccion = "Inconsistencia general función Solicita_datos_caracterizacion_archivo_produccion " & ex.Message
        End Try
    End Function

    Function Activa_editar_documento_carpeta_expediente(ByVal identificacion_produccion As Integer,
                                                        ByVal ref_nombre As String,
                                                        ByVal ref_droplist As DropDownList) As String
        Try
            '-------------------------------------------
            'Solicita datos caracterización producción
            '-------------------------------------------
            Dim Result As String = ""
            Dim Nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_tipo_documetal As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(identificacion_produccion,
                                                                          Nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_tipo_documetal,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Activa_editar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Dim refclas_expediente As New ClassGaExpediente
            '----------------------------------------------
            'Retorna el estado del expediente carpeta
            '----------------------------------------------
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Result = refclas_expediente.Retorna_estado_expediente(id_expediente,
                                                                  estado_expediente,
                                                                  estado_publico)
            If Result <> "YES" Then
                Activa_editar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If estado_expediente <> 1 Then
                Activa_editar_documento_carpeta_expediente = "No se puede editar el documento, la carpeta o expediente se encuentra cerrado"
                Exit Function
            End If
            '----------------------------------------------
            'Valida el nombre del archivo se informe
            '----------------------------------------------
            If ref_nombre = "" Then
                Activa_editar_documento_carpeta_expediente = "Debe informar el nombre del archivo"
                Exit Function
            End If
            '---------------------------------------------
            'Valida la configuración del gabinete
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Activa_editar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Activa_editar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Activa_editar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Activa_editar_documento_carpeta_expediente = "El gabinete " & nombre_gabinete & "  no tiene activa la opción asinar unidad documental"
                Exit Function
            End If
            '---------------------------------------------
            'Solicita las opciones de produción documental
            '---------------------------------------------
            Dim nombre_tipo_documental_combo As String = ""
            If Not ref_droplist.SelectedItem Is Nothing Then
                nombre_tipo_documental_combo = ref_droplist.SelectedItem.Text
            End If
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Activa_editar_documento_carpeta_expediente = Result
                Exit Function
            End If
            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If nombre_tipo_documental_combo = "" Then
                    Activa_editar_documento_carpeta_expediente = "Debe seleccionar el tipo documental"
                    Exit Function
                End If
            End If
            '-----------------------------------------------
            'Retorna estructura expediente
            '-----------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            If nombre_tipo_documental_combo <> "" Then
                Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                            estru_unidad_conservacion)
                If Result <> "YES" Then
                    Activa_editar_documento_carpeta_expediente = Result
                    Exit Function
                End If
                Dim spli() As String = ref_droplist.SelectedItem.Value.ToString.Split("|")
                id_tipo_documetal = spli(0)
            End If
            '--------------------------------------------------
            'Actualiza datos archivo
            '--------------------------------------------------
            Result = Me.Actualiza_indice_documento_producion(identificacion_produccion,
                                                             nombre_gabinete,
                                                             nombre_tipo_documental_combo,
                                                             id_tipo_documetal,
                                                             ref_nombre,
                                                             nombre_tipo_documental,
                                                             Nombre_archivo,
                                                             id_imagen)
            If Result <> "YES" Then
                Activa_editar_documento_carpeta_expediente = Result
                Exit Function
            End If
            Activa_editar_documento_carpeta_expediente = "YES"
        Catch ex As Exception
            Activa_editar_documento_carpeta_expediente = "Inconsistencia general función Activa_editar_documento_carpeta_expediente " & ex.Message
        End Try
    End Function
    Function Actualiza_indice_documento_producion(ByVal identificacion_producion As Long,
                                                  ByVal Nombre_Gabinete As String,
                                                  ByVal nombre_tipo_documental As String,
                                                  ByVal id_tipo_documental As Integer,
                                                  ByVal nombre_archivo As String,
                                                  ByVal old_nombre_tipo_documental As String,
                                                  ByVal old_nombre_archivo As String,
                                                  ByVal id_imagen As Integer) As String
        Dim campos_trans As String = ""
        Dim Update_gabinete As String = ""
        Dim datos_campo As String = ""
        Dim detalle_trans As String = ""
        Dim hor2 As New System.DateTime
        hor2 = Date.Now
        Dim result = ""
        Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
        '------------------------------------------------------------------
        'Construye parametros indice docuarchi
        '-----------------------------------------------------------------
        Dim Ref_class_detalle As New Class_DETALLE_GABIENETE
        Dim xmlArchivo As New XmlDocument
        If old_nombre_archivo <> nombre_archivo Then
            Dim existencia As String = "NO"
            result = Ref_class_detalle.Solicita_existencia_campo_nombre(Nombre_Gabinete,
                                                                        "NOMBRE",
                                                                        existencia)
            If result <> "YES" Then
                Actualiza_indice_documento_producion = result
                Exit Function
            End If
            If existencia = "NO" Then
                Actualiza_indice_documento_producion = "Consulte a su administrador, se debe agregar el campo (NOMBRE) al gabinete (" & Nombre_Gabinete & ")"
                Exit Function
            End If
            Update_gabinete = "update " & Nombre_Gabinete & " set NOMBRE='" & nombre_archivo & "'"
            datos_campo = datos_campo & " Cambio de nombre de archivo " & old_nombre_archivo & " a " & nombre_archivo
        End If
        Dim ref_nombre_tipo_documental As String = "null"
        If nombre_tipo_documental <> "" Then
            ref_nombre_tipo_documental = "'" & nombre_tipo_documental & "'"
        End If
        If nombre_tipo_documental <> old_nombre_tipo_documental Then
            If Update_gabinete = "" Then
                Update_gabinete = "update " & Nombre_Gabinete & " set ID_TIPODOCUMENTO='" & id_tipo_documental & "'" &
                    ", TIPODOCUMENTO=" & ref_nombre_tipo_documental
                datos_campo = datos_campo & " Cambio de tipo documental " & old_nombre_tipo_documental & " a " & nombre_tipo_documental
            Else
                Update_gabinete = Update_gabinete & " ,ID_TIPODOCUMENTO='" & id_tipo_documental & "'" &
                   ", TIPODOCUMENTO=" & ref_nombre_tipo_documental
                datos_campo = datos_campo & " Cambio de tipo documental " & old_nombre_tipo_documental & " a " & nombre_tipo_documental
            End If
        End If
        If Update_gabinete <> "" Then
            Update_gabinete = Update_gabinete & " where ID=" & id_imagen
        End If

        '------------------------------------------------------------------
        'Construye paramteros produción documental
        '-----------------------------------------------------------------
        Dim isert_datos As String = ""
        Dim Update_producion As String = ""
        If old_nombre_archivo <> nombre_archivo Then
            Update_producion = "update registro_producion_documental set SEGUNDO_NOMBRE_DOCUMENTO='" & nombre_archivo & "'"
        End If
        If nombre_tipo_documental <> old_nombre_tipo_documental Then
            If Update_producion = "" Then
                Update_producion = "update registro_producion_documental set ID_TIPO_DOCUMENTO='" & id_tipo_documental & "'" &
                    ", DESCRIPCION_TIPO_DOCUMENTO='" & nombre_tipo_documental & "'"
                detalle_trans = "CAMBIA CLASE DOCUMENTO"
                campos_trans = "CAMBIA CLASE (" & old_nombre_tipo_documental &
                      ") A CLASE (" & nombre_tipo_documental & ")"
            Else
                Update_producion = Update_producion & " ,ID_TIPO_DOCUMENTO='" & id_tipo_documental & "'" &
                   ", DESCRIPCION_TIPO_DOCUMENTO='" & nombre_tipo_documental & "'"
                detalle_trans = "CAMBIA CLASE DOCUMENTO"
            End If
        End If
        If Update_producion <> "" Then
            Update_producion = Update_producion & " Where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & identificacion_producion
        End If
        If Update_gabinete = "" Then
            Actualiza_indice_documento_producion = "No se detectaron cambios para actualizar"
            Exit Function
        End If
        Dim id_expediente As Integer = 0
        Dim id_cert_indice_expediente As Long = 0
        Dim class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Dim stru_produccion_indice As stru_produccion_indice = Nothing
        Dim update_indice As String = ""
        Dim Ruta_archivo_xml As String = ""
        result = Me.Solicita_id_expediente_registro_produccion(identificacion_producion,
                                                              id_expediente, 0)
        If result <> "YES" Then
            Actualiza_indice_documento_producion = result
            Exit Function
        End If
        If id_expediente <> 0 Then
            result = class_ra_cert_indice_expediente.Solicita_existencia_indice_produccion(identificacion_producion,
                                                                                           id_cert_indice_expediente)
            If result <> "YES" Then
                Actualiza_indice_documento_producion = result
                Exit Function
            End If
            If id_cert_indice_expediente <> 0 Then
                result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
                If result <> "YES" Then
                    Actualiza_indice_documento_producion = result
                    Exit Function
                End If
                Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
                Dim class_zerro_fill_ As New Class_zero_fill
                result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
                If result <> "YES" Then
                    Actualiza_indice_documento_producion = result
                    Exit Function
                End If
                Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
                If Directory.Exists(Ruta_expediente) = False Then
                    Actualiza_indice_documento_producion = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                    Exit Function
                End If
                Ruta_expediente = Ruta_expediente & disco_carpeta_
                If Directory.Exists(Ruta_expediente) = False Then
                    Directory.CreateDirectory(Ruta_expediente)
                End If
                Dim expediente_zero_fil As String = id_expediente.ToString
                result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
                If result <> "YES" Then
                    Actualiza_indice_documento_producion = result
                    Exit Function
                End If
                Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
                '----------------------------------------------------------------------------
                'Actualiza indice archivo expediente archivo
                '-----------------------------------------------------------------------------
                Dim classgaexpediente As New ClassGaExpediente
                result = classgaexpediente.Actualiza_indice_tipo_documental_xml_expediente(Ruta_archivo_xml,
                                                                                           identificacion_producion,
                                                                                           nombre_tipo_documental,
                                                                                           xmlArchivo)
                If result <> "YES" Then
                    Actualiza_indice_documento_producion = result
                    Exit Function
                End If
                update_indice = "update ra_cert_indice_expediente set Tipologia_documental='" & nombre_tipo_documental & "'" &
                                " where id_cert_indice_expediente=" & id_cert_indice_expediente
            End If


        End If
        '--------------------------------------------------
        'Actualiza el nombre del y el aleas del expediente
        '--------------------------------------------------
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If result <> "YES" Then
            Actualiza_indice_documento_producion = result
            Exit Function
        End If
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     identificacion_producion & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','GESTOR DOCUMENTAL','" & campos_trans & "')"

        Dim update_gestion As String = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" &
                                    ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                    isert_datos

        Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
        & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES ( "
        SqlTransac = SqlTransac & "'" & id_imagen & "',"
        SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
        SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "',"
        SqlTransac = SqlTransac & "'" & date1al & "',"
        SqlTransac = SqlTransac & "'" & "NONE" & "',"
        SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
        SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "GESTOR DOCUMENTAL'" & ")"
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '------------------------------------------
            'Actualiza gabinete
            '------------------------------------------
            If Update_gabinete <> "" Then
                myCommand2.CommandText = Update_gabinete
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_indice_documento_producion = "Imposible actualizar la tabla gabinete cambios  : " & Update_gabinete
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Actualiza registro producción documental
            '------------------------------------------
            If Update_producion <> "" Then
                myCommand2.CommandText = Update_producion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_indice_documento_producion = "Imposible actualizar la tabla gabinete cambios  : " & Update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza log inventario
            '--------------------------------------------
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_indice_documento_producion = "Imposible actualizar la tabla log inventario cambios  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza indice log  docuarchi
            '--------------------------------------------
            myCommand2.CommandText = SqlTransac
            Switc = myCommand2.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_indice_documento_producion = "Imposible actualizar la tabla log docuarchi cambios  : " & SqlTransac
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------
            'Actualiza indice documento 
            '--------------------------------------------
            If update_indice <> "" Then
                myCommand2.CommandText = update_indice
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_indice_documento_producion = "Imposible actualizar la tabla indice expediente  : " & update_indice
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
            myTrans.Commit()
            'myConnection.Close()
            Actualiza_indice_documento_producion = "YES"
        Catch e As Exception
            Try

                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_indice_documento_producion = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_indice_documento_producion = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try
    End Function

    Function Activa_eliminar_documento_producion_documental(ByVal id_usuario_gestion As Integer,
                                                            ByVal identificacion_produccion As Integer) As String
        Try
            '-------------------------------------------
            'Solicita datos caracterización producción
            '-------------------------------------------
            Dim Result As String = ""
            Dim Nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_tipo_documetal As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim nombre_documento_radicado As String = ""
            Dim registro_radicado As String = ""
            Result = Solicita_documento_radicado_produccion(identificacion_produccion,
                                                            nombre_documento_radicado,
                                                            registro_radicado)
            If Result <> "YES" Then
                Activa_eliminar_documento_producion_documental = Result
                Exit Function
            End If
            If nombre_documento_radicado <> "" Then
                Activa_eliminar_documento_producion_documental = "El documento (" & nombre_documento_radicado & ") se encuentra relacionado radicado(" & registro_radicado & ") , imposible eliminar "
                Exit Function
            End If
            Result = Solicita_datos_caracterizacion_archivo_produccion(identificacion_produccion,
                                                                       Nombre_archivo,
                                                                       nombre_tipo_documental,
                                                                       id_tipo_documetal,
                                                                       id_expediente,
                                                                       id_imagen,
                                                                       nombre_gabinete,
                                                                       fecha_documento,
                                                                       numero_folios)
            If Result <> "YES" Then
                Activa_eliminar_documento_producion_documental = Result
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Activa_eliminar_documento_producion_documental = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Activa_eliminar_documento_producion_documental = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Activa_eliminar_documento_producion_documental = Result
                    Exit Function
                End If
                If stru_permisos_niveles.elimiminar_archivo = 0 Then
                    Activa_eliminar_documento_producion_documental = "El usuario no tiene persmiso para eliminar archivos  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            Dim Refclas As New ClassEliminarDocListResult
            Result = Refclas.EliminarDocumentosGabinete(id_imagen,
                                                              0,
                                                              nombre_gabinete,
                                                              1,
                                                              1,
                                                              0,
                                                              -1,
                                                              "PRODUCCIONDOCUMENTAL")
            If Result <> "YES" Then
                Activa_eliminar_documento_producion_documental = Result
                Exit Function
            End If
            Activa_eliminar_documento_producion_documental = "YES"
            Exit Function
        Catch ex As Exception
            Activa_eliminar_documento_producion_documental = "Inconsistencia general función Activa_eliminar_documento_producion_documental " & ex.Message
        End Try
    End Function

    Function Compartir_documento_produccion_documental(ByVal id_usuario_gestion As Integer,
                                                       ByVal id_registro_producion() As Long,
                                                       ByRef iframe As Object,
                                                       ByRef ref_update_panel As UpdatePanel,
                                                       ByRef modal_popup As Object) As String
        Try
            Dim Result As String = ""
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim stru_documento_compartido() As stru_documentos_compartidos = Nothing
            If id_registro_producion Is Nothing Then
                Compartir_documento_produccion_documental = "Debe selecionar los documentos a compartir"
                Exit Function
            End If
            For i As Integer = 0 To id_registro_producion.Length - 1
                Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion(i),
                                                                              nombre_archivo,
                                                                              nombre_tipo_documental,
                                                                              id_clase_documento,
                                                                              id_expediente,
                                                                              id_imagen,
                                                                              nombre_gabinete,
                                                                              fecha_documento,
                                                                              numero_folios)
                If Result <> "YES" Then
                    Compartir_documento_produccion_documental = Result
                    Exit Function
                End If
                Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
                Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
                Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
                Dim estado_propietario As String = ""
                Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
                Dim id_nivel As Integer = 0
                Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                             id_nivel)
                If Result <> "YES" Then
                    Compartir_documento_produccion_documental = Result
                    Exit Function
                End If
                Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                              id_nivel,
                                                                              estado_propietario)
                If Result <> "YES" Then
                    Compartir_documento_produccion_documental = Result
                    Exit Function
                End If
                If estado_propietario = "NO" Then
                    Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                           id_usuario_gestion,
                                                                                                           stru_permisos_niveles)
                    If Result <> "YES" Then
                        Compartir_documento_produccion_documental = Result
                        Exit Function
                    End If
                    If stru_permisos_niveles.compartir_archivo = 0 Then
                        Compartir_documento_produccion_documental = "El usuario no tiene persmiso para compartir archivos  del expediente, debido a que el expediente pertenece a un nivel de otro usuario archivo relacionado (" & nombre_archivo & ")"
                        Exit Function
                    End If
                End If
                '----------------------------------------------------
                'Solicita datos de auditoria del sistema de gabinetes
                '----------------------------------------------------
                Dim refclasgabinete As New ClassDaGabinete
                Dim datos_log As String = ""
                Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(id_imagen,
                                                                          nombre_gabinete,
                                                                          datos_log)
                If Result <> "YES" Then
                    Compartir_documento_produccion_documental = Result
                    Exit Function
                End If
                '----------------------------------------------------
                'Solicita matriz documentos almacenados
                '----------------------------------------------------
                Dim matri_documentos_almacenados() As String = Nothing
                Dim Refclas As New ClassWorflowVisor
                Dim ClassDaGabinete As New ClassDaGabinete
                Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                         nombre_gabinete,
                                                                                         matri_documentos_almacenados)
                If Result <> "YES" Then
                    Compartir_documento_produccion_documental = Result
                    Exit Function
                End If
                If Not matri_documentos_almacenados Is Nothing Then

                    Dim file_inf As New FileInfo(matri_documentos_almacenados(1))
                    ReDim Preserve stru_documento_compartido(i)
                    stru_documento_compartido(i).id_imagen = id_imagen
                    stru_documento_compartido(i).nombre_gabinete = nombre_gabinete
                    stru_documento_compartido(i).extension = file_inf.Extension
                    Dim Class_da_extension As New Class_da_extension
                    Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                                stru_documento_compartido(i).tipo_documento)
                    If Result <> "YES" Then
                        Compartir_documento_produccion_documental = Result
                        Exit Function
                    End If


                Else
                    Compartir_documento_produccion_documental = "Imposible encontrar documentos relacionados al registro (" & id_registro_producion(i) & ") de producción documental"
                    Exit Function
                End If
            Next
            HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO") = stru_documento_compartido
            HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = "COMPARTIR WORKFLOW"
            iframe.Attributes("SRC") = "../gestion/WebFormGaCompartirDocumento.aspx"
            ref_update_panel.Update()
            modal_popup.Show()
            Compartir_documento_produccion_documental = "YES"
            Exit Function
        Catch ex As Exception
            Compartir_documento_produccion_documental = "Inconsistencia general función Compartir_documento_produccion_documental " & ex.Message
        End Try
    End Function
    Function Descarga_documentos_expediente_produccion(ByVal id_produccion As Long,
                                                       ByVal totalCount As Integer,
                                                       ByRef id_Cont As Integer,
                                                       ByVal id_usuario_gestion As Integer,
                                                       ByRef state_propietary As Integer,
                                                       ByRef stru_file_system() As stru_file_system,
                                                       ByRef out_source_file_zip As String,
                                                       ByRef url_file_zip As String,
                                                       ByRef name_document As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna doocumentos de producción documental para descargar
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_produccion         : Representa la identificación del documento en la 
        '                        producción documental
        'totalCount            : Representa el total de documentos a  descargar
        'id_Cont               : Representa el contador de documentos
        'id_usuario_gestion    : Representa el usuario de gestión de la descarga
        'state_propietary      : Representa el propietario del expediente
        'stru_file_system      : Representa la estructura de archivos a descargar
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'rut_file_zip         : Retorna la ruta del documento ZIP
        'url_file_zip         : Retorna la url para la descarga del ZIP
        'name_document        : Retorna el nombre del documento a descargar
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            out_source_file_zip = ""
            url_file_zip = ""
            name_document = ""
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_produccion,
                                                                          nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_clase_documento,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Descarga_documentos_expediente_produccion = "YES"
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Descarga_documentos_expediente_produccion = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Descarga_documentos_expediente_produccion = Result
                Exit Function
            End If
            state_propietary = 1
            If estado_propietario = "NO" Then
                state_propietary = 0
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Descarga_documentos_expediente_produccion = Result
                    Exit Function
                End If
                If stru_permisos_niveles.descarga_archivo = 0 Then
                    Descarga_documentos_expediente_produccion = "El usuario no tiene persmiso para descargar archivo  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Solicita datos de auditoria del sistema de gabinetes
            '----------------------------------------------------
            Dim refclasgabinete As New ClassDaGabinete
            Dim datos_log As String = ""
            Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(id_imagen,
                                                                      nombre_gabinete,
                                                                      datos_log)
            If Result <> "YES" Then
                Descarga_documentos_expediente_produccion = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Solicita matriz documentos almacenados
            '----------------------------------------------------
            Dim matri_documentos_almacenados() As String = Nothing
            Dim Refclas As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete,
                                                                                     matri_documentos_almacenados)
            If Result <> "YES" Then
                Descarga_documentos_expediente_produccion = Result
                Exit Function
            End If
            Dim Icont_Stru As Integer = 0
            If stru_file_system Is Nothing Then
                ReDim Preserve stru_file_system(Icont_Stru)
            Else
                Icont_Stru = stru_file_system.Length
                ReDim Preserve stru_file_system(Icont_Stru)
            End If
            If matri_documentos_almacenados.Length = 2 Then
                stru_file_system(Icont_Stru).source_file = matri_documentos_almacenados(1)
                stru_file_system(Icont_Stru).name_file_tipo_logia = nombre_tipo_documental
                Dim fileinf As New FileInfo(matri_documentos_almacenados(1))
                stru_file_system(Icont_Stru).name_file = fileinf.Name
            Else
                stru_file_system(Icont_Stru).source_file = ""
                stru_file_system(Icont_Stru).name_file_tipo_logia = nombre_tipo_documental
                Dim fileinf As New FileInfo(matri_documentos_almacenados(1))
                stru_file_system(Icont_Stru).name_file = fileinf.Name
                Dim Icont_ As Integer = 0
                For i As Integer = 1 To matri_documentos_almacenados.Length - 1
                    Dim fileinf_ As New FileInfo(matri_documentos_almacenados(i))
                    Dim file_name As String = fileinf_.Name
                    If i = 1 Then
                        ReDim Preserve stru_file_system(Icont_Stru).stru_files_da(Icont_)
                        stru_file_system(Icont_Stru).stru_files_da(Icont_).source_file = matri_documentos_almacenados(i)
                        stru_file_system(Icont_Stru).stru_files_da(Icont_).name_file = file_name
                    Else
                        ReDim Preserve stru_file_system(Icont_Stru).stru_files_da(Icont_)
                        stru_file_system(Icont_Stru).stru_files_da(Icont_).source_file = matri_documentos_almacenados(i)
                        stru_file_system(Icont_Stru).stru_files_da(Icont_).name_file = file_name
                    End If
                    Icont_ = Icont_ + 1
                Next
            End If
            'Genera el archivo ZIP para la descarga
            Dim source_file_temp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA")
            Dim url_source_file_zip As String = "../Temp_Gestion/" & id_usuario_gestion & "/DESCARGA/"

            Dim Class_fyle_system As New Class_fyle_system
            If totalCount = id_Cont Then
                Result = Class_fyle_system.Add_zip_expediente_files(id_expediente,
                                                                    source_file_temp,
                                                                    url_source_file_zip,
                                                                    stru_file_system,
                                                                    out_source_file_zip,
                                                                    url_file_zip,
                                                                    name_document)
                If Result <> "YES" Then
                    Descarga_documentos_expediente_produccion = Result
                    Exit Function
                End If
            End If
            Descarga_documentos_expediente_produccion = "YES"
        Catch ex As Exception
            Descarga_documentos_expediente_produccion = "Inconsitencia general funcion Descarga_documentos_expediente_produccion " & ex.Message
        End Try
    End Function
    Function Descarga_documento_producion_documental(ByVal id_usuario_gestion As Integer,
                                                     ByVal id_registro_producion As Integer,
                                                     ByRef iframe As Object,
                                                     ByRef ref_update_panel As UpdatePanel,
                                                     ByRef Hidden_ruta_archivo As Object) As String
        Try
            Dim Result As String = ""
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion,
                                                                          nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_clase_documento,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Descarga_documento_producion_documental = Result
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Descarga_documento_producion_documental = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Descarga_documento_producion_documental = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Descarga_documento_producion_documental = Result
                    Exit Function
                End If
                If stru_permisos_niveles.descarga_archivo = 0 Then
                    Descarga_documento_producion_documental = "El usuario no tiene persmiso para descargar archivo  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Solicita datos de auditoria del sistema de gabinetes
            '----------------------------------------------------
            Dim refclasgabinete As New ClassDaGabinete
            Dim datos_log As String = ""
            Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(id_imagen,
                                                                      nombre_gabinete,
                                                                      datos_log)
            If Result <> "YES" Then
                Descarga_documento_producion_documental = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Solicita matriz documentos almacenados
            '----------------------------------------------------
            Dim matri_documentos_almacenados() As String = Nothing
            Dim Refclas As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete,
                                                                                     matri_documentos_almacenados)
            If Result <> "YES" Then
                Descarga_documento_producion_documental = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Prepara archivos para descarga
            '---------------------------------------------------
            Dim ruta_temporal As String = ""
            If matri_documentos_almacenados.Length = 1 Then
                ruta_temporal = matri_documentos_almacenados(0)
            Else
                For i As Integer = 1 To matri_documentos_almacenados.Length - 1
                    If i = 1 Then
                        ruta_temporal = matri_documentos_almacenados(i)
                    Else
                        ruta_temporal = ruta_temporal & "," & matri_documentos_almacenados(i)
                    End If
                Next
            End If
            Hidden_ruta_archivo.value = ruta_temporal
            iframe.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            ref_update_panel.Update()
            Result = refclasgabinete.Registra_Auditoria_Eventos(nombre_gabinete,
                                                                "Todas las páginas (" & matri_documentos_almacenados.Length - 2 & ")" & " Imagen Principal " & matri_documentos_almacenados(1),
                                                                id_imagen,
                                                                datos_log,
                                                                "Guardar")
            If Result <> "YES" Then
                Descarga_documento_producion_documental = "Imposible registrar datos log " & Result
                Exit Function
            End If
            Descarga_documento_producion_documental = "YES"
        Catch ex As Exception
            Descarga_documento_producion_documental = "Inconsistencia general función Descarga_documento_producion_documental " & ex.Message
        End Try
    End Function

    Function Solicita_radicar_documento_produccion(ByVal id_registro_producion_documental As Integer,
                                                   ByVal id_usuario_gestion As Integer,
                                                   ByRef Id_Plantilla As Integer,
                                                   ByRef nombre_plantilla_radicado As String,
                                                   ByRef id_relacion_gestion_remitente As Integer) As String
        Try
            '------------------------------------------------
            'Solicita permiso radicación documento interno
            '------------------------------------------------
            If HttpContext.Current.Session.Item("GA_Radicar_enviar_documento") = 0 Then
                Solicita_radicar_documento_produccion = "El usuario no tiene permisos para radicar y enviar el documento"
                Exit Function
            End If
            Dim Result As String = ""
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion_documental,
                                                               nombre_archivo,
                                                               nombre_tipo_documental,
                                                               id_clase_documento,
                                                               id_expediente,
                                                               id_imagen,
                                                               nombre_gabinete,
                                                               fecha_documento,
                                                               numero_folios)
            If Result <> "YES" Then
                Solicita_radicar_documento_produccion = Result
                Exit Function
            End If
            '------------------------------------------------------
            'Solicita id nombre plantilla radicado predeterminada 
            'para radicación interna
            '-----------------------------------------------------
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Solicita_radicar_documento_produccion = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Solicita_radicar_documento_produccion = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Solicita_radicar_documento_produccion = Result
                    Exit Function
                End If
                If stru_permisos_niveles.radicar_archivo = 0 Then
                    Solicita_radicar_documento_produccion = "El usuario no tiene persmiso para radicar archivos  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = Class_system_plantilla_radicado.Solicita_nombre_id_plantilla_radicación_interna_default(nombre_plantilla_radicado,
                                                                                                             Id_Plantilla,
                                                                                                             1)
            If Result <> "YES" Then
                Solicita_radicar_documento_produccion = Result
                Exit Function
            End If
            'Dim ClassRadicador As New ClassRadicador
            'ClassRadicador.Registra_usuario_gestion_remitente_externo(nombre_plantilla_radicado,
            '                                                          Id_Plantilla,
            '                                                          id_usuario_gestion,
            '                                                          id_relacion_gestion_remitente)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            Solicita_radicar_documento_produccion = "YES"
            Exit Function
            ''-------------------------------------------------
            ''Lista campos adicionales plantilla 
            ''-------------------------------------------------
            'Dim Matri_Campos_plantilla() As Campos_Plantilla = Nothing
            'Dim Estado_opcion_fecha As Integer = 0
            'Dim Estado_opcion_cita_respuesta As Integer = 0
            'Dim Estado_opcion_radicado_general As Integer = 0
            'Dim Refclas As New ClassRadicador
            'Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            'Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(Id_Plantilla, _
            '                                                                                    Matri_Campos_plantilla, _
            '                                                                                    Estado_opcion_fecha, _
            '                                                                                    Estado_opcion_cita_respuesta, _
            '                                                                                    Estado_opcion_radicado_general)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''---------------------------------------
            ''------Asigna datos campos validacion
            ''---------------------------------------
            'Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            'Dim matri_validacion() As validacion_plantilla
            'Erase matri_validacion
            'Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(Id_Plantilla,
            '                                                                           matri_validacion)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            'If Not matri_validacion Is Nothing Then
            '    For i2 As Integer = 0 To matri_validacion.Length - 1
            '        For i3 As Integer = 0 To Matri_Campos_plantilla.Length - 1
            '            If Matri_Campos_plantilla(i3).Campo_Plantilla = matri_validacion(i2).Campo_Plantilla Then
            '                Matri_Campos_plantilla(i3).TIPO_SCRIPT = matri_validacion(i2).TIPO_SCRIPT
            '                Matri_Campos_plantilla(i3).COMBINACION_TECLA = matri_validacion(i2).COMBINACION_TECLA
            '                Matri_Campos_plantilla(i3).VALOR_SCRIPT = matri_validacion(i2).VALOR_SCRIPT
            '                Matri_Campos_plantilla(i3).ESTADO_ESCRIPT = matri_validacion(i2).ESTADO_ESCRIPT
            '                Matri_Campos_plantilla(i3).PLATAFORMA_SCRIPT = matri_validacion(i2).PLATAFORMA_SCRIPT
            '                Matri_Campos_plantilla(i3).ID_SCRIPT = matri_validacion(i2).ID_SCRIPT

            '            End If
            '        Next
            '    Next
            'End If
            ''----------------------------------------------------
            ''Retorna el tipo de validación del campo comparación
            ''----------------------------------------------------
            'Dim nombre_campo As String = ""
            'Dim Tipo_script As String = ""
            'Dim id_SCRIPT As Integer
            'Result = Refclas.Retorna_Tipo_Validacion_Campo(Matri_Campos_plantilla,
            '                                               "REMITENTE_COR",
            '                                               Tipo_script,
            '                                               id_SCRIPT)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''---------------------------------------------------
            ''Retorna nombre plantilla validación
            ''---------------------------------------------------
            'Dim nombre_plantilla_validacion As String = ""
            'Dim Class_plantilla_validacion As New Class_plantilla_validacion
            'Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_SCRIPT, nombre_plantilla_validacion)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''---------------------------------------------------
            ''Solicita identificación plantilla validación
            ''---------------------------------------------------
            'Dim id_plantilla_validacion As Integer = 0
            'Result = Refclas.Retorna_id_plantilla_validacion(nombre_plantilla_validacion, id_plantilla_validacion)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''--------------------------------------------------
            ''Verifica la existencia de campo de relación de 
            ''usuario interno con destinatario externo
            ''--------------------------------------------------
            'Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            'Dim estado_existencia_campo_relacion As String = "NO"
            'Result = Class_campos_plantilla_validacion.Verifica_existencia_campo_identificacion_usuario_gestion_plantilla(id_plantilla_validacion,
            '                                                                                                              estado_existencia_campo_relacion)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            'If estado_existencia_campo_relacion = "NO" Then
            '    Solicita_radicar_documento_produccion = "Imposible encontrar el campo id_interno_radicado en la plantilla de validación " & nombre_plantilla_validacion & _
            '        " contacte a su administrador para que agregue el campo a la plantilla " & nombre_plantilla_validacion
            '    Exit Function
            'End If
            ''-------------------------------------------------
            ''Retorna campo primary plantilla validacion
            ''-------------------------------------------------
            'Dim nombre_campo_primary As String = ""
            'Result = Refclas.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion, _
            '                                                                nombre_campo_primary)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''------------------------------------------------
            ''Solicita relación entre usuario de gestion y 
            ''remitente interno
            ''------------------------------------------------
            'Result = Class_plantilla_validacion.Solicita_id_relacion_usuario_gestion_como_remitente_interno(id_usuario_gestion,
            '                                                                                                nombre_plantilla_validacion,
            '                                                                                                nombre_campo_primary,
            '                                                                                                id_relacion_gestion_remitente)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''----------------------------------------------------
            ''Solicita campos relación remitente usuario gestión
            ''----------------------------------------------------
            'Dim Class_relacion_script_plantilla As New Class_relacion_script_plantilla
            'Dim matri_campos_relacion_getion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO = Nothing
            'Result = Class_relacion_script_plantilla.Solicta_campos_relacion_remitente_usuario_gestion(id_SCRIPT,
            '                                                                                           matri_campos_relacion_getion_remitente)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''---------------------------------------------------
            ''Solicita el nombre del campo remitente en la 
            ''relación de capos entre el usuario de gestión y el 
            ''usuario remitente en la plantilla validación
            ''---------------------------------------------------
            'Dim nombre_campo_nombre_usuario_remitente As String = ""
            'Result = Class_campos_plantilla_validacion.Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente("Nombre_Remitente",
            '                                                                                                                      matri_campos_relacion_getion_remitente,
            '                                                                                                                      nombre_campo_nombre_usuario_remitente)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            'If nombre_campo_nombre_usuario_remitente = "" Then
            '    Solicita_radicar_documento_produccion = "Debe relacionar los campos (nombre remitente) en la plantilla de validación " & nombre_plantilla_validacion & " en la columna (Relacion usuario gestion) del administrador de plantillas"
            '    Exit Function
            'End If
            ''---------------------------------------------------
            ''Solicita datos de caracterización usuario gestión
            ''----------------------------------------------------
            'Dim nombre_usuario_gestion As String = ""
            'Dim correo_electronico As String = ""
            'Dim telefono As String = ""
            'Dim identificacion As String = ""
            'Dim direccion As String = ""
            'Dim Class_remit_dest_interno As New Class_remit_dest_interno
            'Result = Class_remit_dest_interno.Solicita_datos_de_caracterizacion_usuario_gestion(id_usuario_gestion,
            '                                                           nombre_usuario_gestion,
            '                                                           correo_electronico,
            '                                                           telefono,
            '                                                           identificacion,
            '                                                           direccion)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            ''-------------------------------------------------------
            ''Verifica existencia del nombre del usuario de gestión
            ''como remitente de correspondencia
            ''-------------------------------------------------------
            'Dim estado_existencia_usuario_gestion_remitente As String = "NO"
            'Result = Class_plantilla_validacion.Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion(nombre_usuario_gestion,
            '                                                                                                            nombre_plantilla_validacion,
            '                                                                                                            nombre_campo_nombre_usuario_remitente,
            '                                                                                                            estado_existencia_usuario_gestion_remitente)
            'If Result <> "YES" Then
            '    Solicita_radicar_documento_produccion = Result
            '    Exit Function
            'End If
            'If estado_existencia_usuario_gestion_remitente = "NO" Then

            '    '----------------------------------------------------------
            '    'Asigna los datos del usuario de gestión 
            '    '----------------------------------------------------------
            '    Result = Class_campos_plantilla_validacion.Asigna_datos_estructutura_plantilla_validacion_usuario_gestion(id_usuario_gestion,
            '                                              nombre_usuario_gestion,
            '                                              correo_electronico,
            '                                              telefono,
            '                                              identificacion,
            '                                              direccion,
            '                                              id_usuario_gestion,
            '                                              matri_campos_relacion_getion_remitente)
            '    If Result <> "YES" Then
            '        Solicita_radicar_documento_produccion = Result
            '        Exit Function
            '    End If
            '    '------------------------------------------------------------
            '    'Registra el nuevo remitente con los datos del usuario
            '    'de gestión
            '    '------------------------------------------------------------
            '    Dim id_remitente As Integer = 0
            '    Result = Class_plantilla_validacion.Registra_usuario_remitente_interno(nombre_plantilla_validacion,
            '                                                                           id_usuario_gestion,
            '                                                                           nombre_campo_primary,
            '                                                                           matri_campos_relacion_getion_remitente,
            '                                                                           id_remitente)
            '    If Result <> "YES" Then
            '        Solicita_radicar_documento_produccion = Result
            '        Exit Function
            '    Else
            '        id_relacion_gestion_remitente = id_remitente
            '    End If
            'Else
            '    If id_relacion_gestion_remitente = 0 Then
            '        '---------------------------------------------------
            '        'Solicita datos de caracterización usuario gestión
            '        '----------------------------------------------------               
            '        Result = Class_remit_dest_interno.Solicita_datos_de_caracterizacion_usuario_gestion(id_usuario_gestion,
            '                                                                   nombre_usuario_gestion,
            '                                                                   correo_electronico,
            '                                                                   telefono,
            '                                                                   identificacion,
            '                                                                   direccion)
            '        If Result <> "YES" Then
            '            Solicita_radicar_documento_produccion = Result
            '            Exit Function
            '        End If
            '        '----------------------------------------------------------
            '        'Asigna los datos del usuario de gestión 
            '        '----------------------------------------------------------
            '        Result = Class_campos_plantilla_validacion.Asigna_datos_estructutura_plantilla_validacion_usuario_gestion(id_usuario_gestion,
            '                                                                                                                  nombre_usuario_gestion,
            '                                                                                                                  correo_electronico,
            '                                                                                                                  telefono,
            '                                                                                                                  identificacion,
            '                                                                                                                  direccion,
            '                                                                                                                  id_usuario_gestion,
            '                                                                                                                  matri_campos_relacion_getion_remitente)
            '        If Result <> "YES" Then
            '            Solicita_radicar_documento_produccion = Result
            '            Exit Function
            '        End If
            '        '---------------------------------------------------
            '        'Verfica la existencia del usuario de gestión en la
            '        'tabla de validación
            '        '---------------------------------------------------
            '        Dim id_remitente As Integer = 0
            '        Result = Class_plantilla_validacion.Verifica_existencia_usuario_remitente_plantilla_validacion(nombre_campo_nombre_usuario_remitente,
            '                                                                                                       nombre_plantilla_validacion,
            '                                                                                                       nombre_usuario_gestion,
            '                                                                                                       nombre_campo_primary,
            '                                                                                                       id_remitente)
            '        If Result <> "YES" Then
            '            Solicita_radicar_documento_produccion = Result
            '            Exit Function
            '        End If
            '        '------------------------------------------------------------
            '        'Actualiza usuario remitente con los datos del usuario de
            '        'gestión
            '        '-------------------------------------------------------------
            '        Result = Class_plantilla_validacion.Actualiza_usuario_remitente_interno(nombre_plantilla_validacion,
            '                                                                                nombre_campo_primary,
            '                                                                                matri_campos_relacion_getion_remitente,
            '                                                                                id_remitente)
            '        If Result <> "YES" Then
            '            Solicita_radicar_documento_produccion = Result
            '            Exit Function
            '        Else
            '            id_relacion_gestion_remitente = id_usuario_gestion
            '        End If
            '    End If

            'End If
            'Solicita_radicar_documento_produccion = "YES"
        Catch ex As Exception
            Solicita_radicar_documento_produccion = "Inconsistencia general función Solicita_radicar_documento_producicion " & ex.Message
        End Try
    End Function

    'CREA INTERFACE FORMULARIO RADICACION ENTRANTE
    Function Genera_Interface_Radicacion_Entrante(ByVal Codigo_Plantilla As String,
                                                  ByVal Tipo_Plantilla As String,
                                                  ByRef Page1 As Page,
                                                  ByVal nombre_plantilla As String,
                                                  ByVal id_producion_documental As Integer) As String
        Try
            Dim color = System.Drawing.ColorTranslator.FromHtml("#f5f5f5")
            Dim Resultado_General As String = "YES"
            Dim Result As String = ""
            Dim _Plantilla_Impre() As String
            Dim _Campo_NIT As String = ""
            Dim _Campo_NIT_REAL As String = ""
            Erase _Plantilla_Impre
            Dim refclas_radicado As New ClassRadicador
            Dim Ref_classtrdDocumental As New ClassTrdDocumental
            '---------------------------------------------------------------
            'Lista datos de caracterización documento produción documental
            '--------------------------------------------------------------
            Dim ref_clas_producion As New ClassGaProducionDocumental
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim nombre_archivo_documento As String = ""
            Result = ref_clas_producion.Solicita_datos_caracterizacion_archivo_produccion(id_producion_documental,
                                                                                          nombre_archivo_documento,
                                                                                          "",
                                                                                          0,
                                                                                          0,
                                                                                          id_imagen,
                                                                                          nombre_gabinete,
                                                                                          fecha_documento,
                                                                                          numero_folios)
            If Result <> "YES" Then
                Genera_Interface_Radicacion_Entrante = Result
                Exit Function
            End If
            Dim tempo_nombre_archivo As String = ""
            If Not HttpContext.Current.Session.Item("RA_ADJUNTOS_RADICADO_INTERNO") Is Nothing Then
                For i2 As Integer = 0 To HttpContext.Current.Session.Item("RA_ADJUNTOS_RADICADO_INTERNO").Length - 1
                    Result = Me.Solicita_nombre_documento_producion_por_id(HttpContext.Current.Session.Item("RA_ADJUNTOS_RADICADO_INTERNO")(i2),
                                                                          tempo_nombre_archivo)
                    If Result <> "YES" Then
                        Genera_Interface_Radicacion_Entrante = Result
                        Exit Function
                    Else
                        nombre_archivo_documento = nombre_archivo_documento & "-" & tempo_nombre_archivo
                    End If
                Next
            End If
            nombre_archivo_documento = Left(nombre_archivo_documento,
                                            240)

            '----lista los datos de la entida de radicacion
            Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            Result = Class_empresa_gestion_documental.Solicita_detalle_empresa_gestion_radicacion(HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                                                  _Plantilla_Impre)
            If Result <> "YES" Then
                Genera_Interface_Radicacion_Entrante = Result
                Exit Function
            End If
            Dim hiden_Estado_opcion_fecha As HtmlInputHidden = Page1.FindControl("Hidden_Estado_opcion_fecha")
            If hiden_Estado_opcion_fecha Is Nothing Then
                Genera_Interface_Radicacion_Entrante = "Imposible encontrar el control Hidden_Estado_opcion_fecha Genera_Interface_Radicacion_Entrante"
                Exit Function
            End If
            Dim hiden_Estado_opcion_cita_respuesta As HtmlInputHidden = Page1.FindControl("Hidden_Estado_opcion_cita_respuesta")
            If hiden_Estado_opcion_fecha Is Nothing Then
                Genera_Interface_Radicacion_Entrante = "Imposible encontrar el control Hidden_Estado_opcion_cita_respuesta Genera_Interface_Radicacion_Entrante"
                Exit Function
            End If
            Dim hiden_opcion_radicado_general As HtmlInputHidden = Page1.FindControl("Hidden_opcion_radicado_general")
            If hiden_Estado_opcion_fecha Is Nothing Then
                Genera_Interface_Radicacion_Entrante = "Imposible encontrar el control Hidden_Estado_opcion_radicado_general Genera_Interface_Radicacion_Entrante"
                Exit Function
            End If
            hiden_Estado_opcion_fecha.Value = "0"
            '***************************************************************
            'Lista las opciones plantilla
            '***************************************************************
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(Codigo_Plantilla,
                                                                                           Estado_opcion_fecha,
                                                                                           Estado_opcion_cita_respuesta,
                                                                                           Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Genera_Interface_Radicacion_Entrante = Result
                Exit Function
            End If
            hiden_Estado_opcion_fecha.Value = Estado_opcion_fecha
            hiden_Estado_opcion_fecha.Value = Estado_opcion_cita_respuesta
            hiden_Estado_opcion_fecha.Value = Estado_opcion_radicado_general
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(Codigo_Plantilla,
                                                                                                Matri_Datos,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Genera_Interface_Radicacion_Entrante = Result
                Exit Function
            End If
            '---------------------------------------
            '------Asigna datos campos validacion
            '---------------------------------------
            Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            Dim matri() As validacion_plantilla
            Erase matri
            Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(Codigo_Plantilla,
                                                                                       matri)
            If Result <> "YES" Then
                Genera_Interface_Radicacion_Entrante = Result
                Exit Function
            End If
            If Not matri Is Nothing Then
                For i2 As Integer = 0 To matri.Length - 1
                    For i3 As Integer = 0 To Matri_Datos.Length - 1
                        If Matri_Datos(i3).Campo_Plantilla = matri(i2).Campo_Plantilla Then
                            Matri_Datos(i3).TIPO_SCRIPT = matri(i2).TIPO_SCRIPT
                            Matri_Datos(i3).COMBINACION_TECLA = matri(i2).COMBINACION_TECLA
                            Matri_Datos(i3).VALOR_SCRIPT = matri(i2).VALOR_SCRIPT
                            Matri_Datos(i3).ESTADO_ESCRIPT = matri(i2).ESTADO_ESCRIPT
                            Matri_Datos(i3).PLATAFORMA_SCRIPT = matri(i2).PLATAFORMA_SCRIPT
                            Matri_Datos(i3).ID_SCRIPT = matri(i2).ID_SCRIPT

                        End If
                    Next
                Next
            End If
            Dim _LabelboxIco As Label() = {}
            Dim m_TextBoxes() As TextBox = {}
            Dim LabelBox() As Label = {}
            'Dim Picture() As PictureBox = {}
            Dim _ComboBox() As DropDownList = {}
            Dim _CommamdBoton() As Button = {}
            Dim _image() As ImageButton = {}
            Dim hiden_field() As HiddenField = {}
            Dim Contador_Control As Integer = 0
            Dim Contador_Combo As Integer = 0
            Dim Contador_Text As Integer = 0
            Dim z2 As Integer = 0
            Dim pane As Panel = Page1.FindControl("PanelRadicacion")
            Dim panetitle As Panel = Page1.FindControl("PanelTitulo")
            Dim hiden As Object = Page1.FindControl("HiddenPlantilla")
            Dim bot As Button = Page1.FindControl("ButtonDAPCERRAR")
            Dim tablecontrolesdinamicos As Table = Page1.FindControl("tablecontrolesdinamicos")
            hiden.value = nombre_plantilla
            Dim Update As UpdatePanel = Page1.FindControl("UpdatePnaelcontrolesradicacion")
            'Update.ID = "ActualizaindiceImage"
            'Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Table As Table = Page1.FindControl("TableControles")
            Dim tabledest As Table = Page1.FindControl("Tableremitente")
            Dim tableseparacion As Table = Page1.FindControl("Tableseparacion")
            Dim TableTitle As New Table
            Dim objRow As TableRow
            Dim objCell As TableCell
            '***************************************************************
            'Agrega los detalles de las plantillas
            '***************************************************************
            Dim i As Integer = 0
            ReDim Preserve _LabelboxIco(i)
            _LabelboxIco(i) = New Label
            For z2 = 0 To UBound(_Plantilla_Impre)
                'ReDim Preserve _LabelboxIco(z2)
                If z2 = 0 Then
                    _LabelboxIco(i).Text = _Plantilla_Impre(z2)

                Else
                    _LabelboxIco(i).Text = _LabelboxIco(i).Text & " - " & _Plantilla_Impre(z2)
                End If

            Next
            objRow = New TableRow
            objCell = New TableCell
            _LabelboxIco(i).Text = _LabelboxIco(i).Text & " - Plantilla radicación : " & nombre_plantilla
            _LabelboxIco(i).ForeColor = Drawing.Color.White
            _LabelboxIco(i).Font.Size = 8
            _LabelboxIco(Contador_Control).Font.Size = 8
            objCell.Controls.Add(_LabelboxIco(i))
            objRow.Cells.Add(objCell)
            TableTitle.Rows.Add(objRow)
            ''**********************************************
            ''Agrega separador caracteristicas destinatario
            ''***********************************************
            Dim Tableseparador_documento As Table = Page1.FindControl("Tableseparador_documento")
            Tableseparador_documento.Attributes.Add("width", "100%")
            Tableseparador_documento.Controls.Clear()
            objRow = New TableRow
            objCell = New TableCell
            objCell.Width = 1200
            objCell.HorizontalAlign = HorizontalAlign.Center
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            LabelBox(Contador_Control).Text = ""
            LabelBox(Contador_Control).Font.Size = 10
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            Tableseparador_documento.Rows.Add(objRow)
            '***************************************
            'Agrega label tramite documento
            '***************************************
            objRow = New TableRow
            objCell = New TableCell
            objCell.Width = 100
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Dim valor_documento As String = ""
            Result = Ref_classtrdDocumental.Formato_sub_serie("TRAMITE DOCUMENTO",
                                                              valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            LabelBox(Contador_Control).Width = 100
            LabelBox(Contador_Control).Font.Name = "Arial"
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '***************************************
            'Agrega combo documento
            '***************************************
            objCell = New TableCell
            objCell.Width = 140
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "RE_Descripcion_Documento"
            _ComboBox(Contador_Control).Width = 250
            _ComboBox(Contador_Control).Attributes.Add("onchange", "asignar_fecha_vence_tramite();")
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            objCell.Controls.Add(_ComboBox(Contador_Control))
            objRow.Cells.Add(objCell)
            ''******************************
            ' Carga tipos documentales
            ''******************************
            Result = Me.Lista_tipos_documentales_de_radicacion_interna(_ComboBox(Contador_Control),
                                                                       Codigo_Plantilla)
            If Result <> "YES" Then
                Resultado_General = Result

            End If
            '*******************************************************************
            'Agrega label campo anexo
            '*******************************************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.Attributes.Add("cellpadding", "50")
            objCell.HorizontalAlign = HorizontalAlign.Right
            'objCell.VerticalAlign = VerticalAlign.Top
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("ANEXOS *",
                                                              valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            LabelBox(Contador_Control).Width = 100
            LabelBox(Contador_Control).Font.Name = "Arial"
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '*******************************************************************
            'Agrega campo texto anexos
            '*******************************************************************
            objCell = New TableCell
            objCell.Width = 300
            ReDim Preserve m_TextBoxes(Contador_Control)
            m_TextBoxes(Contador_Control) = New TextBox
            m_TextBoxes(Contador_Control).ID = "RE_ANEXOS_COR" & "-" & "ANEXOS_COR" & "-" & "VARCHAR"
            'm_TextBoxes(Contador_Control).Columns = 50
            m_TextBoxes(Contador_Control).Width = 300
            m_TextBoxes(Contador_Control).Rows = 2
            m_TextBoxes(Contador_Control).CssClass = "form-control"
            m_TextBoxes(Contador_Control).TextMode = TextBoxMode.MultiLine
            If m_TextBoxes(Contador_Control).Text = "" Then
                m_TextBoxes(Contador_Control).Text = "Documento " & nombre_archivo_documento
            End If
            objCell.Controls.Add(m_TextBoxes(Contador_Control))
            objRow.Cells.Add(objCell)
            '******************************************************************
            'Agregar auto completar
            '******************************************************************
            Result = refclas_radicado.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, pane, "GetGuiaRadicaconasp", nombre_plantilla, "ANEXOS_COR")
            If Result <> "YES" Then
                'Resultado_General = Result

            End If
            '*******************************************************************
            'Agrega label fecha documento
            '*******************************************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("FECHA DOCUMENTO *",
                                                              valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            LabelBox(Contador_Control).Width = 110
            LabelBox(Contador_Control).Font.Name = "Arial"
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '*******************************************************************
            'Agrega textbox fecha documento
            '*******************************************************************
            objCell = New TableCell
            objCell.Width = 100
            ReDim Preserve m_TextBoxes(Contador_Control)
            m_TextBoxes(Contador_Control) = New TextBox
            m_TextBoxes(Contador_Control).ID = "RE_FECHA_DOCUMENTO" & "-" & "FECHA_DOCUMENTO" & "-" & "DATE"
            m_TextBoxes(Contador_Control).Columns = 50
            m_TextBoxes(Contador_Control).Width = 100
            m_TextBoxes(Contador_Control).CssClass = "form-control_  mr-2"
            If m_TextBoxes(Contador_Control).Text = "" Then
                m_TextBoxes(Contador_Control).Text = fecha_documento
            End If

            objCell.Controls.Add(m_TextBoxes(Contador_Control))
            objRow.Cells.Add(objCell)
            '*******************************************************************
            'Agrega boton fecha documento
            '*******************************************************************
            objCell = New TableCell
            objCell.Width = 20
            Result = refclas_radicado.Agregar_Control_Calendario_dinamico(_image,
                                                                          Contador_Control,
                                                                          objCell, objRow,
                                                                          "FECHA_DOCUMENTO",
                                                                          m_TextBoxes(Contador_Control).ID.ToString,
                                                                          pane)
            If Result <> "YES" Then
                'Genera_Interface_Radicacion_Entrante = Result
                'Exit Function
            End If

            '******************************************************************
            'Agregar LABEL cantidad de folios
            '******************************************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("NUMERO FOLIOS*",
                                                             valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '******************************************************************
            'Agregar TEXTBOX cantidad de folios
            '******************************************************************
            objCell = New TableCell
            objCell.Width = 50
            ReDim Preserve m_TextBoxes(Contador_Control)
            m_TextBoxes(Contador_Control) = New TextBox
            m_TextBoxes(Contador_Control).ID = "RE_NUMERO_FOLIOS" & "-" & "NUMERO_FOLIOS" & "-" & "INT"
            m_TextBoxes(Contador_Control).Columns = 50
            m_TextBoxes(Contador_Control).Width = 50
            m_TextBoxes(Contador_Control).CssClass = "form-control"
            If m_TextBoxes(Contador_Control).Text = "" Then
                m_TextBoxes(Contador_Control).Text = numero_folios
            End If
            objCell.Controls.Add(m_TextBoxes(Contador_Control))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            '******************************************************************
            'Agregar auto completar
            '******************************************************************
            Result = refclas_radicado.agregar_auto_complete(m_TextBoxes(Contador_Control).ID,
                                                            pane,
                                                            "GetGuiaRadicaconasp",
                                                            nombre_plantilla,
                                                            "NUMERO_FOLIOS")
            If Result <> "YES" Then
                'Resultado_General = Result

            End If
            '********************************************************************
            'Agrega label campo fecha vencimiento
            '********************************************************************
            objRow = New TableRow

            If Estado_opcion_fecha = 1 Then
                objCell = New TableCell
                objCell.Width = 100
                objCell.HorizontalAlign = HorizontalAlign.Left
                Contador_Control = Contador_Control + 1
                ReDim Preserve LabelBox(Contador_Control)
                LabelBox(Contador_Control) = New Label
                Result = Ref_classtrdDocumental.Formato_sub_serie("FECHA LIMITE RESPUESTA*",
                                                                  valor_documento)
                LabelBox(Contador_Control).Text = valor_documento
                LabelBox(Contador_Control).Font.Size = 9
                LabelBox(Contador_Control).Font.Name = "Arial"
                LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                objCell.Controls.Add(LabelBox(Contador_Control))
                objRow.Cells.Add(objCell)
            Else
                objCell = New TableCell
                objCell.Width = 100
                objRow.Cells.Add(objCell)

            End If
            '********************************************************************
            'Agrega TEXBOX campo fecha vencimiento
            '********************************************************************
            If Estado_opcion_fecha = 1 Then
                objCell = New TableCell
                objCell.Width = 100
                ReDim Preserve m_TextBoxes(Contador_Control)
                m_TextBoxes(Contador_Control) = New TextBox
                m_TextBoxes(Contador_Control).ID = "RE_FECHALIMITERESPUESTA" & "-" & "FECHALIMITERESPUESTA" & "-" & "DATE"
                m_TextBoxes(Contador_Control).Columns = 50
                m_TextBoxes(Contador_Control).Width = 100
                m_TextBoxes(Contador_Control).CssClass = "form-control_ mt-2 mr-2"
                objCell.Controls.Add(m_TextBoxes(Contador_Control))
                'objRow.Cells.Add(objCell)
                '-------------------------------------------------------
                'Agrega boton calendario
                '-------------------------------------------------------
                'objCell = New TableCell
                'objCell.Width = 20
                Result = refclas_radicado.Agregar_Control_Calendario_dinamico(_image,
                                                                              Contador_Control,
                                                                              objCell,
                                                                              objRow,
                                                                              "FECHALIMITERESPUESTA",
                                                                              m_TextBoxes(Contador_Control).ID.ToString, pane)
                If Result <> "YES" Then
                    'Genera_Interface_Radicacion_Entrante = Result
                    'Exit Function
                End If

            Else
                objCell = New TableCell
                objCell.Width = 100
                objRow.Cells.Add(objCell)
            End If
            '********************************************************
            'Agrega el control label campo asunto no es obligatorio 
            '********************************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("ASUNTO *",
                                                             valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '**********************************************************
            'Agregar campo text de asunto
            '*********************************************************
            objCell = New TableCell
            objCell.Width = 350
            ReDim Preserve m_TextBoxes(Contador_Control)
            m_TextBoxes(Contador_Control) = New TextBox
            m_TextBoxes(Contador_Control).ID = "RE_ASUNTO" & "-" & "ASUNTO" & "-" & "VARCHAR"
            m_TextBoxes(Contador_Control).Columns = 50
            m_TextBoxes(Contador_Control).Width = 300
            m_TextBoxes(Contador_Control).CssClass = "form-control"
            objCell.Controls.Add(m_TextBoxes(Contador_Control))
            objRow.Cells.Add(objCell)
            '******************************************************************
            'Agregar auto completar
            '******************************************************************
            Result = refclas_radicado.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, pane, "GetGuiaRadicaconasp", nombre_plantilla, "ASUNTO")
            If Result <> "YES" Then
                'Resultado_General = Result

            End If
            '********************************************************************
            'Agrega label cita radciado
            '********************************************************************
            'objRow = New TableRow
            If Estado_opcion_cita_respuesta = 1 Then
                objCell = New TableCell
                objCell.Width = 100
                objCell.HorizontalAlign = HorizontalAlign.Right
                Contador_Control = Contador_Control + 1
                ReDim Preserve LabelBox(Contador_Control)
                LabelBox(Contador_Control) = New Label
                Result = Ref_classtrdDocumental.Formato_sub_serie("CITA RADICADO",
                                                            valor_documento)
                LabelBox(Contador_Control).Text = valor_documento
                LabelBox(Contador_Control).Font.Size = 9
                LabelBox(Contador_Control).Font.Name = "Arial"
                LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                objCell.Controls.Add(LabelBox(Contador_Control))
                objRow.Cells.Add(objCell)
            Else
                objCell = New TableCell
                objCell.Width = 100
                objRow.Cells.Add(objCell)

            End If
            '********************************************************************
            'Agrega TEXBOX campo cita radicado
            '********************************************************************
            If Estado_opcion_cita_respuesta = 1 Then
                objCell = New TableCell
                objCell.Width = 100
                Contador_Control = Contador_Control + 1
                ReDim Preserve m_TextBoxes(Contador_Control)
                m_TextBoxes(Contador_Control) = New TextBox
                m_TextBoxes(Contador_Control).ID = "RE_CITARADICADO" & "-" & "CITARADICADO" & "-" & "VARCHAR"
                m_TextBoxes(Contador_Control).Columns = 50
                m_TextBoxes(Contador_Control).Width = 100
                m_TextBoxes(Contador_Control).CssClass = "form-control"
                objCell.Controls.Add(m_TextBoxes(Contador_Control))
                objRow.Cells.Add(objCell)
                '******************************************************************
                'Agregar auto completar  
                '******************************************************************
                Result = refclas_radicado.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, pane, "GetGuiaRadicaconasp", nombre_plantilla, "CITARADICADO")
                If Result <> "YES" Then
                    'Resultado_General = Result

                End If
                'objCell = New TableCell
                'objCell.Width = 20
            Else
                objCell = New TableCell
                objCell.Width = 100
                objRow.Cells.Add(objCell)
            End If
            Table.Rows.Add(objRow)
            '------------------------------------------------------------------
            'Agrega el selecctor de flujos de trabajo relacionados al tramite
            '-------------------------------------------------------------------
            objRow = New TableRow
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Left
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("FLUJO TRAMITE",
                                                            valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            objCell = New TableCell
            objCell.Width = 250
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "RE_flujo_trabajo"
            _ComboBox(Contador_Control).Width = 250
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            _ComboBox(Contador_Control).Attributes.Add("onchange", "asignar_actividades_flujo();")
            objCell.Controls.Add(_ComboBox(Contador_Control))
            '******************************************
            'Agrega botn detalle destinatario
            '******************************************
            'objCell = New TableCell
            'objCell.Width = 50
            Contador_Control = Contador_Control + 1
            ReDim Preserve _CommamdBoton(Contador_Control)
            _CommamdBoton(Contador_Control) = New Button
            _CommamdBoton(Contador_Control).ID = "RA_COMMAMD_DETALLE_FLUJO"
            _CommamdBoton(Contador_Control).Text = "Ver.."
            _CommamdBoton(Contador_Control).Font.Size = 8
            _CommamdBoton(Contador_Control).CssClass = "boton_azul"
            _CommamdBoton(Contador_Control).Attributes.Add("OnClick", "asignar_tipo_flujo();")
            '_CommamdBoton(Contador_Control).ForeColor = Drawing.Color.White
            ' _CommamdBoton(Contador_Control).Attributes.Add("background", "#053061")
            _CommamdBoton(Contador_Control).CssClass = "btn btn-primary"
            AddHandler _CommamdBoton(Contador_Control).Click, AddressOf _
            _comman_clik_ver_detalle_flujo
            objCell.Controls.Add(_CommamdBoton(Contador_Control))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            '------------------------------------------------------------------
            'Agrega el selecctor de actividad de asignacion
            '-------------------------------------------------------------------
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            LabelBox(Contador_Control).Text = "Asignación *"
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            objCell = New TableCell
            objCell.Width = 250
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "RE_actividad_asignacion_trabajo_flujo"
            _ComboBox(Contador_Control).Width = 300
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            _ComboBox(Contador_Control).Attributes.Add("onchange", "asignar_usuarios_wf_tramite();")
            '_ComboBox(Contador_Control).EnableViewState = True
            objCell.Controls.Add(_ComboBox(Contador_Control))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            '------------------------------------------------------------------
            'Agrega el selecctor usuario actividad de asignación
            '-------------------------------------------------------------------
            objRow = New TableRow
            objCell = New TableCell
            objCell.Width = 100
            objRow.Cells.Add(objCell)
            objCell = New TableCell
            objCell.Width = 100
            objRow.Cells.Add(objCell)
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            LabelBox(Contador_Control).Text = "Usuarios de grupo"
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            objCell = New TableCell
            objCell.Width = 250
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "RE_asignacion_asignacion_trabajo_flujo"
            _ComboBox(Contador_Control).Width = 350
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            _ComboBox(Contador_Control).Attributes.Add("onchange", "selecion_usuario_wf_grupo();")
            objCell.Controls.Add(_ComboBox(Contador_Control))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            ''**********************************************
            ''Agrega separador caracteristicas destinatario
            ''***********************************************
            objRow = New TableRow
            tableseparacion.Attributes.Add("width", "100%")
            tabledest.Attributes.Add("width", "100%")
            objRow.BackColor = color
            objCell = New TableCell
            objCell.Width = 1200
            objCell.HorizontalAlign = HorizontalAlign.Center
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            'LabelBox(Contador_Control).Text = "CARACTERIZACION DESTINTARIO Y REMITENTE"
            LabelBox(Contador_Control).Font.Size = 10
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))

            '***************************************
            'Agrega label area destinatario
            '***************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("AREA DEPENDENCIA DESTINATARIO",
                                                            valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            LabelBox(Contador_Control).Width = 100
            LabelBox(Contador_Control).Font.Name = "Arial"
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '****************************************
            'Agregar combo area destinatario
            '****************************************
            objCell = New TableCell
            objCell.Width = 300
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "Area_Destinatario_Cor"
            _ComboBox(Contador_Control).Width = 300
            _ComboBox(Contador_Control).Attributes.Add("onchange", "llenardestinatario();")
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            'AddHandler _ComboBox(Contador_Control).SelectedIndexChanged, AddressOf _
            'drow_SelectedIndexChanged()
            objCell.Controls.Add(_ComboBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '******************************************************************
            'Lista las areas de radicacion
            '******************************************************************
            Dim id_empresa As Integer = -1
            Result = refclas_radicado.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_empresa)
            If Result <> "YES" Then
                Resultado_General = Resultado_General & Result
            End If
            Dim id_organigrama As Integer = -1
            Dim ref_clas_empresa As New Class_registro_organigrama
            Result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa,
                                                                            id_organigrama)
            If Result <> "YES" Then
                Resultado_General = Resultado_General & Result
            End If
            Dim ref_class_area_depar As New Class_areas_depart_radicacion
            Result = ref_class_area_depar.Lista_AreasDep_Organigrama_Series(id_organigrama,
                                                                            _ComboBox(Contador_Control))
            If Result <> "YES" Then
                Resultado_General = Resultado_General & Result
            End If

            '***************************************
            'Agrega  destinatario label
            '***************************************
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Right
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            Result = Ref_classtrdDocumental.Formato_sub_serie("DESTINATARIO",
                                                            valor_documento)
            LabelBox(Contador_Control).Text = valor_documento
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            LabelBox(Contador_Control).Width = 100
            LabelBox(Contador_Control).Font.Name = "Arial"
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            '*****************************************
            'Agrega destinatario combobox
            '*****************************************
            objCell = New TableCell
            objCell.Width = 400
            Contador_Control = Contador_Control + 1
            ReDim Preserve _ComboBox(Contador_Control)
            _ComboBox(Contador_Control) = New DropDownList
            _ComboBox(Contador_Control).ID = "Destinatario_Cor"
            _ComboBox(Contador_Control).Width = 400
            _ComboBox(Contador_Control).Attributes.Add("onchange", "seleccionardestinatario_evento();")
            _ComboBox(Contador_Control).CssClass = "custom-select mr-sm-2"
            objCell.Controls.Add(_ComboBox(Contador_Control))
            objRow.Cells.Add(objCell)

            '******************************************
            'Agrega botn detalle destinatario
            '******************************************
            objCell = New TableCell
            objCell.Width = 50
            Contador_Control = Contador_Control + 1
            ReDim Preserve _CommamdBoton(Contador_Control)
            _CommamdBoton(Contador_Control) = New Button
            _CommamdBoton(Contador_Control).ID = "RA_COMMAMD_DETALLE"
            _CommamdBoton(Contador_Control).Text = "Mas.."
            _CommamdBoton(Contador_Control).Font.Size = 8
            _CommamdBoton(Contador_Control).CssClass = "boton_azul"
            _CommamdBoton(Contador_Control).Attributes.Add("OnClick", "activa_ventana_auxiliar_dest_iterno()")
            _CommamdBoton(Contador_Control).CssClass = "btn btn-primary"
            AddHandler _CommamdBoton(Contador_Control).Click, AddressOf _
            _comman_clik_ver_detalle_usuario_interno
            objCell.Controls.Add(_CommamdBoton(Contador_Control))
            objRow.Cells.Add(objCell)
            'tabledest.Rows.Add(objRow)
            '------------------------------------------
            'Agrega campo texto usuarios
            '------------------------------------------
            objRow = New TableRow
            objCell = New TableCell
            'Contador_Control = Contador_Control + 1
            'ReDim Preserve m_TextBoxes(Contador_Control)
            'm_TextBoxes(Contador_Control) = New TextBox
            'm_TextBoxes(Contador_Control).ID = "RE_USUARIO" & "-" & "DESTINATARIO" & "-" & "VARCHAR"
            'm_TextBoxes(Contador_Control).Columns = 50
            'm_TextBoxes(Contador_Control).Attributes.CssStyle.Add("width", "99%")
            'm_TextBoxes(Contador_Control).Attributes.CssStyle.Add("margin-top", "10px")
            'm_TextBoxes(Contador_Control).Attributes.Add("placeholder", "Para relacionar el usuario al que se  quiere radicar, digite el nombre del usuario o el cargo ")
            'm_TextBoxes(Contador_Control).TextMode = TextBoxMode.MultiLine
            'm_TextBoxes(Contador_Control).Rows = 4
            'objCell.Controls.Add(m_TextBoxes(Contador_Control))
            Dim htmlselec As New HtmlSelect
            htmlselec.Attributes.Add("class", "tokenize-callable-demo1")
            htmlselec.Multiple = True
            htmlselec.Style.Add("width", "100%")
            objCell.Controls.Add(htmlselec)
            objCell.ColumnSpan = 5
            'objCell.Attributes.Add("CssClass", "ui-widget")
            objRow.Cells.Add(objCell)
            tabledest.Rows.Add(objRow)
            ''**********************************************
            ''Agrega separador controles dinamicos
            ''***********************************************
            Dim table_separa_controlesdinamicos As New Table
            table_separa_controlesdinamicos.Attributes.Add("width", "100%")
            objRow = New TableRow
            'objRow.Attributes.Add("background-color", "#E7EDF5")
            objRow.BackColor = color
            objCell = New TableCell
            objCell.Width = 1200
            objCell.HorizontalAlign = HorizontalAlign.Center
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            'LabelBox(Contador_Control).Text = "CAMPOS DINAMICOS RADICACION"
            LabelBox(Contador_Control).Font.Size = 11
            LabelBox(Contador_Control).Font.Name = "Arial"
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
            objCell.Controls.Add(LabelBox(Contador_Control))
            objRow.Cells.Add(objCell)
            table_separa_controlesdinamicos.Rows.Add(objRow)
            '******************************************************************
            'Agrega controles dinamicos
            '******************************************************************
            objRow = New TableRow
            objRow.Width = 1200
            Dim Contador_row As Integer = 1
            If Matri_Datos.Length > 1 Then
                For k As Integer = 0 To Matri_Datos.Length - 1
                    If Matri_Datos(k).Estado_Campo = 1 And Matri_Datos(k).campo_sistema <> 1 And Matri_Datos(k).Campo_rad_interno = 1 Then
                        If Contador_row = 4 Then
                            tablecontrolesdinamicos.Controls.Add(objRow)
                            'If k < Matri_Datos.Length - 1 Then
                            objRow = New TableRow
                            objRow.Width = 1140
                            Contador_row = 1
                            'End If
                        End If
                        '--------------------------------------------------------------
                        'Agrega el label
                        '--------------------------------------------------------------
                        objCell = New TableCell
                        objCell.Width = 100
                        objCell.HorizontalAlign = HorizontalAlign.Left
                        objCell.VerticalAlign = VerticalAlign.Top
                        objCell.Wrap = True
                        Contador_Control = Contador_Control + 1
                        ReDim Preserve LabelBox(Contador_Control)
                        LabelBox(Contador_Control) = New Label
                        If Matri_Datos(k).Campo_Obligatorio = 1 Then
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo & "*").Replace("_", " ")
                        Else
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo).Replace("_", " ")
                        End If
                        Result = Ref_classtrdDocumental.Formato_sub_serie(LabelBox(Contador_Control).Text,
                                                                          LabelBox(Contador_Control).Text)
                        LabelBox(Contador_Control).Font.Size = 9
                        LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                        LabelBox(Contador_Control).Width = 100
                        LabelBox(Contador_Control).Font.Name = "Arial"
                        objCell.Controls.Add(LabelBox(Contador_Control))
                        objRow.Cells.Add(objCell)
                        '---------------------------------------------------------------
                        'Agrega campo texbox
                        '---------------------------------------------------------------
                        Dim id_campo_aspnet As String = "RE_" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Tipo_Campo
                        Matri_Datos(k).ID_CAMPO_ASPNET = id_campo_aspnet
                        If Matri_Datos(k).Comportamiento_Campo = "SELECCION" Then
                            objCell = New TableCell
                            objCell.Width = 300
                            objCell.Wrap = False
                            Contador_Control = Contador_Control + 1
                            ReDim Preserve _ComboBox(Contador_Control)
                            _ComboBox(Contador_Control) = New DropDownList
                            _ComboBox(Contador_Control).ID = id_campo_aspnet
                            _ComboBox(Contador_Control).Width = 200
                            objCell.Controls.Add(_ComboBox(Contador_Control))
                            refclas_radicado.Lista_Valores_campo_seleccion_plantilla_radicacion(Codigo_Plantilla, Matri_Datos(k).Campo_Plantilla, _ComboBox(Contador_Control))
                            objRow.Cells.Add(objCell)
                            Contador_row = Contador_row + 1
                        Else
                            objCell = New TableCell
                            objCell.Width = 300
                            objCell.Wrap = False
                            Contador_Control = Contador_Control + 1
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).ID = id_campo_aspnet
                            m_TextBoxes(Contador_Control).Columns = 50
                            m_TextBoxes(Contador_Control).Width = 200
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            If Matri_Datos(k).Tipo_Campo = "DATE" Then
                                m_TextBoxes(Contador_Control).Width = 80
                                '-------------------------------------------------------
                                'Agrega boton calendario
                                '-------------------------------------------------------
                                Result = refclas_radicado.Agregar_Control_Calendario_dinamico(_image, Contador_Control, objCell, objRow, id_campo_aspnet, m_TextBoxes(Contador_Control).ID.ToString, pane)
                                If Result <> "YES" Then
                                    'Resultado_General = Result
                                    'Exit Function
                                End If
                            Else
                                '******************************************************************
                                'Agregar auto completar
                                '******************************************************************
                                Result = refclas_radicado.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, pane, "GetGuiaRadicaconasp", nombre_plantilla, Matri_Datos(k).Campo_Plantilla)
                                If Result <> "YES" Then
                                    'Resultado_General = Result

                                End If

                            End If
                            '******************************************************************
                            'Agrega el boton validacion si el campo tiene plantilla
                            '******************************************************************
                            Dim Tipo_script_da As String = ""
                            Dim id_escript_da As Integer = -1
                            Dim Class_plantilla_validacion As New Class_plantilla_validacion
                            Result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Matri_Datos,
                                                                                              Matri_Datos(k).Campo_Plantilla,
                                                                                              Tipo_script_da,
                                                                                              id_escript_da)
                            If Result <> "YES" Then
                                Resultado_General = Result
                            End If
                            If Tipo_script_da <> "" Then
                                m_TextBoxes(Contador_Control).BackColor = Drawing.Color.Yellow
                                m_TextBoxes(Contador_Control).Attributes.Add("disabled", "true")
                                Result = ""
                                Result = refclas_radicado.Agrega_boton_validacion(objCell, _CommamdBoton, Contador_Control, Tipo_script_da, Matri_Datos(k).Campo_Plantilla, Codigo_Plantilla, -1, id_escript_da)
                                If Result <> "YES" Then
                                    Resultado_General = Result
                                End If
                            End If
                            objRow.Cells.Add(objCell)
                            Contador_row = Contador_row + 1
                        End If

                    End If
                Next
                If Contador_row <> 1 And objRow.Controls.Count > 0 Then
                    tablecontrolesdinamicos.Controls.Add(objRow)

                End If
            End If
            '******************************************************************
            'Agrega las tablas al panel principal
            '******************************************************************
            panetitle.Controls.Add(TableTitle)
            pane.Controls.Add(Tableseparador_documento)
            pane.Controls.Add(Table)
            pane.Controls.Add(tableseparacion)
            'pane.Controls.Add(tabledest)
            pane.Controls.Add(table_separa_controlesdinamicos)
            pane.Controls.Add(tablecontrolesdinamicos)
            refclas_radicado.Seleccion_tipo_tramite_evento_dibuja_iterno(Page1)
            Genera_Interface_Radicacion_Entrante = Resultado_General
        Catch ex As Exception
            Genera_Interface_Radicacion_Entrante = "Inconsistencia general funcion Genera_Interface_Radicacion_Entrante " & ex.Message
        End Try
    End Function
    'ACTIVA MENSAJE DETALLE USUARIO DESTINATARIO INTERNO
    Private Sub _comman_clik_ver_detalle_usuario_interno(ByVal sender As _
                                                         System.Object,
                                                         ByVal e As System.EventArgs)
        Dim clasjava As New Classscrripjava
        Dim pag As Page = sender.page
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePnaelcontrolesradicacion")
        Dim dat_data_grid_auxiliar_lista As Object = sender.page.findcontrol("grid_auxiliar_lista")
        Dim update_auxiliar As UpdatePanel = sender.page.findcontrol("UpdatePanel_auxiliar_destinatarios_internos_popup")
        Dim modal_popup As ModalPopupExtender = sender.page.findcontrol("ModalPopupExtender_auxiliar_destinatarios_internos_popup")
        Try
            Dim Result As String = ""
            Dim refclajava As New Classscrripjava
            Dim Refclas As New ClassRadicador
            Dim droparea As DropDownList = sender.page.findcontrol("Area_Destinatario_Cor")
            If droparea Is Nothing Then
                'refclajava.Showscripman("Imposible encontrar el control Area_Destinatario_Cor ", update)
                clasjava.Showscripman_menu("Imposible encontrar el control Area_Destinatario_Cor ", update, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = Refclas.listar_usuarios_destinatarios_internos_auxiliar(pag, update_auxiliar)
            If Result <> "YES" Then
                'refclajava.Showscripman(Result, update)
                clasjava.Showscripman_menu(Result, update, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else

                'update_auxiliar.Update()
                'modal_popup.Show()
                '
            End If
        Catch ex As Exception
            If Not update Is Nothing Then
                'clasjava.Showscripman(ex.Message, update)
                clasjava.Showscripman_menu(ex.Message, update, "ModalPopupExtender_mensaje_personalizado")

            End If
        End Try
    End Sub
    Private Sub _comman_clik_ver_detalle_flujo(ByVal sender As _
                                               System.Object,
                                               ByVal e As System.EventArgs)
        Dim clasjava As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePnaelcontrolesradicacion")
        Try
            Dim pag As Page = sender.page
            Dim UpdatePaneltraza_grafica As UpdatePanel = sender.page.findcontrol("UpdatePaneltraza_grafica")
            Dim ModalPopupExtendertraza_grafica As ModalPopupExtender = sender.page.findcontrol("ModalPopupExtendertraza_grafica")
            Dim Iframetraza_grafica As Object = sender.page.findcontrol("Iframetraza_grafica_")
            Dim RE_flujo_trabajo As DropDownList = sender.page.findcontrol("RE_flujo_trabajo")
            Dim Hidden_id_flu As Object = sender.page.findcontrol("Hidden_id_flu")
            Dim Result As String = ""
            Dim refclajava As New Classscrripjava
            Dim Refclas As New ClassRadicador
            If Hidden_id_flu.value <> 0 Then
                HttpContext.Current.Session.Item("DR_ID_FLUJO_TRABAJO_SELECCION") = Hidden_id_flu.value
                Iframetraza_grafica.Attributes.Add("SRC", "../workflow/WebFormListaDiagramaFlujo.aspx")
                UpdatePaneltraza_grafica.Update()
                ModalPopupExtendertraza_grafica.Show()
            End If
        Catch ex As Exception
            If Not update Is Nothing Then
                clasjava.Showscripman_menu(ex.Message, update, "ModalPopupExtender_mensaje_personalizado")

            End If
        End Try
    End Sub

    Function Lista_tipos_documentales_de_radicacion_interna(ByRef RefCombo As DropDownList,
                                                            ByVal id_plantilla As Integer) As String
        Try

            RefCombo.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                id_plantilla & " and tipo_activo_pqr <> 1 and estado_tipo_documento=1 and tipo_activo_rad_interno=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_documentales_de_radicacion_interna = " Error Listando tipos documentales   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_tipos_documentales_de_radicacion_interna = "YES"
                Exit Function
            Else
                'RefCombo.Items.Add("SELECCIONE")
                'For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                '    RefCombo.Items.Add(Datset.Tables(0).Rows(i).Item(1).ToString)
                'Next
                Dim ilist As New ListItem
                ilist.Text = "SELECCIONE"
                ilist.Value = 0
                RefCombo.Items.Add(ilist)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1).ToString
                    RefCombo.Items.Add(ilist)
                Next
                Lista_tipos_documentales_de_radicacion_interna = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tipos_documentales_de_radicacion_interna = ex.Message
        End Try
    End Function

    Function Almacena_documento_radicado_externo(ByVal id_usuario_gestion As Integer,
                                                 ByVal nombre_gabinete_workflow As String,
                                                 ByVal id_registro_producion_documental As Integer,
                                                 ByVal radicado As String,
                                                 ByRef id_registro_copia As Integer,
                                                 ByVal nombre_plantilla_radicado As String,
                                                 ByVal id_plantilla_radicado As Integer,
                                                 ByRef id_imagen_almacenada As Integer,
                                                 ByRef archivo_radicado_server As String,
                                                 ByVal genera_achivo_cert_radicado As Integer,
                                                 ByVal alamcena_documento_radicado As Integer,
                                                 ByVal id_tipo_tramite_documento As Integer) As String
        Try
            Dim Result As String = ""
            Dim refclas_expediente As New ClassGaExpediente
            '-------------------------------------------------
            'Solicita datos de caracterizacion registro
            'produción documental
            '-------------------------------------------------
            Dim nombre_archivo As String = "RAD-" & radicado
            Dim nombre_tipo_documental As String = ""
            Dim id_tipo_documento As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete_producion As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion_documental,
                                                                          nombre_archivo,
                                                                          nombre_tipo_documental,
                                                                          id_tipo_documento,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete_producion,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If

            '---------------------------------------------
            'Valida la configuración del gabinete destino
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete_workflow,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Almacena_documento_radicado_externo = "El gabinete " & nombre_gabinete_workflow & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Almacena_documento_radicado_externo = "El gabinete " & nombre_gabinete_workflow & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Almacena_documento_radicado_externo = "El gabinete " & nombre_gabinete_workflow & "  no tiene activa la opción asinar unidad documental"
                Exit Function
            End If

            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna estructura expediente
            '-----------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                        estru_unidad_conservacion)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion
            Dim matri_documentos() As String
            Result = Me.Retorna_parametros_almacenamiento_documento_radicado_interno(id_expediente,
                                                                                     matri_datos_almacen,
                                                                                     matri_gestion,
                                                                                     matri_documentos,
                                                                                     nombre_gabinete_producion,
                                                                                     radicado,
                                                                                     estru_unidad_conservacion,
                                                                                     id_imagen,
                                                                                     nombre_tipo_documental,
                                                                                     id_tipo_documento,
                                                                                     nombre_plantilla_radicado,
                                                                                     id_plantilla_radicado,
                                                                                     nombre_gabinete_workflow)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If

            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(matri_documentos(1))
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Almacena_documento_radicado_externo = Result
                Exit Function
            End If
            Dim ref_matri_documentos() As String
            For i As Integer = 1 To matri_documentos.Length - 1
                ReDim Preserve ref_matri_documentos(i - 1)
                ref_matri_documentos(i - 1) = matri_documentos(i)
            Next
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_registro As Integer = 0
            nombre_archivo = "RAD-" & radicado
            If alamcena_documento_radicado = 1 Then
                Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_workflow, 0, matri_datos_almacen,
                2, ref_matri_documentos.Length, id_tipo_archivo, ref_matri_documentos, 0,
                id_imagen_almacenada, id_tipo_archivo, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
                matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
                matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
                matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
                matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
                matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
                matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro)
                If Result <> "YES" Then
                    Almacena_documento_radicado_externo = Result
                    Exit Function
                End If
                id_registro_copia = id_registro
            End If
            If genera_achivo_cert_radicado = 1 Then
                '-----------------------------------------------
                'Genera archivo radicado id_tipo_tramite_documento
                '-----------------------------------------------
                Dim ref_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
                Dim id_tipo_documento_tramite As Integer = 0
                Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_id_tipologia_lista_chequeo_rotulo_radicado(id_tipo_tramite_documento,
                                                                                                                  id_tipo_documento_tramite)
                If Result <> "YES" Then
                    Almacena_documento_radicado_externo = Result
                    Exit Function
                End If
                Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
                Dim nombre_tipo_documento_rotulo As String = ""
                If id_tipo_documento_tramite <> 0 Then
                    Result = Class_ra_tipo_doc_series.SolicitaNombreTipoDocumentalSerie(id_tipo_documento_tramite,
                                                                                            nombre_tipo_documento_rotulo)
                    If Result <> "YES" Then
                        Almacena_documento_radicado_externo = Result
                        Exit Function
                    End If
                    matri_gestion.TIPODOCUMENTO = nombre_tipo_documento_rotulo
                    matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento_tramite
                End If
                Dim ruta_archivo As String = ""
                Dim archivo_server As String = ""
                Dim Reclas_radicado As New ClassRadicador
                Result = Reclas_radicado.Genera_archivo_detalle_radicado(radicado,
                                                                         nombre_plantilla_radicado,
                                                                         "RADICACION ENTRANTE",
                                                                         ruta_archivo,
                                                                         archivo_server)
                If Result <> "YES" Then
                    Almacena_documento_radicado_externo = Result
                    Exit Function
                End If
                archivo_radicado_server = archivo_server
                Erase ref_matri_documentos
                ReDim Preserve ref_matri_documentos(0)
                ref_matri_documentos(0) = ruta_archivo
                '-----------------------------------------------
                'Solicita el tipo de archivo según extensión
                '-----------------------------------------------
                Dim file_inf_ As New FileInfo(ruta_archivo)
                Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf_.Extension,
                                                                                                            id_tipo_archivo)
                If Result <> "YES" Then
                    Almacena_documento_radicado_externo = Result
                    Exit Function
                End If
                Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_workflow, 0, matri_datos_almacen,
                2, ref_matri_documentos.Length, id_tipo_archivo, ref_matri_documentos, 0, id_imagen_almacenada,
                id_tipo_archivo, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
                matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
                matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
                matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
                matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
                matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro)
                If Result <> "YES" Then
                    Almacena_documento_radicado_externo = Result
                    Exit Function
                End If
            End If

            Almacena_documento_radicado_externo = "YES"
            Exit Function
        Catch ex As Exception
            Almacena_documento_radicado_externo = "Inconsistencia general función Almacena_documento_radicado_externo " & ex.Message
        End Try
    End Function

    Function Retorna_parametros_almacenamiento_documento_radicado_interno(ByVal id_expediente As Integer,
                                                                          ByRef matri_datos_almacen() As String,
                                                                          ByRef matri_gestion As estructure_gestion,
                                                                          ByRef matri_documentos() As String,
                                                                          ByVal nombre_gabinete_producion As String,
                                                                          ByVal radicado As String,
                                                                          ByVal estru_unidad_conservacion() As expediente_conservacion,
                                                                          ByVal id_imagen As String,
                                                                          ByVal nombre_tipo_documento As String,
                                                                          ByVal id_tipo_documento As Integer,
                                                                          ByVal nombre_plantilla_radicado As String,
                                                                          ByVal id_plantilla_radicado As Integer,
                                                                          ByVal nombre_gabinete_almacenamiento As String) As String
        Try
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim Result As String = ""
            Dim Refclasworkflowvisor As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete_producion,
                                                                                     matri_documentos)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            Dim Refclasalmacenamiento As New ClassAlmacenamiento
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete_almacenamiento,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            ''------------------------------------------------------
            ''Solicita tipo tramite radicado
            ''------------------------------------------------------
            Dim refclas_workflow As New ClassWorkflow
            'Dim tipo_tramite_radicado As String = ""
            'Result = refclas_workflow.Solicita_tipo_tramite_radicado(nombre_plantilla_radicado, radicado, tipo_tramite_radicado)
            'If Result <> "YES" Then
            '    Retorna_parametros_almacenamiento_documento_radicado_interno = Result
            '    Exit Function
            'End If


            '-------------------------------------------------------
            'Solicita id gabinete en la tabla de configuración
            '-------------------------------------------------------
            Dim id_gabinete As Integer = 0
            Result = refclas_workflow.Solicita_id_gabinete_configuracion_gabinete(nombre_gabinete_almacenamiento, id_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Solicita relación campos radicado plantilla gabinete
            '-------------------------------------------------------
            Dim stru_campos_plantilla_gabinete() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
            Dim ref_Class_ra_relacion_plantilla_gabinete As New Class_ra_relacion_plantilla_gabinete
            Result = ref_Class_ra_relacion_plantilla_gabinete.SolicitaCamposRelacionPlantillaGabinete(id_plantilla_radicado,
                                                                                                      id_gabinete,
                                                                                                      stru_campos_plantilla_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            '------------------------------------------------------
            'Asigna los datos a la estructura de relación plantilla
            'gabinete
            '-------------------------------------------------------
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = ref_Class_plantillas_radicacion.AsignaDatosCamposPlantillaRadicadoGabinete(stru_campos_plantilla_gabinete,
                                                                                                radicado,
                                                                                                nombre_plantilla_radicado)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Formatea campos tipo date  y date time
            '--------------------------------------------------------
            Dim refclas_ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATE" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                            Exit Function
                        End If
                    End If

                End If
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATETIME" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                            Exit Function
                        End If
                    End If

                End If
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
                    estructura_gabinete(i).VALORCAMPO = radicado
                End If
                If estructura_gabinete(i).CAMPO = "ENLASE" Then
                    estructura_gabinete(i).VALORCAMPO = radicado
                End If
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                For z As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                    If estructura_gabinete(i).CAMPO = stru_campos_plantilla_gabinete(z).nombre_campo_ruta Then
                        estructura_gabinete(i).VALORCAMPO = stru_campos_plantilla_gabinete(z).dato_campo_plantilla

                    End If
                Next
            Next
            '-------------------------------------------
            'Asigna datos gestión
            '-------------------------------------------
            If Not estru_unidad_conservacion Is Nothing Then
                matri_gestion.CLASE_DOCUMENTO = ""
                matri_gestion.EXPEDIENTE = estru_unidad_conservacion(0).CODIGO_UNICO
                matri_gestion.ID_AREA = estru_unidad_conservacion(0).CODIGO_AREA_TRD
                matri_gestion.ID_CLASE_DOCUMENTO = 0
                matri_gestion.ID_EXPEDIENTE = id_expediente
                matri_gestion.ID_SERIE = estru_unidad_conservacion(0).CODIGO_SERIE
                matri_gestion.NOMBRE_SERIE = estru_unidad_conservacion(0).NOMBRE_SERIE
                matri_gestion.ID_SUB_SERIE = estru_unidad_conservacion(0).CODIGO_SUBSERIE
                matri_gestion.NOMBRE_SUB_SERIE = estru_unidad_conservacion(0).NOMBRE_SUBSERIE
                matri_gestion.ID_TIPO_EXPEDIENTE = estru_unidad_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION
                matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
                matri_gestion.ID_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                matri_gestion.UNIDAD_CONSERVACION = ""
                matri_gestion.FECHA_ELABORACION = ""
                matri_gestion.TIPODOCUMENTO = nombre_tipo_documento
            Else
                matri_gestion.CLASE_DOCUMENTO = ""
                matri_gestion.EXPEDIENTE = 0
                matri_gestion.ID_AREA = 0
                matri_gestion.ID_CLASE_DOCUMENTO = 0
                matri_gestion.ID_EXPEDIENTE = id_expediente
                matri_gestion.ID_SERIE = 0
                matri_gestion.NOMBRE_SERIE = ""
                matri_gestion.ID_SUB_SERIE = 0
                matri_gestion.NOMBRE_SUB_SERIE = ""
                matri_gestion.ID_TIPO_EXPEDIENTE = 0
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
                matri_gestion.ID_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                matri_gestion.UNIDAD_CONSERVACION = ""
                matri_gestion.FECHA_ELABORACION = ""
                matri_gestion.TIPODOCUMENTO = nombre_tipo_documento
            End If

            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_radicado_interno = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPO_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.TIPODOCUMENTO
                End If
            Next
            Dim i2 As Integer = 0
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).VISIBLE = 1 Then
                    ReDim Preserve matri_datos_almacen(i2)
                    matri_datos_almacen(i2) = estructura_gabinete(i).VALORCAMPO
                    i2 = i2 + 1
                End If
            Next
            Retorna_parametros_almacenamiento_documento_radicado_interno = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_radicado_interno = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_radicado_interno " & ex.Message
        End Try
    End Function
    Function Envia_documento_workflow_radicado(ByVal radicado As String,
                                               ByRef hiden As Object,
                                               ByRef ref_update As UpdatePanel,
                                               ByRef iframe As Object,
                                               ByRef resultado_correo As String) As String
        '------------------------------------------------------------------------------
        'Función : Envia documento al flujo de trabajo cuando no se envía el radicado
        'interno por algún error 
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-12-14
        '-------------------------------------------------------------------------------
        Try
            '----------------------------------------------
            'Solicita el id del registro de producción
            'documental relacionado al radicado
            '----------------------------------------------
            Dim Result As String = ""
            Dim id_registro_producion_documental As Long = 0
            Dim id_plantilla_radicado As Integer = 0
            Result = Me.Solicita_id_registro_producion_documental_plantilla_radicado(radicado, id_registro_producion_documental, id_plantilla_radicado)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '-------------------------------------------------
            'Solicita el nombre de la plantilla radicado
            '-------------------------------------------------
            Dim Refclas_radicado As New ClassRadicador
            Dim nombre_plantilla_radicado As String = ""
            Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
            Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(id_plantilla_radicado,
                                                                                 nombre_plantilla_radicado)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '--------------------------------------------------
            'Solicita descripción tipo documental radicado
            '--------------------------------------------------
            Dim descripcion_tipo_documental_radicado As String = ""
            Dim Ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Ref_Class_plantillas_radicacion.retorna_tipo_documental_radicado(radicado,
                                                                                      nombre_plantilla_radicado,
                                                                                      descripcion_tipo_documental_radicado)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            Dim nombre_gabinete_radicado As String = ""
            Dim ref_class_workflow As New ClassWorkflow
            '--------------------------------------------------------
            'Solicita nombre gabinete almacenamiento 
            '--------------------------------------------------------
            Result = ref_class_workflow.Solicita_nombre_gabinete_workflow(descripcion_tipo_documental_radicado, id_plantilla_radicado, nombre_gabinete_radicado)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '---------------------------------------------------------
            'Solicita el id del documentos relacionado al radicado
            'en el gabinete de almacenamiento
            '---------------------------------------------------------
            Dim id_imagen_radicado As Integer = 0
            Result = Me.Solicita_id_imagen_documento_radicado(radicado,
                                                              nombre_gabinete_radicado,
                                                              id_imagen_radicado)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '----------------------------------------------------------
            'Solicita el codigo del remitente de la correspondencia
            '----------------------------------------------------------
            Dim id_codigo_remitente As Long = 0
            Result = Me.Solicita_id_remitente_correspondencia(radicado,
                                                              nombre_plantilla_radicado,
                                                              id_codigo_remitente)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '------------------------------------------------------------
            'Retorna usuario workflow relacionado al usuario de gestión
            'relacionado al usuario workflow para determinar la actividad
            'workflow y el usuario workflow
            '------------------------------------------------------------
            Dim id_usuario_workflow As Integer = 0
            Result = Refclas_radicado.Solicita_usuario_workflow_relacionado_usuario_gestion(id_codigo_remitente,
                                                                                            id_usuario_workflow)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            If id_usuario_workflow = 0 Then
                Envia_documento_workflow_radicado = "El usuario de gestión no tiene relacionado el usuario de workflow "
                Exit Function
            End If
            '---------------------------------------------------------
            'Retorna id actividad usuario workflow
            '---------------------------------------------------------
            Dim id_actividad As Integer = 0
            Result = Refclas_radicado.csfc_solicita_id_actividad_workflow(id_usuario_workflow,
                                                                          id_actividad)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            If id_actividad = 0 Then
                Envia_documento_workflow_radicado = "El usuario workflow " & id_usuario_workflow & " no tiene relacionda una actividad de workflow al grupo"
                Exit Function
            End If
            '-----------------------------------------------------------
            'Verifica si el usuario asginado pertenece a un flujo de 
            'trabajo
            '-----------------------------------------------------------
            Dim id_flujo_trabajo As Integer = 0
            Result = Refclas_radicado.Solicita_id_flujo_documento_relacionado(descripcion_tipo_documental_radicado, id_plantilla_radicado, id_flujo_trabajo)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Determina el tipo modulo gestión documento
            '-----------------------------------------------------
            Dim Refclas_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim estado_modulo_respuesta As Integer = 0
            Result = Refclas_tipo_doc_entrante.Determina_gestion_modulo_pqr_Tipo_Tramite(id_plantilla_radicado,
                                                                                         descripcion_tipo_documental_radicado,
                                                                                         estado_modulo_respuesta)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = Result
                Exit Function
            End If
            '---------------------------------------------------------
            'Solicita la actvidad de flujo documental del usuario 
            'workflow seleccionado
            '---------------------------------------------------------
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_flujo_trabajo As Integer = 0
            Dim id_registro_actvidad_flujo_trabajo As Integer = 0
            Dim nombre_flujo_trabajo As String = ""
            Dim Refclas_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            If id_flujo_trabajo <> 0 Then
                Result = Refclas_flujo_trabajo_workflow.Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow(id_usuario_workflow,
                                                                                                            id_flujo_trabajo,
                                                                                                            id_registro_actvidad_flujo_trabajo,
                                                                                                             id_actividad_flujo_trabajo,
                                                                                                             id_usuario_workflow_flujo_trabajo)
                If Result <> "YES" Then
                    Envia_documento_workflow_radicado = Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Solicita la actvidad de flujo documental de la
                'actividad relacionada al usuario workflow
                '---------------------------------------------------------
                If id_registro_actvidad_flujo_trabajo = 0 Then
                    Result = Refclas_flujo_trabajo_workflow.Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow(id_actividad,
                                                                                                                id_flujo_trabajo,
                                                                                                                id_registro_actvidad_flujo_trabajo,
                                                                                                                 id_actividad_flujo_trabajo)
                    If Result <> "YES" Then
                        Envia_documento_workflow_radicado = Result
                        Exit Function
                    End If
                End If
                Result = Refclas_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                                  nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Envia_documento_workflow_radicado = Result
                    Exit Function
                End If
                If id_registro_actvidad_flujo_trabajo = 0 Then
                    Envia_documento_workflow_radicado = "El destinatario del trámite  no pertenece al flujo de trabjo " & nombre_flujo_trabajo &
                        " relacionado al trámite " & descripcion_tipo_documental_radicado & ", imposible radicar el documento. "
                    Exit Function
                End If
            End If
            Dim Refclas_gaproducion As New ClassGaProducionDocumental
            Dim id_registro_producion_documental_radicado As Integer = 0
            Dim id_imagen_almacenada As Integer = 0
            Dim archivo_server As String = ""
            If id_imagen_radicado = 0 Then
                '--------------------------------------------------------
                'Guarda el archivo del radicado en la base de datos
                '--------------------------------------------------------    
                Result = Refclas_gaproducion.Almacena_documento_radicado_externo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                 nombre_gabinete_radicado,
                                                                                 id_registro_producion_documental,
                                                                                 radicado,
                                                                                 id_registro_producion_documental_radicado,
                                                                                 nombre_plantilla_radicado,
                                                                                 id_plantilla_radicado,
                                                                                 id_imagen_almacenada,
                                                                                 archivo_server,
                                                                                 1,
                                                                                 1,
                                                                                 0)
                If Result <> "YES" Then
                    Envia_documento_workflow_radicado = "No se pudo guardar el documento del radicado " & Result
                    Exit Function
                Else
                    id_imagen_radicado = id_imagen_almacenada
                End If
            End If

            '-----------------------------------------------------
            'Activa el flujo de trabjo en la ruta
            '-----------------------------------------------------
            Dim refclas_workflow As New ClassWorkflow
            Dim id_tarea_workflow As Long = 0
            Dim fecha_selecion As Object = "Null"
            Result = ref_class_workflow.Registra_flujo_documento(id_actividad,
                                                                 id_usuario_workflow,
                                                                 id_imagen_radicado,
                                                                 radicado,
                                                                 id_plantilla_radicado,
                                                                 id_flujo_trabajo,
                                                                 id_registro_actvidad_flujo_trabajo,
                                                                 id_usuario_workflow_flujo_trabajo,
                                                                 0,
                                                                 estado_modulo_respuesta,
                                                                 id_tarea_workflow,
                                                                 fecha_selecion, 0)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = "No se pudo enviar el documento al flujo de trabajo Motivo relacionado " & Result
                Exit Function
            End If
            Result = Refclas_radicado.Actualiza_estado_flow_radicado(radicado, nombre_plantilla_radicado, 2)
            If Result <> "YES" Then
                Envia_documento_workflow_radicado = "No se pudo enviar el documento al flujo de trabajo Motivo relacionado " & Result
                Exit Function
            End If
            Result = Me.Envia_corre_notificacion_radicado_envio_documento(radicado, "RADICACION ENTRANTE", nombre_plantilla_radicado, "")
            If Result <> "YES" Then
                resultado_correo = Result
            End If
            hiden.Value = archivo_server
            iframe.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
            ref_update.Update()
            Envia_documento_workflow_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Envia_documento_workflow_radicado = "Inconsistencia general función Envia_documento_workflow_radicado " & ex.Message
        End Try
    End Function

    Function Solicita_id_registro_producion_documental_plantilla_radicado(ByVal radicado As String,
                                                                          ByRef id_registro_producion_documental As Long,
                                                                          ByRef id_plantilla_radicado As Integer) As String
        '------------------------------------------------------
        'Función : Solicita el registro de produción
        'y el codigo de la plantilla de radicacion
        'Fecha : 2017-12-06
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL,ID_PLANTILLA_RADICADO " &
                "from ra_pro_relacion_rad_interno where RADICADO='" & radicado & "'"

            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_registro_producion_documental_plantilla_radicado = "Functión Solicita_id_registro_producion_documental_plantilla_radicado dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_registro_producion_documental_plantilla_radicado = "Impsosible encontrar la relación del radicado " & radicado &
                    " en el registro de relación de produción documental y el radicado interno "
                Exit Function
            Else
                id_registro_producion_documental = Datset.Tables(0).Rows(0).Item(0)
                id_plantilla_radicado = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_registro_producion_documental_plantilla_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_producion_documental_plantilla_radicado = "Inconsistencia general función Solicita_id_registro_producion_documental_plantilla_radicado " & ex.Message
        End Try
    End Function

    Function Solicita_id_imagen_documento_radicado(ByVal radicado As String,
                                                   ByVal nombre_gabinete As String,
                                                   ByRef id_imagen_radicado As Integer) As String
        '------------------------------------------------------------
        'Solicita el nombre de la imagen relacioanada al radicado
        'en el gabinete
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-06-12
        '------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ID " &
                "from " & nombre_gabinete & " where NUMERORADICA='" & radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_imagen_documento_radicado = "Functión Solicita_id_imagen_documento_radicado dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_imagen_radicado = 0
                Solicita_id_imagen_documento_radicado = "YES"
                Exit Function
            Else
                id_imagen_radicado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_imagen_documento_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_imagen_documento_radicado = "Inconistencia general función Solicita_id_imagen_documento_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_id_remitente_correspondencia(ByVal radicado As String,
                                                   ByVal nombre_plantilla_radicado As String,
                                                   ByRef id_remitente As Long) As String
        '--------------------------------------------------
        'Función : Solicita el código  del remitente 
        'del radicado
        'Fecha : 2017-12-06
        'Ing: Miguel Angel Urueta Miranda
        '---------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Destinatario_Externo_id_Dest_Ext " &
                "from " & nombre_plantilla_radicado & " where Consecutivo_Rad='" & radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_remitente_correspondencia = "Functión Solicita_id_remitente_correspondencia dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_remitente = 0
                Solicita_id_remitente_correspondencia = "Imposible encontrar el consecutivo del radicado " & radicado & " en la plantilla de raidicación " & nombre_plantilla_radicado
                Exit Function
            Else
                id_remitente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_remitente_correspondencia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_remitente_correspondencia = "Inconsistencia general función Solicita_id_remitente_correspondencia " & ex.Message
        End Try
    End Function

    Function Envia_corre_notificacion_radicado_envio_documento(ByVal radicado As String,
                                                               ByVal tipo_radicado As String,
                                                               ByVal nombre_plantilla_radicado As String,
                                                               ByVal corroe_electronico_usuario_copia As String) As String
        Try
            Dim reflcas_consulta_radicacion As New ClassRaConsultaRadicados
            Dim struc_envio As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Result As String = ""
            Result = ref_Class_plantillas_radicacion.retorna_datos_radicacion_estructura(tipo_radicado,
                                                                                         radicado,
                                                                                         nombre_plantilla_radicado,
                                                                                         struc_envio)
            If Result <> "YES" Then
                Envia_corre_notificacion_radicado_envio_documento = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Solicita id usuario de gestión documental que debe
            'responder a la petición
            '-----------------------------------------------------
            Dim id_remitente As Long = 0
            Result = Me.Solicita_id_remitente_correspondencia(radicado,
                                                              nombre_plantilla_radicado,
                                                              id_remitente)
            If Result <> "YES" Then
                Envia_corre_notificacion_radicado_envio_documento = Result
                Exit Function
            End If
            Dim Refclas_gestion_respuesta As New Classgestionrespuesta
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Dim correo_electronico As String = ""
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_remitente,
                                                                              correo_electronico)
            If Result <> "YES" Then
                Envia_corre_notificacion_radicado_envio_documento = "YES"
                Exit Function
            End If

            Dim split_notificacion() As String = {"Trámite asignado : " & radicado & " Tipo tramite " & struc_envio.Descripcion_Documento, "Fecha de radicación : " & struc_envio.Fecha_Radicado _
                       , "Fecha límite de respuesta : " & struc_envio.FECHALIMITERESPUESTA, "Remite : " & struc_envio.Remitente_Cor, "Asunto : " & struc_envio.Asunto, "Radicado : " & radicado,
                       "Para tramitar este radicado por favor ingrese al gestor documental (solo para usuarios registrados)"}
            struc_envio.Asunto = "Asignación de tramite " & radicado & " tipo tramite " & struc_envio.Descripcion_Documento & " Fecha vencimiento  " & struc_envio.FECHALIMITERESPUESTA
            Dim refclascorreo As New ClassCorreo
            Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                        correo_electronico,
                                                                        struc_envio.Asunto)
            If Result <> "YES" Then
                Envia_corre_notificacion_radicado_envio_documento = Result
                Exit Function
            End If
            split_notificacion = {"Solicitud radicada bajo el numero : " & radicado & " Tipo tramite " & struc_envio.Descripcion_Documento, "Fecha de radicación : " & struc_envio.Fecha_Radicado _
                       , "Fecha límite de respuesta : " & struc_envio.FECHALIMITERESPUESTA, "Remite : " & struc_envio.Remitente_Cor, "Asunto : " & struc_envio.Asunto, "Radicado : " & radicado,
                       "Para consultar el estrado del radicado ingrese a la opcion consulta radicado"}
            struc_envio.Asunto = "Asignación de tramite " & radicado & " tipo tramite " & struc_envio.Descripcion_Documento & " Fecha vencimiento  " & struc_envio.FECHALIMITERESPUESTA
            If corroe_electronico_usuario_copia <> "" Then
                Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                           corroe_electronico_usuario_copia,
                                                                           struc_envio.Asunto)
                If Result <> "YES" Then
                    Envia_corre_notificacion_radicado_envio_documento = Result
                    Exit Function
                End If
            End If
            Envia_corre_notificacion_radicado_envio_documento = "YES"
        Catch ex As Exception
            Envia_corre_notificacion_radicado_envio_documento = "Inconsistencia general función Envia_corre_notificacion_radicado_envio_documento " & ex.Message
        End Try
    End Function
    Function Solicita_documento_radicado_produccion(ByVal id_documento_produccion As Long,
                                                    ByRef nombre_documento As String,
                                                    ByRef radicado_relacionado As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select SEGUNDO_NOMBRE_DOCUMENTO, radicado_documento " &
                "from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_documento_produccion
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_documento_radicado_produccion = "Functión Solicita_documento_radicado_produccion dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_documento_radicado_produccion = "Imposible emncontrar el radicado del documento de producción documental (" & id_documento_produccion & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre_documento = ""
                    radicado_relacionado = ""
                Else
                    nombre_documento = Datset.Tables(0).Rows(0).Item(0)
                    radicado_relacionado = Datset.Tables(0).Rows(0).Item(1)
                End If
                Solicita_documento_radicado_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_documento_radicado_produccion = "Inconsistencia general función  Solicita_documento_radicado_produccion " & ex.Message
        End Try
    End Function
    Function Solicita_estado_firma_digital(ByVal id_regitro_produccion As Integer,
                                           ByRef estado_firma_digital As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ESTADO_FIRMA_DIGITAL " &
                "from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_regitro_produccion
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_firma_digital = "Functión Solicita_estado_firma_digital dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_firma_digital = 0
                Solicita_estado_firma_digital = "YES"
                Exit Function
            Else
                estado_firma_digital = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_firma_digital = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_firma_digital = "Inconsistencia general fucnion Solicita_estado_firma_digital " & ex.Message
        End Try
    End Function
    Function Solicita_id_registro_producion_documental(ByVal id_imagen As Integer,
                                                       ByVal nombre_gabinete As String,
                                                       ByRef id_registro_producion_documental As Long) As String
        '----------------------------------------------------------------------
        'Función : Solicita el regitro de produción documental de un documento
        'con los parametros de id de la imagen y el nombre del gabinete
        'Fecha : 2017-12-15
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select ID_REGISTRO_PRODUCION_DOCUMENTAL " &
                "from registro_producion_documental where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_imagen &
                " AND NOMBRE_GABINETE='" & nombre_gabinete & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_registro_producion_documental = "Functión Solicita_id_registro_producion_documental dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_producion_documental = 0
                Solicita_id_registro_producion_documental = "YES"
                Exit Function
            Else
                id_registro_producion_documental = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_registro_producion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_producion_documental = "Inconsistencia general función Solicita_id_registro_producion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_relacion_registro_produccion_documental_con_radicado_interno(ByVal id_registro_producion_documental As Long,
                                                                                    ByRef radicados_relacionados As String) As String
        '---------------------------------------------------------
        'Función : Solicita radicado relacionado a un registro
        'de producción documental 
        'Fecha : 2017-12-15
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select RADICADO " &
                "from ra_pro_relacion_rad_interno where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion_documental
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relacion_registro_produccion_documental_con_radicado_interno = "Functión Solicita_relacion_registro_produccion_documental_con_radicado_interno dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                radicados_relacionados = ""
                Solicita_relacion_registro_produccion_documental_con_radicado_interno = "YES"
                Exit Function
            Else
                radicados_relacionados = Datset.Tables(0).Rows(0).Item(0)
                Solicita_relacion_registro_produccion_documental_con_radicado_interno = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_relacion_registro_produccion_documental_con_radicado_interno = "Inconsistencia general función Solicita_relacion_registro_produccion_documental_con_radicado_interno " & ex.Message
        End Try
    End Function
    Function Activa_digitalizacion_documentos(ByVal id_expediente As Long,
                                              ByRef page As Page) As String
        Try
            Dim Result As String = ""
            Dim Refclas_expediente As New ClassGaExpediente
            '----------------------------------------------------
            'Retorna datos estructura expediente 
            '----------------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = Refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                         estru_unidad_conservacion)
            If Result <> "YES" Then
                Activa_digitalizacion_documentos = Result
                Exit Function
            End If
            Dim Ref_class_tipo_doc_series As New Class_ra_tipo_doc_series
            If estru_unidad_conservacion(0).CODIGO_SERIE <> 0 And estru_unidad_conservacion(0).CODIGO_SUBSERIE = 0 Then
                '-----------------------------------------------------------------
                'Solicita tipos documentales relacionados a la serie documental
                '-----------------------------------------------------------------
                Result = Ref_class_tipo_doc_series.Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred(estru_unidad_conservacion(0).CODIGO_SERIE,
                                                                                                                 page)
                If Result <> "YES" Then
                    Activa_digitalizacion_documentos = Result
                    Exit Function
                End If
            End If
            If estru_unidad_conservacion(0).CODIGO_SUBSERIE <> 0 Then
                '---------------------------------------------------------------------
                'Solicita tipos documentales relacionados a la sub serie documental
                '---------------------------------------------------------------------
                Result = Ref_class_tipo_doc_series.Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred(estru_unidad_conservacion(0).CODIGO_SUBSERIE,
                                                                                                                     page)
                If Result <> "YES" Then
                    Activa_digitalizacion_documentos = Result
                    Exit Function
                End If
            End If
            Activa_digitalizacion_documentos = "YES"
        Catch ex As Exception
            Activa_digitalizacion_documentos = "Inconsistencia general función Activa_digitalizacion_documentos " & ex.Message
        End Try
    End Function
    Function Activa_guardar_documento_digitalizado(ByVal id_expediente As Long,
                                                   ByVal id_documento As Integer,
                                                   ByRef estrucutura_Value As String) As String
        Try
            estrucutura_Value = ""
            Dim Result As String = ""
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Activa_guardar_documento_digitalizado = Result
                Exit Function
            End If
            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If id_documento = -1 Then
                    Activa_guardar_documento_digitalizado = "Debe seleccionar un tipo de documento para guardar el archivo digitalizado"
                    Exit Function
                End If
            End If
            Dim Refclas_expediente As New ClassGaExpediente
            '----------------------------------------------------
            'Retorna datos estructura expediente 
            '----------------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = Refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
            estru_unidad_conservacion)
            If Result <> "YES" Then
                Activa_guardar_documento_digitalizado = Result
                Exit Function
            End If
            Dim Ref_class_serie As New Class_ra_tipo_doc_series
            Dim nombre_tipo_documento As String = ""
            If estru_unidad_conservacion(0).CODIGO_SERIE <> 0 And estru_unidad_conservacion(0).CODIGO_SUBSERIE = 0 And id_documento <> -1 Then
                Result = Ref_class_serie.SolicitaNombreTipoDocumentalSerie(id_documento,
                                                                               nombre_tipo_documento)
                If Result <> "YES" Then
                    Activa_guardar_documento_digitalizado = Result
                    Exit Function
                Else
                    estrucutura_Value = id_documento & "|" & estru_unidad_conservacion(0).CODIGO_SERIE & "|0|SERIE|" & nombre_tipo_documento
                    Activa_guardar_documento_digitalizado = "YES"
                    Exit Function
                End If
            End If

            If estru_unidad_conservacion(0).CODIGO_SUBSERIE <> 0 And id_documento <> -1 Then
                Result = Ref_class_serie.Solicita_nombre_tipo_documental_sub_serie(id_documento,
                                                                                   nombre_tipo_documento)
                If Result <> "YES" Then
                    Activa_guardar_documento_digitalizado = Result
                    Exit Function
                Else
                    estrucutura_Value = id_documento & "|" & estru_unidad_conservacion(0).CODIGO_SERIE & "|" & estru_unidad_conservacion(0).CODIGO_SUBSERIE & "|SUBSERIE|" & nombre_tipo_documento
                    Activa_guardar_documento_digitalizado = "YES"
                    Exit Function
                End If
            End If
            Activa_guardar_documento_digitalizado = "YES"
        Catch ex As Exception
            Activa_guardar_documento_digitalizado = "Inconsistencia general función Activa_guardar_documento_digitalizado " & ex.Message
        End Try
    End Function
    Function Guarda_documento_digitalizado_producion(ByVal id_expediente As Integer,
                                                     ByVal id_usuario_gestion As Integer,
                                                     ByRef nombre_archivo As String,
                                                     ByVal estrucura_value As String,
                                                     ByRef id_registro_copia As Integer,
                                                     ByRef ref_gridview As GridView,
                                                     ByRef ref_update As UpdatePanel,
                                                     ByRef campos_valores As String) As String
        Try
            Dim Result As String = ""
            Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
            Dim Selection As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
            Dim spl() As String = Selection.Split("|")
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            '--------------------------------------------------------
            'Retorna los documentos almacenados en el file system
            '--------------------------------------------------------
            Result = RefclasDigitaliza.SolicitaMatrizDocumentosDigitalizados(Val(spl(0)),
                                                                             HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                             Matri_Documentos_Final)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = "Imposible encontrar documentos para almacenar " & Result
                Exit Function
            End If
            Dim archivo_copia As String = ""
            archivo_copia = Matri_Documentos_Final(0)
            If archivo_copia = "" Then
                Guarda_documento_digitalizado_producion = "Debe informar el archivo que desea guardar"
                Exit Function
            End If
            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(archivo_copia)
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim icono As String = ""
            ClassDaGabinete.Agrega_icono_image_fownt_java(id_tipo_archivo.ToString,
                                                          icono)
            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna estructura expediente
            '-----------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Dim id_tipo_documetal As Integer = 0
            Dim Split() As String = estrucura_value.Split("|")
            Dim nombre_tipo_documento As String = ""
            If estrucura_value <> "" Then
                id_tipo_documetal = Split(0)
                nombre_tipo_documento = Split(4)
            End If
            Dim refclas_expediente As New ClassGaExpediente
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                        estru_unidad_conservacion)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim matri_documentos() As String = Nothing
            Dim date1al As String = Date.Today
            Dim ref_clas_almacenamiento As New ClassAlmacenamiento
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If

            If nombre_archivo = "" Then
                nombre_archivo = file_inf.Name.ToString.Replace("136_doc_adjunto_", "")
                nombre_archivo = nombre_archivo.ToString.Replace("/", "-")
            Else
                nombre_archivo = nombre_archivo & file_inf.Extension
            End If
            Dim nombre_gabinete As String = "PRODUCIONDOC"
            Result = refclas_expediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                               nombre_gabinete)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            Result = Me.Retorna_parametros_almacenamiento_documento_adjunto(id_expediente,
                                                                            matri_datos_almacen,
                                                                            matri_gestion,
                                                                            matri_documentos,
                                                                            nombre_gabinete,
                                                                            nombre_archivo,
                                                                            estru_unidad_conservacion,
                                                                            archivo_copia,
                                                                            nombre_tipo_documento,
                                                                            id_tipo_documetal,
                                                                            0,
                                                                            0,
                                                                            0)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_imagen As Integer = 0
            Dim radicado As String = ""
            Dim id_registro As Integer = 0
            Dim estado_frima_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
            2, Matri_Documentos_Final.Length, id_tipo_archivo, Matri_Documentos_Final, 0,
            id_imagen, id_tipo_archivo, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro, 1, estado_frima_digital)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_producion = Result
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(date1al,
                                                                  fecha_tempo)
            fecha_tempo = Left(fecha_tempo, 10)
            campos_valores = "|" & id_registro.ToString & "|" & nombre_archivo.Replace("|", "") & "|" & fecha_tempo & "|" _
              & nombre_tipo_documento & "|" & nombre_gabinete & "|" & estru_unidad_conservacion(0).CODIGO_UNICO &
                "|" & estado_frima_digital & "|" & icono & "|" & id_imagen
            id_registro_copia = id_registro
            Guarda_documento_digitalizado_producion = "YES"
        Catch ex As Exception
            Guarda_documento_digitalizado_producion = "Incosistencia general función Guarda_documento_digitalizado_producion " & ex.Message
        End Try
    End Function

    Function Activa_agregar_expediente_producion(ByVal id_nivel As Integer,
                                                 ByVal id_usuario_gestion As Integer,
                                                 ByRef drop_lis As DropDownList,
                                                 ByRef drop_list_subseries As DropDownList,
                                                 ByRef droplist_fondo As DropDownList,
                                                 ByRef droplist_gabinete As DropDownList,
                                                 ByRef DropDownList_instrumento As DropDownList,
                                                 ByRef ref_update As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_area_departamento As Integer = 0
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            drop_lis.Items.Clear()
            drop_list_subseries.Items.Clear()
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Activa_agregar_expediente_producion = Result
                    Exit Function
                End If
                If stru_permisos_niveles.agregar_expediente = 0 Then
                    Activa_agregar_expediente_producion = "El usuario no tiene persmisos para agregar un nuevo expediente en el nivel seleccionado"
                    Exit Function
                End If
                'Asigna el usuario propietario del nivel para gestionar las series y subseries segun el area del propietario
                Dim ref_Class_niveles_organizacion As New Class_niveles_organizacion
                Result = ref_Class_niveles_organizacion.Solicita_propietario_nivel_expedientes(id_nivel,
                                                                                              id_usuario_gestion)
                If Result <> "YES" Then
                    Activa_agregar_expediente_producion = Result
                    Exit Function
                End If

            End If
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion,
                                                                                           id_area_departamento)
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            If id_area_departamento = 0 Then
                Activa_agregar_expediente_producion = "El usuario de gestión no esta relacionado a un área o departamento"
                Exit Function
            End If

            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            If id_empresa = 0 Then
                Activa_agregar_expediente_producion = "El usuario de gestión no está relacionado a una empresa de gestión"
                Exit Function
            End If
            Dim Refclas_instrumento As New ClassGaGestionInstrumento
            Dim stru As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru)
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            Dim Refclas_organigrama As New ClassGaOrganigrama
            Dim id_organigrama As Integer = 0
            Dim id_instrumento As Integer = 0
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                Result = Refclas_organigrama.Retorna_Id_Organigrama_activo_empresa(id_empresa,
                                                                                   id_organigrama,
                                                                                   0)
                If Result <> "YES" Then
                    Activa_agregar_expediente_producion = Result
                    Exit Function
                End If
                Result = Refclas_instrumento.Lista_instrumentos_producion_documental(DropDownList_instrumento,
                                                                                     ref_update,
                                                                                     id_instrumento)
                If Result <> "YES" Then
                    Activa_agregar_expediente_producion = Result
                    Exit Function
                End If
                If DropDownList_instrumento.Items.Count > 0 Then
                    id_instrumento = DropDownList_instrumento.SelectedValue
                End If
            Else

                Result = Refclas_instrumento.Lista_instrumentos_producion_documental_manager(DropDownList_instrumento,
                                                                                             ref_update,
                                                                                             id_instrumento)
                If Result <> "YES" Then
                    Activa_agregar_expediente_producion = Result
                    Exit Function
                End If
                If DropDownList_instrumento.Items.Count > 0 Then
                    id_instrumento = DropDownList_instrumento.SelectedValue
                End If
                If DropDownList_instrumento.Items.Count > 0 Then
                    Result = Refclas_instrumento.Solicita_id_organigrama_instrumento(DropDownList_instrumento.SelectedValue,
                                                                                     id_organigrama)

                    If Result <> "YES" Then
                        Activa_agregar_expediente_producion = Result
                        Exit Function
                    End If
                End If

            End If
            '-------------------------------------------
            'Lista series con el organigrama activo
            '-------------------------------------------
            Dim Ref_class_series As New Class_series_documentales
            If id_organigrama <> 0 Then
                If id_instrumento <> 0 Then
                    '-------------------------------------------
                    'Lista series documentales relaciondas al 
                    'instrumento
                    '-------------------------------------------
                    If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                        Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento,
                                                                                                id_instrumento,
                                                                                                drop_lis,
                                                                                                ref_update)
                        If Result <> "YES" Then
                            Activa_agregar_expediente_producion = Result
                            Exit Function
                        Else
                            drop_list_subseries.Items.Clear()
                        End If
                    Else
                        Result = Ref_class_series.Lista_series_relacionadas_a_instrumento(id_instrumento,
                                                                                          drop_lis,
                                                                                          ref_update)
                        If Result <> "YES" Then
                            Activa_agregar_expediente_producion = Result
                            Exit Function
                        Else
                            drop_list_subseries.Items.Clear()
                        End If
                    End If
                Else
                    If stru.ACTIVA_OBLIGA_TRD = 1 Then
                        Activa_agregar_expediente_producion = "Debe activar el instrumento archivístico"
                        Exit Function
                    End If
                End If
            Else
                If stru.ACTIVA_OBLIGA_TRD = 1 Then
                    Activa_agregar_expediente_producion = "Debe activar el organigrama predeterminado"
                    Exit Function
                End If
            End If
            Dim Refclas_exp As New ClassGaExpediente
            Result = Refclas_exp.Listar_fodos_documentales(droplist_fondo, "", "")
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            droplist_gabinete.Items.Clear()
            droplist_gabinete.Items.Add("PRODUCIONDOC")
            Dim ref_drop As New DropDownList
            Dim Ref_class_gabinete As New ClassDaGabinete
            Result = Ref_class_gabinete.Retorna_gabinetes_permitidos_almacenamiento(HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                                    HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                    ref_drop)
            If Result <> "YES" Then
                Activa_agregar_expediente_producion = Result
                Exit Function
            End If
            If ref_drop.Items.Count > 0 Then
                For i As Integer = 0 To ref_drop.Items.Count - 1
                    If ref_drop.Items(i).Text <> "PRODUCIONDOC" Then
                        droplist_gabinete.Items.Add(ref_drop.Items(i).Text)
                    End If
                Next
            End If
            Activa_agregar_expediente_producion = "YES"
        Catch ex As Exception
            Activa_agregar_expediente_producion = "Inconsistencia general función Activa_agregar_expediente_producion " & ex.Message
        End Try
    End Function

    Function Agregar_expediente_produccion_carpeta_a_la_estructura(ByVal id_usuario_gestion As Integer,
                                                                   ByVal nombre_expediente As String,
                                                                   ByVal nombre_serie As String,
                                                                   ByRef nombre_sub_serie As String,
                                                                   ByRef drow_list_fondo As DropDownList,
                                                                   ByRef ref_treview_node As TreeNode,
                                                                   ByRef ref_update_panel_treview_expediente As UpdatePanel,
                                                                   ByVal id_expediente_carpeta_padre As Integer,
                                                                   ByVal id_serie As Integer,
                                                                   ByVal id_sub_serie As Integer,
                                                                   ByVal nombre_gabinete As String,
                                                                   ByVal id_nivel_padre As Integer,
                                                                   ByVal id_instrumento_archivistico As Integer,
                                                                   ByRef Hidden_rest_expe_tit_0009 As Object,
                                                                   ByRef Hidden_rest_ur_expe_0010 As Object,
                                                                   ByVal nombre_solicitante As String,
                                                                   ByVal identificacion_solicitante As String,
                                                                   ByVal asunto As String,
                                                                   ByVal tema As String,
                                                                   ByVal observacion As String) As String
        Try
            Dim Result As String = ""
            Dim Refclastrd As New ClassTrdDocumental
            Dim nombre_area As String = ""
            Dim nombre_organigrama As String = ""
            Dim id_empresa_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDEMPRESA")
            Dim id_usuario_gestion_ As Integer = id_usuario_gestion
            '----------------------------------------------------
            'Solicita opciones de aplicacion tablas de retención
            '----------------------------------------------------
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            Dim Ref_class_pro_nivel As New Class_ra_pro_niveles
            Dim Estado_propietario As String = ""
            Result = Ref_class_pro_nivel.Solicita_estado_nivel_propietario(id_usuario_gestion_,
                                                                           id_nivel_padre,
                                                                           Estado_propietario)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            Dim Refclas_ra_pro_niveles As New Class_ra_pro_niveles
            Dim Nombre As String = ""
            Dim Cargo As String = ""
            If Estado_propietario = "NO" Then
                Result = Refclas_ra_pro_niveles.Solicita_usuario_propietario_nivel(id_nivel_padre,
                                                                                   Nombre,
                                                                                   Cargo,
                                                                                   id_usuario_gestion_)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
            End If
            '---------------------------------------------------
            'Retorn tipo de expediente electrónico 
            '---------------------------------------------------
            Dim id_tipo_expediente_carpeta As Integer = 0
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido(id_tipo_expediente_carpeta,
                                                                                                                    0)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            If id_tipo_expediente_carpeta = 0 And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = "Imposible econtrar el tipo de expediente electrónico"
                Exit Function
            End If
            '-------------------------------------
            'Verfica existencia nombre expediente  
            '-------------------------------------
            If nombre_expediente = "" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = "Debe informar el nombre del expediente o carpeta "
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Verifica existencia clasificación y aplicación tablas de retención
            '-------------------------------------------------------------------
            If nombre_serie = "" And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = "Debe seleccionar el nombre de la serie o sub serie a la que pertenece la carpeta o expediente "
                Exit Function
            End If
            Dim Ref_serie As New Class_series_documentales
            Dim RefclassGestionInstrumento As New ClassGaGestionInstrumento
            Dim Id_tipo_instrumento As Integer = 0
            Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento_archivistico,
                                                                               Id_tipo_instrumento)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Verfica tipos documentales relacionados a al serie o  sub serie
            '-------------------------------------------------------------------
            Dim numero_tipos_relacionados As Integer = 0
            If nombre_sub_serie <> "" Then
                Result = Me.Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie(id_sub_serie,
                                                                                              numero_tipos_relacionados)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
                '--------------------------------------------------------
                'Numero tipos documentales relacionados  a la sub serie
                '--------------------------------------------------------
                If numero_tipos_relacionados = 0 And Id_tipo_instrumento = 1 Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = "La sub serie ( " & id_sub_serie & " ) no tiene tipos documentales relacionados, imposible crear la carpeta expediente"
                    Exit Function
                End If
            Else

                '---------------------------------------------------
                'Verifica tipos documentales relacionados a la serie
                '---------------------------------------------------
                numero_tipos_relacionados = 0
                If nombre_serie <> "" Then
                    Result = Me.Solicita_numero_tipos_de_documentos_relacionados_con_la_serie(id_serie,
                                                                                              numero_tipos_relacionados)
                    If Result <> "YES" Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                        Exit Function
                    End If
                    '--------------------------------------------------
                    'Numero tipos documentales relacionados  a la serie
                    '--------------------------------------------------
                    If numero_tipos_relacionados = 0 And Id_tipo_instrumento = 1 Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = "La serie ( " & id_serie & " ) no tiene tipos documentales relacionados, imposible crear la carpeta expediente"
                        Exit Function
                    End If
                End If
            End If
            '----------------------------------------------
            'Verifica existencia selección fondo documental 
            '----------------------------------------------
            If drow_list_fondo.Text = "" And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = "Debe seleccionar el fondo documenta al que pertenecera el expediente o carpeta "
                Exit Function
            End If

            '---------------------------------------------------------------
            'Solicita organigrama relacionado a  instrumento archivístico
            '---------------------------------------------------------------
            Dim id_organigrama As Integer = 0
            If id_instrumento_archivistico <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_id_organigrama_instrumento(id_instrumento_archivistico,
                                                                                        id_organigrama)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
                '----------------------------------------------------------------
                'Existencia organigrama relacionado al instrumento archivístico
                '----------------------------------------------------------------
                If id_organigrama = 0 Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = "El instrumento archivístico ( " & id_instrumento_archivistico & " )  no esta relaciona a un organigrama"
                    Exit Function
                End If
            End If
            '--------------------------------------------------------------
            'Solicita nombre area relacionada a la producción documental
            '--------------------------------------------------------------
            Dim id_area_departamento As Integer = 0
            If id_serie <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_id_area_serie_documental(id_serie,
                                                                                      id_area_departamento)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
                If id_area_departamento = 0 Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = "La serie ( " & id_serie & " ) no esta relacionda a un área o departamento "
                    Exit Function
                End If
                '------------------------------
                'Solicita nombre area
                '------------------------------
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If id_area_departamento <> 0 Then
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area_departamento,
                                                                                               nombre_area)
                    If Result <> "YES" Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                        Exit Function
                    End If
                    If nombre_area = "" Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = "Imposible encontrar el nombre del área con la identificación (" & id_area_departamento & ")"
                        Exit Function
                    End If
                End If
            Else
                '-------------------------------------------------------------------
                'Solicita nombre serie por el área relacionada al usuario de gestión
                '-------------------------------------------------------------------
                Dim id_area_usuario_gestion As Integer = 0
                Dim Class_remit_dest_interno As New Class_remit_dest_interno
                Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion_,
                                                                                               id_area_usuario_gestion)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If id_area_usuario_gestion <> 0 Then
                    '------------------------------------------------------
                    'Solicita el nombre del área del usuario de gestión  
                    '------------------------------------------------------
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area_usuario_gestion,
                                                                                               nombre_area)
                    If Result <> "YES" Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                        Exit Function
                    End If
                Else
                    Agregar_expediente_produccion_carpeta_a_la_estructura = "El usuario de gestión no  se encuentra relacionado al área "
                    Exit Function
                End If
            End If
            '---------------------------
            'Solicita nombre organigrama
            '---------------------------      
            If id_organigrama <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_nombre_organigrama_por_identidad_organigrama(id_organigrama,
                                                                                                          nombre_organigrama)
                If Result <> "YES" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                    Exit Function
                End If
                '----------------------------------
                'Existencia nombre organigrama
                '----------------------------------
                If nombre_organigrama = "" Then
                    Agregar_expediente_produccion_carpeta_a_la_estructura = "Imposible encontrar el nombre del organigrama relacionado al organigrama (" & id_organigrama & " )"
                    Exit Function
                End If

            Else
                Dim Ref_class_registro_organigrama As New Class_registro_organigrama
                If id_organigrama = 0 Then
                    Result = Ref_class_registro_organigrama.Solicita_datos_caracterizacion_organigrama_activo(id_empresa_usuario_gestion,
                                                                                                              id_organigrama,
                                                                                                              nombre_organigrama)
                    If Result <> "YES" Then
                        Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                        Exit Function
                    End If
                End If
            End If
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If

            Dim nombre_usuario_expediente As String = ""
            Dim identificacion_usuario_expediente As String = ""
            Dim Refclas_remit_dest As New Class_remit_dest_interno
            Result = Refclas_remit_dest.Retorna_identificacion_destinatario_interno(id_usuario_gestion_,
                                                                                    identificacion_usuario_expediente)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            Result = Refclas_remit_dest.Retorna_nombre_cargo_destinatario_interno(id_usuario_gestion_,
                                                                                  nombre_usuario_expediente,
                                                                                  "")
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            End If
            Dim Refclas As New ClassGaExpediente
            Dim estado_codigo_unico As Integer = 1
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Dim id_tipo_expediente As Integer = id_tipo_expediente_carpeta
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_expediente As Integer = 0
            Dim id_registro_relacion As Integer = 0
            Result = Refclas.Registrar_Expediente_Conservacion(id_usuario_gestion_,
                                                               nombre_expediente,
                                                               estado_codigo_unico,
                                                               id_empresa_usuario_gestion,
                                                               date1al,
                                                               "",
                                                               "",
                                                               "",
                                                               tema,
                                                               nombre_organigrama,
                                                               nombre_area,
                                                               nombre_serie,
                                                               nombre_sub_serie,
                                                               id_tipo_expediente,
                                                               "0",
                                                               "0",
                                                               "0",
                                                                asunto,
                                                               1,
                                                               "",
                                                               id_expediente,
                                                               observacion,
                                                               "COMPUESTA(EXPEDIENTE)",
                                                               "",
                                                               option_obliga_archivo_unidad,
                                                               "0",
                                                               "0",
                                                               requiere_unida_conservacion_fisica,
                                                               "Archivo Gestión",
                                                               drow_list_fondo.Text,
                                                               nombre_solicitante,
                                                               identificacion_solicitante,
                                                               nombre_usuario_expediente,
                                                               identificacion_usuario_expediente,
                                                               nombre_expediente,
                                                               id_expediente_carpeta_padre,
                                                               2,
                                                               id_instrumento_archivistico,
                                                               nombre_gabinete,
                                                               id_nivel_padre,
                                                               id_registro_relacion, 0)
            If Result <> "YES" Then
                Agregar_expediente_produccion_carpeta_a_la_estructura = Result
                Exit Function
            Else
                Dim title As String = ""
                If nombre_serie <> "" Then
                    title = " (" & nombre_serie & ")"
                End If
                If nombre_sub_serie <> "" Then
                    title = " (" & nombre_sub_serie & ")"
                End If
                Hidden_rest_ur_expe_0010.value = "../Gestion/imagenes/folder-regular.png"
                Hidden_rest_expe_tit_0009.value = id_registro_relacion & "|" & id_nivel_padre & "|" & id_expediente & "|" & title
                Agregar_expediente_produccion_carpeta_a_la_estructura = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agregar_expediente_produccion_carpeta_a_la_estructura = "Inconsistencia general fución Agregar_expediente_produccion_carpeta_a_la_estructura " & ex.Message
        End Try
    End Function
    Function Activa_editar_expediente_produccion(ByVal id_nivel As Integer,
                                                 ByVal id_expediente As Integer,
                                                 ByVal id_usuario_gestion As Integer,
                                                 ByRef text_box_nombre_expediente As TextBox,
                                                 ByRef TextBox_nombre_persona_expediente_actualizar As TextBox,
                                                 ByRef TextBox_identificacion_persona_expediente_actualizar As TextBox,
                                                 ByRef TextBox_asunto_expediente_actualizar As TextBox,
                                                 ByRef TextBox_tema_expediente_actualizar As TextBox,
                                                 ByRef TextBox_observacion_expediente_actualizar As TextBox,
                                                 ByRef drowlis_nombre_serie As DropDownList,
                                                 ByRef drowlis_nombre_sub_serie As DropDownList,
                                                 ByRef drow_list_fondo As DropDownList,
                                                 ByRef ref_update_expediente As UpdatePanel,
                                                 ByRef droplist_gabinete As DropDownList,
                                                 ByRef DropDownList_instrumento_edita As DropDownList,
                                                 ByRef result_ref As String,
                                                 ByRef boton As Button) As String
        Try
            result_ref = "YES"
            drowlis_nombre_serie.Enabled = True
            drowlis_nombre_sub_serie.Enabled = True
            drow_list_fondo.Enabled = True
            DropDownList_instrumento_edita.Enabled = True
            text_box_nombre_expediente.Enabled = True
            boton.Enabled = True
            Dim Result As String = ""
            Dim id_instrumento As Integer = 0
            DropDownList_instrumento_edita.Items.Clear()
            Dim Ref_GAEexpediente As New ClassGaExpediente
            Dim refclastrdDocumental As New ClassTrdDocumental
            Dim Refclas_gestion_Instrumento As New ClassGaGestionInstrumento
            Dim refclas_radicado As New ClassRadicador
            Dim stru_expediente() As expediente_conservacion = Nothing
            Result = Ref_GAEexpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                       stru_expediente)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = "Función Activa_editar_expediente_produccion dice " & Result
                Exit Function
            End If
            id_instrumento = stru_expediente(0).id_instrumento
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Activa_editar_expediente_produccion = Result
                    Exit Function
                End If
                If stru_permisos_niveles.editar_expediente = 0 Then
                    Activa_editar_expediente_produccion = "El usuario no tiene persmisos para editar expediente en el nivel anidado"
                    Exit Function
                End If
                'Asigna el usuario propietario del nivel para gestionar las series y subseries segun el area del propietario
                Dim ref_Class_niveles_organizacion As New Class_niveles_organizacion
                Result = ref_Class_niveles_organizacion.Solicita_propietario_nivel_expedientes(id_nivel,
                                                                                              id_usuario_gestion)
                If Result <> "YES" Then
                    Activa_editar_expediente_produccion = Result
                    Exit Function
                End If
            End If
            Dim id_tipo_instrumento As Integer = 0
            Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
            Result = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento,
                                                                                             id_tipo_instrumento)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                'Result = Refclas_gestion_Instrumento.Lista_instrumentos_producion_documental_manager(DropDownList_instrumento_edita, _
                '                                                                                     ref_update_expediente, _
                '                                                                                     stru_expediente(0).id_instrumento)
                'If Result <> "YES" Then
                '    Activa_editar_expediente_produccion = Result
                '    Exit Function
                'End If
                'Result = Refclas_organigrama.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                '                                                                   id_organigrama, _
                '                                                                   0)
                'If Result <> "YES" Then
                '    Activa_agregar_expediente_producion = Result
                '    Exit Function
                'End If

                'Result = Refclas_instrumento.Lista_instrumentos_producion_documental(DropDownList_instrumento, _
                '                                                                     ref_update, _
                '                                                                     id_instrumento)
                'If Result <> "YES" Then
                '    Activa_agregar_expediente_producion = Result
                '    Exit Function
                'End If
                If stru_expediente(0).id_instrumento <> 0 Then
                    Result = Refclas_gestion_Instrumento.Lista_instrumentos_producion_documental_edita(DropDownList_instrumento_edita,
                                                                                                     ref_update_expediente,
                                                                                                     stru_expediente(0).id_instrumento)
                    If Result <> "YES" Then
                        Activa_editar_expediente_produccion = Result
                        Exit Function
                    End If
                    'Result = Refclas_gestion_Instrumento.Lista_instrumentos_producion_documental_manager(DropDownList_instrumento_edita, _
                    '                                                                                     ref_update_expediente, _
                    '                                                                                     stru_expediente(0).id_instrumento)
                    'If Result <> "YES" Then
                    '    Activa_editar_expediente_produccion = Result
                    '    Exit Function
                    'End If
                    'DropDownList_instrumento_edita.Enabled = False
                Else
                    Result = Refclas_gestion_Instrumento.Lista_instrumentos_producion_documental_edita(DropDownList_instrumento_edita,
                                                                                                      ref_update_expediente,
                                                                                                      id_instrumento)
                    If Result <> "YES" Then
                        Activa_editar_expediente_produccion = Result
                        Exit Function
                    End If
                End If

            Else
                Result = Refclas_gestion_Instrumento.Lista_instrumentos_producion_documental_manager(DropDownList_instrumento_edita,
                                                                                                     ref_update_expediente,
                                                                                                     stru_expediente(0).id_instrumento)
                If Result <> "YES" Then
                    Activa_editar_expediente_produccion = Result
                    Exit Function
                End If
                DropDownList_instrumento_edita.Enabled = True
            End If
            text_box_nombre_expediente.Text = stru_expediente(0).ALEAS_EXPEDIENTE
            TextBox_nombre_persona_expediente_actualizar.Text = stru_expediente(0).NOMBRE_PERSONA_EXPEDIENTE
            TextBox_identificacion_persona_expediente_actualizar.Text = stru_expediente(0).IDENTIFICACION_PERSONA_EXPEDIENTE
            TextBox_asunto_expediente_actualizar.Text = stru_expediente(0).ASUNTO_EXPEDIENTE
            TextBox_observacion_expediente_actualizar.Text = stru_expediente(0).OBSERVACION_EXPEDIENTE
            TextBox_tema_expediente_actualizar.Text = stru_expediente(0).TEMA_EXPEDIENTE
            '---------------------------------------------------------------------------------
            'lista y asigna series documentales relacionadas al  instrumento del expediente
            '---------------------------------------------------------------------------------
            Dim id_area_departamento As Integer = 0
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion,
                                                                                           id_area_departamento)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            Dim Ref_class_series As New Class_series_documentales
            If stru_expediente(0).id_instrumento <> 0 Then
                If id_tipo_instrumento = 1 Then
                    '----lista las series relacionadas al area, codigo de la serie y al id del instrumento
                    Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area_default_producion(stru_expediente(0).CODIGO_AREA_TRD,
                                                                                                              stru_expediente(0).id_instrumento,
                                                                                                              stru_expediente(0).CODIGO_SERIE,
                                                                                                              drowlis_nombre_serie,
                                                                                                              ref_update_expediente)
                    If Result <> "YES" Then
                        Activa_editar_expediente_produccion = "Función Activa_editar_expediente_produccion dice " & Result
                        Exit Function
                    End If
                End If
                If id_tipo_instrumento = 2 Then
                    '----lista las series relacionadas al  codigo de la serie y al id del instrumento
                    Result = Ref_class_series.Lista_series_relacionadas_instrumento_default_producion_simple(stru_expediente(0).id_instrumento,
                                                                                                             stru_expediente(0).CODIGO_SERIE,
                                                                                                             drowlis_nombre_serie,
                                                                                                             ref_update_expediente)
                    If Result <> "YES" Then
                        Activa_editar_expediente_produccion = "Función Activa_editar_expediente_produccion dice " & Result
                        Exit Function
                    End If
                End If

            Else
                '-----------------------------------------------
                'Lista series relacionadas a intrumento default
                '-----------------------------------------------
                drowlis_nombre_serie.Items.Clear()
                drowlis_nombre_sub_serie.Items.Clear()

            End If
            '----------------------------------------------------
            'Lista y asigna sub series documentales
            '----------------------------------------------------
            Dim Refclas_dos As New ClassGestionDocumental
            Result = Refclas_dos.Listar_SubSeries_Documentales_default_item(stru_expediente(0).CODIGO_SERIE,
                                                                            stru_expediente(0).CODIGO_SUBSERIE,
                                                                            drowlis_nombre_sub_serie)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = "Función Activa_editar_expediente_produccion dice " & Result
                Exit Function
            End If

            Dim refclas As New ClassGaProducionDocumental
            Result = Ref_GAEexpediente.Listar_fodos_documentales(drow_list_fondo,
                                                                 stru_expediente(0).NOMBRE_FONDO,
                                                                 "")
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            Dim Nombre_gabinete_default As String = ""
            Ref_GAEexpediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                      Nombre_gabinete_default)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            droplist_gabinete.Items.Clear()
            If Nombre_gabinete_default <> "" Then
                droplist_gabinete.Items.Add(Nombre_gabinete_default)
            End If
            If Nombre_gabinete_default <> "PRODUCIONDOC" Then
                droplist_gabinete.Items.Add("PRODUCIONDOC")
            End If
            Dim ref_drop As New DropDownList
            Dim Ref_class_gabinete As New ClassDaGabinete
            Result = Ref_class_gabinete.Retorna_gabinetes_permitidos_almacenamiento(HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                                    HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                    ref_drop)
            If Result <> "YES" Then
                Activa_editar_expediente_produccion = Result
                Exit Function
            End If
            If ref_drop.Items.Count > 0 Then
                For i As Integer = 0 To ref_drop.Items.Count - 1
                    If ref_drop.Items(i).Text <> "PRODUCIONDOC" And ref_drop.Items(i).Text <> Nombre_gabinete_default Then
                        droplist_gabinete.Items.Add(ref_drop.Items(i).Text)
                    End If
                Next
            End If
            'Dim nombre_instrumento As String = ""
            'If stru_expediente(0).id_instrumento <> DropDownList_instrumento_edita.SelectedValue Then
            '    drowlis_nombre_serie.Enabled = False
            '    drowlis_nombre_sub_serie.Enabled = False
            '    drow_list_fondo.Enabled = False
            '    boton.Enabled = False
            '    DropDownList_instrumento_edita.Enabled = False
            '    text_box_nombre_expediente.Enabled = False
            '    Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            '    Result = Ref_class_gestion_instrumento.Retorna_nombre_instrumento(stru_expediente(0).id_instrumento, _
            '                                                                      nombre_instrumento)
            '    result_ref = "El expdiente a editar pertenece al instrumento (" & nombre_instrumento & ") el cual no esta permitido para listar a este perfil de usuario, se recomienda no actualizar"
            'Else
            '    drowlis_nombre_serie.Enabled = True
            '    drowlis_nombre_sub_serie.Enabled = True
            '    drow_list_fondo.Enabled = True
            '    boton.Enabled = True
            '    'DropDownList_instrumento_edita.Enabled = True
            '    text_box_nombre_expediente.Enabled = True
            'End If
            Activa_editar_expediente_produccion = "YES"
        Catch ex As Exception
            Activa_editar_expediente_produccion = "Inconsistencia general función Activa_editar_expediente_produccion " & ex.Message
        Finally
            ref_update_expediente.Update()
        End Try

    End Function

    Function Seleccion_instrumento_producion_documental(ByVal id_usuario_gestion As Integer,
                                                        ByRef drop_lis_serie As DropDownList,
                                                        ByRef drop_list_subseries As DropDownList,
                                                        ByRef DropDownList_instrumento As DropDownList,
                                                        ByRef ref_update As UpdatePanel) As String
        Try
            drop_lis_serie.Items.Clear()
            drop_list_subseries.Items.Clear()
            Dim id_instrumento As Integer = 0
            If DropDownList_instrumento.Items.Count > 0 Then
                id_instrumento = DropDownList_instrumento.SelectedValue
            End If
            Dim split_sel() As String = HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Dim id_nivel As Integer = 0
            If split_sel.Length = 0 Then
                Seleccion_instrumento_producion_documental = "Por favor selecione el nivel de organización del expediente valor seleciconado (" &
                    HttpContext.Current.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") & ")"
                Exit Function
            End If
            If split_sel.Length = 1 Then
                id_nivel = Val(split_sel(0))
            Else
                id_nivel = Val(split_sel(1))
            End If
            'Dim id_nivel As Integer = split_sel(1)
            Dim Ref_class_series As New Class_series_documentales
            Dim Refclas_instrumento As New ClassGaGestionInstrumento
            Dim ref_clas_expediente As New ClassGaExpediente
            Dim stru As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Dim Result As String = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru)
            If Result <> "YES" Then
                Seleccion_instrumento_producion_documental = Result
                Exit Function
            End If
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Seleccion_instrumento_producion_documental = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                'Asigna el usuario propietario del nivel para gestionar las series y subseries segun el area del propietario
                Dim ref_Class_niveles_organizacion As New Class_niveles_organizacion
                Result = ref_Class_niveles_organizacion.Solicita_propietario_nivel_expedientes(id_nivel,
                                                                                              id_usuario_gestion)
                If Result <> "YES" Then
                    Seleccion_instrumento_producion_documental = Result
                    Exit Function
                End If
            End If
            Dim id_area_departamento As Integer = 0
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion,
                                                                                           id_area_departamento)
            If Result <> "YES" Then
                Seleccion_instrumento_producion_documental = Result
                Exit Function
            End If
            Dim id_tipo_instrumento As Integer = 0
            Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
            Result = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento,
                                                                                             id_tipo_instrumento)
            If Result <> "YES" Then
                Seleccion_instrumento_producion_documental = Result
                Exit Function
            End If
            If id_instrumento <> 0 Then
                If id_tipo_instrumento = 1 Then
                    '---------Lista solo las series relacioandas al codigo del area  y al código intrumento
                    Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento,
                                                                                            id_instrumento,
                                                                                            drop_lis_serie,
                                                                                            ref_update)
                    If Result <> "YES" Then
                        Seleccion_instrumento_producion_documental = Result
                        Exit Function
                    Else
                        drop_list_subseries.Items.Clear()
                    End If
                End If
                '----Lista todas las series relacionadas al intrumento
                If id_tipo_instrumento = 2 Then
                    Result = Ref_class_series.Lista_series_relacionadas_a_instrumento_simple(id_instrumento,
                                                                                             drop_lis_serie,
                                                                                             ref_update)
                    If Result <> "YES" Then
                        Seleccion_instrumento_producion_documental = Result
                        Exit Function
                    Else
                        drop_list_subseries.Items.Clear()
                    End If
                End If
            Else
                If stru.ACTIVA_OBLIGA_TRD = 1 Then
                    Seleccion_instrumento_producion_documental = "Debe activar el instrumento archivístico"
                    Exit Function
                End If
            End If
            Seleccion_instrumento_producion_documental = "YES"
        Catch ex As Exception
            Seleccion_instrumento_producion_documental = "Inconsistencia general función Seleccion_instrumento_producion_documental " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Activa_visualizacion_documento_producion(ByVal id_usuario_gestion As Integer,
                                                      ByVal id_registro_producion As Integer,
                                                      ByRef Iframe_visor_externo_da As Object,
                                                      ByRef UpdatePanel_visor_externo As UpdatePanel,
                                                      ByRef ModalPopupExtender_visor_externo As AjaxControlToolkit.ModalPopupExtender) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassDaGabinete
            Dim extension As String = ""
            Dim id_tipo_imagen As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim id_expediente As Integer = 0
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion,
                                                                          "",
                                                                          "",
                                                                          0,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Activa_visualizacion_documento_producion = Result
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Activa_visualizacion_documento_producion = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Activa_visualizacion_documento_producion = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Activa_visualizacion_documento_producion = Result
                    Exit Function
                End If
                If stru_permisos_niveles.visualizar_archivo = 0 Then
                    Activa_visualizacion_documento_producion = "El usuario no tiene persmiso para visualizar archivos  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = nombre_gabinete
            Result = Refclas.SolicitaIdTipoImagen(HttpContext.Current.Session.Item("DA_IMAGEN"),
                                                    HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                    id_tipo_imagen)
            If Result <> "YES" Then
                Activa_visualizacion_documento_producion = Result
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                      extension)
            If Result <> "YES" Then
                Activa_visualizacion_documento_producion = Result
                Exit Function
            End If
            Dim Class_logdocuarchi As New Class_logdocuarchi
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "PRODUCCION", "Visualiza", 0, "", "")
                Iframe_visor_externo_da.Attributes("SRC") = "../Docuarchi/WebFormDaVisorDocuarchi.aspx"
                UpdatePanel_visor_externo.Update()
                ModalPopupExtender_visor_externo.Show()
                Activa_visualizacion_documento_producion = "YES"
                Exit Function
            Else
                If HttpContext.Current.Session("UTIL_VISOR_EXPRESS_PRODUCION") = 1 And HttpContext.Current.Session("VALIDA_VISOR_EXPRES") = 1 Then
                    Dim refclas_vis As New ClassVisualisaDocumento
                    Dim matri_doc() As String = Nothing
                    Result = refclas_vis.Genera_Matris_Documentos_Almacenados(id_imagen,
                                                                              nombre_gabinete,
                                                                              matri_doc)
                    If Result <> "YES" Then
                        Activa_visualizacion_documento_producion = Result
                        Exit Function
                    End If
                    Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "PRODUCCION", "Visualiza", 0, "", "")
                    Dim printer As Integer = 0
                    Dim save As Integer = 0
                    Dim stamp As Integer = 0
                    Dim firma As Integer = 0
                    If estado_propietario = "YES" Then
                        printer = 1
                        save = 1
                        stamp = 1
                        firma = 1
                    Else
                        If stru_permisos_niveles.descarga_archivo = 1 Then
                            printer = 1
                            save = 1
                        End If
                    End If
                    Iframe_visor_externo_da.Attributes("SRC") = "../pdfjs/pdf_anotate_view/index.html?rut_image=" & matri_doc(1) & "&urimage_format=" & matri_doc(1).Replace("\", "|") & "&url_firma=" &
                                "../" & HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") & "&" & "ash=../../workflow/Handler_image_wf.ashx" &
                                "&url_id_imagen=" & id_imagen & "&url_cabinete_imagen=" & nombre_gabinete &
                                "&url_radicado=" & "" & "&url_id_workflow=" & 0 & "&url_desc_transacion=" & "PRODUCCION DOCUMENTAL" &
                                "&url_printer=" & printer & "&url_save=" & save &
                                "&url_add_firma=" & firma & "&url_add_stamp=" & stamp
                    UpdatePanel_visor_externo.Update()
                    ModalPopupExtender_visor_externo.Show()
                Else
                    Iframe_visor_externo_da.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExterno.aspx"
                    UpdatePanel_visor_externo.Update()
                    ModalPopupExtender_visor_externo.Show()
                End If

                Activa_visualizacion_documento_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Activa_visualizacion_documento_producion = "Inconsistencia general función Activa_visualizacion_documento_producion " & ex.Message
        End Try
    End Function
    Function Activa_envio_correo_electronico(ByVal id_usuario_gestion As Integer,
                                             ByVal id_registro_producion() As Long,
                                             ByRef Iframe_comparte_coreo As Object,
                                             ByRef UpdatePanel_iframenotifica As UpdatePanel,
                                             ByRef ModalPopupExtender_notifica_gestion As AjaxControlToolkit.ModalPopupExtender,
                                             ByRef Hidden_cuenta_correo_envio As Object,
                                             ByRef Hidden_correo_envio_default As Object) As String
        Try
            Dim Result As String = ""
            Dim extension As String = ""
            Dim id_tipo_imagen As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim id_expediente As Integer = 0
            If id_registro_producion.Length > 10 Then
                Activa_envio_correo_electronico = "El sistema no permite enviar mas de 10 archivos aduntos"
                Exit Function
            End If
            Result = Me.Solicita_datos_caracterizacion_archivo_produccion(id_registro_producion(0),
                                                                          "",
                                                                          "",
                                                                          0,
                                                                          id_expediente,
                                                                          id_imagen,
                                                                          nombre_gabinete,
                                                                          fecha_documento,
                                                                          numero_folios)
            If Result <> "YES" Then
                Activa_envio_correo_electronico = Result
                Exit Function
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim Ref_clas_pro_nivel_Exp As New Class_ra_pro_niveles_has_expediente_archivo
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Dim id_nivel As Integer = 0
            Result = Ref_clas_pro_nivel_Exp.Solicita_id_nivel_expediente(id_expediente,
                                                                         id_nivel)
            If Result <> "YES" Then
                Activa_envio_correo_electronico = Result
                Exit Function
            End If
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(id_usuario_gestion,
                                                                          id_nivel,
                                                                          estado_propietario)
            If Result <> "YES" Then
                Activa_envio_correo_electronico = Result
                Exit Function
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(id_nivel,
                                                                                                       id_usuario_gestion,
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    Activa_envio_correo_electronico = Result
                    Exit Function
                End If
                If stru_permisos_niveles.compartir_archivo = 0 Then
                    Activa_envio_correo_electronico = "El usuario no tiene persmiso para exportar por correo elerctrónico archivos  del expediente, debido a que el expediente pertenece a un nivel de otro usuario"
                    Exit Function
                End If
            End If
            For i As Integer = 0 To id_registro_producion.Length - 1
                If i = 0 Then
                    HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO") = id_registro_producion(i).ToString
                Else
                    HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO") = HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO") + "|" + id_registro_producion(i).ToString
                End If
            Next
            HttpContext.Current.Session.Item("GA_RUTA_TEMPORAL_DESCARGA_ARCHIVO_CORREO") = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\DESCARGA\"
            Hidden_cuenta_correo_envio.Value = ""
            Hidden_correo_envio_default.Value = ""
            Iframe_comparte_coreo.Attributes.Add("src", "../radicador/WebFormNotificar.aspx")
            UpdatePanel_iframenotifica.Update()
            ModalPopupExtender_notifica_gestion.Show()
        Catch ex As Exception
            Activa_envio_correo_electronico = "Inconsistencia general función Activa_envio_correo_electronico " & ex.Message
        End Try
    End Function

    Function Solicita_matris_documentos_producion_documental(ByVal Matri_Id_Doc() As Long,
                                                             ByRef Matri_Temp_Doc() As String,
                                                             ByVal ruta_tempo As String,
                                                             ByRef Matri_tempo_doc_copia() As String) As String
        Try
            Dim Refclasvis As New ClassVisualisaDocumento
            Dim Refclas As New ClassWorkflowReportes
            Dim IcontMatriPrinc As Integer = 0
            Dim Matricopia() As String = Nothing
            Matri_tempo_doc_copia = Nothing
            Dim Result As String = ""
            Dim i_contador As Integer = 0
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_tipo_documental As String = ""
            Dim Refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim nombre_gabinete As String = "PRODUCIONDOC"
            For i As Integer = 0 To Matri_Id_Doc.Length - 1
                Erase Matricopia
                Dim id_imagen As Integer = 0
                Result = Me.Solicita_datos_caracterizacion_archivo_produccion(Matri_Id_Doc(i),
                                                                              nombre_tipo_documental,
                                                                              "",
                                                                              0,
                                                                              0,
                                                                              id_imagen,
                                                                              nombre_gabinete,
                                                                              "",
                                                                              0)
                If Result <> "YES" Then
                    Solicita_matris_documentos_producion_documental = Result
                    Exit Function
                End If
                Result = Refclasvis.Genera_Matris_Documentos_Almacenados(id_imagen,
                                                                         nombre_gabinete,
                                                                         Matricopia)
                If Result <> "YES" Then
                    Solicita_matris_documentos_producion_documental = Result
                    Exit Function
                End If
                Dim Refclas_correo As New ClassCorreo
                If nombre_tipo_documental <> "" Then
                    Result = Refclas_correo.Normaliza_nombre_archivo(nombre_tipo_documental)
                    If Result <> "YES" Then
                        Solicita_matris_documentos_producion_documental = Result
                        Exit Function
                    End If
                End If
                Dim date_string = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(":", "-")
                '-------------------------------------------------------------------
                'Convierte en tif los documentos de formato tiff, jpg y bmp
                '-------------------------------------------------------------------
                Dim file_inf As New FileInfo(Matricopia(1))
                If UCase(file_inf.Extension) = ".TIF" Or UCase(file_inf.Extension) = ".TIFF" _
                    Or UCase(file_inf.Extension) = ".JPG" Or UCase(file_inf.Extension) = ".BMP" Then
                    Dim Ruta_Exportacion As String = ruta_tempo
                    Dim ref_matri_tempo() As String = Nothing
                    Dim it As Integer = 0
                    For i2 As Integer = 1 To Matricopia.Length - 1
                        ReDim Preserve ref_matri_tempo(it)
                        ref_matri_tempo(it) = Matricopia(i2)
                        it = it + 1
                    Next
                    Result = Refclas.Convertir_tif_pdf_correo(ref_matri_tempo,
                                                              Ruta_Exportacion,
                                                              "YES",
                                                              0,
                                                              "",
                                                              i)
                    If Result <> "YES" Then
                        Solicita_matris_documentos_producion_documental = Result
                        Exit Function
                    Else
                        Dim file_inf_copia As New FileInfo(Ruta_Exportacion)
                        Dim tempo_documento As String = ""
                        If nombre_tipo_documental <> "" Then
                            tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & Matri_Id_Doc(i) & "-" & "" & file_inf_copia.Extension
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion,
                                      tempo_documento)
                        Else
                            tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & Matri_Id_Doc(i) & "-" & file_inf_copia.Name
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion,
                                      tempo_documento)
                        End If
                        ReDim Preserve Matri_tempo_doc_copia(i_contador)
                        Matri_tempo_doc_copia(i_contador) = tempo_documento
                    End If
                Else
                    Dim file_inf_copia As New FileInfo(Matricopia(1))
                    Dim tempo_documento As String = ""
                    If nombre_tipo_documental <> "" Then
                        tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & Matri_Id_Doc(i) & "-" & "" & file_inf_copia.Extension
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    Else
                        tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & Matri_Id_Doc(i) & "-" & file_inf_copia.Name
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    End If
                    ReDim Preserve Matri_tempo_doc_copia(i_contador)
                    Matri_tempo_doc_copia(i_contador) = tempo_documento
                End If
                i_contador = i_contador + 1
            Next
            '------------------------------------------------------------------------------------------
            'Agrega en zip los documentos relacionados cuando sobre pasa el máximo número de adjuntos
            '------------------------------------------------------------------------------------------
            If Not Matri_tempo_doc_copia Is Nothing Then
                If Matri_tempo_doc_copia.Length > 20 Then
                    Dim zip As New ZipFile()
                    For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                        Dim temp_archivo As String = Matri_tempo_doc_copia(i)
                        If System.IO.File.Exists(temp_archivo) = True Then
                            zip.AddFile(Matri_tempo_doc_copia(i), "FilesAdjuntos")
                        End If
                    Next
                    Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                    zip.Save(ruta_tempo & zipName)
                    ReDim Preserve Matri_Temp_Doc(0)
                    Matri_Temp_Doc(0) = ruta_tempo & zipName
                    Solicita_matris_documentos_producion_documental = "YES"
                    Exit Function
                Else
                    If Not Matri_tempo_doc_copia Is Nothing Then
                        For i2 As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                            ReDim Preserve Matri_Temp_Doc(i2)
                            Matri_Temp_Doc(i2) = Matri_tempo_doc_copia(i2)
                        Next
                        Erase Matri_tempo_doc_copia
                    End If
                    Solicita_matris_documentos_producion_documental = "YES"
                    Exit Function
                End If
            Else
                Solicita_matris_documentos_producion_documental = "No se encontraron documentos relacionados para adjuntar, función Solicita_matris_documentos_producion_documental"
                Exit Function
            End If
            Solicita_matris_documentos_producion_documental = "YES"
        Catch ex As Exception
            Solicita_matris_documentos_producion_documental = "Inconsistencia general función Solicita_matris_documentos_producion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_id_expediente_registro_produccion(ByVal id_producion_archivo As Long,
                                                        ByRef id_expediente As Integer,
                                                        Optional valida_egistro As Integer = 0) As String
        Try
            Dim Parametro_Consulta = "select EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE " &
           " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_expediente_registro_produccion = "Funcion  Solicita_id_expediente_registro_produccion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_expediente = 0
                If valida_egistro = 1 Then
                    Solicita_id_expediente_registro_produccion = "El sistema no pudo encontrar el expediente del documento " & id_producion_archivo
                    Exit Function
                Else
                    Solicita_id_expediente_registro_produccion = "YES"
                    Exit Function
                End If

            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_expediente_registro_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_expediente_registro_produccion = "Inconsistencia general función Solicita_id_expediente_registro_produccion " & ex.Message
        End Try
    End Function
    Function Solicita_estado_meta_dato_archivo_produccion(ByVal id_producion_archivo As Long,
                                                          ByRef estado_meta_dato As Integer) As String
        Try
            Dim Parametro_Consulta = "select ESTADO_META_DATO " &
           " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_meta_dato_archivo_produccion = "Funcion  Solicita_estado_meta_dato_archivo_produccion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_meta_dato = 0
                Solicita_estado_meta_dato_archivo_produccion = "El sistema no pudo encontrar el estado del meta dato del documento de produccion  (" & id_producion_archivo & ")"
                Exit Function
            Else
                estado_meta_dato = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_meta_dato_archivo_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_meta_dato_archivo_produccion = "Inconsistencia general función Solicita_estado_meta_dato_archivo_produccion " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete_archivo_produccion(ByVal id_producion_archivo As Long,
                                                         ByRef nombre_gabinete As String) As String
        Try
            Dim Parametro_Consulta = "select NOMBRE_GABINETE " &
           " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_producion_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_gabinete_archivo_produccion = "Funcion  Solicita_nombre_gabinete_archivo_produccion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_gabinete = ""
                Solicita_nombre_gabinete_archivo_produccion = "El sistema no pudo encontrar el gabinete del documento de produccion  (" & id_producion_archivo & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Solicita_nombre_gabinete_archivo_produccion = "El sistema no registra el gabinete del documento de produccion  (" & id_producion_archivo & ")"
                    Exit Function
                End If
                nombre_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_gabinete_archivo_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete_archivo_produccion = "Inconsistencia general función Solicita_nombre_gabinete_archivo_produccion " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_produccion_documental(ByVal id_imagen As Integer,
                                                       ByVal gabinete As String,
                                                       ByRef existencia_produccion As String,
                                                       ByRef id_producion As Long,
                                                       ByRef id_expediente As Integer,
                                                       ByRef nombre_expediente As String) As String
        Try
            Dim Parametro_Consulta = "select ID_REGISTRO_PRODUCION_DOCUMENTAL,EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,EXPEDIENTE " &
            " from registro_producion_documental where NOMBRE_GABINETE='" & gabinete & "' and  " &
            "ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_imagen
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_produccion_documental = "Funcion  Solicita_existencia_produccion_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_produccion = "NO"
                id_producion = 0
                Solicita_existencia_produccion_documental = "YES"
                Exit Function
            Else
                existencia_produccion = "YES"
                id_producion = Datset.Tables(0).Rows(0).Item(0)
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    nombre_expediente = ""
                Else
                    nombre_expediente = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_existencia_produccion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_produccion_documental = "Inconsistencia general funcion Solicita_existencia_produccion_documental " & ex.Message
        End Try
    End Function
    Function Solicita_fecha_incorpora_documento(ByVal id_registro_producion As Long,
                                                ByRef fecha As String) As String
        Try
            Dim Parametro_Consulta = "select FECHA_DOCUMENTO " &
           " from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_fecha_incorpora_documento = "Funcion  Solicita_fecha_incorpora_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_fecha_incorpora_documento = "YES"
                Exit Function
            Else
                fecha = Datset.Tables(0).Rows(0).Item(0)
                Solicita_fecha_incorpora_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_fecha_incorpora_documento = "Inconsistencia general función Solicita_fecha_incorpora_documento " & ex.Message
        End Try
    End Function
    Function Registra_documento_inventario_documental(ByVal id_imagen As Integer,
                                                      ByVal gabinete As String,
                                                      ByRef registro_producion_documental As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra el inventario del documento y retorna el registro
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen en el 
        '                        gabinete
        'gabinete              : Representa el gabinete al que pertenece el documento
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'registro_producion_documental  : Retorna el registro de produción
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Option_aplicar_trd As Integer = 0
            Dim Option_unidad_conservacion As Integer = 0
            Dim Class_sytem1_ As New Class_system1
            Result = Class_sytem1_.VerificaOpcionAplicarTablaRetencion(Option_aplicar_trd,
                                                                           gabinete)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Result = Class_sytem1_.VerificaOpcionAplicarInventarioDocumental(Option_unidad_conservacion,
                                                                                 gabinete)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Dim numero_paginas As Integer = 0
            Dim tipo_doc As Integer = 0
            Result = ref_ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(gabinete,
                                                                                              id_imagen,
                                                                                              stru_paramter_image,
                                                                                              Option_aplicar_trd)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Result = ClassAlmacenamiento.Retorna_parametros_almacenamiento_documento_relacionado(id_imagen,
                                                                                                 matri_datos_almacen,
                                                                                                 matri_gestion,
                                                                                                 gabinete)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado As String = ""
            Result = Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(gabinete,
                                                                             nombre_campo_radicado)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            '-----------//Solicita valor campos radicado---------//
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim radicado As String = ""
            Result = ClassDaGabinete.Solicita_valor_campo_gebinete(id_imagen,
                                                                   gabinete,
                                                                   nombre_campo_radicado,
                                                                   radicado)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim extenssion As String = ""
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                                extenssion)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim datos_imagen_gabinete As String = ""
            Result = ref_ClassDaGabinete.Solicita_datos_imagen_gabinete(gabinete,
                                                                        id_imagen,
                                                                        datos_imagen_gabinete)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            extenssion = extenssion.Replace(".", "")
            Dim ref_expediente As String = "null"
            Dim ref_nombre_serie As String = "null"
            Dim ref_nombre_sub_serie As String = "null"
            Dim ref_tipo_documento As String = "null"
            Dim ref_unidad_conserva As String = "null"
            Dim ref_clase_documento As String = "null"
            Dim ref_fecha_elaboracion As String = "null"
            Dim ref_id_expediente As String = "null"
            Dim ref_id_unidad_conservacion As String = "null"
            Dim ref_id_area As String = "null"
            Dim ref_id_serie As String = "null"
            Dim ref_id_tipo_unidad_conservacion As String = "null"
            Dim ref_id_clase_documento As String = "null"
            Dim ref_nombre_area As String = "null"
            Dim ref_id_sub_serie As String = "null"
            Dim ref_id_tipo_documento As String = "null"
            Dim ref_id_tipo_expediente As String = "null"
            Dim ref_id_tipo_unidad_documental As String = "null"
            Dim ref_radicado As String = "null"
            Dim sugundo_nombre_documento As String = ""
            If radicado <> "" Then
                ref_radicado = "'" & radicado & "'"
            End If
            Dim ref_sugundo_nombre_documento As String = ""
            Dim nombre_docuarchi As String = ""
            If sugundo_nombre_documento <> "" Then
                ref_sugundo_nombre_documento = "'" & sugundo_nombre_documento & "'"
                nombre_docuarchi = sugundo_nombre_documento
            Else
                Dim Ceros_Cuerpo_Imag As String = "DIG"
                Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, id_imagen)
                ref_sugundo_nombre_documento = "'DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion & "'"
                nombre_docuarchi = "DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion
            End If
            Dim matri_doc() As String = Nothing
            Dim tamano As String = ""
            Result = ref_ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                         gabinete,
                                                                                         matri_doc)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            Dim tam_archivo As Object = 1024
            For i As Integer = 1 To matri_doc.Length - 1
                Dim fi As New FileInfo(matri_doc(i))
                If fi.Exists Then
                    tam_archivo = tam_archivo + fi.Length
                End If
            Next
            If (tam_archivo / 1024) > 1024 Then
                tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
            Else
                tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
            End If
            '-------------------------------------------------
            'Detecta el numero de paaginas cundo el documento
            'es diferente a TIF, BMP, JPG
            '-------------------------------------------------
            Dim pagi As Integer = matri_doc.Length - 1
            Dim numero_pagina As Integer = -1
            'Dim ref_ClassAlmacenamiento As New ClassAlmacenamiento
            Dim Class_ItexShare As New Class_ItexShare
            Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(matri_doc(1),
                                                                                  numero_pagina)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            End If
            If numero_pagina <> -1 Then
                pagi = numero_pagina
            End If
            Dim nombre_area As String = ""
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If matri_gestion.ID_AREA <> 0 Then
                Result = Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(matri_gestion.ID_AREA,
                                                                                       nombre_area)
                If Result <> "YES" Then
                    Registra_documento_inventario_documental = Result
                    Exit Function
                End If
            End If
            Dim id_expediente As Integer = matri_gestion.ID_EXPEDIENTE
            Dim id_clase_documento As Integer = stru_paramter_image.ID_TIPODOCUMENTO
            Dim tipo_documento As String = stru_paramter_image.TIPODOCUMENTO
            Dim id_tipo_unidad_documental As Integer = matri_gestion.ID_TIPO_UNIDAD_CONSERVACION
            Dim id_tipo_expediente As Integer = matri_gestion.ID_TIPO_EXPEDIENTE
            Dim id_tipo_unidad_conservacion As Integer = matri_gestion.ID_TIPO_UNIDAD_CONSERVACION
            Dim id_sub_serie As Integer = matri_gestion.ID_SERIE
            Dim id_serie As Integer = matri_gestion.ID_SERIE
            Dim id_area As Integer = matri_gestion.ID_AREA
            Dim id_unidad_conservacion = matri_gestion.ID_EXPEDIENTE
            Dim expediente As String = matri_gestion.EXPEDIENTE
            Dim nombre_serie As String = matri_gestion.NOMBRE_SERIE
            Dim nombre_sub_serie As String = matri_gestion.NOMBRE_SUB_SERIE
            Dim unidad_conserva As String = ""
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim fecha_elaboracion As String = date1al
            Dim estado_archivo As Integer = 0
            Dim tipo_archivo_producion As Integer = 0
            If id_clase_documento <> 0 Then
                ref_id_clase_documento = id_clase_documento
            End If
            If id_tipo_unidad_documental <> 0 Then
                ref_id_tipo_unidad_documental = id_tipo_unidad_documental
            End If
            If id_tipo_expediente <> 0 Then
                ref_id_tipo_expediente = id_tipo_expediente
            End If
            If id_sub_serie <> 0 Then
                ref_id_sub_serie = id_sub_serie
            End If
            If nombre_area <> "" Then
                ref_nombre_area = "'" & nombre_area & "'"
            End If
            If id_tipo_unidad_conservacion <> 0 Then
                ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
            End If
            If id_serie <> 0 Then
                ref_id_serie = id_serie
            End If
            If id_area <> 0 Then
                ref_id_area = id_area
            End If
            If id_expediente <> 0 Then
                ref_id_expediente = id_expediente
            End If
            If id_unidad_conservacion <> 0 Then
                ref_id_unidad_conservacion = id_unidad_conservacion
            End If
            If expediente <> "" Then
                ref_expediente = "'" & expediente & "'"
            End If
            If nombre_serie <> "" Then
                ref_nombre_serie = "'" & nombre_serie & "'"
            End If
            If nombre_sub_serie <> "" Then
                ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
            End If
            If nombre_sub_serie <> "" Then
                ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
            End If
            If tipo_documento <> "" Then
                ref_tipo_documento = "'" & tipo_documento & "'"
            End If
            If unidad_conserva <> "" Then
                ref_unidad_conserva = "'" & unidad_conserva & "'"
            End If
            If clase_documento <> "" Then
                ref_clase_documento = "'" & clase_documento & "'"
            End If
            If fecha_elaboracion <> "" Then
                ref_fecha_elaboracion = "'" & fecha_elaboracion & "'"
            End If
            If id_expediente <> 0 Or id_unidad_conservacion <> 0 Then
                estado_archivo = 0
            End If
            Dim sqlinventario As String = "insert into registro_producion_documental (remit_dest_interno_idremit_dest_interno," &
                    "ID_USUARIO_GESTION,FECHA_DOCUMENTO,ID_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO,SERIE_DOCUMENTO," &
                    "ID_SUBSERIE_DOCUMENTO,SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,FULTEXT_DOCUMENTO," &
                    "ID_DOCUMENTO_DOCUARCHI_ALMACEN,ESTADO_DOCUMENTO_ARCHIVO,NOMBRE_GABINETE,NUMERO_FOLIOS," &
                    "EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION," &
                    "ID_UNIDAD_CONSERVACION,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO," &
                    "FECHA_ELABORACION,UNIDADCONSERVA,NOMBRE_AREA_DEPARTAMENTO,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
                    "RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO,DOCUMENTO_PRODUCION_DOCUMENTAL,TAMANO,FORMATO) values "
            Dim datos_insert_inventario As String = "(" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ",'" & date1al & "'," &
            ref_id_area & "," & ref_id_serie & "," & ref_nombre_serie & "," & ref_id_sub_serie & "," & ref_nombre_sub_serie &
            "," & ref_id_clase_documento & "," & ref_tipo_documento & ",'" & datos_imagen_gabinete & "'," & id_imagen & "," &
            estado_archivo & ",'" & gabinete & "'," & pagi & "," & ref_id_expediente & "," & ref_expediente & "," & ref_id_tipo_expediente &
            "," & ref_id_tipo_unidad_conservacion & "," & ref_id_unidad_conservacion & "," & ref_id_clase_documento & "," &
            ref_clase_documento & "," & ref_fecha_elaboracion & "," & ref_unidad_conserva & "," & ref_nombre_area & "," & ref_id_tipo_unidad_documental &
            "," & HttpContext.Current.Session.Item("GA_IDEMPRESA") & "," & ref_radicado & "," & ref_sugundo_nombre_documento & "," & tipo_archivo_producion & ",'" & tamano & "','" & extenssion & "')"
            sqlinventario = sqlinventario & datos_insert_inventario
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sqlinventario,
                                                       registro_producion_documental)
            If Result <> "YES" Then
                Registra_documento_inventario_documental = Result
                Exit Function
            Else
                Registra_documento_inventario_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_documento_inventario_documental = "Inconsistencia funcion  Registra_documento_inventario_documental " & ex.Message
        End Try
    End Function
    Function Solicita_id_documento_produccion_documental(ByVal id_imagen As Integer,
                                                         ByVal nombre_gabinete As String,
                                                         ByRef id_registro_producion As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la producción documental de un documento
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen en el 
        '                        gabinete
        'gabinete              : Representa el gabinete al que pertenece el documento
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'registro_producion_documental  : Retorna la identificacion registro de
        '                                 produción documental
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-06-19
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select ID_REGISTRO_PRODUCION_DOCUMENTAL " &
        " from registro_producion_documental where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_imagen &
        " and NOMBRE_GABINETE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_documento_produccion_documental = "Funcion  Solicita_id_documento_produccion_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_producion = 0
                Solicita_id_documento_produccion_documental = "YES"
                Exit Function
            Else
                id_registro_producion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_documento_produccion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_documento_produccion_documental = "Inconsistencia general funcion Solicita_id_documento_produccion_documental " & ex.Message
        End Try
    End Function
End Class
