<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Context

DOC-91..97 entregaron Login 2FA sobre Web Forms/VB.NET 4.6.1, MySQL y configuración externa. El `.vbproj` registra sus fuentes y recursos. `Web.config` usa `AppSettings.local.config` y Session InProc de 10 minutos. DOC-92 determinó que no existe migración: se reutilizan las diez columnas y los índices actuales de `ra_auth_second_factor_challenge`.

El refinamiento de DOC-98 no ejecuta operaciones. Su implementación sí debe culminar en activación global controlada, después del piloto y únicamente con autorización nueva por ambiente, cuentas, ventana y datos.

## Goals / Non-Goals

**Goals**
- Convertir la implementación verificada en una secuencia operable, auditable y reversible.
- Impedir que un piloto cambie módulos, empresas o esquemas distintos del objetivo aprobado.
- Dejar 2FA activo transversalmente para todos los módulos objetivo, sin habilitación ni exclusión por usuario.
- Probar documentación y scripts sin infraestructura real.

**Non-Goals**
- Crear o modificar lógica de producto, contratos públicos, tablas, columnas, índices o schedulers.
- Ejecutar mutaciones sin autorización vigente o versionar secretos.
- Inventar estados físicos que la tabla no persiste.

## Decisions

### D-01 — Limitar DOC-98 a operación y validación
Los únicos propietarios nuevos serán documentación, SQL operativo bajo `DOC-98/Sql`, pruebas documentales y una puerta CI solo si falta. No se modificarán modelos, DTOs, servicios, repositorios, ASMX, UI, recuperación ni correo. El runbook verificará el registro único de binarios 2FA en el `.vbproj`.

### D-02 — No crear una migración inexistente
La “migración aditiva” se resuelve como comprobación del esquema ya instalado. DOC-92 declara que no hay apply/rollback DDL. El preflight comprobará por `information_schema` columnas e índices y será NO-GO si faltan; no intentará repararlos.

### D-03 — Mantener llaves fuera del repositorio
Se usarán `LoginSecondFactorHmacActiveKeyId` y `LoginSecondFactorHmacKey.<keyId>` en la fuente externa existente. El control confirma presencia, Base64 válido y mínimo 32 bytes sin imprimir valores. También verifica Session=10 minutos y exactamente una fila SMTP `ESTADO_ENVIO=1`.

### D-04 — Fijar una secuencia con abortos explícitos
Orden: autorización/respaldo; preflight; build/CI; esquema; binarios/configuración; smoke de cuatro módulos con 2FA `0`/`NULL`; diagnóstico agregado de identidades/correos; SMTP/llave/cuenta piloto; activación de una fila; E2E autorizado; observación; activación por oleadas de las filas restantes; verificación global. Una fase fallida bloquea las posteriores. El diagnóstico de principales informa riesgo operativo, pero no es un selector ni una dependencia de activación por usuario.

### D-05 — Activar únicamente una fila central y guardada
La activación apunta a `gestor_modulos` central con identificadores y valores esperados del operador. Exige snapshot, estado `0`/`NULL`, `EMAIL=1`, expiración `1..10`, cardinalidad uno y una fila afectada. La desactivación restaura `RequiereSegundoFactor=0`; nunca toca copias homónimas ni ofrece bypass.

### D-06 — Separar métricas persistidas de diagnósticos
SQL solo derivará pendiente, consumido, bloqueado por cinco intentos y expirado desde `Consumed`, `Attempts` y `ExpiresAtUtc`. Entrega/finalización fallida se observa en eventos sanitizados `correlation/stage/outcome`, porque no son estados físicos distintos. No se exponen OTP, MAC, llave, correo, login, identidad, conexión o excepción.

### D-07 — Rotar por identificador conservando verificabilidad
El formato `v1:keyId:mac` permite verificar con la llave original. La rotación instala la nueva, conserva la anterior, cambia el identificador activo y mantiene ambas durante 10 minutos más el margen registrado. La anterior se retira solo tras comprobar cero challenges elegibles que la referencien.

### D-08 — Limpiar solo terminales antiguos manualmente
No habrá scheduler. Primero se cuentan candidatos mayores de 30 días excluyendo filas elegibles. Toda limpieza requiere autorización específica, transacción, límite, revisión de cardinalidad y confirmación manual. DOC-98 no la ejecuta.

### D-09 — Revertir por capas sin borrar datos
Ante configuración inválida, pérdida de login legacy, OTP imposible o aumento de fallos, primero se desactiva solo el piloto. Los pendientes expiran naturalmente; no se borran ni reinterpretan. Luego se restaura el paquete aprobado. El rollback de emergencia no elimina esquema ni evidencia.

### D-10 — Culminar con activación global transversal por oleadas acotadas
Después de aprobar el piloto, la operación construirá un manifiesto exhaustivo de filas centrales para DocuArchi, Gestor, Radicación y Workflow en todas las empresas objetivo. Cada oleada usa IDs explícitos, snapshots y conteo esperado, se verifica antes de continuar y conserva rollback por fila. La aceptación final exige todas las filas del manifiesto en `RequiereSegundoFactor=1`, `SecondFactorProviderType=1` y expiración `1..10`, sin filas fuera del manifiesto modificadas. Esa configuración pertenece al módulo y se aplica a cualquier principal que intente autenticarse: no existen columnas, listas, banderas, excepciones ni bypass de activación por usuario. Las tablas de principales se consultan únicamente para resolver identidad y correo durante el login; un dato inválido falla cerrado y no altera el alcance transversal de la configuración.

## Risks / Trade-offs

- La tabla no diferencia todos los estados de dominio; se combinan agregados y diagnósticos sin sobreafirmar precisión.
- Session InProc implica que reciclados invalidan contextos pendientes y obligan a reiniciar login.
- La falta de transacción distribuida entre finalizador legacy y challenge sigue como riesgo residual DOC-97.
- Los scripts reducen error humano, pero no sustituyen autorización, respaldo ni responsable go/no-go.
- Usuarios con correo ausente/inválido o identidad ambigua no reciben excepción: su autenticación falla cerrada y el diagnóstico agregado permite remediar el dato sin introducir activación por usuario.

## Migration Plan

1. Preparar artefacto, configuración externa y paquete previo sin secretos.
2. Ejecutar preflight y build/CI; cualquier diferencia es NO-GO.
3. Desplegar con módulos OFF/NULL y completar smoke legacy.
4. Revisar el diagnóstico agregado de principales y activar una fila piloto autorizada, sin convertir ese diagnóstico en una lista de habilitación.
5. Ejecutar E2E y observar el piloto; ante umbral de aborto, aplicar D-09.
6. Activar las filas restantes por oleadas explícitas y verificar los cuatro módulos tras cada oleada.
7. Cerrar solo al demostrar configuración global activa, comportamiento transversal y ausencia de cambios fuera del manifiesto.

## Open Questions

- Cada solicitud operativa debe nombrar ambiente, empresa, módulo, cuenta piloto, aprobador, operador, observador, ventana, margen, umbrales y paquete de rollback; DOC-98 no asigna valores ni personas.
