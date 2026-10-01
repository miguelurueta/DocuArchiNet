Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public Structure stru_datos_image_lista
    Public id_imagen As Integer
End Structure

Public NotInheritable Class RepositorioAdjuntoFalso
    Implements IContextoAdjuntoRadicacionRepository

    Public Property Respuesta As String = "YES"
    Public Property Contexto As ContextoAdjuntoRadicacion
    Public Property Llamadas As Integer
    Public Property UltimoId As Long

    Public Function ObtenerAutorizado(ByVal contextoModulo As ContextoModulo,
                                      ByVal idRegistroEstado As Long,
                                      ByRef contextoAdjunto As ContextoAdjuntoRadicacion) As String Implements IContextoAdjuntoRadicacionRepository.ObtenerAutorizado
        Llamadas += 1
        UltimoId = idRegistroEstado
        contextoAdjunto = Contexto
        Return Respuesta
    End Function
End Class

Public NotInheritable Class ConexionAdjuntoFalsa
    Implements IDbConnection

    Public Property ConnectionString As String Implements IDbConnection.ConnectionString
    Public ReadOnly Property ConnectionTimeout As Integer Implements IDbConnection.ConnectionTimeout
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Database As String Implements IDbConnection.Database
        Get
            Return "prueba"
        End Get
    End Property
    Public ReadOnly Property State As ConnectionState Implements IDbConnection.State
        Get
            Return ConnectionState.Open
        End Get
    End Property

    Public Function BeginTransaction() As IDbTransaction Implements IDbConnection.BeginTransaction
        Throw New NotSupportedException()
    End Function
    Public Function BeginTransaction(il As IsolationLevel) As IDbTransaction Implements IDbConnection.BeginTransaction
        Throw New NotSupportedException()
    End Function
    Public Sub ChangeDatabase(databaseName As String) Implements IDbConnection.ChangeDatabase
        Throw New NotSupportedException()
    End Sub
    Public Sub Close() Implements IDbConnection.Close
    End Sub
    Public Function CreateCommand() As IDbCommand Implements IDbConnection.CreateCommand
        Throw New NotSupportedException()
    End Function
    Public Sub Open() Implements IDbConnection.Open
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class

Public NotInheritable Class FabricaConexionAdjuntoFalsa
    Implements IModuleConnectionFactory

    Public Property Llamadas As Integer

    Public Function CreateOpenConnection(contexto As ContextoModulo) As IDbConnection Implements IModuleConnectionFactory.CreateOpenConnection
        Llamadas += 1
        Return New ConexionAdjuntoFalsa()
    End Function
End Class

Public NotInheritable Class EjecutorAdjuntoFalso
    Implements IDataExecutor

    Public Property Retorno As Object
    Public Property Llamadas As Integer
    Public Property Sql As String
    Public ReadOnly Property Parametros As New List(Of IDataParameter)()

    Public Function ExecuteNonQuery(connection As IDbConnection, transaction As IDbTransaction, commandText As String,
                                    parameters As IEnumerable(Of IDataParameter)) As Integer Implements IDataExecutor.ExecuteNonQuery
        Throw New NotSupportedException()
    End Function

    Public Function ExecuteScalar(connection As IDbConnection, transaction As IDbTransaction, commandText As String,
                                  parameters As IEnumerable(Of IDataParameter)) As Object Implements IDataExecutor.ExecuteScalar
        Throw New NotSupportedException()
    End Function

    Public Function ExecuteReader(Of T)(connection As IDbConnection, transaction As IDbTransaction, commandText As String,
                                        parameters As IEnumerable(Of IDataParameter), projector As Func(Of IDataReader, T)) As T Implements IDataExecutor.ExecuteReader
        Llamadas += 1
        Sql = commandText
        Parametros.Clear()
        For Each parameter In parameters
            Parametros.Add(parameter)
        Next
        Return DirectCast(Retorno, T)
    End Function
End Class

'Permite compilar el adaptador de composición del repository sin cargar la conexión legacy.
Public Class conect
    Public Class Dbase_Conction_Mysql_RA
        Public Function Returna_Conexion_Mysql(ByRef connection As MySqlConnection) As String
            Return "NO"
        End Function
    End Class
End Class

Module ServicioAdjuntoRadicacionHarness
    Private Sub Exigir(ByVal condicion As Boolean, ByVal mensaje As String)
        If Not condicion Then Throw New InvalidOperationException(mensaje)
    End Sub

    Private Function ContextoValido(ByVal id As Long, ByVal radicado As String) As ContextoAdjuntoRadicacion
        Return New ContextoAdjuntoRadicacion(id, radicado, id + 100, 7, 11, "GABINETE")
    End Function

    Private Function ModuloValido() As ContextoModulo
        Return New ContextoModulo With {
            .CodigoModulo = "RADICACION_SIMPLIFICADA",
            .IdUsuario = 23,
            .LoginUsuario = "usuario-prueba"
        }
    End Function

    Public Sub Main()
        Dim preparaciones As Integer = 0

        Dim fabrica As New FabricaConexionAdjuntoFalsa()
        Dim ejecutor As New EjecutorAdjuntoFalso With {.Retorno = ContextoValido(90, "RAD-90")}
        Dim repositoryReal As New MySqlContextoAdjuntoRadicacionRepository(fabrica, ejecutor)
        Dim contextoResuelto As ContextoAdjuntoRadicacion = Nothing
        Dim respuestaRepository = repositoryReal.ObtenerAutorizado(ModuloValido(), 90, contextoResuelto)
        Exigir(respuestaRepository = "YES" AndAlso Object.ReferenceEquals(contextoResuelto, ejecutor.Retorno),
               "El resolver real no devolvió el contexto inyectado.")
        Exigir(fabrica.Llamadas = 1 AndAlso ejecutor.Llamadas = 1,
               "El resolver real debe abrir y consultar exactamente una vez.")
        Exigir(ejecutor.Sql.Contains("id_estado_radicado = @idRegistroEstado") AndAlso
               ejecutor.Sql.Contains("id_usuario_radicado = @idUsuarioRadicacion") AndAlso
               ejecutor.Parametros.Count = 2,
               "El resolver real no aplicó los límites parametrizados.")
        Exigir(ejecutor.Parametros(0).ParameterName = "@idRegistroEstado" AndAlso
               Convert.ToInt64(ejecutor.Parametros(0).Value) = 90 AndAlso
               ejecutor.Parametros(1).ParameterName = "@idUsuarioRadicacion" AndAlso
               Convert.ToInt32(ejecutor.Parametros(1).Value) = 23,
               "Los parámetros del resolver real no corresponden al estado y usuario.")

        respuestaRepository = repositoryReal.ObtenerAutorizado(ModuloValido(), 0, contextoResuelto)
        Exigir(respuestaRepository <> "YES" AndAlso fabrica.Llamadas = 1 AndAlso ejecutor.Llamadas = 1,
               "Un ID inválido no debe consultar infraestructura.")

        ejecutor.Retorno = Nothing
        respuestaRepository = repositoryReal.ObtenerAutorizado(ModuloValido(), 90, contextoResuelto)
        Exigir(respuestaRepository <> "YES" AndAlso contextoResuelto Is Nothing,
               "Un registro ausente debe fallar de forma cerrada.")

        ejecutor.Retorno = ContextoValido(90, "")
        respuestaRepository = repositoryReal.ObtenerAutorizado(ModuloValido(), 90, contextoResuelto)
        Exigir(respuestaRepository <> "YES" AndAlso contextoResuelto Is Nothing,
               "El resolver real debe rechazar el radicado autoritativo vacío.")

        Dim inexistente As New RepositorioAdjuntoFalso With {
            .Respuesta = "El registro seleccionado no existe o no está autorizado para el usuario actual."
        }
        Dim resultado = New ServicioAdjuntoRadicacion(inexistente).Adjuntar(
            ModuloValido(), 91, "RAD-91",
            Function(contexto)
                preparaciones += 1
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 1, New stru_datos_image_lista())
            End Function)
        Exigir(Not resultado.Exito AndAlso inexistente.Llamadas = 1 AndAlso preparaciones = 0,
               "Un registro inexistente debe fallar antes de preparar.")

        Dim noAutorizado As New RepositorioAdjuntoFalso With {.Respuesta = "No autorizado."}
        resultado = New ServicioAdjuntoRadicacion(noAutorizado).Adjuntar(
            ModuloValido(), 92, "RAD-92",
            Function(contexto)
                preparaciones += 1
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 1, New stru_datos_image_lista())
            End Function)
        Exigir(Not resultado.Exito AndAlso noAutorizado.Llamadas = 1 AndAlso preparaciones = 0,
               "La pertenencia inválida debe fallar antes de preparar.")

        Dim vacio As New RepositorioAdjuntoFalso With {.Contexto = ContextoValido(93, "")}
        resultado = New ServicioAdjuntoRadicacion(vacio).Adjuntar(
            ModuloValido(), 93, "RAD-93",
            Function(contexto)
                preparaciones += 1
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 1, New stru_datos_image_lista())
            End Function)
        Exigir(Not resultado.Exito AndAlso vacio.Llamadas = 1 AndAlso preparaciones = 0,
               "El radicado autoritativo vacío debe fallar antes de preparar.")

        Dim contextoUno = ContextoValido(94, "RAD-94")
        Dim discrepante As New RepositorioAdjuntoFalso With {.Contexto = contextoUno}
        resultado = New ServicioAdjuntoRadicacion(discrepante).Adjuntar(
            ModuloValido(), 94, "OTRO-RADICADO",
            Function(contexto)
                preparaciones += 1
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 1, New stru_datos_image_lista())
            End Function)
        Exigir(Not resultado.Exito AndAlso discrepante.Llamadas = 1 AndAlso preparaciones = 0,
               "La discrepancia informativa debe fallar antes de preparar.")

        Dim aceptado As New RepositorioAdjuntoFalso With {.Contexto = contextoUno}
        resultado = New ServicioAdjuntoRadicacion(aceptado).Adjuntar(
            ModuloValido(), 94, "",
            Function(contexto)
                preparaciones += 1
                Exigir(Object.ReferenceEquals(contextoUno, contexto), "El contexto se volvió a resolver.")
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 401, New stru_datos_image_lista With {.id_imagen = 401})
            End Function)
        Exigir(resultado.Exito AndAlso resultado.IdImagen = 401 AndAlso resultado.DatosImagen.id_imagen = 401,
               "El valor informativo vacío debe conservar el contexto autoritativo.")
        Exigir(aceptado.Llamadas = 1 AndAlso preparaciones = 1,
               "Una carga lógica debe resolver y preparar exactamente una vez.")

        Dim contextoDos = ContextoValido(95, "RAD-95")
        Dim aislado As New RepositorioAdjuntoFalso With {.Contexto = contextoDos}
        resultado = New ServicioAdjuntoRadicacion(aislado).Adjuntar(
            ModuloValido(), 95, "RAD-95",
            Function(contexto)
                Exigir(contexto.IdRegistroEstado = 95 AndAlso contexto.Radicado = "RAD-95",
                       "Una carga recibió datos de otra carga.")
                Return ResultadoAdjuntoRadicacion.Correcto(contexto, 402, New stru_datos_image_lista With {.id_imagen = 402})
            End Function)
        Exigir(resultado.Exito AndAlso aislado.Llamadas = 1 AndAlso aceptado.UltimoId = 94,
               "Las cargas deben permanecer aisladas.")

        Dim propiedades = GetType(ContextoAdjuntoRadicacion).GetProperties()
        For Each propiedad In propiedades
            Exigir(Not propiedad.CanWrite, "El contexto autoritativo debe ser inmutable.")
        Next
        Exigir(contextoDos.IdTareaWorkflow = 195 AndAlso contextoDos.IdTipoTramite = 7 AndAlso
               contextoDos.IdPlantilla = 11 AndAlso contextoDos.NombreGabinete = "GABINETE",
               "El contexto no materializó todos sus campos.")

        Console.WriteLine("PASS ServicioAdjuntoRadicacion")
    End Sub
End Module
