Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _vistaValores As Func(Of ContextoImportacionCarpetas, String, String, VistaValoresImportacion)
    Private ReadOnly _resolvedorValores As CoordinadorProveedorIndices
    Private ReadOnly _catalogosValores As IPerfilImportacionCatalogosOperativos
    Private ReadOnly _fijarValores As Func(Of ContextoImportacionCarpetas, VistaValoresImportacion, ResultadoEnsayoPerfilImportacion, ArchivoImportacionCarpetas)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), vista As Func(Of ContextoImportacionCarpetas, String, String, VistaValoresImportacion), resolvedor As CoordinadorProveedorIndices, catalogos As IPerfilImportacionCatalogosOperativos, fijar As Func(Of ContextoImportacionCarpetas, VistaValoresImportacion, ResultadoEnsayoPerfilImportacion, ArchivoImportacionCarpetas))
        Me.New(identidad, repositorio, destino, autorizar)
        If vista Is Nothing OrElse resolvedor Is Nothing OrElse catalogos Is Nothing OrElse fijar Is Nothing Then Throw New ArgumentNullException()
        _vistaValores = vista : _resolvedorValores = resolvedor : _catalogosValores = catalogos : _fijarValores = fijar
    End Sub

    'Una fase independiente; la fachada HTTP no encadena almacenamiento.
    Public Function MaterializarArchivo(loteId As String, archivoId As String, revisionEsperada As UInteger) As ArchivoImportacionCarpetas
        If _vistaValores Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivoId) OrElse revisionEsperada = 0 Then Throw NoPermitida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) Then Throw Denegado()
        Dim vista = _vistaValores(contexto, loteId, archivoId)
        ValidarVistaValores(contexto, vista)
        If vista.Lote.LoteId <> loteId OrElse vista.Archivo.ArchivoId <> archivoId OrElse (Not vista.Archivo.ValoresEfectivosFijados AndAlso vista.Archivo.Revision <> revisionEsperada) Then Throw NoPermitida()
        Return MaterializarValoresArchivo(contexto, vista, _resolvedorValores, _catalogosValores,
            Sub()
                If Not MismoContextoControl(contexto, _destino(referencia.NivelRef)) Then Throw Denegado()
                _autorizar(contexto, CopiarValores(vista.Lote.Snapshot.Configuracion))
            End Sub,
            Function(esperada, resultado) _fijarValores(contexto, esperada, resultado))
    End Function

    'Una unidad, sin efectos documentales. El lector comprueba la relacion por
    'IDs; el checkpoint compara de nuevo la vista bajo bloqueo antes de fijar.
    Friend Shared Function MaterializarValoresArchivo(contexto As ContextoImportacionCarpetas, vista As VistaValoresImportacion, resolvedor As CoordinadorProveedorIndices, catalogos As IPerfilImportacionCatalogosOperativos, autorizar As Action, guardar As Func(Of VistaValoresImportacion, ResultadoEnsayoPerfilImportacion, ArchivoImportacionCarpetas)) As ArchivoImportacionCarpetas
        If resolvedor Is Nothing OrElse catalogos Is Nothing OrElse autorizar Is Nothing OrElse guardar Is Nothing Then Throw New ArgumentNullException()
        ValidarVistaValores(contexto, vista)
        autorizar()
        'No se consulta siquiera la publicacion del proveedor en una repeticion.
        If vista.Archivo.ValoresEfectivosFijados Then Return CopiarValores(vista.Archivo)
        If vista.Lote.DetencionSolicitada OrElse vista.Lote.Estado <> "EN_CURSO" OrElse vista.Raiz.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse vista.Archivo.EstadoRecepcion <> "RECIBIDO" OrElse vista.Archivo.EstadoEfecto <> "NO_INICIADO" OrElse vista.Archivo.IntentoActivoId IsNot Nothing OrElse vista.Archivo.Resultado IsNot Nothing Then Throw NoPermitida()
        Dim reglas = vista.Lote.Snapshot.Reglas.Where(Function(r) r.DestinoTipo <> "CAMPO_EXPEDIENTE").ToList()
        Dim plan = vista.Archivo.ValoresPlan
        ValidarCorrespondenciaValores(reglas, plan, False)
        Dim pendientes = reglas.Where(Function(r) plan.Single(Function(v) ClaveValor(v) = ClaveRegla(r)).Diferido).ToList()
        Dim resultado As New ResultadoEnsayoPerfilImportacion()
        If pendientes.Count > 0 Then
            Dim ejemplo As New EjemploPerfilImportacion With {.NombreCarpeta = vista.Raiz.NombreOriginal, .NombreArchivo = vista.Archivo.NombreOriginal}
            'Las propiedades de creacion proceden de valores confirmados, no de
            'nombres externos ni de una nueva ejecucion de sus fuentes.
            ejemplo.Expediente.Add("ID", vista.Raiz.IdentidadExpediente.ExpedienteId)
            For Each propiedad In New String() {"CODIGO", "NOMBRE", "FECHA_INICIAL", "FECHA_FINAL", "RESPONSABLE"}
                Dim valores = vista.Raiz.ValoresPlan.Where(Function(v) v IsNot Nothing AndAlso v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = propiedad AndAlso Not v.Diferido).ToList()
                If valores.Count > 1 Then Throw NoPermitida()
                ejemplo.Expediente.Add(propiedad, If(valores.Count = 0, Nothing, valores(0).Valor))
            Next
            'La clasificacion conocida se resolvio en preflight. Solo se pide
            'si existe una dependencia aun pendiente de esa clasificacion.
            If pendientes.Any(Function(r) ResolvedorReglasPerfilImportacion.PropiedadesClasificacion.Contains(PropiedadDiferida(r))) Then
                Dim clasificacion = catalogos.ClasificacionOperativa(contexto, CopiarValores(vista.Lote.Snapshot.Configuracion))
                If clasificacion Is Nothing Then Throw NoPermitida()
                For Each propiedad In ResolvedorReglasPerfilImportacion.PropiedadesClasificacion
                    If clasificacion.ContainsKey(propiedad) Then ejemplo.Expediente.Add(propiedad, clasificacion(propiedad))
                Next
            End If
            resultado = resolvedor.ResolverOperativo(contexto, CopiarValores(pendientes), New List(Of EjemploPerfilImportacion) From {ejemplo}).Single()
            For Each valor In resultado.Valores.Where(Function(v) v.Diferido)
                resultado.Incidencias.Add(New IncidenciaPerfilImportacion(ClaveValor(valor), "DEPENDENCIA_NO_RESUELTA", "El valor sigue pendiente del expediente confirmado."))
            Next
            If resultado.Incidencias.Count = 0 Then
                Dim errores = catalogos.ValidarValoresOperativos(contexto, CopiarValores(vista.Lote.Snapshot.Configuracion), resultado.Valores)
                If errores Is Nothing Then Throw NoPermitida()
                resultado.Incidencias.AddRange(errores)
            End If
        End If
        If resultado.Incidencias.Count = 0 Then
            Dim nuevos = resultado.Valores.ToDictionary(Function(v) ClaveValor(v), StringComparer.Ordinal)
            resultado.Valores = plan.Select(Function(v) If(v.Diferido, nuevos(ClaveValor(v)), CopiarValores(v))).ToList()
            ValidarCorrespondenciaValores(reglas, resultado.Valores, True)
        Else
            'Ningun conjunto parcialmente resuelto habilita almacenamiento.
            resultado.Valores.Clear()
        End If
        autorizar()
        Return guardar(CopiarValores(vista), resultado)
    End Function

    Friend Shared Sub ValidarVistaValores(contexto As ContextoImportacionCarpetas, vista As VistaValoresImportacion)
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Archivo Is Nothing OrElse Not MismoContextoControl(contexto, vista.Lote.Contexto) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim lote = vista.Lote, raiz = vista.Raiz, archivo = vista.Archivo
        If Not ValidadorPerfilImportacion.ReferenciaValida(lote.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(raiz.RaizId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.ArchivoId) OrElse raiz.LoteId <> lote.LoteId OrElse archivo.RaizId <> raiz.RaizId OrElse raiz.IntentoActivoId IsNot Nothing OrElse Not {"EXPEDIENTE_CONFIRMADO", "PARCIAL", "COMPLETADA", "INCIERTO"}.Contains(raiz.Estado) Then Throw NoPermitida()
        Dim identidad = raiz.IdentidadExpediente
        If identidad Is Nothing OrElse identidad.OrigenLogico <> "RA" OrElse identidad.NivelRef <> contexto.NivelRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.ExpedienteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.RelacionNivelId) Then Throw NoPermitida()
        If lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse lote.Snapshot.Reglas Is Nothing OrElse raiz.ValoresPlan Is Nothing OrElse archivo.ValoresPlan Is Nothing OrElse archivo.ValoresEfectivos Is Nothing Then Throw NoPermitida()
        Dim snapshot = lote.Snapshot
        If New ResolvedorReglasPerfilImportacion().ValidarEstructura(snapshot.Reglas).Count <> 0 OrElse BitConverter.ToString(PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = snapshot.Configuracion, .Reglas = snapshot.Reglas})).Replace("-", "").ToLowerInvariant() <> snapshot.Huella Then Throw NoPermitida()
        Dim reglas = snapshot.Reglas.Where(Function(r) r.DestinoTipo <> "CAMPO_EXPEDIENTE").ToList()
        ValidarCorrespondenciaValores(reglas, archivo.ValoresPlan, False)
        If archivo.ValoresEfectivosFijados Then
            ValidarCorrespondenciaValores(reglas, archivo.ValoresEfectivos, True)
            For Each valor In archivo.ValoresPlan.Where(Function(v) Not v.Diferido)
                If PerfilImportacionJson.Escribir(valor) <> PerfilImportacionJson.Escribir(archivo.ValoresEfectivos.Single(Function(v) ClaveValor(v) = ClaveValor(valor))) Then Throw NoPermitida()
            Next
        ElseIf archivo.ValoresEfectivos.Count <> 0 Then
            Throw NoPermitida()
        End If
    End Sub

    Friend Shared Sub ValidarCorrespondenciaValores(reglas As List(Of ReglaPerfilImportacion), valores As List(Of ValorReglaPerfilImportacion), finales As Boolean)
        If valores Is Nothing OrElse valores.Count <> reglas.Count OrElse valores.Any(Function(v) v Is Nothing) OrElse valores.Select(Function(v) ClaveValor(v)).Distinct(StringComparer.Ordinal).Count() <> valores.Count Then Throw NoPermitida()
        For Each regla In reglas
            Dim encontrados = valores.Where(Function(v) ClaveValor(v) = ClaveRegla(regla)).ToList()
            If encontrados.Count <> 1 Then Throw NoPermitida()
            Dim valor = encontrados(0)
            Dim tipo = If(regla.DestinoTipo = "TIPOLOGIA" AndAlso Not valor.Diferido, "REFERENCIA", regla.TipoValor)
            If valor.FuenteTipo <> regla.FuenteTipo OrElse valor.TipoValor <> tipo Then Throw NoPermitida()
            If regla.FuenteTipo = "PROVEEDOR" Then
                If valor.Procedencia <> regla.Proveedor.ProveedorId & ":" & regla.Proveedor.VersionId & ":" & regla.Proveedor.Propiedad Then Throw NoPermitida()
            End If
            If valor.Diferido Then
                If finales OrElse regla.EtapaResolucion <> "TRAS_EXPEDIENTE" OrElse valor.Valor IsNot Nothing OrElse valor.PropiedadPendiente <> PropiedadDiferida(regla) OrElse String.IsNullOrEmpty(valor.PropiedadPendiente) Then Throw NoPermitida()
            Else
                If valor.PropiedadPendiente IsNot Nothing Then Throw NoPermitida()
                If regla.DestinoTipo = "TIPOLOGIA" Then
                    If Not ValidadorPerfilImportacion.ReferenciaValida(valor.Valor) Then Throw NoPermitida()
                ElseIf ResolvedorReglasPerfilImportacion.ValidarValorResuelto(regla, valor.Valor) IsNot Nothing Then
                    Throw NoPermitida()
                End If
            End If
        Next
    End Sub

    Private Shared Function PropiedadDiferida(regla As ReglaPerfilImportacion) As String
        If regla.FuenteTipo = "EXPEDIENTE" Then Return regla.PropiedadExpediente
        If regla.FuenteTipo = "PROVEEDOR" AndAlso regla.Proveedor.Clave.FuenteTipo = "EXPEDIENTE" Then Return regla.Proveedor.Clave.PropiedadExpediente
        Return Nothing
    End Function
    Private Shared Function ClaveRegla(regla As ReglaPerfilImportacion) As String
        Return regla.DestinoTipo & ":" & regla.DestinoRef
    End Function
    Private Shared Function ClaveValor(valor As ValorReglaPerfilImportacion) As String
        Return valor.DestinoTipo & ":" & valor.DestinoRef
    End Function
    Private Shared Function CopiarValores(Of T)(valor As T) As T
        Return PerfilImportacionJson.Leer(Of T)(PerfilImportacionJson.Escribir(valor))
    End Function
End Class
