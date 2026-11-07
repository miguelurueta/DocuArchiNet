Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq

'Lectura estructural de IDs ya conocidos. No descubre efectos por nombre,
'no atribuye causalidad al intento ni devuelve permiso para repetir creacion.
Public Partial Class MySqlImportacionCarpetasRepository
    Public Function ComprobarResultadoCreacion(comando As ComandoCreacionExpedienteImportacion, identidad As IdentidadExpedienteImportacion, datos As Dictionary(Of String, String)) As Boolean
        If comando Is Nothing Then Throw SolicitudInvalida()
        comando.ValidarEntradasLegacy() : comando.ValidarDatosLegacy(datos)
        Return Operar(Function(conexion, tx) LeerResultadoCreacion(comando, identidad, datos, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)))
    End Function

    Friend Shared Function LeerResultadoCreacion(comando As ComandoCreacionExpedienteImportacion, identidad As IdentidadExpedienteImportacion, datos As Dictionary(Of String, String), consultar As Func(Of String, Object(), DataTable)) As Boolean
        If comando Is Nothing OrElse consultar Is Nothing Then Throw NoDisponible()
        comando.ValidarEntradasLegacy() : comando.ValidarDatosLegacy(datos)
        Dim leidos As DataTable = Nothing
        Dim estructura = LeerClasificacionExpedienteConocido(comando.Contexto, identidad, comando.Snapshot.Configuracion,
            Function(sql, parametros)
                Dim campos = "e.CODIGO_UNICO AS codigo,e.TEMA_EXPEDIENTE AS nombre,e.ALEAS_EXPEDIENTE AS alias_visible,CAST(e.FECHA_EXTREMA_INICIAL AS CHAR) AS fecha_inicial,CAST(e.FECHA_EXTREMA_FINAL AS CHAR) AS fecha_final,e.Id_tipos_ciclo_archivo AS ciclo_id,e.NOMBRE_CICLO_ARCHIVO AS ciclo_nombre,e.GABINETE_PRODUCION AS gabinete,e.NOMBRE_RESPONSABLE_EXPEDIENTE AS responsable_nombre,e.IDENFICACION_RESPONSABLE_EXPEDIENTE AS responsable_identificacion,e.VOLUMEN_EXPEDIENTE AS volumen,"
                leidos = consultar(sql.Insert("SELECT ".Length, campos), parametros)
                Return leidos
            End Function)
        If Not estructura Then Return False
        Dim esperados As New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"codigo", comando.Codigo}, {"nombre", comando.Nombre}, {"alias_visible", comando.Nombre}, {"fecha_inicial", comando.FechaInicial}, {"fecha_final", comando.FechaFinal}, {"ciclo_id", datos("CICLO_ARCHIVO_ID")}, {"ciclo_nombre", comando.CicloArchivoNombre}, {"gabinete", datos("GABINETE_NOMBRE")}, {"responsable_nombre", datos("RESPONSABLE_NOMBRE")}, {"responsable_identificacion", datos("RESPONSABLE_IDENTIFICACION")}, {"volumen", "1"}}
        For Each par In esperados
            If Not leidos.Columns.Contains(par.Key) Then Throw NoDisponible()
            If par.Value Is Nothing Then
                If Not leidos.Rows(0).IsNull(par.Key) Then Return False
            ElseIf leidos.Rows(0).IsNull(par.Key) OrElse Not String.Equals(Convert.ToString(leidos.Rows(0)(par.Key), CultureInfo.InvariantCulture), par.Value, StringComparison.Ordinal) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Function ComprobarClasificacionExpedienteConocido(contexto As ContextoImportacionCarpetas, identidad As IdentidadExpedienteImportacion, configuracion As ConfiguracionPerfilImportacion) As Boolean
        AutorizarOperacion(contexto)
        ValidarReferenciaExpedienteConocido(identidad)
        ValidarClasificacionExpedienteConocido(configuracion)
        If identidad.NivelRef <> contexto.NivelRef Then Return False
        Return Operar(Function(conexion, tx) LeerClasificacionExpedienteConocido(contexto, identidad, configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)))
    End Function

    Public Function ComprobarRelacionExpedienteConocido(contexto As ContextoImportacionCarpetas, identidad As IdentidadExpedienteImportacion) As Boolean
        AutorizarOperacion(contexto)
        ValidarReferenciaExpedienteConocido(identidad)
        If identidad.NivelRef <> contexto.NivelRef Then Return False
        Return Operar(Function(conexion, tx) LeerRelacionExpedienteConocido(contexto, identidad, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)))
    End Function

    Friend Shared Function LeerRelacionExpedienteConocido(contexto As ContextoImportacionCarpetas, identidad As IdentidadExpedienteImportacion, consultar As Func(Of String, Object(), DataTable)) As Boolean
        Return LeerExpedienteConocido(contexto, identidad, consultar, Nothing)
    End Function

    Friend Shared Function LeerClasificacionExpedienteConocido(contexto As ContextoImportacionCarpetas, identidad As IdentidadExpedienteImportacion, configuracion As ConfiguracionPerfilImportacion, consultar As Func(Of String, Object(), DataTable)) As Boolean
        ValidarClasificacionExpedienteConocido(configuracion)
        Return LeerExpedienteConocido(contexto, identidad, consultar, configuracion)
    End Function

    Private Shared Function LeerExpedienteConocido(contexto As ContextoImportacionCarpetas, identidad As IdentidadExpedienteImportacion, consultar As Func(Of String, Object(), DataTable), configuracion As ConfiguracionPerfilImportacion) As Boolean
        AutorizarOperacion(contexto)
        ValidarReferenciaExpedienteConocido(identidad)
        If consultar Is Nothing Then Throw NoDisponible()
        If identidad.NivelRef <> contexto.NivelRef Then Return False
        Dim sql = "SELECT e.ID_EXPEDIENTE AS expediente,r.id_registro AS relacion,r.ra_pro_niveles_id_nivel AS nivel,e.ID_EMPRESA_EXPEDIENTE AS empresa,e.ID_USUARIO_GESTION AS usuario,n.estado_nivel AS nivel_activo,e.estado_expediente_electronico AS electronico,u.Empresa_Gestion_Documental_id_empresa AS empresa_nivel,CASE WHEN (n.remit_dest_interno_id_Remit_Dest_Int=@usuario OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=@usuario AND p.estado_permiso=1 AND p.agregar_expediente=1)) THEN 1 ELSE 0 END AS autorizado FROM ra_pro_niveles_has_expediente_archivo r INNER JOIN expediente_archivo e ON e.ID_EXPEDIENTE=r.expediente_archivo_ID_EXPEDIENTE INNER JOIN ra_pro_niveles n ON n.id_nivel=r.ra_pro_niveles_id_nivel INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE r.id_registro=@relacion AND e.ID_EXPEDIENTE=@expediente AND n.id_nivel=@nivel AND e.ID_EMPRESA_EXPEDIENTE=@empresa AND e.ID_USUARIO_GESTION=@usuario AND u.Empresa_Gestion_Documental_id_empresa=@empresa LIMIT 2"
        If configuracion IsNot Nothing Then
            sql = sql.Insert("SELECT ".Length, "e.ID_FONDO AS fondo,e.id_instrumento AS instrumento,e.CODIGO_AREA_TRD AS area,e.CODIGO_SERIE_TRD AS serie,e.CODIGO_SUB_SERIE_TRD AS subserie,e.RA_TIP_EXPE_ID_TIPO_EXPEDIENTE AS tipo,e.ID_TIPO_UNIDAD_DOCUMENTAL AS unidad,")
        End If
        Dim datos = consultar(sql, New Object() {"@relacion", IdOperacion(identidad.RelacionNivelId), "@expediente", IdOperacion(identidad.ExpedienteId), "@nivel", IdOperacion(contexto.NivelRef), "@empresa", IdOperacion(contexto.AmbitoRef), "@usuario", IdOperacion(contexto.UsuarioRef)})
        If datos Is Nothing Then Throw NoDisponible()
        Dim esperados As New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"expediente", identidad.ExpedienteId}, {"relacion", identidad.RelacionNivelId}, {"nivel", contexto.NivelRef}, {"empresa", contexto.AmbitoRef}, {"usuario", contexto.UsuarioRef}, {"nivel_activo", "1"}, {"electronico", "2"}, {"empresa_nivel", contexto.AmbitoRef}, {"autorizado", "1"}}
        If configuracion IsNot Nothing Then
            For Each par In ClasificacionEsperada(configuracion)
                esperados.Add(par.Key, par.Value)
            Next
            If Not datos.Columns.Contains("subserie") Then Throw NoDisponible()
            If Not String.IsNullOrEmpty(configuracion.SubserieRef) Then esperados.Add("subserie", configuracion.SubserieRef)
        End If
        For Each campo In esperados.Keys
            If Not datos.Columns.Contains(campo) Then Throw NoDisponible()
        Next
        If datos.Rows.Count <> 1 Then Return False
        'Conservacion persiste NULL cuando no se solicita subserie; cero/vacio
        'no se normalizan a ausencia para ocultar una discrepancia.
        If configuracion IsNot Nothing AndAlso String.IsNullOrEmpty(configuracion.SubserieRef) AndAlso Not datos.Rows(0).IsNull("subserie") Then Return False
        Return esperados.All(Function(par) Not datos.Rows(0).IsNull(par.Key) AndAlso String.Equals(Convert.ToString(datos.Rows(0)(par.Key), CultureInfo.InvariantCulture), par.Value, StringComparison.Ordinal))
    End Function

    Private Shared Function ClasificacionEsperada(configuracion As ConfiguracionPerfilImportacion) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"fondo", configuracion.FondoRef}, {"instrumento", configuracion.InstrumentoRef}, {"area", configuracion.AreaRef}, {"serie", configuracion.SerieRef}, {"tipo", configuracion.TipoExpedienteRef}, {"unidad", configuracion.UnidadDocumentalRef}}
    End Function

    Private Shared Sub ValidarClasificacionExpedienteConocido(configuracion As ConfiguracionPerfilImportacion)
        If configuracion Is Nothing Then Throw SolicitudInvalida()
        If ClasificacionEsperada(configuracion).Values.Any(Function(valor) Not ValidadorPerfilImportacion.ReferenciaValida(valor)) Then Throw SolicitudInvalida()
        If Not String.IsNullOrEmpty(configuracion.SubserieRef) AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(configuracion.SubserieRef) Then Throw SolicitudInvalida()
    End Sub

    Private Shared Sub ValidarReferenciaExpedienteConocido(identidad As IdentidadExpedienteImportacion)
        If identidad Is Nothing OrElse identidad.OrigenLogico <> "RA" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.ExpedienteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.RelacionNivelId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.NivelRef) Then Throw SolicitudInvalida()
    End Sub
End Class
