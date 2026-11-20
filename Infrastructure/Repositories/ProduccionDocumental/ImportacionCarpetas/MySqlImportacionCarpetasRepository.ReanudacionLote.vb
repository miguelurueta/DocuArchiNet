Option Strict On
Imports System
Imports System.Linq

Public Partial Class MySqlImportacionCarpetasRepository
    Implements IReanudacionLoteImportacionRepository

    Public Function ReanudarLote(contexto As ContextoImportacionCarpetas, loteId As String, revisionEsperada As UInteger) As LoteImportacionCarpetas Implements IReanudacionLoteImportacionRepository.ReanudarLote
        AutorizarOperacion(contexto) : IdOperacion(loteId)
        If revisionEsperada = 0 OrElse revisionEsperada >= UInteger.MaxValue - 1UI Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          'Orden comun: lote antes de raices, archivos e intentos.
                          Dim vista = CargarVistaConsolidacion(conexion, tx, contexto, loteId)
                          PersistirReanudacionLote(vista, revisionEsperada, Ahora(),
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub(resultado) RegistrarEventoControl(conexion, tx, contexto, loteId, "LOTE", loteId, Nothing, "REANUDAR_LOTE", "DETENCION_LEVANTADA", Ahora(), New ResultadoEfectoImportacion With {.Estado = resultado.Estado, .Codigo = "DETENCION_LEVANTADA", .Mensaje = "Detencion levantada con revision comprobada; no ejecuta efectos documentales."}))
                          Return BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId))
                      End Function)
    End Function

    Friend Shared Function PersistirReanudacionLote(vista As VistaConsolidacionImportacion, revisionEsperada As UInteger, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action(Of ConsolidacionImportacion)) As ConsolidacionImportacion
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing OrElse revisionEsperada = 0 OrElse revisionEsperada >= UInteger.MaxValue - 1UI OrElse vista.Lote.Revision <> revisionEsperada Then Throw ConflictoOperacion()
        Dim resultado = CoordinadorImportacionCarpetas.EvaluarReanudacionLote(vista)
        Dim cambios = resultado.Raices.Where(Function(r) r.Estado <> r.EstadoOriginal).OrderBy(Function(r) IdOperacion(r.RaizId)).ToList()
        If cambios.Any(Function(r) r.Revision >= UInteger.MaxValue - 1UI) Then Throw ConflictoOperacion()
        For Each raiz In cambios
            UnaFila(ejecutar("UPDATE ra_integracion_raiz SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado=@anterior", New Object() {"@estado", raiz.Estado, "@ahora", instante, "@raiz", IdOperacion(raiz.RaizId), "@lote", IdOperacion(resultado.LoteId), "@revision", raiz.Revision, "@anterior", raiz.EstadoOriginal}))
        Next
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,detencion_solicitada=0,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado='DETENIDO' AND detencion_solicitada=1", New Object() {"@estado", resultado.Estado, "@ahora", instante, "@lote", IdOperacion(resultado.LoteId), "@revision", revisionEsperada}))
        evento(resultado)
        Return resultado
    End Function
End Class
