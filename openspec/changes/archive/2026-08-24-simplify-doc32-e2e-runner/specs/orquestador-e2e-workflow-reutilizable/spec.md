## Purpose

Permite ejecutar de forma uniforme las E2E autorizadas de Workflow mediante perfiles reutilizables que no exponen secretos ni debilitan sus controles de seguridad.

## ADDED Requirements

### Requirement: Perfil reutilizable no sensible por DOC y ambiente

El sistema SHALL aceptar un perfil local seleccionable que identifique el DOC, el ambiente, los presupuestos, las tareas descartables, la actividad de ejecución y las consultas de control permitidas por ese DOC. El perfil SHALL validarse contra el contrato registrado del DOC, SHALL rechazarse antes de abrir una sesión si incluye valores o claves de contraseña, cookie, token, credencial, cadena de conexión o autorizaciones, y SHALL no copiarse al repositorio ni a la evidencia.

#### Scenario: Perfil válido para un DOC registrado

- **WHEN** se invoca una corrida con un perfil externo que contiene solo campos permitidos para un DOC y ambiente registrados
- **THEN** el orquestador reutiliza sus valores no sensibles sin solicitar nuevamente esos campos.

#### Scenario: Actividad de devolución fijada por perfil

- **WHEN** el preview de una tarea descartable ofrece más de una actividad anterior
- **THEN** el preview verifica primero que devuelve exactamente la lista no sensible configurada, la ejecución selecciona únicamente la actividad no sensible de esa lista, la carrera selecciona únicamente su actividad no sensible configurada desde su preview vigente, y ambas fallan antes del endpoint mutante si no obtienen una coincidencia única. Después de una respuesta exitosa de ejecución, comprueba mediante `SELECT` que la actividad activa final coincide con la actividad final no sensible configurada independientemente.

#### Scenario: Perfil con dato sensible o campo desconocido

- **WHEN** el perfil contiene una clave o un valor no permitido por el contrato del DOC
- **THEN** la corrida falla antes de abrir sesión, no imprime el valor rechazado y no ejecuta ninguna solicitud E2E.

### Requirement: Secuencia E2E consolidada y configurable

El sistema SHALL ofrecer una única invocación por DOC registrado que ejecute la secuencia configurada de validaciones no mutantes, preview, ejecución y concurrencia. SHALL capturar una sola vez los secretos efímeros requeridos por la secuencia en una consola interactiva y SHALL detener las etapas posteriores cuando una etapa requerida falle.

#### Scenario: Secuencia DOC-32 autorizada

- **WHEN** se inicia la secuencia DOC-32 con perfil válido, secretos efímeros y autorizaciones requeridas
- **THEN** el orquestador ejecuta primero preview, después una devolución sobre la primera tarea descartable y finalmente una carrera fija sobre una segunda tarea distinta.

#### Scenario: Falla de una etapa previa

- **WHEN** el preview o la ejecución previa falla o excede su presupuesto
- **THEN** el orquestador no inicia ninguna etapa mutante posterior y conserva únicamente la evidencia saneada de la etapa realizada.

### Requirement: Autorización explícita por operación mutante

El sistema SHALL exigir confirmación explícita e independiente para el ambiente y para cada operación mutante configurada por el DOC. Una operación de concurrencia SHALL requerir además una tarea descartable distinta y su propia confirmación; ninguna autorización se podrá obtener de un perfil persistente.

#### Scenario: Falta una confirmación requerida

- **WHEN** falta, se rechaza o no coincide una confirmación de ambiente, ejecución o concurrencia
- **THEN** la operación correspondiente no abre una sesión ni modifica una tarea.

#### Scenario: Confirmaciones completas

- **WHEN** las confirmaciones explícitas requeridas se reciben para tareas descartables autorizadas
- **THEN** el orquestador habilita solo las etapas confirmadas y conserva las restricciones de cantidad de solicitudes configuradas por el DOC.

### Requirement: Evidencia y cierre uniforme

El sistema SHALL aplicar al inicio y al cierre de toda secuencia los controles de gate y de rutas legacy definidos para Workflow. SHALL generar evidencia que contenga únicamente códigos, conteos, banderas, latencias y huellas, y SHALL eliminar del entorno de la corrida los secretos efímeros al finalizar.

#### Scenario: Secuencia completada o interrumpida

- **WHEN** una secuencia termina, falla o es interrumpida tras iniciar una etapa
- **THEN** se ejecutan los controles de cierre aplicables, el gate queda apagado con listas vacías y la evidencia no contiene datos sensibles ni cuerpos de respuesta.
