Option Strict On
Imports System
Imports System.Data.Common
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports MySql.Data.MySqlClient

'Sólo política RA existente. No concede administración de fuentes ni lee sus secretos.
Public Class PoliticaDestinoCargaExcelAdapter
    Private ReadOnly _http As HttpContext
    Private ReadOnly _ctx As ContextoCargaExcel
    Public Sub New(http As HttpContext, ctx As ContextoCargaExcel)
        _http = http : _ctx = ctx
    End Sub
    Public Function Obtener(tabla As String) As PoliticaDestinoCargaExcel
        If Not ServicioPlantillasExcel.TablaValida(tabla) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Dim actual = New ImportacionExcelContextAdapter().Resolver(_http)
        If actual.UsuarioId <> _ctx.UsuarioId OrElse actual.EmpresaId <> _ctx.EmpresaId Then Throw ReglasCargaExcel.Fallo("ACCESO_DENEGADO")
        Dim cadena = ModuleSessionConnectionStringResolver.Resolve(_http, "RA_")
        If String.IsNullOrEmpty(cadena) Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
        Using cn As New MySqlConnection(cadena)
            cn.Open()
            Using q = SqlCargaExcel.Cmd(cn, Nothing, "SELECT fuente_id,revision,modo_aislamiento,columna_empresa,conexion_ref,tabla_nombre,ambito_ref FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre=@tabla AND activo=1 LIMIT 257", "@tabla", tabla)
                Using r = q.ExecuteReader()
                    Dim politicas As New List(Of PoliticaDestinoCargaExcel)
                    While r.Read()
                        If SqlCargaExcel.Texto(r, 4) <> "DA" OrElse SqlCargaExcel.Texto(r, 5) <> tabla OrElse politicas.Count >= 256 Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
                        politicas.Add(New PoliticaDestinoCargaExcel With {.FuenteId = SqlCargaExcel.Texto(r, 0), .Revision = Convert.ToUInt32(r(1), CultureInfo.InvariantCulture), .Modo = SqlCargaExcel.Texto(r, 2), .ColumnaEmpresa = SqlCargaExcel.Texto(r, 3), .AmbitoRef = SqlCargaExcel.Texto(r, 6)})
                    End While
                    Return Seleccionar(politicas, _ctx.EmpresaId)
                End Using
            End Using
        End Using
    End Function
    Public Shared Function Seleccionar(politicas As List(Of PoliticaDestinoCargaExcel), empresa As UInteger) As PoliticaDestinoCargaExcel
        Dim propias = politicas.Where(Function(p) p.AmbitoRef = empresa.ToString(CultureInfo.InvariantCulture)).ToList()
        If propias.Count <> 1 Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        Dim propia = propias(0)
        If propia.Revision = 0 OrElse String.IsNullOrEmpty(propia.FuenteId) OrElse (propia.Modo <> "CONEXION" AndAlso propia.Modo <> "COLUMNA") Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        'Nunca devolver políticas ajenas. Sólo se usan para negar exclusividad contradictoria.
        If propia.Modo = "CONEXION" AndAlso politicas.Count <> 1 Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        If propia.Modo = "COLUMNA" AndAlso (Not ServicioPlantillasExcel.IdentificadorValido(propia.ColumnaEmpresa) OrElse politicas.Any(Function(p) p.Modo <> "COLUMNA" OrElse p.ColumnaEmpresa <> propia.ColumnaEmpresa)) Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
        Return propia
    End Function
End Class
