# Verificación transversal de Devolver → Usuario anterior

- Ticket: DOC-38
- Cambio OpenSpec: doc-38-verificacion-transversal
- Clasificacion: cross_cutting

## Objetivo

DOC-38 verifica de forma transversal la capacidad existente de Devolver → Usuario anterior antes de su liberación controlada. La etapa relaciona las decisiones de backend de DOC-36 y la interfaz moderna de DOC-37 con controles reproducibles de seguridad, aislamiento, accesibilidad y no regresión.

No crea una implementación nueva: reúne evidencia local y QA manual no autenticada autorizada, registra los límites de dicha evidencia y produce una recomendación para la etapa 05. Si un control crítico falla, la salida es una corrección con evidencia reproducible, no una liberación condicional.

## Alcance y compatibilidad

El alcance cubre `Services/Workflow/DevolverUsuarioAnterior/`, los endpoints ASMX asociados, `workflow/Webworkflow.aspx`, los módulos de `js/workflow/` y sus pruebas CJS/VB. También compara de forma focal las operaciones de devolver actividad anterior, continuar flujo, enviar a usuario y enviar a grupo.

Se preservan contratos, configuración, datos de negocio y el comportamiento de las operaciones vecinas. DOC-38 no ejecuta una tarea real, E2E autenticada, carga, despliegue ni cambio automático de ambiente. La reversa de una corrección futura se tratará en el ticket que la implemente; esta etapa solo registra el hallazgo y sus condiciones de reproducción.
