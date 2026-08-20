Imports System
Imports System.Collections.Generic

'Modelos internos para el nuevo limite Workflow. No contienen Page, Session ni detalles de persistencia.
Public Class ContextoModuloWorkflow
    Inherits ContextoModulo

    Public Sub New()
        CodigoModulo = "WORKFLOW"
    End Sub

    Public Property IdUsuarioWorkflow As Integer
        Get
            Return IdUsuario
        End Get
        Set(ByVal value As Integer)
            IdUsuario = value
        End Set
    End Property

    Public Property IdGrupoWorkflow As Integer
        Get
            Return IdGrupo
        End Get
        Set(ByVal value As Integer)
            IdGrupo = value
        End Set
    End Property

    Public Property IdRutaWorkflow As Integer
    Public Property IdUsuarioGestion As Integer
    Public Property PuedeCambioRuta As Boolean

    Public Overrides Function EsValido() As Boolean
        Return MyBase.EsValido() AndAlso IdGrupoWorkflow > 0 AndAlso IdRutaWorkflow > 0 AndAlso
               String.Equals(CodigoModulo, "WORKFLOW", StringComparison.OrdinalIgnoreCase)
    End Function
End Class

Public Class TareaWorkflow
    Public Property IdEstado As Long
    Public Property IdTarea As Long
    Public Property Radicado As String
    Public Property IdActividadOrigen As Integer
    Public Property IdActividadFlujoTrabajo As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdRuta As Integer
    Public Property IdGrupoWorkflow As Integer
    Public Property GrupoActual As String
    Public Property TipoDecision As String
    Public Property RequiereNotificacion As Boolean
    Public Property TokenVersion As String
    Public Property EstaActiva As Boolean
End Class

Public Class ResultadoDestinosTransicion
    Public Sub New()
        Destinos = New List(Of DestinoTransicion)()
    End Sub

    Public Property Destinos As IList(Of DestinoTransicion)
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class DestinoTransicion
    Public Property IdConector As Integer
    Public Property IdActividadDestino As Integer
    Public Property IdUsuarioWorkflowDestino As Integer
    Public Property IdGrupoWorkflowDestino As Integer
    Public Property Nombre As String
    Public Property NombreDestinatario As String
    Public Property NombreGrupo As String
    Public Property TipoTransicion As String
    Public Property Orden As Integer
End Class

Public Class RequisitoTransicion
    Public Property Codigo As String
    Public Property Descripcion As String
    Public Property Obligatorio As Boolean
    Public Property Satisfecho As Boolean
End Class

Public Class SolicitudTransicionWorkflow
    Public Property IdTarea As Long
    Public Property IdConector As Integer
    Public Property TokenVersion As String
End Class

'Solicitud exclusiva de Enviar a grupo. No admite conectores ni datos Web Forms.
Public Class SolicitudEnvioGrupoWorkflow
    Public Property IdTarea As Long
    Public Property IdActividadDestino As Integer
    Public Property TokenVersion As String
End Class

Public Class SolicitudBusquedaDestinosEnvioGrupo
    Public Property IdTarea As Long
    Public Property Termino As String
    Public Property Pagina As Integer
    Public Property TamanoPagina As Integer
End Class

Public Class DestinoEnvioGrupoWorkflow
    Public Property IdActividadDestino As Integer
    Public Property IdGrupoWorkflowDestino As Integer
    Public Property NombreActividad As String
    Public Property NombreGrupoDestino As String
    Public Property RequiereNotificacion As Boolean
End Class

Public Class ResultadoDestinosEnvioGrupo
    Public Sub New()
        Destinos = New List(Of DestinoEnvioGrupoWorkflow)()
    End Sub

    Public Property Destinos As IList(Of DestinoEnvioGrupoWorkflow)
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class ResultadoBusquedaDestinosEnvioGrupo
    Public Sub New()
        Destinos = New List(Of DestinoEnvioGrupoWorkflow)()
    End Sub

    Public Property Destinos As IList(Of DestinoEnvioGrupoWorkflow)
    Public Property Pagina As Integer
    Public Property TamanoPagina As Integer
    Public Property TieneMas As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
End Class

Public Class ResultadoResolucionEnvioGrupo
    Public Property Destino As DestinoEnvioGrupoWorkflow
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String

    Public ReadOnly Property EsValido As Boolean
        Get
            Return Destino IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CodigoBloqueo)
        End Get
    End Property
End Class

Public Class ResultadoRequisitosEnvioGrupo
    Public Sub New()
        Requisitos = New List(Of RequisitoTransicion)()
    End Sub

    Public Property Cumple As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property Requisitos As IList(Of RequisitoTransicion)
End Class

Public Class DestinoEjecucionWorkflow
    Public Property IdConector As Integer
    Public Property TipoTransicion As String
    Public Property IdActividadDestino As Integer
    Public Property IdUsuarioWorkflowDestino As Integer
    Public Property IdGrupoWorkflowDestino As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadFlujoTrabajoDestino As Integer
    Public Property IdUsuarioWorkflowFlujoTrabajoDestino As Integer
    Public Property IdUsuarioWorkflowFuente As Integer
    Public Property IdActividadFlujoTrabajoFuente As Integer
    Public Property RequiereNotificacion As Boolean
    Public Property NombreActividadDestino As String
    Public Property NombreDestinatario As String
    Public Property NombreGrupoDestino As String
End Class

Public Class ResultadoResolucionDestinoTransicion
    Public Property Destino As DestinoEjecucionWorkflow
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String

    Public ReadOnly Property EsValido As Boolean
        Get
            Return Destino IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CodigoBloqueo)
        End Get
    End Property
End Class

Public Class ResultadoRequisitosTransicion
    Public Sub New()
        Requisitos = New List(Of RequisitoTransicion)()
    End Sub

    Public Property Cumple As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property Requisitos As IList(Of RequisitoTransicion)
End Class

Public Class ResultadoGuardTransicion
    Public Property Adquirido As Boolean
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property Lease As ITransicionConcurrencyLease
End Class

Public Class ResultadoEjecucionWorkflow
    Public Sub New()
        Advertencias = New List(Of String)()
    End Sub

    Public Property Exito As Boolean
    Public Property EstadoFinal As String
    Public Property CodigoBloqueo As String
    Public Property MensajeFuncional As String
    Public Property ReferenciaAuditoria As String
    Public Property EsReintentable As Boolean
    Public Property Advertencias As IList(Of String)
End Class

Public Class HabilitacionWorkflowModern
    Public Property Estado As String
    Public Property Codigo As String
    Public Property MensajeFuncional As String

    Public ReadOnly Property EstaActiva As Boolean
        Get
            Return String.Equals(Estado, "activo", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property
End Class

Public Class AuditoriaTransicion
    Public Property IdTarea As Long
    Public Property IdUsuarioWorkflow As Integer
    Public Property IdRutaWorkflow As Integer
    Public Property IdFlujoTrabajo As Integer
    Public Property IdActividadOrigen As Integer
    Public Property IdActividadDestino As Integer
    Public Property IdConector As Integer
    Public Property Canal As String
    Public Property Mecanismo As String
    Public Property FechaUtc As DateTime
    Public Property DuracionMilisegundos As Long
    Public Property Resultado As String
    Public Property CodigoFuncional As String
    Public Property Referencia As String
End Class
