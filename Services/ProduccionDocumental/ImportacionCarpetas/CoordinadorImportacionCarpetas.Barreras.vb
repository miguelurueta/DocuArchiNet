Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

'Barrera de metadatos de recepcion; no valida disco ni invoca escritores.
Public Partial Class CoordinadorImportacionCarpetas
    Friend Shared Function EvaluarBarreraCreacion(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, raiz As RaizImportacionCarpetas, raices As List(Of RaizImportacionCarpetas), archivos As List(Of ArchivoImportacionCarpetas)) As Boolean
        Return EvaluarBarreraRecepcion(lote, plan, raiz, raices, archivos, Nothing, Nothing)
    End Function

    Friend Shared Function EvaluarBarreraAlmacenamiento(vista As VistaBarreraCreacionImportacion, objetivo As ArchivoImportacionCarpetas, Optional intentoPropio As String = Nothing) As Boolean
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse objetivo Is Nothing OrElse Not objetivo.ValoresEfectivosFijados Then Return False
        ValidarVistaValores(vista.Lote.Contexto, New VistaValoresImportacion With {.Lote = vista.Lote, .Raiz = vista.Raiz, .Archivo = objetivo})
        Return EvaluarBarreraRecepcion(vista.Lote, vista.Plan, vista.Raiz, vista.Raices, vista.Archivos, objetivo, intentoPropio)
    End Function

    Private Shared Function EvaluarBarreraRecepcion(lote As LoteImportacionCarpetas, plan As PlanImportacionCarpetas, raiz As RaizImportacionCarpetas, raices As List(Of RaizImportacionCarpetas), archivos As List(Of ArchivoImportacionCarpetas), objetivo As ArchivoImportacionCarpetas, intentoPropio As String) As Boolean
        If lote Is Nothing OrElse plan Is Nothing OrElse raiz Is Nothing OrElse raices Is Nothing OrElse archivos Is Nothing OrElse lote.Contexto Is Nothing OrElse lote.Snapshot Is Nothing OrElse plan.Contexto Is Nothing OrElse plan.Manifiesto Is Nothing OrElse plan.Resultado Is Nothing OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing OrElse plan.Manifiesto.Raices Is Nothing OrElse plan.Manifiesto.Archivos Is Nothing OrElse plan.Resultado.UnidadesElegibles Is Nothing OrElse plan.Resultado.Incidencias Is Nothing Then Throw BarreraInvalida()
        If Not ValidadorPerfilImportacion.ReferenciaValida(lote.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(plan.PlanId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(raiz.RaizId) OrElse String.IsNullOrEmpty(raiz.ClienteRaizId) Then Return False
        If lote.DetencionSolicitada OrElse Not plan.Resultado.PuedeConfirmar OrElse Not {"RECIBIENDO", "LISTO", "EN_CURSO", "PARCIAL"}.Contains(lote.Estado) OrElse plan.Estado <> "CONFIRMADO" OrElse lote.PlanId <> plan.PlanId OrElse raiz.LoteId <> lote.LoteId Then Return False
        If objetivo Is Nothing Then
            If Not {"PENDIENTE", "CREANDO"}.Contains(raiz.Estado) OrElse raiz.IdentidadExpediente IsNot Nothing Then Return False
        Else
            If raiz.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse raiz.IdentidadExpediente Is Nothing OrElse objetivo.RaizId <> raiz.RaizId Then Return False
        End If
        If PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(plan.Snapshot) OrElse PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(plan.Contexto) Then Return False
        Dim c = plan.Snapshot.Configuracion
        Dim parcial = c.ModoIntegridad = "PARCIAL_PERMITIDO" AndAlso c.AmbitoIntegridad = "NO_APLICA"
        Dim completo = c.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso {"EXPEDIENTE", "LOTE"}.Contains(c.AmbitoIntegridad)
        If Not parcial AndAlso Not completo Then Throw BarreraInvalida()
        Dim ambitoLote = completo AndAlso c.AmbitoIntegridad = "LOTE"
        If plan.Manifiesto.Raices.Count = 0 OrElse lote.TotalRaices <> CUInt(plan.Manifiesto.Raices.Count) OrElse lote.TotalArchivos <> CUInt(plan.Manifiesto.Archivos.Count) Then Return False
        Dim originales As New Dictionary(Of String, RaizManifiestoImportacion)(StringComparer.Ordinal)
        For Each original In plan.Manifiesto.Raices
            If original Is Nothing OrElse String.IsNullOrEmpty(original.IdCliente) OrElse originales.ContainsKey(original.IdCliente) Then Throw BarreraInvalida()
            originales.Add(original.IdCliente, original)
        Next
        If Not originales.ContainsKey(raiz.ClienteRaizId) OrElse Not plan.Resultado.UnidadesElegibles.Contains(raiz.ClienteRaizId) Then Return False
        If plan.Resultado.Incidencias.Any(Function(i) i Is Nothing) Then Throw BarreraInvalida()
        If plan.Resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso String.IsNullOrEmpty(i.RaizIdCliente)) Then Return False
        Dim esperados As New Dictionary(Of String, ArchivoManifiestoImportacion)(StringComparer.Ordinal)
        Dim total As Decimal = 0D
        For Each archivo In plan.Manifiesto.Archivos
            If archivo Is Nothing OrElse String.IsNullOrEmpty(archivo.RaizIdCliente) OrElse Not originales.ContainsKey(archivo.RaizIdCliente) OrElse String.IsNullOrEmpty(archivo.IdCliente) Then Throw BarreraInvalida()
            Dim bytes As ULong
            If Not ULong.TryParse(archivo.Bytes, NumberStyles.None, CultureInfo.InvariantCulture, bytes) OrElse bytes.ToString(CultureInfo.InvariantCulture) <> archivo.Bytes Then Throw BarreraInvalida()
            total += CDec(bytes)
            If total > CDec(ULong.MaxValue) Then Throw BarreraInvalida()
            Dim clave = archivo.RaizIdCliente & ":" & archivo.IdCliente
            If esperados.ContainsKey(clave) Then Throw BarreraInvalida()
            esperados.Add(clave, archivo)
        Next
        If total.ToString(CultureInfo.InvariantCulture) <> lote.TotalBytes Then Return False
        Dim ambito = originales.Keys.Where(Function(id) ambitoLote OrElse id = raiz.ClienteRaizId).ToList()
        If raices.Count <> ambito.Count Then Return False
        Dim porId As New Dictionary(Of String, RaizImportacionCarpetas)(StringComparer.Ordinal)
        Dim clientes As New HashSet(Of String)(StringComparer.Ordinal)
        For Each actual In raices
            If actual Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(actual.RaizId) OrElse actual.LoteId <> lote.LoteId OrElse Not ambito.Contains(actual.ClienteRaizId) OrElse Not clientes.Add(actual.ClienteRaizId) OrElse porId.ContainsKey(actual.RaizId) Then Return False
            If Not {"PENDIENTE", "CREANDO", "EXPEDIENTE_CONFIRMADO", "COMPLETADA"}.Contains(actual.Estado) OrElse (actual.Estado = "CREANDO" AndAlso actual.RaizId <> raiz.RaizId) Then Return False
            If ambitoLote AndAlso Not plan.Resultado.UnidadesElegibles.Contains(actual.ClienteRaizId) Then Return False
            porId.Add(actual.RaizId, actual)
        Next
        If Not porId.ContainsKey(raiz.RaizId) OrElse porId(raiz.RaizId).ClienteRaizId <> raiz.ClienteRaizId OrElse porId(raiz.RaizId).Estado <> raiz.Estado Then Return False
        Dim previstos = esperados.Where(Function(par) ambito.Contains(par.Value.RaizIdCliente)).ToDictionary(Function(par) par.Key, Function(par) par.Value, StringComparer.Ordinal)
        If previstos.Count = 0 OrElse archivos.Count <> previstos.Count Then Return False
        Dim vistos As New HashSet(Of String)(StringComparer.Ordinal), ids As New HashSet(Of String)(StringComparer.Ordinal)
        Dim listos = 0
        For Each archivo In archivos
            If archivo Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.ArchivoId) OrElse Not ids.Add(archivo.ArchivoId) OrElse Not porId.ContainsKey(archivo.RaizId) OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(archivo.EstadoEfecto) Then Return False
            Dim clave = porId(archivo.RaizId).ClienteRaizId & ":" & archivo.ClienteArchivoId
            If Not previstos.ContainsKey(clave) OrElse Not vistos.Add(clave) OrElse archivo.BytesPrevistos <> previstos(clave).Bytes Then Return False
            Dim elegible = ServicioPreparacionImportacion.EsArchivoElegible(plan, previstos(clave))
            Dim propio = objetivo IsNot Nothing AndAlso archivo.ArchivoId = objetivo.ArchivoId
            If objetivo Is Nothing Then
                If archivo.RaizId = raiz.RaizId AndAlso archivo.EstadoEfecto <> "NO_INICIADO" AndAlso (elegible OrElse archivo.EstadoEfecto <> "RECHAZADO") Then Return False
            Else
                If propio Then
                    If Not elegible OrElse archivo.RaizId <> objetivo.RaizId OrElse archivo.ClienteArchivoId <> objetivo.ClienteArchivoId OrElse archivo.Revision <> objetivo.Revision OrElse archivo.BytesPrevistos <> objetivo.BytesPrevistos OrElse archivo.BytesRecibidos <> objetivo.BytesRecibidos OrElse archivo.ContenidoHash <> objetivo.ContenidoHash OrElse archivo.StagingRef <> objetivo.StagingRef OrElse archivo.EstadoEfecto <> objetivo.EstadoEfecto OrElse archivo.IntentoActivoId <> objetivo.IntentoActivoId Then Return False
                    If intentoPropio Is Nothing Then
                        If archivo.EstadoEfecto <> "NO_INICIADO" OrElse archivo.IntentoActivoId IsNot Nothing Then Return False
                    ElseIf archivo.EstadoEfecto <> "EN_CURSO" OrElse archivo.IntentoActivoId <> intentoPropio Then
                        Return False
                    End If
                ElseIf completo AndAlso Not {"NO_INICIADO", "COMPLETADO"}.Contains(archivo.EstadoEfecto) Then
                    Return False
                End If
            End If
            Dim recibido = RecepcionRegistradaValida(archivo, If(propio, intentoPropio, Nothing))
            If propio AndAlso Not recibido Then Return False
            If completo AndAlso (Not elegible OrElse Not recibido) Then Return False
            If elegible AndAlso recibido Then listos += 1
        Next
        Return listos > 0 AndAlso (objetivo Is Nothing OrElse ids.Contains(objetivo.ArchivoId))
    End Function

    Friend Shared Function RecepcionRegistradaValida(archivo As ArchivoImportacionCarpetas, Optional intentoPropio As String = Nothing) As Boolean
        If archivo Is Nothing OrElse archivo.EstadoRecepcion <> "RECIBIDO" OrElse archivo.BytesRecibidos <> archivo.BytesPrevistos OrElse archivo.BytesPrevistos = "0" OrElse Not HexBarrera(archivo.ContenidoHash, 64) OrElse Not HexBarrera(archivo.StagingRef, 32) Then Return False
        If intentoPropio IsNot Nothing Then Return ValidadorPerfilImportacion.ReferenciaValida(intentoPropio) AndAlso archivo.IntentoActivoId = intentoPropio AndAlso archivo.EstadoEfecto = "EN_CURSO"
        Return archivo.IntentoActivoId Is Nothing AndAlso Not {"EN_CURSO", "INCIERTO"}.Contains(archivo.EstadoEfecto)
    End Function

    Private Shared Function HexBarrera(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function
    Private Shared Function BarreraInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("CONFIGURACION_INVALIDA", "No se pudo comprobar el inventario y la política de integridad confirmados.")
    End Function
End Class
