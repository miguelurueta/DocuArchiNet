Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Class MySqlPerfilImportacionRepository
    Implements IPerfilImportacionRepository
    Private ReadOnly _conexion As String
    Private _esquemaVerificado As Boolean
    Private Const ColumnasPerfil As String = "perfil_id,ambito_ref,codigo,nombre,descripcion,activo,version_publicada_id,revision"
    Private Const ColumnasVersion As String = "version_id,perfil_id,numero,revision,estado,configuracion_doc"

    Public Sub New(conexion As String)
        If String.IsNullOrWhiteSpace(conexion) Then Throw NoDisponible()
        _conexion = conexion
    End Sub

    Private Function Abrir() As MySqlConnection
        Dim conexion As New MySqlConnection(_conexion)
        Try
            conexion.Open()
            Return conexion
        Catch ex As MySqlException
            conexion.Dispose()
            Throw NoDisponible()
        End Try
    End Function

    Public Sub VerificarEsquema() Implements IPerfilImportacionRepository.VerificarEsquema
        If _esquemaVerificado Then Return
        Try
            Using conexion = Abrir()
                Using cmd = Comando(conexion, Nothing, "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla') AND ENGINE='InnoDB' AND TABLE_COLLATION='ascii_bin'")
                    If Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) <> 3 Then Throw NoDisponible()
                End Using
                'Comprueba todas las columnas consumidas sin leer ni modificar datos.
                For Each consulta As String In {
                    "SELECT " & ColumnasPerfil & ",creado_por,actualizado_por,creado_utc,actualizado_utc FROM ra_integracion_perfil LIMIT 0",
                    "SELECT " & ColumnasVersion & ",nombre_visible,gabinete_id,modo_integridad,ambito_integridad,estrategia_carpetas,politica_subcarpetas,esquema_config,config_hash,creado_por,publicado_por,creado_utc,publicado_utc FROM ra_integracion_perfil_version LIMIT 0",
                    "SELECT regla_id,version_id,destino_tipo,destino_ref,fuente_tipo,etapa_resolucion,obligatoria,orden,regla_doc FROM ra_integracion_perfil_regla LIMIT 0"}
                    Using cmd = Comando(conexion, Nothing, consulta), reader = cmd.ExecuteReader()
                    End Using
                Next
                For Each indice In New String()() {
                    New String() {"ra_integracion_perfil", "uq_ra_integracion_perfil_ambito_codigo", "ambito_ref,codigo"},
                    New String() {"ra_integracion_perfil_version", "uq_ra_integracion_perfil_version_numero", "perfil_id,numero"},
                    New String() {"ra_integracion_perfil_regla", "uq_ra_integracion_regla_destino", "version_id,destino_tipo,destino_ref"}}
                    Using cmd = Comando(conexion, Nothing, "SELECT GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX SEPARATOR ',') FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND INDEX_NAME=@indice AND NON_UNIQUE=0 AND SUB_PART IS NULL", "@tabla", indice(0), "@indice", indice(1))
                        If Convert.ToString(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) <> indice(2) Then Throw NoDisponible()
                    End Using
                Next
                VerificarColumnas(conexion)
                For Each relacion In New String()() {
                    New String() {"ra_integracion_perfil_version", "perfil_id", "ra_integracion_perfil", "perfil_id"},
                    New String() {"ra_integracion_perfil_regla", "version_id", "ra_integracion_perfil_version", "version_id"}}
                    'MySQL 5.1 materializa el JOIN de estos catalogos con un coste alto.
                    'Consultar cada catalogo acotado mantiene la validacion sin cache global.
                    Dim restriccion As String
                    Using cmd = Comando(conexion, Nothing, "SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND COLUMN_NAME=@columna AND REFERENCED_TABLE_SCHEMA=DATABASE() AND REFERENCED_TABLE_NAME=@padre AND REFERENCED_COLUMN_NAME=@referencia LIMIT 2", "@tabla", relacion(0), "@columna", relacion(1), "@padre", relacion(2), "@referencia", relacion(3))
                        Using reader = cmd.ExecuteReader()
                            If Not reader.Read() Then Throw NoDisponible()
                            restriccion = CStr(reader(0))
                            If reader.Read() Then Throw NoDisponible()
                        End Using
                    End Using
                    Using cmd = Comando(conexion, Nothing, "SELECT COUNT(*) FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND CONSTRAINT_NAME=@restriccion AND REFERENCED_TABLE_NAME=@padre AND UPDATE_RULE='RESTRICT' AND DELETE_RULE='RESTRICT'", "@tabla", relacion(0), "@restriccion", restriccion, "@padre", relacion(2))
                        If Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) <> 1 Then Throw NoDisponible()
                    End Using
                Next
            End Using
            'Solo dentro de esta instancia por solicitud; no cache global que oculte cambios.
            _esquemaVerificado = True
        Catch ex As MySqlException
            Throw NoDisponible()
        End Try
    End Sub

    Private Shared Sub VerificarColumnas(conexion As MySqlConnection)
        Dim esquemas As New Dictionary(Of String, String()) From {
            {"ra_integracion_perfil", {
                "perfil_id|int unsigned|||NO", "ambito_ref|varchar|128|ascii_bin|NO", "codigo|varchar|64|ascii_bin|NO",
                "nombre|varchar|160|utf8_bin|NO", "descripcion|text|65535|utf8_bin|YES", "activo|tinyint unsigned|||NO",
                "version_publicada_id|int unsigned|||YES", "creado_por|varchar|128|ascii_bin|NO",
                "actualizado_por|varchar|128|ascii_bin|NO", "creado_utc|datetime|||NO", "actualizado_utc|datetime|||NO", "revision|int unsigned|||NO"}},
            {"ra_integracion_perfil_version", {
                "version_id|int unsigned|||NO", "perfil_id|int unsigned|||NO", "numero|int unsigned|||NO",
                "estado|varchar|40|ascii_bin|NO", "revision|int unsigned|||NO", "nombre_visible|varchar|160|utf8_bin|NO",
                "gabinete_id|int unsigned|||YES", "modo_integridad|varchar|40|ascii_bin|NO", "ambito_integridad|varchar|40|ascii_bin|YES",
                "estrategia_carpetas|varchar|40|ascii_bin|YES", "politica_subcarpetas|varchar|40|ascii_bin|YES", "esquema_config|smallint unsigned|||NO",
                "configuracion_doc|longtext|4294967295|ascii_bin|NO", "config_hash|binary|32||YES",
                "creado_por|varchar|128|ascii_bin|NO", "publicado_por|varchar|128|ascii_bin|YES", "creado_utc|datetime|||NO", "publicado_utc|datetime|||YES"}},
            {"ra_integracion_perfil_regla", {
                "regla_id|int unsigned|||NO", "version_id|int unsigned|||NO", "destino_tipo|varchar|40|ascii_bin|NO",
                "destino_ref|varchar|128|ascii_bin|NO", "fuente_tipo|varchar|40|ascii_bin|NO", "etapa_resolucion|varchar|40|ascii_bin|NO",
                "obligatoria|tinyint unsigned|||NO", "orden|int unsigned|||NO", "regla_doc|longtext|4294967295|ascii_bin|NO"}}}
        For Each esquema In esquemas
            Dim columnas As New Dictionary(Of String, String())(StringComparer.Ordinal)
            Using cmd = Comando(conexion, Nothing, "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,COLLATION_NAME,IS_NULLABLE,EXTRA FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla", "@tabla", esquema.Key)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim tipo = CStr(reader("DATA_TYPE"))
                        If CStr(reader("COLUMN_TYPE")).Contains("unsigned") Then tipo &= " unsigned"
                        columnas.Add(CStr(reader("COLUMN_NAME")), {tipo, Convert.ToString(reader("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture), Convert.ToString(reader("COLLATION_NAME"), CultureInfo.InvariantCulture), CStr(reader("IS_NULLABLE")), CStr(reader("EXTRA"))})
                    End While
                End Using
            End Using
            Dim clave = esquema.Value(0).Split("|"c)(0)
            For Each columna In columnas
                If columna.Value(4).Contains("auto_increment") <> (columna.Key = clave) Then Throw NoDisponible()
            Next
            For Each contrato In esquema.Value
                Dim partes = contrato.Split("|"c)
                If Not columnas.ContainsKey(partes(0)) Then Throw NoDisponible()
                Dim actual = columnas(partes(0))
                For i = 1 To 4
                    If actual(i - 1) <> partes(i) Then Throw NoDisponible()
                Next
            Next
            Dim pk = esquema.Value(0).Split("|"c)(0)
            Using cmd = Comando(conexion, Nothing, "SELECT GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX SEPARATOR ',') FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND INDEX_NAME='PRIMARY'", "@tabla", esquema.Key)
                If Convert.ToString(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) <> pk Then Throw NoDisponible()
            End Using
        Next
    End Sub

    Public Function Listar(contexto As ContextoPerfilImportacion, filtro As FiltroPerfilesImportacion) As PaginaPerfilesImportacion Implements IPerfilImportacionRepository.Listar
        Autorizar(contexto)
        If filtro Is Nothing OrElse filtro.Pagina < 1 OrElse filtro.Pagina > 1000000 OrElse filtro.TamanoPagina < 1 OrElse filtro.TamanoPagina > 50 Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Paginación inválida.")
        Return Ejecutar(Function(conexion)
                            Dim salida As New PaginaPerfilesImportacion()
                            Dim where = " WHERE ambito_ref=@ambito AND (@codigo IS NULL OR codigo=@codigo) AND (@nombre IS NULL OR LOCATE(@nombre,nombre)>0) AND (@activo IS NULL OR activo=@activo) AND (@termino IS NULL OR LOCATE(@termino,codigo)>0 OR LOCATE(@termino,nombre)>0)"
                            Dim parametros As Object() = {"@termino", VacioANulo(filtro.Termino), "@ambito", contexto.AmbitoRef, "@codigo", VacioANulo(filtro.Codigo), "@nombre", VacioANulo(filtro.Nombre), "@activo", If(filtro.Activo.HasValue, CType(If(filtro.Activo.GetValueOrDefault(), 1, 0), Object), Nothing)}
                            Using cmd = Comando(conexion, Nothing, "SELECT COUNT(*) FROM ra_integracion_perfil" & where, parametros)
                                salida.Total = Convert.ToInt64(cmd.ExecuteScalar(), CultureInfo.InvariantCulture)
                            End Using
                            Using cmd = Comando(conexion, Nothing, "SELECT " & ColumnasPerfil & " FROM ra_integracion_perfil" & where & " ORDER BY codigo,perfil_id LIMIT @limite OFFSET @offset", parametros)
                                cmd.Parameters.AddWithValue("@limite", filtro.TamanoPagina)
                                cmd.Parameters.AddWithValue("@offset", (filtro.Pagina - 1) * filtro.TamanoPagina)
                                Using reader = cmd.ExecuteReader()
                                    While reader.Read()
                                        salida.Elementos.Add(LeerPerfil(reader))
                                    End While
                                End Using
                            End Using
                            Return salida
                        End Function)
    End Function

    Public Function Obtener(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String) As PerfilImportacion Implements IPerfilImportacionRepository.Obtener
        Autorizar(contexto)
        Return Ejecutar(Function(conexion)
                            'Snapshot coherente de cabecera, versión y reglas.
                            Using tx = conexion.BeginTransaction(IsolationLevel.RepeatableRead)
                                Dim perfil = Cargar(conexion, tx, contexto, perfilId, versionId, False)
                                tx.Commit()
                                Return perfil
                            End Using
                        End Function)
    End Function

    Public Function Historial(contexto As ContextoPerfilImportacion, perfilId As String, pagina As Integer, tamano As Integer) As List(Of VersionPerfilImportacion) Implements IPerfilImportacionRepository.Historial
        Autorizar(contexto)
        If pagina < 1 OrElse pagina > 1000000 OrElse tamano < 1 OrElse tamano > 50 Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Paginación inválida.")
        Return Ejecutar(Function(conexion)
                            Dim salida As New List(Of VersionPerfilImportacion)()
                            Using cmd = Comando(conexion, Nothing, "SELECT v.version_id,v.perfil_id,v.numero,v.revision,v.estado,v.configuracion_doc FROM ra_integracion_perfil_version v INNER JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.ambito_ref=@ambito AND p.perfil_id=@perfil ORDER BY v.numero DESC LIMIT @limite OFFSET @offset", "@ambito", contexto.AmbitoRef, "@perfil", perfilId, "@limite", tamano, "@offset", (pagina - 1) * tamano)
                                Using reader = cmd.ExecuteReader()
                                    While reader.Read()
                                        salida.Add(LeerVersion(reader))
                                    End While
                                End Using
                            End Using
                            Return salida
                        End Function)
    End Function

    Public Function Crear(contexto As ContextoPerfilImportacion, codigo As String, nombre As String, descripcion As String) As PerfilImportacion Implements IPerfilImportacionRepository.Crear
        Autorizar(contexto)
        Dim perfilId As String = Nothing
        Dim version As New VersionPerfilImportacion With {.Numero = 1UI, .Revision = 1UI, .Estado = "BORRADOR", .Configuracion = New ConfiguracionPerfilImportacion With {.NombreVisible = nombre}}
        Return Transaccion(contexto, Function(conexion, tx)
                                         Dim ahora = DateTime.UtcNow
                                         Using cmd = Comando(conexion, tx, "INSERT INTO ra_integracion_perfil (ambito_ref,codigo,nombre,descripcion,activo,creado_por,actualizado_por,creado_utc,actualizado_utc,revision) VALUES (@ambito,@codigo,@nombre,@descripcion,1,@actor,@actor,@ahora,@ahora,1)", "@ambito", contexto.AmbitoRef, "@codigo", codigo, "@nombre", nombre, "@descripcion", descripcion, "@actor", contexto.UsuarioRef, "@ahora", ahora)
                                             cmd.ExecuteNonQuery()
                                             perfilId = IdInsertado(cmd)
                                             version.PerfilId = perfilId
                                         End Using
                                         InsertarVersion(conexion, tx, contexto, version)
                                         Return Cargar(conexion, tx, contexto, perfilId, version.VersionId, False)
                                     End Function)
    End Function

    Public Function Guardar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion) As PerfilImportacion Implements IPerfilImportacionRepository.Guardar
        Return Transaccion(contexto, Function(conexion, tx)
                                         Dim actual = Cargar(conexion, tx, contexto, perfilId, version.VersionId, True)
                                         RevisarVersion(actual.Version, version.Revision, True)
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil_version SET configuracion_doc=@doc,nombre_visible=@nombre,gabinete_id=@gabinete,modo_integridad=@modo,ambito_integridad=@ambito,estrategia_carpetas=@estrategia,politica_subcarpetas=@subcarpetas,esquema_config=@esquema,revision=revision+1 WHERE version_id=@version AND perfil_id=@perfil AND revision=@revision AND estado='BORRADOR'", ParametrosConfiguracion(version))
                                             cmd.Parameters.AddWithValue("@perfil", perfilId)
                                             cmd.Parameters.AddWithValue("@revision", version.Revision)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Using cmd = Comando(conexion, tx, "DELETE FROM ra_integracion_perfil_regla WHERE version_id=@version", "@version", version.VersionId)
                                             cmd.ExecuteNonQuery()
                                         End Using
                                         InsertarReglas(conexion, tx, version)
                                         Return Cargar(conexion, tx, contexto, perfilId, version.VersionId, False)
                                     End Function)
    End Function

    Public Function Publicar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, revisionPerfil As UInteger, revisionVersion As UInteger, huella As Byte()) As PerfilImportacion Implements IPerfilImportacionRepository.Publicar
        Return Transaccion(contexto, Function(conexion, tx)
                                         Dim actual = Cargar(conexion, tx, contexto, perfilId, versionId, True)
                                         RevisarCabecera(actual, revisionPerfil)
                                         RevisarVersion(actual.Version, revisionVersion, True)
                                         Dim huellaActual = PerfilImportacionJson.Huella(actual.Version)
                                         If huella Is Nothing OrElse Not huellaActual.SequenceEqual(huella) Then Throw Conflicto()
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil_version SET estado='PUBLICADA',revision=revision+1,config_hash=@hash,publicado_por=@actor,publicado_utc=@ahora WHERE version_id=@version AND revision=@revision AND estado='BORRADOR'", "@hash", huellaActual, "@actor", contexto.UsuarioRef, "@ahora", DateTime.UtcNow, "@version", versionId, "@revision", revisionVersion)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil SET version_publicada_id=@version,revision=revision+1,actualizado_por=@actor,actualizado_utc=@ahora WHERE perfil_id=@perfil AND ambito_ref=@ambito AND revision=@revision", "@version", versionId, "@actor", contexto.UsuarioRef, "@ahora", DateTime.UtcNow, "@perfil", perfilId, "@ambito", contexto.AmbitoRef, "@revision", revisionPerfil)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Return Cargar(conexion, tx, contexto, perfilId, versionId, False)
                                     End Function)
    End Function

    Public Function CrearVersion(contexto As ContextoPerfilImportacion, perfilId As String, origenId As String, revisionPerfil As UInteger, revisionOrigen As UInteger) As PerfilImportacion Implements IPerfilImportacionRepository.CrearVersion
        Return Transaccion(contexto, Function(conexion, tx)
                                         Dim actual = Cargar(conexion, tx, contexto, perfilId, origenId, True)
                                         RevisarCabecera(actual, revisionPerfil)
                                         RevisarVersion(actual.Version, revisionOrigen, False)
                                         If actual.Version.Estado <> "PUBLICADA" Then Throw Conflicto()
                                         Dim numero As UInteger
                                         Using cmd = Comando(conexion, tx, "SELECT MAX(numero) FROM ra_integracion_perfil_version WHERE perfil_id=@perfil", "@perfil", perfilId)
                                             numero = Convert.ToUInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture)
                                         End Using
                                         If numero = UInteger.MaxValue Then Throw Conflicto()
                                         actual.Version.VersionId = Nothing
                                         actual.Version.Numero = numero + 1UI
                                         actual.Version.Revision = 1UI
                                         actual.Version.Estado = "BORRADOR"
                                         InsertarVersion(conexion, tx, contexto, actual.Version)
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil SET revision=revision+1,actualizado_por=@actor,actualizado_utc=@ahora WHERE perfil_id=@perfil AND ambito_ref=@ambito AND revision=@revision", "@actor", contexto.UsuarioRef, "@ahora", DateTime.UtcNow, "@perfil", perfilId, "@ambito", contexto.AmbitoRef, "@revision", revisionPerfil)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Return Cargar(conexion, tx, contexto, perfilId, actual.Version.VersionId, False)
                                     End Function)
    End Function

    Public Function ActualizarCabecera(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, nombre As String, descripcion As String) As PerfilImportacion Implements IPerfilImportacionRepository.ActualizarCabecera
        Return Transaccion(contexto, Function(conexion, tx)
                                         RevisarCabecera(Cargar(conexion, tx, contexto, perfilId, Nothing, True), revision)
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil SET nombre=@nombre,descripcion=@descripcion,revision=revision+1,actualizado_por=@actor,actualizado_utc=@ahora WHERE perfil_id=@perfil AND ambito_ref=@ambito AND revision=@revision", "@nombre", nombre, "@descripcion", descripcion, "@actor", contexto.UsuarioRef, "@ahora", DateTime.UtcNow, "@perfil", perfilId, "@ambito", contexto.AmbitoRef, "@revision", revision)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Return Cargar(conexion, tx, contexto, perfilId, Nothing, False)
                                     End Function)
    End Function

    Public Function CambiarDisponibilidad(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, activo As Boolean) As PerfilImportacion Implements IPerfilImportacionRepository.CambiarDisponibilidad
        Return Transaccion(contexto, Function(conexion, tx)
                                         RevisarCabecera(Cargar(conexion, tx, contexto, perfilId, Nothing, True), revision)
                                         Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil SET activo=@activo,revision=revision+1,actualizado_por=@actor,actualizado_utc=@ahora WHERE perfil_id=@perfil AND ambito_ref=@ambito AND revision=@revision", "@activo", If(activo, 1, 0), "@actor", contexto.UsuarioRef, "@ahora", DateTime.UtcNow, "@perfil", perfilId, "@ambito", contexto.AmbitoRef, "@revision", revision)
                                             ExigirUna(cmd.ExecuteNonQuery())
                                         End Using
                                         Return Cargar(conexion, tx, contexto, perfilId, Nothing, False)
                                     End Function)
    End Function

    Private Shared Function Cargar(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, bloquear As Boolean) As PerfilImportacion
        Dim perfil As PerfilImportacion
        Dim bloqueo = If(bloquear, " FOR UPDATE", "")
        Using cmd = Comando(conexion, tx, "SELECT " & ColumnasPerfil & " FROM ra_integracion_perfil WHERE perfil_id=@perfil AND ambito_ref=@ambito" & bloqueo, "@perfil", perfilId, "@ambito", contexto.AmbitoRef)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Recurso no disponible para este contexto.")
                perfil = LeerPerfil(reader)
            End Using
        End Using
        Dim destino = If(String.IsNullOrEmpty(versionId), perfil.VersionPublicadaId, versionId)
        Dim filtro = If(String.IsNullOrEmpty(destino), " ORDER BY numero DESC LIMIT 1", " AND version_id=@version")
        Using cmd = Comando(conexion, tx, "SELECT " & ColumnasVersion & " FROM ra_integracion_perfil_version WHERE perfil_id=@perfil" & filtro & bloqueo, "@perfil", perfilId)
            If Not String.IsNullOrEmpty(destino) Then cmd.Parameters.AddWithValue("@version", destino)
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Versión no disponible para este contexto.")
                perfil.Version = LeerVersion(reader)
            End Using
        End Using
        Using cmd = Comando(conexion, tx, "SELECT regla_id,regla_doc FROM ra_integracion_perfil_regla WHERE version_id=@version ORDER BY orden,destino_tipo,destino_ref" & bloqueo, "@version", perfil.Version.VersionId)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim regla = PerfilImportacionJson.Leer(Of ReglaPerfilImportacion)(Convert.ToString(reader("regla_doc"), CultureInfo.InvariantCulture))
                    regla.ReglaId = Convert.ToString(reader("regla_id"), CultureInfo.InvariantCulture)
                    perfil.Version.Reglas.Add(regla)
                End While
            End Using
        End Using
        Return perfil
    End Function

    Private Shared Sub InsertarVersion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoPerfilImportacion, version As VersionPerfilImportacion)
        Using cmd = Comando(conexion, tx, "INSERT INTO ra_integracion_perfil_version (perfil_id,numero,revision,estado,nombre_visible,gabinete_id,modo_integridad,ambito_integridad,estrategia_carpetas,politica_subcarpetas,esquema_config,configuracion_doc,creado_por,creado_utc) VALUES (@perfil,@numero,1,'BORRADOR',@nombre,@gabinete,@modo,@ambito,@estrategia,@subcarpetas,@esquema,@doc,@actor,@ahora)", ParametrosConfiguracion(version))
            cmd.Parameters.AddWithValue("@perfil", version.PerfilId)
            cmd.Parameters.AddWithValue("@numero", version.Numero)
            cmd.Parameters.AddWithValue("@actor", contexto.UsuarioRef)
            cmd.Parameters.AddWithValue("@ahora", DateTime.UtcNow)
            cmd.ExecuteNonQuery()
            version.VersionId = IdInsertado(cmd)
        End Using
        InsertarReglas(conexion, tx, version)
    End Sub

    Private Shared Sub InsertarReglas(conexion As MySqlConnection, tx As MySqlTransaction, version As VersionPerfilImportacion)
        For Each original In version.Reglas
            Dim regla = PerfilImportacionJson.Leer(Of ReglaPerfilImportacion)(PerfilImportacionJson.Escribir(original))
            regla.ReglaId = Nothing
            Using cmd = Comando(conexion, tx, "INSERT INTO ra_integracion_perfil_regla (version_id,destino_tipo,destino_ref,fuente_tipo,etapa_resolucion,obligatoria,orden,regla_doc) VALUES (@version,@tipo,@destino,@fuente,@etapa,@obligatoria,@orden,@doc)", "@version", version.VersionId, "@tipo", regla.DestinoTipo, "@destino", regla.DestinoRef, "@fuente", regla.FuenteTipo, "@etapa", regla.EtapaResolucion, "@obligatoria", If(regla.Obligatoria, 1, 0), "@orden", regla.Orden, "@doc", PerfilImportacionJson.Escribir(regla))
                cmd.ExecuteNonQuery()
                regla.ReglaId = IdInsertado(cmd)
            End Using
            Using cmd = Comando(conexion, tx, "UPDATE ra_integracion_perfil_regla SET regla_doc=@doc WHERE regla_id=@regla AND version_id=@version", "@doc", PerfilImportacionJson.Escribir(regla), "@regla", regla.ReglaId, "@version", version.VersionId)
                ExigirUna(cmd.ExecuteNonQuery())
            End Using
        Next
    End Sub

    Private Shared Function ParametrosConfiguracion(version As VersionPerfilImportacion) As Object()
        Dim c = version.Configuracion
        Return {"@version", version.VersionId, "@doc", PerfilImportacionJson.Escribir(c), "@nombre", If(c.NombreVisible, ""), "@gabinete", If(c.GabineteId.HasValue, CType(c.GabineteId.GetValueOrDefault(), Object), Nothing), "@modo", c.ModoIntegridad, "@ambito", c.AmbitoIntegridad, "@estrategia", c.EstrategiaCarpetas, "@subcarpetas", c.PoliticaSubcarpetas, "@esquema", c.Esquema}
    End Function

    Private Shared Function LeerPerfil(reader As MySqlDataReader) As PerfilImportacion
        Return New PerfilImportacion With {.PerfilId = CStr(reader("perfil_id")), .AmbitoRef = CStr(reader("ambito_ref")), .Codigo = CStr(reader("codigo")), .Nombre = CStr(reader("nombre")), .Descripcion = If(reader.IsDBNull(reader.GetOrdinal("descripcion")), Nothing, CStr(reader("descripcion"))), .Activo = Convert.ToInt32(reader("activo"), CultureInfo.InvariantCulture) = 1, .VersionPublicadaId = If(reader.IsDBNull(reader.GetOrdinal("version_publicada_id")), Nothing, CStr(reader("version_publicada_id"))), .Revision = Convert.ToUInt32(reader("revision"), CultureInfo.InvariantCulture)}
    End Function

    Private Shared Function LeerVersion(reader As MySqlDataReader) As VersionPerfilImportacion
        Return New VersionPerfilImportacion With {.VersionId = CStr(reader("version_id")), .PerfilId = CStr(reader("perfil_id")), .Numero = Convert.ToUInt32(reader("numero"), CultureInfo.InvariantCulture), .Revision = Convert.ToUInt32(reader("revision"), CultureInfo.InvariantCulture), .Estado = CStr(reader("estado")), .Configuracion = PerfilImportacionJson.Leer(Of ConfiguracionPerfilImportacion)(CStr(reader("configuracion_doc")))}
    End Function

    Private Function Transaccion(contexto As ContextoPerfilImportacion, operacion As Func(Of MySqlConnection, MySqlTransaction, PerfilImportacion)) As PerfilImportacion
        Autorizar(contexto)
        Return Ejecutar(Function(conexion)
                            Using tx = conexion.BeginTransaction(IsolationLevel.ReadCommitted)
                                'Toda escritura del perfil bloquea primero cabecera, después versión.
                                Dim resultado = operacion(conexion, tx)
                                tx.Commit()
                                Return resultado
                            End Using
                        End Function)
    End Function

    Private Function Ejecutar(Of T)(operacion As Func(Of MySqlConnection, T)) As T
        Try
            Using conexion = Abrir()
                Return operacion(conexion)
            End Using
        Catch ex As MySqlException
            If ex.Number = 1062 Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "Existe una identidad en conflicto. Consulte el estado antes de repetir.")
            If ex.Number = 1205 OrElse ex.Number = 1213 Then Throw Conflicto()
            Throw NoDisponible()
        End Try
    End Function

    Private Shared Function IdInsertado(cmd As MySqlCommand) As String
        If cmd.LastInsertedId <= 0 OrElse cmd.LastInsertedId > UInteger.MaxValue Then Throw NoDisponible()
        Return cmd.LastInsertedId.ToString(CultureInfo.InvariantCulture)
    End Function

    Private Shared Function Comando(conexion As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray parametros As Object()) As MySqlCommand
        Dim cmd As New MySqlCommand(sql, conexion, tx)
        cmd.CommandTimeout = 30
        For i = 0 To parametros.Length - 1 Step 2
            cmd.Parameters.AddWithValue(CStr(parametros(i)), If(parametros(i + 1), DBNull.Value))
        Next
        Return cmd
    End Function

    Private Shared Sub Autorizar(contexto As ContextoPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Contexto no autorizado.")
    End Sub
    Private Shared Sub RevisarCabecera(perfil As PerfilImportacion, revision As UInteger)
        If perfil.Revision <> revision OrElse revision = UInteger.MaxValue Then Throw Conflicto()
    End Sub
    Private Shared Sub RevisarVersion(version As VersionPerfilImportacion, revision As UInteger, borrador As Boolean)
        If version.Revision <> revision OrElse revision = UInteger.MaxValue OrElse (borrador AndAlso version.Estado <> "BORRADOR") Then Throw Conflicto()
    End Sub
    Private Shared Sub ExigirUna(filas As Integer)
        If filas <> 1 Then Throw Conflicto()
    End Sub
    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "El recurso cambió. Consulte su estado antes de repetir.")
    End Function
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La configuración de perfiles no está disponible.")
    End Function
    Private Shared Function VacioANulo(valor As String) As String
        Return If(String.IsNullOrEmpty(valor), Nothing, valor)
    End Function
End Class
