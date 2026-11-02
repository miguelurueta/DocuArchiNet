Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

'Unico repositorio operativo, incorporado por capacidades. No ejecuta
'migraciones ni invoca los adaptadores documentales.
Public Partial Class MySqlImportacionCarpetasRepository
    Implements IRecepcionArchivosImportacionRepository, IControlLoteImportacionRepository, IConsultaImportacionRepository
    Private ReadOnly _conexion As String
    Private ReadOnly _utcAhora As Func(Of DateTime)
    Private ReadOnly _observadorConfirmacion As Action(Of String, MySqlConnection, MySqlTransaction)
    Private Const TablasPropias As String = "'ra_integracion_plan','ra_integracion_lote','ra_integracion_raiz','ra_integracion_archivo','ra_integracion_intento','ra_integracion_evento'"
    Private Const TablasConPadres As String = TablasPropias & ",'ra_integracion_perfil_version','remit_dest_interno_perfil_produccion'"

    Public Sub New(conexion As String, Optional relojUtc As Func(Of DateTime) = Nothing)
        If String.IsNullOrWhiteSpace(conexion) Then Throw NoDisponible()
        _conexion = conexion
        _utcAhora = If(relojUtc, Function() DateTime.UtcNow)
    End Sub

    'Punto interno de observacion/fallo para verificar la transaccion real.
    'No se recibe desde DTO/HTTP; el constructor productivo no instala observador.
    Friend Sub New(conexion As String, relojUtc As Func(Of DateTime), observador As Action(Of String, MySqlConnection, MySqlTransaction))
        Me.New(conexion, relojUtc)
        _observadorConfirmacion = observador
    End Sub

    Public Sub VerificarEsquema()
        Try
            Using conexion As New MySqlConnection(_conexion)
                conexion.Open()
                ValidarMetadatos(LeerMetadatos(conexion))
            End Using
        Catch ex As MySqlException
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Sub

    Private Shared Function LeerMetadatos(conexion As MySqlConnection) As DataSet
        Dim datos As New DataSet()
        'Catalogos acotados por tablas; no JOIN entre catalogos costosos en 5.1.
        Dim consultas As New Dictionary(Of String, String) From {
            {"Tablas", "SELECT TABLE_NAME,ENGINE,TABLE_COLLATION FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN (" & TablasConPadres & ")"},
            {"Columnas", "SELECT TABLE_NAME,COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,COLLATION_NAME,IS_NULLABLE,EXTRA,COLUMN_DEFAULT FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND (TABLE_NAME IN (" & TablasPropias & ") OR (TABLE_NAME='ra_integracion_perfil_version' AND COLUMN_NAME='version_id') OR (TABLE_NAME='remit_dest_interno_perfil_produccion' AND COLUMN_NAME='IMPORTAR_CARPETAS'))"},
            {"Indices", "SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN (" & TablasPropias & ",'ra_integracion_perfil_version')"},
            {"Claves", "SELECT TABLE_NAME,CONSTRAINT_NAME,COLUMN_NAME,ORDINAL_POSITION,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME,(REFERENCED_TABLE_SCHEMA=DATABASE()) AS MISMO_ESQUEMA FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN (" & TablasPropias & ") AND REFERENCED_TABLE_NAME IS NOT NULL"},
            {"Reglas", "SELECT TABLE_NAME,CONSTRAINT_NAME,UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND TABLE_NAME IN (" & TablasPropias & ")"}}
        For Each consulta In consultas
            Using cmd As New MySqlCommand(consulta.Value, conexion)
                cmd.CommandTimeout = 10
                Using reader = cmd.ExecuteReader()
                    Dim tabla As New DataTable(consulta.Key)
                    tabla.Load(reader)
                    datos.Tables.Add(tabla)
                End Using
            End Using
        Next
        Return datos
    End Function

    Friend Shared Sub ValidarMetadatos(datos As DataSet)
        If datos Is Nothing Then Throw NoDisponible()
        For Each nombre In {"Tablas", "Columnas", "Indices", "Claves", "Reglas"}
            If Not datos.Tables.Contains(nombre) Then Throw NoDisponible()
        Next
        For Each contrato In ColumnasEsperadas
            Dim tablas = Filas(datos, "Tablas").Where(Function(r) Texto(r, "TABLE_NAME") = contrato.Key).ToArray()
            If tablas.Length <> 1 OrElse Texto(tablas(0), "ENGINE") <> "InnoDB" Then Throw NoDisponible()
            Dim propia = contrato.Key.StartsWith("ra_integracion_", StringComparison.Ordinal) AndAlso contrato.Key <> "ra_integracion_perfil_version"
            If propia AndAlso Texto(tablas(0), "TABLE_COLLATION") <> "ascii_bin" Then Throw NoDisponible()
            Dim columnas = Filas(datos, "Columnas").Where(Function(r) Texto(r, "TABLE_NAME") = contrato.Key).ToArray()
            If propia AndAlso columnas.Length <> contrato.Value.Length Then Throw NoDisponible()
            For Each especificacion In contrato.Value
                Dim esperado = especificacion.Split("|"c)
                Dim candidatas = columnas.Where(Function(r) Texto(r, "COLUMN_NAME") = esperado(0)).ToArray()
                If candidatas.Length <> 1 Then Throw NoDisponible()
                Dim columna = candidatas(0)
                Dim tipo = Texto(columna, "DATA_TYPE") & If(Texto(columna, "COLUMN_TYPE").Contains("unsigned"), " unsigned", "")
                Dim actual = {tipo, Texto(columna, "CHARACTER_MAXIMUM_LENGTH"), Texto(columna, "COLLATION_NAME"), Texto(columna, "IS_NULLABLE"), Texto(columna, "EXTRA"), Texto(columna, "COLUMN_DEFAULT")}
                For i = 0 To actual.Length - 1
                    If actual(i) <> esperado(i + 1) Then Throw NoDisponible()
                Next
            Next
        Next
        For Each especificacion In IndicesEsperados
            Dim esperado = especificacion.Split("|"c)
            Dim columnas = Filas(datos, "Indices").Where(Function(r) Texto(r, "TABLE_NAME") = esperado(0) AndAlso Texto(r, "INDEX_NAME") = esperado(1)).OrderBy(Function(r) Convert.ToInt32(r("SEQ_IN_INDEX"), CultureInfo.InvariantCulture)).ToArray()
            If columnas.Length = 0 OrElse columnas.Any(Function(r) Texto(r, "NON_UNIQUE") <> esperado(2) OrElse Not r.IsNull("SUB_PART")) Then Throw NoDisponible()
            If String.Join(",", columnas.Select(Function(r) Texto(r, "COLUMN_NAME"))) <> esperado(3) Then Throw NoDisponible()
        Next
        For Each especificacion In ClavesEsperadas
            Dim esperado = especificacion.Split("|"c)
            Dim claves = Filas(datos, "Claves").Where(Function(r) Texto(r, "TABLE_NAME") = esperado(0) AndAlso Texto(r, "CONSTRAINT_NAME") = esperado(1)).ToArray()
            If claves.Length <> 1 Then Throw NoDisponible()
            Dim clave = claves(0)
            If Texto(clave, "COLUMN_NAME") <> esperado(2) OrElse Texto(clave, "REFERENCED_TABLE_NAME") <> esperado(3) OrElse Texto(clave, "REFERENCED_COLUMN_NAME") <> esperado(4) OrElse Convert.ToInt32(clave("MISMO_ESQUEMA"), CultureInfo.InvariantCulture) <> 1 Then Throw NoDisponible()
            Dim reglas = Filas(datos, "Reglas").Where(Function(r) Texto(r, "TABLE_NAME") = esperado(0) AndAlso Texto(r, "CONSTRAINT_NAME") = esperado(1)).ToArray()
            If reglas.Length <> 1 OrElse Texto(reglas(0), "UPDATE_RULE") <> "RESTRICT" OrElse Texto(reglas(0), "DELETE_RULE") <> "RESTRICT" Then Throw NoDisponible()
        Next
    End Sub

    Private Shared Function Filas(datos As DataSet, nombre As String) As IEnumerable(Of DataRow)
        Return datos.Tables(nombre).Rows.Cast(Of DataRow)()
    End Function

    Private Shared Function Texto(fila As DataRow, columna As String) As String
        Return Convert.ToString(fila(columna), CultureInfo.InvariantCulture)
    End Function

    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El esquema de importación por carpetas no está disponible o es incompatible.")
    End Function

    Private Const ColumnasPlan As String = "plan_id,ambito_ref,usuario_ref,nivel_ref,version_id,estado,revision,esquema_doc,contexto_doc,config_doc,manifiesto_doc,resultado_doc,plan_hash,creado_utc,expira_utc"
    Private Const ColumnasLote As String = "lote_id,plan_id,ambito_ref,solicitante_ref,nivel_ref,clave_confirmacion,solicitud_hash,version_id,modo_integridad,ambito_integridad,contexto_doc,config_doc,estado,detencion_solicitada,total_raices,total_archivos,total_bytes,revision,creado_utc,actualizado_utc"

    'Consulta preventiva: no reserva codigo ni sustituye la revalidacion al
    'ejecutar frente a escritores legacy. Incluye todos los volumenes para no
    'inventar el volumen que Conservacion omite en su INSERT.
    Public Function ComprobarCoincidencias(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, manifiesto As ManifiestoImportacionCarpetas, resultado As ResultadoPreparacionImportacion) As List(Of IncidenciaImportacionCarpetas)
        AutorizarOperacion(contexto)
        Try
            Dim configuracionConexion As New MySqlConnectionStringBuilder(_conexion) With {.CharacterSet = "utf8"}
            Using conexion As New MySqlConnection(configuracionConexion.ConnectionString)
                conexion.Open()
                Return LeerCoincidencias(contexto, snapshot, manifiesto, resultado,
                    Function(sql, parametros)
                        Using cmd = ComandoOperacion(conexion, Nothing, sql, parametros)
                            cmd.CommandTimeout = 5
                            Using reader = cmd.ExecuteReader()
                                Dim datos As New DataTable()
                                datos.Load(reader)
                                Return datos
                            End Using
                        End Using
                    End Function)
            End Using
        Catch ex As MySqlException
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Function

    Friend Shared Function LeerCoincidencias(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, manifiesto As ManifiestoImportacionCarpetas, resultado As ResultadoPreparacionImportacion, leer As Func(Of String, Object(), DataTable)) As List(Of IncidenciaImportacionCarpetas)
        AutorizarOperacion(contexto)
        If snapshot Is Nothing OrElse snapshot.Configuracion Is Nothing OrElse manifiesto Is Nothing OrElse resultado Is Nothing OrElse leer Is Nothing Then Throw SolicitudInvalida()
        Dim configuracion = snapshot.Configuracion
        If Not ValidadorPerfilImportacion.ReferenciaValida(configuracion.AreaRef) OrElse Not {"BLOQUEAR", "REUTILIZAR_VALIDADO"}.Contains(configuracion.PoliticaCoincidencias) Then Throw SolicitudInvalida()
        Dim metadatos = leer("SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_SET_NAME,COLLATION_NAME,CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='expediente_archivo' AND COLUMN_NAME IN ('CODIGO_UNICO','ID_EMPRESA_EXPEDIENTE','CODIGO_AREA_TRD','ID_EXPEDIENTE')", New Object() {})
        If metadatos Is Nothing OrElse metadatos.Rows.Count <> 4 Then Throw NoDisponible()
        Dim codigo = metadatos.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "COLUMN_NAME") = "CODIGO_UNICO").ToList()
        If codigo.Count <> 1 OrElse Texto(codigo(0), "DATA_TYPE") <> "varchar" OrElse Convert.ToInt32(codigo(0)("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture) < 45 Then Throw NoDisponible()
        Dim charset = Texto(codigo(0), "CHARACTER_SET_NAME"), collation = Texto(codigo(0), "COLLATION_NAME")
        'Identificadores proceden de metadatos y de esta lista cerrada; nunca
        'se interpola un nombre/SQL del manifiesto o de configuracion cliente.
        If Not {"latin1", "utf8"}.Contains(charset) OrElse Not Regex.IsMatch(collation, "\A" & charset & "_[a-z0-9_]+\z", RegexOptions.CultureInvariant) Then Throw NoDisponible()
        For Each campo In {"ID_EXPEDIENTE", "ID_EMPRESA_EXPEDIENTE", "CODIGO_AREA_TRD"}
            Dim filas = metadatos.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "COLUMN_NAME") = campo).ToList()
            If filas.Count <> 1 OrElse Texto(filas(0), "DATA_TYPE") <> "int" Then Throw NoDisponible()
        Next
        Dim salida As New List(Of IncidenciaImportacionCarpetas)()
        Dim codigos As New Dictionary(Of String, String)(StringComparer.Ordinal)
        For Each raiz In manifiesto.Raices
            If resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso i.RaizIdCliente = raiz.IdCliente AndAlso String.IsNullOrEmpty(i.ArchivoIdCliente)) Then Continue For
            Dim elementos = resultado.Elementos.Where(Function(e) e.Ambito = "RAIZ" AndAlso e.RaizIdCliente = raiz.IdCliente).ToList()
            If elementos.Count <> 1 Then Throw SolicitudInvalida()
            Dim valores = elementos(0).Valores.Where(Function(v) v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = "CODIGO").ToList()
            If valores.Count <> 1 OrElse valores(0).Diferido OrElse String.IsNullOrWhiteSpace(valores(0).Valor) OrElse valores(0).Valor.Length > 45 Then Throw SolicitudInvalida()
            codigos.Add(raiz.IdCliente, valores(0).Valor)
        Next
        If codigos.Count = 0 Then Return salida
        Dim parametrosComparacion As New List(Of Object)(), seleccion As New List(Of String)(), representaciones As New List(Of String)()
        Dim indice = 0
        For Each par In codigos
            Dim r = "@raiz" & indice.ToString(CultureInfo.InvariantCulture), c = "@codigo" & indice.ToString(CultureInfo.InvariantCulture)
            parametrosComparacion.AddRange(New Object() {r, par.Key, c, par.Value})
            seleccion.Add("SELECT CONVERT(" & r & " USING ascii) COLLATE ascii_bin AS raiz,CONVERT(" & c & " USING " & charset & ") COLLATE " & collation & " AS codigo")
            representaciones.Add("SELECT " & r & " AS raiz,(HEX(CONVERT(" & c & " USING utf8))=HEX(CONVERT(CONVERT(" & c & " USING " & charset & ") USING utf8))) AS representable")
            indice += 1
        Next
        Dim codificacion = leer(String.Join(" UNION ALL ", representaciones), parametrosComparacion.ToArray())
        If codificacion Is Nothing OrElse codificacion.Rows.Count <> codigos.Count Then Throw NoDisponible()
        Dim vistas As New HashSet(Of String)(StringComparer.Ordinal)
        For Each fila In codificacion.Rows.Cast(Of DataRow)()
            Dim raiz = Texto(fila, "raiz")
            If Not codigos.ContainsKey(raiz) OrElse Not vistas.Add(raiz) Then Throw NoDisponible()
            If Texto(fila, "representable") <> "1" Then salida.Add(ConflictoRaiz(raiz, "CODIGO_NO_REPRESENTABLE", "El código no es representable en el origen documental."))
        Next
        If codigos.Count > 1 Then
            Dim consulta = String.Join(" UNION ALL ", seleccion)
            Dim duplicados = leer("SELECT a.raiz FROM (" & consulta & ") a INNER JOIN (" & consulta & ") b ON a.raiz<>b.raiz AND a.codigo=b.codigo GROUP BY a.raiz", parametrosComparacion.ToArray())
            If duplicados Is Nothing OrElse duplicados.Rows.Count > codigos.Count Then Throw NoDisponible()
            For Each fila In duplicados.Rows.Cast(Of DataRow)()
                Dim raiz = Texto(fila, "raiz")
                If Not codigos.ContainsKey(raiz) Then Throw NoDisponible()
                salida.Add(ConflictoRaiz(raiz, "CODIGO_REPETIDO_INVENTARIO", "Otra raíz propone el mismo código según la comparación del origen documental."))
            Next
        End If
        For Each par In codigos
            If salida.Any(Function(i) i.RaizIdCliente = par.Key) Then Continue For
            Dim candidatos = leer("SELECT e.ID_EXPEDIENTE FROM expediente_archivo e WHERE e.ID_EMPRESA_EXPEDIENTE=@empresa AND e.CODIGO_AREA_TRD=@area AND e.CODIGO_UNICO=@codigo LIMIT 2", New Object() {"@empresa", contexto.AmbitoRef, "@area", configuracion.AreaRef, "@codigo", par.Value})
            If candidatos Is Nothing OrElse candidatos.Rows.Count > 2 Then Throw NoDisponible()
            If candidatos.Rows.Count > 1 Then
                salida.Add(ConflictoRaiz(par.Key, "COINCIDENCIA_AMBIGUA", "Hay varios candidatos documentales; revise la raíz."))
            ElseIf candidatos.Rows.Count = 1 Then
                If configuracion.PoliticaCoincidencias = "BLOQUEAR" Then
                    salida.Add(ConflictoRaiz(par.Key, "CODIGO_EXPEDIENTE_EXISTENTE", "Existe un candidato para el código en la empresa y área; revise la raíz."))
                Else
                    salida.Add(ConflictoRaiz(par.Key, "REUTILIZACION_NO_DISPONIBLE", "La reutilización requiere verificar identidad, destino, clasificación, estado y permisos del candidato."))
                End If
            End If
        Next
        Return salida
    End Function

    Private Shared Function ConflictoRaiz(raiz As String, codigo As String, mensaje As String) As IncidenciaImportacionCarpetas
        Return New IncidenciaImportacionCarpetas With {.Ambito = "RAIZ", .RaizIdCliente = raiz, .Campo = "CAMPO_EXPEDIENTE:CODIGO", .Codigo = codigo, .Mensaje = mensaje, .BloqueaUnidad = True}
    End Function

    Public Function GuardarPlan(contexto As ContextoImportacionCarpetas, original As PlanImportacionCarpetas) As PlanImportacionCarpetas
        AutorizarOperacion(contexto)
        Dim plan = PrepararPersistencia(contexto, original)
        Dim instante = Ahora()
        If Fecha(plan.CreadoUtc) > instante OrElse Fecha(plan.ExpiraUtc) <= instante Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          VerificarPublicacion(conexion, tx, contexto, plan.Snapshot)
                          If Fecha(plan.ExpiraUtc) <= Ahora() Then Throw ConflictoOperacion()
                          Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_plan (ambito_ref,usuario_ref,nivel_ref,version_id,estado,revision,esquema_doc,contexto_doc,config_doc,manifiesto_doc,resultado_doc,plan_hash,creado_utc,expira_utc) VALUES (@ambito,@usuario,@nivel,@version,@estado,1,@esquema,@contexto,@config,@manifiesto,@resultado,@hash,@creado,@expira)",
                              "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef,
                              "@version", IdOperacion(plan.Snapshot.VersionId), "@estado", plan.Estado, "@esquema", plan.EsquemaDoc,
                              "@contexto", PerfilImportacionJson.Escribir(plan.Contexto), "@config", PerfilImportacionJson.Escribir(plan.Snapshot),
                              "@manifiesto", PerfilImportacionJson.Escribir(plan.Manifiesto), "@resultado", PerfilImportacionJson.Escribir(plan.Resultado),
                              "@hash", BytesHuella(plan.Huella), "@creado", Fecha(plan.CreadoUtc), "@expira", Fecha(plan.ExpiraUtc))
                              cmd.ExecuteNonQuery()
                              plan.PlanId = IdentidadInsertada(cmd)
                          End Using
                          'Una preparacion es una identidad nueva; nunca se actualiza un plan confirmado.
                          Return plan
                      End Function)
    End Function

    Public Function ObtenerPlan(contexto As ContextoImportacionCarpetas, planId As String) As PlanImportacionCarpetas
        AutorizarOperacion(contexto)
        Dim id = IdOperacion(planId)
        Return Operar(Function(conexion, tx) CargarPlan(conexion, tx, contexto, id, False))
    End Function

    'Resuelve solo la referencia persistida que necesita el servicio para volver
    'a autorizar el destino. No acepta nivel/empresa desde ConfirmarPlan HTTP.
    Public Function ObtenerContextoPlan(identidad As ContextoImportacionCarpetas, planId As String) As ContextoImportacionCarpetas Implements IConsultaImportacionRepository.ObtenerContextoPlan
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw AccesoDenegado()
        Dim id = IdOperacion(planId)
        Return Operar(Function(conexion, tx)
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT contexto_doc FROM ra_integracion_plan WHERE plan_id=@id AND usuario_ref=@usuario AND ambito_ref=@ambito", "@id", id, "@usuario", identidad.UsuarioRef, "@ambito", identidad.AmbitoRef)
                              Dim documento = cmd.ExecuteScalar()
                              If documento Is Nothing OrElse Convert.IsDBNull(documento) Then Throw AccesoDenegado()
                              Dim referencia = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(documento))
                              AutorizarOperacion(referencia)
                              If referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw AccesoDenegado()
                              Return referencia
                          End Using
                      End Function)
    End Function

    Public Function ConfirmarPlan(contexto As ContextoImportacionCarpetas, planId As String, revisionEsperada As UInteger, planHashEsperado As String, claveConfirmacion As String) As LoteImportacionCarpetas
        AutorizarOperacion(contexto)
        Dim id = IdOperacion(planId)
        If revisionEsperada = 0 OrElse revisionEsperada = UInteger.MaxValue OrElse Not CorrelacionValida(claveConfirmacion) Then Throw SolicitudInvalida()
        Dim hash = BytesHuella(planHashEsperado)
        Dim solicitudHash As Byte()
        Using sha = SHA256.Create()
            solicitudHash = sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {.planId = planId, .revision = revisionEsperada, .huella = planHashEsperado})))
        End Using
        Return Operar(Function(conexion, tx)
                          'Todos los confirmadores del mismo plan se serializan aqui.
                          Dim plan = CargarPlan(conexion, tx, contexto, id, True)
                          If Not Iguales(BytesHuella(plan.Huella), hash) Then Throw ConflictoOperacion()
                          Dim porClave = BuscarLote(conexion, tx, contexto, "clave_confirmacion", claveConfirmacion)
                          If porClave IsNot Nothing Then Return Repeticion(porClave, planId, solicitudHash)
                          Dim porPlan = BuscarLote(conexion, tx, contexto, "plan_id", id)
                          If porPlan IsNot Nothing Then Return Repeticion(porPlan, planId, solicitudHash)
                          If plan.Estado <> "REVISABLE" OrElse plan.Revision <> revisionEsperada Then Throw ConflictoOperacion()
                          ValidarConfirmable(plan)
                          VerificarPublicacion(conexion, tx, contexto, plan.Snapshot)
                          Dim instante = Ahora()
                          If Fecha(plan.ExpiraUtc) <= instante Then Throw ConflictoOperacion()
                          Dim total As ULong = 0
                          For Each archivo In plan.Manifiesto.Archivos
                              Dim bytes = BytesDecimales(archivo.Bytes)
                              If ULong.MaxValue - total < bytes Then Throw SolicitudInvalida()
                              total += bytes
                          Next
                          Dim loteId As String
                          Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_lote (plan_id,ambito_ref,solicitante_ref,nivel_ref,clave_confirmacion,solicitud_hash,version_id,modo_integridad,ambito_integridad,contexto_doc,config_doc,estado,detencion_solicitada,total_raices,total_archivos,total_bytes,revision,creado_utc,actualizado_utc) VALUES (@plan,@ambito,@usuario,@nivel,@clave,@hash,@version,@modo,@alcance,@contexto,@config,'RECIBIENDO',0,@raices,@archivos,@bytes,1,@instante,@instante)",
                              "@plan", id, "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef,
                              "@clave", claveConfirmacion, "@hash", solicitudHash, "@version", IdOperacion(plan.Snapshot.VersionId),
                              "@modo", plan.Snapshot.Configuracion.ModoIntegridad, "@alcance", plan.Snapshot.Configuracion.AmbitoIntegridad,
                              "@contexto", PerfilImportacionJson.Escribir(plan.Contexto), "@config", PerfilImportacionJson.Escribir(plan.Snapshot),
                              "@raices", CUInt(plan.Manifiesto.Raices.Count), "@archivos", CUInt(plan.Manifiesto.Archivos.Count), "@bytes", total, "@instante", instante)
                              UnaFila(cmd.ExecuteNonQuery())
                              loteId = IdentidadInsertada(cmd)
                          End Using
                          InsertarInventario(conexion, tx, plan, loteId, instante)
                          RegistrarEvento(conexion, tx, contexto, loteId, "CONFIRMAR", "LOTE_CONFIRMADO", instante)
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_plan SET estado='CONFIRMADO',revision=revision+1 WHERE plan_id=@plan AND revision=@revision AND estado='REVISABLE'", "@plan", id, "@revision", revisionEsperada)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("PLAN_CONFIRMADO", conexion, tx)
                          Dim confirmado = BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId))
                          If confirmado Is Nothing Then Throw NoDisponible()
                          Return confirmado
                      End Function)
    End Function

    Public Function ObtenerLote(contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ObtenerLote, IControlLoteImportacionRepository.ObtenerLote
        AutorizarOperacion(contexto)
        Dim id = IdOperacion(loteId)
        Return Operar(Function(conexion, tx)
                          Dim lote = BuscarLote(conexion, tx, contexto, "lote_id", id)
                          If lote Is Nothing Then Throw AccesoDenegado()
                          Return lote
                      End Function)
    End Function

    Public Function ObtenerContextoLote(identidad As ContextoImportacionCarpetas, loteId As String) As ContextoImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ObtenerContextoLote, IControlLoteImportacionRepository.ObtenerContextoLote, IConsultaImportacionRepository.ObtenerContextoLote
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw AccesoDenegado()
        Dim id = IdOperacion(loteId)
        Return Operar(Function(conexion, tx)
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT contexto_doc FROM ra_integracion_lote WHERE lote_id=@lote AND solicitante_ref=@usuario AND ambito_ref=@ambito", "@lote", id, "@usuario", identidad.UsuarioRef, "@ambito", identidad.AmbitoRef)
                              Dim documento = cmd.ExecuteScalar()
                              If documento Is Nothing OrElse Convert.IsDBNull(documento) Then Throw AccesoDenegado()
                              Dim referencia = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(documento))
                              AutorizarOperacion(referencia)
                              If referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw AccesoDenegado()
                              Return referencia
                          End Using
                      End Function)
    End Function

    Public Function ObtenerArchivo(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As ArchivoImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ObtenerArchivo
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId)
        Return Operar(Function(conexion, tx)
                          If BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId)) Is Nothing Then Throw AccesoDenegado()
                          Return CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, False)
                      End Function)
    End Function

    Public Function ObtenerIntentoRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String) As IntentoImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ObtenerIntentoRecepcion
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId)
        If intentoId IsNot Nothing Then IdOperacion(intentoId)
        Return Operar(Function(conexion, tx)
                          If BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId)) Is Nothing Then Throw AccesoDenegado()
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, False)
                          Dim intento = CargarIntentoRecepcion(conexion, tx, contexto, loteId, archivoId, intentoId, Nothing)
                          If intento Is Nothing Then Throw ConflictoOperacion()
                          ValidarIntentoRecepcion(archivo, intento, loteId, intento.TokenReclamacion)
                          Return intento
                      End Function)
    End Function

    'Reservar no invoca legacy. Un replay devuelve la identidad/token anteriores,
    'nunca una nueva autorizacion para ejecutar el efecto.
    Public Function ReservarIntento(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ReservarIntento, IControlLoteImportacionRepository.ReservarIntento
        AutorizarOperacion(contexto)
        If solicitud IsNot Nothing AndAlso solicitud.Fase <> "RECEPCION" Then Return ReservarIntentoDocumental(contexto, solicitud)
        ValidarSolicitudRecepcion(solicitud)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, solicitud.LoteId)
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, solicitud.LoteId, solicitud.RecursoId, True)
                          Dim repeticion = CargarIntentoRecepcion(conexion, tx, contexto, solicitud.LoteId, solicitud.RecursoId, Nothing, solicitud.SolicitudHash)
                          If repeticion IsNot Nothing Then Return repeticion
                          ValidarReservaRecepcion(lote, archivo, solicitud)
                          Dim instante = Ahora(), intentoId As String
                          Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_intento (lote_id,raiz_id,archivo_id,fase,estado,ejecutor_ref,token_reclamacion,solicitud_hash,inicio_utc,ultimo_contacto_utc) VALUES (@lote,@raiz,@archivo,'RECEPCION','EN_CURSO',@ejecutor,@token,@hash,@ahora,@ahora)",
                              "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(archivo.RaizId), "@archivo", IdOperacion(archivo.ArchivoId), "@ejecutor", solicitud.EjecutorRef, "@token", solicitud.TokenReclamacion, "@hash", BytesHuella(solicitud.SolicitudHash), "@ahora", instante)
                              UnaFila(cmd.ExecuteNonQuery()) : intentoId = IdentidadInsertada(cmd)
                          End Using
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_archivo SET estado_recepcion='RECIBIENDO',intento_activo_id=@intento,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND revision=@revision AND intento_activo_id IS NULL AND estado_recepcion IN ('PENDIENTE','RECHAZADO') AND estado_efecto='NO_INICIADO'", "@intento", IdOperacion(intentoId), "@ahora", instante, "@archivo", IdOperacion(archivo.ArchivoId), "@revision", archivo.Revision)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoRecepcion(conexion, tx, contexto, lote.LoteId, archivo.ArchivoId, intentoId, "RECEPCION_RESERVADA", New ResultadoEfectoImportacion With {.Estado = "EN_CURSO", .Codigo = "RECEPCION_RESERVADA", .Mensaje = "Intento de recepción reservado sin efectos documentales."}, instante)
                          ActualizarRevisionRecepcion(conexion, tx, lote.LoteId, instante)
                          Dim reservado = CargarIntentoRecepcion(conexion, tx, contexto, lote.LoteId, archivo.ArchivoId, intentoId, Nothing)
                          If reservado Is Nothing Then Throw NoDisponible()
                          Return reservado
                      End Function)
    End Function

    Public Function ConfirmarRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, token As String, recepcion As RecepcionVerificadaImportacion) As ArchivoImportacionCarpetas Implements IRecepcionArchivosImportacionRepository.ConfirmarRecepcion
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId) : IdOperacion(intentoId)
        If Not HexExacto(token, 32) Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          BloquearLoteRecepcion(conexion, tx, contexto, loteId)
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, True)
                          Dim intento = CargarIntentoRecepcion(conexion, tx, contexto, loteId, archivoId, intentoId, Nothing)
                          ValidarConfirmacionRecepcion(archivo, intento, loteId, token, recepcion)
                          If archivo.EstadoRecepcion = "RECIBIDO" Then Return archivo
                          Dim instante = Ahora()
                          Dim resultado As New ResultadoEfectoImportacion With {.Estado = "CONFIRMADO", .Codigo = "ARCHIVO_RECIBIDO", .Mensaje = "Contenido recibido y verificado; no implica incorporación documental.", .Recepcion = recepcion}
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_archivo SET bytes_recibidos=@bytes,contenido_hash=@hash,staging_ref=@staging,estado_recepcion='RECIBIDO',intento_activo_id=NULL,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND revision=@revision AND intento_activo_id=@intento AND estado_recepcion IN ('RECIBIENDO','INCIERTO')", "@bytes", BytesDecimales(recepcion.BytesRecibidos), "@hash", BytesHuella(recepcion.ContenidoHash), "@staging", recepcion.StagingRef, "@ahora", instante, "@archivo", IdOperacion(archivoId), "@revision", archivo.Revision, "@intento", IdOperacion(intentoId))
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("RECEPCION_ARCHIVO_CONFIRMADO", conexion, tx)
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_intento SET estado='CONFIRMADO',evidencia_doc=@evidencia,fin_utc=@ahora,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND token_reclamacion=@token AND estado IN ('EN_CURSO','INCIERTO')", "@evidencia", PerfilImportacionJson.Escribir(resultado), "@ahora", instante, "@intento", IdOperacion(intentoId), "@token", token)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoRecepcion(conexion, tx, contexto, loteId, archivoId, intentoId, "ARCHIVO_RECIBIDO", resultado, instante)
                          ActualizarRevisionRecepcion(conexion, tx, loteId, instante)
                          Return CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, False)
                      End Function)
    End Function

    Public Sub RegistrarFalloRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, token As String, codigo As String) Implements IRecepcionArchivosImportacionRepository.RegistrarFalloRecepcion
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(archivoId) : IdOperacion(intentoId)
        If Not HexExacto(token, 32) Then Throw SolicitudInvalida()
        Dim conocido = {"ARCHIVO_RECHAZADO", "LIMITE_EXCEDIDO", "FORMATO_NO_SOPORTADO", "SOLICITUD_INVALIDA", "RECEPCION_OCUPADA"}.Contains(codigo)
        If Not conocido AndAlso Not {"RESULTADO_INCIERTO", "CAPACIDAD_NO_DISPONIBLE", "ERROR_INTERNO", "ACCESO_DENEGADO", "CONFLICTO_REVISION", "OPERACION_EN_CURSO"}.Contains(codigo) Then Throw SolicitudInvalida()
        Dim registrado = Operar(Function(conexion, tx)
                                   BloquearLoteRecepcion(conexion, tx, contexto, loteId)
                                   Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, loteId, archivoId, True)
                                   Dim intento = CargarIntentoRecepcion(conexion, tx, contexto, loteId, archivoId, intentoId, Nothing)
                                   ValidarIntentoRecepcion(archivo, intento, loteId, token)
                                   'Una respuesta perdida no degrada un commit positivo.
                                   If archivo.EstadoRecepcion = "RECIBIDO" OrElse intento.Estado = "RECHAZADO" OrElse intento.Estado = "CONFIRMADO" Then Return True
                                   If archivo.IntentoActivoId <> intentoId Then Throw ConflictoOperacion()
                                   Dim estado = If(conocido, "RECHAZADO", "INCIERTO"), instante = Ahora()
                                   If archivo.EstadoRecepcion <> estado Then CoordinadorImportacionCarpetas.ValidarTransicion("RECEPCION", archivo.EstadoRecepcion, estado)
                                   If intento.Estado <> estado Then CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", intento.Estado, estado)
                                   Dim resultado As New ResultadoEfectoImportacion With {.Estado = estado, .Codigo = codigo, .Mensaje = "Recepción no confirmada; se conserva la evidencia privada."}
                                   Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_intento SET estado=@estado,evidencia_doc=@evidencia,fin_utc=@fin,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND token_reclamacion=@token AND estado IN ('EN_CURSO','INCIERTO')", "@estado", estado, "@evidencia", PerfilImportacionJson.Escribir(resultado), "@fin", If(conocido, DirectCast(instante, Object), DBNull.Value), "@ahora", instante, "@intento", IdOperacion(intentoId), "@token", token)
                                       UnaFila(cmd.ExecuteNonQuery())
                                   End Using
                                   Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_archivo SET estado_recepcion=@estado,intento_activo_id=@activo,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND intento_activo_id=@intento", "@estado", estado, "@activo", If(conocido, DirectCast(DBNull.Value, Object), IdOperacion(intentoId)), "@ahora", instante, "@archivo", IdOperacion(archivoId), "@intento", IdOperacion(intentoId))
                                       UnaFila(cmd.ExecuteNonQuery())
                                   End Using
                                   RegistrarEventoRecepcion(conexion, tx, contexto, loteId, archivoId, intentoId, "RECEPCION_NO_CONFIRMADA", resultado, instante)
                                   ActualizarRevisionRecepcion(conexion, tx, loteId, instante)
                                   Return True
                               End Function)
    End Sub

    Friend Shared Sub ValidarSolicitudRecepcion(solicitud As ReclamacionImportacionCarpetas)
        If solicitud Is Nothing OrElse solicitud.Fase <> "RECEPCION" OrElse solicitud.RecursoTipo <> "ARCHIVO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.RecursoId) OrElse solicitud.RevisionEsperada = 0 OrElse solicitud.RevisionEsperada >= UInteger.MaxValue - 1UI OrElse Not HexExacto(solicitud.TokenReclamacion, 32) OrElse Not HexExacto(solicitud.SolicitudHash, 64) OrElse String.IsNullOrEmpty(solicitud.EjecutorRef) OrElse solicitud.EjecutorRef.Length > 128 OrElse Not CorrelacionValida(solicitud.EjecutorRef) Then Throw SolicitudInvalida()
    End Sub

    Friend Shared Sub ValidarReservaRecepcion(lote As LoteImportacionCarpetas, archivo As ArchivoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas)
        ValidarSolicitudRecepcion(solicitud)
        If lote Is Nothing OrElse archivo Is Nothing OrElse lote.LoteId <> solicitud.LoteId OrElse archivo.ArchivoId <> solicitud.RecursoId OrElse archivo.Revision <> solicitud.RevisionEsperada Then Throw ConflictoOperacion()
        If lote.DetencionSolicitada OrElse Not {"RECIBIENDO", "LISTO", "EN_CURSO", "PARCIAL"}.Contains(lote.Estado) Then Throw ConflictoOperacion()
        If Not String.IsNullOrEmpty(archivo.IntentoActivoId) OrElse {"RECIBIENDO", "INCIERTO"}.Contains(archivo.EstadoRecepcion) Then Throw New PerfilImportacionException("OPERACION_EN_CURSO", "El archivo tiene un intento pendiente de verificación.")
        If Not {"PENDIENTE", "RECHAZADO"}.Contains(archivo.EstadoRecepcion) OrElse archivo.EstadoEfecto <> "NO_INICIADO" Then Throw ConflictoOperacion()
        CoordinadorImportacionCarpetas.ValidarTransicion("RECEPCION", archivo.EstadoRecepcion, "RECIBIENDO")
    End Sub

    Friend Shared Sub ValidarConfirmacionRecepcion(archivo As ArchivoImportacionCarpetas, intento As IntentoImportacionCarpetas, loteId As String, token As String, recepcion As RecepcionVerificadaImportacion)
        ValidarIntentoRecepcion(archivo, intento, loteId, token)
        If recepcion Is Nothing OrElse Not HexExacto(recepcion.StagingRef, 32) OrElse Not HexExacto(recepcion.ContenidoHash, 64) OrElse recepcion.BytesRecibidos <> archivo.BytesPrevistos OrElse BytesDecimales(recepcion.BytesRecibidos) = 0UL Then Throw SolicitudInvalida()
        If archivo.EstadoRecepcion = "RECIBIDO" Then
            If intento.Estado <> "CONFIRMADO" OrElse archivo.StagingRef <> recepcion.StagingRef OrElse archivo.BytesRecibidos <> recepcion.BytesRecibidos OrElse archivo.ContenidoHash <> recepcion.ContenidoHash OrElse intento.Resultado Is Nothing OrElse intento.Resultado.Recepcion Is Nothing OrElse PerfilImportacionJson.Escribir(intento.Resultado.Recepcion) <> PerfilImportacionJson.Escribir(recepcion) Then Throw ConflictoOperacion()
            Return
        End If
        If archivo.IntentoActivoId <> intento.IntentoId OrElse Not {"RECIBIENDO", "INCIERTO"}.Contains(archivo.EstadoRecepcion) OrElse Not {"EN_CURSO", "INCIERTO"}.Contains(intento.Estado) OrElse archivo.EstadoEfecto <> "NO_INICIADO" OrElse archivo.Revision = UInteger.MaxValue Then Throw ConflictoOperacion()
        CoordinadorImportacionCarpetas.ValidarTransicion("RECEPCION", archivo.EstadoRecepcion, "RECIBIDO")
        CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", intento.Estado, "CONFIRMADO")
    End Sub

    Private Shared Sub ValidarIntentoRecepcion(archivo As ArchivoImportacionCarpetas, intento As IntentoImportacionCarpetas, loteId As String, token As String)
        If archivo Is Nothing OrElse intento Is Nothing OrElse intento.Fase <> "RECEPCION" OrElse intento.LoteId <> loteId OrElse intento.ArchivoId <> archivo.ArchivoId OrElse intento.RaizId <> archivo.RaizId OrElse Not HexExacto(token, 32) OrElse intento.TokenReclamacion <> token Then Throw ConflictoOperacion()
    End Sub

    Private Shared Function HexExacto(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function

    Private Shared Function Texto(valor As Object) As String
        Return Convert.ToString(valor, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function BloquearLoteRecepcion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT lote_id FROM ra_integracion_lote WHERE lote_id=@lote AND ambito_ref=@ambito AND solicitante_ref=@usuario AND nivel_ref=@nivel FOR UPDATE", "@lote", IdOperacion(loteId), "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef)
            If cmd.ExecuteScalar() Is Nothing Then Throw AccesoDenegado()
        End Using
        Dim lote = BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId))
        If lote Is Nothing Then Throw AccesoDenegado()
        Return lote
    End Function

    Private Shared Function CargarArchivoRecepcion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, bloquear As Boolean) As ArchivoImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT f.*,r.cliente_raiz_id AS cliente_raiz,r.datos_doc AS datos_raiz FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE f.archivo_id=@archivo AND r.lote_id=@lote AND l.ambito_ref=@ambito AND l.solicitante_ref=@usuario AND l.nivel_ref=@nivel" & If(bloquear, " FOR UPDATE", ""), "@archivo", IdOperacion(archivoId), "@lote", IdOperacion(loteId), "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw AccesoDenegado()
                Return LeerArchivoRecepcion(reader)
            End Using
        End Using
    End Function

    Friend Shared Function LeerArchivoRecepcion(reader As IDataRecord) As ArchivoImportacionCarpetas
        Dim origen = LeerPersistido(Of ArchivoManifiestoImportacion)(CStr(reader("origen_doc")))
        Dim raiz = LeerPersistido(Of DocumentoRaizPersistida)(CStr(reader("datos_raiz")))
        Dim archivo As New ArchivoImportacionCarpetas With {.ArchivoId = Texto(reader("archivo_id")), .RaizId = Texto(reader("raiz_id")), .ClienteArchivoId = CStr(reader("cliente_archivo_id")),
            .NombreOriginal = origen.Nombre, .RutaRelativa = origen.RutaRelativa, .RutaHash = Hexadecimal(DirectCast(reader("ruta_hash"), Byte())), .UltimaModificacionLocal = origen.UltimaModificacionLocal,
            .BytesPrevistos = Convert.ToUInt64(reader("bytes_previstos"), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture), .EstadoRecepcion = CStr(reader("estado_recepcion")), .EstadoEfecto = CStr(reader("estado_efecto")), .Revision = Convert.ToUInt32(reader("revision"), CultureInfo.InvariantCulture),
            .BytesRecibidos = If(reader.IsDBNull(reader.GetOrdinal("bytes_recibidos")), Nothing, Texto(reader("bytes_recibidos"))),
            .ContenidoHash = If(reader.IsDBNull(reader.GetOrdinal("contenido_hash")), Nothing, Hexadecimal(DirectCast(reader("contenido_hash"), Byte()))),
            .StagingRef = If(reader.IsDBNull(reader.GetOrdinal("staging_ref")), Nothing, CStr(reader("staging_ref"))),
            .IntentoActivoId = If(reader.IsDBNull(reader.GetOrdinal("intento_activo_id")), Nothing, Texto(reader("intento_activo_id"))),
            .ValoresPlan = LeerPersistido(Of List(Of ValorReglaPerfilImportacion))(CStr(reader("plan_valores_doc")))}
        archivo.ValoresEfectivosFijados = Not reader.IsDBNull(reader.GetOrdinal("valores_efectivos_doc"))
        If archivo.ValoresEfectivosFijados Then archivo.ValoresEfectivos = LeerPersistido(Of List(Of ValorReglaPerfilImportacion))(CStr(reader("valores_efectivos_doc")))
        If Not reader.IsDBNull(reader.GetOrdinal("resultado_doc")) Then archivo.Resultado = LeerPersistido(Of ResultadoEfectoImportacion)(CStr(reader("resultado_doc")))
        If raiz.Esquema <> 1 OrElse raiz.ValoresPlan Is Nothing OrElse origen.IdCliente <> archivo.ClienteArchivoId OrElse origen.RaizIdCliente <> CStr(reader("cliente_raiz")) OrElse raiz.IdCliente <> origen.RaizIdCliente OrElse origen.Bytes <> archivo.BytesPrevistos OrElse archivo.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(archivo.EstadoEfecto) Then Throw NoDisponible()
        Using sha = SHA256.Create()
            Dim ruta = CanonizadorManifiestoImportacion.RutaCanonica(origen, New RaizManifiestoImportacion With {.IdCliente = raiz.IdCliente, .Nombre = raiz.NombreOriginal})
            If Hexadecimal(sha.ComputeHash(Encoding.UTF8.GetBytes(ruta))) <> archivo.RutaHash Then Throw NoDisponible()
        End Using
        Return archivo
    End Function

    Private Shared Function CargarIntentoRecepcion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, solicitudHash As String) As IntentoImportacionCarpetas
        Dim filtro As String = " AND i.estado='CONFIRMADO'"
        Dim parametros As New List(Of Object) From {"@lote", IdOperacion(loteId), "@archivo", IdOperacion(archivoId), "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef}
        If intentoId IsNot Nothing Then
            filtro = " AND i.intento_id=@intento" : parametros.AddRange(New Object() {"@intento", IdOperacion(intentoId)})
        ElseIf solicitudHash IsNot Nothing Then
            filtro = " AND i.solicitud_hash=@hash" : parametros.AddRange(New Object() {"@hash", BytesHuella(solicitudHash)})
        End If
        Using cmd = ComandoOperacion(conexion, tx, "SELECT i.* FROM ra_integracion_intento i INNER JOIN ra_integracion_lote l ON l.lote_id=i.lote_id WHERE i.lote_id=@lote AND i.archivo_id=@archivo AND i.fase='RECEPCION' AND l.ambito_ref=@ambito AND l.solicitante_ref=@usuario AND l.nivel_ref=@nivel" & filtro & " ORDER BY i.intento_id DESC LIMIT 1", parametros.ToArray())
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Return Nothing
                Dim intento As New IntentoImportacionCarpetas With {.IntentoId = Texto(reader("intento_id")), .LoteId = Texto(reader("lote_id")), .RaizId = Texto(reader("raiz_id")), .ArchivoId = Texto(reader("archivo_id")), .Fase = CStr(reader("fase")), .Estado = CStr(reader("estado")), .EjecutorRef = CStr(reader("ejecutor_ref")), .TokenReclamacion = CStr(reader("token_reclamacion")), .SolicitudHash = Hexadecimal(DirectCast(reader("solicitud_hash"), Byte())), .InicioUtc = TextoUtc(CDate(reader("inicio_utc"))), .UltimoContactoUtc = TextoUtc(CDate(reader("ultimo_contacto_utc")))}
                If Not reader.IsDBNull(reader.GetOrdinal("fin_utc")) Then intento.FinUtc = TextoUtc(CDate(reader("fin_utc")))
                If Not reader.IsDBNull(reader.GetOrdinal("evidencia_doc")) Then intento.Resultado = LeerPersistido(Of ResultadoEfectoImportacion)(CStr(reader("evidencia_doc")))
                If Not EstadosImportacionCarpetas.EsIntento(intento.Estado) OrElse Not HexExacto(intento.TokenReclamacion, 32) Then Throw NoDisponible()
                Return intento
            End Using
        End Using
    End Function

    Private Sub RegistrarEventoRecepcion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, codigo As String, resultado As ResultadoEfectoImportacion, instante As DateTime)
        If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("RECEPCION_ANTES_EVENTO", conexion, tx)
        Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,@intento,'ARCHIVO',@archivo,'RECEPCION',@codigo,@actor,@ahora,@detalle)", "@lote", IdOperacion(loteId), "@intento", IdOperacion(intentoId), "@archivo", IdOperacion(archivoId), "@codigo", codigo, "@actor", contexto.UsuarioRef, "@ahora", instante, "@detalle", PerfilImportacionJson.Escribir(resultado))
            UnaFila(cmd.ExecuteNonQuery())
        End Using
    End Sub

    Private Shared Sub ActualizarRevisionRecepcion(conexion As MySqlConnection, tx As MySqlTransaction, loteId As String, instante As DateTime)
        Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote", "@ahora", instante, "@lote", IdOperacion(loteId))
            UnaFila(cmd.ExecuteNonQuery())
        End Using
    End Sub

    Private Shared Function Repeticion(lote As LoteImportacionCarpetas, planId As String, solicitudHash As Byte()) As LoteImportacionCarpetas
        If lote.PlanId <> planId OrElse Not Iguales(BytesHuella(lote.SolicitudHash), solicitudHash) Then Throw ConflictoOperacion()
        'Un plan ya confirmado converge aun con otra clave; conserva su clave original.
        'No se vuelve a publicar ni se exige que su TTL siga vigente para consultarlo.
        Return lote
    End Function

    Private Shared Function BuscarLote(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, campo As String, referencia As Object) As LoteImportacionCarpetas
        If Not {"lote_id", "plan_id", "clave_confirmacion"}.Contains(campo) Then Throw SolicitudInvalida()
        Using cmd = ComandoOperacion(conexion, tx, "SELECT " & ColumnasLote & " FROM ra_integracion_lote WHERE " & campo & "=@ref AND ambito_ref=@ambito AND solicitante_ref=@usuario AND nivel_ref=@nivel", "@ref", referencia, "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Return Nothing
                Dim lote As New LoteImportacionCarpetas With {
                    .LoteId = Convert.ToString(reader("lote_id"), CultureInfo.InvariantCulture), .PlanId = Convert.ToString(reader("plan_id"), CultureInfo.InvariantCulture),
                    .Contexto = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(reader("contexto_doc"))),
                    .Snapshot = LeerPersistido(Of SnapshotPerfilImportacionOperativa)(CStr(reader("config_doc"))),
                    .ClaveConfirmacion = CStr(reader("clave_confirmacion")), .SolicitudHash = Hexadecimal(DirectCast(reader("solicitud_hash"), Byte())),
                    .Estado = CStr(reader("estado")), .DetencionSolicitada = Convert.ToBoolean(reader("detencion_solicitada"), CultureInfo.InvariantCulture),
                    .TotalRaices = Convert.ToUInt32(reader("total_raices"), CultureInfo.InvariantCulture), .TotalArchivos = Convert.ToUInt32(reader("total_archivos"), CultureInfo.InvariantCulture),
                    .TotalBytes = Convert.ToUInt64(reader("total_bytes"), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
                    .Revision = Convert.ToUInt32(reader("revision"), CultureInfo.InvariantCulture),
                    .CreadoUtc = TextoUtc(CDate(reader("creado_utc"))), .ActualizadoUtc = TextoUtc(CDate(reader("actualizado_utc")))}
                If Not MismoDestino(contexto, lote.Contexto) Then Throw ConflictoOperacion()
                If lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse lote.Snapshot.VersionId <> Convert.ToString(reader("version_id"), CultureInfo.InvariantCulture) OrElse lote.Snapshot.Configuracion.ModoIntegridad <> CStr(reader("modo_integridad")) OrElse lote.Snapshot.Configuracion.AmbitoIntegridad <> CStr(reader("ambito_integridad")) OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) OrElse lote.Revision = 0 Then Throw NoDisponible()
                If lote.Snapshot.Reglas Is Nothing OrElse lote.Snapshot.Reglas.Any(Function(r) r Is Nothing) OrElse Not HexExacto(lote.Snapshot.Huella, 64) OrElse Hexadecimal(PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = lote.Snapshot.Configuracion, .Reglas = lote.Snapshot.Reglas})) <> lote.Snapshot.Huella Then Throw NoDisponible()
                Return lote
            End Using
        End Using
    End Function

    Friend Shared Sub ValidarConfirmable(plan As PlanImportacionCarpetas)
        ValidarInventarioPersistible(plan)
        If Not plan.Resultado.PuedeConfirmar OrElse plan.Resultado.UnidadesElegibles.Count = 0 Then Throw ConflictoOperacion()
        Dim completo = plan.Snapshot.Configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO"
        For Each raiz In plan.Resultado.UnidadesElegibles
            If plan.Resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso i.RaizIdCliente = raiz AndAlso (completo OrElse String.IsNullOrEmpty(i.ArchivoIdCliente))) Then Throw ConflictoOperacion()
            If Not plan.Manifiesto.Archivos.Any(Function(a) a.RaizIdCliente = raiz AndAlso ArchivoElegible(plan, a)) Then Throw ConflictoOperacion()
        Next
        If completo AndAlso plan.Snapshot.Configuracion.AmbitoIntegridad = "LOTE" AndAlso (plan.Resultado.UnidadesElegibles.Count <> plan.Manifiesto.Raices.Count OrElse plan.Resultado.Incidencias.Any(Function(i) i.BloqueaUnidad)) Then Throw ConflictoOperacion()
    End Sub

    Private Shared Function ArchivoElegible(plan As PlanImportacionCarpetas, archivo As ArchivoManifiestoImportacion) As Boolean
        Return ServicioPreparacionImportacion.EsArchivoElegible(plan, archivo)
    End Function

    Private Shared Sub InsertarInventario(conexion As MySqlConnection, tx As MySqlTransaction, plan As PlanImportacionCarpetas, loteId As String, instante As DateTime)
        For Each raiz In plan.Manifiesto.Raices
            Dim datos As New DocumentoRaizPersistida With {.SeleccionId = raiz.SeleccionId, .IdCliente = raiz.IdCliente, .NombreOriginal = raiz.Nombre, .ValoresPlan = ValoresPlan(plan, raiz.IdCliente, Nothing)}
            Dim raizId As String
            Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_raiz (lote_id,cliente_raiz_id,datos_doc,estado,revision,creado_utc,actualizado_utc) VALUES (@lote,@cliente,@doc,@estado,1,@instante,@instante)", "@lote", IdOperacion(loteId), "@cliente", raiz.IdCliente, "@doc", PerfilImportacionJson.Escribir(datos), "@estado", If(plan.Resultado.UnidadesElegibles.Contains(raiz.IdCliente), "PENDIENTE", "BLOQUEADO"), "@instante", instante)
                UnaFila(cmd.ExecuteNonQuery())
                raizId = IdentidadInsertada(cmd)
            End Using
            Dim rutas As New Dictionary(Of String, String)(StringComparer.Ordinal)
            For Each archivo In plan.Manifiesto.Archivos.Where(Function(a) a.RaizIdCliente = raiz.IdCliente)
                Dim ruta = RutaCanonica(archivo, raiz)
                Dim hash As Byte()
                Using sha = SHA256.Create()
                    hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ruta))
                End Using
                Dim clave = Hexadecimal(hash)
                'Una coincidencia de hash nunca combina dos rutas distintas.
                If rutas.ContainsKey(clave) Then Throw SolicitudInvalida()
                rutas.Add(clave, ruta)
                Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_archivo (raiz_id,cliente_archivo_id,origen_doc,ruta_hash,bytes_previstos,estado_recepcion,estado_efecto,plan_valores_doc,revision,creado_utc,actualizado_utc) VALUES (@raiz,@cliente,@doc,@hash,@bytes,@recepcion,@efecto,@valores,1,@instante,@instante)",
                    "@raiz", IdOperacion(raizId), "@cliente", archivo.IdCliente, "@doc", PerfilImportacionJson.Escribir(archivo), "@hash", hash, "@bytes", BytesDecimales(archivo.Bytes),
                    "@recepcion", If(ArchivoElegible(plan, archivo), "PENDIENTE", "RECHAZADO"), "@efecto", If(ArchivoElegible(plan, archivo), "NO_INICIADO", "RECHAZADO"),
                    "@valores", PerfilImportacionJson.Escribir(ValoresPlan(plan, raiz.IdCliente, archivo.IdCliente)), "@instante", instante)
                    UnaFila(cmd.ExecuteNonQuery())
                End Using
            Next
        Next
    End Sub

    Private Shared Function ValoresPlan(plan As PlanImportacionCarpetas, raizId As String, archivoId As String) As List(Of ValorReglaPerfilImportacion)
        Dim elemento = plan.Resultado.Elementos.SingleOrDefault(Function(e) e.RaizIdCliente = raizId AndAlso If(e.ArchivoIdCliente, "") = If(archivoId, ""))
        Return If(elemento Is Nothing, New List(Of ValorReglaPerfilImportacion)(), elemento.Valores)
    End Function

    Private Class DocumentoRaizPersistida
        Public Property Esquema As UShort = 1
        Public Property SeleccionId As String
        Public Property IdCliente As String
        Public Property NombreOriginal As String
        Public Property ValoresPlan As List(Of ValorReglaPerfilImportacion)
    End Class

    Private Sub RegistrarEvento(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, accion As String, codigo As String, instante As DateTime)
        Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento (lote_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,'LOTE',@lote,@accion,@codigo,@actor,@instante,@detalle)",
            "@lote", IdOperacion(loteId), "@accion", accion, "@codigo", codigo, "@actor", contexto.UsuarioRef, "@instante", instante,
            "@detalle", PerfilImportacionJson.Escribir(New ResultadoEfectoImportacion With {.Estado = "CONFIRMADO", .Codigo = codigo, .Mensaje = "Inventario confirmado sin efectos documentales."}))
            If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("ANTES_EVENTO", conexion, tx)
            UnaFila(cmd.ExecuteNonQuery())
            If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("EVENTO_INSERTADO", conexion, tx)
        End Using
    End Sub

    Private Shared Sub UnaFila(cantidad As Integer)
        If cantidad <> 1 Then Throw ConflictoOperacion()
    End Sub

    Private Shared Function CargarPlan(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, id As UInteger, bloquear As Boolean) As PlanImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT " & ColumnasPlan & " FROM ra_integracion_plan WHERE plan_id=@id AND ambito_ref=@ambito AND usuario_ref=@usuario AND nivel_ref=@nivel" & If(bloquear, " FOR UPDATE", ""),
            "@id", id, "@ambito", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef, "@nivel", contexto.NivelRef)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw AccesoDenegado()
                Dim plan = LeerPlan(reader)
                If Not MismoDestino(contexto, plan.Contexto) Then Throw ConflictoOperacion()
                Return plan
            End Using
        End Using
    End Function

    Private Shared Function LeerPlan(reader As IDataRecord) As PlanImportacionCarpetas
        Dim plan As New PlanImportacionCarpetas With {
            .PlanId = Convert.ToString(reader("plan_id"), CultureInfo.InvariantCulture),
            .Estado = CStr(reader("estado")), .Revision = Convert.ToUInt32(reader("revision"), CultureInfo.InvariantCulture),
            .EsquemaDoc = Convert.ToUInt16(reader("esquema_doc"), CultureInfo.InvariantCulture),
            .Contexto = LeerPersistido(Of ContextoImportacionCarpetas)(CStr(reader("contexto_doc"))),
            .Snapshot = LeerPersistido(Of SnapshotPerfilImportacionOperativa)(CStr(reader("config_doc"))),
            .Manifiesto = LeerPersistido(Of ManifiestoImportacionCarpetas)(CStr(reader("manifiesto_doc"))),
            .Resultado = LeerPersistido(Of ResultadoPreparacionImportacion)(CStr(reader("resultado_doc"))),
            .Huella = Hexadecimal(DirectCast(reader("plan_hash"), Byte())),
            .CreadoUtc = TextoUtc(CDate(reader("creado_utc"))), .ExpiraUtc = TextoUtc(CDate(reader("expira_utc")))}
        If plan.EsquemaDoc <> 1 OrElse plan.Contexto Is Nothing OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing OrElse plan.Snapshot.Reglas Is Nothing OrElse plan.Resultado Is Nothing OrElse plan.Manifiesto Is Nothing Then Throw NoDisponible()
        If plan.Contexto.AmbitoRef <> CStr(reader("ambito_ref")) OrElse plan.Contexto.UsuarioRef <> CStr(reader("usuario_ref")) OrElse plan.Contexto.NivelRef <> CStr(reader("nivel_ref")) OrElse plan.Snapshot.VersionId <> Convert.ToString(reader("version_id"), CultureInfo.InvariantCulture) Then Throw NoDisponible()
        If Not EstadosImportacionCarpetas.EsPlan(plan.Estado) OrElse plan.Revision = 0 OrElse Not Iguales(PerfilImportacionJson.HuellaPlan(plan), BytesHuella(plan.Huella)) Then Throw NoDisponible()
        Return plan
    End Function

    'Validacion de representacion persistible, no sustituye preflight ni cuotas del servicio.
    Friend Shared Function PrepararPersistencia(contexto As ContextoImportacionCarpetas, original As PlanImportacionCarpetas) As PlanImportacionCarpetas
        AutorizarOperacion(contexto)
        If original Is Nothing OrElse Not String.IsNullOrEmpty(original.PlanId) Then Throw SolicitudInvalida()
        Dim plan = PerfilImportacionJson.Leer(Of PlanImportacionCarpetas)(PerfilImportacionJson.Escribir(original))
        If Not MismoDestino(contexto, plan.Contexto) OrElse plan.EsquemaDoc <> 1 OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing OrElse plan.Snapshot.Reglas Is Nothing OrElse plan.Resultado Is Nothing OrElse plan.Manifiesto Is Nothing Then Throw SolicitudInvalida()
        IdOperacion(plan.Snapshot.VersionId) : IdOperacion(plan.Snapshot.PerfilId)
        If plan.Snapshot.Reglas.Any(Function(r) r Is Nothing) Then Throw SolicitudInvalida()
        If plan.Snapshot.NumeroVersion = 0 OrElse Fecha(plan.ExpiraUtc) <= Fecha(plan.CreadoUtc) Then Throw SolicitudInvalida()
        Dim huellaPerfil = PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = plan.Snapshot.Configuracion, .Reglas = plan.Snapshot.Reglas})
        If Not Iguales(huellaPerfil, BytesHuella(plan.Snapshot.Huella)) Then Throw SolicitudInvalida()
        Dim configuracion = plan.Snapshot.Configuracion
        If Not ((configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso {"EXPEDIENTE", "LOTE"}.Contains(configuracion.AmbitoIntegridad)) OrElse (configuracion.ModoIntegridad = "PARCIAL_PERMITIDO" AndAlso configuracion.AmbitoIntegridad = "NO_APLICA")) Then Throw SolicitudInvalida()
        ValidarInventarioPersistible(plan)
        plan.Manifiesto.Raices = plan.Manifiesto.Raices.OrderBy(Function(r) r.IdCliente, StringComparer.Ordinal).ToList()
        plan.Manifiesto.Archivos = plan.Manifiesto.Archivos.OrderBy(Function(a) a.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(a) a.IdCliente, StringComparer.Ordinal).ToList()
        plan.Resultado.UnidadesElegibles = plan.Resultado.UnidadesElegibles.OrderBy(Function(r) r, StringComparer.Ordinal).ToList()
        plan.Resultado.Elementos = plan.Resultado.Elementos.OrderBy(Function(r) r.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(r) r.ArchivoIdCliente, StringComparer.Ordinal).ThenBy(Function(r) r.Ambito, StringComparer.Ordinal).ToList()
        plan.Snapshot.Reglas = plan.Snapshot.Reglas.OrderBy(Function(r) r.DestinoTipo, StringComparer.Ordinal).ThenBy(Function(r) r.DestinoRef, StringComparer.Ordinal).ToList()
        plan.Resultado.Incidencias = plan.Resultado.Incidencias.OrderBy(Function(i) i.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(i) i.ArchivoIdCliente, StringComparer.Ordinal).ThenBy(Function(i) i.Campo, StringComparer.Ordinal).ThenBy(Function(i) i.Codigo, StringComparer.Ordinal).ThenBy(Function(i) i.Mensaje, StringComparer.Ordinal).ThenBy(Function(i) i.Ambito, StringComparer.Ordinal).ThenBy(Function(i) i.BloqueaUnidad).ToList()
        For Each elemento In plan.Resultado.Elementos
            If elemento.Valores.Any(Function(v) v Is Nothing) Then Throw SolicitudInvalida()
            elemento.Valores = elemento.Valores.OrderBy(Function(v) v.DestinoTipo, StringComparer.Ordinal).ThenBy(Function(v) v.DestinoRef, StringComparer.Ordinal).ToList()
        Next
        plan.Estado = If(plan.Resultado.PuedeConfirmar, "REVISABLE", "BLOQUEADO")
        plan.Revision = 1UI
        plan.Huella = Hexadecimal(PerfilImportacionJson.HuellaPlan(plan))
        Return plan
    End Function

    Private Shared Sub ValidarInventarioPersistible(plan As PlanImportacionCarpetas)
        Dim manifiesto = plan.Manifiesto, resultado = plan.Resultado
        If manifiesto.Raices Is Nothing OrElse manifiesto.Archivos Is Nothing OrElse resultado.UnidadesElegibles Is Nothing OrElse resultado.Elementos Is Nothing OrElse resultado.Incidencias Is Nothing Then Throw SolicitudInvalida()
        Dim raices As New Dictionary(Of String, RaizManifiestoImportacion)(StringComparer.Ordinal)
        Dim archivos As New HashSet(Of String)(StringComparer.Ordinal), rutas As New HashSet(Of String)(StringComparer.Ordinal)
        Dim total As ULong = 0
        For Each raiz In manifiesto.Raices
            If raiz Is Nothing OrElse Not CorrelacionValida(raiz.IdCliente) OrElse Not CorrelacionValida(raiz.SeleccionId) OrElse String.IsNullOrWhiteSpace(raiz.Nombre) OrElse raices.ContainsKey(raiz.IdCliente) Then Throw SolicitudInvalida()
            raices.Add(raiz.IdCliente, raiz)
        Next
        For Each archivo In manifiesto.Archivos
            If archivo Is Nothing OrElse Not CorrelacionValida(archivo.IdCliente) OrElse archivo.RaizIdCliente Is Nothing OrElse Not raices.ContainsKey(archivo.RaizIdCliente) Then Throw SolicitudInvalida()
            If Not archivos.Add(archivo.RaizIdCliente & ":" & archivo.IdCliente) Then Throw SolicitudInvalida()
            Dim ruta = RutaCanonica(archivo, raices(archivo.RaizIdCliente))
            If Not rutas.Add(archivo.RaizIdCliente & ":" & ruta) Then Throw SolicitudInvalida()
            Dim bytes = BytesDecimales(archivo.Bytes)
            If ULong.MaxValue - total < bytes Then Throw SolicitudInvalida()
            total += bytes
        Next
        Dim elegibles As New HashSet(Of String)(StringComparer.Ordinal)
        For Each id In resultado.UnidadesElegibles
            If id Is Nothing OrElse Not raices.ContainsKey(id) OrElse Not elegibles.Add(id) OrElse Not manifiesto.Archivos.Any(Function(a) a.RaizIdCliente = id) Then Throw SolicitudInvalida()
        Next
        Dim elementos As New HashSet(Of String)(StringComparer.Ordinal)
        For Each elemento In resultado.Elementos
            If elemento Is Nothing OrElse elemento.Valores Is Nothing OrElse elemento.RaizIdCliente Is Nothing OrElse Not raices.ContainsKey(elemento.RaizIdCliente) Then Throw SolicitudInvalida()
            If String.IsNullOrEmpty(elemento.ArchivoIdCliente) Then
                If elemento.Ambito <> "RAIZ" Then Throw SolicitudInvalida()
            ElseIf elemento.Ambito <> "ARCHIVO" OrElse Not archivos.Contains(elemento.RaizIdCliente & ":" & elemento.ArchivoIdCliente) Then
                Throw SolicitudInvalida()
            End If
            If Not elementos.Add(elemento.RaizIdCliente & ":" & elemento.ArchivoIdCliente) Then Throw SolicitudInvalida()
        Next
        For Each incidencia In resultado.Incidencias
            If incidencia Is Nothing Then Throw SolicitudInvalida()
            If Not String.IsNullOrEmpty(incidencia.ArchivoIdCliente) AndAlso (String.IsNullOrEmpty(incidencia.RaizIdCliente) OrElse Not archivos.Contains(incidencia.RaizIdCliente & ":" & incidencia.ArchivoIdCliente)) Then Throw SolicitudInvalida()
            If Not String.IsNullOrEmpty(incidencia.RaizIdCliente) AndAlso Not raices.ContainsKey(incidencia.RaizIdCliente) Then Throw SolicitudInvalida()
        Next
        If resultado.PuedeConfirmar AndAlso (elegibles.Count = 0 OrElse resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso String.IsNullOrEmpty(i.RaizIdCliente))) Then Throw SolicitudInvalida()
    End Sub

    Private Shared Function RutaCanonica(archivo As ArchivoManifiestoImportacion, raiz As RaizManifiestoImportacion) As String
        Return CanonizadorManifiestoImportacion.RutaCanonica(archivo, raiz)
    End Function

    Private Shared Sub VerificarPublicacion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa)
        Using cmd = ComandoOperacion(conexion, tx, "SELECT v.perfil_id,v.numero,v.config_hash,v.configuracion_doc FROM ra_integracion_perfil_version v INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE v.version_id=@version AND p.ambito_ref=@ambito AND p.activo=1 AND v.estado='PUBLICADA' LOCK IN SHARE MODE", "@version", IdOperacion(snapshot.VersionId), "@ambito", contexto.AmbitoRef)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() OrElse reader.IsDBNull(reader.GetOrdinal("config_hash")) Then Throw ConflictoOperacion()
                If Convert.ToString(reader("perfil_id"), CultureInfo.InvariantCulture) <> snapshot.PerfilId OrElse Convert.ToUInt32(reader("numero"), CultureInfo.InvariantCulture) <> snapshot.NumeroVersion OrElse Not Iguales(DirectCast(reader("config_hash"), Byte()), BytesHuella(snapshot.Huella)) Then Throw ConflictoOperacion()
                Dim actual = PerfilImportacionJson.Leer(Of ConfiguracionPerfilImportacion)(CStr(reader("configuracion_doc")))
                If PerfilImportacionJson.Escribir(actual) <> PerfilImportacionJson.Escribir(snapshot.Configuracion) Then Throw ConflictoOperacion()
            End Using
        End Using
    End Sub

    Private Function Operar(Of T)(operacion As Func(Of MySqlConnection, MySqlTransaction, T), Optional aislamiento As IsolationLevel = IsolationLevel.ReadCommitted) As T
        Try
            Using conexion As New MySqlConnection(_conexion)
                conexion.Open()
                ValidarMetadatos(LeerMetadatos(conexion))
                Using tx = conexion.BeginTransaction(aislamiento)
                    Dim resultado = operacion(conexion, tx)
                    tx.Commit()
                    Return resultado
                End Using
            End Using
        Catch ex As MySqlException
            If {1062, 1205, 1213}.Contains(ex.Number) Then Throw ConflictoOperacion()
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Function

    Private Shared Function ComandoOperacion(conexion As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray parametros As Object()) As MySqlCommand
        Dim cmd As New MySqlCommand(sql, conexion, tx)
        cmd.CommandTimeout = 30
        For i = 0 To parametros.Length - 1 Step 2
            Dim valor = parametros(i + 1)
            cmd.Parameters.AddWithValue(CStr(parametros(i)), If(valor, DBNull.Value))
        Next
        Return cmd
    End Function

    Private Shared Function IdentidadInsertada(cmd As MySqlCommand) As String
        If cmd.LastInsertedId <= 0 OrElse cmd.LastInsertedId > UInteger.MaxValue Then Throw NoDisponible()
        Return cmd.LastInsertedId.ToString(CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AutorizarOperacion(contexto As ContextoImportacionCarpetas)
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.NivelRef) OrElse String.IsNullOrWhiteSpace(contexto.SelloDestino) OrElse String.IsNullOrWhiteSpace(contexto.RutaCanonica) Then Throw AccesoDenegado()
    End Sub

    Private Shared Function MismoDestino(a As ContextoImportacionCarpetas, b As ContextoImportacionCarpetas) As Boolean
        Return a IsNot Nothing AndAlso b IsNot Nothing AndAlso a.UsuarioRef = b.UsuarioRef AndAlso a.AmbitoRef = b.AmbitoRef AndAlso a.NivelRef = b.NivelRef AndAlso a.SelloDestino = b.SelloDestino AndAlso a.RutaCanonica = b.RutaCanonica AndAlso b.PuedeImportar
    End Function

    Private Shared Function CorrelacionValida(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant)
    End Function

    Private Shared Function IdOperacion(valor As String) As UInteger
        If Not ValidadorPerfilImportacion.ReferenciaValida(valor) Then Throw SolicitudInvalida()
        Return UInteger.Parse(valor, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function BytesDecimales(valor As String) As ULong
        Dim resultado As ULong
        If Not ULong.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, resultado) OrElse resultado.ToString(CultureInfo.InvariantCulture) <> valor Then Throw SolicitudInvalida()
        Return resultado
    End Function

    Private Function Ahora() As DateTime
        Dim utc = _utcAhora()
        If utc.Kind <> DateTimeKind.Utc Then Throw NoDisponible()
        Return New DateTime(utc.Ticks - utc.Ticks Mod TimeSpan.TicksPerSecond, DateTimeKind.Utc)
    End Function

    Private Shared Function TextoUtc(valor As DateTime) As String
        Return valor.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
    End Function

    Private Shared Function Fecha(valor As String) As DateTime
        Dim utc As DateTime
        If Not DateTime.TryParseExact(valor, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, utc) OrElse utc.Year < 1000 Then Throw SolicitudInvalida()
        Return utc
    End Function

    Private Shared Function BytesHuella(valor As String) As Byte()
        If valor Is Nothing OrElse Not Regex.IsMatch(valor, "\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant) Then Throw SolicitudInvalida()
        Return Enumerable.Range(0, 32).Select(Function(i) Convert.ToByte(valor.Substring(i * 2, 2), 16)).ToArray()
    End Function

    Private Shared Function Hexadecimal(valor As Byte()) As String
        Return BitConverter.ToString(valor).Replace("-", "").ToLowerInvariant()
    End Function

    Private Shared Function Iguales(a As Byte(), b As Byte()) As Boolean
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> 32 OrElse b.Length <> 32 Then Return False
        Dim diferencia As Integer = 0
        For i = 0 To 31
            diferencia = diferencia Or (CInt(a(i)) Xor CInt(b(i)))
        Next
        Return diferencia = 0
    End Function

    Private Shared Function AccesoDenegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "El recurso no está disponible para este contexto.")
    End Function
    Private Shared Function LeerPersistido(Of T)(texto As String) As T
        Try
            Return PerfilImportacionJson.Leer(Of T)(texto)
        Catch ex As PerfilImportacionException
            Throw NoDisponible()
        End Try
    End Function
    Private Shared Function SolicitudInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La preparación o solicitud no es válida.")
    End Function
    Private Shared Function ConflictoOperacion() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "La operación cambió. Consulte su estado antes de repetir.")
    End Function

    Private Shared ReadOnly ColumnasEsperadas As New Dictionary(Of String, String())(StringComparer.Ordinal) From {
        {"ra_integracion_plan", {
            "plan_id|int unsigned|||NO|auto_increment|",
            "ambito_ref|varchar|128|ascii_bin|NO||",
            "usuario_ref|varchar|128|ascii_bin|NO||",
            "nivel_ref|varchar|128|ascii_bin|NO||",
            "version_id|int unsigned|||NO||",
            "estado|varchar|40|ascii_bin|NO||",
            "revision|int unsigned|||NO||",
            "esquema_doc|smallint unsigned|||NO||",
            "contexto_doc|longtext|4294967295|ascii_bin|NO||",
            "config_doc|longtext|4294967295|ascii_bin|NO||",
            "manifiesto_doc|longtext|4294967295|ascii_bin|NO||",
            "resultado_doc|longtext|4294967295|ascii_bin|NO||",
            "plan_hash|binary|32||NO||",
            "creado_utc|datetime|||NO||",
            "expira_utc|datetime|||NO||"
        }},
        {"ra_integracion_lote", {
            "lote_id|int unsigned|||NO|auto_increment|",
            "plan_id|int unsigned|||NO||",
            "ambito_ref|varchar|128|ascii_bin|NO||",
            "solicitante_ref|varchar|128|ascii_bin|NO||",
            "nivel_ref|varchar|128|ascii_bin|NO||",
            "clave_confirmacion|varchar|64|ascii_bin|NO||",
            "solicitud_hash|binary|32||NO||",
            "version_id|int unsigned|||NO||",
            "modo_integridad|varchar|40|ascii_bin|NO||",
            "ambito_integridad|varchar|40|ascii_bin|YES||",
            "contexto_doc|longtext|4294967295|ascii_bin|NO||",
            "config_doc|longtext|4294967295|ascii_bin|NO||",
            "estado|varchar|40|ascii_bin|NO||",
            "detencion_solicitada|tinyint unsigned|||NO||",
            "total_raices|int unsigned|||NO||",
            "total_archivos|int unsigned|||NO||",
            "total_bytes|bigint unsigned|||NO||",
            "revision|int unsigned|||NO||",
            "creado_utc|datetime|||NO||",
            "actualizado_utc|datetime|||NO||"
        }},
        {"ra_integracion_raiz", {
            "raiz_id|int unsigned|||NO|auto_increment|",
            "lote_id|int unsigned|||NO||",
            "cliente_raiz_id|varchar|64|ascii_bin|NO||",
            "datos_doc|longtext|4294967295|ascii_bin|NO||",
            "estado|varchar|40|ascii_bin|NO||",
            "identidad_expediente_doc|longtext|4294967295|ascii_bin|YES||",
            "valores_efectivos_doc|longtext|4294967295|ascii_bin|YES||",
            "intento_activo_id|int unsigned|||YES||",
            "revision|int unsigned|||NO||",
            "creado_utc|datetime|||NO||",
            "actualizado_utc|datetime|||NO||"
        }},
        {"ra_integracion_archivo", {
            "archivo_id|int unsigned|||NO|auto_increment|",
            "raiz_id|int unsigned|||NO||",
            "cliente_archivo_id|varchar|64|ascii_bin|NO||",
            "origen_doc|longtext|4294967295|ascii_bin|NO||",
            "ruta_hash|binary|32||NO||",
            "bytes_previstos|bigint unsigned|||NO||",
            "bytes_recibidos|bigint unsigned|||YES||",
            "contenido_hash|binary|32||YES||",
            "staging_ref|char|32|ascii_bin|YES||",
            "estado_recepcion|varchar|40|ascii_bin|NO||",
            "estado_efecto|varchar|40|ascii_bin|NO||",
            "plan_valores_doc|longtext|4294967295|ascii_bin|NO||",
            "valores_efectivos_doc|longtext|4294967295|ascii_bin|YES||",
            "resultado_doc|longtext|4294967295|ascii_bin|YES||",
            "intento_activo_id|int unsigned|||YES||",
            "revision|int unsigned|||NO||",
            "creado_utc|datetime|||NO||",
            "actualizado_utc|datetime|||NO||"
        }},
        {"ra_integracion_intento", {
            "intento_id|int unsigned|||NO|auto_increment|",
            "lote_id|int unsigned|||NO||",
            "raiz_id|int unsigned|||YES||",
            "archivo_id|int unsigned|||YES||",
            "fase|varchar|40|ascii_bin|NO||",
            "estado|varchar|40|ascii_bin|NO||",
            "ejecutor_ref|varchar|128|ascii_bin|NO||",
            "token_reclamacion|char|32|ascii_bin|NO||",
            "solicitud_hash|binary|32||NO||",
            "inicio_utc|datetime|||NO||",
            "ultimo_contacto_utc|datetime|||NO||",
            "fin_utc|datetime|||YES||",
            "evidencia_doc|longtext|4294967295|ascii_bin|YES||"
        }},
        {"ra_integracion_evento", {
            "evento_id|int unsigned|||NO|auto_increment|",
            "lote_id|int unsigned|||NO||",
            "intento_id|int unsigned|||YES||",
            "recurso_tipo|varchar|40|ascii_bin|NO||",
            "recurso_id|int unsigned|||YES||",
            "accion|varchar|40|ascii_bin|NO||",
            "codigo|varchar|40|ascii_bin|NO||",
            "actor_ref|varchar|128|ascii_bin|NO||",
            "ocurrido_utc|datetime|||NO||",
            "detalle_doc|longtext|4294967295|ascii_bin|NO||"
        }},
        {"ra_integracion_perfil_version", {"version_id|int unsigned|||NO|auto_increment|"}},
        {"remit_dest_interno_perfil_produccion", {"IMPORTAR_CARPETAS|tinyint unsigned|||NO||0"}}
    }

    Private Shared ReadOnly IndicesEsperados As String() = {
        "ra_integracion_plan|PRIMARY|0|plan_id",
        "ra_integracion_plan|ix_integracion_plan_contexto|1|ambito_ref,usuario_ref,creado_utc",
        "ra_integracion_plan|ix_integracion_plan_version|1|version_id",
        "ra_integracion_lote|PRIMARY|0|lote_id",
        "ra_integracion_lote|uq_integracion_lote_plan|0|plan_id",
        "ra_integracion_lote|uq_integracion_lote_confirmacion|0|ambito_ref,solicitante_ref,clave_confirmacion",
        "ra_integracion_lote|ix_integracion_lote_contexto|1|ambito_ref,solicitante_ref,creado_utc",
        "ra_integracion_lote|ix_integracion_lote_estado|1|estado,actualizado_utc",
        "ra_integracion_lote|ix_integracion_lote_version|1|version_id",
        "ra_integracion_raiz|PRIMARY|0|raiz_id",
        "ra_integracion_raiz|uq_integracion_raiz_cliente|0|lote_id,cliente_raiz_id",
        "ra_integracion_raiz|ix_integracion_raiz_estado|1|lote_id,estado",
        "ra_integracion_archivo|PRIMARY|0|archivo_id",
        "ra_integracion_archivo|uq_integracion_archivo_cliente|0|raiz_id,cliente_archivo_id",
        "ra_integracion_archivo|uq_integracion_archivo_ruta|0|raiz_id,ruta_hash",
        "ra_integracion_archivo|ix_integracion_archivo_estado|1|raiz_id,estado_recepcion,estado_efecto",
        "ra_integracion_intento|PRIMARY|0|intento_id",
        "ra_integracion_intento|ix_integracion_intento_lote|1|lote_id,estado,inicio_utc",
        "ra_integracion_intento|ix_integracion_intento_raiz|1|raiz_id,fase",
        "ra_integracion_intento|ix_integracion_intento_archivo|1|archivo_id,fase",
        "ra_integracion_evento|PRIMARY|0|evento_id",
        "ra_integracion_evento|ix_integracion_evento_lote|1|lote_id,ocurrido_utc,evento_id",
        "ra_integracion_evento|ix_integracion_evento_intento|1|intento_id",
        "ra_integracion_perfil_version|PRIMARY|0|version_id"
    }

    Private Shared ReadOnly ClavesEsperadas As String() = {
        "ra_integracion_plan|fk_integracion_plan_version|version_id|ra_integracion_perfil_version|version_id",
        "ra_integracion_lote|fk_integracion_lote_plan|plan_id|ra_integracion_plan|plan_id",
        "ra_integracion_lote|fk_integracion_lote_version|version_id|ra_integracion_perfil_version|version_id",
        "ra_integracion_raiz|fk_integracion_raiz_lote|lote_id|ra_integracion_lote|lote_id",
        "ra_integracion_archivo|fk_integracion_archivo_raiz|raiz_id|ra_integracion_raiz|raiz_id",
        "ra_integracion_intento|fk_integracion_intento_lote|lote_id|ra_integracion_lote|lote_id",
        "ra_integracion_intento|fk_integracion_intento_raiz|raiz_id|ra_integracion_raiz|raiz_id",
        "ra_integracion_intento|fk_integracion_intento_archivo|archivo_id|ra_integracion_archivo|archivo_id",
        "ra_integracion_evento|fk_integracion_evento_lote|lote_id|ra_integracion_lote|lote_id",
        "ra_integracion_evento|fk_integracion_evento_intento|intento_id|ra_integracion_intento|intento_id"
    }
End Class
