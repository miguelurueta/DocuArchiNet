# DOC-127 — Regresión de integración y aceptación pendiente

**141/141 pruebas locales PASS**, cero fallos. Se ejecutaron conjuntamente las11 suites existentes de navegación compartida DOC119, barra/árbol/coordinación de carpetas DOC120, editores de perfiles/fuentes/proveedores DOC110 y UI/cliente/adaptadores Excel DOC126–127. El corte anterior inmediato pasó140/140; la ampliación añade un caso que comprueba que la misma guarda de integración usada por la E2E detecta la reasociación accidental a `form1` y la desactivación global de validación. Los conteos se solapan con evidencias anteriores; no se suman.

El hallazgo de campos Excel que bloqueaban el gestor permanece corregido en17bdf09e. La comprobación de propietarios de controles se comparte ahora con la E2E de lectura existente, antes de abrir Excel y después de cerrarlo con Escape, sólo cuando el scope identifica aceptación DOC127. No envía un postback del gestor ni cambia la semántica de las ventanas históricas DOC126. El diagnóstico conserva conteos y condiciones de aislamiento, sin valores de campos.

El preflight actualizado ejecutó11 SELECT y cero escrituras: empresa2, DA11/RA12, cuentas141/194/196 activas con permisos esperados, dos destinos XLS nuevos y1.002/1.000 filas conservadas en los fixtures XLSX. Esto no sustituye las huellas completas de la futura ejecución. La consola propuesta se probó sin autorización: se detuvo con `DOC127_READ_NOT_AUTHORIZED` antes de conectar o pedir contraseñas.

Se preparó un scope nuevo de lectura con los hashes actuales y32 tablas protegidas, reutilizando el runner, resolver, controles y consola privada existentes. La [propuesta concreta](doc127-lectura-propuesta-20261125.md) identifica cuentas194/196, plantilla60, acciones y cero efectos funcionales. Se solicitó autorización expresa conforme a `AGENTS.md`; el estado de esta evidencia es **PENDIENTE_AUTORIZACION**, sin E2E autenticada ejecutada.

Avance **20/33**.7.1 registra el hallazgo local corregido y conserva pendiente el corte integral;5.1 conserva pendiente la aceptación real vigente. Las18 combinaciones de capacidad y EXPLAIN29 mantienen sus tareas propias. No se modificó producto ni se reaplicó SQL en esta ampliación. El JSON adjunto conserva comando, SHA base, hashes de fuentes/log y los límites de la comprobación.

Validación posterior: refinamiento OPSXJ15/15 y OpenSpec estricto PASS; discovery Playwright enumera13 escenarios de la suite existente sin ejecutarlos. `git diff --check` PASS.
