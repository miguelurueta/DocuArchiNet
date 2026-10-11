# DOBLE-FACTOR-ORQUESTACION

- Ticket: DOC-95
- Cambio OpenSpec: doc-95-doble-factor-orquestacion
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

`WebServiceLoginSegundoFactor.asmx` publica cuatro métodos `POST` JSON con `EnableSession:=True`: `ObtenerEstado() As SegundoFactorEstadoDto`, `Verificar(codigo As String) As SegundoFactorResultadoDto`, `Reenviar() As SegundoFactorResultadoDto` y `Cancelar() As SegundoFactorResultadoDto`. El cliente aporta únicamente el código en la verificación; `LoginSecondFactorSessionGate` obtiene y valida el contexto pendiente desde `HttpContext.Current.Session`.

`SegundoFactorEstadoDto` contiene `RequiereCodigo`, `EstadoPublico`, `DestinoEnmascarado`, `ExpiraEnSegundos` y `ReenvioDisponibleEnSegundos`. `SegundoFactorResultadoDto` agrega `Exito`, `Codigo`, `MensajeVisible`, `Estado` y `RutaLocal`. La ruta se resuelve del lado servidor y se restringe a la ruta local configurada.

La integración persiste en `ra_auth_second_factor_challenge`, resuelve configuración por `gestor_modulos` y obtiene SMTP desde la fila activa de `Config_Smpt_Side` (`ESTADO_ENVIO = 1`) usando la conexión legacy. No se introduce DDL. Los contratos se registran una sola vez en `GestionDocumental-Docuarchi.net.vbproj` y no modifican `ClassCorreo`, recuperación de contraseña ni otros ASMX.
