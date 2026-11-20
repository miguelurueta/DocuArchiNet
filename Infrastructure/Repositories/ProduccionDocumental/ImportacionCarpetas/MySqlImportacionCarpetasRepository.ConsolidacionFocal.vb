Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    'Agregados SQL: ninguna fila origen_doc/resultado_doc/manifest se materializa.
    'La prueba integral se conserva para el unico cierre candidato a COMPLETADO.
    Private Function ConsolidarFocal(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, instante As DateTime) As ConsolidacionImportacion
        Dim datos = LeerTablaConsulta(conexion, tx,
            "SELECT r.raiz_id,r.estado,r.revision,(r.identidad_expediente_doc IS NOT NULL) AS creada,(r.intento_activo_id IS NOT NULL) AS activa," &
            "COUNT(f.archivo_id) AS total,IFNULL(SUM(f.estado_efecto='COMPLETADO'),0) AS confirmados,IFNULL(SUM(f.estado_efecto='INCIERTO' OR f.estado_recepcion='INCIERTO'),0) AS inciertos," &
            "IFNULL(SUM(f.estado_efecto='EN_CURSO' OR f.estado_recepcion='RECIBIENDO'),0) AS activos,IFNULL(SUM(f.estado_recepcion='RECIBIDO'),0) AS recibidos," &
            "IFNULL(SUM(f.estado_efecto='RECHAZADO' OR f.estado_recepcion='RECHAZADO' OR (f.estado_efecto='NO_INICIADO' AND f.resultado_doc IS NOT NULL)),0) AS incidencias " &
            "FROM ra_integracion_raiz r LEFT JOIN ra_integracion_archivo f ON f.raiz_id=r.raiz_id WHERE r.lote_id=@lote GROUP BY r.raiz_id,r.estado,r.revision,r.identidad_expediente_doc IS NOT NULL,r.intento_activo_id IS NOT NULL ORDER BY r.raiz_id",
            New Object() {"@lote", IdOperacion(lote.LoteId)})
        If datos.Rows.Count <> lote.TotalRaices Then Throw ConflictoOperacion()
        Dim salida As New ConsolidacionImportacion With {.LoteId = lote.LoteId, .Revision = lote.Revision, .EstadoOriginal = lote.Estado}, total As Long = 0, recibidos As Long = 0
        For Each r As DataRow In datos.Rows
            Dim n = Convert.ToInt32(r("total")), completos = Convert.ToInt32(r("confirmados")), creada = Convert.ToBoolean(r("creada")), activa = Convert.ToBoolean(r("activa")) OrElse Convert.ToInt32(r("activos")) > 0
            total += n : recibidos += Convert.ToInt32(r("recibidos"))
            salida.ArchivosConfirmados += CUInt(completos)
            If creada Then salida.RaicesConfirmadas += 1UI
            Dim incierta = Convert.ToInt32(r("inciertos")) > 0 OrElse Texto(r("estado")) = "INCIERTO"
            salida.HayIncertidumbre = salida.HayIncertidumbre OrElse incierta
            salida.HayIntentosActivos = salida.HayIntentosActivos OrElse activa
            salida.HayRechazados = salida.HayRechazados OrElse Convert.ToInt32(r("incidencias")) > 0
            Dim estado = CalcularEstadoRaizFocal(Texto(r("estado")), n, completos, creada, activa, incierta, Convert.ToInt32(r("incidencias")))
            salida.Raices.Add(New EstadoRaizConsolidadaImportacion With {.RaizId = Texto(r("raiz_id")), .Revision = Convert.ToUInt32(r("revision")), .EstadoOriginal = Texto(r("estado")), .Estado = estado})
        Next
        If total <> lote.TotalArchivos Then Throw ConflictoOperacion()
        'Incluye intentos sin marcador y marcadores huerfanos, sin inferir un lease.
        Dim estados = LeerTablaConsulta(conexion, tx, "SELECT IFNULL(SUM(estado IN ('RESERVADO','EN_CURSO')),0) AS activos,IFNULL(SUM(estado='INCIERTO'),0) AS inciertos FROM ra_integracion_intento WHERE lote_id=@lote AND fase<>'RECEPCION'", New Object() {"@lote", IdOperacion(lote.LoteId)})
        salida.HayIntentosActivos = salida.HayIntentosActivos OrElse Convert.ToInt32(estados.Rows(0)("activos")) > 0
        salida.HayIncertidumbre = salida.HayIncertidumbre OrElse Convert.ToInt32(estados.Rows(0)("inciertos")) > 0 OrElse lote.Estado = "PENDIENTE_VERIFICACION"
        salida.HayPendientes = salida.ArchivosConfirmados <> lote.TotalArchivos
        If Not salida.HayPendientes AndAlso salida.RaicesConfirmadas = lote.TotalRaices AndAlso Not salida.HayIntentosActivos Then
            'Valida identidad/resultados/duplicados por la ruta integral al cerrar.
            Dim integral = CoordinadorImportacionCarpetas.EvaluarConsolidacion(CargarVistaConsolidacion(conexion, tx, contexto, lote.LoteId))
            If integral.Estado <> "COMPLETADO" Then Throw ConflictoOperacion()
            salida = integral
        ElseIf salida.HayIntentosActivos Then
            salida.Estado = "EN_CURSO"
        ElseIf salida.HayIncertidumbre Then
            salida.Estado = "PENDIENTE_VERIFICACION"
        ElseIf lote.DetencionSolicitada OrElse lote.Estado = "DETENIDO" Then
            salida.Estado = "DETENIDO"
        ElseIf salida.HayRechazados Then
            salida.Estado = "PARCIAL"
        ElseIf salida.RaicesConfirmadas > 0 Then
            salida.Estado = "EN_CURSO"
        Else
            salida.Estado = If(recibidos = total, "LISTO", "RECIBIENDO")
        End If
        Dim cambios = salida.Raices.Where(Function(r) r.Estado <> r.EstadoOriginal).ToList()
        If cambios.Count = 0 AndAlso salida.Estado = lote.Estado Then Return salida
        For Each r In cambios
            Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_raiz SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado=@anterior", "@estado", r.Estado, "@ahora", instante, "@raiz", IdOperacion(r.RaizId), "@lote", IdOperacion(lote.LoteId), "@revision", r.Revision, "@anterior", r.EstadoOriginal)
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            RegistrarEventoControl(conexion, tx, contexto, lote.LoteId, "RAIZ", r.RaizId, Nothing, "CONSOLIDAR_ESTADO", "RAIZ_" & r.Estado, instante)
        Next
        Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado=@anterior", "@estado", salida.Estado, "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision, "@anterior", lote.Estado)
            UnaFila(cmd.ExecuteNonQuery())
        End Using
        RegistrarEventoControl(conexion, tx, contexto, lote.LoteId, "LOTE", lote.LoteId, Nothing, "CONSOLIDAR_ESTADO", "LOTE_" & salida.Estado, instante)
        Return salida
    End Function

    Friend Shared Function CalcularEstadoRaizFocal(estado As String, total As Integer, completos As Integer, creada As Boolean, activa As Boolean, incierta As Boolean, incidencias As Integer) As String
        If Not EstadosImportacionCarpetas.EsRaiz(estado) OrElse total <= 0 OrElse completos < 0 OrElse completos > total OrElse incidencias < 0 OrElse incidencias > total Then Throw ConflictoOperacion()
        If estado = "COMPLETADA" AndAlso (Not creada OrElse completos <> total OrElse incierta OrElse activa) Then Throw ConflictoOperacion()
        'Un agregado de otro archivo no reactiva una raiz excluida.
        If {"BLOQUEADO", "CANCELADA"}.Contains(estado) Then Return estado
        If incierta Then Return "INCIERTO"
        If Not activa AndAlso creada Then Return If(completos = total, "COMPLETADA", If(incidencias > 0, "PARCIAL", "EXPEDIENTE_CONFIRMADO"))
        Return estado
    End Function
End Class
