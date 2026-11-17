Option Strict On
Imports System
Imports System.Linq

Public Partial Class ServicioConsultaImportacion
    Public Function ConsultarFocal(solicitud As ConsultarRecursoLoteSolicitud) As ConsultaFocalImportacionResultado
        If solicitud Is Nothing OrElse Not {"RAICES", "ARCHIVOS"}.Contains(solicitud.tipo) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.recursoId) Then Throw Invalida()
        Dim previa = Autorizar(solicitud), repositorio = TryCast(_repositorio, IConsultaFocalImportacionRepository)
        If repositorio Is Nothing Then Throw NoDisponible()
        Dim vista = repositorio.ConsultarRecursoFocal(previa.Lote.Contexto, solicitud.loteId, solicitud.tipo, solicitud.recursoId)
        If vista Is Nothing OrElse vista.Lectura Is Nothing OrElse vista.Lectura.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Progreso Is Nothing OrElse
            vista.Lectura.Lote.LoteId <> solicitud.loteId OrElse Not MismoDestino(previa.Lote.Contexto, vista.Lectura.Lote.Contexto) OrElse vista.Raiz.LoteId <> solicitud.loteId OrElse
            vista.Lectura.Lote.Snapshot Is Nothing OrElse vista.Lectura.Lote.Snapshot.Configuracion Is Nothing OrElse vista.Lectura.Lote.Snapshot.VersionId <> previa.Lote.Snapshot.VersionId OrElse vista.Lectura.Lote.Snapshot.PerfilId <> previa.Lote.Snapshot.PerfilId OrElse
            vista.HuellaRecursos Is Nothing OrElse Not System.Text.RegularExpressions.Regex.IsMatch(vista.HuellaRecursos, "\A[0-9a-f]{64}\z") Then Throw NoDisponible()
        _autorizar(vista.Lectura.Lote.Contexto, vista.Lectura.Lote.Snapshot.Configuracion)
        If vista.Raiz.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRaiz(vista.Raiz.Estado) OrElse vista.Lectura.Lote.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsLote(vista.Lectura.Lote.Estado) Then Throw NoDisponible()
        If vista.Archivo IsNot Nothing AndAlso (vista.Archivo.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRecepcion(vista.Archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(vista.Archivo.EstadoEfecto)) Then Throw NoDisponible()
        If solicitud.tipo = "RAICES" Then
            If vista.Raiz.RaizId <> solicitud.recursoId OrElse vista.Archivo IsNot Nothing Then Throw NoDisponible()
        ElseIf vista.Archivo Is Nothing OrElse vista.Archivo.ArchivoId <> solicitud.recursoId OrElse vista.Archivo.RaizId <> vista.Raiz.RaizId Then
            Throw NoDisponible()
        End If
        Return New ConsultaFocalImportacionResultado With {.estado = ProyectarEstado(vista.Lectura), .tipo = solicitud.tipo,
            .recurso = If(solicitud.tipo = "RAICES", ProyectarRaiz(vista.Raiz), ProyectarArchivo(vista.Archivo)),
            .padre = If(solicitud.tipo = "ARCHIVOS", ProyectarRaiz(vista.Raiz), Nothing), .huellaRecursos = vista.HuellaRecursos, .progreso = vista.Progreso}
    End Function
End Class
