# 08 — Despliegue, activación gradual y rollback

## ROL ESPERADO

Actúa como arquitecto de solución y responsable de release.

## OBJETIVO

Preparar, en el OpenSpec independiente de esta Jira, una entrega operable y reversible del Login 2FA ya verificado. Esta tarea no autoriza desplegar ni activar producción.

## CONTEXTO

Lee la exploración, migraciones, configuración, CI, evidencia transversal y convenciones de despliegue existentes antes de redactar o modificar scripts.

## PRECONDICIONES DE RUTAS

Esta Jira no crea modelos, DTOs, servicios, repositorios ni ASMX. El `design.md` debe confirmar que los binarios ya entregados están registrados en `GestionDocumental-Docuarchi.net.vbproj`. Sus cambios se limitan a:

- `Doc/Actualizacion/Login/Implementacion/<JIRA>/` para runbook, matriz go/no-go y evidencia;
- `Doc/Actualizacion/Login/Implementacion/<JIRA>/Sql/` para preflight/apply/rollback operativos que pertenezcan a esta Jira;
- transformaciones/configuración ya usadas por el proyecto, solo si el ambiente las versiona y sin valores secretos;
- `.github/workflows/opsxj-validation.yml` únicamente si falta una puerta automatizable demostrada.

No duplicar SQL de la tarea 02, no crear scripts sueltos en la raíz y no modificar rutas de código para acomodar el despliegue.

## REQUISITOS POSITIVOS

1. Consolidar orden exacto: respaldo/verificación, migración aditiva, despliegue binario/config, inyección de llaves, smoke técnico con 2FA apagado, activación controlada y observación.
2. Crear runbook parametrizado por ambiente sin valores secretos ni credenciales de ejemplo reales.
3. Definir prechecks solo lectura para esquema, índices, una sola fila SMTP `ESTADO_ENVIO = 1`, llave activa/disponibilidad de llaves anteriores, valores 2FA y timeout Session.
4. Definir activación por fila central de `gestor_modulos`, primero una empresa/módulo/cuenta autorizada; `0`/`NULL` es el estado compatible inicial. No modificar copias homónimas de otros esquemas.
5. Definir observabilidad sanitizada: challenges por estado, entregas fallidas, bloqueos, expiraciones y finalizaciones fallidas; nunca OTP, hash, llave, correo, login completo o conexión.
6. Definir rotación de llave con convivencia durante máximo 10 minutos más margen operativo y retiro posterior comprobado.
7. Definir limpieza autorizada de estados terminales mayores de 30 días; no instalar un scheduler inexistente.
8. Definir rollback por capa: desactivar 2FA, rollback binario compatible, tratamiento de challenges pendientes y reversión DDL solo si es segura. No borrar columnas/datos en un rollback de emergencia.

## RESTRICCIONES CRITICAS

- No desplegar, migrar, activar, rotar llaves, limpiar datos ni enviar correo sin autorización explícita de ambiente y operación.
- No versionar secretos, incluir comandos destructivos automáticos, borrar columnas en rollback ni habilitar bypass.
- No modificar comportamiento de Login, recuperación o correo en esta tarea salvo un defecto documental/script comprobado.

## PUERTAS GO/NO-GO

- Build/CI y matriz transversal en verde o bloqueos aceptados explícitamente.
- Migración verificada en ambiente no productivo y reaplicación idempotente.
- Login de los cuatro módulos comprobado con 2FA apagado.
- SMTP, llave y correo de cuentas piloto validados sin exposición.
- E2E autorizado del piloto exitoso antes de ampliar.
- Métricas, responsable de decisión y ventana de reversión definidos.

Ante error de configuración, aumento de `DELIVERY_FAILED`/`FINALIZATION_FAILED`, pérdida de login legacy o imposibilidad de verificar OTP, el runbook debe indicar desactivar la fila 2FA afectada y preservar evidencia; nunca habilitar un bypass.

## PRUEBAS OBLIGATORIAS

Valida sintaxis/completitud de scripts y runbooks, ensaya en infraestructura descartable autorizada y ejecuta smoke sin mutación no autorizada. Registra comandos y salidas saneadas. Si no hay ambiente o autorización, entrega el runbook como no ejecutado y enumera exactamente la puerta pendiente.

Incluye evidencia unitaria/focal reproducible para validadores y scripts; un ensayo operativo/E2E real requiere el runbook, autorización vigente, infraestructura existente, secretos efímeros, controles `SELECT` y evidencia sanitizada. Sin ambiente autorizado, registra bloqueo y no afirma ejecución.

## CRITERIOS DE ACEPTACION

- El despliegue es secuencial, verificable y reversible sin depender de conocimiento tácito.
- La activación no cambia módulos distintos al piloto.
- Desactivar 2FA restaura el recorrido legacy sin quitar la infraestructura instalada.
- Ningún secreto está versionado y ninguna activación real ocurrió por efecto de esta tarea.
- El OpenSpec traza requisitos/decisiones a scripts, gates y evidencias.

## DOCUMENTACION TECNICA

Actualiza documentación operativa existente y guarda runbook, matriz go/no-go y evidencia bajo `Doc/Actualizacion/Login/Implementacion/<JIRA>/`.

## ENTREGABLE FINAL

Entrega runbook, scripts no destructivos, pruebas con rutas/casos, comandos, códigos de salida, responsables/gates, riesgos y bloqueos; declara expresamente que no se activó producción.
