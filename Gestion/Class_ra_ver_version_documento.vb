Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports Neodynamic
Imports Neodynamic.WebControls.ImageDraw
Public Class class_version_paramerter_replace
    Public NameModulo As String
    Public Gabinete As String
    Public IdImagen As Integer
End Class
Public Class class_Ref_ra_ver_version_documento
    Public Error_result As String
    Public id_registro_migracion As Long
End Class
Public Class class_list_detalle_version_document
    Public Property id_registro_version As Long
    Public Property id_version_doc As Integer
    Public Property fecha_registro_version As String
    Public Property ESTADO_ACTIVO_GABINETE As String
    Public Property error_sistema As String
    Public Property id_registro_version_old As Long
    Public Property TIPO_ARCHIVO As String
    Public Property ID As Integer
    Public Property DBT As Object
    Public Property option_remplaza As Integer
    Public Property IconoAsome As String
    Public Property ESTADO_FIRMA_DIGITAL As Integer
End Class
Public Class class_result_list_detalle_version_document
    Public ILIST_lista_detalle_version_document As New List(Of class_list_detalle_version_document)
    Public Property Error_result As String
    Public Property Gabinete As String
    Public Property imagen As Integer
    Public Property id_registro_version As Long
    Public Property extension_archivo As String

End Class
Public Structure Stru_registro_version_documento
    Dim id_registro_version As Long
    Dim system1_id_gabinete As Integer
    Dim ra_mig_reg_mig_id_registro_migracion As Long
    Dim fecha_registro_version As String
    Dim nombre_gabinete As String
    Dim id_version_doc As Integer
    Dim id_usuario_da As Integer
    Dim id_usuario_gestion As Integer
    Dim descripcion_version As String
    Dim ID As Integer
    Dim DISC As Integer
    Dim PAG As Integer
    Dim DBT As Integer
    Dim IDEX As Integer
    Dim USER_DA As String
    Dim CTRL_ACES As Integer
    Dim PESO_DOCUMENTO As String
    Dim TIPO_ARCHIVO As String
    Dim PAGINAS_DOCUMENT As Integer
    Dim ESTADO_FIRMA_DIGITAL As Integer
    Dim ESTADO_ACTIVO_GABINETE As Integer
    Dim EstruDocumentosRelacionados() As String
End Structure
Public Class ra_ver_version_documento
    Public Property id_registro_version As Long
    Public Property system1_id_gabinete As Integer
    Public Property ra_mig_reg_mig_id_registro_migracion As Long
    Public Property fecha_registro_version As String
    Public Property nombre_gabinete As String
    Public Property id_version_doc As Integer
    Public Property id_usuario_da As Integer
    Public Property id_usuario_gestion As Integer
    Public Property name_usuario As String
    Public Property descripcion_version As String
    Public Property ID As Integer
    Public Property DISC As Integer
    Public Property PAG As Integer
    Public Property DBT As Integer
    Public Property IDEX As Integer
    Public Property USER_DA As String
    Public Property CTRL_ACES As Integer
    Public Property PESO_DOCUMENTO As String
    Public Property TIPO_ARCHIVO As String
    Public Property PAGINAS_DOCUMENT As Integer
    Public Property ESTADO_FIRMA_DIGITAL As Integer
    Public Property ESTADO_ACTIVO_GABINETE As Integer
    Public Property Error_result As String
End Class
Public Class Class_ra_ver_version_documento
    Function Solicita_class_version_documento(ByVal id_registro_version As Long,
                                              ByRef ra_ver_version_documento As ra_ver_version_documento) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la clase del registro de versión de un documento
        '          con el parametro de idneitifcación del registro
        '
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               versión del documento
        '                               
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'ra_ver_version_documento : Retorna la clase del registro de version
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " Select id_registro_version,system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion,fecha_registro_version," &
                "nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,descripcion_version,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
                "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE,rdi.Login_Usuario,rdi.Nombre_Remitente,rdi.Cargo_Remite" &
                " from ra_ver_version_documento " &
                " left OUTER JOIN remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=id_usuario_gestion)" &
                " where id_registro_version=" & id_registro_version
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_version_documento = "Funcion  Solicita_class_version_documento " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_version_documento = "Imposible encontrar la estructura del registro de version (" & id_registro_version & ")"
                Exit Function
            Else
                ra_ver_version_documento.id_registro_version = Datset.Tables(0).Rows(0).Item("id_registro_version")
                ra_ver_version_documento.system1_id_gabinete = Datset.Tables(0).Rows(0).Item("system1_id_gabinete")
                If Datset.Tables(0).Rows(0).IsNull("ra_mig_reg_mig_id_registro_migracion") Then
                    ra_ver_version_documento.ra_mig_reg_mig_id_registro_migracion = 0
                Else
                    ra_ver_version_documento.ra_mig_reg_mig_id_registro_migracion = Datset.Tables(0).Rows(0).Item("ra_mig_reg_mig_id_registro_migracion")
                End If
                ra_ver_version_documento.fecha_registro_version = Datset.Tables(0).Rows(0).Item("fecha_registro_version")
                ra_ver_version_documento.nombre_gabinete = Datset.Tables(0).Rows(0).Item("nombre_gabinete")
                ra_ver_version_documento.id_version_doc = Datset.Tables(0).Rows(0).Item("id_version_doc")
                ra_ver_version_documento.id_usuario_da = Datset.Tables(0).Rows(0).Item("id_usuario_da")
                ra_ver_version_documento.id_usuario_gestion = Datset.Tables(0).Rows(0).Item("id_usuario_gestion")
                If Datset.Tables(0).Rows(0).IsNull("descripcion_version") Then
                    ra_ver_version_documento.descripcion_version = ""
                Else
                    ra_ver_version_documento.descripcion_version = Datset.Tables(0).Rows(0).Item("descripcion_version")
                End If
                ra_ver_version_documento.ID = Datset.Tables(0).Rows(0).Item("ID")
                ra_ver_version_documento.DISC = Datset.Tables(0).Rows(0).Item("DISC")
                ra_ver_version_documento.PAG = Datset.Tables(0).Rows(0).Item("PAG")
                ra_ver_version_documento.IDEX = Datset.Tables(0).Rows(0).Item("IDEX")
                ra_ver_version_documento.DBT = Datset.Tables(0).Rows(0).Item("DBT")
                ra_ver_version_documento.USER_DA = Datset.Tables(0).Rows(0).Item("USER_DA")
                ra_ver_version_documento.CTRL_ACES = Datset.Tables(0).Rows(0).Item("CTRL_ACES")
                ra_ver_version_documento.PESO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("PESO_DOCUMENTO")
                ra_ver_version_documento.TIPO_ARCHIVO = Datset.Tables(0).Rows(0).Item("TIPO_ARCHIVO")
                ra_ver_version_documento.PAGINAS_DOCUMENT = Datset.Tables(0).Rows(0).Item("PAGINAS_DOCUMENT")
                ra_ver_version_documento.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(0).Item("ESTADO_FIRMA_DIGITAL")
                ra_ver_version_documento.ESTADO_ACTIVO_GABINETE = Datset.Tables(0).Rows(0).Item("ESTADO_ACTIVO_GABINETE")
                ra_ver_version_documento.name_usuario = Datset.Tables(0).Rows(0).Item("Login_Usuario") & " (" & Datset.Tables(0).Rows(0).Item("Nombre_Remitente") & ")" &
                    "(" & Datset.Tables(0).Rows(0).Item("Cargo_Remite") & ")"
                Solicita_class_version_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_version_documento = "Inconsistencia general funcion Solicita_class_version_documento " & ex.Message
        End Try
    End Function
    Function Solicita_registro_activo_gabinete(ByVal id_imagen As Integer,
                                               ByVal id_gabinete As Integer,
                                               ByRef id_registro_version As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la identicicación del registro de versión activo en el 
        '          gabinete
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'id_gabinete                  : Representa la identiiccación del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identificacion del registro de
        '                               version activo en el gabinete 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " Select id_registro_version from ra_ver_version_documento " &
               " where ID=" & id_imagen & " and system1_id_gabinete=" & id_gabinete & " and ESTADO_ACTIVO_GABINETE=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_activo_gabinete = "Funcion  Solicita_registro_activo_gabinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_version = 0
                Solicita_registro_activo_gabinete = "YES"
                Exit Function
            Else
                id_registro_version = Datset.Tables(0).Rows(0).Item(0)
                Solicita_registro_activo_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_activo_gabinete = "Inconsistencia general funcion Solicita_registro_activo_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_version_documento(ByVal id_registro_version As Long,
                                                   ByRef Stru_registro_version_documento As stru_registro_version_documento) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registro de versión de un documento
        '          con el parametro de idneitifcación del registro
        '
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               versión del documento
        '                               
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Stru_registro_version_documento : Retorna la estructura del registro de version
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-07
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " Select id_registro_version,system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion,fecha_registro_version," &
                "nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,descripcion_version,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
                "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE" &
                " from ra_ver_version_documento where id_registro_version=" & id_registro_version
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_version_documento = "Funcion  Solicita_estructura_version_documento " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_version_documento = "Imposible encontrar la estructura del registro de version (" & id_registro_version & ")"
                Exit Function
            Else
                Stru_registro_version_documento.id_registro_version = Datset.Tables(0).Rows(0).Item("id_registro_version")
                Stru_registro_version_documento.system1_id_gabinete = Datset.Tables(0).Rows(0).Item("system1_id_gabinete")
                If Datset.Tables(0).Rows(0).IsNull("ra_mig_reg_mig_id_registro_migracion") Then
                    Stru_registro_version_documento.ra_mig_reg_mig_id_registro_migracion = 0
                Else
                    Stru_registro_version_documento.ra_mig_reg_mig_id_registro_migracion = Datset.Tables(0).Rows(0).Item("ra_mig_reg_mig_id_registro_migracion")
                End If
                Stru_registro_version_documento.fecha_registro_version = Datset.Tables(0).Rows(0).Item("fecha_registro_version")
                Stru_registro_version_documento.nombre_gabinete = Datset.Tables(0).Rows(0).Item("nombre_gabinete")
                Stru_registro_version_documento.id_version_doc = Datset.Tables(0).Rows(0).Item("id_version_doc")
                Stru_registro_version_documento.id_usuario_da = Datset.Tables(0).Rows(0).Item("id_usuario_da")
                Stru_registro_version_documento.id_usuario_gestion = Datset.Tables(0).Rows(0).Item("id_usuario_gestion")
                If Datset.Tables(0).Rows(0).IsNull("descripcion_version") Then
                    Stru_registro_version_documento.descripcion_version = ""
                Else
                    Stru_registro_version_documento.descripcion_version = Datset.Tables(0).Rows(0).Item("descripcion_version")
                End If
                Stru_registro_version_documento.ID = Datset.Tables(0).Rows(0).Item("ID")
                Stru_registro_version_documento.DISC = Datset.Tables(0).Rows(0).Item("DISC")
                Stru_registro_version_documento.PAG = Datset.Tables(0).Rows(0).Item("PAG")
                Stru_registro_version_documento.IDEX = Datset.Tables(0).Rows(0).Item("IDEX")
                Stru_registro_version_documento.DBT = Datset.Tables(0).Rows(0).Item("DBT")
                Stru_registro_version_documento.USER_DA = Datset.Tables(0).Rows(0).Item("USER_DA")
                Stru_registro_version_documento.CTRL_ACES = Datset.Tables(0).Rows(0).Item("CTRL_ACES")
                Stru_registro_version_documento.PESO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("PESO_DOCUMENTO")
                Stru_registro_version_documento.TIPO_ARCHIVO = Datset.Tables(0).Rows(0).Item("TIPO_ARCHIVO")
                Stru_registro_version_documento.PAGINAS_DOCUMENT = Datset.Tables(0).Rows(0).Item("PAGINAS_DOCUMENT")
                Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(0).Item("ESTADO_FIRMA_DIGITAL")
                Stru_registro_version_documento.ESTADO_ACTIVO_GABINETE = Datset.Tables(0).Rows(0).Item("ESTADO_ACTIVO_GABINETE")
                Solicita_estructura_version_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_version_documento = "Inconsistencia general funcion Solicita_estructura_version_documento " & ex.Message
        End Try
    End Function
    Function SolicitaEstructurasVersionesDocumentoPorIdGabinete(ByVal IdImagen As Integer,
                                                                ByVal IdGabinete As Integer,
                                                                ByVal EstadoActivo As Integer,
                                                                ByRef Stru_registro_version_documento() As Stru_registro_version_documento) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la matriz de estructuras del registro de versiones de un
        'documento con los parametros identificacion del gabinete y identificacin de la 
        'imagenen en el gabinete
        '          
        '
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'IdImagen          : Representa la identiifcación de la imagen en el gabinete
        'IdGabinete        : Representa la identiifcación del gabinete                          
        '                               
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Stru_registro_version_documento() : Retorna la estructura del registro de versiones
        ' de un documento
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2025-02-06
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " Select id_registro_version,system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion,fecha_registro_version," &
                "nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,descripcion_version,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
                "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE" &
                " from ra_ver_version_documento where ID=" & IdImagen & " and system1_id_gabinete=" & IdGabinete & " and ESTADO_ACTIVO_GABINETE=" & EstadoActivo
            Stru_registro_version_documento = Nothing
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructurasVersionesDocumentoPorIdGabinete = "Funcion  SolicitaEstructurasVersionesDocumentoPorIdGabinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstructurasVersionesDocumentoPorIdGabinete = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Stru_registro_version_documento(i)
                    Stru_registro_version_documento(i).id_registro_version = Datset.Tables(0).Rows(i).Item("id_registro_version")
                    Stru_registro_version_documento(i).system1_id_gabinete = Datset.Tables(0).Rows(i).Item("system1_id_gabinete")
                    If Datset.Tables(0).Rows(i).IsNull("ra_mig_reg_mig_id_registro_migracion") Then
                        Stru_registro_version_documento(i).ra_mig_reg_mig_id_registro_migracion = 0
                    Else
                        Stru_registro_version_documento(i).ra_mig_reg_mig_id_registro_migracion = Datset.Tables(0).Rows(i).Item("ra_mig_reg_mig_id_registro_migracion")
                    End If
                    Stru_registro_version_documento(i).fecha_registro_version = Datset.Tables(0).Rows(i).Item("fecha_registro_version")
                    Stru_registro_version_documento(i).nombre_gabinete = Datset.Tables(0).Rows(i).Item("nombre_gabinete")
                    Stru_registro_version_documento(i).id_version_doc = Datset.Tables(0).Rows(i).Item("id_version_doc")
                    Stru_registro_version_documento(i).id_usuario_da = Datset.Tables(0).Rows(i).Item("id_usuario_da")
                    Stru_registro_version_documento(i).id_usuario_gestion = Datset.Tables(0).Rows(i).Item("id_usuario_gestion")
                    If Datset.Tables(0).Rows(i).IsNull("descripcion_version") Then
                        Stru_registro_version_documento(i).descripcion_version = ""
                    Else
                        Stru_registro_version_documento(i).descripcion_version = Datset.Tables(0).Rows(i).Item("descripcion_version")
                    End If
                    Stru_registro_version_documento(i).ID = Datset.Tables(0).Rows(i).Item("ID")
                    Stru_registro_version_documento(i).DISC = Datset.Tables(0).Rows(i).Item("DISC")
                    Stru_registro_version_documento(i).PAG = Datset.Tables(0).Rows(i).Item("PAG")
                    Stru_registro_version_documento(i).IDEX = Datset.Tables(0).Rows(i).Item("IDEX")
                    Stru_registro_version_documento(i).DBT = Datset.Tables(0).Rows(i).Item("DBT")
                    Stru_registro_version_documento(i).USER_DA = Datset.Tables(0).Rows(i).Item("USER_DA")
                    Stru_registro_version_documento(i).CTRL_ACES = Datset.Tables(0).Rows(i).Item("CTRL_ACES")
                    Stru_registro_version_documento(i).PESO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("PESO_DOCUMENTO")
                    Stru_registro_version_documento(i).TIPO_ARCHIVO = Datset.Tables(0).Rows(i).Item("TIPO_ARCHIVO")
                    Stru_registro_version_documento(i).PAGINAS_DOCUMENT = Datset.Tables(0).Rows(i).Item("PAGINAS_DOCUMENT")
                    Stru_registro_version_documento(i).ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(i).Item("ESTADO_FIRMA_DIGITAL")
                    Stru_registro_version_documento(i).ESTADO_ACTIVO_GABINETE = Datset.Tables(0).Rows(i).Item("ESTADO_ACTIVO_GABINETE")
                Next
                SolicitaEstructurasVersionesDocumentoPorIdGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructurasVersionesDocumentoPorIdGabinete = "Inconsistencia general funcion SolicitaEstructurasVersionesDocumentoPorIdGabinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_discos_carpetas(ByVal id_imagen As Integer,
                                            ByVal id_gabinete As Integer,
                                            ByRef Stru_registro_version_documento() As stru_registro_version_documento) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la esctructura de discos y carpetas de una version de las
        '          versiones de un documento
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'id_gabinete                  : Representa la identiiccación del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Stru_registro_version_documento : Retorna la estructura de los discos y 
        '                                  carpetas de una version
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " Select DISC,IDEX from ra_ver_version_documento " &
                " where ID=" & id_imagen & " and system1_id_gabinete=" & id_gabinete
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_discos_carpetas = "Funcion  Solicita_lista_discos_carpetas " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_discos_carpetas = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Stru_registro_version_documento(i)
                    Stru_registro_version_documento(i).DISC = Datset.Tables(0).Rows(i).Item(0)
                    Stru_registro_version_documento(i).IDEX = Datset.Tables(0).Rows(i).Item(1)

                Next
                Solicita_lista_discos_carpetas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_discos_carpetas = "Inconsistencia general funcion Solicita_lista_discos_carpetas " & ex.Message
        End Try
    End Function
    Function Solicita_lista_versiones_de_documentos(ByVal id_imagen As Integer,
                                                    ByVal id_gabinete As Integer,
                                                    ByRef Class_list_detalle_version_document As List(Of class_list_detalle_version_document)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la lista de versiones de un documento
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'id_gabinete                  : Representa la identiiccación del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_list_detalle_version_document : Retorna la estructura del detalle de la lista
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim SQL_consulta As String = " Select id_registro_version,id_version_doc,fecha_registro_version,ESTADO_ACTIVO_GABINETE,TIPO_ARCHIVO,ESTADO_FIRMA_DIGITAL from ra_ver_version_documento " &
                " where ID=" & id_imagen & " and system1_id_gabinete=" & id_gabinete & " and ESTADO_ELIMINA_VERSION=0"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_versiones_de_documentos = "Funcion  Solicita_lista_versiones_de_documentos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_versiones_de_documentos = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item As New class_list_detalle_version_document
                    item.id_registro_version = Datset.Tables(0).Rows(i).Item(0)
                    item.id_version_doc = Datset.Tables(0).Rows(i).Item(1)
                    item.fecha_registro_version = Datset.Tables(0).Rows(i).Item(2)
                    If Datset.Tables(0).Rows(i).Item(3) = 0 Then
                        item.ESTADO_ACTIVO_GABINETE = ""
                    Else
                        item.ESTADO_ACTIVO_GABINETE = "SI"
                    End If
                    item.TIPO_ARCHIVO = Datset.Tables(0).Rows(i).Item(4)
                    item.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(i).Item(5)
                    If item.ESTADO_FIRMA_DIGITAL = 1 Then
                        item.IconoAsome = "fa-file-certificate"
                    Else
                        ClassDaGabinete.Agrega_icono_image_fownt_extension_cort(item.TIPO_ARCHIVO,
                                                                                item.IconoAsome)
                    End If
                    Class_list_detalle_version_document.Add(item)
                Next
                Solicita_lista_versiones_de_documentos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_versiones_de_documentos = "Inconsistencia general funcion Solicita_lista_versiones_de_documentos " & ex.Message
        End Try
    End Function
    Function Remplaza_version_documento(ByVal id_registro_migracion As Long,
                                        ByVal id_imagen As Integer,
                                        ByVal gabinete As String,
                                        ByVal id_usuario_gestion As Integer,
                                        ByVal id_usuario_da As Integer,
                                        ByVal logui_usuario_gestion As String,
                                        ByVal logui_usuario_da As String,
                                        ByRef Extension As String) As String
        '--------------------------------------------------------------------------------
        'Funcion : función que remplaza la version de un documento migrado en el servidor
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete.
        'gabinete              : Representa el nombre del gabinete.
        'id_usuario_da         : Representa la identificación del usuario docuarchi
        'id_registro_migracion : Representa la identificación del registro de migración
        '                        de documentos.
        'logui_usuario_gestion : Representa el login del usuario de gestion
        'logui_usuario_da      : Representa el logion del usuaario docuarchi
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Extension             : Retorna la extensión del archivo migrado
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            Dim Stru_registro_migracion As stru_registro_migracion = Nothing
            If HttpContext.Current.Session.Item("UTIL_MIGRA_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                Remplaza_version_documento = "El usuario no tiene permiso para remplazar la versión del documento en modulo de migración"
                Exit Function
            End If
            Result = Class_ra_mig_registro_migracion.Solicita_estructura_registro_migracion_documento(id_registro_migracion,
                                                                                                      Stru_registro_migracion)
            If Result <> "YES" Then
                Remplaza_version_documento = Result
                Exit Function
            End If
            Dim matri_documentos() As String = Nothing
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     gabinete,
                                                                                     matri_documentos)
            If Result <> "YES" Then
                Remplaza_version_documento = Result
                Exit Function
            End If
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Result = ClassAlmacenamiento.Almacena_documento_migrado_nueva_version(Stru_registro_migracion,
                                                                                  matri_documentos,
                                                                                  id_usuario_gestion,
                                                                                  id_usuario_da,
                                                                                  logui_usuario_gestion,
                                                                                  logui_usuario_da,
                                                                                  Extension)
            If Result <> "YES" Then
                Remplaza_version_documento = Result
                Exit Function
            End If
            Remplaza_version_documento = "YES"
        Catch ex As Exception
            Remplaza_version_documento = "Inconsistencia general funcion Remplaza_version_documento " & ex.Message
        End Try
    End Function

    Function AdjuntaVersionDocumento(ByVal name_modulo As String,
                                     ByVal option_remplaza_gabinete As Integer,
                                     ByVal gabinete As String,
                                     ByVal id_imagen As Integer,
                                     ByVal ruta_documento As String,
                                     ByVal IdUsuarioGestion As Integer,
                                     ByVal IdUsuarioDocuarchi As Integer,
                                     ByVal LoginUsuarioGestion As String,
                                     ByVal LoginUsuarioDocuarchi As String,
                                     ByRef extension_documento As String,
                                     ByRef item_list As class_list_detalle_version_document) As String
        '---------------------------------------------------------------------------------------
        'Funcion : Adjunta la versión del documento al registro de versiones
        '          con la opción de remplazar en el gabinete la version del documento      
        '          estableciendo el parametro  "option_remplaza_gabinete" en 1
        '---------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------------
        'name_modulo                  : Representa el nombre del modulo que reaiza
        '                               la opoeracion
        '                               1- Modulo migracion  (MIGRACION)
        '                               2- Modulo workflow (WORFKLOW)
        '                               3- modulo producion (PRODUCCION)
        '                               4- modulo docuarchi (DOCUARCHI)
        '                               5- Modulo Radicacion  (RADICACION)
        '                               6- Modulo gestion correspondencia  (CORRESPO)
        '                               7- Modulo sistema (Remplaza sin control)
        'option_remplaza_gabinete     : Representa la opcion de de remplazar el documento
        '                               de nueva version en el gabinete como imagen princiapal
        'id_imagen                    : Representa la identiifcación de la imagen
        'gabinete                     : Representa el nombre del gabinete al que 
        '                               pertecence la imagen
        'ruta_documento               : Representa la ruta temporal del documento de la nueva
        '                               version
        '---------------------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------------------
        'extension_documento          : Representa la extensión del nuevo documento
        'item_list                    : Representa la estructura con los detalles del nuevo documento
        '                               de version                         
        '---------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------------------
        'Fecha                 : 2024-07-27
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------------------

        Try
            item_list.option_remplaza = option_remplaza_gabinete
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            If (File.Exists(ruta_documento) = False) Then
                AdjuntaVersionDocumento = "Imposible encontar el archivo (" & ruta_documento & ") en el servidor"
                Exit Function
            End If
            '////---------Retorna icono file
            Dim FileINF As New FileInfo(ruta_documento)
            Dim Extension As String = UCase(FileINF.Extension)
            ClassDaGabinete.Agrega_icono_image_fownt_extension_cort(Extension, item_list.IconoAsome)
            '////---------Opción remplaza documento en gabinete
            If option_remplaza_gabinete = 1 Then
                If name_modulo = "MIGRACION" And HttpContext.Current.Session.Item("UTIL_VER_MIG_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de migración"
                    Exit Function
                End If
                If name_modulo = "WORKFLOW" And HttpContext.Current.Session.Item("UTIL_VER_WF_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de workflow"
                    Exit Function
                End If
                If name_modulo = "PRODUCCION" And HttpContext.Current.Session.Item("UTIL_VER_PR_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de producción documental"
                    Exit Function
                End If
                If name_modulo = "DOCUARCHI" And HttpContext.Current.Session.Item("UTIL_VER_DA_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de docuarchi"
                    Exit Function
                End If
                If name_modulo = "RADICACION" And HttpContext.Current.Session.Item("UTIL_VER_RA_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de radicación"
                    Exit Function
                End If
                If name_modulo = "CORRESPO" And HttpContext.Current.Session.Item("UTIL_VER_COR_REMPLAZA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    AdjuntaVersionDocumento = "El usuario no tiene permisos para remplazar la versión del documento en el modulo de gestión de correspodnecia"
                    Exit Function
                End If
                Dim Stru_paramter_image As stru_paramter_image = Nothing
                Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete,
                                                                         id_imagen,
                                                                         Stru_paramter_image,
                                                                         0,
                                                                         1,
                                                                         0,
                                                                         0)
                If Result <> "YES" Then
                    AdjuntaVersionDocumento = Result
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                    Select Case name_modulo
                        Case "WORKFLOW"
                            If HttpContext.Current.Session.Item("UTIL_VER_WF_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                                If UCase(Stru_paramter_image.USER) <> UCase(HttpContext.Current.Session.Item("DA_Login_Usuario")) Then
                                    AdjuntaVersionDocumento = "El usuario (" & HttpContext.Current.Session.Item("DA_Login_Usuario") & ") no es el propietario del documento en el gabinete (" & gabinete & ") imposible remplazar, el propietrario es (" & Stru_paramter_image.USER & ")"
                                    Exit Function
                                End If
                            End If
                        Case "SISTEMA"
                            'Pasa sin cnontrol
                        Case Else
                            If UCase(Stru_paramter_image.USER) <> UCase(HttpContext.Current.Session.Item("DA_Login_Usuario")) Then
                                AdjuntaVersionDocumento = "El usuario (" & HttpContext.Current.Session.Item("DA_Login_Usuario") & ") no es el propietario del documento en el gabinete (" & gabinete & ") imposible remplazar, el propietrario es (" & Stru_paramter_image.USER & ")"
                                Exit Function
                            End If
                    End Select
                End If
            End If
            Dim ClassNeodynamic As New ClassNeodynamic
            Dim file_inf As New FileInfo(ruta_documento)
            Dim Matri_documentos() As String
            Erase Matri_documentos
            If UCase(file_inf.Extension) = ".TIF" Then
                Result = ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(ruta_documento,
                                                                              Matri_documentos,
                                                                              HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    AdjuntaVersionDocumento = "Imposible extraer documento Multi tif "
                    Exit Function
                End If
                If Matri_documentos Is Nothing Then
                    AdjuntaVersionDocumento = "La matriz de multi tif esta Nothing "
                    Exit Function
                End If
            Else
                ReDim Preserve Matri_documentos(0)
                Matri_documentos(0) = ruta_documento
            End If
            Dim Class_system1 As New Class_system1
            Dim id_gabinete As Integer = 0
            Result = Class_system1.SolicitaIdGabineteDocuarchi(gabinete,
                                                               id_gabinete)
            If Result <> "YES" Then
                AdjuntaVersionDocumento = Result
                Exit Function
            End If
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Result = ClassAlmacenamiento.Almacena_documento_nueva_version(id_gabinete,
                                                                          gabinete,
                                                                          id_imagen,
                                                                          Matri_documentos,
                                                                          IdUsuarioGestion,
                                                                          IdUsuarioDocuarchi,
                                                                          LoginUsuarioGestion,
                                                                          LoginUsuarioDocuarchi,
                                                                          option_remplaza_gabinete,
                                                                          1,
                                                                          extension_documento,
                                                                          item_list)
            If Result <> "YES" Then
                AdjuntaVersionDocumento = Result
                Exit Function
            End If
            AdjuntaVersionDocumento = "YES"
        Catch ex As Exception
            AdjuntaVersionDocumento = "Inconsistencia general función AdjuntaVersionDocumento " & ex.Message
        End Try

    End Function
    Function Elimina_version_documento(ByVal id_registro_version As Long,
                                       ByVal tipo_modulo As Integer,
                                       ByVal id_usuario_gestion As Integer,
                                       ByVal login_usuario As String,
                                       ByVal elimina_permante As Integer,
                                       ByVal id_registro_migracion As Long,
                                       ByVal valida_firma_digital As Integer) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Funcion elimina version del documento  con la 
        '          identiifcación del registro de version y el tipo de modulo que despliega
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_registro_version   : Representa la identificación del registro de versión
        'tipo_modulo           : Representa el tipo de modulo que elimina el registro de 
        '                        version
        'id_usuario_gestion    : Representa la idnetiifcacion del usuario que realiza la oper
        '                        cion de eliminacion
        'login_usuario         : Representa el loguin de usuario que realiza la operacion
        'elimina_permante      : Representa la opcion de eliminar permante de la versión del
        '                        documento
        'id_registro_migracion : Representa el registro de migración
        'valida_firma_digital  : Representa la validación la no eliminación por firma digital
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        '
        '                                      
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------
        '                        ACTUALIZACION
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Actualiza             : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------
        'Se agrega la actualización del registros de migración para la eliminación de documen
        'tos desde la consulta de migración, se agrega el parametro id_registro_migracion
        '-----------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim modulo As String = ""
        Dim Stru_registro_version_documento As stru_registro_version_documento = Nothing
        If tipo_modulo = 1 And HttpContext.Current.Session.Item("UTIL_VER_MIG_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de migración"
            Exit Function
        End If
        If tipo_modulo = 1 Then
            modulo = "MIGRACION"
        End If
        If tipo_modulo = 2 And HttpContext.Current.Session.Item("UTIL_VER_WF_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de workflow"
            Exit Function
        End If
        If tipo_modulo = 2 Then
            modulo = "WORKFLOW"
        End If
        If tipo_modulo = 3 And HttpContext.Current.Session.Item("UTIL_VER_PR_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de producción documental"
            Exit Function
        End If
        If tipo_modulo = 3 Then
            modulo = "PRODUCCION"
        End If
        If tipo_modulo = 4 And HttpContext.Current.Session.Item("UTIL_VER_DA_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de docuarchi"
            Exit Function
        End If
        If tipo_modulo = 4 Then
            modulo = "DOCUARCHI"
        End If
        If tipo_modulo = 5 And HttpContext.Current.Session.Item("UTIL_VER_RA_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de radicación"
            Exit Function
        End If
        If tipo_modulo = 5 Then
            modulo = "RADICACION"
        End If
        If tipo_modulo = 6 And HttpContext.Current.Session.Item("UTIL_VER_COR_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de gestion de correspondencia"
            Exit Function
        End If
        If tipo_modulo = 7 And HttpContext.Current.Session.Item("UTIL_VER_CON_MIGRA_ELIMINA_VERSION_DOCUMENTO") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            Elimina_version_documento = "El usuario no tiene permisos para eliminar la versión del documento en el modulo de consulta registro migración"
            Exit Function
        End If
        If tipo_modulo = 6 Then
            modulo = "CORRESPO"
        End If
        If tipo_modulo = 7 Then
            modulo = "CONSULTA MIGRACION"
        End If
        If modulo = "" Then
            Elimina_version_documento = "Imposible encontrar el nombre del modulo "
            Exit Function
        End If
        Result = Me.Solicita_estructura_version_documento(id_registro_version,
                                                          Stru_registro_version_documento)
        If Result <> "YES" Then
            Elimina_version_documento = Result
            Exit Function
        End If
        If HttpContext.Current.Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0 Then
            If Stru_registro_version_documento.id_usuario_gestion <> id_usuario_gestion Then
                Elimina_version_documento = "El usuario no es propietario de la versión del documento imposible eliminar, usuario propietario (" & Stru_registro_version_documento.id_usuario_gestion & "-" & id_usuario_gestion & ")"
                Exit Function
            End If
        End If
        If Stru_registro_version_documento.ESTADO_ACTIVO_GABINETE = 1 Then
            Elimina_version_documento = "La versión del documento se encuentra activa en el gabinete imposible eliminar."
            Exit Function
        End If
        If Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL = 1 Then
            Elimina_version_documento = "La versión del documento se encuentra firmada digitalmente, imposible eliminar."
            Exit Function
        End If
        Dim Stru_paramter_image As stru_paramter_image = Nothing
        Dim ClassDaGabinete As New ClassDaGabinete
        Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Stru_registro_version_documento.nombre_gabinete,
                                                                 Stru_registro_version_documento.ID,
                                                                 Stru_paramter_image,
                                                                 1,
                                                                 0,
                                                                 1,
                                                                 1)
        If Result <> "YES" Then
            Elimina_version_documento = Result
            Exit Function
        End If
        If valida_firma_digital = 1 Then
            If Stru_paramter_image.ID <> 0 Then
                If Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL = 1 And Stru_paramter_image.ESTADO_FIRMA_DIGITAL = 0 Then
                    Elimina_version_documento = "Para eliminar una versión de documento con firmado digital o electrónico debe estar firmado el documento principal en el gabinete"
                    Exit Function
                End If
            End If
        End If
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim time1al As String = Date.Now.ToString
        ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim fecha_registro As String = time1al
        Dim Matriz_documentos() As String = Nothing
        Result = Solicita_matriz_documentos_version(Stru_registro_version_documento,
                                                    Matriz_documentos)
        If Result <> "YES" Then
            Elimina_version_documento = Result
            Exit Function
        End If
        If Matriz_documentos Is Nothing Then
            Elimina_version_documento = "Imposible encontrar la matriz de documentos "
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim myTrans As MySqlTransaction
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Elimina_version_documento = Result
            Exit Function
        End If
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim SQL_delete_version As String = ""
            If elimina_permante = 1 Then
                SQL_delete_version = "Delete from ra_ver_version_documento where id_registro_version=" & id_registro_version
            Else
                SQL_delete_version = "Update ra_ver_version_documento Set ESTADO_ELIMINA_VERSION=1 where id_registro_version=" & id_registro_version
            End If
            myCommand.CommandText = SQL_delete_version
            Dim Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_version_documento = "Imposible eliminar el registro de version  : " & SQL_delete_version
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza registro log de version 
            If elimina_permante = 0 Then
                Dim SQL_insert_log_version As String = "insert into ra_ver_log_version_documento (ra_ver_ver_docu_id_registro_version,id_usuario_gestion,loguin_usuario_gestion," &
               "id_imagen,id_gabinete, fecha_registro,transaccion,modulo) values (" & id_registro_version & "," & id_usuario_gestion & ",'" & login_usuario & "'," &
               Stru_registro_version_documento.ID & "," & Stru_registro_version_documento.system1_id_gabinete & ",'" & fecha_registro & "','CAMBIA  ESTADO ELIMINADO VERSION" & "','" & modulo & "')"
                myCommand.CommandText = SQL_insert_log_version
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Elimina_version_documento = "Imposible registrar el log de version   : " & SQL_insert_log_version
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Actualiza registro migración documento
            If id_registro_migracion <> 0 Then
                Dim SQL_update_registro_migracion As String = "update ra_mig_registro_migracion set id_registro_version_anterior=0," &
                    "fecha_registro_elimina_doc_fuente='" & fecha_registro & "',id_usuario_gestion_elimina_doc_fuente=" & id_usuario_gestion &
                    ",user_loguin_elimina_doc_fuente='" & login_usuario & "' where id_registro_migracion=" & id_registro_migracion
                myCommand.CommandText = SQL_update_registro_migracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Elimina_version_documento = "Imposible actualizar el registro de migración   : " & SQL_update_registro_migracion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If elimina_permante = 1 Then
                For i As Integer = 0 To Matriz_documentos.Length - 1
                    If File.Exists(Matriz_documentos(i)) Then
                        Kill(Matriz_documentos(i))
                    End If
                Next
            End If
            myTrans.Commit()
            Elimina_version_documento = "YES"
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Elimina_version_documento = "An exception of type " + ex.GetType().ToString() +
                                              " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_version_documento = "Error General " & e.Message
            Exit Function
        End Try
    End Function

    Function Load_visor_tiff_version(ByVal id_registro_version As Long,
                                     ByVal pag As Page,
                                     ByRef DropDownList_zom As DropDownList,
                                     ByRef UpdatePanelButon As UpdatePanel,
                                     ByRef Doc_actual As Integer,
                                     ByRef Matriz_documentos() As String) As String
        Try
            HttpContext.Current.Session.Item("VER_IMAGE_TEMPORAL_EMERGENTE") = ""
            HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = ""
            HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE") = "0"
            Dim Stru_registro_version_documento As stru_registro_version_documento = Nothing
            Dim Result As String = ""
            Result = Solicita_estructura_version_documento(id_registro_version,
                                                           Stru_registro_version_documento)
            If Result <> "YES" Then
                Load_visor_tiff_version = Result
                Exit Function
            End If
            Matriz_documentos = Nothing
            Result = Solicita_matriz_documentos_version(Stru_registro_version_documento,
                                                        Matriz_documentos)
            If Result <> "YES" Then
                Load_visor_tiff_version = Result
                Exit Function
            End If

            For i As Integer = 0 To Matriz_documentos.Length - 1
                If HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = "" Then
                    HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = Matriz_documentos(i)
                Else
                    HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") & "|" & Matriz_documentos(i)
                End If

            Next
            Result = Show_visor_tif_version_documento(Matriz_documentos,
                                                      Doc_actual,
                                                      "inicio",
                                                      0,
                                                      pag,
                                                      HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                      HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                      HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"),
                                                      HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                      HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                      DropDownList_zom,
                                                      UpdatePanelButon)
            If Result <> "YES" Then
                Load_visor_tiff_version = Result
                Exit Function
            End If
            Load_visor_tiff_version = "YES"
        Catch ex As Exception
            Load_visor_tiff_version = "Inconsistencia general funcion Load_visor_tiff_version " & ex.Message
        End Try
    End Function
    Function Escale_visor_tiff_version(ByRef Matri_Doc_Visual() As String,
                                       ByVal Escala As String,
                                       ByRef pag As Page,
                                       ByRef WF_DOC_ACTUAL As Object,
                                       ByRef WF_IMAGE_HEIHG As Object,
                                       ByRef WF_IMAGE_WITH As Object,
                                       ByRef drop_list As DropDownList,
                                       ByRef up_date As UpdatePanel) As String
        Try
            Dim EscaleHeigt As Integer = 0
            Dim EscaleWidth As Integer = 0
            If Escala = "+" Then
                If WF_IMAGE_HEIHG < 20 Or WF_IMAGE_HEIHG >= 100 Then
                    Escale_visor_tiff_version = "YES"
                    Exit Function
                End If
            End If
            If Escala = "-" Then
                If WF_IMAGE_HEIHG <= 20 Or WF_IMAGE_HEIHG > 100 Then
                    Escale_visor_tiff_version = "YES"
                    Exit Function
                End If
            End If
            Dim Doc_Actual As Integer = WF_DOC_ACTUAL
            If HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = 0 Then
                HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = WF_IMAGE_HEIHG
            Else
                WF_IMAGE_HEIHG = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
                WF_IMAGE_WITH = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
            End If
            If Escala = "+" Then

                EscaleHeigt = WF_IMAGE_HEIHG + 10
                EscaleWidth = WF_IMAGE_WITH + 10
            End If
            If Escala = "-" Then
                EscaleHeigt = WF_IMAGE_HEIHG - 10
                EscaleWidth = WF_IMAGE_WITH - 10
            End If
            If Escala = "x" Then
                EscaleHeigt = 30
                EscaleWidth = 30
            End If
            HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = EscaleHeigt
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim actio As Object = noami.Elements(0).Actions(0)
            actio.HeightPercentage = EscaleHeigt
            actio.WidthPercentage = EscaleWidth
            WF_IMAGE_HEIHG = EscaleHeigt
            WF_IMAGE_WITH = EscaleWidth
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            noami.Attributes.Add("zon_heig", actio.HeightPercentage)
            noami.Attributes.Add("zon_with", actio.WidthPercentage)
            For i As Integer = 0 To drop_list.Items.Count - 1
                If Val(drop_list.Items(i).Value) = WF_IMAGE_WITH Then
                    drop_list.Items(i).Selected = True
                    drop_list.Text = WF_IMAGE_WITH
                    Exit For
                End If
            Next
            Dim UpdatePanel_drows_bot As UpdatePanel = pag.FindControl("UpdatePanel_drows_bot")
            If Not UpdatePanel_drows_bot Is Nothing Then
                UpdatePanel_drows_bot.Update()
            End If
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            up_date.Update()
            If Not up Is Nothing Then
                up.Update()
            End If
            Escale_visor_tiff_version = "YES"
        Catch ex As Exception
            Escale_visor_tiff_version = "Funcion Escale_visor_tiff_version " & ex.Message
        End Try
    End Function
    Function Escale_visor_tiff_version_drow_list(ByRef Matri_Doc_Visual() As String,
                                                ByVal Escala As Integer,
                                                ByRef pag As Page,
                                                ByRef WF_DOC_ACTUAL As Object,
                                                ByRef WF_IMAGE_HEIHG As Object,
                                                ByRef WF_IMAGE_WITH As Object,
                                                ByRef drop_list As DropDownList,
                                                ByRef up_date As UpdatePanel) As String
        Try
            HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = Escala
            Dim EscaleHeigt As Integer = Escala
            Dim EscaleWidth As Integer = Escala
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim actio As Object = noami.Elements(0).Actions(0)
            actio.HeightPercentage = EscaleHeigt
            actio.WidthPercentage = EscaleWidth
            WF_IMAGE_HEIHG = EscaleHeigt
            WF_IMAGE_WITH = EscaleWidth
            Dim lab As Label = pag.FindControl("Labeldatos")
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            noami.Attributes.Add("zon_heig", actio.HeightPercentage)
            noami.Attributes.Add("zon_with", actio.WidthPercentage)
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = actio.HeightPercentage Then
                    drop_list.Items(i).Selected = True
                    drop_list.Text = actio.HeightPercentage.ToString
                    Exit For
                End If
            Next
            up_date.Update()
            up.Update()
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Escale_visor_tiff_version_drow_list = "YES"
        Catch ex As Exception
            Escale_visor_tiff_version_drow_list = "Funcion Escale_visor_tiff_version_drow_list " & ex.Message
        End Try
    End Function
    Function Show_visor_tif_version_documento(ByRef Matri_Doc_Visual() As String,
                                              ByRef Doc_actual1 As Integer,
                                              ByVal Naveg As String,
                                              ByVal Pag_Selec As Integer,
                                              ByRef pag As Page,
                                              ByRef WF_DOC_ACTUAL As Object,
                                              ByRef WF_IMAGE_HEIHG As Object,
                                              ByRef WF_IMAGE_WITH As Object,
                                              ByRef WF_IMAGE_HEIHG_SIZE As Object,
                                              ByRef WF_IMAGE_WITH_SIZE As Object,
                                              ByRef drop_list As DropDownList,
                                              ByRef up_date As UpdatePanel) As String

        Try
            Dim Doc_Actual As Integer = Val(WF_DOC_ACTUAL)
            Select Case Naveg
                Case "+1"
                    If (Matri_Doc_Visual.Length - 1) = Doc_Actual Or Doc_Actual > (Matri_Doc_Visual.Length - 1) Then
                        Show_visor_tif_version_documento = "YES"
                        Exit Function
                    Else
                        Doc_Actual = Doc_Actual + 1
                    End If
                Case "-1"
                    If Doc_Actual = 0 Then
                        Show_visor_tif_version_documento = "YES"
                        Exit Function
                    Else
                        Doc_Actual = Doc_Actual - 1
                    End If
                Case "inicio"
                    Doc_Actual = 0
                Case "final"
                    Doc_Actual = Matri_Doc_Visual.Length - 1
                Case "seleccion"
                    If Pag_Selec > Matri_Doc_Visual.Length - 1 Then
                        Show_visor_tif_version_documento = "YES"
                        Exit Function
                    End If
                    If Pag_Selec < 1 Then
                        Show_visor_tif_version_documento = "YES"
                        Exit Function
                    End If
                    Doc_Actual = Pag_Selec
            End Select
            WF_DOC_ACTUAL = Doc_Actual
            Dim noami As Object = pag.FindControl("noaming")
            If noami Is Nothing Then
                Show_visor_tif_version_documento = "Imposible encontrar noami"
                Exit Function
            End If
            Dim imgElem As New Neodynamic.WebControls.ImageDraw.ImageElement
            If noami.Elements.Count > 0 Then
                Dim elemen As ImageElement = noami.Elements(0)
                elemen.SourceFile = Matri_Doc_Visual(Doc_Actual)
                Dim actio As Object = noami.Elements(0).Actions(0)
                If HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") <> 0 Then
                    actio.HeightPercentage = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
                    actio.WidthPercentage = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
                Else
                    HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = actio.HeightPercentage
                End If
                noami.Attributes.Add("zon_heig", actio.HeightPercentage)
                noami.Attributes.Add("zon_with", actio.WidthPercentage)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = actio.HeightPercentage Then
                        drop_list.Items(i).Selected = True
                        drop_list.Text = actio.HeightPercentage.ToString
                        Exit For
                    End If
                Next
                HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = actio.HeightPercentage.ToString
                up_date.Update()
            Else
                imgElem.SourceFile = Matri_Doc_Visual(Doc_Actual)
                Dim rotate As New Neodynamic.WebControls.ImageDraw.Scale
                If HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") <> 0 Then
                    WF_IMAGE_HEIHG = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
                    WF_IMAGE_WITH = HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF")
                Else
                    WF_IMAGE_HEIHG = 50
                    WF_IMAGE_WITH = 50
                    HttpContext.Current.Session.Item("VER_ZOON_VISOR_WEB_TIF") = WF_IMAGE_HEIHG
                End If
                rotate.HeightPercentage = WF_IMAGE_HEIHG
                rotate.WidthPercentage = WF_IMAGE_WITH
                imgElem.Actions.Add(rotate)
                noami.Elements.Add(imgElem)
                Dim actio As Object = noami.Elements(0).Actions(0)
                noami.Attributes.Add("zon_heig", actio.HeightPercentage)
                noami.Attributes.Add("zon_with", actio.WidthPercentage)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = actio.HeightPercentage Then
                        drop_list.Items(i).Selected = True
                        drop_list.Text = actio.HeightPercentage.ToString
                        Exit For
                    End If
                Next
                WF_IMAGE_HEIHG_SIZE = actio.HeightPercentage
                WF_IMAGE_WITH_SIZE = actio.WidthPercentage
                up_date.Update()
            End If
            Dim lab As TextBox = pag.FindControl("LabelConteo")
            If Not lab Is Nothing Then
                lab.Text = Doc_Actual + 1 & "/" & Matri_Doc_Visual.Length
            End If
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            If Not up Is Nothing Then
                up.Update()
            End If
            Dim UpdatePanel_conte_bot As Object = pag.FindControl("UpdatePanel_conte_bot")
            If Not UpdatePanel_conte_bot Is Nothing Then
                UpdatePanel_conte_bot.Update()
            End If
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Show_visor_tif_version_documento = "YES"
        Catch ex As Exception
            Show_visor_tif_version_documento = "Error General Funcion : Show_visor_tif_version_documento Descrip Error : " & ex.Message
        End Try
    End Function
    Function Descarga_version_documento(ByVal id_registro_version As Long,
                                        ByRef class_stru_visor_migracion As class_stru_visor_migracion) As String
        '---------------------------------------------------------------------------
        'Funcion : Funcion que solicita la url de descarga de documentos
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               version del documento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de
        '                             visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Stru_registro_version_documento As stru_registro_version_documento = Nothing
            Dim Class_fyle_system As New Class_fyle_system
            Dim Result As String = ""
            Result = Solicita_estructura_version_documento(id_registro_version,
                                                           Stru_registro_version_documento)
            If Result <> "YES" Then
                Descarga_version_documento = Result
                Exit Function
            End If
            Dim ruta_temp As String = HttpContext.Current.Session.Item("GA_RUTA_FIRMA_GESTION")
            ruta_temp = ruta_temp & "\donwload_version"
            Dim ruta_temp_fuera As String = ruta_temp & "\"
            If Directory.Exists(ruta_temp) = False Then
                Directory.CreateDirectory(ruta_temp)
            End If
            ruta_temp = ruta_temp & "\" & id_registro_version & "\"
            Dim Matriz_documentos() As String = Nothing
            Result = Solicita_matriz_documentos_version(Stru_registro_version_documento,
                                                        Matriz_documentos)
            If Result <> "YES" Then
                Descarga_version_documento = Result
                Exit Function
            End If
            If Stru_registro_version_documento.TIPO_ARCHIVO = ".TIF" Or Stru_registro_version_documento.TIPO_ARCHIVO = ".JPG" Or Stru_registro_version_documento.TIPO_ARCHIVO = ".BMP" Then
                Dim Ruta_file_zip_version As String = ""
                Result = Class_fyle_system.Add_zip_version_documento(id_registro_version,
                                                                     Matriz_documentos,
                                                                     ruta_temp,
                                                                     ruta_temp_fuera,
                                                                     Ruta_file_zip_version)
                If Result <> "YES" Then
                    Descarga_version_documento = Result
                    Exit Function
                End If
                Dim file_inf As New FileInfo(Ruta_file_zip_version)
                class_stru_visor_migracion.name_file = file_inf.Name
                class_stru_visor_migracion.url_iframe = "../workflow/Handler_image_wf.ashx?rut_image=" & Ruta_file_zip_version
                Descarga_version_documento = "YES"
                Exit Function
            Else
                Dim file_inf As New FileInfo(Matriz_documentos(0))
                class_stru_visor_migracion.name_file = "file_version_" & id_registro_version & file_inf.Extension
                class_stru_visor_migracion.url_iframe = "../workflow/Handler_image_wf.ashx?rut_image=" & Matriz_documentos(0)
                Descarga_version_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Descarga_version_documento = "Inconsistencia general  Descarga_version_documento " & ex.Message
        End Try
    End Function
    Function Solicita_documentos_visor_version(ByVal id_registro_version As Long,
                                               ByRef class_stru_visor_migracion As class_stru_visor_migracion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de visuzalizacion de documentos
        '          de version para visualziacion
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               version del documento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Stru_registro_version_documento As stru_registro_version_documento = Nothing
            Dim Result As String = ""
            Result = Solicita_estructura_version_documento(id_registro_version,
                                                           Stru_registro_version_documento)
            If Result <> "YES" Then
                Solicita_documentos_visor_version = Result
                Exit Function
            End If
            If Stru_registro_version_documento.TIPO_ARCHIVO = ".TIF" Or Stru_registro_version_documento.TIPO_ARCHIVO = ".JPG" Or Stru_registro_version_documento.TIPO_ARCHIVO = ".BMP" Then
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorVersion.aspx"
                HttpContext.Current.Session.Item("VER_ID_REGISTRO_VERSION") = id_registro_version
                Solicita_documentos_visor_version = "YES"
                Exit Function
            Else
                Dim Matriz_documentos() As String = Nothing
                Result = Solicita_matriz_documentos_version(Stru_registro_version_documento,
                                                            Matriz_documentos)
                If Result <> "YES" Then
                    Solicita_documentos_visor_version = Result
                    Exit Function
                End If
                class_stru_visor_migracion.url_iframe = "../workflow/Handler_image_wf.ashx?rut_image=" & Matriz_documentos(0)
                Solicita_documentos_visor_version = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_documentos_visor_version = "Incosistencia general funcion Solicita_documentos_visor_version " & ex.Message
        End Try
    End Function
    Function Restaura_version_documento_gabinete(ByVal id_registro_version As Long,
                                                 ByVal tipo_modulo As Integer,
                                                 ByVal id_usuario_gestion As Integer,
                                                 ByVal login_usuario As String,
                                                 ByRef class_result_list_detalle_version_document As class_result_list_detalle_version_document) As String

        '--------------------------------------------------------------------------------
        'Funcion : función que restaura la version de un documento en el gabinete
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_version   : Representa la identificación del registro de la versión
        '                        del documento a restuarar como documento en el gabinete.
        'tipo_modulo           : Representa el tipo de modulo que realiza la restauración.
        'id_usuario_gestion    : Representa la identificación del usuario de gestión 
        'login_usuario         : Representa el loguin del usuario de gestión.
        '                        
        '
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Extension                  : Retorna la extensión del archivo de remplazo
        'id_registro_version_activo : Retorna el registro de versión activo a restaurar
        'id_imagen                  : Retorna la identificación de la imagen a restaurar
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-15
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim modulo As String = ""
        Dim id_registro_version_activo As Long = 0
        Dim Stru_registro_version_documento As stru_registro_version_documento = Nothing
        If tipo_modulo = 1 And HttpContext.Current.Session.Item("UTIL_VER_MIG_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos de restaurar el documento en el gabinete en el modulo de migración"
            Exit Function
        End If
        If tipo_modulo = 1 Then
            modulo = "MIGRACION"
        End If
        '-----///Caso remplaza documento workflow-----////
        If tipo_modulo = 2 And HttpContext.Current.Session.Item("UTIL_VER_WF_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos de restaurar el documento en el gabinete en el modulo de workflow"
            Exit Function
        End If
        If tipo_modulo = 2 Then
            modulo = "WORKFLOW"
        End If
        If tipo_modulo = 3 And HttpContext.Current.Session.Item("UTIL_VER_PR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos de restaurar el documento en el gabinete en el modulo de producción documental"
            Exit Function
        End If
        If tipo_modulo = 3 Then
            modulo = "PRODUCCION"
        End If
        If tipo_modulo = 4 And HttpContext.Current.Session.Item("UTIL_VER_DA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos de restaurar el documento en el gabinete en el modulo de docuarchi"
            Exit Function
        End If
        If tipo_modulo = 4 Then
            modulo = "DOCUARCHI"
        End If
        If tipo_modulo = 5 And HttpContext.Current.Session.Item("UTIL_VER_RA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos de restaurar el documento en el gabinete en el modulo de radicación"
            Exit Function
        End If
        If tipo_modulo = 5 Then
            modulo = "RADICACION"
        End If
        If tipo_modulo = 6 And HttpContext.Current.Session.Item("UTIL_VER_COR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 And HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Restaura_version_documento_gabinete = "El usuario no tiene permisos para restaurar la versión del documento en el modulo de gestion de correspondencia"
            Exit Function
        End If
        If tipo_modulo = 6 Then
            modulo = "CORRESPO"
        End If
        If modulo = "" Then
            Restaura_version_documento_gabinete = "Imposible encontrar el modulo para realizar el proceso de restauración"
            Exit Function
        End If
        Result = Me.Solicita_estructura_version_documento(id_registro_version,
                                                          Stru_registro_version_documento)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        If HttpContext.Current.Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0 Then
            Select Case modulo
                Case "WORKFLOW"
                    If HttpContext.Current.Session.Item("UTIL_VER_WF_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0 Then
                        If Stru_registro_version_documento.id_usuario_gestion <> id_usuario_gestion Then
                            Restaura_version_documento_gabinete = "El usuario (" & HttpContext.Current.Session.Item("DA_Login_Usuario") & ") no es el propietario del documento en el gabinete (" & Stru_registro_version_documento.nombre_gabinete & ") imposible remplazar el documento, teniendo en cuenta que el propietrario es (" & Stru_registro_version_documento.USER_DA & ")"
                            Exit Function
                        End If
                    End If
                Case Else
                    If Stru_registro_version_documento.id_usuario_gestion <> id_usuario_gestion Then
                        Restaura_version_documento_gabinete = "El usuario (" & HttpContext.Current.Session.Item("DA_Login_Usuario") & ") no es el propietario del documento en el gabinete (" & Stru_registro_version_documento.nombre_gabinete & ") imposible remplazar el documento, teniendo en cuenta que el propietrario es (" & Stru_registro_version_documento.USER_DA & ")"
                        Exit Function
                    End If
            End Select

        End If
        If Stru_registro_version_documento.ESTADO_ACTIVO_GABINETE = 1 Then
            Restaura_version_documento_gabinete = "La versión del documento se encuentra activa en el gabinete imposible restaurar"
            Exit Function
        End If
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Dim id_registro_producion As Long = 0
        Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(Stru_registro_version_documento.ID,
                                                                                      Stru_registro_version_documento.nombre_gabinete,
                                                                                      id_registro_producion)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        class_result_list_detalle_version_document.imagen = Stru_registro_version_documento.ID
        Dim Stru_produccion_indice As stru_produccion_indice = Nothing
        Dim Stru_expediente() As expediente_conservacion = Nothing
        Dim Ruta_archivo_xml As String = ""
        Dim ClassGaExpediente As New ClassGaExpediente
        If id_registro_producion <> 0 Then
            Result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(id_registro_producion,
                                                                                           Stru_produccion_indice)
            If Result <> "YES" Then
                Restaura_version_documento_gabinete = Result
                Exit Function
            End If
            If Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE <> 0 Then
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                             Stru_expediente)
                If Result <> "YES" Then
                    Restaura_version_documento_gabinete = Result
                    Exit Function
                End If
                If Stru_expediente(0).ESTADO_EXPEDIENTE = 2 Then
                    Restaura_version_documento_gabinete = "El expediente (" & Stru_expediente(0).CODIGO_UNICO & ") vinculante de este documento se encuentra cerrado imposible restaurar la versión del documento en el gabinete"
                    Exit Function
                End If
                Result = ClassGaExpediente.Solicita_archivo_indice_expediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                              Ruta_archivo_xml)
                If Result <> "YES" Then
                    Restaura_version_documento_gabinete = Result
                    Exit Function
                End If
                If File.Exists(Ruta_archivo_xml) = False Then
                    Restaura_version_documento_gabinete = "Imposible encontrar el archivo indice del expediente (" & Ruta_archivo_xml & ")"
                    Exit Function
                End If
            End If
        End If
        Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Dim _Ruta_Almacenamiento As String = ""
        Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                               Stru_registro_version_documento.nombre_gabinete)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        '--------Consolida ruta de carpeta de almacenamiento
        Dim ClassAlmacenamiento As New ClassAlmacenamiento
        Dim RutaDisco As String = _Ruta_Almacenamiento & Stru_registro_version_documento.nombre_gabinete & Stru_registro_version_documento.DISC
        Dim carpealma As String = ""
        Dim ruta_almacenamiento As String = ""
        Result = ClassAlmacenamiento.Solicita_Carpeta_almacenamiento(carpealma,
                                                                     Stru_registro_version_documento.IDEX,
                                                                     ruta_almacenamiento,
                                                                     RutaDisco)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        Dim ZeroFillImagen As String = ""
        Dim NameNEWarchivo As String = ""
        Result = Ceros_Imagen_Almacenada(ZeroFillImagen,
                                         Stru_registro_version_documento.ID)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = "Error generando ZerroFill imagen " & Result
            Exit Function
        End If
        '-------Valida que este registra la version del documento del gabinete
        Result = Solicita_registro_activo_gabinete(Stru_registro_version_documento.ID,
                                                   Stru_registro_version_documento.system1_id_gabinete,
                                                   id_registro_version_activo)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        class_result_list_detalle_version_document.id_registro_version = id_registro_version_activo
        Dim Stru_paramter_image As stru_paramter_image = Nothing
        Dim ClassDaGabinete As New ClassDaGabinete
        Dim ID_rem_version As Integer = 0
        Dim DISC_rem_version As Integer = 0
        Dim PAG_rem_version As Integer = 0
        Dim DBT_rem_version As Integer = 0
        Dim IDEX_rem_version As Integer = 0
        Dim USER_DA_rem_version As String = ""
        Dim CTRL_ACES_rem_version As Integer = 0
        Dim PESO_DOCUMENTO_rem_version As String = ""
        Dim TIPO_ARCHIVO_rem_version As String = ""
        Dim ID_REG_MIGRA_rem_version As Long = 0
        Dim ID_REGISTRO_VERSION_rem_version As Integer = 0
        Dim ID_VERSION_DOC_rem_version As Integer = 0
        Dim PAGINA_DOCUMENT_rem_version As Integer = 0
        Dim ESTADO_FIRMA_DIGITAL_rem_version As Integer = 0
        Dim DATE_rem_version As String = ""
        Dim datetemp As String = ""
        Dim ClassGestionFechas As New ClassGestionFechas
        '///Solicita icono del documento
        Dim ItemIlit As New class_list_detalle_version_document
        If Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL = 1 Then
            ItemIlit.IconoAsome = "fa-file-certificate"
            class_result_list_detalle_version_document.ILIST_lista_detalle_version_document.Add(ItemIlit)
        Else
            ClassDaGabinete.Agrega_icono_image_fownt_extension_cort(Stru_registro_version_documento.TIPO_ARCHIVO,
                                                                    ItemIlit.IconoAsome)
            class_result_list_detalle_version_document.ILIST_lista_detalle_version_document.Add(ItemIlit)
        End If
        If id_registro_version_activo = 0 Then
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Stru_registro_version_documento.nombre_gabinete,
                                                                     Stru_registro_version_documento.ID,
                                                                     Stru_paramter_image,
                                                                     1,
                                                                     1,
                                                                     1,
                                                                     1)
            If Result <> "YES" Then
                Restaura_version_documento_gabinete = Result
                Exit Function
            End If
            datetemp = Stru_paramter_image.DATE1 & " " & Stru_paramter_image.TIME1
            ClassGestionFechas.Formatea_fecha_time_db(datetemp, DATE_rem_version)
            ID_rem_version = class_result_list_detalle_version_document.imagen
            DISC_rem_version = Stru_paramter_image.DISC
            PAG_rem_version = Stru_paramter_image.PAG
            DBT_rem_version = Stru_paramter_image.DBT_TIPO_IMAGEN
            IDEX_rem_version = Stru_paramter_image.IDEX
            USER_DA_rem_version = Stru_paramter_image.USER
            CTRL_ACES_rem_version = Stru_paramter_image.CTRL_ACES
            ID_REGISTRO_VERSION_rem_version = Stru_paramter_image.ID_REGISTRO_VERSION
            ID_VERSION_DOC_rem_version = Stru_paramter_image.ID_VERSION_DOC
            ESTADO_FIRMA_DIGITAL_rem_version = Stru_paramter_image.ESTADO_FIRMA_DIGITAL
            Dim matri_documemtos_gabinete() As String = Nothing
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(Stru_registro_version_documento.ID,
                                                                                     Stru_registro_version_documento.nombre_gabinete,
                                                                                     matri_documemtos_gabinete)
            If Result <> "YES" Then
                Restaura_version_documento_gabinete = Result
                Exit Function
            End If
            Dim ref_matri_documento() As String = Nothing
            Dim Icont As Integer = 0
            For i As Integer = 1 To matri_documemtos_gabinete.Length - 1
                ReDim Preserve ref_matri_documento(Icont)
                ref_matri_documento(Icont) = matri_documemtos_gabinete(i)
                Icont = Icont + 1
            Next
            '----------Solicita peso documento gabinete a remplazar
            Dim Class_fyle_system As New Class_fyle_system
            Result = Class_fyle_system.Solicita_peso_matriz_documentos(ref_matri_documento,
                                                                       PESO_DOCUMENTO_rem_version)
            If Result <> "YES" Then
                Restaura_version_documento_gabinete = Result
                Exit Function
            End If
            If Stru_paramter_image.DBT_TIPO_IMAGEN = -10 Then
                PAGINA_DOCUMENT_rem_version = ref_matri_documento.Length - 1
            Else
                PAGINA_DOCUMENT_rem_version = Stru_paramter_image.PAG
            End If
            '----------Solicita la extension o tipo documento del gabinete
            Dim file_inf_rem As New FileInfo(ref_matri_documento(0))
            TIPO_ARCHIVO_rem_version = UCase(file_inf_rem.Extension)
        End If
        NameNEWarchivo = "DIG" & ZeroFillImagen & Stru_registro_version_documento.ID & Stru_registro_version_documento.TIPO_ARCHIVO
        Dim time1al As String = Date.Now.ToString
        ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim fecha_registro As String = time1al
        Dim myConnection As New MySqlConnection
        Dim myTrans As MySqlTransaction
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Restaura_version_documento_gabinete = Result
            Exit Function
        End If
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Switc As Integer = 0
            '----------Valida exitencia registro activo en gabinete
            If id_registro_version_activo <> 0 Then
                '------Inactiva la versión del gabinete como versión activa 
                Dim SQL_inactiva_version_gabinete As String = "UPDATE ra_ver_version_documento SET ESTADO_ACTIVO_GABINETE=0 " &
                  " WHERE system1_id_gabinete=" & Stru_registro_version_documento.system1_id_gabinete & " and ID=" & Stru_registro_version_documento.ID & " and ESTADO_ACTIVO_GABINETE=1"
                myCommand.CommandText = SQL_inactiva_version_gabinete
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Restaura_version_documento_gabinete = "Imposible inactivar las activas en el gabinete  : " & SQL_inactiva_version_gabinete
                    myConnection.Close()
                    Exit Function
                End If
            Else
                Dim SQL_insert_version_new As String = "Insert into ra_ver_version_documento (system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion," &
               "fecha_registro_version,nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
               "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE) VALUES (" & Stru_registro_version_documento.system1_id_gabinete & "," & "0" & ",'" & fecha_registro & "','" & Stru_registro_version_documento.nombre_gabinete &
               "'," & 1 & "," & HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") & "," & id_usuario_gestion & "," & ID_rem_version & "," & DISC_rem_version & "," &
               PAG_rem_version & "," & DBT_rem_version & "," & IDEX_rem_version & ",'" & USER_DA_rem_version & "'," & CTRL_ACES_rem_version &
               ",'" & PESO_DOCUMENTO_rem_version & "','" & TIPO_ARCHIVO_rem_version & "'," & PAGINA_DOCUMENT_rem_version & "," & ESTADO_FIRMA_DIGITAL_rem_version & "," & 0 & ")"
                myCommand.CommandText = SQL_insert_version_new
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Restaura_version_documento_gabinete = "Imposible registrar la version del documento a remplazar  : " & SQL_insert_version_new
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Actualiza registro de gabinete
            Dim SQL_update_registro_gabinete As String = "update " & Stru_registro_version_documento.nombre_gabinete & " set DISC=" & Stru_registro_version_documento.DISC & " , PAG=" & Stru_registro_version_documento.PAG &
                " , DBT=" & Stru_registro_version_documento.DBT & " , IDEX=" & Stru_registro_version_documento.IDEX &
                " , ID_REGISTRO_VERSION=" & id_registro_version & " ,ID_VERSION_DOC=" & Stru_registro_version_documento.id_version_doc &
                " , ESTADO_FIRMA_DIGITAL=" & Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL &
                " where ID=" & Stru_registro_version_documento.ID
            myCommand.CommandText = SQL_update_registro_gabinete
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Restaura_version_documento_gabinete = "Imposible actualizar el registro de gabinete   : " & SQL_update_registro_gabinete
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza estado activo documento en el gagbinete
            Dim SQL_update_activo_documento As String = "UPDATE ra_ver_version_documento SET ESTADO_ACTIVO_GABINETE=1 " &
                " where id_registro_version=" & id_registro_version
            myCommand.CommandText = SQL_update_activo_documento
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Restaura_version_documento_gabinete = "Imposible activar en el registro de versión el documento activo en el gabinete   : " & SQL_update_activo_documento
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza registro produccion
            If id_registro_producion <> 0 Then
                Dim SQL_actualiza_registro_produccion As String = "update registro_producion_documental set FORMATO='" & Stru_registro_version_documento.TIPO_ARCHIVO & "' , " &
               "TAMANO='" & Stru_registro_version_documento.PESO_DOCUMENTO & "', ESTADO_FIRMA_DIGITAL=" & Stru_registro_version_documento.ESTADO_FIRMA_DIGITAL & " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                myCommand.CommandText = SQL_actualiza_registro_produccion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Restaura_version_documento_gabinete = "Imposible actualizar el registro de produccion documental   : " & SQL_actualiza_registro_produccion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Actualiza indice base de datos expediente
            Dim SQL_update_actualiza_indice_expediente As String = ""
            Dim Ruta_Alamce_Image As String = ruta_almacenamiento & "\" & NameNEWarchivo
            Dim Ruta_indice_documento As String = Ruta_Alamce_Image
            Dim file_inf_name As New FileInfo(Ruta_Alamce_Image)
            Ruta_indice_documento = Ruta_indice_documento.Replace("\", "/")
            class_result_list_detalle_version_document.extension_archivo = Stru_registro_version_documento.TIPO_ARCHIVO
            If id_registro_producion <> 0 Then
                If Not Stru_expediente Is Nothing Then
                    If Stru_expediente(0).estado_expediente_electronico = 2 Then
                        SQL_update_actualiza_indice_expediente = "update ra_cert_indice_expediente set formato='" & Stru_registro_version_documento.TIPO_ARCHIVO & "' , " &
                        "dimension_kb='" & Stru_registro_version_documento.PESO_DOCUMENTO & "' , ruta_documento='" & Ruta_indice_documento & "' " &
                        " , Nombre_documento='" & file_inf_name.Name & "' " &
                        " where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                        myCommand.CommandText = SQL_update_actualiza_indice_expediente
                        Switc = myCommand.ExecuteNonQuery()
                        If Switc = 0 Then
                            Restaura_version_documento_gabinete = "Imposible actualizar el registro del indice del expediente   : " & SQL_update_actualiza_indice_expediente
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        Dim Stru_values_cambio_indice() As ClassGaExpediente.stru_values_cambio_indice
                        ReDim Preserve Stru_values_cambio_indice(0)
                        Stru_values_cambio_indice(0).clave_index = "Formato"
                        Stru_values_cambio_indice(0).value_index = Stru_registro_version_documento.TIPO_ARCHIVO
                        ReDim Preserve Stru_values_cambio_indice(1)
                        Stru_values_cambio_indice(1).clave_index = "Tamano"
                        Stru_values_cambio_indice(1).value_index = Stru_registro_version_documento.PESO_DOCUMENTO
                        ReDim Preserve Stru_values_cambio_indice(2)
                        Stru_values_cambio_indice(2).clave_index = "Nombre_Documento"
                        Stru_values_cambio_indice(2).value_index = file_inf_name.Name
                        Result = ClassGaExpediente.Actualiza_campos_indice_expediente_xml_expediente(Ruta_archivo_xml,
                                                                                                     id_registro_producion,
                                                                                                     Stru_values_cambio_indice)
                        If Result <> "YES" Then
                            Restaura_version_documento_gabinete = "Imposible actualizar el archivo del indice del expediente   : " & Result
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                    End If
                End If
            End If
            'Actualiza registro log de version 
            Dim SQL_insert_log_version As String = "insert into ra_ver_log_version_documento (ra_ver_ver_docu_id_registro_version,id_usuario_gestion,loguin_usuario_gestion," &
                "id_imagen,id_gabinete, fecha_registro,transaccion,modulo) values (" & id_registro_version & "," & id_usuario_gestion & ",'" & login_usuario & "'," &
                Stru_registro_version_documento.ID & "," & Stru_registro_version_documento.system1_id_gabinete & ",'" & fecha_registro & "','RESTAURA VERSION DE DOCUMENTO EN GABINETE" & "','" & modulo & "')"
            myCommand.CommandText = SQL_insert_log_version
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Restaura_version_documento_gabinete = "Imposible registrar el log de version   : " & SQL_insert_log_version
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            Restaura_version_documento_gabinete = "YES"
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Restaura_version_documento_gabinete = "An exception of type " + ex.GetType().ToString() +
                                              " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Restaura_version_documento_gabinete = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_matriz_documentos_version(ByVal Stru_registro_version_documento As stru_registro_version_documento,
                                                ByRef Matriz_documentos() As String) As String
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim Ruta_Busqueda As String = ""
            Dim Numero_Doc_Añadidos As Integer = 0
            Dim Numero_Doc_Principal As Integer = 0
            If Stru_registro_version_documento.DBT = 1 Or
            Stru_registro_version_documento.DBT = -10 Or
            Stru_registro_version_documento.DBT = -20 Or
            Stru_registro_version_documento.DBT = -30 Or
            Stru_registro_version_documento.DBT = -40 _
            Then
                Result = ClassDaGabinete.Suma_Numero_Documentos_Añadidos(Stru_registro_version_documento.ID,
                                                                         Stru_registro_version_documento.nombre_gabinete,
                                                                         Numero_Doc_Añadidos)
                If Result <> "YES" Then
                    Solicita_matriz_documentos_version = Result
                    Exit Function
                End If
                Numero_Doc_Principal = Stru_registro_version_documento.PAG
            Else
                Numero_Doc_Principal = Stru_registro_version_documento.PAG
            End If
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Result = Class_SYSTEM1RUT.Consulta_Ruta_Busqueda_Webservice(Ruta_Busqueda,
                                                                        Stru_registro_version_documento.nombre_gabinete)
            If Result <> "YES" Then
                Solicita_matriz_documentos_version = Result
                Exit Function
            End If
            Result = Genera_matriz_documentos_version(Stru_registro_version_documento,
                                                      Ruta_Busqueda,
                                                      Numero_Doc_Principal,
                                                      Stru_registro_version_documento.nombre_gabinete,
                                                      0,
                                                      Matriz_documentos)
            If Result <> "YES" Then
                Solicita_matriz_documentos_version = Result
                Exit Function
            End If
            '*******************************************
            'Determina si tiene documentos añadidos
            '*******************************************
            If Numero_Doc_Añadidos = 0 Then
                Solicita_matriz_documentos_version = "YES"
                Exit Function
            End If
            '*******************************************
            'Genera matriz datos doc añadidos
            '*******************************************
            Dim Matri_Dat_Añadidos() As Datos_Registro
            Erase Matri_Dat_Añadidos
            If Stru_registro_version_documento.DBT = 1 Or
            Stru_registro_version_documento.DBT = -10 Or
            Stru_registro_version_documento.DBT = -20 Or
            Stru_registro_version_documento.DBT = -30 Or
            Stru_registro_version_documento.DBT = -40 _
            Then
                Result = ClassDaGabinete.Consulta_Documentos_Añadidos(Stru_registro_version_documento.ID,
                                                                      Stru_registro_version_documento.nombre_gabinete,
                                                                      Matri_Dat_Añadidos)
                If Result <> "YES" Then
                    Solicita_matriz_documentos_version = "Error buscando documentos añadidos " & Result
                    Exit Function
                End If
                If Matri_Dat_Añadidos Is Nothing Then
                    Solicita_matriz_documentos_version = "YES"
                    Exit Function
                End If
                '*************************************************
                'Genera matriz documentos añadidos
                '*************************************************
                For z As Integer = 0 To UBound(Matri_Dat_Añadidos)
                    Result = ""
                    Result = ClassDaGabinete.Genera_Matriz_Documentos(Matriz_documentos,
                                                                      Matri_Dat_Añadidos(z).Id,
                                                                      Stru_registro_version_documento.DBT,
                                                                      Ruta_Busqueda,
                                                                      Matri_Dat_Añadidos,
                                                                      Matri_Dat_Añadidos(z).Pag,
                                                                      Stru_registro_version_documento.nombre_gabinete,
                                                                      z)
                    If Result <> "YES" Then
                        Solicita_matriz_documentos_version = "Error Generando matriz doc añadidos " & Result
                        Exit Function
                    End If
                Next
            End If
            Solicita_matriz_documentos_version = "YES"
        Catch ex As Exception
            Solicita_matriz_documentos_version = "Inconsistencia general funcion Solicita_matriz_documentos_version " & ex.Message
        End Try
    End Function
    Function Genera_matriz_documentos_version(ByVal Stru_registro_version_documento As stru_registro_version_documento,
                                              ByVal Ruta_Documento As String,
                                              ByVal Numero_Documentos As Integer,
                                              ByVal Nombre_Gabinete As String,
                                              ByVal Indice_Matri_Datos As Integer,
                                              ByRef Matri_Documentos() As String) As String


        Try
            Dim Ceros_Cuerpo_Imag As String = ""
            Dim Result As String = ""
            Dim i As Integer = 0
            Dim i2 As Integer = 0
            Dim Ceros_Ext As String = ""
            Dim Imagen_Principal As String = ""
            Dim Carpeta_Documento As String = ""
            Dim Disco_Documento As String = ""
            Dim Ceros_Carpeta As String = ""
            Dim Icremento_Matri As Integer = 0
            Dim Cuerpo_Doc As String = ""
            Dim Ruta_Documento_Completa As String = ""
            '*****************************************
            'Crea disco imagen
            '*****************************************
            Disco_Documento = Nombre_Gabinete & Stru_registro_version_documento.DISC
            '*****************************************
            'Crea Carpeta almacenamiento
            '****************************************
            Result = Ceros_Imagen_Carpeta(Stru_registro_version_documento.IDEX.ToString,
                                          Ceros_Carpeta)
            If Result <> "YES" Then
                Genera_matriz_documentos_version = "Error generando ceros carpeta " & Result & Ceros_Carpeta
                Exit Function
            End If
            '*****************************************
            'Crea la ruta del documento
            '*****************************************
            Ruta_Documento_Completa = Ruta_Documento & Disco_Documento & "\" & Ceros_Carpeta &
            Stru_registro_version_documento.IDEX.ToString & "\"

            '*****************************************
            'Crea el cuerpo de la imagen
            '*****************************************
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                             Stru_registro_version_documento.ID)
            If Result <> "YES" Then
                Genera_matriz_documentos_version = "Error generando ceros de imagen " & Result
                Exit Function
            End If
            Dim ref_clas As New Classactualizacionvisor
            Dim visor As String = ""
            Dim ext As String = ""
            Dim Est_doc As String = ""
            Result = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(Stru_registro_version_documento.DBT,
                                                                          visor,
                                                                          ext,
                                                                          Est_doc)
            If Result <> "YES" Then
                Genera_matriz_documentos_version = "Error generando imagen añadida " & Result
                Exit Function
            End If
            '***************************************
            'Agrega el documento principal
            'a la matriz de documentos
            '***************************************
            Cuerpo_Doc = "DIG" & Ceros_Cuerpo_Imag & Stru_registro_version_documento.ID
            If Matri_Documentos Is Nothing Then
                Icremento_Matri = 0
                ReDim Preserve Matri_Documentos(0)
            Else
                Icremento_Matri = UBound(Matri_Documentos) + 1
                ReDim Preserve Matri_Documentos(Icremento_Matri)
            End If
            Matri_Documentos(Icremento_Matri) = Ruta_Documento_Completa & Cuerpo_Doc & ext

            '*******************************************
            'Agrega los documentos del tif
            '*******************************************
            If UCase(ext) = ".TIF" Or UCase(ext) = ".BMP" Or UCase(ext) = ".JPG" Then
                If Numero_Documentos > 1 Then
                    For i3 As Integer = 0 To (Numero_Documentos - 2)
                        Result = ""
                        Ceros_Ext = ""
                        Result = Ceros_Imagen_Alamacenada_ext(i3,
                                                              Ceros_Ext)
                        If Result <> "YES" Then
                            Genera_matriz_documentos_version = "Error generando ceros extension imagen " & Result
                            Exit Function
                        End If
                        Icremento_Matri = Icremento_Matri + 1
                        ReDim Preserve Matri_Documentos(Icremento_Matri)
                        Matri_Documentos(Icremento_Matri) = Ruta_Documento_Completa & Cuerpo_Doc & "." & Ceros_Ext & i3
                    Next
                End If
            End If
            Genera_matriz_documentos_version = "YES"
        Catch ex As Exception
            Genera_matriz_documentos_version = "Inconsistencia general funcion Genera_matriz_documentos_version " & ex.Message
        End Try
    End Function
End Class
