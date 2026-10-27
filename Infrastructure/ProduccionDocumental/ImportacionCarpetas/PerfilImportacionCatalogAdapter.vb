Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

'Dos orígenes explícitos. Ningún ID de gabinete/índice se consulta en RA.
Public Class PerfilImportacionCatalogAdapter
    Implements IPerfilImportacionCatalogos
    Implements IClasificacionEnsayoPerfil
    Private ReadOnly _ra As String
    Private ReadOnly _da As String
    Private ReadOnly _consulta As Func(Of String, String, Object(), DataTable)
    Private Const Organigramas As String = "SELECT ID_ORGANIGRAMA FROM registro_organigrama WHERE EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa"
    Private Const Instrumentos As String = "SELECT id_instrumento FROM ra_registro_instrumento_archivistico WHERE Estado_instrumento=1 AND registro_organigrama_ID_ORGANIGRAMA IN (" & Organigramas & ")"
    Private Const PermisoGabinete As String = "(EXISTS (SELECT 1 FROM permisos_usuarios_gabinetes pu WHERE pu.USUARIOS_DA_Clave_Usuario=@usuarioDA AND pu.NOMBRE_GABINETE=s.NOMBRE AND pu.ALMACENA_IMAGEN=1) OR EXISTS (SELECT 1 FROM permisos_grupos_gabinetes pg INNER JOIN relacion_usu_grup ru ON ru.GRUPOS_DA_Clave_Grupo=pg.GRUPOS_DA_Clave_Grupo WHERE ru.USUARIOS_DA_Clave_Usuario=@usuarioDA AND pg.NOMBRE_GABINETE=s.NOMBRE AND pg.ALMACENA_IMAGEN=1))"

    Public Sub New(conexionRA As String, conexionDA As String, Optional consulta As Func(Of String, String, Object(), DataTable) = Nothing)
        _ra = conexionRA
        _da = conexionDA
        _consulta = consulta
    End Sub

    Public Function ClasificacionPropuesta(contexto As ContextoPerfilImportacion, c As ConfiguracionPerfilImportacion) As Dictionary(Of String, String) Implements IClasificacionEnsayoPerfil.ClasificacionPropuesta
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse c Is Nothing Then Throw Invalida()
        Dim valores As New Dictionary(Of String, String)(StringComparer.Ordinal)
        For Each tipo In {"SERIE", "SUBSERIE"}
            Dim referencia = If(tipo = "SERIE", c.SerieRef, c.SubserieRef)
            If String.IsNullOrEmpty(referencia) Then
                'Sin serie, la clasificacion aun esta pendiente; sin subserie con serie es ausencia conocida.
                If tipo = "SUBSERIE" AndAlso Not String.IsNullOrEmpty(c.SerieRef) Then
                    For Each sufijo In {"ID", "CODIGO", "NOMBRE"}
                        valores.Add(tipo & "_" & sufijo, Nothing)
                    Next
                End If
                Continue For
            End If
            Dim opciones = Consultar(contexto, tipo, c, 1, 2, referencia)
            If opciones.Count <> 1 Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Clasificacion no autorizada para el perfil.")
            valores.Add(tipo & "_ID", opciones(0).value)
            valores.Add(tipo & "_NOMBRE", opciones(0).text)
            Dim codigos = Consultar(contexto, tipo & "_CODIGO", c, 1, 2, referencia)
            If codigos.Count <> 1 Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Codigo de clasificacion no disponible.")
            valores.Add(tipo & "_CODIGO", codigos(0).text)
        Next
        Return valores
    End Function

    Public Function Validar(contexto As ContextoPerfilImportacion, c As ConfiguracionPerfilImportacion, reglas As List(Of ReglaPerfilImportacion), publicar As Boolean) As List(Of IncidenciaPerfilImportacion) Implements IPerfilImportacionCatalogos.Validar
        Dim errores As New List(Of IncidenciaPerfilImportacion)()
        For Each referencia In New Dictionary(Of String, String) From {
            {"FONDO", c.FondoRef}, {"INSTRUMENTO", c.InstrumentoRef}, {"AREA", c.AreaRef},
            {"SERIE", c.SerieRef}, {"SUBSERIE", c.SubserieRef}, {"TIPO_EXPEDIENTE", c.TipoExpedienteRef}, {"UNIDAD_DOCUMENTAL", c.UnidadDocumentalRef}}
            If Not String.IsNullOrEmpty(referencia.Value) AndAlso Consultar(contexto, referencia.Key, c, 1, 1, referencia.Value).Count <> 1 Then Agregar(errores, referencia.Key, "REFERENCIA_NO_AUTORIZADA")
        Next
        If c.OrigenGabinete <> "DA" Then Agregar(errores, "OrigenGabinete", "ORIGEN_INVALIDO")
        If c.GabineteId.HasValue Then
            Dim actual = HuellaOrigen()
            If String.IsNullOrEmpty(c.HuellaOrigenGabinete) AndAlso Not publicar Then c.HuellaOrigenGabinete = actual
            If Not String.Equals(actual, c.HuellaOrigenGabinete, StringComparison.Ordinal) Then Agregar(errores, "GabineteId", "ORIGEN_CAMBIADO")
            Dim gabinete = Consultar(contexto, "GABINETE", c, 1, 1, c.GabineteId.Value.ToString(CultureInfo.InvariantCulture))
            If gabinete.Count <> 1 Then
                Agregar(errores, "GabineteId", "GABINETE_NO_AUTORIZADO")
            Else
                Dim campos = Me.Campos(contexto, c)
                For Each regla In reglas.Where(Function(r) r.DestinoTipo = "INDICE_GABINETE")
                    Dim campo = campos.SingleOrDefault(Function(f) f.value = regla.DestinoRef)
                    If campo Is Nothing OrElse Not TipoCompatible(campo, regla) Then Agregar(errores, regla.DestinoTipo & ":" & regla.DestinoRef, "INDICE_INCOMPATIBLE")
                Next
                If publicar Then
                    For Each campo In campos.Where(Function(f) f.Requerida)
                        If Not reglas.Any(Function(r) r.DestinoTipo = "INDICE_GABINETE" AndAlso r.DestinoRef = campo.value AndAlso r.Obligatoria) Then Agregar(errores, "INDICE_GABINETE:" & campo.value, "INDICE_REQUERIDO")
                    Next
                End If
            End If
        ElseIf reglas.Any(Function(r) r.DestinoTipo = "INDICE_GABINETE") Then
            Agregar(errores, "GabineteId", "GABINETE_REQUERIDO")
        End If
        Dim constantes = New ResolvedorReglasPerfilImportacion().Resolver(reglas.Where(Function(r) r.FuenteTipo = "CONSTANTE").ToList(), New EjemploPerfilImportacion())
        If constantes.Incidencias.Count = 0 Then errores.AddRange(ValidarValores(contexto, c, constantes.Valores))
        Return errores
    End Function

    Public Function ValidarValores(contexto As ContextoPerfilImportacion, c As ConfiguracionPerfilImportacion, valores As List(Of ValorReglaPerfilImportacion)) As List(Of IncidenciaPerfilImportacion) Implements IPerfilImportacionCatalogos.ValidarValores
        Dim errores As New List(Of IncidenciaPerfilImportacion)()
        Dim indices As List(Of OpcionPerfilImportacion) = Nothing
        If valores.Any(Function(v) v.DestinoTipo = "INDICE_GABINETE" AndAlso Not v.Diferido) Then indices = Campos(contexto, c)
        For Each valor In valores.Where(Function(v) Not v.Diferido)
            If valor.DestinoTipo = "TIPOLOGIA" Then
                'REFERENCIA es ID interno; TEXTO se busca por código_documento, nunca como ID implícito.
                Dim opciones = Consultar(contexto, If(valor.TipoValor = "REFERENCIA", "TIPOLOGIA", "TIPOLOGIA_CODIGO"), c, 1, 2, valor.Valor)
                If opciones.Count <> 1 Then
                    Agregar(errores, "TIPOLOGIA", "TIPOLOGIA_NO_UNIVOCA")
                Else
                    valor.Valor = opciones(0).value
                    valor.TipoValor = "REFERENCIA"
                End If
            ElseIf valor.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso valor.DestinoRef = "RESPONSABLE" Then
                Dim opciones = Consultar(contexto, If(valor.FuenteTipo = "CARPETA", "RESPONSABLE_IDENTIFICACION", "RESPONSABLE"), c, 1, 2, valor.Valor)
                If opciones.Count <> 1 Then
                    Agregar(errores, "RESPONSABLE", "RESPONSABLE_NO_AUTORIZADO")
                Else
                    valor.Valor = opciones(0).value
                End If
            ElseIf valor.DestinoTipo = "INDICE_GABINETE" Then
                Dim campos = indices.Where(Function(f) f.value = valor.DestinoRef).ToList()
                If campos.Count = 1 AndAlso Not campos(0).Requerida AndAlso valor.FuenteTipo = "EXPEDIENTE" AndAlso valor.Valor Is Nothing Then Continue For
                If campos.Count <> 1 OrElse Not ValorCompatible(campos(0), valor.Valor) Then Agregar(errores, "INDICE_GABINETE:" & valor.DestinoRef, "VALOR_INDICE_INVALIDO")
            End If
        Next
        Return errores
    End Function

    Public Function NivelCompatible(contexto As ContextoPerfilImportacion, c As ConfiguracionPerfilImportacion, nivelRef As String) As Boolean Implements IPerfilImportacionCatalogos.NivelCompatible
        'El nivel es de Producción, no de clasificación; se conserva y se autoriza sin sustituirlo.
        Dim tabla = Leer(_ra, "SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.id_nivel=@nivel AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=@empresa AND (u.id_Remit_Dest_Int=@usuario OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=@usuario AND p.estado_permiso=1 AND p.agregar_expediente=1))", "@nivel", nivelRef, "@empresa", contexto.AmbitoRef, "@usuario", contexto.UsuarioRef)
        Return tabla.Rows.Count = 1
    End Function

    Public Function Consultar(contexto As ContextoPerfilImportacion, catalogo As String, c As ConfiguracionPerfilImportacion, pagina As Integer, tamano As Integer, Optional referencia As String = Nothing) As List(Of OpcionPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse c Is Nothing OrElse pagina < 1 OrElse pagina > 1000000 OrElse tamano < 1 OrElse tamano > 50 Then Throw Invalida()
        If referencia IsNot Nothing AndAlso Not {"TIPOLOGIA_CODIGO", "RESPONSABLE_IDENTIFICACION"}.Contains(catalogo) AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(referencia) Then Throw Invalida()
        For Each filtro In {c.InstrumentoRef, c.AreaRef, c.SerieRef, c.SubserieRef}
            If filtro IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(filtro) Then Throw Invalida()
        Next
        Dim codigoClasificacion = catalogo = "SERIE_CODIGO" OrElse catalogo = "SUBSERIE_CODIGO"
        If codigoClasificacion Then catalogo = catalogo.Replace("_CODIGO", "")
        Dim consulta As String
        Dim origen = _ra
        Dim parametros As New List(Of Object) From {"@empresa", contexto.AmbitoRef, "@instrumento", c.InstrumentoRef, "@area", c.AreaRef, "@serie", c.SerieRef, "@subserie", c.SubserieRef}
        Select Case catalogo
            Case "FONDO"
                consulta = "SELECT Id_fondo_documental AS id,Nombre_fondo_documental AS nombre FROM ra_de_fondo_documental"
            Case "INSTRUMENTO"
                consulta = "SELECT id_instrumento AS id,nombre_instrumento AS nombre FROM ra_registro_instrumento_archivistico WHERE id_instrumento IN (" & Instrumentos & ")"
            Case "AREA"
                consulta = "SELECT a.Codigo_Area AS id,a.Nombre_Area AS nombre FROM areas_depart_radicacion a INNER JOIN ra_registro_instrumento_archivistico i ON i.registro_organigrama_ID_ORGANIGRAMA=a.Registro_Organigrama_Id_Organigrama WHERE a.Estado_Area=1 AND i.id_instrumento=@instrumento AND i.id_instrumento IN (" & Instrumentos & ")"
            Case "SERIE"
                consulta = "SELECT s.Id_Series AS id,s.Nombre_Serie AS nombre FROM series_documentales s INNER JOIN areas_depart_radicacion a ON a.Codigo_Area=s.Areas_Depart_Radicacion_Codigo_Area INNER JOIN ra_registro_instrumento_archivistico i ON i.id_instrumento=s.Ra_registro_instrumento_archivistico_id_instrumento AND i.registro_organigrama_ID_ORGANIGRAMA=a.Registro_Organigrama_Id_Organigrama WHERE s.Estado_Serie=1 AND a.Estado_Area=1 AND a.Codigo_Area=@area AND i.id_instrumento=@instrumento AND i.id_instrumento IN (" & Instrumentos & ")"
            Case "SUBSERIE"
                consulta = "SELECT ss.Id_SubSeries AS id,ss.Nombre_Subserie AS nombre FROM subseries_documentales ss INNER JOIN series_documentales s ON s.Id_Series=ss.Series_Documentales_Id_Series INNER JOIN areas_depart_radicacion a ON a.Codigo_Area=s.Areas_Depart_Radicacion_Codigo_Area WHERE ss.Estado_SubSerie=1 AND s.Estado_Serie=1 AND s.Id_Series=@serie AND a.Codigo_Area=@area AND s.Ra_registro_instrumento_archivistico_id_instrumento=@instrumento AND ss.Ra_registro_instrumento_archivistico_id_instrumento=@instrumento AND a.Registro_Organigrama_Id_Organigrama IN (" & Organigramas & ")"
            Case "TIPO_EXPEDIENTE"
                consulta = "SELECT ID_TIPO_EXPEDIENTE AS id,NOMBRE_TIPO_EXPEDIENTE AS nombre FROM ra_tipo_expediente WHERE ESTADO_TIPO=1"
            Case "UNIDAD_DOCUMENTAL"
                consulta = "SELECT ID_TIPO_UNIDAD_DOCUMENTAL AS id,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS nombre FROM tipos_unidad_documental"
            Case "RESPONSABLE", "RESPONSABLE_IDENTIFICACION"
                consulta = "SELECT id_Remit_Dest_Int AS id,Nombre_Remitente AS nombre,IDENTIFICACION FROM remit_dest_interno WHERE Estado_Usuario=1 AND Empresa_Gestion_Documental_id_empresa=@empresa"
            Case "TIPOLOGIA", "TIPOLOGIA_CODIGO"
                consulta = "SELECT t.Id_Tipo_Doc_Series AS id,t.Descripcion_Documento AS nombre,t.codigo_documento FROM tipo_doc_series t WHERE t.Estado_Tipo=1 AND t.id_instrumento=@instrumento AND t.id_instrumento IN (" & Instrumentos & ") AND ((t.Series_Documentales_Id_Series=@serie AND ((@subserie IS NULL AND (t.sub_serie_id_serie IS NULL OR t.sub_serie_id_serie=0)) OR t.sub_serie_id_serie=@subserie)) OR t.tipo_doc_trasversal=1)"
            Case "GABINETE"
                origen = _da
                parametros.AddRange({"@usuarioDA", UsuarioDA(contexto)})
                consulta = "SELECT s.id_gabinete AS id,s.NOMBRE AS nombre FROM system1 s WHERE " & PermisoGabinete
            Case "INDICE"
                Dim campos = Me.Campos(contexto, c)
                Return campos.Where(Function(f) referencia Is Nothing OrElse f.value = referencia).Skip((pagina - 1) * tamano).Take(tamano).ToList()
            Case Else
                Throw Invalida()
        End Select
        If codigoClasificacion Then consulta = consulta.Replace(If(catalogo = "SERIE", "s.Nombre_Serie AS nombre", "ss.Nombre_Subserie AS nombre"), If(catalogo = "SERIE", "s.Codigo_Arbitrario AS nombre", "ss.Codigo_Arbitrario AS nombre"))
        consulta = "SELECT id,nombre FROM (" & consulta & ") opciones WHERE (@ref IS NULL OR " & If(catalogo = "TIPOLOGIA_CODIGO", "codigo_documento", If(catalogo = "RESPONSABLE_IDENTIFICACION", "IDENTIFICACION", "id")) & "=@ref) ORDER BY nombre,id LIMIT @limite OFFSET @offset"
        parametros.AddRange({"@ref", referencia, "@limite", tamano, "@offset", (pagina - 1) * tamano})
        Dim datos = Leer(origen, consulta, parametros.ToArray())
        Return datos.AsEnumerable().Select(Function(r) New OpcionPerfilImportacion With {.value = Convert.ToString(r("id"), CultureInfo.InvariantCulture), .text = Convert.ToString(r("nombre"), CultureInfo.InvariantCulture)}).ToList()
    End Function

    Public Function Campos(contexto As ContextoPerfilImportacion, c As ConfiguracionPerfilImportacion) As List(Of OpcionPerfilImportacion)
        If Not c.GabineteId.HasValue Then Throw Invalida()
        Dim usuario = UsuarioDA(contexto)
        Dim datos = Leer(_da, "SELECT d.id_detalle_gabinete AS id,d.CAMPO,d.ALEAS_CAMPO,d.IS_REQUIRED_BACKEND,ic.DATA_TYPE,ic.COLUMN_TYPE,ic.NUMERIC_PRECISION,ic.NUMERIC_SCALE,ic.CHARACTER_SET_NAME,ic.CHARACTER_MAXIMUM_LENGTH FROM system1 s INNER JOIN detalle_gabienete d ON d.id_gabinete=s.id_gabinete AND d.GABINETE=s.NOMBRE INNER JOIN information_schema.COLUMNS ic ON ic.TABLE_SCHEMA=DATABASE() AND ic.TABLE_NAME=s.NOMBRE AND ic.COLUMN_NAME=d.CAMPO WHERE s.id_gabinete=@gabinete AND d.VISIBLE=1 AND " & PermisoGabinete & " ORDER BY d.IDENTI LIMIT 129", "@gabinete", c.GabineteId.Value, "@usuarioDA", usuario)
        If datos.Rows.Count > 128 Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "El gabinete excede el máximo de campos admitidos por esta entrega.")
        Dim resultado As New List(Of OpcionPerfilImportacion)()
        For Each fila As DataRow In datos.Rows
            Dim tipo = Convert.ToString(fila("DATA_TYPE"), CultureInfo.InvariantCulture)
            Dim contrato = If({"char", "varchar", "text", "tinytext", "mediumtext", "longtext"}.Contains(tipo), "TEXTO", If({"tinyint", "smallint", "mediumint", "int", "bigint"}.Contains(tipo), "ENTERO", If({"decimal", "numeric"}.Contains(tipo), "DECIMAL", If(tipo = "date", "FECHA", "NO_SOPORTADO"))))
            Dim longitud = If(Convert.IsDBNull(fila("CHARACTER_MAXIMUM_LENGTH")), 64, CInt(Math.Min(4096L, Convert.ToInt64(fila("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture))))
            'El catálogo legacy usa 0 como ausencia de alias, además de NULL/vacío.
            Dim aliasCampo = Convert.ToString(fila("ALEAS_CAMPO"), CultureInfo.InvariantCulture).Trim()
            Dim etiqueta = If(aliasCampo.Length = 0 OrElse aliasCampo = "0", CStr(fila("CAMPO")), aliasCampo)
            resultado.Add(New OpcionPerfilImportacion With {.value = Convert.ToString(fila("id"), CultureInfo.InvariantCulture), .text = etiqueta, .Tipo = contrato, .Longitud = longitud, .Requerida = Convert.ToString(fila("IS_REQUIRED_BACKEND"), CultureInfo.InvariantCulture) = "1", .TipoFisico = CStr(fila("COLUMN_TYPE")), .Precision = If(Convert.IsDBNull(fila("NUMERIC_PRECISION")), 0, Convert.ToInt32(fila("NUMERIC_PRECISION"), CultureInfo.InvariantCulture)), .Escala = If(Convert.IsDBNull(fila("NUMERIC_SCALE")), 0, Convert.ToInt32(fila("NUMERIC_SCALE"), CultureInfo.InvariantCulture)), .Charset = Convert.ToString(fila("CHARACTER_SET_NAME"), CultureInfo.InvariantCulture)})
        Next
        Return resultado
    End Function

    Private Function UsuarioDA(contexto As ContextoPerfilImportacion) As UInteger
        Dim relacion = Leer(_ra, "SELECT RELACION_DA FROM remit_dest_interno WHERE id_Remit_Dest_Int=@usuario AND Empresa_Gestion_Documental_id_empresa=@empresa AND Estado_Usuario=1", "@usuario", contexto.UsuarioRef, "@empresa", contexto.AmbitoRef)
        If relacion.Rows.Count <> 1 OrElse Convert.IsDBNull(relacion.Rows(0)(0)) Then Throw Denegado()
        Dim id As UInteger
        If Not UInteger.TryParse(Convert.ToString(relacion.Rows(0)(0), CultureInfo.InvariantCulture), id) OrElse id = 0 Then Throw Denegado()
        Dim usuario = Leer(_da, "SELECT Clave_Usuario FROM usuarios_da WHERE Clave_Usuario=@id AND Relacion_Gestion=@usuario", "@id", id, "@usuario", contexto.UsuarioRef)
        If usuario.Rows.Count <> 1 Then Throw Denegado()
        Return id
    End Function

    Private Function HuellaOrigen() As String
        Dim origen = Leer(_da, "SELECT @@hostname AS host,@@port AS puerto,@@server_id AS servidor,DATABASE() AS base")
        Dim partes = origen.Rows(0).ItemArray.Select(Function(v) Convert.ToString(v, CultureInfo.InvariantCulture)).ToArray()
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(partes)))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Private Shared Function TipoCompatible(campo As OpcionPerfilImportacion, regla As ReglaPerfilImportacion) As Boolean
        Return campo.Tipo <> "NO_SOPORTADO" AndAlso (campo.Tipo = regla.TipoValor OrElse (campo.Tipo = "TEXTO" AndAlso regla.TipoValor = "REFERENCIA")) AndAlso regla.LongitudMaxima <= campo.Longitud
    End Function
    Public Shared Function ValorCompatible(campo As OpcionPerfilImportacion, valor As String) As Boolean
        If String.IsNullOrEmpty(valor) Then Return Not campo.Requerida
        If valor.Length > campo.Longitud Then Return False
        If campo.Charset = "ascii" AndAlso valor.Any(Function(ch) AscW(ch) > 127 OrElse AscW(ch) < 0) Then Return False
        If campo.Charset = "latin1" Then
            Try
                Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(valor)
            Catch ex As EncoderFallbackException
                Return False
            End Try
        End If
        If campo.Tipo = "ENTERO" Then
            Dim numero As Decimal
            If Not Decimal.TryParse(valor, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, numero) Then Return False
            Dim bits = If(campo.TipoFisico.StartsWith("tinyint", StringComparison.Ordinal), 8, If(campo.TipoFisico.StartsWith("smallint", StringComparison.Ordinal), 16, If(campo.TipoFisico.StartsWith("mediumint", StringComparison.Ordinal), 24, If(campo.TipoFisico.StartsWith("bigint", StringComparison.Ordinal), 64, 32))))
            Dim sinSigno = campo.TipoFisico.Contains("unsigned")
            Dim potencia As Decimal = 1D
            For i = 1 To If(sinSigno, bits, bits - 1)
                potencia *= 2D
            Next
            Return numero >= If(sinSigno, 0D, -potencia) AndAlso numero <= potencia - 1D
        End If
        If campo.Tipo = "DECIMAL" Then
            Dim sinSigno = If(valor.StartsWith("-", StringComparison.Ordinal) OrElse valor.StartsWith("+", StringComparison.Ordinal), valor.Substring(1), valor)
            If sinSigno.Length = 0 OrElse sinSigno.Any(Function(ch) (ch < "0"c OrElse ch > "9"c) AndAlso ch <> "."c) Then Return False
            Dim partes = sinSigno.Split("."c)
            If partes.Any(Function(p) p.Length = 0) Then Return False
            Dim enteros = partes(0).TrimStart("0"c).Length
            Dim decimales = If(partes.Length = 2, partes(1).Length, 0)
            Return partes.Length <= 2 AndAlso enteros <= campo.Precision - campo.Escala AndAlso decimales <= campo.Escala AndAlso Not (campo.TipoFisico.Contains("unsigned") AndAlso valor.StartsWith("-", StringComparison.Ordinal))
        End If
        Return campo.Tipo <> "NO_SOPORTADO"
    End Function
    Private Function Leer(cadena As String, sql As String, ParamArray parametros As Object()) As DataTable
        If _consulta IsNot Nothing Then Return _consulta(cadena, sql, parametros)
        If String.IsNullOrWhiteSpace(cadena) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El origen documental no está disponible.")
        Try
            Using cn As New MySqlConnection(cadena), cmd As New MySqlCommand(sql, cn)
                cn.Open()
                cmd.CommandTimeout = 30
                For i = 0 To parametros.Length - 1 Step 2
                    cmd.Parameters.AddWithValue(CStr(parametros(i)), If(parametros(i + 1), DBNull.Value))
                Next
                Using reader = cmd.ExecuteReader()
                    Dim tabla As New DataTable()
                    tabla.Load(reader)
                    Return tabla
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo consultar el catálogo documental.")
        End Try
    End Function
    Private Shared Sub Agregar(errores As List(Of IncidenciaPerfilImportacion), campo As String, codigo As String)
        errores.Add(New IncidenciaPerfilImportacion(campo, codigo, "Revise la referencia y sus permisos vigentes."))
    End Sub
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "No se encontró una relación autorizada con Docuarchi.")
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "Solicitud de catálogo inválida.")
    End Function
End Class
