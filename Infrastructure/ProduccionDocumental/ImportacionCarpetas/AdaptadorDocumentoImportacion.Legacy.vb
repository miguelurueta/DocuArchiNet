Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Web

Public Partial Class AdaptadorDocumentoImportacion
    Public Sub New(http As HttpContext, staging As IAlmacenStagingImportacion)
        If http Is Nothing OrElse http.Session Is Nothing OrElse staging Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
        _staging = staging
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
        Dim catalogos As New PerfilImportacionCatalogAdapter(ra, da)
        _autorizar = Sub(comando)
                         If Not Object.ReferenceEquals(http, HttpContext.Current) OrElse ModuleSessionConnectionStringResolver.Resolve(http, "RA_") <> ra OrElse ModuleSessionConnectionStringResolver.Resolve(http, "DA_") <> da OrElse String.IsNullOrWhiteSpace(Convert.ToString(http.Session("DA_Login_Usuario"), CultureInfo.InvariantCulture)) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental ha cambiado.")
                         ImportacionCarpetasContextAdapter.ValidarConexionCreacionLegacy(http)
                         ImportacionCarpetasContextAdapter.ValidarConexionCreacionLegacy(http, "DA_")
                         autoridad.AutorizarRecursos(http, comando.Contexto, comando.Snapshot.Configuracion, "ALMACENAR_DOCUMENTO")
                         If Not repositorio.ComprobarClasificacionExpedienteConocido(comando.Contexto, comando.Raiz.IdentidadExpediente, comando.Snapshot.Configuracion) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "La relacion del expediente ha cambiado.")
                     End Sub
        _datos = AddressOf catalogos.PrepararDatosAlmacenamiento
        _iniciar = AddressOf repositorio.IniciarAlmacenamientoDocumento
        _invocar = AddressOf InvocarAlmacenamiento
        _verificar = AddressOf catalogos.ComprobarAlmacenamiento
        _registrar = AddressOf repositorio.RegistrarResultadoDocumento
    End Sub
    Private Shared Sub InvocarAlmacenamiento(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion, copia As String, salida As SalidaAlmacenamientoLegacy)
        If comando.EsVerificacion Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Un comando de verificacion no puede ejecutar almacenamiento.")
        comando.ValidarDatos(datos)
        Dim campos = datos.Campos.Where(Function(c) Not ComandoAlmacenamientoCarpetas.EsCampoAutomatico(c.Nombre)).Select(Function(c) New CDcamposAsignaAlmacenamiento With {.NombreCampoGabinete = c.Nombre, .ValorCampoGabinete = c.Valor}).ToList()
        Dim documentoId As Integer = 0, estructura As stru_datos_image_lista = Nothing
        Try
            salida.Respuesta = New ClassAlmacenamiento().AlmacenamientoDocumentoProduccionDocumental(
                NombreGabinete:=datos.GabineteNombre, RutaArchivoAlmacenar:=copia, NombreDocumento:=comando.Archivo.NombreOriginal,
                IdExpediente:=Integer.Parse(comando.Raiz.IdentidadExpediente.ExpedienteId, CultureInfo.InvariantCulture),
                DescripcionTipo:=datos.TipologiaNombre, IdTipoLogia:=Integer.Parse(comando.TipologiaId, CultureInfo.InvariantCulture),
                TipoAlmacenamiento:=0, CDcamposAsignaAlmacenamiento:=campos, ValorObjeto:="", NombreCaso:="",
                NombreClaseFormatoDocumento:=datos.ClaseDocumentoNombre, IdImagenAlamacenada:=documentoId, EstructuraDatosImagen:=estructura)
        Finally
            salida.DocumentoId = documentoId : salida.InventarioId = estructura.id_registro
            salida.NombreArchivo = estructura.nombre_archivo : salida.GabineteNombre = estructura.nombre_gabinete
        End Try
    End Sub
End Class
