Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic

'Solo puertos de consulta. No tiene referencias a writers, staging o reservas.
Public NotInheritable Class VerificadorReconciliacionImportacion
    Private ReadOnly _datosCreacion As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String))
    Private ReadOnly _verificarCreacion As Func(Of ComandoCreacionExpedienteImportacion, IdentidadExpedienteImportacion, Dictionary(Of String, String), Boolean)
    Private ReadOnly _datosDocumento As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion)
    Private ReadOnly _verificarDocumento As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean)
    Public Sub New(catalogos As PerfilImportacionCatalogAdapter, repositorio As MySqlImportacionCarpetasRepository)
        Me.New(AddressOf catalogos.PrepararDatosCreacionLegacy, AddressOf repositorio.ComprobarResultadoCreacion, AddressOf catalogos.PrepararDatosAlmacenamiento, AddressOf catalogos.ComprobarAlmacenamiento)
    End Sub
    Friend Sub New(datosCreacion As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String)), verificarCreacion As Func(Of ComandoCreacionExpedienteImportacion, IdentidadExpedienteImportacion, Dictionary(Of String, String), Boolean), datosDocumento As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion), verificarDocumento As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean))
        If datosCreacion Is Nothing OrElse verificarCreacion Is Nothing OrElse datosDocumento Is Nothing OrElse verificarDocumento Is Nothing Then Throw New ArgumentNullException()
        _datosCreacion = datosCreacion : _verificarCreacion = verificarCreacion : _datosDocumento = datosDocumento : _verificarDocumento = verificarDocumento
    End Sub
    Public Function Verificar(vista As VistaReconciliacionImportacion) As ResultadoEfectoImportacion
        If Not CoordinadorImportacionCarpetas.PuedeReconciliar(vista) Then Return Nothing
        Dim resultado = MySqlImportacionCarpetasRepository.ResultadoConfirmadoReconciliacion(vista)
        If vista.Intento.Fase = "CREAR_EXPEDIENTE" Then
            Dim comando = ComandoCreacionExpedienteImportacion.PrepararVerificacion(vista.Lote.Contexto, vista.Lote, vista.Raiz, vista.Intento)
            Dim datos = _datosCreacion(comando)
            If datos Is Nothing OrElse Not datos.ContainsKey("CICLO_ARCHIVO_ID") OrElse datos("CICLO_ARCHIVO_ID") <> vista.Intento.Resultado.Creacion.CicloArchivoId Then Return Nothing
            If Not _verificarCreacion(comando, vista.Intento.Resultado.Expediente, datos) Then Return Nothing
        Else
            Dim comando = ComandoAlmacenamientoCarpetas.PrepararVerificacion(vista.Lote.Contexto, vista.Raiz, vista.Archivo, vista.Lote.Snapshot, vista.Intento)
            Dim datos = _datosDocumento(comando)
            If Not _verificarDocumento(comando, datos, vista.Intento.Resultado) Then Return Nothing
        End If
        Return resultado
    End Function
End Class
