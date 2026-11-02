Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MySql.Data.MySqlClient

'Mismo repositorio/tablas: consolida metadatos bajo el bloqueo de lote.
'No verifica archivos fisicos, ejecuta writers o reconcilia intentos inciertos.
Public Partial Class MySqlImportacionCarpetasRepository
    Public Function ConsolidarLote(contexto As ContextoImportacionCarpetas, loteId As String, revisionEsperada As UInteger) As ConsolidacionImportacion
        AutorizarOperacion(contexto)
        If revisionEsperada = 0 Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
                          If lote.Revision <> revisionEsperada Then Throw ConflictoOperacion()
                          Return ConsolidarEnTransaccion(conexion, tx, contexto, loteId, Ahora())
                      End Function)
    End Function

    Private Function ConsolidarEnTransaccion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, instante As DateTime) As ConsolidacionImportacion
        Dim vista = CargarVistaConsolidacion(conexion, tx, contexto, loteId)
        Return PersistirConsolidacion(vista, instante,
            Function(sql, parametros)
                Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                    Return cmd.ExecuteNonQuery()
                End Using
            End Function,
            Sub(resultado)
                For Each raiz In resultado.Raices.Where(Function(r) r.Estado <> r.EstadoOriginal)
                    RegistrarEventoControl(conexion, tx, contexto, loteId, "RAIZ", raiz.RaizId, Nothing, "CONSOLIDAR_ESTADO", "RAIZ_" & raiz.Estado, instante, New ResultadoEfectoImportacion With {.Estado = raiz.Estado, .Codigo = "RAIZ_" & raiz.Estado, .Mensaje = "Estado calculado desde inventario y resultados persistidos."})
                Next
                RegistrarEventoControl(conexion, tx, contexto, loteId, "LOTE", loteId, Nothing, "CONSOLIDAR_ESTADO", "LOTE_" & resultado.Estado, instante, New ResultadoEfectoImportacion With {.Estado = resultado.Estado, .Codigo = "LOTE_" & resultado.Estado, .Mensaje = "Estado calculado sin excluir originales ni repetir efectos."})
            End Sub)
    End Function

    Private Shared Function CargarVistaConsolidacion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String) As VistaConsolidacionImportacion
        Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
        Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
        Dim vista As New VistaConsolidacionImportacion With {.Lote = lote, .Plan = plan}
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_raiz WHERE lote_id=@lote ORDER BY raiz_id LIMIT @limite FOR UPDATE", "@lote", IdOperacion(loteId), "@limite", plan.Manifiesto.Raices.Count + 1)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    vista.Raices.Add(LeerRaizControl(reader))
                End While
            End Using
        End Using
        Using cmd = ComandoOperacion(conexion, tx, "SELECT f.*,r.cliente_raiz_id AS cliente_raiz,r.datos_doc AS datos_raiz FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lote ORDER BY f.archivo_id LIMIT @limite FOR UPDATE", "@lote", IdOperacion(loteId), "@limite", plan.Manifiesto.Archivos.Count + 1)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    vista.Archivos.Add(LeerArchivoRecepcion(reader))
                End While
            End Using
        End Using
        'Recepciones terminales no prueban efectos documentales. Se acotan
        'creacion/almacenamiento/incorporacion confirmados y toda actividad o
        'incertidumbre; exceder el limite hace fallar la vista, sin truncarla.
        Dim maximo = plan.Manifiesto.Raices.Count + 3 * plan.Manifiesto.Archivos.Count
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE lote_id=@lote AND (estado IN ('RESERVADO','EN_CURSO','INCIERTO') OR (fase<>'RECEPCION' AND estado='CONFIRMADO')) ORDER BY intento_id LIMIT @limite FOR UPDATE", "@lote", IdOperacion(loteId), "@limite", maximo + 1)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    vista.Intentos.Add(LeerIntentoDocumental(reader))
                End While
            End Using
        End Using
        If vista.Intentos.Count > maximo Then Throw NoDisponible()
        Return vista
    End Function

    Friend Shared Function PersistirConsolidacion(vista As VistaConsolidacionImportacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action(Of ConsolidacionImportacion)) As ConsolidacionImportacion
        If ejecutar Is Nothing OrElse evento Is Nothing Then Throw SolicitudInvalida()
        Dim resultado = CoordinadorImportacionCarpetas.EvaluarConsolidacion(vista)
        Dim cambios = resultado.Raices.Where(Function(r) r.Estado <> r.EstadoOriginal).OrderBy(Function(r) IdOperacion(r.RaizId)).ToList()
        If cambios.Count = 0 AndAlso resultado.Estado = resultado.EstadoOriginal Then Return resultado
        If resultado.Revision >= UInteger.MaxValue - 1UI OrElse cambios.Any(Function(r) r.Revision >= UInteger.MaxValue - 1UI) Then Throw ConflictoOperacion()
        For Each raiz In cambios
            UnaFila(ejecutar("UPDATE ra_integracion_raiz SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado=@anterior", New Object() {"@estado", raiz.Estado, "@ahora", instante, "@raiz", IdOperacion(raiz.RaizId), "@lote", IdOperacion(resultado.LoteId), "@revision", raiz.Revision, "@anterior", raiz.EstadoOriginal}))
        Next
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado=@anterior", New Object() {"@estado", resultado.Estado, "@ahora", instante, "@lote", IdOperacion(resultado.LoteId), "@revision", resultado.Revision, "@anterior", resultado.EstadoOriginal}))
        evento(resultado)
        Return resultado
    End Function
End Class
