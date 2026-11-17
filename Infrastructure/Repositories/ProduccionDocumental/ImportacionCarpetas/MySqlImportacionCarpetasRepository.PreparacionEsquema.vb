Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports MySql.Data.MySqlClient

'Capacidad aditiva independiente: las consultas/esquema1 no exigen estas tablas.
Public Partial Class MySqlImportacionCarpetasRepository
    Private Const TablasPreparacion As String = "'ra_integracion_preparacion','ra_integracion_preparacion_parte','ra_integracion_preparacion_resultado'"

    Public Sub VerificarEsquemaPreparacion() Implements IPreparacionSegmentadaImportacionRepository.VerificarEsquemaPreparacion
        Try
            Using conexion As New MySqlConnection(_conexion)
                conexion.Open()
                ValidarMetadatos(LeerMetadatos(conexion))
                ValidarMetadatosPreparacion(LeerMetadatos(conexion, TablasPreparacion, TablasPreparacion))
            End Using
        Catch ex As MySqlException
            Throw NoDisponible()
        Catch ex As ArgumentException
            Throw NoDisponible()
        Catch ex As InvalidOperationException
            Throw NoDisponible()
        End Try
    End Sub

    Friend Shared Sub ValidarMetadatosPreparacion(datos As DataSet)
        ValidarContratoMetadatos(datos, ColumnasPreparacion, IndicesPreparacion, ClavesPreparacion)
        Dim unicos = New HashSet(Of String)(IndicesPreparacion.Where(Function(i) i.Split("|"c)(2) = "0").Select(Function(i) i.Split("|"c)(0) & ":" & i.Split("|"c)(1)), StringComparer.Ordinal)
        If Filas(datos, "Indices").Where(Function(r) ColumnasPreparacion.ContainsKey(Texto(r, "TABLE_NAME"))).Any(Function(r) Texto(r, "NON_UNIQUE") = "0" AndAlso Not unicos.Contains(Texto(r, "TABLE_NAME") & ":" & Texto(r, "INDEX_NAME"))) Then Throw NoDisponible()
        Dim claves = New HashSet(Of String)(ClavesPreparacion.Select(Function(i) i.Split("|"c)(0) & ":" & i.Split("|"c)(1)), StringComparer.Ordinal)
        If Filas(datos, "Claves").Any(Function(r) Not claves.Contains(Texto(r, "TABLE_NAME") & ":" & Texto(r, "CONSTRAINT_NAME"))) Then Throw NoDisponible()
    End Sub

    Private Shared ReadOnly ColumnasPreparacion As New Dictionary(Of String, String())(StringComparer.Ordinal) From {
        {"ra_integracion_preparacion", {
            "preparacion_id|int unsigned|||NO|auto_increment|", "ambito_ref|varchar|128|ascii_bin|NO||",
            "usuario_ref|varchar|128|ascii_bin|NO||", "nivel_ref|varchar|128|ascii_bin|NO||",
            "version_id|int unsigned|||NO||", "clave|varchar|32|ascii_bin|NO||", "cabecera_hash|binary|32||NO||",
            "contexto_doc|longtext|4294967295|ascii_bin|NO||", "config_doc|longtext|4294967295|ascii_bin|NO||",
            "total_raices|int unsigned|||NO||", "total_archivos|int unsigned|||NO||", "total_bytes|bigint unsigned|||NO||",
            "partes_raices|int unsigned|||NO||", "partes_archivos|int unsigned|||NO||",
            "estado|varchar|40|ascii_bin|NO||", "revision|int unsigned|||NO||", "plan_id|int unsigned|||YES||",
            "creado_utc|datetime|||NO||", "expira_utc|datetime|||NO||"}},
        {"ra_integracion_preparacion_parte", {
            "preparacion_id|int unsigned|||NO||", "tipo|varchar|16|ascii_bin|NO||", "numero|int unsigned|||NO||",
            "contenido_doc|longtext|4294967295|ascii_bin|NO||", "contenido_hash|binary|32||NO||",
            "estado|varchar|40|ascii_bin|NO||", "sello_catalogos|varchar|64|ascii_bin|YES||"}},
        {"ra_integracion_preparacion_resultado", {
            "preparacion_id|int unsigned|||NO||", "tipo|varchar|24|ascii_bin|NO||", "numero|int unsigned|||NO||",
            "contenido_doc|longtext|4294967295|ascii_bin|NO||", "contenido_hash|binary|32||NO||"}}
    }
    Private Shared ReadOnly IndicesPreparacion As String() = {
        "ra_integracion_preparacion|PRIMARY|0|preparacion_id",
        "ra_integracion_preparacion|uq_preparacion_clave|0|ambito_ref,usuario_ref,clave",
        "ra_integracion_preparacion|uq_preparacion_plan|0|plan_id",
        "ra_integracion_preparacion|ix_preparacion_contexto|1|ambito_ref,usuario_ref,creado_utc",
        "ra_integracion_preparacion|ix_preparacion_estado|1|estado,expira_utc",
        "ra_integracion_preparacion|ix_preparacion_version|1|version_id",
        "ra_integracion_preparacion_parte|PRIMARY|0|preparacion_id,tipo,numero",
        "ra_integracion_preparacion_parte|ix_preparacion_parte_estado|1|preparacion_id,estado",
        "ra_integracion_preparacion_resultado|PRIMARY|0|preparacion_id,tipo,numero"}
    Private Shared ReadOnly ClavesPreparacion As String() = {
        "ra_integracion_preparacion|fk_preparacion_version|version_id|ra_integracion_perfil_version|version_id",
        "ra_integracion_preparacion|fk_preparacion_plan|plan_id|ra_integracion_plan|plan_id",
        "ra_integracion_preparacion_parte|fk_preparacion_parte|preparacion_id|ra_integracion_preparacion|preparacion_id",
        "ra_integracion_preparacion_resultado|fk_preparacion_resultado|preparacion_id|ra_integracion_preparacion|preparacion_id"}
End Class
