Option Strict On
Imports System

'Composición interna del mismo repositorio. RV-04 debe cumplir estos contratos:
'inicio/gate/retorno y diario atómicos; acuse solo tras commit confirmado.
'No es un repositorio alternativo ni permite callbacks opcionales/fallback V1.
Friend NotInheritable Class PersistenciaDocumentoV2Importacion
    Friend ReadOnly Iniciar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion)
    Friend ReadOnly AutorizarInvocacion As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion)
    Friend ReadOnly ObservarRetorno As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion)
    Friend ReadOnly RegistrarResultado As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion)
    Friend Sub New(inicio As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion), autorizacion As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion), retorno As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, AcuseDocumentoV2Importacion), resultado As Func(Of ComandoAlmacenamientoCarpetas, DocumentoAlmacenamientoV2Importacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion))
        If inicio Is Nothing OrElse autorizacion Is Nothing OrElse retorno Is Nothing OrElse resultado Is Nothing Then Throw New ArgumentNullException()
        Iniciar = inicio : AutorizarInvocacion = autorizacion : ObservarRetorno = retorno : RegistrarResultado = resultado
    End Sub
End Class

'Acuse efímero del puerto interno, nunca deserializado desde HTTP o JSON.
'El tipo no demuestra por sí solo un commit: lo garantiza el repositorio RV-04.
<Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)>
Friend NotInheritable Class AcuseDocumentoV2Importacion
    Private ReadOnly _documento As String
    Friend Sub New(documento As DocumentoAlmacenamientoV2Importacion)
        _documento = DocumentosAlmacenamientoV2Importacion.Escribir(documento)
    End Sub
    Friend ReadOnly Property Documento As DocumentoAlmacenamientoV2Importacion
        Get
            Return DocumentosAlmacenamientoV2Importacion.Leer(_documento)
        End Get
    End Property
End Class
