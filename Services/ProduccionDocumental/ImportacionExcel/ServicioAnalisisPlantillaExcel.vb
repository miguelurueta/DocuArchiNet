Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports ExcelNumberFormat

'Sólo descubrimiento: lectura secuencial, sin archivo durable, plantilla o ejecución.
Public NotInheritable Class ServicioAnalisisPlantillaExcel
    Private Shared ReadOnly Inv As CultureInfo = CultureInfo.InvariantCulture
    Private Class Perfil
        Friend Longitud As Integer = 1
        Friend Familia As String
        Friend Enteros As Integer = 1
        Friend Escala As Integer
        Friend Sub Observar(celda As CeldaLibroExcel, texto As String)
            Longitud = Math.Max(Longitud, texto.Length)
            If celda.Valor Is Nothing Then Return
            Dim tipo = "TEXTO"
            If TypeOf celda.Valor Is DateTime Then
                Dim fecha = DirectCast(celda.Valor, DateTime)
                If fecha.TimeOfDay <> TimeSpan.Zero Then Throw ReglasCargaExcel.Fallo("FECHA_CON_HORA_NO_ADMITIDA")
                If fecha.Year >= 1000 AndAlso fecha.TimeOfDay = TimeSpan.Zero Then tipo = "DATE"
            ElseIf TypeOf celda.Valor Is Double OrElse TypeOf celda.Valor Is Integer OrElse TypeOf celda.Valor Is Long OrElse TypeOf celda.Valor Is Decimal Then
                Dim numero = Convert.ToString(celda.Valor, Inv)
                If TypeOf celda.Valor Is Double Then numero = CDbl(celda.Valor).ToString("R", Inv)
                'Un formato 00000 identifica códigos; no convertirlos a número.
                Dim formatoEntero = If(celda.Formato, "").Split(";"c)(0).Split("."c)(0)
                Dim m = Regex.Match(numero, "\A-?([0-9]+)(?:\.([0-9]+))?\z")
                If m.Success AndAlso Not Regex.IsMatch(formatoEntero, "0{2,}") Then
                    Dim parte = m.Groups(1).Value.TrimStart("0"c), decimales = m.Groups(2).Value.TrimEnd("0"c)
                    Enteros = Math.Max(Enteros, Math.Max(1, parte.Length)) : Escala = Math.Max(Escala, decimales.Length)
                    If Enteros + Escala <= 65 AndAlso Escala <= 30 Then tipo = "NUMERO"
                End If
            End If
            If Familia Is Nothing Then
                Familia = tipo
            ElseIf Familia <> tipo Then
                Familia = "TEXTO"
            End If
        End Sub
        Friend Function Tipo() As String
            If Familia = "DATE" Then Return "DATE"
            If Familia = "NUMERO" Then Return "DECIMAL(" & (Enteros + Escala).ToString(Inv) & "," & Escala.ToString(Inv) & ")"
            If Longitud > 4096 Then Throw ReglasCargaExcel.Fallo("LONGITUD_NO_ADMITIDA")
            Return "VARCHAR(" & Math.Min(4096, Math.Max(32, Longitud + Longitud \ 4)).ToString(Inv) & ")"
        End Function
    End Class
    Public Function Analizar(contenido As Stream, nombre As String, hoja As String, limites As LimitesLecturaExcel, Optional destino As DestinoPlantillaExcel = Nothing) As AnalisisPlantillaExcel
        ReglasCargaExcel.Limites(limites)
        Dim formato = Path.GetExtension(If(nombre, "")).TrimStart("."c).ToUpperInvariant(), lector As New LectorArchivoExcel
        Dim inspeccion = lector.Inspeccionar(contenido, formato, nombre, limites)
        If String.IsNullOrEmpty(hoja) Then hoja = inspeccion.Hojas(0)
        If hoja.Length > 120 OrElse Not inspeccion.Hojas.Contains(hoja, StringComparer.Ordinal) Then Throw ReglasCargaExcel.Fallo("HOJA_NO_ENCONTRADA")
        Dim salida As New AnalisisPlantillaExcel With {.Formato = formato, .Hoja = hoja, .Hojas = inspeccion.Hojas}
        Dim perfiles As New List(Of Perfil), origenes As New HashSet(Of String)(StringComparer.Ordinal), nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim solicitud As New SolicitudLecturaExcel With {.NombreArchivo = nombre, .FormatoEsperado = formato, .Hoja = hoja, .Limites = limites}
        lector.Recorrer(contenido, solicitud, New ResultadoLecturaExcel,
            Sub(fila)
                For Each celda In fila.Celdas
                    If celda.ErrorCodigo IsNot Nothing Then Throw ReglasCargaExcel.Fallo(celda.ErrorCodigo)
                Next
                If fila.Fila = 1 Then
                    Dim ultima = fila.Celdas.FindLastIndex(Function(c) c.Valor IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Convert.ToString(c.Valor, Inv))) + 1
                    If ultima = 0 OrElse ultima > 256 Then Throw ReglasCargaExcel.Fallo("ENCABEZADOS_INVALIDOS")
                    For i = 0 To ultima - 1
                        Dim fuente = Convert.ToString(fila.Celdas(i).Valor, Inv)
                        If String.IsNullOrWhiteSpace(fuente) OrElse fuente.Length > 120 OrElse fuente.Any(Function(c) Char.IsControl(c)) Then Throw ReglasCargaExcel.Fallo("ENCABEZADOS_INVALIDOS")
                        If Not origenes.Add(fuente) Then Throw ReglasCargaExcel.Fallo("ENCABEZADO_AMBIGUO")
                        Dim propuesto = NombreDestino(fuente), candidato = propuesto, sufijo = 1
                        While Not nombres.Add(candidato)
                            sufijo += 1 : candidato = propuesto.Substring(0, Math.Min(propuesto.Length, 54)) & "_" & sufijo.ToString(Inv)
                        End While
                        salida.Campos.Add(New CampoPlantillaExcel With {.CAMPO_FUENTE = fuente, .CAMPO_DESTINO = candidato, .REMPLAZA_COMA = 0, .REMPLAZA_PUNTO = 0, .OPCION_SPLITER = 0, .NUMERO_MATRIZ_EXPLITER = 0})
                        perfiles.Add(New Perfil)
                    Next
                    Return
                End If
                If fila.Celdas.All(Function(c) c.Valor Is Nothing OrElse (TypeOf c.Valor Is String AndAlso CStr(c.Valor).Length = 0)) Then Return
                salida.FilasObservadas += 1
                If salida.FilasObservadas > limites.MaxFilasDatos Then Throw ReglasCargaExcel.Fallo("LIMITE_FILAS_EXCEDIDO")
                If perfiles.Count = 0 OrElse fila.Celdas.Skip(perfiles.Count).Any(Function(c) c.Valor IsNot Nothing AndAlso Convert.ToString(c.Valor, Inv).Length > 0) Then Throw ReglasCargaExcel.Fallo("ENCABEZADOS_INVALIDOS")
                Dim muestra As New List(Of String)
                For i = 0 To perfiles.Count - 1
                    Dim celda = fila.Celdas(i), texto = TextoCelda(celda)
                    perfiles(i).Observar(celda, If(texto, ""))
                    If salida.Muestra.Count < 25 Then muestra.Add(If(texto Is Nothing, Nothing, texto.Substring(0, Math.Min(texto.Length, 120))))
                Next
                If salida.Muestra.Count < 25 Then salida.Muestra.Add(muestra)
            End Sub)
        If salida.FilasObservadas = 0 Then Throw ReglasCargaExcel.Fallo("LIBRO_SIN_DATOS")
        For i = 0 To perfiles.Count - 1
            Dim campo = salida.Campos(i)
            campo.TIPO_CAMPO = perfiles(i).Tipo()
            If destino IsNot Nothing AndAlso destino.Existe Then
                Dim fisica = destino.Columnas.SingleOrDefault(Function(c) String.Equals(c.Nombre, campo.CAMPO_FUENTE, StringComparison.OrdinalIgnoreCase))
                If fisica Is Nothing Then fisica = destino.Columnas.SingleOrDefault(Function(c) String.Equals(c.Nombre, campo.CAMPO_DESTINO, StringComparison.OrdinalIgnoreCase))
                If fisica IsNot Nothing Then
                    campo.CAMPO_DESTINO = fisica.Nombre : campo.TIPO_CAMPO = fisica.TipoCompleto
                ElseIf Not salida.Avisos.Contains("REVISAR_MAPEO_DESTINO") Then
                    salida.Avisos.Add("REVISAR_MAPEO_DESTINO")
                End If
            End If
        Next
        If salida.Campos.Select(Function(c) c.CAMPO_DESTINO).Distinct(StringComparer.OrdinalIgnoreCase).Count() <> salida.Campos.Count Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        salida.Avisos.Add("PROPUESTA_REQUIERE_REVISION")
        Dim posicion = contenido.Position
        Try
            contenido.Position = 0
            Using digest = SHA256.Create()
                salida.ArchivoHash = BitConverter.ToString(digest.ComputeHash(contenido)).Replace("-", "").ToLowerInvariant()
            End Using
        Finally
            contenido.Position = posicion
        End Try
        Return salida
    End Function
    Private Shared Function NombreDestino(fuente As String) As String
        Dim b As New StringBuilder
        For Each c In fuente.Normalize(NormalizationForm.FormD)
            If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then b.Append(c)
        Next
        Dim nombre = Regex.Replace(b.ToString().ToUpperInvariant(), "[^A-Z0-9_]+", "_").Trim("_"c)
        If nombre.Length = 0 Then nombre = "CAMPO"
        If Char.IsDigit(nombre(0)) Then nombre = "CAMPO_" & nombre
        Return nombre.Substring(0, Math.Min(nombre.Length, 64))
    End Function
    Private Shared Function TextoCelda(celda As CeldaLibroExcel) As String
        If celda.Valor Is Nothing Then Return Nothing
        If TypeOf celda.Valor Is String Then Return CStr(celda.Valor)
        If TypeOf celda.Valor Is Double AndAlso (Double.IsNaN(CDbl(celda.Valor)) OrElse Double.IsInfinity(CDbl(celda.Valor))) Then Throw ReglasCargaExcel.Fallo("NUMERO_INVALIDO")
        If Not String.IsNullOrEmpty(celda.Formato) AndAlso celda.Formato <> "General" AndAlso celda.Formato <> "@" Then
            Dim formato As New NumberFormat(celda.Formato)
            If Not formato.IsValid Then Throw ReglasCargaExcel.Fallo("FORMATO_CELDA_INVALIDO")
            Return formato.Format(celda.Valor, Inv)
        End If
        If TypeOf celda.Valor Is DateTime Then Return DirectCast(celda.Valor, DateTime).ToString("yyyy-MM-dd", Inv)
        If TypeOf celda.Valor Is Double Then Return CDbl(celda.Valor).ToString("R", Inv)
        Return Convert.ToString(celda.Valor, Inv)
    End Function
End Class
