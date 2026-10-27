Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class ValidadorPerfilImportacion
    Private ReadOnly _reglas As New ResolvedorReglasPerfilImportacion()

    Public Function Validar(configuracion As ConfiguracionPerfilImportacion, reglas As List(Of ReglaPerfilImportacion), publicar As Boolean) As List(Of IncidenciaPerfilImportacion)
        Dim errores = _reglas.ValidarEstructura(reglas)
        If configuracion Is Nothing Then
            errores.Add(New IncidenciaPerfilImportacion("configuracion", "CONFIGURACION_REQUERIDA", "Indique la configuración."))
            Return errores
        End If
        If configuracion.Esquema <> 1 Then ErrorCampo(errores, "Esquema", "ESQUEMA_INVALIDO")
        If Not TextoValido(configuracion.NombreVisible, 160, Not publicar) Then ErrorCampo(errores, "NombreVisible", "NOMBRE_INVALIDO")
        If Not {"BLOQUEAR", "REUTILIZAR_VALIDADO"}.Contains(configuracion.PoliticaCoincidencias) Then ErrorCampo(errores, "PoliticaCoincidencias", "POLITICA_INVALIDA")
        If configuracion.ModoIntegridad = "PARCIAL_PERMITIDO" Then
            configuracion.AmbitoIntegridad = "NO_APLICA"
        ElseIf configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO" Then
            If Not String.IsNullOrEmpty(configuracion.AmbitoIntegridad) AndAlso Not {"EXPEDIENTE", "LOTE"}.Contains(configuracion.AmbitoIntegridad) Then ErrorCampo(errores, "AmbitoIntegridad", "AMBITO_INVALIDO")
            If publicar AndAlso String.IsNullOrEmpty(configuracion.AmbitoIntegridad) Then ErrorCampo(errores, "AmbitoIntegridad", "AMBITO_REQUERIDO")
        Else
            ErrorCampo(errores, "ModoIntegridad", "MODO_INVALIDO")
        End If
        Opcion(errores, "EstrategiaCarpetas", configuracion.EstrategiaCarpetas, {"RAIZ_EXPEDIENTE"}, publicar)
        Opcion(errores, "PoliticaSubcarpetas", configuracion.PoliticaSubcarpetas, {"INCLUIR", "RECHAZAR"}, publicar)
        Opcion(errores, "PoliticaCarpetasVacias", configuracion.PoliticaCarpetasVacias, {"RECHAZAR"}, publicar)
        Opcion(errores, "PoliticaArchivosSueltos", configuracion.PoliticaArchivosSueltos, {"RECHAZAR"}, publicar)
        For Each referencia In New Dictionary(Of String, String) From {
            {"FondoRef", configuracion.FondoRef}, {"InstrumentoRef", configuracion.InstrumentoRef},
            {"AreaRef", configuracion.AreaRef}, {"SerieRef", configuracion.SerieRef},
            {"SubserieRef", configuracion.SubserieRef}, {"TipoExpedienteRef", configuracion.TipoExpedienteRef},
            {"UnidadDocumentalRef", configuracion.UnidadDocumentalRef}}
            If Not String.IsNullOrEmpty(referencia.Value) AndAlso Not ReferenciaValida(referencia.Value) Then ErrorCampo(errores, referencia.Key, "REFERENCIA_INVALIDA")
            If publicar AndAlso referencia.Key <> "SubserieRef" AndAlso String.IsNullOrEmpty(referencia.Value) Then ErrorCampo(errores, referencia.Key, "REFERENCIA_REQUERIDA")
        Next
        If configuracion.GabineteId.HasValue AndAlso configuracion.GabineteId.Value = 0UI Then ErrorCampo(errores, "GabineteId", "REFERENCIA_INVALIDA")
        If publicar AndAlso Not configuracion.GabineteId.HasValue Then ErrorCampo(errores, "GabineteId", "GABINETE_REQUERIDO")
        If configuracion.MaximoBytesArchivo.HasValue AndAlso configuracion.MaximoBytesArchivo.Value <= 0 Then ErrorCampo(errores, "MaximoBytesArchivo", "LIMITE_INVALIDO")
        If configuracion.Extensiones Is Nothing OrElse configuracion.Extensiones.Count > 64 Then
            ErrorCampo(errores, "Extensiones", "EXTENSIONES_INVALIDAS")
        Else
            Dim vistos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each extension In configuracion.Extensiones
                If String.IsNullOrEmpty(extension) OrElse extension.Length > 16 OrElse extension.Any(Function(c) Not ((c >= "a"c AndAlso c <= "z"c) OrElse (c >= "0"c AndAlso c <= "9"c))) OrElse Not vistos.Add(extension) Then ErrorCampo(errores, "Extensiones", "EXTENSION_INVALIDA")
            Next
        End If
        If errores.Count > 0 Then Return errores
        If publicar Then
            For Each campo In {"CODIGO", "NOMBRE", "FECHA_INICIAL"}
                If Not reglas.Any(Function(r) r.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso r.DestinoRef = campo) Then ErrorCampo(errores, "CAMPO_EXPEDIENTE:" & campo, "FUENTE_REQUERIDA")
            Next
            If Not reglas.Any(Function(r) r.DestinoTipo = "TIPOLOGIA") Then ErrorCampo(errores, "TIPOLOGIA", "FUENTE_REQUERIDA")
        End If
        'Constantes se comprueban también al guardar; fuentes futuras no requieren datos ficticios.
        errores.AddRange(_reglas.Resolver(reglas.Where(Function(r) r.FuenteTipo = "CONSTANTE").ToList(), New EjemploPerfilImportacion()).Incidencias)
        Return errores
    End Function

    Public Shared Function TextoValido(valor As String, maximo As Integer, opcional As Boolean) As Boolean
        If String.IsNullOrWhiteSpace(valor) Then Return opcional
        Return valor.Length <= maximo AndAlso Not valor.Any(Function(c) Char.IsSurrogate(c) OrElse c = ChrW(0))
    End Function

    Public Shared Function ReferenciaValida(valor As String) As Boolean
        Dim numero As UInteger
        Return UInteger.TryParse(valor, Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture, numero) AndAlso numero > 0 AndAlso numero.ToString(Globalization.CultureInfo.InvariantCulture) = valor
    End Function

    Private Shared Sub Opcion(errores As List(Of IncidenciaPerfilImportacion), campo As String, valor As String, opciones As String(), requerido As Boolean)
        If String.IsNullOrEmpty(valor) Then
            If requerido Then ErrorCampo(errores, campo, "OPCION_REQUERIDA")
        ElseIf Not opciones.Contains(valor) Then
            ErrorCampo(errores, campo, "OPCION_INVALIDA")
        End If
    End Sub

    Private Shared Sub ErrorCampo(errores As List(Of IncidenciaPerfilImportacion), campo As String, codigo As String)
        errores.Add(New IncidenciaPerfilImportacion(campo, codigo, "Revise la configuración del campo indicado."))
    End Sub
End Class
