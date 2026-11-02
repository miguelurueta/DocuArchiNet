Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Public Partial Class CoordinadorImportacionCarpetas
    'Paso interno anterior a reservar: no llama al adaptador documental.
    Friend Shared Function ReservarCreacionConStaging(vista As VistaBarreraCreacionImportacion, solicitud As ReclamacionImportacionCarpetas, cargar As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), reservar As Func(Of ContextoImportacionCarpetas, ReclamacionImportacionCarpetas, IntentoImportacionCarpetas)) As ReservaCreacionVerificadaImportacion
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse cargar Is Nothing OrElse staging Is Nothing OrElse autorizar Is Nothing OrElse reservar Is Nothing Then Throw BarreraInvalida()
        If solicitud Is Nothing OrElse solicitud.LoteId <> vista.Lote.LoteId OrElse solicitud.RecursoId <> vista.Raiz.RaizId OrElse solicitud.RecursoTipo <> "RAIZ" OrElse solicitud.Fase <> "CREAR_EXPEDIENTE" OrElse solicitud.RevisionEsperada = 0 OrElse solicitud.RevisionEsperada <> vista.Raiz.Revision OrElse Not HexBarrera(solicitud.TokenReclamacion, 32) OrElse Not HexBarrera(solicitud.SolicitudHash, 64) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La reserva no corresponde a la raíz revisada.")
        Dim comando = PerfilImportacionJson.Leer(Of ReclamacionImportacionCarpetas)(PerfilImportacionJson.Escribir(solicitud))
        If vista.Raiz.Estado <> "PENDIENTE" OrElse vista.Raiz.IntentoActivoId IsNot Nothing Then Throw StagingCreacionOcupado()
        Dim salida = VerificarStagingCreacion(vista, cargar, staging, autorizar, Nothing)
        autorizar(vista.Lote.Contexto, vista.Lote.Snapshot.Configuracion)
        Dim intento = reservar(vista.Lote.Contexto, comando)
        If intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intento.LoteId <> comando.LoteId OrElse intento.RaizId <> comando.RecursoId OrElse intento.ArchivoId IsNot Nothing OrElse intento.Fase <> "CREAR_EXPEDIENTE" OrElse intento.SolicitudHash <> comando.SolicitudHash OrElse Not HexBarrera(intento.TokenReclamacion, 32) OrElse Not EstadosImportacionCarpetas.EsIntento(intento.Estado) Then Throw StagingCreacionOcupado()
        If intento.EsNuevaReserva AndAlso (intento.Estado <> "RESERVADO" OrElse intento.TokenReclamacion <> comando.TokenReclamacion OrElse intento.Resultado IsNot Nothing OrElse intento.FinUtc IsNot Nothing) Then Throw StagingCreacionOcupado()
        salida.Intento = intento
        Return salida
    End Function

    Friend Shared Function ConservarStagingCreacion(vista As VistaBarreraCreacionImportacion, cargar As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)) As RetencionesStagingCreacionImportacion
        Dim retenciones As New List(Of IRetencionVerificadaImportacion)()
        Try
            Dim pruebas = VerificarStagingCreacion(vista, cargar, staging, autorizar, retenciones)
            Return New RetencionesStagingCreacionImportacion(retenciones, pruebas)
        Catch
            Dim cierre As New RetencionesStagingCreacionImportacion(retenciones, Nothing)
            cierre.Dispose()
            Throw
        End Try
    End Function

    Private Shared Function VerificarStagingCreacion(vista As VistaBarreraCreacionImportacion, cargar As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), retenciones As List(Of IRetencionVerificadaImportacion), Optional objetivo As ArchivoImportacionCarpetas = Nothing, Optional intentoPropio As String = Nothing) As ReservaCreacionVerificadaImportacion
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse cargar Is Nothing OrElse staging Is Nothing OrElse autorizar Is Nothing Then Throw BarreraInvalida()
        If vista.Lote.Contexto Is Nothing OrElse vista.Lote.Snapshot Is Nothing OrElse vista.Lote.Snapshot.Configuracion Is Nothing Then Throw BarreraInvalida()
        Dim contexto = vista.Lote.Contexto
        autorizar(contexto, vista.Lote.Snapshot.Configuracion)
        If objetivo Is Nothing Then
            If Not EvaluarBarreraCreacion(vista.Lote, vista.Plan, vista.Raiz, vista.Raices, vista.Archivos) Then Throw StagingCreacionOcupado()
        ElseIf Not EvaluarBarreraAlmacenamiento(vista, objetivo, intentoPropio) Then
            Throw New PerfilImportacionException("OPERACION_EN_CURSO", "La unidad no tiene una recepcion disponible para almacenar.")
        End If
        Dim parcial = vista.Plan.Snapshot.Configuracion.ModoIntegridad = "PARCIAL_PERMITIDO"
        Dim raices = vista.Raices.ToDictionary(Function(r) r.RaizId, Function(r) r.ClienteRaizId, StringComparer.Ordinal)
        Dim originales = vista.Plan.Manifiesto.Archivos.ToDictionary(Function(a) a.RaizIdCliente & ":" & a.IdCliente, StringComparer.Ordinal)
        Dim candidatos = vista.Archivos.Where(Function(a) (objetivo Is Nothing OrElse Not parcial OrElse a.ArchivoId = objetivo.ArchivoId) AndAlso RecepcionRegistradaValida(a, If(objetivo IsNot Nothing AndAlso a.ArchivoId = objetivo.ArchivoId, intentoPropio, Nothing)) AndAlso ServicioPreparacionImportacion.EsArchivoElegible(vista.Plan, originales(raices(a.RaizId) & ":" & a.ClienteArchivoId))).OrderBy(Function(a) UInteger.Parse(a.ArchivoId, CultureInfo.InvariantCulture)).Select(Function(a) PerfilImportacionJson.Leer(Of ArchivoImportacionCarpetas)(PerfilImportacionJson.Escribir(a))).ToList()
        Dim salida As New ReservaCreacionVerificadaImportacion()
        For Each metadatos In candidatos
            autorizar(contexto, vista.Lote.Snapshot.Configuracion)
            Try
                Dim archivo = cargar(contexto, vista.Lote.LoteId, metadatos.ArchivoId)
                Dim original = originales(raices(metadatos.RaizId) & ":" & metadatos.ClienteArchivoId)
                If archivo Is Nothing OrElse archivo.ArchivoId <> metadatos.ArchivoId OrElse archivo.RaizId <> metadatos.RaizId OrElse archivo.ClienteArchivoId <> metadatos.ClienteArchivoId OrElse archivo.Revision = 0 OrElse archivo.Revision <> metadatos.Revision OrElse archivo.BytesPrevistos <> metadatos.BytesPrevistos OrElse archivo.BytesRecibidos <> metadatos.BytesRecibidos OrElse archivo.ContenidoHash <> metadatos.ContenidoHash OrElse archivo.StagingRef <> metadatos.StagingRef OrElse archivo.EstadoEfecto <> metadatos.EstadoEfecto OrElse archivo.IntentoActivoId <> metadatos.IntentoActivoId OrElse Not RecepcionRegistradaValida(archivo, If(objetivo IsNot Nothing AndAlso archivo.ArchivoId = objetivo.ArchivoId, intentoPropio, Nothing)) OrElse archivo.NombreOriginal <> original.Nombre OrElse archivo.RutaRelativa <> original.RutaRelativa Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "La recepción cambió durante la revisión previa.")
                Dim almacen = staging(vista.Lote, archivo)
                If almacen Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el staging privado.")
                Dim retenido As RecepcionVerificadaImportacion
                If retenciones Is Nothing Then
                    retenido = almacen.VerificarRetenido(contexto)
                Else
                    Dim guardia = almacen.ConservarVerificado(contexto)
                    If guardia Is Nothing Then Throw StagingCreacionIncierto()
                    Try
                        retenido = guardia.Recepcion
                        If Not MismaRecepcionStaging(retenido, archivo) Then Throw StagingCreacionIncierto()
                        retenciones.Add(guardia) : guardia = Nothing
                    Finally
                        If guardia IsNot Nothing Then guardia.Dispose()
                    End Try
                End If
                If retenido Is Nothing OrElse retenido.StagingRef <> archivo.StagingRef OrElse retenido.BytesRecibidos <> archivo.BytesRecibidos OrElse retenido.ContenidoHash <> archivo.ContenidoHash Then Throw StagingCreacionIncierto()
                salida.ArchivosVerificados.Add(archivo.ArchivoId)
            Catch ex As PerfilImportacionException When parcial AndAlso objetivo Is Nothing AndAlso {"RESULTADO_INCIERTO", "ARCHIVO_RECHAZADO", "CONTENIDO_DISTINTO"}.Contains(ex.Codigo)
                salida.ArchivosInciertos.Add(metadatos.ArchivoId)
            Catch ex As PerfilImportacionException
                Throw
            Catch ex As Exception
                Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el staging privado.")
            End Try
        Next
        If salida.ArchivosVerificados.Count = 0 Then Throw StagingCreacionIncierto()
        autorizar(contexto, vista.Lote.Snapshot.Configuracion)
        Return salida
    End Function

    Private Shared Function MismaRecepcionStaging(retenido As RecepcionVerificadaImportacion, archivo As ArchivoImportacionCarpetas) As Boolean
        Return retenido IsNot Nothing AndAlso retenido.StagingRef = archivo.StagingRef AndAlso retenido.BytesRecibidos = archivo.BytesRecibidos AndAlso retenido.ContenidoHash = archivo.ContenidoHash
    End Function

    Private Shared Function StagingCreacionOcupado() As PerfilImportacionException
        Return New PerfilImportacionException("OPERACION_EN_CURSO", "La raíz no tiene una recepción disponible para reservar creación.")
    End Function
    Private Shared Function StagingCreacionIncierto() As PerfilImportacionException
        Return New PerfilImportacionException("RESULTADO_INCIERTO", "El staging no pudo comprobarse; no se autoriza reservar creación.")
    End Function
End Class

Public NotInheritable Class RetencionesStagingCreacionImportacion
    Implements IDisposable
    Private ReadOnly _retenciones As List(Of IRetencionVerificadaImportacion)
    Private _cerrado As Boolean
    Public ReadOnly Property Resultado As ReservaCreacionVerificadaImportacion
    Friend Sub New(retenciones As List(Of IRetencionVerificadaImportacion), resultado As ReservaCreacionVerificadaImportacion)
        _retenciones = retenciones : Me.Resultado = resultado
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        If _cerrado Then Return
        _cerrado = True
        Dim errorCierre As Exception = Nothing
        For Each retenido In _retenciones
            Try
                retenido.Dispose()
            Catch ex As Exception
                If errorCierre Is Nothing Then errorCierre = ex
            End Try
        Next
        _retenciones.Clear()
        If errorCierre IsNot Nothing Then Throw errorCierre
    End Sub
End Class
