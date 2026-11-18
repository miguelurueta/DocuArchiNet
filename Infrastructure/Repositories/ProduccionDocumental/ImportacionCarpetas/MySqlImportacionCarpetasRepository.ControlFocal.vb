Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Implements IControlArchivosIndependientesImportacion

    'Solo una fase mutante consentida llama aqui. Consultar/exportar nunca sella.
    Public Sub PrepararControlArchivos(contexto As ContextoImportacionCarpetas, loteId As String) Implements IControlArchivosIndependientesImportacion.PrepararControlArchivos
        AutorizarOperacion(contexto) : IdOperacion(loteId)
        Operar(Function(conexion, tx)
            ExigirIndiceControlFocal(conexion, tx)
            Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
            If lote.ControlArchivos IsNot Nothing OrElse {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then Return True
            If lote.DetencionSolicitada Then Throw ConflictoOperacion()
            Dim vista = CargarVistaConsolidacion(conexion, tx, contexto, loteId)
            'Valida una vez el plan completo, valores e identidades originales.
            CoordinadorImportacionCarpetas.EvaluarConsolidacion(vista)
            Dim sellado = ControlArchivoIndependienteImportacion.Sellar(vista.Lote, vista.Plan, vista.Raices, vista.Archivos)
            Dim registros As New List(Of Tuple(Of String, String, String, String, String))()
            For Each archivo In vista.Archivos.OrderBy(Function(a) IdOperacion(a.ArchivoId))
                registros.Add(Tuple.Create("ARCHIVO", archivo.ArchivoId, ControlArchivoIndependienteImportacion.AccionPrueba, "ORIGEN_SELLADO_V1", PerfilImportacionJson.Escribir(sellado.Pruebas(archivo.ArchivoId))))
            Next
            registros.Add(Tuple.Create("LOTE", loteId, ControlArchivoIndependienteImportacion.AccionCabecera, "CONTROL_FOCAL_V1", PerfilImportacionJson.Escribir(sellado.Cabecera)))
            InsertarControlesFocales(conexion, tx, contexto, loteId, registros)
            For Each archivo In vista.Archivos.Where(Function(a) a.EstadoEfecto = "COMPLETADO")
                If archivo.Resultado Is Nothing OrElse archivo.Resultado.Estado <> "CONFIRMADO" Then Throw ConflictoOperacion()
                RegistrarIdentidadFocal(conexion, tx, contexto, loteId, archivo.ArchivoId, archivo.Resultado)
            Next
            Return True
        End Function)
    End Sub

    Private Shared Sub ExigirIndiceControlFocal(conexion As MySqlConnection, tx As MySqlTransaction)
        Dim datos = LeerTablaConsulta(conexion, tx, "SELECT COLUMN_NAME,SEQ_IN_INDEX,NON_UNIQUE,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_integracion_evento' AND INDEX_NAME='ix_integracion_evento_recurso' ORDER BY SEQ_IN_INDEX", New Object() {})
        ValidarIndiceControlFocal(datos)
    End Sub
    Friend Shared Sub ValidarIndiceControlFocal(datos As DataTable)
        Dim columnas = {"lote_id", "recurso_tipo", "recurso_id", "accion"}
        If datos Is Nothing OrElse datos.Rows.Count <> columnas.Length Then Throw EsquemaFocalRequerido()
        For i = 0 To columnas.Length - 1
            Dim r = datos.Rows(i)
            If Texto(r("COLUMN_NAME")) <> columnas(i) OrElse Convert.ToInt32(r("SEQ_IN_INDEX"), CultureInfo.InvariantCulture) <> i + 1 OrElse Texto(r("NON_UNIQUE")) <> "1" OrElse Not r.IsNull("SUB_PART") Then Throw EsquemaFocalRequerido()
        Next
    End Sub
    Private Shared Function EsquemaFocalRequerido() As PerfilImportacionException
        Return New PerfilImportacionException("CONTROL_FOCAL_NO_CONFIGURADO", "Falta el índice de control por archivo. Solicita aplicar manualmente el script09 de importación antes de iniciar.")
    End Function

    Private Shared Function LeerControlFocal(conexion As MySqlConnection, tx As MySqlTransaction, lote As LoteImportacionCarpetas) As CabeceraControlArchivosImportacion
        Dim datos = LeerTablaConsulta(conexion, tx, "SELECT detalle_doc FROM ra_integracion_evento WHERE lote_id=@lote AND recurso_tipo='LOTE' AND recurso_id=@lote AND accion=@accion AND actor_ref=@actor AND intento_id IS NULL ORDER BY evento_id LIMIT 2", New Object() {"@lote", IdOperacion(lote.LoteId), "@accion", ControlArchivoIndependienteImportacion.AccionCabecera, "@actor", IdOperacion(lote.Contexto.UsuarioRef)})
        If datos.Rows.Count = 0 Then Return Nothing
        If datos.Rows.Count <> 1 Then Throw ConflictoOperacion()
        Dim c = LeerPersistido(Of CabeceraControlArchivosImportacion)(Texto(datos.Rows(0)("detalle_doc")))
        If c Is Nothing OrElse c.TotalBytes <> lote.TotalBytes OrElse c.SnapshotDocumentoHash <> ControlArchivoIndependienteImportacion.Hash(PerfilImportacionJson.Escribir(lote.Snapshot)) Then Throw ConflictoOperacion()
        If c.Version <> 1 OrElse c.LoteId <> lote.LoteId OrElse c.PlanId <> lote.PlanId OrElse c.TotalArchivos <> lote.TotalArchivos OrElse c.TotalRaices <> lote.TotalRaices OrElse c.SnapshotHash <> lote.Snapshot.Huella OrElse c.ContextoHash <> ControlArchivoIndependienteImportacion.Hash(PerfilImportacionJson.Escribir(lote.Contexto)) OrElse Not ControlArchivoIndependienteImportacion.EsHash(c.InventarioHash) Then Throw ConflictoOperacion()
        Dim plan = LeerTablaConsulta(conexion, tx, "SELECT plan_hash FROM ra_integracion_plan WHERE plan_id=@plan AND ambito_ref=@ambito AND usuario_ref=@usuario AND nivel_ref=@nivel AND version_id=@version AND estado='CONFIRMADO'", New Object() {"@plan", IdOperacion(lote.PlanId), "@ambito", lote.Contexto.AmbitoRef, "@usuario", lote.Contexto.UsuarioRef, "@nivel", lote.Contexto.NivelRef, "@version", IdOperacion(lote.Snapshot.VersionId)})
        If plan.Rows.Count <> 1 OrElse Hexadecimal(DirectCast(plan.Rows(0)("plan_hash"), Byte())) <> c.PlanHash Then Throw ConflictoOperacion()
        Return c
    End Function

    Private Shared Sub VerificarArchivoFocal(conexion As MySqlConnection, tx As MySqlTransaction, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas)
        If lote.ControlArchivos Is Nothing Then Return
        Dim datos = LeerTablaConsulta(conexion, tx, "SELECT detalle_doc FROM ra_integracion_evento WHERE lote_id=@lote AND recurso_tipo='ARCHIVO' AND recurso_id=@archivo AND accion=@accion AND actor_ref=@actor AND intento_id IS NULL ORDER BY evento_id LIMIT 2", New Object() {"@lote", IdOperacion(lote.LoteId), "@archivo", IdOperacion(archivo.ArchivoId), "@accion", ControlArchivoIndependienteImportacion.AccionPrueba, "@actor", IdOperacion(lote.Contexto.UsuarioRef)})
        If datos.Rows.Count <> 1 Then Throw ConflictoOperacion()
        Dim prueba = LeerPersistido(Of PruebaControlArchivoImportacion)(Texto(datos.Rows(0)("detalle_doc")))
        ControlArchivoIndependienteImportacion.Verificar(lote, raiz, archivo, lote.ControlArchivos, prueba)
        VerificarIdentidadRaizFocal(conexion, tx, lote, raiz)
        archivo.ProcedenciaVerificada = True
    End Sub

    Private Shared Sub VerificarIdentidadRaizFocal(conexion As MySqlConnection, tx As MySqlTransaction, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas)
        If raiz.IdentidadExpediente Is Nothing Then Return
        Dim intentos As New List(Of IntentoImportacionCarpetas)()
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE lote_id=@lote AND raiz_id=@raiz AND archivo_id IS NULL AND fase='CREAR_EXPEDIENTE' AND estado='CONFIRMADO' ORDER BY intento_id LIMIT 2", "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId))
            Using reader = EjecutarLecturaControl(cmd)
                While reader.Read()
                    intentos.Add(LeerIntentoDocumental(reader))
                End While
            End Using
        End Using
        If raiz.IntentoActivoId IsNot Nothing OrElse Not CoordinadorImportacionCarpetas.CreacionConfirmadaConsolidacion(lote, raiz, intentos) Then Throw ConflictoOperacion()
    End Sub

    Private Sub InsertarControlesFocales(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, registros As List(Of Tuple(Of String, String, String, String, String)))
        Dim instante = Ahora()
        For inicio = 0 To registros.Count - 1 Step 50
            Dim parte = registros.Skip(inicio).Take(50).ToList(), valores As New List(Of String)(), parametros As New List(Of Object) From {"@lote", IdOperacion(loteId), "@actor", IdOperacion(contexto.UsuarioRef), "@instante", instante}
            For i = 0 To parte.Count - 1
                Dim sufijo = i.ToString(CultureInfo.InvariantCulture), r = parte(i)
                valores.Add("(@lote,NULL,@tipo" & sufijo & ",@id" & sufijo & ",@accion" & sufijo & ",@codigo" & sufijo & ",@actor,@instante,@doc" & sufijo & ")")
                parametros.AddRange(New Object() {"@tipo" & sufijo, r.Item1, "@id" & sufijo, IdOperacion(r.Item2), "@accion" & sufijo, r.Item3, "@codigo" & sufijo, r.Item4, "@doc" & sufijo, r.Item5})
            Next
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES " & String.Join(",", valores), parametros.ToArray())
                If cmd.ExecuteNonQuery() <> parte.Count Then Throw ConflictoOperacion()
            End Using
        Next
    End Sub

    Private Shared Function IdentidadFocalLibre(conexion As MySqlConnection, tx As MySqlTransaction, loteId As String, archivoId As String, resultado As ResultadoEfectoImportacion) As Boolean
        If resultado Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId) Then Return False
        For Each par In New String() {"DOCUMENTO", "INVENTARIO"}
            Dim id = If(par = "DOCUMENTO", resultado.DocumentoId, resultado.InventarioId)
            Dim datos = LeerTablaConsulta(conexion, tx, "SELECT detalle_doc FROM ra_integracion_evento WHERE lote_id=@lote AND recurso_tipo=@tipo AND recurso_id=@id AND accion=@accion AND intento_id IS NULL ORDER BY evento_id LIMIT 2", New Object() {"@lote", IdOperacion(loteId), "@tipo", par, "@id", IdOperacion(id), "@accion", ControlArchivoIndependienteImportacion.AccionIdentidad})
            If datos.Rows.Count > 1 Then Return False
            If datos.Rows.Count = 1 Then
                Dim anterior = LeerPersistido(Of ReferenciaIdentidadArchivoImportacion)(Texto(datos.Rows(0)("detalle_doc")))
                If anterior.ArchivoId <> archivoId OrElse anterior.DocumentoId <> resultado.DocumentoId OrElse anterior.InventarioId <> resultado.InventarioId OrElse anterior.GabineteId <> resultado.GabineteId OrElse anterior.Origen <> resultado.OrigenGabinete Then Return False
            End If
        Next
        Return True
    End Function
    Private Sub RegistrarIdentidadFocal(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, resultado As ResultadoEfectoImportacion)
        If Not IdentidadFocalLibre(conexion, tx, loteId, archivoId, resultado) Then Throw ConflictoOperacion()
        Dim referencia As New ReferenciaIdentidadArchivoImportacion With {.ArchivoId = archivoId, .Origen = resultado.OrigenGabinete, .GabineteId = resultado.GabineteId, .DocumentoId = resultado.DocumentoId, .InventarioId = resultado.InventarioId}
        Dim registros As New List(Of Tuple(Of String, String, String, String, String)) From {
            Tuple.Create("DOCUMENTO", resultado.DocumentoId, ControlArchivoIndependienteImportacion.AccionIdentidad, "DOCUMENTO_CONFIRMADO", PerfilImportacionJson.Escribir(referencia)),
            Tuple.Create("INVENTARIO", resultado.InventarioId, ControlArchivoIndependienteImportacion.AccionIdentidad, "INVENTARIO_CONFIRMADO", PerfilImportacionJson.Escribir(referencia))}
        'Releer una confirmacion no duplica la proyeccion. El lote esta bloqueado.
        registros = registros.Where(Function(r)
            Using cmd = ComandoOperacion(conexion, tx, "SELECT COUNT(*) FROM ra_integracion_evento WHERE lote_id=@lote AND recurso_tipo=@tipo AND recurso_id=@id AND accion=@accion AND intento_id IS NULL", "@lote", IdOperacion(loteId), "@tipo", r.Item1, "@id", IdOperacion(r.Item2), "@accion", r.Item3)
                Return Convert.ToInt32(EjecutarEscalarControl(cmd), CultureInfo.InvariantCulture) = 0
            End Using
        End Function).ToList()
        InsertarControlesFocales(conexion, tx, contexto, loteId, registros)
    End Sub

    Friend Shared Sub CompletarRecepcionIndividualConsulta(lote As LoteImportacionCarpetas, pagina As PaginaRecursosImportacion(Of RaizImportacionCarpetas), consultar As Func(Of String, Object(), DataTable))
        For Each raiz In pagina.Elementos
            raiz.RecepcionListaParaCrear = False
            If lote.DetencionSolicitada OrElse raiz.Estado <> "PENDIENTE" OrElse raiz.IntentoActivoId IsNot Nothing OrElse raiz.IdentidadExpediente IsNot Nothing Then Continue For
            Dim datos = consultar("SELECT EXISTS(SELECT 1 FROM ra_integracion_archivo WHERE raiz_id=@raiz AND estado_recepcion='RECIBIDO' AND estado_efecto='NO_INICIADO' AND intento_activo_id IS NULL ) AS disponible", New Object() {"@raiz", IdOperacion(raiz.RaizId)})
            If datos.Rows.Count <> 1 Then Throw NoDisponible()
            raiz.RecepcionListaParaCrear = Convert.ToBoolean(datos.Rows(0)("disponible"), CultureInfo.InvariantCulture)
        Next
    End Sub

    Private Shared Function CargarBarreraFocal(conexion As MySqlConnection, tx As MySqlTransaction, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas) As VistaBarreraCreacionImportacion
        VerificarIdentidadRaizFocal(conexion, tx, lote, raiz)
        Dim vista = CrearVistaBarreraFocal(lote, raiz)
        If raiz.IdentidadExpediente Is Nothing Then
            Dim id As Object
            Using cmd = ComandoOperacion(conexion, tx, "SELECT archivo_id FROM ra_integracion_archivo WHERE raiz_id=@raiz AND estado_recepcion='RECIBIDO' AND estado_efecto='NO_INICIADO' AND intento_activo_id IS NULL ORDER BY (resultado_doc IS NOT NULL),archivo_id LIMIT 1", "@raiz", IdOperacion(raiz.RaizId))
                id = EjecutarEscalarControl(cmd)
            End Using
            If id IsNot Nothing AndAlso id IsNot DBNull.Value Then
                Dim archivo = CargarArchivoRecepcion(conexion, tx, lote.Contexto, lote.LoteId, Texto(id), False)
                VerificarArchivoFocal(conexion, tx, lote, raiz, archivo)
                vista.Archivos.Add(archivo)
            End If
        End If
        Return vista
    End Function

    Friend Shared Function CrearVistaBarreraFocal(lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas) As VistaBarreraCreacionImportacion
        Return New VistaBarreraCreacionImportacion With {
            .Lote = lote, .Raiz = raiz, .EsFocal = True,
            .Raices = New List(Of RaizImportacionCarpetas) From {raiz},
            .Archivos = New List(Of ArchivoImportacionCarpetas)()}
    End Function

    'No se infiere disponibilidad por tiempo ni se libera un intento ajeno.
    'Solo un retorno V2 duradero permite aislar un INCIERTO de otro archivo.
    Private Shared Function HayEscritorVivoFocal(conexion As MySqlConnection, tx As MySqlTransaction, loteId As String) As Boolean
        Dim retorno = "(SELECT COUNT(*) FROM ra_integracion_evento e INNER JOIN ra_integracion_lote l ON l.lote_id=e.lote_id WHERE e.lote_id=i.lote_id AND e.intento_id=i.intento_id AND e.recurso_tipo='ARCHIVO' AND e.recurso_id=i.archivo_id AND e.actor_ref=l.solicitante_ref AND e.accion='OBSERVAR_RETORNO_DOCUMENTAL' AND e.codigo='RETORNO_DOCUMENTAL_OBSERVADO')=1" &
            " AND (SELECT COUNT(*) FROM ra_integracion_evento e WHERE e.lote_id=i.lote_id AND e.intento_id=i.intento_id AND e.recurso_tipo='ARCHIVO' AND e.recurso_id=i.archivo_id AND e.accion='AUTORIZAR_INVOCACION_DOCUMENTAL' AND e.codigo='INVOCACION_DOCUMENTAL_AUTORIZADA')=1" &
            " AND (SELECT COUNT(*) FROM ra_integracion_evento e WHERE e.lote_id=i.lote_id AND e.intento_id=i.intento_id AND e.recurso_tipo='ARCHIVO' AND e.recurso_id=i.archivo_id AND e.accion='INICIAR_ALMACENAMIENTO_V2' AND e.codigo='ALMACENAMIENTO_PREPARADO_V2')=1"
        Dim sql = "SELECT EXISTS(SELECT 1 FROM ra_integracion_intento i WHERE i.lote_id=@lote AND (i.estado IN ('RESERVADO','EN_CURSO') OR (i.estado='INCIERTO' AND NOT (i.fase='ALMACENAR' AND " & retorno & "))))" &
            " OR EXISTS(SELECT 1 FROM ra_integracion_raiz WHERE lote_id=@lote AND (intento_activo_id IS NOT NULL OR estado='CREANDO' OR (estado='INCIERTO' AND identidad_expediente_doc IS NULL)))" &
            " OR EXISTS(SELECT 1 FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=@lote AND (a.estado_efecto='EN_CURSO' OR a.estado_recepcion IN ('RECIBIENDO','INCIERTO') OR ((a.estado_efecto='INCIERTO' OR a.intento_activo_id IS NOT NULL) AND NOT EXISTS(SELECT 1 FROM ra_integracion_intento i WHERE i.intento_id=a.intento_activo_id AND i.lote_id=r.lote_id AND i.archivo_id=a.archivo_id AND i.raiz_id=a.raiz_id AND i.fase='ALMACENAR' AND i.estado='INCIERTO' AND " & retorno & "))))"
        Using cmd = ComandoOperacion(conexion, tx, sql, "@lote", IdOperacion(loteId))
            Return Convert.ToBoolean(EjecutarEscalarControl(cmd), CultureInfo.InvariantCulture)
        End Using
    End Function

    Public Sub RegistrarOriginalNoDisponible(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, revision As UInteger, causa As String, Optional intento As IntentoImportacionCarpetas = Nothing) Implements IControlArchivosIndependientesImportacion.RegistrarOriginalNoDisponible
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId)
        If Not {"ORIGINAL_AUSENTE", "ORIGINAL_ALTERADO", "ORIGINAL_ILEGIBLE", "ORIGINAL_NO_COMPROBADO"}.Contains(causa) Then Throw SolicitudInvalida()
        Operar(Function(conexion, tx)
            Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
            If lote.ControlArchivos Is Nothing OrElse lote.DetencionSolicitada Then Throw ConflictoOperacion()
            Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, False), raiz = CargarRaizControl(conexion, tx, loteId, archivo.RaizId)
            archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, True)
            VerificarArchivoFocal(conexion, tx, lote, raiz, archivo)
            If archivo.Revision <> revision OrElse archivo.EstadoRecepcion <> "RECIBIDO" OrElse (archivo.Resultado IsNot Nothing AndAlso Not ControlArchivoIndependienteImportacion.EsNoDisponible(archivo.Resultado)) Then Throw ConflictoOperacion()
            If intento Is Nothing Then
                If archivo.IntentoActivoId IsNot Nothing OrElse archivo.EstadoEfecto <> "NO_INICIADO" Then Throw ConflictoOperacion()
            Else
                Dim datos = LeerTablaConsulta(conexion, tx, "SELECT estado,token_reclamacion,evidencia_doc,fin_utc FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote AND archivo_id=@archivo AND fase='ALMACENAR' FOR UPDATE", New Object() {"@intento", IdOperacion(intento.IntentoId), "@lote", IdOperacion(loteId), "@archivo", IdOperacion(archivoId)})
                If datos.Rows.Count <> 1 OrElse Texto(datos.Rows(0)("estado")) <> "RESERVADO" OrElse Texto(datos.Rows(0)("token_reclamacion")) <> intento.TokenReclamacion OrElse Not datos.Rows(0).IsNull("evidencia_doc") OrElse Not datos.Rows(0).IsNull("fin_utc") OrElse archivo.IntentoActivoId <> intento.IntentoId OrElse archivo.EstadoEfecto <> "EN_CURSO" Then Throw ConflictoOperacion()
                Dim eventos = LeerTablaConsulta(conexion, tx, "SELECT accion,codigo,recurso_tipo,recurso_id,actor_ref FROM ra_integracion_evento WHERE lote_id=@lote AND intento_id=@intento ORDER BY evento_id LIMIT 2", New Object() {"@lote", IdOperacion(loteId), "@intento", IdOperacion(intento.IntentoId)})
                ValidarReservaSinInvocacionFocal(lote, archivo, intento, eventos)
            End If
            Dim evidencia As New ResultadoEfectoImportacion With {.Estado = "RECHAZADO", .Codigo = ControlArchivoIndependienteImportacion.CodigoNoDisponible, .Mensaje = causa, .Expediente = raiz.IdentidadExpediente, .ValoresEfectivos = archivo.ValoresEfectivos}
            Dim instante = Ahora(), evidenciaJson = PerfilImportacionJson.Escribir(evidencia)
            If intento IsNot Nothing Then
                Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_intento SET estado='RECHAZADO',evidencia_doc=@doc,fin_utc=@instante,ultimo_contacto_utc=@instante WHERE intento_id=@intento AND lote_id=@lote AND archivo_id=@archivo AND fase='ALMACENAR' AND estado='RESERVADO' AND token_reclamacion=@token AND solicitud_hash=@hash AND fin_utc IS NULL AND evidencia_doc IS NULL", "@doc", evidenciaJson, "@instante", instante, "@intento", IdOperacion(intento.IntentoId), "@lote", IdOperacion(loteId), "@archivo", IdOperacion(archivoId), "@token", intento.TokenReclamacion, "@hash", BytesHuella(intento.SolicitudHash))
                    UnaFila(cmd.ExecuteNonQuery())
                End Using
            End If
            Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_archivo SET estado_efecto='NO_INICIADO',intento_activo_id=NULL,resultado_doc=@doc,revision=revision+1,actualizado_utc=@instante WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto=@estado AND intento_activo_id <=> @intento", "@doc", evidenciaJson, "@instante", instante, "@archivo", IdOperacion(archivoId), "@raiz", IdOperacion(raiz.RaizId), "@revision", revision, "@estado", archivo.EstadoEfecto, "@intento", If(intento Is Nothing, DirectCast(DBNull.Value, Object), IdOperacion(intento.IntentoId)))
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@instante WHERE lote_id=@lote AND revision=@revision", "@instante", instante, "@lote", IdOperacion(loteId), "@revision", lote.Revision)
                UnaFila(cmd.ExecuteNonQuery())
            End Using
            RegistrarEventoControl(conexion, tx, contexto, loteId, "ARCHIVO", archivoId, If(intento Is Nothing, Nothing, intento.IntentoId), "ORIGINAL_NO_DISPONIBLE", causa, instante, evidencia)
            ConsolidarEnTransaccion(conexion, tx, contexto, loteId, instante)
            Return True
        End Function)
    End Sub

    Friend Shared Sub ValidarReservaSinInvocacionFocal(lote As LoteImportacionCarpetas, archivo As ArchivoImportacionCarpetas, intento As IntentoImportacionCarpetas, diario As DataTable)
        If lote Is Nothing OrElse archivo Is Nothing OrElse intento Is Nothing OrElse diario Is Nothing OrElse diario.Rows.Count <> 1 Then Throw ConflictoOperacion()
        If intento.LoteId <> lote.LoteId OrElse intento.ArchivoId <> archivo.ArchivoId OrElse intento.RaizId <> archivo.RaizId OrElse intento.Fase <> "ALMACENAR" OrElse intento.Estado <> "RESERVADO" OrElse intento.Resultado IsNot Nothing OrElse intento.FinUtc IsNot Nothing OrElse archivo.EstadoEfecto <> "EN_CURSO" OrElse archivo.IntentoActivoId <> intento.IntentoId Then Throw ConflictoOperacion()
        Dim fila = diario.Rows(0)
        If Texto(fila("accion")) <> "RESERVAR_INTENTO" OrElse Texto(fila("codigo")) <> "INTENTO_RESERVADO" OrElse Texto(fila("recurso_tipo")) <> "ARCHIVO" OrElse Texto(fila("recurso_id")) <> archivo.ArchivoId OrElse Texto(fila("actor_ref")) <> lote.Contexto.UsuarioRef Then Throw ConflictoOperacion()
    End Sub
End Class

Public Class ReferenciaIdentidadArchivoImportacion
    Public Property ArchivoId As String
    Public Property Origen As String
    Public Property GabineteId As String
    Public Property DocumentoId As String
    Public Property InventarioId As String
End Class
