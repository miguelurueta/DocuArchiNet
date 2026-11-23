Option Strict On
Imports System
Imports System.Text
Imports System.Web.Security
Imports Newtonsoft.Json

'Referencia entre sesiones del mismo propietario; CSRF se valida aparte en cada sesión.
Public Class ReferenciaRecepcionExcel
    Private ReadOnly _proteger As Func(Of Byte(), Byte())
    Private ReadOnly _abrir As Func(Of Byte(), Byte())
    Private ReadOnly _ahora As Func(Of DateTime)
    Public Sub New()
        Me.New(Function(b) MachineKey.Protect(b, "ImportacionExcelRecepcion", "DOC124-v1"), Function(b) MachineKey.Unprotect(b, "ImportacionExcelRecepcion", "DOC124-v1"), Function() DateTime.UtcNow)
    End Sub
    Public Sub New(proteger As Func(Of Byte(), Byte()), abrir As Func(Of Byte(), Byte()), ahora As Func(Of DateTime))
        _proteger = proteger : _abrir = abrir : _ahora = ahora
    End Sub
    Public Function Emitir(c As ContextoCargaExcel, p As PlantillaExcel) As InicioRecepcionExcel
        ReglasCargaExcel.Autorizar(c)
        Dim inicio As New InicioRecepcionExcel With {.IdCarga = Guid.NewGuid().ToString("D"), .ExpiraUtc = _ahora().AddMinutes(30), .RevisionPlantilla = p.REVISION_CARGA, .PlantillaHash = ServicioPlantillasExcel.Huella(p), .Formato = p.Formato}
        inicio.ReferenciaRecepcion = Convert.ToBase64String(_proteger(Encoding.UTF8.GetBytes(Documento(c, p, inicio))))
        Return inicio
    End Function
    Public Sub Validar(c As ContextoCargaExcel, p As PlantillaExcel, solicitud As SolicitudRecepcionExcel)
        ReglasCargaExcel.Autorizar(c)
        Try
            If solicitud Is Nothing OrElse Not ReglasCargaExcel.IdValido(solicitud.IdCarga) OrElse solicitud.ReferenciaRecepcion Is Nothing OrElse solicitud.ReferenciaRecepcion.Length > 8192 Then Throw New FormatException()
            Dim bytes = _abrir(Convert.FromBase64String(solicitud.ReferenciaRecepcion))
            If bytes Is Nothing Then Throw New FormatException()
            Dim doc = JsonConvert.DeserializeObject(Of ReferenciaInterna)(New UTF8Encoding(False, True).GetString(bytes))
            If doc Is Nothing OrElse doc.ExpiraUtc <= _ahora() OrElse doc.ExpiraUtc > _ahora().AddMinutes(30) Then Throw New FormatException()
            Dim inicio As New InicioRecepcionExcel With {.IdCarga = solicitud.IdCarga, .ExpiraUtc = doc.ExpiraUtc, .RevisionPlantilla = p.REVISION_CARGA, .PlantillaHash = ServicioPlantillasExcel.Huella(p), .Formato = p.Formato}
            If Not String.Equals(New UTF8Encoding(False, True).GetString(bytes), Documento(c, p, inicio), StringComparison.Ordinal) Then Throw New FormatException()
        Catch
            Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
        End Try
    End Sub
    Private Shared Function Documento(c As ContextoCargaExcel, p As PlantillaExcel, i As InicioRecepcionExcel) As String
        Return ReglasCargaExcel.Canonico(New ReferenciaInterna With {.Version = 1, .Id = i.IdCarga, .Usuario = c.UsuarioId, .Empresa = c.EmpresaId, .Modulo = c.ModuloId, .Conexion = c.ConexionId, .Identidad = c.IdentidadConexion, .Plantilla = p.ID_PLANTILLAIMP, .Revision = i.RevisionPlantilla, .Huella = i.PlantillaHash, .Formato = i.Formato, .ExpiraUtc = i.ExpiraUtc})
    End Function
    Public Class ReferenciaInterna
        Public Property Version As Integer
        Public Property Id As String
        Public Property Usuario As UInteger
        Public Property Empresa As UInteger
        Public Property Modulo As UInteger
        Public Property Conexion As UInteger
        Public Property Identidad As String
        Public Property Plantilla As UInteger
        Public Property Revision As UInteger
        Public Property Huella As String
        Public Property Formato As String
        Public Property ExpiraUtc As DateTime
    End Class
End Class
