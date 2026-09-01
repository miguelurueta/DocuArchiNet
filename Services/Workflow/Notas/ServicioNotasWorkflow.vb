Imports System

'Servicio de fundación. Valida contexto, tarea y ruta antes de delegar; no conoce ASMX, Session, controles WebForms ni persistencia legacy.
Public Class ServicioNotasWorkflow
    Implements IServicioNotasWorkflow

    'MySQL 5.1 utf8 admite únicamente el plano básico multilingüe. 16.000 unidades UTF-16 usan como máximo 48.000 bytes.
    Friend Const LongitudMaximaContenido As Integer = 16000
    Friend Const TamanoPaginaPredeterminado As Integer = 25
    Friend Const TamanoPaginaMaximo As Integer = 50

    Private ReadOnly _tareaRepository As ITareaWorkflowRepository
    Private ReadOnly _notasRepository As INotasWorkflowRepository
    Private ReadOnly _cursorCodec As INotasWorkflowCursorCodec

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal notasRepository As INotasWorkflowRepository)
        Me.New(tareaRepository, notasRepository, Nothing)
    End Sub

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal notasRepository As INotasWorkflowRepository,
                   ByVal cursorCodec As INotasWorkflowCursorCodec)
        _tareaRepository = tareaRepository
        _notasRepository = notasRepository
        _cursorCodec = cursorCodec
    End Sub

    Public Function Listar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Listar
        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo

        Dim normalizada As SolicitudListarNotasWorkflow = Nothing
        bloqueo = NormalizarSolicitudListar(contexto, tarea, solicitud, normalizada)
        If bloqueo IsNot Nothing Then Return bloqueo

        Dim resultado As ResultadoNotasWorkflow = EjecutarRepositorio(Function() _notasRepository.Listar(contexto, tarea, normalizada))
        If resultado Is Nothing OrElse Not resultado.EsExitoso OrElse Not resultado.TieneMas Then Return resultado
        If resultado.Notas Is Nothing OrElse resultado.Notas.Count = 0 Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "No fue posible continuar el listado de notas.")
        End If

        Dim ultima As NotaWorkflow = resultado.Notas(resultado.Notas.Count - 1)
        resultado.CursorSiguiente = If(_cursorCodec Is Nothing, String.Empty,
                                      _cursorCodec.Proteger(contexto, tarea, ultima.FechaCreacionUtc, ultima.IdNota))
        If String.IsNullOrWhiteSpace(resultado.CursorSiguiente) Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "No fue posible continuar el listado de notas.")
        End If
        Return resultado
    End Function

    Public Function Contar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Contar
        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo
        Return EjecutarRepositorio(Function() _notasRepository.Contar(contexto, tarea, solicitud))
    End Function

    Public Function Crear(ByVal contexto As ContextoModuloWorkflow,
                         ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Crear
        Dim solicitudInvalida As ResultadoNotasWorkflow = ValidarSolicitudCliente(If(solicitud Is Nothing, Nothing, solicitud.IdSolicitudCliente))
        If solicitudInvalida IsNot Nothing Then Return solicitudInvalida
        Dim contenidoInvalido As ResultadoNotasWorkflow = ValidarContenido(If(solicitud Is Nothing, Nothing, solicitud.Contenido))
        If contenidoInvalido IsNot Nothing Then Return contenidoInvalido

        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo
        Return EjecutarRepositorio(Function() _notasRepository.Crear(contexto, tarea, solicitud))
    End Function

    Public Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                              ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Consultar
        Dim notaInvalida As ResultadoNotasWorkflow = ValidarNota(If(solicitud Is Nothing, 0L, solicitud.IdNota))
        If notaInvalida IsNot Nothing Then Return notaInvalida

        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo
        Return EjecutarRepositorio(Function() _notasRepository.Consultar(contexto, tarea, solicitud))
    End Function

    Public Function Actualizar(ByVal contexto As ContextoModuloWorkflow,
                               ByVal solicitud As SolicitudActualizarNotaWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Actualizar
        Dim notaInvalida As ResultadoNotasWorkflow = ValidarNota(If(solicitud Is Nothing, 0L, solicitud.IdNota))
        If notaInvalida IsNot Nothing Then Return notaInvalida
        Dim contenidoInvalido As ResultadoNotasWorkflow = ValidarContenido(If(solicitud Is Nothing, Nothing, solicitud.Contenido))
        If contenidoInvalido IsNot Nothing Then Return contenidoInvalido
        Dim versionInvalida As ResultadoNotasWorkflow = ValidarVersion(If(solicitud Is Nothing, Nothing, solicitud.Version))
        If versionInvalida IsNot Nothing Then Return versionInvalida

        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo
        Return EjecutarRepositorio(Function() _notasRepository.Actualizar(contexto, tarea, solicitud))
    End Function

    Public Function Eliminar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudEliminarNotaWorkflow) As ResultadoNotasWorkflow Implements IServicioNotasWorkflow.Eliminar
        Dim notaInvalida As ResultadoNotasWorkflow = ValidarNota(If(solicitud Is Nothing, 0L, solicitud.IdNota))
        If notaInvalida IsNot Nothing Then Return notaInvalida
        Dim versionInvalida As ResultadoNotasWorkflow = ValidarVersion(If(solicitud Is Nothing, Nothing, solicitud.Version))
        If versionInvalida IsNot Nothing Then Return versionInvalida

        Dim tarea As TareaWorkflow = Nothing
        Dim bloqueo As ResultadoNotasWorkflow = PrepararOperacion(contexto, If(solicitud Is Nothing, 0L, solicitud.IdTarea), tarea)
        If bloqueo IsNot Nothing Then Return bloqueo
        Return EjecutarRepositorio(Function() _notasRepository.Eliminar(contexto, tarea, solicitud))
    End Function

    Private Function PrepararOperacion(ByVal contexto As ContextoModuloWorkflow,
                                       ByVal idTarea As Long,
                                       ByRef tarea As TareaWorkflow) As ResultadoNotasWorkflow
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse Not contexto.PuedeInteractuarAnotaciones Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.Forbidden, "No tiene autorización para interactuar con notas.")
        End If
        If idTarea <= 0 Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.TaskNotActive, "La tarea no está disponible para notas.")
        End If
        If _tareaRepository Is Nothing OrElse _notasRepository Is Nothing Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "El servicio de notas no está disponible.")
        End If

        Try
            tarea = _tareaRepository.ObtenerTarea(contexto, idTarea)
        Catch
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "No fue posible validar la tarea para notas.")
        End Try

        If tarea Is Nothing OrElse Not tarea.EstaActiva OrElse tarea.IdTarea <> idTarea OrElse tarea.IdRuta <= 0 OrElse
           tarea.IdRuta <> contexto.IdRutaWorkflow Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.TaskNotActive, "La tarea no está disponible para notas.")
        End If
        Return Nothing
    End Function

    Private Function NormalizarSolicitudListar(ByVal contexto As ContextoModuloWorkflow,
                                               ByVal tarea As TareaWorkflow,
                                               ByVal solicitud As SolicitudListarNotasWorkflow,
                                               ByRef normalizada As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow
        normalizada = Nothing
        If solicitud Is Nothing Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.TaskNotActive, "La tarea no está disponible para notas.")
        End If
        If solicitud.TamanoPagina < 0 OrElse solicitud.TamanoPagina > TamanoPaginaMaximo Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.NoteNotFound, "La página solicitada no está disponible.")
        End If

        normalizada = New SolicitudListarNotasWorkflow With {
            .IdTarea = solicitud.IdTarea,
            .Cursor = If(solicitud.Cursor, String.Empty),
            .TamanoPagina = If(solicitud.TamanoPagina = 0, TamanoPaginaPredeterminado, solicitud.TamanoPagina)
        }
        If String.IsNullOrWhiteSpace(normalizada.Cursor) Then Return Nothing
        If _cursorCodec Is Nothing Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "El servicio de notas no está disponible.")
        End If

        Dim fechaCursorUtc As DateTime = DateTime.MinValue
        Dim idNotaCursor As Long = 0L
        If Not _cursorCodec.Validar(contexto, tarea, normalizada.Cursor, fechaCursorUtc, idNotaCursor) Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.NoteNotFound, "La página solicitada no está disponible.")
        End If
        normalizada.FechaCursorUtc = fechaCursorUtc
        normalizada.IdNotaCursor = idNotaCursor
        Return Nothing
    End Function

    Private Shared Function ValidarContenido(ByVal contenido As String) As ResultadoNotasWorkflow
        If String.IsNullOrWhiteSpace(contenido) OrElse contenido.IndexOf(ChrW(0)) >= 0 OrElse
           contenido.Length > LongitudMaximaContenido OrElse ContieneCaracterFueraDelPlanoBasico(contenido) Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.InvalidContent, "El contenido de la nota no es válido.")
        End If
        Return Nothing
    End Function

    Private Shared Function ContieneCaracterFueraDelPlanoBasico(ByVal contenido As String) As Boolean
        For Each caracter As Char In contenido
            If Char.IsSurrogate(caracter) Then Return True
        Next
        Return False
    End Function

    Private Shared Function ValidarNota(ByVal idNota As Long) As ResultadoNotasWorkflow
        If idNota <= 0 Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.NoteNotFound, "La nota solicitada no está disponible.")
        End If
        Return Nothing
    End Function

    Private Shared Function ValidarVersion(ByVal version As String) As ResultadoNotasWorkflow
        If String.IsNullOrWhiteSpace(version) Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.VersionConflict, "La versión de la nota no está disponible.")
        End If
        Return Nothing
    End Function

    Private Shared Function ValidarSolicitudCliente(ByVal idSolicitud As String) As ResultadoNotasWorkflow
        Dim solicitud As Guid
        If String.IsNullOrWhiteSpace(idSolicitud) OrElse Not Guid.TryParse(idSolicitud, solicitud) OrElse solicitud = Guid.Empty Then
            Return Bloqueado(CodigosResultadoNotasWorkflow.InvalidContent, "La solicitud de cliente no es válida.")
        End If
        Return Nothing
    End Function

    Private Function EjecutarRepositorio(ByVal operacion As Func(Of ResultadoNotasWorkflow)) As ResultadoNotasWorkflow
        Try
            Dim resultado As ResultadoNotasWorkflow = operacion()
            If resultado Is Nothing Then
                Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "El servicio de notas no está disponible.")
            End If
            Return resultado
        Catch
            Return Bloqueado(CodigosResultadoNotasWorkflow.Unavailable, "El servicio de notas no está disponible.")
        End Try
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String, ByVal mensaje As String) As ResultadoNotasWorkflow
        Return New ResultadoNotasWorkflow With {
            .Codigo = codigo,
            .MensajeFuncional = mensaje
        }
    End Function
End Class
