Option Strict On
Imports System
Imports System.IO
Imports System.Linq
Imports ExcelDataReader

Public NotInheritable Class LectorArchivoExcel
    Public Function Inspeccionar(stream As Stream, formato As String, nombre As String, limites As LimitesLecturaExcel) As ResultadoLecturaExcel
        ReglasCargaExcel.Limites(limites)
        If stream Is Nothing OrElse Not stream.CanSeek OrElse Not stream.CanRead OrElse stream.Length < 8 Then Throw Fallo("LIBRO_INVALIDO")
        If stream.Length > CLng(limites.MaxArchivoMB) * 1000000L Then Throw Fallo("LIMITE_ARCHIVO_EXCEDIDO")
        If (formato <> "XLS" AndAlso formato <> "XLSX") OrElse Not String.Equals(Path.GetExtension(nombre), "." & formato, StringComparison.OrdinalIgnoreCase) Then Throw Fallo("FORMATO_NO_COINCIDE")
        Dim posicion = stream.Position
        Try
            Dim inspeccion = InspectorFidelidadExcel.Inspeccionar(stream, Nothing, CLng(limites.MaxArchivoMB) * 40000000L)
            If inspeccion.Formato <> formato Then Throw Fallo("FORMATO_NO_COINCIDE")
            Dim resultado As New ResultadoLecturaExcel With {.Formato = formato}
            stream.Position = 0
            Using reader = ExcelReaderFactory.CreateReader(stream, New ExcelReaderConfiguration With {.LeaveOpen = True})
                Do
                    If resultado.Hojas.Contains(reader.Name, StringComparer.Ordinal) Then Throw Fallo("LIBRO_INVALIDO")
                    resultado.Hojas.Add(reader.Name)
                Loop While reader.NextResult()
            End Using
            If resultado.Hojas.Count = 0 Then Throw Fallo("LIBRO_INVALIDO")
            Return resultado
        Catch ex As ImportacionExcelException
            Throw
        Catch
            Throw Fallo("LIBRO_INVALIDO")
        Finally
            stream.Position = posicion
        End Try
    End Function
    Friend Sub Recorrer(stream As Stream, solicitud As SolicitudLecturaExcel, resultado As ResultadoLecturaExcel, procesar As Action(Of FilaLibroExcel))
        If stream Is Nothing OrElse Not stream.CanRead OrElse Not stream.CanSeek Then Throw Fallo("ARCHIVO_INVALIDO")
        Dim posicion = stream.Position
        Try
            If solicitud.Limites Is Nothing OrElse solicitud.Limites.MaxFilasDatos < 1 OrElse solicitud.Limites.MaxArchivoMB < 1 OrElse solicitud.Limites.MaxArchivoMB > Integer.MaxValue \ 1000000 Then Throw Fallo("CONFIGURACION_INVALIDA")
            Dim maxBytes = CLng(solicitud.Limites.MaxArchivoMB) * 1000000L
            If stream.Length > maxBytes Then Throw Fallo("LIMITE_ARCHIVO_EXCEDIDO")
            If stream.Length < 8 Then Throw Fallo("LIBRO_INVALIDO")
            If solicitud.FormatoEsperado <> "XLS" AndAlso solicitud.FormatoEsperado <> "XLSX" Then Throw Fallo("FORMATO_NO_SOPORTADO")
            Dim extension = Path.GetExtension(If(solicitud.NombreArchivo, ""))
            If Not String.Equals(extension, "." & solicitud.FormatoEsperado, StringComparison.OrdinalIgnoreCase) Then Throw Fallo("FORMATO_NO_COINCIDE")
            Dim inspector = InspectorFidelidadExcel.Inspeccionar(stream, solicitud.Hoja, maxBytes * 40L)
            resultado.Formato = inspector.Formato
            If inspector.Formato <> solicitud.FormatoEsperado Then Throw Fallo("FORMATO_NO_COINCIDE")
            stream.Position = 0
            Using reader = ExcelReaderFactory.CreateReader(stream, New ExcelReaderConfiguration With {.LeaveOpen = True})
                Do
                    resultado.Hojas.Add(reader.Name)
                Loop While reader.NextResult()
                If resultado.Hojas.Where(Function(n) String.Equals(n, solicitud.Hoja, StringComparison.Ordinal)).Count() <> 1 Then Throw Fallo("HOJA_NO_ENCONTRADA")
                reader.Reset()
                While Not String.Equals(reader.Name, solicitud.Hoja, StringComparison.Ordinal)
                    If Not reader.NextResult() Then Throw Fallo("HOJA_NO_ENCONTRADA")
                End While
                While reader.Read()
                    Dim fila As New FilaLibroExcel With {.Fila = reader.Depth + 1}
                    For i = 0 To reader.FieldCount - 1
                        Dim marca As String = Nothing, valor = reader.GetValue(i)
                        inspector.Marcas.TryGetValue(InspectorFidelidadExcel.Clave(fila.Fila, i + 1), marca)
                        If marca = InspectorFidelidadExcel.VacioFormula Then
                            valor = "" : marca = Nothing
                        End If
                        fila.Celdas.Add(New CeldaLibroExcel With {.Columna = i + 1, .Valor = valor, .Formato = reader.GetNumberFormatString(i), .ErrorCodigo = marca})
                    Next
                    procesar(fila)
                End While
            End Using
        Finally
            stream.Position = posicion
        End Try
    End Sub
    Private Shared Function Fallo(codigo As String) As ImportacionExcelException
        Return New ImportacionExcelException(codigo, "Revise el formato, la hoja y los límites del archivo Excel.")
    End Function
End Class
