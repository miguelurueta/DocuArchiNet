Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization

'Lectura adicional del catalogo compartido; no otro repositorio/endpoint.
'Resuelve el ciclo de Produccion aprobado, sin elegir IDs por defecto.
Public Partial Class PerfilImportacionCatalogAdapter
    'Preparacion estrecha para el futuro adaptador: valida antes de leer y
    'comprueba tambien los nombres leidos. No invoca efectos legacy.
    Public Function PrepararDatosCreacionLegacy(comando As ComandoCreacionExpedienteImportacion) As Dictionary(Of String, String)
        If comando Is Nothing Then Throw Invalida()
        comando.ValidarEntradasLegacy()
        Dim datos = ResolverDatosCreacionExpediente(comando)
        comando.ValidarDatosLegacy(datos)
        Return datos
    End Function

    Public Function ResolverDatosCreacionExpediente(comando As ComandoCreacionExpedienteImportacion) As Dictionary(Of String, String)
        If comando Is Nothing Then Throw Invalida()
        Dim contexto = comando.Contexto, c = comando.Snapshot.Configuracion
        Dim autoridad = ParaOperacion(contexto)
        If Not NivelNucleo(autoridad, c, contexto.NivelRef) Then Throw Denegado()
        AutorizarGabineteImportacion(contexto, c)
        Dim salida As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim ciclo = FilaCreacion(Leer(_ra, "SELECT Id_tipos_ciclo_archivo AS id,Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Nombre_Tipo_ciclo_archivo=@nombre LIMIT 2", "@nombre", comando.CicloArchivoNombre), Nothing)
        If Not String.Equals(ciclo("nombre"), comando.CicloArchivoNombre, StringComparison.Ordinal) Then Throw Denegado()
        salida.Add("CICLO_ARCHIVO_ID", ciclo("id")) : salida.Add("CICLO_ARCHIVO_NOMBRE", ciclo("nombre"))
        Dim org = Leer(_ra, "SELECT o.ID_ORGANIGRAMA AS id,o.NOMBRE_ORGANIGRAMA AS nombre FROM registro_organigrama o INNER JOIN ra_registro_instrumento_archivistico i ON i.registro_organigrama_ID_ORGANIGRAMA=o.ID_ORGANIGRAMA INNER JOIN areas_depart_radicacion a ON a.Registro_Organigrama_Id_Organigrama=o.ID_ORGANIGRAMA WHERE o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa AND i.id_instrumento=@instrumento AND i.Estado_instrumento=1 AND a.Codigo_Area=@area AND a.Estado_Area=1 LIMIT 2", "@empresa", contexto.AmbitoRef, "@instrumento", c.InstrumentoRef, "@area", c.AreaRef)
        Dim idOrg = FilaCreacion(org, Nothing)("id")
        Dim nombreOrg = FilaCreacion(org, idOrg)("nombre")
        VerificarNombreCreacion("SELECT ID_ORGANIGRAMA AS id,NOMBRE_ORGANIGRAMA AS nombre FROM registro_organigrama WHERE NOMBRE_ORGANIGRAMA=@nombre AND EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa LIMIT 2", idOrg, nombreOrg, "@empresa", contexto.AmbitoRef)
        salida.Add("ORGANIGRAMA_ID", idOrg) : salida.Add("ORGANIGRAMA_NOMBRE", nombreOrg)
        Dim referencias As New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"FONDO", c.FondoRef}, {"AREA", c.AreaRef}, {"SERIE", c.SerieRef}, {"TIPO_EXPEDIENTE", c.TipoExpedienteRef}, {"UNIDAD_DOCUMENTAL", c.UnidadDocumentalRef}, {"GABINETE", c.GabineteId.Value.ToString(CultureInfo.InvariantCulture)}}
        If Not String.IsNullOrEmpty(c.SubserieRef) Then referencias.Add("SUBSERIE", c.SubserieRef)
        For Each par In referencias
            Dim opciones = ConsultarNucleo(autoridad, par.Key, c, 1, 2, par.Value)
            If opciones Is Nothing OrElse opciones.Count <> 1 OrElse opciones(0).value <> par.Value OrElse String.IsNullOrWhiteSpace(opciones(0).text) Then Throw Denegado()
            Dim nombre = opciones(0).text
            Select Case par.Key
                Case "FONDO"
                    VerificarNombreCreacion("SELECT Id_fondo_documental AS id,Nombre_fondo_documental AS nombre FROM ra_de_fondo_documental WHERE Nombre_fondo_documental=@nombre LIMIT 2", par.Value, nombre)
                Case "AREA"
                    VerificarNombreCreacion("SELECT Codigo_Area AS id,Nombre_Area AS nombre FROM areas_depart_radicacion WHERE Registro_Organigrama_Id_Organigrama=@organigrama AND Nombre_Area=@nombre LIMIT 2", par.Value, nombre, "@organigrama", idOrg)
                Case "SERIE"
                    VerificarNombreCreacion("SELECT Id_Series AS id,Nombre_Serie AS nombre FROM series_documentales WHERE Areas_Depart_Radicacion_Codigo_Area=@area AND Ra_registro_instrumento_archivistico_id_instrumento=@instrumento AND Nombre_Serie=@nombre LIMIT 2", par.Value, nombre, "@area", c.AreaRef, "@instrumento", c.InstrumentoRef)
                Case "SUBSERIE"
                    VerificarNombreCreacion("SELECT Id_SubSeries AS id,Nombre_Subserie AS nombre FROM subseries_documentales WHERE Series_Documentales_Id_Series=@serie AND Nombre_Subserie=@nombre LIMIT 2", par.Value, nombre, "@serie", c.SerieRef)
                Case "UNIDAD_DOCUMENTAL"
                    'El helper legacy aplica Trim antes de buscar: no alterar el nombre.
                    If nombre <> nombre.Trim() Then Throw Denegado()
                    VerificarNombreCreacion("SELECT ID_TIPO_UNIDAD_DOCUMENTAL AS id,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS nombre FROM tipos_unidad_documental WHERE NOMBRE_TIPO_UNIDAD_DOCUMENTAL=@nombre LIMIT 2", par.Value, nombre)
                Case "GABINETE"
                    Dim gabinete = FilaCreacion(Leer(_da, "SELECT id_gabinete AS id,NOMBRE AS nombre FROM system1 WHERE NOMBRE=@nombre LIMIT 2", "@nombre", nombre), par.Value)
                    If gabinete("nombre") <> nombre Then Throw Denegado()
            End Select
            salida.Add(par.Key & "_NOMBRE", nombre)
        Next
        If String.IsNullOrEmpty(c.SubserieRef) Then salida.Add("SUBSERIE_NOMBRE", Nothing)
        'ResponsableRef ya es ID interno resuelto. No repetir la busqueda por
        'identificacion aunque la fuente original de la regla fuera CARPETA.
        If comando.ResponsableRef IsNot Nothing Then
            Dim datos = Leer(_ra, "SELECT id_Remit_Dest_Int AS id,Nombre_Remitente AS nombre,IDENTIFICACION AS identificacion FROM remit_dest_interno WHERE id_Remit_Dest_Int=@responsable AND Estado_Usuario=1 AND Empresa_Gestion_Documental_id_empresa=@empresa LIMIT 2", "@responsable", comando.ResponsableRef, "@empresa", contexto.AmbitoRef)
            Dim fila = FilaCreacion(datos, comando.ResponsableRef)
            If Not datos.Columns.Contains("identificacion") Then Throw CatalogoCreacionNoDisponible()
            If datos.Rows(0).IsNull("identificacion") Then Throw Denegado()
            Dim identificacion = Convert.ToString(datos.Rows(0)("identificacion"), CultureInfo.InvariantCulture)
            If String.IsNullOrWhiteSpace(identificacion) OrElse identificacion.Length > 60 Then Throw Denegado()
            salida.Add("RESPONSABLE_NOMBRE", fila("nombre")) : salida.Add("RESPONSABLE_IDENTIFICACION", identificacion)
        Else
            salida.Add("RESPONSABLE_NOMBRE", Nothing) : salida.Add("RESPONSABLE_IDENTIFICACION", Nothing)
        End If
        Return salida
    End Function

    Private Sub VerificarNombreCreacion(sql As String, esperadoId As String, esperadoNombre As String, ParamArray parametros As Object())
        Dim todos As New List(Of Object)(parametros)
        todos.AddRange({"@nombre", esperadoNombre})
        Dim fila = FilaCreacion(Leer(_ra, sql, todos.ToArray()), esperadoId)
        If fila("nombre") <> esperadoNombre Then Throw Denegado()
    End Sub

    Private Shared Function FilaCreacion(datos As DataTable, esperadoId As String) As Dictionary(Of String, String)
        If datos Is Nothing OrElse Not datos.Columns.Contains("id") OrElse Not datos.Columns.Contains("nombre") Then Throw CatalogoCreacionNoDisponible()
        If datos.Rows.Count <> 1 OrElse datos.Rows(0).IsNull("id") OrElse datos.Rows(0).IsNull("nombre") Then Throw Denegado()
        Dim id = Convert.ToString(datos.Rows(0)("id"), CultureInfo.InvariantCulture)
        Dim nombre = Convert.ToString(datos.Rows(0)("nombre"), CultureInfo.InvariantCulture)
        If Not ValidadorPerfilImportacion.ReferenciaValida(id) OrElse (esperadoId IsNot Nothing AndAlso id <> esperadoId) OrElse String.IsNullOrWhiteSpace(nombre) Then Throw Denegado()
        Return New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"id", id}, {"nombre", nombre}}
    End Function
    Private Shared Function CatalogoCreacionNoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el catálogo para creación.")
    End Function
End Class
