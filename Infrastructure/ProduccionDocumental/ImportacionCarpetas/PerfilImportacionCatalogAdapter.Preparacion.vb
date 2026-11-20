Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq

Public Partial Class PerfilImportacionCatalogAdapter
    Implements ICatalogosLecturaPreparacion
    Private _lecturasPreparacion As Dictionary(Of String, DataTable)
    Private _ambitoLecturasPreparacion As String

    Public Sub IniciarLecturasPreparacion(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa) Implements ICatalogosLecturaPreparacion.IniciarLecturasPreparacion
        ParaOperacion(contexto)
        If snapshot Is Nothing OrElse String.IsNullOrEmpty(snapshot.Huella) Then Throw Invalida()
        _ambitoLecturasPreparacion = PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(New With {.contexto = contexto, .publicacion = snapshot.Huella}))
        _lecturasPreparacion = New Dictionary(Of String, DataTable)(StringComparer.Ordinal)
    End Sub

    Private Function ClaveLecturaPreparacion(cadena As String, sql As String, parametros As Object()) As String
        'El modulo es una etiqueta, nunca se serializa la cadena de conexion.
        Dim modulo = If(String.Equals(cadena, _ra, StringComparison.Ordinal), "RA", "DA")
        Dim valores = parametros.Select(Function(p) If(p Is Nothing OrElse Convert.IsDBNull(p), "NULL", p.GetType().FullName & ":" & TextoParametroPreparacion(p))).ToList()
        Return PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(New With {.ambito = _ambitoLecturasPreparacion, .modulo = modulo, .consulta = sql, .parametros = valores}))
    End Function

    Private Shared Function TextoParametroPreparacion(p As Object) As String
        If TypeOf p Is Byte() Then Return Convert.ToBase64String(DirectCast(p, Byte()))
        If TypeOf p Is DateTime Then Return DirectCast(p, DateTime).ToString("o", CultureInfo.InvariantCulture)
        Return Convert.ToString(p, CultureInfo.InvariantCulture)
    End Function

    Public Function SelloLecturasPreparacion(contexto As ContextoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa) As String Implements ICatalogosLecturaPreparacion.SelloLecturasPreparacion
        Dim cache = _lecturasPreparacion
        _lecturasPreparacion = Nothing
        Try
            'Estas lecturas siempre son frescas. Compararlas antes/despues de
            'calcular evita publicar datos bajo un sello cambiado en la llamada.
            Dim autoridad = ParaOperacion(contexto)
            Dim clasificacion = ClasificarNucleo(autoridad, snapshot.Configuracion)
            Dim campos = CamposNucleo(autoridad, snapshot.Configuracion)
            Return PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(New With {.publicacion = snapshot.Huella, .clasificacion = clasificacion, .campos = campos}))
        Finally
            _lecturasPreparacion = cache
        End Try
    End Function
End Class
