Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

'Solo metadatos. Ninguna ruta recibida se convierte en ubicación física.
Public NotInheritable Class CanonizadorManifiestoImportacion
    Private Sub New()
    End Sub

    Public Shared Function RutaCanonica(archivo As ArchivoManifiestoImportacion, raiz As RaizManifiestoImportacion) As String
        If archivo Is Nothing OrElse raiz Is Nothing Then Throw Invalido()
        Dim ruta = archivo.RutaRelativa
        If String.IsNullOrEmpty(ruta) OrElse ruta.Any(Function(c) Char.IsControl(c) OrElse c = "\"c OrElse c = ":"c) Then Throw Invalido()
        Dim partes = ruta.Split("/"c)
        If partes.Length < 2 OrElse partes.Any(Function(p) p = "" OrElse p = "." OrElse p = "..") OrElse partes(0) <> raiz.Nombre OrElse partes.Last() <> archivo.Nombre Then Throw Invalido()
        Try
            Return ruta.Normalize(NormalizationForm.FormC)
        Catch ex As ArgumentException
            Throw Invalido()
        End Try
    End Function

    Public Shared Function Canonizar(original As ManifiestoImportacionCarpetas, limites As LimitesImportacionCarpetas) As ManifiestoImportacionCarpetas
        LimitesImportacionCarpetas.Exigir(limites)
        If original Is Nothing OrElse original.Raices Is Nothing OrElse original.Archivos Is Nothing OrElse original.Raices.Count = 0 OrElse original.Archivos.Count = 0 Then Throw Invalido()
        If original.Raices.Count > limites.MaximoRaices OrElse original.Archivos.Count > limites.MaximoArchivos Then Throw Excedido()
        Dim raices As New Dictionary(Of String, RaizManifiestoImportacion)(StringComparer.Ordinal)
        Dim selecciones As New HashSet(Of String)(StringComparer.Ordinal)
        Dim archivos As New HashSet(Of String)(StringComparer.Ordinal)
        Dim rutas As New HashSet(Of String)(StringComparer.Ordinal)
        Dim directorios As New HashSet(Of String)(StringComparer.Ordinal)
        Dim pobladas As New HashSet(Of String)(StringComparer.Ordinal)
        For Each raiz In original.Raices
            If raiz Is Nothing OrElse Not Correlacion(raiz.IdCliente) OrElse Not Correlacion(raiz.SeleccionId) OrElse String.IsNullOrWhiteSpace(raiz.Nombre) OrElse raices.ContainsKey(raiz.IdCliente) OrElse Not selecciones.Add(raiz.SeleccionId) Then Throw Invalido()
            raices.Add(raiz.IdCliente, raiz)
        Next
        Dim total As ULong = 0UL
        For Each archivo In original.Archivos
            If archivo Is Nothing OrElse Not Correlacion(archivo.IdCliente) OrElse archivo.RaizIdCliente Is Nothing OrElse Not raices.ContainsKey(archivo.RaizIdCliente) OrElse Not archivos.Add(archivo.RaizIdCliente & ":" & archivo.IdCliente) Then Throw Invalido()
            Dim ruta = RutaCanonica(archivo, raices(archivo.RaizIdCliente))
            Dim clave = archivo.RaizIdCliente & ":" & ruta
            If Not rutas.Add(clave) Then Throw Invalido()
            Dim partes = ruta.Split("/"c)
            For i = 1 To partes.Length - 1
                directorios.Add(archivo.RaizIdCliente & ":" & String.Join("/", partes.Take(i)))
            Next
            Dim bytes = DecimalCanonico(archivo.Bytes)
            DecimalCanonico(archivo.UltimaModificacionLocal)
            If bytes > limites.MaximoBytesArchivo OrElse bytes > limites.MaximoBytesLote - total Then Throw Excedido()
            total += bytes
            pobladas.Add(archivo.RaizIdCliente)
        Next
        If pobladas.Count <> raices.Count OrElse rutas.Overlaps(directorios) Then Throw Invalido()
        'El guard limita también el sobre de solicitud real. Aquí se acota su documento persistible.
        Dim json = PerfilImportacionJson.Escribir(original)
        If Encoding.UTF8.GetByteCount(json) > limites.MaximoBytesManifiesto Then Throw Excedido()
        Dim copia = PerfilImportacionJson.Leer(Of ManifiestoImportacionCarpetas)(json)
        copia.Raices = copia.Raices.OrderBy(Function(r) r.IdCliente, StringComparer.Ordinal).ToList()
        copia.Archivos = copia.Archivos.OrderBy(Function(a) a.RaizIdCliente, StringComparer.Ordinal).ThenBy(Function(a) a.IdCliente, StringComparer.Ordinal).ToList()
        'Se conservan los nombres originales; NFC solo determina la clave de colisión.
        Return copia
    End Function

    Public Shared Function EvaluarAgrupacion(manifiesto As ManifiestoImportacionCarpetas, configuracion As ConfiguracionPerfilImportacion) As List(Of IncidenciaImportacionCarpetas)
        If configuracion Is Nothing OrElse configuracion.EstrategiaCarpetas <> "RAIZ_EXPEDIENTE" OrElse Not {"INCLUIR", "RECHAZAR"}.Contains(configuracion.PoliticaSubcarpetas) Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "La estrategia de carpetas no esta implementada.")
        If manifiesto Is Nothing OrElse manifiesto.Raices Is Nothing OrElse manifiesto.Archivos Is Nothing Then Throw Invalido()
        Dim incidencias As New List(Of IncidenciaImportacionCarpetas)()
        If configuracion.PoliticaSubcarpetas = "RECHAZAR" Then
            For Each archivo In manifiesto.Archivos
                If archivo Is Nothing OrElse String.IsNullOrEmpty(archivo.RutaRelativa) Then Throw Invalido()
                If archivo.RutaRelativa.Split("/"c).Length > 2 Then incidencias.Add(New IncidenciaImportacionCarpetas With {.Ambito = "ARCHIVO", .RaizIdCliente = archivo.RaizIdCliente, .ArchivoIdCliente = archivo.IdCliente, .Campo = "rutaRelativa", .Codigo = "SUBCARPETA_NO_PERMITIDA", .Mensaje = "El perfil no admite archivos en subcarpetas.", .BloqueaUnidad = True})
            Next
        End If
        Return incidencias
    End Function

    Private Shared Function Correlacion(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant)
    End Function

    Private Shared Function DecimalCanonico(valor As String) As ULong
        Dim numero As ULong
        If String.IsNullOrEmpty(valor) OrElse Not ULong.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, numero) OrElse numero.ToString(CultureInfo.InvariantCulture) <> valor Then Throw Invalido()
        Return numero
    End Function

    Private Shared Function Invalido() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "El manifiesto esta incompleto o contiene rutas o correlaciones invalidas.")
    End Function

    Private Shared Function Excedido() As PerfilImportacionException
        Return New PerfilImportacionException("LIMITE_EXCEDIDO", "El manifiesto excede la capacidad configurada.")
    End Function
End Class
