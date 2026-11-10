Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.Threading

Public Partial Class AdaptadorDocumentoImportacion
    Private _ejecucionV2 As Integer

    Friend Shared Function EnlazarV2(adaptador As AdaptadorDocumentoImportacion, persistencia As PersistenciaDocumentoV2Importacion, obtenerLote As Func(Of ContextoImportacionCarpetas, String, LoteImportacionCarpetas)) As IAlmacenamientoDocumentoImportacion
        If adaptador Is Nothing OrElse persistencia Is Nothing OrElse obtenerLote Is Nothing Then Throw New ArgumentNullException()
        Return New EnlaceDocumentoV2(adaptador, persistencia, obtenerLote)
    End Function

    'El coordinador resuelve historial antes de crear este enlace. La revisión
    'se obtiene del mismo repositorio; no se toma de HTTP ni del staging.
    Private NotInheritable Class EnlaceDocumentoV2
        Implements IAlmacenamientoDocumentoImportacion
        Private ReadOnly _adaptador As AdaptadorDocumentoImportacion
        Private ReadOnly _persistencia As PersistenciaDocumentoV2Importacion
        Private ReadOnly _obtenerLote As Func(Of ContextoImportacionCarpetas, String, LoteImportacionCarpetas)
        Public Sub New(adaptador As AdaptadorDocumentoImportacion, persistencia As PersistenciaDocumentoV2Importacion, obtenerLote As Func(Of ContextoImportacionCarpetas, String, LoteImportacionCarpetas))
            _adaptador = adaptador : _persistencia = persistencia : _obtenerLote = obtenerLote
        End Sub
        Public Function Almacenar(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion Implements IAlmacenamientoDocumentoImportacion.Almacenar
            Dim comando = ComandoAlmacenamientoCarpetas.Preparar(contexto, raiz, archivo, snapshot, intento)
            _adaptador._autorizar(comando)
            Dim obtenido = _obtenerLote(Copiar(comando.Contexto), comando.Intento.LoteId)
            If obtenido Is Nothing Then Throw LoteIncompatible()
            Dim lote = Copiar(obtenido)
            If lote.LoteId <> comando.Intento.LoteId OrElse lote.Estado <> "EN_CURSO" OrElse lote.DetencionSolicitada OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI OrElse PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) Then Throw LoteIncompatible()
            Return _adaptador.AlmacenarV2(contexto, raiz, archivo, snapshot, intento, lote.Revision, _persistencia)
        End Function
        Private Shared Function LoteIncompatible() As PerfilImportacionException
            Return New PerfilImportacionException("CONFLICTO_REVISION", "El lote ya no coincide con la solicitud documental.")
        End Function
    End Class

    'Recorrido V2 utilizado por el enlace tipado de la composición documental.
    'Reutiliza los puertos existentes; jamás llama _iniciar/_registrar de V1.
    Friend Function AlmacenarV2(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas, revisionLote As UInteger, persistencia As PersistenciaDocumentoV2Importacion) As ResultadoEfectoImportacion
        If persistencia Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La persistencia V2 no está compuesta.")
        Dim proveedor = TryCast(_staging, IAlmacenStagingV2Importacion)
        If proveedor Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El proveedor no acredita retención V2.")
        Dim comando = ComandoAlmacenamientoCarpetas.Preparar(contexto, raiz, archivo, snapshot, intento)
        If Interlocked.CompareExchange(_ejecucionV2, 1, 0) <> 0 Then Throw New PerfilImportacionException("OPERACION_EN_CURSO", "Esta instancia ya consumió su ejecución V2.")
        'La denegación inicial se propaga igual que en V1, antes de devolver
        'evidencia del expediente o adquirir una retención para esta operación.
        _autorizar(comando)
        Dim datos = Copiar(_datos(comando))
        comando.ValidarDatos(datos)
        Dim resultado = ResultadoInciertoV2(comando)
        Dim retenido As IRetencionOriginalV2Importacion = Nothing
        Try
            retenido = proveedor.ConservarOriginalV2(comando.Contexto)
            If retenido Is Nothing Then Throw AcuseInvalidoV2()
            retenido.Validar()
            Dim preparado = DocumentosAlmacenamientoV2Importacion.Preparar(comando, retenido.Original, revisionLote)
            _autorizar(comando)
            retenido.Validar()
            'Consumir también ante respuesta de inicio perdida; nunca reintentar.
            intento.EsNuevaReserva = False
            Dim iniciado = ExigirAcuseV2(persistencia.Iniciar(comando, Copiar(datos), Copiar(preparado)), preparado, "PREPARADO")
            retenido.Validar()
            _autorizar(comando)
            Dim copia = _staging.CrearCopiaTrabajo(comando.Contexto, comando.Archivo.StagingRef, comando.Archivo.NombreOriginal)
            VerificarCopia(comando, copia)
            retenido.Validar()
            _autorizar(comando)
            Dim autorizado = ExigirAcuseV2(persistencia.AutorizarInvocacion(comando, Copiar(iniciado)), iniciado, "AUTORIZADO")
            retenido.Validar()
            _autorizar(comando)
            Dim salida As New SalidaAlmacenamientoLegacy()
            Dim datosInvocacion = Copiar(datos)
            Dim concluida As Boolean = False, observada As Boolean = False
            Dim retorno As DocumentoAlmacenamientoV2Importacion = Nothing
            Try
                Try
                    _invocar(comando, datosInvocacion, copia, salida)
                    concluida = True
                Finally
                    'Alcanzado solo al entrar al escritor. Una interrupción no
                    'inventa retorno durable, pero conserva IDs observados.
                    If salida.DocumentoId > 0 Then resultado.DocumentoId = salida.DocumentoId.ToString(CultureInfo.InvariantCulture)
                    If salida.InventarioId > 0 Then resultado.InventarioId = salida.InventarioId.ToString(CultureInfo.InvariantCulture)
                    Try
                        retenido.Validar()
                        retorno = DocumentosAlmacenamientoV2Importacion.ConRetorno(autorizado, Copiar(resultado))
                        ExigirAcuseV2(persistencia.ObservarRetorno(comando, Copiar(retorno)), retorno, "RETORNO_OBSERVADO")
                        observada = True
                    Catch ex As Exception When Not EsInterrupcionV2(ex)
                        'Sin retry, mensajes legacy o permiso inferido del fallo.
                    End Try
                End Try
            Catch ex As Exception When Not EsInterrupcionV2(ex)
                'Se mantiene INCIERTO y cualquier ID que entregó el escritor.
            End Try
            If Not observada Then Return resultado
            'Retorno observado permanece INCIERTO: la confirmación física es una
            'comprobación posterior y no reescribe la observación del escritor.
            If concluida AndAlso salida.Respuesta = "YES" AndAlso salida.DocumentoId > 0 AndAlso salida.InventarioId > 0 AndAlso salida.NombreArchivo = comando.Archivo.NombreOriginal AndAlso salida.GabineteNombre = datos.GabineteNombre Then
                Try
                    retenido.Validar()
                    _autorizar(comando)
                    If _verificar(comando, Copiar(datos), Copiar(resultado)) Then
                        resultado.Estado = "CONFIRMADO" : resultado.Codigo = "DOCUMENTO_CONFIRMADO"
                        resultado.Mensaje = "Documento, gabinete e incorporacion comprobados en el expediente confirmado."
                    End If
                Catch ex As Exception When Not EsInterrupcionV2(ex)
                    'No atribuir certeza a una comprobación interrumpida.
                End Try
            End If
            Try
                retenido.Validar()
                _autorizar(comando)
                Dim guardado = persistencia.RegistrarResultado(comando, Copiar(retorno), Copiar(resultado))
                If guardado IsNot Nothing AndAlso PerfilImportacionJson.Escribir(guardado) = PerfilImportacionJson.Escribir(resultado) Then Return Copiar(guardado)
            Catch ex As Exception When Not EsInterrupcionV2(ex)
                'El checkpoint pudo confirmar aunque su respuesta se perdiera.
            End Try
        Catch ex As Exception When Not EsInterrupcionV2(ex)
            'Incluye acuse ausente/incoherente: cero llamadas posteriores.
        Finally
            If retenido IsNot Nothing Then retenido.Dispose()
        End Try
        resultado.Estado = "INCIERTO" : resultado.Codigo = "RESULTADO_INCIERTO"
        resultado.Mensaje = "El almacenamiento requiere comprobacion antes de cualquier repeticion."
        Return resultado
    End Function

    Private Shared Function ExigirAcuseV2(acuse As AcuseDocumentoV2Importacion, esperado As DocumentoAlmacenamientoV2Importacion, fase As String) As DocumentoAlmacenamientoV2Importacion
        If acuse Is Nothing Then Throw AcuseInvalidoV2()
        Dim recibido = acuse.Documento
        If recibido.Fase <> fase OrElse PerfilImportacionJson.Escribir(recibido.Inicio) <> PerfilImportacionJson.Escribir(esperado.Inicio) Then Throw AcuseInvalidoV2()
        If fase <> "AUTORIZADO" AndAlso DocumentosAlmacenamientoV2Importacion.Escribir(recibido) <> DocumentosAlmacenamientoV2Importacion.Escribir(esperado) Then Throw AcuseInvalidoV2()
        Return recibido
    End Function
    Private Shared Function ResultadoInciertoV2(comando As ComandoAlmacenamientoCarpetas) As ResultadoEfectoImportacion
        Return New ResultadoEfectoImportacion With {.Estado = "INCIERTO", .Codigo = "RESULTADO_INCIERTO", .Mensaje = "El almacenamiento requiere comprobacion antes de cualquier repeticion.", .Expediente = comando.Raiz.IdentidadExpediente, .OrigenGabinete = comando.Snapshot.Configuracion.OrigenGabinete, .GabineteId = comando.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture), .ValoresEfectivos = comando.Archivo.ValoresEfectivos}
    End Function
    Private Shared Function EsInterrupcionV2(ex As Exception) As Boolean
        Return TypeOf ex Is ThreadAbortException OrElse TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is AppDomainUnloadedException OrElse TypeOf ex Is OutOfMemoryException OrElse TypeOf ex Is StackOverflowException
    End Function
    Private Shared Function AcuseInvalidoV2() As PerfilImportacionException
        Return New PerfilImportacionException("EVIDENCIA_DOCUMENTAL_V2_INVALIDA", "No se confirmó la persistencia V2 esperada.")
    End Function
End Class
