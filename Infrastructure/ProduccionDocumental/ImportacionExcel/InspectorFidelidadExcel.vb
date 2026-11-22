Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Xml

'ExcelDataReader3.6.0 oculta errores como Nothing. Este observador solo conserva
'errores/ausencia de caché; la biblioteca sigue leyendo todos los valores y formatos.
Friend NotInheritable Class InspectorFidelidadExcel
    Friend Const VacioFormula As String = "FORMULA_VACIA"
    Friend ReadOnly Marcas As New Dictionary(Of String, String)(StringComparer.Ordinal)
    Friend Property Formato As String
    Friend Shared Function Clave(fila As Integer, columna As Integer) As String
        Return fila.ToString(CultureInfo.InvariantCulture) & ":" & columna.ToString(CultureInfo.InvariantCulture)
    End Function
    Friend Shared Function Inspeccionar(stream As Stream, hoja As String, presupuesto As Long) As InspectorFidelidadExcel
        Dim salida As New InspectorFidelidadExcel, posicion = stream.Position
        Try
            stream.Position = 0
            Dim probe(7) As Byte
            If stream.Read(probe, 0, 8) <> 8 Then Throw Invalido()
            stream.Position = 0
            If probe(0) = &H50 AndAlso probe(1) = &H4B Then
                salida.Formato = "XLSX"
                salida.Xml(stream, hoja, presupuesto)
            Else
                salida.Formato = "XLS"
                Dim datos(CInt(stream.Length) - 1) As Byte
                Dim usados = 0
                While usados < datos.Length
                    Dim leidos = stream.Read(datos, usados, datos.Length - usados)
                    If leidos = 0 Then Throw Invalido()
                    usados += leidos
                End While
                If probe.SequenceEqual(New Byte() {&HD0, &HCF, &H11, &HE0, &HA1, &HB1, &H1A, &HE1}) Then
                    datos = New ContenedorCompuesto(datos).Libro()
                ElseIf Not {&H9, &H209, &H409, &H809}.Contains(CInt(BitConverter.ToUInt16(datos, 0))) Then
                    Throw New ImportacionExcelException("FORMATO_NO_SOPORTADO", "El archivo debe ser un libro XLS o XLSX válido.")
                End If
                salida.Biff(datos, hoja)
            End If
            Return salida
        Finally
            stream.Position = posicion
        End Try
    End Function
    Private Shared Function Invalido() As ImportacionExcelException
        Return New ImportacionExcelException("LIBRO_INVALIDO", "No se pudo interpretar el libro de Excel.")
    End Function
    Private Shared Function XmlSeguro(entrada As ZipArchiveEntry, presupuesto As Long) As XmlReader
        If entrada Is Nothing Then Throw Invalido()
        Return XmlReader.Create(entrada.Open(), New XmlReaderSettings With {.DtdProcessing = DtdProcessing.Prohibit, .XmlResolver = Nothing, .CloseInput = True, .MaxCharactersInDocument = presupuesto})
    End Function
    Private Sub Xml(stream As Stream, hoja As String, presupuesto As Long)
        Using zip As New ZipArchive(stream, ZipArchiveMode.Read, True)
            Dim total As Long = 0, nombres As New HashSet(Of String)(StringComparer.Ordinal)
            For Each e In zip.Entries
                If Not nombres.Add(e.FullName) OrElse e.Length > presupuesto - total Then Throw Invalido()
                total += e.Length
            Next
            Dim tipoCorrecto = False
            Using x = XmlSeguro(zip.GetEntry("[Content_Types].xml"), presupuesto)
                While x.Read()
                    If x.NodeType = XmlNodeType.Element AndAlso x.LocalName = "Override" AndAlso x.GetAttribute("PartName") = "/xl/workbook.xml" Then
                        tipoCorrecto = x.GetAttribute("ContentType") = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"
                    End If
                End While
            End Using
            If Not tipoCorrecto Then Throw New ImportacionExcelException("FORMATO_NO_SOPORTADO", "El archivo debe ser XLSX sin macros.")
            Dim relacion As String = Nothing, encontradas = 0
            Using x = XmlSeguro(zip.GetEntry("xl/workbook.xml"), presupuesto)
                While x.Read()
                    If x.NodeType = XmlNodeType.Element AndAlso x.LocalName = "sheet" AndAlso x.GetAttribute("name") = hoja Then
                        relacion = x.GetAttribute("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships")
                        encontradas += 1
                    End If
                End While
            End Using
            If encontradas > 1 Then Throw Invalido()
            If relacion Is Nothing Then Return 'El lector informará hoja ausente.
            Dim target As String = Nothing
            Using x = XmlSeguro(zip.GetEntry("xl/_rels/workbook.xml.rels"), presupuesto)
                While x.Read()
                    If x.NodeType = XmlNodeType.Element AndAlso x.LocalName = "Relationship" AndAlso x.GetAttribute("Id") = relacion Then
                        If target IsNot Nothing OrElse x.GetAttribute("TargetMode") = "External" OrElse Not If(x.GetAttribute("Type"), "").EndsWith("/worksheet", StringComparison.Ordinal) Then Throw Invalido()
                        Dim ruta = New Uri(New Uri("http://package.invalid/xl/workbook.xml"), x.GetAttribute("Target"))
                        If ruta.Host <> "package.invalid" OrElse ruta.Scheme <> "http" Then Throw Invalido()
                        target = Uri.UnescapeDataString(ruta.AbsolutePath.TrimStart("/"c))
                    End If
                End While
            End Using
            If target Is Nothing Then Throw Invalido()
            Using x = XmlSeguro(zip.GetEntry(target), presupuesto)
                While x.Read()
                    If x.NodeType <> XmlNodeType.Element OrElse x.LocalName <> "c" Then Continue While
                    Dim referencia = x.GetAttribute("r"), tipo = x.GetAttribute("t")
                    Dim formula = False, tieneValor = False, valor As String = Nothing
                    Using celda = x.ReadSubtree()
                        While celda.Read()
                            If celda.NodeType <> XmlNodeType.Element Then Continue While
                            If celda.LocalName = "f" Then formula = True
                            If celda.LocalName = "v" Then
                                tieneValor = True
                                valor = celda.ReadElementContentAsString()
                            End If
                        End While
                    End Using
                    Dim marca As String = Nothing
                    If tipo = "e" Then
                        marca = "ERROR_CELDA_EXCEL"
                    ElseIf formula AndAlso (Not tieneValor OrElse (String.IsNullOrEmpty(valor) AndAlso tipo <> "str")) Then
                        marca = "FORMULA_SIN_RESULTADO"
                    ElseIf formula AndAlso tipo = "str" AndAlso valor = "" Then
                        marca = VacioFormula
                    End If
                    If marca IsNot Nothing Then
                        Dim match = System.Text.RegularExpressions.Regex.Match(If(referencia, ""), "\A([A-Z]+)([1-9][0-9]*)\z")
                        If Not match.Success Then Throw Invalido()
                        Dim columna = 0
                        For Each ch In match.Groups(1).Value
                            columna = columna * 26 + AscW(ch) - 64
                        Next
                        Dim fila = Integer.Parse(match.Groups(2).Value, CultureInfo.InvariantCulture)
                        If columna < 1 OrElse columna > 16384 OrElse fila > 1048576 Then Throw Invalido()
                        If Marcas.ContainsKey(Clave(fila, columna)) Then Throw Invalido()
                        Marcas.Add(Clave(fila, columna), marca)
                    End If
                End While
            End Using
        End Using
    End Sub
    Private Sub Biff(datos As Byte(), hoja As String)
        Dim inicio = 0, offset = 0, version = 0, esGlobal = False, encontrado = False
        While offset + 4 <= datos.Length
            Dim id = CInt(BitConverter.ToUInt16(datos, offset)), largo = CInt(BitConverter.ToUInt16(datos, offset + 2)), p = offset + 4
            If p + largo > datos.Length Then Throw Invalido()
            If {&H9, &H209, &H409, &H809}.Contains(id) Then
                If largo < 4 Then Throw Invalido()
                version = If(id = &H809, CInt(BitConverter.ToUInt16(datos, p)), If(id = &H9, &H200, If(id = &H209, &H300, &H400)))
                esGlobal = BitConverter.ToUInt16(datos, p + 2) = 5
                If Not esGlobal Then
                    encontrado = True : inicio = offset : Exit While
                End If
            ElseIf id = &H2F Then
                Throw Invalido() 'FILEPASS: no claves de cifrado ni reintentos.
            ElseIf id = &H85 Then
                If largo < 7 Then Throw Invalido()
                Dim nombre As String, n = CInt(datos(p + 6))
                If version >= &H600 Then
                    If largo < 8 OrElse largo < 8 + n * If((datos(p + 7) And 1) = 1, 2, 1) Then Throw Invalido()
                    nombre = If((datos(p + 7) And 1) = 1, Encoding.Unicode.GetString(datos, p + 8, n * 2), Encoding.GetEncoding(1252).GetString(datos, p + 8, n))
                Else
                    If largo < 7 + n Then Throw Invalido()
                    nombre = Encoding.GetEncoding(1252).GetString(datos, p + 7, n)
                End If
                If nombre = hoja Then
                    If encontrado Then Throw Invalido()
                    inicio = CInt(BitConverter.ToUInt32(datos, p)) : encontrado = True
                End If
            ElseIf id = &HA Then
                Exit While
            End If
            offset = p + largo
        End While
        If Not encontrado Then Return
        offset = inicio
        While offset + 4 <= datos.Length
            Dim id = CInt(BitConverter.ToUInt16(datos, offset)), largo = CInt(BitConverter.ToUInt16(datos, offset + 2)), p = offset + 4
            If p + largo > datos.Length Then Throw Invalido()
            If id = &HA Then Exit While
            If id = &H2F Then Throw Invalido()
            Dim marca As String = Nothing
            If id = &H205 OrElse id = &H5 Then
                Dim cabecera = If(id = &H5, 7, 6)
                If largo < cabecera + 2 Then Throw Invalido()
                If datos(p + cabecera + 1) <> 0 Then marca = "ERROR_CELDA_EXCEL"
            ElseIf {&H6, &H206, &H406}.Contains(id) Then
                Dim cabecera = If(version = &H200, 7, 6)
                If largo < cabecera + 8 Then Throw Invalido()
                Dim v = p + cabecera
                If datos(v + 6) = 255 AndAlso datos(v + 7) = 255 Then
                    Select Case datos(v)
                        Case 2 : marca = "ERROR_CELDA_EXCEL"
                        Case 3 : marca = VacioFormula
                        Case 0
                            Dim siguiente = p + largo
                            If siguiente + 4 > datos.Length Then Throw Invalido()
                            If BitConverter.ToUInt16(datos, siguiente) = &H4BC Then siguiente += 4 + CInt(BitConverter.ToUInt16(datos, siguiente + 2))
                            If siguiente + 4 > datos.Length OrElse Not {&H207, &H7}.Contains(CInt(BitConverter.ToUInt16(datos, siguiente))) Then marca = "FORMULA_SIN_RESULTADO"
                        Case 1 'Booleano guardado; lo interpreta ExcelDataReader.
                        Case Else : marca = "FORMULA_SIN_RESULTADO"
                    End Select
                End If
            End If
            If marca IsNot Nothing Then
                If largo < 4 Then Throw Invalido()
                Dim fila = CInt(BitConverter.ToUInt16(datos, p)) + 1, columna = CInt(BitConverter.ToUInt16(datos, p + 2)) + 1
                If Marcas.ContainsKey(Clave(fila, columna)) Then Throw Invalido()
                Marcas.Add(Clave(fila, columna), marca)
            End If
            offset = p + largo
        End While
    End Sub

    'Extracción acotada del stream BIFF, incluidas FAT/DIFAT/miniFAT.
    Private NotInheritable Class ContenedorCompuesto
        Private ReadOnly _datos As Byte(), _sector As Integer
        Private ReadOnly _fat As New List(Of UInteger)
        Private Const Fin As UInteger = &HFFFFFFFEUI
        Private Const Libre As UInteger = &HFFFFFFFFUI
        Friend Sub New(datos As Byte())
            _datos = datos
            If datos.Length < 512 OrElse BitConverter.ToUInt16(datos, 28) <> &HFFFE Then Throw Invalido()
            Dim shift = BitConverter.ToUInt16(datos, 30)
            If (shift <> 9 AndAlso shift <> 12) OrElse BitConverter.ToUInt16(datos, 32) <> 6 Then Throw Invalido()
            _sector = 1 << shift
            Dim sectoresFat As New List(Of UInteger), vistos As New HashSet(Of UInteger)
            For i = 0 To 108
                Dim sid = BitConverter.ToUInt32(datos, 76 + i * 4)
                If sid <> Libre Then sectoresFat.Add(sid)
            Next
            Dim dif = BitConverter.ToUInt32(datos, 68), cuenta = BitConverter.ToUInt32(datos, 72)
            If cuenta > datos.Length \ _sector Then Throw Invalido()
            For i As UInteger = 1 To cuenta
                If Not vistos.Add(dif) Then Throw Invalido()
                Dim bloque = Sector(dif)
                For j = 0 To _sector - 8 Step 4
                    Dim sid = BitConverter.ToUInt32(bloque, j)
                    If sid <> Libre Then sectoresFat.Add(sid)
                Next
                dif = BitConverter.ToUInt32(bloque, _sector - 4)
            Next
            If sectoresFat.Count <> BitConverter.ToUInt32(datos, 44) OrElse sectoresFat.Distinct().Count() <> sectoresFat.Count Then Throw Invalido()
            For Each sid In sectoresFat
                Dim bloque = Sector(sid)
                For j = 0 To _sector - 4 Step 4
                    _fat.Add(BitConverter.ToUInt32(bloque, j))
                Next
            Next
        End Sub
        Private Function Sector(sid As UInteger) As Byte()
            Dim pos = (CLng(sid) + 1L) * _sector
            If pos < _sector OrElse pos > _datos.LongLength - _sector Then Throw Invalido()
            Dim b(_sector - 1) As Byte
            Buffer.BlockCopy(_datos, CInt(pos), b, 0, _sector)
            Return b
        End Function
        Private Function Cadena(inicio As UInteger, tabla As List(Of UInteger), tamano As Long, Optional mini As Byte() = Nothing) As Byte()
            If tamano < 0 OrElse tamano > _datos.LongLength Then Throw Invalido()
            Dim sid = inicio, vistos As New HashSet(Of UInteger), unidad = If(mini Is Nothing, _sector, 64)
            Using salida As New MemoryStream()
                While sid <> Fin
                    If sid >= tabla.Count OrElse Not vistos.Add(sid) OrElse salida.Length >= _datos.LongLength Then Throw Invalido()
                    Dim b(unidad - 1) As Byte
                    If mini Is Nothing Then
                        b = Sector(sid)
                    Else
                        Dim pos = CLng(sid) * unidad
                        If pos > mini.LongLength - unidad Then Throw Invalido()
                        Buffer.BlockCopy(mini, CInt(pos), b, 0, unidad)
                    End If
                    salida.Write(b, 0, b.Length)
                    sid = tabla(CInt(sid))
                End While
                If tamano > salida.Length Then Throw Invalido()
                Dim bytes = salida.ToArray()
                If tamano = 0 Then Return bytes 'Directorio: tamaño de cadena, no metadato de archivo.
                Array.Resize(bytes, CInt(tamano))
                Return bytes
            End Using
        End Function
        Friend Function Libro() As Byte()
            Dim directorio = Cadena(BitConverter.ToUInt32(_datos, 48), _fat, 0)
            If directorio.Length < 128 OrElse directorio(66) <> 5 Then Throw Invalido()
            Dim inicio As UInteger = Libre, tamano As Long = 0, candidatos = 0
            For p = 0 To directorio.Length - 128 Step 128
                Dim n = CInt(BitConverter.ToUInt16(directorio, p + 64))
                If directorio(p + 66) <> 2 OrElse n < 2 OrElse n > 64 OrElse n Mod 2 <> 0 Then Continue For
                Dim nombre = Encoding.Unicode.GetString(directorio, p, n - 2)
                If nombre = "EncryptedPackage" Then Throw Invalido()
                If nombre <> "Workbook" AndAlso nombre <> "Book" Then Continue For
                inicio = BitConverter.ToUInt32(directorio, p + 116)
                tamano = CLng(BitConverter.ToUInt64(directorio, p + 120))
                candidatos += 1
            Next
            If candidatos <> 1 OrElse tamano <= 0 Then Throw Invalido()
            If tamano >= BitConverter.ToUInt32(_datos, 56) Then Return Cadena(inicio, _fat, tamano)
            Dim miniRoot = Cadena(BitConverter.ToUInt32(directorio, 116), _fat, CLng(BitConverter.ToUInt64(directorio, 120)))
            Dim miniBytes = Cadena(BitConverter.ToUInt32(_datos, 60), _fat, CLng(BitConverter.ToUInt32(_datos, 64)) * _sector)
            Dim miniFat As New List(Of UInteger)
            For p = 0 To miniBytes.Length - 4 Step 4
                miniFat.Add(BitConverter.ToUInt32(miniBytes, p))
            Next
            Return Cadena(inicio, miniFat, tamano, miniRoot)
        End Function
    End Class
End Class
