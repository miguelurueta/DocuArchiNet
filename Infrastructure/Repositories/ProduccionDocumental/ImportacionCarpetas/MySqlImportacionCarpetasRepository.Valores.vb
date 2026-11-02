Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Public Function ObtenerVistaValores(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As VistaValoresImportacion
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim vista = CargarVistaValores(conexion, tx, contexto, loteId, archivoId)
                          CoordinadorImportacionCarpetas.ValidarVistaValores(contexto, vista)
                          If Not LeerClasificacionExpedienteConocido(contexto, vista.Raiz.IdentidadExpediente, vista.Lote.Snapshot.Configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Throw ConflictoOperacion()
                          Return vista
                      End Function)
    End Function

    'Proveedores y catalogos ya se consultaron fuera de esta transaccion.
    'SQL NULL es el unico estado no fijado; [] tambien es un checkpoint.
    Public Function FijarValoresArchivo(contexto As ContextoImportacionCarpetas, esperada As VistaValoresImportacion, resultado As ResultadoEnsayoPerfilImportacion) As ArchivoImportacionCarpetas
        AutorizarOperacion(contexto)
        CoordinadorImportacionCarpetas.ValidarVistaValores(contexto, esperada)
        Return Operar(Function(conexion, tx)
                          Dim actual = CargarVistaValores(conexion, tx, contexto, esperada.Lote.LoteId, esperada.Archivo.ArchivoId)
                          If Not LeerClasificacionExpedienteConocido(contexto, actual.Raiz.IdentidadExpediente, actual.Lote.Snapshot.Configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Throw ConflictoOperacion()
                          Dim instante = Ahora()
                          Dim persistido = PersistirValoresArchivo(contexto, esperada, actual, resultado, instante,
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub(codigo)
                                  RegistrarEventoControl(conexion, tx, contexto, actual.Lote.LoteId, "ARCHIVO", actual.Archivo.ArchivoId, Nothing, "MATERIALIZAR_VALORES", codigo, instante)
                              End Sub)
                          If Not actual.Archivo.ValoresEfectivosFijados Then ConsolidarEnTransaccion(conexion, tx, contexto, actual.Lote.LoteId, instante)
                          Return persistido
                      End Function)
    End Function

    Private Shared Function CargarVistaValores(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As VistaValoresImportacion
        Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
        Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, False)
        Dim raiz = CargarRaizControl(conexion, tx, loteId, archivo.RaizId)
        archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, True)
        Return New VistaValoresImportacion With {.Lote = lote, .Raiz = raiz, .Archivo = archivo}
    End Function

    Friend Shared Function PersistirValoresArchivo(contexto As ContextoImportacionCarpetas, esperada As VistaValoresImportacion, actual As VistaValoresImportacion, resultado As ResultadoEnsayoPerfilImportacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action(Of String)) As ArchivoImportacionCarpetas
        CoordinadorImportacionCarpetas.ValidarVistaValores(contexto, esperada)
        CoordinadorImportacionCarpetas.ValidarVistaValores(contexto, actual)
        If resultado Is Nothing OrElse resultado.Valores Is Nothing OrElse resultado.Incidencias Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing Then Throw SolicitudInvalida()
        Dim archivo = actual.Archivo, lote = actual.Lote
        If actual.Lote.LoteId <> esperada.Lote.LoteId OrElse actual.Raiz.RaizId <> esperada.Raiz.RaizId OrElse archivo.ArchivoId <> esperada.Archivo.ArchivoId OrElse PerfilImportacionJson.Escribir(actual.Lote.Snapshot) <> PerfilImportacionJson.Escribir(esperada.Lote.Snapshot) OrElse PerfilImportacionJson.Escribir(actual.Raiz) <> PerfilImportacionJson.Escribir(esperada.Raiz) Then Throw ConflictoOperacion()
        If archivo.ValoresEfectivosFijados Then
            If resultado.Incidencias.Count <> 0 OrElse PerfilImportacionJson.Escribir(archivo.ValoresEfectivos) <> PerfilImportacionJson.Escribir(resultado.Valores) Then Throw ConflictoOperacion()
            Return archivo
        End If
        If PerfilImportacionJson.Escribir(actual) <> PerfilImportacionJson.Escribir(esperada) OrElse lote.Estado <> "EN_CURSO" OrElse lote.DetencionSolicitada OrElse archivo.EstadoRecepcion <> "RECIBIDO" OrElse archivo.EstadoEfecto <> "NO_INICIADO" OrElse archivo.IntentoActivoId IsNot Nothing OrElse archivo.Resultado IsNot Nothing OrElse archivo.Revision = 0 OrElse archivo.Revision >= UInteger.MaxValue - 1UI OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI Then Throw ConflictoOperacion()
        Dim correcto = resultado.Incidencias.Count = 0
        If correcto Then
            CoordinadorImportacionCarpetas.ValidarCorrespondenciaValores(lote.Snapshot.Reglas.Where(Function(r) r.DestinoTipo <> "CAMPO_EXPEDIENTE").ToList(), resultado.Valores, True)
            For Each valor In archivo.ValoresPlan.Where(Function(v) Not v.Diferido)
                If PerfilImportacionJson.Escribir(valor) <> PerfilImportacionJson.Escribir(resultado.Valores.Single(Function(v) v.DestinoTipo = valor.DestinoTipo AndAlso v.DestinoRef = valor.DestinoRef)) Then Throw ConflictoOperacion()
            Next
        Else
            If resultado.Valores.Count <> 0 Then Throw SolicitudInvalida()
            CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", archivo.EstadoEfecto, "RECHAZADO")
        End If
        Dim codigo = If(correcto, "VALORES_FIJADOS", "VALORES_INCOMPATIBLES")
        Dim evidencia As ResultadoEfectoImportacion = Nothing
        If Not correcto Then evidencia = New ResultadoEfectoImportacion With {.Estado = "RECHAZADO", .Codigo = codigo, .Mensaje = "Los valores del documento requieren revision; se conserva el expediente confirmado.", .Expediente = actual.Raiz.IdentidadExpediente}
        UnaFila(ejecutar("UPDATE ra_integracion_archivo SET valores_efectivos_doc=@valores,estado_efecto=@estado,resultado_doc=@resultado,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND valores_efectivos_doc IS NULL AND intento_activo_id IS NULL AND estado_recepcion='RECIBIDO' AND estado_efecto='NO_INICIADO' AND resultado_doc IS NULL", New Object() {"@valores", If(correcto, DirectCast(PerfilImportacionJson.Escribir(resultado.Valores), Object), DBNull.Value), "@estado", If(correcto, "NO_INICIADO", "RECHAZADO"), "@resultado", If(correcto, DirectCast(DBNull.Value, Object), PerfilImportacionJson.Escribir(evidencia)), "@ahora", instante, "@archivo", IdOperacion(archivo.ArchivoId), "@raiz", IdOperacion(actual.Raiz.RaizId), "@revision", archivo.Revision}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND detencion_solicitada=0 AND estado='EN_CURSO'", New Object() {"@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision}))
        evento(codigo)
        Dim salida = PerfilImportacionJson.Leer(Of ArchivoImportacionCarpetas)(PerfilImportacionJson.Escribir(archivo))
        salida.Revision += 1UI
        salida.ValoresEfectivosFijados = correcto
        salida.ValoresEfectivos = If(correcto, PerfilImportacionJson.Leer(Of List(Of ValorReglaPerfilImportacion))(PerfilImportacionJson.Escribir(resultado.Valores)), New List(Of ValorReglaPerfilImportacion)())
        salida.EstadoEfecto = If(correcto, "NO_INICIADO", "RECHAZADO")
        salida.Resultado = evidencia
        Return salida
    End Function
End Class
