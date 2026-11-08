Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

'Datos internos, no DTO HTTP ni permisos. SERVIDOR es un discriminador de esquema,
'no prueba de procedencia: RV-04 deberá contrastar inicio original y diario durable.
'Ninguna operación de este archivo accede a disco, SQL, Session o almacenamiento.
Friend Class OriginalAlmacenamientoV2Importacion
    Public Property ProveedorRef As String
    Public Property NodoRef As String
    Public Property IdentidadFisicaHash As String
    Public Property ContenidoHash As String
    Public Property Bytes As String
End Class

Friend Class InicioAlmacenamientoV2Importacion
    Public Property Version As UShort
    Public Property Origen As String
    Public Property Clase As String
    Public Property IntentoId As String
    Public Property LoteId As String
    Public Property RaizId As String
    Public Property ArchivoId As String
    Public Property UsuarioRef As String
    Public Property AmbitoRef As String
    Public Property NivelRef As String
    Public Property SolicitudHash As String
    Public Property ContextoHash As String
    Public Property SnapshotHash As String
    Public Property RecepcionHash As String
    Public Property ValoresHash As String
    Public Property ExpedienteId As String
    Public Property RelacionNivelId As String
    Public Property OrigenGabinete As String
    Public Property GabineteId As String
    Public Property RevisionLote As UInteger
    Public Property RevisionRaiz As UInteger
    Public Property RevisionArchivo As UInteger
    Public Property Original As OriginalAlmacenamientoV2Importacion
End Class

Friend Class AutorizacionInvocacionV2Importacion
    Public Property Version As UShort
    Public Property Origen As String
    Public Property Clase As String
    Public Property IntentoId As String
    Public Property InicioHash As String
    Public Property AutorizacionRef As String
    Public Property RevisionLote As UInteger
End Class

Friend Class RetornoObservadoV2Importacion
    Public Property Version As UShort
    Public Property Origen As String
    Public Property Clase As String
    Public Property IntentoId As String
    Public Property InicioHash As String
    Public Property AutorizacionHash As String
    Public Property AutorizacionRef As String
    Public Property Estado As String
    Public Property DocumentoId As String
    Public Property InventarioId As String
End Class

Friend Class DocumentoAlmacenamientoV2Importacion
    Public Property Version As UShort
    Public Property Origen As String
    Public Property Fase As String
    Public Property Inicio As InicioAlmacenamientoV2Importacion
    Public Property Autorizacion As AutorizacionInvocacionV2Importacion
    Public Property Retorno As RetornoObservadoV2Importacion
End Class

Friend NotInheritable Class DocumentosAlmacenamientoV2Importacion
    Private Sub New()
    End Sub

    'Solo compone datos a partir del comando interno. No acredita al proveedor:
    'RV-02-B debe obtener la identidad bajo retención; RV-04 debe persistirla.
    Friend Shared Function Preparar(comando As ComandoAlmacenamientoCarpetas, original As OriginalAlmacenamientoV2Importacion, revisionLote As UInteger) As DocumentoAlmacenamientoV2Importacion
        If comando Is Nothing OrElse comando.EsVerificacion OrElse original Is Nothing Then Throw Fallo()
        Dim contexto = comando.Contexto, raiz = comando.Raiz, archivo = comando.Archivo, intento = comando.Intento
        Dim snapshot = comando.Snapshot
        If intento.Estado <> "RESERVADO" OrElse intento.Resultado IsNot Nothing OrElse intento.FinUtc IsNot Nothing OrElse Not Hexadecimal(snapshot.Huella, 64) Then Throw Fallo()
        If original.ContenidoHash <> archivo.ContenidoHash OrElse original.Bytes <> archivo.BytesRecibidos Then Throw Fallo()
        Dim inicio As New InicioAlmacenamientoV2Importacion With {
            .Version = 2, .Origen = "SERVIDOR", .Clase = "INICIO_ALMACENAMIENTO",
            .IntentoId = intento.IntentoId, .LoteId = intento.LoteId, .RaizId = intento.RaizId, .ArchivoId = intento.ArchivoId,
            .UsuarioRef = contexto.UsuarioRef, .AmbitoRef = contexto.AmbitoRef, .NivelRef = contexto.NivelRef,
            .SolicitudHash = intento.SolicitudHash,
            .ContextoHash = Huella(New With {contexto.UsuarioRef, contexto.AmbitoRef, contexto.NivelRef, contexto.RutaCanonica, contexto.SelloDestino}),
            .SnapshotHash = snapshot.Huella,
            .RecepcionHash = Huella(New With {archivo.StagingRef, archivo.BytesRecibidos, archivo.ContenidoHash}),
            .ValoresHash = Huella(archivo.ValoresEfectivos),
            .ExpedienteId = raiz.IdentidadExpediente.ExpedienteId, .RelacionNivelId = raiz.IdentidadExpediente.RelacionNivelId,
            .OrigenGabinete = snapshot.Configuracion.OrigenGabinete,
            .GabineteId = snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture),
            .RevisionLote = revisionLote, .RevisionRaiz = raiz.Revision, .RevisionArchivo = archivo.Revision, .Original = original}
        Return Copiar(New DocumentoAlmacenamientoV2Importacion With {.Version = 2, .Origen = "SERVIDOR", .Fase = "PREPARADO", .Inicio = inicio})
    End Function

    'Transformaciones puras: no son una autorización durable ni un acuse del gate.
    Friend Shared Function ConAutorizacion(documento As DocumentoAlmacenamientoV2Importacion, autorizacionRef As String, revisionLote As UInteger) As DocumentoAlmacenamientoV2Importacion
        Dim copia = Copiar(documento)
        If copia.Fase <> "PREPARADO" Then Throw Fallo()
        copia.Autorizacion = New AutorizacionInvocacionV2Importacion With {
            .Version = 2, .Origen = "SERVIDOR", .Clase = "AUTORIZAR_INVOCACION_DOCUMENTAL", .IntentoId = copia.Inicio.IntentoId,
            .InicioHash = Huella(copia.Inicio), .AutorizacionRef = autorizacionRef, .RevisionLote = revisionLote}
        copia.Fase = "AUTORIZADO"
        Return Copiar(copia)
    End Function

    Friend Shared Function ConRetorno(documento As DocumentoAlmacenamientoV2Importacion, resultado As ResultadoEfectoImportacion) As DocumentoAlmacenamientoV2Importacion
        Dim copia = Copiar(documento)
        If copia.Fase <> "AUTORIZADO" OrElse resultado Is Nothing OrElse resultado.Recepcion IsNot Nothing OrElse resultado.Creacion IsNot Nothing OrElse resultado.Expediente Is Nothing OrElse resultado.ValoresEfectivos Is Nothing Then Throw Fallo()
        Dim inicio = copia.Inicio
        If resultado.Expediente.OrigenLogico <> "RA" OrElse resultado.Expediente.ExpedienteId <> inicio.ExpedienteId OrElse resultado.Expediente.RelacionNivelId <> inicio.RelacionNivelId OrElse resultado.Expediente.NivelRef <> inicio.NivelRef OrElse resultado.OrigenGabinete <> inicio.OrigenGabinete OrElse resultado.GabineteId <> inicio.GabineteId OrElse Huella(resultado.ValoresEfectivos) <> inicio.ValoresHash Then Throw Fallo()
        'No persiste mensajes/códigos arbitrarios, rutas ni excepciones del escritor.
        copia.Retorno = New RetornoObservadoV2Importacion With {
            .Version = 2, .Origen = "SERVIDOR", .Clase = "OBSERVAR_RETORNO_DOCUMENTAL", .IntentoId = inicio.IntentoId,
            .InicioHash = Huella(inicio), .AutorizacionHash = Huella(copia.Autorizacion), .AutorizacionRef = copia.Autorizacion.AutorizacionRef,
            .Estado = resultado.Estado, .DocumentoId = resultado.DocumentoId, .InventarioId = resultado.InventarioId}
        copia.Fase = "RETORNO_OBSERVADO"
        Return Copiar(copia)
    End Function

    Friend Shared Function Leer(texto As String) As DocumentoAlmacenamientoV2Importacion
        Try
            Dim documento = PerfilImportacionJson.Leer(Of DocumentoAlmacenamientoV2Importacion)(texto)
            Validar(documento)
            Return documento
        Catch ex As PerfilImportacionException
            Throw Fallo()
        End Try
    End Function

    Friend Shared Function Escribir(documento As DocumentoAlmacenamientoV2Importacion) As String
        Validar(documento)
        Return PerfilImportacionJson.Escribir(documento)
    End Function

    Private Shared Function Copiar(documento As DocumentoAlmacenamientoV2Importacion) As DocumentoAlmacenamientoV2Importacion
        Return Leer(Escribir(documento))
    End Function

    Private Shared Sub Validar(documento As DocumentoAlmacenamientoV2Importacion)
        If documento Is Nothing OrElse documento.Version <> 2 OrElse documento.Origen <> "SERVIDOR" OrElse documento.Inicio Is Nothing Then Throw Fallo()
        Dim inicio = documento.Inicio
        If inicio.Version <> 2 OrElse inicio.Origen <> "SERVIDOR" OrElse inicio.Clase <> "INICIO_ALMACENAMIENTO" OrElse inicio.Original Is Nothing Then Throw Fallo()
        For Each id In New String() {inicio.IntentoId, inicio.LoteId, inicio.RaizId, inicio.ArchivoId, inicio.UsuarioRef, inicio.AmbitoRef, inicio.NivelRef, inicio.ExpedienteId, inicio.RelacionNivelId, inicio.GabineteId}
            If Not Referencia(id) Then Throw Fallo()
        Next
        For Each hash In New String() {inicio.SolicitudHash, inicio.ContextoHash, inicio.SnapshotHash, inicio.RecepcionHash, inicio.ValoresHash, inicio.Original.IdentidadFisicaHash, inicio.Original.ContenidoHash}
            If Not Hexadecimal(hash, 64) Then Throw Fallo()
        Next
        If Not Revision(inicio.RevisionLote) OrElse Not Revision(inicio.RevisionRaiz) OrElse Not Revision(inicio.RevisionArchivo) OrElse (inicio.OrigenGabinete <> "RA" AndAlso inicio.OrigenGabinete <> "DA") Then Throw Fallo()
        If Not Hexadecimal(inicio.Original.ProveedorRef, 32) OrElse Not Hexadecimal(inicio.Original.NodoRef, 32) OrElse Not Cantidad(inicio.Original.Bytes) Then Throw Fallo()
        Select Case documento.Fase
            Case "PREPARADO"
                If documento.Autorizacion IsNot Nothing OrElse documento.Retorno IsNot Nothing Then Throw Fallo()
            Case "AUTORIZADO", "RETORNO_OBSERVADO"
                Dim autorizacion = documento.Autorizacion
                If autorizacion Is Nothing OrElse autorizacion.Version <> 2 OrElse autorizacion.Origen <> "SERVIDOR" OrElse autorizacion.Clase <> "AUTORIZAR_INVOCACION_DOCUMENTAL" OrElse autorizacion.IntentoId <> inicio.IntentoId OrElse autorizacion.InicioHash <> Huella(inicio) OrElse Not Hexadecimal(autorizacion.AutorizacionRef, 32) OrElse Not Revision(autorizacion.RevisionLote) OrElse autorizacion.RevisionLote <= inicio.RevisionLote Then Throw Fallo()
                If documento.Fase = "AUTORIZADO" Then
                    If documento.Retorno IsNot Nothing Then Throw Fallo()
                Else
                    Dim retorno = documento.Retorno
                    If retorno Is Nothing OrElse retorno.Version <> 2 OrElse retorno.Origen <> "SERVIDOR" OrElse retorno.Clase <> "OBSERVAR_RETORNO_DOCUMENTAL" OrElse retorno.IntentoId <> inicio.IntentoId OrElse retorno.InicioHash <> Huella(inicio) OrElse retorno.AutorizacionHash <> Huella(autorizacion) OrElse retorno.AutorizacionRef <> autorizacion.AutorizacionRef Then Throw Fallo()
                    If retorno.Estado <> "CONFIRMADO" AndAlso retorno.Estado <> "INCIERTO" Then Throw Fallo()
                    If retorno.DocumentoId IsNot Nothing AndAlso Not Referencia(retorno.DocumentoId) Then Throw Fallo()
                    If retorno.InventarioId IsNot Nothing AndAlso Not Referencia(retorno.InventarioId) Then Throw Fallo()
                    If retorno.Estado = "CONFIRMADO" AndAlso (retorno.DocumentoId Is Nothing OrElse retorno.InventarioId Is Nothing) Then Throw Fallo()
                End If
            Case Else
                Throw Fallo()
        End Select
    End Sub

    Private Shared Function Huella(valor As Object) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(valor)))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Private Shared Function Hexadecimal(valor As String, longitud As Integer) As Boolean
        Return valor IsNot Nothing AndAlso valor.Length = longitud AndAlso Regex.IsMatch(valor, "\A[0-9a-f]+\z", RegexOptions.CultureInvariant)
    End Function
    Private Shared Function Referencia(valor As String) As Boolean
        Dim numero As Long
        Return Long.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, numero) AndAlso numero > 0 AndAlso numero.ToString(CultureInfo.InvariantCulture) = valor
    End Function
    Private Shared Function Cantidad(valor As String) As Boolean
        Dim numero As ULong
        Return ULong.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, numero) AndAlso numero > 0 AndAlso numero.ToString(CultureInfo.InvariantCulture) = valor
    End Function
    Private Shared Function Revision(valor As UInteger) As Boolean
        Return valor > 0 AndAlso valor < UInteger.MaxValue - 1UI
    End Function
    Private Shared Function Fallo() As PerfilImportacionException
        Return New PerfilImportacionException("EVIDENCIA_DOCUMENTAL_V2_INVALIDA", "Evidencia documental V2 inválida.")
    End Function
End Class
