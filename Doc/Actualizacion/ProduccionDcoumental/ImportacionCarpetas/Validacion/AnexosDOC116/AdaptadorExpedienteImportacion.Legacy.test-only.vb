Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Web

'Composicion real separada del nucleo probado con dobles. No abre endpoint.
Public Partial Class AdaptadorExpedienteImportacion
    Public Sub New(http As HttpContext, conservarStaging As Func(Of IDisposable))
        If http Is Nothing OrElse http.Session Is Nothing Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesión de Gestión vigente.")
        If conservarStaging Is Nothing Then Throw New ArgumentNullException("conservarStaging")
        _conservar = conservarStaging
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        'El checkpoint vuelve al mismo repositorio/origen capturado al componer
        'la fase, aunque Session cambie o la autoridad se revoque despues.
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        _autorizar = Sub(comando)
                         autoridad.AutorizarCreacionLegacy(http, comando)
                         Doc116InternalFaultCandidate.ExigirAlcance(comando)
                     End Sub
        _catalogos = Function(comando) New PerfilImportacionCatalogAdapter(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), ModuleSessionConnectionStringResolver.Resolve(http, "DA_")).PrepararDatosCreacionLegacy(comando)
        _iniciar = AddressOf repositorio.IniciarCreacionExpediente
        _invocar = AddressOf InvocarConservacion
        _verificar = AddressOf repositorio.ComprobarResultadoCreacion
        _registrar = Function(comando, resultado) Doc116InternalFaultCandidate.Registrar(repositorio, comando, resultado)
    End Sub

    Private Shared Sub InvocarConservacion(comando As ComandoCreacionExpedienteImportacion, datos As Dictionary(Of String, String), salida As SalidaCreacionExpedienteLegacy)
        If comando.EsVerificacion Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Un comando de verificacion no puede ejecutar creacion.")
        Dim contexto = comando.Contexto, c = comando.Snapshot.Configuracion
        Dim expedienteId As Integer = 0, relacionId As Integer = 0
        Try
            salida.Respuesta = New ClassGaExpediente().Registrar_Expediente_Conservacion(
                id_usuario_gestion:=Integer.Parse(contexto.UsuarioRef, CultureInfo.InvariantCulture),
                codigo_unico:=comando.Codigo, estado_codigo_unico:=1,
                id_empresa:=Integer.Parse(contexto.AmbitoRef, CultureInfo.InvariantCulture),
                fecha_extrema_incial:=comando.FechaInicial, fecha_extrema_final:=If(comando.FechaFinal, ""),
                rango_extremo_inicial:="", rango_extremo_final:="", tema_unidad_conservacion:=comando.Nombre,
                nombre_organigrama:=datos("ORGANIGRAMA_NOMBRE"), nombre_area:=datos("AREA_NOMBRE"),
                nombre_serie:=datos("SERIE_NOMBRE"), nombre_sub_serie:=If(datos("SUBSERIE_NOMBRE"), ""),
                id_tipo_expediente:=Integer.Parse(c.TipoExpedienteRef, CultureInfo.InvariantCulture),
                numero_documento_digitalizado:=0, numero_folios_fisicos:=0, numero_documentos_electronicos:=0,
                asunto_expediente:="", volumen_expediente:=1, nombre_tipo_unidad_conservacion:="", id_expediente:=expedienteId,
                observacion:="", nombre_tipo_unidad_documental:=datos("UNIDAD_DOCUMENTAL_NOMBRE"), nombre_sub_seccion:="",
                opcio_requiere_unidad_contenedora:=0, tipo_unidad_contendora:="", id_unidad_contendora:=0, requiere_unidad_fisica:=0,
                nombre_ciclo_documental:=datos("CICLO_ARCHIVO_NOMBRE"), nombre_fondo_documental:=datos("FONDO_NOMBRE"),
                nombre_persona_expediente:="", indentificacion_persona_expediente:="",
                nombre_responsable:=If(datos("RESPONSABLE_NOMBRE"), ""), identificacion_responsable:=If(datos("RESPONSABLE_IDENTIFICACION"), ""),
                aleas_expediente:=comando.Nombre, expediente_relacion_producion:=0, Estado_Publico_Sub_Expediente:=2,
                id_instrumento:=Integer.Parse(c.InstrumentoRef, CultureInfo.InvariantCulture), nombre_gabinete:=datos("GABINETE_NOMBRE"),
                id_nivel_padre:=Integer.Parse(contexto.NivelRef, CultureInfo.InvariantCulture), id_registro_relacion:=relacionId, id_auto_registro:=0)
        Finally
            salida.ExpedienteId = expedienteId : salida.RelacionNivelId = relacionId
        End Try
    End Sub
End Class
