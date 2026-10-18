Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

' Selecciona el transporte sin modificar ni copiar el PDF almacenado.
Public NotInheritable Class EstrategiaCargaPdf
    Public Const ModoRangos As String = "range"
    Public Const ModoStreamingCompleto As String = "stream"
    Public Const UmbralRangosBytes As Long = 1048576L
    Private Const BytesInspeccion As Integer = 4096

    Private Sub New()
    End Sub

    Public Shared Function Seleccionar(ByVal ruta As String) As String
        If String.IsNullOrWhiteSpace(ruta) Then Return ModoStreamingCompleto

        Try
            Dim archivo As New FileInfo(ruta)
            If Not archivo.Exists OrElse archivo.Length < UmbralRangosBytes Then Return ModoStreamingCompleto

            Dim cantidad As Integer = CInt(Math.Min(CLng(BytesInspeccion), archivo.Length))
            Dim cabecera(cantidad - 1) As Byte
            Using origen As New FileStream(archivo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                Dim leidos As Integer = 0
                While leidos < cantidad
                    Dim actual As Integer = origen.Read(cabecera, leidos, cantidad - leidos)
                    If actual = 0 Then Exit While
                    leidos += actual
                End While
                If leidos <> cantidad Then Return ModoStreamingCompleto
            End Using

            Return If(EsCabeceraLinearizada(cabecera), ModoRangos, ModoStreamingCompleto)
        Catch
            ' La selección es una optimización; ante duda se conserva el camino más compatible.
            Return ModoStreamingCompleto
        End Try
    End Function

    Public Shared Function EsCandidatoDerivado(ByVal ruta As String) As Boolean
        If String.IsNullOrWhiteSpace(ruta) Then Return False
        Try
            Dim archivo As New FileInfo(ruta)
            If Not archivo.Exists OrElse archivo.Length < UmbralRangosBytes Then Return False
            Dim cantidad As Integer = CInt(Math.Min(CLng(BytesInspeccion), archivo.Length))
            Dim cabecera(cantidad - 1) As Byte
            Using origen As New FileStream(archivo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                If origen.Read(cabecera, 0, cantidad) <> cantidad Then Return False
            End Using
            Return EsCabeceraPdf(cabecera) AndAlso Not EsCabeceraLinearizada(cabecera)
        Catch
            Return False
        End Try
    End Function

    Public Shared Function EsCabeceraPdf(ByVal cabecera As Byte()) As Boolean
        If cabecera Is Nothing OrElse cabecera.Length = 0 Then Return False
        Dim texto As String = Encoding.ASCII.GetString(cabecera)
        Dim inicioPdf As Integer = texto.IndexOf("%PDF-", StringComparison.Ordinal)
        Return inicioPdf >= 0 AndAlso inicioPdf <= 16
    End Function

    Public Shared Function EsCabeceraLinearizada(ByVal cabecera As Byte()) As Boolean
        If Not EsCabeceraPdf(cabecera) Then Return False
        Dim texto As String = Encoding.ASCII.GetString(cabecera)
        Dim inicioPdf As Integer = texto.IndexOf("%PDF-", StringComparison.Ordinal)
        Return texto.IndexOf("/Linearized", inicioPdf, StringComparison.Ordinal) >= 0
    End Function

    Public Shared Function TryObtenerSeccionInicialLinearizada(ByVal cabecera As Byte(),
                                                                ByRef longitudDeclarada As Long,
                                                                ByRef finPrimeraSeccion As Long) As Boolean
        longitudDeclarada = 0L
        finPrimeraSeccion = 0L
        If Not EsCabeceraLinearizada(cabecera) Then Return False
        Dim texto As String = Encoding.ASCII.GetString(cabecera)
        Dim coincidenciaLongitud As Match = Regex.Match(texto, "/L\s+(\d+)", RegexOptions.CultureInvariant)
        Dim coincidenciaFin As Match = Regex.Match(texto, "/E\s+(\d+)", RegexOptions.CultureInvariant)
        Return coincidenciaLongitud.Success AndAlso coincidenciaFin.Success AndAlso
               Long.TryParse(coincidenciaLongitud.Groups(1).Value, longitudDeclarada) AndAlso
               Long.TryParse(coincidenciaFin.Groups(1).Value, finPrimeraSeccion) AndAlso
               longitudDeclarada > 0L AndAlso finPrimeraSeccion > 0L AndAlso finPrimeraSeccion < longitudDeclarada
    End Function
End Class
