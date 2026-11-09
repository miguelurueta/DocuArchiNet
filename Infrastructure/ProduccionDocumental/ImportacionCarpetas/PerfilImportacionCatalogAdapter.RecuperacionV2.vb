Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Globalization

Public Partial Class PerfilImportacionCatalogAdapter
    Friend Function ComprobarAusenciaRecuperacionV2(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion) As Boolean
        If comando Is Nothing OrElse Not comando.EsVerificacion Then Throw Invalida()
        AutorizarGabineteImportacion(comando.Contexto, comando.Snapshot.Configuracion)
        Return LeerAusenciaRecuperacionV2(comando, datos, Function(origen, sql, parametros) Leer(If(origen = "RA", _ra, _da), sql, parametros))
    End Function
    Friend Shared Function LeerAusenciaRecuperacionV2(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion, consultar As Func(Of String, String, Object(), DataTable)) As Boolean
        If comando Is Nothing OrElse Not comando.EsVerificacion OrElse consultar Is Nothing Then Throw Invalida()
        comando.ValidarDatos(datos)
        Dim contexto = comando.Contexto, c = comando.Snapshot.Configuracion, identidad = comando.Raiz.IdentidadExpediente
        If Not MySqlImportacionCarpetasRepository.LeerClasificacionExpedienteConocido(contexto, identidad, c, Function(sql, parametros) consultar("RA", sql, parametros)) Then Return False
        'Conservador: cualquier documento del expediente impide cierre negativo.
        'No se atribuye ni confirma una identidad mediante nombres o coincidencias.
        For Each consultaSql As String In New String() {
            "SELECT COUNT(*) AS total FROM `" & datos.GabineteNombre & "` WHERE ID_EXPEDIENTE=@expediente",
            "SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=@expediente",
            "SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=@expediente"}
            Dim filas = consultar("DA", consultaSql, New Object() {"@expediente", identidad.ExpedienteId})
            If filas Is Nothing OrElse filas.Rows.Count <> 1 OrElse Not filas.Columns.Contains("total") OrElse filas.Rows(0).IsNull("total") Then Return False
            Dim total As ULong
            If Not ULong.TryParse(Convert.ToString(filas.Rows(0)("total"), CultureInfo.InvariantCulture), NumberStyles.None, CultureInfo.InvariantCulture, total) OrElse total <> 0 Then Return False
        Next
        Return True
    End Function
End Class
