
Public Class Class_permisos_grupos_gabinetes
    Function SolicitaGabinetesPermitidosGrupo(ByVal IdGrupo As Integer,
                                              ByRef CDGabinetesPermitidos As List(Of CDGabinetesPermitidos)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita gabinetes relacionados al grupo relacionado al usuario docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGrupo           : Representa la identificación del grupo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDGabinetesPermitidos  : Retorna estructura con los gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-25
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select sys.id_gabinete, NOMBRE_GABINETE  from  permisos_grupos_gabinetes as pgb " &
                   " inner join system1 as sys on (sys.NOMBRE=pgb.NOMBRE_GABINETE) " &
                   " where pgb.GRUPOS_DA_Clave_Grupo='" & IdGrupo & "'"
            Dim DataConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_grupos_gabinetes")
            Dim Result As String = DataConexion.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitaGabinetesPermitidosGrupo dice : " & Result
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
            Return "Inconsistencia función  SolicitaGabinetesPermitidosGrupo " & ex.Message
        End Try
    End Function
    Function SolicitaGabinetesPermitidosGrupo(ByVal IdGrupo As Integer,
                                              ByRef EstructuraDatosGabinetes() As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita gabinetes relacionados al grupo relacionado al usuario docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_script           : Representa la identificación del script de validación
        'campo_radicacion    : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstructuraDatosGabinetes  : Retorna estructura con los gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2015-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_GABINETE  from  permisos_grupos_gabinetes " &
                   " where GRUPOS_DA_Clave_Grupo='" & IdGrupo & "'"
            Dim DataConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_grupos_gabinetes")
            Dim Result As String = DataConexion.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitaGabinetesPermitidosGrupo dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "YES"
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve EstructuraDatosGabinetes(i)
                    EstructuraDatosGabinetes(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Return "YES"
            End If

        Catch ex As Exception
            Return "Inconsistencia función  SolicitaGabinetesPermitidosGrupo " & ex.Message
        End Try
    End Function

    Function Retorna_gabinetes_permitidos_grupos_almacenaminento(ByVal Id_grupo As Integer,
                                                                 ByRef Matri_Datos() As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de los gabinetes permitidos
        'por el grupo
        'Fecha : 2019-07-17
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_GABINETE  from  permisos_grupos_gabinetes " &
                   " where GRUPOS_DA_Clave_Grupo='" & Id_grupo & "' and ALMACENA_IMAGEN=1"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_grupos_gabinetes")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_gabinetes_permitidos_grupos_almacenaminento = " La funcion Retorna_gabinetes_permitidos_grupos_almacenaminento dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_gabinetes_permitidos_grupos_almacenaminento = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(i)
                    Matri_Datos(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_gabinetes_permitidos_grupos_almacenaminento = "YES"
            End If
        Catch ex As Exception
            Retorna_gabinetes_permitidos_grupos_almacenaminento = "Inconsistencia función  Retorna_gabinetes_permitidos_grupos_almacenaminento " & ex.Message
        End Try
    End Function

    Function SolicitaPermisosGabineteGrupo(ByVal NombreGabinete As String,
                                           ByVal IdGrupoDocuarchi As Integer,
                                           ByRef StruPermisoGabinete As stru_permiso_gabinete,
                                           ByRef EstadoPermisoGrupo As String) As String

        '-----------------------------------------------------------------------------------------------
        'Funcion : Solcita los permisos de un grupo sobre un gabinete
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
            Dim Parametro_Consulta As String = "select  CONSULTA_IMAGEN,ALMACENA_IMAGEN,PREINDEX_IMAGEN,EXPORT_IMAGE_GABINETE," &
                 "EXPORT_IMAGE_FYLESYSTEM,EXDPORT_IMAGE_CARPETA,EXPOR_IMAGE_CORREO,ELIMINAR_REGISTRO,ADD_IMAGEN_REGISTRO," &
                 "EDITAR_REGISTRO,EXPORTAR_LISTA_REGISTRO,ACTUALIZA_BATCH_REGISTRO,EDICION_IMAGEN,IMPRI_IMAGEN,GUARDAR_IMAGEN," &
                 "CROP_IMAGEN,ADD_SELLO_IMAGEN,ADD_FIRMA_DIGTIAL_IMAGEN,ADD_ESTAMP_CRONOLOGICO_IMAGEN,ADD_COPIA_ANOTACION_IMAGEN," &
                 "ADD_CAPO_WF_IMAGEN,ADD_STAMP_RADICADO_IMAGEN,ADD_BIPMAN_IMAGE,ADD_OCR_IMAGE,ADD_TRANSFORM_IMAGE,ADD_DESKIEW_IMAGE,MASTER_ELIMINAR_REGISTRO" &
                 " from  permisos_grupos_gabinetes" &
                 " where GRUPOS_DA_Clave_Grupo=" & IdGrupoDocuarchi & " and NOMBRE_GABINETE='" & NombreGabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("permisos_grupos_gabinetes")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Return " La funcion SolicitaPermisosGabineteGrupo dice : " & Result

            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                EstadoPermisoGrupo = "YES"
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
                EstadoPermisoGrupo = "NO"
                Return "YES"
            End If
        Catch ex As Exception
            Return "Error General Funcion : SolicitaPermisosGabineteGrupo Error : " & ex.Message
        End Try

    End Function
End Class
