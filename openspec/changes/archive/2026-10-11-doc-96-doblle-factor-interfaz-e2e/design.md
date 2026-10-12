<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13 -->
# Diseño técnico — DOC-96 Interfaz WebForms y E2E 2FA

## Contexto

DOC-95 dejó operativo el segundo factor hasta la frontera ASMX. `gestor.aspx.vb:Button1_Click` recibe `SECOND_FACTOR_REQUIRED`, pero hoy lo presenta como alerta. DOC-96 convierte solo ese resultado en transición visual a OTP y mantiene el login legacy para cualquier otro resultado.

## Inventario y límites

| Área | Archivo/símbolo | Responsabilidad DOC-96 |
| --- | --- | --- |
| Página | `gestor.aspx` | Host accesible del panel OTP y referencias JS/CSS. |
| Postback | `gestor.aspx.vb:Button1_Click` | Reconocer exactamente el pendiente y preservar demás caminos. |
| Designer | `gestor.aspx.designer.vb` | Sin cambio previsto; modificar solo si Web Forms exige un control servidor demostrado. |
| Cliente | `js/login/segundo-factor.js` | ASMX, estados, tiempo visual, accesibilidad y navegación segura. |
| Estilo | `Styles/login/segundo-factor.css` | Estilos aislados, responsive, foco y disabled. |
| Contrato existente | `WebServiceLoginSegundoFactor.asmx.vb`, `SegundoFactorDtos.vb` | Solo consumo; sin cambio autorizado. |
| Proyecto | `GestionDocumental-Docuarchi.net.vbproj` | Registrar los dos recursos como `Content`. |
| Pruebas | `tests/`, `tools/e2e/` | Política, cliente y recorrido UI real. |
| Documentación | `Doc/Actualizacion/Login/Implementacion/DOC-96/` | Arquitectura, matriz, comandos y evidencia saneada. |

Fuera de alcance: SQL, tablas, migraciones, SMTP, `ClassCorreo`, permisos, flags, recuperación de contraseña, otro ASMX/login, SPA o cambios backend DOC-95.

## Decisiones

### D-01 — Bootstrap desde el postback existente

`Button1_Click` conserva la llamada actual. Ante `SECOND_FACTOR_REQUIRED`, limpia `TextBoxpasw.Text` y registra un script estático que invoca el módulo 2FA después del update parcial. No inserta usuario, correo, challenge u OTP. Otros errores conservan su tratamiento; `YES` conserva la redirección backend.

### D-02 — Panel accesible en la página

El panel usa `hidden`, título, instrucción, label, input de texto con `inputmode="numeric"`, `autocomplete="one-time-code"`, `maxlength="6"`, estado `aria-live="polite"`, error `role="alert"` y botones reales validar/reenviar/cancelar. Al abrir enfoca OTP; al cerrar devuelve foco al usuario.

### D-03 — Módulo cliente aislado

`segundo-factor.js` será un IIFE/fábrica testeable con operaciones públicas mínimas de inicio/restauración. No modifica la responsabilidad de `js/workflow/gestor.js` ni recibe identidad.

### D-04 — Adaptador ASMX estricto

Usa `POST` JSON a `webservice/WebServiceLoginSegundoFactor.asmx/{operacion}`, mismo origen y Session. Valida `{ d: ... }`. `Verificar` envía solo `{ codigo }`; las otras operaciones `{}`. Fallos HTTP/JSON/contrato se vuelven mensajes genéricos.

### D-05 — Estado y tiempo de servidor

Estados cliente: `LOGIN`, `CONSULTANDO`, `PENDIENTE`, `SOLICITANDO`, `COMPLETADO`. Solo hay una mutación en vuelo. Los segundos del DTO fijan marcas monotónicas para representación; llegar a cero nunca autoriza, sino que dispara consulta de estado.

### D-06 — Restauración por Session

En carga inicial y tras postback parcial llama `ObtenerEstado`. Solo `RequiereCodigo=true` y `PENDIENTE` muestran OTP. `NO_DISPONIBLE` conserva/restaura login sin alerta espuria.

### D-07 — Navegación local fail-closed

Solo `Exito=true`, código `SECOND_FACTOR_COMPLETED` y estado `COMPLETADO` navegan. `RutaLocal` debe iniciar `~/` y no contener `..`, `\\`, `:`, `?` o `#`; se resuelve contra la raíz de aplicación. Cualquier desviación falla sin navegar.

### D-08 — Respuestas recuperables y terminales

- Recuperables: `SECOND_FACTOR_CODE_INVALID`, `SECOND_FACTOR_RESEND_COOLDOWN`, `SECOND_FACTOR_PENDING`, `SECOND_FACTOR_RESENT`.
- Terminales: límite, sesión inválida, bloqueo, cancelación, fallos de entrega/inicio/verificación/reenvío/finalización, respuesta inválida o transporte.
- Los terminales limpian OTP/contraseña, intervalos y estado, reactivan credenciales y no autentican.

### D-09 — Minimización de datos

Solo se guarda en memoria DTO público, banderas y marcas temporales. No se usa storage, cookies propias, query string o logs. El OTP se limpia después de respuesta y el destino proviene enmascarado.

### D-10 — CSS y proyecto

CSS con clases `da-login-2fa-*`, regla `hidden`, contraste, `:focus-visible`, disabled y móvil. Los dos assets se incluyen y registran una vez; no hay nueva dependencia.

### D-11 — Pruebas focales

La fábrica inyecta transporte, reloj, navegación y vista para pruebas Node sin jsdom. Se verifican payload, sobre ASMX, transiciones, exclusión mutua, cooldown, ruta, ausencia de storage/logs, markup y `.vbproj`.

### D-12 — E2E UI real

El helper ofrece una variante que conserva `{ context, page, postbackResponse }` sin romper su API actual. Un runner DOC-96 reutiliza variables, perfil, validadores y autorización. Prueba mínimo módulo OFF y ON y documenta cuatro módulos. No usa `.env`, secretos embebidos ni sleeps largos. El diagnóstico ordinario usa solo `SELECT`; dado que las cuentas disponibles de módulos OFF no completaron el login, un controlador excepcional y expresamente autorizado limita la escritura temporal a `GESTOR.RequiereSegundoFactor`, exige snapshot ON válido, restaura en `finally` y comprueba después ON con `SELECT` independiente.

### D-13 — Backend congelado

El contrato actual satisface la UI. Si una prueba descubre incompatibilidad, se documenta y se refina OpenSpec antes de cambiar DTO/ASMX/backend.

## Flujo técnico

```text
[Usuario]
   | módulo + usuario + contraseña (postback)
   v
[gestor.aspx / Button1_Click]
   v
[ClassGestorSesion.InicioAplicacionWebGestorDocumental]
   |-- YES -----------------------------> [flujo legacy]
   |-- error público -------------------> [mensaje; no OTP]
   `-- SECOND_FACTOR_REQUIRED
          v
 [segundo-factor.js.iniciar]
          v
 [ASMX.ObtenerEstado()] ---> [Session pendiente]
          |-- NO_DISPONIBLE -----------> [restaurar login]
          `-- PENDIENTE ---------------> [panel OTP]
                                               |
                    +--------------------------+------------------+
                    |                          |                  |
             Verificar(codigo)             Reenviar()        Cancelar()
                    |                          |                  |
          PENDIENTE o COMPLETADO         estado servidor     restaurar login
                    |
          mensaje o RutaLocal validada
```

## Riesgos y mitigaciones

- `UpdatePanel`: bootstrap idempotente y restauración con `PageRequestManager.endRequest`.
- Doble click: exclusión mutua cliente más garantía concurrente DOC-95.
- Reloj distinto: segundos relativos; no fechas del equipo.
- Open redirect: formato `~/` y segmentos estrictos.
- Regresión OFF: rama por código exacto y E2E legacy.
- Configuración temporal OFF: precondición ON, `UPDATE` acotado/guardado, restauración obligatoria y diagnóstico posterior independiente.
- Fuga: códigos de etapa y evidencia saneada.

## Rollout y rollback

No requiere datos. Se publican markup, JS, CSS y ensamblado. Rollback: retirar bootstrap/panel y assets DOC-96; DOC-95 y el login OFF permanecen operativos.
