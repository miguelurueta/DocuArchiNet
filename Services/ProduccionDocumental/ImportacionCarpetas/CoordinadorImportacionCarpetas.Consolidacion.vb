Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Public Partial Class CoordinadorImportacionCarpetas
    'Calculo puro, sin fuentes, disco, catalogos o efectos. Nunca reconcilia
    'un INCIERTO ni deduce ausencia de efecto por tiempo o falta de respuesta.
    Friend Shared Function EvaluarConsolidacion(vista As VistaConsolidacionImportacion) As ConsolidacionImportacion
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Plan Is Nothing OrElse vista.Raices Is Nothing OrElse vista.Archivos Is Nothing OrElse vista.Intentos Is Nothing Then Throw BarreraInvalida()
        Dim lote = vista.Lote, plan = vista.Plan
        If lote.Contexto Is Nothing OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse plan.Manifiesto Is Nothing OrElse plan.Manifiesto.Raices Is Nothing OrElse plan.Manifiesto.Archivos Is Nothing OrElse plan.Resultado Is Nothing OrElse plan.Resultado.Elementos Is Nothing OrElse plan.Resultado.UnidadesElegibles Is Nothing OrElse plan.Resultado.Incidencias Is Nothing Then Throw BarreraInvalida()
        If Not ValidadorPerfilImportacion.ReferenciaValida(lote.LoteId) OrElse lote.PlanId <> plan.PlanId OrElse plan.Estado <> "CONFIRMADO" OrElse Not plan.Resultado.PuedeConfirmar OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) OrElse lote.Revision = 0 OrElse PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(plan.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(plan.Snapshot) Then Throw BarreraInvalida()
        If Not lote.Contexto.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(lote.Contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(lote.Contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(lote.Contexto.NivelRef) Then Throw BarreraInvalida()
        Dim c = lote.Snapshot.Configuracion
        If Not c.GabineteId.HasValue OrElse c.GabineteId.Value = 0 OrElse c.OrigenGabinete <> "DA" OrElse Not ((c.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso {"EXPEDIENTE", "LOTE"}.Contains(c.AmbitoIntegridad)) OrElse (c.ModoIntegridad = "PARCIAL_PERMITIDO" AndAlso c.AmbitoIntegridad = "NO_APLICA")) Then Throw BarreraInvalida()
        If plan.Manifiesto.Raices.Count = 0 OrElse lote.TotalRaices <> CUInt(plan.Manifiesto.Raices.Count) OrElse lote.TotalArchivos <> CUInt(plan.Manifiesto.Archivos.Count) OrElse vista.Raices.Count <> plan.Manifiesto.Raices.Count OrElse vista.Archivos.Count <> plan.Manifiesto.Archivos.Count Then Throw BarreraInvalida()
        Dim originales As New Dictionary(Of String, RaizManifiestoImportacion)(StringComparer.Ordinal)
        For Each raiz In plan.Manifiesto.Raices
            If raiz Is Nothing OrElse String.IsNullOrEmpty(raiz.IdCliente) OrElse originales.ContainsKey(raiz.IdCliente) Then Throw BarreraInvalida()
            originales.Add(raiz.IdCliente, raiz)
        Next
        If plan.Resultado.UnidadesElegibles.Distinct(StringComparer.Ordinal).Count() <> plan.Resultado.UnidadesElegibles.Count OrElse plan.Resultado.UnidadesElegibles.Any(Function(id) Not originales.ContainsKey(id)) OrElse plan.Resultado.Incidencias.Any(Function(i) i Is Nothing) Then Throw BarreraInvalida()
        Dim raices As New Dictionary(Of String, RaizImportacionCarpetas)(StringComparer.Ordinal), clientes As New HashSet(Of String)(StringComparer.Ordinal)
        For Each raiz In vista.Raices
            If raiz Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(raiz.RaizId) OrElse raiz.LoteId <> lote.LoteId OrElse raiz.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRaiz(raiz.Estado) OrElse Not originales.ContainsKey(raiz.ClienteRaizId) OrElse Not clientes.Add(raiz.ClienteRaizId) OrElse raices.ContainsKey(raiz.RaizId) OrElse raiz.NombreOriginal <> originales(raiz.ClienteRaizId).Nombre Then Throw BarreraInvalida()
            ExigirValoresPlanConsolidacion(plan, "RAIZ", raiz.ClienteRaizId, Nothing, raiz.ValoresPlan)
            raices.Add(raiz.RaizId, raiz)
        Next
        Dim previstos As New Dictionary(Of String, ArchivoManifiestoImportacion)(StringComparer.Ordinal), total As Decimal = 0D
        For Each archivo In plan.Manifiesto.Archivos
            If archivo Is Nothing OrElse Not originales.ContainsKey(archivo.RaizIdCliente) OrElse String.IsNullOrEmpty(archivo.IdCliente) Then Throw BarreraInvalida()
            Dim clave = archivo.RaizIdCliente & ":" & archivo.IdCliente, bytes As ULong
            If previstos.ContainsKey(clave) OrElse Not ULong.TryParse(archivo.Bytes, NumberStyles.None, CultureInfo.InvariantCulture, bytes) OrElse bytes.ToString(CultureInfo.InvariantCulture) <> archivo.Bytes Then Throw BarreraInvalida()
            previstos.Add(clave, archivo) : total += CDec(bytes)
        Next
        If total > CDec(ULong.MaxValue) OrElse total.ToString(CultureInfo.InvariantCulture) <> lote.TotalBytes OrElse plan.Resultado.Elementos.Count <> originales.Count + previstos.Count Then Throw BarreraInvalida()
        Dim archivos As New Dictionary(Of String, ArchivoImportacionCarpetas)(StringComparer.Ordinal), vistos As New HashSet(Of String)(StringComparer.Ordinal)
        For Each archivo In vista.Archivos
            If archivo Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.ArchivoId) OrElse archivos.ContainsKey(archivo.ArchivoId) OrElse Not raices.ContainsKey(archivo.RaizId) OrElse archivo.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(archivo.EstadoEfecto) Then Throw BarreraInvalida()
            Dim cliente = raices(archivo.RaizId).ClienteRaizId, clave = cliente & ":" & archivo.ClienteArchivoId
            If Not previstos.ContainsKey(clave) OrElse Not vistos.Add(clave) Then Throw BarreraInvalida()
            Dim original = previstos(clave)
            If archivo.BytesPrevistos <> original.Bytes OrElse archivo.NombreOriginal <> original.Nombre OrElse archivo.RutaRelativa <> original.RutaRelativa OrElse archivo.UltimaModificacionLocal <> original.UltimaModificacionLocal Then Throw BarreraInvalida()
            ExigirValoresPlanConsolidacion(plan, "ARCHIVO", cliente, archivo.ClienteArchivoId, archivo.ValoresPlan)
            archivos.Add(archivo.ArchivoId, archivo)
        Next
        Dim intentos As New Dictionary(Of String, IntentoImportacionCarpetas)(StringComparer.Ordinal)
        For Each intento In vista.Intentos
            If intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intentos.ContainsKey(intento.IntentoId) OrElse intento.LoteId <> lote.LoteId OrElse Not raices.ContainsKey(intento.RaizId) OrElse Not EstadosImportacionCarpetas.EsIntento(intento.Estado) OrElse Not {"RECEPCION", "CREAR_EXPEDIENTE", "ALMACENAR", "INCORPORAR"}.Contains(intento.Fase) Then Throw BarreraInvalida()
            If intento.Fase = "CREAR_EXPEDIENTE" Then
                If intento.ArchivoId IsNot Nothing Then Throw BarreraInvalida()
            ElseIf intento.ArchivoId Is Nothing OrElse Not archivos.ContainsKey(intento.ArchivoId) OrElse archivos(intento.ArchivoId).RaizId <> intento.RaizId Then
                Throw BarreraInvalida()
            End If
            intentos.Add(intento.IntentoId, intento)
        Next
        Dim salida As New ConsolidacionImportacion With {.LoteId = lote.LoteId, .Revision = lote.Revision, .EstadoOriginal = lote.Estado, .Estado = lote.Estado}
        Dim conIds = vista.Archivos.Where(Function(a) a.EstadoEfecto = "COMPLETADO" AndAlso a.Resultado IsNot Nothing AndAlso ValidadorPerfilImportacion.ReferenciaValida(a.Resultado.DocumentoId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(a.Resultado.InventarioId)).ToList()
        Dim documentosDuplicados = conIds.GroupBy(Function(a) a.Resultado.OrigenGabinete & ":" & a.Resultado.GabineteId & ":" & a.Resultado.DocumentoId, StringComparer.Ordinal).Where(Function(g) g.Count() > 1).SelectMany(Function(g) g).Select(Function(a) a.ArchivoId).ToList()
        Dim inventariosDuplicados = conIds.GroupBy(Function(a) a.Resultado.InventarioId, StringComparer.Ordinal).Where(Function(g) g.Count() > 1).SelectMany(Function(g) g).Select(Function(a) a.ArchivoId).ToList()
        salida.HayIntentosActivos = vista.Intentos.Any(Function(i) {"RESERVADO", "EN_CURSO"}.Contains(i.Estado))
        salida.HayIncertidumbre = vista.Intentos.Any(Function(i) i.Estado = "INCIERTO")
        Dim automaticos As Boolean = False, recuperables As Boolean = False, recibidos As Boolean = True, progreso As Boolean = False
        For Each raiz In vista.Raices
            Dim propios = vista.Archivos.Where(Function(a) a.RaizId = raiz.RaizId).ToList()
            If propios.Count = 0 Then Throw BarreraInvalida()
            Dim elegible = plan.Resultado.UnidadesElegibles.Contains(raiz.ClienteRaizId)
            Dim creada = CreacionConfirmadaConsolidacion(lote, raiz, vista.Intentos)
            If creada Then salida.RaicesConfirmadas += 1UI : progreso = True
            Dim incierta = raiz.Estado = "INCIERTO" OrElse (raiz.IdentidadExpediente IsNot Nothing AndAlso Not creada) OrElse (raiz.Estado = "CREANDO" AndAlso raiz.IntentoActivoId Is Nothing)
            If propios.Any(Function(a) documentosDuplicados.Contains(a.ArchivoId) OrElse inventariosDuplicados.Contains(a.ArchivoId)) Then incierta = True
            Dim intentosPropios = vista.Intentos.Where(Function(i) i.RaizId = raiz.RaizId).ToList()
            If intentosPropios.Any(Function(i) i.Estado = "INCIERTO") Then incierta = True
            For Each activo In intentosPropios.Where(Function(i) {"RESERVADO", "EN_CURSO"}.Contains(i.Estado))
                If activo.ArchivoId Is Nothing Then
                    If raiz.IntentoActivoId <> activo.IntentoId Then incierta = True
                ElseIf archivos(activo.ArchivoId).IntentoActivoId <> activo.IntentoId Then
                    incierta = True
                End If
            Next
            If raiz.IntentoActivoId IsNot Nothing Then
                salida.HayIntentosActivos = True
                If Not MarcadorConsolidacion(intentos, raiz.IntentoActivoId, raiz.RaizId, Nothing) Then incierta = True
            End If
            Dim confirmados = 0, pendientesAutomaticos As Boolean = False, rechazo As Boolean = Not elegible
            For Each archivo In propios
                Dim original = previstos(raiz.ClienteRaizId & ":" & archivo.ClienteArchivoId)
                Dim permitido = elegible AndAlso ServicioPreparacionImportacion.EsArchivoElegible(plan, original) AndAlso raiz.Estado <> "CANCELADA"
                Dim confirmado = permitido AndAlso creada AndAlso DocumentoConfirmadoConsolidacion(lote, raiz, archivo, vista.Intentos)
                If confirmado Then
                    confirmados += 1 : salida.ArchivosConfirmados += 1UI : progreso = True
                Else
                    salida.HayPendientes = True
                    If archivo.EstadoEfecto = "COMPLETADO" Then incierta = True
                    If permitido AndAlso archivo.EstadoEfecto <> "CANCELADO" AndAlso archivo.EstadoRecepcion <> "CANCELADO" Then
                        recuperables = True
                        If archivo.EstadoEfecto <> "RECHAZADO" Then pendientesAutomaticos = True
                    End If
                End If
                If archivo.EstadoRecepcion = "INCIERTO" OrElse archivo.EstadoEfecto = "INCIERTO" Then incierta = True
                If archivo.IntentoActivoId IsNot Nothing Then
                    salida.HayIntentosActivos = True
                    If Not MarcadorConsolidacion(intentos, archivo.IntentoActivoId, raiz.RaizId, archivo.ArchivoId) Then incierta = True
                End If
                If archivo.EstadoRecepcion = "RECIBIENDO" OrElse archivo.EstadoEfecto = "EN_CURSO" Then
                    salida.HayIntentosActivos = True
                    If archivo.IntentoActivoId Is Nothing Then incierta = True
                End If
                If {"RECHAZADO", "CANCELADO"}.Contains(archivo.EstadoRecepcion) OrElse {"RECHAZADO", "CANCELADO"}.Contains(archivo.EstadoEfecto) OrElse Not permitido Then rechazo = True
                If permitido AndAlso Not RecepcionRegistradaValida(archivo) Then recibidos = False
            Next
            automaticos = automaticos OrElse pendientesAutomaticos
            salida.HayRechazados = salida.HayRechazados OrElse rechazo
            salida.HayIncertidumbre = salida.HayIncertidumbre OrElse incierta
            Dim estado = raiz.Estado
            If raiz.Estado = "COMPLETADA" AndAlso (Not creada OrElse confirmados <> propios.Count OrElse incierta) Then Throw BarreraInvalida()
            If incierta Then
                If {"CREANDO", "EXPEDIENTE_CONFIRMADO", "PARCIAL"}.Contains(estado) Then estado = "INCIERTO"
            ElseIf creada AndAlso confirmados = propios.Count AndAlso raiz.IntentoActivoId Is Nothing Then
                estado = "COMPLETADA"
            ElseIf creada AndAlso confirmados > 0 AndAlso rechazo AndAlso Not pendientesAutomaticos AndAlso estado = "EXPEDIENTE_CONFIRMADO" Then
                estado = "PARCIAL"
            ElseIf Not elegible AndAlso estado = "PENDIENTE" AndAlso raiz.IntentoActivoId Is Nothing Then
                estado = "BLOQUEADO"
            End If
            If estado <> raiz.Estado Then ValidarTransicion("RAIZ", raiz.Estado, estado)
            salida.Raices.Add(New EstadoRaizConsolidadaImportacion With {.RaizId = raiz.RaizId, .Revision = raiz.Revision, .EstadoOriginal = raiz.Estado, .Estado = estado})
        Next
        Dim completo = salida.ArchivosConfirmados = lote.TotalArchivos AndAlso salida.Raices.All(Function(r) r.Estado = "COMPLETADA")
        If lote.Estado = "PENDIENTE_VERIFICACION" AndAlso Not completo Then salida.HayIncertidumbre = True
        If salida.HayIncertidumbre Then
            salida.Estado = "PENDIENTE_VERIFICACION"
        ElseIf salida.HayIntentosActivos Then
            salida.Estado = "EN_CURSO"
        ElseIf completo Then
            salida.Estado = "COMPLETADO"
        ElseIf lote.DetencionSolicitada OrElse lote.Estado = "DETENIDO" Then
            salida.Estado = "DETENIDO"
        ElseIf lote.Estado = "PENDIENTE_VERIFICACION" Then
            salida.Estado = lote.Estado
        ElseIf progreso AndAlso salida.HayRechazados AndAlso Not automaticos Then
            salida.Estado = "PARCIAL"
        ElseIf Not recuperables AndAlso Not progreso Then
            salida.Estado = "FALLIDO"
        ElseIf progreso Then
            salida.Estado = "EN_CURSO"
        Else
            salida.Estado = If(recibidos, "LISTO", "RECIBIENDO")
        End If
        If salida.Estado <> lote.Estado Then ValidarTransicion("LOTE", lote.Estado, salida.Estado)
        Return salida
    End Function

    Private Shared Sub ExigirValoresPlanConsolidacion(plan As PlanImportacionCarpetas, ambito As String, raiz As String, archivo As String, valores As List(Of ValorReglaPerfilImportacion))
        Dim elementos = plan.Resultado.Elementos.Where(Function(e) e IsNot Nothing AndAlso e.Ambito = ambito AndAlso e.RaizIdCliente = raiz AndAlso e.ArchivoIdCliente = archivo).ToList()
        If valores Is Nothing OrElse elementos.Count <> 1 OrElse PerfilImportacionJson.Escribir(elementos(0).Valores) <> PerfilImportacionJson.Escribir(valores) Then Throw BarreraInvalida()
    End Sub
    Private Shared Function MarcadorConsolidacion(intentos As Dictionary(Of String, IntentoImportacionCarpetas), id As String, raiz As String, archivo As String) As Boolean
        Return intentos.ContainsKey(id) AndAlso intentos(id).RaizId = raiz AndAlso intentos(id).ArchivoId = archivo AndAlso {"RESERVADO", "EN_CURSO", "INCIERTO"}.Contains(intentos(id).Estado)
    End Function
    Private Shared Function CreacionConfirmadaConsolidacion(lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intentos As List(Of IntentoImportacionCarpetas)) As Boolean
        Dim id = raiz.IdentidadExpediente
        If id Is Nothing OrElse id.OrigenLogico <> "RA" OrElse id.NivelRef <> lote.Contexto.NivelRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(id.ExpedienteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(id.RelacionNivelId) Then Return False
        Dim pruebas = intentos.Where(Function(i) i.RaizId = raiz.RaizId AndAlso i.Fase = "CREAR_EXPEDIENTE" AndAlso i.Estado = "CONFIRMADO").ToList()
        If pruebas.Count <> 1 Then Return False
        Dim prueba = pruebas(0), resultado = prueba.Resultado
        Return prueba.FinUtc IsNot Nothing AndAlso resultado IsNot Nothing AndAlso resultado.Estado = "CONFIRMADO" AndAlso resultado.Codigo = "EXPEDIENTE_CONFIRMADO" AndAlso resultado.Creacion IsNot Nothing AndAlso resultado.Creacion.IntentoId = prueba.IntentoId AndAlso resultado.Creacion.CicloArchivoNombre = "Archivo Gestión" AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.Creacion.CicloArchivoId) AndAlso resultado.DocumentoId Is Nothing AndAlso resultado.InventarioId Is Nothing AndAlso resultado.Recepcion Is Nothing AndAlso PerfilImportacionJson.Escribir(resultado.Expediente) = PerfilImportacionJson.Escribir(id)
    End Function
    Private Shared Function DocumentoConfirmadoConsolidacion(lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, intentos As List(Of IntentoImportacionCarpetas)) As Boolean
        Dim resultado = archivo.Resultado
        If archivo.EstadoEfecto <> "COMPLETADO" OrElse Not archivo.ValoresEfectivosFijados OrElse Not RecepcionRegistradaValida(archivo) OrElse resultado Is Nothing OrElse resultado.Estado <> "CONFIRMADO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId) OrElse resultado.OrigenGabinete <> lote.Snapshot.Configuracion.OrigenGabinete OrElse resultado.GabineteId <> lote.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture) OrElse PerfilImportacionJson.Escribir(resultado.Expediente) <> PerfilImportacionJson.Escribir(raiz.IdentidadExpediente) OrElse PerfilImportacionJson.Escribir(resultado.ValoresEfectivos) <> PerfilImportacionJson.Escribir(archivo.ValoresEfectivos) Then Return False
        If resultado.Codigo <> "DOCUMENTO_CONFIRMADO" OrElse resultado.Creacion IsNot Nothing OrElse resultado.Recepcion IsNot Nothing Then Return False
        Dim pruebas = intentos.Where(Function(i) i.ArchivoId = archivo.ArchivoId AndAlso i.Fase = "ALMACENAR" AndAlso i.Estado = "CONFIRMADO").ToList()
        Return pruebas.Count = 1 AndAlso pruebas(0).FinUtc IsNot Nothing AndAlso PerfilImportacionJson.Escribir(pruebas(0).Resultado) = PerfilImportacionJson.Escribir(resultado)
    End Function
End Class
