Option Strict On
Imports System
Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Services

<WebService(Namespace:="urn:docuarchi:importacion-excel"), WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1), ScriptService>
Public Class WebServiceImportacionExcel
    Inherits WebService

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function AbrirCargaExcel() As RespuestaAperturaExcel
        Return Ejecutar(Of RespuestaAperturaExcel)(
            Function()
                ImportacionExcelRequestGuard.ValidarTransporte(Context)
                Dim ctx = New ImportacionExcelContextAdapter().Resolver(Context)
                Dim adaptador As New ImportacionExcelConexionAdapter()
                Dim datos As New AperturaImportacionExcel With {
                    .MaxFilasDatos = adaptador.Limite("MaxFilasDatos", 10000),
                    .MaxArchivoMB = adaptador.Limite("MaxArchivoMB", 10),
                    .Conexiones = adaptador.Referencias(Context, ctx)}
                datos.CsrfToken = ImportacionExcelRequestGuard.Emitir(Context, ctx)
                Return New RespuestaAperturaExcel With {.datos = datos}
            End Function)
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ListarPlantillas(solicitud As SolicitudListaPlantillasExcel) As RespuestaListaPlantillasExcel
        Return Ejecutar(Of RespuestaListaPlantillasExcel)(
            Function()
                Dim ctx = ContextoVerificado()
                If solicitud Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("SOLICITUD_INVALIDA")
                Dim servicio = Componer(ctx, solicitud.ConexionId)
                Return New RespuestaListaPlantillasExcel With {.datos = servicio.Listar(ctx, solicitud)}
            End Function)
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarPlantilla(solicitud As SolicitudDetallePlantillaExcel) As RespuestaPlantillaExcel
        Return Ejecutar(Of RespuestaPlantillaExcel)(
            Function()
                Dim ctx = ContextoVerificado()
                If solicitud Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("SOLICITUD_INVALIDA")
                Return New RespuestaPlantillaExcel With {.datos = Componer(ctx, solicitud.ConexionId).Obtener(ctx, solicitud.PlantillaId)}
            End Function)
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function GuardarPlantilla(solicitud As SolicitudGuardarPlantillaExcel) As RespuestaPlantillaExcel
        Return Ejecutar(Of RespuestaPlantillaExcel)(
            Function()
                Dim ctx = ContextoVerificado()
                If solicitud Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("SOLICITUD_INVALIDA")
                Return New RespuestaPlantillaExcel With {.datos = Componer(ctx, solicitud.ConexionId).Guardar(ctx, solicitud)}
            End Function)
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function IniciarRecepcionExcel(solicitud As SolicitudInicioRecepcionExcel) As RespuestaInicioRecepcionExcel
        Return Ejecutar(Of RespuestaInicioRecepcionExcel)(Function()
                                                            ContextoVerificado()
                                                            If solicitud Is Nothing Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
                                                            Dim servicios As New ComposicionCargaExcel(Context, solicitud.ConexionId)
                                                            Return New RespuestaInicioRecepcionExcel With {.datos = servicios.Recepcion.Iniciar(solicitud)}
                                                        End Function)
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function Preparar(solicitud As SolicitudPrepararExcel) As RespuestaPreparacionExcel
        Return Ejecutar(Of RespuestaPreparacionExcel)(Function()
                                                        ContextoVerificado()
                                                        If solicitud Is Nothing Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
                                                        Dim servicios As New ComposicionCargaExcel(Context, solicitud.ConexionId)
                                                        Dim datos = servicios.Preparacion.Preparar(solicitud)
                                                        Return New RespuestaPreparacionExcel With {.datos = datos, .codigo = datos.Codigo}
                                                    End Function)
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarCargaExcel(solicitud As SolicitudConsultaCargaExcel) As RespuestaEstadoCargaExcel
        Return Ejecutar(Of RespuestaEstadoCargaExcel)(Function()
                                                        ContextoVerificado()
                                                        If solicitud Is Nothing Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
                                                        Dim servicios As New ComposicionCargaExcel(Context, solicitud.ConexionId)
                                                        Return New RespuestaEstadoCargaExcel With {.datos = servicios.Recepcion.Consultar(solicitud)}
                                                    End Function)
    End Function
    Private Function ContextoVerificado() As ContextoImportacionExcel
        ImportacionExcelRequestGuard.ValidarTransporte(Context)
        Dim ctx = New ImportacionExcelContextAdapter().Resolver(Context)
        ImportacionExcelRequestGuard.Validar(Context, ctx)
        Return ctx
    End Function
    Private Function Componer(ctx As ContextoImportacionExcel, id As UInteger) As ServicioPlantillasExcel
        Dim adaptador As New ImportacionExcelConexionAdapter()
        'La configuración inválida de límites cierra también las operaciones posteriores.
        adaptador.Limite("MaxFilasDatos", 10000)
        adaptador.Limite("MaxArchivoMB", 10)
        Return New ServicioPlantillasExcel(adaptador.Resolver(Context, ctx, id), id)
    End Function
    Private Function Ejecutar(Of T As {RespuestaImportacionExcel, New})(accion As Func(Of T)) As T
        Dim operacion = Guid.NewGuid().ToString("N")
        Try
            Dim result = accion()
            If String.IsNullOrEmpty(result.codigo) Then result.codigo = "OK"
            result.ok = result.codigo = "OK"
            result.mensaje = If(result.ok, "Operación completada.", "Revise las incidencias de la preparación.")
            result.operacionId = operacion
            Return result
        Catch ex As ImportacionExcelException
            Return New T With {.ok = False, .codigo = ex.Codigo, .mensaje = ex.Message, .operacionId = operacion}
        Catch ex As Exception
            'Sin mensajes del proveedor o configuración en respuesta o trazas.
            Return New T With {.ok = False, .codigo = "CAPACIDAD_NO_DISPONIBLE", .mensaje = "La operación no está disponible.", .operacionId = operacion}
        End Try
    End Function
End Class
