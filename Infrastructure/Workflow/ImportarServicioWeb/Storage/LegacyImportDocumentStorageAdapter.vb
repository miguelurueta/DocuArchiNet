Imports System
Imports System.Collections.Generic
Imports System.IO

' Único límite nuevo autorizado para invocar AlmacenaDocumentoTareaWorkflow.
Public NotInheritable Class LegacyImportDocumentStorageAdapter
    Implements IImportDocumentStoragePort

    Public Function Almacenar(ByVal comando As ComandoAlmacenamientoImportacion) As ResultadoFaseImportacion Implements IImportDocumentStoragePort.Almacenar
        If comando Is Nothing OrElse String.IsNullOrWhiteSpace(comando.RutaArchivo) OrElse comando.IdTareaWorkflow <= 0 Then
            Return Fallo("INVALID_STORAGE_COMMAND", "No fue posible preparar el documento.", True)
        End If
        Try
            Dim idImagen As Integer = 0
            Dim imagen As stru_datos_image_lista = Nothing
            Dim campos As New List(Of CDcamposAsignaAlmacenamiento)()
            For Each campo In comando.Campos
                If campo IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(campo.Nombre) Then
                    campos.Add(New CDcamposAsignaAlmacenamiento With {.NombreCampoGabinete = campo.Nombre, .ValorCampoGabinete = If(campo.Valor, String.Empty)})
                End If
            Next
            Dim respuesta = New ClassAlmacenamiento().AlmacenaDocumentoTareaWorkflow(
                1, comando.NombreGabinete, comando.Radicado, comando.RutaArchivo,
                comando.NombreRutaWorkflow, comando.IdRutaWorkflow, comando.IdTareaWorkflow,
                comando.DescripcionTipo, comando.IdTipoListaChequeo, comando.TipoAlmacenamiento,
                campos, ValorCampo(comando.Campos, "MATRICULA"), comando.NombreCaso, comando.NombreClaseFormatoDocumento,
                idImagen, imagen)
            If String.Equals(respuesta, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True, .IdDocumento = idImagen}
            End If
            Return Fallo(ClasificarRechazo(respuesta), MensajeDiagnosticoLegacy(respuesta & " | " & DiagnosticoFormato(comando)), True)
        Catch ex As Exception
            Return Fallo("DOCUMENT_STORAGE_UNCERTAIN", MensajeDiagnosticoLegacy(ex.Message & " | " & DiagnosticoFormato(comando)), False)
        End Try
    End Function

    Private Shared Function ClasificarRechazo(ByVal respuesta As String) As String
        Dim texto = If(respuesta, String.Empty).ToUpperInvariant()
        If texto.Contains("USUARIO DOCUARCHI.NET DEBE ESTAR ASOCIADO") Then Return "DOCUMENT_STORAGE_GESTION_USER_MISSING"
        If texto.Contains("NO TIENE UNA EMPRESA ASOCIADA") Then Return "DOCUMENT_STORAGE_GESTION_COMPANY_MISSING"
        If texto.Contains("IDIMAGEN") OrElse texto.Contains("IMAGEN TAREA") OrElse texto.Contains("DAT_ADIC") Then Return "DOCUMENT_STORAGE_TASK_IMAGE_LINK_UNAVAILABLE"
        If texto.Contains("TIPOFORMATO") OrElse texto.Contains("TIPO FORMATO") OrElse texto.Contains("CLASE FORMATO") Then Return "DOCUMENT_STORAGE_DOCUMENT_CLASS_UNAVAILABLE"
        If texto.Contains("FORMATEAFECHA") OrElse texto.Contains("FORMATEA FECHA") Then Return "DOCUMENT_STORAGE_DATE_INVALID"
        If texto.Contains("ARCHIVO SOLICITADO") OrElse texto.Contains("IMPOSIBLE ACCEDER AL ARCHIVO") Then Return "DOCUMENT_STORAGE_SOURCE_FILE_UNAVAILABLE"
        If texto.Contains("CAMPO") OrElse texto.Contains("GABINETE_DETALLE") OrElse texto.Contains("MATRICES") Then Return "DOCUMENT_STORAGE_CABINET_FIELDS_INVALID"
        If texto.Contains("EXTENSION") OrElse texto.Contains("TIPO ARCHIVO") OrElse texto.Contains("TIPOARCHIVO") OrElse texto.Contains("NÚMERO DE PAGINAS") OrElse texto.Contains("NUMERO DE PAGINAS") OrElse texto.Contains("PAGINAS_DOCUMENTOS") Then Return "DOCUMENT_STORAGE_FILE_FORMAT_INVALID"
        If texto.Contains("EXPEDIENTE") OrElse texto.Contains("UNIDAD DE CONSERV") OrElse texto.Contains("TRD") Then Return "DOCUMENT_STORAGE_CLASSIFICATION_INVALID"
        If texto.Contains("CONEXION") OrElse texto.Contains("CONECTAR") OrElse texto.Contains("BASE DE DATOS") OrElse texto.Contains("ACCESS DENIED") OrElse texto.Contains("MYSQL") OrElse texto.Contains("UNKNOWN DATABASE") Then Return "DOCUMENT_STORAGE_CONNECTION_UNAVAILABLE"
        If texto.Contains("RUTA") Then Return "DOCUMENT_STORAGE_ROUTE_UNAVAILABLE"
        Return "DOCUMENT_STORAGE_REJECTED"
    End Function

    ' Conserva la causa funcional de la frontera legacy para diagnóstico sin publicar
    ' secretos, cadenas de conexión, URLs con query ni texto sin límite.
    Private Shared Function MensajeDiagnosticoLegacy(ByVal respuesta As String) As String
        Dim texto = If(respuesta, String.Empty)
        texto = Text.RegularExpressions.Regex.Replace(texto, "[\r\n\t]+", " ").Trim()
        If texto.Length = 0 Then Return "La función legacy rechazó el almacenamiento sin informar detalle."
        texto = Text.RegularExpressions.Regex.Replace(texto,
            "(?i)\b(password|pwd|contrase(?:ña|na)|token|authorization|cookie|connection\s*string|cadena\s+de\s+conexi(?:ó|o)n)\b\s*[:=]\s*[^;,\s]+",
            "$1=[REDACTED]")
        texto = Text.RegularExpressions.Regex.Replace(texto, "(?i)(https?://[^?\s]+)\?[^\s]+", "$1?[REDACTED]")
        If texto.Length > 500 Then texto = texto.Substring(0, 500) & "…"
        Return texto
    End Function

    Private Shared Function DiagnosticoFormato(ByVal comando As ComandoAlmacenamientoImportacion) As String
        If comando Is Nothing Then Return "format-diagnostic=command-missing"
        Dim originalName = Path.GetFileName(If(comando.NombreArchivoOrigen, String.Empty))
        Dim preparedExtension = Path.GetExtension(If(comando.RutaArchivo, String.Empty))
        Return "nombreArchivo=" & ValorDiagnostico(originalName) &
            "; formatoSii=" & ValorDiagnostico(comando.FormatoProveedor) &
            "; tipoContenido=" & ValorDiagnostico(comando.TipoContenidoOrigen) &
            "; extensionPreparada=" & ValorDiagnostico(preparedExtension) &
            "; extensionLegacy=" & ValorDiagnostico(New FileInfo(comando.RutaArchivo).Extension)
    End Function

    Private Shared Function ValorDiagnostico(ByVal value As String) As String
        Dim safeValue = Text.RegularExpressions.Regex.Replace(If(value, String.Empty), "[^\p{L}\p{N} ._()/-]+", "?").Trim()
        If safeValue.Length = 0 Then Return "[VACIO]"
        Return If(safeValue.Length > 120, safeValue.Substring(0, 120) & "…", safeValue)
    End Function

    Private Shared Function ValorCampo(ByVal campos As IList(Of CampoAlmacenamientoImportacion), ByVal nombre As String) As String
        For Each campo In campos
            If campo IsNot Nothing AndAlso String.Equals(campo.Nombre, nombre, StringComparison.OrdinalIgnoreCase) Then Return campo.Valor
        Next
        Return String.Empty
    End Function

    Private Shared Function Fallo(ByVal codigo As String, ByVal mensaje As String, ByVal conocida As Boolean) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Codigo = codigo, .MensajeVisible = mensaje, .PersistenciaConocida = conocida, .Reintentable = conocida}
    End Function
End Class
