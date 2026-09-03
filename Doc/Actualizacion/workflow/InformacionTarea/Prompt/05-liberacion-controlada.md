# 05 — Liberación y operación controlada

## Rol esperado

Responsable técnico de liberación con foco en privacidad, observabilidad y reversibilidad.

## Objetivo

Preparar matriz de ambientes, decisión y runbook. Esta etapa no despliega, configura ni ejecuta pruebas reales.

## Contexto obligatorio

- Requiere 04 aprobado y sin defectos críticos.
- Leer `00-contexto-obligatorio.md`, evidencia final, versión candidata, inventario y mecanismos existentes de despliegue y rollback.
- La aprobación técnica no constituye autorización operativa.

## Requisitos positivos

- Identificar versión, artefactos, dependencias, responsables, ventana y evidencia.
- Definir verificaciones previas/posteriores únicamente de lectura y con resultados saneados.
- Confirmar autorización, catálogo publicable, enmascaramiento, UI moderna y ausencia del recorrido legacy.
- Definir monitoreo de códigos funcionales sin registrar datos personales o valores de campos.
- Definir rollback por artefactos para nuevos intentos, sin modificar tareas o datos históricos.
- Registrar por ambiente: bloquear, solicitar autorización o listo para despliegue autorizado.

## Restricciones críticas

- No desplegar, editar configuración, gates, usuarios o grupos.
- No ejecutar pruebas autenticadas, carga ni consultas sobre información real.
- No inferir autorización entre ambientes.
- No reactivar `S-DTS`, `SELECT *`, controles ocultos o postbacks como contingencia.

## Reglas de antirregresión

Preservar operaciones vecinas y datos existentes. El rollback no cambia tareas ni información y no debe restablecer una ruta insegura de forma parcial.

## Criterios de aceptación

- Matriz, responsables, observabilidad y rollback son ejecutables y no contienen secretos.
- La versión candidata coincide exactamente con la verificada en 04.
- La decisión por ambiente es explícita.

## Pruebas obligatorias

No ejecutar pruebas reales. Verificar documentalmente que el build MSBuild, pruebas unitarias/focales y recorridos autorizados de 04 corresponden a la versión candidata. Registrar comandos, resultados, referencias y ausencias como bloqueo.

## Documentación técnica

Actualizar el paquete documental canónico con matriz, runbook, privacidad, monitoreo, responsables, riesgos y rollback; registrar cualquier ruta faltante y no crear documentos en la raíz.

## Entregable final

Entregar documentación, diagramas y evidencia coherente. Confirmar que no se desplegó, modificó configuración ni ejecutó pruebas reales.

