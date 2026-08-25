<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05 -->
## Contexto

DOC-35 es la etapa 04 posterior a DOC-32, DOC-33 y DOC-34. DOC-34 registró compilación correcta, 83 pruebas CJS focales correctas, QA no autenticada y evidencia saneada de E2E autorizadas anteriores. El merge del PR #29 en main es la versión de referencia para una solicitud operativa futura.

Esta etapa prepara documentos; no edita configuración, no despliega, no ejecuta E2E o carga y no usa ni registra secretos.

## Objetivos y exclusiones

Los objetivos son emitir una decisión de liberación única, dejar una matriz de ambientes verificable y describir el procedimiento que un operador podrá seguir solamente después de una autorización explícita.

Quedan fuera de alcance la ejecución de despliegues, consultas de ambiente sin autorización, cambios de gate, reversión de tareas confirmadas, postbacks Web Forms y cualquier cambio de contrato de Devolver, Continuar flujo, Enviar a usuario, Enviar a grupo o Usuario anterior.

## Decisiones

### D-01 — Base técnica y decisión vigente

La base técnica es DOC-34 y la versión de referencia es main en el merge del PR #29. Como no hay ambiente, ventana ni responsables autorizados, la decisión vigente es solicitar aprobación operativa; la evidencia técnica no equivale a permiso de operación.

### D-02 — Autorización aislada por ambiente

La matriz inicia con cero ambientes elegibles. Cada solicitud futura debe identificar ambiente exacto, versión, alcance, ventana, aprobador, operador de despliegue, dueño funcional, evidencia y acción de continuación. La autorización no se transfiere entre ambientes, versiones ni ventanas.

### D-03 — Controles y operación mínima

Después de autorización explícita, el operador solo puede realizar comprobaciones documentales y consultas SELECT parametrizadas y saneadas. El runbook prohíbe E2E, carga, cambios de configuración, cambios de gate y registro de credenciales, cookies, cadenas de conexión o datos de tarea.

### D-04 — Reversión por paquete e invariantes

La reversión se ejecuta exclusivamente mediante la gestión de despliegue aprobada y afecta intentos nuevos. No revierte tareas, auditoría ni transiciones ya confirmadas, no reactiva postbacks ni una ruta UI alternativa y conserva la ruta moderna oficial, los conectores entrantes Ruta/Flujo, el lock por tarea y el aislamiento de respuestas.

### D-05 — Registro de resultado saneado

El resultado operativo registra solamente decisión, ambiente, versión y referencias saneadas. Se aborta antes de desplegar ante autorización incompleta, diferencia de versión o contrato, controles no conformes o retiro de aprobación.

## Riesgos y compatibilidad

El riesgo principal es interpretar la evidencia previa como autorización de ambiente. La matriz vacía y el criterio de aborto lo evitan. Persisten la advertencia heredada MSB3247 y la cobertura visual anónima limitada; ninguno bloquea la solicitud de aprobación, pero ambos deben acompañar la evidencia del ambiente.

## Plan de operación

1. Recibir una solicitud que complete todos los campos de la matriz para un ambiente y una versión concretos.
2. Confirmar evidencia, alcance e invariantes mediante controles autorizados de solo lectura.
3. Continuar, abortar o revertir únicamente por la gestión de despliegue aprobada y registrar el resultado saneado.
