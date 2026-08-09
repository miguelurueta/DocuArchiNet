Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class WebService_integracion_sii
    Inherits System.Web.Services.WebService
    Public Class ArrayItem_integracion
        Public idliquidacion As Integer
        Public fecha As String
        Public tipotramite As String
        Public idmatriculabase As String
        Public idproponentebase As String
        Public identificacionbase As String
        Public nombrebase As String
        Public numerorecibo As String
        Public numerorecuperacion As String
        Public numeroradicacion As String
        Public tramitepresencial As String
        Public firmadoelectronicamente As String
        Public IMP_02_ID_CLAVE As Integer
        Public estado_migrado As Integer
        Public error_funcion As String
    End Class
    Public Class aray_item_registro
        Public id_migra_registro As Integer
        Public codigo_sii As String
        Public fecha_migracion As String
        Public usuario_migracion As String
        Public imagenes As String
        Public matricula As String
        Public nit_identificacion As String
        Public recibo_sii As String
    End Class
    Public Class estado_respuesta_sello_sii
        Public Property error_gestion As String
        Public Property dato_lista As String
        Public Property structure_lis As List(Of Class_lista_inscripcioes_sello)
        Public Property recibo As String
        Public Property codigo As String
    End Class
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function SeviceGuardaDocumentoAnexoSII(ByVal CDParameterAnexosSII As Object,
                                                  ByVal CDlistaAnexosSII As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Guarda anexo de documento SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII             : Representa la estructura de la inscripcion 
        'CDParameterAnexosSII       : Representa los parametros del almacenamiento
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDEstadoAnexosSII  : Retorna la estructura del resultado del alamacenamiento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim ListCDEstadoAnexosSII = New List(Of CDEstadoAnexosSII)()
        Dim CCDEstadoAnexosSII As CDEstadoAnexosSII = New CDEstadoAnexosSII()
        Try
            Dim Result As String = ""
            Dim _CDlistaAnexosSII As New List(Of CDlistaAnexosSII)
            _CDlistaAnexosSII = JsonConvert.DeserializeObject(Of List(Of CDlistaAnexosSII))(CDlistaAnexosSII)
            Dim _CDParameterAnexosSII As New List(Of CDParameterAnexosSII)
            _CDParameterAnexosSII = JsonConvert.DeserializeObject(Of List(Of CDParameterAnexosSII))(CDParameterAnexosSII)
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim IdImagenAlamacenada As Integer = 0
            Dim EstructuraDatosImagen As stru_datos_image_lista = Nothing
            Dim NombreClaseDocumento As String = "DOCUMENTO ELECTRONICO"
            CCDEstadoAnexosSII.error_gestion = ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII(_CDParameterAnexosSII(0).IdTipoChekLista,
                                                                                                                  _CDParameterAnexosSII(0).DescripcionTipo,
                                                                                                                  _CDParameterAnexosSII(0).IdTipoTaramite,
                                                                                                                  _CDParameterAnexosSII(0).MultiAnexos,
                                                                                                                  _CDParameterAnexosSII(0).Gabinete,
                                                                                                                  _CDParameterAnexosSII(0).CodigoBarras,
                                                                                                                  _CDParameterAnexosSII(0).ReciboSII,
                                                                                                                  _CDlistaAnexosSII(0),
                                                                                                                  NombreClaseDocumento,
                                                                                                                  IdImagenAlamacenada,
                                                                                                                  EstructuraDatosImagen)


            If CCDEstadoAnexosSII.error_gestion = "YES" Then
                Dim file_icon_some = "fa-file"
                If EstructuraDatosImagen.icono_icono_awe_some <> "" Then
                    file_icon_some = EstructuraDatosImagen.icono_icono_awe_some
                End If
                Dim date_campo = EstructuraDatosImagen.nombre_gabinete & "|" & EstructuraDatosImagen.id_imagen & "|" & EstructuraDatosImagen.radicado & "|" &
                EstructuraDatosImagen.tipodocumental & "|" & EstructuraDatosImagen.notipodocumento & "|" & EstructuraDatosImagen.id_tarea_workflow & "|" &
                EstructuraDatosImagen.estado_firma_digital & "|" & file_icon_some
                CCDEstadoAnexosSII.dato_lista = date_campo
                ListCDEstadoAnexosSII.Add(CCDEstadoAnexosSII)
                Return ListCDEstadoAnexosSII
            Else
                CCDEstadoAnexosSII.dato_lista = ""
                ListCDEstadoAnexosSII.Add(CCDEstadoAnexosSII)
                Return ListCDEstadoAnexosSII
            End If
        Catch ex As Exception
            CCDEstadoAnexosSII.error_gestion = "Inconsistencia general funcion SeviceGuardaDocumentoAnexoSII " & ex.Message
            CCDEstadoAnexosSII.dato_lista = ""
            ListCDEstadoAnexosSII.Add(CCDEstadoAnexosSII)
            Return ListCDEstadoAnexosSII
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaArchivosAnexosrelacionadosRadicadoSII(ByVal RadicadoSII As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos y registros de anexos relacionados
        '          de un radicado SII
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'radicado_sii    : Representa el consecutivo del radicado SII
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_boot_table  : Retorna la estructura de campos de tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_boot_table)
        Dim item_ilist As class_boot_table = New class_boot_table
        Try
            Dim ClassListaAnexosSII As New ClassListaAnexosSII
            item_ilist.Error_result = ClassListaAnexosSII.SolicitaArchivosAnexosrelacionadosRadicadoSII(item_ilist.ReciboSII,
                                                                                                        item_ilist.CodigoBarras,
                                                                                                        item_ilist.Gabinete,
                                                                                                        item_ilist.row_table_boot,
                                                                                                        item_ilist.field_table_boot)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaListaConstanciaInscripcionSII(ByVal CodigoBarraSII As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura constancias de inscripcion SII
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'radicado_sii    : Representa el consecutivo del radicado SII
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_boot_table  : Retorna la estructura de campos de tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-07-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_boot_table)
        Dim item_ilist As class_boot_table = New class_boot_table
        Try
            Dim Class_consultarInformacionSello As New Class_consultarInformacionSello
            item_ilist.Error_result = Class_consultarInformacionSello.SolicitaListaConstanciasIncripcionSII(item_ilist.row_table_boot,
                                                                                                            item_ilist.field_table_boot)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceRegistraCacheInscripcionRadicadoSII(ByVal CIncripcionSII As Object, ByVal IdTramite As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio registra cache de inscripción de documento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII           : Representa la estructura de la insscripion SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim CcacheIncripcionSIIResult = New List(Of CcacheIncripcionSIIResult)()
        Dim _CcacheIncripcionSIIResult As CcacheIncripcionSIIResult = New CcacheIncripcionSIIResult()
        Try
            Dim ClassRaSiiCahcheInscripcion As New ClassRaSiiCahcheInscripcion
            Dim Result As String = ""
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            Dim StruSiiCahcheInscripcion As New StruSiiCahcheInscripcion
            StruSiiCahcheInscripcion.RadicadoSII = SCIncripcionSII.Item(0).RADICADO_SII
            StruSiiCahcheInscripcion.CodBarras = SCIncripcionSII.Item(0).COD_BARRA_SII
            StruSiiCahcheInscripcion.Matricula = SCIncripcionSII.Item(0).MATRICULA_SII
            If StruSiiCahcheInscripcion.Matricula <> "" Then
                StruSiiCahcheInscripcion.Matricula = StruSiiCahcheInscripcion.Matricula.Replace("SO", "")
            End If
            StruSiiCahcheInscripcion.MatriculaPropietario = SCIncripcionSII.Item(0).MATRICULA_SII
            StruSiiCahcheInscripcion.Rsocial = SCIncripcionSII.Item(0).RSOCIAL_SII
            StruSiiCahcheInscripcion.NombrePropietario = SCIncripcionSII.Item(0).RSOCIAL_SII
            StruSiiCahcheInscripcion.NitIdentificacion = SCIncripcionSII.Item(0).NIT_SII
            StruSiiCahcheInscripcion.Identificacionpro = SCIncripcionSII.Item(0).NIT_SII
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            _CcacheIncripcionSIIResult.AppError = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                                                    CTipoDocEntrante)
            If _CcacheIncripcionSIIResult.AppError <> "YES" Then
                CcacheIncripcionSIIResult.Add(_CcacheIncripcionSIIResult)
                Return CcacheIncripcionSIIResult
            End If
            StruSiiCahcheInscripcion.NombreGabinete = CTipoDocEntrante.nombre_gabinete_workflow
            _CcacheIncripcionSIIResult.AppError = ClassRaSiiCahcheInscripcion.RegistraCacheInscripcionRadicadoSII(StruSiiCahcheInscripcion)
            CcacheIncripcionSIIResult.Add(_CcacheIncripcionSIIResult)
            Return CcacheIncripcionSIIResult
        Catch ex As Exception
            _CcacheIncripcionSIIResult.AppError = "Inconsistencia general funcion ServiceRegistraCacheInscripcionRadicadoSII " & ex.Message
            CcacheIncripcionSIIResult.Add(_CcacheIncripcionSIIResult)
            Return CcacheIncripcionSIIResult
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraCacheInscripcionRadicado(ByVal ReciboSII As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita cache de inscripción SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ReciboSII              : Representa la estructura de la insscripion SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CcacheIncripcionSIIResult   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim CcacheIncripcionSIIResult = New List(Of CcacheIncripcionSIIResult)()
        Dim _CcacheIncripcionSIIResult As CcacheIncripcionSIIResult = New CcacheIncripcionSIIResult()
        Try
            Dim ClassRaSiiCahcheInscripcion As New ClassRaSiiCahcheInscripcion
            Dim CacheInscripcion As New CacheInscripcion
            _CcacheIncripcionSIIResult.CahcheInscripcion = New List(Of CacheInscripcion)()
            _CcacheIncripcionSIIResult.AppError = ClassRaSiiCahcheInscripcion.SolicitaEstructuraCacheInscripcionRadicado(ReciboSII,
                                                                                                                         CacheInscripcion)
            _CcacheIncripcionSIIResult.CahcheInscripcion.Add(CacheInscripcion)
            CcacheIncripcionSIIResult.Add(_CcacheIncripcionSIIResult)
            Return CcacheIncripcionSIIResult
        Catch ex As Exception
            _CcacheIncripcionSIIResult.AppError = "Inconsistencia general funcion ServiceSolicitaEstructuraCacheInscripcionRadicado " & ex.Message
            CcacheIncripcionSIIResult.Add(_CcacheIncripcionSIIResult)
            Return CcacheIncripcionSIIResult
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceActualizaEstadoVinculacionDocumentoSII(ByVal IdExpediente As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion :Servicio web que expone la actualización estado documentos vinculados al
        'expedediente en el cache sii
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente   : Representa la identificacion del expediente
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim IlistCdefaultSiiCacheSII As New List(Of CdefaultSiiCacheSII)()
        Dim CdefaultSiiCacheSII As CdefaultSiiCacheSII = New CdefaultSiiCacheSII
        Try
            Dim Result As String = ""
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            CdefaultSiiCacheSII.ErrorService = ClassRaSIiCacheExpediente.ActualizaEstadoVinculacionDocumentoSII(IdExpediente)
            IlistCdefaultSiiCacheSII.Add(CdefaultSiiCacheSII)
            Return IlistCdefaultSiiCacheSII
        Catch ex As Exception
            CdefaultSiiCacheSII.ErrorService = "Inconsistecia general funcion ServiceActualizaEstadoVinculacionDocumentoSII " & ex.Message
            IlistCdefaultSiiCacheSII.Add(CdefaultSiiCacheSII)
            Return IlistCdefaultSiiCacheSII
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceActualizaIndiceDocumentosSII(ByVal CIncripcionSII As Object, ByVal IdTramite As Object, ByVal ReciboSII As String)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio actualiza documentos indices workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identificación del tramite SII
        'CIncripcionSII      : Representa la estructura de los sellos de inscripción
        'ReciboSII           : Representa el consecutivo de recibo SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim CcacheInceSIIResult = New List(Of CcacheIncripcionSIIResult)()
        Dim _CcacheInceSIIResulta As CcacheIncripcionSIIResult = New CcacheIncripcionSIIResult()
        Try
            Dim ClassRaSIICacheActualizaIndice As New ClassRaSIICacheActualizaIndice
            Dim Result As String = ""
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            _CcacheInceSIIResulta.AppError = ClassRaSIICacheActualizaIndice.ActualizaIndiceDocumentosSII(IdTramite,
                                                                                                         ReciboSII,
                                                                                                         SCIncripcionSII)
            CcacheInceSIIResult.Add(_CcacheInceSIIResulta)
            Return CcacheInceSIIResult
        Catch ex As Exception
            _CcacheInceSIIResulta.AppError = "Inconsistencia general funcion ServiceActualizaIndiceDocumentosSII " & ex.Message
            CcacheInceSIIResult.Add(_CcacheInceSIIResulta)
            Return CcacheInceSIIResult
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceRegistraCahcheVinculacionSII(ByVal CStruSiiCahcheVinculacion As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el registro cache vincualción expediente SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheVinculacion  : Representa la estrucrura del cache de vinculaión a expediente
        '             
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdefaultSiiCahcheVinculacion   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim IlistCdefaultSiiCahcheVinculacion As New List(Of CdefaultSiiCahcheVinculacion)()
        Dim CdefaultSiiCahcheVinculacion As CdefaultSiiCahcheVinculacion = New CdefaultSiiCahcheVinculacion
        Try
            Dim Result As String = ""
            Dim ClassRaSiiCacheVinculacion As New ClassRaSiiCacheVinculacion
            Dim _CStruSiiCahcheVinculacion As New List(Of CStruSiiCahcheVinculacion)
            _CStruSiiCahcheVinculacion = JsonConvert.DeserializeObject(Of List(Of CStruSiiCahcheVinculacion))(CStruSiiCahcheVinculacion)
            _CStruSiiCahcheVinculacion(0).Matricula = _CStruSiiCahcheVinculacion(0).Matricula.Replace("S0", "")
            CdefaultSiiCahcheVinculacion.ErrorService = ClassRaSiiCacheVinculacion.RegistraCahcheVinculacionSII(_CStruSiiCahcheVinculacion(0),
                                                                                                                CdefaultSiiCahcheVinculacion.id_sii_cache_vinculacion)
            IlistCdefaultSiiCahcheVinculacion.Add(CdefaultSiiCahcheVinculacion)
            Return IlistCdefaultSiiCahcheVinculacion
        Catch ex As Exception
            CdefaultSiiCahcheVinculacion.ErrorService = "Inconsistecia general funcion ServiceRegistraCahcheVinculacionSII " & ex.Message
            IlistCdefaultSiiCahcheVinculacion.Add(CdefaultSiiCahcheVinculacion)
            Return IlistCdefaultSiiCahcheVinculacion
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaCahcheVinculacionSII(ByVal CIncripcionSII As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que Solicita la estructura cache vinculacion de integración SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheVinculacion   : Representa la estructura del registro de vinculacion
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim RCStruSiiCahcheVinculacion As New List(Of CStruSiiCahcheVinculacion)()
        Dim CStruSiiCahcheVinculacion As CStruSiiCahcheVinculacion = New CStruSiiCahcheVinculacion
        Try
            Dim Result As String = ""
            Dim ClassRaSiiCacheVinculacion As New ClassRaSiiCacheVinculacion
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            CStruSiiCahcheVinculacion.ErrorService = ClassRaSiiCacheVinculacion.SolicitaCahcheVinculacionSII(SCIncripcionSII.Item(0).RADICADO_SII,
                                                                                                             CStruSiiCahcheVinculacion)
            RCStruSiiCahcheVinculacion.Add(CStruSiiCahcheVinculacion)
            Return RCStruSiiCahcheVinculacion
        Catch ex As Exception
            CStruSiiCahcheVinculacion.ErrorService = "Inconsistecia general funcion ServiceSolicitaCahcheVinculacionSII " & ex.Message
            RCStruSiiCahcheVinculacion.Add(CStruSiiCahcheVinculacion)
            Return RCStruSiiCahcheVinculacion
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceRegistraCacheCreacionExpedienteSII(ByVal CStruSiiCahcheExpediente As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servici web que expone el registro cache creacion expediente SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheExpediente  : Representa la estrucrura creacion registro cache
        '             
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheExpediente   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim IlistCdefaultSiiCacheSII As New List(Of CdefaultSiiCacheSII)()
        Dim CdefaultSiiCacheSII As CdefaultSiiCacheSII = New CdefaultSiiCacheSII
        Try
            Dim Result As String = ""
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim _CStruSiiCahcheExpediente As New List(Of CStruSiiCahcheExpediente)
            _CStruSiiCahcheExpediente = JsonConvert.DeserializeObject(Of List(Of CStruSiiCahcheExpediente))(CStruSiiCahcheExpediente)
            _CStruSiiCahcheExpediente(0).Matricula = _CStruSiiCahcheExpediente(0).Matricula.Replace("S0", "")
            CdefaultSiiCacheSII.ErrorService = ClassRaSIiCacheExpediente.RegistraCacheCreacionExpedienteSII(_CStruSiiCahcheExpediente(0),
                                                                                                            CdefaultSiiCacheSII.id_ra_sii_cache_exepediente)
            IlistCdefaultSiiCacheSII.Add(CdefaultSiiCacheSII)
            Return IlistCdefaultSiiCacheSII
        Catch ex As Exception
            CdefaultSiiCacheSII.ErrorService = "Inconsistecia general funcion ServiceRegistraCacheCreacionExpedienteSII " & ex.Message
            IlistCdefaultSiiCacheSII.Add(CdefaultSiiCacheSII)
            Return IlistCdefaultSiiCacheSII
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaRegistroExpedienteMatricula(ByVal CIncripcionSII As Object, ByVal IdTramite As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que Solicita la estructura del registro de la creción del expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII        : Representa la estructura con la inscripción
        'IdTramite             : Representa la identificación del tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheExpediente   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim RCStruSiiCahcheExpediente As New List(Of CStruSiiCahcheExpediente)()
        Dim CStruSiiCahcheExpediente As CStruSiiCahcheExpediente = New CStruSiiCahcheExpediente
        Try
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            CStruSiiCahcheExpediente.ErrorService = ClassRaSIiCacheExpediente.SolicitaRegistroExpedienteMatricula(IdTramite,
                                                                                                                  SCIncripcionSII,
                                                                                                                  CStruSiiCahcheExpediente)
            RCStruSiiCahcheExpediente.Add(CStruSiiCahcheExpediente)
            Return RCStruSiiCahcheExpediente
        Catch ex As Exception
            CStruSiiCahcheExpediente.ErrorService = "Inconsistecia general funcion ServiceSolicitaRegistroExpedienteMatricula " & ex.Message
            RCStruSiiCahcheExpediente.Add(CStruSiiCahcheExpediente)
            Return RCStruSiiCahcheExpediente
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_recibo_radicado_sii(ByVal radicado_sii As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el recibo relacionado al radicado del SII
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'radicado_sii    : Representa el consecutivo del radicado SII
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_ConsultarRadicado_sii_servcio  : 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-01-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConsultarRadicado_sii_servcio)
        Dim item_ilist As Class_ConsultarRadicado_sii_servcio = New Class_ConsultarRadicado_sii_servcio
        Try
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
            item_ilist.Error_gestion = Class_ConsultarRadicado_sii.ConSultarRadicado(radicado_sii,
                                                                                     stru_consulta_radicado)
            item_ilist.recibo_sii = stru_consulta_radicado.recibo
            If item_ilist.Error_gestion <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_gestion = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_lista_archivos_relacionados_radicado_sii(ByVal radicado_sii As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos y registros de las imagenes 
        '          de un radicado SII
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'radicado_sii    : Representa el consecutivo del radicado SII
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_boot_table  : Retorna la estructura de campos de tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_boot_table)
        Dim item_ilist As class_boot_table = New class_boot_table
        Try
            Dim Class_lista_imagenes_sii As New Class_lista_imagenes_sii
            item_ilist.Error_result = Class_lista_imagenes_sii.SolicitaArchivosRelacionadosRadicadoSII(radicado_sii,
                                                                                                                    item_ilist.row_table_boot,
                                                                                                                    item_ilist.field_table_boot)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function web_service_Solicita_rago_fecha_migracion_tramite_sii(ByVal DName As String, ByVal DName_ As String)
        Dim country As New List(Of ArrayItem_integracion)
        Try
            Dim Class_imp_02_MIGRA_SII_FECHA As New Class_imp_02_MIGRA_SII_FECHA
            Dim stru_radicado_si() As stru_radicado_si = Nothing
            Dim Result As String = ""
            Result = Class_imp_02_MIGRA_SII_FECHA.Solicita_rago_fecha_migracion_tramite_sii(DName,
                                                                                            DName_,
                                                                                            stru_radicado_si)
            If Result <> "YES" Then
                Dim item As New ArrayItem_integracion
                item.error_funcion = Result
                country.Add(item)
                Return country
            Else
                For i As Integer = 0 To stru_radicado_si.Length - 1
                    Dim item As New ArrayItem_integracion
                    item.error_funcion = Result
                    item.idliquidacion = stru_radicado_si(i).idliquidacion
                    item.fecha = stru_radicado_si(i).fecha
                    item.tipotramite = stru_radicado_si(i).tipotramite
                    item.idmatriculabase = stru_radicado_si(i).idmatriculabase
                    item.idproponentebase = stru_radicado_si(i).idproponentebase
                    item.identificacionbase = stru_radicado_si(i).identificacionbase
                    item.nombrebase = stru_radicado_si(i).nombrebase
                    item.numerorecibo = stru_radicado_si(i).numerorecibo
                    item.numerorecuperacion = stru_radicado_si(i).numerorecuperacion
                    item.numeroradicacion = stru_radicado_si(i).numeroradicacion
                    item.tramitepresencial = stru_radicado_si(i).tramitepresencial
                    item.firmadoelectronicamente = stru_radicado_si(i).firmadoelectronicamente
                    item.IMP_02_ID_CLAVE = stru_radicado_si(i).IMP_02_ID_CLAVE
                    item.estado_migrado = stru_radicado_si(i).estado_migrado
                    item.estado_migrado = stru_radicado_si(i).estado_migrado
                    country.Add(item)
                Next
                Return country
            End If
        Catch ex As Exception
            Dim item As New ArrayItem_integracion
            item.error_funcion = "Inconsistencia general funcion web_service_lista_item_menu : " & ex.Message
            country.Add(item)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_migra_sii_registro(ByVal parameter As Object)
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of ArrayItem_integracion_))(parameter)
            Dim Class_ra_sii_migra_imagenes As New Class_ra_sii_migra_imagenes
            Dim Result As String = ""
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
            Else
                Dim ArrayItem_integracion_ As ArrayItem_integracion_
                ArrayItem_integracion_.firmadoelectronicamente = deserialize_parameter(0).firmadoelectronicamente
                ArrayItem_integracion_.identificacionbase = deserialize_parameter(0).identificacionbase
                ArrayItem_integracion_.idliquidacion = deserialize_parameter(0).idliquidacion
                ArrayItem_integracion_.idmatriculabase = deserialize_parameter(0).idmatriculabase
                ArrayItem_integracion_.idproponentebase = deserialize_parameter(0).idproponentebase
                ArrayItem_integracion_.IMP_02_ID_CLAVE = deserialize_parameter(0).IMP_02_ID_CLAVE
                ArrayItem_integracion_.nombrebase = deserialize_parameter(0).nombrebase
                ArrayItem_integracion_.numeroradicacion = deserialize_parameter(0).numeroradicacion
                ArrayItem_integracion_.numerorecibo = deserialize_parameter(0).numerorecibo
                ArrayItem_integracion_.numerorecuperacion = deserialize_parameter(0).numerorecuperacion
                ArrayItem_integracion_.tipotramite = deserialize_parameter(0).tipotramite
                ArrayItem_integracion_.tramitepresencial = deserialize_parameter(0).tramitepresencial
                Result = Class_ra_sii_migra_imagenes.Migra_documento_radicado_sii(ArrayItem_integracion_.numeroradicacion)
                If Result <> "YES" Then
                    Return Result
                Else
                    Return "YES"
                End If
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_migra_registro_radicado_sii(ByVal dname As Object)
        Try

            Dim Class_ra_sii_migra_imagenes As New Class_ra_sii_migra_imagenes
            Dim Class_sii_migra_registro As New Class_sii_migra_registro
            Dim Result As String = ""
            Dim existencia As String = ""
            Result = Class_sii_migra_registro.Solicita_existencia_registro_codigo_sii(dname, _
                                                                                     existencia)
            If Result <> "YES" Then
                Return Result
            End If
            If existencia = "YES" Then
                Return "El codigo informado ya se encuentra migrado"
            End If
            Result = Class_ra_sii_migra_imagenes.Migra_documento_radicado_sii(dname)
            If Result <> "YES" Then
                Return Result
            Else
                Return "YES"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function solicita_lista_registro_sii_migrados(ByVal fecha_ini As String,
                                                         ByVal fecha_fin As String,
                                                         ByVal codigo_sii As String)
        Dim country As New List(Of aray_item_registro)
        Try
            Dim Class_sii_migra_registro As New Class_sii_migra_registro
            Dim Result As String = ""
            Result = Class_sii_migra_registro.Solicita_lista_registro_sii_migrados(fecha_ini,
                                                                                   fecha_fin,
                                                                                   codigo_sii,
                                                                                   country)
            If Result <> "YES" Then
                Return Result
            Else
                Return country
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function SeviceGuardaConstanciaInscripcionSII(ByVal CIncripcionSII As Object,
                                                         ByVal IdTipoTramite As Object,
                                                         ByVal IdTramite As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Guarda sello o constancia de inscripción integración SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII      : Representa la estructura de la inscripcion 
        'IdTipoTramite       : Representa la identificacion del tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'estado_respuesta_sello_sii  : Retorna la estructura del sello
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-21
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Dim Listestado_respuesta_sello_sii = New List(Of estado_respuesta_sello_sii)()
        Dim CestadoRespuestaSelloSII As estado_respuesta_sello_sii = New estado_respuesta_sello_sii()
        Try
            Dim Result As String = ""
            Dim _CIncripcionSII As New List(Of CIncripcionSII)
            _CIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim datos_image As stru_datos_image_lista = Nothing
            If IdTipoTramite = 0 Then
                HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
            Else
                HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = Val(IdTipoTramite)
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            CestadoRespuestaSelloSII.error_gestion = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                                                       CTipoDocEntrante)

            If CestadoRespuestaSelloSII.error_gestion <> "YES" Then
                CestadoRespuestaSelloSII.dato_lista = ""
                Listestado_respuesta_sello_sii.Add(CestadoRespuestaSelloSII)
                Return Listestado_respuesta_sello_sii
            End If
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            CestadoRespuestaSelloSII.error_gestion = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(_CIncripcionSII(0).MATRICULA_SII,
                                                                                                                _CIncripcionSII(0).PROPONENTE_SII,
                                                                                                                CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                                                StruSiiCahcheInscripcion)
            If CestadoRespuestaSelloSII.error_gestion <> "YES" Then
                CestadoRespuestaSelloSII.dato_lista = ""
                Listestado_respuesta_sello_sii.Add(CestadoRespuestaSelloSII)
                Return Listestado_respuesta_sello_sii
            End If
            Dim NitIndentificacion As String = ""
            Dim RazonSocial As String = ""
            NitIndentificacion = StruSiiCahcheInscripcion.NitIdentificacion
            RazonSocial = StruSiiCahcheInscripcion.Rsocial
            If RazonSocial <> "" Then
                RazonSocial = RazonSocial.Replace("'", "")
                RazonSocial = Left(RazonSocial, 120)
            End If
            Dim IdImgenAlmacenada As Integer = 0
            Result = ClassAlmacenamiento.PreAlmacenaConstanciaIsncripcionsSII(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                              "",
                                                                             IdTipoTramite,
                                                                             "DOCUMENTO ELECTRONICO",
                                                                             _CIncripcionSII(0),
                                                                             IdImgenAlmacenada,
                                                                             datos_image)
            If Result <> "YES" Then
                CestadoRespuestaSelloSII.error_gestion = Result
                CestadoRespuestaSelloSII.dato_lista = ""
                Listestado_respuesta_sello_sii.Add(CestadoRespuestaSelloSII)
                Return Listestado_respuesta_sello_sii
            Else
                Dim file_icon_some = "fa-file"
                If datos_image.icono_icono_awe_some <> "" Then
                    file_icon_some = datos_image.icono_icono_awe_some
                End If
                Dim date_campo = datos_image.nombre_gabinete & "|" & datos_image.id_imagen & "|" & datos_image.radicado & "|" &
                datos_image.tipodocumental & "|" & datos_image.notipodocumento & "|" & datos_image.id_tarea_workflow & "|" &
                datos_image.estado_firma_digital & "|" & file_icon_some
                CestadoRespuestaSelloSII.error_gestion = Result
                CestadoRespuestaSelloSII.dato_lista = date_campo
                Listestado_respuesta_sello_sii.Add(CestadoRespuestaSelloSII)
                Return Listestado_respuesta_sello_sii
            End If
        Catch ex As Exception
            CestadoRespuestaSelloSII.error_gestion = "Inconsistencia general funcion SeviceGuardaConstanciaInscripcionSII " & ex.Message
            CestadoRespuestaSelloSII.dato_lista = ""
            Listestado_respuesta_sello_sii.Add(CestadoRespuestaSelloSII)
            Return Listestado_respuesta_sello_sii
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_registros_sellos_sii(ByVal na As Object)
        Dim resultList = New List(Of estado_respuesta_sello_sii)()
        Dim parameter_gestion As estado_respuesta_sello_sii = New estado_respuesta_sello_sii()
        Try
            If HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC") Is Nothing Then
                parameter_gestion.error_gestion = "Seleccione los registros a guardar"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim myStructList As List(Of Class_lista_inscripcioes_sello) = HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC")
            If myStructList.Count > 0 Then
                For i As Integer = 0 To myStructList.Count - 1
                    If myStructList.Item(i).nombre <> "" Then
                        Dim Nombre As String = myStructList.Item(i).nombre
                        Nombre = Nombre.Replace("'", "")
                        Nombre = Nombre.Replace("""", "")
                        Nombre = Nombre.Replace("/", "")
                        Nombre = Nombre.Replace("\", "")
                        myStructList.Item(i).nombre = Nombre
                    End If
                    If myStructList.Item(i).acto <> "" Then
                        Dim acto As String = myStructList.Item(i).acto
                        acto = acto.Replace("'", "")
                        acto = acto.Replace("""", "")
                        acto = acto.Replace("/", "")
                        acto = acto.Replace("\", "")
                        myStructList.Item(i).acto = acto
                    End If
                    If myStructList.Item(i).noticia <> "" Then
                        Dim noticia As String = myStructList.Item(i).noticia
                        noticia = noticia.Replace("'", "")
                        noticia = noticia.Replace("""", "")
                        noticia = noticia.Replace("/", "")
                        noticia = noticia.Replace("\", "")
                        myStructList.Item(i).noticia = noticia
                    End If
                Next
            End If
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.dato_lista = ""
            parameter_gestion.structure_lis = myStructList
            parameter_gestion.recibo = HttpContext.Current.Session.Item("SII_RECIBO")
            parameter_gestion.codigo = HttpContext.Current.Session.Item("SII_COD_BARRAS")
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
End Class