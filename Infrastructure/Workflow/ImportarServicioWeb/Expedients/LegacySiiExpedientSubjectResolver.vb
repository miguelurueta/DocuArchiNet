Imports System

' Adaptador transitorio: conserva SolicitaEstructuraExpedienteSII sin modificarla y sanea su contrato textual.
Public NotInheritable Class LegacySiiExpedientSubjectResolver
    Implements ISiiExpedientSubjectResolver

    Private ReadOnly _normalizer As IImportExpedientIdentityNormalizer

    Public Sub New(ByVal normalizer As IImportExpedientIdentityNormalizer)
        If normalizer Is Nothing Then Throw New ArgumentNullException("normalizer")
        _normalizer = normalizer
    End Sub

    Public Function Resolver(ByVal contexto As ContextoImportacionServicio,
                             ByVal inscripcion As InscripcionImportacion,
                             ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion Implements ISiiExpedientSubjectResolver.Resolver
        If contexto Is Nothing OrElse inscripcion Is Nothing OrElse configuracion Is Nothing OrElse String.IsNullOrWhiteSpace(configuracion.NombreGabinete) Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "SII_SUBJECT_INPUT_INVALID")
        Dim legacy As New StruSiiCahcheInscripcion()
        Dim response = New ClassConsultaExpedienteSII().SolicitaEstructuraExpedienteSII(inscripcion.Matricula, inscripcion.Proponente, configuracion.NombreGabinete, legacy)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "SII_SUBJECT_UNAVAILABLE")
        inscripcion.IdentificacionSujeto = If(legacy.NitIdentificacion, String.Empty).Trim()
        inscripcion.RazonSocial = If(legacy.Rsocial, String.Empty).Trim()
        inscripcion.Matricula = If(legacy.Matricula, inscripcion.Matricula)
        inscripcion.NombrePropietario = If(legacy.NombrePropietario, String.Empty).Trim()
        inscripcion.IdentificacionPropietario = If(legacy.Identificacionpro, String.Empty).Trim()
        inscripcion.MatriculaPropietario = If(legacy.MatriculaPropietario, String.Empty).Trim()
        If String.IsNullOrWhiteSpace(inscripcion.IdentificacionSujeto) OrElse String.IsNullOrWhiteSpace(inscripcion.RazonSocial) Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, "SII_SUBJECT_INCOMPLETE")
        Dim normalized = _normalizer.Normalizar(contexto, configuracion.NombreGabinete, inscripcion.Matricula, inscripcion.Proponente)
        If Not normalized.Valida Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, normalized.Codigo)
        inscripcion.MatriculaNormalizada = normalized.ValorPersistencia
        Dim identityResult = MaterializeIdentityFields(inscripcion, configuracion)
        If identityResult IsNot Nothing Then Return identityResult
        Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "SII_SUBJECT_CONFIRMED")
    End Function

    Friend Shared Function MaterializeIdentityFields(ByVal inscripcion As InscripcionImportacion,
                                                     ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
        If configuracion.IdAutoRegistro <= 0 OrElse configuracion.CamposIdentidad Is Nothing OrElse configuracion.CamposIdentidad.Count = 0 Then
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_IDENTITY_CONFIGURATION_INVALID")
        End If

        Try
            Dim legacyAutoRegistration As New Class_ra_auto_registro_expediente()
            Dim autoRegistrationName As String = String.Empty
            Dim dataFunction As String = String.Empty
            Dim response = legacyAutoRegistration.SolicitaDatosAutoRegistro(configuracion.IdAutoRegistro, autoRegistrationName, dataFunction)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrWhiteSpace(dataFunction) Then
                Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_IDENTITY_FUNCTION_UNAVAILABLE")
            End If

            Dim legacyFields(configuracion.CamposIdentidad.Count - 1) As stru_campos_expediente
            For index As Integer = 0 To configuracion.CamposIdentidad.Count - 1
                legacyFields(index).campo_expediente = configuracion.CamposIdentidad(index).NombreCampo
                legacyFields(index).estado_obligatorio = If(configuracion.CamposIdentidad(index).Obligatorio, 1, 0)
                legacyFields(index).estado_unico = 1
                legacyFields(index).valor_campo_expediente = String.Empty
            Next

            Dim authoritative As New CIncripcionSII With {
                .LIBRO_SII = inscripcion.Libro,
                .REGISTRO_SII = inscripcion.Registro,
                .MATRICULA_SII = inscripcion.MatriculaNormalizada,
                .PROPONENTE_SII = If(String.Equals(configuracion.NombreGabinete, "RUP", StringComparison.OrdinalIgnoreCase), inscripcion.MatriculaNormalizada, inscripcion.Proponente),
                .NIT_SII = inscripcion.IdentificacionSujeto,
                .RSOCIAL_SII = inscripcion.RazonSocial,
                .RADICADO_SII = inscripcion.RadicadoSii
            }
            response = legacyAutoRegistration.SolicitaDatosFuncionAutoRegistro(dataFunction, authoritative, configuracion.IdAutoRegistro, legacyFields)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_IDENTITY_MATERIALIZATION_FAILED")
            End If

            response = MaterializeManagementFields(configuracion.IdAutoRegistro, legacyFields)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Result(EstadoEfectoExpedienteImportacion.Fallido, response)
            End If

            For index As Integer = 0 To configuracion.CamposIdentidad.Count - 1
                configuracion.CamposIdentidad(index).Valor = If(legacyFields(index).valor_campo_expediente, String.Empty).Trim()
            Next
            Return Nothing
        Catch
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_IDENTITY_MATERIALIZATION_FAILED")
        End Try
    End Function

    Private Shared Function MaterializeManagementFields(ByVal autoRegistrationId As Integer,
                                                        ByRef fields() As stru_campos_expediente) As String
        Dim fundId As Integer = 0
        Dim instrumentId As Integer = 0
        Dim areaId As Integer = 0
        Dim seriesId As Integer = 0
        Dim subseriesId As Integer = 0
        Dim response = New Class_ra_auto_campos_gestion_expediente().SolicitaDatosGestionCamposAutoRegistro(
            autoRegistrationId, fundId, instrumentId, areaId, seriesId, subseriesId)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_CONFIGURATION_UNAVAILABLE"

        Dim areaName As String = String.Empty
        Dim fundName As Object = String.Empty
        Dim seriesName As String = String.Empty
        Dim subseriesName As String = String.Empty
        If areaId <> 0 Then
            response = New Class_areas_depart_radicacion().Solicita_nombre_area_departamento(areaId, areaName)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_AREA_UNAVAILABLE"
        End If
        If fundId <> 0 Then
            response = New Class_ra_de_fondo_documental().Retorna_nombre_fondo_documental(fundId, fundName)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_FUND_UNAVAILABLE"
        End If
        If seriesId <> 0 Then
            response = New Class_series_documentales().Solicita_nombre_serie_documental(seriesId, seriesName)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_SERIES_UNAVAILABLE"
        End If
        If subseriesId <> 0 Then
            response = New Class_subseries_documentales().Retorna_nombre_sub_serie(subseriesId, subseriesName)
            If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_SUBSERIES_UNAVAILABLE"
        End If

        Dim conservationUnitName As String = "CARPETA CUATRO ALETAS"
        Dim conservationUnitId As Integer = 0
        response = New Class_tipo_unidad_conservacion().Retorna_id_tipo_unidad_conservacion_expediente(conservationUnitName, conservationUnitId, 2)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_UNIT_UNAVAILABLE"
        Dim creationDate As String = Date.Today
        response = New ClassGestionFechas().FormateaFechaAlmacenamiento(creationDate)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return "EXPEDIENT_MANAGEMENT_DATE_UNAVAILABLE"

        For index As Integer = 0 To fields.Length - 1
            Select Case fields(index).campo_expediente
                Case "CODIGO_AREA_TRD" : fields(index).valor_campo_expediente = areaId
                Case "NOMBRE_AREA_TRD" : fields(index).valor_campo_expediente = areaName
                Case "CODIGO_SERIE_TRD" : fields(index).valor_campo_expediente = seriesId
                Case "NOMBRE_SERIE_TRD" : fields(index).valor_campo_expediente = seriesName
                Case "CODIGO_SUB_SERIE_TRD" : fields(index).valor_campo_expediente = subseriesId
                Case "NOMBRE_SUBSERIE_TRD" : fields(index).valor_campo_expediente = subseriesName
                Case "NOMBRE_TIPO_UNIDAD_DOCUMENTAL" : fields(index).valor_campo_expediente = conservationUnitName
                Case "ID_TIPO_UNIDAD_DOCUMENTAL" : fields(index).valor_campo_expediente = conservationUnitId
                Case "ID_FONDO" : fields(index).valor_campo_expediente = fundId
                Case "NOMBRE_FONDO" : fields(index).valor_campo_expediente = Convert.ToString(fundName)
                Case "id_instrumento" : fields(index).valor_campo_expediente = instrumentId
                Case "FECHA_CREACION" : fields(index).valor_campo_expediente = creationDate
            End Select
        Next
        Return "YES"
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion, ByVal code As String) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {.Estado = state, .Codigo = code}
    End Function
End Class
