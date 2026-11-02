Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

'Composicion interna, sin efectos ni lectura de Session/catalogos/proveedores.
'No representa permiso de invocacion: faltan autoridad vigente, barreras,
'compatibilidad de nombres/conexion legacy y correlacion del resultado.
Public NotInheritable Class ComandoCreacionExpedienteImportacion
    Private ReadOnly _contextoDoc As String
    Private ReadOnly _snapshotDoc As String
    Private ReadOnly _valoresDoc As String
    Public ReadOnly Property LoteId As String
    Public ReadOnly Property RaizId As String
    Public ReadOnly Property IntentoId As String
    Public ReadOnly Property TokenReclamacion As String
    Public ReadOnly Property SolicitudHash As String
    Public ReadOnly Property RevisionRaiz As UInteger
    Public ReadOnly Property Codigo As String
    Public ReadOnly Property Nombre As String
    Public ReadOnly Property FechaInicial As String
    Public ReadOnly Property FechaFinal As String
    Public ReadOnly Property ResponsableRef As String
    Public ReadOnly Property CicloArchivoNombre As String
    Public ReadOnly Property EsVerificacion As Boolean

    Public ReadOnly Property Contexto As ContextoImportacionCarpetas
        Get
            Return PerfilImportacionJson.Leer(Of ContextoImportacionCarpetas)(_contextoDoc)
        End Get
    End Property
    Public ReadOnly Property Snapshot As SnapshotPerfilImportacionOperativa
        Get
            Return PerfilImportacionJson.Leer(Of SnapshotPerfilImportacionOperativa)(_snapshotDoc)
        End Get
    End Property
    Public ReadOnly Property Valores As List(Of ValorReglaPerfilImportacion)
        Get
            Return PerfilImportacionJson.Leer(Of List(Of ValorReglaPerfilImportacion))(_valoresDoc)
        End Get
    End Property

    Private Sub New(contexto As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas, campos As Dictionary(Of String, ValorReglaPerfilImportacion), verificacion As Boolean)
        EsVerificacion = verificacion
        _contextoDoc = PerfilImportacionJson.Escribir(contexto)
        _snapshotDoc = PerfilImportacionJson.Escribir(lote.Snapshot)
        _valoresDoc = PerfilImportacionJson.Escribir(raiz.ValoresPlan)
        LoteId = lote.LoteId : RaizId = raiz.RaizId : IntentoId = intento.IntentoId
        TokenReclamacion = intento.TokenReclamacion : SolicitudHash = intento.SolicitudHash : RevisionRaiz = raiz.Revision
        Codigo = campos("CODIGO").Valor : Nombre = campos("NOMBRE").Valor : FechaInicial = campos("FECHA_INICIAL").Valor
        FechaFinal = If(campos.ContainsKey("FECHA_FINAL"), campos("FECHA_FINAL").Valor, Nothing)
        ResponsableRef = If(campos.ContainsKey("RESPONSABLE"), campos("RESPONSABLE").Valor, Nothing)
        'Politica aprobada por el usuario: misma eleccion que Produccion.
        CicloArchivoNombre = "Archivo Gestión"
    End Sub

    Public Shared Function Preparar(contextoActual As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas) As ComandoCreacionExpedienteImportacion
        Return Componer(contextoActual, lote, raiz, intento, False)
    End Function
    Public Shared Function PrepararVerificacion(contextoActual As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas) As ComandoCreacionExpedienteImportacion
        Return Componer(contextoActual, lote, raiz, intento, True)
    End Function
    Private Shared Function Componer(contextoActual As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas, verificacion As Boolean) As ComandoCreacionExpedienteImportacion
        If contextoActual Is Nothing OrElse Not contextoActual.PuedeImportar OrElse Not Referencia(contextoActual.UsuarioRef) OrElse Not Referencia(contextoActual.AmbitoRef) OrElse Not Referencia(contextoActual.NivelRef) OrElse String.IsNullOrWhiteSpace(contextoActual.SelloDestino) OrElse String.IsNullOrWhiteSpace(contextoActual.RutaCanonica) Then Throw Fallo("ACCESO_DENEGADO")
        If lote Is Nothing OrElse lote.Contexto Is Nothing OrElse Not lote.Contexto.PuedeImportar OrElse contextoActual.UsuarioRef <> lote.Contexto.UsuarioRef OrElse contextoActual.AmbitoRef <> lote.Contexto.AmbitoRef OrElse contextoActual.NivelRef <> lote.Contexto.NivelRef OrElse contextoActual.SelloDestino <> lote.Contexto.SelloDestino OrElse contextoActual.RutaCanonica <> lote.Contexto.RutaCanonica Then Throw Fallo("ACCESO_DENEGADO")
        If Not Referencia(lote.LoteId) OrElse lote.Revision = 0 OrElse lote.Revision = UInteger.MaxValue Then Throw Fallo("CONFLICTO_REVISION")
        If raiz Is Nothing OrElse Not Referencia(raiz.RaizId) OrElse raiz.LoteId <> lote.LoteId OrElse raiz.Revision = 0 OrElse raiz.Revision = UInteger.MaxValue OrElse raiz.ValoresEfectivos Is Nothing OrElse raiz.ValoresEfectivos.Count <> 0 Then Throw Fallo("CONFLICTO_REVISION")
        If intento Is Nothing OrElse Not Referencia(intento.IntentoId) OrElse intento.LoteId <> lote.LoteId OrElse intento.RaizId <> raiz.RaizId OrElse Not String.IsNullOrEmpty(intento.ArchivoId) OrElse intento.Fase <> "CREAR_EXPEDIENTE" OrElse Not String.IsNullOrEmpty(intento.FinUtc) OrElse raiz.IntentoActivoId <> intento.IntentoId OrElse Not Hexadecimal(intento.TokenReclamacion, 32) OrElse Not Hexadecimal(intento.SolicitudHash, 64) Then Throw Fallo("CONFLICTO_REVISION")
        If verificacion Then
            If raiz.Estado <> "INCIERTO" OrElse raiz.IdentidadExpediente IsNot Nothing OrElse intento.Estado <> "INCIERTO" OrElse intento.EsNuevaReserva OrElse intento.Resultado Is Nothing OrElse intento.Resultado.Estado <> "INCIERTO" OrElse intento.Resultado.Expediente Is Nothing OrElse Not Referencia(intento.Resultado.Expediente.ExpedienteId) OrElse Not Referencia(intento.Resultado.Expediente.RelacionNivelId) OrElse intento.Resultado.Expediente.OrigenLogico <> "RA" OrElse intento.Resultado.Expediente.NivelRef <> contextoActual.NivelRef OrElse intento.Resultado.Creacion Is Nothing OrElse intento.Resultado.Creacion.IntentoId <> intento.IntentoId OrElse intento.Resultado.Creacion.CicloArchivoNombre <> "Archivo Gestión" OrElse Not Referencia(intento.Resultado.Creacion.CicloArchivoId) OrElse {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then Throw Fallo("RESULTADO_INCIERTO")
        ElseIf lote.DetencionSolicitada OrElse lote.Estado <> "EN_CURSO" OrElse raiz.Estado <> "CREANDO" OrElse raiz.IdentidadExpediente IsNot Nothing OrElse intento.Estado <> "RESERVADO" OrElse Not intento.EsNuevaReserva OrElse intento.Resultado IsNot Nothing Then
            Throw Fallo("CONFLICTO_REVISION")
        End If
        Dim snapshot = lote.Snapshot
        If snapshot Is Nothing OrElse snapshot.Configuracion Is Nothing OrElse snapshot.Reglas Is Nothing OrElse Not Referencia(snapshot.PerfilId) OrElse Not Referencia(snapshot.VersionId) OrElse snapshot.NumeroVersion = 0 OrElse Not Hexadecimal(snapshot.Huella, 64) Then Throw Fallo("CONFIGURACION_INVALIDA")
        'Validar trabaja sobre copia: no normalizar el snapshot confirmado.
        Dim copia = PerfilImportacionJson.Leer(Of SnapshotPerfilImportacionOperativa)(PerfilImportacionJson.Escribir(snapshot))
        If New ValidadorPerfilImportacion().Validar(copia.Configuracion, copia.Reglas, True).Count <> 0 Then Throw Fallo("CONFIGURACION_INVALIDA")
        If PerfilImportacionJson.Escribir(copia.Configuracion) <> PerfilImportacionJson.Escribir(snapshot.Configuracion) Then Throw Fallo("CONFIGURACION_INVALIDA")
        Dim huella = BitConverter.ToString(PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = snapshot.Configuracion, .Reglas = snapshot.Reglas})).Replace("-", "").ToLowerInvariant()
        If huella <> snapshot.Huella Then Throw Fallo("CONFIGURACION_INVALIDA")
        Dim reglas = snapshot.Reglas.Where(Function(r) r.DestinoTipo = "CAMPO_EXPEDIENTE").ToDictionary(Function(r) r.DestinoRef, StringComparer.Ordinal)
        If raiz.ValoresPlan Is Nothing OrElse raiz.ValoresPlan.Count <> reglas.Count Then Throw Fallo("CONFIGURACION_INVALIDA")
        Dim campos As New Dictionary(Of String, ValorReglaPerfilImportacion)(StringComparer.Ordinal)
        For Each valor In raiz.ValoresPlan
            If valor Is Nothing OrElse valor.DestinoTipo <> "CAMPO_EXPEDIENTE" OrElse String.IsNullOrEmpty(valor.DestinoRef) OrElse Not reglas.ContainsKey(valor.DestinoRef) OrElse campos.ContainsKey(valor.DestinoRef) OrElse valor.Diferido OrElse Not String.IsNullOrEmpty(valor.PropiedadPendiente) Then Throw Fallo("CONFIGURACION_INVALIDA")
            Dim regla = reglas(valor.DestinoRef)
            If valor.FuenteTipo <> regla.FuenteTipo OrElse valor.TipoValor <> regla.TipoValor OrElse ResolvedorReglasPerfilImportacion.ValidarValorResuelto(regla, valor.Valor) IsNot Nothing Then Throw Fallo("CONFIGURACION_INVALIDA")
            If valor.DestinoRef = "RESPONSABLE" AndAlso Not Referencia(valor.Valor) Then Throw Fallo("CONFIGURACION_INVALIDA")
            campos.Add(valor.DestinoRef, valor)
        Next
        If campos.ContainsKey("FECHA_FINAL") AndAlso String.CompareOrdinal(campos("FECHA_INICIAL").Valor, campos("FECHA_FINAL").Valor) > 0 Then Throw Fallo("CONFIGURACION_INVALIDA")
        Return New ComandoCreacionExpedienteImportacion(contextoActual, lote, raiz, intento, campos, verificacion)
    End Function

    'Subconjunto compatible con los argumentos Integer y el SQL concatenado
    'de Conservacion. No escapa, trunca o cambia los valores confirmados.
    Public Sub ValidarEntradasLegacy()
        Dim contextoFijo = Contexto, c = Snapshot.Configuracion
        For Each referenciaActual In New String() {contextoFijo.UsuarioRef, contextoFijo.AmbitoRef, contextoFijo.NivelRef, c.InstrumentoRef, c.TipoExpedienteRef, c.FondoRef, c.AreaRef, c.SerieRef, c.UnidadDocumentalRef}
            EnteroLegacy(referenciaActual)
        Next
        If Not String.IsNullOrEmpty(c.SubserieRef) Then EnteroLegacy(c.SubserieRef)
        TextoLegacy(Codigo, True) : TextoLegacy(Nombre, True)
        'El arbol de Produccion presenta ALEAS_EXPEDIENTE. Conservacion limita
        'ese argumento a 120; no truncar el nombre confirmado para rellenarlo.
        If Nombre.Length > 120 Then Throw Fallo("CONFIGURACION_INVALIDA")
        TextoLegacy(FechaInicial, True) : TextoLegacy(FechaFinal, False)
    End Sub

    Public Sub ValidarDatosLegacy(datos As Dictionary(Of String, String))
        If datos Is Nothing Then Throw Fallo("CONFIGURACION_INVALIDA")
        Dim requeridos = New String() {"ORGANIGRAMA_ID", "ORGANIGRAMA_NOMBRE", "FONDO_NOMBRE", "AREA_NOMBRE", "SERIE_NOMBRE", "SUBSERIE_NOMBRE", "TIPO_EXPEDIENTE_NOMBRE", "UNIDAD_DOCUMENTAL_NOMBRE", "GABINETE_NOMBRE", "RESPONSABLE_NOMBRE", "RESPONSABLE_IDENTIFICACION", "CICLO_ARCHIVO_ID", "CICLO_ARCHIVO_NOMBRE"}
        If datos.Count <> requeridos.Length OrElse requeridos.Any(Function(campo) Not datos.ContainsKey(campo)) Then Throw Fallo("CONFIGURACION_INVALIDA")
        EnteroLegacy(datos("ORGANIGRAMA_ID"))
        EnteroLegacy(datos("CICLO_ARCHIVO_ID"))
        If Not String.Equals(datos("CICLO_ARCHIVO_NOMBRE"), CicloArchivoNombre, StringComparison.Ordinal) Then Throw Fallo("CONFIGURACION_INVALIDA")
        TextoLegacy(datos("CICLO_ARCHIVO_NOMBRE"), True)
        For Each campo In New String() {"ORGANIGRAMA_NOMBRE", "FONDO_NOMBRE", "AREA_NOMBRE", "SERIE_NOMBRE", "UNIDAD_DOCUMENTAL_NOMBRE", "GABINETE_NOMBRE"}
            TextoLegacy(datos(campo), True)
        Next
        Dim c = Snapshot.Configuracion
        If String.IsNullOrEmpty(c.SubserieRef) Then
            If datos("SUBSERIE_NOMBRE") IsNot Nothing Then Throw Fallo("CONFIGURACION_INVALIDA")
        Else
            TextoLegacy(datos("SUBSERIE_NOMBRE"), True)
        End If
        If ResponsableRef Is Nothing Then
            If datos("RESPONSABLE_NOMBRE") IsNot Nothing OrElse datos("RESPONSABLE_IDENTIFICACION") IsNot Nothing Then Throw Fallo("CONFIGURACION_INVALIDA")
        Else
            TextoLegacy(datos("RESPONSABLE_NOMBRE"), True) : TextoLegacy(datos("RESPONSABLE_IDENTIFICACION"), True)
            If datos("RESPONSABLE_IDENTIFICACION").Length > 60 Then Throw Fallo("CONFIGURACION_INVALIDA")
        End If
    End Sub

    Friend Shared Sub EnteroLegacy(valor As String)
        Dim numero As Integer
        If Not Referencia(valor) OrElse Not Integer.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, numero) OrElse numero <= 0 Then Throw Fallo("CONFIGURACION_INVALIDA")
    End Sub
    Friend Shared Sub TextoLegacy(valor As String, requerido As Boolean)
        If valor Is Nothing AndAlso Not requerido Then Return
        If String.IsNullOrWhiteSpace(valor) OrElse valor.Any(Function(c) c = "'"c OrElse c = "\"c OrElse Char.IsControl(c) OrElse Char.IsSurrogate(c)) Then Throw Fallo("CONFIGURACION_INVALIDA")
    End Sub

    Private Shared Function Referencia(valor As String) As Boolean
        Return ValidadorPerfilImportacion.ReferenciaValida(valor)
    End Function
    Private Shared Function Hexadecimal(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function
    Private Shared Function Fallo(codigo As String) As PerfilImportacionException
        Return New PerfilImportacionException(codigo, "No se pudo componer el comando de creación con el contexto confirmado.")
    End Function
End Class
