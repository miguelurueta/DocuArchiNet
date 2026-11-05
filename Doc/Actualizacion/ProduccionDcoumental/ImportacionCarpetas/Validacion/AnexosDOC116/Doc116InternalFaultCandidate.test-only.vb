Option Strict On
Imports System
Imports System.IO
Imports System.Text

' LAB ONLY. SCOPED CANDIDATE. Activation still requires explicit authorization.
' Not included in the production project; compiled only by the overlay target.
Friend NotInheritable Class Doc116InternalFaultCandidate
    Private Const LoteAutorizado As String = "18"
    Private Const RaizAntesCheckpoint As String = "30"
    Private Const RaizRetornoCheckpoint As String = "29"
    Private Shared ReadOnly Diario As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "DOC116-INTERNAL-LOTE18")
    Private Sub New()
    End Sub
    Friend Shared Sub ExigirAlcance(comando As ComandoCreacionExpedienteImportacion)
        If Not String.Equals(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar), "D:\imagenesda\DocuachiNet\DocuArchiNet", StringComparison.OrdinalIgnoreCase) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Sitio de laboratorio distinto.")
        If Not ValidadorPerfilImportacion.ReferenciaValida(LoteAutorizado) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Candidata de laboratorio sin autorizacion de ejecucion.")
        If comando Is Nothing OrElse comando.EsVerificacion OrElse comando.Contexto Is Nothing OrElse comando.Snapshot Is Nothing Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Alcance de laboratorio no autorizado.")
        If comando.LoteId <> LoteAutorizado OrElse Not {RaizAntesCheckpoint, RaizRetornoCheckpoint}.Contains(comando.RaizId) OrElse comando.Contexto.UsuarioRef <> "141" OrElse comando.Contexto.AmbitoRef <> "2" OrElse comando.Contexto.NivelRef <> "215" OrElse comando.Snapshot.VersionId <> "38" Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Alcance de laboratorio no autorizado.")
        If File.Exists(Marca(comando)) Then Throw New PerfilImportacionException("RESULTADO_INCIERTO", "La inyeccion ya fue observada; consultar evidencia sin recrear.")
        Directory.CreateDirectory(Diario)
        Using prueba As New FileStream(Path.Combine(Diario, "acceso-" & comando.RaizId & ".probe"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
            prueba.Flush(True)
        End Using
    End Sub
    Private Shared Function Marca(comando As ComandoCreacionExpedienteImportacion) As String
        If Not ValidadorPerfilImportacion.ReferenciaValida(comando.RaizId) Then Throw New InvalidOperationException("DOC116_CANDIDATE_SCOPE_INVALID")
        Return Path.Combine(Diario, "raiz-" & comando.RaizId & ".json")
    End Function
    Private Shared Sub RegistrarMarca(comando As ComandoCreacionExpedienteImportacion, resultado As ResultadoEfectoImportacion, fase As String)
        Dim datos = PerfilImportacionJson.Escribir(New With {.modo = fase, .loteId = comando.LoteId, .raizId = comando.RaizId, .intentoId = comando.IntentoId, .expediente = resultado.Expediente, .creacion = resultado.Creacion, .estado = resultado.Estado})
        Directory.CreateDirectory(Diario)
        Dim bytes = New UTF8Encoding(False).GetBytes(datos)
        Using archivo As New FileStream(Marca(comando), FileMode.CreateNew, FileAccess.Write, FileShare.None)
            archivo.Write(bytes, 0, bytes.Length)
            archivo.Flush(True)
        End Using
    End Sub
    Friend Shared Function Registrar(repo As MySqlImportacionCarpetasRepository, comando As ComandoCreacionExpedienteImportacion, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        ExigirAlcance(comando)
        If resultado Is Nothing OrElse resultado.Estado <> "CONFIRMADO" OrElse resultado.Expediente Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.ExpedienteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.RelacionNivelId) Then Throw New InvalidOperationException("DOC116_CANDIDATE_RESULT_NOT_CONFIRMED")
        If comando.RaizId = RaizAntesCheckpoint Then
            RegistrarMarca(comando, resultado, "AFTER_LEGACY_BEFORE_CHECKPOINT")
            Throw New InvalidOperationException("DOC116_CANDIDATE_INTERNAL_FAILURE")
        End If
        Dim guardado = repo.RegistrarResultadoCreacion(comando, resultado)
        If guardado Is Nothing OrElse guardado.Estado <> "CONFIRMADO" Then Throw New InvalidOperationException("DOC116_CANDIDATE_CHECKPOINT_NOT_CONFIRMED")
        RegistrarMarca(comando, guardado, "AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN")
        Throw New InvalidOperationException("DOC116_CANDIDATE_INTERNAL_FAILURE")
    End Function
End Class
