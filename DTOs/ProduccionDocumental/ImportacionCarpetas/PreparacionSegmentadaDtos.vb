Option Strict On
Imports System.Collections.Generic

Public Interface ISolicitudPreparacionSegmentada
End Interface

Public Class IniciarPreparacionPlanSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property nivelRef As String
    Public Property versionId As String
    Public Property clavePreparacion As String
    Public Property totalRaices As UInteger
    Public Property totalArchivos As UInteger
    Public Property totalBytes As String
    Public Property partesRaices As UInteger
    Public Property partesArchivos As UInteger
End Class

Public Class ConsultarRecursoLoteSolicitud
    Inherits ConsultarLoteImportacionSolicitud
    Public Property versionLectura As Integer = 1
    Public Property tipo As String
    Public Property recursoId As String
End Class

Public Class ProgresoConfirmadoImportacion
    Public Property recibidos As Integer
    Public Property expedientes As Integer
    Public Property datos As Integer
    Public Property guardados As Integer
    Public Property pendientesRecepcion As Integer
    Public Property incidencias As Integer
    Public Property bytesRecibidos As String
End Class

Public Class ConsultaFocalImportacionResultado
    Public Property versionLectura As Integer = 1
    Public Property estado As EstadoLoteImportacionResultado
    Public Property tipo As String
    Public Property recurso As RecursoConsultaImportacionResultado
    Public Property padre As RecursoConsultaImportacionResultado
    Public Property huellaRecursos As String
    Public Property progreso As ProgresoConfirmadoImportacion
End Class

Public Class RegistrarPartePreparacionSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property preparacionId As String
    Public Property revisionEsperada As UInteger
    Public Property tipo As String
    Public Property numero As UInteger
    Public Property raices As New List(Of RaizManifiestoImportacionSolicitud)()
    Public Property archivos As New List(Of ArchivoManifiestoImportacionSolicitud)()
End Class

Public Class ConsultarPreparacionPlanSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property preparacionId As String
    Public Property clavePreparacion As String
    Public Property nivelRef As String
    Public Property tipo As String = "RESUMEN"
    Public Property pagina As Integer = 1
End Class

Public Class ValidarPartePreparacionSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property preparacionId As String
    Public Property revisionEsperada As UInteger
    Public Property tipo As String
    Public Property numero As UInteger
End Class

Public Class SellarPreparacionPlanSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property preparacionId As String
    Public Property revisionEsperada As UInteger
    Public Property claveCierre As String
End Class

Public Class ConsultarRevisionPlanSolicitud
    Implements ISolicitudPreparacionSegmentada
    Public Property planId As String
    Public Property revision As UInteger
    Public Property tipo As String = "RESUMEN"
    Public Property pagina As Integer = 1
    Public Property ambito As String
End Class

Public Class EstadoPartePreparacionResultado
    Public Property tipo As String
    Public Property numero As UInteger
    Public Property hash As String
    Public Property estado As String
End Class

Public Class EstadoPreparacionPlanResultado
    Public Property existe As Boolean = True
    Public Property preparacionId As String
    Public Property revision As UInteger
    Public Property estado As String
    Public Property planId As String
    Public Property expiraUtc As String
    Public Property totalRaices As UInteger
    Public Property totalArchivos As UInteger
    Public Property partesRecibidas As Integer
    Public Property partesValidadas As Integer
    Public Property archivosRegistrados As Integer
    Public Property archivosRevisados As Integer
    Public Property pagina As Integer
    Public Property puedeHaberMas As Boolean
    Public Property partes As New List(Of EstadoPartePreparacionResultado)()
End Class

Public Class RevisionPlanSegmentadoResultado
    Public Property planId As String
    Public Property revision As UInteger
    Public Property planHash As String
    Public Property nivelRef As String
    Public Property expiraUtc As String
    Public Property puedeConfirmar As Boolean
    Public Property unidadesElegibles As Integer
    Public Property totalArchivos As Integer
    Public Property totalIncidencias As Integer
    Public Property tipo As String
    Public Property pagina As Integer
    Public Property puedeHaberMas As Boolean
    Public Property valores As New List(Of ValoresElementoImportacion)()
    Public Property unidadesElegiblesPagina As New List(Of String)()
    Public Property incidencias As New List(Of IncidenciaImportacionCarpetas)()
End Class
