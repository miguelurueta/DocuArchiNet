# Prompt 02 — Preparación, persistencia y reconciliación de anexos ENLASE

## Contexto de ejecución OPSXJ

Este prompt se ejecuta dentro de un ticket OPSXJ previamente creado y revisado. No ejecutar `opsxj:new`, no crear otro ticket, no reiniciar la orquestación y no crear cambios OpenSpec manuales. Continúa exclusivamente sobre el contexto, identificador y artefactos OPSXJ recibidos.

Verifica que el Prompt 01 haya publicado contratos y pruebas antes de implementar efectos.

## Rol esperado

Actúa como arquitecto backend senior especializado en consistencia, idempotencia, repositorios documentales y adaptación segura de funciones legacy.

## Contexto obligatorio

Lee completamente antes de investigar o modificar código:

- `../exploracion/modernizacion-importacion-anexos-sii-enlase.md` como base arquitectónica común.
- Prompt 01 y sus artefactos reales.
- Intenciones, preflight, orquestación y reconciliación modernas de `ImportarServicioWeb`.
- `workflow/ClassAlmacenamiento.vb`, especialmente `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`.
- `webservice/WebService_integracion_sii.asmx.vb`, especialmente `SeviceGuardaDocumentoAnexoSII`.
- Reglas de documentos relacionados y asignación en `workflow/Webworkflow.aspx.vb`.
- Documentación y pruebas de DOC-52, DOC-53, DOC-54, DOC-55, DOC-56, DOC-67, DOC-68, DOC-70, DOC-71, DOC-75, DOC-76, DOC-77 y DOC-78.

Aplica expresamente el flujo objetivo, las reglas críticas, los riesgos y la matriz de reutilización de la exploración. Documenta en OPSXJ qué componentes se reutilizan directamente, cuáles se extienden y cuáles quedan detrás de un adaptador.

Si la caracterización del almacenamiento, el contrato publicado por el Prompt 01 o evidencia verificable contradicen la exploración, detén el punto afectado, registra la desviación y actualiza la documentación mediante OPSXJ antes de cambiar el diseño. No resuelvas inconsistencias modificando silenciosamente funciones legacy o debilitando idempotencia y reconciliación.

## Objetivo

Preparar y ejecutar de forma idempotente la importación de uno o varios anexos SII hacia la tarea `ENLASE`, encapsulando la persistencia legacy, verificando el resultado físico y permitiendo reconciliación sin asignar automáticamente la tarea.

## Flujo obligatorio

```text
Selección normalizada
        |
        v
Preflight y tipologías válidas
        |
        v
Crear o reutilizar intención
        |
        v
Revalidar contexto ENLASE
        |
        v
Descargar y validar anexo
        |
        v
Adaptador de almacenamiento legacy
        |
        v
Verificar documento físico y relación
        |
        v
Persistir resultado y auditoría
        |
        v
Reconciliar respuesta por elemento
```

## Implementación obligatoria

- Publicar catálogo de tipologías permitido para la tarea y el checklist correspondiente.
- Permitir preparación individual y múltiple mediante el mismo contrato de colección.
- Aplicar tipología predeterminada únicamente cuando exista una coincidencia inequívoca y autorizada.
- Crear una intención para toda la selección, no una intención por anexo.
- Generar idempotencia con contexto inmutable, proveedor, capacidad e identidades externas.
- Revalidar usuario, tarea, actividad, ruta, trámite, proveedor y capacidad antes del primer efecto.
- Descargar cada recurso mediante el proveedor moderno y validar tamaño, formato y contenido.
- Encapsular la persistencia existente detrás de un puerto/adaptador moderno.
- Traducir resultados legacy a DTO estructurado dentro del backend; nunca en el navegador.
- Registrar la identidad externa asociada al documento almacenado.
- Verificar tanto registro lógico como existencia física antes de considerar un anexo importado.
- Si el registro existe pero el documento físico no, devolver un estado recuperable conforme a la política confirmada.
- Soportar éxito, omisión idempotente, fallo antes de persistir, resultado parcial y resultado incierto.
- Reconciliar por intención, tarea, proveedor, capacidad e identidad externa.
- Mantener disponible el identificador interno autorizado para actualizar la lista documental.

## Persistencia y compatibilidad

- Caracteriza primero las precondiciones, efectos y retornos de `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`.
- Reutiliza la función mediante adaptador si puede hacerse con contexto explícito y resultado verificable.
- No copies el cuerpo de la función ni crees una segunda ruta de almacenamiento.
- Cualquier extracción o modificación de lógica legacy requiere pruebas de caracterización y justificación documental.
- La asignación de la tarea permanece fuera del orquestador de importación.

## Restricciones críticas y antirregresión

- No guardar autoridad en `localStorage`, campos ocultos o datos libres del navegador.
- No usar una URL temporal como clave de idempotencia.
- No reintentar automáticamente un resultado incierto.
- No marcar éxito únicamente por recibir `YES`; verificar efectos autoritativos.
- No crear expedientes ni efectos propios de constancias cuando esta capacidad no los requiera.
- No cambiar el contrato o comportamiento de importación de constancias.
- No fragmentar la ejecución para simular progreso por elemento.
- No asignar la tarea después de importar.
- No anunciar éxito total si existe un elemento fallido, incierto o no procesado.
- No imprimir secretos, rutas físicas ni excepciones crudas.

## Matriz mínima de aceptación

| Caso | Resultado esperado |
|---|---|
| Un anexo válido | Documento confirmado una vez |
| Varios anexos | Una intención y resultados por elemento |
| Repetición de la misma intención | Sin duplicado |
| Dos solicitudes concurrentes | Una ejecución efectiva o resultado idempotente |
| Tipología ausente | Preflight bloqueado sin escritura |
| Tipología predeterminada ambigua | Selección manual obligatoria |
| Fallo de descarga | Fallo antes de persistir |
| Respuesta perdida tras almacenar | Resultado incierto y reconciliación |
| Registro y archivo físico existentes | Estado importado |
| Registro existente y archivo ausente | Estado recuperable según política documentada |
| Resultado parcial | Elementos confirmados y fallidos diferenciados |
| Finalización | Tarea todavía no asignada |

## Pruebas obligatorias

- Caracterización del almacenamiento legacy sin alterar datos reales.
- Unitarias de preflight, tipología, idempotencia y transiciones.
- Integración local de repositorios e intención.
- Concurrencia controlada.
- Resultado incierto y reconciliación.
- Existencia física y prevención de duplicados.
- Invariancia de la capacidad de constancias y del flujo legacy.

Las pruebas mutadoras reales requieren autorización expresa y datos descartables. Las consultas de verificación serán únicamente `SELECT`.

## Documentación técnica

Documenta exclusivamente en:

```text
Doc/Actualizacion/workflow/ImportarServiciWebEnlace/<TICKET>-preparacion-persistencia-reconciliacion/
```

Incluye flujo, máquina de estados, idempotencia, mapping legacy, funciones reutilizadas, persistencia, reconciliación, pruebas, rollback y metadata OPSXJ.

## Entregable final

Entrega implementación, pruebas y documentación con resultados por elemento verificables. No cierres OPSXJ si la persistencia no puede reconciliarse de forma autoritativa o si existe riesgo conocido de duplicidad sin control.
