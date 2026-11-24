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
        If Not TablaValida(p.NOMBRE_PLANTILLA) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "NOMBRE_PLANTILLA", "NOMBRE_TABLA_INVALIDO", limite:=64)
        If Not IdentificadorValido(p.CAMPO_UNICO) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_UNICO", "CLAVE_NO_MAPEADA")
        If String.IsNullOrWhiteSpace(p.LIBRO_EXCEL) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "LIBRO_EXCEL", "REQUERIDO")
        If p.LIBRO_EXCEL.Length > 120 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "LIBRO_EXCEL", "LONGITUD_EXCEDIDA", limite:=120, longitud:=p.LIBRO_EXCEL.Length)
        If p.LIBRO_EXCEL.Any(Function(c) Char.IsControl(c)) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "LIBRO_EXCEL", "CARACTERES_CONTROL")
        If p.ESTADO_PLANTILLA > 1 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "ESTADO_PLANTILLA", "OPCION_INVALIDA")
        Select Case p.Formato
            Case "XLS" : p.CONEXION_FUENTE = "8.0 xls"
            Case "XLSX" : p.CONEXION_FUENTE = "12.0 xlsx"
            Case Else : Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "Formato", "FORMATO_NO_ADMITIDO")
        End Select
        p.ELIMINA_TABLA = 0
        Select Case p.Modo
            Case "CREAR", "AGREGAR" : p.ELIMINA_INSERTA_RECORD = 0 : p.INSERTA_RECORD = 1
            Case "REEMPLAZAR" : p.ELIMINA_INSERTA_RECORD = 1 : p.INSERTA_RECORD = 0
            Case Else : Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "Modo", "MODO_NO_ADMITIDO")
        End Select
        If p.Campos Is Nothing OrElse p.Campos.Count = 0 OrElse p.Campos.Count > 256 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "Campos", "CANTIDAD_CAMPOS", limite:=256)
        Dim nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For i = 0 To p.Campos.Count - 1
            Dim campo = p.Campos(i), fila = i + 1
            If campo Is Nothing Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "Campos", "REQUERIDO", fila)
            If Not IdentificadorValido(campo.CAMPO_DESTINO) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_DESTINO", "IDENTIFICADOR_INVALIDO", fila)
            If Not nombres.Add(campo.CAMPO_DESTINO) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_DESTINO", "DESTINO_DUPLICADO", fila, campo.CAMPO_DESTINO)
            If String.IsNullOrWhiteSpace(campo.CAMPO_FUENTE) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_FUENTE", "REQUERIDO", fila, campo.CAMPO_DESTINO)
            If campo.CAMPO_FUENTE.Length > 120 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_FUENTE", "LONGITUD_EXCEDIDA", fila, campo.CAMPO_DESTINO, 120, campo.CAMPO_FUENTE.Length)
            If campo.CAMPO_FUENTE.Any(Function(c) Char.IsControl(c)) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_FUENTE", "CARACTERES_CONTROL", fila, campo.CAMPO_DESTINO)
            If String.IsNullOrWhiteSpace(campo.TIPO_CAMPO) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "TIPO_CAMPO", "REQUERIDO", fila, campo.CAMPO_DESTINO)
            If campo.TIPO_CAMPO.Length > 64 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "TIPO_CAMPO", "LONGITUD_EXCEDIDA", fila, campo.CAMPO_DESTINO, 64, campo.TIPO_CAMPO.Length)
            If If(campo.CARACTER_SPLITER, "").Length > 120 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CARACTER_SPLITER", "LONGITUD_EXCEDIDA", fila, campo.CAMPO_DESTINO, 120, campo.CARACTER_SPLITER.Length)
            For Each opcion In New String() {"REMPLAZA_COMA", "REMPLAZA_PUNTO", "OPCION_SPLITER"}
                Dim valor = If(opcion = "REMPLAZA_COMA", campo.REMPLAZA_COMA, If(opcion = "REMPLAZA_PUNTO", campo.REMPLAZA_PUNTO, campo.OPCION_SPLITER))
                If valor.HasValue AndAlso valor.Value > 1 Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", opcion, "OPCION_INVALIDA", fila, campo.CAMPO_DESTINO)
            Next
            If campo.OPCION_SPLITER.GetValueOrDefault() = 1 Then
                If String.IsNullOrEmpty(campo.CARACTER_SPLITER) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CARACTER_SPLITER", "SEPARADOR_REQUERIDO", fila, campo.CAMPO_DESTINO)
                If Not campo.NUMERO_MATRIZ_EXPLITER.HasValue Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "NUMERO_MATRIZ_EXPLITER", "POSICION_REQUERIDA", fila, campo.CAMPO_DESTINO)
            End If
        Next
        If Not p.Campos.Any(Function(c) String.Equals(c.CAMPO_DESTINO, p.CAMPO_UNICO, StringComparison.Ordinal)) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "CAMPO_UNICO", "CLAVE_NO_MAPEADA")
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
            If p.Modo <> "CREAR" Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "NOMBRE_PLANTILLA", "TABLA_NO_EXISTE")
            For i = 0 To p.Campos.Count - 1
                Dim c = p.Campos(i)
                If Not TipoCreable(c.TIPO_CAMPO) Then Throw ErrorConfiguracion("CONFIGURACION_INVALIDA", "TIPO_CAMPO", "TIPO_NO_ADMITIDO", i + 1, c.CAMPO_DESTINO)
            Next
            Return
        End If
        If p.Modo = "CREAR" Then Throw ErrorConfiguracion("DESTINO_YA_EXISTE", "NOMBRE_PLANTILLA", "TABLA_YA_EXISTE")
        If Not String.Equals(destino.Motor, "InnoDB", StringComparison.OrdinalIgnoreCase) Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "NOMBRE_PLANTILLA", "MOTOR_NO_ADMITIDO")
        For i = 0 To p.Campos.Count - 1
            Dim c = p.Campos(i)
            Dim columna = destino.Columnas.SingleOrDefault(Function(x) String.Equals(x.Nombre, c.CAMPO_DESTINO, StringComparison.Ordinal))
            If columna Is Nothing Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "CAMPO_DESTINO", "COLUMNA_NO_EXISTE", i + 1, c.CAMPO_DESTINO)
            If Not String.IsNullOrEmpty(columna.Extra) Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "CAMPO_DESTINO", "COLUMNA_AUTOMATICA", i + 1, c.CAMPO_DESTINO)
            If Not TipoCompatible(c.TIPO_CAMPO, columna) Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "TIPO_CAMPO", "TIPO_DESTINO_INCOMPATIBLE", i + 1, c.CAMPO_DESTINO, tipoEsperado:=columna.TipoCompleto)
        Next
        For Each columna In destino.Columnas
            If Not columna.Nulable AndAlso columna.Predeterminado Is Nothing AndAlso String.IsNullOrEmpty(columna.Extra) AndAlso Not p.Campos.Any(Function(c) c.CAMPO_DESTINO = columna.Nombre) Then Throw ErrorConfiguracion("DESTINO_INVALIDO", "Campos", "COLUMNA_OBLIGATORIA_NO_MAPEADA", destino:=columna.Nombre)
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
    Public Shared Function ErrorConfiguracion(codigo As String, campo As String, regla As String, Optional fila As Nullable(Of Integer) = Nothing, Optional destino As String = Nothing, Optional limite As Nullable(Of Integer) = Nothing, Optional longitud As Nullable(Of Integer) = Nothing, Optional tipoEsperado As String = Nothing) As ImportacionExcelException
        Dim salida = ErrorControlado(codigo)
        If Not IdentificadorValido(destino) Then destino = Nothing
        If tipoEsperado IsNot Nothing AndAlso Not Regex.IsMatch(tipoEsperado, "\A[A-Za-z0-9(), ]{1,64}\z") Then tipoEsperado = Nothing
        salida.Detalles.Add(New DetalleConfiguracionExcel With {.Campo = campo, .Regla = regla, .FilaCampo = fila, .CampoDestino = destino, .Limite = limite, .LongitudActual = longitud, .TipoEsperado = tipoEsperado})
        Return salida
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
