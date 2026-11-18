Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

'Prueba de datos originales emitida una sola vez por el servidor. No es permiso,
'reserva, prueba del disco ni resultado documental. No contiene secretos.
Public Class CabeceraControlArchivosImportacion
    Public Property Version As Integer = 1
    Public Property LoteId As String
    Public Property PlanId As String
    Public Property PlanHash As String
    Public Property SnapshotHash As String
    Public Property SnapshotDocumentoHash As String
    Public Property ContextoHash As String
    Public Property TotalArchivos As Integer
    Public Property TotalRaices As Integer
    Public Property TotalBytes As String
    Public Property InventarioHash As String
End Class

Public Class PruebaControlArchivoImportacion
    Public Property Version As Integer = 1
    Public Property ArchivoId As String
    Public Property Indice As Integer
    Public Property Hermanos As New List(Of String)()
End Class

Public Class ControlArchivosSelladoImportacion
    Public Property Cabecera As CabeceraControlArchivosImportacion
    Public Property Pruebas As New Dictionary(Of String, PruebaControlArchivoImportacion)(StringComparer.Ordinal)
End Class

Public Interface IControlArchivosIndependientesImportacion
    Sub PrepararControlArchivos(contexto As ContextoImportacionCarpetas, loteId As String)
    Sub RegistrarOriginalNoDisponible(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, revision As UInteger, causa As String, Optional intento As IntentoImportacionCarpetas = Nothing)
End Interface

Public Class ArchivoNoDisponibleImportacionException
    Inherits PerfilImportacionException
    Public ReadOnly Property LoteId As String
    Public ReadOnly Property ArchivoId As String
    Public ReadOnly Property RaizId As String
    Public ReadOnly Property Fase As String
    Public ReadOnly Property Causa As String
    Public Sub New(lote As String, archivo As String, raiz As String, faseActual As String, causaActual As String)
        MyBase.New("ARCHIVO_NO_DISPONIBLE", "No se pudo comprobar el original de este archivo; decide si continuar con los demás o pausar para revisarlo.")
        LoteId = lote : ArchivoId = archivo : RaizId = raiz : Fase = faseActual : Causa = causaActual
    End Sub
End Class

Public Class OriginalNoComprobadoImportacionException
    Inherits PerfilImportacionException
    Public ReadOnly Property Motivo As String
    Public Sub New(causa As String)
        MyBase.New("RESULTADO_INCIERTO", "No se pudo verificar el original retenido.")
        Motivo = causa
    End Sub
End Class

Public NotInheritable Class ControlArchivoIndependienteImportacion
    Public Const AccionCabecera As String = "SELLAR_CONTROL_ARCHIVOS"
    Public Const AccionPrueba As String = "SELLAR_CONTROL_ARCHIVO"
    Public Const AccionIdentidad As String = "FIJAR_IDENTIDAD_DOCUMENTAL"
    Public Const CodigoNoDisponible As String = "ORIGINAL_NO_DISPONIBLE"
    Private Sub New()
    End Sub
    Public Shared Function Hash(texto As String) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(texto))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Public Shared Function EsHash(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = 64 AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function
    Public Shared Function Sellar(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, raices As List(Of RaizImportacionCarpetas), archivos As List(Of ArchivoImportacionCarpetas)) As ControlArchivosSelladoImportacion
        If lote Is Nothing OrElse plan Is Nothing OrElse raices Is Nothing OrElse archivos Is Nothing OrElse archivos.Count = 0 OrElse archivos.Count <> lote.TotalArchivos OrElse raices.Count <> lote.TotalRaices OrElse plan.Estado <> "CONFIRMADO" OrElse plan.PlanId <> lote.PlanId OrElse Not EsHash(plan.Huella) Then Throw Invalido()
        Dim porRaiz = raices.ToDictionary(Function(r) r.RaizId, StringComparer.Ordinal)
        Dim orden = archivos.OrderBy(Function(a) UInteger.Parse(a.ArchivoId, CultureInfo.InvariantCulture)).ToList()
        If orden.Select(Function(a) a.ArchivoId).Distinct(StringComparer.Ordinal).Count() <> orden.Count Then Throw Invalido()
        Dim cabecera As New CabeceraControlArchivosImportacion With {.LoteId = lote.LoteId, .PlanId = lote.PlanId, .PlanHash = plan.Huella, .SnapshotHash = lote.Snapshot.Huella, .SnapshotDocumentoHash = Hash(PerfilImportacionJson.Escribir(lote.Snapshot)), .ContextoHash = Hash(PerfilImportacionJson.Escribir(lote.Contexto)), .TotalArchivos = archivos.Count, .TotalRaices = raices.Count, .TotalBytes = lote.TotalBytes}
        Dim niveles As New List(Of List(Of String)) From {orden.Select(Function(a) Hoja(cabecera, porRaiz(a.RaizId), a)).ToList()}
        While niveles.Last().Count > 1
            Dim anterior = niveles.Last(), siguiente As New List(Of String)()
            For i = 0 To anterior.Count - 1 Step 2
                siguiente.Add(Hash("N:" & anterior(i) & ":" & anterior(Math.Min(i + 1, anterior.Count - 1))))
            Next
            niveles.Add(siguiente)
        End While
        cabecera.InventarioHash = niveles.Last()(0)
        Dim salida As New ControlArchivosSelladoImportacion With {.Cabecera = cabecera}
        For i = 0 To orden.Count - 1
            Dim prueba As New PruebaControlArchivoImportacion With {.ArchivoId = orden(i).ArchivoId, .Indice = i}, indice = i
            For nivel = 0 To niveles.Count - 2
                Dim hermano = If(indice Mod 2 = 0, Math.Min(indice + 1, niveles(nivel).Count - 1), indice - 1)
                prueba.Hermanos.Add(niveles(nivel)(hermano)) : indice \= 2
            Next
            salida.Pruebas.Add(prueba.ArchivoId, prueba)
        Next
        Return salida
    End Function
    Public Shared Sub Verificar(lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, cabecera As CabeceraControlArchivosImportacion, prueba As PruebaControlArchivoImportacion)
        If lote Is Nothing OrElse lote.Snapshot Is Nothing OrElse lote.Contexto Is Nothing Then Throw Invalido()
        If lote Is Nothing OrElse raiz Is Nothing OrElse archivo Is Nothing OrElse cabecera Is Nothing OrElse prueba Is Nothing OrElse cabecera.Version <> 1 OrElse prueba.Version <> 1 OrElse cabecera.LoteId <> lote.LoteId OrElse cabecera.PlanId <> lote.PlanId OrElse cabecera.TotalArchivos <> lote.TotalArchivos OrElse cabecera.TotalRaices <> lote.TotalRaices OrElse cabecera.SnapshotHash <> lote.Snapshot.Huella OrElse cabecera.ContextoHash <> Hash(PerfilImportacionJson.Escribir(lote.Contexto)) OrElse Not EsHash(cabecera.PlanHash) OrElse Not EsHash(cabecera.InventarioHash) OrElse prueba.ArchivoId <> archivo.ArchivoId OrElse prueba.Indice < 0 OrElse prueba.Indice >= cabecera.TotalArchivos OrElse prueba.Hermanos Is Nothing Then Throw Invalido()
        If cabecera.TotalBytes <> lote.TotalBytes OrElse cabecera.SnapshotDocumentoHash <> Hash(PerfilImportacionJson.Escribir(lote.Snapshot)) Then Throw Invalido()
        Dim actual = Hoja(cabecera, raiz, archivo), indice = prueba.Indice, cantidad = cabecera.TotalArchivos, nivel = 0
        While cantidad > 1
            If nivel >= prueba.Hermanos.Count OrElse Not EsHash(prueba.Hermanos(nivel)) Then Throw Invalido()
            Dim hermano = prueba.Hermanos(nivel)
            If indice Mod 2 = 0 AndAlso indice + 1 >= cantidad AndAlso hermano <> actual Then Throw Invalido()
            actual = If(indice Mod 2 = 0, Hash("N:" & actual & ":" & hermano), Hash("N:" & hermano & ":" & actual))
            indice \= 2 : cantidad = (cantidad + 1) \ 2 : nivel += 1
        End While
        If nivel <> prueba.Hermanos.Count OrElse actual <> cabecera.InventarioHash Then Throw Invalido()
    End Sub
    Private Shared Function Hoja(c As CabeceraControlArchivosImportacion, r As RaizImportacionCarpetas, a As ArchivoImportacionCarpetas) As String
        If r Is Nothing OrElse a Is Nothing OrElse r.LoteId <> c.LoteId OrElse a.RaizId <> r.RaizId OrElse Not ValidadorPerfilImportacion.ReferenciaValida(a.ArchivoId) OrElse r.ValoresPlan Is Nothing OrElse a.ValoresPlan Is Nothing Then Throw Invalido()
        Return Hash("A:" & PerfilImportacionJson.Escribir(New With {.version = 1, .lote = c.LoteId, .plan = c.PlanId, .planHash = c.PlanHash, .snapshot = c.SnapshotHash, .contexto = c.ContextoHash,
            .snapshotDocumento = c.SnapshotDocumentoHash, .totalBytes = c.TotalBytes, .raiz = r.RaizId, .clienteRaiz = r.ClienteRaizId, .nombreRaiz = r.NombreOriginal, .valoresRaiz = r.ValoresPlan,
            .archivo = a.ArchivoId, .clienteArchivo = a.ClienteArchivoId, .nombre = a.NombreOriginal, .ruta = a.RutaRelativa, .rutaHash = a.RutaHash, .bytes = a.BytesPrevistos, .modificado = a.UltimaModificacionLocal, .valoresArchivo = a.ValoresPlan}))
    End Function
    Public Shared Function EsNoDisponible(resultado As ResultadoEfectoImportacion) As Boolean
        Return resultado IsNot Nothing AndAlso resultado.Estado = "RECHAZADO" AndAlso resultado.Codigo = CodigoNoDisponible AndAlso resultado.DocumentoId Is Nothing AndAlso resultado.InventarioId Is Nothing AndAlso resultado.Creacion Is Nothing AndAlso resultado.Recepcion Is Nothing AndAlso resultado.OrigenGabinete Is Nothing AndAlso resultado.GabineteId Is Nothing
    End Function
    Public Shared Function Invalido() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "No se pudo comprobar la procedencia del archivo confirmado.")
    End Function
End Class
