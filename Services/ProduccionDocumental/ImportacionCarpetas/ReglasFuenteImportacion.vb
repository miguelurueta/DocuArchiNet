Option Strict On
Imports System
Imports System.Linq
Imports System.Text.RegularExpressions

'Validacion estructural compartida; el servicio debe contrastar ademas los metadatos del origen.
Public NotInheritable Class ReglasFuenteImportacion
    Public Shared Sub Autorizar(c As ContextoPerfilImportacion)
        If c Is Nothing OrElse Not c.PuedeAdministrar OrElse Not c.PuedeAdministrarFuentes OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.AmbitoRef) Then Fallar("ACCESO_DENEGADO")
    End Sub
    Public Shared Sub Validar(accion As String, cambio As CambioFuenteImportacion)
        If Not {"ALTA", "EDITAR", "ACTIVAR", "REVOCAR"}.Contains(accion) OrElse cambio Is Nothing OrElse cambio.Documento Is Nothing OrElse Not ValidadorPerfilImportacion.TextoValido(cambio.Motivo, 500, False) OrElse String.IsNullOrWhiteSpace(cambio.Motivo) Then Fallar("SOLICITUD_INVALIDA")
        Dim d = cambio.Documento
        If accion <> "ALTA" AndAlso (Not ValidadorPerfilImportacion.ReferenciaValida(d.FuenteId) OrElse d.Revision = 0UI OrElse d.Revision = UInteger.MaxValue) Then Fallar("SOLICITUD_INVALIDA")
        If accion = "ALTA" AndAlso (d.FuenteId IsNot Nothing OrElse d.Revision <> 0UI) Then Fallar("SOLICITUD_INVALIDA")
        If accion = "ACTIVAR" OrElse accion = "REVOCAR" Then Return
        If Not {"RA", "DA"}.Contains(d.ConexionRef) OrElse Not Nombre(d.Tabla) OrElse Not {"COLUMNA", "CONEXION"}.Contains(d.ModoAislamiento) OrElse d.Columnas Is Nothing OrElse d.Columnas.Count < 1 OrElse d.Columnas.Count > 128 Then Fallar("CONFIGURACION_INVALIDA")
        If d.Columnas.Any(Function(x) Not Nombre(x)) OrElse d.Columnas.Distinct(StringComparer.OrdinalIgnoreCase).Count() <> d.Columnas.Count Then Fallar("CONFIGURACION_INVALIDA")
        If d.ModoAislamiento = "COLUMNA" Then
            If Not Nombre(d.ColumnaEmpresa) OrElse Not d.Columnas.Contains(d.ColumnaEmpresa) Then Fallar("CONFIGURACION_INVALIDA")
        ElseIf Not String.IsNullOrEmpty(d.ColumnaEmpresa) Then
            Fallar("CONFIGURACION_INVALIDA")
        End If
    End Sub
    Public Shared Sub Pagina(pagina As Integer)
        If pagina < 1 OrElse pagina > 1000000 Then Fallar("SOLICITUD_INVALIDA")
    End Sub
    Public Shared Sub Id(valor As String)
        If Not ValidadorPerfilImportacion.ReferenciaValida(valor) Then Fallar("SOLICITUD_INVALIDA")
    End Sub
    Private Shared Function Nombre(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Regex.IsMatch(valor, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z")
    End Function
    Private Shared Sub Fallar(codigo As String)
        Throw New PerfilImportacionException(codigo, "Revise la autorizacion y el contrato de la fuente.")
    End Sub
End Class
