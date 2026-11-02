Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Data
Imports System.Linq

Public Partial Class PerfilImportacionCatalogAdapter
    Public Function PrepararDatosAlmacenamiento(comando As ComandoAlmacenamientoCarpetas) As DatosAlmacenamientoImportacion
        If comando Is Nothing Then Throw Invalida()
        Dim contexto = comando.Contexto, c = comando.Snapshot.Configuracion, autoridad = ParaOperacion(contexto)
        AutorizarGabineteImportacion(contexto, c)
        Dim valores = comando.Archivo.ValoresEfectivos
        Dim antes = PerfilImportacionJson.Escribir(valores)
        If ValidarValoresOperativos(contexto, c, valores).Count <> 0 OrElse PerfilImportacionJson.Escribir(valores) <> antes Then Throw Invalida()
        Dim gabinetes = ConsultarNucleo(autoridad, "GABINETE", c, 1, 2, c.GabineteId.Value.ToString(CultureInfo.InvariantCulture))
        Dim tipos = ConsultarNucleo(autoridad, "TIPOLOGIA", c, 1, 2, comando.TipologiaId)
        If gabinetes.Count <> 1 OrElse tipos.Count <> 1 OrElse gabinetes(0).value <> c.GabineteId.Value.ToString(CultureInfo.InvariantCulture) OrElse tipos(0).value <> comando.TipologiaId Then Throw Denegado()
        Dim nombre = gabinetes(0).text
        'Legacy resuelve gabinete y clase por nombre y toma la primera fila.
        'Exigir correspondencia unica con el ID fijado antes de invocar.
        Dim gabinete = FilaCreacion(Leer(_da, "SELECT id_gabinete AS id,NOMBRE AS nombre FROM system1 WHERE NOMBRE=@nombre LIMIT 2", "@nombre", nombre), c.GabineteId.Value.ToString(CultureInfo.InvariantCulture))
        If gabinete("nombre") <> nombre Then Throw Denegado()
        Dim clase = FilaCreacion(Leer(_ra, "SELECT ID_TIPO_DOCUMENTO AS id,DOCUMENTO AS nombre FROM ra_tipo_documento WHERE DOCUMENTO=@nombre LIMIT 2", "@nombre", ComandoAlmacenamientoCarpetas.ClaseDocumentoNombre), Nothing)
        If clase("nombre") <> ComandoAlmacenamientoCarpetas.ClaseDocumentoNombre Then Throw Invalida()
        'ESTADO_NORMAL es un codigo firmado de formato (DBT), no una clave
        'positiva. Conservar el valor del catalogo PDF existente.
        Dim extensiones = Leer(_da, "SELECT ESTADO_NORMAL AS id,ESTENSION AS nombre FROM DA_EXTENSION WHERE ESTENSION=@extension LIMIT 2", "@extension", ".PDF")
        If extensiones Is Nothing OrElse Not extensiones.Columns.Contains("id") OrElse Not extensiones.Columns.Contains("nombre") Then Throw Invalida()
        If extensiones.Rows.Count <> 1 Then Throw Denegado()
        If extensiones.Rows(0).IsNull("id") OrElse extensiones.Rows(0).IsNull("nombre") Then Throw Invalida()
        Dim formato = Convert.ToString(extensiones.Rows(0)("id"), CultureInfo.InvariantCulture)
        ComandoAlmacenamientoCarpetas.CodigoFormatoLegacy(formato)
        If Not String.Equals(Convert.ToString(extensiones.Rows(0)("nombre"), CultureInfo.InvariantCulture), ".PDF", StringComparison.Ordinal) Then Throw Invalida()
        Dim fisicos As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim campos = CamposNucleo(autoridad, c, fisicos)
        If fisicos.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count() <> fisicos.Count Then Throw Invalida()
        'NOMBRE es una columna automatica; no exige un indice visible del perfil.
        Dim columnaNombre = Leer(_da, "SELECT ic.DATA_TYPE,ic.CHARACTER_MAXIMUM_LENGTH,ic.CHARACTER_SET_NAME FROM system1 s INNER JOIN information_schema.COLUMNS ic ON ic.TABLE_SCHEMA=DATABASE() AND ic.TABLE_NAME=s.NOMBRE AND ic.COLUMN_NAME='NOMBRE' WHERE s.id_gabinete=@gabinete LIMIT 2", "@gabinete", c.GabineteId.Value)
        If columnaNombre.Rows.Count <> 1 OrElse columnaNombre.Rows(0).IsNull("CHARACTER_MAXIMUM_LENGTH") Then Throw Invalida()
        Dim nombreFisico = columnaNombre.Rows(0)
        If Not {"char", "varchar", "text", "tinytext", "mediumtext", "longtext"}.Contains(Convert.ToString(nombreFisico("DATA_TYPE"), CultureInfo.InvariantCulture), StringComparer.Ordinal) Then Throw Invalida()
        Dim capacidadNombre As New OpcionPerfilImportacion With {.Tipo = "TEXTO", .Requerida = True, .Longitud = CInt(Math.Min(Integer.MaxValue, Convert.ToInt64(nombreFisico("CHARACTER_MAXIMUM_LENGTH"), CultureInfo.InvariantCulture))), .Charset = Convert.ToString(nombreFisico("CHARACTER_SET_NAME"), CultureInfo.InvariantCulture)}
        If Not ValorCompatible(capacidadNombre, comando.Archivo.NombreOriginal) Then Throw Invalida()
        Dim datos As New DatosAlmacenamientoImportacion With {.GabineteNombre = nombre, .TipologiaNombre = tipos(0).text, .ClaseDocumentoId = clase("id"), .ClaseDocumentoNombre = clase("nombre"), .TipoArchivoId = formato}
        For Each valor In valores.Where(Function(v) v.DestinoTipo = "INDICE_GABINETE")
            If Not fisicos.ContainsKey(valor.DestinoRef) Then Throw Invalida()
            datos.Campos.Add(New CampoIndiceImportacionCarpetas With {.Referencia = valor.DestinoRef, .Nombre = fisicos(valor.DestinoRef), .Valor = valor.Valor})
        Next
        comando.ValidarDatos(datos)
        Return datos
    End Function

    'Solo SELECT por los IDs conocidos. No busca por nombre para atribuir un
    'efecto ni incorpora de nuevo lo que legacy ya haya registrado.
    Public Function ComprobarAlmacenamiento(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion, resultado As ResultadoEfectoImportacion) As Boolean
        If comando Is Nothing Then Throw Invalida()
        AutorizarGabineteImportacion(comando.Contexto, comando.Snapshot.Configuracion)
        Return LeerAlmacenamiento(comando, datos, resultado, Function(origen, sql, parametros) Leer(If(origen = "RA", _ra, _da), sql, parametros))
    End Function

    Friend Shared Function LeerAlmacenamiento(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion, resultado As ResultadoEfectoImportacion, consultar As Func(Of String, String, Object(), DataTable)) As Boolean
        If comando Is Nothing OrElse consultar Is Nothing OrElse resultado Is Nothing Then Throw Invalida()
        comando.ValidarDatos(datos)
        If Not ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId) Then Return False
        Dim contexto = comando.Contexto, c = comando.Snapshot.Configuracion, identidad = comando.Raiz.IdentidadExpediente
        If resultado.OrigenGabinete <> c.OrigenGabinete OrElse resultado.GabineteId <> c.GabineteId.Value.ToString(CultureInfo.InvariantCulture) OrElse PerfilImportacionJson.Escribir(resultado.Expediente) <> PerfilImportacionJson.Escribir(identidad) OrElse PerfilImportacionJson.Escribir(resultado.ValoresEfectivos) <> PerfilImportacionJson.Escribir(comando.Archivo.ValoresEfectivos) Then Return False
        If Not MySqlImportacionCarpetasRepository.LeerClasificacionExpedienteConocido(contexto, identidad, c, Function(sql, parametros) consultar("RA", sql, parametros)) Then Return False
        Dim parametrosSql As New List(Of Object) From {"@documento", resultado.DocumentoId, "@inventario", resultado.InventarioId, "@gabinete", c.GabineteId.Value}
        Dim campos = "g.ID AS documento,g.ID_INVENTARIO_DOCUMENTAL AS inventario,g.ID_EXPEDIENTE AS expediente,g.ID_USUARIO_GESTION AS usuario,g.NOMBRE AS nombre,s.id_gabinete AS gabinete,r.ID_DOCUMENTO_DOCUARCHI_ALMACEN AS documento_inventario,r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE AS expediente_inventario,r.ID_EMPRESA_DOCUMENTO AS empresa,r.ID_USUARIO_GESTION AS usuario_inventario,r.NOMBRE_GABINETE AS gabinete_nombre,r.ID_TIPO_DOCUMENTO AS tipologia,r.ID_CLASE_DOCUMENTO AS clase,r.SEGUNDO_NOMBRE_DOCUMENTO AS segundo_nombre,i.expediente_archivo_ID_EXPEDIENTE AS expediente_indice,i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL AS inventario_indice,i.segundo_nombre AS nombre_indice"
        campos &= ",g.DBT AS tipo_archivo,r.FORMATO AS formato_inventario,i.formato AS formato_indice"
        'El wrapper legacy solo materializa los campos VISIBLE=1. El nombre
        'original siempre se conserva aparte en inventario e indice.
        Dim configuracionNombre = " FROM detalle_gabienete dn WHERE dn.GABINETE=s.NOMBRE AND UPPER(dn.CAMPO)='NOMBRE'"
        campos &= ",(SELECT COUNT(*)" & configuracionNombre & ") AS nombre_configuraciones,(SELECT COALESCE(MAX(dn.VISIBLE),-1)" & configuracionNombre & ") AS nombre_visible"
        For indice = 0 To datos.Campos.Count - 1
            'CAST conserva DATE como yyyy-MM-dd sin conversion cultural .NET.
            campos &= ",CAST(g.`" & datos.Campos(indice).Nombre & "` AS CHAR) AS valor_" & indice.ToString(CultureInfo.InvariantCulture)
        Next
        Dim sqlDocumento = "SELECT " & campos & " FROM `" & datos.GabineteNombre & "` g INNER JOIN system1 s ON s.id_gabinete=@gabinete AND s.NOMBRE=@nombreGabinete INNER JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL INNER JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE g.ID=@documento AND g.ID_INVENTARIO_DOCUMENTAL=@inventario LIMIT 2"
        parametrosSql.AddRange(New Object() {"@nombreGabinete", datos.GabineteNombre})
        Dim filas = consultar("DA", sqlDocumento, parametrosSql.ToArray())
        If filas Is Nothing OrElse filas.Rows.Count <> 1 Then Return False
        For Each columna In New String() {"nombre", "nombre_configuraciones", "nombre_visible"}
            If Not filas.Columns.Contains(columna) Then Throw Invalida()
        Next
        Dim configuracionesNombre = Convert.ToString(filas.Rows(0)("nombre_configuraciones"), CultureInfo.InvariantCulture)
        Dim nombreVisible = Convert.ToString(filas.Rows(0)("nombre_visible"), CultureInfo.InvariantCulture)
        If configuracionesNombre = "0" AndAlso nombreVisible = "-1" Then nombreVisible = "0"
        If Not {"0", "1"}.Contains(configuracionesNombre) OrElse Not {"0", "1"}.Contains(nombreVisible) OrElse (configuracionesNombre = "0" AndAlso nombreVisible <> "0") Then Return False
        Dim nombreFisico = If(filas.Rows(0).IsNull("nombre"), "", Convert.ToString(filas.Rows(0)("nombre"), CultureInfo.InvariantCulture))
        Dim esperados As New Dictionary(Of String, String)(StringComparer.Ordinal) From {{"documento", resultado.DocumentoId}, {"inventario", resultado.InventarioId}, {"expediente", identidad.ExpedienteId}, {"usuario", contexto.UsuarioRef}, {"nombre", comando.Archivo.NombreOriginal}, {"gabinete", c.GabineteId.Value.ToString(CultureInfo.InvariantCulture)}, {"documento_inventario", resultado.DocumentoId}, {"expediente_inventario", identidad.ExpedienteId}, {"empresa", contexto.AmbitoRef}, {"usuario_inventario", contexto.UsuarioRef}, {"gabinete_nombre", datos.GabineteNombre}, {"tipologia", comando.TipologiaId}, {"clase", datos.ClaseDocumentoId}, {"segundo_nombre", comando.Archivo.NombreOriginal}, {"expediente_indice", identidad.ExpedienteId}, {"inventario_indice", resultado.InventarioId}, {"nombre_indice", comando.Archivo.NombreOriginal}}
        'Solo admitir vacio/NULL cuando el catalogo no lo incluye como visible.
        'Un nombre diferente no vacio sigue siendo discrepancia.
        If nombreVisible = "0" AndAlso nombreFisico = "" Then esperados.Remove("nombre")
        For indice = 0 To datos.Campos.Count - 1
            esperados.Add("valor_" & indice.ToString(CultureInfo.InvariantCulture), datos.Campos(indice).Valor)
        Next
        esperados.Add("tipo_archivo", datos.TipoArchivoId)
        esperados.Add("formato_inventario", ".PDF") : esperados.Add("formato_indice", ".PDF")
        For Each par In esperados
            If Not filas.Columns.Contains(par.Key) Then Throw Invalida()
            If par.Value Is Nothing Then
                If Not filas.Rows(0).IsNull(par.Key) Then Return False
            Else
                If filas.Rows(0).IsNull(par.Key) Then Return False
                Dim actual = Convert.ToString(filas.Rows(0)(par.Key), CultureInfo.InvariantCulture)
                If par.Key.StartsWith("valor_", StringComparison.Ordinal) Then
                    Dim campo = datos.Campos(Integer.Parse(par.Key.Substring(6), CultureInfo.InvariantCulture))
                    Dim valor = comando.Archivo.ValoresEfectivos.Single(Function(v) v.DestinoTipo = "INDICE_GABINETE" AndAlso v.DestinoRef = campo.Referencia)
                    If valor.TipoValor = "DECIMAL" Then
                        'DECIMAL puede devolver escala fisica adicional (1.20).
                        'No normalizar texto/referencias: conservar sus ceros.
                        Dim numero As Decimal, esperado As Decimal
                        Dim estilo = NumberStyles.AllowLeadingSign Or NumberStyles.AllowDecimalPoint
                        If Not Decimal.TryParse(actual, estilo, CultureInfo.InvariantCulture, numero) OrElse Not Decimal.TryParse(par.Value, estilo, CultureInfo.InvariantCulture, esperado) OrElse numero <> esperado Then Return False
                        Continue For
                    End If
                End If
                If Not String.Equals(actual, par.Value, StringComparison.Ordinal) Then Return False
            End If
        Next
        Return True
    End Function
End Class
