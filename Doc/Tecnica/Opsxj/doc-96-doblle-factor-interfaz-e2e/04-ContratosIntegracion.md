# DOBLLE-FACTOR-INTERFAZ-E2E

- Ticket: DOC-96
- Cambio OpenSpec: doc-96-doblle-factor-interfaz-e2e
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

El cliente usa `POST` JSON de mismo origen contra `webservice/WebServiceLoginSegundoFactor.asmx`, conserva la Session existente y exige el sobre ASP.NET `{ d: ... }`.

| Método ASMX | Entrada cliente | Uso |
| --- | --- | --- |
| `ObtenerEstado()` | `{}` | Restaurar el pendiente en carga o refresh. |
| `Verificar(codigo As String)` | `{ "codigo": "<seis dígitos>" }` | Validar el OTP sin enviar identidad ni challenge. |
| `Reenviar()` | `{}` | Solicitar nuevo código respetando cooldown y límites del servidor. |
| `Cancelar()` | `{}` | Invalidar el pendiente de la Session y volver al login. |

No se aceptan desde JavaScript usuario, correo, empresa, módulo, challenge ni ruta de retorno. No se modificaron DTOs, métodos ASMX, autenticación, esquema de base de datos ni SMTP. Las respuestas HTTP, JSON o contractuales inválidas se convierten en códigos/mensajes públicos y fallan de forma cerrada.
