Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class ServicioPerfilImportacion
    Private ReadOnly _repositorio As IPerfilImportacionRepository
    Private ReadOnly _catalogos As IPerfilImportacionCatalogos
    Private ReadOnly _validador As New ValidadorPerfilImportacion()
    Private ReadOnly _resolvedor As New ResolvedorReglasPerfilImportacion()
    Private ReadOnly _proveedores As CoordinadorProveedorIndices

    Public Sub New(repositorio As IPerfilImportacionRepository, catalogos As IPerfilImportacionCatalogos, Optional proveedores As CoordinadorProveedorIndices = Nothing)
        If repositorio Is Nothing OrElse catalogos Is Nothing Then Throw New ArgumentNullException()
        _repositorio = repositorio
        _catalogos = catalogos
        _proveedores = proveedores
    End Sub

    Private Sub Autorizar(contexto As ContextoPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Operación no autorizada.")
        _repositorio.VerificarEsquema()
    End Sub

    Public Function Listar(contexto As ContextoPerfilImportacion, filtro As FiltroPerfilesImportacion) As PaginaPerfilesImportacion
        Autorizar(contexto)
        If filtro Is Nothing OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Termino, 160, True) OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Nombre, 160, True) OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Codigo, 64, True) Then Throw SolicitudInvalida()
        Return _repositorio.Listar(contexto, filtro)
    End Function
    Public Function ListarProveedores(contexto As ContextoPerfilImportacion, pagina As Integer, tamano As Integer, proveedorId As String, versionId As String) As List(Of OpcionProveedorIndices)
        Autorizar(contexto)
        If _proveedores Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Proveedores no disponibles.")
        Return _proveedores.Listar(contexto, pagina, tamano, proveedorId, versionId)
    End Function

    Public Function Obtener(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        If Not String.IsNullOrEmpty(versionId) Then ExigirId(versionId)
        Return _repositorio.Obtener(contexto, perfilId, versionId)
    End Function

    Public Function Historial(contexto As ContextoPerfilImportacion, perfilId As String, pagina As Integer, tamano As Integer) As List(Of VersionPerfilImportacion)
        Autorizar(contexto)
        ExigirId(perfilId)
        _repositorio.Obtener(contexto, perfilId, Nothing)
        Return _repositorio.Historial(contexto, perfilId, pagina, tamano)
    End Function

    Public Function Crear(contexto As ContextoPerfilImportacion, codigo As String, nombre As String, descripcion As String) As PerfilImportacion
        Autorizar(contexto)
        If String.IsNullOrWhiteSpace(codigo) OrElse codigo.Length > 64 OrElse codigo.Any(Function(c) AscW(c) < 33 OrElse AscW(c) > 126) Then Throw SolicitudInvalida()
        ValidarCabecera(nombre, descripcion)
        Return _repositorio.Crear(contexto, codigo, nombre, descripcion)
    End Function

    Public Function Guardar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        If version Is Nothing Then Throw SolicitudInvalida()
        ExigirId(version.VersionId)
        Dim actual = _repositorio.Obtener(contexto, perfilId, version.VersionId)
        If actual.Version.Estado <> "BORRADOR" OrElse actual.Version.Revision <> version.Revision Then Throw Conflicto()
        ExigirConfiguracion(contexto, version, False)
        Return _repositorio.Guardar(contexto, perfilId, version)
    End Function

    Public Function Ensayar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion, ejemplos As List(Of EjemploPerfilImportacion)) As List(Of ResultadoEnsayoPerfilImportacion)
        Autorizar(contexto)
        ExigirId(perfilId)
        If version Is Nothing OrElse ejemplos Is Nothing OrElse ejemplos.Count < 1 OrElse ejemplos.Count > 20 Then Throw SolicitudInvalida()
        ExigirId(version.VersionId)
        Dim actual = _repositorio.Obtener(contexto, perfilId, version.VersionId)
        'Una publicación se ensaya desde su snapshot persistido, nunca desde reglas reenviadas.
        If actual.Version.Estado = "PUBLICADA" Then version = actual.Version
        ExigirConfiguracion(contexto, version, False)
        Dim heredadas = version.Reglas.Where(Function(r) (r.FuenteTipo = "EXPEDIENTE" AndAlso ResolvedorReglasPerfilImportacion.PropiedadesClasificacion.Contains(r.PropiedadExpediente)) OrElse
            (r.FuenteTipo = "PROVEEDOR" AndAlso r.Proveedor.Clave.FuenteTipo = "EXPEDIENTE" AndAlso ResolvedorReglasPerfilImportacion.PropiedadesClasificacion.Contains(r.Proveedor.Clave.PropiedadExpediente))).ToList()
        If heredadas.Count > 0 Then
            Dim origen = TryCast(_catalogos, IClasificacionEnsayoPerfil)
            If origen Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se puede resolver la clasificacion del perfil.")
            Dim propuesta = origen.ClasificacionPropuesta(contexto, version.Configuracion)
            Dim preparados As New List(Of EjemploPerfilImportacion)()
            For Each ejemplo In ejemplos
                If ejemplo Is Nothing OrElse ejemplo.Expediente Is Nothing Then Throw SolicitudInvalida()
                Dim copia As New EjemploPerfilImportacion With {.NombreCarpeta = ejemplo.NombreCarpeta, .NombreArchivo = ejemplo.NombreArchivo, .Expediente = New Dictionary(Of String, String)(ejemplo.Expediente, StringComparer.Ordinal)}
                'El cliente no puede sustituir la clasificacion autorizada por un supuesto expediente confirmado.
                For Each propiedad In ResolvedorReglasPerfilImportacion.PropiedadesClasificacion
                    copia.Expediente.Remove(propiedad)
                Next
                For Each par In propuesta
                    copia.Expediente.Add(par.Key, par.Value)
                Next
                preparados.Add(copia)
            Next
            ejemplos = preparados
        End If
        Dim resultados As List(Of ResultadoEnsayoPerfilImportacion)
        If version.Reglas.Any(Function(r) r.FuenteTipo = "PROVEEDOR") Then
            resultados = _proveedores.Ensayar(contexto, version.Reglas, ejemplos)
        Else
            resultados = ejemplos.Select(Function(e) _resolvedor.Resolver(version.Reglas, e)).ToList()
        End If
        For Each resultado In resultados
            For Each valor In resultado.Valores
                If heredadas.Any(Function(r) r.DestinoTipo = valor.DestinoTipo AndAlso r.DestinoRef = valor.DestinoRef) Then valor.Procedencia = "PROPUESTA_PERFIL"
            Next
            If resultado.Incidencias.Count = 0 Then resultado.Incidencias.AddRange(_catalogos.ValidarValores(contexto, version.Configuracion, resultado.Valores))
        Next
        Return resultados
    End Function

    Public Function Publicar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, revisionPerfil As UInteger, revisionVersion As UInteger) As PerfilImportacion
        Dim actual = Obtener(contexto, perfilId, versionId)
        If actual.Revision <> revisionPerfil OrElse actual.Version.Revision <> revisionVersion OrElse actual.Version.Estado <> "BORRADOR" Then Throw Conflicto()
        ExigirConfiguracion(contexto, actual.Version, True)
        Return _repositorio.Publicar(contexto, perfilId, versionId, revisionPerfil, revisionVersion, PerfilImportacionJson.Huella(actual.Version))
    End Function

    Public Function CrearVersion(contexto As ContextoPerfilImportacion, perfilId As String, origenId As String, revisionPerfil As UInteger, revisionOrigen As UInteger) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        ExigirId(origenId)
        Return _repositorio.CrearVersion(contexto, perfilId, origenId, revisionPerfil, revisionOrigen)
    End Function

    Public Function CambiarDisponibilidad(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, activo As Boolean) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        Return _repositorio.CambiarDisponibilidad(contexto, perfilId, revision, activo)
    End Function

    Public Function ActualizarCabecera(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, nombre As String, descripcion As String) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        ValidarCabecera(nombre, descripcion)
        Return _repositorio.ActualizarCabecera(contexto, perfilId, revision, nombre, descripcion)
    End Function

    Public Function Seleccionar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, nivelRef As String, proposito As String) As PerfilImportacion
        Autorizar(contexto)
        If proposito <> "ADMINISTRACION" Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La selección operativa pertenece a la entrega de importación.")
        ExigirId(versionId)
        Dim actual = Obtener(contexto, perfilId, versionId)
        If Not actual.Activo OrElse actual.Version.Estado <> "PUBLICADA" Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "La versión no está disponible para selección.")
        ExigirConfiguracion(contexto, actual.Version, True)
        If Not ValidadorPerfilImportacion.ReferenciaValida(nivelRef) OrElse Not _catalogos.NivelCompatible(contexto, actual.Version.Configuracion, nivelRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El nivel no es compatible o no está autorizado.")
        Return actual
    End Function

    Private Sub ExigirConfiguracion(contexto As ContextoPerfilImportacion, version As VersionPerfilImportacion, publicar As Boolean)
        Dim errores = _validador.Validar(version.Configuracion, version.Reglas, publicar)
        If errores.Count = 0 AndAlso version.Reglas.Any(Function(r) r.FuenteTipo = "PROVEEDOR") Then
            If _proveedores Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La fuente de proveedor no esta disponible.")
            errores.AddRange(_proveedores.Validar(contexto, version.Reglas))
        End If
        If errores.Count = 0 Then errores.AddRange(_catalogos.Validar(contexto, version.Configuracion, version.Reglas, publicar))
        If errores.Count > 0 Then Throw New ConfiguracionPerfilImportacionException(errores)
        PerfilImportacionJson.Escribir(version.Configuracion)
        PerfilImportacionJson.Escribir(version.Reglas)
    End Sub

    Private Shared Sub ExigirId(id As String)
        If Not ValidadorPerfilImportacion.ReferenciaValida(id) Then Throw SolicitudInvalida()
    End Sub
    Private Shared Sub ValidarCabecera(nombre As String, descripcion As String)
        '16000 BMP caracteres caben en TEXT utf8 (máximo 48000 bytes).
        If Not ValidadorPerfilImportacion.TextoValido(nombre, 160, False) OrElse Not ValidadorPerfilImportacion.TextoValido(descripcion, 16000, True) Then Throw SolicitudInvalida()
    End Sub
    Private Shared Function SolicitudInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "Revise los datos de la solicitud.")
    End Function
    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "La versión cambió o ya está publicada.")
    End Function
End Class

'Mediciones del importador: no modifica Trace.Listeners ni registra datos de negocio.
Public NotInheritable Class MedicionAlmacenamientoImportacion
    Private Const ClaveCaptura As String = "DOC120_TIEMPOS_SOLICITUD"
    Private Shared ReadOnly Partes As String() = {"prev_auth", "prev_meta", "prev_io", "ret_auth", "ret_meta", "ret_io", "v2_hash"}
    Private NotInheritable Class Acumulado
        Public Milisegundos As Double
        Public Llamadas As Integer
    End Class
    Private Sub New()
    End Sub
    Public Shared Sub Preparar(http As System.Web.HttpContext, entrada As AvanzarLoteImportacionSolicitud)
        Try
            If http Is Nothing OrElse Not Object.ReferenceEquals(http, System.Web.HttpContext.Current) Then Return
            http.Items.Remove(ClaveCaptura)
            If entrada Is Nothing OrElse entrada.fase <> "ALMACENAR" Then Return
            Dim referencia = http.Request.Headers("X-DOC120-Medicion")
            If referencia Is Nothing OrElse Not System.Text.RegularExpressions.Regex.IsMatch(referencia, "\A[0-9a-f]{32}\z") Then Return
            http.Items(ClaveCaptura) = New Dictionary(Of String, Acumulado)(StringComparer.Ordinal)
        Catch
        End Try
    End Sub
    Private Shared Function Captura() As Dictionary(Of String, Acumulado)
        Try
            Dim http = System.Web.HttpContext.Current
            Return If(http Is Nothing, Nothing, TryCast(http.Items(ClaveCaptura), Dictionary(Of String, Acumulado)))
        Catch
            Return Nothing
        End Try
    End Function
    Private Shared Sub Acumular(nombre As String, milisegundos As Double)
        Try
            Dim datos = Captura()
            If datos Is Nothing Then Return
            Dim item As Acumulado = Nothing
            If Not datos.TryGetValue(nombre, item) Then
                item = New Acumulado() : datos.Add(nombre, item)
            End If
            item.Milisegundos += milisegundos : item.Llamadas += 1
        Catch
        End Try
    End Sub
    Public Shared Function MedirParte(Of T)(nombre As String, trabajo As Func(Of T)) As T
        If Captura() Is Nothing OrElse Not Partes.Contains(nombre) Then Return trabajo()
        Dim reloj = System.Diagnostics.Stopwatch.StartNew()
        Try
            Return trabajo()
        Finally
            Acumular(nombre, reloj.Elapsed.TotalMilliseconds)
        End Try
    End Function
    Friend Shared Function Cabecera() As String
        Dim datos = Captura()
        If datos Is Nothing OrElse datos.Count = 0 Then Return Nothing
        Return String.Join(", ", datos.OrderBy(Function(p) p.Key, StringComparer.Ordinal).Select(Function(p) "doc120_" & p.Key & ";dur=" & p.Value.Milisegundos.ToString("F3", Globalization.CultureInfo.InvariantCulture) & ";desc=""n=" & p.Value.Llamadas.ToString(Globalization.CultureInfo.InvariantCulture) & """"))
    End Function
    Public Shared Sub Adjuntar(http As System.Web.HttpContext)
        Try
            If http Is Nothing OrElse Not Object.ReferenceEquals(http, System.Web.HttpContext.Current) Then Return
            Dim valor = Cabecera()
            If http IsNot Nothing AndAlso valor IsNot Nothing Then http.Response.AppendHeader("Server-Timing", valor)
        Catch
            'Una medicion no cambia el resultado de negocio.
        End Try
    End Sub
    Public Shared Function Medir(Of T)(etapa As String, loteId As String, archivoId As String, trabajo As Func(Of T)) As T
        Dim reloj = System.Diagnostics.Stopwatch.StartNew(), confirmado As Boolean = False
        Try
            Dim resultado = trabajo()
            confirmado = True
            Return resultado
        Finally
            reloj.Stop()
            Registrar(etapa, loteId, archivoId, reloj.Elapsed.TotalMilliseconds, confirmado)
        End Try
    End Function
    Private Shared Sub Registrar(etapa As String, loteId As String, archivoId As String, milisegundos As Double, finalizo As Boolean)
        Try
            If Not {"TOTAL", "CONSULTA_PREFLIGHT", "ORIGINALES_PREVIOS", "RESERVA", "ORIGINALES_RETENIDOS", "ADAPTADOR", "DATOS_DOCUMENTALES", "ESCRITOR_LEGACY", "COMPROBACION_POSTERIOR", "CHECKPOINT", "LECTURA_FINAL", "ORIGINAL_V2", "COPIA_TRABAJO", "V2_INICIO", "V2_AUTORIZACION", "V2_RETORNO"}.Contains(etapa) Then Return
            If Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivoId) Then Return
            Acumular(etapa.ToLowerInvariant(), milisegundos)
            Dim registro = "DOC120_MEDICION etapa=" & etapa & " lote=" & loteId & " archivo=" & archivoId & " ms=" & milisegundos.ToString("F3", Globalization.CultureInfo.InvariantCulture) & " finalizo=" & If(finalizo, "1", "0")
            Dim http = System.Web.HttpContext.Current
            Dim operacion = If(http Is Nothing, Nothing, TryCast(http.Items("DOC120_OPERACION"), String))
            If operacion IsNot Nothing AndAlso System.Text.RegularExpressions.Regex.IsMatch(operacion, "\A[0-9a-f]{32}\z") Then registro &= " operacion=" & operacion
            For Each listener As System.Diagnostics.TraceListener In System.Diagnostics.Trace.Listeners
                Try
                    listener.TraceEvent(New System.Diagnostics.TraceEventCache(), "ImportacionCarpetas", System.Diagnostics.TraceEventType.Information, 121, registro)
                Catch
                    'El observador no puede cambiar el resultado ni disparar otro efecto.
                End Try
            Next
        Catch
        End Try
    End Sub
End Class

Public Class ConfiguracionPerfilImportacionException
    Inherits PerfilImportacionException
    Public ReadOnly Property Incidencias As List(Of IncidenciaPerfilImportacion)
    Public Sub New(incidencias As List(Of IncidenciaPerfilImportacion), Optional codigo As String = "CONFIGURACION_INVALIDA")
        MyBase.New(codigo, "La configuración contiene errores.")
        Me.Incidencias = incidencias
    End Sub
End Class

'Diagnostico cerrado del recorrido operativo. No lee negocio ni crea auditoria.
'Reutiliza Campo/Codigo/Mensaje del contrato; nunca publica Exception.Message.
Public NotInheritable Class DiagnosticoOperacionImportacion
    Private Sub New()
    End Sub
    Public Shared Function Texto(codigo As String) As String
        Select Case codigo
            Case "SEGMENTO_INVALIDO" : Return "La posición o el separador de extracción no cumple la regla configurada."
            Case "LONGITUD_INVALIDA", "LIMITE_INVALIDO" : Return "El límite configurado no está dentro del intervalo admitido."
            Case "LONGITUD_CODIGO_EXCEDIDA", "CODIGO_NO_REPRESENTABLE" : Return "El código del expediente excede la longitud o representación admitida por el destino."
            Case "LONGITUD_NOMBRE_EXCEDIDA" : Return "El nombre del expediente supera la longitud admitida por el destino."
            Case "EJEMPLO_INVALIDO" : Return "La entrada de comprobación no contiene los metadatos necesarios o excede su límite."
            Case "FUENTE_REQUERIDA" : Return "Falta seleccionar una fuente para resolver el campo."
            Case "FUENTE_CIRCULAR_O_AMBIGUA" : Return "La regla del expediente depende de un archivo o del propio expediente todavía no creado."
            Case "TIPO_DEPENDENCIA_INVALIDO" : Return "El tipo del campo no es compatible con la propiedad del expediente de la que depende."
            Case "ETAPA_INVALIDA" : Return "La etapa de resolución no corresponde a la fuente de la regla."
            Case "EXTRACCION_INVALIDA" : Return "La forma de extraer el valor no está admitida por la regla."
            Case "EXTENSION_INVALIDA" : Return "Quitar la extensión solo está permitido sobre el nombre de un archivo."
            Case "PROVEEDOR_INVALIDO", "PROVEEDOR_INCOMPATIBLE" : Return "El proveedor no es compatible con la regla y versión seleccionadas."
            Case "PROVEEDOR_NO_APLICA" : Return "La regla incluye un proveedor aunque utiliza otra fuente de datos."
            Case "REFERENCIA_INVALIDA", "REFERENCIA_REQUERIDA" : Return "Falta una referencia válida requerida por la configuración."
            Case "REFERENCIA_NO_AUTORIZADA", "RESPONSABLE_NO_AUTORIZADO", "GABINETE_NO_AUTORIZADO" : Return "Una referencia configurada no tiene autorización vigente para esta operación."
            Case "TIPOLOGIA_NO_UNIVOCA" : Return "La tipología documental no tiene una coincidencia única en la clasificación autorizada."
            Case "ORIGEN_INVALIDO", "ORIGEN_CAMBIADO", "ORIGEN_NO_DISPONIBLE" : Return "El origen configurado no pudo verificarse o ya no coincide con el confirmado."
            Case "GABINETE_REQUERIDO" : Return "Seleccione el gabinete de destino requerido por el perfil."
            Case "INDICE_REQUERIDO" : Return "Falta una regla para un índice obligatorio del gabinete."
            Case "INDICE_INCOMPATIBLE", "VALOR_INDICE_INVALIDO" : Return "El índice o su valor no cumple el tipo y formato del gabinete confirmado."
            Case "ESQUEMA_INVALIDO", "ESQUEMA_FUENTES_INCOMPLETO" : Return "El esquema requerido por la importación no está disponible con la estructura esperada."
            Case "NOMBRE_INVALIDO" : Return "El nombre indicado no cumple las restricciones de la configuración."
            Case "POLITICA_INVALIDA", "AMBITO_INVALIDO", "AMBITO_REQUERIDO", "MODO_INVALIDO" : Return "La política, modo o ámbito de integridad no forman una combinación admitida."
            Case "EXTENSIONES_INVALIDAS", "EXTENSION_NO_PERMITIDA" : Return "La extensión no está incluida en los formatos permitidos por el perfil."
            Case "OPCION_REQUERIDA", "OPCION_INVALIDA", "CONFIGURACION_REQUERIDA" : Return "Falta una opción requerida o su valor no está permitido por la configuración."
            Case "CODIGO_REPETIDO_INVENTARIO" : Return "Esta revisión se guardó con una comprobación anterior que rechazaba códigos repetidos. Prepara una nueva revisión; el importador actual permite repetir el código y asigna un ID a cada expediente."
            Case "COINCIDENCIA_AMBIGUA", "AMBIGUO" : Return "La consulta devolvió varias coincidencias donde se exige una sola."
            Case "CODIGO_EXPEDIENTE_EXISTENTE" : Return "Esta revisión se guardó con una comprobación anterior que rechazaba códigos repetidos. Prepara una nueva revisión; el importador actual permite repetir el código y asigna un ID a cada expediente."
            Case "REUTILIZACION_NO_DISPONIBLE" : Return "No se pudo comprobar una reutilización compatible con la política del perfil."
            Case "IDENTIDAD_INMUTABLE" : Return "La identidad confirmada no se puede sustituir mediante esta operación."
            Case "SIN_COINCIDENCIA" : Return "La consulta de datos no encontró una coincidencia para la clave de esta regla."
            Case "CLAVE_INVALIDA" : Return "La clave de consulta no cumple el tipo o formato exigido por el proveedor."
            Case "PROPIEDAD_AUSENTE" : Return "La fuente no devolvió la propiedad requerida por el campo."
            Case "SUBCARPETA_NO_PERMITIDA" : Return "El perfil no admite subcarpetas dentro de la raíz seleccionada."
            Case "IMPORTACION_CERRADA" : Return "Esta vista de importación está cerrada. Abra el módulo y consulte su referencia."
            Case "PREPARACION_REQUERIDA" : Return "Prepare y revise el plan antes de confirmar la importación."
            Case "RESPUESTA_NO_CONFIRMADA", "RECEPCION_NO_CONFIRMADA" : Return "No hay un acuse válido para confirmar el resultado; consulte la misma referencia."
            Case "CONTEXTO_INVALIDO" : Return "No se pudo validar el contexto de acceso al módulo. Recargue la página."
            Case "RUTA_RELATIVA_INVALIDA", "METADATOS_ARCHIVO_INVALIDOS" : Return "El archivo no tiene una ruta relativa, tamaño o fecha local válidos para la selección."
            Case "SELECCION_MULTIPLES_RAICES" : Return "La selección contiene varias raíces. Agregue cada carpeta mediante una selección independiente."
            Case "RUTA_DUPLICADA" : Return "La misma raíz contiene más de un archivo con la misma ruta relativa."
            Case "CODIGO_PERFIL_EXISTENTE" : Return "Ya existe un perfil con ese código en el ámbito autorizado."
            Case "EVIDENCIA_DOCUMENTAL_V2_INVALIDA", "EVIDENCIA_NO_VERIFICABLE" : Return "La evidencia retenida no permite confirmar este resultado documental."
            Case "NO_SOPORTADO" : Return "La capacidad solicitada no está habilitada en este recorrido."
            Case "DEPENDENCIA_NO_RESUELTA" : Return "Todavía falta resolver una propiedad del expediente o de su clasificación."
            Case "ORIGINAL_NO_VERIFICABLE" : Return "El original retenido no pudo comprobarse para esta operación."
            Case "LIMPIEZA_NO_CONFIRMADA" : Return "La limpieza no tiene un resultado confirmado; se conserva la evidencia retenida."
            Case "RETENCION_NO_CONFIGURADA" : Return "No hay una política de retención configurada para esa capacidad."
            Case "FECHA_NO_VERIFICABLE" : Return "No se pudo comprobar la fecha requerida por la operación."
            Case "RESULTADO_REGISTRADO" : Return "El intento ya tiene un resultado registrado; consulte su evidencia sin repetir el efecto."
            Case "LIMITE_PERFIL_EXCEDIDO" : Return "El archivo supera el límite configurado en el perfil confirmado."
            Case "RESELECCION_NO_COINCIDE" : Return "El archivo no coincide con el nombre, tamaño y fecha del pendiente elegido. Seleccione el original correspondiente."
            Case "NAVEGADOR_NO_COMPATIBLE" : Return "El navegador no dispone de las capacidades necesarias para este selector de carpetas."
            Case "VALOR_REQUERIDO" : Return "Falta un valor obligatorio. Revise la regla del campo."
            Case "VALOR_VACIO" : Return "La fuente produjo un valor vacío. Revise la selección y la regla."
            Case "SEGMENTO_AUSENTE" : Return "El nombre no contiene el segmento que exige la regla. Revise la estructura del nombre."
            Case "LONGITUD_EXCEDIDA", "ENTRADA_EXCEDIDA", "CODIGO_EXCEDE_45" : Return "El texto supera la longitud permitida para el campo."
            Case "FECHA_INVALIDA", "FECHA_CONSTANTE_NO_CANONICA" : Return "La fecha no coincide con el formato de la regla o no es válida."
            Case "RANGO_FECHAS_INVALIDO" : Return "La fecha final es anterior a la fecha inicial. Corrija la regla o el origen."
            Case "ENTERO_INVALIDO" : Return "El campo requiere un número entero representable, sin separadores."
            Case "DECIMAL_INVALIDO" : Return "El campo requiere un número decimal representable con punto decimal."
            Case "CARACTER_NO_REPRESENTABLE", "TEXTO_LEGACY_INVALIDO" : Return "El texto contiene caracteres que el destino no admite. No se modificó ni truncó el valor."
            Case "VALOR_NO_CANONICO" : Return "El valor guardado no coincide con la representación canónica exigida."
            Case "FUENTE_NO_RESUELTA", "FUENTE_NO_DISPONIBLE" : Return "No se pudo resolver la fuente del campo. Revise su disponibilidad y configuración."
            Case "REGLA_INVALIDA", "DESTINO_INVALIDO", "CAMPO_INVALIDO", "TIPO_INVALIDO", "FORMATO_FECHA_INVALIDO", "DEPENDENCIA_INVALIDA", "FUENTE_INVALIDA" : Return "La regla del campo no es compatible con el destino o tipo configurado."
            Case "DESTINO_DUPLICADO" : Return "Hay más de una regla para el mismo campo de destino."
            Case "VALORES_NO_COINCIDEN" : Return "Los valores no coinciden con las reglas del perfil confirmado."
            Case "SNAPSHOT_NO_COINCIDE" : Return "La configuración no coincide con el perfil confirmado. Consulte la importación sin sustituir su plan."
            Case "REFERENCIA_LEGACY_INVALIDA" : Return "Una referencia no cabe en el identificador admitido por el destino."
            Case "RECEPCION_INCOMPLETA" : Return "Todavía falta recepción verificada en el ámbito exigido por el perfil."
            Case "BARRERA_CREACION_NO_DISPONIBLE" : Return "La reserva, estado o recepción del ámbito exigido ya no permite crear el expediente. Consulte las acciones actuales."
            Case "CONTEXTO_DOCUMENTAL_NO_COINCIDE" : Return "El contexto, archivo o intento no coincide con los datos confirmados exigidos para almacenar."
            Case "CONTEXTO_EXPEDIENTE_NO_DISPONIBLE" : Return "El expediente de destino no tiene identidad y contexto disponibles o mantiene un intento activo."
            Case "REFERENCIAS_EVIDENCIA_INVALIDAS" : Return "Las referencias de la reserva o las huellas de recepción no cumplen el contrato de evidencia exigido."
            Case "LIMITES_PERFIL_INCOMPATIBLES" : Return "La extensión o el tamaño del archivo no cumple las restricciones del perfil confirmado."
            Case "RESERVA_NO_DISPONIBLE" : Return "La reserva o sus revisiones ya no permiten esta operación. Consulte el estado actual."
            Case "PLAN_NO_REVISABLE" : Return "El plan ya no está revisable, cambió de revisión o conserva incidencias que impiden confirmarlo."
            Case "PLAN_EXPIRADO" : Return "La vigencia del plan no permite confirmarlo. Prepare una revisión nueva tras verificar que no exista confirmación pendiente."
            Case "PERFIL_CAMBIADO" : Return "La publicación del perfil cambió desde la revisión. Prepare un plan con la versión vigente."
            Case "COINCIDENCIA_DETECTADA" : Return "La comprobación del destino detectó una coincidencia que bloquea esta unidad. Revise el detalle del plan."
            Case "DESTINO_CAMBIADO" : Return "La ruta o el sello del destino ya no coincide con la referencia consultada."
            Case "DETENCION_SOLICITADA" : Return "La importación tiene una detención solicitada. Consulte las acciones permitidas antes de continuar."
            Case "CATALOGO_NO_DISPONIBLE" : Return "No se pudo comprobar el catálogo requerido por esta fase."
            Case "INDICES_PENDIENTES" : Return "Primero debe preparar los índices del archivo en el expediente confirmado."
            Case "EXPEDIENTE_NO_CONFIRMADO" : Return "No hay una identidad de expediente confirmada disponible para almacenar este archivo."
            Case "GABINETE_INCOMPATIBLE" : Return "El gabinete confirmado no es compatible con el destino de almacenamiento."
            Case "TIPOLOGIA_INCOMPATIBLE" : Return "No hay una única tipología documental válida para el archivo."
            Case "ORIGINAL_NO_COMPROBADO" : Return "El original recibido no coincide con la evidencia de recepción exigida."
            Case "NOMBRE_ARCHIVO_INVALIDO" : Return "El nombre del archivo no es compatible con el destino. Revise su nombre y extensión."
            Case "PDF_INVALIDO" : Return "El contenido no cumple la validación del PDF admitido."
            Case "PDF_FIRMA_INVALIDA" : Return "El contenido recibido no tiene la cabecera PDF admitida."
            Case "PDF_CIERRE_INVALIDO" : Return "El contenido recibido no tiene un cierre PDF completo."
            Case "PDF_ESTRUCTURA_INVALIDA" : Return "El PDF está cifrado, requiere reconstrucción o supera los límites de páginas u objetos admitidos."
            Case "BYTES_NO_COINCIDEN" : Return "Los bytes recibidos no coinciden con el tamaño previsto para el archivo."
            Case "RESPUESTA_INVALIDA" : Return "La respuesta del servidor no es válida para esta fase."
            Case "ERROR_RED", "ERROR_TRANSPORTE" : Return "Se interrumpió la comunicación con el servidor."
            Case "TIEMPO_AGOTADO" : Return "El servidor no devolvió una respuesta dentro del tiempo de espera."
            Case "SOLICITUD_CANCELADA" : Return "Se canceló la espera de la solicitud; esto no revierte efectos ya iniciados."
            Case "CONTENIDO_DISTINTO" : Return "El archivo seleccionado no coincide con el contenido retenido; no se sustituyó el original."
            Case "PERFIL_REQUERIDO", "VERSION_INVALIDA" : Return "Seleccione una versión publicada del perfil antes de preparar la revisión."
            Case "SELECCION_DIRECTORIOS_NO_DISPONIBLE" : Return "Este navegador no permite seleccionar carpetas. Use un navegador compatible."
            Case "FORMATO_NO_SOPORTADO" : Return "Esta importación admite PDF; TIFF y otros formatos no están habilitados."
            Case "LIMITE_EXCEDIDO" : Return "La selección o el archivo supera un límite configurado para esta importación."
            Case "RECEPCION_OCUPADA" : Return "La recepción está ocupada por otro intento. Consulte el archivo antes de volver a enviarlo."
            Case "CONFLICTO_REVISION" : Return "La revisión o el estado consultado cambió. Actualice la misma referencia antes de continuar."
            Case "ACCESO_DENEGADO" : Return "No tiene autorización vigente para esta operación."
            Case "SESION_REQUERIDA" : Return "La sesión no está vigente. Inicie sesión y consulte la referencia conservada."
            Case "SOLICITUD_NO_VERIFICADA" : Return "El contexto de seguridad no pudo verificarse. Recargue la página."
            Case "SOLICITUD_INVALIDA" : Return "La solicitud no cumple el contrato de esta operación. Revise los datos y la selección."
            Case "CONFIGURACION_INVALIDA" : Return "La configuración no cumple una comprobación de esta fase. Revise las incidencias disponibles."
            Case "CAPACIDAD_NO_DISPONIBLE" : Return "No se pudo comprobar una dependencia necesaria para esta fase."
            Case "OPERACION_EN_CURSO" : Return "Hay un intento sin resultado final confirmado. Consulte su estado; no repita el efecto."
            Case "COORDINACION_LIMITE" : Return "Se alcanzó el tope de consultas u operaciones de esta continuación. Consulta el mismo lote y revisa lo confirmado antes de continuar."
            Case "RESULTADO_PENDIENTE_LECTURA" : Return "La consulta todavía no refleja el resultado de la última operación. Consulta el mismo lote; no repitas esa operación."
            Case "ORIGINAL_REQUERIDO" : Return "Falta el PDF original de un archivo pendiente. Vincúlalo al mismo archivo y pulsa Continuar importación; seleccionarlo no lo envía."
            Case "SIN_AVANCE_DISPONIBLE" : Return "El estado consultado no permite iniciar otra fase. Revisa los pendientes y sus requisitos en el mismo lote."
            Case "REVISION_TAMANO_EXCEDIDO" : Return "Los datos necesarios para preparar esta revisión superan el tamaño que admite el servicio de importación."
            Case "RESULTADO_INCIERTO", "ERROR_INTERNO" : Return "No se recibió un resultado comprobable de esta operación. Conserve su referencia."
            Case "ARCHIVO_RECHAZADO" : Return "El archivo fue rechazado por una comprobación de la fase. Revise sus incidencias antes de continuar."
            Case Else : Return "La comprobación no pudo completarse con los datos disponibles."
        End Select
    End Function
    Public Shared Function Incidencia(campo As String, codigo As String, Optional longitud As Integer = -1, Optional maximo As Integer = -1, Optional posicion As Integer = -1, Optional formato As String = Nothing) As IncidenciaPerfilImportacion
        Dim mensaje = Texto(codigo)
        If longitud >= 0 AndAlso maximo >= 0 Then mensaje &= " Longitud observada: " & longitud.ToString(Globalization.CultureInfo.InvariantCulture) & ". Máximo permitido: " & maximo.ToString(Globalization.CultureInfo.InvariantCulture) & "."
        If posicion >= 0 AndAlso posicion < 128 Then mensaje &= " Segmento requerido: " & (posicion + 1).ToString(Globalization.CultureInfo.InvariantCulture) & " (desde 1)."
        If {"yyyy-MM-dd", "yyyyMMdd", "dd-MM-yyyy", "dd_MM_yyyy", "dd/MM/yyyy H:mm:ss"}.Contains(formato) Then mensaje &= " Formato requerido: " & formato & "."
        Return New IncidenciaPerfilImportacion(campo, codigo, mensaje)
    End Function
    Public Shared Function Fallo(codigo As String, campo As String, motivo As String, Optional longitud As Integer = -1, Optional maximo As Integer = -1) As PerfilImportacionException
        If codigo = "ACCESO_DENEGADO" Then Return New PerfilImportacionException(codigo, Texto(codigo))
        Return New ConfiguracionPerfilImportacionException(New List(Of IncidenciaPerfilImportacion) From {Incidencia(campo, motivo, longitud, maximo)}, codigo)
    End Function
    Public Shared Function CampoSeguro(campo As String) As String
        If campo IsNot Nothing AndAlso System.Text.RegularExpressions.Regex.IsMatch(campo, "\A(?:(?:CAMPO_EXPEDIENTE:)?(?:CODIGO|NOMBRE|FECHA_INICIAL|FECHA_FINAL|RESPONSABLE)|INDICE_GABINETE:[A-Za-z0-9_]{1,64}|TIPOLOGIA:TIPO_DOCUMENTAL|reglas|seleccion|recepcion|creacion|almacenamiento|confirmacion)\z") Then Return campo
        Return "seleccion"
    End Function
    Public Shared Sub Publicar(salida As RespuestaPerfilImportacion, ex As PerfilImportacionException)
        salida.ok = False
        salida.codigo = If(Texto(ex.Codigo) = Texto(Nothing), "ERROR_INTERNO", ex.Codigo)
        salida.mensaje = Texto(ex.Codigo)
        salida.incidencias.Clear()
        If ex.Codigo = "ACCESO_DENEGADO" OrElse ex.Codigo = "SESION_REQUERIDA" Then Return
        Dim detalle = TryCast(ex, ConfiguracionPerfilImportacionException)
        If detalle Is Nothing OrElse detalle.Incidencias Is Nothing Then Return
        For Each item In detalle.Incidencias.Take(128)
            If item Is Nothing Then Continue For
            'Solo frases fabricadas por este catalogo pueden viajar como detalle.
            Dim segura = Incidencia(item.Campo, item.Codigo)
            segura.Campo = CampoSeguro(item.Campo)
            If Texto(item.Codigo) = Texto(Nothing) Then segura.Codigo = "COMPROBACION_NO_COMPLETADA"
            If item.Mensaje IsNot Nothing AndAlso item.Mensaje.StartsWith(segura.Mensaje, StringComparison.Ordinal) Then
                Dim resto = item.Mensaje.Substring(segura.Mensaje.Length)
                If System.Text.RegularExpressions.Regex.IsMatch(resto, "\A(?: Longitud observada: [0-9]{1,10}\. Máximo permitido: [0-9]{1,10}\.)?(?: Segmento requerido: [0-9]{1,3} \(desde 1\)\.)?(?: Formato requerido: (?:yyyy-MM-dd|yyyyMMdd|dd-MM-yyyy|dd_MM_yyyy|dd/MM/yyyy H:mm:ss)\.)?\z") Then segura.Mensaje = item.Mensaje
            End If
            salida.incidencias.Add(segura)
        Next
    End Sub
    Public Shared Sub Registrar(ex As Exception, fase As String, Optional operacionId As String = Nothing, Optional punto As String = "ENTRADA")
        Try
            If operacionId Is Nothing AndAlso System.Web.HttpContext.Current IsNot Nothing Then operacionId = TryCast(System.Web.HttpContext.Current.Items("DOC120_OPERACION"), String)
            If operacionId Is Nothing OrElse Not System.Text.RegularExpressions.Regex.IsMatch(operacionId, "\A[0-9a-f]{32}\z") Then Return
            If Not {"PREPARACION", "CONFIRMACION", "RECEPCION", "CREACION", "ALMACENAMIENTO", "OPERATIVA"}.Contains(fase) Then fase = "OPERATIVA"
            If Not {"ENTRADA", "COORDINACION_CREACION", "COORDINACION_DOCUMENTO", "ESCRITOR_CREACION", "VERIFICACION_CREACION", "CHECKPOINT_CREACION", "ESCRITOR_DOCUMENTO", "VERIFICACION_DOCUMENTO", "CHECKPOINT_DOCUMENTO"}.Contains(punto) Then punto = "ENTRADA"
            Dim tipo = If(TypeOf ex Is PerfilImportacionException, "RECHAZO_CONTROLADO", If(TypeOf ex Is IO.IOException, "IO_EXCEPTION", "EXCEPTION"))
            Dim registro = "DOC120 operacion=" & operacionId & " fase=" & fase & " punto=" & punto & " captura=" & tipo
            registro &= " version=" & GetType(DiagnosticoOperacionImportacion).Assembly.GetName().Version.ToString()
            Dim odbc = TryCast(ex, System.Data.Odbc.OdbcException)
            Dim mysql = TryCast(ex, MySql.Data.MySqlClient.MySqlException)
            If mysql IsNot Nothing Then registro &= " driver=" & mysql.Number.ToString(Globalization.CultureInfo.InvariantCulture)
            If odbc IsNot Nothing AndAlso odbc.Errors.Count > 0 Then
                Dim estado = odbc.Errors(0).SQLState
                If estado IsNot Nothing AndAlso System.Text.RegularExpressions.Regex.IsMatch(estado, "\A[A-Z0-9]{5}\z") Then registro &= " sqlstate=" & estado
                registro &= " driver=" & odbc.Errors(0).NativeError.ToString(Globalization.CultureInfo.InvariantCulture)
            End If
            Dim detalle = TryCast(ex, ConfiguracionPerfilImportacionException)
            If detalle IsNot Nothing AndAlso System.Web.HttpContext.Current IsNot Nothing Then
                Dim publica As New RespuestaPerfilImportacion()
                Publicar(publica, detalle)
                System.Web.HttpContext.Current.Items("DOC120_INCIDENCIAS") = publica.incidencias
                For Each item In publica.incidencias.Take(5)
                    If Texto(item.Codigo) <> Texto(Nothing) Then registro &= " causa=" & item.Codigo
                Next
            End If
            For Each listener As System.Diagnostics.TraceListener In System.Diagnostics.Trace.Listeners
                Try
                    listener.TraceEvent(New System.Diagnostics.TraceEventCache(), "ImportacionCarpetas", System.Diagnostics.TraceEventType.Warning, 120, registro)
                Catch
                    'Un destino de traza caido no cambia el resultado de negocio.
                End Try
            Next
        Catch
        End Try
    End Sub
    Public Shared Sub Adjuntar(salida As RespuestaPerfilImportacion)
        Dim http = System.Web.HttpContext.Current
        If http Is Nothing OrElse salida.ok OrElse salida.codigo = "ACCESO_DENEGADO" OrElse salida.codigo = "SESION_REQUERIDA" Then Return
        Dim incidencias = TryCast(http.Items("DOC120_INCIDENCIAS"), List(Of IncidenciaPerfilImportacion))
        If incidencias IsNot Nothing Then salida.incidencias = incidencias
    End Sub
End Class
