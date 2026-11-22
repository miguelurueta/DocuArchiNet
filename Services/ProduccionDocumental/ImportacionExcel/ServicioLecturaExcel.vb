Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Numerics
Imports System.Text
Imports System.Text.RegularExpressions
Imports ExcelNumberFormat

Public NotInheritable Class ServicioLecturaExcel
    Private Shared ReadOnly Inv As CultureInfo = CultureInfo.InvariantCulture
    Private NotInheritable Class Mapeo
        Friend Campo As CampoPlantillaExcel
        Friend Columna As ColumnaDestinoExcel
        Friend Tipo As String
        Friend Posicion As Integer
    End Class
    Public Function Leer(stream As Stream, solicitud As SolicitudLecturaExcel) As ResultadoLecturaExcel
        Dim salida As New ResultadoLecturaExcel With {.Hoja = If(solicitud Is Nothing, Nothing, solicitud.Hoja)}
        Try
            Dim mapeos = Configurar(solicitud)
            Dim encabezadoLeido = False
            Dim lector As New LectorArchivoExcel()
            lector.Recorrer(stream, solicitud, salida,
                Sub(fila)
                    If fila.Fila = 1 Then
                        encabezadoLeido = True
                        For Each celda In fila.Celdas
                            If celda.ErrorCodigo IsNot Nothing Then
                                Agregar(salida, celda.ErrorCodigo, 1, celda.Columna)
                            End If
                            salida.Encabezados.Add(New EncabezadoLecturaExcel With {.Columna = celda.Columna, .Nombre = If(celda.Valor Is Nothing, "", Convert.ToString(celda.Valor, Inv))})
                        Next
                        For Each m In mapeos
                            Dim coincidencias = salida.Encabezados.Where(Function(h) h.Nombre = m.Campo.CAMPO_FUENTE).ToList()
                            If coincidencias.Count <> 1 Then
                                Agregar(salida, If(coincidencias.Count = 0, "ENCABEZADO_NO_ENCONTRADO", "ENCABEZADO_AMBIGUO"), 1, Nothing, m.Campo)
                            Else
                                m.Posicion = coincidencias(0).Columna
                            End If
                        Next
                        If salida.TotalIncidencias > 0 Then Throw Fallo("ENCABEZADOS_INVALIDOS")
                        Return
                    End If
                    If Not encabezadoLeido Then Throw Fallo("ENCABEZADOS_INVALIDOS")
                    If fila.Celdas.All(Function(c) c.ErrorCodigo Is Nothing AndAlso (c.Valor Is Nothing OrElse (TypeOf c.Valor Is String AndAlso CStr(c.Valor).Length = 0))) Then Return
                    salida.FilasDatosObservadas += 1
                    If salida.FilasDatosObservadas > solicitud.Limites.MaxFilasDatos Then
                        Agregar(salida, "LIMITE_FILAS_EXCEDIDO", fila.Fila)
                        Throw Fallo("LECTURA_INTERRUMPIDA")
                    End If
                    Dim normalizada As New FilaNormalizadaExcel With {.Fila = fila.Fila}
                    For Each m In mapeos
                        Dim celda = fila.Celdas(m.Posicion - 1)
                        If celda.ErrorCodigo IsNot Nothing Then
                            Agregar(salida, celda.ErrorCodigo, fila.Fila, m.Posicion, m.Campo)
                            If celda.ErrorCodigo = "FORMULA_SIN_RESULTADO" Then Throw Fallo("LECTURA_INTERRUMPIDA")
                            Continue For
                        End If
                        Try
                            normalizada.Valores.Add(Normalizar(celda, m, solicitud.Plantilla.CAMPO_UNICO))
                        Catch ex As ImportacionExcelException
                            Agregar(salida, ex.Codigo, fila.Fila, m.Posicion, m.Campo)
                        End Try
                    Next
                    'Errores en columnas no mapeadas tampoco son filas vacías válidas.
                    For Each celda In fila.Celdas.Where(Function(c) c.ErrorCodigo IsNot Nothing AndAlso Not mapeos.Any(Function(m) m.Posicion = c.Columna))
                        Agregar(salida, celda.ErrorCodigo, fila.Fila, celda.Columna)
                        If celda.ErrorCodigo = "FORMULA_SIN_RESULTADO" Then Throw Fallo("LECTURA_INTERRUMPIDA")
                    Next
                    salida.Filas.Add(normalizada)
                End Sub)
            salida.LecturaCompleta = True
            If Not encabezadoLeido OrElse salida.FilasDatosObservadas = 0 Then Agregar(salida, "LIBRO_SIN_DATOS")
        Catch ex As ImportacionExcelException
            If ex.Codigo <> "LECTURA_INTERRUMPIDA" AndAlso ex.Codigo <> "ENCABEZADOS_INVALIDOS" Then Agregar(salida, ex.Codigo)
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is System.Xml.XmlException OrElse TypeOf ex Is ArgumentException OrElse TypeOf ex Is OverflowException OrElse TypeOf ex Is InvalidOperationException OrElse TypeOf ex Is NotSupportedException OrElse TypeOf ex Is FormatException OrElse TypeOf ex Is IndexOutOfRangeException OrElse If(ex.GetType().Namespace, "").StartsWith("ExcelDataReader.Exceptions", StringComparison.Ordinal)
            Agregar(salida, "LIBRO_INVALIDO")
        End Try
        If Not salida.Valido Then salida.Filas.Clear() 'Ningún prefijo queda disponible para escritura.
        Return salida
    End Function
    Private Shared Function Configurar(s As SolicitudLecturaExcel) As List(Of Mapeo)
        If s Is Nothing OrElse s.Plantilla Is Nothing OrElse s.Destino Is Nothing OrElse String.IsNullOrWhiteSpace(s.Hoja) Then Throw Fallo("CONFIGURACION_INVALIDA")
        Dim p = s.Plantilla
        If p.Campos Is Nothing OrElse p.Campos.Count = 0 OrElse p.Campos.Count > 256 OrElse Not ServicioPlantillasExcel.IdentificadorValido(p.CAMPO_UNICO) Then Throw Fallo("CONFIGURACION_INVALIDA")
        If s.Destino.Columnas Is Nothing OrElse s.Destino.Columnas.Any(Function(x) x Is Nothing) OrElse p.Campos.Any(Function(x) x Is Nothing) Then Throw Fallo("CONFIGURACION_INVALIDA")
        ServicioPlantillasExcel.ValidarDestino(p, s.Destino)
        Dim destinos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase), salida As New List(Of Mapeo)
        For Each c In p.Campos
            If c Is Nothing OrElse Not ServicioPlantillasExcel.IdentificadorValido(c.CAMPO_DESTINO) OrElse Not destinos.Add(c.CAMPO_DESTINO) OrElse String.IsNullOrWhiteSpace(c.CAMPO_FUENTE) OrElse c.CAMPO_FUENTE.Length > 120 Then Throw Fallo("CONFIGURACION_INVALIDA")
            If {c.REMPLAZA_COMA, c.REMPLAZA_PUNTO, c.OPCION_SPLITER}.Any(Function(v) v.HasValue AndAlso v.Value > 1UI) Then Throw Fallo("TRANSFORMACION_INVALIDA")
            If c.OPCION_SPLITER.GetValueOrDefault() = 1 AndAlso (String.IsNullOrEmpty(c.CARACTER_SPLITER) OrElse Not c.NUMERO_MATRIZ_EXPLITER.HasValue) Then Throw Fallo("TRANSFORMACION_INVALIDA")
            Dim columnas = s.Destino.Columnas.Where(Function(x) String.Equals(x.Nombre, c.CAMPO_DESTINO, StringComparison.Ordinal)).ToList()
            If columnas.Count <> 1 Then Throw Fallo("DESTINO_INVALIDO")
            Dim col = columnas(0), tipo = Familia(col.Tipo)
            If tipo = "TEXTO" AndAlso (Not col.Longitud.HasValue OrElse col.Longitud.Value < 1 OrElse String.IsNullOrEmpty(col.Charset)) Then Throw Fallo("DESTINO_INVALIDO")
            If tipo = "DECIMAL" Then PrecisionEscala(col)
            salida.Add(New Mapeo With {.Campo = c, .Columna = col, .Tipo = tipo})
        Next
        If Not salida.Any(Function(m) String.Equals(m.Campo.CAMPO_DESTINO, p.CAMPO_UNICO, StringComparison.Ordinal)) Then Throw Fallo("CONFIGURACION_INVALIDA")
        Return salida
    End Function
    Private Shared Function Familia(tipo As String) As String
        Select Case If(tipo, "").ToLowerInvariant()
            Case "char", "varchar", "text", "tinytext", "mediumtext", "longtext" : Return "TEXTO"
            Case "tinyint", "smallint", "mediumint", "int", "integer", "bigint" : Return "ENTERO"
            Case "decimal", "numeric" : Return "DECIMAL"
            Case "date" : Return "DATE"
            Case Else : Throw Fallo("TIPO_NO_SOPORTADO")
        End Select
    End Function
    Private Shared Function Normalizar(celda As CeldaLibroExcel, m As Mapeo, clave As String) As ValorNormalizadoExcel
        Dim esClave = String.Equals(m.Campo.CAMPO_DESTINO, clave, StringComparison.Ordinal)
        Dim salida As New ValorNormalizadoExcel With {.Columna = celda.Columna, .CampoFuente = m.Campo.CAMPO_FUENTE, .CampoDestino = m.Campo.CAMPO_DESTINO, .Tipo = m.Tipo}
        If celda.Valor Is Nothing Then
            If esClave Then Throw Fallo("CLAVE_REQUERIDA")
            If Not m.Columna.Nulable Then Throw Fallo("VALOR_REQUERIDO")
            salida.EsNulo = True : Return salida
        End If
        Dim valor As Object = celda.Valor
        Dim transforma = m.Campo.REMPLAZA_COMA.GetValueOrDefault() = 1 OrElse m.Campo.REMPLAZA_PUNTO.GetValueOrDefault() = 1 OrElse m.Campo.OPCION_SPLITER.GetValueOrDefault() = 1
        If m.Tipo = "TEXTO" OrElse transforma Then
            Dim texto = TextoCelda(celda, m.Tipo = "TEXTO")
            If m.Campo.REMPLAZA_COMA.GetValueOrDefault() = 1 Then texto = texto.Replace(",", "")
            If m.Campo.REMPLAZA_PUNTO.GetValueOrDefault() = 1 Then texto = texto.Replace(".", "")
            If m.Campo.OPCION_SPLITER.GetValueOrDefault() = 1 Then
                Dim partes = texto.Split(New String() {m.Campo.CARACTER_SPLITER}, StringSplitOptions.None)
                If m.Campo.NUMERO_MATRIZ_EXPLITER.Value >= CUInt(partes.Length) Then Throw Fallo("TRANSFORMACION_INVALIDA")
                texto = partes(CInt(m.Campo.NUMERO_MATRIZ_EXPLITER.Value))
            End If
            valor = texto
        End If
        If esClave AndAlso TypeOf valor Is String AndAlso String.IsNullOrWhiteSpace(CStr(valor)) Then Throw Fallo("CLAVE_REQUERIDA")
        Select Case m.Tipo
            Case "TEXTO"
                salida.Texto = CStr(valor)
                ValidarTexto(m.Columna, salida.Texto)
            Case "ENTERO"
                Dim canonico = Numero(valor), enteroGrande As BigInteger = BigInteger.Zero
                If canonico.Contains(".") OrElse Not BigInteger.TryParse(canonico, NumberStyles.AllowLeadingSign, Inv, enteroGrande) Then Throw Fallo("ENTERO_INVALIDO")
                canonico = enteroGrande.ToString(Inv)
                If Not PerfilImportacionCatalogAdapter.ValorCompatible(Opcion(m.Columna, "ENTERO"), canonico) Then Throw Fallo("RANGO_EXCEDIDO")
                salida.NumeroCanonico = canonico
            Case "DECIMAL"
                Dim canonico = Numero(valor)
                If Not PerfilImportacionCatalogAdapter.ValorCompatible(Opcion(m.Columna, "DECIMAL"), canonico) Then Throw Fallo("PRECISION_EXCEDIDA")
                salida.NumeroCanonico = canonico
            Case "DATE"
                Dim fecha As DateTime
                If TypeOf valor Is DateTime Then
                    fecha = DirectCast(valor, DateTime)
                ElseIf Not TypeOf valor Is String OrElse Not DateTime.TryParseExact(CStr(valor), New String() {"yyyy-MM-dd", "dd/MM/yyyy"}, Inv, DateTimeStyles.None, fecha) Then
                    Throw Fallo("FECHA_INVALIDA")
                End If
                If fecha.Year < 1000 Then Throw Fallo("FECHA_INVALIDA")
                salida.Fecha = fecha.Date
        End Select
        Return salida
    End Function
    Private Shared Function TextoCelda(c As CeldaLibroExcel, formatoVisual As Boolean) As String
        If TypeOf c.Valor Is String Then Return CStr(c.Valor)
        If TypeOf c.Valor Is Double AndAlso (Double.IsNaN(CDbl(c.Valor)) OrElse Double.IsInfinity(CDbl(c.Valor))) Then Throw Fallo("NUMERO_INVALIDO")
        If formatoVisual AndAlso Not String.IsNullOrEmpty(c.Formato) AndAlso c.Formato <> "General" AndAlso c.Formato <> "@" Then
            Dim f As New NumberFormat(c.Formato)
            If Not f.IsValid Then Throw Fallo("FORMATO_CELDA_INVALIDO")
            Return f.Format(c.Valor, Inv)
        End If
        If TypeOf c.Valor Is DateTime Then Return DirectCast(c.Valor, DateTime).ToString("yyyy-MM-dd", Inv)
        If TypeOf c.Valor Is Double Then Return CDbl(c.Valor).ToString("R", Inv)
        Return Convert.ToString(c.Valor, Inv)
    End Function
    Private Shared Function Numero(valor As Object) As String
        Dim texto As String
        If TypeOf valor Is String Then
            texto = CStr(valor)
            If Not Regex.IsMatch(texto, "\A[+-]?[0-9]+(?:\.[0-9]+)?\z", RegexOptions.CultureInvariant) Then Throw Fallo("NUMERO_INVALIDO")
        ElseIf TypeOf valor Is Double OrElse TypeOf valor Is Single Then
            Dim n = Convert.ToDouble(valor, Inv)
            If Double.IsNaN(n) OrElse Double.IsInfinity(n) Then Throw Fallo("NUMERO_INVALIDO")
            texto = n.ToString("R", Inv)
        ElseIf TypeOf valor Is Decimal OrElse TypeOf valor Is Integer OrElse TypeOf valor Is Long OrElse TypeOf valor Is UInteger OrElse TypeOf valor Is ULong OrElse TypeOf valor Is Short OrElse TypeOf valor Is Byte Then
            texto = Convert.ToString(valor, Inv)
        Else
            Throw Fallo("NUMERO_INVALIDO")
        End If
        Dim exponente = Regex.Match(texto, "\A([+-]?)([0-9]+)(?:\.([0-9]+))?[Ee]([+-]?[0-9]+)\z", RegexOptions.CultureInvariant)
        If exponente.Success Then
            Dim digitos = exponente.Groups(2).Value & exponente.Groups(3).Value
            Dim punto = exponente.Groups(2).Value.Length + Integer.Parse(exponente.Groups(4).Value, Inv)
            If Math.Abs(punto) > 1024 Then Throw Fallo("NUMERO_INVALIDO")
            texto = exponente.Groups(1).Value & If(punto <= 0, "0." & New String("0"c, -Math.Min(punto, 0)) & digitos, If(punto >= digitos.Length, digitos & New String("0"c, Math.Max(0, punto - digitos.Length)), digitos.Insert(Math.Max(0, Math.Min(punto, digitos.Length)), ".")))
        End If
        Dim negativo = texto.StartsWith("-", StringComparison.Ordinal)
        texto = texto.TrimStart("+"c, "-"c)
        Dim partes = texto.Split("."c), entero = partes(0).TrimStart("0"c)
        If entero = "" Then entero = "0"
        Return If(negativo, "-", "") & entero & If(partes.Length = 2, "." & partes(1), "")
    End Function
    Private Shared Function PrecisionEscala(c As ColumnaDestinoExcel) As Integer()
        Dim match = Regex.Match(If(c.TipoCompleto, ""), "\A(?:decimal|numeric)\(([1-9][0-9]?),([0-9]{1,2})\)(?: unsigned)?\z", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)
        If Not match.Success Then Throw Fallo("DESTINO_INVALIDO")
        Dim p = Integer.Parse(match.Groups(1).Value, Inv), e = Integer.Parse(match.Groups(2).Value, Inv)
        If p > 65 OrElse e > 30 OrElse e > p Then Throw Fallo("DESTINO_INVALIDO")
        Return New Integer() {p, e}
    End Function
    Private Shared Function Opcion(c As ColumnaDestinoExcel, tipo As String) As OpcionPerfilImportacion
        Dim o As New OpcionPerfilImportacion With {.Tipo = tipo, .TipoFisico = If(c.TipoCompleto, "").ToLowerInvariant(), .Longitud = If(tipo = "TEXTO", CInt(Math.Min(Integer.MaxValue, c.Longitud.GetValueOrDefault())), Integer.MaxValue), .Requerida = Not c.Nulable, .Charset = If(c.Charset, "").ToLowerInvariant()}
        If tipo = "DECIMAL" Then
            Dim pe = PrecisionEscala(c) : o.Precision = pe(0) : o.Escala = pe(1)
        End If
        Return o
    End Function
    Private Shared Sub ValidarTexto(columna As ColumnaDestinoExcel, texto As String)
        Dim escalares = 0, pares = 0, i = 0
        While i < texto.Length
            If Char.IsHighSurrogate(texto(i)) Then
                If i + 1 >= texto.Length OrElse Not Char.IsLowSurrogate(texto(i + 1)) Then Throw Fallo("CHARSET_INCOMPATIBLE")
                pares += 1 : i += 1
            ElseIf Char.IsLowSurrogate(texto(i)) Then
                Throw Fallo("CHARSET_INCOMPATIBLE")
            End If
            escalares += 1 : i += 1
        End While
        If escalares > columna.Longitud.Value Then Throw Fallo("LONGITUD_EXCEDIDA")
        Dim charset = columna.Charset.ToLowerInvariant()
        If Not {"ascii", "latin1", "utf8", "utf8mb3", "utf8mb4"}.Contains(charset) Then Throw Fallo("CHARSET_NO_SOPORTADO")
        If (charset = "utf8" OrElse charset = "utf8mb3") AndAlso pares > 0 Then Throw Fallo("CHARSET_INCOMPATIBLE")
        If texto.Length = 0 AndAlso Not columna.Nulable Then Throw Fallo("VALOR_REQUERIDO")
        Dim descriptor = Opcion(columna, "TEXTO")
        descriptor.Longitud = CInt(Math.Min(Integer.MaxValue, columna.Longitud.Value + pares))
        If Not PerfilImportacionCatalogAdapter.ValorCompatible(descriptor, texto) Then Throw Fallo("CHARSET_INCOMPATIBLE")
    End Sub
    Private Shared Function Fallo(codigo As String) As ImportacionExcelException
        Return New ImportacionExcelException(codigo, "Revise el valor y la configuración de la columna indicada.")
    End Function
    Private Shared Sub Agregar(r As ResultadoLecturaExcel, codigo As String, Optional fila As Nullable(Of Integer) = Nothing, Optional columna As Nullable(Of Integer) = Nothing, Optional campo As CampoPlantillaExcel = Nothing)
        r.TotalIncidencias += 1
        If r.Incidencias.Count >= 1000 Then Return
        r.Incidencias.Add(New IncidenciaLecturaExcel With {.Codigo = codigo, .Mensaje = "Revise el archivo y la columna indicada (" & codigo & ").", .Hoja = r.Hoja, .Fila = fila, .Columna = columna, .CampoFuente = If(campo Is Nothing, Nothing, campo.CAMPO_FUENTE), .CampoDestino = If(campo Is Nothing, Nothing, campo.CAMPO_DESTINO)})
    End Sub
End Class
