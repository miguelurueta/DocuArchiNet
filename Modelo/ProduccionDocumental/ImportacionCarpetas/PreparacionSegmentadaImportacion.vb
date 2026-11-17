Option Strict On
Imports System
Imports System.Collections.Generic

'Estado interno; no se devuelve el snapshot/inventario completo por transporte.
Public Class PreparacionSegmentadaImportacion
    Public Property PreparacionId As String
    Public Property Contexto As ContextoImportacionCarpetas
    Public Property Snapshot As SnapshotPerfilImportacionOperativa
    Public Property Clave As String
    Public Property CabeceraHash As String
    Public Property TotalRaices As UInteger
    Public Property TotalArchivos As UInteger
    Public Property TotalBytes As String
    Public Property PartesRaices As UInteger
    Public Property PartesArchivos As UInteger
    Public Property Estado As String
    Public Property Revision As UInteger
    Public Property PlanId As String
    Public Property CreadoUtc As String
    Public Property ExpiraUtc As String
End Class

Public Class PartePreparacionImportacion
    Public Property Tipo As String
    Public Property Numero As UInteger
    Public Property Contenido As String
    Public Property Huella As String
    Public Property Estado As String
    Public Property SelloCatalogos As String
End Class

Public Class FragmentoPlanImportacion
    Public Property Tipo As String
    Public Property Numero As UInteger
    Public Property Contenido As String
    Public Property Huella As String
End Class

Public Class ReferenciaPlanSegmentadoImportacion
    Public Property Esquema As UShort
    Public Property PreparacionId As String
    Public Property Huella As String
    Public Property Cantidad As Integer
End Class

'Puerto de preparacion. Ninguna operacion recibe bytes PDF o escritores DA.
Public Interface IPreparacionSegmentadaImportacionRepository
    Sub VerificarEsquemaPreparacion()
    Function IniciarPreparacion(contexto As ContextoImportacionCarpetas, preparacion As PreparacionSegmentadaImportacion) As PreparacionSegmentadaImportacion
    Function ObtenerReferenciaPreparacion(identidad As ContextoImportacionCarpetas, preparacionId As String) As ContextoImportacionCarpetas
    Function ObtenerPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String) As PreparacionSegmentadaImportacion
    Function BuscarPreparacionPorClave(contexto As ContextoImportacionCarpetas, clave As String) As PreparacionSegmentadaImportacion
    Function LeerPartesPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String) As List(Of PartePreparacionImportacion)
    Function RegistrarParte(contexto As ContextoImportacionCarpetas, preparacionId As String, revision As UInteger, parte As PartePreparacionImportacion) As PreparacionSegmentadaImportacion
    Function GuardarValidacionParte(contexto As ContextoImportacionCarpetas, preparacionId As String, revision As UInteger, tipo As String, numero As UInteger, sello As String, fragmentos As List(Of FragmentoPlanImportacion)) As PreparacionSegmentadaImportacion
    Function LeerResultadosPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String) As List(Of FragmentoPlanImportacion)
    Function SellarPreparacion(contexto As ContextoImportacionCarpetas, preparacionId As String, revision As UInteger, clave As String, plan As PlanImportacionCarpetas) As PlanImportacionCarpetas
    Function ObtenerPlan(contexto As ContextoImportacionCarpetas, planId As String) As PlanImportacionCarpetas
    Function ObtenerContextoPlan(identidad As ContextoImportacionCarpetas, planId As String) As ContextoImportacionCarpetas
End Interface

Public Class VistaConsultaFocalImportacion
    Public Property Lectura As EstadoLecturaImportacion
    Public Property Raiz As RaizImportacionCarpetas
    Public Property Archivo As ArchivoImportacionCarpetas
    Public Property HuellaRecursos As String
    Public Property Progreso As ProgresoConfirmadoImportacion
End Class

Public Interface IConsultaFocalImportacionRepository
    Function ConsultarRecursoFocal(contexto As ContextoImportacionCarpetas, loteId As String, tipo As String, recursoId As String) As VistaConsultaFocalImportacion
End Interface

'Solo durante una peticion de preparacion; nunca una cache global o de permisos duradera.
Public Interface ICatalogosLecturaPreparacion
    Sub IniciarLecturasPreparacion(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa)
    Function SelloLecturasPreparacion(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa) As String
End Interface
