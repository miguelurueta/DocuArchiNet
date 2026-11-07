Option Strict On
'Identidades persistidas: INT UNSIGNED AUTO_INCREMENT en RA; transporte decimal
'canónico String (1..4294967295), sin GUID ni ceros iniciales. Referencias opcionales Nothing.
Imports System.Collections.Generic

Public Class RespuestaPerfilImportacion
    Public Property ok As Boolean
    Public Property codigo As String
    Public Property mensaje As String
    Public Property operacionId As String
    Public Property datos As Object
    Public Property incidencias As New List(Of IncidenciaPerfilImportacion)()
End Class
Public Class SobreSolicitudPerfil(Of T)
    Public Property solicitud As T
End Class
Public Class ListarPerfilesSolicitud
    Public Property termino As String
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 25
    Public Property codigo As String
    Public Property nombre As String
    Public Property activo As Nullable(Of Boolean)
End Class
Public Class ObtenerPerfilSolicitud
    Public Property perfilId As String
    Public Property versionId As String
End Class
Public Class HistorialPerfilSolicitud
    Public Property perfilId As String
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 25
End Class
Public Class CatalogoPerfilSolicitud
    Public Property catalogo As String
    Public Property referencia As String
    Public Property configuracion As New ConfiguracionPerfilImportacion()
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 25
End Class
Public Class CatalogoProveedoresSolicitud
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 25
    Public Property proveedorId As String
    Public Property versionId As String
End Class
Public Class MetadatosProveedorSolicitud
    Public Property tipo As String
    Public Property conexionRef As String
    Public Property tabla As String
    Public Property pagina As Integer = 1
End Class
Public Class CrearPerfilSolicitud
    Public Property codigo As String
    Public Property nombre As String
    Public Property descripcion As String
End Class
Public Class GuardarPerfilSolicitud
    Public Property perfilId As String
    Public Property versionId As String
    Public Property revisionVersionEsperada As UInteger
    Public Property configuracion As ConfiguracionPerfilImportacion
    Public Property reglas As List(Of ReglaPerfilImportacion)
End Class
Public Class EnsayarPerfilSolicitud
    Inherits GuardarPerfilSolicitud
    Public Property ejemplos As List(Of EjemploPerfilImportacion)
End Class
Public Class PublicarPerfilSolicitud
    Inherits ObtenerPerfilSolicitud
    Public Property revisionPerfilEsperada As UInteger
    Public Property revisionVersionEsperada As UInteger
End Class
Public Class CrearVersionPerfilSolicitud
    Public Property perfilId As String
    Public Property versionOrigenId As String
    Public Property revisionPerfilEsperada As UInteger
    Public Property revisionOrigenEsperada As UInteger
End Class
Public Class CabeceraPerfilSolicitud
    Public Property perfilId As String
    Public Property revisionPerfilEsperada As UInteger
    Public Property nombre As String
    Public Property descripcion As String
End Class
Public Class DisponibilidadPerfilSolicitud
    Public Property perfilId As String
    Public Property revisionPerfilEsperada As UInteger
    Public Property activo As Boolean
End Class
Public Class SeleccionPerfilSolicitud
    Inherits ObtenerPerfilSolicitud
    Public Property nivelRef As String
    Public Property proposito As String
End Class
