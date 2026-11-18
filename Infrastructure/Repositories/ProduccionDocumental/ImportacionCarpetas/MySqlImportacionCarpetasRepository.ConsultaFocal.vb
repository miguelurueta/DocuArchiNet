Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Implements IConsultaFocalImportacionRepository, IConsultaFocalVersionadaImportacionRepository

    Public Function ConsultarRecursoFocal(contexto As ContextoImportacionCarpetas, loteId As String, tipo As String, recursoId As String) As VistaConsultaFocalImportacion Implements IConsultaFocalImportacionRepository.ConsultarRecursoFocal
        Return ConsultarRecursoFocalVersionado(contexto, loteId, tipo, recursoId, 1)
    End Function

    Public Function ConsultarRecursoFocalVersionado(contexto As ContextoImportacionCarpetas, loteId As String, tipo As String, recursoId As String, version As Integer) As VistaConsultaFocalImportacion Implements IConsultaFocalVersionadaImportacionRepository.ConsultarRecursoFocalVersionado
        If version <> 1 AndAlso version <> 2 Then Throw SolicitudInvalida()
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(recursoId)
        If Not {"RAICES", "ARCHIVOS"}.Contains(tipo) Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
            Dim lectura = LeerEstadoConsulta(conexion, tx, contexto, loteId), lote = lectura.Lote
            Dim vista As New VistaConsultaFocalImportacion With {.VersionLectura = version, .Lectura = lectura}
            Dim raizId = recursoId
            If tipo = "ARCHIVOS" Then
                Using cmd = ComandoOperacion(conexion, tx, "SELECT f.*,r.cliente_raiz_id AS cliente_raiz,r.datos_doc AS datos_raiz FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote AND f.archivo_id=@archivo", "@lote", IdOperacion(loteId), "@archivo", IdOperacion(recursoId))
                    Using reader = EjecutarLecturaControl(cmd)
                        If Not reader.Read() Then Throw AccesoDenegado()
                        vista.Archivo = LeerArchivoRecepcion(reader) : raizId = vista.Archivo.RaizId
                    End Using
                End Using
            End If
            Dim consultar As Func(Of String, Object(), DataTable) = Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)
            Dim pagina = LeerRaicesConsulta(lote, New FiltroConsultaImportacion With {.Pagina = 1, .TamanoPagina = 1, .RaizId = raizId}, consultar)
            If pagina.Elementos.Count <> 1 Then Throw AccesoDenegado()
            vista.Raiz = pagina.Elementos(0)
            CompletarRetomarCreacionConsulta(lote, pagina, consultar)
            If version = 2 Then
                CompletarRecepcionIndividualConsulta(lote, pagina, consultar)
                If vista.Archivo IsNot Nothing AndAlso lote.ControlArchivos IsNot Nothing Then VerificarArchivoFocal(conexion, tx, lote, vista.Raiz, vista.Archivo)
                Return vista
            End If
            'Mientras quede una recepcion pendiente no hace falta reconstruir
            'el plan/barrera global para anunciar que aun no se puede crear.
            If Not lote.DetencionSolicitada AndAlso vista.Raiz.Estado = "PENDIENTE" AndAlso vista.Raiz.IntentoActivoId Is Nothing AndAlso vista.Raiz.IdentidadExpediente Is Nothing Then
                Dim ambitoCompleto = lote.Snapshot.Configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso lote.Snapshot.Configuracion.AmbitoIntegridad = "LOTE"
                Dim pendiente = consultar("SELECT EXISTS(SELECT 1 FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote" & If(ambitoCompleto, "", " AND r.raiz_id=@raiz") & " AND f.estado_recepcion IN ('PENDIENTE','RECIBIENDO','INCIERTO') AND f.estado_efecto NOT IN ('CANCELADO','RECHAZADO')) AS pendiente", If(ambitoCompleto, New Object() {"@lote", IdOperacion(loteId)}, New Object() {"@lote", IdOperacion(loteId), "@raiz", IdOperacion(raizId)}))
                If pendiente.Rows.Count <> 1 Then Throw NoDisponible()
                If Not Convert.ToBoolean(pendiente.Rows(0)("pendiente"), CultureInfo.InvariantCulture) Then
                    CompletarRecepcionCreacionConsulta(lote, CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False), pagina, consultar)
                End If
            End If
            CompletarHuellaFocal(conexion, tx, lote, vista)
            Return vista
        End Function, IsolationLevel.RepeatableRead)
    End Function

    Private Shared Sub CompletarHuellaFocal(conexion As MySqlConnection, tx As MySqlTransaction, lote As LoteImportacionCarpetas, vista As VistaConsultaFocalImportacion)
        Dim sql = "SELECT 'R' AS tipo,r.raiz_id AS recurso,r.raiz_id AS raiz,r.cliente_raiz_id AS cliente,r.revision,r.estado,'' AS recepcion,(r.identidad_expediente_doc IS NOT NULL) AS confirmado,0 AS datos,0 AS bytes FROM ra_integracion_raiz r WHERE r.lote_id=@lote UNION ALL SELECT 'A',f.archivo_id,f.raiz_id,f.cliente_archivo_id,f.revision,f.estado_efecto,f.estado_recepcion,(f.resultado_doc IS NOT NULL),(f.valores_efectivos_doc IS NOT NULL),IFNULL(f.bytes_recibidos,0) FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote"
        Dim datos = LeerTablaConsulta(conexion, tx, sql, New Object() {"@lote", IdOperacion(lote.LoteId)})
        ProyectarVersionesFocal(lote, vista, datos)
    End Sub

    Friend Shared Sub ProyectarVersionesFocal(lote As LoteImportacionCarpetas, vista As VistaConsultaFocalImportacion, datos As DataTable)
        Dim registros As New List(Of String)(), progreso As New ProgresoConfirmadoImportacion(), raices = 0, archivos = 0, bytes As ULong = 0UL
        If datos Is Nothing Then Throw NoDisponible()
        Dim identidades As New HashSet(Of String)(StringComparer.Ordinal), padres As New HashSet(Of String)(StringComparer.Ordinal), hijos As New HashSet(Of String)(StringComparer.Ordinal)
        For Each r As DataRow In datos.Rows
            Dim tipo = CStr(r("tipo")), id = Convert.ToString(r("recurso"), CultureInfo.InvariantCulture), raiz = Convert.ToString(r("raiz"), CultureInfo.InvariantCulture), cliente = CStr(r("cliente")), revision = Convert.ToUInt32(r("revision"))
            IdOperacion(id) : IdOperacion(raiz)
            If Not CorrelacionValida(cliente) OrElse revision = 0 Then Throw NoDisponible()
            If Not {"R", "A"}.Contains(tipo) OrElse Not identidades.Add(tipo & ":" & id) Then Throw NoDisponible()
            If tipo = "R" Then
                If Not EstadosImportacionCarpetas.EsRaiz(CStr(r("estado"))) Then Throw NoDisponible()
                padres.Add(id)
            Else
                If Not EstadosImportacionCarpetas.EsEfecto(CStr(r("estado"))) OrElse Not EstadosImportacionCarpetas.EsRecepcion(CStr(r("recepcion"))) Then Throw NoDisponible()
                hijos.Add(raiz)
            End If
            registros.Add(tipo & ":" & id & ":" & If(tipo = "A", raiz & ":", "") & cliente & ":" & revision.ToString(CultureInfo.InvariantCulture))
            If tipo = "R" Then
                raices += 1
                If Convert.ToBoolean(r("confirmado"), CultureInfo.InvariantCulture) Then progreso.expedientes += 1
            Else
                archivos += 1
                Dim recepcion = CStr(r("recepcion")), estado = CStr(r("estado")), recibido = Convert.ToUInt64(r("bytes"), CultureInfo.InvariantCulture)
                If recibido > ULong.MaxValue - bytes Then Throw NoDisponible()
                bytes += recibido
                If recepcion = "RECIBIDO" Then progreso.recibidos += 1
                If recepcion = "PENDIENTE" Then progreso.pendientesRecepcion += 1
                If Convert.ToBoolean(r("datos"), CultureInfo.InvariantCulture) Then progreso.datos += 1
                If {"ALMACENADO", "COMPLETADO"}.Contains(estado) AndAlso Convert.ToBoolean(r("confirmado"), CultureInfo.InvariantCulture) Then progreso.guardados += 1
                If estado = "RECHAZADO" OrElse recepcion = "RECHAZADO" Then progreso.incidencias += 1
            End If
        Next
        If hijos.Any(Function(id) Not padres.Contains(id)) Then Throw NoDisponible()
        If raices <> lote.TotalRaices OrElse archivos <> lote.TotalArchivos OrElse bytes > BytesDecimales(lote.TotalBytes) Then Throw NoDisponible()
        registros.Sort(StringComparer.Ordinal)
        vista.HuellaRecursos = PlanSegmentadoImportacionJson.HuellaTexto("DOC120_RECURSOS_1" & vbLf & lote.LoteId & vbLf & String.Join(vbLf, registros) & vbLf)
        progreso.bytesRecibidos = bytes.ToString(CultureInfo.InvariantCulture) : vista.Progreso = progreso
    End Sub
End Class
