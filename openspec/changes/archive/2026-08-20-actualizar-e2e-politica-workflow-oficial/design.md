## Context

La suite DOC-10 separa la llamada anónima, el bootstrap autenticado y la comprobación completa con huellas. El caso completo y su documentación todavía exigen un segundo usuario para afirmar que el gate lo bloquea con `WORKFLOW_MODERN_INACTIVE`; esa condición contradice la política oficial actual. Véase `proposal.md` para la motivación.

## Goals / Non-Goals

**Goals:**

- Verificar que una sesión Gestión con contexto Workflow válido recibe una respuesta funcional o el bloqueo de negocio explícitamente esperado, nunca un bloqueo de gate.
- Mantener las huellas antes/después de estado y auditoría mediante las mismas consultas `SELECT` parametrizadas y la cuenta MySQL de solo lectura.
- Ofrecer una comprobación opcional de una segunda cuenta válida que pruebe la política oficial sin usar el concepto de piloto.
- Reutilizar `authenticated-workflow-session.cjs` para todas las sesiones autenticadas y documentar las variables requeridas sin secretos.

**Non-Goals:**

- No modificar endpoints, Web Forms, valores del gate, usuarios/grupos configurados ni permisos de negocio.
- No ejecutar `EjecutarEnvioTarea`, carga, consultas no `SELECT`, ni crear otro mecanismo de login.
- No reescribir evidencia histórica de corridas realizadas bajo el gate retirado.

## Decisions

### Separar contexto válido de disponibilidad de negocio

El caso completo solo usará la cuenta principal y exigirá que el preview no devuelva `WORKFLOW_CONTEXT_INVALID` ni `WORKFLOW_MODERN_INACTIVE`. Si la tarea tiene un bloqueo funcional conocido, seguirá usando `DOC10_E2E_AUTHORIZED_EXPECTED_CODE`; de lo contrario exigirá destinos. Así una tarea no asignada, como `WORKFLOW_TASK_UNAVAILABLE`, no se interpreta como un gate.

Se descarta conservar el caso piloto/no piloto: afirma una política eliminada y requiere alterar configuración para ejecutarse.

### Renombrar la validación de dos cuentas a contextos oficiales

El antiguo escenario `authorization` se reemplazará por `contexts`. Recibirá una cuenta primaria y una secundaria, ambas de contexto Workflow válido, y comprobará que ninguna recibe los códigos de contexto/gate retirado. La disponibilidad de la tarea queda como resultado de negocio resumido, no como permiso de piloto.

Se descarta reutilizar el nombre `authorization`: induciría a que operadores vuelvan a activar el gate para ejecutar la prueba.

### Mantener la seguridad del arnés

Las variables de conexión y contraseña existen solamente durante el proceso de prueba. El arnés conserva su helper autenticado, cierra todos los contextos y solo persiste evidencia resumida sin secretos ni cuerpos. El runbook indicará que el gate se verifica apagado al cierre, pero nunca se cambia para una corrida.

## Risks / Trade-offs

- [Una cuenta válida no tiene acceso a la tarea] → Informar el código funcional y usar una tarea asignada o el código esperado para el caso funcional; no clasificarlo como gate.
- [Automatizaciones existentes invocan `test:authorization`] → El comando se retirará y la documentación expondrá `test:contexts` como reemplazo explícito.
- [Un cambio accidental reintroduce un bloqueo por configuración] → Las aserciones de contexto validan que ambos usuarios no reciben el código de gate y las pruebas estáticas buscan las referencias retiradas.

## Migration Plan

1. Actualizar los modos de configuración, pruebas Playwright y pruebas unitarias del arnés.
2. Actualizar README y runbook sin editar el gate ni crear archivos de secretos.
3. Ejecutar validaciones estáticas y las corridas anónima, autenticada, de contextos y completa autorizadas; comparar huellas antes/después.
4. Si ocurre una regresión, revertir solamente el cambio del arnés; los valores del gate permanecen apagados durante todo el proceso.
