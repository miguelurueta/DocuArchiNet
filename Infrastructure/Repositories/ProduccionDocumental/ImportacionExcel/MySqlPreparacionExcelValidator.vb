Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

'Sólo metadatos/SELECT de negocio y temporales de esta conexión. Compatible con MySQL 5.1.
Public Class MySqlPreparacionExcelValidator
    Private ReadOnly _cn As DbConnection
    Private ReadOnly _ctx As ContextoCargaExcel
    Private ReadOnly _politica As Func(Of String, PoliticaDestinoCargaExcel)
    Private _tx As DbTransaction
    Private ReadOnly _temporales As New List(Of String)
    Private _modoOriginal As String
    Private _modoTocado As Boolean
    Public Sub New(cn As DbConnection, c As ContextoCargaExcel, politica As Func(Of String, PoliticaDestinoCargaExcel), Optional tx As DbTransaction = Nothing)
        _cn = cn : _ctx = c : _politica = politica : _tx = tx
    End Sub
    Public Shared Function Identidad(cn As DbConnection, Optional tx As DbTransaction = Nothing) As String
        Using q = SqlCargaExcel.Cmd(cn, tx, "SELECT @@hostname,@@port,DATABASE()"), r = q.ExecuteReader()
            If Not r.Read() Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
            Return ReglasCargaExcel.HashDocumento(New With {.Servidor = SqlCargaExcel.Texto(r, 0), .Puerto = Convert.ToUInt32(r(1), CultureInfo.InvariantCulture), .Base = SqlCargaExcel.Texto(r, 2)})
        End Using
    End Function
    Public Sub TransaccionActual(tx As DbTransaction)
        _tx = tx
    End Sub
    Public Function Inspeccionar(p As PlantillaExcel, modo As String) As EsquemaCargaExcel
        If p Is Nothing OrElse Not ServicioPlantillasExcel.TablaValida(p.NOMBRE_PLANTILLA) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Dim e = Metadatos(p.NOMBRE_PLANTILLA)
        If Not e.Destino.Existe Then
            If modo <> "CREAR" Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
            Return Proyectado(p)
        End If
        If modo = "CREAR" Then Throw ReglasCargaExcel.Fallo("DESTINO_YA_EXISTE")
        e.Politica = _politica(p.NOMBRE_PLANTILLA)
        ValidarEsquema(p, e, modo)
        e.Hash = HuellaEsquema(e)
        Return e
    End Function
    Public Function Proyectado(p As PlantillaExcel) As EsquemaCargaExcel
        If p Is Nothing OrElse Not ServicioPlantillasExcel.TablaValida(p.NOMBRE_PLANTILLA) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Dim e As New EsquemaCargaExcel With {.Destino = New DestinoPlantillaExcel, .IdentidadFisica = Identidad(_cn, _tx), .Politica = New PoliticaDestinoCargaExcel With {.Modo = "PROYECTADA"}}
        Proyectar(p, e)
        ValidarEsquema(p, e, "CREAR")
        e.Hash = HuellaEsquema(e)
        Return e
    End Function
    Public Function Metadatos(tabla As String) As EsquemaCargaExcel
        If Not ServicioPlantillasExcel.TablaValida(tabla) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Dim e As New EsquemaCargaExcel With {.Destino = New DestinoPlantillaExcel, .IdentidadFisica = Identidad(_cn, _tx)}
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT ENGINE,TABLE_TYPE FROM information_schema.TABLES WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla", "@tabla", tabla), r = q.ExecuteReader()
            If r.Read() Then
                If SqlCargaExcel.Texto(r, 1) <> "BASE TABLE" Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
                e.Destino.Existe = True : e.Destino.Motor = SqlCargaExcel.Texto(r, 0)
            End If
        End Using

        If e.Destino.Existe Then
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,COLUMN_DEFAULT,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA FROM information_schema.COLUMNS WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla ORDER BY ORDINAL_POSITION", "@tabla", tabla), r = q.ExecuteReader()
                While r.Read()
                    e.Destino.Columnas.Add(New ColumnaDestinoExcel With {.Nombre = SqlCargaExcel.Texto(r, 0), .Tipo = SqlCargaExcel.Texto(r, 1), .TipoCompleto = SqlCargaExcel.Texto(r, 2), .Longitud = If(r.IsDBNull(3), New Nullable(Of Long), New Nullable(Of Long)(Convert.ToInt64(r(3), CultureInfo.InvariantCulture))), .Nulable = SqlCargaExcel.Texto(r, 4) = "YES", .Predeterminado = SqlCargaExcel.Texto(r, 5), .Charset = SqlCargaExcel.Texto(r, 6), .Collation = SqlCargaExcel.Texto(r, 7), .Extra = SqlCargaExcel.Texto(r, 8)})
                End While
            End Using
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla ORDER BY INDEX_NAME,SEQ_IN_INDEX", "@tabla", tabla), r = q.ExecuteReader()
                While r.Read()
                    Dim nombre = SqlCargaExcel.Texto(r, 0), indice = e.Indices.SingleOrDefault(Function(x) x.Nombre = nombre)
                    If indice Is Nothing Then
                        indice = New IndiceCargaExcel With {.Nombre = nombre, .Unico = Convert.ToInt32(r(1), CultureInfo.InvariantCulture) = 0} : e.Indices.Add(indice)
                    End If
                    indice.Componentes.Add(New ComponenteIndiceCargaExcel With {.Columna = SqlCargaExcel.Texto(r, 3), .Posicion = Convert.ToInt32(r(2), CultureInfo.InvariantCulture), .Prefijo = If(r.IsDBNull(4), New Nullable(Of Integer), New Nullable(Of Integer)(Convert.ToInt32(r(4), CultureInfo.InvariantCulture)))})
                End While
            End Using
        End If
        Return e
    End Function
    Private Sub Proyectar(p As PlantillaExcel, e As EsquemaCargaExcel)
        Dim charset, collation As String, maxCaracter As Integer
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@character_set_database,@@collation_database"), r = q.ExecuteReader()
            If Not r.Read() Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
            charset = SqlCargaExcel.Texto(r, 0) : collation = SqlCargaExcel.Texto(r, 1)
        End Using
        SqlCargaExcel.Identificador(charset) : SqlCargaExcel.Identificador(collation)
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT MAXLEN FROM information_schema.CHARACTER_SETS WHERE CHARACTER_SET_NAME=@charset", "@charset", charset)
            maxCaracter = Convert.ToInt32(q.ExecuteScalar(), CultureInfo.InvariantCulture)
        End Using
        If maxCaracter < 1 OrElse maxCaracter > 4 Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
        e.Destino.Motor = "InnoDB"
        For Each campo In p.Campos.OrderBy(Function(x) x.CAMPO_DESTINO, StringComparer.Ordinal)
            If Not ServicioPlantillasExcel.TipoCreable(campo.TIPO_CAMPO) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
            Dim tipo = campo.TIPO_CAMPO.ToLowerInvariant(), baseTipo = Regex.Match(tipo, "\A[a-z]+").Value
            Dim texto = baseTipo = "char" OrElse baseTipo = "varchar"
            Dim longitud As Nullable(Of Long) = Nothing
            If texto Then longitud = Long.Parse(Regex.Match(tipo, "\(([0-9]+)\)").Groups(1).Value, CultureInfo.InvariantCulture)
            Dim col As New ColumnaDestinoExcel With {.Nombre = campo.CAMPO_DESTINO, .Tipo = baseTipo, .TipoCompleto = tipo, .Longitud = longitud, .Nulable = campo.CAMPO_DESTINO <> p.CAMPO_UNICO, .Predeterminado = Nothing, .Charset = If(texto, charset, Nothing), .Collation = If(texto, collation, Nothing), .Extra = ""}
            If col.Nombre = p.CAMPO_UNICO AndAlso If(texto, col.Longitud.GetValueOrDefault() * maxCaracter, 32L) > 767 Then Throw ReglasCargaExcel.Fallo("INDICE_COMPLETO_NO_SOPORTADO")
            e.Destino.Columnas.Add(col)
        Next
        e.Indices.Add(New IndiceCargaExcel With {.Nombre = "uq_carga_clave", .Unico = True, .Componentes = New List(Of ComponenteIndiceCargaExcel) From {New ComponenteIndiceCargaExcel With {.Columna = p.CAMPO_UNICO, .Posicion = 1}}})
    End Sub
    Public Shared Function Claves(p As PlantillaExcel, e As EsquemaCargaExcel) As List(Of String)
        Dim clavesResult As New List(Of String)
        If e.Politica.Modo = "COLUMNA" Then clavesResult.Add(e.Politica.ColumnaEmpresa)
        If Not clavesResult.Contains(p.CAMPO_UNICO, StringComparer.Ordinal) Then clavesResult.Add(p.CAMPO_UNICO)
        Return clavesResult
    End Function
    Public Shared Sub ValidarEsquema(p As PlantillaExcel, e As EsquemaCargaExcel, modo As String)
        If modo <> "CREAR" AndAlso modo <> "AGREGAR" AndAlso modo <> "REEMPLAZAR" Then Throw ReglasCargaExcel.Fallo("MODO_INVALIDO")
        If e Is Nothing OrElse e.Destino Is Nothing OrElse e.Politica Is Nothing OrElse Not String.Equals(e.Destino.Motor, "InnoDB", StringComparison.OrdinalIgnoreCase) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
        If (modo = "CREAR") = e.Destino.Existe Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        If (e.Destino.Existe AndAlso e.Politica.Modo <> "CONEXION" AndAlso e.Politica.Modo <> "COLUMNA") OrElse (Not e.Destino.Existe AndAlso e.Politica.Modo <> "PROYECTADA") Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        If e.Destino.Existe AndAlso (e.Politica.Revision = 0 OrElse String.IsNullOrWhiteSpace(e.Politica.FuenteId)) Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        If e.Politica.Modo = "COLUMNA" Then
            Dim columna = e.Destino.Columnas.SingleOrDefault(Function(x) x.Nombre = e.Politica.ColumnaEmpresa)
            If columna Is Nothing OrElse Not Regex.IsMatch(columna.Tipo, "\A(?:char|varchar|tinyint|smallint|mediumint|int|bigint)\z", RegexOptions.IgnoreCase) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
        End If
        For Each nombre In Claves(p, e)
            Dim columna = e.Destino.Columnas.SingleOrDefault(Function(x) x.Nombre = nombre)
            If columna Is Nothing OrElse columna.Nulable OrElse Not String.IsNullOrEmpty(columna.Extra) OrElse Not p.Campos.Any(Function(x) x.CAMPO_DESTINO = nombre) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
            Definicion(columna)
        Next
        Dim identidad = Claves(p, e)
        If Not e.Indices.Any(Function(i) i.Unico AndAlso i.Componentes.Count = identidad.Count AndAlso i.Componentes.All(Function(c) Not c.Prefijo.HasValue AndAlso identidad.Contains(c.Columna, StringComparer.Ordinal)) AndAlso i.Componentes.Select(Function(c) c.Columna).Distinct(StringComparer.Ordinal).Count() = identidad.Count) Then Throw ReglasCargaExcel.Fallo("INDICE_UNICO_REQUERIDO")
        'Usar modo efectivo sin cambiar la huella física v1.
        Dim copia = Newtonsoft.Json.JsonConvert.DeserializeObject(Of PlantillaExcel)(Newtonsoft.Json.JsonConvert.SerializeObject(p))
        copia.Modo = modo
        ServicioPlantillasExcel.ValidarDestino(copia, e.Destino)
    End Sub
    Public Shared Function HuellaEsquema(e As EsquemaCargaExcel) As String
        Return ReglasCargaExcel.HashDocumento(New With {.Version = 1, .IdentidadFisica = e.IdentidadFisica, .Existe = e.Destino.Existe, .Motor = e.Destino.Motor, .Columnas = e.Destino.Columnas.OrderBy(Function(c) c.Nombre, StringComparer.Ordinal).ToArray(), .Indices = e.Indices.OrderBy(Function(i) i.Nombre, StringComparer.Ordinal).Select(Function(i) New With {.Nombre = i.Nombre, .Unico = i.Unico, .Componentes = i.Componentes.OrderBy(Function(c) c.Posicion).ToArray()}).ToArray()})
    End Function
    Public Shared Function Definicion(c As ColumnaDestinoExcel) As String
        If c Is Nothing OrElse Not Regex.IsMatch(If(c.TipoCompleto, ""), "\A(?:(?:tinyint|smallint|mediumint|int|bigint)(?:\([0-9]{1,3}\))?(?: unsigned)?|(?:decimal|numeric)\([0-9]{1,2},[0-9]{1,2}\)(?: unsigned)?|(?:char|varchar)\([0-9]{1,5}\)|date)\z", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
        Dim result = c.TipoCompleto
        If c.Charset IsNot Nothing Then result &= " CHARACTER SET " & SqlCargaExcel.Identificador(c.Charset) & " COLLATE " & SqlCargaExcel.Identificador(c.Collation)
        Return result & " NOT NULL"
    End Function
    Public Function Validar(p As PlantillaExcel, e As EsquemaCargaExcel, filas As List(Of FilaNormalizadaExcel), modo As String) As IncidenciasClavesExcel
        ValidarEsquema(p, e, modo)
        ActivarModoEstricto()
        Dim nombres = Claves(p, e), cols = nombres.Select(Function(n) e.Destino.Columnas.Single(Function(c) c.Nombre = n)).ToArray()
        Dim temporalFilas = "dx_f_" & Guid.NewGuid().ToString("N"), temporalGrupos = "dx_g_" & Guid.NewGuid().ToString("N")
        Dim partes = cols.Select(Function(c, i) "k" & i.ToString(CultureInfo.InvariantCulture) & " " & Definicion(c)).ToArray()
        CrearTemporal(temporalFilas, "_fila INT NOT NULL," & String.Join(",", partes))
        CrearTemporal(temporalGrupos, String.Join(",", partes))
        Dim maxBytes As Long
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@max_allowed_packet")
            maxBytes = Math.Min(65536L, Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) \ 2)
        End Using
        If maxBytes < 1024 Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
        Insertar(temporalFilas, nombres, filas, maxBytes)
        Dim lista = String.Join(",", nombres.Select(Function(n, i) "k" & i.ToString(CultureInfo.InvariantCulture))), resultado As New IncidenciasClavesExcel
        If e.Politica.Modo = "COLUMNA" Then
            AgregarConsulta(resultado, "EMPRESA_NO_AUTORIZADA", "SELECT _fila FROM " & temporalFilas & " WHERE k0<>@empresa ORDER BY _fila", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture))
        End If
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "INSERT INTO " & temporalGrupos & " (" & lista & ") SELECT " & lista & " FROM " & temporalFilas & " GROUP BY " & lista & " HAVING COUNT(*)>1")
            q.ExecuteNonQuery() : SinAdvertencias()
        End Using
        Dim iguales = String.Join(" AND ", nombres.Select(Function(n, i) "f.k" & i.ToString(CultureInfo.InvariantCulture) & "=g.k" & i.ToString(CultureInfo.InvariantCulture)))
        AgregarConsulta(resultado, "CLAVE_DUPLICADA_ARCHIVO", "SELECT f._fila FROM " & temporalFilas & " f INNER JOIN " & temporalGrupos & " g ON " & iguales & " ORDER BY f._fila", Nothing)
        If modo = "AGREGAR" Then
            Dim join = String.Join(" AND ", nombres.Select(Function(n, i) "f.k" & i.ToString(CultureInfo.InvariantCulture) & "=d." & SqlCargaExcel.Identificador(n)))
            Dim filtro = If(e.Politica.Modo = "COLUMNA", " WHERE d." & SqlCargaExcel.Identificador(e.Politica.ColumnaEmpresa) & "=@empresa", "")
            AgregarConsulta(resultado, "CLAVE_EXISTENTE_DESTINO", "SELECT DISTINCT f._fila FROM " & temporalFilas & " f INNER JOIN " & SqlCargaExcel.Identificador(p.NOMBRE_PLANTILLA) & " d ON " & join & filtro & " ORDER BY f._fila", If(e.Politica.Modo = "COLUMNA", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), Nothing))
        End If
        Return resultado
    End Function
    Public Sub ActivarModoEstricto()
        If _modoTocado Then Return
        If _modoTocado Then Return
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@session.sql_mode")
            _modoOriginal = Convert.ToString(q.ExecuteScalar(), CultureInfo.InvariantCulture)
        End Using
        Dim modos = _modoOriginal.Split(","c).Where(Function(x) x.Length > 0).ToList()
        For Each requerido In {"STRICT_ALL_TABLES", "NO_ZERO_DATE", "NO_ZERO_IN_DATE", "ERROR_FOR_DIVISION_BY_ZERO"}
            If Not modos.Contains(requerido, StringComparer.OrdinalIgnoreCase) Then modos.Add(requerido)
        Next
        _modoTocado = True
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SET SESSION sql_mode=@modo", "@modo", String.Join(",", modos))
            q.ExecuteNonQuery()
        End Using
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@session.sql_mode")
            If Not Convert.ToString(q.ExecuteScalar(), CultureInfo.InvariantCulture).Split(","c).Contains("STRICT_ALL_TABLES", StringComparer.OrdinalIgnoreCase) Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
        End Using
    End Sub
    Private Sub CrearTemporal(nombre As String, columnas As String)
        _temporales.Add(nombre) 'Incluye CREATE de resultado incierto para su DROP propio.
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "CREATE TEMPORARY TABLE " & nombre & " (" & columnas & ") ENGINE=InnoDB")
            q.ExecuteNonQuery() : SinAdvertencias()
        End Using
    End Sub
    Private Sub Insertar(tabla As String, claves As List(Of String), filas As List(Of FilaNormalizadaExcel), maxBytes As Long)
        Dim indice As Integer = 0
        While indice < filas.Count
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "")
                Dim grupos As New List(Of String), bytes As Long = 256
                While indice < filas.Count AndAlso q.Parameters.Count + claves.Count + 1 <= 128
                    Dim fila = filas(indice), valores = claves.Select(Function(n) fila.Valores.Single(Function(v) v.CampoDestino = n)).ToArray()
                    Dim costo = valores.Sum(Function(v) CLng(Encoding.UTF8.GetByteCount(If(v.Texto, If(v.NumeroCanonico, "")))) * 2L + 128L)
                    If bytes + costo > maxBytes Then
                        If grupos.Count = 0 Then Throw ReglasCargaExcel.Fallo("LIMITE_PAQUETE_EXCEDIDO")
                        Exit While
                    End If
                    Dim args As New List(Of String), nombre = "@p" & q.Parameters.Count.ToString(CultureInfo.InvariantCulture)
                    ImportacionExcelContextAdapter.Parametro(q, nombre, fila.Fila) : args.Add(nombre)
                    For Each valor In valores
                        If valor.EsNulo Then Throw ReglasCargaExcel.Fallo("VALOR_REQUERIDO")
                        nombre = "@p" & q.Parameters.Count.ToString(CultureInfo.InvariantCulture)
                        Dim dato As Object = If(valor.Tipo = "DATE", CType(valor.Fecha.Value, Object), If(valor.Tipo = "TEXTO", valor.Texto, valor.NumeroCanonico))
                        ImportacionExcelContextAdapter.Parametro(q, nombre, dato) : args.Add(nombre)
                    Next
                    grupos.Add("(" & String.Join(",", args) & ")") : bytes += costo : indice += 1
                End While
                q.CommandText = "INSERT INTO " & tabla & " VALUES " & String.Join(",", grupos)
                q.ExecuteNonQuery() : SinAdvertencias()
            End Using
        End While
    End Sub
    Private Sub SinAdvertencias()
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SHOW COUNT(*) WARNINGS")
            If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> 0 Then Throw ReglasCargaExcel.Fallo("CONVERSION_MYSQL_INVALIDA")
        End Using
    End Sub
    Private Sub AgregarConsulta(resultado As IncidenciasClavesExcel, codigo As String, sql As String, empresa As String)
        Using q = SqlCargaExcel.Cmd(_cn, _tx, sql)
            If empresa IsNot Nothing Then ImportacionExcelContextAdapter.Parametro(q, "@empresa", empresa)
            Using r = q.ExecuteReader()
                While r.Read()
                    resultado.Total += 1
                    If resultado.Muestra.Count < 25 Then resultado.Muestra.Add(New IncidenciaLecturaExcel With {.Codigo = codigo, .Mensaje = "Revise la identidad de la fila.", .Fila = Convert.ToInt32(r(0), CultureInfo.InvariantCulture)})
                End While
            End Using
        End Using
    End Sub
    Public Function Cerrar() As Boolean
        Dim limpio = True
        For Each nombre In _temporales
            Try
                Using q = SqlCargaExcel.Cmd(_cn, _tx, "DROP TEMPORARY TABLE IF EXISTS " & nombre)
                    q.ExecuteNonQuery()
                End Using
            Catch
                limpio = False
            End Try
        Next
        _temporales.Clear()
        If _modoTocado Then
            Try
                Using q = SqlCargaExcel.Cmd(_cn, _tx, "SET SESSION sql_mode=@modo", "@modo", _modoOriginal)
                    q.ExecuteNonQuery()
                End Using
                Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@session.sql_mode")
                    If Convert.ToString(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> _modoOriginal Then limpio = False
                End Using
            Catch
                limpio = False
            End Try
            _modoTocado = False
        End If
        Return limpio
    End Function
End Class
