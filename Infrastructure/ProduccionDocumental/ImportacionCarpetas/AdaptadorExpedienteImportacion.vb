Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic

'Interno, sin endpoint. El coordinador debe comprobar la barrera antes de
'llamar. No repite efectos ni declara ausencia por un fallo de legacy.
Public Partial Class AdaptadorExpedienteImportacion
    Implements ICreacionExpedienteImportacion
    Private ReadOnly _autorizar As Action(Of ComandoCreacionExpedienteImportacion)
    Private ReadOnly _catalogos As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String))
    Private ReadOnly _iniciar As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String), Boolean)
    Private ReadOnly _invocar As Action(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String), SalidaCreacionExpedienteLegacy)
    Private ReadOnly _verificar As Func(Of ComandoCreacionExpedienteImportacion, IdentidadExpedienteImportacion, Dictionary(Of String, String), Boolean)
    Private ReadOnly _registrar As Func(Of ComandoCreacionExpedienteImportacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion)
    Private ReadOnly _conservar As Func(Of IDisposable)

    Friend Sub New(autorizar As Action(Of ComandoCreacionExpedienteImportacion), catalogos As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String)), iniciar As Func(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String), Boolean), invocar As Action(Of ComandoCreacionExpedienteImportacion, Dictionary(Of String, String), SalidaCreacionExpedienteLegacy), verificar As Func(Of ComandoCreacionExpedienteImportacion, IdentidadExpedienteImportacion, Dictionary(Of String, String), Boolean), registrar As Func(Of ComandoCreacionExpedienteImportacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion), conservar As Func(Of IDisposable))
        If autorizar Is Nothing OrElse catalogos Is Nothing OrElse iniciar Is Nothing OrElse invocar Is Nothing OrElse verificar Is Nothing OrElse registrar Is Nothing OrElse conservar Is Nothing Then Throw New ArgumentNullException()
        _autorizar = autorizar : _catalogos = catalogos : _iniciar = iniciar : _invocar = invocar : _verificar = verificar
        _registrar = registrar
        _conservar = conservar
    End Sub

    Public Function Crear(contexto As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion Implements ICreacionExpedienteImportacion.Crear
        Dim comando = ComandoCreacionExpedienteImportacion.Preparar(contexto, lote, raiz, intento)
        comando.ValidarEntradasLegacy()
        _autorizar(comando)
        Dim originales = _catalogos(comando)
        If originales Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el catálogo de creación.")
        Dim datos As New Dictionary(Of String, String)(originales, StringComparer.Ordinal)
        comando.ValidarDatosLegacy(datos)
        _autorizar(comando)
        If Not _iniciar(comando, New Dictionary(Of String, String)(datos, StringComparer.Ordinal)) Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "El intento ya no puede iniciar creación.")
        intento.EsNuevaReserva = False
        Dim retenciones As IDisposable = Nothing
        Try
            Dim salida As New SalidaCreacionExpedienteLegacy()
            Dim llamadaConcluida As Boolean = False
            Dim resultado As New ResultadoEfectoImportacion With {.Estado = "INCIERTO", .Codigo = "RESULTADO_INCIERTO", .Mensaje = "La creación requiere comprobación antes de cualquier repetición.",
                .Creacion = New EvidenciaCreacionExpedienteImportacion With {.IntentoId = comando.IntentoId, .CicloArchivoId = datos("CICLO_ARCHIVO_ID"), .CicloArchivoNombre = datos("CICLO_ARCHIVO_NOMBRE")}}
            Try
                _autorizar(comando)
                retenciones = _conservar()
                If retenciones Is Nothing Then Throw New PerfilImportacionException("RESULTADO_INCIERTO", "No se pudo conservar el staging verificado.")
                _autorizar(comando)
                _invocar(comando, New Dictionary(Of String, String)(datos, StringComparer.Ordinal), salida)
                llamadaConcluida = True
            Catch ex As Exception
                'El escritor puede haber confirmado o asignado IDs antes del fallo.
                'No incluir su mensaje (puede contener SQL/rutas) ni reintentar.
            End Try
            If salida.ExpedienteId > 0 OrElse salida.RelacionNivelId > 0 Then
                resultado.Expediente = New IdentidadExpedienteImportacion With {.OrigenLogico = "RA", .NivelRef = comando.Contexto.NivelRef,
                    .ExpedienteId = If(salida.ExpedienteId > 0, salida.ExpedienteId.ToString(Globalization.CultureInfo.InvariantCulture), Nothing),
                    .RelacionNivelId = If(salida.RelacionNivelId > 0, salida.RelacionNivelId.ToString(Globalization.CultureInfo.InvariantCulture), Nothing)}
            End If
            If Not llamadaConcluida OrElse salida.Respuesta <> "YES" OrElse salida.ExpedienteId <= 0 OrElse salida.RelacionNivelId <= 0 Then Return RegistrarSalida(comando, resultado)
            Try
                _autorizar(comando)
                If _verificar(comando, resultado.Expediente, New Dictionary(Of String, String)(datos, StringComparer.Ordinal)) Then
                    resultado.Estado = "CONFIRMADO" : resultado.Codigo = "EXPEDIENTE_CONFIRMADO" : resultado.Mensaje = "Expediente y relación comprobados en el destino confirmado."
                End If
            Catch ex As Exception
                'Conservar IDs/evidencia aun si la autoridad o la lectura ya no es valida.
            End Try
            Return RegistrarSalida(comando, resultado)
        Finally
            If retenciones IsNot Nothing Then retenciones.Dispose()
        End Try
    End Function

    Private Function RegistrarSalida(comando As ComandoCreacionExpedienteImportacion, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        Try
            Dim copia = PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(resultado))
            Dim guardado = _registrar(comando, copia)
            If guardado IsNot Nothing AndAlso PerfilImportacionJson.Escribir(guardado) = PerfilImportacionJson.Escribir(resultado) Then Return guardado
        Catch ex As Exception
            'Puede haber commit sin respuesta. Consultar el intento; nunca
            'volver a invocar al escritor para reparar un checkpoint perdido.
        End Try
        resultado.Estado = "INCIERTO" : resultado.Codigo = "RESULTADO_INCIERTO"
        resultado.Mensaje = "La creación requiere comprobación antes de cualquier repetición."
        Return resultado
    End Function
End Class

Public Class SalidaCreacionExpedienteLegacy
    Public Property Respuesta As String
    Public Property ExpedienteId As Integer
    Public Property RelacionNivelId As Integer
End Class
