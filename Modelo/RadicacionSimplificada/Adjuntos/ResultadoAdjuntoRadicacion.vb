Imports System

Public NotInheritable Class ResultadoAdjuntoRadicacion
    Public Property Exito As Boolean
    Public Property Mensaje As String
    Public Property Contexto As ContextoAdjuntoRadicacion
    Public Property IdImagen As Integer
    Public Property DatosImagen As stru_datos_image_lista

    Public Shared Function Fallo(ByVal mensaje As String,
                                 Optional ByVal contexto As ContextoAdjuntoRadicacion = Nothing) As ResultadoAdjuntoRadicacion
        Return New ResultadoAdjuntoRadicacion With {
            .Exito = False,
            .Mensaje = If(mensaje, String.Empty),
            .Contexto = contexto
        }
    End Function

    Public Shared Function Correcto(ByVal contexto As ContextoAdjuntoRadicacion,
                                    ByVal idImagen As Integer,
                                    ByVal datosImagen As stru_datos_image_lista) As ResultadoAdjuntoRadicacion
        Return New ResultadoAdjuntoRadicacion With {
            .Exito = True,
            .Mensaje = "YES",
            .Contexto = contexto,
            .IdImagen = idImagen,
            .DatosImagen = datosImagen
        }
    End Function
End Class
