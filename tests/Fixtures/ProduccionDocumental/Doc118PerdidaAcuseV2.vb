Option Strict On
Option Compare Binary
Imports System
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

'Solo se compila en el harness/paquete de prueba autorizado. No incluir en el
'vbproj productivo, exponer por HTTP o decidir activación desde una petición.
<Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)>
Friend NotInheritable Class Doc118PerdidaAcuseV2
    Implements IDisposable
    Private ReadOnly _sync As New Object()
    Private ReadOnly _huella As String
    Private ReadOnly _revision As UInteger
    Private ReadOnly _objetivo As Integer
    Private _siguiente As Integer, _callbacks As Integer, _acuses As Integer, _perdidas As Integer
    Private _ocupado As Boolean, _cerrado As Boolean, _decorado As Boolean

    Friend Sub New(runId As String, fase As String, comando As ComandoAlmacenamientoCarpetas, revisionLote As UInteger)
        If Not Regex.IsMatch(If(runId, ""), "\ADOC118-V2-[A-Za-z0-9_-]{1,48}\z") OrElse comando Is Nothing OrElse comando.EsVerificacion OrElse revisionLote = 0 Then Throw Invalida()
        _objetivo = Array.IndexOf(New String() {"INICIO", "AUTORIZAR", "OBSERVAR"}, fase)
        'Preparar ya exigió la reserva nueva; EsNuevaReserva es JsonIgnore y
        'no forma parte de la copia inmutable que expone el comando.
        If _objetivo < 0 OrElse comando.Intento.Estado <> "RESERVADO" OrElse comando.Intento.EjecutorRef <> "WEB_DOCUMENTO" OrElse {"18", "21", "24"}.Contains(comando.Intento.LoteId) OrElse {"41", "53", "69"}.Contains(comando.Intento.IntentoId) Then Throw Invalida()
        _huella = Huella(comando) : _revision = revisionLote
    End Sub

    Friend Function Decorar(nativa As PersistenciaDocumentoV2Importacion) As PersistenciaDocumentoV2Importacion
        SyncLock _sync
            If nativa Is Nothing OrElse _decorado OrElse _cerrado Then Throw Invalida()
            _decorado = True
            Return New PersistenciaDocumentoV2Importacion(
                Function(c, d, doc) Paso(0, c, doc, Function() nativa.Iniciar(c, d, doc)),
                Function(c, doc) Paso(1, c, doc, Function() nativa.AutorizarInvocacion(c, doc)),
                Function(c, doc) Paso(2, c, doc, Function() nativa.ObservarRetorno(c, doc)),
                Function(c, doc, resultado)
                    'Las tres ventanas seleccionables interrumpen antes de este
                    'callback. Jamás guardar un checkpoint desde el decorador.
                    SyncLock _sync
                        _cerrado = True
                        Throw Invalida()
                    End SyncLock
                End Function)
        End SyncLock
    End Function

    Private Function Paso(indice As Integer, comando As ComandoAlmacenamientoCarpetas, documento As DocumentoAlmacenamientoV2Importacion, ejecutar As Func(Of AcuseDocumentoV2Importacion)) As AcuseDocumentoV2Importacion
        SyncLock _sync
            If _cerrado OrElse _ocupado OrElse indice <> _siguiente Then
                _cerrado = True : Throw Invalida()
            End If
            _ocupado = True
            Try
                If comando Is Nothing OrElse documento Is Nothing OrElse Huella(comando) <> _huella OrElse documento.Inicio.RevisionLote <> _revision Then Throw Invalida()
                Dim fasesEntrada = New String() {"PREPARADO", "PREPARADO", "RETORNO_OBSERVADO"}
                If documento.Fase <> fasesEntrada(indice) Then Throw Invalida()
                DocumentosAlmacenamientoV2Importacion.Escribir(documento)
                _callbacks += 1
                Dim acuse = ejecutar()
                If acuse Is Nothing OrElse _cerrado Then Throw Invalida()
                Dim recibido = acuse.Documento
                Dim fasesSalida = New String() {"PREPARADO", "AUTORIZADO", "RETORNO_OBSERVADO"}
                If recibido.Fase <> fasesSalida(indice) OrElse PerfilImportacionJson.Escribir(recibido.Inicio) <> PerfilImportacionJson.Escribir(documento.Inicio) Then Throw Invalida()
                If indice <> 1 AndAlso DocumentosAlmacenamientoV2Importacion.Escribir(recibido) <> DocumentosAlmacenamientoV2Importacion.Escribir(documento) Then Throw Invalida()
                _acuses += 1 : _siguiente += 1
                If indice = _objetivo Then
                    _cerrado = True : _perdidas += 1
                    Throw New PerfilImportacionException("DOC118_ACUSE_PERDIDO", "Acuse de prueba no entregado.")
                End If
                Return acuse
            Catch
                _cerrado = True
                Throw
            Finally
                _ocupado = False
            End Try
        End SyncLock
    End Function

    Friend Function Observacion() As Object
        SyncLock _sync
            Dim fases = New String() {"INICIO", "AUTORIZAR", "OBSERVAR"}
            Return New With {.fase = fases(_objetivo), .callbacks = _callbacks, .acusesValidos = _acuses, .perdidas = _perdidas, .cerrado = _cerrado, .ocupado = _ocupado}
        End SyncLock
    End Function
    Public Sub Dispose() Implements IDisposable.Dispose
        SyncLock _sync
            _cerrado = True
        End SyncLock
    End Sub
    Private Shared Function Huella(c As ComandoAlmacenamientoCarpetas) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {c.Contexto, c.Raiz, c.Archivo, c.Snapshot, c.Intento, c.EsVerificacion})))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("DOC118_PRUEBA_NO_DISPONIBLE", "El enlace de prueba no está disponible para esta operación.")
    End Function
End Class
