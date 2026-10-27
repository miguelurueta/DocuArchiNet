# DOC-110 — Asignación de permisos en aplicación externa

El usuario confirmó el 2026-10-20 que los permisos de Producción se administran en otra aplicación, fuera de este repositorio. Se cierra la investigación de su ubicación dentro de DocuArchiNet. No se conocen nombre, repositorio, pantalla o API de esa aplicación; no se inventan ni se afirma que ya soporte el campo nuevo.

## Límite de responsabilidades

Actualización explícita del usuario: la aplicación externa no está configurada y no pertenece a este proyecto. Autoriza retirar temporalmente el permiso de la cuenta de prueba y hacer la comprobación directamente, con restauración. Para esta aceptación del efecto de revocación se admite el ensayo controlado 1→0→1 en RA, limitado al campo ADMINISTRAR_PERFILES_IMPORTACION de LUZ.AGUILERA; no se requiere implementar/inventar integración externa. Esto no acredita ni entrega la aplicación administradora. Preparación y recuperación: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-revocacion-permiso-preparacion.md`.

| Entrega | Responsabilidad |
|---|---|
| DOC-110 en este repo | Script manual del campo administrativo, inicialización/carga compatible, consulta vigente, autorización por ámbito/recurso y controles de la interfaz del perfil. |
| Aplicación administradora externa | Exponer asignación/revocación a administradores autorizados, usando su mecanismo y auditoría existentes. Conservar permisos ajenos. |
| Equipo de despliegue | Aplicar SQL revisado y coordinar versión compatible de ambas aplicaciones. Confirmar concesión/revocación en ambiente autorizado antes de habilitación operativa. |

No crear una pantalla o endpoint de concesión en el editor del perfil. No implementar un nuevo servicio entre aplicaciones sin evidencia de que sea necesario. El contrato describe el efecto esperado sobre el permiso existente; el mecanismo externo concreto lo determina su propietario.

## Contrato del dato

- Tabla destino: `remit_dest_interno_perfil_produccion` en el origen documental autorizado.
- Usuario: referencia `remit_dest_interno_idremit_dest_interno`; no confundir identificador de cuenta de otra aplicación con usuario de Gestión.
- Campo nuevo DOC-110: `ADMINISTRAR_PERFILES_IMPORTACION`, propuesto como TINYINT UNSIGNED NOT NULL DEFAULT 0 en SQL 01.
- Asignación explícita: 1. Revocación: 0. No herencia automática desde permisos generales.
- Solo un registro inequívoco del usuario puede alimentar la autorización. El índice no único observado exige detectar ambigüedad; no actualizar indiscriminadamente filas duplicadas ni crear otra fila para eludirla.
- La clave `GA_ADMINISTRAR_PERFILES_IMPORTACION` es copia de sesión de DocuArchiNet, no un campo que la aplicación externa deba escribir directamente.
- La siguiente solicitud protegida debe observar revocación confirmada; la sesión antigua no mantiene el privilegio. No promete cancelar una operación ya iniciada.
- `IMPORTAR_CARPETAS` pertenece a otra entrega; este contrato no solicita habilitarlo ni incluirlo en SQL 01.

## Aceptación conjunta antes de habilitar operación

1. Campo aplicado con default 0; cuentas previas no quedan habilitadas automáticamente.
2. Administrador autorizado concede a una cuenta de prueba inequívoca mediante la aplicación externa; para la prueba solicitada, se admite el cambio directo puntual en RA expresamente autorizado por el usuario, con restauración y conservación del resto de la fila.
3. DocuArchiNet permite solo las operaciones administrativas autorizadas de su ámbito.
4. La aplicación externa o el mecanismo puntual aprobado revoca y la siguiente solicitud protegida en DocuArchiNet se deniega.
5. Otras capacidades y carga individual conservan comportamiento; no se cambia SII.
6. Registrar evidencia saneada y referencia real de entrega externa cuando se disponga de ella.

Estado actual: efecto de revocación/restauración aceptado por mecanismo directo autorizado, **1 passed (22.7 s)**; cuenta en su estado original y otros campos intactos. Evidencia: `Validacion/doc110-revocacion-permiso-preparacion.md`. No se afirma que la aplicación externa esté configurada o integrada; queda fuera del proyecto conforme a la indicación del usuario. La regresión funcional de carga individual mantiene su aceptación propia.
