# 01 — Alcance y diseño para ticket Jira

## ROL ESPERADO

Actúa como arquitecto senior de .NET Framework, VB.NET, ASP.NET Web Forms, MySQL y Workflow.

## OBJETIVO

Desde el ticket Jira inicial, consolidar la decisión técnica de **Devolver a actividad anterior** que habilita la etapa 02. Esta etapa no implementa código ni crea planificación paralela.

## CONTEXTO OBLIGATORIO

- Requiere ticket de inicio aprobado y lectura de `00-contexto-obligatorio.md`.
- Leer los dos documentos de `../Exploracion/`.
- Registrar en Jira si la salida habilita o bloquea 02.

## REQUISITOS POSITIVOS

- Precisar contratos objetivo de `PreviewDevolverActividad` y `EjecutarDevolverActividad`.
- Definir el modelo único de destino por conector entrante y las diferencias internas entre Ruta y Flujo.
- Definir búsqueda por actividad, usuario o grupo, longitud mínima, límite de página, cursor, orden estable, debounce y descarte de respuesta obsoleta.
- Definir auditoría, lock, token, códigos de bloqueo y el adaptador exclusivo hacia `Terminar_Tarea_Workflow`.
- Registrar exclusiones: Usuario anterior, otras operaciones modernas y todo tratamiento de respuestas.

## RESTRICCIONES CRÍTICAS

- No modificar código, configuración, endpoints, pruebas ni tickets Jira fuera de la evidencia autorizada.
- No usar `PreviewEnviarTarea` o `EjecutarEnvioTarea` como contrato de devolución sin una decisión explícita de compatibilidad.
- No diseñar postback ni ruta Web Forms alternativa.
- No incluir reglas, datos o componentes de respuestas.

## REGLAS DE ANTIRREGRESIÓN

- Preservar los contratos de Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior.
- Mantener el conector entrante como la única identidad de ejecución de esta capacidad.

## CRITERIOS DE ACEPTACIÓN

- La documentación identifica una operación de devolución separada y verificable.
- Define cómo Ruta y Flujo resuelven predecesores sin exponer datos ajenos.
- El ticket habilita 02 o queda bloqueado con causa y responsable.

## PRUEBAS OBLIGATORIAS

No ejecutar pruebas ni compilación por defecto: no hay código. Dejar la matriz de pruebas, compilación y QA reproducible para los tickets sucesores.

## DOCUMENTACIÓN TÉCNICA

Actualizar únicamente los documentos de exploración y la propuesta de arquitectura del paquete documental de DevolverTarea.

## ENTREGABLE FINAL

Reportar ticket, decisiones cerradas, bloqueos, archivos documentales actualizados y criterio exacto para iniciar 02.
