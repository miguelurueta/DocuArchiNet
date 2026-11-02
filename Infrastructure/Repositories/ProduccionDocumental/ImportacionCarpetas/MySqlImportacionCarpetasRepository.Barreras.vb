Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq

Public Partial Class MySqlImportacionCarpetasRepository
    'Consulta previa interna; su True no reserva ni permite omitir la nueva
    'comprobacion bajo el bloqueo de inicio del efecto.
    Public Function ComprobarBarreraCreacion(contexto As ContextoImportacionCarpetas, loteId As String, raizId As String) As Boolean
        Dim vista = ObtenerBarreraCreacion(contexto, loteId, raizId)
        Return CoordinadorImportacionCarpetas.EvaluarBarreraCreacion(vista.Lote, vista.Plan, vista.Raiz, vista.Raices, vista.Archivos)
    End Function

    Public Function ObtenerBarreraCreacion(contexto As ContextoImportacionCarpetas, loteId As String, raizId As String) As VistaBarreraCreacionImportacion
        Return ObtenerBarreraDocumental(contexto, loteId, raizId)
    End Function

    Public Function ObtenerBarreraDocumental(contexto As ContextoImportacionCarpetas, loteId As String, raizId As String) As VistaBarreraCreacionImportacion
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(raizId)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
                          Dim raiz = CargarRaizControl(conexion, tx, loteId, raizId)
                          Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
                          Return LeerVistaBarreraCreacion(lote, plan, raiz, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros))
                      End Function)
    End Function

    Friend Shared Function LeerBarreraCreacion(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, raiz As RaizImportacionCarpetas, consultar As Func(Of String, Object(), DataTable)) As Boolean
        Dim vista = LeerVistaBarreraCreacion(lote, plan, raiz, consultar)
        Return CoordinadorImportacionCarpetas.EvaluarBarreraCreacion(vista.Lote, vista.Plan, vista.Raiz, vista.Raices, vista.Archivos)
    End Function

    Private Shared Function LeerVistaBarreraCreacion(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, raiz As RaizImportacionCarpetas, consultar As Func(Of String, Object(), DataTable)) As VistaBarreraCreacionImportacion
        If lote Is Nothing OrElse raiz Is Nothing OrElse plan Is Nothing OrElse plan.Manifiesto Is Nothing OrElse plan.Manifiesto.Raices Is Nothing OrElse plan.Manifiesto.Archivos Is Nothing OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing OrElse consultar Is Nothing Then Throw NoDisponible()
        Dim c = plan.Snapshot.Configuracion
        Dim ambitoLote = c.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso c.AmbitoIntegridad = "LOTE"
        Dim filtro = If(ambitoLote, "", " AND r.raiz_id=@raiz")
        Dim parametros = New Object() {"@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@limite", plan.Manifiesto.Raices.Count + 1}
        Dim datosRaices = consultar("SELECT r.raiz_id,r.lote_id,r.cliente_raiz_id,r.estado FROM ra_integracion_raiz r WHERE r.lote_id=@lote" & filtro & " ORDER BY r.raiz_id LIMIT @limite", parametros)
        Dim datosArchivos = consultar("SELECT f.archivo_id,f.raiz_id,f.cliente_archivo_id,f.bytes_previstos,f.bytes_recibidos,f.contenido_hash,f.staging_ref,f.estado_recepcion,f.estado_efecto,f.intento_activo_id,f.revision FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote" & filtro & " ORDER BY f.archivo_id LIMIT @limite", New Object() {"@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@limite", plan.Manifiesto.Archivos.Count + 1})
        If datosRaices Is Nothing OrElse datosArchivos Is Nothing Then Throw NoDisponible()
        For Each campo In {"raiz_id", "lote_id", "cliente_raiz_id", "estado"}
            If Not datosRaices.Columns.Contains(campo) Then Throw NoDisponible()
        Next
        For Each campo In {"archivo_id", "raiz_id", "cliente_archivo_id", "bytes_previstos", "bytes_recibidos", "contenido_hash", "staging_ref", "estado_recepcion", "estado_efecto", "intento_activo_id", "revision"}
            If Not datosArchivos.Columns.Contains(campo) Then Throw NoDisponible()
        Next
        Dim raices As New List(Of RaizImportacionCarpetas)()
        For Each fila As DataRow In datosRaices.Rows
            raices.Add(New RaizImportacionCarpetas With {.RaizId = Texto(fila("raiz_id")), .LoteId = Texto(fila("lote_id")), .ClienteRaizId = Texto(fila("cliente_raiz_id")), .Estado = Texto(fila("estado"))})
        Next
        Dim archivos As New List(Of ArchivoImportacionCarpetas)()
        For Each fila As DataRow In datosArchivos.Rows
            archivos.Add(New ArchivoImportacionCarpetas With {.ArchivoId = Texto(fila("archivo_id")), .RaizId = Texto(fila("raiz_id")), .ClienteArchivoId = Texto(fila("cliente_archivo_id")), .BytesPrevistos = Texto(fila("bytes_previstos")), .BytesRecibidos = If(fila.IsNull("bytes_recibidos"), Nothing, Texto(fila("bytes_recibidos"))), .ContenidoHash = If(fila.IsNull("contenido_hash"), Nothing, Hexadecimal(DirectCast(fila("contenido_hash"), Byte()))), .StagingRef = If(fila.IsNull("staging_ref"), Nothing, Texto(fila("staging_ref"))), .EstadoRecepcion = Texto(fila("estado_recepcion")), .EstadoEfecto = Texto(fila("estado_efecto")), .IntentoActivoId = If(fila.IsNull("intento_activo_id"), Nothing, Texto(fila("intento_activo_id"))), .Revision = Convert.ToUInt32(fila("revision"), Globalization.CultureInfo.InvariantCulture)})
        Next
        Return New VistaBarreraCreacionImportacion With {.Lote = lote, .Plan = plan, .Raiz = raiz, .Raices = raices, .Archivos = archivos}
    End Function
End Class
