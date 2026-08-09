Public Structure stru_permisos_usuario_grupo_gabinete
    Dim CONSULTA_IMAGEN As Integer
    Dim ALMACENA_IMAGEN As Integer
    Dim PREINDEX_IMAGEN As Integer
    Dim EXPORT_IMAGE_GABINETE As Integer
    Dim EXPORT_IMAGE_FYLESYSTEM As Integer
    Dim EXDPORT_IMAGE_CARPETA As Integer
    Dim EXPOR_IMAGE_CORREO As Integer
    Dim ELIMINAR_REGISTRO As Integer
    Dim MASTER_ELIMINAR_REGISTRO As Integer
    Dim ADD_IMAGEN_REGISTRO As Integer
    Dim EDITAR_REGISTRO As Integer
    Dim EXPORTAR_LISTA_REGISTRO As Integer
    Dim ACTUALIZA_BATCH_REGISTRO As Integer
    Dim EDICION_IMAGEN As Integer
    Dim IMPRI_IMAGEN As Integer
    Dim GUARDAR_IMAGEN As Integer
    Dim CROP_IMAGEN As Integer
    Dim ADD_SELLO_IMAGEN As Integer
    Dim ADD_FIRMA_DIGTIAL_IMAGEN As Integer
    Dim ADD_ESTAMP_CRONOLOGICO_IMAGEN As Integer
    Dim ADD_COPIA_ANOTACION_IMAGEN As Integer
    Dim ADD_CAPO_WF_IMAGEN As Integer
    Dim ADD_STAMP_RADICADO_IMAGEN As Integer
    Dim ADD_BIPMAN_IMAGE As Integer
    Dim ADD_OCR_IMAGE As Integer
    Dim ADD_TRANSFORM_IMAGE As Integer
    Dim ADD_DESKIEW_IMAGE As Integer
End Structure
Public Class Class_permisos_usuarios_gabinetes
    Function SolicitagabinetesPermitidosUsuario(ByVal IdUsuario As Integer,
                                                ByRef CDGabinetesPermitidos As List(Of CDGabinetesPermitidos)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita gabinetes relacionados al usuario docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdUsuario          : Representa la identificación del usuario
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDGabinetesPermitidos  : Retorna estructura con los gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2015-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "select  sys.id_gabinete,NOMBRE_GABINETE  from  permisos_usuarios_gabinetes as pug " &
                   " inner join system1 as sys on (sys.NOMBRE=pug.NOMBRE_GABINETE) " &
                   " where pug.USUARIOS_DA_Clave_Usuario=" & IdUsuario
            Dim DaConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_usuarios_gabinetes")
            Dim Result As String = DaConexion.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitagabinetesPermitidosUsuario dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CDGabinetesPermitidos = Nothing
                Return "YES"
            Else
                Dim _CDGabinetesPermitidos As New CDGabinetesPermitidos
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    _CDGabinetesPermitidos = New CDGabinetesPermitidos
                    _CDGabinetesPermitidos.IdGabinete = Datset.Tables(0).Rows(i).Item(0)
                    _CDGabinetesPermitidos.NombreGabinete = Datset.Tables(0).Rows(i).Item(1)
                    CDGabinetesPermitidos.Add(_CDGabinetesPermitidos)
                Next
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia función  SolicitagabinetesPermitidosUsuario " & ex.Message
        End Try
    End Function
    Function SolicitagabinetesPermitidosUsuario(ByVal IdUsuario As Integer,
                                                ByRef EstruDatosGabinete() As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita gabinetes relacionados al usuario docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdUsuario          : Representa la identificación del usuario
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstruDatosGabinete  : Retorna estructura con los gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2015-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "select  NOMBRE_GABINETE  from  permisos_usuarios_gabinetes " &
                   " where USUARIOS_DA_Clave_Usuario='" & IdUsuario & "'"
            Dim DaConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_usuarios_gabinetes")
            Dim Result As String = DaConexion.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitagabinetesPermitidosUsuario dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "YES"
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve EstruDatosGabinete(i)
                    EstruDatosGabinete(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia función  SolicitagabinetesPermitidosUsuario " & ex.Message
        End Try
    End Function
    Function Retorna_gabinetes_permitidos_usuario_almacenamiento(ByVal Id_usuario As Integer,
                                                                 ByRef Matri_Datos() As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de los gabinetes permitidos
        'por el grupo
        'Fecha : 2019-07-17
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_GABINETE  from  permisos_usuarios_gabinetes " &
                   " where USUARIOS_DA_Clave_Usuario='" & Id_usuario & "' and ALMACENA_IMAGEN=1"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_usuarios_gabinetes")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_gabinetes_permitidos_usuario_almacenamiento = " La funcion Retorna_gabinetes_permitidos_usuario_almacenamiento dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_gabinetes_permitidos_usuario_almacenamiento = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(i)
                    Matri_Datos(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_gabinetes_permitidos_usuario_almacenamiento = "YES"
            End If

        Catch ex As Exception
            Retorna_gabinetes_permitidos_usuario_almacenamiento = "Inconsistencia función  Retorna_gabinetes_permitidos_usuario_almacenamiento " & ex.Message
        End Try
    End Function

    Function SolicitaPermisosGabineteUsuario(ByVal NombreGabinete As String,
                                             ByVal IdusuarioLogueado As Integer,
                                             ByRef StruPermisoGabinete As stru_permiso_gabinete) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solcita los permisos de un usuario sobre un gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        'IdGrupoDocuarchi    : Representa la identiifcación del grupo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruPermisoGabinete  : Retorna la estructura de permisos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2010-12-27
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            StruPermisoGabinete.CONSULTA_IMAGEN = 0
            StruPermisoGabinete.ALMACENA_IMAGEN = 0
            StruPermisoGabinete.PREINDEX_IMAGEN = 0
            StruPermisoGabinete.EXPORT_IMAGE_GABINETE = 0
            StruPermisoGabinete.EXPORT_IMAGE_FYLESYSTEM = 0
            StruPermisoGabinete.EXDPORT_IMAGE_CARPETA = 0
            StruPermisoGabinete.EXPOR_IMAGE_CORREO = 0
            StruPermisoGabinete.ELIMINAR_REGISTRO = 0
            StruPermisoGabinete.MASTER_ELIMINAR_REGISTRO = 0
            StruPermisoGabinete.ADD_IMAGEN_REGISTRO = 0
            StruPermisoGabinete.EDITAR_REGISTRO = 0
            StruPermisoGabinete.EXPORTAR_LISTA_REGISTRO = 0
            StruPermisoGabinete.ACTUALIZA_BATCH_REGISTRO = 0
            StruPermisoGabinete.EDICION_IMAGEN = 0
            StruPermisoGabinete.IMPRI_IMAGEN = 0
            StruPermisoGabinete.GUARDAR_IMAGEN = 0
            StruPermisoGabinete.CROP_IMAGEN = 0
            StruPermisoGabinete.ADD_SELLO_IMAGEN = 0
            StruPermisoGabinete.ADD_FIRMA_DIGTIAL_IMAGEN = 0
            StruPermisoGabinete.ADD_ESTAMP_CRONOLOGICO_IMAGEN = 0
            StruPermisoGabinete.ADD_COPIA_ANOTACION_IMAGEN = 0
            StruPermisoGabinete.ADD_CAPO_WF_IMAGEN = 0
            StruPermisoGabinete.ADD_STAMP_RADICADO_IMAGEN = 0
            StruPermisoGabinete.ADD_BIPMAN_IMAGE = 0
            StruPermisoGabinete.ADD_OCR_IMAGE = 0
            StruPermisoGabinete.ADD_TRANSFORM_IMAGE = 0
            StruPermisoGabinete.ADD_DESKIEW_IMAGE = 0
            Dim SqlConsulta As String = "select * from permisos_usuarios_gabinetes  " &
            " where USUARIOS_DA_Clave_Usuario = " & IdusuarioLogueado &
            " and  NOMBRE_GABINETE='" & NombreGabinete & "'"
            Dim DaConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_usuarios_gabinetes")
            Dim Result As String = DaConexion.SELECTION_SELECT_FIELDA(SqlConsulta,
                                                                      Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitaPermisosGabineteUsuario dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                StruPermisoGabinete.CONSULTA_IMAGEN = Datset.Tables(0).Rows(0).Item("CONSULTA_IMAGEN")
                StruPermisoGabinete.ALMACENA_IMAGEN = Datset.Tables(0).Rows(0).Item("ALMACENA_IMAGEN")
                StruPermisoGabinete.PREINDEX_IMAGEN = Datset.Tables(0).Rows(0).Item("PREINDEX_IMAGEN")
                StruPermisoGabinete.EXPORT_IMAGE_GABINETE = Datset.Tables(0).Rows(0).Item("EXPORT_IMAGE_GABINETE")
                StruPermisoGabinete.EXPORT_IMAGE_FYLESYSTEM = Datset.Tables(0).Rows(0).Item("EXPORT_IMAGE_FYLESYSTEM")
                StruPermisoGabinete.EXDPORT_IMAGE_CARPETA = Datset.Tables(0).Rows(0).Item("EXDPORT_IMAGE_CARPETA")
                StruPermisoGabinete.EXPOR_IMAGE_CORREO = Datset.Tables(0).Rows(0).Item("EXPOR_IMAGE_CORREO")
                StruPermisoGabinete.ELIMINAR_REGISTRO = Datset.Tables(0).Rows(0).Item("ELIMINAR_REGISTRO")
                StruPermisoGabinete.MASTER_ELIMINAR_REGISTRO = Datset.Tables(0).Rows(0).Item("MASTER_ELIMINAR_REGISTRO")
                StruPermisoGabinete.ADD_IMAGEN_REGISTRO = Datset.Tables(0).Rows(0).Item("ADD_IMAGEN_REGISTRO")
                StruPermisoGabinete.EDITAR_REGISTRO = Datset.Tables(0).Rows(0).Item("EDITAR_REGISTRO")
                StruPermisoGabinete.EXPORTAR_LISTA_REGISTRO = Datset.Tables(0).Rows(0).Item("EXPORTAR_LISTA_REGISTRO")
                StruPermisoGabinete.ACTUALIZA_BATCH_REGISTRO = Datset.Tables(0).Rows(0).Item("ACTUALIZA_BATCH_REGISTRO")
                StruPermisoGabinete.EDICION_IMAGEN = Datset.Tables(0).Rows(0).Item("EDICION_IMAGEN")
                StruPermisoGabinete.IMPRI_IMAGEN = Datset.Tables(0).Rows(0).Item("IMPRI_IMAGEN")
                StruPermisoGabinete.GUARDAR_IMAGEN = Datset.Tables(0).Rows(0).Item("GUARDAR_IMAGEN")
                StruPermisoGabinete.CROP_IMAGEN = Datset.Tables(0).Rows(0).Item("CROP_IMAGEN")
                StruPermisoGabinete.ADD_SELLO_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_SELLO_IMAGEN")
                StruPermisoGabinete.ADD_FIRMA_DIGTIAL_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_FIRMA_DIGTIAL_IMAGEN")
                StruPermisoGabinete.ADD_ESTAMP_CRONOLOGICO_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_ESTAMP_CRONOLOGICO_IMAGEN")
                StruPermisoGabinete.ADD_COPIA_ANOTACION_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_COPIA_ANOTACION_IMAGEN")
                StruPermisoGabinete.ADD_CAPO_WF_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_CAPO_WF_IMAGEN")
                StruPermisoGabinete.ADD_STAMP_RADICADO_IMAGEN = Datset.Tables(0).Rows(0).Item("ADD_STAMP_RADICADO_IMAGEN")
                StruPermisoGabinete.ADD_BIPMAN_IMAGE = Datset.Tables(0).Rows(0).Item("ADD_BIPMAN_IMAGE")
                StruPermisoGabinete.ADD_OCR_IMAGE = Datset.Tables(0).Rows(0).Item("ADD_OCR_IMAGE")
                StruPermisoGabinete.ADD_TRANSFORM_IMAGE = Datset.Tables(0).Rows(0).Item("ADD_TRANSFORM_IMAGE")
                StruPermisoGabinete.ADD_DESKIEW_IMAGE = Datset.Tables(0).Rows(0).Item("ADD_DESKIEW_IMAGE")
                Return "YES"
            Else
                StruPermisoGabinete.CONSULTA_IMAGEN = 0
                StruPermisoGabinete.ALMACENA_IMAGEN = 0
                StruPermisoGabinete.PREINDEX_IMAGEN = 0
                StruPermisoGabinete.EXPORT_IMAGE_GABINETE = 0
                StruPermisoGabinete.EXPORT_IMAGE_FYLESYSTEM = 0
                StruPermisoGabinete.EXDPORT_IMAGE_CARPETA = 0
                StruPermisoGabinete.EXPOR_IMAGE_CORREO = 0
                StruPermisoGabinete.ELIMINAR_REGISTRO = 0
                StruPermisoGabinete.ADD_IMAGEN_REGISTRO = 0
                StruPermisoGabinete.EDITAR_REGISTRO = 0
                StruPermisoGabinete.EXPORTAR_LISTA_REGISTRO = 0
                StruPermisoGabinete.ACTUALIZA_BATCH_REGISTRO = 0
                StruPermisoGabinete.EDICION_IMAGEN = 0
                StruPermisoGabinete.IMPRI_IMAGEN = 0
                StruPermisoGabinete.GUARDAR_IMAGEN = 0
                StruPermisoGabinete.CROP_IMAGEN = 0
                StruPermisoGabinete.ADD_SELLO_IMAGEN = 0
                StruPermisoGabinete.ADD_FIRMA_DIGTIAL_IMAGEN = 0
                StruPermisoGabinete.ADD_ESTAMP_CRONOLOGICO_IMAGEN = 0
                StruPermisoGabinete.ADD_COPIA_ANOTACION_IMAGEN = 0
                StruPermisoGabinete.ADD_CAPO_WF_IMAGEN = 0
                StruPermisoGabinete.ADD_STAMP_RADICADO_IMAGEN = 0
                StruPermisoGabinete.ADD_BIPMAN_IMAGE = 0
                StruPermisoGabinete.ADD_OCR_IMAGE = 0
                StruPermisoGabinete.ADD_TRANSFORM_IMAGE = 0
                StruPermisoGabinete.ADD_DESKIEW_IMAGE = 0
                StruPermisoGabinete.MASTER_ELIMINAR_REGISTRO = 0
                Return "YES"
            End If
        Catch ex As Exception
            SolicitaPermisosGabineteUsuario = "Error General Funcion : SolicitaPermisosGabineteUsuario Error : " & ex.Message
        End Try
    End Function
End Class
