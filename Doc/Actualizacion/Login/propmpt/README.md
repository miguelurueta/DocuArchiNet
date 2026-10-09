# Prompts Jira/OpenSpec: Login con doble factor

Fuente arquitectónica cerrada: `../Exploracion/exploracion-doble-factor-autenticacion.md`.

No existe un prompt `00`: la exploración ya cumple esa función y no representa una tarea Jira. Tampoco existe una tarea para “convertir la exploración en especificación”. Cada prompt siguiente se monta como una tarea Jira independiente y, al ejecutarse, crea o continúa su propio OpenSpec Spec-Driven con propuesta, diseño, delta spec y tareas atómicas.

| Orden | Prompt | Resultado implementable | Verificación dentro de la tarea |
|---|---|---|---|
| 01 | `01-fundacion-contratos-seguridad.md` | Contratos, OTP/HMAC, reloj y contexto pendiente de Session. | Unitarias/contratos, secretos y build. |
| 02 | `02-persistencia-challenge.md` | DDL compatible y repositorio transaccional de challenges. | Migración, concurrencia e integración descartable autorizada. |
| 03 | `03-infraestructura-smtp-2fa.md` | Lector de `Config_Smpt_Side` y transporte SMTP exclusivo de 2FA. | Dobles sin red y envío real solo si se autoriza. |
| 04 | `04-preautenticacion-finalizador-modulos.md` | Separación del login legacy y adaptadores de los cuatro módulos. | Caracterización por módulo con 2FA apagado/encendido simulado. |
| 05 | `05-orquestacion-asmx.md` | Ciclo de challenge y ASMX de Login sin UI. | Contratos HTTP/locales y prueba integrada autorizada antes de UI. |
| 06 | `06-ui-webforms-e2e.md` | Pantalla OTP integrada en `gestor.aspx` y recorrido de usuario. | E2E en la misma unidad de entrega, sujeto a autorización vigente. |
| 07 | `07-seguridad-regresion-transversal.md` | Cierre de matriz de seguridad/no regresión en cuatro módulos. | Automatización CI, concurrencia y E2E transversal autorizado. |
| 08 | `08-despliegue-activacion-rollback.md` | Runbook, migración, llaves, activación gradual y rollback. | Ensayo no productivo y puertas verificables; no activa producción. |

Reglas comunes:

- Ejecutar en orden; una tarea no asume como correcto un artefacto previo sin inspeccionarlo.
- Corregir dentro de la misma tarea cualquier defecto que esta introduzca y repetir sus pruebas y las regresiones afectadas.
- No modificar recuperación de contraseña, `radicador/ClassCorreo.vb`, sus llamadores ni el comportamiento legacy cuando `RequiereSegundoFactor` sea `0` o `NULL`.
- Usar las abstracciones ADO.NET, MSBuild, validadores, pruebas CJS, CI y `tools/e2e` existentes. No crear otra aplicación, otro arnés Playwright ni otro stack de datos.
- Cada tarea documenta en su propio OpenSpec los `D-XX`, `RQ-XX`, tareas, pruebas, resultados y limitaciones; la evidencia adicional se guarda en `Doc/Actualizacion/Login/Implementacion/<JIRA>/` para evitar que tareas diferentes sobrescriban un paquete común.
- Antes de editar, cada tarea debe inventariar en `design.md` los archivos exactos que creará o modificará. Debe usar las rutas vinculantes de la sección 13.1 de la exploración: `Modelo/Login/SegundoFactor/`, `DTOs/Login/SegundoFactor/`, `Services/Login/SegundoFactor/`, `Infrastructure/Repositories/Login/SegundoFactor/`, `Infrastructure/Login/SegundoFactor/`, `webservice/Login/SegundoFactor/` y el ASMX dedicado en `webservice/`.
- Repositorios no pueden ubicarse en `Services`; DTOs no pueden ubicarse en `Modelo`; Infrastructure no puede leer Session/`HttpContext`; el ASMX no puede contener SQL ni reglas de negocio. Una desviación exige evidencia del árbol vigente y una decisión explícita del OpenSpec.
- Todo `.vb`, `.asmx` o recurso nuevo debe registrarse explícitamente en `GestionDocumental-Docuarchi.net.vbproj` reproduciendo el tipo de elemento de un archivo equivalente.
- No consultar bases reales, migrar, autenticar, enviar correo ni ejecutar E2E sin autorización explícita vigente para ambiente, cuentas y datos descartables. No imprimir ni guardar secretos.
- Registrar comandos y códigos de salida reales. Un bloqueo se informa; no se sustituye con mocks engañosos ni con una afirmación de éxito.

El E2E no es una tarea tardía aislada: el prompt 05 prueba la frontera HTTP, el 06 prueba el recorrido incorporado y el 07 repite la matriz transversal cuando exista autorización.
