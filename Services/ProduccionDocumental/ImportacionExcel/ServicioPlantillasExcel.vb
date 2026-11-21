Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ServicioPlantillasExcel
    Private ReadOnly _repo As IPlantillaExcelRepository
    Private ReadOnly _conexionId As UInteger
    Public Sub New(repo As IPlantillaExcelRepository, conexionId As UInteger)
        If repo Is Nothing OrElse conexionId = 0 Then Throw ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        _repo = repo
        _conexionId = conexionId
    End Sub
    Public Function Listar(ctx As ContextoImportacionExcel, solicitud As SolicitudListaPlantillasExcel) As PaginaPlantillasExcel
        Autorizar(ctx)
        If solicitud Is Nothing OrElse solicitud.ConexionId <> _conexionId OrElse solicitud.Pagina < 1 OrElse solicitud.Pagina > 1000000 OrElse solicitud.TamanoPagina < 1 OrElse solicitud.TamanoPagina > 100 OrElse If(solicitud.Termino, "").Length > 120 Then Throw ErrorControlado("SOLICITUD_INVALIDA")
        Return _repo.Listar(If(solicitud.Termino, ""), solicitud.Pagina, solicitud.TamanoPagina)
    End Function
    Public Function Obtener(ctx As ContextoImportacionExcel, id As UInteger) As PlantillaExcel
        Autorizar(ctx)
        If id = 0 Then Throw ErrorControlado("SOLICITUD_INVALIDA")
        Dim p = _repo.Obtener(id)
        If p Is Nothing Then Throw ErrorControlado("NO_ENCONTRADO")
        If p.IMP01_XMLCONEXION_ID_XMLCONEXION <> _conexionId OrElse Not TablaValida(p.NOMBRE_PLANTILLA) Then Throw ErrorControlado("DESTINO_INVALIDO")
        p.Modo = ModoHistorico(p)
        Dim destino = _repo.Destino(p.NOMBRE_PLANTILLA)
        If Not destino.Existe Then
            If p.Modo <> "AGREGAR" Then Throw ErrorControlado("DESTINO_INVALIDO")
            p.Modo = "CREAR"
        End If
        p.Formato = FormatoHistorico(p.CONEXION_FUENTE)
        p.Huella = Huella(p)
        Return p
    End Function
    Public Function Guardar(ctx As ContextoImportacionExcel, s As SolicitudGuardarPlantillaExcel) As PlantillaExcel
        Autorizar(ctx)
        If s Is Nothing OrElse s.Plantilla Is Nothing OrElse s.ConexionId <> _conexionId Then Throw ErrorControlado("SOLICITUD_INVALIDA")
        Dim p = s.Plantilla
        If p.IMP01_XMLCONEXION_ID_XMLCONEXION <> _conexionId Then Throw ErrorControlado("ACCESO_DENEGADO")
        If p.ID_PLANTILLAIMP = 0 Then
            If s.RevisionEsperada <> 0 OrElse Not String.IsNullOrEmpty(s.HuellaEsperada) Then Throw ErrorControlado("SOLICITUD_INVALIDA")
            p.RUTA_ARCHIVO_FUENTE = "web/" & Guid.NewGuid().ToString("N") & If(p.Formato = "XLS", ".xls", ".xlsx")
        Else
            If s.RevisionEsperada = 0 OrElse s.RevisionEsperada = UInteger.MaxValue OrElse Not Regex.IsMatch(If(s.HuellaEsperada, ""), "\A[0-9a-f]{64}\z") Then Throw ErrorControlado("CONFLICTO_REVISION")
            'Nunca aceptar la ruta que un navegador presenta como autoridad.
            p.RUTA_ARCHIVO_FUENTE = Nothing
        End If
        If Not TablaValida(p.NOMBRE_PLANTILLA) OrElse Not IdentificadorValido(p.CAMPO_UNICO) OrElse String.IsNullOrWhiteSpace(p.LIBRO_EXCEL) OrElse p.LIBRO_EXCEL.Length > 120 OrElse p.LIBRO_EXCEL.Any(Function(c) Char.IsControl(c)) OrElse p.ESTADO_PLANTILLA > 1 Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
        Select Case p.Formato
            Case "XLS" : p.CONEXION_FUENTE = "8.0 xls"
            Case "XLSX" : p.CONEXION_FUENTE = "12.0 xlsx"
            Case Else : Throw ErrorControlado("CONFIGURACION_INVALIDA")
        End Select
        p.ELIMINA_TABLA = 0
        Select Case p.Modo
            Case "CREAR", "AGREGAR" : p.ELIMINA_INSERTA_RECORD = 0 : p.INSERTA_RECORD = 1
            Case "REEMPLAZAR" : p.ELIMINA_INSERTA_RECORD = 1 : p.INSERTA_RECORD = 0
            Case Else : Throw ErrorControlado("CONFIGURACION_INVALIDA")
        End Select
        If p.Campos Is Nothing OrElse p.Campos.Count = 0 OrElse p.Campos.Count > 256 Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
        Dim nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each campo In p.Campos
            If campo Is Nothing OrElse Not IdentificadorValido(campo.CAMPO_DESTINO) OrElse Not nombres.Add(campo.CAMPO_DESTINO) OrElse String.IsNullOrWhiteSpace(campo.CAMPO_FUENTE) OrElse campo.CAMPO_FUENTE.Length > 120 OrElse campo.CAMPO_FUENTE.Any(Function(c) Char.IsControl(c)) OrElse String.IsNullOrWhiteSpace(campo.TIPO_CAMPO) OrElse campo.TIPO_CAMPO.Length > 64 OrElse If(campo.CARACTER_SPLITER, "").Length > 120 Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
            If (campo.REMPLAZA_COMA.HasValue AndAlso campo.REMPLAZA_COMA.Value > 1) OrElse (campo.REMPLAZA_PUNTO.HasValue AndAlso campo.REMPLAZA_PUNTO.Value > 1) OrElse (campo.OPCION_SPLITER.HasValue AndAlso campo.OPCION_SPLITER.Value > 1) Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
            If campo.OPCION_SPLITER.GetValueOrDefault() = 1 AndAlso (String.IsNullOrEmpty(campo.CARACTER_SPLITER) OrElse Not campo.NUMERO_MATRIZ_EXPLITER.HasValue) Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
        Next
        If Not p.Campos.Any(Function(c) String.Equals(c.CAMPO_DESTINO, p.CAMPO_UNICO, StringComparison.Ordinal)) Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
        ValidarDestino(p, _repo.Destino(p.NOMBRE_PLANTILLA))
        Dim guardada = _repo.Guardar(p, s.RevisionEsperada, s.HuellaEsperada)
        guardada.Modo = p.Modo
        guardada.Formato = p.Formato
        guardada.Huella = Huella(guardada)
        Return guardada
    End Function
    Public Shared Sub ValidarDestino(p As PlantillaExcel, destino As DestinoPlantillaExcel)
        If destino Is Nothing Then Throw ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        If Not destino.Existe Then
            If p.Modo <> "CREAR" Then Throw ErrorControlado("DESTINO_INVALIDO")
            For Each c In p.Campos
                If Not TipoCreable(c.TIPO_CAMPO) Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
            Next
            Return
        End If
        If p.Modo = "CREAR" Then Throw ErrorControlado("DESTINO_YA_EXISTE")
        If Not String.Equals(destino.Motor, "InnoDB", StringComparison.OrdinalIgnoreCase) Then Throw ErrorControlado("DESTINO_INVALIDO")
        For Each c In p.Campos
            Dim columna = destino.Columnas.SingleOrDefault(Function(x) String.Equals(x.Nombre, c.CAMPO_DESTINO, StringComparison.Ordinal))
            If columna Is Nothing OrElse Not String.IsNullOrEmpty(columna.Extra) OrElse Not TipoCompatible(c.TIPO_CAMPO, columna) Then Throw ErrorControlado("DESTINO_INVALIDO")
        Next
        For Each columna In destino.Columnas
            If Not columna.Nulable AndAlso columna.Predeterminado Is Nothing AndAlso String.IsNullOrEmpty(columna.Extra) AndAlso Not p.Campos.Any(Function(c) c.CAMPO_DESTINO = columna.Nombre) Then Throw ErrorControlado("DESTINO_INVALIDO")
        Next
    End Sub
    Private Shared Function TipoCompatible(tipo As String, columna As ColumnaDestinoExcel) As Boolean
        'El tipo histórico puede ser VARCHAR sin longitud. La longitud física actual manda.
        Return String.Equals(tipo, columna.Tipo, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(tipo, columna.TipoCompleto, StringComparison.OrdinalIgnoreCase)
    End Function
    Public Shared Function TipoCreable(tipo As String) As Boolean
        If tipo Is Nothing Then Return False
        If Regex.IsMatch(tipo, "\A(?:TINYINT|SMALLINT|MEDIUMINT|INT|BIGINT)(?: UNSIGNED)?\z") OrElse tipo = "DATE" Then Return True
        Dim texto = Regex.Match(tipo, "\A(CHAR|VARCHAR)\(([1-9][0-9]{0,3})\)\z")
        If texto.Success Then Return Integer.Parse(texto.Groups(2).Value, CultureInfo.InvariantCulture) <= If(texto.Groups(1).Value = "CHAR", 255, 4096)
        Dim numero = Regex.Match(tipo, "\A(?:DECIMAL|NUMERIC)\(([1-9][0-9]?),([0-9]{1,2})\)\z")
        If Not numero.Success Then Return False
        Dim precision = Integer.Parse(numero.Groups(1).Value, CultureInfo.InvariantCulture)
        Dim escala = Integer.Parse(numero.Groups(2).Value, CultureInfo.InvariantCulture)
        Return precision <= 65 AndAlso escala <= 30 AndAlso escala <= precision
    End Function
    Public Shared Function IdentificadorValido(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z", RegexOptions.CultureInvariant)
    End Function
    Public Shared Function TablaValida(valor As String) As Boolean
        Return IdentificadorValido(valor) AndAlso valor.StartsWith("imp_02_", StringComparison.Ordinal) AndAlso valor.Length > 7
    End Function
    Public Shared Sub Autorizar(ctx As ContextoImportacionExcel)
        If ctx Is Nothing OrElse Not ctx.Autorizado OrElse ctx.UsuarioId = 0 OrElse ctx.EmpresaId = 0 Then Throw ErrorControlado("ACCESO_DENEGADO")
    End Sub
    Public Shared Function ModoHistorico(p As PlantillaExcel) As String
        If p.ELIMINA_TABLA = 0 AndAlso p.ELIMINA_INSERTA_RECORD = 0 AndAlso p.INSERTA_RECORD = 1 Then Return "AGREGAR"
        If p.ELIMINA_TABLA = 0 AndAlso p.ELIMINA_INSERTA_RECORD = 1 AndAlso p.INSERTA_RECORD = 0 Then Return "REEMPLAZAR"
        Throw ErrorControlado("CONFIGURACION_INVALIDA")
    End Function
    Public Shared Function FormatoHistorico(valor As String) As String
        Select Case valor
            Case "8.0 xls" : Return "XLS"
            Case "12.0 xlsx" : Return "XLSX"
            Case Else : Throw ErrorControlado("CONFIGURACION_INVALIDA")
        End Select
    End Function
    Public Shared Function Huella(p As PlantillaExcel) As String
        If p Is Nothing OrElse p.Campos Is Nothing Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
        Dim serializer As New JsonSerializer With {.Culture = CultureInfo.InvariantCulture}
        Dim cabecera = JObject.FromObject(p, serializer)
        For Each nombre In {"Campos", "Modo", "Formato", "Huella"}
            cabecera.Remove(nombre)
        Next
        Dim destinos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim campos As New JArray()
        For Each c In p.Campos.OrderBy(Function(x) x.CAMPO_DESTINO, StringComparer.Ordinal).ThenBy(Function(x) x.CAMPO_FUENTE, StringComparer.Ordinal)
            If Not destinos.Add(c.CAMPO_DESTINO) Then Throw ErrorControlado("CONFIGURACION_INVALIDA")
            Dim item = JObject.FromObject(c, serializer)
            'La identidad de fila también detecta reemplazos por herramientas históricas.
            item.Add("IMP01_PLANTILLAIMP_ID_PLANTILLAIMP", p.ID_PLANTILLAIMP)
            campos.Add(item)
        Next
        cabecera.Add("campos", campos)
        cabecera.Add("versionSnapshot", 1)
        Dim json As String
        Using writer As New IO.StringWriter(CultureInfo.InvariantCulture)
            Using jw As New JsonTextWriter(writer)
                jw.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                Canonico(cabecera).WriteTo(jw)
                jw.Flush()
                json = writer.ToString()
            End Using
        End Using
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(New UTF8Encoding(False, True).GetBytes(json))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Private Shared Function Canonico(token As JToken) As JToken
        If TypeOf token Is JObject Then
            Dim result As New JObject()
            For Each prop In DirectCast(token, JObject).Properties().OrderBy(Function(x) x.Name, StringComparer.Ordinal)
                result.Add(prop.Name, Canonico(prop.Value))
            Next
            Return result
        End If
        If TypeOf token Is JArray Then Return New JArray(DirectCast(token, JArray).Select(Function(x) Canonico(x)))
        Return token.DeepClone()
    End Function
    Public Shared Function ErrorControlado(codigo As String) As ImportacionExcelException
        Dim mensaje As String
        Select Case codigo
            Case "ACCESO_DENEGADO" : mensaje = "No tiene acceso a esta operación."
            Case "SESION_REQUERIDA" : mensaje = "Se requiere una sesión de Gestión vigente."
            Case "SOLICITUD_NO_VERIFICADA" : mensaje = "Recargue el contexto antes de continuar."
            Case "CONFLICTO_REVISION" : mensaje = "La plantilla cambió. Consúltela de nuevo antes de guardar."
            Case "NO_ENCONTRADO" : mensaje = "No se encontró la plantilla en el ámbito autorizado."
            Case "CAPACIDAD_NO_DISPONIBLE" : mensaje = "La configuración de importación no está disponible."
            Case "GUARDADO_NO_COMPLETADO" : mensaje = "No se confirmó el guardado. Consulte la plantilla antes de intentarlo de nuevo."
            Case "DESTINO_INVALIDO" : mensaje = "La tabla o sus columnas no corresponden al destino autorizado."
            Case "DESTINO_YA_EXISTE" : mensaje = "La tabla ya existe. Seleccione agregar o reemplazar."
            Case Else : mensaje = "La solicitud o configuración de la plantilla no es válida."
        End Select
        Return New ImportacionExcelException(codigo, mensaje)
    End Function
End Class
