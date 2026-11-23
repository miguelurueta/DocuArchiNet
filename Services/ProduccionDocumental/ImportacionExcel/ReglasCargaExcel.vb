Option Strict On
Imports System
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public NotInheritable Class ReglasCargaExcel
    Private Sub New()
    End Sub
    Public Shared Sub Autorizar(c As ContextoCargaExcel)
        ServicioPlantillasExcel.Autorizar(c)
        If c.ModuloId = 0 OrElse c.ConexionId = 0 OrElse Not HashValido(c.IdentidadConexion) Then Throw Fallo("ACCESO_DENEGADO")
    End Sub
    Public Shared Sub MismaAutoridad(a As ContextoCargaExcel, b As ContextoCargaExcel)
        Autorizar(a) : Autorizar(b)
        If a.UsuarioId <> b.UsuarioId OrElse a.EmpresaId <> b.EmpresaId OrElse a.ModuloId <> b.ModuloId OrElse a.ConexionId <> b.ConexionId OrElse a.IdentidadConexion <> b.IdentidadConexion Then Throw Fallo("ACCESO_DENEGADO")
    End Sub
    Public Shared Function IdValido(id As String) As Boolean
        Dim parsed As Guid
        Return Guid.TryParseExact(id, "D", parsed) AndAlso parsed <> Guid.Empty AndAlso parsed.ToString("D") = id
    End Function
    Public Shared Function HashValido(hash As String) As Boolean
        Return Regex.IsMatch(If(hash, ""), "\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant)
    End Function
    Public Shared Sub Limites(l As LimitesLecturaExcel)
        If l Is Nothing OrElse l.MaxFilasDatos < 1 OrElse l.MaxArchivoMB < 1 OrElse l.MaxArchivoMB > Integer.MaxValue \ 1000000 Then Throw Fallo("CAPACIDAD_NO_DISPONIBLE")
    End Sub
    Public Shared Sub Propietario(c As ContextoCargaExcel, e As CargaExcelEjecucion)
        Autorizar(c)
        If e Is Nothing OrElse e.EmpresaId <> c.EmpresaId OrElse e.UsuarioId <> c.UsuarioId OrElse e.ConexionRef <> c.ConexionRef OrElse Not IdValido(e.Id) Then Throw Fallo("REFERENCIA_NO_DISPONIBLE")
    End Sub
    Public Shared Sub Plantilla(c As ContextoCargaExcel, p As PlantillaExcel, revision As UInteger, hash As String)
        Autorizar(c)
        If p Is Nothing OrElse p.IMP01_XMLCONEXION_ID_XMLCONEXION <> c.ConexionId OrElse p.ESTADO_PLANTILLA <> 1 OrElse Not ServicioPlantillasExcel.TablaValida(p.NOMBRE_PLANTILLA) Then Throw Fallo("REFERENCIA_NO_DISPONIBLE")
        If revision = 0 OrElse p.REVISION_CARGA <> revision OrElse Not HashValido(hash) OrElse ServicioPlantillasExcel.Huella(p) <> hash Then Throw Fallo("CONFLICTO_REVISION")
        ServicioPlantillasExcel.ModoHistorico(p)
    End Sub
    Public Shared Function Hash(bytes As Byte()) As String
        Using digest = SHA256.Create()
            Return BitConverter.ToString(digest.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Public Shared Function Canonico(documento As Object) As String
        Dim serializer As New JsonSerializer With {.Culture = CultureInfo.InvariantCulture, .DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ"}
        Dim token = JToken.FromObject(documento, serializer)
        Using output As New IO.StringWriter(CultureInfo.InvariantCulture), writer As New JsonTextWriter(output)
            writer.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            Ordenar(token).WriteTo(writer) : writer.Flush()
            Return output.ToString()
        End Using
    End Function
    Private Shared Function Ordenar(token As JToken) As JToken
        If TypeOf token Is JObject Then
            Dim result As New JObject
            For Each p In DirectCast(token, JObject).Properties().OrderBy(Function(x) x.Name, StringComparer.Ordinal)
                result.Add(p.Name, Ordenar(p.Value))
            Next
            Return result
        End If
        If TypeOf token Is JArray Then Return New JArray(DirectCast(token, JArray).Select(Function(x) Ordenar(x)))
        Return token.DeepClone()
    End Function
    Public Shared Function HashDocumento(documento As Object) As String
        Return Hash(New UTF8Encoding(False, True).GetBytes(Canonico(documento)))
    End Function
    Public Shared Function Estado(e As CargaExcelEjecucion) As EstadoCargaExcel
        Return New EstadoCargaExcel With {.IdCarga = e.Id, .PlantillaId = e.PlantillaId, .Estado = e.Estado, .NombreArchivo = e.Archivo.Nombre, .Bytes = e.Archivo.Bytes, .ArchivoHash = e.Archivo.Hash, .Tabla = e.Tabla, .Modo = e.Modo, .RevisionPlantilla = e.RevisionPlantilla, .ConfiguracionHash = e.ConfiguracionHash, .EsquemaHash = e.EsquemaHash, .FilasPrevistas = e.FilasPrevistas, .CodigoIncidencia = e.CodigoIncidencia, .FilasConfirmadas = e.FilasConfirmadas, .ConfirmadoUtc = e.ConfirmadoUtc, .Hojas = e.Archivo.Hojas.ToList()}
    End Function
    Public Shared Function Fallo(codigo As String) As ImportacionExcelException
        Return New ImportacionExcelException(codigo, "Revise el estado de la carga y sus datos (" & codigo & ").")
    End Function
End Class
