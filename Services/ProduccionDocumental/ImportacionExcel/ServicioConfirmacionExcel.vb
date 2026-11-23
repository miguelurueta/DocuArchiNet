Option Strict On
Imports System
Imports System.Linq
Imports Newtonsoft.Json.Linq

Public Class ServicioConfirmacionExcel
    Private ReadOnly _contexto As Func(Of ContextoCargaExcel)
    Private ReadOnly _cargas As IConfirmacionExcelRepository
    Private ReadOnly _almacen As Func(Of IAlmacenArchivoExcel)
    Private ReadOnly _limites As Func(Of LimitesLecturaExcel)
    Public Sub New(contexto As Func(Of ContextoCargaExcel), cargas As IConfirmacionExcelRepository, almacen As IAlmacenArchivoExcel, limites As Func(Of LimitesLecturaExcel))
        Me.New(contexto, cargas, Function() almacen, limites)
    End Sub
    Public Sub New(contexto As Func(Of ContextoCargaExcel), cargas As IConfirmacionExcelRepository, almacen As Func(Of IAlmacenArchivoExcel), limites As Func(Of LimitesLecturaExcel))
        _contexto = contexto : _cargas = cargas : _almacen = almacen : _limites = limites
    End Sub
    Private Function Contexto(s As SolicitudConsultaCargaExcel) As ContextoCargaExcel
        Dim c = _contexto()
        ReglasCargaExcel.Autorizar(c)
        If s Is Nothing OrElse s.ConexionId <> c.ConexionId OrElse Not ReglasCargaExcel.IdValido(s.IdCarga) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        Return c
    End Function
    Public Shared Sub Identidad(s As SolicitudConfirmarExcel, e As CargaExcelEjecucion)
        If s Is Nothing OrElse Not ReglasCargaExcel.HashValido(s.ArchivoHash) OrElse Not ReglasCargaExcel.HashValido(s.ConfiguracionHash) OrElse Not ReglasCargaExcel.HashValido(s.EsquemaHash) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        If e Is Nothing Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        If e.Id <> s.IdCarga OrElse e.Archivo Is Nothing OrElse e.Archivo.Hash <> s.ArchivoHash OrElse e.ConfiguracionHash <> s.ConfiguracionHash OrElse e.EsquemaHash <> s.EsquemaHash Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
        If e.Modo = "REEMPLAZAR" AndAlso Not s.ConfirmacionReemplazo Then Throw ReglasCargaExcel.Fallo("CONFIRMACION_REEMPLAZO_REQUERIDA")
    End Sub
    Public Shared Function Documento(e As CargaExcelEjecucion) As JObject
        Try
            If Not ReglasCargaExcel.HashValido(e.ConfiguracionHash) OrElse ReglasCargaExcel.Hash(Text.Encoding.UTF8.GetBytes(If(e.ConfiguracionDoc, ""))) <> e.ConfiguracionHash Then Throw New FormatException()
            Dim doc = JObject.Parse(e.ConfiguracionDoc)
            If doc.Value(Of Integer)("Version") <> 1 OrElse doc.Value(Of Integer)("PlantillaVersion") <> 1 Then Throw New FormatException()
            If Not ReglasCargaExcel.HashValido(doc.Value(Of String)("PlantillaHash")) OrElse String.IsNullOrEmpty(doc.Value(Of String)("Hoja")) Then Throw New FormatException()
            Return doc
        Catch
            Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        End Try
    End Function
    Public Function ConsultarResultado(s As SolicitudConsultaCargaExcel) As EstadoCargaExcel
        Dim c = Contexto(s), e = _cargas.Consultar(c, s.IdCarga)
        If e Is Nothing Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        ReglasCargaExcel.Propietario(c, e)
        Dim resultado = ReglasCargaExcel.Estado(e)
        'CONFIRMADA describe la ejecución histórica: no depende del archivo/esquema actual.
        If e.Modo = "CREAR" AndAlso e.Estado <> "CONFIRMADA" Then
            resultado.EstadoEstructura = "SIN_VERIFICAR"
            Try
                Dim p = Documento(e)("Plantilla").ToObject(Of PlantillaExcel)()
                If p IsNot Nothing AndAlso p.ID_PLANTILLAIMP = e.PlantillaId AndAlso p.NOMBRE_PLANTILLA = e.Tabla Then resultado.EstadoEstructura = _cargas.ConsultarEstructura(c, e, p)
            Catch ex As ImportacionExcelException
                resultado.EstadoEstructura = "SIN_VERIFICAR"
            End Try
        End If
        ReglasCargaExcel.MismaAutoridad(c, _contexto())
        Return resultado
    End Function
    Public Function Confirmar(s As SolicitudConfirmarExcel) As EstadoCargaExcel
        Dim c = Contexto(s), e = _cargas.Consultar(c, s.IdCarga)
        If e Is Nothing Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        ReglasCargaExcel.Propietario(c, e) : Identidad(s, e)
        If e.Estado = "CONFIRMADA" Then
            ReglasCargaExcel.MismaAutoridad(c, _contexto())
            Return ReglasCargaExcel.Estado(e)
        End If
        Using sesion = _cargas.AbrirConfirmacion(c, s.IdCarga)
            e = sesion.Obtener()
            ReglasCargaExcel.Propietario(c, e) : Identidad(s, e)
            If e.Estado = "CONFIRMADA" Then Return Historico(c, e)
            If e.Estado <> "PREPARADA" AndAlso e.Estado <> "FALLIDA" Then Throw ReglasCargaExcel.Fallo("ESTADO_NO_CONFIRMABLE")
            sesion.BloquearDestino(e.Tabla)
            Dim doc = Documento(e), limites = _limites(), p = sesion.LeerPlantilla(e.PlantillaId)
            Dim esquema = sesion.Inspeccionar(p, e)
            ValidarActual(c, e, p, esquema, doc, limites)
            p.Modo = e.Modo : p.Formato = ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE)
            If e.Archivo.Bytes > CLng(limites.MaxArchivoMB) * 1000000L Then Throw ReglasCargaExcel.Fallo("LIMITE_ARCHIVO_EXCEDIDO")
            Using archivo = _almacen().Retener(c, e.Id, e.Archivo)
                Dim lectura = New ServicioLecturaExcel().Leer(archivo.Contenido, New SolicitudLecturaExcel With {.NombreArchivo = e.Archivo.Nombre, .FormatoEsperado = ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE), .Hoja = doc.Value(Of String)("Hoja"), .Plantilla = p, .Destino = esquema.Destino, .Limites = limites})
                archivo.Verificar()
                If Not lectura.Valido OrElse lectura.FilasDatosObservadas <> e.FilasPrevistas.GetValueOrDefault() Then Throw ReglasCargaExcel.Fallo("LECTURA_INVALIDA")
                If sesion.ValidarClaves(p, esquema, lectura.Filas, e.Modo).Total <> 0 Then Throw ReglasCargaExcel.Fallo("CLAVES_INVALIDAS")
                ValidarActual(c, e, p, sesion.Inspeccionar(p, e), doc, _limites())
                sesion.AsegurarEstructura(p, e)
                Dim confirmada = sesion.Ejecutar(e, lectura.Filas,
                    Sub(actual, estructura)
                        archivo.Verificar()
                        ValidarActual(c, e, actual, estructura, doc, _limites())
                    End Sub)
                Return ReglasCargaExcel.Estado(confirmada)
            End Using
        End Using
    End Function
    Private Function Historico(c As ContextoCargaExcel, e As CargaExcelEjecucion) As EstadoCargaExcel
        ReglasCargaExcel.MismaAutoridad(c, _contexto())
        Return ReglasCargaExcel.Estado(e)
    End Function
    Private Sub ValidarActual(c As ContextoCargaExcel, e As CargaExcelEjecucion, p As PlantillaExcel, esquema As EsquemaCargaExcel, doc As JObject, limites As LimitesLecturaExcel)
        ReglasCargaExcel.MismaAutoridad(c, _contexto())
        ReglasCargaExcel.Limites(limites)
        ReglasCargaExcel.Plantilla(c, p, e.RevisionPlantilla.GetValueOrDefault(), doc.Value(Of String)("PlantillaHash"))
        If p.ID_PLANTILLAIMP <> e.PlantillaId OrElse p.NOMBRE_PLANTILLA <> e.Tabla OrElse doc.Value(Of String)("Modo") <> e.Modo OrElse esquema.Hash <> e.EsquemaHash Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
        Dim modo = ServicioPlantillasExcel.ModoHistorico(p)
        If (e.Modo = "REEMPLAZAR" AndAlso modo <> "REEMPLAZAR") OrElse ((e.Modo = "CREAR" OrElse e.Modo = "AGREGAR") AndAlso modo <> "AGREGAR") Then Throw ReglasCargaExcel.Fallo("MODO_INVALIDO")
        Dim solicitud As New SolicitudPrepararExcel With {.Hoja = doc.Value(Of String)("Hoja"), .Modo = e.Modo}
        Dim actual = ServicioPreparacionExcel.Documento(c, p, esquema, solicitud, limites)
        If ReglasCargaExcel.Hash(Text.Encoding.UTF8.GetBytes(actual)) <> e.ConfiguracionHash Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
    End Sub
End Class
