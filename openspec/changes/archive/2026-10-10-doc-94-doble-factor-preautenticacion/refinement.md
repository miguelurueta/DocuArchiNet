<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-94-doble-factor-preautenticacion

## Fuente, alcance y evidencia inspeccionada

- Ticket: `DOC-94` — DOBLE-FACTOR-PREAUTENTICACION.
- Alcance: separar validación de credenciales y finalización del login WebForms para DocuArchi, Gestor, Radicación y Workflow. No incluye OTP, correo, challenge, ASMX ni interfaz.
- Entrada: `gestor.aspx.vb::Button1_Click` llama `ClassGestorSesion.InicioAplicacionWebGestorDocumental(String,String,String,String)`.
- Núcleo legacy: `Defaul/ClassGestorSesion.vb::InicioAplicacionWebGestorDocumental` prepara conexiones, obtiene el tipo de módulo, llama `ValidaUserAplicacion(String, ByRef String, String, ByRef Integer)` y ejecuta luego sesiones, relaciones, permisos, auditoría y redirect.
- Infraestructura: `Defaul/GestorModuleSesion.vb::InicializaconexionesModulos(String,String)`, `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` y `Domain/Shared/ContextoModulo.vb`.
- Contratos previos: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` y `SegundoFactorInterfaces.vb`, implementados por DOC-91/92/93.
- Esquema comprobado previamente mediante `information_schema`: catálogo central `gestor_modulos` y las cuatro tablas de usuario existen en el esquema configurado. Esta fase no ejecutó consultas reales.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | DOC-94 termina en un resultado interno de preautenticación; no crea challenge, envía correo, expone ASMX ni cambia UI. | Jira y ausencia de `Services/Login/SegundoFactor/` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Se conservan literalmente `InicializaconexionesModulos`, `Retorna_tipo_modulo` y `ValidaUserAplicacion`, incluidos estados distintos por módulo. | `Defaul/GestorModuleSesion.vb`; `Defaul/ClassGestorSesion.vb:2310,3012` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El bloque posterior a credenciales se extrae a un único finalizador sin redirect; el wrapper legacy redirige solo después de finalizar con éxito. | `Defaul/ClassGestorSesion.vb:2369-2777` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | `ILegacyLoginFinalizer` recibe `LegacyLoginFinalizationContext`, desacoplado del challenge; el contexto de challenge queda reservado al futuro OTP. | `SegundoFactorInterfaces.vb:59`; `SegundoFactorModels.vb:255,261` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La configuración se resuelve autoritativamente en el catálogo central mediante la conexión ODBC existente, con cardinalidad exacta y consultas parametrizadas. | `GestorModuleSesion.vb` usa `OdbcServicesGestor`; IDs de Session no son homogéneos | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Hay un adaptador MySQL por módulo para resolver ID, login canónico y correo desde las tablas y columnas verificadas. | `usuarios_da`, `remit_dest_interno`, `usuario_radicador`, `usuario_workflow` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Se crea `ContextoPreautenticacionModulo`, que permite ID 0 solo antes de resolver identidad; no se debilita `ContextoModulo.EsValido()`. | `Domain/Shared/ContextoModulo.vb` exige `IdUsuario > 0` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | `0`/`NULL` finaliza como hoy; `1` devuelve `SECOND_FACTOR_REQUIRED` sin efectos autenticados; valores inválidos fallan cerrados y sanitizados. | Columnas de `gestor_modulos`; límite en `ClassGestorSesion.vb:2367-2369` | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | La contraseña se descarta después de validarla; repositorios no reciben `HttpContext`/Session y recuperación/`ClassCorreo` no cambian. | Firma ByRef de `ValidaUserAplicacion`; límites del ticket | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Las pruebas usan dobles, cubren los cuatro módulos y demuestran equivalencia apagada y cero efectos activa; no usan infraestructura real sin autorización vigente. | AGENTS.md y runbooks del repositorio | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Se actualiza documentación técnica y su validación estructural con rutas y símbolos implementados. | Convenciones DOC-91/92/93 y `tools/validation/` | D-11 | RQ-11 | Origen: D-11, RQ-11 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | No existen nuevos endpoints, challenge, correo ni UI. | El diff de DOC-94 no toca ASMX, markup OTP ni SMTP. | Evita activar un flujo incompleto. |
| RQ-02 | Credenciales y estados conservan comportamiento. | Los cuatro módulos producen los mismos resultados de validación previos. | No agregar bloqueo a Radicación/DocuArchi. |
| RQ-03 | Existe un único finalizador reutilizable. | Login desactivado y futura continuación usan el mismo finalizador; solo el wrapper redirige. | Evita divergencia y doble auditoría. |
| RQ-04 | Finalizar no exige datos ficticios de challenge. | El contrato acepta únicamente contexto de identidad/módulo verificado. | Compatibilidad explícita con DOC-91. |
| RQ-05 | Configuración central inequívoca. | Cero o múltiples filas fallan cerradas; empresa/módulo/tipo no vienen del navegador. | Session contiene IDs con semánticas distintas. |
| RQ-06 | Identidad resuelta por módulo. | Cada consulta parametrizada devuelve exactamente una identidad o error controlado. | Variantes de mayúsculas del esquema MySQL. |
| RQ-07 | La conexión puede abrirse antes del ID. | Solo el contexto especializado acepta ID 0; el contexto base sigue rechazándolo. | No inventar IDs positivos. |
| RQ-08 | Decisión 2FA segura. | `0`/`NULL` finaliza; `1` se detiene; otro valor no autentica. | Filas activas bloquearán login hasta la fase OTP. |
| RQ-09 | No se filtran secretos ni lógica HTTP a infraestructura. | Password limpiado; interfaces/repositorios sin Session/HttpContext. | Protege límites existentes. |
| RQ-10 | Regresión demostrable. | Pruebas por módulo y ramas de error; MSBuild y suites DOC-91/92/93. | E2E/DB real requiere nueva autorización. |
| RQ-11 | Trazabilidad técnica actualizada. | Inventario, flujo, matriz, diagramas y manifiesto concuerdan con símbolos finales. | La validación estructural no prueba conducta completa. |

## Riesgo de despliegue

DOC-94 no implementa el paso OTP. Por tanto, solo puede desplegarse si los módulos afectados permanecen con `RequiereSegundoFactor = 0` o `NULL`. Confirmarlo exige un `SELECT` autorizado en el ambiente objetivo; no se presume en este refinamiento.

## Resultado del refinamiento

- Estado: aprobado arquitectónicamente.
- No se ejecutaron E2E, cargas ni consultas de base reales.
- Comando de sincronización: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-94 --sync`.
