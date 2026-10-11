Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceLoginSegundoFactor
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ObtenerEstado() As SegundoFactorEstadoDto
        Dim mapper As New LoginSecondFactorPresentationMapper()
        Try
            Dim pending As PendingSecondFactorContext = New LoginSecondFactorSessionGate().AsegurarContexto()
            If pending Is Nothing Then Return mapper.Failure("SECOND_FACTOR_SESSION_INVALID").Estado
            Return mapper.MapState(CreateService(pending).GetStatus())
        Catch
            Return mapper.Failure("SECOND_FACTOR_UNAVAILABLE").Estado
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function Verificar(ByVal codigo As String) As SegundoFactorResultadoDto
        Return Execute(Function(service) service.Verify(codigo))
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function Reenviar() As SegundoFactorResultadoDto
        Return Execute(Function(service) service.Resend())
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function Cancelar() As SegundoFactorResultadoDto
        Return Execute(Function(service) service.Cancel())
    End Function

    Private Function Execute(ByVal operation As Func(Of ISecondFactorAuthenticationService, SecondFactorAuthenticationResult)) As SegundoFactorResultadoDto
        Dim mapper As New LoginSecondFactorPresentationMapper()
        Try
            Dim pending As PendingSecondFactorContext = New LoginSecondFactorSessionGate().AsegurarContexto()
            If pending Is Nothing Then Return mapper.Failure("SECOND_FACTOR_SESSION_INVALID")
            Return mapper.MapResult(operation(CreateService(pending)))
        Catch
            Return mapper.Failure("SECOND_FACTOR_UNAVAILABLE")
        End Try
    End Function

    Private Shared Function CreateService(ByVal pending As PendingSecondFactorContext) As ISecondFactorAuthenticationService
        Return LoginSecondFactorCompositionFactory.CreateFromPending(HttpContext.Current, New ClassGestorSesion(), pending)
    End Function
End Class
