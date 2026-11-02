# DOC-111: revisión local y matriz de aceptación pendiente

Actualización vigente 2026-11-01: cierre explícito de reserva nunca iniciada y runner para continuar lote 2 implementados y desplegados; pruebas locales y salud PASS, ejecución autenticada pendiente. [Evidencia/SHA actuales](doc111-reserva-sin-inicio-20261101.md). Se mantienen **121/130** y los expedientes 648/649.

Estado vigente tras la corrida real: **121/130**. Se cierran solo autorización/recursos (13.10) y verificación SELECT del esquema (13.11). Plan 4/lote 2 recibió dos PDF y confirmó expedientes 648/649; almacenamiento detenido con intento 5 reservado. Las correcciones de metadatos del adaptador están desplegadas, pero falta recuperación del mismo lote y aceptación final. [Evidencia vigente y SHA](doc111-lote2-formato-nombre-20261030.md). Los cortes inferiores, incluido el listado original de once tareas abiertas, son históricos.

Fecha: 2026-10-30. Cambio: `doc-111-produccion-importacion-carga-carpeta`. Estado: **119/130**; esta revisión prepara 13.17/13.18, sin cerrar su aceptación final.

## Versión revisada

HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`. Hay cambios sin commit; HEAD por sí solo no identifica el código evaluado. El [manifiesto SHA](doc111-sha-local-20261030.json) identifica 107 archivos de implementación, contratos, proyecto y pruebas con huellas individuales. SHA-256 agregado: `da23dcb4eef55ad0a866c04f65f468d95248c278bd5e057f9e3a6fa5d6ec4758`.

Los 27 archivos protegidos registrados coinciden con HEAD al normalizar finales de línea. Incluyen almacenamiento legacy, creación/producción documental, Neodynamic, conexión y servicio web de importación existente. Se conserva la decisión del usuario de descartar el aislamiento adicional del XML frente a otras cargas.

| Binario observado | Bytes | SHA-256 |
| --- | ---: | --- |
| `bin/GestionDocumental-Docuarchi.net.dll` | 9602560 | `25beb85ef62ed7c03a7b8739a90771def1e0a0d20dc356dc57ead76a86ca266d` |
| `.tmp/doc111-limpieza-build/bin/GestionDocumental-Docuarchi.net.dll` | 9620992 | `2c1d704312fd8a225c49469a0ed1141589bdef479819830dff462450d3ced959` |

La diferencia no demuestra por sí sola qué código ejecuta IIS. La correspondencia del despliegue queda pendiente. No se reemplazó la DLL del sitio. La compilación aislada y sus límites están documentados en [limpieza](doc111-limpieza-local-20261030.md).

## Comprobaciones de este corte

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: **2/2 PASS**. Incluye compilación/ejecución del arnés VB en .NET Framework, regresión DOC-110 y coherencia de columnas UPDATE con DDL. Sin BD ni HTTP real.
- Conjunto Node de cliente DOC-111, drivers REC, sesión compartida, composición del servicio y regresiones DOC-88: **64/64 PASS**, cero omitidos. Archivos: `doc111-importacion-carpetas-driver.test.cjs`, `authenticated-workflow-session.test.cjs`, `authenticated-workflow-session-usage.test.cjs`, `doc111-importacion-carpetas-client.test.cjs`, `doc88-produccion-upload-contract.test.cjs`, `doc88-produccion-carga-almacenamiento.test.cjs`, `importar-servicio-web-production-composition.test.cjs` y `doc88-production-document-upload-policy.test.cjs` en sus directorios existentes. Chromium usa servicios sintéticos; estos resultados no son aceptación autenticada.

## Matriz de capacidades

| Capacidad | Evidencia disponible | Aceptación pendiente |
| --- | --- | --- |
| Autoridad operativa y separación de administración DOC-110 | Arnés Framework; evidencia histórica de SELECT y lectura operativa en el expediente técnico | Revocación en sesión HTTP y regresión administrativa real (5.9/6.6) |
| Manifiesto, preflight, plan, confirmación y staging | Pruebas locales; persistencia/rollback reales históricos limitados al repositorio | REC con recepción real y versión desplegada identificada |
| Creación, materialización de valores e índices efectivos | [Valores diferidos](doc111-valores-diferidos-local-20261030.md), Framework | EXPEDIENTE/LOTE/PARCIAL reales |
| Adaptador y fase documental | [Adaptador](doc111-adaptador-documento-local-20261030.md), [fase](doc111-fase-documento-local-20261030.md); copia consumible, nombre original y conservación del temporal | Almacenamiento real y controles SELECT de IDs, relaciones, nombres e índices |
| Consolidación, incertidumbre y reconciliación | [Consolidación](doc111-consolidacion-local-20261030.md), [reconciliación](doc111-reconciliacion-local-20261030.md) | Recuperación/repetición real sin duplicación |
| Fachada HTTP y cliente/rama | [HTTP](doc111-http-fases-local-20261030.md), [cliente](doc111-cliente-rama-local-20261030.md); pruebas sintéticas | Sesión autenticada y recursos reales autorizados |
| Drivers de aceptación | [Drivers REC](doc111-rec-drivers-local-20261030.md); 18 casos unitarios dentro de los 64 | Los tres REC reales están **NO_EJECUTADO** |
| Limpieza conservadora del original | [Limpieza](doc111-limpieza-local-20261030.md); arnés local y compilación aislada | Sin ejecución real; deshabilitada por defecto |
| Carga individual/SII | Regresiones locales de contratos y composición | Regresión real de 13.16; no se acredita ejecución SII |

Las evidencias reales históricas del expediente técnico conservan su alcance y autorización originales. No acreditan una nueva aceptación DOC-111 ni autorizan nuevas escrituras.

## Once tareas abiertas

5.9 y 6.6: revocación operativa y regresión administrativa en sesión real. 13.10: ambiente/cuenta/versiones y datos descartables. 13.11: esquema del destino autorizado mediante SELECT. 13.12/13.13/13.14: REC EXPEDIENTE/LOTE/PARCIAL. 13.15: degradación por esquema ausente/incompatible. 13.16: carga individual/SII. 13.17: matriz final con aceptación. 13.18: revisión final OpenSpec/gobierno/contratos.

La búsqueda posterior solicitada por el usuario localizó el ambiente histórico del visor: **`https://localhost/GestionDocumental-Docuarchi.net/`**, en `tools/e2e/profiles/workflow-pdf-viewer-performance-220604.runtime.json` y `tools/e2e/profiles/doc103-graph-save-220604.runtime.json`. Los perfiles se inspeccionaron mediante campos permitidos, sin imprimir consultas ni secretos. La [evidencia DOC-103](../../../VisorPdf/Implementacion/DOC-103/Evidencia.md) registra una E2E real aprobada el 2026-10-14; los [resultados DOC-105](../../../VisorPdf/Implementacion/DOC-105/resultados-rendimiento.md) documentan corridas posteriores del visor. Esto identifica la configuración histórica; no comprueba disponibilidad actual ni aceptación DOC-111.

También existe la variante HTTP en perfiles de carga individual, pero no se asume equivalente a HTTPS. Solo existe un perfil DOC-111 de ejemplo: sus IDs y URL no son recursos de aceptación. Falta asignar cuenta, nivel, versiones publicadas y datos descartables. No se solicitan contraseñas en documentación o chat. La información de ambiente no constituye autorización para efectos reales.

Antes de cualquier corrida real se requiere autorización explícita de ambiente, cuentas y alcance, conforme a `AGENTS.md` y los runbooks aplicables. Una alteración real del esquema tampoco queda autorizada por esta revisión. No hubo SQL, carga, limpieza real ni despliegue en este corte.

## Disponibilidad comprobada del ambiente

Por solicitud posterior del usuario se verificaron IIS y respuestas GET sin autenticar: raíz, login y página de Producción responden HTTP 200. IIS apunta a este workspace. Certificado autofirmado, contemplado en los perfiles históricos. Véanse [resultados y límites](doc111-ambiente-disponibilidad-20261030.md). No se verificó aceptación DOC-111 ni se desplegó código.

## Permisos comprobados por SELECT

La inspección inicial de la cuenta histórica 141/empresa 2 confirmó identidad activa, administración concedida e importación denegada. Después, por instrucción explícita «cambia permisos», se habilitó únicamente `IMPORTAR_CARPETAS` (0→1), con una fila afectada y los demás campos conservados. Una nueva conexión SELECT confirmó **importación concedida**. Hay 91 niveles que cumplen el predicado inicial de derechos nativos; el perfil histórico 24 sigue sin cumplir la condición conjunta de disponibilidad/publicación. No se activaron perfiles. Véanse [alcance y resultados](doc111-permisos-readonly-20261030.md). Esto no acredita revocación HTTP ni aceptación REC.

## Activación posterior autorizada

Por instrucción «habilita el perfil 24», el repositorio existente habilitó ese perfil de la empresa 2, revisión 11→12, conservando referencia publicada 38, contenido y reglas. Una conexión SELECT nueva confirmó importación concedida y disponibilidad/publicación del perfil. [Evidencia y secuencia](doc111-permisos-readonly-20261030.md). La habilitación no acredita aceptación REC ni autoriza cargas por sí sola.

## Preparación posterior de LOTE

Se verificaron por SELECT los esquemas de los repositorios, catálogos operativos, gabinete y nivel histórico 7 con sus ancestros. Se preparó el perfil runtime LOTE de la versión 38 y `--check` pasó sin efectos. La inspección de metadatos confirmó tipos ausentes en la DLL del sitio y presentes en la compilación aislada. [Recursos propuestos, evidencia y condiciones de ejecución](doc111-preparacion-lote-20261030.md). No hubo despliegue ni carga real.

## Despliegue posterior autorizado

El usuario autorizó reemplazar la DLL de IIS. Se desplegó el binario aislado validado, se conservó la copia anterior y se comprobaron sus SHA-256. Inicio, login y Producción respondieron HTTP 200 sin indicadores de error ASP.NET. [Evidencia de despliegue y reversión](doc111-despliegue-local-20261030.md). Queda resuelta la diferencia de binario observada en los cortes anteriores; no se ejecutó aceptación autenticada ni carga real.

## Corrección de criterio pendiente

El usuario rechazó la propuesta de nuevas claves obligatorias en Web.config para habilitar DOC-111. La propuesta quedó marcada como descartada; no se aplicó y no se reintentó la carga. El perfil/editor existentes ya resuelven extensiones y límite por archivo. La ubicación de otras políticas y la eliminación de la dependencia actual requieren corregir el diseño; no se considera resuelto el bloqueo por proponer configuración. [Decisión y diagnóstico histórico](doc111-fallo-operativo-configuracion-20261030.md).

## Corrección implementada y aplicada al sitio de pruebas

Se eliminó la dependencia de claves Doc111 en appSettings, reutilizando perfil publicado, transporte/JSON y sesión existentes. Limpieza deshabilitada sin política interna explícita. Arnés Framework, 34 regresiones y build completo PASS; DLL corregida desplegada en el mismo sitio autorizado y arranque básico HTTP 200. Web.config y funciones protegidas intactos. [Evidencia vigente y SHA](doc111-correccion-sin-appsettings-20261030.md). No se repitió la aceptación autenticada; los 119/130 no aumentan por esta corrección.

## Corrección de confirmación tras plan real 3

La sesión 09 creó el plan 3 pero falló al confirmar por una comparación sensible al orden físico de las reglas. Se corrigió y probó la equivalencia canónica con rechazo de cambios reales; se desplegó en el sitio autorizado y pasó el arranque. El plan 3 venció sin lote y se conserva intacto. La sesión 10 prepara una operación nueva, pendiente de resultado real. [Evidencia vigente](doc111-confirmacion-orden-reglas-20261030.md). No aumenta el progreso ni acredita REC.

## Gobierno local

La auditoría usa las funciones locales de opsxj, sin Jira/GitHub ni variables que sustituyan la revisión. Se registra únicamente evidencia `unit=pass`, asociada al HEAD y a este manifiesto del workspace. Refinamiento: PASS. Gobierno: pendiente por tareas abiertas, revisión final y QA manual; no se registra PASS de QA/E2E. Resultado detallado: [auditoría local](doc111-gobierno-local-20261030.json).
