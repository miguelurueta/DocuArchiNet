Option Strict On
Imports System
Imports System.Data
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports MySql.Data.MySqlClient

'Autoridad operativa independiente de la administracion DOC-110. El destino y
'sus facultades se incorporan al resolver la accion, no desde Session mutable.
Public Class ImportacionCarpetasContextAdapter
    Private ReadOnly _leer As Func(Of HttpContext, UInteger, DataTable)
    Private ReadOnly _recursos As Func(Of String, String, Object(), DataTable)

    Public Sub New()
        _leer = AddressOf LeerAutoridadActual
    End Sub

    Friend Sub New(lector As Func(Of HttpContext, UInteger, DataTable))
        Me.New(lector, Nothing)
    End Sub

    Friend Sub New(lector As Func(Of HttpContext, UInteger, DataTable), recursos As Func(Of String, String, Object(), DataTable))
        If lector Is Nothing Then Throw NoDisponible()
        _leer = lector
        _recursos = recursos
    End Sub

    Public Function Resolver(http As HttpContext) As ContextoImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing Then Throw SesionRequerida()
        Dim usuario = Convert.ToString(http.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture)
        Dim empresa = Convert.ToString(http.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture)
        Dim login = Convert.ToString(http.Session("GA_LOGINUSUARIOGESTION"), CultureInfo.InvariantCulture).Trim()
        If Not ValidadorPerfilImportacion.ReferenciaValida(usuario) OrElse String.IsNullOrWhiteSpace(login) OrElse String.Equals(login, "CONSULTAPUBLICO", StringComparison.OrdinalIgnoreCase) Then Throw SesionRequerida()
        If Not ValidadorPerfilImportacion.ReferenciaValida(empresa) Then Throw Denegado()
        'Siempre se vuelve a RA. No se acepta un flag de permiso de Session,
        'token o contexto anterior, y nunca se exige PuedeAdministrar.
        Dim autoridad = _leer(http, UInteger.Parse(usuario, CultureInfo.InvariantCulture))
        If autoridad Is Nothing Then Throw NoDisponible()
        For Each columna In {"usuario_ref", "empresa_ref", "usuario_activo", "empresa_existente", "puede_importar"}
            If Not autoridad.Columns.Contains(columna) Then Throw NoDisponible()
        Next
        If autoridad.Rows.Count <> 1 Then Throw Denegado()
        Dim fila = autoridad.Rows(0)
        If Texto(fila("usuario_ref")) <> usuario OrElse Texto(fila("empresa_ref")) <> empresa OrElse Texto(fila("empresa_existente")) <> empresa OrElse Not EsUno(fila("usuario_activo")) OrElse Not EsUno(fila("puede_importar")) Then Throw Denegado()
        Return New ContextoImportacionCarpetas With {.UsuarioRef = usuario, .AmbitoRef = empresa, .PuedeImportar = True}
    End Function

    Public Function ResolverDestino(http As HttpContext, nivelRef As String, Optional accion As String = "ABRIR") As ContextoImportacionCarpetas
        If Not ValidadorPerfilImportacion.ReferenciaValida(nivelRef) Then Throw SolicitudInvalida()
        Dim permiso As String
        Select Case accion
            Case "ABRIR", "PREPARAR", "CONFIRMAR", "CONSULTAR", "DETENER_LOTE", "RECONCILIAR", "LIMPIAR_STAGING"
                permiso = "p.agregar_expediente=1 AND p.carga_archivo=1"
            Case "CREAR_EXPEDIENTE"
                permiso = "p.agregar_expediente=1"
            Case "RECIBIR_ARCHIVO", "ALMACENAR_DOCUMENTO"
                permiso = "p.carga_archivo=1"
            Case Else
                Throw SolicitudInvalida()
        End Select
        Dim contexto = Resolver(http)
        Dim destino = LeerRecurso(http, "RA", "SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.id_nivel=@nivel AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=@empresa AND (u.id_Remit_Dest_Int=@usuario OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=@usuario AND p.estado_permiso=1 AND " & permiso & ")) LIMIT 2", "@nivel", nivelRef, "@usuario", contexto.UsuarioRef, "@empresa", contexto.AmbitoRef)
        If destino.Rows.Count <> 1 OrElse Texto(destino.Rows(0)(0)) <> nivelRef Then Throw Denegado()
        Dim cadena As New List(Of NodoDestinoImportacion)()
        Dim vistos As New HashSet(Of String)(StringComparer.Ordinal)
        Dim actual = nivelRef
        While actual <> "0"
            If Not vistos.Add(actual) OrElse vistos.Count > 128 Then Throw Denegado()
            Dim datos = LeerRecurso(http, "RA", "SELECT n.id_nivel,n.id_nivel_padre,n.nombre_nivel,n.estado_nivel,n.remit_dest_interno_id_Remit_Dest_Int AS propietario FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.id_nivel=@nivel AND u.Empresa_Gestion_Documental_id_empresa=@empresa LIMIT 2", "@nivel", actual, "@empresa", contexto.AmbitoRef)
            If datos.Rows.Count <> 1 Then Throw Denegado()
            Dim fila = datos.Rows(0)
            If Texto(fila("id_nivel")) <> actual OrElse Not EsUno(fila("estado_nivel")) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(Texto(fila("propietario"))) Then Throw Denegado()
            Dim padre = If(fila.IsNull("id_nivel_padre"), "0", Texto(fila("id_nivel_padre")))
            If padre <> "0" AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(padre) Then Throw Denegado()
            Dim nombre = Texto(fila("nombre_nivel"))
            If String.IsNullOrWhiteSpace(nombre) OrElse nombre.Any(Function(ch) Char.IsControl(ch)) Then Throw Denegado()
            cadena.Add(New NodoDestinoImportacion With {.Id = actual, .Padre = padre, .Nombre = nombre, .Propietario = Texto(fila("propietario")), .Estado = "1"})
            actual = padre
        End While
        cadena.Reverse()
        Dim origen = LeerRecurso(http, "RA", "SELECT @@hostname AS host,@@port AS puerto,@@server_id AS servidor,DATABASE() AS base")
        If origen.Rows.Count <> 1 OrElse origen.Columns.Count <> 4 OrElse origen.Rows(0).ItemArray.Any(Function(v) String.IsNullOrWhiteSpace(Texto(v))) Then Throw NoDisponible()
        contexto.NivelRef = nivelRef
        contexto.RutaCanonica = String.Join(" / ", cadena.Select(Function(n) n.Nombre))
        Using sha = SHA256.Create()
            Dim documento = PerfilImportacionJson.Escribir(New With {.esquema = 1, .empresa = contexto.AmbitoRef, .origenRA = origen.Rows(0).ItemArray.Select(Function(v) Texto(v)).ToArray(), .niveles = cadena})
            contexto.SelloDestino = BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(documento))).Replace("-", "").ToLowerInvariant()
        End Using
        Return contexto
    End Function

    Public Function AutorizarRecursos(http As HttpContext, referencia As ContextoImportacionCarpetas, configuracion As ConfiguracionPerfilImportacion, accion As String) As ContextoImportacionCarpetas
        If referencia Is Nothing OrElse configuracion Is Nothing Then Throw SolicitudInvalida()
        Dim actual = ResolverDestino(http, referencia.NivelRef, accion)
        If actual.UsuarioRef <> referencia.UsuarioRef OrElse actual.AmbitoRef <> referencia.AmbitoRef Then Throw Denegado()
        If actual.SelloDestino <> referencia.SelloDestino OrElse actual.RutaCanonica <> referencia.RutaCanonica Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "El destino cambió. Revise el plan antes de continuar.")
        Dim ra = If(_recursos Is Nothing, ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), "RA")
        Dim da = If(_recursos Is Nothing, ModuleSessionConnectionStringResolver.Resolve(http, "DA_"), "DA")
        Dim catalogo As New PerfilImportacionCatalogAdapter(ra, da, _recursos)
        catalogo.AutorizarGabineteImportacion(actual, configuracion)
        Return actual
    End Function

    'Preparacion interna para Conservacion: la funcion legacy usa Current y
    'otra construccion de conexion. No reemplazar Session ni abrir esa conexion.
    Public Function AutorizarCreacionLegacy(http As HttpContext, comando As ComandoCreacionExpedienteImportacion) As ContextoImportacionCarpetas
        If comando Is Nothing Then Throw SolicitudInvalida()
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(HttpContext.Current, http) Then Throw SesionRequerida()
        comando.ValidarEntradasLegacy()
        ValidarConexionCreacionLegacy(http)
        Return AutorizarRecursos(http, comando.Contexto, comando.Snapshot.Configuracion, "CREAR_EXPEDIENTE")
    End Function

    Friend Shared Sub ValidarConexionCreacionLegacy(http As HttpContext, Optional prefijo As String = "RA_")
        If http Is Nothing OrElse http.Session Is Nothing Then Throw SesionRequerida()
        If prefijo <> "RA_" AndAlso prefijo <> "DA_" Then Throw SolicitudInvalida()
        Try
            Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, prefijo)
            If String.IsNullOrWhiteSpace(cadena) Then Throw NoDisponible()
            Dim esperada As New MySqlConnectionStringBuilder(cadena)
            'Mismos argumentos que conect.Dbase_Conction_Mysql_RA:38-52.
            'Solo en memoria; no devolver, persistir o registrar las cadenas.
            Dim textoLegacy = "Persist Security Info=True;database=" & Texto(http.Session(prefijo & "DB_NAME_MODULO")) &
                ";server=" & Texto(http.Session(prefijo & "IP_SERVER_MODULO")) &
                ";user id=" & Texto(http.Session(prefijo & "USER_DBMS_MODULO")) &
                ";pwd=" & Texto(http.Session(prefijo & "PASW_DBMS_MODULO")) &
                ";Pooling=" & If(Texto(http.Session(prefijo & "ACTIVA_POOL_DBMS")) = "1", "True", "False") &
                ";Min Pool Size=0;Max Pool Size=" & Texto(http.Session(prefijo & "NUMERO_DBMS_CONEX"))
            Dim legacy As New MySqlConnectionStringBuilder(textoLegacy)
            'Legacy fija PersistSecurityInfo; se admite esa diferencia conocida.
            esperada.PersistSecurityInfo = True
            Dim claves = legacy.Keys.Cast(Of String)().Union(esperada.Keys.Cast(Of String)(), StringComparer.OrdinalIgnoreCase)
            For Each clave In claves
                If Not String.Equals(Convert.ToString(legacy(clave), CultureInfo.InvariantCulture), Convert.ToString(esperada(clave), CultureInfo.InvariantCulture), StringComparison.Ordinal) Then Throw NoDisponible()
            Next
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        Catch ex As FormatException
            Throw NoDisponible()
        Catch ex As OverflowException
            Throw NoDisponible()
        End Try
    End Sub

    Private Class NodoDestinoImportacion
        Public Property Id As String
        Public Property Padre As String
        Public Property Nombre As String
        Public Property Propietario As String
        Public Property Estado As String
    End Class

    Private Function LeerRecurso(http As HttpContext, origen As String, sql As String, ParamArray parametros As Object()) As DataTable
        If _recursos IsNot Nothing Then Return _recursos(origen, sql, parametros)
        Try
            Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, origen & "_")
            If String.IsNullOrWhiteSpace(cadena) Then Throw NoDisponible()
            Using conexion As New MySqlConnection(cadena), cmd As New MySqlCommand(sql, conexion)
                cmd.CommandTimeout = 5
                For i = 0 To parametros.Length - 1 Step 2
                    cmd.Parameters.AddWithValue(CStr(parametros(i)), parametros(i + 1))
                Next
                conexion.Open()
                Using reader = cmd.ExecuteReader()
                    Dim datos As New DataTable()
                    datos.Load(reader)
                    Return datos
                End Using
            End Using
        Catch ex As MySqlException
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Function

    Private Shared Function LeerAutoridadActual(http As HttpContext, usuario As UInteger) As DataTable
        Try
            Dim cadena = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
            If String.IsNullOrWhiteSpace(cadena) Then Throw NoDisponible()
            Using conexion As New MySqlConnection(cadena)
                conexion.Open()
                Using cmd As New MySqlCommand("SELECT u.id_Remit_Dest_Int AS usuario_ref,u.Empresa_Gestion_Documental_id_empresa AS empresa_ref,u.Estado_Usuario AS usuario_activo,e.ID_EMPRESA AS empresa_existente,p.IMPORTAR_CARPETAS AS puede_importar FROM remit_dest_interno u INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa LEFT JOIN remit_dest_interno_perfil_produccion p ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int WHERE u.id_Remit_Dest_Int=@usuario LIMIT 2", conexion)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@usuario", usuario)
                    Using reader = cmd.ExecuteReader()
                        Dim datos As New DataTable()
                        datos.Load(reader)
                        Return datos
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Function

    Private Shared Function Texto(valor As Object) As String
        Return Convert.ToString(valor, CultureInfo.InvariantCulture)
    End Function
    Private Shared Function EsUno(valor As Object) As Boolean
        Return valor IsNot Nothing AndAlso Not Convert.IsDBNull(valor) AndAlso Texto(valor) = "1"
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "La identidad o facultad de importación no está autorizada.")
    End Function
    Private Shared Function SolicitudInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La acción o el destino de importación no son válidos.")
    End Function
    Private Shared Function SesionRequerida() As PerfilImportacionException
        Return New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesión de Gestión vigente.")
    End Function
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar la autoridad de importación.")
    End Function
End Class
