# DOC-111: corrección de la dependencia de Web.config

Solicitud del usuario: rechazar la propuesta de nuevas claves obligatorias y corregir la implementación. Esta evidencia sustituye la propuesta XML descartada y el requisito de habilitación mediante appSettings de los cortes anteriores.

## Comportamiento corregido

- Las reglas funcionales de extensiones y tamaño máximo por archivo siguen en la versión publicada existente. Se comprueban en preparación y recepción; no se cambia el perfil 24/versión 38 ni su huella.
- La composición ASMX/ASHX y las fases usan `LimitesImportacionCarpetas.DesdeContexto`. Esta consulta el `maxRequestLength` estándar de ASP.NET y el timeout de la sesión existente, sin claves `Doc111.*`.
- El manifiesto mantiene el techo de serialización de `PerfilImportacionJson` y la capacidad del transporte. La cantidad de elementos queda limitada por ese documento, sin un cupo funcional de lote nuevo. La suma de bytes conserva el control de overflow UInt64; su máximo representable no representa una cuota de negocio aprobada.
- La vigencia del plan deriva del timeout de sesión al prepararlo, conservando creación y vencimiento persistidos. Los límites inferiores específicos del perfil siguen aplicándose.
- Se retiran las claves de presupuestos PDF obligatorios. El staging productivo conserva las comprobaciones de PDF estructural, cifrado/reparación, longitud, hash y pertenencia. No se añaden cupos funcionales de páginas/objetos; se admiten los rangos representables del parser. Los consumidores internos que suministran presupuestos explícitos conservan sus verificaciones. No se afirma que el tamaño comprimido limite por sí solo el costo de procesar cualquier PDF.
- La consulta reutiliza el techo técnico de paginación de 200 ya existente. La limpieza conserva el puerto de política explícita: sin inyección de una política válida permanece deshabilitada, sin endpoint/job ni caducidad que elimine originales. No se bloquea importar por ausencia de política de limpieza.

No hay migración, nuevas tablas, reglas de perfil duplicadas o modificación de IIS/Web.config. Se conserva la función compleja de almacenamiento legacy y SII. No se habilitó un borrado ni se sustituyó la propuesta rechazada por valores fijos de dos carpetas/1 MiB/15 minutos.

## Verificación local

- Arnés .NET Framework y coherencia de DDL: 2/2 pruebas Node PASS; el ejecutable cubre las familias DOC-110/DOC-111. Casos nuevos: composición con sesión sin claves adicionales, capacidad de transporte menor, vigencia de sesión, rechazo del tamaño del perfil antes de leer el stream, PDF retenido con constructor productivo y rechazo de PDF cifrado sin presupuesto de appSettings.
- Cliente, Chromium con servicios sintéticos, contratos de carga individual y composición SII: 34/34 PASS.
- MSBuild Debug completo: cero errores, 308 advertencias existentes. Salida aislada `.tmp/doc111-sin-appsettings-build/bin/`; no se usa el build para modificar configuración.
- OpenSpec estricto válido; revisión de diff sin errores. Los once pendientes de aceptación/revisión final permanecen abiertos, 119/130.
- Los 27 archivos protegidos coinciden con HEAD al normalizar finales de línea. Web.config sin cambios.

Fuente: [manifiesto de 110 archivos](doc111-sha-sin-appsettings-20261030.json), SHA-256 agregado `6c617eb5adc0f7f3401c10a84a6b27175b126317db13f38fdc7d146c9e99c524`. HEAD `b2e93b726f221868cf2b82d43200a8e568871117` no identifica por sí solo el workspace sin commit. El manifiesto incluye los controles/diagnósticos E2E actualizados; no incluye secretos, perfiles runtime o contenidos de Web.config.

## Sitio de pruebas actualizado

Se aplicó la corrección a la DLL del mismo sitio IIS con despliegue previamente autorizado. Reemplazo atómico a las 21:51:18 UTC; copia previa conservada:

| Binario | SHA-256 |
| --- | --- |
| Activo corregido `bin/GestionDocumental-Docuarchi.net.dll` | `c85547f134de48f08eaccbba6e41ca477f8ff660a5f340af2dffa552dd8fcca9` |
| Anterior `.tmp/doc111-correction-deployment-20261030-215118/previous.dll` | `2c1d704312fd8a225c49469a0ed1141589bdef479819830dff462450d3ced959` |

Comprobación de arranque a las 21:51:42 UTC: raíz, `gestor.aspx` y página de Producción respondieron HTTP 200 con formulario, sin indicadores de error ASP.NET. El login conserva el campo de contraseña. Solo GET sin autenticar, sin cuerpos/cookies/credenciales en evidencia. Reporte saneado: `tools/e2e/artifacts/doc111-sin-appsettings-deployment.json`.

No se abrió otra consola ni se repitió la carga real. La aceptación HTTP autenticada y REC siguen pendientes: el arranque básico no se registra como PASS de importación.
