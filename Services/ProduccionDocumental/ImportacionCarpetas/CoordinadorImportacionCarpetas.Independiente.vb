Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Partial Class CoordinadorImportacionCarpetas
    Private Sub PrepararArchivosIndependientes(contexto As ContextoImportacionCarpetas, loteId As String)
        Dim control = TryCast(_repositorio, IControlArchivosIndependientesImportacion)
        If control Is Nothing Then Return 'Puertos anteriores conservan contrato version1.
        Dim lote = _repositorio.ObtenerLote(contexto, loteId)
        If lote Is Nothing Then Throw Denegado()
        ReautorizarFase(contexto, lote.Snapshot.Configuracion)
        If lote.ControlArchivos IsNot Nothing OrElse {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then Return
        control.PrepararControlArchivos(contexto, loteId)
    End Sub

    Friend Shared Function EvaluarCreacionIndividual(vista As VistaBarreraCreacionImportacion) As Boolean
        If vista Is Nothing OrElse Not vista.EsFocal OrElse vista.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Lote.ControlArchivos Is Nothing Then Return False
        Dim lote = vista.Lote, raiz = vista.Raiz
        If lote.DetencionSolicitada OrElse {"COMPLETADO", "FALLIDO", "DETENIDO"}.Contains(lote.Estado) OrElse raiz.LoteId <> lote.LoteId OrElse raiz.IdentidadExpediente IsNot Nothing OrElse Not {"PENDIENTE", "CREANDO"}.Contains(raiz.Estado) Then Return False
        Return vista.Archivos IsNot Nothing AndAlso vista.Archivos.Any(Function(a) a.ProcedenciaVerificada AndAlso a.RaizId = raiz.RaizId AndAlso RecepcionRegistradaValida(a) AndAlso a.EstadoEfecto = "NO_INICIADO" AndAlso (a.Resultado Is Nothing OrElse ControlArchivoIndependienteImportacion.EsNoDisponible(a.Resultado)))
    End Function

    Friend Shared Function EvaluarBarreraIndividual(vista As VistaBarreraCreacionImportacion, archivo As ArchivoImportacionCarpetas, Optional intentoPropio As String = Nothing) As Boolean
        If vista Is Nothing OrElse Not vista.EsFocal OrElse vista.Lote Is Nothing OrElse vista.Lote.ControlArchivos Is Nothing OrElse vista.Raiz Is Nothing OrElse archivo Is Nothing OrElse Not archivo.ProcedenciaVerificada Then Return False
        Dim lote = vista.Lote, raiz = vista.Raiz
        If lote.DetencionSolicitada OrElse {"COMPLETADO", "FALLIDO", "DETENIDO"}.Contains(lote.Estado) OrElse raiz.LoteId <> lote.LoteId OrElse raiz.RaizId <> archivo.RaizId OrElse raiz.IntentoActivoId IsNot Nothing OrElse raiz.IdentidadExpediente Is Nothing OrElse Not {"EXPEDIENTE_CONFIRMADO", "PARCIAL", "INCIERTO"}.Contains(raiz.Estado) Then Return False
        If archivo.Resultado IsNot Nothing AndAlso Not ControlArchivoIndependienteImportacion.EsNoDisponible(archivo.Resultado) Then Return False
        Return RecepcionRegistradaValida(archivo, intentoPropio) AndAlso If(intentoPropio Is Nothing, archivo.EstadoEfecto = "NO_INICIADO" AndAlso archivo.IntentoActivoId Is Nothing, archivo.EstadoEfecto = "EN_CURSO" AndAlso archivo.IntentoActivoId = intentoPropio)
    End Function

    'No enumera ni abre originales hermanos; conserva el handle de un solo PDF.
    Private Shared Function VerificarOriginalIndividual(vista As VistaBarreraCreacionImportacion, cargar As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), retenciones As List(Of IRetencionVerificadaImportacion), objetivo As ArchivoImportacionCarpetas, intentoPropio As String) As ReservaCreacionVerificadaImportacion
        If objetivo Is Nothing Then
            If Not EvaluarCreacionIndividual(vista) Then Throw StagingCreacionOcupado()
            objetivo = vista.Archivos.First(Function(a) a.ProcedenciaVerificada AndAlso a.RaizId = vista.Raiz.RaizId AndAlso RecepcionRegistradaValida(a) AndAlso a.EstadoEfecto = "NO_INICIADO")
        ElseIf Not EvaluarBarreraAlmacenamiento(vista, objetivo, intentoPropio) Then
            Throw StagingCreacionOcupado()
        End If
        autorizar(vista.Lote.Contexto, vista.Lote.Snapshot.Configuracion)
        Dim almacen = staging(vista.Lote, objetivo)
        If almacen Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el original privado de este archivo.")
        Try
            Dim prueba As RecepcionVerificadaImportacion
            If retenciones Is Nothing Then
                prueba = MedicionAlmacenamientoImportacion.MedirParte("prev_io", Function() almacen.VerificarRetenido(vista.Lote.Contexto))
            Else
                Dim guardia = MedicionAlmacenamientoImportacion.MedirParte("ret_io", Function() almacen.ConservarVerificado(vista.Lote.Contexto))
                If guardia Is Nothing Then Throw StagingCreacionIncierto()
                Try
                    prueba = guardia.Recepcion
                    If Not MismaRecepcionStaging(prueba, objetivo) Then Throw New OriginalNoComprobadoImportacionException("ORIGINAL_ALTERADO")
                    retenciones.Add(guardia) : guardia = Nothing
                Finally
                    If guardia IsNot Nothing Then guardia.Dispose()
                End Try
            End If
            If Not MismaRecepcionStaging(prueba, objetivo) Then Throw New OriginalNoComprobadoImportacionException("ORIGINAL_ALTERADO")
        Catch ex As OriginalNoComprobadoImportacionException
            Throw New ArchivoNoDisponibleImportacionException(vista.Lote.LoteId, objetivo.ArchivoId, objetivo.RaizId, If(intentoPropio Is Nothing AndAlso vista.Raiz.IdentidadExpediente Is Nothing, "CREAR_EXPEDIENTE", "ALMACENAR"), ex.Motivo)
        End Try
        Dim salida As New ReservaCreacionVerificadaImportacion()
        salida.ArchivosVerificados.Add(objetivo.ArchivoId)
        Return salida
    End Function

    Private Sub RegistrarNoDisponible(contexto As ContextoImportacionCarpetas, errorOriginal As ArchivoNoDisponibleImportacionException, revision As UInteger, Optional intento As IntentoImportacionCarpetas = Nothing)
        Dim control = TryCast(_repositorio, IControlArchivosIndependientesImportacion)
        If control Is Nothing Then Throw StagingCreacionIncierto()
        ReautorizarFase(contexto, _repositorio.ObtenerLote(contexto, errorOriginal.LoteId).Snapshot.Configuracion)
        control.RegistrarOriginalNoDisponible(contexto, errorOriginal.LoteId, errorOriginal.ArchivoId, revision, errorOriginal.Causa, intento)
    End Sub
End Class
