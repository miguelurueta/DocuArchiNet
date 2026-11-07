Option Strict On
Option Compare Binary
Imports System
Imports System.IO
Imports System.Text
Imports System.Linq
Imports System.Globalization

' LAB ONLY. Default constants deliberately reject all execution. No product hooks.
Friend NotInheritable Class Doc117InternalDocumentCandidate
    Private Const SitioAutorizado As String = "NO_AUTORIZADO"
    Private Const Ejecucion As String = "NO_AUTORIZADA"
    Private Const LoteAutorizado As String = "NO_AUTORIZADO"
    Private Const HuellaSnapshot As String = "NO_AUTORIZADA"
    Private Const RaizA As String = "NO_AUTORIZADA_A"
    Private Const ArchivoA As String = "NO_AUTORIZADO_A"
    Private Const ExpedienteA As String = "NO_AUTORIZADO_A"
    Private Const RelacionA As String = "NO_AUTORIZADA_A"
    Private Const NombreA As String = "NO_AUTORIZADO_A"
    Private Const BytesA As String = "NO_AUTORIZADOS_A"
    Private Const ContenidoA As String = "NO_AUTORIZADO_A"
    Private Const SolicitudA As String = "NO_AUTORIZADA_A"
    Private Const RaizB As String = "NO_AUTORIZADA_B"
    Private Const ArchivoB As String = "NO_AUTORIZADO_B"
    Private Const ExpedienteB As String = "NO_AUTORIZADO_B"
    Private Const RelacionB As String = "NO_AUTORIZADA_B"
    Private Const NombreB As String = "NO_AUTORIZADO_B"
    Private Const BytesB As String = "NO_AUTORIZADOS_B"
    Private Const ContenidoB As String = "NO_AUTORIZADO_B"
    Private Const SolicitudB As String = "NO_AUTORIZADA_B"
    Private Shared ReadOnly Diario As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "DOC117-ALM02-" & Ejecucion)
    Private Sub New()
    End Sub
    Friend Shared Sub ExigirAlcance(comando As ComandoAlmacenamientoCarpetas, Optional exigirFijacion As Boolean = False)
        If Not ValidadorPerfilImportacion.ReferenciaValida(LoteAutorizado) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Candidata de laboratorio sin autorizacion de ejecucion.")
        If Not String.Equals(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar), SitioAutorizado.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then Throw Fallo("SCOPE_INVALID")
        If comando Is Nothing OrElse comando.EsVerificacion OrElse comando.Contexto Is Nothing OrElse comando.Snapshot Is Nothing OrElse comando.Snapshot.Configuracion Is Nothing OrElse comando.Intento Is Nothing OrElse comando.Raiz Is Nothing OrElse comando.Archivo Is Nothing Then Throw Fallo("SCOPE_INVALID")
        Dim c = comando.Contexto, s = comando.Snapshot, i = comando.Intento, r = comando.Raiz, a = comando.Archivo
        If r.LoteId <> LoteAutorizado OrElse i.LoteId <> LoteAutorizado OrElse c.UsuarioRef <> "141" OrElse c.AmbitoRef <> "2" OrElse c.NivelRef <> "7" OrElse s.VersionId <> "38" OrElse s.Configuracion.OrigenGabinete <> "DA" OrElse s.Configuracion.GabineteId <> 10 OrElse s.Huella <> HuellaSnapshot Then Throw Fallo("SCOPE_INVALID")
        If r.IdentidadExpediente Is Nothing OrElse r.IdentidadExpediente.OrigenLogico <> "RA" OrElse r.IdentidadExpediente.NivelRef <> "7" Then Throw Fallo("SCOPE_INVALID")
        Dim esA = r.RaizId = RaizA AndAlso a.ArchivoId = ArchivoA
        Dim esB = r.RaizId = RaizB AndAlso a.ArchivoId = ArchivoB
        'EsNuevaReserva is transient/JsonIgnore and is already checked by Preparar.
        'The immutable command intentionally does not carry that flag forward.
        If Not (esA OrElse esB) OrElse a.RaizId <> r.RaizId OrElse i.RaizId <> r.RaizId OrElse i.ArchivoId <> a.ArchivoId OrElse i.Fase <> "ALMACENAR" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(i.IntentoId) OrElse i.IntentoId = "41" OrElse i.Estado <> "RESERVADO" OrElse i.FinUtc IsNot Nothing OrElse i.Resultado IsNot Nothing Then Throw Fallo("SCOPE_INVALID")
        If r.IdentidadExpediente.ExpedienteId <> If(esA, ExpedienteA, ExpedienteB) OrElse r.IdentidadExpediente.RelacionNivelId <> If(esA, RelacionA, RelacionB) OrElse a.NombreOriginal <> If(esA, NombreA, NombreB) OrElse a.BytesPrevistos <> If(esA, BytesA, BytesB) OrElse a.BytesRecibidos <> a.BytesPrevistos OrElse a.ContenidoHash <> If(esA, ContenidoA, ContenidoB) OrElse i.SolicitudHash <> If(esA, SolicitudA, SolicitudB) Then Throw Fallo("SCOPE_INVALID")
        ExigirRuta(Diario)
        If File.Exists(Marca(comando)) Then Throw Fallo("ALREADY_OBSERVED")
        If exigirFijacion OrElse File.Exists(Fijacion(comando)) Then ComprobarFijacion(comando)
    End Sub
    Private Shared Function Fallo(codigo As String) As InvalidOperationException
        Return New InvalidOperationException("DOC117_ALM02_" & codigo)
    End Function
    Private Shared Sub ExigirRuta(ruta As String)
        Dim actual = Path.GetFullPath(ruta), sitio = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar)
        If Not actual.StartsWith(sitio & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then Throw Fallo("PIN_PATH_INVALID")
        Do While actual.Length > sitio.Length
            If (File.Exists(actual) OrElse Directory.Exists(actual)) AndAlso (File.GetAttributes(actual) And FileAttributes.ReparsePoint) <> 0 Then Throw Fallo("PIN_PATH_INVALID")
            actual = Path.GetDirectoryName(actual)
        Loop
        If (File.GetAttributes(sitio) And FileAttributes.ReparsePoint) <> 0 Then Throw Fallo("PIN_PATH_INVALID")
    End Sub
    Private Shared Function Fijacion(comando As ComandoAlmacenamientoCarpetas) As String
        Return Path.Combine(Diario, "intento-archivo-" & comando.Archivo.ArchivoId & ".json")
    End Function
    Private Shared Function Marca(comando As ComandoAlmacenamientoCarpetas) As String
        Return Path.Combine(Diario, "archivo-" & comando.Archivo.ArchivoId & ".json")
    End Function
    Private Shared Function EntradaEscritor(comando As ComandoAlmacenamientoCarpetas) As String
        Return Path.Combine(Diario, "escritor-archivo-" & comando.Archivo.ArchivoId & ".json")
    End Function
    Friend Shared Sub Invocar(comando As ComandoAlmacenamientoCarpetas, invocarReal As Action)
        ExigirAlcance(comando, True)
        If invocarReal Is Nothing Then Throw Fallo("WRITER_NOT_AVAILABLE")
        Dim ruta = EntradaEscritor(comando), datos = DatosFijacion(comando)
        ExigirRuta(ruta)
        Try
            Using archivo As New FileStream(ruta, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                archivo.Write(datos, 0, datos.Length)
                archivo.Flush(True)
            End Using
        Catch
            Throw Fallo("WRITER_ALREADY_OBSERVED")
        End Try
        invocarReal()
    End Sub
    Private Shared Sub ComprobarEntradaEscritor(comando As ComandoAlmacenamientoCarpetas)
        Dim ruta = EntradaEscritor(comando)
        ExigirRuta(ruta)
        Try
            If Not File.ReadAllBytes(ruta).SequenceEqual(DatosFijacion(comando)) Then Throw Fallo("WRITER_NOT_VERIFIED")
        Catch
            Throw Fallo("WRITER_NOT_VERIFIED")
        End Try
    End Sub
    Private Shared Function DatosFijacion(comando As ComandoAlmacenamientoCarpetas) As Byte()
        Return New UTF8Encoding(False).GetBytes(PerfilImportacionJson.Escribir(New With {.version = 1, .loteId = comando.Raiz.LoteId, .raizId = comando.Raiz.RaizId, .archivoId = comando.Archivo.ArchivoId, .intentoId = comando.Intento.IntentoId, .solicitudHash = comando.Intento.SolicitudHash}))
    End Function
    Private Shared Sub ComprobarFijacion(comando As ComandoAlmacenamientoCarpetas)
        Try
            Dim ruta = Fijacion(comando), esperado = DatosFijacion(comando)
            ExigirRuta(ruta)
            Using archivo As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read)
                If archivo.Length <> esperado.Length OrElse archivo.Length > 4096 Then Throw Fallo("PIN_CHANGED")
                Dim leido(esperado.Length - 1) As Byte, total As Integer = 0
                While total < leido.Length
                    Dim n = archivo.Read(leido, total, leido.Length - total)
                    If n = 0 Then Throw Fallo("PIN_CHANGED")
                    total += n
                End While
                If Not esperado.SequenceEqual(leido) Then Throw Fallo("PIN_CHANGED")
            End Using
        Catch
            Throw Fallo("PIN_NOT_VERIFIED")
        End Try
    End Sub
    Friend Shared Function Iniciar(comando As ComandoAlmacenamientoCarpetas, iniciarReal As Func(Of Boolean)) As Boolean
        ExigirAlcance(comando)
        If iniciarReal Is Nothing Then Throw Fallo("PIN_NOT_CREATED")
        Try
            Directory.CreateDirectory(Diario)
            Dim ruta = Fijacion(comando), datos = DatosFijacion(comando)
            ExigirRuta(ruta)
            Using archivo As New FileStream(ruta, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                archivo.Write(datos, 0, datos.Length)
                archivo.Flush(True)
            End Using
        Catch
            'Never remove or replace a failed/previous claim to permit a retry.
            Throw Fallo("PIN_NOT_CREATED")
        End Try
        ComprobarFijacion(comando)
        Return iniciarReal()
    End Function
    Private Shared Sub RegistrarMarca(comando As ComandoAlmacenamientoCarpetas, resultado As ResultadoEfectoImportacion, fase As String)
        Dim datos = PerfilImportacionJson.Escribir(New With {.modo = fase, .loteId = comando.Raiz.LoteId, .raizId = comando.Raiz.RaizId, .archivoId = comando.Archivo.ArchivoId, .intentoId = comando.Intento.IntentoId, .documentoId = resultado.DocumentoId, .inventarioId = resultado.InventarioId, .estado = resultado.Estado})
        Dim ruta = Marca(comando)
        ExigirRuta(ruta)
        Dim bytes = New UTF8Encoding(False).GetBytes(datos)
        Using archivo As New FileStream(ruta, FileMode.CreateNew, FileAccess.Write, FileShare.None)
            archivo.Write(bytes, 0, bytes.Length)
            archivo.Flush(True)
        End Using
    End Sub
    Friend Shared Function Registrar(comando As ComandoAlmacenamientoCarpetas, resultado As ResultadoEfectoImportacion, registrarReal As Func(Of ComandoAlmacenamientoCarpetas, ResultadoEfectoImportacion, ResultadoEfectoImportacion)) As ResultadoEfectoImportacion
        ExigirAlcance(comando, True)
        If resultado Is Nothing OrElse resultado.Estado <> "CONFIRMADO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId) OrElse registrarReal Is Nothing Then Throw Fallo("RESULT_NOT_CONFIRMED")
        ComprobarEntradaEscritor(comando)
        If comando.Archivo.ArchivoId = ArchivoB Then
            RegistrarMarca(comando, resultado, "AFTER_VERIFICATION_BEFORE_CHECKPOINT")
            Throw Fallo("INTERNAL_FAILURE")
        End If
        Dim guardado = registrarReal(comando, resultado)
        If guardado Is Nothing OrElse guardado.Estado <> "CONFIRMADO" OrElse guardado.DocumentoId <> resultado.DocumentoId OrElse guardado.InventarioId <> resultado.InventarioId Then Throw Fallo("CHECKPOINT_NOT_CONFIRMED")
        RegistrarMarca(comando, guardado, "AFTER_CHECKPOINT_COMMIT_BEFORE_RETURN")
        Throw Fallo("INTERNAL_FAILURE")
    End Function
End Class
