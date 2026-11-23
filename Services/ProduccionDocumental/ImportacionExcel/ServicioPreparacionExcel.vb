Option Strict On
Imports System
Imports System.Linq
Imports Newtonsoft.Json

Public Class ServicioPreparacionExcel
    Private ReadOnly _contexto As Func(Of ContextoCargaExcel)
    Private ReadOnly _plantillas As IPlantillaExcelRepository
    Private ReadOnly _cargas As ICargaExcelRepository
    Private ReadOnly _almacen As IAlmacenArchivoExcel
    Private ReadOnly _limites As LimitesLecturaExcel
    Public Sub New(contexto As Func(Of ContextoCargaExcel), plantillas As IPlantillaExcelRepository, cargas As ICargaExcelRepository, almacen As IAlmacenArchivoExcel, limites As LimitesLecturaExcel)
        _contexto = contexto : _plantillas = plantillas : _cargas = cargas : _almacen = almacen : _limites = limites
        ReglasCargaExcel.Limites(limites)
    End Sub
    Public Function Preparar(s As SolicitudPrepararExcel) As PreparacionCargaExcel
        Dim c = _contexto()
        ReglasCargaExcel.Autorizar(c)
        If s Is Nothing OrElse s.ConexionId <> c.ConexionId OrElse Not ReglasCargaExcel.IdValido(s.IdCarga) OrElse s.PlantillaId = 0 OrElse String.IsNullOrWhiteSpace(s.Hoja) OrElse s.Hoja.Length > 120 Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        'La contención de un ID ajeno no debe revelar la actividad de su propietario.
        ReglasCargaExcel.Propietario(c, _cargas.Consultar(c, s.IdCarga))
        Using sesion = _cargas.Abrir(c, s.IdCarga)
            Dim e = sesion.Obtener()
            ReglasCargaExcel.Propietario(c, e)
            If e.PlantillaId <> s.PlantillaId Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
            If e.Estado <> "RECIBIDA" AndAlso e.Estado <> "PREPARADA" Then Throw ReglasCargaExcel.Fallo("ESTADO_NO_PREPARABLE")
            If e.Estado = "PREPARADA" AndAlso Not String.IsNullOrEmpty(s.PreparacionHashEsperada) AndAlso s.PreparacionHashEsperada <> e.ConfiguracionHash Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
            If e.Estado = "PREPARADA" AndAlso String.IsNullOrEmpty(s.PreparacionHashEsperada) Then
                'Sin CAS explícito sólo puede repetirse la intención ya publicada.
                Try
                    If Not ReglasCargaExcel.HashValido(e.ConfiguracionHash) OrElse ReglasCargaExcel.Hash(Text.Encoding.UTF8.GetBytes(If(e.ConfiguracionDoc, ""))) <> e.ConfiguracionHash Then Throw New FormatException()
                    Dim documentoActual = Newtonsoft.Json.Linq.JObject.Parse(e.ConfiguracionDoc)
                    If documentoActual.Value(Of String)("Hoja") <> s.Hoja OrElse documentoActual.Value(Of String)("Modo") <> s.Modo OrElse documentoActual.Value(Of String)("PlantillaHash") <> s.HuellaEsperada OrElse e.RevisionPlantilla.GetValueOrDefault() <> s.RevisionEsperada Then Throw New FormatException()
                Catch
                    Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
                End Try
            End If
            Dim hashAnterior = e.ConfiguracionHash
            Dim respuesta As New PreparacionCargaExcel With {.Carga = ReglasCargaExcel.Estado(e), .Hoja = s.Hoja}
            Dim publicando As Boolean = False
            Try
                Dim p = _plantillas.Obtener(s.PlantillaId)
                ReglasCargaExcel.Plantilla(c, p, s.RevisionEsperada, s.HuellaEsperada)
                respuesta.CampoUnico = p.CAMPO_UNICO
                If p.NOMBRE_PLANTILLA <> e.Tabla Then Throw ReglasCargaExcel.Fallo("CONFLICTO_REVISION")
                Dim historico = ServicioPlantillasExcel.ModoHistorico(p)
                If (s.Modo = "REEMPLAZAR" AndAlso historico <> "REEMPLAZAR") OrElse ((s.Modo = "CREAR" OrElse s.Modo = "AGREGAR") AndAlso historico <> "AGREGAR") Then Throw ReglasCargaExcel.Fallo("MODO_INVALIDO")
                p.Modo = s.Modo : p.Formato = ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE)
                Dim esquema = sesion.Inspeccionar(p, s.Modo)
                Using retenido = _almacen.Retener(c, e.Id, e.Archivo)
                    Dim lector As New ServicioLecturaExcel
                    Dim lectura = lector.Leer(retenido.Contenido, New SolicitudLecturaExcel With {.NombreArchivo = e.Archivo.Nombre, .FormatoEsperado = p.Formato, .Hoja = s.Hoja, .Plantilla = p, .Destino = esquema.Destino, .Limites = _limites})
                    respuesta.LecturaCompleta = lectura.LecturaCompleta
                    respuesta.FilasObservadas = lectura.FilasDatosObservadas
                    respuesta.TotalIncidencias = lectura.TotalIncidencias
                    respuesta.Incidencias = lectura.Incidencias.Take(25).ToList()
                    retenido.Verificar()
                    If Not lectura.Valido Then Throw ReglasCargaExcel.Fallo("LECTURA_INVALIDA")
                    Dim claves = sesion.ValidarClaves(p, esquema, lectura.Filas, s.Modo)
                    respuesta.DuplicadosEvaluados = True
                    respuesta.TotalIncidencias += claves.Total
                    respuesta.Incidencias.AddRange(claves.Muestra.Take(25 - respuesta.Incidencias.Count))
                    respuesta.Muestra = lectura.Filas.Take(25).ToList()
                    respuesta.ValidacionCompleta = True
                    If claves.Total > 0 Then Throw ReglasCargaExcel.Fallo("CLAVES_INVALIDAS")
                    retenido.Verificar()
                    ReglasCargaExcel.MismaAutoridad(c, _contexto())
                    ReglasCargaExcel.Plantilla(c, _plantillas.Obtener(p.ID_PLANTILLAIMP), p.REVISION_CARGA, ServicioPlantillasExcel.Huella(p))
                    Dim ultimo = sesion.Inspeccionar(p, s.Modo)
                    If ultimo.Hash <> esquema.Hash OrElse ReglasCargaExcel.Canonico(ultimo.Politica) <> ReglasCargaExcel.Canonico(esquema.Politica) Then Throw ReglasCargaExcel.Fallo("ESQUEMA_MODIFICADO")
                    Dim preparada = JsonConvert.DeserializeObject(Of CargaExcelEjecucion)(JsonConvert.SerializeObject(e))
                    preparada.Estado = "PREPARADA" : preparada.Modo = s.Modo : preparada.RevisionPlantilla = p.REVISION_CARGA
                    preparada.ConfiguracionDoc = Documento(c, p, esquema, s, _limites)
                    preparada.ConfiguracionHash = ReglasCargaExcel.Hash(Text.Encoding.UTF8.GetBytes(preparada.ConfiguracionDoc))
                    preparada.EsquemaHash = esquema.Hash : preparada.FilasPrevistas = lectura.FilasDatosObservadas : preparada.CodigoIncidencia = Nothing
                    publicando = True 'Desde aquí un fallo nunca degrada una publicación incierta.
                    respuesta.Carga = ReglasCargaExcel.Estado(sesion.Publicar(preparada, s.PreparacionHashEsperada))
                    respuesta.Codigo = "OK"
                End Using
                Return respuesta
            Catch ex As ImportacionExcelException
                If publicando OrElse ex.Codigo = "RESULTADO_INCIERTO" OrElse ex.Codigo = "OPERACION_EN_CURSO" OrElse ex.Codigo = "ACCESO_DENEGADO" Then Throw
                ReglasCargaExcel.MismaAutoridad(c, _contexto())
                sesion.Rechazar(hashAnterior, ex.Codigo)
                e = sesion.Obtener()
                respuesta.Carga = ReglasCargaExcel.Estado(e)
                respuesta.Codigo = ex.Codigo
                If respuesta.TotalIncidencias = 0 Then
                    respuesta.TotalIncidencias = 1
                    respuesta.Incidencias.Add(New IncidenciaLecturaExcel With {.Codigo = ex.Codigo, .Mensaje = ex.Message, .Hoja = s.Hoja})
                End If
                Return respuesta
            End Try
        End Using
    End Function
    Public Shared Function Documento(c As ContextoCargaExcel, p As PlantillaExcel, e As EsquemaCargaExcel, s As SolicitudPrepararExcel, limites As LimitesLecturaExcel) As String
        Dim serializer As New JsonSerializer With {.Culture = Globalization.CultureInfo.InvariantCulture}
        Dim fisica = Newtonsoft.Json.Linq.JObject.FromObject(p, serializer)
        For Each nombre In {"Modo", "Formato", "Huella", "Campos"}
            fisica.Remove(nombre)
        Next
        fisica.Add("Campos", Newtonsoft.Json.Linq.JArray.FromObject(p.Campos.OrderBy(Function(x) x.CAMPO_DESTINO, StringComparer.Ordinal).ThenBy(Function(x) x.CAMPO_FUENTE, StringComparer.Ordinal).ToArray(), serializer))
        Return ReglasCargaExcel.Canonico(New With {.Version = 1, .PlantillaVersion = 1, .PlantillaHash = ServicioPlantillasExcel.Huella(p), .Plantilla = fisica, .Hoja = s.Hoja, .Modo = s.Modo, .Clave = p.CAMPO_UNICO, .Politica = e.Politica, .IdentidadFisica = e.IdentidadFisica, .Empresa = c.EmpresaId, .Modulo = c.ModuloId, .Conexion = c.ConexionRef, .IdentidadConexion = c.IdentidadConexion, .Limites = limites})
    End Function
End Class
