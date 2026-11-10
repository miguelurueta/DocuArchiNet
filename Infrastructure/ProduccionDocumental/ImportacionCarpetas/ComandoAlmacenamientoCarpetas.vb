Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions

'Comando inmutable del archivo reclamado; no resuelve fuentes ni usa Session.
Public NotInheritable Class ComandoAlmacenamientoCarpetas
    Private ReadOnly _contexto As String, _raiz As String, _archivo As String, _snapshot As String, _intento As String
    Public Const ClaseDocumentoNombre As String = "DOCUMENTO ELECTRONICO"
    Public ReadOnly Property EsVerificacion As Boolean
    Public ReadOnly Property Contexto As ContextoImportacionCarpetas
        Get
            Return PerfilImportacionJson.Leer(Of ContextoImportacionCarpetas)(_contexto)
        End Get
    End Property
    Public ReadOnly Property Raiz As RaizImportacionCarpetas
        Get
            Return PerfilImportacionJson.Leer(Of RaizImportacionCarpetas)(_raiz)
        End Get
    End Property
    Public ReadOnly Property Archivo As ArchivoImportacionCarpetas
        Get
            Return PerfilImportacionJson.Leer(Of ArchivoImportacionCarpetas)(_archivo)
        End Get
    End Property
    Public ReadOnly Property Snapshot As SnapshotPerfilImportacionOperativa
        Get
            Return PerfilImportacionJson.Leer(Of SnapshotPerfilImportacionOperativa)(_snapshot)
        End Get
    End Property
    Public ReadOnly Property Intento As IntentoImportacionCarpetas
        Get
            Return PerfilImportacionJson.Leer(Of IntentoImportacionCarpetas)(_intento)
        End Get
    End Property
    Public ReadOnly Property TipologiaId As String
        Get
            Return Archivo.ValoresEfectivos.Single(Function(v) v.DestinoTipo = "TIPOLOGIA").Valor
        End Get
    End Property
    Private Sub New(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas, verificacion As Boolean)
        EsVerificacion = verificacion
        _contexto = PerfilImportacionJson.Escribir(contexto) : _raiz = PerfilImportacionJson.Escribir(raiz)
        _archivo = PerfilImportacionJson.Escribir(archivo) : _snapshot = PerfilImportacionJson.Escribir(snapshot) : _intento = PerfilImportacionJson.Escribir(intento)
    End Sub
    Public Shared Function Preparar(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ComandoAlmacenamientoCarpetas
        Return Componer(contexto, raiz, archivo, snapshot, intento, False)
    End Function
    Public Shared Function PrepararVerificacion(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ComandoAlmacenamientoCarpetas
        Return Componer(contexto, raiz, archivo, snapshot, intento, True)
    End Function
    Public Shared Function PrepararComprobacionFinal(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ComandoAlmacenamientoCarpetas
        Return Componer(contexto, raiz, archivo, snapshot, intento, True, True)
    End Function
    Friend Shared Function PrepararRecuperacionV2(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas, documento As DocumentoAlmacenamientoV2Importacion) As ComandoAlmacenamientoCarpetas
        Dim prueba = DocumentosAlmacenamientoV2Importacion.Leer(DocumentosAlmacenamientoV2Importacion.Escribir(documento))
        If intento Is Nothing OrElse archivo Is Nothing OrElse prueba.Inicio.IntentoId <> intento.IntentoId OrElse prueba.Inicio.LoteId <> intento.LoteId OrElse prueba.Inicio.RaizId <> intento.RaizId OrElse prueba.Inicio.ArchivoId <> archivo.ArchivoId Then Throw Fallo()
        Return Componer(contexto, raiz, archivo, snapshot, intento, True, False, True)
    End Function
    Private Shared Function Componer(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas, verificacion As Boolean, Optional final As Boolean = False, Optional recuperacionV2 As Boolean = False) As ComandoAlmacenamientoCarpetas
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse raiz Is Nothing OrElse archivo Is Nothing OrElse snapshot Is Nothing OrElse intento Is Nothing Then Throw Fallo()
        For Each referencia In New String() {contexto.UsuarioRef, contexto.AmbitoRef, contexto.NivelRef, raiz.LoteId, raiz.RaizId, archivo.ArchivoId, intento.IntentoId}
            ComandoCreacionExpedienteImportacion.EnteroLegacy(referencia)
        Next
        If String.IsNullOrWhiteSpace(contexto.RutaCanonica) OrElse String.IsNullOrWhiteSpace(contexto.SelloDestino) OrElse raiz.IntentoActivoId IsNot Nothing OrElse raiz.IdentidadExpediente Is Nothing Then Throw Fallo()
        Dim identidad = raiz.IdentidadExpediente
        If identidad.OrigenLogico <> "RA" OrElse identidad.NivelRef <> contexto.NivelRef Then Throw Fallo()
        ComandoCreacionExpedienteImportacion.EnteroLegacy(identidad.ExpedienteId)
        ComandoCreacionExpedienteImportacion.EnteroLegacy(identidad.RelacionNivelId)
        If intento.Fase <> "ALMACENAR" OrElse intento.LoteId <> raiz.LoteId OrElse intento.RaizId <> raiz.RaizId OrElse intento.ArchivoId <> archivo.ArchivoId OrElse archivo.RaizId <> raiz.RaizId OrElse archivo.EstadoRecepcion <> "RECIBIDO" OrElse archivo.Revision = 0 OrElse archivo.Revision >= UInteger.MaxValue - 1UI Then Throw Fallo()
        If final Then
            If Not verificacion OrElse raiz.Estado <> "COMPLETADA" OrElse archivo.EstadoEfecto <> "COMPLETADO" OrElse archivo.IntentoActivoId IsNot Nothing OrElse intento.Estado <> "CONFIRMADO" OrElse String.IsNullOrEmpty(intento.FinUtc) OrElse intento.EsNuevaReserva OrElse intento.Resultado Is Nothing OrElse intento.Resultado.Estado <> "CONFIRMADO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.Resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.Resultado.InventarioId) OrElse PerfilImportacionJson.Escribir(archivo.Resultado) <> PerfilImportacionJson.Escribir(intento.Resultado) Then Throw Fallo()
        ElseIf intento.FinUtc IsNot Nothing OrElse archivo.IntentoActivoId <> intento.IntentoId Then
            Throw Fallo()
        ElseIf recuperacionV2 Then
            If Not verificacion OrElse raiz.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse intento.Estado <> "EN_CURSO" OrElse intento.EsNuevaReserva OrElse archivo.EstadoEfecto <> "EN_CURSO" OrElse archivo.Resultado IsNot Nothing OrElse intento.Resultado Is Nothing OrElse intento.Resultado.Estado <> "EN_CURSO" OrElse intento.Resultado.Codigo <> "ALMACENAMIENTO_PREPARADO_V2" Then Throw Fallo()
        ElseIf verificacion Then
            If Not {"EXPEDIENTE_CONFIRMADO", "INCIERTO"}.Contains(raiz.Estado) OrElse intento.Estado <> "INCIERTO" OrElse intento.EsNuevaReserva OrElse archivo.EstadoEfecto <> "INCIERTO" OrElse intento.Resultado Is Nothing OrElse intento.Resultado.Estado <> "INCIERTO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.Resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.Resultado.InventarioId) OrElse PerfilImportacionJson.Escribir(archivo.Resultado) <> PerfilImportacionJson.Escribir(intento.Resultado) Then Throw Fallo()
        ElseIf raiz.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse intento.Estado <> "RESERVADO" OrElse Not intento.EsNuevaReserva OrElse intento.Resultado IsNot Nothing OrElse archivo.EstadoEfecto <> "EN_CURSO" OrElse archivo.Resultado IsNot Nothing Then
            Throw Fallo()
        End If
        If Not Hexadecimal(intento.TokenReclamacion, 32) OrElse Not Hexadecimal(intento.SolicitudHash, 64) OrElse Not Hexadecimal(archivo.StagingRef, 32) OrElse Not Hexadecimal(archivo.ContenidoHash, 64) Then Throw Fallo()
        Dim cantidad As ULong
        If Not ULong.TryParse(archivo.BytesPrevistos, NumberStyles.None, CultureInfo.InvariantCulture, cantidad) OrElse cantidad = 0 OrElse cantidad.ToString(CultureInfo.InvariantCulture) <> archivo.BytesPrevistos OrElse archivo.BytesRecibidos <> archivo.BytesPrevistos Then Throw Fallo()
        If String.IsNullOrEmpty(archivo.NombreOriginal) OrElse archivo.NombreOriginal.Length > 255 OrElse archivo.NombreOriginal.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 OrElse archivo.NombreOriginal <> Path.GetFileName(archivo.NombreOriginal) OrElse archivo.NombreOriginal.Contains("|") OrElse archivo.NombreOriginal.EndsWith(".", StringComparison.Ordinal) OrElse archivo.NombreOriginal.EndsWith(" ", StringComparison.Ordinal) Then Throw Fallo()
        ComandoCreacionExpedienteImportacion.TextoLegacy(archivo.NombreOriginal, True)
        If Not String.Equals(Path.GetExtension(archivo.NombreOriginal), ".pdf", StringComparison.OrdinalIgnoreCase) Then Throw New PerfilImportacionException("FORMATO_NO_SOPORTADO", "La importacion documental admite PDF en esta entrega.")
        If snapshot.Configuracion Is Nothing OrElse snapshot.Reglas Is Nothing OrElse New ResolvedorReglasPerfilImportacion().ValidarEstructura(snapshot.Reglas).Count <> 0 Then Throw Fallo()
        If BitConverter.ToString(PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = snapshot.Configuracion, .Reglas = snapshot.Reglas})).Replace("-", "").ToLowerInvariant() <> snapshot.Huella Then Throw Fallo()
        Dim c = snapshot.Configuracion
        If c.OrigenGabinete <> "DA" OrElse Not c.GabineteId.HasValue OrElse Not Hexadecimal(c.HuellaOrigenGabinete, 64) Then Throw Fallo()
        If c.Extensiones Is Nothing OrElse (c.Extensiones.Count > 0 AndAlso Not c.Extensiones.Contains("pdf", StringComparer.OrdinalIgnoreCase)) OrElse (c.MaximoBytesArchivo.HasValue AndAlso (c.MaximoBytesArchivo.Value <= 0 OrElse cantidad > CULng(c.MaximoBytesArchivo.Value))) Then Throw Fallo()
        ComandoCreacionExpedienteImportacion.EnteroLegacy(c.GabineteId.Value.ToString(CultureInfo.InvariantCulture))
        If Not archivo.ValoresEfectivosFijados Then Throw Fallo()
        CoordinadorImportacionCarpetas.ValidarCorrespondenciaValores(snapshot.Reglas.Where(Function(r) r.DestinoTipo <> "CAMPO_EXPEDIENTE").ToList(), archivo.ValoresEfectivos, True)
        If archivo.ValoresEfectivos.Where(Function(v) v.DestinoTipo = "TIPOLOGIA").Count() <> 1 Then Throw Fallo()
        For Each valor In archivo.ValoresEfectivos
            ComandoCreacionExpedienteImportacion.TextoLegacy(valor.Valor, valor.DestinoTipo = "TIPOLOGIA")
        Next
        Dim comando As New ComandoAlmacenamientoCarpetas(contexto, raiz, archivo, snapshot, intento, verificacion)
        ComandoCreacionExpedienteImportacion.EnteroLegacy(comando.TipologiaId)
        Return comando
    End Function

    Friend Shared Function CodigoFormatoLegacy(valor As String) As Integer
        Dim codigo As Integer
        If String.IsNullOrEmpty(valor) OrElse Not Integer.TryParse(valor, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, codigo) OrElse codigo.ToString(CultureInfo.InvariantCulture) <> valor Then Throw Fallo()
        Return codigo
    End Function

    Public Sub ValidarDatos(datos As DatosAlmacenamientoImportacion)
        If datos Is Nothing OrElse datos.Campos Is Nothing OrElse datos.ClaseDocumentoNombre <> ClaseDocumentoNombre Then Throw Fallo()
        IdentificadorSql(datos.GabineteNombre)
        ComandoCreacionExpedienteImportacion.TextoLegacy(datos.TipologiaNombre, True)
        ComandoCreacionExpedienteImportacion.EnteroLegacy(datos.ClaseDocumentoId)
        CodigoFormatoLegacy(datos.TipoArchivoId)
        Dim valores = Archivo.ValoresEfectivos.Where(Function(v) v.DestinoTipo = "INDICE_GABINETE").ToList()
        If datos.Campos.Count <> valores.Count Then Throw Fallo()
        Dim nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase), referencias As New HashSet(Of String)(StringComparer.Ordinal)
        For Each campo In datos.Campos
            If campo Is Nothing OrElse Not referencias.Add(campo.Referencia) OrElse Not nombres.Add(campo.Nombre) Then Throw Fallo()
            IdentificadorSql(campo.Nombre)
            If New String() {"ID", "DISC", "PAG", "DBT", "IDEX", "USER", "DATE1", "TIME1", "ID_AREA", "ID_SERIE", "ID_SUB_SERIE", "ID_TIPODOCUMENTO", "ID_EXPEDIENTE", "ID_TIPO_EXPEDIENTE", "ID_UNIDAD_CONSERVACION", "ID_TIPO_UNIDAD_CONSERVACION", "ID_CLASE_DOCUMENTO", "ID_TIPO_UNIDAD_DOCUMENTAL", "ID_INVENTARIO_DOCUMENTAL", "ID_USUARIO_GESTION"}.Contains(campo.Nombre, StringComparer.OrdinalIgnoreCase) Then Throw Fallo()
            Dim valor = valores.SingleOrDefault(Function(v) v.DestinoRef = campo.Referencia)
            If valor Is Nothing OrElse Not String.Equals(valor.Valor, campo.Valor, StringComparison.Ordinal) Then Throw Fallo()
            If EsCampoAutomatico(campo.Nombre) Then
                Dim esperado As String = Nothing
                Select Case campo.Nombre.ToUpperInvariant()
                    Case "NOMBRE" : esperado = Archivo.NombreOriginal
                    Case "CLASEDOCUMENTO" : esperado = ClaseDocumentoNombre
                    Case "TIPODOCUMENTO" : esperado = datos.TipologiaNombre
                    Case "EXPEDIENTE"
                        Dim codigo = Raiz.ValoresPlan.Where(Function(v) v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = "CODIGO" AndAlso Not v.Diferido).ToList()
                        If codigo.Count <> 1 Then Throw Fallo()
                        esperado = codigo(0).Valor
                    Case Else
                        'Serie/subserie/fecha se cargan dentro de legacy. No
                        'aceptar una regla que pueda ser ignorada silenciosamente.
                        Throw Fallo()
                End Select
                If Not String.Equals(campo.Valor, esperado, StringComparison.Ordinal) Then Throw Fallo()
            End If
        Next
    End Sub
    Friend Shared Function EsCampoAutomatico(nombre As String) As Boolean
        Return New String() {"NOMBRE", "EXPEDIENTE", "CLASEDOCUMENTO", "FECHAELABORACION", "TIPODOCUMENTO", "NOMBRESERIE", "NOMBRESUBSERIE"}.Contains(nombre, StringComparer.OrdinalIgnoreCase)
    End Function
    Friend Shared Sub IdentificadorSql(nombre As String)
        If nombre Is Nothing OrElse Not Regex.IsMatch(nombre, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z", RegexOptions.CultureInvariant) Then Throw Fallo()
    End Sub
    Private Shared Function Hexadecimal(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso valor.All(Function(c) (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "a"c AndAlso c <= "f"c))
    End Function
    Private Shared Function Fallo() As PerfilImportacionException
        Return New PerfilImportacionException("CONFIGURACION_INVALIDA", "El comando documental no coincide con el archivo, valores e intento confirmados.")
    End Function
End Class

Public Class DatosAlmacenamientoImportacion
    Public Property GabineteNombre As String
    Public Property TipologiaNombre As String
    Public Property ClaseDocumentoId As String
    Public Property ClaseDocumentoNombre As String
    Public Property TipoArchivoId As String
    Public Property Campos As New List(Of CampoIndiceImportacionCarpetas)()
End Class
Public Class CampoIndiceImportacionCarpetas
    Public Property Referencia As String
    Public Property Nombre As String
    Public Property Valor As String
End Class
