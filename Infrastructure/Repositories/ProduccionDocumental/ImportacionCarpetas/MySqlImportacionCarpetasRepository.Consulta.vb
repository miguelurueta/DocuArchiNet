Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

'Parte de la misma clase/repositorio. Consultas SELECT sin FOR UPDATE;
'cada pagina tiene snapshot propio, no un snapshot global de varias peticiones.
Public Partial Class MySqlImportacionCarpetasRepository
    'Solo consulta. No confirma, libera ni toma control de una llamada incierta.
    Public Function ConsultarConfirmacion(contexto As ContextoImportacionCarpetas, planId As String) As ConfirmacionLecturaImportacion Implements IConsultaImportacionRepository.ConsultarConfirmacion
        AutorizarOperacion(contexto) : IdOperacion(planId)
        Return Operar(Function(conexion, tx)
                          Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(planId), False)
                          If plan Is Nothing Then Throw AccesoDenegado()
                          Dim lote = BuscarLote(conexion, tx, contexto, "plan_id", IdOperacion(planId))
                          If (lote IsNot Nothing AndAlso plan.Estado <> "CONFIRMADO") OrElse (lote Is Nothing AndAlso plan.Estado = "CONFIRMADO") Then Throw NoDisponible()
                          Return New ConfirmacionLecturaImportacion With {.Plan = plan, .Lote = lote}
                      End Function, IsolationLevel.RepeatableRead)
    End Function

    Public Function ConsultarEstado(contexto As ContextoImportacionCarpetas, loteId As String) As EstadoLecturaImportacion Implements IConsultaImportacionRepository.ConsultarEstado
        AutorizarOperacion(contexto) : IdOperacion(loteId)
        Return Operar(Function(conexion, tx)
                          Dim lote = LoteConsulta(conexion, tx, contexto, loteId)
                          Dim sql = "SELECT (EXISTS(SELECT 1 FROM ra_integracion_intento WHERE lote_id=@lote AND estado IN ('RESERVADO','EN_CURSO')) OR EXISTS(SELECT 1 FROM ra_integracion_raiz WHERE lote_id=@lote AND (intento_activo_id IS NOT NULL OR estado='CREANDO')) OR EXISTS(SELECT 1 FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote AND (f.intento_activo_id IS NOT NULL OR f.estado_recepcion='RECIBIENDO' OR f.estado_efecto='EN_CURSO'))) AS activos,(EXISTS(SELECT 1 FROM ra_integracion_intento WHERE lote_id=@lote AND estado='INCIERTO') OR EXISTS(SELECT 1 FROM ra_integracion_raiz WHERE lote_id=@lote AND estado='INCIERTO') OR EXISTS(SELECT 1 FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote AND (f.estado_recepcion='INCIERTO' OR f.estado_efecto='INCIERTO'))) AS inciertos"
                          Dim datos = LeerTablaConsulta(conexion, tx, sql, New Object() {"@lote", IdOperacion(loteId)})
                          If datos.Rows.Count <> 1 Then Throw NoDisponible()
                          Return New EstadoLecturaImportacion With {.Lote = lote, .HayIntentosActivos = Convert.ToBoolean(datos.Rows(0)("activos"), CultureInfo.InvariantCulture), .HayIncertidumbre = lote.Estado = "PENDIENTE_VERIFICACION" OrElse Convert.ToBoolean(datos.Rows(0)("inciertos"), CultureInfo.InvariantCulture)}
                      End Function, IsolationLevel.RepeatableRead)
    End Function

    Public Function ConsultarRaices(contexto As ContextoImportacionCarpetas, loteId As String, filtro As FiltroConsultaImportacion) As PaginaRecursosImportacion(Of RaizImportacionCarpetas) Implements IConsultaImportacionRepository.ConsultarRaices
        AutorizarOperacion(contexto) : IdOperacion(loteId) : ValidarFiltroConsulta("RAICES", filtro)
        Return Operar(Function(conexion, tx)
                          Dim lote = LoteConsulta(conexion, tx, contexto, loteId)
                          Dim consultar As Func(Of String, Object(), DataTable) = Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)
                          Dim pagina = LeerRaicesConsulta(lote, filtro, consultar)
                          CompletarRetomarCreacionConsulta(lote, pagina, consultar)
                          If Not lote.DetencionSolicitada AndAlso pagina.Elementos.Any(Function(r) r.Estado = "PENDIENTE" AndAlso r.IntentoActivoId Is Nothing AndAlso r.IdentidadExpediente Is Nothing) Then
                              Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
                              CompletarRecepcionCreacionConsulta(lote, plan, pagina, consultar)
                          End If
                          Return pagina
                      End Function, IsolationLevel.RepeatableRead)
    End Function

    'Mismo snapshot de lectura, sin filtros de archivos ni FOR UPDATE. La pista
    'reutiliza la barrera vigente; no verifica disco, reserva ni ejecuta efectos.
    Friend Shared Sub CompletarRecepcionCreacionConsulta(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, pagina As PaginaRecursosImportacion(Of RaizImportacionCarpetas), consultar As Func(Of String, Object(), DataTable))
        If lote Is Nothing OrElse plan Is Nothing OrElse pagina Is Nothing OrElse consultar Is Nothing Then Throw NoDisponible()
        For Each raiz In pagina.Elementos
            raiz.RecepcionListaParaCrear = False
        Next
        If lote.DetencionSolicitada Then Return
        Dim pendientes = pagina.Elementos.Where(Function(r) r.Estado = "PENDIENTE" AndAlso r.IntentoActivoId Is Nothing AndAlso r.IdentidadExpediente Is Nothing).ToList()
        If pendientes.Count = 0 Then Return
        Dim ambitoLote = lote.Snapshot.Configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso lote.Snapshot.Configuracion.AmbitoIntegridad = "LOTE"
        Dim vista = LeerVistaBarreraCreacion(lote, plan, pendientes(0), consultar, pendientes.Select(Function(r) r.RaizId).ToList())
        For Each raiz In pendientes
            Dim raices = If(ambitoLote, vista.Raices, vista.Raices.Where(Function(r) r.RaizId = raiz.RaizId).ToList())
            Dim archivos = If(ambitoLote, vista.Archivos, vista.Archivos.Where(Function(a) a.RaizId = raiz.RaizId).ToList())
            raiz.RecepcionListaParaCrear = CoordinadorImportacionCarpetas.EvaluarBarreraCreacion(lote, plan, raiz, raices, archivos)
        Next
    End Sub

    Public Function ConsultarArchivos(contexto As ContextoImportacionCarpetas, loteId As String, filtro As FiltroConsultaImportacion) As PaginaRecursosImportacion(Of ArchivoImportacionCarpetas) Implements IConsultaImportacionRepository.ConsultarArchivos
        AutorizarOperacion(contexto) : IdOperacion(loteId) : ValidarFiltroConsulta("ARCHIVOS", filtro)
        Return Operar(Function(conexion, tx) LeerArchivosConsulta(LoteConsulta(conexion, tx, contexto, loteId), filtro, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)), IsolationLevel.RepeatableRead)
    End Function

    Private Shared Function LoteConsulta(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas
        Dim lote = BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId))
        If lote Is Nothing Then Throw AccesoDenegado()
        Return lote
    End Function

    Private Shared Function LeerTablaConsulta(conexion As MySqlConnection, tx As MySqlTransaction, sql As String, parametros As Object()) As DataTable
        Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
            Using reader = cmd.ExecuteReader()
                Dim datos As New DataTable()
                datos.Load(reader)
                Return datos
            End Using
        End Using
    End Function

    Friend Shared Sub ValidarFiltroConsulta(tipo As String, filtro As FiltroConsultaImportacion)
        'Techo tecnico de transporte; la politica explicita del servicio puede
        'reducirlo. No es una cuota de archivos del lote ni habilita capacidad.
        If filtro Is Nothing OrElse filtro.Pagina < 1 OrElse filtro.TamanoPagina < 1 OrElse filtro.TamanoPagina > 200 OrElse (CLng(filtro.Pagina) - 1L) * filtro.TamanoPagina > UInteger.MaxValue Then Throw SolicitudInvalida()
        If tipo <> "RAICES" AndAlso tipo <> "ARCHIVOS" Then Throw SolicitudInvalida()
        If filtro.RaizId IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(filtro.RaizId) Then Throw SolicitudInvalida()
        If filtro.Estado IsNot Nothing AndAlso Not If(tipo = "RAICES", EstadosImportacionCarpetas.EsRaiz(filtro.Estado), EstadosImportacionCarpetas.EsEfecto(filtro.Estado)) Then Throw SolicitudInvalida()
        If filtro.EstadoRecepcion IsNot Nothing AndAlso (tipo <> "ARCHIVOS" OrElse Not EstadosImportacionCarpetas.EsRecepcion(filtro.EstadoRecepcion)) Then Throw SolicitudInvalida()
    End Sub

    Friend Shared Function LeerRaicesConsulta(lote As LoteImportacionCarpetas, filtro As FiltroConsultaImportacion, consultar As Func(Of String, Object(), DataTable)) As PaginaRecursosImportacion(Of RaizImportacionCarpetas)
        Dim datos = LeerPaginaConsulta(lote, "RAICES", filtro, consultar)
        Dim pagina As New PaginaRecursosImportacion(Of RaizImportacionCarpetas) With {.Total = datos.Item2, .RevisionLectura = lote.Revision}
        Using reader = datos.Item1.CreateDataReader()
            While reader.Read()
                Dim raiz = LeerRaizControl(reader)
                If raiz.LoteId <> lote.LoteId Then Throw NoDisponible()
                pagina.Elementos.Add(raiz)
            End While
        End Using
        Return pagina
    End Function

    Friend Shared Function LeerArchivosConsulta(lote As LoteImportacionCarpetas, filtro As FiltroConsultaImportacion, consultar As Func(Of String, Object(), DataTable)) As PaginaRecursosImportacion(Of ArchivoImportacionCarpetas)
        Dim datos = LeerPaginaConsulta(lote, "ARCHIVOS", filtro, consultar)
        Dim pagina As New PaginaRecursosImportacion(Of ArchivoImportacionCarpetas) With {.Total = datos.Item2, .RevisionLectura = lote.Revision}
        Using reader = datos.Item1.CreateDataReader()
            While reader.Read()
                pagina.Elementos.Add(LeerArchivoRecepcion(reader))
            End While
        End Using
        Return pagina
    End Function

    Private Shared Function LeerPaginaConsulta(lote As LoteImportacionCarpetas, tipo As String, filtro As FiltroConsultaImportacion, consultar As Func(Of String, Object(), DataTable)) As Tuple(Of DataTable, UInteger)
        ValidarFiltroConsulta(tipo, filtro)
        If lote Is Nothing OrElse lote.Contexto Is Nothing OrElse consultar Is Nothing OrElse lote.Revision = 0 Then Throw NoDisponible()
        AutorizarOperacion(lote.Contexto)
        Dim raices = tipo = "RAICES"
        Dim desde = If(raices, "ra_integracion_raiz r", "ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id") & " INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id"
        Dim donde = " WHERE l.lote_id=@lote AND l.ambito_ref=@ambito AND l.solicitante_ref=@usuario AND l.nivel_ref=@nivel"
        Dim parametros As New List(Of Object) From {"@lote", IdOperacion(lote.LoteId), "@ambito", lote.Contexto.AmbitoRef, "@usuario", lote.Contexto.UsuarioRef, "@nivel", lote.Contexto.NivelRef}
        If filtro.RaizId IsNot Nothing Then
            donde &= " AND r.raiz_id=@raiz" : parametros.AddRange(New Object() {"@raiz", IdOperacion(filtro.RaizId)})
        End If
        If filtro.Estado IsNot Nothing Then
            donde &= If(raices, " AND r.estado=@estado", " AND f.estado_efecto=@estado") : parametros.AddRange(New Object() {"@estado", filtro.Estado})
        End If
        If filtro.EstadoRecepcion IsNot Nothing Then
            donde &= " AND f.estado_recepcion=@recepcion" : parametros.AddRange(New Object() {"@recepcion", filtro.EstadoRecepcion})
        End If
        Dim cuenta = consultar("SELECT COUNT(*) AS total FROM " & desde & donde, parametros.ToArray())
        If cuenta Is Nothing OrElse cuenta.Rows.Count <> 1 Then Throw NoDisponible()
        Dim total = Convert.ToUInt32(cuenta.Rows(0)("total"), CultureInfo.InvariantCulture)
        If total > If(raices, lote.TotalRaices, lote.TotalArchivos) Then Throw NoDisponible()
        parametros.AddRange(New Object() {"@tamano", filtro.TamanoPagina, "@inicio", (CLng(filtro.Pagina) - 1L) * filtro.TamanoPagina})
        Dim columnas = If(raices, "r.*", "f.*,r.cliente_raiz_id AS cliente_raiz,r.datos_doc AS datos_raiz")
        Dim datos = consultar("SELECT " & columnas & " FROM " & desde & donde & " ORDER BY " & If(raices, "r.raiz_id", "f.archivo_id") & " LIMIT @tamano OFFSET @inicio", parametros.ToArray())
        If datos Is Nothing OrElse datos.Rows.Count > filtro.TamanoPagina OrElse CULng(datos.Rows.Count) > total Then Throw NoDisponible()
        Return Tuple.Create(datos, total)
    End Function
End Class
