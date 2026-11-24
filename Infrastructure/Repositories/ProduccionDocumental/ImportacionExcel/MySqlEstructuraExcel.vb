Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class MySqlEstructuraExcel
    Private ReadOnly _cn As DbConnection
    Private _tx As DbTransaction
    Private ReadOnly _validador As MySqlPreparacionExcelValidator
    Private ReadOnly _relacionesVerificadas As New HashSet(Of String)(StringComparer.Ordinal)
    Public Sub New(cn As DbConnection, validador As MySqlPreparacionExcelValidator, Optional tx As DbTransaction = Nothing)
        _cn = cn : _validador = validador : _tx = tx
    End Sub
    Public Sub TransaccionActual(tx As DbTransaction)
        _tx = tx
    End Sub
    Public Shared Function Marca(e As CargaExcelEjecucion) As String
        If Not ReglasCargaExcel.IdValido(e.Id) OrElse Not ReglasCargaExcel.HashValido(e.EsquemaHash) Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        'MySQL 5.1 limita TABLE COMMENT a 60 caracteres. Vincular el UUID y
        'la huella completa mediante SHA256/base64url, sin truncar ninguno.
        Dim vinculo = ReglasCargaExcel.HashDocumento(New With {.Carga = e.Id, .Esquema = e.EsquemaHash})
        Return "docuarchi/x1/" & Convert.ToBase64String(SqlCargaExcel.Binario(vinculo)).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
    End Function
    Private Shared Function MarcaPropia(comentario As String, e As CargaExcelEjecucion) As Boolean
        Return comentario = Marca(e) OrElse comentario = "docuarchi-excel/v1/" & e.Id & "/" & e.EsquemaHash
    End Function
    Public Shared Function HuellaCreable(e As EsquemaCargaExcel) As String
        Return ReglasCargaExcel.HashDocumento(New With {
            .Fisica = e.IdentidadFisica, .Motor = e.Destino.Motor.ToLowerInvariant(),
            .Columnas = e.Destino.Columnas.OrderBy(Function(c) c.Nombre, StringComparer.Ordinal).Select(Function(c) New With {.Nombre = c.Nombre, .Tipo = TipoCanonico(c.Tipo), .Completo = TipoCanonico(c.TipoCompleto), .Longitud = c.Longitud, .Nulable = c.Nulable, .Predeterminado = c.Predeterminado, .Charset = c.Charset, .Collation = c.Collation, .Extra = c.Extra}).ToArray(),
            .Indices = e.Indices.OrderBy(Function(i) i.Nombre, StringComparer.Ordinal).Select(Function(i) New With {.Nombre = i.Nombre, .Unico = i.Unico, .Componentes = i.Componentes.OrderBy(Function(c) c.Posicion).ToArray()}).ToArray()})
    End Function
    Private Shared Function TipoCanonico(tipo As String) As String
        Dim t = If(tipo, "").ToLowerInvariant()
        t = Regex.Replace(t, "\Ainteger\b", "int")
        t = Regex.Replace(t, "\Anumeric\b", "decimal")
        Return Regex.Replace(t, "\A(tinyint|smallint|mediumint|int|bigint)\([0-9]+\)", "$1")
    End Function
    Private Function Comentario(tabla As String) As String
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT TABLE_COMMENT FROM information_schema.TABLES WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla", "@tabla", tabla)
            Return Convert.ToString(q.ExecuteScalar(), CultureInfo.InvariantCulture)
        End Using
    End Function
    Public Function ConsultarPropia(p As PlantillaExcel, carga As CargaExcelEjecucion) As String
        Dim actual = _validador.Metadatos(p.NOMBRE_PLANTILLA)
        If Not actual.Destino.Existe Then Return "NO_CREADA"
        If Not MarcaPropia(Comentario(p.NOMBRE_PLANTILLA), carga) Then Return "AJENA"
        If HuellaCreable(actual) <> HuellaCreable(_validador.Proyectado(p)) Then Return "INCOMPATIBLE"
        Return "PROPIA_COMPATIBLE"
    End Function
    Public Function Inspeccionar(p As PlantillaExcel, carga As CargaExcelEjecucion) As EsquemaCargaExcel
        If p Is Nothing OrElse p.NOMBRE_PLANTILLA <> carga.Tabla Then Throw ReglasCargaExcel.Fallo("CONFLICTO_REVISION")
        VerificarSeguridad(carga.Tabla)
        If carga.Modo <> "CREAR" Then Return _validador.Inspeccionar(p, carga.Modo)
        Dim estado = ConsultarPropia(p, carga)
        If estado <> "NO_CREADA" AndAlso estado <> "PROPIA_COMPATIBLE" Then Throw ReglasCargaExcel.Fallo("CONFLICTO_ESTRUCTURA")
        'Conservar la proyección v1 incluso si el CREATE propio ya se materializó.
        Return _validador.Proyectado(p)
    End Function
    Public Sub ExigirVacia(tabla As String)
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT 1 FROM " & SqlCargaExcel.Identificador(tabla) & " LIMIT 1")
            If q.ExecuteScalar() IsNot Nothing Then Throw ReglasCargaExcel.Fallo("CONFLICTO_ESTRUCTURA")
        End Using
    End Sub
    Public Sub Asegurar(p As PlantillaExcel, carga As CargaExcelEjecucion)
        If _tx IsNot Nothing Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        Dim proyectado = Inspeccionar(p, carga)
        If proyectado.Hash <> carga.EsquemaHash Then Throw ReglasCargaExcel.Fallo("ESQUEMA_MODIFICADO")
        If carga.Modo <> "CREAR" Then Return
        If ConsultarPropia(p, carga) = "NO_CREADA" Then
            _validador.ActivarModoEstricto()
            Dim columnas = proyectado.Destino.Columnas.Select(
                Function(c)
                    Dim definicion = MySqlPreparacionExcelValidator.Definicion(c)
                    If c.Nulable Then definicion = definicion.Substring(0, definicion.Length - "NOT NULL".Length) & "NULL"
                    Return SqlCargaExcel.Identificador(c.Nombre) & " " & definicion
                End Function).ToList()
            columnas.Add("UNIQUE KEY `uq_carga_clave` (" & SqlCargaExcel.Identificador(p.CAMPO_UNICO) & ")")
            Try
                Using q = SqlCargaExcel.Cmd(_cn, Nothing, "CREATE TABLE " & SqlCargaExcel.Identificador(p.NOMBRE_PLANTILLA) & " (" & String.Join(",", columnas) & ") ENGINE=InnoDB COMMENT=@marca", "@marca", Marca(carga))
                    q.ExecuteNonQuery()
                End Using
            Catch
                'El DDL puede haberse aplicado. Consultar procedencia antes de reintentar.
                Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
            End Try
        End If
        If ConsultarPropia(p, carga) <> "PROPIA_COMPATIBLE" Then Throw ReglasCargaExcel.Fallo("CONFLICTO_ESTRUCTURA")
        ExigirVacia(p.NOMBRE_PLANTILLA)
    End Sub
    Private Function TienePrivilegio(privilegio As String, tabla As String, Optional globalSolamente As Boolean = False) As Boolean
        Const propio As String = "GRANTEE=CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',1)),'@',QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',-1)))"
        Dim sql = "SELECT COUNT(*) FROM information_schema.USER_PRIVILEGES WHERE " & propio & " AND PRIVILEGE_TYPE=@privilegio"
        Using q = SqlCargaExcel.Cmd(_cn, _tx, sql, "@privilegio", privilegio)
            If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) > 0 Then Return True
        End Using
        If globalSolamente Then Return False
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT COUNT(*) FROM information_schema.SCHEMA_PRIVILEGES WHERE " & propio & " AND BINARY TABLE_SCHEMA=BINARY DATABASE() AND PRIVILEGE_TYPE=@privilegio", "@privilegio", privilegio)
            If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) > 0 Then Return True
        End Using
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT COUNT(*) FROM information_schema.TABLE_PRIVILEGES WHERE " & propio & " AND BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla AND PRIVILEGE_TYPE=@privilegio", "@privilegio", privilegio, "@tabla", tabla)
            Return Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) > 0
        End Using
    End Function
    Public Sub VerificarSeguridad(tabla As String)
        If Not ServicioPlantillasExcel.TablaValida(tabla) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        For Each control In {"imp01_carga_ejecucion", "imp01_plantillaimp", "imp01_campos_plantilla"}
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT ENGINE,TABLE_TYPE FROM information_schema.TABLES WHERE BINARY TABLE_SCHEMA=BINARY DATABASE() AND BINARY TABLE_NAME=BINARY @tabla", "@tabla", control), r = q.ExecuteReader()
                If Not r.Read() OrElse Not String.Equals(SqlCargaExcel.Texto(r, 0), "InnoDB", StringComparison.OrdinalIgnoreCase) OrElse SqlCargaExcel.Texto(r, 1) <> "BASE TABLE" Then Throw ReglasCargaExcel.Fallo("ESQUEMA_INCOMPATIBLE")
            End Using
        Next
        'Sin visibilidad global no se acredita ausencia de FK entrantes de otras bases.
        'Nunca conceder permisos desde aquí; una adecuación requiere su archivo manual.
        If Not TienePrivilegio("SELECT", tabla, True) OrElse Not TienePrivilegio("TRIGGER", tabla) OrElse Not TienePrivilegio("TRIGGER", "imp01_carga_ejecucion") Then Throw ReglasCargaExcel.Fallo("METADATOS_NO_ACREDITADOS")
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT COUNT(*) FROM information_schema.TRIGGERS WHERE BINARY EVENT_OBJECT_SCHEMA=BINARY DATABASE() AND (BINARY EVENT_OBJECT_TABLE=BINARY @tabla OR EVENT_OBJECT_TABLE='imp01_carga_ejecucion')", "@tabla", tabla)
            If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> 0 Then Throw ReglasCargaExcel.Fallo("EFECTOS_RELACIONADOS_NO_ADMITIDOS")
        End Using
        'Acreditación acotada a esta operación/conexión/destino. MySQL 5.1 recorre
        'todas las bases al buscar FK entrantes. El mantenimiento DDL concurrente
        'está excluido; el CREATE propio no define FK. Nunca compartir entre cargas.
        Dim claveRelaciones = _cn.Database & "|" & tabla
        If Not _relacionesVerificadas.Contains(claveRelaciones) Then
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_NAME IS NOT NULL AND ((BINARY TABLE_SCHEMA=BINARY DATABASE() AND (BINARY TABLE_NAME=BINARY @tabla OR TABLE_NAME='imp01_carga_ejecucion')) OR (BINARY REFERENCED_TABLE_SCHEMA=BINARY DATABASE() AND (BINARY REFERENCED_TABLE_NAME=BINARY @tabla OR REFERENCED_TABLE_NAME='imp01_carga_ejecucion')))", "@tabla", tabla)
                'El barrido completo medido alcanza 15s. Presupuesto específico,
                'una vez por operación, dentro del límite HTTP existente de 30s.
                q.CommandTimeout = 20
                If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> 0 Then Throw ReglasCargaExcel.Fallo("EFECTOS_RELACIONADOS_NO_ADMITIDOS")
            End Using
            _relacionesVerificadas.Add(claveRelaciones)
        End If
    End Sub
End Class
