Option Strict On
Imports System
Imports System.IO

Public Class ServicioRecepcionExcel
    Private ReadOnly _contexto As Func(Of ContextoCargaExcel)
    Private ReadOnly _plantillas As IPlantillaExcelRepository
    Private ReadOnly _cargas As ICargaExcelRepository
    Private ReadOnly _almacen As IAlmacenArchivoExcel
    Private ReadOnly _referencias As ReferenciaRecepcionExcel
    Private ReadOnly _limites As LimitesLecturaExcel
    Public Sub New(contexto As Func(Of ContextoCargaExcel), plantillas As IPlantillaExcelRepository, cargas As ICargaExcelRepository, almacen As IAlmacenArchivoExcel, referencias As ReferenciaRecepcionExcel, limites As LimitesLecturaExcel)
        _contexto = contexto : _plantillas = plantillas : _cargas = cargas : _almacen = almacen : _referencias = referencias : _limites = limites
        ReglasCargaExcel.Limites(limites)
    End Sub
    Public Function Iniciar(s As SolicitudInicioRecepcionExcel) As InicioRecepcionExcel
        Dim c = _contexto()
        ReglasCargaExcel.Autorizar(c)
        If s Is Nothing OrElse s.ConexionId <> c.ConexionId OrElse s.PlantillaId = 0 Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        Dim p = _plantillas.Obtener(s.PlantillaId)
        ReglasCargaExcel.Plantilla(c, p, s.RevisionEsperada, s.HuellaEsperada)
        p.Formato = ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE)
        ReglasCargaExcel.MismaAutoridad(c, _contexto())
        Return _referencias.Emitir(c, p)
    End Function
    Public Function Recibir(s As SolicitudRecepcionExcel, nombre As String, contenido As Stream, Optional bytesDeclarados As Long = -1) As EstadoCargaExcel
        Dim c = _contexto()
        ReglasCargaExcel.Autorizar(c)
        If s Is Nothing OrElse s.ConexionId <> c.ConexionId OrElse s.PlantillaId = 0 OrElse Not ReglasCargaExcel.IdValido(s.IdCarga) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        Dim p = _plantillas.Obtener(s.PlantillaId)
        If p Is Nothing Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
        p.Formato = ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE)
        ReglasCargaExcel.Plantilla(c, p, p.REVISION_CARGA, ServicioPlantillasExcel.Huella(p))
        _referencias.Validar(c, p, s)
        Using sesion = _cargas.Abrir(c, s.IdCarga)
            Dim anterior = sesion.Obtener()
            If anterior IsNot Nothing Then
                ReglasCargaExcel.Propietario(c, anterior)
                If anterior.PlantillaId <> s.PlantillaId OrElse anterior.Tabla <> p.NOMBRE_PLANTILLA OrElse anterior.Archivo.Nombre <> AlmacenArchivoExcel.NombreSeguro(nombre) Then Throw ReglasCargaExcel.Fallo("CONFLICTO_RECEPCION")
            End If
            Dim archivo = _almacen.Recibir(c, s.IdCarga, nombre, p.Formato, contenido, _limites)
            If bytesDeclarados >= 0 AndAlso archivo.Bytes <> bytesDeclarados Then Throw ReglasCargaExcel.Fallo("TAMANO_ARCHIVO_INCONSISTENTE")
            'Después de publicar no se borra el completo, aunque falle autorización o commit.
            Using retenido = _almacen.Retener(c, s.IdCarga, archivo)
                ReglasCargaExcel.MismaAutoridad(c, _contexto())
                Dim actual = _plantillas.Obtener(s.PlantillaId)
                ReglasCargaExcel.Plantilla(c, actual, p.REVISION_CARGA, ServicioPlantillasExcel.Huella(p))
                retenido.Verificar()
                Dim recibida As New CargaExcelEjecucion With {.Id = s.IdCarga, .EmpresaId = c.EmpresaId, .UsuarioId = c.UsuarioId, .ConexionRef = c.ConexionRef, .PlantillaId = s.PlantillaId, .Tabla = p.NOMBRE_PLANTILLA, .Archivo = archivo, .Estado = "RECIBIDA"}
                Dim registrada = sesion.RegistrarRecibida(recibida)
                registrada.Archivo.Hojas = archivo.Hojas
                Return ReglasCargaExcel.Estado(registrada)
            End Using
        End Using
    End Function
    Public Function Consultar(s As SolicitudConsultaCargaExcel) As EstadoCargaExcel
        Dim c = _contexto()
        ReglasCargaExcel.Autorizar(c)
        If s Is Nothing OrElse s.ConexionId <> c.ConexionId OrElse Not ReglasCargaExcel.IdValido(s.IdCarga) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        Dim e = _cargas.Consultar(c, s.IdCarga)
        'Una lectura ausente no acredita que un upload no esté en curso. Sin reparación.
        If e Is Nothing Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        ReglasCargaExcel.Propietario(c, e) : ReglasCargaExcel.MismaAutoridad(c, _contexto())
        Return ReglasCargaExcel.Estado(e)
    End Function
End Class
