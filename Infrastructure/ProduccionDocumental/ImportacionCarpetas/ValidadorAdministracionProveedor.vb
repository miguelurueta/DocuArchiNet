Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class ValidadorAdministracionProveedor
    Private ReadOnly _politica As PoliticaFuentesProveedor
    Private ReadOnly _meta As MetadatosProveedorIndices
    Public Sub New(politica As PoliticaFuentesProveedor, meta As MetadatosProveedorIndices)
        _politica = politica : _meta = meta
    End Sub
    Public Sub Validar(c As ContextoPerfilImportacion, contrato As ContratoProveedorIndices, completo As Boolean)
        If contrato Is Nothing OrElse contrato.Propiedades Is Nothing OrElse contrato.Propiedades.Count > 128 OrElse Not {"TEXTO", "ENTERO", "DECIMAL", "FECHA"}.Contains(contrato.TipoClave) Then Throw Invalido()
        For Each nombre In {contrato.Tabla, contrato.ColumnaClave, contrato.ColumnaEmpresa}
            If Not String.IsNullOrEmpty(nombre) AndAlso Not Identificador(nombre) Then Throw Invalido()
        Next
        Dim nombres As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each p In contrato.Propiedades
            If p Is Nothing OrElse Not Identificador(p.Nombre) OrElse Not Identificador(p.Columna) OrElse Not {"TEXTO", "ENTERO", "DECIMAL", "FECHA"}.Contains(p.Tipo) OrElse Not nombres.Add(p.Nombre) Then Throw Invalido()
        Next
        PerfilImportacionJson.Escribir(contrato)
        Dim fuentes = _politica.Fuentes(c)
        If String.IsNullOrEmpty(contrato.ConexionRef) Then
            If completo OrElse Not String.IsNullOrEmpty(contrato.Tabla) OrElse Not String.IsNullOrEmpty(contrato.ColumnaClave) OrElse Not String.IsNullOrEmpty(contrato.ColumnaEmpresa) OrElse contrato.Propiedades.Count > 0 Then Throw Invalido()
            Return
        End If
        If Not fuentes.Any(Function(f) f.ConexionRef = contrato.ConexionRef) Then Throw Denegado()
        If String.IsNullOrEmpty(contrato.Tabla) Then
            If completo OrElse Not String.IsNullOrEmpty(contrato.ColumnaClave) OrElse Not String.IsNullOrEmpty(contrato.ColumnaEmpresa) OrElse contrato.Propiedades.Count > 0 Then Throw Invalido()
            Return
        End If
        Dim fuente = fuentes.SingleOrDefault(Function(f) f.ConexionRef = contrato.ConexionRef AndAlso f.Tabla = contrato.Tabla)
        If fuente Is Nothing Then Throw Denegado()
        If (Not String.IsNullOrEmpty(contrato.ColumnaClave) AndAlso Not fuente.Columnas.Contains(contrato.ColumnaClave)) OrElse (Not String.IsNullOrEmpty(contrato.ColumnaEmpresa) AndAlso Not fuente.Columnas.Contains(contrato.ColumnaEmpresa)) OrElse contrato.Propiedades.Any(Function(p) Not fuente.Columnas.Contains(p.Columna)) Then Throw Denegado()
        If Not completo Then Return
        If fuente.ColumnaEmpresaAutorizada IsNot Nothing AndAlso If(contrato.ColumnaEmpresa, "") <> fuente.ColumnaEmpresaAutorizada Then Throw Denegado()
        If String.IsNullOrEmpty(contrato.ColumnaEmpresa) AndAlso Not fuente.ExclusivaEmpresa Then Throw Denegado()
        Dim documento As New DocumentoProveedorAdmin With {.ProveedorId = "1", .VersionId = "1", .Contrato = contrato}
        'IDs sinteticos se usan exclusivamente para validar la forma, nunca para consultar datos.
        If Not MySqlProveedorDatosIndices.ConfiguracionValida(ServicioAdministracionProveedor.VersionConsulta(c, documento)) Then Throw Invalido()
        Dim columnas As New List(Of OpcionPerfilImportacion)()
        For pagina = 1 To 6
            Dim lote = _meta.Consultar(c, "COLUMNAS", contrato.ConexionRef, contrato.Tabla, pagina)
            columnas.AddRange(lote)
            If lote.Count < 25 Then Exit For
        Next
        If Not columnas.Any(Function(x) x.value = contrato.ColumnaClave AndAlso x.Tipo = contrato.TipoClave) OrElse (Not String.IsNullOrEmpty(contrato.ColumnaEmpresa) AndAlso Not columnas.Any(Function(x) x.value = contrato.ColumnaEmpresa AndAlso {"TEXTO", "ENTERO"}.Contains(x.Tipo))) Then Throw Invalido()
        For Each p In contrato.Propiedades
            If Not columnas.Any(Function(x) x.value = p.Columna AndAlso x.Tipo = p.Tipo) Then Throw Invalido()
        Next
    End Sub
    Private Shared Function Identificador(v As String) As Boolean
        Return v IsNot Nothing AndAlso Regex.IsMatch(v, "\A[A-Za-z_][A-Za-z0-9_]{0,63}\z")
    End Function
    Private Shared Function Invalido() As PerfilImportacionException
        Return New PerfilImportacionException("CONFIGURACION_INVALIDA", "Complete las referencias y propiedades autorizadas del proveedor.")
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "La fuente no esta autorizada para el contexto.")
    End Function
End Class
